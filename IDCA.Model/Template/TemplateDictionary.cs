
using System.Collections.Generic;
using System.IO;

namespace IDCA.Model.Template
{
    public class TemplateDictionary
    {
        public TemplateDictionary()
        {
            _templates = new Dictionary<string, TemplateCollection>();
        }

        readonly Dictionary<string, TemplateCollection> _templates;

        /// <summary>
        /// 尝试通过ID编号获取对应模板集合对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TemplateCollection? TryGet(string id)
        {
            return _templates.ContainsKey(id) ? _templates[id] : null;
        }

        /// <summary>
        /// 获取当前集合中所有模板集合组成的数组
        /// </summary>
        /// <returns></returns>
        public TemplateCollection[] GetTemplates()
        {
            TemplateCollection[] templates = new TemplateCollection[_templates.Count];
            int i = 0;
            foreach (var template in _templates.Values)
            {
                templates[i++] = template;
            }
            return templates;
        }

        /// <summary>
        /// 从文件夹载入其中所有符合规则的模板配置信息
        /// </summary>
        /// <param name="folderPath"></param>
        public void LoadFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Logger.Error("TemplateRootFolderIsNotExist", ExceptionMessages.TemplateRootFolderIsNotExist, folderPath);
                return;
            }
            string[] templates = Directory.GetDirectories(folderPath);
            foreach (string template in templates)
            {
                string xmlPath = Path.Combine(template, "Template.xml");
                if (!File.Exists(xmlPath))
                {
                    Logger.Warning("TemplateDefinitionXmlFileIsNotExist", ExceptionMessages.TemplateDefinitionXmlFileIsNotExist, template);
                    continue;
                }
                var templateCollection = new TemplateCollection();
                templateCollection.Load(xmlPath);
                string id = StringHelper.ConvertToHexString(templateCollection.Name);
                templateCollection.Id = id;
                if (string.IsNullOrEmpty(id))
                {
                    Logger.Warning("TemplateNameCannotBeEmpty", ExceptionMessages.TemplateNameCannotBeEmpty, template);
                    continue;
                }
                if (_templates.ContainsKey(id))
                {
                    _templates[id] = templateCollection;
                }
                else
                {
                    _templates.Add(id, templateCollection);
                }
            }
        }

        /// <summary>
        /// 清空集合中的所有模板
        /// </summary>
        public void Clear()
        {
            _templates.Clear();
        }

    }
}
