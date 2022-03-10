
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
        Properties? _templates;
        Properties? _notes;
        Types? _helperFields;

        readonly VariableUsage _usageType = VariableUsage.Variable;

        bool _hasCaseData = false;
        bool _versioned = false;

        new public MDMObjectType ObjectType => _objectType;

        public Elements? Elements { get => _elements; internal set => _elements = value; }
        public Categories? Categories { get => _categories; internal set => _categories = value; }
        public Properties? Templates { get => _templates; internal set => _templates = value; }
        public Properties? Notes { get => _notes; internal set => _notes = value; }
        public Types? HelperFields { get => _helperFields; internal set => _helperFields = value; }
        public VariableUsage UsageType => _usageType;

        public bool HasCaseData { get => _hasCaseData; internal set => _hasCaseData = value; }
        public bool Versioned { get => _versioned; internal set => _versioned = value; }
    }

    public class Variables : MDMNamedCollection<Variable>, IMDMObject
    {
        internal Variables(IMDMDocument document) : base(document, document, collection => new Variable(document, collection))
        {
            _objectType = MDMObjectType.Variables;
        }

        new public MDMObjectType ObjectType => _objectType;
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
