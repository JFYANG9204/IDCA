
namespace IDCA.Bll.MDM
{
    public interface IProperty : IMDMNamedObject
    {
        /// <summary>
        /// 属性值，可以是字符串、整数、实数类型
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 属性值类型
        /// </summary>
        public PropertyValueType Type { get; }
        /// <summary>
        /// 上下文类型
        /// </summary>
        string Context { get; }
    }

    /// <summary>
    /// 属性值类型，可以是整数、实数、字符串、集合和布尔类型，其中，布尔类型时，-1为true，0为false
    /// </summary>
    public enum PropertyValueType
    { 
        /// <summary>
        /// 整数数值类型
        /// </summary>
        Integer = 3,
        /// <summary>
        /// 浮点数类型
        /// </summary>
        Decimal = 5,
        /// <summary>
        /// 字符串
        /// </summary>
        Text = 8,
        /// <summary>
        /// 次级属性集合
        /// </summary>
        Collection = 9,
        /// <summary>
        /// 布尔类型，XML数据中，-1为true，0为false
        /// </summary>
        Bool = 11,
    }

    public interface IProperties : IMDMNamedCollection<Property>
    {
        /// <summary>
        /// 属性集合
        /// </summary>
        public Properties? SubProperties { get; internal set; }
    }


}
