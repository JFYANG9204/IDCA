
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace IDCA.Client.ViewModel
{
    public class GuidStartPageViewModel : ObservableObject
    {
        public GuidStartPageViewModel() 
        {
            _templateItems = new ObservableCollection<TemplateElementViewModel>
            {
                new TemplateElementViewModel
                {
                    TemplateName = "默认模板",
                    TemplateDescription = "普通的多期模板"
                }
            };
        }

        ObservableCollection<TemplateElementViewModel> _templateItems;
        public ObservableCollection<TemplateElementViewModel> TemplateItems
        {
            get { return _templateItems; }
            set { SetProperty(ref _templateItems, value); }
        }
    }
}
