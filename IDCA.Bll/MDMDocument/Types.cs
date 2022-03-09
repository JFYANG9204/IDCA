
namespace IDCA.Bll.MDMDocument
{

    public class Type : MDMLabeledObject, IType
    {
        internal Type(IMDMObject parent) : base(parent.Document, parent)
        {
            _categories = new Categories(_parent.Document, _parent);
        }

        readonly ICategories _categories;

        bool _globalNamespace = false;

        public ICategories Categories => _categories;
        public bool GlobalNamespace { get => _globalNamespace; internal set => _globalNamespace = value; }
    }

    public class Types : MDMNamedCollection<Type>, ITypes<Type>
    {
        internal Types(IMDMDocument document) : base(document, document, collection => new Type(collection))
        {
        }

        bool _globalNamespace = false;

        public bool GlobalNamespace { get => _globalNamespace; set => _globalNamespace = value; }

    }
}
