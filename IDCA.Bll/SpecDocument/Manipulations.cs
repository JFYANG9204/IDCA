

using IDCA.Bll.Template;

namespace IDCA.Bll.SpecDocument
{
    public class Manipulations : SpecObjectCollection<Manipulation>
    {

        public Manipulations(SpecDocument document) : base(document, collection => new Manipulation(collection))
        {
        }


    }


    public enum ManipulationType
    {
        None,
        Title,
        Axis,
        ResponseLabel,
        DefinitionLabel,
    }

    /// <summary>
    /// 此类存储所有MDM文档修改相关函数内容，最后会合并到集合中，并写入到单独的文件里。
    /// </summary>
    public class Manipulation : SpecObject
    {

        public Manipulation(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.Manipulation;
        }


        ManipulationType _type = ManipulationType.None;
        /// <summary>
        /// 当前文档修改函数的类型
        /// </summary>
        public ManipulationType Type { get => _type; set => _type = value; }

        FunctionTemplate? _template;

        /// <summary>
        /// 需要从已知的模板集合中读取所需类型的函数模板
        /// </summary>
        /// <param name="templates">已经读取完成的模板集合</param>
        public void Init(TemplateCollection templates)
        {
            switch (_type)
            {
                case ManipulationType.None:
                    break;
                case ManipulationType.Title:
                    _template = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateTitleLabel);
                    break;
                case ManipulationType.Axis:
                    _template = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideAxis);
                    break;
                case ManipulationType.ResponseLabel:
                    _template = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideLabel);
                    break;
                case ManipulationType.DefinitionLabel:
                    _template = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateTypeLabel);
                    break;
                default:
                    break;
            }
        }

    }


}
