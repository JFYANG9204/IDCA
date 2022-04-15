
using IDCA.Client.Singleton;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace IDCA.Client.ViewModel
{
    public class GuidStartPageViewModel : ObservableObject
    {
        public GuidStartPageViewModel() 
        {
            _templateItems = new ObservableCollection<TemplateElementViewModel>();
            var template = new TemplateElementViewModel
            {
                TemplateName = "默认模板",
                TemplateDescription = "普通的多期模板"
            };
            _templateItems.Add(template);
        }

        ObservableCollection<TemplateElementViewModel> _templateItems;
        public ObservableCollection<TemplateElementViewModel> TemplateItems
        {
            get { return _templateItems; }
            set { SetProperty(ref _templateItems, value); }
        }

        int _templateSelectedIndex = GlobalConfig.Instance.TemplateSelectIndex;
        public int TemplateSelectedIndex
        {
            get { return _templateSelectedIndex; }
            set
            {
                SetProperty(ref _templateSelectedIndex, value);
                GlobalConfig.Instance.TemplateSelectIndex = value;
            }
        }

    }
}
