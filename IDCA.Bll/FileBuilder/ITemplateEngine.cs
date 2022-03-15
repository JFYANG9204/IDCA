
namespace IDCA.Bll.FileBuilder
{
    /// <summary>
    /// 模板引擎接口，所有可通过模板生成代码或代码片段的对象都需要实现这个接口
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// 模板引擎的编码，全局范围内唯一
        /// </summary>
        string EngineCode { get; }

    }



}
