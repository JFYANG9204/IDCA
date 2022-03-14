
namespace IDCA.Bll.FileBuilder
{
    /// <summary>
    /// 模板接口，所有可用于生成文件的模板都需要实现这个接口
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// 模板的唯一标识符，在全局是唯一的
        /// </summary>
        string Id { get; }

    }

    /// <summary>
    /// 文件模板接口，所有可用于文本替换的文件模板都需要实现这个接口
    /// </summary>
    public interface IFileTemplate
    {
        /// <summary>
        /// 文件名，包括扩展名，不包括文件夹路径
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// 文件夹路径，相对于根路径
        /// </summary>
        string Directory { get; }
        /// <summary>
        /// 文件内容
        /// </summary>
        string Content { get; }
        /// <summary>
        /// 需要替换的文本数量
        /// </summary>
        int ReplacementCount { get; }
        /// <summary>
        /// 替换文本内容
        /// </summary>
        /// <param name="replacements"></param>
        void Replace(params string[] replacements);
    }

}
