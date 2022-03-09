
namespace IDCA.Bll.MDMDocument
{
    public class MDMObject : IMDMObject
    {
        internal MDMObject(IMDMDocument document, IMDMObject parent)
        {
            _parent = parent;
            _document = document;
        }

        readonly protected IMDMObject _parent;
        readonly protected IMDMDocument _document;

        protected MDMObjectType _objectType = MDMObjectType.Unknown;
        protected IProperties<Property>? _properties;

        public MDMObjectType ObjectType { get => _objectType; internal set => _objectType = value; }
        public IMDMObject Parent => _parent;
        public IMDMDocument Document => _document;
        public IProperties<Property>? Properties { get => _properties; internal set => _properties = value; }
    }

    public class MDMNamedObject : MDMObject, IMDMNamedObject
    {
        internal MDMNamedObject(IMDMDocument document, IMDMObject parent) : base(document, parent)
        {
        }

        protected string _id = string.Empty;
        protected string _name = string.Empty;
        protected bool _isReference = false;
        protected bool _isSystem = false;

        public string Id { get => _id; internal set => _id = value; }
        public string Name { get => _name; internal set => _name = value; }
        public bool IsReference { get => _isReference; internal set => _isReference = value; }
        public bool IsSystem { get => _isSystem; internal set => _isSystem = value; }
    }

    public class MDMLabeledObject : MDMNamedObject, IMDMLabeledObject
    {
        internal MDMLabeledObject(IMDMDocument document, IMDMObject parent) : base(document, parent)
        {
        }

        protected Labels? _labels;
        protected Style? _labelStyle;

        public Labels? Labels { get => _labels; internal set => _labels = value; }
        public Style? LabelStyles { get => _labelStyle; internal set => _labelStyle = value; }

        public string Label
        {
            get
            {
                if (_labels == null)
                {
                    return string.Empty;
                }
                var label = _labels[_document.Language, _document.Context];
                return label == null ? string.Empty : label.Text;
            }
        }

    }

}
