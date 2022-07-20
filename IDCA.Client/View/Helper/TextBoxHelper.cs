
using System.Windows;
using System.Windows.Input;

namespace IDCA.Client.View.Helper
{
    public static class TextBoxHelper
    {

        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.RegisterAttached(
                "ButtonWidth",
                typeof(double),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(22d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        public static double GetButtonWidth(DependencyObject d)
        {
            return (double)d.GetValue(ButtonWidthProperty);
        }

        public static void SetButtonWidth(DependencyObject d, double value)
        {
            d.SetValue(ButtonWidthProperty, value);
        }


        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.RegisterAttached(
                "ButtonCommand",
                typeof(ICommand),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null));

        public static ICommand? GetButtonCommand(DependencyObject d)
        {
            return (ICommand?)d.GetValue(ButtonCommandProperty);
        }

        public static void SetButtonCommand(DependencyObject d, ICommand? value)
        {
            d.SetValue(ButtonCommandProperty, value);
        }

    }
}
