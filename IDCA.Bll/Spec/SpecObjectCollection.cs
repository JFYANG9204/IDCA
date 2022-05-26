
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Model.Spec
{
    public class SpecObjectCollection<T> : SpecObject, IEnumerable where T : SpecObject
    {

        internal SpecObjectCollection(Func<SpecObject, T> constructor) : base()
        {
            _objectType = SpecObjectType.Collection;
            _constructor = constructor;
        }

        internal SpecObjectCollection(SpecObject parent, Func<SpecObject, T> constructor) : base(parent)
        {
            _objectType = SpecObjectType.Collection;
            _constructor = constructor;
        }

        readonly Func<SpecObject, T> _constructor;
        protected List<T> _items = new();

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
        /// 清空集合内的所有内容
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

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
        public void Swap(int sourceIndex, int targetIndex)
        {
            CollectionHelper.Swap(_items, sourceIndex, targetIndex);
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

            for (int i = removeIndex.Count - 1; i <= 0; i--)
            {
                _items.RemoveAt(removeIndex[i]);
            }
        }

    }
}
