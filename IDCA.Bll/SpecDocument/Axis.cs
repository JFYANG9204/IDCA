
using System.Collections.Generic;

namespace IDCA.Bll.SpecDocument
{
    public class Axis : SpecObjectCollection<IAxisElement>, IAxis
    {
        internal Axis(ISpecObject parent, AxisType axisType) : base(parent, collection => new AxisElement(collection))
        {
            _objectType = SpecObjectType.Axis;
            _type = axisType;
        }

        AxisType _type;
        public AxisType Type { get => _type; internal set => _type = value; }

        public override string ToString()
        {
            string text = string.Join(',', _items);
            return _type == AxisType.Normal ? $"{{{text}}}" : $"axis({{{text}}})";
        }
    }

    public class AxisElement : SpecObject, IAxisElement
    {
        internal AxisElement(ISpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisElement;
            _template = new AxisElementTemplate(this);
        }

        string _name = string.Empty;
        string _description = string.Empty;
        IAxisElementTemplate _template;

        public string Name { get => _name; internal set => _name = value; } 
        public string Description { get => _description; internal set => _description = value; }
        public IAxisElementTemplate Template { get => _template; internal set => _template = value; }

        public override string ToString()
        {
            return $"{_name} '{_description}' {_template}";
        }
    }

    public class AxisElementTemplate : SpecObject, IAxisElementTemplate
    {
        internal AxisElementTemplate(ISpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisElementTemplate;
            _parameters = new SpecObjectCollection<IAxisElement>(this, collection => (IAxisElement)new AxisParameter(collection));
        }

        int _requirParamNumber = 0;
        AxisElementType _elementType = AxisElementType.Text;
        readonly ISpecObjectCollection<IAxisElement> _parameters;

        public int RequireParamNumber { get => _requirParamNumber; internal set => _requirParamNumber = value; }
        public ISpecObjectCollection<IAxisParameter> Parameters => (ISpecObjectCollection<IAxisParameter>)_parameters;
        public AxisElementType ElementType { get => _elementType; internal set => _elementType = value; }

        public override string ToString()
        {
            return _elementType switch
            {
                AxisElementType.Text => "text()",
                AxisElementType.Base => $"base({(_parameters.Count > 0 ? $"'{_parameters[0]}'" : "")})",
                AxisElementType.UnweightedBase => $"unweightedbase({(_parameters.Count > 0 ? $"'{_parameters[0]}'" : "")})",
                AxisElementType.Mean => $"mean({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.StdDev => $"stddev({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.StdErr => $"stderr({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Total => "total()",
                AxisElementType.SubTotal => "subtotal()",
                AxisElementType.Min => $"min({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Max => $"max({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Net => $"net({{{(_parameters.Count > 0 ? _parameters[0].ToString() : "")}}})",
                AxisElementType.Combine => $"combine({{{(_parameters.Count > 0 ? _parameters[0].ToString() : "")}}})",
                AxisElementType.Expression => $"expression('{(_parameters.Count > 0 ? _parameters[0].ToString() : "")}')",
                AxisElementType.Numeric => $"numeric({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Derived => $"derived('{(_parameters.Count > 0 ? _parameters[0].ToString() : "")}')",
                AxisElementType.Sum => $"sum({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.EffectiveBase => "effectivebase()",
                AxisElementType.Median => $"median({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Percentile => $"percentile({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", {_parameters[1]}" : "")}{(_parameters.Count > 2 ? $", '{_parameters[2]}'" : "")})",
                AxisElementType.Mode => $"mode({(_parameters.Count > 0 ? _parameters[0].ToString() : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Ntd => "ntd()",
                _ => string.Empty,
            };
        }

    }

    public class AxisParameter : SpecObject, IAxisParameter
    {
        internal AxisParameter(ISpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisParameter;
        }

        readonly List<string> _items = new();
        bool _isCategorical = false;

        public List<string> Items => _items;
        public bool IsCategorical { get => _isCategorical; internal set => _isCategorical = value;}

        public void Add(string item)
        {
            _items.Add(item);
        }

        public override string ToString()
        {
            if (_items.Count > 0)
            {
                string value = string.Join(',', _items);
                return _isCategorical ? $"{{{value}}}" : value;
            }
            return string.Empty;
        }
    }

}
