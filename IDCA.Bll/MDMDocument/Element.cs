using System;
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public class Element : IElement
    {
        internal Element(IMDMObject parent)
        {
            _parent = parent;
            _document = parent.Document;
        }

        readonly IDocument _document;
        readonly IMDMObject _parent;
        IElement? _reference;
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
        ILabels? _labels;
        Style? _labelStyle;
        string _name = string.Empty;
        string _id = string.Empty;

        public IElement? Reference { get => _reference; internal set => _reference = value; }
        public ElementType Type => throw new NotImplementedException();
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
        public ILabels? Labels { get => _labels; internal set => _labels = value; }
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
        public Style? LabelStyles { get => _labelStyle; internal set => _labelStyle = value; }
        public string Id { get => _id; internal set => _id = value; }
        public string Name { get => _name; internal set => _name = value; }
        public MDMObjectType ObjectType => throw new NotImplementedException();
        public IMDMObject Parent => _parent;
        public bool IsReference => throw new NotImplementedException();
        public bool IsSystem => _parent.IsSystem;
        public IDocument Document => _document;

        public IProperties? Properties => throw new NotImplementedException();

    }
}
