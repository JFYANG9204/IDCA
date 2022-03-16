
using System.Text;

namespace IDCA.Bll.Template
{
    internal class ScriptFactory
    {
        /// <summary>
        /// 创建ContainsAny条件判断语句的脚本
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <param name="codes">判断选中的码号</param>
        /// <param name="exact">是否仅包含</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateContainsAnyScript(string? variable, string? codes, bool exact)
        {
            return $"{variable ?? string.Empty}.ContainsAny({{{codes ?? string.Empty}}}{(exact ? ", True" : "")})";
        }

        /// <summary>
        /// 创建ContainsAll条件判断语句的脚本
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <param name="codes">判断选中的码号</param>
        /// <param name="exact">是否包含仅全部</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateContainsAllScript(string? variable, string? codes, bool exact = false)
        {
            return $"{variable ?? string.Empty}.ContainsAll({{{codes ?? string.Empty}}}{(exact ? ", True" : "")})";
        }

        /// <summary>
        /// 创建ContainsSome条件判断语句脚本
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <param name="codes">判断选中的码号</param>
        /// <param name="min">最小数量</param>
        /// <param name="max">最大数量</param>
        /// <param name="exact">是否不包含其他</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateContainsSomeScript(string? variable, string? codes, string? min = null, string? max = null, bool exact = false)
        {
            return $"{variable ?? string.Empty}.ContainsSome({{{codes ?? string.Empty}}}{(string.IsNullOrEmpty(min) ? string.Empty : $", {min}")}{(string.IsNullOrEmpty(max) ? string.Empty : $", {max}")}{(exact ? ", True" : string.Empty)})";
        }

        /// <summary>
        /// 创建IsEmpty条件语句的脚本
        /// </summary>
        /// <param name="variable">变量名</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateIsEmptyScript(string? variable)
        {
            return $"{variable ?? string.Empty}.IsEmpty()";
        }

        /// <summary>
        /// 创建变量声明脚本语句
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        internal static string CreateDeclareScript(string?[] variables)
        {
            return $"Dim {(variables == null ? string.Empty : string.Join(',', variables))}";
        }

        /// <summary>
        /// 创建二元操作符语句，需要提供左值和右值
        /// </summary>
        /// <param name="left">左侧表达式，默认会在两端生成括号</param>
        /// <param name="right">右侧表达式，默认会在两端生成括号</param>
        /// <param name="type">二元操作符类型</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateBinaryScript(string? left, string? right, BinaryOperatorFlags type)
        {
            string logic = string.Empty;
            switch (type)
            {
                case BinaryOperatorFlags.And:           logic = "And"; break;
                case BinaryOperatorFlags.Or:            logic = "Or";  break;
                case BinaryOperatorFlags.Xor:           logic = "Xor"; break;
                case BinaryOperatorFlags.Equal:         logic = "=";   break;
                case BinaryOperatorFlags.NotEqual:      logic = "<>";  break;
                case BinaryOperatorFlags.Plus:          logic = "+";   break;
                case BinaryOperatorFlags.Min:           logic = "-";   break;
                case BinaryOperatorFlags.Asterisk:      logic = "*";   break;
                case BinaryOperatorFlags.Slash:         logic = "/";   break;
                case BinaryOperatorFlags.Greater:       logic = ">";   break;
                case BinaryOperatorFlags.GreaterEqual:  logic = ">=";  break;
                case BinaryOperatorFlags.Less:          logic = "<";   break;
                case BinaryOperatorFlags.LessEqual:     logic = "<=";  break;
                default:                                               break;
            }
            return $"{left ?? string.Empty} {logic} {right ?? string.Empty}";
        }

        /// <summary>
        /// 创建否定逻辑语句，需要提供右值，默认会在右值两端生成括号
        /// </summary>
        /// <param name="right">右侧表达式</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateNotScript(string? right)
        {
            return $"Not ({right ?? string.Empty})";
        }

        readonly static string tab = "    ";

        static string GetIndent(int indentLevel)
        {
            StringBuilder builder = new();
            for (int i = 0; i < indentLevel; i++)
            {
                builder.Append(tab);
            }
            return builder.ToString();
        }

        static string GetMultipleLineScript(int indentLevel, params string?[] scripts)
        {
            string indent = GetIndent(indentLevel);
            StringBuilder builder = new();
            if (scripts.Length > 0)
            {
                for (int i = 0; i < scripts.Length; i++)
                {
                    builder.AppendLine($"{indent}{scripts[i] ?? string.Empty}");
                }
            }
            return builder.ToString();
        }


        static string GetScriptWithVariable(int indentLevel, string?[]? templates, params string?[]? variables)
        {
            string indent = GetIndent(indentLevel);
            StringBuilder builder = new();
            if (variables != null && templates != null && templates.Length > 0)
            {
                foreach (string? template in templates)
                {
                    builder.AppendLine($"{indent}{string.Format(template ?? string.Empty, variables)}");
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 创建变量模板，可以是多级变量或者单级变量
        /// </summary>
        /// <param name="topVariable">最上级变量名</param>
        /// <param name="parameter">下级变量参数</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateVariableTemplate(string? topVariable, (string subVariable, string code)[]? parameter = null)
        {
            StringBuilder builder = new();
            builder.Append(topVariable);
            if (parameter != null)
            {
                for (int i = 0; i < parameter.Length; i++)
                {
                    var (subVariable, code) = parameter[i];
                    if (!string.IsNullOrEmpty(subVariable) && !string.IsNullOrEmpty(code))
                    {
                        builder.Append($"[{code}].{subVariable}");
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 创建If... Then ... 语法语句
        /// </summary>
        /// <param name="test">If测试的条件语句</param>
        /// <param name="thens">条件为True时执行的语句</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateIfScript(string? test, string?[] thens, int indentLevel = 0)
        {
            string indent = GetIndent(indentLevel);
            string then = GetMultipleLineScript(indentLevel, thens);
            bool endIf = thens.Length > 1;
            return endIf ? $"{indent}If {test ?? string.Empty} Then\n{indent}{tab}{then}\n{indent}End If" : $"{indent}If {test ?? string.Empty} Then {then}";
        }

        /// <summary>
        /// 构建If ... Then ... ElseIf ... Then ... End If 多级条件判断语句
        /// </summary>
        /// <param name="indentLevel">缩放级别</param>
        /// <param name="parameters">多级条件和后续语句</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateIfElseScript(int indentLevel, params (string test, string[] thens)[] parameters)
        {
            if (parameters.Length == 1)
            {
                return CreateIfScript(parameters[0].test, parameters[0].thens, indentLevel);
            }
            StringBuilder builder = new();
            string indent = GetIndent(indentLevel);
            for (int i = 0; i < parameters.Length; i++)
            {
                var (test, thens) = parameters[i];
                builder.AppendLine($"{indent}{(i == 0 ? $"If {test}" : (i == parameters.Length - 1 ? "Else" : $"ElseIf {test}"))} Then");
                builder.AppendLine($"{tab}{GetMultipleLineScript(indentLevel, thens)}");
                if (i == parameters.Length - 1)
                {
                    builder.AppendLine($"{indent}End If");
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 创建For [variable] = [lbound] To [ubound] ... Next 格式的For循环语句
        /// </summary>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="variable">循环变量名</param>
        /// <param name="totalVariable">所有级别循环的变量名，顺序需要和templates代码模板中的替换顺序相同</param>
        /// <param name="lBound">上限</param>
        /// <param name="uBound">下限</param>
        /// <param name="templates">循环体语句模板，需要是字符串替换模板</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateForScript(int indentLevel, string? variable, string[]? totalVariable, string? lBound, string? uBound, string?[]? templates)
        {
            string indent = GetIndent(indentLevel);
            return $"{indent}For {variable ?? string.Empty} = {lBound ?? string.Empty} To {uBound ?? string.Empty}\n{GetScriptWithVariable(indentLevel, templates, totalVariable)}\n{indent}Next";
        }

        /// <summary>
        /// 创建For Each [variable] In [collection] ... Next格式的For循环语句
        /// </summary>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="variable">循环变量名</param>
        /// <param name="totalVariable">所有级别循环的变量名，顺序需要和templates代码模板中的替换顺序相同</param>
        /// <param name="collection">集合变量名</param>
        /// <param name="templates">循环体语句模板，需要是字符串替换模板</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateForEachScript(int indentLevel, string? variable, string?[]? totalVariable, string? collection, string?[]? templates)
        {
            string indent = GetIndent(indentLevel);
            return $"{indent}For Each {variable ?? string.Empty} In {collection ?? string.Empty}\n{GetScriptWithVariable(indentLevel, templates, totalVariable)}\n{indent}Next";
        }

        /// <summary>
        /// 创建While [Test] ... End While格式的While循环语句
        /// </summary>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="test">循环判断语句</param>
        /// <param name="totalVariable">所有级别循环的变量名，顺序需要和templates代码模板中的替换顺序相同</param>
        /// <param name="templates">循环体脚本语句模板</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateWhileScript(int indentLevel, string? test, string?[]? totalVariable, string?[]? templates)
        {
            string indent = GetIndent(indentLevel);
            return $"{indent}While {test ?? string.Empty}\n{GetScriptWithVariable(indentLevel, templates, totalVariable)}\n{indent}End While";
        }

        /// <summary>
        /// 创建Do ... Loop While [TestExpression]格式的DoLoop循环语句
        /// </summary>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="test">循环判断语句</param>
        /// <param name="totalVariable">所有级别循环的变量名，顺序需要和templates代码模板中的替换顺序相同</param>
        /// <param name="templates">循环体脚本语句模板</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateDoLoopWhileScript(int indentLevel, string? test, string?[]? totalVariable, string?[]? templates)
        {
            string indent = GetIndent(indentLevel);
            return $"{indent}Do\n{GetScriptWithVariable(indentLevel, templates, totalVariable)}\n{indent}Loop While {test ?? string.Empty}";
        }

        /// <summary>
        /// 创建Do ... Loop Until [TestExpression]格式的DoLoop循环语句
        /// </summary>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="test">循环判断语句</param>
        /// <param name="totalVariable">所有级别循环的变量名，顺序需要和templates代码模板中的替换顺序相同</param>
        /// <param name="templates">循环体脚本语句模板</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateDoLoopUntilScript(int indentLevel, string? test, string?[]? totalVariable, string?[]? templates)
        {
            string indent = GetIndent(indentLevel);
            return $"{indent}Do\n{GetScriptWithVariable(indentLevel, templates, totalVariable)}\n{indent}Loop Until {test ?? string.Empty}";
        }

        /// <summary>
        /// 创建Do While [TestExpression]... Loop格式的DoLoop循环语句
        /// </summary>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="test">循环判断语句</param>
        /// <param name="totalVariable">所有级别循环的变量名，顺序需要和templates代码模板中的替换顺序相同</param>
        /// <param name="templates">循环体脚本语句模板</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateDoWhileLoopScript(int indentLevel, string? test, string?[]? totalVariable, string?[]? templates)
        {
            string indent = GetIndent(indentLevel);
            return $"{indent}Do While {test ?? string.Empty}\n{GetScriptWithVariable(indentLevel, templates, totalVariable)}\n{indent}Loop";
        }

        /// <summary>
        /// 创建Do Until [TestExpression]... Loop格式的DoLoop循环语句
        /// </summary>
        /// <param name="indentLevel">缩进级别</param>
        /// <param name="test">循环判断语句</param>
        /// <param name="totalVariable">所有级别循环的变量名，顺序需要和templates代码模板中的替换顺序相同</param>
        /// <param name="templates">循环体脚本语句模板</param>
        /// <returns>脚本代码字符串</returns>
        internal static string CreateDoUntilLoopScript(int indentLevel, string? test, string?[]? totalVariable, string?[]? templates)
        {
            string indent = GetIndent(indentLevel);
            return $"{indent}Do Until {test ?? string.Empty}\n{GetScriptWithVariable(indentLevel, templates, totalVariable)}\n{indent}Loop";
        }

    }
}
