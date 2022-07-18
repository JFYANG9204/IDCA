
using System.Windows;
using System.Windows.Input;

namespace IDCA.Client.View.Helper
{
    public static class TextBoxHelper
    {

        public static readonly DependencyProperty ClearTextButtonProperty =
            DependencyProperty.RegisterAttached(
                "ClearTextButton",
                typeof(bool),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata());

        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }


        public static void SetClearTextButton(DependencyObject d, bool value)
        {
            d.SetValue(ClearTextButtonProperty, value);
        }

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
                typeof(TextBoxHelper));

        public static ICommand? GetButtonCommand(UIElement element)
        {
            return (ICommand?)element.GetValue(ButtonCommandProperty);
        }

        public static void SetButtonCommand(UIElement element, ICommand? value)
        {
            element.SetValue(ButtonCommandProperty, value);
        }

        public static readonly DependencyProperty ButtonCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "ButtonCommandParameter",
                typeof(object),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null));

        public static object GetButtonCommandParameter(DependencyObject d)
        {
            return d.GetValue(ButtonCommandParameterProperty);
        }

        public static void SetButtonCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(ButtonCommandParameterProperty, value);
        }




    }
}
