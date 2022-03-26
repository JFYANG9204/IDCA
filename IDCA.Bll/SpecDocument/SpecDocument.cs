
using IDCA.Bll.Template;

namespace IDCA.Bll.SpecDocument
{
    public class SpecDocument : SpecObject
    {
        public SpecDocument() : base()
        {
            _objectType = SpecObjectType.Document;
            _tables = new Tables(this);
            _templates = new TemplateCollection();
        }

        readonly Tables _tables;
        /// <summary>
        /// 当前文档的表格集合
        /// </summary>
        public Tables Tables => _tables;

        readonly TemplateCollection _templates;
        /// <summary>
        /// 当前文档的模板集合
        /// </summary>
        public TemplateCollection Templates => _templates;



    }
}
