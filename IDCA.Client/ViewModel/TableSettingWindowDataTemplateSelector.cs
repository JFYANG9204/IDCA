
using System.Windows;
using System.Windows.Controls;

namespace IDCA.Client.ViewModel
{
    public class TableSettingWindowDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate? TableSettingTemplate { get; set; }

        public DataTemplate? HeaderSettingTemplate { get; set; }

        public DataTemplate? OverViewTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TableSettingTreeNode node)
            {
                if (node.ViewModel is TableSettingViewModel && TableSettingTemplate != null)
                {
                    return TableSettingTemplate;
                }
                else if (node.ViewModel is HeaderSettingViewModel && HeaderSettingTemplate != null)
                {
                    return HeaderSettingTemplate;
                }
                else if (node.ViewModel is OverviewViewModel && OverViewTemplate != null)
                {
                    return OverViewTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }


    }
}
