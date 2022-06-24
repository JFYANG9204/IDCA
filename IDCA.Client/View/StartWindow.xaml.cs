
using IDCA.Client.ViewModel.Common;

namespace IDCA.Client.View
{
    /// <summary>
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : CustomWindow
    {
        public StartWindow()
        {
            InitializeComponent();
            WindowManager.Register<TableSettingWindow>("TableSettingWindow");
            WindowManager.Register<SettingWindow>("SettingWindow");
        }

    }


}
