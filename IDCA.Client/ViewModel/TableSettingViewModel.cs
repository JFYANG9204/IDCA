
using IDCA.Client.Common;
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
    [ViewModelMark(SettingViewType.TableSetting, "IDCA.Client.View.TableSettingView", "IDCA.Client.View")]
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

    }

    public class TableSettingElementViewModel : ObservableObject
    {

        // 初始化表格类型
        public static readonly string[] TableTypeSelections = { 
            ViewModelConstants.TableTypeNormal,
            ViewModelConstants.TableTypeGrid,
            ViewModelConstants.TableTypeGridSlice,
            ViewModelConstants.TableTypeMeanSummary,
            ViewModelConstants.TableTypeResponseSummary
        };

        /**
         *  由于一个变量可能在表格配置中出现多次，为了避免重复设定轴表达式，在ViewModel中设定_axisViewModel/_axisOperator/_manipulate为
         *  可空类型，其中，
         *      + 如果_manipulation不为null，表示这个变量是第一次出现，需要同步追加Manipulation文件中的内容，同时_axsiViewModel和_axsiOperator绑定轴配置；
         *      + 如果_manipulation为null，同时_axisViewModel和_axisOperator不为null，表示此表格表侧变量追加轴表达式，将体现在Table文件中。
         */

        public TableSettingElementViewModel(Table table, Config config, TemplateCollection templates) 
        {
            _table = table;
            _config = config;
            _templates = templates;
            _isAxisSettingButtonEnabled = false;
            //var sideAxis = _table.SideAxis ?? _table.CreateSideAxis(AxisType.Normal);
            //_axisOperator = new AxisOperator(sideAxis, config, templates);
            //if (sideAxis.Count == 0)
            //{
                // 如果是空表达式，创建基础的表达式
                //_axisOperator.CreateBasicAxisExpression(null);
            //}
            //_axisViewModel = new AxisSettingViewModel(this, _axisOperator);
            _tableTypeSelectedIndex = 0;
        }

        readonly Config _config;
        readonly TemplateCollection _templates;

        readonly Table _table;
        AxisSettingViewModel? _axisViewModel;
        AxisOperator? _axisOperator;

        Model.Spec.Manipulation? _manipulation;
        /// <summary>
        /// 当前表格使用的Manipulation对象，如果同一个变量的表头重复出现，
        /// 应当只有第一个对象此属性不为null，后续对象应当为null
        /// </summary>
        public Model.Spec.Manipulation? Manipulation
        {
            get => _manipulation;
            set => _manipulation = value;
        }

        Metadata? _metadata;
        /// <summary>
        /// 当前表格配置需要添加的新变量
        /// </summary>
        public Metadata? Metadata
        {
            get => _metadata;
            set => _metadata = value;
        }

        bool _isAxisSettingButtonEnabled;
        /// <summary>
        /// 控制轴表达式配置窗口按钮是否可用
        /// </summary>
        public bool IsAxisSettingButtonEnabled
        {
            get { return _isAxisSettingButtonEnabled; }
            set 
            { 
                SetProperty(ref _isAxisSettingButtonEnabled, value);
                if (value)
                {
                    if (_axisOperator == null)
                    {
                        if (_manipulation != null)
                        {
                            _axisOperator = new AxisOperator(_manipulation.Axis, _config, _templates);
                            _axisViewModel = new AxisSettingViewModel(this, _axisOperator);
                        }
                        else
                        {
                            _axisOperator = new AxisOperator(_table.CreateSideAxis(AxisType.Normal), _config, _templates);
                            _axisViewModel = new AxisSettingViewModel(this, _axisOperator);
                        }
                    }
                }
                else
                {
                    if (_manipulation == null)
                    {
                        _axisOperator = null;
                        _axisViewModel = null;
                    }
                }
            }
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

        string _variableName = string.Empty;
        /// <summary>
        /// 当前表格配置使用的变量名
        /// </summary>
        public string VariableName
        {
            get { return _variableName; }
            set 
            { 
                SetProperty(ref _variableName, value);
                var manipulations = _table.Document?.Manipulations;
                if (manipulations != null)
                {
                    if (_manipulation != null)
                    {
                        manipulations.Remove(_manipulation);
                    }
                    _manipulation = manipulations.FromField(value, _title);
                    IsAxisSettingButtonEnabled = true;
                    if (_axisOperator != null)
                    {
                        _manipulation.SetAxis(_axisOperator.Axis);
                    }
                }
            }
        }

        string _title = string.Empty;
        /// <summary>
        /// 当前表格配置的标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set 
            { 
                SetProperty(ref _title, value);
                if (_manipulation != null)
                {
                    var titleFunction = _manipulation.First(FunctionTemplateFlags.ManipulateTitleLabel);
                    if (titleFunction == null)
                    {
                        _manipulation.AppendTitleTextFunction(value);
                    }
                    else
                    {
                        titleFunction.SetFunctionParameterValue(value, TemplateParameterUsage.ManipulateLabelText);
                    }
                }
            }
        }

        string _baseText = string.Empty;
        /// <summary>
        /// 当前表格配置的Base行标签
        /// </summary>
        public string BaseText
        {
            get { return _baseText; }
            set 
            { 
                SetProperty(ref _baseText, value);
                // 同步修改轴表达式中的配置
                var e = _axisOperator?.First(AxisElementType.Base);
                if (e != null)
                {
                    e.Description = value;
                }
                // 如果当前 Manipulation == null，表示当前变量不是第一次出现，需要将描述同步修改到Table对象中
                if (_manipulation == null)
                {
                    _table.TableBase = value;
                }
            }
        }

        string _baseFilter = string.Empty;
        /// <summary>
        /// 当前表格配置的Base中Filter内容，即base('')中单引号内的内容
        /// </summary>
        public string BaseFilter
        {
            get { return _baseFilter; }
            set 
            { 
                SetProperty(ref _baseFilter, value);
                // 同步修改轴表达式中的配置
                var e = _axisOperator?.First(AxisElementType.Base);
                if (e != null)
                {
                    var parameter = e.Template.GetParameter(0);
                    if (parameter == null)
                    {
                        parameter = e.Template.NewParameter();
                        e.Template.PushParameter(parameter);
                    }
                    parameter.SetValue(value);
                }
            }
        }

        string _tableFilter = string.Empty;
        /// <summary>
        /// 当前表格配置的表格筛选器，此筛选器将在Tab.mrs中应用于表格整体。
        /// </summary>
        public string TableFilter
        {
            get { return _tableFilter; }
            set 
            { 
                SetProperty(ref _tableFilter, value);
                _table.FilterInTableFile = value;
            }
        }

        string _tableAppendLabel = string.Empty;
        /// <summary>
        /// 当前表格在Tab.mrs中的追加标签
        /// </summary>
        public string TableAppendLabel
        {
            get { return _tableAppendLabel; }
            set 
            { 
                SetProperty(ref _tableAppendLabel, value);
                _table.LabelInTableFile = value;
            }
        }


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


    }



}
