
namespace IDCA.Bll.MDMDocument
{
    public interface IMDMCollection<T>
    {
        /// <summary>
        /// 当前集合内元素数量
        /// </summary>
        public int Count { get; }
        /// <summary>
        /// 将属性对象添加到集合的末尾
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item);
        /// <summary>
        /// 根据当前对象信息创建新的属性对象，但是不添加进集合
        /// </summary>
        public T NewObject();

    }
}
