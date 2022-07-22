
using IDCA.Client.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace IDCA.Client.View.Selector
{
    public class ComboBoxTextBoxDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate? ComboBoxTemplate { get; set; }
        public DataTemplate? TextBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is IValueSettingTemplate viewModel)
            {
                if (viewModel.ControlType == ValueSettingControlType.ComboBox && ComboBoxTemplate != null)
                {
                    return ComboBoxTemplate;
                }
                else if (viewModel.ControlType == ValueSettingControlType.TextBox && TextBoxTemplate != null)
                {
                    return TextBoxTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }


    }
}
