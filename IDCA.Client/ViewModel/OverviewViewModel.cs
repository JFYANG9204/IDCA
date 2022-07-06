using IDCA.Client.Common;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace IDCA.Client.ViewModel
{
    [ViewModelMark(SettingViewType.Overview, "IDCA.Client.View.OverviewControl", "IDCA.Client.View")]
    public class OverviewViewModel : ObservableObject
    {
        public OverviewViewModel()
        {
            _items = new ObservableCollection<OverviewInfo>();
        }

        TableSettingTreeNode? _node;
        public TableSettingTreeNode? Node
        {
            get { return _node; }
            set { SetProperty(ref _node, value); }
        }

        public class OverviewInfo : ObservableObject
        {
            public OverviewInfo()
            {
                _name = string.Empty;
                _description = string.Empty;
                _itemsCount = 0;
            }

            string _name;
            public string Name 
            {
                get { return _name; } 
                set { SetProperty(ref _name, value); }
            }

            string _description;
            public string Description
            {
                get { return _description; }
                set { SetProperty(ref _description, value); }
            }

            int _itemsCount;
            public int ItemsCount
            {
                get { return _itemsCount; }
                set { SetProperty(ref _itemsCount, value); }
            }
        }

        ObservableCollection<OverviewInfo> _items;
        public ObservableCollection<OverviewInfo> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        void AddNewOverviewInfo()
        {
            _items.Add(new OverviewInfo());
        }

    }
}
