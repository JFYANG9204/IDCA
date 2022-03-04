
using System.Collections.Generic;

namespace IDCA.Bll.MddDocument
{
    public interface IDocument : IMDMLabeledObject
    {
        /// <summary>
        /// MDM文档完整路径
        /// </summary>
        string Url { get; }
        /// <summary>
        /// 文档创建时的版本
        /// </summary>
        string CreateVersion { get; }
        /// <summary>
        /// 文档最新的版本
        /// </summary>
        string LastVersion { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties Properties { get; }
        /// <summary>
        /// 模板配置集合
        /// </summary>
        IProperties Templates { get; }
        /// <summary>
        /// 数据库链接集合
        /// </summary>
        IDataSources DataSources { get; }
        /// <summary>
        /// 文档的所有语言配置
        /// </summary>
        ILanguages Languages { get; }
        /// <summary>
        /// 文档的所有上下文类型
        /// </summary>
        IContexts Contexts { get; }
        /// <summary>
        /// 文档的所有标签类型
        /// </summary>
        IContexts LabelTypes { get; }
        /// <summary>
        /// 文档的所有上下文路由
        /// </summary>
        IContexts RoutingContexts { get; }
        /// <summary>
        /// 脚本类型
        /// </summary>
        IContexts ScriptTypes { get; }
        /// <summary>
        /// 分类元素表
        /// </summary>
        ICategoryMap CategoryMap { get; }
        /// <summary>
        /// 文档元素名称
        /// </summary>
        List<string> Atoms { get; }
        /// <summary>
        /// 用户存储信息
        /// </summary>
        ISaveLogs SaveLogs { get; }
        /// <summary>
        /// 文档存储的脚本，没有脚本将返回null
        /// </summary>
        string Script { get; }
        /// <summary>
        /// 打开对应路径文档
        /// </summary>
        /// <param name="path">文档完整路径</param>
        void Open(string path);
        /// <summary>
        /// 关闭文档
        /// </summary>
        void Close();
        /// <summary>
        /// 清空文档对象内容
        /// </summary>
        void Clear();
        /// <summary>
        /// 文档的XML内容
        /// </summary>
        string Xml { get; }
    }
}
