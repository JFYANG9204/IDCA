
using System;

namespace IDCA.Bll.Template
{

    public enum TemplateType
    {
        File,
        Script,
        Expression,
    }

    public enum TemplateValueType
    {
        String,
        Variable,
        Categorical,
        Number,
        Expression,
    }

    public abstract class Template
    {
        public Template(TemplateType type)
        {
            _content = "";
            _type = type;
            _parameters = new(this);
        }

        protected string _content;
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
        /// 文件文本模板内容，所有需要替换的内容需要满足格式 ：$[variable]，变量名不区分大小写
        /// </summary>
        public string Content { get => _content; set => _content = value; }
    }

    public class FileTemplate : Template
    {
        public FileTemplate() : base(TemplateType.File)
        {
            _directory = "";
            _fileName = "";
            _content = "";
        }

        string _directory;
        string _fileName;

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string Directory { get => _directory; set => _directory = value; }
        /// <summary>
        /// 文件名，包含文件名和扩展名
        /// </summary>
        public string FileName { get => _fileName; set => _fileName = value; }

        public override string Exec()
        {
            string result = _content;
            if (_parameters.Count > 0)
            {
                foreach (TemplateParameter parameter in Parameters)
                {
                    result = result.Replace(parameter.ToString(), parameter.GetValue<string>(), StringComparison.OrdinalIgnoreCase);
                }
            }
            return result;
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

        public override string Exec()
        {
            string result = string.Empty;
            if ((_flag == ScriptTemplateFlags.IfStatement || 
                _flag == ScriptTemplateFlags.IfElseStatement || 
                _flag == ScriptTemplateFlags.IfElseIfSatement) && Parameters.Count >= 2)
            {
                result = ScriptFactory.CreateIfElseScript(_indentLevel, Parameters.GetTupleParameters<string, string[]>(0));
            }
            else if (_flag == ScriptTemplateFlags.ForStatement && Parameters.Count >= 5)
            {
                result = ScriptFactory.CreateForScript(_indentLevel, Parameters[0].GetValue<string>(), Parameters[1].GetValue<string[]>(), Parameters[2].GetValue<string>(), Parameters[3].GetValue<string>(), Parameters[4].GetValue<string[]>());
            }
            else if (_flag == ScriptTemplateFlags.ForEachStatement && Parameters.Count >= 4)
            {
                result = ScriptFactory.CreateForEachScript(_indentLevel, Parameters[0].GetValue<string>(), Parameters[1].GetValue<string[]>(), Parameters[2].GetValue<string>(), Parameters[3].GetValue<string[]>());
            }
            else if (_flag == ScriptTemplateFlags.WhileStatement && Parameters.Count >= 3)
            {
                result = ScriptFactory.CreateWhileScript(_indentLevel, Parameters[0].GetValue<string>(), Parameters[1].GetValue<string[]>(), Parameters[2].GetValue<string[]>());
            }
            else if (_flag == ScriptTemplateFlags.DoWhileLoopStatement && Parameters.Count >= 3)
            {
                result = ScriptFactory.CreateDoWhileLoopScript(_indentLevel, Parameters[0].GetValue<string>(), Parameters[1].GetValue<string[]>(), Parameters[2].GetValue<string[]>());
            }
            else if (_flag == ScriptTemplateFlags.DoUntilLoopStatement && Parameters.Count >= 3)
            {
                result = ScriptFactory.CreateDoUntilLoopScript(_indentLevel, Parameters[0].GetValue<string>(), Parameters[1].GetValue<string[]>(), Parameters[2].GetValue<string[]>());
            }
            else if (_flag == ScriptTemplateFlags.DoLoopWhileStatement && Parameters.Count >= 3)
            {
                result = ScriptFactory.CreateDoLoopWhileScript(_indentLevel, Parameters[0].GetValue<string>(), Parameters[1].GetValue<string[]>(), Parameters[2].GetValue<string[]>());
            }
            else if (_flag == ScriptTemplateFlags.DoLoopUntilStatement && Parameters.Count >= 3)
            {
                result = ScriptFactory.CreateDoLoopUntilScript(_indentLevel, Parameters[0].GetValue<string>(), Parameters[1].GetValue<string[]>(), Parameters[2].GetValue<string[]>());
            }
            return result;
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
        public ExpressionTemplate() : base(TemplateType.Expression)
        {
            _flag = ExpressionTemplateFlags.None;
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
        public FieldExpressionTemplate() : base()
        {
            _topField = string.Empty;
            _flag = ExpressionTemplateFlags.Field;
        }

        string _topField;
        /// <summary>
        /// 最上级变量名
        /// </summary>
        public string TopField { get => _topField; set => _topField = value; }

        /// <summary>
        /// 添加级别到当前列表的末尾
        /// </summary>
        /// <param name="codeName">码号位置的字符</param>
        /// <param name="variableName">下级变量名</param>
        /// <param name="isCategorical">是否是Categorical类型</param>
        public void PushLevel(string codeName, string variableName, bool isCategorical)
        {
            var parameter = Parameters.NewObject();
            parameter.Name = codeName;
            parameter.Value = new FieldLevel(codeName, variableName, isCategorical);
            Parameters.Add(parameter);
        }

        class FieldLevel
        {
            public FieldLevel(string codeName, string variableName, bool isCategorical)
            {
                CodeName = codeName;
                VariableName = variableName;
                IsCategorical = isCategorical;
            }
            public string CodeName { get; set; }
            public string VariableName { get; set; }
            public bool IsCategorical { get; set; }
        }

        public override string Exec()
        {
            string result = _topField;
            if (Parameters.Count > 0)
            {
                (string, string)[] fields = new (string, string)[Parameters.Count];
                for (int i = 0; i < Parameters.Count; i++)
                {
                    FieldLevel? fieldLevel = Parameters[i].GetValue<FieldLevel>();
                    if (fieldLevel != null)
                    {
                        fields[i] = new(fieldLevel.VariableName, fieldLevel.IsCategorical ? $"{{{fieldLevel.CodeName}}}" : fieldLevel.CodeName);
                    }
                }
                result = ScriptFactory.CreateVariableTemplate(_topField, fields);
            }
            return result;
        }

    }

    public enum FunctionTemplateUsage
    {
        ManipulateTitleLabel,
        ManipulateSideLabel,
        ManipulateSideAxis,
        ManipulateTypeLabel,
    }

    /// <summary>
    /// 调用函数的表达式模板，可以是通过.调用，也可以是直接调用，函数的具体参数根据Parameter属性
    /// 模板参数填充。
    /// </summary>
    public class CallExpressionTemplate : ExpressionTemplate
    {
        public CallExpressionTemplate() : base()
        {
            _object = string.Empty;
            _functionTemplate = string.Empty;
            _flag = ExpressionTemplateFlags.CallExpression;
        }

        string _object;
        string _functionTemplate;

        /// <summary>
        /// 调用对象名称，例如：Object.ContainsAny中的Object，如果不通过.操作符调用，
        /// 此属性的值赋值为空
        /// </summary>
        public string Object { get => _object; set => _object = value; }
        /// <summary>
        /// 调用的函数模板
        /// </summary>
        public string FunctionTemplate { get => _functionTemplate; set => _functionTemplate = value; }

        class FunctionParameter
        {
            public FunctionParameter(string name, string value, TemplateValueType valueType)
            {
                Name = name;
                Value = value;
                Type = valueType;
            }

            public string Name { get; set; }
            public string Value { get; set; }
            public TemplateValueType Type { get; set; }

            public string GetValue()
            {
                return Type switch
                {
                    TemplateValueType.String => $"\"{Value}\"",
                    TemplateValueType.Categorical => $"{{{Value}}}",
                    _ => Value,
                };
            }

            public string GetTemplate()
            {
                return $"$[{Name}]";
            }
        }

        /// <summary>
        /// 向集合中添加函数的参数信息
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="valueType"></param>
        public void PushParameter(string parameterName, string parameterValue, TemplateValueType valueType)
        {
            var parameter = Parameters.NewObject();
            parameter.Name = parameterName;
            parameter.Value = new FunctionParameter(parameterName, parameterValue, valueType);
            Parameters.Add(parameter);
        }

        /// <summary>
        /// 修改指定索引位置的变量值
        /// </summary>
        /// <param name="index">参数位置，0开始</param>
        /// <param name="value">需要修改的参数值</param>
        public void SetParameterValue(int index, string value, TemplateValueType valueType)
        {
            if (index < 0 || index >= Parameters.Count)
            {
                return;
            }
            if (Parameters[index].Value is FunctionParameter param)
            {
                param.Value = value;
                param.Type = valueType;
            }
        }

        public override string Exec()
        {
            string result = _object;
            if (!string.IsNullOrEmpty(_functionTemplate))
            {
                result = $"{(string.IsNullOrEmpty(_object) ? string.Empty : $"{_object}.")}{_functionTemplate}";
                foreach (TemplateParameter param in Parameters)
                {
                    FunctionParameter? functionParam = param.GetValue<FunctionParameter>();
                    if (functionParam != null)
                    {
                        result = result.Replace(functionParam.GetTemplate(), functionParam.GetValue(), StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            return result;
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
    /// 二元表达式的模板信息，二元操作符包括：+、-、*、/、>、>=、<、<=、=、<>
    /// </summary>
    public class BinaryExpressionTemplate : ExpressionTemplate
    {
        public BinaryExpressionTemplate() : base()
        {
            _flag = ExpressionTemplateFlags.BinaryExpression;
            _operatorFlag = BinaryOperatorFlags.None;
        }

        BinaryOperatorFlags _operatorFlag;
        /// <summary>
        /// 当前的二元操作符类型
        /// </summary>
        public BinaryOperatorFlags OperatorFlag { get => _operatorFlag; set => _operatorFlag = value; }

        public override string Exec()
        {
            string? left = Parameters.Count >= 1 ? Parameters[0].GetValue<string>() : string.Empty;
            string? right = Parameters.Count >= 2 ? Parameters[1].GetValue<string>() : string.Empty;
            return ScriptFactory.CreateBinaryScript(left, right, _operatorFlag);
        }

    }


    public class DeclareExpressionTemplate : ExpressionTemplate
    {
        public DeclareExpressionTemplate() : base()
        {
            _flag = ExpressionTemplateFlags.DeclareVariable;
        }

        public override string Exec()
        {
            string result = string.Empty;
            if (Parameters.Count > 0)
            {
                string?[] parameters = new string[Parameters.Count];
                for (int i = 0; i < Parameters.Count; i++)
                {
                    parameters[i] = Parameters[i].GetValue<string>();
                }
                result = ScriptFactory.CreateDeclareScript(parameters);
            }
            return result;
        }
    }

}
