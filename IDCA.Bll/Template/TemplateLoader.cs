

using System.Collections.Generic;

namespace IDCA.Bll.Template
{
    public class TemplateLoader
    {
        public TemplateLoader()
        {
            _path = string.Empty;
        }

        string _path;
        readonly Dictionary<string, Template> _tableTemplates = new();
        readonly Dictionary<string, Template> _functionTemplates = new();

        public void Load(string xmlPath)
        {
            _path = xmlPath;
        }




    }
}
