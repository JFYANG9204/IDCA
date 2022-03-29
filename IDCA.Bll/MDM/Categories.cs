

namespace IDCA.Bll.MDM
{
    public class Categories : MDMNamedCollection<Element>, ICategories
    {
        internal Categories(IMDMDocument document, IMDMObject parent) : base(document, parent, collection => new Element(collection))
        {
            _objectType = MDMObjectType.Categories;
        }
    }

}
