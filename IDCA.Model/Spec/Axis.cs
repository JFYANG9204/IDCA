
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

        internal Axis(Axis axisObject) : base(collection => new AxisElement(collection))
        {
            _parent = axisObject.Parent;
            _document = axisObject.Document;
            _objectType = axisObject.SpecObjectType;
            _type = axisObject.Type;
            foreach (AxisElement ele in axisObject)
            {
                _items.Add(new AxisElement(ele));
            }
        }

        AxisType _type;
        /// <summary>
        /// 轴表达式类型
        /// </summary>
        public AxisType Type { get => _type; set => _type = value; }

        string _name = string.Empty;
        /// <summary>
        /// 轴表达式的名称，如果表达式类型为AxisVariable，会以 as name 'label'的格式放在最后
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        string _label = string.Empty;
        /// <summary>
        /// 轴表达式的标签，如果表达式类型为AxisVariable，会以 as name 'label'的格式放在最后
        /// </summary>
        public string Label { get => _label; set => _label = value; }
        /// <summary>
        /// 从已有的轴表达式对象复制数据
        /// </summary>
        /// <param name="axis"></param>
        public void FromAxis(Axis axis)
        {
            _name = axis.Name;
            _label = axis.Label;
            _type = axis.Type;
            foreach (AxisElement element in axis)
            {
                Add(new AxisElement(element));
            }
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string text = string.Join(", ", _items);
            return _type == AxisType.Normal ? $"{{{text}}}" : $"axis({(_type == AxisType.MetadataAxis ? "\"" : "")}{{{text}}}{(_type == AxisType.MetadataAxis ? "\"" : "")}){(!string.IsNullOrEmpty(_name) ? $" as {_name} '${_label}'" : "")}";
        }

        /// <summary>
        /// 计算当前集合中指定类型表达式元素的数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CountIf(AxisElementType type)
        {
            int count = 0;
            _items.ForEach(e => {
                if (e.Template.ElementType == type)
                {
                    count++;
                }
                count += e.CountChild(type);
            });
            return count;
        }

        /// <summary>
        /// 添加符合类型的默认轴元素，所有参数都为默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AxisElement AppendElement(AxisElementType type)
        {
            return type switch
            {
                AxisElementType.Category => AppendCategory(),
                AxisElementType.CategoryRange => AppendCategoryRange(),
                AxisElementType.InsertFunctionOrVariable => AppendInsertFunction(),
                AxisElementType.Text => AppendText(),
                AxisElementType.Base => AppendBase(),
                AxisElementType.UnweightedBase => AppendUnweightedBase(),
                AxisElementType.EffectiveBase => AppendEffectiveBase(),
                AxisElementType.Expression => AppendExpression(),
                AxisElementType.Numeric => AppendNumeric(),
                AxisElementType.Derived => AppendDerived(),
                AxisElementType.Mean => AppendMean(),
                AxisElementType.StdErr => AppendStdErr(),
                AxisElementType.StdDev => AppendStdDev(),
                AxisElementType.Total => AppendTotal(),
                AxisElementType.SubTotal => AppendSubTotal(),
                AxisElementType.Min => AppendMin(),
                AxisElementType.Max => AppendMax(),
                AxisElementType.Net => AppendNet(),
                AxisElementType.Combine => AppendCombine(),
                AxisElementType.Sum => AppendSum(),
                AxisElementType.Median => AppendMedian(),
                AxisElementType.Percentile => AppendPercentile(),
                AxisElementType.Mode => AppendMode(),
                AxisElementType.Ntd => AppendNtd(),
                _ => AppendElement(AxisElementType.None),
            };
        }

        AxisElement AppendElement(AxisElementType type, string? label, Action<AxisElement>? callback, params object[] parameters)
        {
            AxisElement element = NewObject();
            element.Template.ElementType = type;
            element.Description = label;
            callback?.Invoke(element);
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param.ToString()))
                {
                    var parameter = element.Template.NewParameter();
                    parameter.SetValue(param);
                    element.Template.PushParameter(parameter);
                }
            }
            Add(element);
            return element;
        }

        AxisElement AppendNamedElement(AxisElementType type, string? label, string name, params string[] parameters)
        {
            return AppendElement(type, label, element => element.Name = $"{name}{CountIf(type) + 1}", parameters);
        }

        AxisElement AppendMeanLike(AxisElementType type, string? label, string name, string variable = "", string expression = "")
        {
            if (string.IsNullOrEmpty(expression))
            {
                return AppendElement(type, label, expr => expr.Name = $"{name}{CountIf(type) + 1}", variable);
            }
            return AppendElement(type, label, expr => expr.Name = $"{name}{CountIf(type) + 1}", variable, expression);
        }

        AxisElement AppendNamedMeanLike(AxisElementType type, string name, string? label = null, string variable = "", string expression = "")
        {
            if (string.IsNullOrEmpty(expression))
            {
                return AppendElement(type, label, e => e.Name = name, variable);
            }
            return AppendElement(type, label, e => e.Name = name, variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个CategoryRange类型的轴表达式元素，语法格式为[lower]..[upper]
        /// </summary>
        /// <param name="lowerBoundary"></param>
        /// <param name="upperBoundary"></param>
        /// <returns></returns>
        public AxisElement AppendCategoryRange(string lowerBoundary = "", string upperBoundary = "")
        {
            return AppendNamedElement(AxisElementType.CategoryRange, "", "CategoryRange", lowerBoundary, upperBoundary);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个text类型的轴表达式元素，可以修改标签
        /// </summary>
        /// <param name="label"></param>
        public AxisElement AppendText(string? label = "")
        {
            return AppendNamedElement(AxisElementType.Text, label, "e");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个base类型的轴表达式元素，可以修改标签和一个参数
        /// </summary>
        /// <param name="label">base元素的标签</param>
        /// <param name="parameter">base元素的参数，一般为表达式或true</param>
        public AxisElement AppendBase(string? label = null, string parameter = "")
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
        public AxisElement AppendUnweightedBase(string? label = null)
        {
            return AppendNamedElement(AxisElementType.UnweightedBase, label, "unweightedbase");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个EffectiveBase类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        public AxisElement AppendEffectiveBase(string? label = null)
        {
            return AppendNamedElement(AxisElementType.EffectiveBase, label, "effectivebase");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Expression类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public AxisElement AppendExpression(string? label = null, string expression = "")
        {
            return AppendNamedElement(AxisElementType.Expression, label, "expr", expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Expression类型的轴表达式元素，此方法可以自定义变量名，
        /// 可以用于后续计算
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public AxisElement AppendNamedExpression(string name, string label = "", string expression = "")
        {
            return AppendElement(AxisElementType.Expression, label, e => e.Name = name, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Numeric类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendNumeric(string? label = null, string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Numeric, label, "numeric", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Derived类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public AxisElement AppendDerived(string? label = null, string expression = "")
        {
            return AppendNamedElement(AxisElementType.Derived, label, "dev", expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Derived类型的轴表达式元素，可以自定义元素的名称，
        /// 可以用于后续添加表达式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public AxisElement AppendNamedDerived(string name, string? label = null, string expression = "")
        {
            return AppendElement(AxisElementType.Derived, label, d => d.Name = name, expression);
        }

        /// <summary>
        /// 向当前集合的末尾追加一个可命名的Mean类型轴元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label"></param>
        /// <param name="variable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public AxisElement AppendNamedMean(string name, string? label = null, string variable = "", string expression = "")
        {
            return AppendNamedMeanLike(AxisElementType.Mean, name, label, variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Mean类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendMean(string? label = null, string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Mean, label, "mean", variable, expression);
        }

        /// <summary>
        /// 向当前集合末尾添加一个可以命名的StdErr类型的轴表达式元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label"></param>
        /// <param name="variable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public AxisElement AppendNamedStdErr(string name, string? label = null, string variable = "", string expression = "")
        {
            return AppendNamedMeanLike(AxisElementType.StdErr, name, label, variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个StdErr类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendStdErr(string? label = null, string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.StdErr, label, "stderr", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个可命名的StdDev类型的轴表达式元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label"></param>
        /// <param name="variable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public AxisElement AppendNamedStdDev(string name, string? label = null, string variable = "", string expression = "")
        {
            return AppendNamedMeanLike(AxisElementType.StdDev, name, label, variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个StdDev类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendStdDev(string? label = null, string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.StdDev, label, "stddev", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Total类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <returns></returns>
        public AxisElement AppendTotal(string? label = null)
        {
            return AppendNamedElement(AxisElementType.Total, label, "total");
        }

        /// <summary>
        /// 向当前集合的末尾添加一个SubTotal类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <returns></returns>
        public AxisElement AppendSubTotal(string? label = null)
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
        public AxisElement AppendMin(string? label = null, string variable = "", string expression = "")
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
        public AxisElement AppendMax(string? label = null, string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Max, label, "max", variable, expression);
        }

        /// <summary>
        /// 向集合末尾添加一个Net类型的轴表达式元素，必须提供码号参数，字符串中间由逗号分隔
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="codes">需要包含的码号</param>
        /// <returns></returns>
        public AxisElement AppendNet(string? label = null, string codes = "")
        {
            return AppendNamedElement(AxisElementType.Net, label, "net", codes);
        }

        /// <summary>
        /// 向集合末尾添加一个Combine类型的轴表达式元素，提供码号参数，字符串中间由逗号分隔
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="codes">需要包含的码号</param>
        /// <returns></returns>
        public AxisElement AppendCombine(string? label = null, string codes = "..")
        {
            return AppendNamedElement(AxisElementType.Combine, label, "com", codes);
        }

        /// <summary>
        /// 向集合末尾添加一个可命名的Net类型的轴表达式元素
        /// </summary>
        /// <param name="name">元素名</param>
        /// <param name="label">元素描述</param>
        /// <param name="codes"></param>
        /// <returns></returns>
        public AxisElement AppendNamedNet(string name, string? label = null, string codes = "..")
        {
            return AppendElement(AxisElementType.Net, label, net => net.Name = name, codes);
        }

        /// <summary>
        /// 向集合末尾添加一个可命名的Combine类型的轴表达式元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label"></param>
        /// <param name="codes"></param>
        /// <returns></returns>
        public AxisElement AppendNamedCombine(string name, string? label = null, string codes = "..")
        {
            return AppendElement(AxisElementType.Combine, label, combine => combine.Name = name, codes);
        }
        /// <summary>
        /// 向当前集合的末尾添加一个Sum类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <param name="variable">基于的变量名</param>
        /// <param name="expression">可选的筛选器表达式</param>
        /// <returns></returns>
        public AxisElement AppendSum(string? label = null, string variable = "", string expression = "")
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
        public AxisElement AppendMedian(string? label = null, string variable = "", string expression = "")
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
        public AxisElement AppendPercentile(string? label = null, string variable = "", double cutOffValue = 50, string expression = "")
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
        public AxisElement AppendMode(string? label = null, string variable = "", string expression = "")
        {
            return AppendMeanLike(AxisElementType.Mode, label, "mode", variable, expression);
        }

        /// <summary>
        /// 向当前集合的末尾添加一个Ntd类型的轴表达式元素，可以添加标签描述
        /// </summary>
        /// <param name="label">可添加的描述</param>
        /// <returns></returns>
        public AxisElement AppendNtd(string? label = null)
        {
            return AppendNamedElement(AxisElementType.Ntd, label, "ntd");
        }

        /// <summary>
        /// 向当前集合末尾添加一个插入的新本地变量轴表达元素，只需要设置变量名，最终会以字符串拼接的形式写入文件。
        /// </summary>
        /// <param name="variable"></param>
        public AxisElement AppendInsertVariable(string variable = "")
        {
            return AppendElement(AxisElementType.InsertFunctionOrVariable, "", null, variable);
        }

        /// <summary>
        /// 向当前集合末尾添加一个插入的新本地函数调用表达式元素，需要已知的函数模板，最终会以字符串拼接的形式写入文件。
        /// </summary>
        /// <param name="function"></param>
        public AxisElement AppendInsertFunction(FunctionTemplate? function = null)
        {
            return AppendElement(AxisElementType.InsertFunctionOrVariable, "", null, function == null ? "" : function);
        }

        ///// <summary>
        ///// 向当前集合末尾添加一个所有Category的表达式元素'..'
        ///// </summary>
        //public void AppendAllCategory()
        //{
        //    AppendElement(AxisElementType.CategoryRange, string.Empty, null);
        //}

        /// <summary>
        /// 向当前集合末尾添加单个Category元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="label"></param>
        public AxisElement AppendCategory(string name = "", string? label = null)
        {
            return AppendNamedElement(AxisElementType.Category, label, name);
        }

        /// <summary>
        /// 从已有的轴表达式读取元素并存入集合
        /// </summary>
        /// <param name="expression"></param>
        public void FromString(string expression)
        {
            Clear();
            var parser = new AxisParser(this);
            parser.Parse(expression);
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
        /// <summary>
        /// Metadata元数据定义时使用的轴表达式，格式：axis("{....}")
        /// </summary>
        MetadataAxis
    }

    public class AxisElement : SpecObject
    {
        internal AxisElement(AxisElement axisElement) : base()
        {
            _parent = axisElement.Parent;
            _template = new AxisElementTemplate(axisElement.Template);
            _suffix = new AxisElementSuffixCollection(axisElement.Suffix);
        }

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
        string? _description;
        readonly AxisElementTemplate _template;
        readonly AxisElementSuffixCollection _suffix;

        bool _exclude = false;

        /// <summary>
        /// 轴表达式元素的名称
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// 轴表达式元素的描述
        /// </summary>
        public string? Description { get => _description; set => _description = value; }
        /// <summary>
        /// 轴表达式元素模板
        /// </summary>
        public AxisElementTemplate Template => _template;
        /// <summary>
        /// 当前元素的后缀配置集合
        /// </summary>
        public AxisElementSuffixCollection Suffix => _suffix;
        /// <summary>
        /// 当前的轴元素是否是被排除在外的
        /// </summary>
        public bool Exclude { get => _exclude; internal set => _exclude = value; }

        /// <summary>
        /// 计算模板参数中指定轴元素类型值的数量，会递归计算子元素的子元素
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CountChild(AxisElementType type)
        {
            int count = 0;

            for (int i = 0; i < _template.Count; i++)
            {
                if (_template.GetParameter(i)?.GetValue() is AxisElement element)
                {
                    if (element.Template.ElementType == type)
                    {
                        count++;
                    };
                    count += element.CountChild(type);
                }
            }

            return count;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{(_exclude ? "^" : "")}{((string.IsNullOrEmpty(_name) || _template.ElementType == AxisElementType.CategoryRange) ? "" : _name)}{((_description == null || _template.ElementType == AxisElementType.CategoryRange) ? "" : $" '{_description}'")}{(string.IsNullOrEmpty(_name) || _template.ElementType == AxisElementType.CategoryRange ? "" : " ")}{_template}{_suffix}";
        }
    }


    public enum AxisElementType
    {
        None,
        Category,
        CategoryRange,
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
        Ntd,
    }

    /// <summary>
    /// 轴表达式元素的字符串文本模板
    /// </summary>
    public class AxisElementTemplate : SpecObject
    {
        internal AxisElementTemplate(AxisElementTemplate template) : base()
        {
            _parent = template.Parent;
            _objectType = template.SpecObjectType;
            _parameters = new SpecObjectCollection<AxisElementParameter>(collection => new AxisElementParameter(collection));
            foreach (AxisElementParameter parameter in template._parameters)
            {
                _parameters.Add(new AxisElementParameter(parameter));
            }
        }

        internal AxisElementTemplate(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisElementTemplate;
            _parameters = new SpecObjectCollection<AxisElementParameter>(this, collection => new AxisElementParameter(collection));
        }

        AxisElementType _elementType = AxisElementType.None;
        readonly SpecObjectCollection<AxisElementParameter> _parameters;

        /// <summary>
        /// 元素的类型
        /// </summary>
        public AxisElementType ElementType { get => _elementType; set => _elementType = value; }

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
        /// 将参数插入到指定索引位置，如果索引无效，将插入到最后
        /// </summary>
        /// <param name="index"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool InsertParameter(int index, AxisElementParameter parameter)
        {
            return _parameters.Insert(index, parameter);
        }

        /// <summary>
        /// 获取指定索引位置的参数对象，如果不存在，返回null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AxisElementParameter? GetParameter(int index)
        {
            return index >= 0 && index < _parameters.Count ? _parameters[index] : null;
        }

        /// <summary>
        /// 交换两个参数的位置，如果交换成功，返回true，失败返回false
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        /// <returns></returns>
        public bool Swap(int sourceIndex, int targetIndex)
        {
            return _parameters.Swap(sourceIndex, targetIndex);
        }

        /// <summary>
        /// 当前模板中参数的数量
        /// </summary>
        public int Count => _parameters.Count;

        /// <summary>
        /// 移除索引位置的模板参数
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            _parameters.RemoveAt(index);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // 移除值为空的参数
            //_parameters.RemoveIf(e => string.IsNullOrEmpty(e.ToString()));
            return _elementType switch
            {
                AxisElementType.InsertFunctionOrVariable => $"\" + {(_parameters.Count > 0 ? _parameters[0] : "")} + \"",
                AxisElementType.Text => "text()",
                AxisElementType.Base => $"base({(_parameters.Count > 0 ? $"'{_parameters[0]}'" : "")})",
                AxisElementType.UnweightedBase => $"unweightedbase({(_parameters.Count > 0 ? $"'{_parameters[0]}'" : "")})",
                AxisElementType.Mean => $"mean({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.StdDev => $"stddev({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.StdErr => $"stderr({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Total => "total()",
                AxisElementType.SubTotal => "subtotal()",
                AxisElementType.Min => $"min({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Max => $"max({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Net => $"net({{{string.Join<AxisElementParameter>(',', _parameters.ToArray())}}})",
                AxisElementType.Combine => $"combine({{{string.Join<AxisElementParameter>(',', _parameters.ToArray())}}})",
                AxisElementType.Expression => $"expression('{(_parameters.Count > 0 ? _parameters[0] : "")}')",
                AxisElementType.Numeric => $"numeric({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Derived => $"derived('{(_parameters.Count > 0 ? _parameters[0] : "")}')",
                AxisElementType.Sum => $"sum({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.EffectiveBase => "effectivebase()",
                AxisElementType.Median => $"median({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Percentile => $"percentile({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", {_parameters[1]}" : "")}{(_parameters.Count > 2 ? $", '{_parameters[2]}'" : "")})",
                AxisElementType.Mode => $"mode({(_parameters.Count > 0 ? _parameters[0] : "")}{(_parameters.Count > 1 ? $", '{_parameters[1]}'" : "")})",
                AxisElementType.Ntd => "ntd()",
                AxisElementType.Category => "",
                AxisElementType.CategoryRange => $"{(_parameters.Count > 0 ? _parameters[0] : "")}..{(_parameters.Count > 1 ? _parameters[1] : "")}",
                _ => string.Empty,
            };
        }

    }


    public class AxisElementParameter : SpecObject
    {
        internal AxisElementParameter(AxisElementParameter parameter) : base()
        {
            _parent = parameter.Parent;
            _objectType = parameter.SpecObjectType;
            _document = parameter.Document;
            _value = parameter._value;
        }

        internal AxisElementParameter(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.AxisParameter;
        }

        object _value = string.Empty;

        /// <summary>
        /// 设置当前参数对象的值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(object value)
        {
            _value = value;
        }

        /// <summary>
        /// 获取当前的参数值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// 保存表达式元素后缀配置的集合
    /// </summary>
    public class AxisElementSuffixCollection : SpecObjectCollection<AxisElementSuffix>
    {

        public AxisElementSuffixCollection(AxisElementSuffixCollection suffixes) : base(collection => new AxisElementSuffix(collection))
        {
            _parent = suffixes.Parent;
            _objectType = suffixes.SpecObjectType;
            _document = suffixes.Document;
            foreach (AxisElementSuffix ele in suffixes)
            {
                Add(new AxisElementSuffix(ele));
            }
        }

        public AxisElementSuffixCollection(SpecObject parent) : base(parent, collection => new AxisElementSuffix(collection))
        {
        }

        /// <summary>
        /// 获取特定类型的后缀配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AxisElementSuffix? this[AxisElementSuffixType type]
        {
            get
            {
                return _items.Find(e => e.Type == type);
            }
        }

        /// <summary>
        /// 向当前集合末尾追加固定类型的后缀对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AxisElementSuffix Append(AxisElementSuffixType type)
        {
            return type switch
            {
                AxisElementSuffixType.CalculationScope => AppendCalculationScope(AxisElementSuffixCalculationScope.AllElements),
                AxisElementSuffixType.CountsOnly => AppendCountsOnly(false),
                AxisElementSuffixType.Decimals => AppendDecimals(0),
                AxisElementSuffixType.Factor => AppendFactor(0),
                AxisElementSuffixType.IsFixed => AppendIsFixed(false),
                AxisElementSuffixType.IsHidden => AppendIsHidden(false),
                AxisElementSuffixType.IsHiddenWhenColumn => AppendIsHiddenWhenColumn(false),
                AxisElementSuffixType.IsHiddenWhenRow => AppendIsHiddenWhenRow(false),
                AxisElementSuffixType.IncludeInBase => AppendIncludeInBase(false),
                AxisElementSuffixType.IsUnweighted => AppendIsUnweighted(false),
                AxisElementSuffixType.Multiplier => AppendMultiplier(""),
                AxisElementSuffixType.Weight => AppendWeight(""),
                _ => AppendItem(type, ""),
            };
        }

        AxisElementSuffix AppendItem(AxisElementSuffixType type, object value)
        {
            AxisElementSuffix? elementSuffix = _items.Find(x => x.Type == type);
            if (elementSuffix != null)
            {
                elementSuffix.Value = value;
            }
            else
            {
                elementSuffix = NewObject();
                elementSuffix.Type = type;
                elementSuffix.Value = value;
                Add(elementSuffix);
            }
            return elementSuffix;
        }

        /// <summary>
        /// 向当前集合末尾添加CalculationScope类型的配置字段
        /// </summary>
        /// <param name="scope"></param>
        public AxisElementSuffix AppendCalculationScope(AxisElementSuffixCalculationScope scope)
        {
            return AppendItem(AxisElementSuffixType.CalculationScope,
                scope == AxisElementSuffixCalculationScope.AllElements ? "AllElements" : "PrecedingElements");
        }
        
        /// <summary>
        /// 向当前集合末尾添加CountsOnly类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendCountsOnly(bool value)
        {
            return AppendItem(AxisElementSuffixType.CountsOnly, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加Decimals类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendDecimals(int value)
        {
            return AppendItem(AxisElementSuffixType.Decimals, value);
        }

        /// <summary>
        /// 向当前集合末尾添加Factor类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendFactor(double value)
        {
            return AppendItem(AxisElementSuffixType.Factor, value);
        }

        /// <summary>
        /// 向当前集合末尾添加IsFixed类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendIsFixed(bool value)
        {
            return AppendItem(AxisElementSuffixType.IsFixed, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsHidden类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendIsHidden(bool value)
        {
            return AppendItem(AxisElementSuffixType.IsHidden, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsHiddenWhenColumn类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendIsHiddenWhenColumn(bool value)
        {
            return AppendItem(AxisElementSuffixType.IsHiddenWhenColumn, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsHiddenWhenRow类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendIsHiddenWhenRow(bool value)
        {
            return AppendItem(AxisElementSuffixType.IsHiddenWhenRow, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IncludeInBase类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendIncludeInBase(bool value)
        {
            return AppendItem(AxisElementSuffixType.IncludeInBase, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加IsUnweighted类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendIsUnweighted(bool value)
        {
            return AppendItem(AxisElementSuffixType.IsUnweighted, value ? "True" : "False");
        }

        /// <summary>
        /// 向当前集合末尾添加Multiplier类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendMultiplier(string value)
        {
            return AppendItem(AxisElementSuffixType.Multiplier, value);
        }

        /// <summary>
        /// 向当前集合末尾添加Weight类型的配置字段
        /// </summary>
        /// <param name="value"></param>
        public AxisElementSuffix AppendWeight(string value)
        {
            return AppendItem(AxisElementSuffixType.Weight, value);
        }

        public override string ToString()
        {
            if (_items.Where(suffix => suffix.Type != AxisElementSuffixType.None).Any())
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
        internal AxisElementSuffix(AxisElementSuffix suffix) : base()
        {
            _parent = suffix.Parent;
            _document = suffix.Document;
            _type = suffix.Type;
            _value = suffix.Value;
            _objectType = SpecObjectType.AxisElementSuffix;
        }

        internal AxisElementSuffix(SpecObject parent) : base(parent)
        {
            _type = AxisElementSuffixType.None;
            _objectType = SpecObjectType.AxisElementSuffix;
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
