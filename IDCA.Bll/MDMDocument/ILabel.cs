﻿
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public interface ILabel
    {
        /// <summary>
        /// 标签文本，XML文档中对应 text 包裹的文本
        /// </summary>
        string Text { get; }
        /// <summary>
        /// 父级对象，标签的父级一定是标签集合
        /// </summary>
        ILabels Parent { get; }
        /// <summary>
        /// 上下文类型
        /// </summary>
        IContext Context { get; }
        /// <summary>
        /// 语言类型
        /// </summary>
        ILanguage Language { get; }
        /// <summary>
        /// 设置上下文、语言类型和内部文本
        /// </summary>
        /// <param name="context"></param>
        /// <param name="language"></param>
        void Set(string context, string language, string text);
    }

    public interface ILabels : IEnumerable, IMDMCollection<ILabel>
    {
        /// <summary>
        /// 依据语言类型或者上下文类型，获取标签对象
        /// </summary>
        /// <param name="lanugage">语言，不区分大小写</param>
        /// <param name="context">上下文类型，不区分大小写，可选</param>
        /// <returns>对应语言的标签，如果未找到，返回null，如果忽略上下文类型的同时查找到多个，返回第一个符合的对象</returns>
        ILabel? this[string language, string context = ""] { get; }
        /// <summary>
        /// 上下文类型
        /// </summary>
        IContext Context { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties? Properties { get; }
        /// <summary>
        /// 父级对象，Labels对象的父级可能是任何MDMObject对象
        /// </summary>
        IMDMObject Parent { get; }
        /// <summary>
        /// 所在的文档对象
        /// </summary>
        IDocument Document { get; }
    }

}
