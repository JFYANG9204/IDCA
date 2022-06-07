
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

        /// <summary>
        /// 向列表中的指定索引位置插入元素，并对受到影响的元素执行回调函数。
        /// 如果索引无效，将会把元素插入到列表最后。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static bool Insert<T>(IList<T> list, int index, T obj, Action<T>? callback = null)
        {
            if (index >= 0 && index < list.Count)
            {
                list.Insert(index, obj);
                if (callback != null)
                {
                    for (int i = index + 1; i < list.Count; i++)
                    {
                        callback(list[i]);
                    }
                }
                return true;
            }
            else
            {
                list.Add(obj);
                return false;
            }
        }

    }

}
