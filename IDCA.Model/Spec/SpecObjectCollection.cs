
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IDCA.Model.Spec
{
    public class SpecObjectCollection<T> : SpecObject, IEnumerable where T : SpecObject
    {

        internal SpecObjectCollection(Func<SpecObject, T> constructor) : base()
        {
            _objectType = SpecObjectType.Collection;
            _constructor = constructor;
            _items = new List<T>();
        }

        internal SpecObjectCollection(SpecObject parent, Func<SpecObject, T> constructor) : base(parent)
        {
            _objectType = SpecObjectType.Collection;
            _constructor = constructor;
            _items = new List<T>();
        }

        readonly Func<SpecObject, T> _constructor;
        protected List<T> _items;

        /// <summary>
        /// 当前集合内元素的数量
        /// </summary>
        public int Count => _items.Count;
        /// <summary>
        /// 根据数值索引当前位置的对象，如果索引越限会抛出错误
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _items.Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return _items[index];
            }
        }

        /// <summary>
        /// 将新的对象添加进集合
        /// </summary>
        /// <param name="obj"></param>
        public void Add(T obj)
        {
            _items.Add(obj);
        }

        /// <summary>
        /// 将对象插入到索引处，如果索引错误，将插入到最后
        /// </summary>
        /// <param name="index">插入的索引位置</param>
        /// <param name="obj">插入的对象</param>
        /// <param name="callback">对于受影响元素执行的回调函数</param>
        public bool Insert(int index, T obj, Action<T>? callback = null)
        {
            return CollectionHelper.Insert(_items, index, obj, callback);
        }

        /// <summary>
        /// 将对象插入到已有对象之后
        /// </summary>
        /// <param name="previousObj"></param>
        /// <param name="insertObj"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool Insert(T previousObj, T insertObj, Action<T>? callback = null)
        {
            int index = _items.IndexOf(previousObj);
            if (index == -1)
            {
                return false;
            }
            return Insert(index + 1, insertObj, callback);
        }

        /// <summary>
        /// 获取指定集合元素的索引，如果不在当前集合中，返回-1
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// 清空集合内的所有内容
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// 根据回调函数的返回值查找对应的对象
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public T? Find(Predicate<T> match)
        {
            return _items.Find(match);
        }

        /// <summary>
        /// 根据回调函数查找匹配的最后一个对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T? Last(Func<T, bool> predicate)
        {
            return _items.Last(predicate);
        }

        /// <summary>
        /// 根据回调函数结果返回符合条件的已有元素列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> Elements(Func<T, bool> predicate)
        {
            return _items.Where(predicate);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// 创建新的对象，但是不添加进集合
        /// </summary>
        /// <returns></returns>
        public T NewObject()
        {
            return _constructor(this);
        }

        /// <summary>
        /// 调换两个索引位置的内容，如果是无效索引，此方法将直接返回，不做任何操作
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        public bool Swap(int sourceIndex, int targetIndex)
        {
            return CollectionHelper.Swap(_items, sourceIndex, targetIndex);
        }

        /// <summary>
        /// 调换两个指定对象的位置，如果有任意一个对象不在集合内，将返回false
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public bool Swap(T sourceObj, T targetObj)
        {
            int sourceIndex = _items.IndexOf(sourceObj);
            int targetIndex = _items.IndexOf(targetObj);

            if (sourceIndex < 0 || targetIndex < 0)
            {
                return false;
            }

            return CollectionHelper.Swap(_items, sourceIndex, targetIndex);
        }

        /// <summary>
        /// 将指定索引位置的对象向前移动一个位置，移动成功返回true，否则返回false
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool MoveUp(int index)
        {
            return Swap(index, index - 1);
        }

        /// <summary>
        /// 将指定对象向前移动一个位置，如果对象不在集合内或者索引越限，将移动失败，移动成功返回true，否则返回false
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool MoveUp(T obj)
        {
            return CollectionHelper.MoveUp(_items, obj);
        }

        /// <summary>
        /// 将指定索引位置的对象向后移动一个位置，移动成功返回true，否则返回false
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool MoveDown(int index)
        {
            return Swap(index, index + 1);
        }

        /// <summary>
        /// 将指定对象向后移动一个位置，如果对象不在集合内或者索引越限，将移动失败，移动成功返回true，否则返回false
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool MoveDown(T obj)
        {
            return CollectionHelper.MoveDown(_items, obj);
        }

        /// <summary>
        /// 移除特定位置的元素，此方法不会抛出错误
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return;
            }
            _items.RemoveAt(index);
        }

        /// <summary>
        /// 移除集合内的特定元素
        /// </summary>
        /// <param name="obj"></param>
        public bool Remove(T obj)
        {
            return _items.Remove(obj);
        }

        /// <summary>
        /// 根据回调函数的结果删除所有返回值为True的列表元素。
        /// </summary>
        /// <param name="checkIfFunc"></param>
        public void RemoveIf(Func<T, bool> checkIfFunc)
        {
            var removeIndex = new List<int>();
            
            for (int i = 0; i < _items.Count; i++)
            {
                if (checkIfFunc(_items[i]))
                {
                    removeIndex.Add(i);
                }
            }

            if (removeIndex.Count == 0)
            {
                return;
            }

            for (int i = removeIndex.Count - 1; i >= 0; i--)
            {
                _items.RemoveAt(removeIndex[i]);
            }
        }

        /// <summary>
        /// 将当前元素内容转换为数组
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return _items.ToArray();
        }

    }
}
