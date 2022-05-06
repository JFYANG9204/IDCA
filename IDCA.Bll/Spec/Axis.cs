﻿
using IDCA.Model.Template;
using System;
using System.Linq;
using System.Text;

namespace IDCA.Model.Spec
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

        string _name = string.Empty;
        /// <summary>
        /// 轴表达式的名称，如果表达式类型为AxisVariable，会以 as name 'label'的格式放在最后
        /// </summary>
        public string Name { get => _name; internal set => _name = value; }

        string _label = string.Empty;
        /// <summary>
        /// 轴表达式的标签，如果表达式类型为AxisVariable，会以 as name 'label'的格式放在最后
        /// </summary>
        public string Label { get => _label; internal set => _label = value; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string text = string.Join(',', _items);
            return _type == AxisType.Normal ? $"{{{text}}}" : $"axis({{{text}}}){(!string.IsNullOrEmpty(_name) ? $" as {_name} '${_label}'" : "")}";
        }

        /// <summary>
        /// 计算当前集合中指定类型表达式元素的数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CountIf(AxisElementType type)
        {
            return _items.Select(item => item.Template.ElementType == type).Count(val => val);
        }

        AxisElement AppendElement(AxisElementType type, string label, Action<AxisElement>? callback, params string[] parameters)
        {
            AxisElement element = NewObject();
            element.Template.ElementType = type;
            element.Description = label;
            callback?.Invoke(element);
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param))
                {
                    var parameter = element.Template.NewParameter();
                    parameter.SetValue(param);
                    element.Template.PushParameter(parameter);
                }
            }
            Add(element);
            return element;
        }

        AxisElement AppendNamedElement(AxisElementType type, string label, string name, params string[] parameters)
        {
            return AppendElement(type, label, element => element.Name = $"{name}{CountIf(type) + 1}", parameters);
        }

        AxisElement AppendMeanLike(AxisElementType type, string label, string name, string variable = "", string expression = "")
        {
            if (string.IsNullOrEmpty(expression))
            {
                return AppendElement(type, label, expr => expr.Name = $"{name}{CountIf(type) + 1}", variable);
            }
            return AppendElement(type, label, expr => expr.Name = $"{name}{CountIf(type) + 1}", variable, expression);
        }


        /// <summary>
        /// 向当前集合的末尾添加一个text类型的轴表达式元素，可以修改标签
        /// </summary>
        /// <param name="label"></param>
        public AxisElement AppendTextElement(string label = "")
        {
            return AppendNamedElement(AxisElementType.Text, label, "e");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个base类型的轴表达式元素，可以修改标签和一个参数
        /// </summary>
        /// <param name="label">base元素的标签</param>
        /// <param name="parameter">base元素的参数，一般为表达式或true</param>
        public AxisElement AppendBaseElement(string label = "", string parameter = "")
        {
            return AppendElement(AxisElementType.Base, label, item =>
            {
                int count = CountIf(AxisElementType.Base);
                item.Name = count == 0 ? "base" : $"base{count}";
            }, parameter);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个unweightedbase类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        public AxisElement AppendUnweightedBaseElement(string label = "")
        {
            return AppendNamedElement(AxisElementType.UnweightedBase, label, "unweightedbase");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个EffectiveBase类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        public AxisElement AppendEffectiveBaseElement(string label = "")
        {
            return AppendNamedElement(AxisElementType.EffectiveBase, label, "effectivebase");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Expression类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public AxisElement AppendExpression(string label = "", string expression = "")
        {
            return AppendNamedElement(AxisElementType.Expression, label, "expr", expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Numeric类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendNumeric(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Numeric, label, "numeric", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Derived类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public AxisElement AppendDerived(string label = "", string expression = "")
        {
            return AppendNamedElement(AxisElementType.Derived, label, "dev", expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Mean类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendMean(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Mean, label, "mean", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个StdErr类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendStdErr(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.StdErr, label, "stderr", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个StdDev类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendStdDev(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.StdDev, label, "stddev", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Total类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <returns></returns>
        public AxisElement AppendTotal(string label = "")
        {
            return AppendNamedElement(AxisElementType.Total, label, "total");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个SubTotal类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <returns></returns>
        public AxisElement AppendSubTotal(string label = "")
        {
            return AppendNamedElement(AxisElementType.SubTotal, label, "subTotal");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Min类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendMin(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Min, label, "min", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Max类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendMax(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Max, label, "max", variable, expression);
        }

        /// <summary>
        /// 向集合末尾添加一个Net类型的轴表达式元素，必须提供码号参数，字符串中间由逗号分隔
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="codes">需要包含的码号</param>
        /// <returns></returns>
        public AxisElement AppendNet(string label = "", string codes = "")
        {
            return AppendNamedElement(AxisElementType.Net, label, "net", codes);
        }

        /// <summary>
        /// 向集合末尾添加一个Combine类型的轴表达式元素，必须提供码号参数，字符串中间由逗号分隔
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="codes">需要包含的码号</param>
        /// <returns></returns>
        public AxisElement AppendCombine(string label = "", string codes = "")
        {
            return AppendNamedElement(AxisElementType.Combine, label, "com", codes);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Sum类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendSum(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Sum, label, "sum", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Median类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendMedian(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Median, label, "med", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Percentile类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="cutOffValue">所需要的百分比数值，如果为50，则和中位数(Median)相同</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendPercentile(string label = "", string variable = "", double cutOffValue = 50, string expression = "")
        {
            return AppendNamedElement(AxisElementType.Percentile, label, "pt", variable, cutOffValue.ToString(), expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Mode类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendMode(string label = "", string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Mode, label, "mode", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Ntd类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <returns></returns>
        public AxisElement AppendNtd(string label = "")
        {
            return AppendNamedElement(AxisElementType.Ntd, label, "ntd");
        }

        /// <summary>
        /// 向当前集合末尾添加一个插入的新本地变量轴表达元素，只需要设置变量名，最终会以字符串拼接的形式写入文件。
        /// </summary>
        /// <param name="variable"></param>
        public void AppendInsertVariable(string variable = "")
        {
            AppendElement(AxisElementType.InsertFunctionOrVariable, "", null, variable);
        }

        /// <summary>
        /// 向当前集合末尾添加一个插入的新本地函数调用表达式元素，需要已知的函数模板，最终会以字符串拼接的形式写入文件。
        /// </summary>
        /// <param name="function"></param>
        public void AppendInsertFunction(FunctionTemplate? function = null)
        {
            AppendElement(AxisElementType.InsertFunctionOrVariable, "", null, function == null ? "" : function.Exec());
        }

        /// <summary>
        /// 向当前集合末尾添加一个所有Category的表达式元素'..'
        /// </summary>
        public void AppendAllCategory()
        {
            AppendElement(AxisElementType.AllCategory, string.Empty, null);
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
        AxisVariable,
    }

    public class AxisElement : SpecObject
    {
        internal AxisElement(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisElement;
            _template = new AxisElementTemplate(this);
            _suffix = new AxisElementSuffixCollection(this);
        }

        internal AxisElement(SpecObject parent, AxisElementType type) : this(parent)
        {
            _template.ElementType = type;
        }

        string _name = string.Empty;
        string _description = string.Empty;
        readonly AxisElementTemplate _template;
        readonly AxisElementSuffixCollection _suffix;

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
        public AxisElementTemplate Template => _template;
        /// <summary>
        /// 当前元素的后缀配置集合
        /// </summary>
        public AxisElementSuffixCollection Suffix => _suffix;

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(_name) ? "" : _name)}{(string.IsNullOrEmpty(_name) ? "" : $" '{_description}'")}{(string.IsNullOrEmpty(_name) ? "" : " ")}{_template}{_suffix}";
        }
    }


    public enum AxisElementType
    {
        AllCategory,
        InsertFunctionOrVariable,
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
            _parameters = new SpecObjectCollection<AxisElementParameter>(this, collection => new AxisElementParameter(collection));
        }

        AxisElementType _elementType = AxisElementType.Text;
        readonly SpecObjectCollection<AxisElementParameter> _parameters;

        /// <summary>
        /// 元素的类型
        /// </summary>
        public AxisElementType ElementType { get => _elementType; internal set => _elementType = value; }

        /// <summary>
        /// 创建新的表达式元素参数
        /// </summary>
        /// <returns></returns>
        public AxisElementParameter NewParameter()
        {
            return _parameters.NewObject();
        }

        /// <summary>
        /// 将新的参数添加到参数列表末尾
        /// </summary>
        /// <param name="parameter"></param>
        public void PushParameter(AxisElementParameter parameter)
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
                AxisElementType.AllCategory => "..",
                AxisElementType.InsertFunctionOrVariable => $"\" + {(_parameters.Count > 0 ? _parameters[0].ToString() : "")} + \"",
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


    public class AxisElementParameter : SpecObject
    {
        internal AxisElementParameter(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisParameter;
        }

        string _value = string.Empty;

        /// <summary>
        /// 设置当前参数对象的值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            _value = value;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }
    }

    /// <summary>
    /// 保存表达式元素后缀配置的集合
    /// </summary>
    public class AxisElementSuffixCollection : SpecObjectCollection<AxisElementSuffix>
    {

        internal AxisElementSuffixCollection(SpecObject parent) : base(parent, collection => new AxisElementSuffix(collection))
        {
        }


        void AppendItem(AxisElementSuffixType type, object value)
        {
            AxisElementSuffix? elementSuffix = _items.Find(x => x.Type == type);
            if (elementSuffix != null)
            {
                elementSuffix.Value = value;
            }
            else
            {
                AxisElementSuffix suffix = NewObject();
                suffix.Type = type;
                suffix.Value = value;
                Add(suffix);
            }
        }

        /// <summary>
        /// 向当前集合末尾添加CalculationScope类型的配置字段
        /// </summary>
        /// <param name="scope"></param>
        public void AppendCalculationScope(AxisElementSuffixCalculationScope scope)
        {
            AppendItem(AxisElementSuffixType.CalculationScope,
                scope == AxisElementSuffixCalculationScope.AllElements ? "AllElements" : "PrecedingElements");
        }
        
        /// <summary>
        /// 向当前集合末尾添加CountsOnly类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendCountsOnly(bool value)
        {
            AppendItem(AxisElementSuffixType.CountsOnly, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加Decimals类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendDecimals(int value)
        {
            AppendItem(AxisElementSuffixType.Decimals, value);
        }

        /// <summary>
        /// 向当前集合末尾添加Factor类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendFactor(double value)
        {
            AppendItem(AxisElementSuffixType.Factor, value);
        }

        /// <summary>
        /// 向当前集合末尾添加IsFixed类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendIsFixed(bool value)
        {
            AppendItem(AxisElementSuffixType.IsFixed, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsHidden类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendIsHidden(bool value)
        {
            AppendItem(AxisElementSuffixType.IsHidden, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsHiddenWhenColumn类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendIsHiddenWhenColumn(bool value)
        {
            AppendItem(AxisElementSuffixType.IsHiddenWhenColumn, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsHiddenWhenRow类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendIsHiddenWhenRow(bool value)
        {
            AppendItem(AxisElementSuffixType.IsHiddenWhenRow, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IncludeInBase类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendIncludeInBase(bool value)
        {
            AppendItem(AxisElementSuffixType.IncludeInBase, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsUnweighted类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendIsUnweighted(bool value)
        {
            AppendItem(AxisElementSuffixType.IsUnweighted, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加Multiplier类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendMultiplier(string value)
        {
            AppendItem(AxisElementSuffixType.Multiplier, value);
        }

        /// <summary>
        /// 向当前集合末尾添加Weight类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public void AppendWeight(string value)
        {
            AppendItem(AxisElementSuffixType.Weight, value);
        }

        public override string ToString()
        {
            if (Count > 0)
            {
                StringBuilder builder = new();
                builder.Append(" [");
                for (int i = 0; i < Count; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(_items[i].ToString());
                }
                builder.Append(']');
                return builder.ToString();
            }
            return string.Empty;
        }

    }

    /// <summary>
    /// 轴表达式元素计算区间类型
    /// </summary>
    public enum AxisElementSuffixCalculationScope
    {
        AllElements,
        PrecedingElements,
    }

    /// <summary>
    /// 轴表达式元素的后缀设定类型
    /// </summary>
    public enum AxisElementSuffixType
    {
        None,
        // CalculationScope=AllElements|PrecedingElements
        CalculationScope,
        // CountsOnly=True|False
        CountsOnly,
        // Decimals=DecimalPlaces
        Decimals,
        // Factor=FactorValue
        Factor,
        // IsFixed=True|False 
        IsFixed,
        // IsHidden=True|False   
        IsHidden,
        // IsHiddenWhenColumn=True|False
        IsHiddenWhenColumn,
        // IsHiddenWhenRow=True|False 
        IsHiddenWhenRow,
        // IncludeInBase=True|False
        IncludeInBase,
        // IsUnweighted=True|False
        IsUnweighted,
        // Multiplier=MultiplierVariable
        Multiplier,
        // Weight=WeightVariable
        Weight,
    }

    /// <summary>
    /// 轴表达式元素的后缀配置类型，一般以[setting]的形式跟在元素表达式后
    /// </summary>
    public class AxisElementSuffix : SpecObject
    {
        internal AxisElementSuffix(SpecObject parent) : base(parent)
        {
            _type = AxisElementSuffixType.None;
        }

        AxisElementSuffixType _type;
        /// <summary>
        /// 后缀配置的类型
        /// </summary>
        public AxisElementSuffixType Type { get => _type; set => _type = value; }

        object _value = string.Empty;
        /// <summary>
        /// 需要配置的配置值
        /// </summary>
        public object Value { get => _value; set => _value = value; }

        public override string ToString()
        {
            return _type switch
            {
                AxisElementSuffixType.CalculationScope => $"CalculationScope={_value}",
                AxisElementSuffixType.CountsOnly => $"CountsOnly={_value}",
                AxisElementSuffixType.Decimals => $"Decimals={_value}",
                AxisElementSuffixType.Factor => $"Factor={_value}",
                AxisElementSuffixType.IsFixed => $"IsFixed={_value}",
                AxisElementSuffixType.IsHidden => $"IsHidden={_value}",
                AxisElementSuffixType.IsHiddenWhenColumn => $"IsHiddenWhenColumn={_value}",
                AxisElementSuffixType.IsHiddenWhenRow => $"IsHiddenWhenRow={_value}",
                AxisElementSuffixType.IncludeInBase => $"IncludeInBase={_value}",
                AxisElementSuffixType.IsUnweighted => $"IsUnweighted={_value}",
                AxisElementSuffixType.Multiplier => $"Multiplier={_value}",
                AxisElementSuffixType.Weight => $"Weight={_value}",
                _ => string.Empty
            };
        }

    }


}
