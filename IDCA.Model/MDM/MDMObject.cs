
namespace IDCA.Model.MDM
{
    public class MDMObject
    {
        internal MDMObject(MDMDocument? document, MDMObject? parent)
        {
            _parent = parent;
            _document = document;
        }

        readonly protected MDMObject? _parent;
        readonly protected MDMDocument? _document;

        protected MDMObjectType _objectType = MDMObjectType.Unknown;
        protected Properties? _properties;
        protected Properties? _templates;

        public MDMObjectType ObjectType { get => _objectType; internal set => _objectType = value; }
        public MDMObject? Parent => _parent;
        public MDMDocument? Document => _document;
        public Properties? Properties { get => _properties; internal set => _properties = value; }
        public Properties? Templates { get => _templates; internal set => _templates = value; }
    }

    public class MDMNamedObject : MDMObject
    {
        internal MDMNamedObject(MDMDocument? document, MDMObject? parent) : base(document, parent)
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

    public class MDMLabeledObject : MDMNamedObject
    {
        internal MDMLabeledObject(MDMDocument? document, MDMObject? parent) : base(document, parent)
        {
        }

        protected Labels? _labels;
        protected Style? _labelStyle;
        protected Style? _style;

        public Labels? Labels { get => _labels; internal set => _labels = value; }
        public Style? LabelStyles { get => _labelStyle; internal set => _labelStyle = value; }
        public Style? Style { get => _style; internal set => _style = value; }

        public string Label
        {
            get
            {
                if (_labels == null || _document == null)
                {
                    return string.Empty;
                }
                var label = _labels[_document.Language, _document.Context];
                return label == null ? string.Empty : label.Text;
            }
        }

    }


    public enum MDMDataType
    {
        None = -1,
        Info = 0,
        Long = 1,
        Text = 2,
        Categorical = 3,
        Date = 5,
        Double = 6,
        Boolean = 7,
    }

    public enum MDMObjectType
    {
        Unknown,
        Variable,
        Grid,
        Class,
        Element,
        Elements,
        Categories,
        Label,
        Labels,
        Field,
        HelperFields,
        Fields,
        Type,
        Types,
        Property,
        Properties,
        Routing,
        Routings,
        Context,
        Contexts,
        Language,
        Languages,
        VariableInstance,
        RoutingItem,
        Compound,
        RoutingItems,
        Variables,
        Mapping,
        Script,
        Scripts,
        ScriptType,
        ScriptTypes,
        Page,
        Pages,
        SaveLog,
        SaveLogs,
        CategoryMap,
        DataSource,
        DataSources,
        Document
    }


}
