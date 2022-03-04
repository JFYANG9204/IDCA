
using System.Collections;

namespace IDCA.Bll.MddDocument
{
    public interface IProperties : IEnumerable, IMDMCollection<IProperty>
    {
        /// <summary>
        /// 依据数值索引获取集合元素
        /// </summary>
        /// <param name="index">数值索引</param>
        /// <returns></returns>
        public IProperty this[int index] { get; }
        /// <summary>
        /// 依据属性名获取集合元素
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <returns></returns>
        public IProperty this[string name] { get; }
        /// <summary>
        /// 当前对象的父级对象，可以是属性集合(IProperties)或者是属性(IProperty)
        /// </summary>
        public object Parent { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties Properties { get; }
    }
}
