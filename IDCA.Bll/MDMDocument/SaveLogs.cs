
using System;
using System.Collections;

namespace IDCA.Bll.MDMDocument
{
    public interface ISaveLogs : IEnumerable, IMDMCollection<ISaveLog>
    {
        /// <summary>
        /// 依据数字索引获取对应位置元素
        /// </summary>
        /// <param name="index">数字索引位置</param>
        /// <returns>对应索引位置的存储对象</returns>
        ISaveLog this[int index] { get; }
    }

    public interface ISaveLog
    {
        DateTime Date { get; }
        string VersionSet { get; }
        string UserName { get; }
        string FileVersion { get; }
        IMDMUsers Users { get; }
        int SaveCount { get; }
    }

    public interface IMDMUsers : IMDMCollection<MDMUser>
    {
        /// <summary>
        /// 依据数字索引获取用户信息对象
        /// </summary>
        /// <param name="index">数字索引</param>
        /// <returns>索引位置的对象，如果索引越线，返回null</returns>
        MDMUser? this[int index] { get; }
        /// <summary>
        /// 依据用户名获取对象
        /// </summary>
        /// <param name="name">索引用户名</param>
        /// <returns>对应用户名的对象，如果不存在，返回null</returns>
        MDMUser? this[string name] { get; }
    }

    /// <summary>
    /// 存储时的用户信息
    /// </summary>
    public struct MDMUser
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name;
        /// <summary>
        /// 用户文件版本
        /// </summary>
        public string FileVersion;
        /// <summary>
        /// 用户添加的注释
        /// </summary>
        public string Comment;
    }

}
