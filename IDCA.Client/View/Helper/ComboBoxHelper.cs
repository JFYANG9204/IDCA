
using System.Windows;
using System.Windows.Media;

namespace IDCA.Client.View.Helper
{
    public static class ComboBoxHelper
    {

        public static readonly DependencyProperty ItemBorderThicknessProperty =
            DependencyProperty.RegisterAttached(
                "ItemBorderThickness",
                typeof(Thickness),
                typeof(ComboBoxHelper),
                new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetItemBorderThickness(DependencyObject d)
        {
            return (Thickness)d.GetValue(ItemBorderThicknessProperty);
        }

        public static void SetItemBorderThickness(DependencyObject d, Thickness value)
        {
            d.SetValue(ItemBorderThicknessProperty, value);
        }


        public static readonly DependencyProperty ItemCornerRadiusProperty =
            DependencyProperty.RegisterAttached(
                "ItemCornerRadius",
                typeof(CornerRadius),
                typeof(ComboBoxHelper),
                new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static CornerRadius GetItemCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(ItemCornerRadiusProperty);
        }

        public static void SetItemCornerRadius(DependencyObject d, CornerRadius value)
        {
            d.SetValue(ItemCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty ItemBorderBrushProperty =
            DependencyProperty.RegisterAttached(
                "ItemBorderBrush",
                typeof(Brush),
                typeof(ComboBoxHelper),
                new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static Brush GetItemBorderBrush(DependencyObject d)
        {
            return (Brush)d.GetValue(ItemBorderBrushProperty);
        }

        public static void SetItemBorderBrush(DependencyObject d, Brush value)
        {
            d.SetValue(ItemBorderBrushProperty, value);
        }

    }
}
