
namespace IDCA.Bll.MddDocument
{
    public interface IMDMObject
    {
        /// <summary>
        /// 对象名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 父级对象
        /// </summary>
        IMDMObject Parent { get; }

        /// <summary>
        /// 父级文档对象
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        /// 对象属性集合
        /// </summary>
        IProperties Properties { get; }

        /// <summary>
        /// 对象类型
        /// </summary>
        ObjectTypesConstants ObjectTypeValue { get; }

        /// <summary>
        /// 是否是引用变量
        /// </summary>
        bool IsReference { get; }
        
        /// <summary>
        /// 唯一识别符号
        /// </summary>
        string UUID { get; }

        /// <summary>
        /// 是否是系统变量
        /// </summary>
        bool IsSystem { get; }

    }
}
