using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
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
    }

    public class Context : IContext
    {
        internal Context(IContexts parent)
        {
            _parent = parent;
        }

        readonly IContexts _parent;
        string _name = string.Empty;
        IContextAlternatives? _alternatives = null;
        string _description = string.Empty;
        ContextUsage _usage = ContextUsage.Properties;

        public string Name { get => _name; internal set => _name = value; }
        public IContextAlternatives? Alternatives => _alternatives;
        public string Description { get => _description; internal set => _description = value; }
        public ContextUsage Usage { get => _usage; internal set => _usage = value; }
        public IContexts Parent => _parent;
        public bool IsDefault => string.IsNullOrEmpty(_name);

        public IContextAlternatives NewAlternatives()
        {
            return _alternatives = new ContextAlternatives(this);
        }
    }

    public class Contexts : IContexts
    {
        internal Contexts(IDocument document, string @base)
        {
            _base = @base;
            _document = document;
            _default = new Context(this);
        }

        readonly string _base;
        readonly IDocument _document;
        readonly List<IContext> _items = new();
        readonly Dictionary<string, IContext> _cache = new();
        readonly IContext _default;
        
        public IContext this[string name] => _cache.ContainsKey(name.ToLower()) ? _cache[name.ToLower()] : Default;
        public string Base => _base;
        public int Count => _items.Count;
        public IDocument Document => _document;
        public IContext Default => _default;

        public void Add(IContext item)
        {
            string lName = item.Name.ToLower();
            if (!string.IsNullOrEmpty(lName) && !_cache.ContainsKey(lName))
            {
                _items.Add(item);
                _cache.Add(lName, item);
            }
        }

        public IContext NewObject()
        {
            return new Context(this);
        }
    }
}
