
using System.Collections;
using System.Runtime.InteropServices;

namespace IDCA.Bll.MDMDocument
{
    public interface ILabel
    {
        /// <summary>
        /// 标签名，XML文档中对应 text 包裹的文本
        /// </summary>
        string Name { get; internal set; }
        /// <summary>
        /// 父级文档对象
        /// </summary>
        IDocument Document { get; }
        /// <summary>
        /// 父级对象，标签的父级一定是标签集合
        /// </summary>
        ILabels Parent { get; }
        /// <summary>
        /// 上下文类型
        /// </summary>
        string Context { get; internal set; }
        /// <summary>
        /// 语言类型
        /// </summary>
        ILanguage Language { get; internal set; }
    }

    public interface ILabels : IEnumerable, IMDMCollection<ILabel>
    {
        /// <summary>
        /// 依据语言类型或者上下文类型，获取标签对象
        /// </summary>
        /// <param name="lanugage">语言，不区分大小写</param>
        /// <param name="context">上下文类型，不区分大小写，可选</param>
        /// <returns>对应语言的标签，如果未找到，返回null，如果忽略上下文类型的同时查找到多个，返回第一个符合的对象</returns>
        ILabel this[string lanugage, [Optional] string context] { get; }
        /// <summary>
        /// 上下文类型
        /// </summary>
        string Context { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties Properties { get; }
    }

}
