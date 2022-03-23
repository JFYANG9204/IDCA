

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

        readonly List<FileTemplate> _libraryFileTemplates = new();
        readonly List<FileTemplate> _otherUsefulFileTemplates = new();
        readonly Dictionary<FileTemplateFlags, Template> _fileTemplates = new();
        readonly Dictionary<FunctionTemplateFlags, Template> _functionTemplates = new();
        readonly Dictionary<ScriptTemplateFlags, Template> _scriptTemplates = new();

        public void Load(string xmlPath)
        {
            _path = Path.GetDirectoryName(xmlPath) ?? string.Empty;
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
            param.SetValue(TryReadStringValue(element.Attribute("default")));
            parameters.Add(param);
        }

        void LoadTemplate(XElement element)
        {
            TemplateType type = TryReadEnumValue<TemplateType>(element.Attribute("type"));
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

                case TemplateType.Script:
                    {
                        ScriptTemplateFlags scriptFlag = TryReadEnumValue<ScriptTemplateFlags>(element.Attribute("flag"));
                        template = new ScriptTemplate
                        {
                            IndentLevel = TryReadIntValue(element.Attribute("indent")),
                            Flag = scriptFlag
                        };
                        SetTemplateValue(template, scriptFlag, _scriptTemplates);
                        break;
                    }

                case TemplateType.Function:
                    {
                        FunctionTemplateFlags functionFlag = TryReadEnumValue<FunctionTemplateFlags>(element.Attribute("flag"));
                        template = new FunctionTemplate { Flag = functionFlag };
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
                                FileTemplate fileTemplate = new();
                                fileTemplate.FileName = Path.GetFileName(file);
                                fileTemplate.Directory = Path.GetDirectoryName(file) ?? string.Empty;
                                fileTemplate.Flag = FileTemplateFlags.LibraryFile;
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
                LoadParameter(template.Parameters, param);
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
                return _fileTemplates.ContainsKey(fileFlag) ? (T)_fileTemplates[fileFlag] : null;
            }
            else if (flag is FunctionTemplateFlags functionFlag && type.Equals(typeof(FunctionTemplate)))
            {
                return _functionTemplates.ContainsKey(functionFlag) ? (T)_functionTemplates[functionFlag] : null;
            }
            else if (flag is ScriptTemplateFlags scriptFlag && type.Equals(typeof(ScriptTemplate)))
            {
                return _scriptTemplates.ContainsKey(scriptFlag) ? (T)_scriptTemplates[scriptFlag] : null;
            }
            return null;
        }

    }
}
