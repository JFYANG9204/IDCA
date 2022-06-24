
using IDCA.Bll;
using IDCA.Model;
using IDCA.Model.Spec;
using IDCA.Model.Template;
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

        public TableSettingViewModel(Tables tables, Config config, TemplateCollection templates)
        {
            _elementList = new ObservableCollection<TableSettingElementViewModel>();
            _tableName = tables.Name;
            _tables = tables;
            _headers = new ObservableCollection<CheckableItemViewModel>();
            _checkedHeaders = string.Empty;
            _config = config;
            _templates = templates;
        }

        readonly Tables _tables;
        readonly Config _config;
        readonly TemplateCollection _templates;

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
            var element = new TableSettingElementViewModel(_tables.NewTable(), _config, _templates);
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
                var tableSettingViewModel = new TableSettingElementViewModel(_tables.NewTable(), _config, _templates);
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
            return new TableSettingViewModel(new Tables(node.SpecDocument), new Config(), new TemplateCollection())
            {
                Node = node
            };
        }

    }

    public class TableSettingElementViewModel : ObservableObject
    {
        public TableSettingElementViewModel(Table table, Config config, TemplateCollection templates) 
        {
            _table = table;
            var sideAxis = _table.SideAxis ?? _table.CreateSideAxis(AxisType.Normal);
            _axisOperater = new AxisOperator(sideAxis, config, templates);
            if (sideAxis.Count == 0)
            {
                // 如果是空表达式，创建基础的表达式
                _axisOperater.CreateBasicAxisExpression(null);
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
            _topBottomBoxSelections = new ObservableCollection<CheckableItemViewModel>
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
            // Factor Sequence
            _factorSequenceSelectedIndex = 0;
            // Axis Operator
            _npsTopBox = _axisOperater.NpsTopBox.ToString();
            _npsBottomBox = _axisOperater.NpsBottomBox.ToString();
            _donotAppendMeanOrAverage = true;
            _appendMean = false;
            _appendAverage = false;
            _meanVariable = string.Empty;
        }

        CheckableItemViewModel CreateTableTopBottomBoxSelectionItem(string label)
        {
            var vm = new CheckableItemViewModel(label, OnTableTopBottomBoxNameCheckedChanged);
            vm.Selected += () => UpdateTableTopBottomBoxName();
            return vm;
        }

        readonly Table _table;
        readonly AxisSettingViewModel _axisViewModel;
        readonly AxisOperator _axisOperater;        

        Metadata? _metadata;
        /// <summary>
        /// 当前表格配置需要添加的新变量
        /// </summary>
        public Metadata? Metadata
        {
            get => _metadata;
            set => _metadata = value;
        }

        VariableSettingViewModel? _variableSettingViewModel;

        public VariableSettingViewModel? VariableSettingViewModel
        {
            get => _variableSettingViewModel;
            set => _variableSettingViewModel = value;
        }

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

        bool _addNewVariable;
        /// <summary>
        /// 是否添加新的变量
        /// </summary>
        public bool AddNewVariable
        {
            get { return _addNewVariable; }
            set 
            { 
                SetProperty(ref _addNewVariable, value);
                if (!value && _metadata != null)
                {
                    _table.Document?.DmsMetadata.Remove(_metadata.Name);
                    _metadata = null;
                    _variableSettingViewModel = null;
                }
                else if (value && _metadata == null)
                {
                    _metadata = _table.Document!.DmsMetadata.NewMetadata();
                    // 创建新的变量时，验证已填入的变量名，需要同时满足：不是空字符串、名称定义合法、集合中没有重名，
                    // 如果同时满足上述条件，将新变量名修改为当前已填入的名称
                    if (!string.IsNullOrEmpty(_variableName) &&
                        StringHelper.ValidateElementName(_variableName) && (
                        _table.Document == null || (
                        _table.Document.DmsMetadata[_variableName] == null &&
                        _table.Document.Metadata[_variableName] == null)))
                    {
                        _metadata.Name = _variableName;
                    }
                    // 如果上述条件有任一失败，将当前变量名修改为自动设置的名称
                    else
                    {
                        VariableName = _metadata.Name;
                    }
                    _variableSettingViewModel = new VariableSettingViewModel(_metadata);
                    _variableSettingViewModel.BeforeRenamed += name =>
                    {
                        return !string.IsNullOrEmpty(name) && 
                                StringHelper.ValidateElementName(name) && (
                                _table.Document == null || (
                                _table.Document.DmsMetadata[name] == null &&
                                _table.Document.Metadata[name] == null));
                    };
                    _variableSettingViewModel.Renamed += vm =>
                    {
                        VariableName = vm.Name;
                    };
                }
            }
        }

        bool _donotAppendMeanOrAverage;
        /// <summary>
        /// 不添加均值或平均提及，与AppendMean和AppendAverage互斥
        /// </summary>
        public bool DonotAppendMeanOrAverage
        {
            get { return _donotAppendMeanOrAverage; }
            set 
            { 
                SetProperty(ref _donotAppendMeanOrAverage, value);
                if (value)
                {
                    _appendMean = false;
                    _appendAverage = false;
                    _axisOperater.AppendMean = false;
                    _axisOperater.AppendAverage = false;
                }
            }
        }

        bool _appendMean;
        /// <summary>
        /// 添加计算均值，与DonotAppendMeanOrAverage和AppendAverage互斥
        /// </summary>
        public bool AppendMean
        {
            get { return _appendMean; }
            set
            {
                SetProperty(ref _appendMean, value);
                _axisOperater.AppendMean = value;
                if (value)
                {
                    _donotAppendMeanOrAverage = false;
                    _appendAverage = false;
                }
                UpdateAxisExpression();
            }
        }

        bool _appendAverage;
        /// <summary>
        /// 添加计算平均提及，与DonotAppendMeanOrAverage和AppendMean互斥
        /// </summary>
        public bool AppendAverage
        {
            get { return _appendAverage; }
            set
            {
                SetProperty(ref _appendAverage, value);
                _axisOperater.AppendAverage = value;
                if (value)
                {
                    _donotAppendMeanOrAverage = false;
                    _appendMean = false;
                }
                UpdateAxisExpression();
            }
        }

        string _meanVariable;
        /// <summary>
        /// 添加计算均值时使用的变量
        /// </summary>
        public string MeanVariable
        {
            get { return _meanVariable; }
            set
            {
                SetProperty(ref _meanVariable, value);
                _axisOperater.MeanVariable = value;
                UpdateAxisExpression();
            }
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

        ObservableCollection<CheckableItemViewModel> _topBottomBoxSelections;
        /// <summary>
        /// 表格轴表达式需要添加的Top/Bottom Box类型的勾选选项
        /// </summary>
        public ObservableCollection<CheckableItemViewModel> TopBottomBoxSelections
        {
            get { return _topBottomBoxSelections; }
            set { SetProperty(ref _topBottomBoxSelections, value); }
        }

        string _topBottomBoxName = string.Empty;
        /// <summary>
        /// 当前已选中的Top/Bottom Box的配置名，中间用逗号分隔
        /// </summary>
        public string TopBottomBoxName
        {
            get { return _topBottomBoxName; }
            set { SetProperty(ref _topBottomBoxName, value); }
        }

        /// <summary>
        /// 获取当前已选中的Box数值，如果未选中，返回null
        /// </summary>
        /// <returns></returns>
        public int[]? GetTopBottomBox()
        {
            var selectedBox = _topBottomBoxSelections.Where(s => s.Checked && s.Name != ViewModelConstants.AxisNpsName);
            if (!selectedBox.Any())
            {
                return null;
            }

            return selectedBox.Select(s =>
            {
                return s.Name switch
                {
                    ViewModelConstants.AxisTop1BoxName => 1,
                    ViewModelConstants.AxisTop2BoxName => 2,
                    ViewModelConstants.AxisTop3BoxName => 3,
                    ViewModelConstants.AxisTop4BoxName => 4,
                    ViewModelConstants.AxisTop5BoxName => 5,
                    ViewModelConstants.AxisTop6BoxName => 6,
                    ViewModelConstants.AxisBottom1BoxName => -1,
                    ViewModelConstants.AxisBottom2BoxName => -2,
                    ViewModelConstants.AxisBottom3BoxName => -3,
                    ViewModelConstants.AxisBottom4BoxName => -4,
                    ViewModelConstants.AxisBottom5BoxName => -5,
                    ViewModelConstants.AxisBottom6BoxName => -6,
                    _ => 0
                };
            }).ToArray();
        }

        /// <summary>
        /// 判定当前是否选中了NPS
        /// </summary>
        /// <returns></returns>
        public bool SelectedNps()
        {
            return _topBottomBoxSelections.Any(e => e.Checked && e.Name == ViewModelConstants.AxisNpsName);
        }

        void UpdateAxisExpression()
        {
            var boxes = GetTopBottomBox();
            var nps = SelectedNps();
            if (boxes != null)
            {
                _axisOperater.CreateTopBottomBoxAxisExpression(_baseText, nps, boxes);
                _axisViewModel.LoadFromAxis(_axisOperater.Axis);
                return;
            }
            // 只添加NPS
            if (nps)
            {
                _axisOperater.CreateTopBottomBoxAxisExpression(_baseText, true);
                _axisViewModel.LoadFromAxis(_axisOperater.Axis);
                return;
            }

            _axisOperater.CreateBasicAxisExpression(_baseText);
            _axisViewModel.LoadFromAxis(_axisOperater.Axis);
        }

        string[] _factorSequence = { "正序", "逆序" };
        /// <summary>
        /// 当前表格表侧Factor赋值的顺序，正序时递增，逆序时递减。
        /// </summary>
        public string[] FactorSequence
        {
            get { return _factorSequence; }
            set { SetProperty(ref _factorSequence, value); }
        }

        int _factorSequenceSelectedIndex;
        /// <summary>
        /// 当前表格Factor赋值顺序选择索引，修改时同步修改Axis对象配置
        /// </summary>
        public int FactorSequenceSelectedIndex
        {
            get { return _factorSequenceSelectedIndex; }
            set 
            {
                SetProperty(ref _factorSequenceSelectedIndex, value);
                _axisOperater.IsTopBottomBoxReversed = value == 1;
                UpdateAxisExpression();
            }
        }

        string _npsTopBox;
        /// <summary>
        /// Nps使用的Top Box包含的选项数量
        /// </summary>
        public string NpsTopBox
        {
            get { return _npsTopBox; }
            set
            {
                SetProperty(ref _npsTopBox, value);
                if (int.TryParse(value, out int iValue) && iValue > 0)
                {
                    _axisOperater.NpsTopBox = iValue;
                }
            }
        }

        string _npsBottomBox;
        /// <summary>
        /// Nps使用的Bottom Box包含的选项数量
        /// </summary>
        public string NpsBottomBox
        {
            get { return _npsBottomBox; }
            set
            {
                SetProperty(ref _npsBottomBox, value);
                if (int.TryParse(value, out int iValue) && iValue > 0)
                {
                    _axisOperater.NpsBottomBox = iValue;
                }
            }
        }

        void UpdateTableTopBottomBoxName()
        {
            TopBottomBoxName = string.Join(',',
                _topBottomBoxSelections.Where(t => t.Checked)
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
        public string[] NetTypeSelections
        {
            get { return _tableNetTypeSelections; }
            set { SetProperty(ref _tableNetTypeSelections, value); }
        }

        int _netTypeSelectedIndex = 0;
        /// <summary>
        /// 当前表格添加Net的类型的选中索引
        /// </summary>
        public int NetTypeSelectedIndex
        {
            get { return _netTypeSelectedIndex; }
            set 
            { 
                SetProperty(ref _netTypeSelectedIndex, value);
                _axisOperater.NetType = (AxisNetType)value;
            }
        }

        /// <summary>
        /// 获取当前配置的表格Net类型
        /// </summary>
        /// <returns></returns>
        public AxisNetType GetTableNetType()
        {
            return (AxisNetType)_netTypeSelectedIndex;
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

        public ICommand NewVariableSettingCommand => new RelayCommand(ShowNewVariableSettingDialog);
        void ShowNewVariableSettingDialog()
        {
            Common.WindowManager.ShowWindow("VariableSettingWindow", _variableSettingViewModel);
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
