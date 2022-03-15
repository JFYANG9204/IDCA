
namespace IDCA.Bll.SpecDocument
{
    public interface ITable : ISpecObject
    {
        /// <summary>
        /// Table类型
        /// </summary>
        TableType Type { get; }
        /// <summary>
        /// Table所用的变量名
        /// </summary>
        string Variable { get; }
        /// <summary>
        /// Table所用的表头变量名
        /// </summary>
        string Header { get; }
        /// <summary>
        /// Table标题名称
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Base名称
        /// </summary>
        string BaseText { get; }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
    }

    public enum TableType
    {
        Normal,
        Grid,
        GridSlice,
        ResponseSummary,
        MeanSummary,
    }

}
