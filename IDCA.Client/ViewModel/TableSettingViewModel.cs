
using IDCA.Model;
using IDCA.Model.Spec;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{

    /// <summary>
    /// TableSettingView控件使用的View Model对象，应该在TableSettingWindowViewModel类的
    /// 下一级。
    /// </summary>
    public class TableSettingViewModel : ObservableObject
    {

        public TableSettingViewModel(Tables tables)
        {
            _elementList = new ObservableCollection<TableSettingElementViewModel>();
            _tableName = tables.Name;
            _tables = tables;
            _headers = new ObservableCollection<CheckableItemViewModel>();
            _checkedHeaders = string.Empty;
        }

        readonly Tables _tables;

        TableSettingTreeNode? _node;
        /// <summary>
        /// 当前ViewModel对应的配置节点，对应界面中TreeView中对应的对象
        /// </summary>
        public TableSettingTreeNode? Node
        {
            get { return _node; }
            set { SetProperty(ref _node, value); }
        }

        Action<string, string>? _renamed;
        /// <summary>
        /// 当此对象Name属性更改时触发的事件，主要用于更新TreeView中显示的名称
        /// </summary>
        public event Action<string, string>? Renamed
        {
            add { _renamed += value; }
            remove { _renamed -= value; }
        }

        Func<string, bool>? _beforeRemoved;
        /// <summary>
        /// 当此对象的Name属性变更时，如果此事件不是null，优先判定此事件的结果，
        /// 如果返回true才会修改Name属性数据，如果此事件时null，则始终可以修改Name
        /// 属性数据。
        /// </summary>
        public event Func<string, bool>? BeforeRemoved
        {
            add { _beforeRemoved += value; }
            remove { _beforeRemoved -= value; }
        }

        ObservableCollection<CheckableItemViewModel> _headers;
        /// <summary>
        /// 此对象允许配置的所有表头信息，允许外部更新这个集合的内容。
        /// </summary>
        public ObservableCollection<CheckableItemViewModel> Headers
        {
            get { return _headers; }
            set { SetProperty(ref _headers, value); }
        }

        /// <summary>
        /// 向当前表头信息中添加新的表头，默认为不选中
        /// </summary>
        /// <param name="append">是否在原有值基础上追加</param>
        /// <param name="headerNames">新添加的表头名称</param>
        public void AddHeaderInfos(bool append, params string[] headerNames)
        {
            if (!append)
            {
                _headers.Clear();
            }
            foreach (string header in headerNames)
            {
                // 表头名称不允许重复，如果有重复的直接跳过
                if (!_headers.Any(i => i.Name.Equals(header, StringComparison.OrdinalIgnoreCase)))
                {
                    var info = new CheckableItemViewModel(header);
                    info.CheckedChanged += i => UpdateCheckedHeaders();
                    info.Selected += () => UpdateCheckedHeaders();
                    _headers.Add(info);
                }
            }
            UpdateCheckedHeaders();
        }

        /// <summary>
        /// 移除指定名称的表头信息，名称不区分大小写
        /// </summary>
        /// <param name="headerNames"></param>
        public void RemoveHeaderInfos(params string[] headerNames)
        {
            foreach (string header in headerNames)
            {
                var exist = _headers.First(h => h.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                if (exist != null)
                {
                    _headers.Remove(exist);
                }
            }
            UpdateCheckedHeaders();
        }

        /// <summary>
        /// 获取当前所有已选中的表头名称
        /// </summary>
        /// <returns></returns>
        public string[] GetSelectedHeaders()
        {
            return _headers.Where(h => h.Checked)
                           .Select(h => h.Name)
                           .ToArray();
        }

        string _checkedHeaders;
        /// <summary>
        /// 当前表头列表中所有选中的表头名，中间以逗号分隔
        /// </summary>
        public string CheckedHeaders
        {
            get { return _checkedHeaders; }
            set { SetProperty(ref _checkedHeaders, value); }
        }

        void UpdateCheckedHeaders()
        {
            CheckedHeaders = string.Join(',', GetSelectedHeaders());
        }
        /// <summary>
        /// 当出示表头选中时触发的命令
        /// </summary>
        public ICommand HeaderSelectedCommand => new RelayCommand(UpdateCheckedHeaders);

        string _tableName;
        /// <summary>
        /// 当前表格配置集合的名称，对应生成的tab.mrs文件名，此文件名不允许重复，且不区分大小写。
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set 
            {
                if (_beforeRemoved == null || _beforeRemoved(value))
                {
                    string oldName = _tableName;
                    SetProperty(ref _tableName, value);
                    _tables.Name = value;
                    _renamed?.Invoke(oldName, value);
                }
            }
        }

        Action<TableSettingViewModel>? _removing;
        /// <summary>
        /// 用于控制移除此对象时触发的事件，由于此对象无法从父级对象中移除自身，而添加父级对象进
        /// 构造函数会使对象创建变得复杂，所以使用回调来从父级集合中移除自身。
        /// </summary>
        public event Action<TableSettingViewModel> Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        void Remove()
        {
            _removing?.Invoke(this);
        }
        /// <summary>
        /// 删除按钮命令
        /// </summary>
        public ICommand RemoveCommand => new RelayCommand(Remove);

        ObservableCollection<TableSettingElementViewModel> _elementList;
        /// <summary>
        /// 当前表格配置中所有的单表格配置对象
        /// </summary>
        public ObservableCollection<TableSettingElementViewModel> ElementList
        {
            get { return _elementList; }
            set { SetProperty(ref _elementList, value); }
        }
        /// <summary>
        /// 添加新的单表格配置进当前集合的末尾
        /// </summary>
        public ICommand PushNewElementCommand => new RelayCommand(Push);
        void Push()
        {
            var element = new TableSettingElementViewModel(_tables.NewTable());
            element.Removing += RemoveElementAt;
            element.MovingUp += MoveElementUp;
            element.MovingDown += MoveElementDown;
            ElementList.Add(element);
        }

        void RemoveElementAt(TableSettingElementViewModel viewModel)
        {
            _elementList.Remove(viewModel);
            _tables.Remove(viewModel.Table);
        }

        void MoveElementUp(TableSettingElementViewModel viewModel)
        {
            CollectionHelper.MoveUp(_elementList, viewModel);
            _tables.MoveUp(viewModel.Table);
        }

        void MoveElementDown(TableSettingElementViewModel viewModel)
        {
            CollectionHelper.MoveDown(_elementList, viewModel);
            _tables.MoveDown(viewModel.Table);
        }

        public void LoadFromTableSettingCollection(TableSettingCollection tableSettings)
        {
            _tableName = tableSettings.SpecTables.Name;
            foreach (TableSetting setting in tableSettings)
            {
                var tableSettingViewModel = new TableSettingElementViewModel(_tables.NewTable());
                tableSettingViewModel.LoadFromTableSetting(setting);
                _elementList.Add(tableSettingViewModel);
            }
        }
        /// <summary>
        /// 空白的View Model对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static TableSettingViewModel Empty(TableSettingTreeNode node)
        {
            return new TableSettingViewModel(new Tables(node.SpecDocument))
            {
                Node = node
            };
        }

    }

    public class TableSettingElementViewModel : ObservableObject
    {
        public TableSettingElementViewModel(Table table) 
        {
            _table = table;
            var sideAxis = _table.SideAxis ?? _table.CreateSideAxis(AxisType.Normal);
            if (sideAxis.Count == 0)
            {
                // 如果是空表达式，创建基础的表达式
                sideAxis.AppendText();
                sideAxis.AppendBase("Base : Total Respondent");
                sideAxis.AppendText();
                sideAxis.AppendCategoryRange();
                sideAxis.AppendText();
                sideAxis.AppendSubTotal();
            }
            _axisViewModel = new AxisSettingViewModel(sideAxis);
            // 初始化表格类型
            _tableTypeSelections = new string[5] 
            { 
                ViewModelConstants.TableTypeNormal,
                ViewModelConstants.TableTypeGrid,
                ViewModelConstants.TableTypeGridSlice,
                ViewModelConstants.TableTypeMeanSummary,
                ViewModelConstants.TableTypeResponseSummary
            };
            _tableTypeSelectedIndex = 0;
            // 初始化Top/Bottom Box选项类别
            _tableTopBottomBoxSelections = new ObservableCollection<CheckableItemViewModel>
            {
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop1BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop2BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop3BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop4BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop5BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop6BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom1BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom2BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom3BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom4BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom5BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom6BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisNpsName)
            };
        }

        CheckableItemViewModel CreateTableTopBottomBoxSelectionItem(string label)
        {
            var vm = new CheckableItemViewModel(label, OnTableTopBottomBoxNameCheckedChanged);
            vm.Selected += () => UpdateTableTopBottomBoxName();
            return vm;
        }

        readonly Table _table;
        readonly AxisSettingViewModel _axisViewModel;

        /// <summary>
        /// View Model对应的Model.Table对象
        /// </summary>
        public Table Table => _table;

        Action<TableSettingElementViewModel>? _removing = null;
        /// <summary>
        /// 移除此对象时使用的回调
        /// </summary>
        public event Action<TableSettingElementViewModel> Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        Action<TableSettingElementViewModel>? _movingUp;
        /// <summary>
        /// 将此对象向上移动时使用的回调
        /// </summary>
        public event Action<TableSettingElementViewModel>? MovingUp
        {
            add { _movingUp += value; }
            remove { _movingUp -= value; }
        }

        Action<TableSettingElementViewModel>? _movingDown;
        /// <summary>
        /// 将此对象向下移动时使用的回调
        /// </summary>
        public event Action<TableSettingElementViewModel>? MovingDown
        {
            add { _movingDown += value; }
            remove { _movingDown -= value; }
        }

        string[] _tableTypeSelections;
        /// <summary>
        /// TableType选择的选项
        /// </summary>
        public string[] TableTypeSelections
        {
            get { return _tableTypeSelections; }
            set { SetProperty(ref _tableTypeSelections, value); }
        }

        int _tableTypeSelectedIndex;
        /// <summary>
        /// TableType下拉框选择的索引，配置选项顺序应该和TableType枚举的顺序相同
        /// </summary>
        public int TableTypeSelectedIndex
        {
            get { return _tableTypeSelectedIndex; }
            set 
            { 
                SetProperty(ref _tableTypeSelectedIndex, value);
                _table.Type = (TableType)_tableTypeSelectedIndex;
            }
        }

        ObservableCollection<CheckableItemViewModel> _tableTopBottomBoxSelections;
        /// <summary>
        /// 表格轴表达式需要添加的Top/Bottom Box类型的勾选选项
        /// </summary>
        public ObservableCollection<CheckableItemViewModel> TableTopBottomBoxSelections
        {
            get { return _tableTopBottomBoxSelections; }
            set { SetProperty(ref _tableTopBottomBoxSelections, value); }
        }

        string _tableTopBottomBoxName = string.Empty;
        /// <summary>
        /// 当前已选中的Top/Bottom Box的配置名，中间用逗号分隔
        /// </summary>
        public string TableTopBottomBoxName
        {
            get { return _tableTopBottomBoxName; }
            set { SetProperty(ref _tableTopBottomBoxName, value); }
        }

        void UpdateTableTopBottomBoxName()
        {
            TableTopBottomBoxName = string.Join(',',
                _tableTopBottomBoxSelections.Where(t => t.Checked)
                                            .Select(t => t.Name));
        }

        void OnTableTopBottomBoxNameCheckedChanged(CheckableItemViewModel _)
        {
            UpdateTableTopBottomBoxName();
        }

        string[] _tableNetTypeSelections =
        {
            "标准Net",
            "在具体选项前的Combine",
            "在Subtotal和具体选项之间的Combine",
            "放在最后的Combine",
        };
        /// <summary>
        /// 当前表格添加Net的类型
        /// </summary>
        public string[] TableNetTypeSelections
        {
            get { return _tableNetTypeSelections; }
            set { SetProperty(ref _tableNetTypeSelections, value); }
        }

        int _tableNetTypeSelectedIndex = 0;
        /// <summary>
        /// 当前表格添加Net的类型的选中索引
        /// </summary>
        public int TableNetTypeSelectedIndex
        {
            get { return _tableNetTypeSelectedIndex; }
            set { SetProperty(ref _tableNetTypeSelectedIndex, value); }
        }

        /// <summary>
        /// 获取当前配置的表格Net类型
        /// </summary>
        /// <returns></returns>
        public AxisNetType GetTableNetType()
        {
            return (AxisNetType)_tableNetTypeSelectedIndex;
        }

        string _variableName = string.Empty;
        /// <summary>
        /// 当前表格配置使用的变量名
        /// </summary>
        public string VariableName
        {
            get { return _variableName; }
            set { SetProperty(ref _variableName, value); }
        }

        string _title = string.Empty;
        /// <summary>
        /// 当前表格配置的标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        string _baseText = string.Empty;
        /// <summary>
        /// 当前表格配置的Base行标签
        /// </summary>
        public string BaseText
        {
            get { return _baseText; }
            set { SetProperty(ref _baseText, value); }
        }

        string _baseFilter = string.Empty;
        /// <summary>
        /// 当前表格配置的Base中Filter内容，即base('')中单引号内的内容
        /// </summary>
        public string BaseFilter
        {
            get { return _baseFilter; }
            set { SetProperty(ref _baseFilter, value); }
        }

        string _tableFilter = string.Empty;
        /// <summary>
        /// 当前表格配置的表格筛选器，此筛选器将在Tab.mrs中应用于表格整体。
        /// </summary>
        public string TableFilter
        {
            get { return _tableFilter; }
            set { SetProperty(ref _tableFilter, value); }
        }

        string _tableAppendLabel = string.Empty;
        /// <summary>
        /// 当前表格在Tab.mrs中的追加标签
        /// </summary>
        public string TableAppendLabel
        {
            get { return _tableAppendLabel; }
            set { SetProperty(ref _tableAppendLabel, value); }
        }

        //int _indexOfParent = 0;
        ///// <summary>
        ///// 此对象在父级对象中的索引
        ///// </summary>
        //public int IndexOfParent { get => _indexOfParent; set => _indexOfParent = value; }

        public ICommand AxisSettingCommand => new RelayCommand(ShowAxisSettingDialog);
        void ShowAxisSettingDialog()
        {
            Common.WindowManager.ShowWindow("AxisSettingWindow", _axisViewModel);
        }

        public ICommand RemoveCommand => new RelayCommand(Remove);
        void Remove()
        {
            _removing?.Invoke(this);
        }

        public ICommand MoveUpCommand => new RelayCommand(MoveUp);
        void MoveUp()
        {
            _movingUp?.Invoke(this);
        }

        public ICommand MoveDownCommand => new RelayCommand(MoveDown);
        void MoveDown()
        {
            _movingDown?.Invoke(this);
        }

        public void LoadFromTableSetting(TableSetting tableSetting)
        {
            _variableName = tableSetting.Field != null ? tableSetting.Field.FullName : string.Empty;
            _title = tableSetting.TableTitle;
            _baseText = tableSetting.BaseLabel;
            _baseFilter = tableSetting.BaseFilter;
            _tableFilter = tableSetting.TableFilter;
        }


    }



}
