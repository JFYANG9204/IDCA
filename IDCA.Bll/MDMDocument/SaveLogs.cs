
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
{
    public class MDMUsers : IMDMUsers
    {
        internal MDMUsers(ISaveLog saveLog)
        {
            _parent = saveLog;
        }

        readonly List<MDMUser> _list = new();
        readonly Dictionary<string, MDMUser> _cache = new();
        readonly ISaveLog _parent;

        public MDMUser? this[int index] => index >= 0 && index < _list.Count ? _list[index] : null; 
        public MDMUser? this[string name] => _cache.ContainsKey(name.ToLower()) ? _cache[name] : null;
        public int Count => _list.Count;
        public ISaveLog Parent => _parent;

        public void Add(MDMUser item)
        {
            string lName = item.Name.ToLower();
            if (!string.IsNullOrEmpty(lName) && !_cache.ContainsKey(lName))
            {
                _list.Add(item);
                _cache.Add(lName, item);
            }
        }

        public MDMUser NewObject()
        {
            return new MDMUser
            {
                Name = "",
                FileVersion = "",
                Comment = ""
            };
        }
    }

    public class SaveLog : ISaveLog
    {
        internal SaveLog(ISaveLogs parent)
        {
            _parent = parent;
        }

        readonly ISaveLogs _parent;
        DateTime _date = DateTime.MinValue;
        string _versionSet = "";
        string _userName = "";
        string _fileVersion = "";
        IMDMUsers? _users = null;
        int _saveCount = 0;

        public DateTime Date { get => _date; internal set => _date = value; }
        public string VersionSet { get => _versionSet; internal set => _versionSet = value; }
        public string UserName { get => _userName; internal set => _userName = value; }
        public string FileVersion { get => _fileVersion; internal set => _fileVersion = value; }
        public IMDMUsers? Users => _users;
        public int SaveCount { get => _saveCount; internal set => _saveCount = value; }
        public ISaveLogs Parent => _parent;

        public IMDMUsers NewUsers()
        {
            return _users = new MDMUsers(this);
        }
    }

    public class SaveLogs : ISaveLogs
    {
        internal SaveLogs(IDocument document)
        {
            _document = document;
        }

        readonly IDocument _document;
        readonly List<ISaveLog> _logs = new();

        public ISaveLog? this[int index] => index >= 0 && index < _logs.Count ? _logs[index] : null;

        public int Count => _logs.Count;

        public IDocument Document => _document;

        public void Add(ISaveLog item)
        {
            _logs.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _logs.GetEnumerator();
        }

        public ISaveLog NewObject()
        {
            return new SaveLog(this);
        }
    }
}
