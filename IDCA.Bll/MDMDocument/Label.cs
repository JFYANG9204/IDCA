
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
{
    public class Label : ILabel
    {
        internal Label(ILabels parent, IContexts contexts, ILanguages languages)
        {
            _parent = parent;
            _contexts = contexts;
            _context = _contexts.Default;
            _languages = languages;
            _language = _languages.Default;
            _text = string.Empty;
        }

        readonly ILabels _parent;
        readonly IContexts _contexts;
        readonly ILanguages _languages;

        string _text;
        IContext _context;
        ILanguage _language;

        public string Text { get => _text; internal set => _text = value; }
        public ILabels Parent => _parent;
        public IContext Context => _context;
        public ILanguage Language => _language;

        public void Set(string context, string language, string text)
        {
            IContext targetContext = _contexts[context];
            if (targetContext.IsDefault)
            {
                _context = targetContext;
            }

            ILanguage targetLanguage = _languages[context];
            if (targetLanguage.IsDefault)
            {
                _language = targetLanguage;
            }

            _text = text;
        }

    }

    public class Labels : ILabels
    {
        internal Labels(IMDMObject parent, IDocument document, string context)
        {
            _parent = parent;
            _document = document;
            _currentContext = document.Contexts[context];
        }

        readonly IMDMObject _parent;
        readonly IDocument _document;
        readonly List<ILabel> _items = new();
        readonly Dictionary<string, Dictionary<string, ILabel>> _languageLabelsCache = new();
        readonly IContext _currentContext;

        public ILabel? this[string language, string context = ""]
        {
            get
            {
                string lowerLanguage = language.ToLower();
                if (!string.IsNullOrEmpty(language) && _languageLabelsCache.ContainsKey(lowerLanguage))
                {
                    var labels = _languageLabelsCache[lowerLanguage];

                    string lcontext;
                    if (string.IsNullOrEmpty(context))
                    {
                        lcontext = _currentContext.Name.ToLower();
                        if (!labels.ContainsKey(lcontext))
                        {
                            return null;
                        }
                        return labels[lcontext];
                    }

                    lcontext = context.ToLower();
                    if (labels.ContainsKey(lcontext))
                    {
                        return labels[lcontext];
                    }
                }
                return null;
            }
        }
        public IContext Context => _currentContext;
        public IProperties? Properties { get; internal set; } = null;
        public int Count => _items.Count;
        public IMDMObject Parent => _parent;
        public IDocument Document => _document;

        public void Add(ILabel item)
        {
            _items.Add(item);
            string lowerLanguage = item.Language.LongCode.ToLower();
            string lowerContext = item.Context.Name.ToLower();
            if (!_languageLabelsCache.ContainsKey(lowerLanguage))
            {
                _languageLabelsCache.Add(lowerLanguage, new());
                _languageLabelsCache[lowerLanguage].Add(lowerContext, item);
            }
            else
            {
                var contextLabels = _languageLabelsCache[lowerLanguage];
                if (!contextLabels.ContainsKey(lowerContext))
                {
                    contextLabels.Add(lowerContext, item);
                }
                else
                {
                    contextLabels[lowerContext] = item;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public ILabel NewObject()
        {
            return new Label(this, _document.Contexts, _document.Languages);
        }
    }


}
