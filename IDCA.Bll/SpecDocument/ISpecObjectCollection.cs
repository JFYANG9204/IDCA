using System.Collections;

namespace IDCA.Bll.SpecDocument
{
    public interface ISpecObjectCollection<T> : ISpecObject, IEnumerable
    {
        /// <summary>
        /// 根据数值索引当前位置的对象，如果索引越限会抛出错误
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] { get; }
        /// <summary>
        /// 当前集合内元素的数量
        /// </summary>
        public int Count { get; }
        /// <summary>
        /// 创建新的对象，但是不添加进集合
        /// </summary>
        /// <returns></returns>
        public T NewObject();
        /// <summary>
        /// 将新的对象添加进集合
        /// </summary>
        /// <param name="obj"></param>
        public void Add(T obj);
        /// <summary>
        /// 清空集合内的所有内容
        /// </summary>
        public void Clear();
    }
}
