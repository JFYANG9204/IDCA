
using System.Collections;

namespace IDCA.Bll.MDM
{

    /// <summary>
    /// Types对应MDM文档中用define关键字定义的列表类型变量
    /// </summary>
    public interface IType : IMDMLabeledObject
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        Categories Categories { get; }

        bool GlobalNamespace { get; }
    }

    public interface ITypes<T> : IMDMNamedCollection<T>, IMDMObject, IEnumerable where T : IType
    {
        bool GlobalNamespace { get; }
    }
}
