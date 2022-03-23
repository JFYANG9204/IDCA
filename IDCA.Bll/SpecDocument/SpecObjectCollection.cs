
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.SpecDocument
{
    public class SpecObjectCollection<T> : SpecObject, ISpecObjectCollection<T> where T : ISpecObject
    {
        internal SpecObjectCollection(ISpecObject parent, Func<ISpecObject, T> constructor) : base(parent)
        {
            _objectType = SpecObjectType.Collection;
            _constructor = constructor;
        }

        readonly Func<ISpecObject, T> _constructor;
        protected List<T> _items = new();

        public int Count => _items.Count;
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

        public void Add(T obj)
        {
            _items.Add(obj);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public T NewObject()
        {
            return _constructor(this);
        }

    }
}
