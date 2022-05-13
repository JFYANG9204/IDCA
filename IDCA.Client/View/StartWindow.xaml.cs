
using IDCA.Client.ViewModel.Common;
using MahApps.Metro.Controls;

namespace IDCA.Client.View
{
    /// <summary>
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : MetroWindow
    {
        public StartWindow()
        {
            InitializeComponent();
            WindowManager.Register<TableSettingWindow>("TableSettingWindow");
        }

    }


}
