using System.Collections;
using System.Collections.Generic;

namespace IDCA.Model.MDM
{

    public class ContextAlternatives
    {
        internal ContextAlternatives(Context parent)
        {
            _parent = parent;
        }

        public string this[int index] => index >= 0 && index < _alternatives.Count ? _alternatives[index] : "";

        readonly List<string> _alternatives = new();
        readonly Context _parent;

        public int Count => _alternatives.Count;
        public Context Parent => _parent;

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

    public class Context : MDMNamedObject
    {
        internal Context(MDMObject? parent) : base(parent?.Document, parent)
        {
            _objectType = MDMObjectType.Context;
        }

        public static Context Default => new(null) { _isDefault = true };

        ContextAlternatives? _alternatives = null;
        string _description = string.Empty;
        ContextUsage _usage = ContextUsage.Properties;

        public ContextAlternatives? Alternatives => _alternatives;
        public string Description { get => _description; internal set => _description = value; }
        public ContextUsage Usage { get => _usage; internal set => _usage = value; }

        bool _isDefault = false;
        public bool IsDefault { get => _isDefault; private set => _isDefault = value; }


        public ContextAlternatives NewAlternatives()
        {
            return _alternatives = new ContextAlternatives(this);
        }
    }

    public class Contexts : MDMObjectCollection<Context>
    {
        internal Contexts(MDMDocument? document, string @base) : base(document, document, collection => new Context(collection))
        {
            _base = @base;
            _objectType = MDMObjectType.Contexts;
        }

        public Context? this[string name] => !string.IsNullOrEmpty(name) && _cache.ContainsKey(name.ToLower()) ? _cache[name.ToLower()] : null;

        readonly Dictionary<string, Context> _cache = new();
        string _base;

        public string Base { get => _base; internal set => _base = value; }

        public override void Add(Context item)
        {
            if (!_cache.ContainsKey(item.Name.ToLower()))
            {
                _cache.Add(item.Name.ToLower(), item);
                base.Add(item);
            }
        }


    }
    public enum ContextUsage
    {
        Routings,
        Labels,
        Properties,
    }

}
