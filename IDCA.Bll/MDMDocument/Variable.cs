
namespace IDCA.Bll.MDMDocument
{
    public class Variable : MDMLabeledObject, IVariable
    {
        internal Variable(IMDMDocument document, IMDMObject parent) : base(document, parent)
        {
            _objectType = MDMObjectType.Variable;
        }

        IProperties<Property>? _templates;
        IProperties<Property>? _notes;
        ITypes<Type>? _helperFields;

        readonly VariableUsage _usageType = VariableUsage.Variable;

        bool _hasCaseData = false;
        bool _versioned = false;

        public IProperties<Property>? Templates { get => _templates; internal set => _templates = value; }
        public IProperties<Property>? Notes { get => _notes; internal set => _notes = value; }
        public ITypes<Type>? HelperFields { get => _helperFields; internal set => _helperFields = value; }
        public VariableUsage UsageType => _usageType;

        public bool HasCaseData { get => _hasCaseData; internal set => _hasCaseData = value; }
        public bool Versioned { get => _versioned; internal set => _versioned = value; }
    }

    public class Variables : MDMNamedCollection<Variable>
    {
        internal Variables(IMDMDocument document) : base(document, document, collection => new Variable(document, collection))
        {
            _objectType = MDMObjectType.Fields;
        }
    }

    public class VariableInstance : MDMObject, IVariableInstance
    {
        internal VariableInstance(IMDMObject parent) : base(parent.Document, parent) { }

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
    }

    public class Mapping : MDMObjectCollection<VariableInstance>
    {
        internal Mapping(IMDMDocument document) : base(document, document, collection => new VariableInstance(collection))
        { 
        }
    }

}
