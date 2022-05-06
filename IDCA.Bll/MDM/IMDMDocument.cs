
using System.Collections.Generic;

namespace IDCA.Model.MDM
{
    public interface IMDMDocument : IMDMObject
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
        /// 文档ID编号
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 数据版本
        /// </summary>
        string DataVersion { get; }
        /// <summary>
        /// 数据下级版本
        /// </summary>
        string DataSubVersion { get; }
        /// <summary>
        /// 是否支持系统变量
        /// </summary>
        bool SystemVariable { get; }
        /// <summary>
        /// 是否对数据库筛选器进行有效性检查
        /// </summary>
        bool DbFilterValidation { get; }
        /// <summary>
        /// 当前上下文类型
        /// </summary>
        string Context { get; }
        /// <summary>
        /// 当前语言类型
        /// </summary>
        string Language { get; }
        /// <summary>
        /// 数据库链接集合
        /// </summary>
        DataSources DataSources { get; }
        /// <summary>
        /// 文档名称标签
        /// </summary>
        Labels Labels { get; }
        /// <summary>
        /// 变量定义集合
        /// </summary>
        Variables Variables { get; }
        /// <summary>
        /// 变量设计集合
        /// </summary>
        Fields Fields { get; }
        /// <summary>
        /// 定义的类型集合，在XML中由 types 标签包裹
        /// </summary>
        Types Types { get; }
        /// <summary>
        /// 页面属性配置集合
        /// </summary>
        Pages Pages { get; }
        /// <summary>
        /// 变量路由配置
        /// </summary>
        Routings Routings { get; }
        /// <summary>
        /// 系统变量路由配置
        /// </summary>
        Routings SystemRoutings { get; }
        /// <summary>
        /// 变量实例列表
        /// </summary>
        Mapping Mapping { get; }
        /// <summary>
        /// 文档的所有语言配置
        /// </summary>
        Languages Languages { get; }
        /// <summary>
        /// 文档的所有上下文类型
        /// </summary>
        Contexts Contexts { get; }
        /// <summary>
        /// 文档的所有标签类型
        /// </summary>
        Contexts LabelTypes { get; }
        /// <summary>
        /// 文档的所有上下文路由
        /// </summary>
        Contexts RoutingContexts { get; }
        /// <summary>
        /// 脚本类型
        /// </summary>
        Contexts ScriptTypes { get; }
        /// <summary>
        /// 分类元素表
        /// </summary>
        CategoryMap CategoryMap { get; }
        /// <summary>
        /// 文档元素名称
        /// </summary>
        List<string> Atoms { get; }
        /// <summary>
        /// 用户存储信息
        /// </summary>
        SaveLogs SaveLogs { get; }
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
        /// 文档的父级对象应为空
        /// </summary>
        new IMDMObject? Parent { get; }
        /// <summary>
        /// 设定当前的上下文类型，需要当前Contexts集合中存在这个名称
        /// </summary>
        /// <param name="context"></param>
        void SetContext(string context);
        /// <summary>
        /// 设定当前的语言类型，需要当前Langauges集合中存在这个名称
        /// </summary>
        /// <param name="language"></param>
        void SetLanguage(string language);
    }
}
