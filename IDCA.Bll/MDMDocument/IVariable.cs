
namespace IDCA.Bll.MddDocument
{
    public interface IVariable : IField
    {
        IElements Elements { get; }
        MDMDataType DataType { get; }
        ITypes HelperFields { get; }
        VariableUsage UsageType { get; }
        bool HasCaseData { get; }
        bool Versioned { get; }
    }

    public enum VariableUsage
    {
        Variable = 0,
        HelperField = 272,
        SourceFile,
        Coding = 528,
        OtherSpecify = 1040,
        Multiplier = 2064,
        Grid = 1,
        Compound = 2,
        Class = 4,
        Array = 8,
        Filter,
        Weight
    }

    public interface IVariableInstance : IVariable
    {
        IVariable Variable { get; }
        string Expression { get; }
        SourceType SourceType { get; }

    }

    public enum SourceType
    {
        DataField,
        Expression,
        Expressions,
        NoCaseData,
        None
    }

}
