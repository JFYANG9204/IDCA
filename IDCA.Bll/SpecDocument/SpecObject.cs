
namespace IDCA.Bll.SpecDocument
{
    public class SpecObject : ISpecObject
    {
        protected SpecObject(ISpecObject parent)
        {
            _parent = parent;
            _document = parent.Document;
            _objectType = SpecObjectType.None;
        }

        protected ISpecObject _parent;
        protected ISpecDocument _document;
        protected SpecObjectType _objectType;

        public SpecObjectType SpecObjectType => _objectType;
        public ISpecDocument Document => _document;
        public ISpecObject Parent => _parent;
    }
}



