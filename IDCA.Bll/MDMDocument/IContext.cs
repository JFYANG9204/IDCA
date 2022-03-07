
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public interface IContext
    {
        /// <summary>
        /// 上下文类型名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 可替换的其他上下文类型
        /// </summary>
        IContextAlternatives? Alternatives { get; }
        /// <summary>
        /// 当前上下文类型描述，没有描述返回空字符串
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 当前上下文类型用处
        /// </summary>
        ContextUsage Usage { get; }
        /// <summary>
        /// 创建新的IContextAlternatives集合对象后将其赋值给Alternatives属性并返回
        /// </summary>
        /// <returns></returns>
        IContextAlternatives NewAlternatives();
        /// <summary>
        /// 父级Context集合对象
        /// </summary>
        IContexts Parent { get; }
        /// <summary>
        /// 当Name属性是空字符串时，对象是默认值
        /// </summary>
        bool IsDefault { get; }
    }

    public interface IContexts : IMDMCollection<IContext>
    {
        /// <summary>
        /// 依据上下文类型名获取相应对象，如果不存在，返回默认值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IContext this[string name] { get; }
        string Base { get; }
        /// <summary>
        /// 所在文档对象
        /// </summary>
        IDocument Document { get; }
        /// <summary>
        /// 默认值，用于替换null
        /// </summary>
        IContext Default { get; }
    }

    public interface IContextAlternatives : IEnumerable, IMDMCollection<string>
    {
        /// <summary>
        /// 依据数值索引获取上下文配置对象
        /// </summary>
        /// <param name="index">数值索引</param>
        /// <returns>上下文类型字符串</returns>
        string this[int index] { get; }
        /// <summary>
        /// 父级IContext对象
        /// </summary>
        IContext Parent { get; }
    }

    public enum ContextUsage
    {
        Routings,
        Labels,
        Properties,
    }


}
