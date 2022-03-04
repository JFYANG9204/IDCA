
using System.Collections;

namespace IDCA.Bll.MddDocument
{
    public interface ILanguage
    {
        /// <summary>
        /// 语言名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 对应国家缩写
        /// </summary>
        string Country { get; }
        /// <summary>
        /// 语言长文本表示字符
        /// </summary>
        string LongName { get; }
        /// <summary>
        /// 属性集合
        /// </summary>
        IProperties Properties { get; }
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
        ILanguage this[string name] { get; }
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
        IProperties Properties { get; }
    }


}
