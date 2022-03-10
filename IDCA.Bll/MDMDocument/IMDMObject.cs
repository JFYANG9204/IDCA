
namespace IDCA.Bll.MDMDocument
{
    public interface IMDMObject
    {
        /// <summary>
        /// 对象的类型
        /// </summary>
        MDMObjectType ObjectType { get; }
        /// <summary>
        /// 父对象
        /// </summary>
        IMDMObject Parent { get; }
        /// <summary>
        /// 所在的文档对象
        /// </summary>
        IMDMDocument Document { get; }
        /// <summary>
        /// 属性集合，可以为空
        /// </summary>
        IProperties<Property>? Properties { get; }
    }

    public interface IMDMNamedObject : IMDMObject
    {
        /// <summary>
        /// 对象的ID编号
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 对象名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 是否引用自其他对象
        /// </summary>
        bool IsReference { get; }
        /// <summary>
        /// 是否是系统变量
        /// </summary>
        bool IsSystem { get; }
    }

    public interface IMDMLabeledObject : IMDMNamedObject
    {
        /// <summary>
        /// 标签集合
        /// </summary>
        Labels? Labels { get; }
        /// <summary>
        /// 当前默认的标签
        /// </summary>
        string Label { get; }
        /// <summary>
        /// 标签格式配置，可以为空
        /// </summary>
        Style? LabelStyles { get; }
        /// <summary>
        /// 外观配置
        /// </summary>
        Style? Style { get; }
    }

    public interface IMDMRange
    {
        /// <summary>
        /// 最小值，针对不同的数据类型可能有不同类型的值，没有设定时返回null
        /// </summary>
        object MinValue { get; }
        /// <summary>
        /// 最大值，针对不同的数据类型可能有不同类型的值，没有设定时返回null
        /// </summary>
        object MaxValue { get; }
        /// <summary>
        /// 最小的有效值，没有设定时返回null
        /// </summary>
        object EffectiveMinValue { get; }
        /// <summary>
        /// 最大有效值，没有设定时返回null
        /// </summary>
        object EffectiveMaxValue { get; }
    }

    public enum MDMDataType
    {
        None = -1,
        Info = 0,
        Long = 1,
        Text = 2,
        Categorical = 3,
        Date = 5,
        Double = 6,
        Boolean = 7,
    }

    public enum MDMObjectType
    {
        Unknown,
        Variable,
        Grid,
        Class,
        Element,
        Elements,
        Categories,
        Label,
        Labels,
        Field,
        HelperFields,
        Fields,
        Type,
        Types,
        Property,
        Properties,
        Routing,
        Routings,
        Context,
        Contexts,
        Language,
        Languages,
        VariableInstance,
        RoutingItem,
        Compound,
        RoutingItems,
        Variables,
        Mapping,
        Script,
        Scripts,
        ScriptType,
        ScriptTypes,
        Page,
        Pages,
        SaveLog,
        SaveLogs,
        CategoryMap,
        DataSource,
        DataSources,
    }

}
