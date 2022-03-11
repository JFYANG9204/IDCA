﻿
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
{
    public class Script : MDMObject, IScript
    {
        internal Script(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Script;
        }

        string _name = string.Empty;
        bool _default = false;
        string _text = string.Empty;

        new public MDMObjectType ObjectType => _objectType;
        public string Name { get => _name; internal set => _name = value; }
        public bool Default { get => _default; internal set => _default = value; }
        public string Text { get => _text; internal set => _text = value; }
    }

    public class ScriptType : MDMObjectCollection<Script>, IScriptType
    {
        internal ScriptType(IMDMObject parent) : base(parent.Document, parent, collection => new Script(collection))
        {
            _objectType = MDMObjectType.ScriptType;
        }

        string _type = string.Empty;
        string _context = string.Empty;
        InterviewModes _interviewModes = InterviewModes.Web;
        bool _useKeyCodes = false;

        new public MDMObjectType ObjectType => _objectType;
        public string Type { get => _type; internal set => _type = value; }
        public string Context { get => _context; internal set => _context = value; }
        public InterviewModes InterviewMode { get => _interviewModes; internal set => _interviewModes = value; }
        public bool UseKeyCodes { get => _useKeyCodes; internal set => _useKeyCodes = value; }
    }

    public class Scripts : MDMObject, IMDMObjectCollection<ScriptType>
    {
        internal Scripts(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Scripts;
        }

        readonly List<ScriptType> _items = new();

        public ScriptType? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;
        public int Count => _items.Count;
        new public MDMObjectType ObjectType => _objectType;

        public void Add(ScriptType item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public ScriptType NewObject()
        {
            return new ScriptType(this);
        }
    }

    public class RoutingItem : MDMObject, IRoutingItem
    {
        internal RoutingItem(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.RoutingItem;
        }

        string _name = string.Empty;
        string _item = string.Empty;

        public string Name { get => _name; internal set => _name = value; }
        public string Item { get => _item; internal set => _item = value; }
        new public MDMObjectType ObjectType => _objectType;
    }

    public class Routing : MDMObject, IRouting
    {
        internal Routing(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Routing;
        }

        string _context = string.Empty;
        InterviewModes _interviewMode = InterviewModes.Default;
        bool _useKeyCodes = false;
        readonly List<RoutingItem> _items = new();

        public string Context { get => _context; internal set => _context = value; }
        public InterviewModes InterviewMode { get => _interviewMode; internal set => _interviewMode = value; }
        public bool UseKeyCodes { get => _useKeyCodes; internal set => _useKeyCodes = value; }
        public int Count => _items.Count;
        new public MDMObjectType ObjectType => _objectType;

        public RoutingItem? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;

        public void Add(RoutingItem item)
        {
            _items.Add(item);
        }

        public RoutingItem NewObject()
        {
            return new RoutingItem(this);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    public class Routings : MDMObject, IRoutings<Routing, ScriptType>
    {
        internal Routings(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Routings;
        }

        readonly List<Routing> _items = new();
        IMDMObjectCollection<ScriptType>? _scripts;

        new public MDMObjectType ObjectType => _objectType;

        public IMDMObjectCollection<ScriptType>? Scripts { get => _scripts; internal set => _scripts = value; }
        public int Count => _items.Count;
        public Routing? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Add(Routing item)
        {
            _items.Add(item);
        }

        public Routing NewObject()
        {
            return new Routing(this);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }

}
