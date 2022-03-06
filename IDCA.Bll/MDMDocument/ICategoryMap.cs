
namespace IDCA.Bll.MDMDocument
{
    public interface ICategoryMap
    {
        /// <summary>
        /// 当前集合中的元素数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 向集合中添加新的元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void Add(string name, string value);
    }

    public struct CategoryId
    {
        public string Name;
        public string Value;
    }

}
