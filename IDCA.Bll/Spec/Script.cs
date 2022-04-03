
using System;
using System.Text;

namespace IDCA.Bll.Spec
{

    public enum ScriptType
    {
        Empty,
        Field,
        CallExpression,
        BinaryExpression,
        UnaryExpression,
        BlockStatement,
        IfStatement,
        ForStatement,
        ForEachStatement,
        WhileStatement,
        DoWhileStatement,
        DoUntilStatement,
    }

    public abstract class Script : SpecObject
    {
        protected Script(SpecObject parent, ScriptType type) : base(parent)
        {
            _type = type;
            _objectType = SpecObjectType.Script;
        }

        readonly ScriptType _type;
        /// <summary>
        /// 当前脚本的类型
        /// </summary>
        public ScriptType Type { get => _type; }

        int _indentLevel = 0;
        /// <summary>
        /// 当前脚本的缩进级别
        /// </summary>
        public int IndentLevel { get => _indentLevel; set => _indentLevel = value; }

        string _information = string.Empty;
        /// <summary>
        /// 当前已保存的注释信息
        /// </summary>
        public string Info => _information;
        /// <summary>
        /// 配置注释信息
        /// </summary>
        /// <param name="info"></param>
        public void SetInfo(string info)
        {
            _information = info;
        }

        /// <summary>
        /// 导出符合当前属性配置的字符串
        /// </summary>
        /// <returns></returns>
        public abstract string Export();
    }

    internal class EmptyScript : Script
    {
        public EmptyScript(SpecObject parent) : base(parent, ScriptType.Empty)
        {
        }

        public override string Export()
        {
            return string.Empty;
        }
    }

    public class FieldScript : Script
    {
        internal FieldScript(SpecObject parent) : base(parent, ScriptType.Field)
        {
        }

        Tuple<string, string>[] _fields = Array.Empty<Tuple<string, string>>();
        /// <summary>
        /// 向当前级别集合的末尾添加新的变量名和码号
        /// 如果Code为null，当只有一级时，是顶级变量，如果是下级Field，Code部分默认为..
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="code"></param>
        public void PushLevelField(string variable, string code = "", bool isCategorical = false)
        {
            if (!string.IsNullOrEmpty(variable))
            {
                Array.Resize(ref _fields, _fields.Length + 1);
                _fields[^1] = new(variable, string.IsNullOrEmpty(code) ? code : (isCategorical ? $"{{{code}}}" : code));
            }
        }

        /// <summary>
        /// 从字符串创建FieldScript对象，需要提供此对象的父级对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static FieldScript FromString(SpecObject parent, string? field)
        {
            var script = new FieldScript(parent);
            script.FromString(field);
            return script;
        }

        /// <summary>
        /// 从字符串读取Field脚本配置
        /// </summary>
        /// <param name="field"></param>
        public void FromString(string? field)
        {
            _fields = Array.Empty<Tuple<string, string>>();
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            StringBuilder variable = new();
            StringBuilder codes = new();
            bool isCategorical = false;

            int i = 0;
            while (i < field.Length)
            {
                char c = field[i];
                // []外部的字符是为变量名
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '@' || c == '_' || c == '#' || (c >= '0' && c <= '9'))
                {
                    variable.Append(c);
                }

                if (c == '[' || i == field.Length - 1)
                {
                    PushLevelField(variable.ToString(), codes.ToString(), isCategorical);
                    variable.Clear();
                    codes.Clear();
                    isCategorical = false;
                    // 读取到'['时，开始读取下级变量码号，读取到'].'结束，需要[]内至少有一个字符
                    if (c == '[' && i < field.Length - 3)
                    {
                        i++;
                        // 开始Categorical
                        if (field[i] == '{')
                        {
                            isCategorical = true;
                            i++;
                        }
                        while (i < field.Length && field[i] != ']' && field[i] != '}')
                        {
                            codes.Append(field[i]);
                            i++;
                        }
                        // 读取到结尾时，直接跳出，忽略当前码号
                        if (i >= field.Length - 1)
                        {
                            break;
                        }
                        // 结束Categorical
                        if (field[i] == '}')
                        {
                            i++;
                        }
                        // 跳过']'
                        i++;
                    }
                }

                i++;
            }

        }

        /// <summary>
        /// 顶级变量名，如果没有配置，返回空字符串
        /// </summary>
        public string TopLevel
        {
            get
            {
                if (_fields.Length == 0)
                {
                    return string.Empty;
                }
                return _fields[0].Item1;
            }
        }

        /// <summary>
        /// 获取下级变量和码号，ValueTuple的第一个值为变量名，第二个值为码号名。
        /// 如果码号为空，默认为'..'
        /// </summary>
        public (string, string)[] SubLevels
        {
            get
            {
                if (_fields.Length == 1)
                {
                    return Array.Empty<(string, string)>();
                }
                var result = new (string, string)[_fields.Length - 1];
                for (int i = 1; i < _fields.Length; i++)
                {
                    result[i - 1] = _fields[i].ToValueTuple();
                }
                return result;
            }
        }

        public override string Export()
        {
            StringBuilder builder = new();
            builder.Append(_fields[0].Item1);
            if (_fields.Length == 1)
            {
                return builder.ToString();
            }
            else
            {
                for (int i = 1; i < _fields.Length; i++)
                {
                    builder.Append($"[{(string.IsNullOrEmpty(_fields[i].Item2) ? ".." : $"{_fields[i].Item2}")}].{_fields[i].Item1}");
                }
            }
            return builder.ToString();
        }
    }

    public class CallExpressionScript : Script
    {
        internal CallExpressionScript(SpecObject parent) : base(parent, ScriptType.CallExpression)
        {
        }

        string _object = string.Empty;
        string _functionName = string.Empty;
        string[] _arguments = Array.Empty<string>();

        /// <summary>
        /// 配置当前表达式调用对象的名称
        /// </summary>
        /// <param name="objectName"></param>
        public void SetObjectName(string objectName)
        {
            _object = objectName;
        }

        /// <summary>
        /// 配置当前表达式调用的函数名称
        /// </summary>
        /// <param name="functionName"></param>
        public void SetFunctionName(string functionName)
        {
            _functionName = functionName;
        }

        /// <summary>
        /// 向当前参数列表的末尾添加参数值
        /// </summary>
        /// <param name="arg"></param>
        public void PushArgument(string arg)
        {
            Array.Resize(ref _arguments, _arguments.Length + 1);
            _arguments[^1] = arg;
        }

        public override string Export()
        {
            return $"{new string(' ', IndentLevel * 4)}{(string.IsNullOrEmpty(_object) ? "" : $"{_object}.")}{_functionName}({string.Join(',', _arguments)})";
        }
    }


    public class UnaryExpressionScript : Script
    {
        internal UnaryExpressionScript(SpecObject parent) : base(parent, ScriptType.UnaryExpression)
        {
        }

        string _expression = string.Empty;
        /// <summary>
        /// 配置当前一元操作符的后缀表达式
        /// </summary>
        /// <param name="expression">已知的表达式字符串</param>
        public void SetExpression(string expression)
        {
            _expression = expression;
        }

        /// <summary>
        /// 配置当前一元操作符的后缀表达式
        /// </summary>
        /// <param name="expression">已知的函数调用脚本对象</param>
        public void SetExpression(CallExpressionScript expression)
        {
            SetExpression(expression.Export());
        }

        /// <summary>
        /// 配置当前一元操作符的后缀表达式
        /// </summary>
        /// <param name="expression">已知的一元表达式脚本对象</param>
        public void SetExpression(UnaryExpressionScript expression)
        {
            SetExpression(expression.Export());
        }

        /// <summary>
        /// 配置当前一元操作符的后缀表达式
        /// </summary>
        /// <param name="expression">已知的二元表达式脚本对象</param>
        public void SetExpression(BinaryExpressionScript expression)
        {
            SetExpression($"({expression.Export()})");
        }


        public override string Export()
        {
            return $"{new string(' ', IndentLevel * 4)}Not {_expression}";
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

    public class BinaryExpressionScript : Script
    {
        internal BinaryExpressionScript(SpecObject parent) : base(parent, ScriptType.BinaryExpression)
        {
        }

        string _left = string.Empty;
        string _right = string.Empty;
        BinaryOperatorFlags _flag = BinaryOperatorFlags.None;

        /// <summary>
        /// 配置二元操作符的操作符类型
        /// </summary>
        /// <param name="flag"></param>
        public void SetOperator(BinaryOperatorFlags flag)
        {
            _flag = flag;
        }

        /// <summary>
        /// 配置二元操作符的左值
        /// </summary>
        /// <param name="left"></param>
        public void SetLeft(string left)
        {
            _left = left;
        }

        /// <summary>
        /// 配置二元操作符的左值
        /// </summary>
        /// <param name="left"></param>
        /// <param name="paren"></param>
        public void SetLeft(Script left, bool paren)
        {
            SetLeft(paren ? $"({left.Export()})" : left.Export());
        }

        /// <summary>
        /// 配置二元操作符的右值
        /// </summary>
        /// <param name="right"></param>
        public void SetRight(string right)
        {
            _right = right;
        }

        /// <summary>
        /// 配置二元操作符的右值
        /// </summary>
        /// <param name="right"></param>
        public void SetRight(Script right, bool paren)
        {
            SetRight(paren ? $"({right.Export()})" : right.Export());
        }

        public override string Export()
        {
            string logic = string.Empty;
            switch (_flag)
            {
                case BinaryOperatorFlags.And: logic = "And"; break;
                case BinaryOperatorFlags.Or: logic = "Or"; break;
                case BinaryOperatorFlags.Xor: logic = "Xor"; break;
                case BinaryOperatorFlags.Equal: logic = "="; break;
                case BinaryOperatorFlags.NotEqual: logic = "<>"; break;
                case BinaryOperatorFlags.Plus: logic = "+"; break;
                case BinaryOperatorFlags.Min: logic = "-"; break;
                case BinaryOperatorFlags.Asterisk: logic = "*"; break;
                case BinaryOperatorFlags.Slash: logic = "/"; break;
                case BinaryOperatorFlags.Greater: logic = ">"; break;
                case BinaryOperatorFlags.GreaterEqual: logic = ">="; break;
                case BinaryOperatorFlags.Less: logic = "<"; break;
                case BinaryOperatorFlags.LessEqual: logic = "<="; break;
                default: break;
            }
            return $"{new string(' ', IndentLevel * 4)}{_left} {logic} {_right}";
        }
    }


    public class BlockStatementScript : Script
    {

        internal BlockStatementScript(SpecObject parent) : base(parent, ScriptType.BlockStatement)
        {
        }

        Script[] _blockBody = Array.Empty<Script>();

        /// <summary>
        /// 向当前集合的末尾追加新的单行脚本语句
        /// </summary>
        /// <param name="script"></param>
        public void Push(Script script)
        {
            Array.Resize(ref _blockBody, _blockBody.Length + 1);
            _blockBody[^1] = script;
        }

        public override string Export()
        {
            StringBuilder builder = new();
            Array.ForEach(_blockBody, body => builder.AppendLine($"{new string(' ', IndentLevel * 4)}{body.Export()}"));
            return builder.ToString();
        }
    }


    public class IfStatementScript : Script
    {
        internal IfStatementScript(SpecObject parent) : base(parent, ScriptType.IfStatement)
        {
        }

        Tuple<Script, Script>[] _body = Array.Empty<Tuple<Script, Script>>();

        /// <summary>
        /// 向当前脚本添加新的IfElse脚本
        /// </summary>
        /// <param name="test">If后的判断脚本，最后一个为空时，默认为Else语句</param>
        /// <param name="body"></param>
        public void PushLevel(Script test, Script body)
        {
            Array.Resize(ref _body, _body.Length + 1);
            body.IndentLevel = IndentLevel + 1;
            _body[^1] = new Tuple<Script, Script>(test, body);
        }

        public override string Export()
        {
            StringBuilder result = new();
            string _indent = new(' ', IndentLevel * 4);
            for (int i = 0; i < _body.Length; i++)
            {
                (Script test, Script body) = _body[i];
                if (i == 0)
                {
                    result.Append($"{_indent}If {test.Export()} Then");
                }
                else if (_body.Length > 1 && i > 0 && test.Type != ScriptType.Empty)
                {
                    result.AppendLine($"{_indent}ElseIf {test.Export()} Then");
                }
                else if (_body.Length > 1 && i == _body.Length - 1 && test.Type == ScriptType.Empty)
                {
                    result.AppendLine($"{_indent}Else");
                }
                if (!(_body.Length == 1 && i == 1 && (body.Type == ScriptType.BinaryExpression || body.Type == ScriptType.CallExpression)))
                {
                    result.AppendLine();
                }
                result.AppendLine(body.Export());
                if ((_body.Length > 1 || !(body.Type == ScriptType.BinaryExpression || body.Type == ScriptType.CallExpression)) && i == _body.Length - 1)
                {
                    result.AppendLine($"{_indent}End If");
                }
            }
            return result.ToString();
        }
    }

    public abstract class LoopStatementScript : Script
    {
        protected LoopStatementScript(SpecObject parent, ScriptType type) : base(parent, type)
        {
        }

        protected Script? _body;

        /// <summary>
        /// 创建Block类型的语法循环体
        /// </summary>
        public void CreateBlockBody()
        {
            _body = new BlockStatementScript(this) { IndentLevel = this.IndentLevel + 1 };
        }

        /// <summary>
        /// 配置语句语法体
        /// </summary>
        /// <param name="body"></param>
        public void SetBody(Script body)
        {
            body.IndentLevel = IndentLevel + 1;
            _body = body;
        }

    }

    public class ForStatementScript : LoopStatementScript
    {
        internal ForStatementScript(SpecObject parent) : base(parent, ScriptType.ForStatement)
        {
        }

        string _variable = string.Empty;
        string _lowerBoundary = string.Empty;
        string _upperBoundary = string.Empty;
        /// <summary>
        /// 配置For循环语句循环变量名
        /// </summary>
        /// <param name="variableName"></param>
        public void SetVariableName(string variableName)
        {
            _variable = variableName;
        }

        /// <summary>
        /// 配置For循环语句的上限和下限
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void SetBoundary(string lower, string upper)
        {
            _lowerBoundary = lower;
            _upperBoundary = upper;
        }

        public override string Export()
        {
            StringBuilder builder = new();
            string _indent = new(' ', IndentLevel * 4);
            builder.AppendLine($"{_indent}For {_variable} = {_lowerBoundary} To {_upperBoundary}");
            builder.AppendLine(_body?.Export());
            builder.AppendLine($"{_indent}Next");
            return builder.ToString();
        }
    }

    public class ForEachStatementScript : LoopStatementScript
    {
        internal ForEachStatementScript(SpecObject parent) : base(parent, ScriptType.ForEachStatement)
        {
        }

        string _variable = string.Empty;
        string _collection = string.Empty;

        /// <summary>
        /// 配置ForEach循环语句循环变量名
        /// </summary>
        /// <param name="variableName"></param>
        public void SetVariable(string variableName)
        {
            _variable = variableName;
        }

        /// <summary>
        /// 配置ForEach循环语句的目标集合
        /// </summary>
        /// <param name="collection"></param>
        public void SetCollection(string collection)
        {
            _collection = collection;
        }

        public override string Export()
        {
            StringBuilder builder = new();
            string _indent = new(' ', IndentLevel * 4);
            builder.AppendLine($"{_indent}For Each {_variable} In {_collection}");
            builder.AppendLine(_body?.Export());
            builder.AppendLine($"{_indent}Next");
            return builder.ToString();
        }
    }

    public abstract class WhileLikeStatementScript : LoopStatementScript
    {
        protected WhileLikeStatementScript(SpecObject parent, ScriptType type) : base(parent, type)
        {
        }

        protected string _test = string.Empty;
        /// <summary>
        /// 配置当前循环语句的判断条件语句
        /// </summary>
        /// <param name="test"></param>
        public void SetTest(string test)
        {
            _test = test;
        }

        /// <summary>
        /// 配置当前循环语句的判断条件语句
        /// </summary>
        /// <param name="test"></param>
        public void SetTest(Script script)
        {
            _test = script.Export();
        }

    }

    public class WhileStatementScript : WhileLikeStatementScript
    {
        internal WhileStatementScript(SpecObject parent) : base(parent, ScriptType.WhileStatement)
        {
        }

        public override string Export()
        {
            StringBuilder builder = new();
            string _indent = new(' ', IndentLevel * 4);
            builder.AppendLine($"{_indent}While {_test}");
            builder.AppendLine(_body?.Export());
            builder.AppendLine($"{_indent}End While");
            return builder.ToString();
        }
    }

    public class DoWhileStatementScript : WhileLikeStatementScript
    {
        internal DoWhileStatementScript(SpecObject parent) : base(parent, ScriptType.DoWhileStatement)
        {
        }

        public override string Export()
        {
            StringBuilder builder = new();
            string _indent = new(' ', IndentLevel * 4);
            builder.AppendLine($"{_indent}Do");
            builder.AppendLine(_body?.Export());
            builder.AppendLine($"{_indent}Loop While {_test}");
            return builder.ToString();
        }
    }

    public class DoUntilStatementScript : WhileLikeStatementScript
    {
        internal DoUntilStatementScript(SpecObject parent) : base(parent, ScriptType.DoUntilStatement)
        {
        }

        public override string Export()
        {
            StringBuilder builder = new();
            string _indent = new(' ', IndentLevel * 4);
            builder.AppendLine($"{_indent}Do");
            builder.AppendLine(_body?.Export());
            builder.AppendLine($"{_indent}Loop Until {_test}");
            return builder.ToString();
        }
    }

}
