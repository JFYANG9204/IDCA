using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Model.MDM
{
    public class MDMCollection<T> : MDMObject where T : MDMObject
    {
        protected MDMCollection(MDMDocument? document, MDMObject? parent) : base(document, parent)
        {
            _constructor = constructor => (T)new MDMObject(document, parent);
        }

        internal MDMCollection(MDMDocument? document, MDMObject? parent, Func<MDMCollection<T>, T> constructor) : base(document, parent)
        {
            _constructor = constructor;
        }

        protected readonly Func<MDMCollection<T>, T> _constructor;

        protected List<T> _items = new();
        public T? this[int index] => index >= 0 && index < _items.Count ? _items[index] : default;
        public int Count => _items.Count;

        public virtual void Add(T item)
        {
            _items.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public virtual T NewObject()
        {
            return _constructor(this);
        }

        public virtual void Clear()
        {
            _items.Clear();
        }
    }

    public class MDMObjectCollection<T> : MDMCollection<T> where T : MDMObject
    {
        protected MDMObjectCollection(MDMDocument? document, MDMObject? parent) : base(document, parent)
        {
            _objectConstructor = constructor => (T)new MDMObject(document, parent);
        }

        internal MDMObjectCollection(MDMDocument? document, MDMObject? parent, Func<MDMObjectCollection<T>, T> constructor) : base(document, parent)
        { 
            _objectConstructor = constructor;
        }

        protected readonly Func<MDMObjectCollection<T>, T> _objectConstructor;

        public override T NewObject()
        {
            return _objectConstructor(this);
        }

    }

    public class MDMNamedCollection<T> : MDMObjectCollection<T> where T : MDMNamedObject
    {
        internal MDMNamedCollection(MDMDocument? document, MDMObject? parent, Func<MDMNamedCollection<T>, T> constructor) : base(document, parent)
        {
            _namedConstructor = constructor;
        }

        protected readonly Func<MDMNamedCollection<T>, T> _namedConstructor;

        protected readonly Dictionary<string, Element> _itemCache = new();
        protected readonly Dictionary<string, T> _cache = new();
        protected readonly Dictionary<string, T> _idCache = new();

        protected Labels? _labels;
        protected Style? _styles;
        protected Style? _labelStyles;
        protected bool _isReference = false;
        protected bool _isSystem = false;
        protected string _id = string.Empty;
        protected string _name = string.Empty;

        public T? this[string name] => _cache.ContainsKey(name.ToLower()) ? _cache[name.ToLower()] : default;

        public Labels? Labels { get => _labels; internal set => _labels = value; }
        public string Label
        {
            get
            {
                if (_labels != null && _document != null)
                {
                    var label = _labels[_document.Language, _document.Context];
                    if (label != null)
                    {
                        return label.Text;
                    }
                }
                return string.Empty;
            }
        }
        public string Id { get => _id; internal set => _id = value; }
        public string Name { get => _name; internal set => _name = value; }
        public Style? LabelStyles { get => _labelStyles; internal set => _labelStyles = value; }
        public bool IsReference { get => _isReference; internal set => _isReference = value; }
        public bool IsSystem { get => _isSystem; internal set => _isSystem = value; }
        public Style? Style { get => _styles; internal set => _styles = value; }

        public T? GetById(string id)
        {
            string lId = id.ToLower();
            return !string.IsNullOrEmpty(id) && _idCache.ContainsKey(lId) ? _idCache[lId] : null;
        }

        public override void Add(T item)
        {
            string lowerId = item.Id.ToLower();
            if (!string.IsNullOrEmpty(lowerId) && !_idCache.ContainsKey(lowerId))
            {
                base.Add(item);
                _idCache.Add(lowerId, item);
                string lowerName = item.Name.ToLower();
                if (!string.IsNullOrEmpty(lowerName) && !_cache.ContainsKey(lowerName))
                {
                    _cache.Add(lowerName, item);
                }
            }
        }

        public override T NewObject()
        {
            return _namedConstructor(this);
        }

        public override void Clear()
        {
            base.Clear();
            _cache.Clear();
            _idCache.Clear();
        }

    }

}
