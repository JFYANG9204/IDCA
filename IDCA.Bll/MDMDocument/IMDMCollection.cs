
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public interface IMDMCollection<T> : IEnumerable
    {
        /// <summary>
        /// 数值索引获取集合数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T? this[int index] { get; }
        /// <summary>
        /// 当前集合内元素数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 将属性对象添加到集合的末尾
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);
        /// <summary>
        /// 根据当前对象信息创建新的属性对象，但是不添加进集合
        /// </summary>
        T NewObject();
        /// <summary>
        /// 清空集合
        /// </summary>
        void Clear();
    }

    public interface IMDMObjectCollection<T> : IMDMCollection<T>, IMDMObject
    {
    }

    public interface IMDMNamedCollection<T> : IMDMObjectCollection<T>, IMDMLabeledObject
    {
        /// <summary>
        /// 依据名称索引集合数据，不区分大小写，当有重复名称时，只会返回第一个符合的值。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        T? this[string name] { get; }
        /// <summary>
        /// 依据集合内变量ID编号获取对应数据，不区分大小写
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? GetById(string id);
    }


}
