
using System.Collections.Generic;

namespace IDCA.Bll
{
    public static class CollectionHelper
    {
        /// <summary>
        /// 交换列表中指定两个索引的值，如果索引错误，此函数不做任何操作，也不会抛出错误。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        public static void Swap<T>(IList<T> collection, int sourceIndex, int targetIndex)
        {
            if (collection == null ||
                sourceIndex < 0 || sourceIndex >= collection.Count ||
                targetIndex < 0 || targetIndex >= collection.Count ||
                targetIndex == sourceIndex)
            {
                return;
            }
            (collection[targetIndex], collection[sourceIndex]) = (collection[sourceIndex], collection[targetIndex]);
        }

    }
}
