using IDCA.Client.View.Helper;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace IDCA.Client.View.Behavior
{

    public static class DataGridDragDropRowBehavior
    {
        private static DataGrid? _dataGrid;
        private static Popup? _popup;

        private static bool _enable = false;

        private static object? _draggedItem;

        public static object? DraggedItem
        {
            get => _draggedItem;
            set => _draggedItem = value;
        }

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(DataGridDragDropRowBehavior), new UIPropertyMetadata(false, OnEnabledChanged));

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        private static void OnEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not bool)
            {
                throw new ArgumentException("Value should be of bool type");
            }
            _enable = (bool)e.NewValue;
        }

        public static readonly DependencyProperty PopupControlProperty =
            DependencyProperty.RegisterAttached("PopupControl", typeof(Popup), typeof(DataGridDragDropRowBehavior), new UIPropertyMetadata(null, OnPopupControlChanged));

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static Popup GetPopupControl(DependencyObject obj)
        {
            return (Popup)obj.GetValue(PopupControlProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static void SetPopupControl(DependencyObject obj, Popup value)
        {
            obj.SetValue(PopupControlProperty, value);
        }

        private static void OnPopupControlChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null || args.NewValue is not Popup)
            {
                throw new ArgumentException("PopupControl should be set");
            }

            _popup = args.NewValue as Popup;
            _dataGrid = obj as DataGrid;
            if (_dataGrid is null)
            {
                return;
            }

            if (_enable && _popup != null)
            {
                _dataGrid.BeginningEdit += OnBeginEdit;
                _dataGrid.CellEditEnding += OnEndEdit;
                _dataGrid.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                _dataGrid.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                _dataGrid.PreviewMouseMove += OnPreviewMouseMove;
            }
            else
            {
                _dataGrid.BeginningEdit -= OnBeginEdit;
                _dataGrid.CellEditEnding -= OnEndEdit;
                _dataGrid.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                _dataGrid.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
                _dataGrid.PreviewMouseMove -= OnPreviewMouseMove;

                _dataGrid = null;
                _popup = null;
                _draggedItem = null;
                IsEditing = false;
                IsDragging = false;
            }
        }

        public static bool IsEditing { get; set; }
        public static bool IsDragging { get; set; }

        /// <summary>
        /// 关闭Popup，设置DataGrid为只读
        /// </summary>
        private static void ResetDragDrop()
        {
            IsDragging = false;
            
            if (_popup != null)
            {
                _popup.IsOpen = false;
            }

            if (_dataGrid != null)
            {
                _dataGrid.IsReadOnly = false;
            }
        }


        private static void OnPreviewMouseMove(object? sender, MouseEventArgs e)
        {
            if (!IsDragging || e.LeftButton != MouseButtonState.Pressed || _dataGrid is null || _popup is null)
            {
                return;
            }

            if (_dataGrid.Cursor != Cursors.SizeAll)
            {
                _dataGrid.Cursor = Cursors.SizeAll;
            }

            _popup.DataContext = DraggedItem;

            if (!_popup.IsOpen)
            {
                _dataGrid.IsReadOnly = true;
                _popup.IsOpen = true;
            }

            var popupSize = new Size(_popup.ActualWidth, _popup.ActualHeight);
            _popup.PlacementRectangle = new Rect(e.GetPosition(_dataGrid), popupSize);

            var pos = e.GetPosition(_dataGrid);
            var row = UIHelper.TryFindFromPoint<DataGridRow>(_dataGrid, pos);
            if (row != null)
            {
                _dataGrid.SelectedItem = row.Item;
            }
        }

        private static void OnPreviewMouseLeftButtonUp(object? sender, MouseButtonEventArgs e)
        {
            if (!IsDragging || IsEditing || _dataGrid is null)
            {
                return;
            }

            _dataGrid.Cursor = Cursors.Arrow;
            var targetItem = _dataGrid.SelectedItem;

            if (targetItem != null || !ReferenceEquals(DraggedItem, targetItem))
            {
                if (_dataGrid.ItemsSource is IList list)
                {
                    var targetIndex = list.IndexOf(targetItem);
                    list.Remove(DraggedItem);
                    if (targetIndex > -1)
                    {
                        list.Insert(targetIndex, DraggedItem);
                    }
                    _dataGrid.SelectedItem = DraggedItem;
                }
            }

            ResetDragDrop();
        }

        private static void OnPreviewMouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
        {
            if (IsEditing || sender is null)
            {
                return;
            }

            var rowHeader = UIHelper.TryFindFromPoint<DataGridRowHeader>((UIElement)sender, e.GetPosition(_dataGrid));
            if (rowHeader == null)
            {
                return;
            }
            
            var row = UIHelper.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(_dataGrid));
            if (row == null || row.IsEditing)
            {
                return;
            }

            IsDragging = true;
            DraggedItem = row.Item;
        }

        private static void OnBeginEdit(object? sender, DataGridBeginningEditEventArgs e)
        {
            IsEditing = true;
            if (IsDragging)
            {
                ResetDragDrop();
            }
        }

        private static void OnEndEdit(object? sender, DataGridCellEditEndingEventArgs e)
        {
            IsEditing = false;
        }

    }
}
