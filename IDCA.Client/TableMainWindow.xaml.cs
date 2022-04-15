
using System.Windows;
using System.Windows.Input;

namespace IDCA.Client
{
    /// <summary>
    /// TableMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TableMainWindow : Window
    {
        public TableMainWindow()
        {
            InitializeComponent();
            if (WindowState == WindowState.Maximized)
            {
                MaximizedImageVisibility = Visibility.Hidden;
                NormalImageVisibility = Visibility.Visible;
            }
            else
            {
                MaximizedImageVisibility = Visibility.Visible;
                NormalImageVisibility = Visibility.Hidden;
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void HeaderMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                MaximizedImageVisibility = Visibility.Hidden;
                NormalImageVisibility = Visibility.Visible;
            }
            else
            {
                WindowState = WindowState.Normal;
                MaximizedImageVisibility = Visibility.Visible;
                NormalImageVisibility = Visibility.Hidden;
            }
        }

        public readonly static DependencyProperty MaximizedVisibility =
            DependencyProperty.Register("MaximizedImageVisibility", typeof(Visibility), typeof(TableMainWindow));
        public readonly static DependencyProperty NormalVisbility =
            DependencyProperty.Register("NormalImageVisibility", typeof(Visibility), typeof(TableMainWindow));

        public Visibility MaximizedImageVisibility
        {
            get { return (Visibility)GetValue(MaximizedVisibility); }
            set { SetValue(MaximizedVisibility, value); }
        }

        public Visibility NormalImageVisibility
        {
            get { return (Visibility)GetValue(NormalVisbility); }
            set { SetValue(NormalVisbility, value); }
        }

    }
}
