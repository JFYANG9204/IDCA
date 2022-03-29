using System;

namespace IDCA.Bll.MDM
{
    public interface ISaveLog : IMDMCollection<MDMUser>
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
        /// 存储计数
        /// </summary>
        int SaveCount { get; }
    }

    /// <summary>
    /// 存储时的用户信息
    /// </summary>
    public class MDMUser : MDMObject
    {
        internal MDMUser(IMDMDocument document, IMDMObject parent) : base(document, parent)
        {
            Name = string.Empty;
            FileVersion = string.Empty;
            Comment = string.Empty;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户文件版本
        /// </summary>
        public string FileVersion { get; set; }
        /// <summary>
        /// 用户添加的注释
        /// </summary>
        public string Comment { get; set; }
    }

}
