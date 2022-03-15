
namespace IDCA.Bll.Template
{

    public enum TemplateType
    {
        File,
        Script,
    }

    public interface ITemplate
    {
        /// <summary>
        /// 模板类型
        /// </summary>
        TemplateType Type { get; }
        /// <summary>
        /// 模板参数集合
        /// </summary>
        ITemplateParameters<TemplateParameter> Parameters { get; }
        /// <summary>
        /// 按照当前的参数配置进行文本修改，返回最终结果
        /// </summary>
        /// <returns></returns>
        string Exec();
        /// <summary>
        /// 文件文本模板内容，所有需要替换的内容需要满足格式 ：$[variable]，变量名不区分大小写
        /// </summary>
        string Content { get; }
    }

    public interface IFileTemplate : ITemplate
    {
        /// <summary>
        /// 模板文件路径
        /// </summary>
        string Directory { get; }
        /// <summary>
        /// 文件名，包含文件名和扩展名
        /// </summary>
        string FileName { get; }
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
        DoWhileStatement,
        DoUntilStatement,
    }

    /// <summary>
    /// 脚本模板，用于表示各种脚本语句，例如：If ... Then ..。
    /// 但是不包括具体表达式的内容
    /// </summary>
    public interface IScriptTemplate : ITemplate
    {
        /// <summary>
        /// 脚本类型标记
        /// </summary>
        ScriptTemplateFlags Flag { get; }
        /// <summary>
        /// 脚本的缩进级别
        /// </summary>
        int IndentLevel { get; }
    }

    public enum ExpressionTemplateFlags
    {
        Field,
        Function,
        CallExpression,
        BinaryExpression,
        UnaryExpression,
    }

    /// <summary>
    /// 表达式模板，用于表示各种表达式的内容，例如：ContainsAny，ContainsAll
    /// </summary>
    public interface IExpressionTemplate : ITemplate
    {
        /// <summary>
        /// 表达式模板类型
        /// </summary>
        ExpressionTemplateFlags Flag { get; }
    }

    /// <summary>
    /// 多级变量定义模板，可用于表示多级变量，例如：Top[code].Side
    /// </summary>
    public interface IFieldExpressionTemplate : IExpressionTemplate
    {
        /// <summary>
        /// 当前变量级别，如果不是多级循环变量，值应该是0
        /// </summary>
        public int Level { get; }
        /// <summary>
        /// 添加级别到当前列表的末尾
        /// </summary>
        /// <param name="codeName">码号位置的字符</param>
        /// <param name="variableName">下级变量名</param>
        /// <param name="isCategorical">是否是Categorical类型</param>
        public void PushLevel(string codeName, string variableName, bool isCategorical);
    }

    /// <summary>
    /// 调用函数的表达式模板，可以是通过.调用，也可以是直接调用，函数的具体参数根据Parameter属性
    /// 模板参数填充。
    /// </summary>
    public interface ICallExpressionTemplate : IExpressionTemplate
    {
        /// <summary>
        /// 调用对象名称，例如：Object.ContainsAny中的Object，如果不通过.操作符调用，
        /// 此属性的值赋值为空
        /// </summary>
        string Object { get; }
        /// <summary>
        /// 调用的函数名称
        /// </summary>
        string FunctionName { get; }
    }

    public enum BinaryOperatorFlags
    {
        Asterisk,
        Slash,
        Plus,
        Min,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        Equal,
        NotEqual,
    }

    /// <summary>
    /// 二元表达式的模板信息，二元操作符包括：+、-、*、/、>、>=、<、<=、=、<>
    /// </summary>
    public interface IBinaryExpressionTemplate : IExpressionTemplate
    {
        /// <summary>
        /// 当前的二元操作符类型
        /// </summary>
        BinaryOperatorFlags OperatorFlag { get; }
    }

}
