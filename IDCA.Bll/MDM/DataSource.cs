
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Model.MDM
{
    public class DataSource : MDMObject
    {
        internal DataSource(MDMDocument? document, MDMObject? parent) : base(document, parent)
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

    public class DataSources : MDMObjectCollection<DataSource>
    {
        internal DataSources(MDMDocument? document) : base(document, document, collection => new DataSource(collection.Document, collection))
        {
            _objectType = MDMObjectType.DataSources;
        }

        string _default = string.Empty;

        public string Default { get => _default; internal set => _default = value; }
        new public MDMObjectType ObjectType => _objectType;

    }

}
