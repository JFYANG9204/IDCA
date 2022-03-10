
namespace IDCA.Bll.MDMDocument
{
    public class Field : MDMLabeledObject, IField
    {

        internal Field(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Field;
        }

        Field? _reference;
        Categories? _categories;
        Elements? _elements;

        Variables? _items;
        MDMDataType _type = MDMDataType.None;

        object _minValue = string.Empty;
        object _maxValue = string.Empty;
        object _effectiveMinValue = string.Empty;
        object _effectiveMaxValue = string.Empty;

        new public MDMObjectType ObjectType => _objectType;

        public Field? Reference { get => _reference; internal set => _reference = value; }
        public Categories? Categories { get => _categories; internal set => _categories = value; }
        public Elements? Elements { get => _elements; internal set => _elements = value; }
        public MDMDataType DataType { get => _type; internal set => _type = value; }

        public object MinValue { get => _minValue; internal set => _minValue = value; }
        public object MaxValue { get => _maxValue; internal set => _maxValue = value; }
        public object EffectiveMinValue { get => _effectiveMinValue; internal set => _effectiveMinValue = value; }
        public object EffectiveMaxValue { get => _effectiveMaxValue; internal set => _effectiveMaxValue = value; }
        public Variables? Items { get => _items; internal set => _items = value; }
    }

    public class Fields : MDMNamedCollection<Field>, IMDMNamedCollection<Field>
    {
        internal Fields(IMDMDocument document) : base(document, document, collection => new Field(collection))
        {
            _objectType = MDMObjectType.Fields;
        }

        new public MDMObjectType ObjectType => _objectType;
    }

}
