using System.Collections.Generic;

namespace IDCA.Model.MDM
{
    public class Label : MDMObject, ILabel
    {
        internal Label(Labels parent, IContexts<Context> contexts, ILanguages<Language> languages) : base(parent.Document, parent)
        {
            _contexts = contexts;
            _context = _contexts.Default;
            _languages = languages;
            _language = _languages.Default;
            _text = string.Empty;
            _objectType = MDMObjectType.Label;
        }

        readonly IContexts<Context> _contexts;
        readonly ILanguages<Language> _languages;

        string _text;
        IContext _context;
        ILanguage _language;

        public string Text { get => _text; internal set => _text = value; }
        public IContext Context => _context;
        public ILanguage Language => _language;

        public void Set(string context, string language, string text)
        {
            IContext targetContext = _contexts[context] ?? _contexts.Default;
            if (!targetContext.IsDefault)
            {
                _context = targetContext;
            }

            ILanguage targetLanguage = _languages[language] ?? _languages.Default;
            if (!targetLanguage.IsDefault)
            {
                _language = targetLanguage;
            }

            _text = text;
        }

    }

    public class Labels : MDMCollection<Label>, ILabels<Label>
    {
        internal Labels(IMDMObject parent, IMDMDocument document, string context) : base(parent.Document, parent)
        {
            _currentContext = document.Contexts[context] ?? document.Contexts.Default;
        }

        readonly Dictionary<string, Dictionary<string, Label>> _languageLabelsCache = new();
        IContext _currentContext;

        new public Label? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;

        public Label? this[string language, string context = ""]
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
        public IContext Context { get => _currentContext; internal set => _currentContext = value; }
        public Properties? Properties { get; internal set; } = null;
        public MDMObjectType ObjectType => MDMObjectType.Labels;
        public Properties? Templates { get; internal set; } = null;

        public override void Add(Label item)
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

        public override Label NewObject()
        {
            return new Label(this, _document.Contexts, _document.Languages);
        }
    }


}
