
using IDCA.Client.ViewModel.Common;
using MahApps.Metro.Controls;

namespace IDCA.Client.View
{
    /// <summary>
    /// TableSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TableSettingWindow : MetroWindow
    {
        public TableSettingWindow()
        {
            InitializeComponent();
            WindowManager.Register<AxisSettingWindow>("AxisSettingWindow");
            WindowManager.Register<VariableSettingWindow>("VariableSettingWindow");
        }

    }
}
