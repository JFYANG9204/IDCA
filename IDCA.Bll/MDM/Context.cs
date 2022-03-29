using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDM
{

    public class ContextAlternatives : IContextAlternatives
    {
        internal ContextAlternatives(IContext parent)
        {
            _parent = parent;
        }

        public string this[int index] => index >= 0 && index < _alternatives.Count ? _alternatives[index] : "";

        readonly List<string> _alternatives = new();
        readonly IContext _parent;

        public int Count => _alternatives.Count;
        public IContext Parent => _parent;

        public void Add(string item)
        {
            if (!string.IsNullOrEmpty(item) && !_alternatives.Contains(item))
            {
                _alternatives.Add(item);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _alternatives.GetEnumerator();
        }

        public string NewObject()
        {
            return string.Empty;
        }

        public void Clear()
        {
            _alternatives.Clear();
        }
    }

    public class Context : MDMNamedObject, IContext
    {
        internal Context(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Context;
        }

        IContextAlternatives? _alternatives = null;
        string _description = string.Empty;
        ContextUsage _usage = ContextUsage.Properties;

        public IContextAlternatives? Alternatives => _alternatives;
        public string Description { get => _description; internal set => _description = value; }
        public ContextUsage Usage { get => _usage; internal set => _usage = value; }
        public bool IsDefault => string.IsNullOrEmpty(_name);

        public IContextAlternatives NewAlternatives()
        {
            return _alternatives = new ContextAlternatives(this);
        }
    }

    public class Contexts : MDMObjectCollection<Context>, IContexts<Context>
    {
        internal Contexts(IMDMDocument document, string @base) : base(document, document, collection => new Context(collection))
        {
            _base = @base;
            _default = new Context(this);
            _objectType = MDMObjectType.Contexts;
        }

        public IContext? this[string name] => _cache.ContainsKey(name.ToLower()) ? _cache[name.ToLower()] : null;

        readonly Dictionary<string, IContext> _cache = new();
        string _base;
        readonly IContext _default;

        public string Base { get => _base; internal set => _base = value; }
        public IContext Default => _default;

        public override void Add(Context item)
        {
            if (!_cache.ContainsKey(item.Name.ToLower()))
            {
                _cache.Add(item.Name.ToLower(), item);
                base.Add(item);
            }
        }


    }
}
