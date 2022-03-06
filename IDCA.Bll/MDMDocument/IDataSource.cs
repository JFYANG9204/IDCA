
using System.Collections;
using System.Runtime.InteropServices;

namespace IDCA.Bll.MDMDocument
{
    public interface IDataSource
    {
        /// <summary>
        /// 数据库链接名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 数据库路径
        /// </summary>
        string DBLocation { get; }

        string CDSCName { get; }
        /// <summary>
        /// 项目名称
        /// </summary>
        string Project { get; }
        /// <summary>
        /// 编号
        /// </summary>
        string Id { get; }
    }

    public interface IDataSources : IMDMCollection<IDataSource>, IEnumerable
    {
        /// <summary>
        /// 默认数据链接
        /// </summary>
        IDataSource Default { get; }
        /// <summary>
        /// 依据数字索引获取对应位置的数据链接，如果没有找到，返回null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IDataSource this[int index] { get; }
    }

}
