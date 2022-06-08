
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
        /// 调换集合内两个对象的位置，如果两个对象有任意一个不在集合中，将调换失败，返回false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool Swap<T>(IList<T> collection, T obj1, T obj2)
        {
            int index1 = collection.IndexOf(obj1);
            int index2 = collection.IndexOf(obj2);

            if (index1 < 0 || index2 < 0)
            {
                return false;
            }

            return Swap(collection, index1, index2);
        }

        /// <summary>
        /// 将集合中的对象向前移动一个位置，如果此对象不在集合中或者是第一个对象，
        /// 将移动失败，返回false，否则返回true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool MoveUp<T>(IList<T> collection, T obj)
        {
            int index = collection.IndexOf(obj);
            if (index <= 0)
            {
                return false;
            }
            return Swap(collection, index, index - 1);
        }

        /// <summary>
        /// 将集合中的对象向后移动一个位置，如果此对象不在集合中或者是最后一个对象，
        /// 将移动失败，返回false，否则返回true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool MoveDown<T>(IList<T> collection, T obj)
        {
            int index = collection.IndexOf(obj);
            if (index < 0 || index >= collection.Count - 1)
            {
                return false;
            }
            return Swap(collection, index, index + 1);
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
