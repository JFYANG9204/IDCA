
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
{
    public class SaveLog : MDMObject, ISaveLog
    {
        internal SaveLog(IMDMObjectCollection<SaveLog> parent) : base(parent.Document, parent)
        {
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

    public class SaveLogs : MDMObjectCollection<SaveLog>, IMDMObjectCollection<SaveLog>
    {
        internal SaveLogs(IMDMDocument document) : base(document, document, collection => new SaveLog(collection))
        {
        }
    }
}
