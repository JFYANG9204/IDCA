
namespace IDCA.Bll.MDMDocument
{
    public interface IVariable : IMDMLabeledObject, IMDMRange
    {
        MDMDataType DataType { get; }
        Elements? Elements { get; }
        Categories? Categories { get; }
        Properties? Notes { get; }
        Variables? HelperFields { get; }
        VariableUsage UsageType { get; }
        bool HasCaseData { get; }
        bool Versioned { get; }
    }

    public enum VariableUsage
    {
        Variable = 0,
        HelperField = 0x10,
        SourceFile = 272,
        Coding = 528,
        OtherSpecify = 1040,
        Multiplier = 2064,
        Grid = 1,
        Compound = 2,
        Class = 4,
        Array = 8,
        Filter = 0x1000,
        Weight = 0x2000
    }

    public interface IVariableInstance
    {
        /// <summary>
        /// 变量名称，如果不是最上级变量，此属性值应是当前级别变量名，例如：完整变量是Top[{T}].Side的话，此属性是Side
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 完整名称，如果是循环变量的下级变量，则为Top[{T}].Side这种格式
        /// </summary>
        string FullName { get; }
        IVariable? Variable { get; }
        string Expression { get; }
        SourceType SourceType { get; }
    }

    public enum SourceType
    {
        DataField = 1,
        Expression = 2,
        Expressions = 4,
        NoCaseData = 9,
        None = 0x1000
    }

}
