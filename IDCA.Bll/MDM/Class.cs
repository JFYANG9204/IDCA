
namespace IDCA.Model.MDM
{
    public class Class : MDMLabeledObject, IClass
    {
        internal Class(IField field) : base(field.Document, field)
        {
            _objectType = MDMObjectType.Class;
        }

        Types? _types;
        Fields? _fields;
        Pages? _pages;

        public Variable? this[string name] => _fields?[name];

        public Types? Types { get => _types; internal set => _types = value; }
        public Fields? Fields { get => _fields; internal set => _fields = value; }
        public Pages? Pages { get => _pages; internal set => _pages = value; }

    }
}
