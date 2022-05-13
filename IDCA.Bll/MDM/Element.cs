

namespace IDCA.Model.MDM
{
    public class Element : MDMLabeledObject
    {
        internal Element(MDMObject? parent) : base(parent?.Document, parent)
        {
            _objectType = MDMObjectType.Element;
        }

        Element? _reference;
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

        public Element? Reference { get => _reference; internal set => _reference = value; }
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

    public class Elements : MDMNamedCollection<Element>
    {
        internal Elements(MDMObject? parent) : base(parent?.Document, parent, ctor => new Element(ctor))
        {
            _objectType = MDMObjectType.Elements;
        }
    }

    public enum ElementType
    {
        Category = 0,
        AnalysisSubheading = 1,
        AnalysisBase = 2,
        AnalysisSubtotal = 3,
        AnalysisSummaryData = 4,
        AnalysisDerived = 5,
        AnalysisTotal = 6,
        AnalysisMean = 7,
        AnalysisStdDev = 8,
        AnalysisStdErr = 9,
        AnalysisSampleVariance = 10,
        AnalysisMinimun = 11,
        AnalysisMaximun = 12,
        AnalysisCategory = 14,
    }

    public enum CategoryFlag
    {
        None,
        User,
        DontKnow,
        Refuse,
        Noanswer,
        Other,
        Multiplier,
        Exclusive,
        FixedPosition,
        NoFilter,
        Inline
    }

    public enum FactorType
    {
        None = 0,
        Long = 3,
        Double = 5,
        Text = 8,
    }
}
