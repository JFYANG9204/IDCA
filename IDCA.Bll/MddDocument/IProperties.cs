using System.Runtime.InteropServices;

namespace IDCA.Bll.MddDocument
{
    /// <summary>
    /// 存储文档各单元属性的集合
    /// </summary>
    public interface IProperties
    {
        /// <summary>
        /// 修改或获取对应名称的属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="context">上下文类型</param>
        /// <returns></returns>
        object this[[In] object name, [Optional][In] object context] { get; [param: In] set; }

        /// <summary>
        /// 当前存储的属性数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 属性集合名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 父级文档对象
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        /// 对应上下文类型的名称
        /// </summary>
        string NameByContext { get; }

        /// <summary>
        /// 移除对应上下文类型和名称的属性值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="context">上下文类型</param>
        void Remove([In] object name, [Optional][In] bool context);
    }
}
