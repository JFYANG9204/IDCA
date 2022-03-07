
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
        ISaveLog? this[int index] { get; }
        /// <summary>
        /// 所在的文档对象
        /// </summary>
        IDocument Document { get; }
    }

    public interface ISaveLog
    {
        /// <summary>
        /// 存储时间
        /// </summary>
        DateTime Date { get; }
        /// <summary>
        /// 版本集合
        /// </summary>
        string VersionSet { get; }
        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }
        /// <summary>
        /// 文件版本
        /// </summary>
        string FileVersion { get; }
        /// <summary>
        /// 用户对象集合
        /// </summary>
        IMDMUsers? Users { get; }
        /// <summary>
        /// 存储计数
        /// </summary>
        int SaveCount { get; }
        /// <summary>
        /// 父级对象，一定是ISaveLogs集合对象
        /// </summary>
        ISaveLogs Parent { get; }
        /// <summary>
        /// 创建新的MDMUsers对象，并将其赋值给Users属性，最后将其返回
        /// </summary>
        /// <returns></returns>
        IMDMUsers NewUsers();
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
        /// <summary>
        /// 父级对象
        /// </summary>
        ISaveLog Parent { get; }
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
