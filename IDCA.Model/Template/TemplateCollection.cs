

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace IDCA.Model.Template
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

    public class TemplateCollection
    {
        public TemplateCollection()
        {
            _path = string.Empty;
            _name = string.Empty;
            _id = string.Empty;
            _description = string.Empty;

            _libraryFileTemplates = new List<FileTemplate>();
            _otherUsefulFileTemplates = new List<FileTemplate>();
            _fileTemplates = new Dictionary<FileTemplateFlags, Template>();
            _functionTemplates = new Dictionary<FunctionTemplateFlags, Template>();
        }

        string _path;
        string _name;
        string _id;
        string _description;

        readonly List<FileTemplate> _libraryFileTemplates;
        readonly List<FileTemplate> _otherUsefulFileTemplates;
        readonly Dictionary<FileTemplateFlags, Template> _fileTemplates;
        readonly Dictionary<FunctionTemplateFlags, Template> _functionTemplates;

        /// <summary>
        /// Template名称
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// Template编号
        /// </summary>
        public string Id { get => _id; set => _id = value; }
        /// <summary>
        /// Template模板描述
        /// </summary>
        public string Description { get => _description; set => _description = value; }
        /// <summary>
        /// Template的XML描述文件路径
        /// </summary>
        public string XmlPath { get => _path; }

        /// <summary>
        /// 当前模板集合中的Library部分文件模板
        /// </summary>
        public List<FileTemplate> Library => _libraryFileTemplates;
        /// <summary>
        /// 当前模板集合中的所有其他类型文件模板
        /// </summary>
        public List<FileTemplate> OtherUsefulFile => _otherUsefulFileTemplates;

        /// <summary>
        /// 根据XML文件配置载入当前模板配置
        /// </summary>
        /// <param name="xmlPath">模板内的XML文件路径</param>
        /// <exception cref="Exception">XML文件格式错误时抛出读取错误</exception>
        public void Load(string xmlPath)
        {
            _path = Path.GetDirectoryName(xmlPath) ?? string.Empty;
            XDocument xml = XDocument.Load(xmlPath);
            XElement? root = xml.Root;

            if (root is null)
            {
                throw new Exception("XML文件格式不正确，读取失败。");
            }

            XElement? properties = root.Element("properties");
            if (properties != null)
            {
                foreach (XElement element in properties.Elements("property"))
                {
                    string name = TryReadStringValue(element.Attribute("name"));
                    string value = TryReadStringValue(element.Attribute("value"));
                    if (name.Equals("Name"))
                    {
                        _name = value;
                    }
                    else if (name.Equals("Description"))
                    {
                        _description = value;
                    }
                }
            }

            LoadNodeElements(root, "file", TemplateType.File);
            LoadNodeElements(root, "folder", TemplateType.Folder);
            LoadNodeElements(root, "function", TemplateType.Function);
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

        static T TryReadEnumValue<T>(XAttribute? attribute)
        {
            return (attribute != null && int.TryParse(attribute.Value, out var value)) ? (T)(object)value : (T)(object)0;
        }

        static void SetTemplateValue<T>(Template value, T usage, Dictionary<T, Template> collection) where T : Enum
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
            var valueType = TryReadEnumValue<TemplateValueType>(element.Attribute("valuetype"));
            param.SetValue(TryReadStringValue(element.Attribute("default")), valueType);
            parameters.Add(param);
        }

        void LoadNodeElements(XElement root, string tagName, TemplateType type)
        {
            XElement? node = root.Element(tagName);
            if (node != null)
            {
                foreach (XElement element in node.Elements("template"))
                {
                    LoadTemplate(element, type);
                }
            }
        }

        void LoadTemplate(XElement element, TemplateType type)
        {
            Template? template = null;
            switch (type)
            {
                case TemplateType.File:
                    {
                        string fileName, directory;
                        FileTemplateFlags fileFlag = TryReadEnumValue<FileTemplateFlags>(element.Attribute("flag"));
                        template = new FileTemplate
                        {
                            Directory = directory = TryReadStringValue(element.Attribute("path")),
                            FileName = fileName = TryReadStringValue(element.Attribute("filename")),
                            Flag = fileFlag
                        };
                        ((FileTemplate)template).SetContent(TryReadTextFile(Path.Combine(_path, directory, fileName)));

                        if (fileFlag == FileTemplateFlags.LibraryFile)
                        {
                            _libraryFileTemplates.Add((FileTemplate)template);
                        }
                        else if (fileFlag == FileTemplateFlags.OtherUsefulFile)
                        {
                            _otherUsefulFileTemplates.Add((FileTemplate)template);
                        }
                        else
                        {
                            SetTemplateValue(template, fileFlag, _fileTemplates);
                        }
                        break;
                    }

                case TemplateType.Function:
                    {
                        FunctionTemplateFlags functionFlag = TryReadEnumValue<FunctionTemplateFlags>(element.Attribute("flag"));
                        template = new FunctionTemplate { Flag = functionFlag };
                        ((FunctionTemplate)template).SetFunctionName(TryReadStringValue(element.Attribute("name")));
                        SetTemplateValue(template, functionFlag, _functionTemplates);
                        break;
                    }

                case TemplateType.Folder:
                    {
                        string path = TryReadStringValue(element.Attribute("path"));
                        string fullPath = Path.Combine(_path, path);
                        if (Directory.Exists(fullPath))
                        {
                            foreach (var file in Directory.GetFiles(fullPath))
                            {
                                var fileTemplate = new FileTemplate
                                {
                                    FileName = Path.GetFileName(file),
                                    Directory = path,
                                    Flag = FileTemplateFlags.LibraryFile
                                };
                                fileTemplate.SetContent(TryReadTextFile(file));
                                _libraryFileTemplates.Add(fileTemplate);
                            }
                        }
                        break;
                    }

                default:
                    break;
            }

            if (template == null)
            {
                return;
            }

            foreach (XElement param in element.Elements("param"))
            {
                if (type == TemplateType.Function)
                {
                    ((FunctionTemplate)template).PushFunctionParameter(
                        TryReadStringValue(param.Attribute("name")), 
                        TryReadStringValue(param.Attribute("default")), 
                        TryReadEnumValue<TemplateValueType>(param.Attribute("valuetype")),
                        TryReadEnumValue<TemplateParameterUsage>(param.Attribute("usage")));
                }
                else
                {
                    LoadParameter(template.Parameters, param);
                }
            }
        }

        /// <summary>
        /// 获取指定类型的模板，如果未设置，返回null
        /// </summary>
        /// <typeparam name="T">模板类型</typeparam>
        /// <typeparam name="Flag">模板标记类型</typeparam>
        /// <param name="flag">需要查找的模板标记类型</param>
        /// <returns>指定类型和标记的模板对象，未找到时返回null</returns>
        public T? TryGet<T, Flag>(Flag flag) where T: Template
        {
            Type type = typeof(T);
            if (flag is FileTemplateFlags fileFlag && type.Equals(typeof(FileTemplate)))
            {
                return _fileTemplates.ContainsKey(fileFlag) ? (T)_fileTemplates[fileFlag].Clone() : null;
            }
            else if (flag is FunctionTemplateFlags functionFlag && type.Equals(typeof(FunctionTemplate)))
            {
                return _functionTemplates.ContainsKey(functionFlag) ? (T)_functionTemplates[functionFlag].Clone() : null;
            }
            return null;
        }

        /// <summary>
        /// 更新所有模板中指定用途的参数值
        /// </summary>
        /// <typeparam name="Flag">模板类型枚举，可以是FileTemplateFlags或FunctionTemplateFlags</typeparam>
        /// <param name="flag">模板类型</param>
        /// <param name="usage">参数类型</param>
        /// <param name="value">参数值</param>
        public void SetParameter<Flag>(Flag flag, TemplateParameterUsage usage, string value, TemplateValueType valueType = TemplateValueType.String)
        {
            if (flag is FileTemplateFlags fileFlage)
            {
                if (_fileTemplates.ContainsKey(fileFlage))
                {
                    _fileTemplates[fileFlage]?.Parameters[usage]?.SetValue(value, valueType);
                }
            }
            else if (flag is FunctionTemplateFlags functionFlag)
            {
                if (_functionTemplates.ContainsKey(functionFlag))
                {
                    _functionTemplates[functionFlag]?.Parameters[usage]?.SetValue(value, valueType);
                }
            }
        }

    }
}
