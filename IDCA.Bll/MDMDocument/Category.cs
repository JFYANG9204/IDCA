

using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDMDocument
{
    public class Categories : ICategories
    {
        internal Categories(IMDMObject parent, IDocument document)
        {
            _parent = parent;
            _document = document;
        }

        readonly IDocument _document;
        readonly IMDMObject _parent;
        readonly List<IElement> _items = new();
        readonly Dictionary<string, IElement> _itemCache = new();
        IProperties? _properties;
        string _id = string.Empty;
        string _name = string.Empty;
        MDMObjectType _type = MDMObjectType.Unknown;
        bool _isReference = false;
        bool _isSystem = false;

        public IElement? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;
        public IElement? this[string name] => _itemCache.ContainsKey(name.ToLower()) ? _itemCache[name.ToLower()] : null;
        public IProperties? Properties { get => _properties; internal set => _properties = value; }
        public int Count => _items.Count;
        public string Id { get => _id; internal set => _id = value; }
        public string Name { get => _name; internal set => _name = value; }
        public MDMObjectType ObjectType { get => _type; internal set => _type = value; }
        public IMDMObject Parent => _parent;
        public bool IsReference { get => _isReference; internal set => _isReference = value; }
        public bool IsSystem { get => _isSystem; internal set => _isSystem = value; }
        public IDocument Document => _document;

        public void Add(IElement item)
        {
            string lowerName = item.Name.ToLower();
            if (!string.IsNullOrEmpty(lowerName) && _itemCache.ContainsKey(lowerName))
            {
                _itemCache.Add(lowerName, item);
                _items.Add(item);
            }
        }

        public IElement NewObject()
        {
            return new Element(this);
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

}
