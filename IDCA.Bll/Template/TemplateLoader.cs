

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace IDCA.Bll.Template
{

    // 模板由XML文件定义
    //
    // <!-- 无参数模板 -->
    // <template name="" usage="" directory="" filename="" />
    //
    // <!-- 含参数模板 -->
    // <template name="" usage="" type="" >
    //      <params>
    //          <param name="" valuetype="" usage="" default="" />
    //      </params>
    // </template>
    //
    //

    public class TemplateLoader
    {
        public TemplateLoader()
        {
            _path = string.Empty;
        }

        string _path;
        readonly Dictionary<TemplateUsage, Template> _fileTemplates = new();
        readonly Dictionary<TemplateUsage, Template> _tableTemplates = new();
        readonly Dictionary<TemplateUsage, Template> _functionTemplates = new();
        readonly Dictionary<TemplateUsage, Template> _scriptTemplates = new();

        public void Load(string xmlPath)
        {
            _path = xmlPath;
            XDocument xml = XDocument.Load(xmlPath);
            XElement? root = xml.Root;

            if (root is null)
            {
                throw new Exception("XML文件格式不正确，读取失败。");
            }

            foreach (XElement element in root.Elements("template"))
            {
                LoadTemplate(element);
            }
        }

        static string TryReadTextFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return string.Empty;
        }

        static string TryReadStringValue(XAttribute? attribute)
        {
            return attribute is null ? string.Empty : attribute.Value;
        }

        static int TryReadIntValue(XAttribute? attribute)
        {
            return (attribute != null && int.TryParse(attribute.Value, out int value)) ? value : 0;
        }

        static T TryReadEnumValue<T>(XAttribute? attribute)
        {
            return (attribute != null && int.TryParse(attribute.Value, out var value)) ? (T)(object)value : (T)(object)0;
        }

        static void SetTemplateValue(Template value, TemplateUsage usage, Dictionary<TemplateUsage, Template> collection)
        {
            if (collection.ContainsKey(usage))
            {
                collection[usage] = value;
            }
            else
            {
                collection.Add(usage, value);
            }
        }

        static void LoadParameter(TemplateParameters parameters, XElement? element)
        {
            if (element is null)
            {
                return;
            }
            var param = parameters.NewObject();
            param.Name = TryReadStringValue(element.Attribute("name"));
            param.Usage = TryReadEnumValue<TemplateParameterUsage>(element.Attribute("usage"));
            param.SetValue(TryReadStringValue(element.Attribute("default")));
            parameters.Add(param);
        }

        void LoadTemplate(XElement element)
        {
            TemplateType type = TryReadEnumValue<TemplateType>(element.Attribute("type"));
            TemplateUsage usage = TryReadEnumValue<TemplateUsage>(element.Attribute("usage"));

            Template? template = null;
            switch (type)
            {
                case TemplateType.File:
                    {
                        string fileName, directory;
                        template = new FileTemplate
                        {
                            Directory = directory = TryReadStringValue(element.Attribute("directory")),
                            FileName = fileName = TryReadStringValue(element.Attribute("filename")),
                            Content = TryReadTextFile(Path.Combine(_path, directory, fileName))
                        };
                        SetTemplateValue(template, usage, _fileTemplates);
                        break;
                    }

                case TemplateType.Script:
                    {
                        template = new ScriptTemplate
                        {
                            IndentLevel = TryReadIntValue(element.Attribute("indent"))
                        };
                        SetTemplateValue(template, usage, _scriptTemplates);
                        break;
                    }

                case TemplateType.Function:
                    template = new FunctionTemplate();
                    SetTemplateValue(template, usage, _functionTemplates);
                    break;

                default:
                    break;
            }

            if (template == null)
            {
                return;
            }

            template.Usage = usage;
            LoadParameter(template.Parameters, element.Element("params"));
        }

    }
}
