using System;
using System.Windows;
using System.Windows.Input;

namespace IDCA.Client.View
{
    [TemplatePart(Name = PART_CloseButton, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_MinimizeButton, Type = typeof(UIElement))]
    public class CustomWindow : Window
    {
        public const string PART_CloseButton = "PART_CloseButton";
        public const string PART_MinimizeButton = "PART_MinimizeButton";

        public static readonly DependencyProperty RightTitleBarButtonProperty = DependencyProperty.Register(nameof(RightTitleBarButton), typeof(ICommand), typeof(CustomWindow));

        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            DefaultStyleKey = typeof(CustomWindow);
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseButtonClicked));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeButtonClicked, CanMinimizeWindow));
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs args)
        {
            Close();
        }

        public void OnMinimizeButtonClicked(object sender, RoutedEventArgs args)
        {
            SystemCommands.MinimizeWindow(this);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
            {
                InvalidateMeasure();
            }
        }

        private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        public UIElement RightTitleBarButton
        {
            get { return (UIElement)GetValue(RightTitleBarButtonProperty); }
            set { SetValue(RightTitleBarButtonProperty, value); }
        }

    }
}
