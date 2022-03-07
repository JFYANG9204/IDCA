
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public interface ILanguage
    {
        /// <summary>
        /// 语言名称，例如：Arabic - Algeria
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 3字符缩写
        /// </summary>
        string ShortCode { get; }
        /// <summary>
        /// 语言长文本表示字符
        /// </summary>
        string LongCode { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties? Properties { get; }
        /// <summary>
        /// 父级集合对象
        /// </summary>
        ILanguages Parent { get; }
        /// <summary>
        /// 根据语言长代码设定语言类型数据
        /// </summary>
        /// <param name="longCode"></param>
        void SetLongCode(string longCode);
        /// <summary>
        /// 提示此对象是否是默认值
        /// </summary>
        bool IsDefault { get; }
    }


    public interface ILanguages : IEnumerable, IMDMCollection<ILanguage>
    {
        /// <summary>
        /// 依据数值索引获取语言对象
        /// </summary>
        /// <param name="index">数值索引</param>
        /// <returns></returns>
        ILanguage this[int index] { get; }
        /// <summary>
        /// 依据语言名称获取语言对象
        /// </summary>
        /// <param name="name">语言名称，不区分大小写</param>
        /// <returns></returns>
        ILanguage this[string longCode] { get; }
        /// <summary>
        /// 当前语言名称
        /// </summary>
        string Current { get; }
        /// <summary>
        /// 基础语言
        /// </summary>
        string Base { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties? Properties { get; }
        /// <summary>
        /// 语言列表所在的文档对象
        /// </summary>
        IDocument Document { get; }
        /// <summary>
        /// 默认语言类型，用于替换此类型的null
        /// </summary>
        ILanguage Default { get; }
    }


}
