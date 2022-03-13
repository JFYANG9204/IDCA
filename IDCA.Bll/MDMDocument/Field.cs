
namespace IDCA.Bll.MDMDocument
{
    public class Field : Variable, IField
    {

        internal Field(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Field;
        }

        Variable? _reference;
        Class? _class;
        IteratorType? _iteratorType;
        int? _upperBound;
        int? _lowerBound;

        public Variable? Reference
        {
            get => _reference;
            internal set
            {
                _reference = value;
                _isReference = value != null;
                if (value != null)
                {
                    Name = value.Name;
                    Labels = value.Labels;
                    DataType = value.DataType;
                    MinValue = value.MinValue;
                    MaxValue = value.MaxValue;
                    EffectiveMaxValue = value.EffectiveMaxValue;
                    EffectiveMinValue = value.EffectiveMinValue;
                    Categories = value.Categories;
                    HelperFields = value.HelperFields;
                    HasCaseData = value.HasCaseData;
                    Versioned = value.Versioned;
                    Properties = value.Properties;
                    Templates = value.Templates;
                    Style = value.Style;
                    LabelStyles = value.LabelStyles;
                }
            }
        }
        public Class? Class { get => _class; internal set => _class = value; }
        public IteratorType? IteratorType { get => _iteratorType; internal set => _iteratorType = value; }
        public int? LowerBound { get => _lowerBound; internal set => _lowerBound = value; }
        public int? UpperBound { get => _upperBound; internal set => _upperBound = value; }
    }

    public class Fields : MDMNamedCollection<Field>, IMDMNamedCollection<Field>
    {
        internal Fields(IMDMDocument document, IMDMObject? parent = null) : base(document, parent ?? document, collection => new Field(collection))
        {
            _objectType = MDMObjectType.Fields;
        }

        bool _globalNamespace = false;

        new public MDMObjectType ObjectType => _objectType;
        public bool GlobalNamespace { get => _globalNamespace; internal set => _globalNamespace = value; }
    }

}
