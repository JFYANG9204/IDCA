
using System.Collections;

namespace IDCA.Bll.MDM
{

    public interface IElement : IMDMLabeledObject
    {
        new Properties? Templates { get; }
        IElement? Reference { get; }
        ElementType ElementType { get; }
        CategoryFlag Flag { get; }
        Variable? OtherReference { get; }
        Variable? OtherVariable { get; }
        /// <summary>
        /// MDM元素的Factor可以是数值类型或字符串类型
        /// 在脚本中，系统自动完成转换：
        /// - 如果是整数，转换成长整型
        /// - 如果是小数，转换成浮点数
        /// - 其他情况，转换成字符串
        /// </summary>
        object? Factor { get; }
        /// <summary>
        /// 标记Factor数据类型，可以是长整型、浮点型或字符串类型
        /// </summary>
        FactorType? FactorType { get; }
        bool IsOtherLocal { get; }
        Variable? MultiplierReference { get; }
        Variable? MultiplierVariable { get; }
        bool IsMultiplierLocal { get; }
        bool Versioned { get; }
    }

    public interface IElements : IMDMNamedCollection<Element>, IEnumerable
    {
    }


    public enum ElementType
    {
        Category = 0,
        AnalysisSubheading = 1,
        AnalysisBase = 2,
        AnalysisSubtotal = 3,
        AnalysisSummaryData = 4,
        AnalysisDerived = 5,
        AnalysisTotal = 6,
        AnalysisMean = 7,
        AnalysisStdDev = 8,
        AnalysisStdErr = 9,
        AnalysisSampleVariance = 10,
        AnalysisMinimun = 11,
        AnalysisMaximun = 12,
        AnalysisCategory = 14,
    }

    public enum CategoryFlag
    {
        None,
        User,
        DontKnow,
        Refuse,
        Noanswer,
        Other,
        Multiplier,
        Exclusive,
        FixedPosition,
        NoFilter,
        Inline
    }

    public enum FactorType
    {
        None = 0,
        Long = 3,
        Double = 5,
        Text = 8,
    }
}
