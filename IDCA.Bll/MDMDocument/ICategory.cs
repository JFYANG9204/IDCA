
namespace IDCA.Bll.MDMDocument
{
    public interface ICategory
    {
        string Name { get; }
        string Id { get; }
        /// <summary>
        /// 标签集合，可以包含多种语言和上下文类型的标签对象
        /// </summary>
        ILabels Labels { get; }
        /// <summary>
        /// Cateogory对象的父级对象一定是Categories对象
        /// </summary>
        ICategories Parent { get; }
        /// <summary>
        /// 所在的文档对象
        /// </summary>
        IDocument Document { get; }
    }

    public interface ICategories : IMDMCollection<ICategory>
    {
        /// <summary>
        /// 依据数字索引获取Category集合对象
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>对应位置的对象，超出索引范围，返回null</returns>
        ICategory this[int index] { get; }
        /// <summary>
        /// 依据元素名称获取对应集合对象
        /// </summary>
        /// <param name="name">元素名索引，不区分大小写</param>
        /// <returns>对应名称的集合元素，如果不存在，返回null</returns>
        ICategory this[string name] { get; }
        /// <summary>
        /// 所在的文档对象
        /// </summary>
        IDocument Document { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties Properties { get; }
    }
}
