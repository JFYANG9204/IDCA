
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Model.MDM
{

    /// <summary>
    /// 存储时的用户信息
    /// </summary>
    public class MDMUser : MDMObject
    {
        internal MDMUser(MDMDocument? document, MDMObject? parent) : base(document, parent)
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

    public class SaveLog : MDMObject
    {
        internal SaveLog(MDMObjectCollection<SaveLog> parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.SaveLog;
        }

        DateTime _date = DateTime.MinValue;
        string _versionSet = "";
        string _userName = "";
        string _fileVersion = "";
        int _saveCount = 0;

        readonly List<MDMUser> _items = new();

        public DateTime Date { get => _date; internal set => _date = value; }
        public string VersionSet { get => _versionSet; internal set => _versionSet = value; }
        public string UserName { get => _userName; internal set => _userName = value; }
        public string FileVersion { get => _fileVersion; internal set => _fileVersion = value; }
        public int SaveCount { get => _saveCount; internal set => _saveCount = value; }
        public int Count => _items.Count;
        public MDMUser? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;

        public void Add(MDMUser item)
        {
            _items.Add(item);
        }

        public MDMUser NewObject()
        {
            return new MDMUser(_document, this);
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Clear()
        {
            _items.Clear();
        }
    }

    public class SaveLogs : MDMObjectCollection<SaveLog>
    {
        internal SaveLogs(MDMDocument document) : base(document, document, collection => new SaveLog(collection))
        {
            _objectType = MDMObjectType.SaveLogs;
        }

    }
}
