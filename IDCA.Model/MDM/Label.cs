using System.Collections.Generic;

namespace IDCA.Model.MDM
{
    public class Label : MDMObject
    {
        internal Label(Labels? parent, Contexts? contexts, Languages? languages) : base(parent?.Document, parent)
        {
            _contexts = contexts;
            _context = Context.Default;
            _languages = languages;
            _language = Language.Default;
            _text = string.Empty;
            _objectType = MDMObjectType.Label;
        }

        readonly Contexts? _contexts;
        readonly Languages? _languages;

        string _text;
        Context _context;
        Language _language;

        public string Text { get => _text; internal set => _text = value; }
        public Context Context => _context;
        public Language Language => _language;

        public void Set(string context, string language, string text)
        {
            Context targetContext = _contexts?[context] ?? Context.Default;
            if (!targetContext.IsDefault)
            {
                _context = targetContext;
            }

            Language targetLanguage = _languages?[language] ?? Language.Default;
            if (!targetLanguage.IsDefault)
            {
                _language = targetLanguage;
            }

            _text = text;
        }

    }

    public class Labels : MDMCollection<Label>
    {
        internal Labels(MDMObject? parent, MDMDocument? document, string context) : base(parent?.Document, parent)
        {
            _currentContext = document?.Contexts[context] ?? Context.Default;
        }

        readonly Dictionary<string, Dictionary<string, Label>> _languageLabelsCache = new();
        Context _currentContext;

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
        public Context Context { get => _currentContext; internal set => _currentContext = value; }

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
            return new Label(this, _document?.Contexts, _document?.Languages);
        }
    }


}
