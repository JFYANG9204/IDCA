
namespace IDCA.Bll.MDMDocument
{
    public class Field : MDMLabeledObject, IField
    {

        internal Field(IMDMObject parent) : base(parent.Document, parent)
        {
        }

        IField? _reference;
        ICategories? _categories;
        IElements? _elements;

        IMDMObjectCollection<Variable>? _items;
        MDMDataType _type = MDMDataType.None;

        bool _isSystemVariable = false;

        object _minValue = string.Empty;
        object _maxValue = string.Empty;
        object _effectiveMinValue = string.Empty;
        object _effectiveMaxValue = string.Empty;

        public IField? Reference { get => _reference; internal set => _reference = value; }
        public ICategories? Categories { get => _categories; internal set => _categories = value; }
        public IElements? Elements { get => _elements; internal set => _elements = value; }
        public MDMDataType DataType { get => _type; internal set => _type = value; }
        public bool IsSystemVariable { get => _isSystemVariable; internal set => _isSystemVariable = value; }

        public object MinValue { get => _minValue; internal set => _minValue = value; }
        public object MaxValue { get => _maxValue; internal set => _maxValue = value; }
        public object EffectiveMinValue { get => _effectiveMinValue; internal set => _effectiveMinValue = value; }
        public object EffectiveMaxValue { get => _effectiveMaxValue; internal set => _effectiveMaxValue = value; }
        public IMDMObjectCollection<Variable>? Items { get => _items; internal set => _items = value; }
    }

    public class Fields : MDMNamedCollection<Field>, IMDMNamedCollection<Field>
    {
        internal Fields(IMDMDocument document) : base(document, document, collection => new Field(collection))
        {
        }

    }

}
