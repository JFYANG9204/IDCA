﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
{
    public class MDMCollection<T> : IMDMCollection<T> where T : MDMObject
    {
        protected MDMCollection(IMDMDocument document, IMDMObject parent)
        {
            _document = document;
            _parent = parent;
            _constructor = constructor => (T)new MDMObject(document, parent);
        }

        internal MDMCollection(IMDMDocument document, IMDMObject parent, Func<IMDMCollection<T>, T> constructor)
        {
            _document = document;
            _parent = parent;
            _constructor = constructor;
        }

        protected readonly IMDMDocument _document;
        protected readonly IMDMObject _parent;
        protected readonly Func<IMDMCollection<T>, T> _constructor;

        protected List<T> _items = new();
        public T? this[int index] => index >= 0 && index < _items.Count ? _items[index] : default;
        public int Count => _items.Count;

        public IMDMDocument Document => _document;
        public IMDMObject Parent => _parent;

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

    public class MDMObjectCollection<T> : MDMCollection<T>, IMDMObjectCollection<T> where T : MDMObject
    {
        protected MDMObjectCollection(IMDMDocument document, IMDMObject parent) : base(document, parent)
        {
            _objectConstructor = constructor => (T)new MDMObject(document, parent);
        }

        internal MDMObjectCollection(IMDMDocument document, IMDMObject parent, Func<IMDMObjectCollection<T>, T> constructor) : base(document, parent)
        { 
            _objectConstructor = constructor;
        }

        protected readonly Func<IMDMObjectCollection<T>, T> _objectConstructor;

        protected MDMObjectType _objectType;
        protected Properties? _properties;
        protected Properties? _templates;

        public MDMObjectType ObjectType { get => _objectType; internal set => _objectType = value; }
        public Properties? Properties { get => _properties; internal set => _properties = value; }
        public Properties? Templates { get => _templates; internal set => _templates = value; }

        public override T NewObject()
        {
            return _objectConstructor(this);
        }

    }

    public class MDMNamedCollection<T> : MDMObjectCollection<T>, IMDMNamedCollection<T>, IMDMObject where T : MDMNamedObject
    {
        internal MDMNamedCollection(IMDMDocument document, IMDMObject parent, Func<IMDMNamedCollection<T>, T> constructor) : base(document, parent)
        {
            _namedConstructor = constructor;
        }

        protected readonly Func<IMDMNamedCollection<T>, T> _namedConstructor;

        protected readonly Dictionary<string, IElement> _itemCache = new();
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
                if (_labels != null)
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
