
using System.Collections.Generic;

namespace IDCA.Bll.SpecDocument
{
    public interface IAxis : ISpecObjectCollection<IAxisElement>
    {
        /// <summary>
        /// 轴表达式类型
        /// </summary>
        public AxisType Type { get; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
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

    public interface IAxisElement : ISpecObject
    {
        /// <summary>
        /// 轴表达式元素的名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 轴表达式元素的描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 轴表达式元素模板
        /// </summary>
        IAxisElementTemplate Template { get; }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
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
    public interface IAxisElementTemplate : ISpecObject
    {
        /// <summary>
        /// 模板需要的参数数量
        /// </summary>
        int RequireParamNumber { get; }
        /// <summary>
        /// 参数列表
        /// </summary>
        ISpecObjectCollection<IAxisParameter> Parameters { get; }
        /// <summary>
        /// 元素的类型
        /// </summary>
        AxisElementType ElementType { get; }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
    }


    public interface IAxisParameter : ISpecObject
    {
        /// <summary>
        /// 是否是Categorical类型
        /// </summary>
        bool IsCategorical { get; }
        /// <summary>
        /// 参数元素列表
        /// </summary>
        List<string> Items { get; }
        /// <summary>
        /// 向集合中添加元素
        /// </summary>
        /// <param name="item"></param>
        void Add(string item);
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
    }

}
