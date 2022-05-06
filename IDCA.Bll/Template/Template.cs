
using System;
using System.Text;

namespace IDCA.Model.Template
{
    /// <summary>
    /// 模板类型，标记模板的基础用途
    /// </summary>
    public enum TemplateType
    {
        None = 0,
        /// <summary>
        /// 文件模板
        /// </summary>
        File = 1,
        /// <summary>
        /// 函数模板
        /// </summary>
        Function = 3,
        /// <summary>
        /// 变量模板
        /// </summary>
        Field = 4,
        /// <summary>
        /// 将引用文件夹内的所有文件
        /// </summary>
        Folder = 99
    }

    /// <summary>
    /// 所有模板的基类，同时是虚类，无法被实例化，所有派生类都需要实现它的方法
    /// </summary>
    public abstract class Template : ICloneable
    {
        public Template(TemplateType type)
        {
            _type = type;
            _parameters = new(this);
        }

        protected Template(Template template)
        {
            _type = template.Type;
            _parameters = new(this);
        }

        protected readonly TemplateType _type;
        protected readonly TemplateParameters _parameters;

        /// <summary>
        /// 模板类型
        /// </summary>
        public TemplateType Type => _type;
        /// <summary>
        /// 模板参数集合
        /// </summary>
        internal TemplateParameters Parameters => _parameters;
        /// <summary>
        /// 按照当前的参数配置进行文本修改，返回最终结果
        /// </summary>
        /// <returns></returns>
        public abstract string Exec();
        /// <summary>
        /// 复制当前的模板对象并返回
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
        /// <summary>
        /// 将当前的模板参数复制进新的对象中
        /// </summary>
        /// <param name="template"></param>
        protected void Clone(Template template)
        {
            foreach (TemplateParameter parameter in _parameters)
            {
                template.Parameters.Add((TemplateParameter)parameter.Clone());
            }
        }

        /// <summary>
        /// 创建或者取得固定用途的参数
        /// </summary>
        /// <param name="usage"></param>
        /// <returns></returns>
        protected TemplateParameter GetOrCreateParameter(TemplateParameterUsage usage)
        {
            TemplateParameter? parameter = _parameters[usage];
            if (parameter == null)
            {
                parameter = _parameters.NewObject();
                parameter.Usage = usage;
                _parameters.Add(parameter);
            }
            return parameter;
        }

    }

    /// <summary>
    /// 文件类型标记枚举类，用来标记文件的用途
    /// </summary>
    public enum FileTemplateFlags
    {
        /// <summary>
        /// 默认值，创建文件时忽略此标记的模板
        /// </summary>
        None = 0,
        /// <summary>
        /// 配置文件，一般命名Job.ini
        /// </summary>
        JobFile = 1,
        /// <summary>
        /// MDM文档文本标签修改文件，一般命名MDD_Manipulation.mrs
        /// </summary>
        ManipulationFile = 2,
        /// <summary>
        /// 添加表格的文件模板
        /// </summary>
        TableFile = 3,
        /// <summary>
        /// 修改数据脚本的文件，一般命名OnNextCase.mrs
        /// </summary>
        OnNextCaseFile = 4,
        /// <summary>
        /// 直接新变量的文件，一般命名sbMetadata.mrs
        /// </summary>
        MetadataFile = 5,
        /// <summary>
        /// 新变量脚本文件，一般命名Metadata_DMS.mrs
        /// </summary>
        DmsMetadataFile = 6,
        /// <summary>
        /// 空文件
        /// </summary>
        EmptyFile = 7,
        /// <summary>
        /// 库文件，库文件内容不会被修改
        /// </summary>
        LibraryFile = 8,
        /// <summary>
        /// 其他有用的文件，允许被修改
        /// </summary>
        OtherUsefulFile = 9,
    }

    public class FileTemplate : Template
    {
        public FileTemplate() : base(TemplateType.File)
        {
            _directory = "";
            _fileName = "";
            _flag = FileTemplateFlags.None;
            _content = new();
        }

        protected FileTemplate(FileTemplate template) : base(template)
        {
            _directory = template.Directory;
            _fileName = template.FileName;
            _flag = FileTemplateFlags.None;
            _content = new();
        }

        string _directory;
        string _fileName;
        FileTemplateFlags _flag;
        protected StringBuilder _content;
        public Encoding _encoding = Encoding.GetEncoding("GB2312");

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string Directory { get => _directory; set => _directory = value; }
        /// <summary>
        /// 文件名，包含文件名和扩展名
        /// </summary>
        public string FileName { get => _fileName; set => _fileName = value; }
        /// <summary>
        /// 文件类型标记
        /// </summary>
        public FileTemplateFlags Flag { get => _flag; set => _flag = value; }
        /// <summary>
        /// 文件文本模板内容，所有需要替换的内容需要满足格式 ：$[variable]，变量名不区分大小写
        /// </summary>
        public string Content { get => _content.ToString(); }
        /// <summary>
        /// 当前文本的编码格式
        /// </summary>
        public Encoding Encoding { get => _encoding; set => _encoding = value; }

        /// <summary>
        /// 设置当前的文本内容，会清除之前已有的内容
        /// </summary>
        /// <param name="content"></param>
        public void SetContent(string content)
        {
            _content.Clear();
            _content.Append(content);
        }

        /// <summary>
        /// 向当前的内容后追加新的文本内容，会在之前添加新行
        /// </summary>
        /// <param name="value"></param>
        public void AppendLine(string value)
        {
            _content.AppendLine(value);
        }

        public override string Exec()
        {
            string result = _content.ToString();
            if (_parameters.Count > 0)
            {
                foreach (TemplateParameter parameter in Parameters)
                {
                    result = result.Replace(parameter.ToString(), parameter.GetValue<string>(), StringComparison.OrdinalIgnoreCase);
                }
            }
            return result;
        }

        public override object Clone()
        {
            var clone = new FileTemplate(this);
            Clone(clone);
            return clone;
        }
    }

    /// <summary>
    /// 函数模板类型标记
    /// </summary>
    public enum FunctionTemplateFlags
    {
        None = 0,

        ManipulateTitleLabel = 101,
        ManipulateSideLabel = 102,
        ManipulateSideAxis = 103,
        ManipulateTypeLabel = 104,
        ManipulateAxisAverage = 105,
        ManipulateAxisRebase = 106,

        TableNormal = 201,
        TableGrid = 202,
        TableGridSlice = 203,
        TableMeanSummary = 204,
        TableResponseSummary = 205,

    }

    public class FunctionTemplate : Template
    {
        public FunctionTemplate() : base(TemplateType.Function)
        {
            _flag = FunctionTemplateFlags.None;
        }

        public FunctionTemplate(FunctionTemplate template) : base(template)
        {
        }

        FunctionTemplateFlags _flag;
        /// <summary>
        /// 当前函数模板的用处标记
        /// </summary>
        public FunctionTemplateFlags Flag { get => _flag; set => _flag = value; }

        /// <summary>
        /// 设定函数模板的函数名
        /// </summary>
        /// <param name="functionName">函数名</param>
        public void SetFunctionName(string functionName)
        {
            TemplateParameter nameParam = GetOrCreateParameter(TemplateParameterUsage.FunctionName);
            nameParam.Name = "FunctionName";
            nameParam.SetValue(functionName);
        }

        /// <summary>
        /// 获取当前模板的函数名
        /// </summary>
        /// <returns></returns>
        public string GetFunctionName()
        {
            TemplateParameter? parameter = _parameters[TemplateParameterUsage.FunctionName];
            if (parameter != null)
            {
                string? functionName = parameter.GetValue<string>();
                return functionName is null ? string.Empty : functionName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 将新的函数参数模板添加进集合末尾
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public void PushFunctionParameter(string name, string value, TemplateValueType type, TemplateParameterUsage usage)
        {
            if (_parameters[usage] == null)
            {
                TemplateParameter parameter = _parameters.NewObject();
                parameter.Name = name;
                parameter.Usage = usage;
                parameter.SetValue(new TemplateValue(value, type));
                _parameters.Add(parameter);
            }
        }

        /// <summary>
        /// 设定当前模板中特定用途的参数的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="usage"></param>
        public void SetFunctionParameterValue(string value, TemplateValueType valueType, TemplateParameterUsage usage)
        {
            TemplateParameter? parameter = _parameters[usage];
            TemplateValue? paramValue;
            if (parameter != null && (paramValue = parameter.GetValue<TemplateValue>()) != null)
            {
                paramValue.Value = value;
                paramValue.ValueType = valueType;
            }
        }

        public override object Clone()
        {
            var clone = new FunctionTemplate(this);
            Clone(clone);
            return clone;
        }

        public override string Exec()
        {
            TemplateParameter? functionNameParam = _parameters[TemplateParameterUsage.FunctionName];
            string functionName = functionNameParam?.GetValue<string>() ?? string.Empty;
            StringBuilder result = new();
            result.Append(functionName);
            result.Append('(');
            int count = 0;
            _parameters.All(parameter =>
                {
                    TemplateValue? paramValue = parameter.GetValue<TemplateValue>();
                    if (paramValue != null)
                    {
                        if (count > 0)
                        {
                            result.Append(", ");
                        }
                        result.Append(paramValue.ToString());
                        count++;
                    }
                },
                parameter => (int)parameter.Usage > 1000 && (int)parameter.Usage % 10 == 2
            );
            result.Append(')');
            return result.ToString();
        }
    }

}
