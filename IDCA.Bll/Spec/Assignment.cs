
namespace IDCA.Bll.Spec
{

    public class Assignments : SpecObjectCollection<SpecObject>
    {
        public Assignments(SpecDocument document) : base(document, collection => new Assignment(collection))
        {
        }


    }

    public class Assignment : SpecObject
    {

        public Assignment(SpecObject parent) : base(parent)
        {
        }



    }
}
