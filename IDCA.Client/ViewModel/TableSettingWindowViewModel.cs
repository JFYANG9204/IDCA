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
            _headerNames = new List<string>();
            _tablesNames = new List<string>();

            _headerNode = new TableSettingTreeNode("表头", null);
            _headerNode.SelectedChanged += OnSelectedChanged;
            _tableNode = new TableSettingTreeNode("表侧", null);
            _tableNode.SelectedChanged += OnSelectedChanged;
            _selectedNode = _headerNode;
        }

        readonly List<string> _headerNames;
        readonly List<string> _tablesNames;

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


        void OnSelectedChanged(TableSettingTreeNode node, bool selected)
        {
            if (selected)
            {
                SelectedNode = node;
            }
        }

        public void AppendNewHeader(string name = "")
        {
            string headerName = name;
            if (string.IsNullOrEmpty(headerName))
            {
                headerName = $"TopBreak_{_headerNode.Children.Count + 1}";
            }
            var header = new TableSettingTreeNode(
                headerName, 
                new HeaderSettingViewModel(headerName, _headerNames));
            header.SelectedChanged += OnSelectedChanged;
            _headerNode.Children.Add(header);
            SelectedNode = _headerNode;
        }

        public void AppendNewTables(string name = "")
        {
            string tablesName = name;
            if (string.IsNullOrEmpty(tablesName))
            {
                tablesName = $"Tab_{_tableNode.Children.Count + 1}";
            }
            var tables = new TableSettingTreeNode(
                tablesName, 
                new TableSettingViewModel(tablesName, _tablesNames));
            tables.SelectedChanged += OnSelectedChanged;
            _tableNode.Children.Add(tables);
            SelectedNode = tables;
        }

        public ICommand NewHeaderCommand => new RelayCommand(() => AppendNewHeader());
        public ICommand NewTablesCommand => new RelayCommand(() => AppendNewTables());

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

            var type = dataContext?.GetType();

            if (dataContext == null || type == null)
            {
                _headerViewModel = new HeaderSettingViewModel("") { Node = this };
                _tableViewModel = new TableSettingViewModel("") { Node = this };
                _overviewViewModel = new OverviewViewModel() { Node = this };
            }
            else if (type.Equals(typeof(TableSettingViewModel)))
            {
                _tableViewModel = (TableSettingViewModel)dataContext;
                _tableViewModel.Node = this;
                _tableViewModel.Renamed += name => Name = name;
                _isTableView = true;

                _headerViewModel = new HeaderSettingViewModel("") { Node = this };
                _overviewViewModel = new OverviewViewModel() { Node = this };
            }
            else if (type.Equals(typeof(HeaderSettingViewModel)))
            {
                _headerViewModel = (HeaderSettingViewModel)dataContext;
                _headerViewModel.Node = this;
                _headerViewModel.Renamed += name => Name = name;
                _isHeaderView = true;

                _tableViewModel = new TableSettingViewModel("") { Node = this };
                _overviewViewModel = new OverviewViewModel() { Node = this };
            }
            else if (type.Equals(typeof(OverviewViewModel)))
            {
                _overviewViewModel = (OverviewViewModel)dataContext;
                _overviewViewModel.Node = this;
                _isOverView = true;

                _tableViewModel = new TableSettingViewModel("") { Node = this };
                _headerViewModel = new HeaderSettingViewModel("") { Node = this };
            }
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

    }



}
