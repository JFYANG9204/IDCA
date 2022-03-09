
namespace IDCA.Bll.MDMDocument
{
    public class Element : MDMLabeledObject, IElement
    {
        internal Element(IMDMObject parent) : base(parent.Document, parent)
        {
        }

        IElement? _reference;
        ElementType _type = ElementType.Category;
        CategoryFlag _flag = CategoryFlag.None;
        IVariable? _otherReference;
        IVariable? _otherVariable;
        object? _factor;
        FactorType? _factorType;
        bool _isOtherLocal = false;
        IVariable? _multiplierReference;
        IVariable? _multiplierVariable;
        bool _isMultiplierLocal = false;
        bool _versioned = false;

        public IElement? Reference { get => _reference; internal set => _reference = value; }
        public ElementType Type { get => _type; internal set => _type = value; }
        public CategoryFlag Flag { get => _flag; internal set => _flag = value; }
        public IVariable? OtherReference { get => _otherReference; internal set => _otherReference = value; }
        public IVariable? OtherVariable { get => _otherVariable; internal set => _otherVariable = value; }
        public object? Factor { get => _factor; internal set => _factor = value; }
        public FactorType? FactorType { get => _factorType; internal set => _factorType = value; }
        public bool IsOtherLocal { get => _isOtherLocal; internal set => _isOtherLocal = value; }
        public IVariable? MultiplierReference { get => _multiplierReference; internal set => _multiplierReference = value; }
        public IVariable? MultiplierVariable { get => _multiplierVariable; internal set => _multiplierVariable = value; }
        public bool IsMultiplierLocal { get => _isMultiplierLocal; internal set => _isMultiplierLocal = value; }
        public bool Versioned { get => _versioned; internal set => _versioned = value; }

    }
}
