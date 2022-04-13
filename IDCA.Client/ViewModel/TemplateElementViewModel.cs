
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace IDCA.Client.ViewModel
{
    public class TemplateElementViewModel : ObservableObject
    {
        public TemplateElementViewModel() { }

        string _templateName = string.Empty;
        public string TemplateName
        {
            get { return _templateName; }
            set { SetProperty(ref _templateName, value); }
        }

        string _templateDescription = string.Empty;
        public string TemplateDescription
        {
            get { return _templateDescription; }
            set { SetProperty(ref _templateDescription, value); }
        }

    }
}
