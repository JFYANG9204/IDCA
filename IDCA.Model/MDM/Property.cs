
namespace IDCA.Model.MDM
{
    public class Property : MDMNamedObject
    {
        internal Property(MDMObject? parent) : base(parent?.Document, parent)
        {
            _name = "";
            _value = "";
            _valueType = PropertyValueType.Text;
            _context = "";
            _objectType = MDMObjectType.Property;
        }

        object _value;
        PropertyValueType _valueType;
        string _context;
        public object Value { get => _value; internal set => _value = value; }
        public PropertyValueType Type { get => _valueType; internal set => _valueType = value; }
        public string Context { get => _context; internal set => _context = value; }
    }

    /// <summary>
    /// 属性值类型，可以是整数、实数、字符串、集合和布尔类型，其中，布尔类型时，-1为true，0为false
    /// </summary>
    public enum PropertyValueType
    {
        /// <summary>
        /// 整数数值类型
        /// </summary>
        Integer = 3,
        /// <summary>
        /// 浮点数类型
        /// </summary>
        Decimal = 5,
        /// <summary>
        /// 字符串
        /// </summary>
        Text = 8,
        /// <summary>
        /// 次级属性集合
        /// </summary>
        Collection = 9,
        /// <summary>
        /// 布尔类型，XML数据中，-1为true，0为false
        /// </summary>
        Bool = 11,
    }

    public class Properties : MDMNamedCollection<Property>
    {
        internal Properties(MDMObject? parent) : base(parent?.Document, parent, collection => new Property(collection))
        {
            _objectType = MDMObjectType.Properties;
        }
        public Properties? SubProperties { get => _properties; set => _properties = value; }

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
