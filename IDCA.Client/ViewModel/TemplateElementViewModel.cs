
using CommunityToolkit.Mvvm.ComponentModel;
using IDCA.Model.Template;

namespace IDCA.Client.ViewModel
{
    public class TemplateElementViewModel : ObservableObject
    {
        public TemplateElementViewModel(TemplateCollection templateCollection) 
        {
            _template = templateCollection;
            _templateName = templateCollection.Name;
            _templateDescription = templateCollection.Description;
        }

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

        readonly TemplateCollection _template;
        /// <summary>
        /// 此元素对应的模板对象
        /// </summary>
        public TemplateCollection Template => _template;

    }
}
