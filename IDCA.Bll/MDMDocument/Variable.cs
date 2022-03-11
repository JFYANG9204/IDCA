
namespace IDCA.Bll.MDMDocument
{
    public class Variable : MDMLabeledObject, IVariable
    {
        internal Variable(IMDMDocument document, IMDMObject parent) : base(document, parent)
        {
            _objectType = MDMObjectType.Variable;
        }

        Elements? _elements;
        Categories? _categories;
        Properties? _notes;
        Variables? _helperFields;

        VariableUsage _usageType = VariableUsage.Variable;
        MDMDataType _type = MDMDataType.None;

        bool _hasCaseData = false;
        bool _versioned = false;

        object _minValue = string.Empty;
        object _maxValue = string.Empty;
        object _effectiveMinValue = string.Empty;
        object _effectiveMaxValue = string.Empty;

        public MDMDataType DataType { get => _type; internal set => _type = value; }
        public Elements? Elements { get => _elements; internal set => _elements = value; }
        public Categories? Categories { get => _categories; internal set => _categories = value; }
        public Properties? Notes { get => _notes; internal set => _notes = value; }
        public Variables? HelperFields { get => _helperFields; internal set => _helperFields = value; }
        public VariableUsage UsageType { get => _usageType; internal set => _usageType = value; }
        public object MinValue { get => _minValue; internal set => _minValue = value; }
        public object MaxValue { get => _maxValue; internal set => _maxValue = value; }
        public object EffectiveMinValue { get => _effectiveMinValue; internal set => _effectiveMinValue = value; }
        public object EffectiveMaxValue { get => _effectiveMaxValue; internal set => _effectiveMaxValue = value; }

        public bool HasCaseData { get => _hasCaseData; internal set => _hasCaseData = value; }
        public bool Versioned { get => _versioned; internal set => _versioned = value; }
    }

    public class Variables : MDMNamedCollection<Variable>, IMDMObject
    {
        internal Variables(IMDMDocument document, IMDMObject? parent = null) : base(document, parent ?? document, collection => new Variable(document, collection))
        {
            _objectType = MDMObjectType.Variables;
        }

        bool _globalNamespace = false;

        new public MDMObjectType ObjectType => _objectType;
        public bool GlobalNamespace { get => _globalNamespace; internal set => _globalNamespace = value; }
    }

    public class VariableInstance : MDMObject, IVariableInstance
    {
        internal VariableInstance(IMDMObject parent) : base(parent.Document, parent) 
        { 
            _objectType = MDMObjectType.VariableInstance;
        }

        new readonly MDMObjectType _objectType;

        string _name = string.Empty;
        string _fullName = string.Empty;
        IVariable? _variable;
        string _expression = string.Empty;
        SourceType _sourceType = SourceType.None;

        public string Name { get => _name; internal set => _name = value; }
        public string FullName { get => _fullName; internal set => _fullName = value; }
        public IVariable? Variable { get => _variable; internal set => _variable = value; }
        public string Expression { get => _expression; internal set => _expression = value; }
        public SourceType SourceType { get => _sourceType; internal set => _sourceType = value; }
        new public MDMObjectType ObjectType => _objectType;
    }

    public class Mapping : MDMObjectCollection<VariableInstance>
    {
        internal Mapping(IMDMDocument document) : base(document, document, collection => new VariableInstance(collection))
        {
            _objectType = MDMObjectType.Mapping;
        }

        new public MDMObjectType ObjectType => _objectType;
    }

}
