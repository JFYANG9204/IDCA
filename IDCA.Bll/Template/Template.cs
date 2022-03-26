
using System;
using System.Text;

namespace IDCA.Bll.Template
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
        /// 脚本模板
        /// </summary>
        Script = 2,
        /// <summary>
        /// 函数模板
        /// </summary>
        Function = 3,
        /// <summary>
        /// 变量模板
        /// </summary>
        Field = 4,
        /// <summary>
        /// 函数调用表达式模板
        /// </summary>
        Call = 5,
        /// <summary>
        /// 声明变量名模板
        /// </summary>
        Declare = 6,
        /// <summary>
        /// 将引用文件夹内的所有文件
        /// </summary>
        Folder = 99
    }

    /// <summary>
    /// 模板值类型，会影响文本替换的结果
    /// </summary>
    public enum TemplateValueType
    {
        /// <summary>
        /// 字符串类型，替换后会被双引号包裹
        /// </summary>
        String,
        /// <summary>
        /// 变量名，不做修改
        /// </summary>
        Variable,
        /// <summary>
        /// 分类变量值类型，替换后会被花括号包裹
        /// </summary>
        Categorical,
        /// <summary>
        /// 数值，不做修改
        /// </summary>
        Number,
        /// <summary>
        /// 任意表达式，不做修改
        /// </summary>
        Expression,
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
        public TemplateParameters Parameters => _parameters;
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

        /// <summary>
        /// 如果特定用途参数的值是参数集合，尝试向参数列表的末尾添加新的元素
        /// </summary>
        /// <param name="value">添加的参数值</param>
        /// <param name="usage">需要先其中添加的参数类型</param>
        /// <param name="parameterUsage">需要添加参数的参数类型</param>
        protected void TryPushParameter(object value, TemplateParameterUsage usage, TemplateParameterUsage parameterUsage)
        {
            TemplateParameter? parameter = _parameters[usage];
            if (parameter is null)
            {
                parameter = _parameters.NewObject();
                parameter.Usage = usage;
                parameter.SetValue(new TemplateParameters(_parameters.Template));
                _parameters.Add(parameter);
            }
            parameter.TryPushParam(value, parameterUsage);
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
        public void Append(string value)
        {
            _content.AppendLine();
            _content.Append(value);
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

        ManipulateTitleLabel,
        ManipulateSideLabel,
        ManipulateSideAxis,
        ManipulateTypeLabel,

        TableNormal,
        TableGrid,
        TableGridSlice,
        TableMeanSummary,
        TableResponseSummary,

    }

    public class FunctionTemplate : Template
    {
        public FunctionTemplate() : base(TemplateType.Function)
        {
            _flag = FunctionTemplateFlags.None;
        }

        public FunctionTemplate(FunctionTemplate template) : base(template)
        {
            _flag = FunctionTemplateFlags.None;
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
            GetOrCreateParameter(TemplateParameterUsage.FunctionName).SetValue(functionName);
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

        class FunctionParameter : ICloneable
        {
            public FunctionParameter(string name, string value, TemplateValueType valueType)
            {
                _name = name;
                _value = value;
                _valueType = valueType;
            }

            string _name;
            public string Name { get => _name; set => _name = value; }

            string _value;
            public string Value { get => _value; set => _value = value; }

            TemplateValueType _valueType;
            public TemplateValueType ValueType { get => _valueType; set => _valueType = value; }

            public string GetTemplate()
            {
                return $"$[{_name}]";
            }

            public string GetValue()
            {
                return _valueType switch
                {
                    TemplateValueType.String => $"\"{_value}\"",
                    TemplateValueType.Categorical => $"{{{_value}}}",
                    _ => _value,
                };
            }

            public object Clone()
            {
                return new FunctionParameter(_name, _value, _valueType);
            }
        }

        /// <summary>
        /// 将新的函数参数模板添加进集合末尾
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public void PushFunctionParameter(string name, string value, TemplateValueType type, TemplateParameterUsage usage)
        {
            TryPushParameter(new FunctionParameter(name, value, type), TemplateParameterUsage.FunctionParameters, usage);
        }

        /// <summary>
        /// 设定当前模板中特定用途的参数的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="usage"></param>
        public void SetFunctionParameterValue(object value, TemplateParameterUsage usage)
        {
            TemplateParameter? parameter = _parameters[TemplateParameterUsage.FunctionParameters];
            TemplateParameters? values;
            if (parameter is not null && (values = parameter.GetValue<TemplateParameters>()) != null)
            {
                values[usage]?.SetValue(value);
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
            string functionName = functionNameParam?.ToString() ?? string.Empty;
            StringBuilder result = new();
            result.Append(functionName);
            result.Append('(');
            TemplateParameter? parameterParam = _parameters[TemplateParameterUsage.FunctionParameters];
            TemplateParameters? functionParams = parameterParam?.GetValue<TemplateParameters>();
            if (parameterParam != null && functionParams != null)
            {
                for (int i = 0; i < functionParams.Count; i++)
                {
                    if (i > 0)
                    {
                        result.Append(", ");
                    }
                    result.Append(functionParams[i].GetValue<string>());
                }
            }
            result.Append(')');
            return result.ToString();
        }
    }


    public enum ScriptTemplateFlags
    {
        None,
        IfStatement,
        IfElseStatement,
        IfElseIfSatement,
        ForStatement,
        ForEachStatement,
        WhileStatement,
        DoWhileLoopStatement,
        DoUntilLoopStatement,
        DoLoopWhileStatement,
        DoLoopUntilStatement
    }


    public class ScriptTemplate : Template
    {
        public ScriptTemplate() : base(TemplateType.Script)
        {
            _flag = ScriptTemplateFlags.None;
            _indentLevel = 0;
        }

        protected ScriptTemplate(ScriptTemplate template) : base(template)
        {
            _flag = template.Flag;
            _indentLevel = template.IndentLevel;
        }

        ScriptTemplateFlags _flag;
        int _indentLevel;

        /// <summary>
        /// 脚本类型标记
        /// </summary>
        public ScriptTemplateFlags Flag { get => _flag; set => _flag = value; }
        /// <summary>
        /// 脚本的缩进级别
        /// </summary>
        public int IndentLevel { get => _indentLevel; set => _indentLevel = value; }

        /// <summary>
        /// 向当前参数列表中添加循环体内容
        /// </summary>
        /// <param name="value"></param>
        public void PushScriptBody<BodyType>(object value)
        {
            TryPushParameter(value, TemplateParameterUsage.ScriptBody, TemplateParameterUsage.ScriptBody);
        }

        /// <summary>
        /// 配置特定用途的参数对象，如果不存在会创建新的，如果存在会修改当前值
        /// </summary>
        /// <param name="value">需要配置的值</param>
        /// <param name="usage">参数的用途</param>
        public void SetParameter(object value, TemplateParameterUsage usage)
        {
            TemplateParameter? parameter = _parameters[usage];
            if (parameter is null)
            {
                parameter = _parameters.NewObject();
                parameter.Usage = usage;
                _parameters.Add(parameter);
            }
            parameter.SetValue(value);
        }

        public override string Exec()
        {
            string result = string.Empty;
            string[]? totalVariables = _parameters[TemplateParameterUsage.ScriptLoopVariables]?.GetValue<string[]>();
            TemplateParameter? bodyParam = _parameters[TemplateParameterUsage.ScriptBody];
            switch (_flag)
            {
                case ScriptTemplateFlags.IfStatement:
                case ScriptTemplateFlags.IfElseStatement:
                case ScriptTemplateFlags.IfElseIfSatement:
                    {
                        result = ScriptFactory.CreateIfElseScript(_indentLevel, bodyParam?.TryGetArray<(string, string[])>());
                        break;
                    }

                case ScriptTemplateFlags.ForStatement:
                    {
                        result = ScriptFactory.CreateForScript(
                            _indentLevel,
                            _parameters[TemplateParameterUsage.ScriptForVariable]?.GetValue<string>(),
                            totalVariables,
                            _parameters[TemplateParameterUsage.ScriptLowerBoundary]?.GetValue<string>(),
                            _parameters[TemplateParameterUsage.ScriptUpperBoundary]?.GetValue<string>(),
                            bodyParam?.TryGetArray<string>());
                        break;
                    }

                case ScriptTemplateFlags.ForEachStatement:
                    {
                        result = ScriptFactory.CreateForEachScript(
                            _indentLevel,
                            _parameters[TemplateParameterUsage.ScriptForVariable]?.GetValue<string>(),
                            totalVariables,
                            _parameters[TemplateParameterUsage.ScriptCollection]?.GetValue<string>(),
                            bodyParam?.TryGetArray<string>());
                        break;
                    }

                case ScriptTemplateFlags.WhileStatement:
                    {
                        result = ScriptFactory.CreateWhileScript(
                            _indentLevel,
                            _parameters[TemplateParameterUsage.ScriptTest]?.GetValue<string>(),
                            totalVariables,
                            bodyParam?.TryGetArray<string>());
                        break;
                    }

                case ScriptTemplateFlags.DoWhileLoopStatement:
                    {
                        result = ScriptFactory.CreateDoWhileLoopScript(
                            _indentLevel,
                            _parameters[TemplateParameterUsage.ScriptTest]?.GetValue<string>(),
                            totalVariables,
                            bodyParam?.TryGetArray<string>());
                        break;
                    }

                case ScriptTemplateFlags.DoUntilLoopStatement:
                    {
                        result = ScriptFactory.CreateDoUntilLoopScript(
                            _indentLevel,
                            _parameters[TemplateParameterUsage.ScriptTest]?.GetValue<string>(),
                            totalVariables,
                            bodyParam?.TryGetArray<string>());
                        break;
                    }

                case ScriptTemplateFlags.DoLoopWhileStatement:
                    {
                        result = ScriptFactory.CreateDoLoopWhileScript(
                            _indentLevel,
                            _parameters[TemplateParameterUsage.ScriptTest]?.GetValue<string>(),
                            totalVariables,
                            bodyParam?.TryGetArray<string>());
                        break;
                    }

                case ScriptTemplateFlags.DoLoopUntilStatement:
                    {
                        result = ScriptFactory.CreateDoLoopUntilScript(
                            _indentLevel,
                            _parameters[TemplateParameterUsage.ScriptTest]?.GetValue<string>(),
                            totalVariables,
                            bodyParam?.TryGetArray<string>());
                        break;
                    }
            }
            return result;
        }

        public override object Clone()
        {
            var clone = new ScriptTemplate(this);
            Clone(clone);
            return clone;
        }
    }

    public enum ExpressionTemplateFlags
    {
        None,
        Field,
        DeclareVariable,
        CallExpression,
        BinaryExpression,
        UnaryExpression,
    }

    /// <summary>
    /// 表达式模板，用于表示各种表达式的内容，例如：ContainsAny，ContainsAll
    /// 此类为表达式基类，不可实例化
    /// </summary>
    public abstract class ExpressionTemplate : Template
    {
        public ExpressionTemplate(TemplateType type) : base(type)
        {
            _flag = ExpressionTemplateFlags.None;
        }

        protected ExpressionTemplate(ExpressionTemplate template) : base(template)
        {
            _flag = template.Flag;
        }

        protected ExpressionTemplateFlags _flag;
        /// <summary>
        /// 表达式模板类型
        /// </summary>
        public ExpressionTemplateFlags Flag { get; set; }

    }

    /// <summary>
    /// 多级变量定义模板，可用于表示多级变量，例如：Top[code].Side
    /// </summary>
    public class FieldExpressionTemplate : ExpressionTemplate
    {
        public FieldExpressionTemplate() : base(TemplateType.Field)
        {
            _flag = ExpressionTemplateFlags.Field;
            // Top Level Field
            var topLevelField = _parameters.NewObject();
            topLevelField.Usage = TemplateParameterUsage.ScriptTopField;
            topLevelField.SetValue("");
            _parameters.Add(topLevelField);
            // Sub Levels
            var subLevelFields = _parameters.NewObject();
            subLevelFields.Usage = TemplateParameterUsage.ScriptLevelField;
            subLevelFields.SetValue(Array.Empty<(string, string)[]>());
            _parameters.Add(subLevelFields);
        }

        protected FieldExpressionTemplate(FieldExpressionTemplate template) : base(template)
        {
            _flag = template.Flag;
        }

        /// <summary>
        /// 设定顶级变量名
        /// </summary>
        /// <param name="topField"></param>
        public void SetTopField(string topField)
        {
            GetOrCreateParameter(TemplateParameterUsage.ScriptTopField).SetValue(topField);
        }

        /// <summary>
        /// 当前顶级变量名，未设定返回空字符串
        /// </summary>
        public string TopField
        {
            get
            {
                TemplateParameter? top;
                if ((top = _parameters[TemplateParameterUsage.ScriptTopField]) != null)
                {
                    return top.Name;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 添加级别到当前列表的末尾
        /// </summary>
        /// <param name="codeName">码号位置的字符</param>
        /// <param name="variableName">下级变量名</param>
        /// <param name="isCategorical">是否是Categorical类型</param>
        public void PushLevel(string codeName, string variableName, bool isCategorical)
        {
            if (_parameters.Count == 0)
            {
                var topField = _parameters.NewObject();
                topField.Usage = TemplateParameterUsage.ScriptTopField;
                _parameters.Add(topField);
            }
            TemplateParameter? parameter = _parameters[TemplateParameterUsage.ScriptLevelField];
            (string, string) item = new(variableName, isCategorical ? $"{{{codeName}}}" : codeName);
            if (parameter == null)
            {
                parameter = _parameters.NewObject();
                parameter.Usage = TemplateParameterUsage.ScriptLevelField;
                parameter.SetValue(new (string, string)[] { item });
                _parameters.Add(parameter);
            }
            else
            {
                parameter.TryPushParam(item, 0);
            }
        }

        public override string Exec()
        {
            string topField = _parameters.Count > 0 ? (_parameters[0].GetValue<string>() ?? string.Empty) : string.Empty;
            string result = topField;
            TemplateParameter? topFieldParam = _parameters[TemplateParameterUsage.ScriptTopField];
            TemplateParameter? levelFieldParam = _parameters[TemplateParameterUsage.ScriptLevelField];
            if (topFieldParam != null)
            {
                (string, string)[]? levels = null;
                if (levelFieldParam != null)
                {
                    levels = levelFieldParam.GetValue<(string, string)[]>();
                }
                result = ScriptFactory.CreateVariableTemplate(topField, levels);
            }
            return result;
        }

        public override object Clone()
        {
            var clone = new FieldExpressionTemplate(this);
            Clone(clone);
            return clone;
        }
    }

    /// <summary>
    /// 调用函数的表达式模板，可以是通过.调用，也可以是直接调用，函数的具体参数根据Parameter属性
    /// 模板参数填充。
    /// </summary>
    public class CallExpressionTemplate : ExpressionTemplate
    {
        public CallExpressionTemplate(FunctionTemplate functionTemplate) : base(TemplateType.Call)
        {
            _flag = ExpressionTemplateFlags.CallExpression;
            TemplateParameter template = _parameters.NewObject();
            template.Usage = TemplateParameterUsage.FunctionTemplate;
            template.SetValue(functionTemplate);
            _parameters.Add(template);
        }

        protected CallExpressionTemplate(CallExpressionTemplate template) : base(template)
        {
            _flag = template.Flag;
        }

        /// <summary>
        /// 设置模板的对象名参数信息
        /// </summary>
        /// <param name="objectName"></param>
        public void SetObjectName(string objectName)
        {
            GetOrCreateParameter(TemplateParameterUsage.ScriptObjectName).SetValue(objectName);
        }

        /// <summary>
        /// 获取模板的对象名
        /// </summary>
        /// <returns></returns>
        public string GetObjectName()
        {
            TemplateParameter? parameter = _parameters[TemplateParameterUsage.ScriptObjectName];
            if (parameter != null)
            {
                return parameter.GetValue<string>() ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 设定函数模板的参数值，需要提供参数的用途类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="usage"></param>
        public void SetParameterValue(object value, TemplateParameterUsage usage)
        {
            FunctionTemplate? function = _parameters[TemplateParameterUsage.FunctionTemplate]?.GetValue<FunctionTemplate>();
            if (function != null)
            {
                function.SetFunctionParameterValue(value, usage);
            }
        }

        public override string Exec()
        {
            TemplateParameter? topField = _parameters[TemplateParameterUsage.ScriptTopField];
            FunctionTemplate? functionTemplate = _parameters[TemplateParameterUsage.FunctionTemplate]?.GetValue<FunctionTemplate>();
            if (functionTemplate is null)
            {
                return string.Empty;
            }

            string result = topField != null ? (topField.GetValue<string>() ?? string.Empty) : string.Empty;
            result += (topField != null ? "." : string.Empty) + functionTemplate.Exec();
            return result;
        }

        public override object Clone()
        {
            var clone = new CallExpressionTemplate(this);
            Clone(clone);
            return clone;
        }
    }

    public enum BinaryOperatorFlags
    {
        None,
        And,
        Or,
        Xor,
        Equal,
        NotEqual,
        Plus,
        Min,
        Asterisk,
        Slash,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
    }

    /// <summary>
    /// 声明语句的模板信息
    /// </summary>
    public class DeclareExpressionTemplate : ExpressionTemplate
    {
        public DeclareExpressionTemplate() : base(TemplateType.Declare)
        {
            _flag = ExpressionTemplateFlags.DeclareVariable;
        }

        public DeclareExpressionTemplate(DeclareExpressionTemplate template) : base(template)
        {
            _flag = ExpressionTemplateFlags.DeclareVariable;
        }

        /// <summary>
        /// 像当前集合的末尾添加新的参数对象
        /// </summary>
        /// <param name="variable"></param>
        public void PushDeclaration(string variable)
        {
            var param = _parameters.NewObject();
            param.Usage = TemplateParameterUsage.ScriptDeclaration;
            param.SetValue(variable);
            _parameters.Add(param);
        }

        public override object Clone()
        {
            var clone = new DeclareExpressionTemplate(this);
            Clone(clone);
            return clone;
        }

        public override string Exec()
        {
            string result = string.Empty;
            if (Parameters.Count > 0)
            {
                string[] parameters = new string[Parameters.Count];
                for (int i = 0; i < Parameters.Count; i++)
                {
                    string? value = Parameters[i].GetValue<string>();
                    if (!string.IsNullOrEmpty(value))
                    {
                        parameters[i] = value;
                    }
                }
                result = ScriptFactory.CreateDeclareScript(parameters);
            }
            return result;
        }
    }

}
