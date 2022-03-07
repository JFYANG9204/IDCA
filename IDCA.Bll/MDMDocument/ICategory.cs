
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public interface ICategories : IMDMCollection<IElement>, IMDMObject, IEnumerable
    {
        /// <summary>
        /// 依据数字索引获取Category集合对象
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>对应位置的对象，超出索引范围，返回null</returns>
        IElement? this[int index] { get; }
        /// <summary>
        /// 依据元素名称获取对应集合对象
        /// </summary>
        /// <param name="name">元素名索引，不区分大小写</param>
        /// <returns>对应名称的集合元素，如果不存在，返回null</returns>
        IElement? this[string name] { get; }
    }
}
