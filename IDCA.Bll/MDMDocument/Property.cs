﻿
namespace IDCA.Bll.MDMDocument
{
    public class Property : MDMNamedObject, IProperty
    {
        internal Property(IMDMObject parent) : base(parent.Document, parent)
        {
            _name = "";
            _value = "";
            _valueType = PropertyValueType.Text;
            _context = "";
        }

        object _value;
        PropertyValueType _valueType;
        string _context;

        public object Value { get => _value; internal set => _value = value; }
        public PropertyValueType Type { get => _valueType; internal set => _valueType = value; }
        public string Context { get => _context; internal set => _context = value; }
    }


    public class Properties : MDMNamedCollection<Property>, IProperties<Property>
    {
        internal Properties(IMDMObject parent) : base(parent.Document, parent, collection => new Property(collection))
        {
        }

        public IProperties<Property>? SubProperties { get => _properties; set => _properties = value; }

        public override void Add(Property item)
        {
            string lName = item.Name.ToLower();
            if (!string.IsNullOrEmpty(lName) && !_cache.ContainsKey(lName))
            {
                _cache.Add(lName, item);
                _items.Add(item);
            }
        }
    }
}
