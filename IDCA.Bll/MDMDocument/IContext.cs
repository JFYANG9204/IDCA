
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
        IContextAlternatives Alternatives { get; }
        /// <summary>
        /// 当前上下文类型描述，没有描述返回空字符串
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 当前上下文类型用处
        /// </summary>
        ContextUsage Usage { get; }
    }

    public interface IContexts : IMDMCollection<IContext>
    {
        string Base { get; }
    }

    public interface IContextAlternatives : IEnumerable, IMDMCollection<string>
    {
        /// <summary>
        /// 依据数值索引获取上下文配置对象
        /// </summary>
        /// <param name="index">数值索引</param>
        /// <returns>上下文类型字符串</returns>
        string this[int index] { get; }
    }

    public enum ContextUsage
    {
        Routings,
        Labels,
        Properties,
    }


}
