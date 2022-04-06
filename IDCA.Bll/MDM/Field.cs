
using System.Text;

namespace IDCA.Bll.MDM
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

        public string FullName
        {
            get
            {
                StringBuilder builder = new();
                builder.Append(_name);
                IMDMObject field = _parent.Parent.Parent;
                while (field.ObjectType == MDMObjectType.Field)
                {
                    Field fieldObj = (Field)field;
                    builder.Insert(0, $"{fieldObj.Name}{(fieldObj.IteratorType == MDM.IteratorType.Categorical ? "[..]" : "")}.");
                    field = field.Parent.Parent.Parent;
                }
                return builder.ToString();
            }
        }

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
                    Elements = value.Elements;
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
    
        new public Field? this[string name]
        {
            get
            {
                string lowerName = name.ToLower();
                if (_cache.ContainsKey(lowerName))
                {
                    return _cache[lowerName];
                }
                string[] fields = StringHelper.ReadFieldNames(name);
                if (fields.Length > 1)
                {
                    Fields? sub = this;
                    for (int i = 0; i < fields.Length; i++)
                    {
                        string field = fields[i];
                        Field? subField = sub[field];
                        if (i == field.Length - 1)
                        {
                            return subField;
                        }
                        if (subField == null || subField.Class == null || subField.Class.Fields == null)
                        {
                            return null;
                        }
                        sub = subField.Class.Fields;
                    }
                }
                return null;
            }
        }
    }

}
