using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace IDCA.Client.ViewModel.Common
{
    public class MainWindowBehavior : Behavior<Window>
    {

        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(bool), typeof(MainWindowBehavior), new PropertyMetadata(false, OnCloseChanged));

        public bool Close
        {
            get { return (bool)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        static void OnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = ((MainWindowBehavior)d).AssociatedObject;
            var newValue = (bool)e.NewValue;
            if (newValue)
            {
                window.Close();
            }
        }
    }
}
