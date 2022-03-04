
namespace IDCA.Bll.MddDocument
{
    public interface IField : IMDMLabeledObject, IMDMRange
    {
        /// <summary>
        /// 引用的对象，不引用返回null
        /// </summary>
        IField Reference { get; }
        /// <summary>
        /// 当前对象的Category分类集合，不存在返回null
        /// </summary>
        ICategories Categories { get; }

        bool IsSystemVariable { get; }
    }


}
