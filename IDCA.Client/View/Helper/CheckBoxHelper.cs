
using System.Windows;

namespace IDCA.Client.View.Helper
{
    public static class CheckBoxHelper
    {

        public static readonly DependencyProperty CheckBoxSizeProperty =
            DependencyProperty.RegisterAttached(
                "CheckBoxSize",
                typeof(double),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(18d));

        public static double GetCheckBoxSize(DependencyObject d)
        {
            return (double)d.GetValue(CheckBoxSizeProperty);
        }

        public static void SetCheckBoxSize(DependencyObject d, double value)
        {
            d.SetValue(CheckBoxSizeProperty, value);
        }


        public static readonly DependencyProperty CheckBoxCornerRadiusProperty =
            DependencyProperty.RegisterAttached(
                "CheckBoxCornerRadius",
                typeof(CornerRadius),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static CornerRadius GetCheckBoxCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(CheckBoxCornerRadiusProperty);
        }

        public static void SetCheckBoxCornerRadius(DependencyObject d, CornerRadius value)
        {
            d.SetValue(CheckBoxCornerRadiusProperty, value);
        }



    }
}
