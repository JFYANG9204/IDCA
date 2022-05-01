using System.Windows;
using System.Windows.Input;

namespace IDCA.Client.Dialog
{
    /// <summary>
    /// SpecAxisSettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SpecAxisSettingDialog : Window
    {
        public SpecAxisSettingDialog()
        {
            InitializeComponent();
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
    }
}
