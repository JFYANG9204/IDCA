
using IDCA.Model.Spec;

namespace IDCA.Model
{
    public static class Converter
    {
        /// <summary>
        /// 将配置数据转换成AxisTopBottomBoxPosition枚举类型数据，
        /// 默认值是BeforeAllCategory
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AxisTopBottomBoxPosition ConvertToAxisTopBottomBoxPosition(object? value)
        {
            if (value != null &&
                value is int intValue &&
                intValue >= (int)AxisTopBottomBoxPosition.BeforeAllCategory &&
                intValue <= (int)AxisTopBottomBoxPosition.AfterSigma)
            {
                return (AxisTopBottomBoxPosition)intValue;
            }
            return AxisTopBottomBoxPosition.BeforeAllCategory;
        }

        /// <summary>
        /// 将字符串转换为其对应的AxisElementType类型值，不区分大小写
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static AxisElementType ConvertToAxisElementType(string element)
        {
            return element.ToLower() switch
            {
                ".." => AxisElementType.AllCategory,
                "text" => AxisElementType.Text,
                "base" => AxisElementType.Base,
                "unweightedbase" => AxisElementType.UnweightedBase,
                "effectivebase" => AxisElementType.EffectiveBase,
                "expression" => AxisElementType.Expression,
                "numeric" => AxisElementType.Numeric,
                "derived" => AxisElementType.Derived,
                "mean" => AxisElementType.Mean,
                "stderr" => AxisElementType.StdErr,
                "stddev" => AxisElementType.StdDev,
                "total" => AxisElementType.Total,
                "subtotal" => AxisElementType.SubTotal,
                "min" => AxisElementType.Min,
                "max" => AxisElementType.Max,
                "net" => AxisElementType.Net,
                "combine" => AxisElementType.Combine,
                "sum" => AxisElementType.Sum,
                "median" => AxisElementType.Median,
                "percentile" => AxisElementType.Percentile,
                "mode" => AxisElementType.Mode,
                "ntd" => AxisElementType.Ntd,
                _ => AxisElementType.None
            };
        }
        /// <summary>
        /// 将字符串转换为对应的轴表达式后缀类型
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static AxisElementSuffixType ConvertToAxisElementSuffixType(string suffix)
        {
            return suffix.ToLower() switch
            {
                "calculationscope" => AxisElementSuffixType.CalculationScope,
                "countsonly" => AxisElementSuffixType.CountsOnly,
                "decimals" => AxisElementSuffixType.Decimals,
                "factor" => AxisElementSuffixType.Factor,
                "isfixed" => AxisElementSuffixType.IsFixed,
                "ishidden" => AxisElementSuffixType.IsHidden,
                "ishiddenwhencolumn" => AxisElementSuffixType.IsHiddenWhenColumn,
                "ishiddenwhenrow" => AxisElementSuffixType.IsHiddenWhenRow,
                "includeinbase" => AxisElementSuffixType.IncludeInBase,
                "isunweighted" => AxisElementSuffixType.IsUnweighted,
                "multiplier" => AxisElementSuffixType.Multiplier,
                "weight" => AxisElementSuffixType.Weight,
                _ => AxisElementSuffixType.None
            };
        }
    }
}