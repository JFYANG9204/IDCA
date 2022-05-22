
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

    }
}
