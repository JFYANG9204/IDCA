
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.Spec
{
    public class SpecObjectCollection<T> : SpecObject where T : SpecObject
    {
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

    }
}
