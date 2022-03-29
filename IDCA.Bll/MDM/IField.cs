
namespace IDCA.Bll.MDM
{
    public interface IField : IMDMLabeledObject, IMDMRange
    {
        /// <summary>
        /// 引用的对象，不引用返回null
        /// </summary>
        Variable? Reference { get; }
        /// <summary>
        /// 当前对象的Category分类集合，不存在返回null
        /// </summary>
        Categories? Categories { get; }
        /// <summary>
        /// 所有的Element对象集合，脚本中由ElementType定义
        /// </summary>
        Elements? Elements { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        MDMDataType DataType { get; }
        /// <summary>
        /// 下级变量集合，用于表示多级变量
        /// </summary>
        Class? Class { get; }
        /// <summary>
        /// 迭代器类型
        /// </summary>
        IteratorType? IteratorType { get; }
        /// <summary>
        /// 如果迭代器为数值类型，此属性保存区间上限
        /// </summary>
        int? UpperBound { get; }
        /// <summary>
        /// 如果迭代器为数值类型，此属性保存区间下限
        /// </summary>
        int? LowerBound { get; }
    }

    public enum IteratorType
    {
        None = 0,
        Categorical = 2,
        NumericRanges = 3,
    }

}
