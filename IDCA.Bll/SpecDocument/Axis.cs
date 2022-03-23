
using System.Collections.Generic;

namespace IDCA.Bll.SpecDocument
{
    public class Axis : SpecObjectCollection<AxisElement>
    {
        internal Axis(SpecObject parent, AxisType axisType) : base(parent, collection => new AxisElement(collection))
        {
            _objectType = SpecObjectType.Axis;
            _type = axisType;
        }

        AxisType _type;
        /// <summary>
        /// 轴表达式类型
        /// </summary>
        public AxisType Type { get => _type; internal set => _type = value; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string text = string.Join(',', _items);
            return _type == AxisType.Normal ? $"{{{text}}}" : $"axis({{{text}}})";
        }
    }

    public enum AxisType
    {
        /// <summary>
        /// 一般的表达式，由左右花括号开始和结尾
        /// </summary>
        Normal,
        /// <summary>
        /// 可以直接添加表格的表达式，由"axis("开头，右括号")"结尾
        /// </summary>
        AxisTable,
    }

    public class AxisElement : SpecObject
    {
        internal AxisElement(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisElement;
            _template = new AxisElementTemplate(this);
        }

        string _name = string.Empty;
        string _description = string.Empty;
        AxisElementTemplate _template;

        /// <summary>
        /// 轴表达式元素的名称
        /// </summary>
        public string Name { get => _name; internal set => _name = value; }
        /// <summary>
        /// 轴表达式元素的描述
        /// </summary>
        public string Description { get => _description; internal set => _description = value; }
        /// <summary>
        /// 轴表达式元素模板
        /// </summary>
        public AxisElementTemplate Template { get => _template; internal set => _template = value; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{_name} '{_description}' {_template}";
        }
    }


    public enum AxisElementType
    {
        Text,
        Base,
        UnweightedBase,
        EffectiveBase,
        Expression,
        Numeric,
        Derived,
        Mean,
        StdErr,
        StdDev,
        Total,
        SubTotal,
        Min,
        Max,
        Net,
        Combine,
        Sum,
        Median,
        Percentile,
        Mode,
        Ntd
    }

    /// <summary>
    /// 轴表达式元素的字符串文本模板
    /// </summary>
    public class AxisElementTemplate : SpecObject
    {
        internal AxisElementTemplate(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisElementTemplate;
            _parameters = new SpecObjectCollection<AxisParameter>(this, collection => new AxisParameter(collection));
        }

        AxisElementType _elementType = AxisElementType.Text;
        readonly SpecObjectCollection<AxisParameter> _parameters;

        /// <summary>
        /// 参数列表
        /// </summary>
        public SpecObjectCollection<AxisParameter> Parameters => _parameters;
        /// <summary>
        /// 元素的类型
        /// </summary>
        public AxisElementType ElementType { get => _elementType; internal set => _elementType = value; }


        public void PushParameter(AxisParameter parameter)
        {
            _parameters.Add(parameter);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
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




    public class AxisParameter : SpecObject
    {
        internal AxisParameter(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisParameter;
        }

        readonly List<string> _items = new();
        bool _isCategorical = false;

        /// <summary>
        /// 参数元素列表
        /// </summary>
        public List<string> Items => _items;
        /// <summary>
        /// 是否是Categorical类型
        /// </summary>
        public bool IsCategorical { get => _isCategorical; internal set => _isCategorical = value;}

        /// <summary>
        /// 向集合中添加元素
        /// </summary>
        /// <param name="item"></param>
        public void Add(string item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
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
