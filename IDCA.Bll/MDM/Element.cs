

namespace IDCA.Bll.MDM
{
    public class Element : MDMLabeledObject, IElement
    {
        internal Element(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Element;
        }

        IElement? _reference;
        ElementType _type = ElementType.Category;
        CategoryFlag _flag = CategoryFlag.None;
        Variable? _otherReference;
        Variable? _otherVariable;
        object? _factor;
        FactorType? _factorType;
        bool _isOtherLocal = false;
        Variable? _multiplierReference;
        Variable? _multiplierVariable;
        bool _isMultiplierLocal = false;
        bool _versioned = false;

        public IElement? Reference { get => _reference; internal set => _reference = value; }
        public ElementType ElementType { get => _type; internal set => _type = value; }
        public CategoryFlag Flag { get => _flag; internal set => _flag = value; }
        public Variable? OtherReference { get => _otherReference; internal set => _otherReference = value; }
        public Variable? OtherVariable { get => _otherVariable; internal set => _otherVariable = value; }
        public object? Factor { get => _factor; internal set => _factor = value; }
        public FactorType? FactorType { get => _factorType; internal set => _factorType = value; }
        public bool IsOtherLocal { get => _isOtherLocal; internal set => _isOtherLocal = value; }
        public Variable? MultiplierReference { get => _multiplierReference; internal set => _multiplierReference = value; }
        public Variable? MultiplierVariable { get => _multiplierVariable; internal set => _multiplierVariable = value; }
        public bool IsMultiplierLocal { get => _isMultiplierLocal; internal set => _isMultiplierLocal = value; }
        public bool Versioned { get => _versioned; internal set => _versioned = value; }
    }

    public class Elements : MDMNamedCollection<Element>, IElements
    {
        internal Elements(IMDMObject parent) : base(parent.Document, parent, ctor => new Element(ctor))
        {
            _objectType = MDMObjectType.Elements;
        }

        new public MDMObjectType ObjectType => _objectType;
    }

}
