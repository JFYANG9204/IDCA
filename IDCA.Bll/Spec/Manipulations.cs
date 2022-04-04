
using IDCA.Bll.MDM;
using IDCA.Bll.Template;
using System;
using System.Text;

namespace IDCA.Bll.Spec
{
    public class Manipulations : SpecObjectCollection<Manipulation>
    {

        public Manipulations(SpecDocument document) : base(document, collection => new Manipulation((Manipulations)collection))
        {
            _document = document;
            _templates = document.Templates;
        }

        readonly TemplateCollection _templates;
        /// <summary>
        /// 创建空白的Manipulation对象，并添加进当前集合中
        /// </summary>
        /// <returns></returns>
        public Manipulation CreateManipulation()
        {
            Manipulation manipulation = NewObject();
            manipulation.Init(_templates);
            Add(manipulation);
            return manipulation;
        }

        /// <summary>
        /// 从MDM文档列表对象创建基础的修改标签函数脚本
        /// </summary>
        /// <param name="type"></param>
        public Manipulation FromType(MDM.Type type)
        {
            Manipulation manipulation = CreateManipulation();
            foreach (Element category in type.Categories)
            {
                manipulation.AppendDefinitionLabelFunction(category.Name, category.Label);
            }
            return manipulation;
        }

        static void FromSingleField(Manipulation manipulation, Field field, string title)
        {
            manipulation.AppendTitleTextFunction(field.Name, title);
            if (field.Categories != null)
            {
                foreach (Element category in field.Categories)
                {
                    manipulation.AppendResponseLabelFunction(category.Name, category.Label);
                }
            }
        }

        /// <summary>
        /// 从MDM Field列表对象创建基础的修改标签函数脚本
        /// </summary>
        /// <param name="type"></param>
        public Manipulation FromField(Field field, string title)
        {
            Manipulation manipulation = CreateManipulation();
            FromSingleField(manipulation, field, title);

            return manipulation;
        }

        /// <summary>
        /// 导出当前配置到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            StringBuilder builder = new();

            foreach (var manipulation in _items)
            {
                builder.AppendLine();
                builder.Append(manipulation.Export());
                builder.AppendLine();
            }

            return builder.ToString();
        }

    }


    /// <summary>
    /// 此类存储所有MDM文档修改相关函数内容，最后会合并到集合中，并写入到单独的文件里。
    /// </summary>
    public class Manipulation : SpecObject
    {

        internal Manipulation(Manipulations parent) : base(parent)
        {
            _objectType = SpecObjectType.Manipulation;
            _field = new(this);
            _axis = new(this, AxisType.Normal);
        }

        readonly FieldScript _field;
        readonly Axis _axis;

        FunctionTemplate? _titleFunction;
        FunctionTemplate? _axisFunction;
        FunctionTemplate? _responseLabelFunction;
        FunctionTemplate? _definitionLabelFunction;
        FunctionTemplate? _axisAverageFunction;

        FunctionTemplate[] _templates = Array.Empty<FunctionTemplate>();

        /// <summary>
        /// 当前Field的表侧轴表达式，同一个对象只能有一个
        /// </summary>
        public Axis Axis => _axis;

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
            _axisAverageFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateAxisAverage);
        }

        /// <summary>
        /// 遍历所有当前集合中的模板元素，对每个元素执行回调函数
        /// </summary>
        /// <param name="callback"></param>
        public void All(Action<FunctionTemplate> callback)
        {
            foreach (FunctionTemplate template in _templates)
            {
                callback(template);
            }
        }

        void Add(FunctionTemplate functionTemplate)
        {
            Array.Resize(ref _templates, _templates.Length + 1);
            _templates[^1] = functionTemplate;
        }

        /// <summary>
        /// 向模板集合末尾追加修改变量标题的函数模板
        /// </summary>
        /// <param name="title"></param>
        public void AppendTitleTextFunction(string variable, string title)
        {
            if (_titleFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_titleFunction.Clone();
            template.SetFunctionParameterValue(variable, TemplateValueType.String, TemplateParameterUsage.ManipulateFieldName);
            template.SetFunctionParameterValue(title, TemplateValueType.String, TemplateParameterUsage.ManipulateLabelText);
            Add(template);
        }

        /// <summary>
        /// 向模板集合末尾追加修改轴表达式的函数模板
        /// </summary>
        /// <param name="axis"></param>
        public void AppendAxisFunction(string axis)
        {
            if (_axisFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_axisFunction.Clone();
            template.SetFunctionParameterValue(_field.Export(), TemplateValueType.String, TemplateParameterUsage.ManipulateFieldName);
            template.SetFunctionParameterValue(axis, TemplateValueType.String, TemplateParameterUsage.ManipulateSideAxis);
            Add(template);
        }

        /// <summary>
        /// 向模板集合末尾追加修改表侧码号描述的函数模板
        /// </summary>
        public void AppendResponseLabelFunction(string code, string label)
        {
            if (_responseLabelFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_responseLabelFunction.Clone();
            template.SetFunctionParameterValue(code, TemplateValueType.String, TemplateParameterUsage.ManipulateCodeName);
            template.SetFunctionParameterValue(label, TemplateValueType.String, TemplateParameterUsage.ManipulateLabelText);
            Add(template);
        }

        /// <summary>
        /// 向模板集合末尾追加修改列表码号描述的函数模板
        /// </summary>
        public void AppendDefinitionLabelFunction(string code, string label)
        {
            if (_definitionLabelFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_definitionLabelFunction.Clone();
            template.SetFunctionParameterValue(code, TemplateValueType.String, TemplateParameterUsage.ManipulateCodeName);
            template.SetFunctionParameterValue(label, TemplateValueType.String, TemplateParameterUsage.ManipulateLabelText);
            Add(template);
        }

        /// <summary>
        /// 配置当前函数模板中的轴表达式
        /// </summary>
        /// <param name="axis"></param>
        public void SetAxis(Axis axis)
        {
            if (_axisFunction == null)
            {
                return;
            }
            FunctionTemplate? template = Array.Find(_templates, t => t.Flag == FunctionTemplateFlags.ManipulateSideAxis);
            if (template == null)
            {
                template = (FunctionTemplate)_axisFunction.Clone();
                Add(template);
            }
            template.SetFunctionParameterValue(axis.ToString(), TemplateValueType.String, TemplateParameterUsage.ManipulateSideAxis);
        }

        /// <summary>
        /// 未当前对象配置默认的轴表达式
        /// </summary>
        /// <param name="baseLabel"></param>
        /// <param name="baseFilter"></param>
        public void SetDefaultAxis(string baseLabel = "", string baseFilter = "", bool addAverage = false, string averageVariable = "")
        {
            Config? config = Document?.Config;
            Axis axis = new(this, AxisType.Normal);
            axis.AppendTextElement();
            axis.AppendBaseElement(baseLabel, baseFilter);
            axis.AppendTextElement();
            axis.AppendAllCategory();
            axis.AppendTextElement();
            axis.AppendSubTotal(config?.TryGet<string>(SpecConfigKeys.AxisSigmaLabel) ?? "");
            if (addAverage && _axisAverageFunction != null)
            {
                _axisAverageFunction.SetFunctionParameterValue(averageVariable, TemplateValueType.String, TemplateParameterUsage.ManipulateRebaseAverageVariable);
                axis.AppendInsertFunction(_axisAverageFunction);
            }
            SetAxis(axis);
        }

        /// <summary>
        /// 导出当前配置到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            StringBuilder builder = new();
            builder.AppendLine($"'*************** {_field.TopLevel} ***************");
            foreach (var temp in _templates)
            {
                builder.AppendLine(temp.Exec());
            }
            builder.AppendLine($"'***************     End     ***************");
            return builder.ToString();
        }

    }


}
