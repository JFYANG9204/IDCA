
namespace IDCA.Bll.SpecDocument
{
    /// <summary>
    /// Spec描述对象的基础接口，所有对象都需要实现这个接口
    /// </summary>
    public interface ISpecObject
    {
        /// <summary>
        /// Spec描述对象类型
        /// </summary>
        SpecObjectType SpecObjectType { get; }
        /// <summary>
        /// 对象所在的文档对象
        /// </summary>
        ISpecDocument Document { get; }
        /// <summary>
        /// 此对象的父级对象
        /// </summary>
        ISpecObject Parent { get; }
    }

    public enum SpecObjectType
    {
        None,
        Document,
        Collection,
        Table,
        Axis,
        AxisElement,
        AxisElementTemplate,
        AxisParameter,
        NewVariable,
    }

}
