
using System.Windows;
using System.Windows.Controls;

namespace IDCA.Client.View.Helper
{
    public static class DataGridHelper
    {

        public static readonly DependencyProperty ColumnHeaderPaddingProperty =
            DependencyProperty.RegisterAttached(
                "ColumnHeaderPadding",
                typeof(Thickness),
                typeof(DataGridHelper),
                new FrameworkPropertyMetadata(new Thickness(10, 0, 4, 0), FrameworkPropertyMetadataOptions.Inherits));

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static Thickness GetColumnHeaderPadding(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ColumnHeaderPaddingProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static void SetColumnHeaderPadding(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ColumnHeaderPaddingProperty, value);
        }


        public static readonly DependencyProperty CellPaddingProperty =
            DependencyProperty.RegisterAttached(
                "CellPadding",
                typeof(Thickness),
                typeof(DataGridHelper),
                new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.Inherits));

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        [AttachedPropertyBrowsableForType(typeof(DataGridRow))]
        public static Thickness GetCellPadding(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(CellPaddingProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        [AttachedPropertyBrowsableForType(typeof(DataGridRow))]
        public static void SetCellPadding(DependencyObject obj, Thickness value)
        {
            obj.SetValue(CellPaddingProperty, value);
        }
    }
}
