
using IDCA.Model.MDM;
using IDCA.Model.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDCA.Model.Spec
{
    public class Manipulations : SpecObjectCollection<Manipulation>
    {

        public Manipulations(SpecDocument document) : base(document, collection => new Manipulation((Manipulations)collection))
        {
            _document = document;
            _templates = document.Templates;
            _nameCache = new Dictionary<string, Manipulation>();
        }

        readonly Dictionary<string, Manipulation> _nameCache;
        readonly TemplateCollection _templates;

        /// <summary>
        /// 判断当前集合中是否已存在对应Field的配置，不区分大小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist(string name)
        {
            return _nameCache.ContainsKey(name.ToLower());
        }

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
            string lowerName = type.Name.ToLower();
            if (_nameCache.ContainsKey(lowerName))
            {
                return _nameCache[lowerName];
            }

            Manipulation manipulation = CreateManipulation();
            manipulation.SetField(type.Name);
            _nameCache.Add(lowerName, manipulation);
            foreach (Element category in type.Categories)
            {
                manipulation.AppendDefinitionLabelFunction(category.Name, category.Label);
            }
            return manipulation;
        }

        /// <summary>
        /// 从MDM Field列表对象创建基础的修改标签函数脚本，只添加本级Field的相关配置
        /// </summary>
        /// <param name="type"></param>
        public Manipulation FromField(Field field, string title)
        {
            string lowerName = field.FullName.ToLower();
            if (_nameCache.ContainsKey(lowerName))
            {
                return _nameCache[lowerName];
            }

            Manipulation manipulation = CreateManipulation();
            _nameCache.Add(lowerName, manipulation);
            manipulation.SetField(field.FullName);
            manipulation.AppendTitleTextFunction(title);
            // 如果是最下级变量，添加Axis轴表达式
            if (field.Class == null)
            {
                manipulation.SetDefaultAxis();
            }
            // 添加Category元素的标签修改脚本
            if (field.Categories != null)
            {
                foreach (Element category in field.Categories)
                {
                    manipulation.AppendResponseLabelFunction(category.Name, category.Label);
                }
            }
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
            _field = new FieldScript(this);
            _axis = new Axis(this, AxisType.Normal);
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
        /// 配置当前对象的Field名
        /// </summary>
        /// <param name="field"></param>
        public void SetField(string field)
        {
            _field.FromString(field);
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
        public void AppendTitleTextFunction(string title)
        {
            if (_titleFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_titleFunction.Clone();
            template.SetFunctionParameterValue(_field.Export(), TemplateValueType.String, TemplateParameterUsage.ManipulateFieldName);
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
        public void SetDefaultAxis(string baseFilter = "", bool addAverage = false, string averageVariable = "")
        {
            Config? config = Document?.Config;
            _axis.Clear();
            _axis.AppendTextElement();
            _axis.AppendBaseElement(config?.TryGet<string>(SpecConfigKeys.AxisBaseLabel) ?? "", baseFilter);
            _axis.AppendTextElement();
            _axis.AppendAllCategory();
            _axis.AppendTextElement();
            _axis.AppendSubTotal(config?.TryGet<string>(SpecConfigKeys.AxisSigmaLabel) ?? "");
            if (addAverage && _axisAverageFunction != null)
            {
                _axisAverageFunction.SetFunctionParameterValue(averageVariable, TemplateValueType.String, TemplateParameterUsage.ManipulateRebaseAverageVariable);
                _axis.AppendInsertFunction(_axisAverageFunction);
            }
            SetAxis(_axis);
        }

        /// <summary>
        /// 导出当前配置到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            // 更新Axis表达式
            var axisFunction = Array.Find(_templates, f => f.Flag == FunctionTemplateFlags.ManipulateSideAxis);
            if (axisFunction != null)
            {
                var parameter = axisFunction.Parameters[TemplateParameterUsage.ManipulateSideAxis];
                if (parameter != null)
                {
                    parameter.SetValue(_axis);
                }
            }
            //
            var builder = new StringBuilder();
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
