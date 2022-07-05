using IDCA.Client.Singleton;
using IDCA.Client.ViewModel.Common;
using IDCA.Model.MDM;
using IDCA.Model.Spec;
using IDCA.Model.Template;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    /// <summary>
    /// TableSettingWindow窗口使用的ViewModel
    /// </summary>
    public class TableSettingWindowViewModel : ObservableObject
    {
        public TableSettingWindowViewModel(TemplateCollection template, MDMDocument mDM)
        {
            _spec = new SpecDocument(GlobalConfig.Instance.ProjectRootPath, template, GlobalConfig.Instance.Config)
            {
                ProjectDescription = GlobalConfig.Instance.ProjectName
            };
            _spec.HeaderAdded += OnHeaderAdded;
            _spec.HeaderRemoved += OnHeaderRemoved;
            _spec.Init(mDM);

            _tree = new TableSettingTreeNode("Root", _spec, GlobalConfig.Instance.OverviewViewModel);

            _headerNode = new TableSettingTreeNode("表头", _spec, GlobalConfig.Instance.OverviewViewModel);
            _headerNode.SelectedChanged += OnSelectedChanged;
            _headerNode.AddNewChild += () => AppendNewHeader();

            _tableNode = new TableSettingTreeNode("表侧", _spec, GlobalConfig.Instance.OverviewViewModel);
            _tableNode.SelectedChanged += OnSelectedChanged;
            _tableNode.AddNewChild += () => AppendNewTables();

            _tree.Children.Add(_headerNode);
            _tree.Children.Add(_tableNode);

            _selectedNode = _headerNode;

            _mdd = new MDMDocument();
            if (!string.IsNullOrEmpty(GlobalConfig.Instance.MdmDocumentPath) &&
                File.Exists(GlobalConfig.Instance.MdmDocumentPath))
            {
                _mdd.Open(GlobalConfig.Instance.MdmDocumentPath);
            }
        }

        readonly SpecDocument _spec;
        readonly MDMDocument _mdd;

        public SpecDocument SpecDocument => _spec;
        public MDMDocument MdmDocument => _mdd;

        string _projectPath = GlobalConfig.Instance.ProjectRootPath;
        /// <summary>
        /// 当前项目的根目录，此窗口可以修改，但是只能通过文件夹选择窗口选择，此属性绑定的TextBox应该是只读的。
        /// </summary>
        public string ProjectPath
        {
            get { return _projectPath; }
            set 
            { 
                SetProperty(ref _projectPath, value);
                _spec.ProjectPath = value;
            }
        }

        string _mdmDocumentPath = GlobalConfig.Instance.MdmDocumentPath;
        /// <summary>
        /// 当前项目的MDD文档路径，此窗口可以修改，但是只能通过文件选择窗口选择，此属性绑定的TextBox应该是只读的。
        /// </summary>
        public string MdmDocumentPath
        {
            get { return _mdmDocumentPath; }
            set
            {
                if (!string.IsNullOrEmpty(value) && File.Exists(value))
                {
                    SetProperty(ref _mdmDocumentPath, value);
                    _mdd.Close();
                    _mdd.Open(value);
                }
            }
        }

        string _excelSettingFilePath = GlobalConfig.Instance.ExcelSettingFilePath;
        /// <summary>
        /// 当前项目的Excel配置文档路径，此窗口可以修改，但是只能通过文件选择窗口选择，此属性绑定的TextBox应该是只读的。
        /// </summary>
        public string ExcelSettingFilePath
        {
            get { return _excelSettingFilePath; }
            set
            {
                if (!string.IsNullOrEmpty(value) && File.Exists(value))
                {
                    SetProperty(ref _excelSettingFilePath, value);
                }
            }
        }


        TableSettingTreeNode _tree;
        /// <summary>
        /// 左侧“表头/表侧”出示的树视图
        /// </summary>
        public TableSettingTreeNode Tree
        {
            get { return _tree; }
            set { SetProperty(ref _tree, value); }
        }

        TableSettingTreeNode _headerNode;
        TableSettingTreeNode _tableNode;
        /// <summary>
        /// 存储表头内容的节点
        /// </summary>
        public TableSettingTreeNode HeaderNode
        {
            get { return _headerNode; }
            set { SetProperty(ref _headerNode, value); }
        }
        /// <summary>
        /// 存储表侧内容的节点
        /// </summary>
        public TableSettingTreeNode TableNode
        {
            get { return _tableNode; }
            set { SetProperty(ref _tableNode, value); }
        }

        TableSettingTreeNode? _selectedNode;
        /// <summary>
        /// “表头/表侧”数视图中选中的节点，如果没有选中的，值为null。
        /// </summary>
        public TableSettingTreeNode? SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
        }

        bool ValidateHeaderName(string name) 
        {
            return _spec.ValidateHeaderName(name);
        }

        bool ValidateTablesName(string name)
        {
            return _spec.ValidateTablesName(name);
        }

        void OnHeaderRenamed(string oldeName, string newName)
        {
            if (SelectedNode != null)
            {
                SelectedNode.Name = newName;
            }
        }

        void OnTablesRenamed(string oldName, string newName)
        {
            if (SelectedNode != null)
            {
                SelectedNode.Name = newName;
            }
        }

        void OnSelectedChanged(TableSettingTreeNode node, bool selected)
        {
            if (selected)
            {
                SelectedNode = node;
            }
        }

        void OnHeaderAdded(Metadata header)
        {
            foreach (var node in _tableNode.Children)
            {
                if (node.ViewModel is TableSettingViewModel viewModel)
                {
                    viewModel.AddHeaderInfos(true, header.Name);
                }
            }
        }

        void OnHeaderRemoved(Metadata header)
        {
            foreach (var node in _tableNode.Children)
            {
                if (node.ViewModel is TableSettingViewModel viewModel)
                {
                    viewModel.RemoveHeaderInfos(header.Name);
                }
            }
        }

        public void AppendNewHeader(string name = "")
        {
            var vm = new HeaderSettingViewModel(_spec.NewHeader(name));
            vm.BeforeRenamed += ValidateHeaderName;
            vm.Renamed += OnHeaderRenamed;

            var header = new TableSettingTreeNode(vm.HeaderName, _spec, vm) { AllowAppendChild = false };
            header.SelectedChanged += OnSelectedChanged;
            header.Removing += OnNodeRemoving;
            _headerNode.Children.Add(header);

            SelectedNode = header;
        }

        public void AppendNewTables(string name = "")
        {
            var vm = new TableSettingViewModel(_spec.NewTables(name), _spec.Config, _spec.Templates);
            vm.AddHeaderInfos(false, _spec.GetHeaderNames());
            vm.BeforeRemoved += ValidateTablesName;
            vm.Renamed += OnTablesRenamed;

            var tables = new TableSettingTreeNode(vm.TableName, _spec, vm) { AllowAppendChild = false };
            tables.SelectedChanged += OnSelectedChanged;
            tables.Removing += OnNodeRemoving;
            _tableNode.Children.Add(tables);

            SelectedNode = tables;
        }

        void OnNodeRemoving(TableSettingTreeNode node)
        {
            if (node.ViewModel is TableSettingViewModel)
            {
                _tableNode.RemoveChild(node);
                _spec.RemoveTables(node.Name);
            }
            else if (node.ViewModel is HeaderSettingViewModel)
            {
                _headerNode.RemoveChild(node);
                _spec.RemoveHeader(node.Name);                
            }
        }

        void SelectMdmDocument()
        {
            string? dialogResult = WindowManager.ShowOpenFileDialog("MDM Document|*.mdd");
            if (dialogResult != null && File.Exists(dialogResult))
            {
                MdmDocumentPath = dialogResult;
            }
        }
        public ICommand SelectMdmDocumentCommand => new RelayCommand(SelectMdmDocument);

        void SelectProjectPath()
        {
            string? dialogResult = WindowManager.ShowFolderBrowserDialog();
            if (dialogResult != null)
            {
                ProjectPath = dialogResult;
            }
        }
        public ICommand SelectProjectRootPathCommand => new RelayCommand(SelectProjectPath);

        void SelectExcelSettingFile()
        {
            string? dialogResult = WindowManager.ShowOpenFileDialog("Excel File|*.xlsx");
            if (!string.IsNullOrEmpty(dialogResult) && File.Exists(dialogResult))
            {
                ExcelSettingFilePath = dialogResult;
            }
        }
        public ICommand SelectExcelSettingFileCommand => new RelayCommand(SelectExcelSettingFile);

        void ShowSettingWindow()
        {
            WindowManager.ShowWindow("SettingWindow", GlobalConfig.Instance.SettingWindowViewModel);
        }
        public ICommand ShowSettingWindowCommand => new RelayCommand(ShowSettingWindow);

        void Execute()
        {
            _spec.Exec();
        }
        public ICommand ExecuteCommand => new RelayCommand(Execute);

    }

    public class TableSettingTreeNode : ObservableObject
    {
        public TableSettingTreeNode(string name, SpecDocument spec, ObservableObject dataContext)
        {
            _name = name;
            _spec = spec;

            _children = new ObservableCollection<TableSettingTreeNode>();

            _viewModel = dataContext;

            if (dataContext is OverviewViewModel)
            {
                _allowAppendChild = true;
                _allowNodeRemoving = false;
            }
            else
            {
                _allowAppendChild = false;
                _allowNodeRemoving = true;
            }

            //if (dataContext == null || type == null)
            //{
            //    _headerViewModel = HeaderSettingViewModel.Empty(this);
            //    _tableViewModel = TableSettingViewModel.Empty(this);
            //    _overviewViewModel = new OverviewViewModel() { Node = this };
            //}
            //else if (type.Equals(typeof(TableSettingViewModel)))
            //{
            //    _tableViewModel = (TableSettingViewModel)dataContext;
            //    _tableViewModel.Node = this;
            //    _isTableView = true;
            //
            //    _headerViewModel = HeaderSettingViewModel.Empty(this);
            //    _overviewViewModel = new OverviewViewModel() { Node = this };
            //}
            //else if (type.Equals(typeof(HeaderSettingViewModel)))
            //{
            //    _headerViewModel = (HeaderSettingViewModel)dataContext;
            //    _headerViewModel.Node = this;
            //    _isHeaderView = true;
            //
            //    _tableViewModel = TableSettingViewModel.Empty(this);
            //    _overviewViewModel = new OverviewViewModel() { Node = this };
            //}
            //else if (type.Equals(typeof(OverviewViewModel)))
            //{
            //    _overviewViewModel = (OverviewViewModel)dataContext;
            //    _overviewViewModel.Node = this;
            //    _isOverView = true;
            //
            //    _tableViewModel = TableSettingViewModel.Empty(this);
            //    _headerViewModel = HeaderSettingViewModel.Empty(this);
            //}
        }

        readonly SpecDocument _spec;
        /// <summary>
        /// 当前的Spec文档对象
        /// </summary>
        public SpecDocument SpecDocument => _spec;

        bool _allowNodeRemoving;
        public bool AllowNodeRemoving
        {
            get { return _allowNodeRemoving; }
            set { SetProperty(ref _allowNodeRemoving, value); }
        }

        bool _allowAppendChild;
        public bool AllowAppendChild
        {
            get { return _allowAppendChild; }
            set { SetProperty(ref _allowAppendChild, value); }
        }

        Action<TableSettingTreeNode>? _removing;
        public event Action<TableSettingTreeNode>? Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        Action? _addNewChild;
        public event Action? AddNewChild
        {
            add { _addNewChild += value; }
            remove { _addNewChild -= value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        ///bool _isTableView;
        ///public bool IsTableView
        ///{
        ///    get { return _isTableView; }
        ///    set { SetProperty(ref _isTableView, value); }
        ///}
        ///
        ///bool _isHeaderView;
        ///public bool IsHeaderView
        ///{
        ///    get { return _isHeaderView; }
        ///    set { SetProperty(ref _isHeaderView, value); }
        ///}
        ///
        ///bool _isOverView;
        ///public bool IsOverView
        ///{
        ///    get { return _isOverView; }
        ///    set { SetProperty(ref _isOverView, value); }
        ///}

        readonly ObservableObject _viewModel;
        /// <summary>
        /// 当前节点使用的ViewModel，对应不同的DaTaTemplate
        /// </summary>
        public ObservableObject ViewModel => _viewModel;

        //TableSettingViewModel? _tableViewModel;
        //public TableSettingViewModel? TableViewModel
        //{
        //    get { return _tableViewModel; }
        //    set { SetProperty(ref _tableViewModel, value); }
        //}
        //
        //HeaderSettingViewModel? _headerViewModel;
        //public HeaderSettingViewModel? HeaderViewModel
        //{
        //    get { return _headerViewModel; }
        //    set { SetProperty(ref _headerViewModel, value); }
        //}
        //
        //OverviewViewModel? _overviewViewModel;
        //public OverviewViewModel? OverviewViewModel
        //{
        //    get { return _overviewViewModel; }
        //    set { SetProperty(ref _overviewViewModel, value); }
        //}

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
                _selectedChanged?.Invoke(this, value);
            }
        }

        Action<TableSettingTreeNode, bool>? _selectedChanged;
        public event Action<TableSettingTreeNode, bool> SelectedChanged
        {
            add { _selectedChanged += value; }
            remove { _selectedChanged -= value; }
        }

        ObservableCollection<TableSettingTreeNode> _children;
        public ObservableCollection<TableSettingTreeNode> Children
        {
            get { return _children; }
            set { SetProperty(ref _children, value); }
        }        

        public void RemoveChild(TableSettingTreeNode node)
        {
            int index = _children.IndexOf(node);
            if (index > -1)
            {
                _children.RemoveAt(index);
            }
        }

        public ICommand RemoveCommand => new RelayCommand(() => _removing?.Invoke(this));
        public ICommand AddChildCommand => new RelayCommand(() => _addNewChild?.Invoke());

    }



}
