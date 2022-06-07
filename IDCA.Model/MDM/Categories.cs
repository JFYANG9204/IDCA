

namespace IDCA.Model.MDM
{
    public class Categories : MDMNamedCollection<Element>
    {
        internal Categories(MDMDocument? document, MDMObject? parent) : base(document, parent, collection => new Element(collection))
        {
            _objectType = MDMObjectType.Categories;
        }
    }

}
