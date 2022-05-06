
using System.Collections;

namespace IDCA.Model.MDM
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

    public interface IDataSources : IMDMObjectCollection<IDataSource>, IEnumerable
    {
        /// <summary>
        /// 默认数据链接
        /// </summary>
        IDataSource Default { get; }
    }

}
