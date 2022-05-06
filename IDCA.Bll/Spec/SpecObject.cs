
namespace IDCA.Model.Spec
{
    public abstract class SpecObject
    {
        protected SpecObject()
        {
            _objectType = SpecObjectType.None;
        }

        protected SpecObject(SpecObject parent)
        {
            _parent = parent;
            _document = parent.Document;
            _objectType = SpecObjectType.None;
        }

        protected SpecObject? _parent;
        protected SpecDocument? _document;
        protected SpecObjectType _objectType;

        /// <summary>
        /// Spec描述对象类型
        /// </summary>
        public SpecObjectType SpecObjectType => _objectType;
        /// <summary>
        /// 对象所在的文档对象
        /// </summary>
        public SpecDocument? Document => _document;
        /// <summary>
        /// 此对象的父级对象
        /// </summary>
        public SpecObject? Parent => _parent;
    }


    public enum SpecObjectType
    {
        None,
        Document,
        Collection,
        Table,
        Axis,
        AxisElement,
        AxisElementTemplate,
        AxisParameter,
        Manipulation,
        Script,
        Metadata,
        MetadataProperty,
        MetadataCategorical,
    }

}



