using IDCA.Client.Dialog;
using IDCA.Client.ViewModel.Common;
using System.Windows.Controls;

namespace IDCA.Client.Control
{
    /// <summary>
    /// SpecSettingTable.xaml 的交互逻辑
    /// </summary>
    public partial class SpecSettingTable : UserControl
    {
        public SpecSettingTable()
        {
            InitializeComponent();
            WindowManager.Register<SpecAxisSettingDialog>("SpecAxisSettingDialog");
        }

    }
}
