
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public interface ILanguage : IMDMNamedObject
    {
        /// <summary>
        /// 3字符缩写
        /// </summary>
        string ShortCode { get; }
        /// <summary>
        /// 语言长文本表示字符
        /// </summary>
        string LongCode { get; }
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


    public interface ILanguages<T> : IEnumerable, IMDMNamedCollection<T> where T : ILanguage
    {
        /// <summary>
        /// 当前语言名称
        /// </summary>
        string Current { get; }
        /// <summary>
        /// 基础语言
        /// </summary>
        string Base { get; }
        /// <summary>
        /// 默认语言类型，用于替换此类型的null
        /// </summary>
        ILanguage Default { get; }
    }


}
