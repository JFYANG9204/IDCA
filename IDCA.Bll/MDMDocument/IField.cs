
namespace IDCA.Bll.MDMDocument
{
    public interface IField : IMDMLabeledObject, IMDMRange
    {
        /// <summary>
        /// 引用的对象，不引用返回null
        /// </summary>
        IField? Reference { get; }
        /// <summary>
        /// 当前对象的Category分类集合，不存在返回null
        /// </summary>
        ICategories? Categories { get; }
        /// <summary>
        /// 所有的Element对象集合，脚本中由ElementType定义
        /// </summary>
        IElements? Elements { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        MDMDataType DataType { get; }
        /// <summary>
        /// 是否是系统变量，在XML文档中的system标签下
        /// </summary>
        bool IsSystemVariable { get; }
        /// <summary>
        /// 下级变量集合，用于表示多级变量
        /// </summary>
        IMDMObjectCollection<Variable>? Items { get; }
    }

}
