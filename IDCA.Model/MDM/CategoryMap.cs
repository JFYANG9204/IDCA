
using System.Collections.Generic;

namespace IDCA.Model.MDM
{
    public class CategoryMap : MDMObject
    {
        internal CategoryMap(MDMDocument? document) : base(document, document)
        {
            _objectType = MDMObjectType.CategoryMap;
        }

        readonly List<CategoryId> _items = new();
        readonly Dictionary<string, CategoryId> _cache = new();

        public int Count => _items.Count;

        public void Add(string name, string value)
        {
            string lName = name.ToLower();
            if (!string.IsNullOrEmpty(lName) && !_cache.ContainsKey(lName))
            {
                var newItem = new CategoryId
                {
                    Name = name,
                    Value = value
                };
                _items.Add(newItem);
                _cache.Add(lName, newItem);
            }
        }
    }

    public struct CategoryId
    {
        public string Name;
        public string Value;
    }

}
