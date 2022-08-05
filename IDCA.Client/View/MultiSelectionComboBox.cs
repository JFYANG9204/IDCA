
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace IDCA.Client.View
{
    public class MultiSelectionComboBox : ItemsControl
    {

        public const string PART_ToggleButton = "PART_ToggleButton";
        public const string PART_ItemListBox = "PART_ItemListBox";
        public const string PART_Popup = "PART_Popup";
        public const string PART_SelectedItemsPresenter = "PART_SelectedItemsPresenter";

        static MultiSelectionComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(typeof(MultiSelectionComboBox)));
        }

        private ListBox? _itemsListBox;

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem),
                typeof(object),
                typeof(MultiSelectionComboBox));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems),
                typeof(IList),
                typeof(MultiSelectionComboBox));

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex),
                typeof(int),
                typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndicesProperty = 
            DependencyProperty.Register(nameof(SelectedIndices),
                typeof(IList),
                typeof(MultiSelectionComboBox));

        public IList SelectedIndices
        {
            get { return (IList)GetValue(SelectedIndicesProperty); }
            set { SetValue(SelectedIndicesProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen),
                typeof(bool),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(false));

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Items Container
            _itemsListBox = GetTemplateChild(PART_ItemListBox) as ListBox;
            if (_itemsListBox != null)
            {
                _itemsListBox.SelectionChanged += OnItemsListBoxSelectionChanged;
                SyncSelectedItems(SelectedItems, _itemsListBox.SelectedItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                _itemsListBox.SelectionChanged -= OnItemsListBoxSelectionChanged;
            }

        }

        private void OnItemsListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReferenceEquals(_itemsListBox, e.OriginalSource))
            {
                e.Handled = true;
            }
        }

        public void UnselectAll()
        {
            _itemsListBox?.UnselectAll();
        }

        private void StartListeningForSelectionChanges()
        {
            if (_itemsListBox?.SelectedItems is INotifyCollectionChanged selectedItemsCollection)
            {
                selectedItemsCollection.CollectionChanged += OnItemsListBoxSelectedItemsCollectionChanged;
            }
            if (SelectedItems is INotifyCollectionChanged selectedItems)
            {
                selectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
        }

        private void StopListeningForSelectionChanges()
        {
            if (_itemsListBox?.SelectedItems is INotifyCollectionChanged selectedItemsCollection)
            {
                selectedItemsCollection.CollectionChanged -= OnItemsListBoxSelectedItemsCollectionChanged;
            }
            if (SelectedItems is INotifyCollectionChanged selectedItems)
            {
                selectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
            }
        }

        private void OnItemsListBoxSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SyncSelectedItems(_itemsListBox?.SelectedItems, SelectedItems, e);
        }

        private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SyncSelectedItems(sender as IList, _itemsListBox?.SelectedItems, e);
        }

        private void SyncSelectedItems(IList? sourceCollection, IList? targetCollection, NotifyCollectionChangedEventArgs e)
        {
            if (sourceCollection is null || targetCollection is null || !IsInitialized)
            {
                return;
            }

            StopListeningForSelectionChanges();

            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems is not null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                targetCollection.Add(item);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems is not null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                targetCollection.Remove(item);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (e.OldItems is not null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                targetCollection.Remove(item);
                            }
                        }
                        if (e.NewItems is not null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                targetCollection.Add(item);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        if (e.OldItems is not null)
                        {
                            for (int i = 0; i < e.OldItems.Count; i++)
                            {
                                targetCollection.RemoveAt(e.OldStartingIndex);
                            }
                        }
                        if (e.NewItems is not null)
                        {
                            for (int i = 0; i < e.NewItems.Count; i++)
                            {
                                var insertPoint = e.NewStartingIndex + i;
                                if (insertPoint > targetCollection.Count)
                                {
                                    targetCollection.Add(e.NewItems[i]);
                                }
                                else
                                {
                                    targetCollection.Insert(insertPoint, e.NewItems[i]);
                                }
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        targetCollection.Clear();
                        foreach (var item in sourceCollection)
                        {
                            targetCollection.Add(item);
                        }
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                StartListeningForSelectionChanges();
            }

        }


    }
}
