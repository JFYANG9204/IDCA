using IDCA.Client.Singleton;
using IDCA.Model.Spec;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{

    public class TableSettingWindowViewModel : ObservableObject
    {
        public TableSettingWindowViewModel()
        {
            _tree = new TableSettingTreeNode("Root", null);

            _headerNode = new TableSettingTreeNode("表头", null) { AllowNodeRemoving = false };
            _headerNode.SelectedChanged += OnSelectedChanged;
            _headerNode.AddNewChild += () => AppendNewHeader();

            _tableNode = new TableSettingTreeNode("表侧", null) { AllowNodeRemoving = false };
            _tableNode.SelectedChanged += OnSelectedChanged;
            _tableNode.AddNewChild += () => AppendNewTables();

            _tree.Children.Add(_headerNode);
            _tree.Children.Add(_tableNode);

            _selectedNode = _headerNode;

            _spec = GlobalConfig.Instance.SpecDocument;
            _spec.HeaderAdded += OnHeaderAdded;
            _spec.HeaderRemoved += OnHeaderRemoved;
        }

        readonly SpecDocument _spec;

        TableSettingTreeNode _tree;
        public TableSettingTreeNode Tree
        {
            get { return _tree; }
            set { SetProperty(ref _tree, value); }
        }


        TableSettingTreeNode _headerNode;
        TableSettingTreeNode _tableNode;

        public TableSettingTreeNode HeaderNode
        {
            get { return _headerNode; }
            set { SetProperty(ref _headerNode, value); }
        }

        public TableSettingTreeNode TableNode
        {
            get { return _tableNode; }
            set { SetProperty(ref _tableNode, value); }
        }

        TableSettingTreeNode? _selectedNode;
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
                node.TableViewModel?.AddHeaderInfos(true, header.Name);
            }
        }

        void OnHeaderRemoved(Metadata header)
        {
            foreach (var node in _tableNode.Children)
            {
                node.TableViewModel?.RemoveHeaderInfos(header.Name);
            }
        }

        public void AppendNewHeader(string name = "")
        {
            var vm = new HeaderSettingViewModel(_spec.NewHeader(name));
            vm.BeforeRenamed += ValidateHeaderName;
            vm.Renamed += OnHeaderRenamed;

            var header = new TableSettingTreeNode(vm.HeaderName, vm) { AllowAppendChild = false };
            header.SelectedChanged += OnSelectedChanged;
            header.Removing += OnNodeRemoving;
            _headerNode.Children.Add(header);

            SelectedNode = header;
        }

        public void AppendNewTables(string name = "")
        {
            var vm = new TableSettingViewModel(_spec.NewTables(name));
            vm.AddHeaderInfos(false, _spec.GetHeaderNames());
            vm.BeforeRemoved += ValidateTablesName;
            vm.Renamed += OnTablesRenamed;

            var tables = new TableSettingTreeNode(vm.TableName, vm) { AllowAppendChild = false };
            tables.SelectedChanged += OnSelectedChanged;
            tables.Removing += OnNodeRemoving;
            _tableNode.Children.Add(tables);

            SelectedNode = tables;
        }

        void OnNodeRemoving(TableSettingTreeNode node)
        {
            if (node.IsTableView)
            {
                _tableNode.RemoveChild(node);
                _spec.RemoveTables(node.Name);
            }
            else if (node.IsHeaderView)
            {
                _headerNode.RemoveChild(node);
                _spec.RemoveHeader(node.Name);                
            }
        }

    }

    public class TableSettingTreeNode : ObservableObject
    {
        public TableSettingTreeNode(string name, ObservableObject? dataContext)
        {
            _name = name;
            _children = new ObservableCollection<TableSettingTreeNode>();

            _isHeaderView = false;
            _isTableView = false;
            _isOverView = false;

            _allowAppendChild = true;
            _allowNodeRemoving = true;

            var type = dataContext?.GetType();

            if (dataContext == null || type == null)
            {
                _headerViewModel = HeaderSettingViewModel.Empty(this);
                _tableViewModel = TableSettingViewModel.Empty(this);
                _overviewViewModel = new OverviewViewModel() { Node = this };
            }
            else if (type.Equals(typeof(TableSettingViewModel)))
            {
                _tableViewModel = (TableSettingViewModel)dataContext;
                _tableViewModel.Node = this;
                _isTableView = true;

                _headerViewModel = HeaderSettingViewModel.Empty(this);
                _overviewViewModel = new OverviewViewModel() { Node = this };
            }
            else if (type.Equals(typeof(HeaderSettingViewModel)))
            {
                _headerViewModel = (HeaderSettingViewModel)dataContext;
                _headerViewModel.Node = this;
                _isHeaderView = true;

                _tableViewModel = TableSettingViewModel.Empty(this);
                _overviewViewModel = new OverviewViewModel() { Node = this };
            }
            else if (type.Equals(typeof(OverviewViewModel)))
            {
                _overviewViewModel = (OverviewViewModel)dataContext;
                _overviewViewModel.Node = this;
                _isOverView = true;

                _tableViewModel = TableSettingViewModel.Empty(this);
                _headerViewModel = HeaderSettingViewModel.Empty(this);
            }
        }

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

        bool _isTableView;
        public bool IsTableView
        {
            get { return _isTableView; }
            set { SetProperty(ref _isTableView, value); }
        }

        bool _isHeaderView;
        public bool IsHeaderView
        {
            get { return _isHeaderView; }
            set { SetProperty(ref _isHeaderView, value); }
        }

        bool _isOverView;
        public bool IsOverView
        {
            get { return _isOverView; }
            set { SetProperty(ref _isOverView, value); }
        }

        TableSettingViewModel? _tableViewModel;
        public TableSettingViewModel? TableViewModel
        {
            get { return _tableViewModel; }
            set { SetProperty(ref _tableViewModel, value); }
        }

        HeaderSettingViewModel? _headerViewModel;
        public HeaderSettingViewModel? HeaderViewModel
        {
            get { return _headerViewModel; }
            set { SetProperty(ref _headerViewModel, value); }
        }

        OverviewViewModel? _overviewViewModel;
        public OverviewViewModel? OverviewViewModel
        {
            get { return _overviewViewModel; }
            set { SetProperty(ref _overviewViewModel, value); }
        }

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
