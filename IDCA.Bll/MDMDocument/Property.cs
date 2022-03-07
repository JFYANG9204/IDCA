using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
{
    public class Property : IProperty
    {
        internal Property(IProperties parent)
        {
            _parent = parent;
            _name = "";
            _value = "";
            _valueType = PropertyValueType.Text;
            _context = "";
        }

        readonly IProperties _parent;
        string _name;
        object _value;
        PropertyValueType _valueType;
        string _context;

        public string Name { get => _name; internal set => _name = value; }

        public object Value { get => _value; internal set => _value = value; }

        public PropertyValueType Type { get => _valueType; internal set => _valueType = value; }

        public string Context { get => _context; internal set => _context = value; }

        public IProperties Parent => _parent;
    }


    public class Properties : IProperties
    {
        internal Properties(object parent)
        {
            _parent = parent;
        }

        readonly object _parent;
        readonly List<IProperty> _items = new();
        readonly Dictionary<string, IProperty> _cache = new();
        IProperties? _properties = null;

        public IProperty? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;

        public IProperty? this[string name] => _cache.ContainsKey(name.ToLower()) ? _cache[name.ToLower()] : null;

        public object Parent => _parent;

        public int Count => _items.Count;

        public IProperties? SubProperties { get => _properties; set => _properties = value; }

        public void Add(IProperty item)
        {
            string lName = item.Name.ToLower();
            if (!string.IsNullOrEmpty(lName) && !_cache.ContainsKey(lName))
            {
                _cache.Add(lName, item);
                _items.Add(item);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IProperty NewObject()
        {
            return new Property(this);
        }
    }
}
