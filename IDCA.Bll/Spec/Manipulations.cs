

using IDCA.Bll.Template;
using System;

namespace IDCA.Bll.Spec
{
    public class Manipulations : SpecObjectCollection<Manipulation>
    {

        public Manipulations(SpecDocument document) : base(document, collection => new Manipulation(collection))
        {
            _document = document;
            _templates = document.Templates;
        }

        readonly TemplateCollection _templates;
        /// <summary>
        /// 创建空白的Manipulation对象，并添加进当前集合中
        /// </summary>
        /// <returns></returns>
        public Manipulation CreateManipulation(string fieldName)
        {
            Manipulation manipulation = NewObject();
            manipulation.Init(_templates);
            Add(manipulation);
            return manipulation;
        }

    }


    /// <summary>
    /// 此类存储所有MDM文档修改相关函数内容，最后会合并到集合中，并写入到单独的文件里。
    /// </summary>
    public class Manipulation : SpecObject
    {

        internal Manipulation(SpecObject parent) : base(parent)
        {
            _objectType = SpecObjectType.Manipulation;
        }

        FunctionTemplate? _titleFunction;
        FunctionTemplate? _axisFunction;
        FunctionTemplate? _responseLabelFunction;
        FunctionTemplate? _definitionLabelFunction;

        FunctionTemplate[] _templates = Array.Empty<FunctionTemplate>();

        /// <summary>
        /// 需要从已知的模板集合中读取所需类型的函数模板
        /// </summary>
        /// <param name="templates">已经读取完成的模板集合</param>
        public void Init(TemplateCollection templates)
        {
            _titleFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateTitleLabel);
            _axisFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideAxis);
            _responseLabelFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideLabel);
            _definitionLabelFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateTypeLabel);
        }

        void Add(FunctionTemplate functionTemplate)
        {
            Array.Resize(ref _templates, _templates.Length + 1);
            _templates[^1] = functionTemplate;
        }



    }


}
