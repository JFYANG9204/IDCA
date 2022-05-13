
namespace IDCA.Model.MDM
{

    public class Type : MDMLabeledObject
    {
        internal Type(MDMObject? parent) : base(parent?.Document, parent)
        {
            _categories = new Categories(_parent?.Document, _parent);
            _objectType = MDMObjectType.Type;
        }

        readonly Categories _categories;
        bool _globalNamespace = false;

        public Categories Categories => _categories;
        public bool GlobalNamespace { get => _globalNamespace; internal set => _globalNamespace = value; }
    }

    public class Types : MDMNamedCollection<Type>
    {
        internal Types(MDMDocument? document, MDMObject? parent = null) : base(document, parent ?? document, collection => new Type(collection))
        {
            _objectType = MDMObjectType.Types;
        }

        bool _globalNamespace = false;

        public bool GlobalNamespace { get => _globalNamespace; set => _globalNamespace = value; }
    }
}
