
using IDCA.Client.ViewModel.Common;
using System.Windows;

namespace IDCA.Client.View
{
    /// <summary>
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            WindowManager.Register<TableSettingWindow>("TableSettingWindow");
        }

    }


}
