
using System.Collections;

namespace IDCA.Bll.MDMDocument
{

    /// <summary>
    /// Types对应MDM文档中用define关键字定义的列表类型变量
    /// </summary>
    public interface IType : IMDMLabeledObject
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        ICategories Categories { get; }

        bool GlobalNamespace { get; }
    }

    public interface ITypes<T> : IMDMNamedCollection<T>, IMDMObject, IEnumerable where T : IType
    {
        bool GlobalNamespace { get; }
    }
}
