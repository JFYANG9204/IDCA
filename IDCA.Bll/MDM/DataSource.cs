
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.MDM
{
    public class DataSource : MDMObject, IDataSource
    {
        internal DataSource(IMDMDocument document, IMDMObject parent) : base(document, parent)
        {
            _objectType = MDMObjectType.DataSource;
        }

        string _name = string.Empty;
        string _dbLocation = string.Empty;
        string _cdscName = string.Empty;
        string _project = string.Empty;
        string _id = string.Empty;

        public string Name { get => _name; internal set => _name = value; }
        public string DBLocation { get => _dbLocation; internal set => _dbLocation = value; }
        public string CDSCName { get => _cdscName; internal set => _cdscName = value; }
        public string Project { get => _project; internal set => _project = value; }
        public string Id { get => _id; internal set => _id = value; }
    }

    public class DataSources : MDMObject, IMDMObjectCollection<DataSource>
    {
        internal DataSources(IMDMDocument document) : base(document, document)
        {
            _objectType = MDMObjectType.DataSources;
        }

        string _default = string.Empty;
        readonly List<DataSource> _items = new();

        public string Default { get => _default; internal set => _default = value; }
        public DataSource? this[int index] => index >= 0 && index < _items.Count ? _items[index] : null;
        public int Count => _items.Count;
        new public MDMObjectType ObjectType => _objectType;

        public void Add(DataSource item)
        {
            _items.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public DataSource NewObject()
        {
            return new DataSource(_document, this);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }

}
