using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace IDCA.Client.ViewModel
{
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

        public struct OverviewInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int ItemsCount { get; set; }
        }

        ObservableCollection<OverviewInfo> _items;
        public ObservableCollection<OverviewInfo> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

    }
}
