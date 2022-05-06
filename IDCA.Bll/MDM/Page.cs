
namespace IDCA.Model.MDM
{
    public class Page : MDMLabeledObject, IPage
    {
        internal Page(IMDMObject parent) : base(parent.Document, parent)
        {
            _objectType = MDMObjectType.Page;
            _items = new Variables(_document, this);
        }

        string _reference = string.Empty;
        readonly Variables _items;
        public string Reference { get => _reference; internal set => _reference = value; }
        public Variables? Items => _items;
    }

    public class Pages : MDMNamedCollection<Page>, IPages<Page>
    {
        internal Pages(IMDMObject parent) : base(parent.Document, parent, collection => new Page(collection))
        {
            _objectType = MDMObjectType.Pages;
        }

        bool _globalNamespace = false;
        public bool GlobalNamespace { get => _globalNamespace; internal set => _globalNamespace = value; }
    }

}
