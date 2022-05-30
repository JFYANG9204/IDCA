
using System;
using System.Collections.Generic;

namespace IDCA.Model
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
        public static bool Swap<T>(IList<T> collection, int sourceIndex, int targetIndex)
        {
            if (collection == null ||
                sourceIndex < 0 || sourceIndex >= collection.Count ||
                targetIndex < 0 || targetIndex >= collection.Count ||
                targetIndex == sourceIndex)
            {
                return false;
            }
            (collection[targetIndex], collection[sourceIndex]) = (collection[sourceIndex], collection[targetIndex]);
            return true;
        }
        /// <summary>
        /// 遍历集合中的各元素并执行回调函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="callback"></param>
        public static void ForEach<T>(ICollection<T> collection, Action<T> callback)
        {
            foreach (var child in collection)
            {
                callback(child);
            }
        }

    }

}
