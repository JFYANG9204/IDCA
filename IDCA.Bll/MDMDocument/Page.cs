
namespace IDCA.Bll.MDMDocument
{
    public class Page : MDMObject, IPage
    {
        internal Page(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Page;
        }

        string _id = string.Empty;
        string _name = string.Empty;
        string _reference = string.Empty;

        new public MDMObjectType ObjectType => _objectType;
        public string Id { get => _id; internal set => _id = value; }
        public string Name { get => _name; internal set => _name = value; }
        public string Reference { get => _reference; internal set => _reference = value; }
    }

    public class Pages : MDMObjectCollection<Page>, IPages<Page>
    {
        internal Pages(IMDMObject parent) : base(parent.Document, parent, collection => new Page(collection))
        {
            _objectType = MDMObjectType.Pages;
        }

        string _name = string.Empty;
        bool _globalNamespace = false;

        new public MDMObjectType ObjectType => _objectType;
        public string Name { get => _name; internal set => _name = value; }
        public bool GlobalNamespace { get => _globalNamespace; internal set => _globalNamespace = value; }
    }

}
