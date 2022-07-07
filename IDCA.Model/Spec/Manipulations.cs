
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
        }

        readonly TemplateCollection _templates;

        /// <summary>
        /// 获取指定Field的Manipulation对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exact"></param>
        /// <returns></returns>
        public Manipulation? this[string name, bool exact = true]
        {
            get
            {
                return _items.Find(e => e.Field.MatchField(name, exact));
            }
        }

        /// <summary>
        /// 判断当前集合中是否已存在对应Field的配置，不区分大小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist(string name)
        {
            return _items.Exists(e => e.Field.MatchField(name, true));
        }

        /// <summary>
        /// 移除指定名称的Manipulation元素
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name, bool exact = true)
        {
            _items.RemoveAll(e => e.Field.MatchField(name, exact));
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
        /// 从MDM文档列表名载入对象并创建修改标签的函数，如果列表名不存在，将跳过添加修改标签函数的步骤
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Manipulation FromType(string typeName)
        {
            var exist = this[typeName];
            if (exist != null)
            {
                return exist;
            }

            Manipulation manipulation = CreateManipulation();
            manipulation.SetField(typeName);

            var type = Document?.MDMDocument?.Types[typeName];
            if (type != null)
            {
                foreach (Element category in type.Categories)
                {
                    manipulation.AppendDefinitionLabelFunction(category.Name, category.Label);
                }
            }

            return manipulation;
        }

        /// <summary>
        /// 从MDM文档列表对象创建基础的修改标签函数脚本
        /// </summary>
        /// <param name="type"></param>
        public Manipulation FromType(MDM.Type type)
        {
            var exist = this[type.Name];
            if (exist != null)
            {
                return exist;
            }

            Manipulation manipulation = CreateManipulation();
            manipulation.SetField(type.Name);
            foreach (Element category in type.Categories)
            {
                manipulation.AppendDefinitionLabelFunction(category.Name, category.Label);
            }
            return manipulation;
        }

        /// <summary>
        /// 从Field名称载入MDM对象并添加标题配置、轴配置和码号描述配置，如果提供的变量名不存在，将跳过添加码号描述配置的步骤。
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public Manipulation FromField(string fieldName, string title)
        {
            var exist = this[fieldName];
            if (exist != null)
            {
                return exist;
            }

            var manipulation = CreateManipulation();
            manipulation.SetField(fieldName);
            manipulation.AppendTitleTextFunction(title);
            // 检查MDM文档配置
            var mdmField = Document?.MDMDocument?.Fields[fieldName];
            if (mdmField != null)
            {
                if (mdmField.Class == null)
                {
                    manipulation.SetDefaultAxis();
                }
                if (mdmField.Categories != null)
                {
                    foreach (Element element in mdmField.Categories)
                    {
                        manipulation.AppendResponseLabelFunction(element.Name, element.Label);
                    }
                }
            }
            return manipulation;
        }

        /// <summary>
        /// 从MDM Field列表对象创建基础的修改标签函数脚本，只添加本级Field的相关配置
        /// </summary>
        /// <param name="type"></param>
        public Manipulation FromField(Field field, string title)
        {
            var exist = this[field.FullName];
            if (exist != null)
            {
                return exist;
            }

            Manipulation manipulation = CreateManipulation();
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
            _templates = new List<FunctionTemplate>();
        }

        readonly FieldScript _field;
        Axis _axis;

        FunctionTemplate? _titleFunction;
        FunctionTemplate? _axisFunction;
        FunctionTemplate? _responseLabelFunction;
        FunctionTemplate? _definitionLabelFunction;
        FunctionTemplate? _sideAxisFunction;
        FunctionTemplate? _singleCodeFactorFunction;
        FunctionTemplate? _sequentialCodeFacotrFunction;
        FunctionTemplate? _axisInsertaverageMentionFunction;

        readonly List<FunctionTemplate> _templates;

        /// <summary>
        /// 当前Manipulation对象修改的MDMField对象名
        /// </summary>
        public FieldScript Field => _field;

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
            _responseLabelFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideResponseLabel);
            _definitionLabelFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateTypeSideResponseLabel);
            _sideAxisFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideAverage);
            _singleCodeFactorFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSetSingleCodeFactor);
            _sequentialCodeFacotrFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSetSequentialCodeFactor);
            _axisInsertaverageMentionFunction = templates.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateAxisInsertAverage);
        }

        /// <summary>
        /// 判断指定类型的函数模板是否可用
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool IsAvailable(FunctionTemplateFlags flags)
        {
            if (flags == FunctionTemplateFlags.ManipulateTitleLabel)
            {
                return _titleFunction != null;
            }

            if (flags == FunctionTemplateFlags.ManipulateSideAxis)
            {
                return _axisFunction != null;
            }

            if (flags == FunctionTemplateFlags.ManipulateSideResponseLabel)
            {
                return _responseLabelFunction != null;
            }

            if (flags == FunctionTemplateFlags.ManipulateTypeSideResponseLabel)
            {
                return _definitionLabelFunction != null;
            }

            if (flags == FunctionTemplateFlags.ManipulateAxisInsertAverage)
            {
                return _axisInsertaverageMentionFunction != null;
            }

            if (flags == FunctionTemplateFlags.ManipulateSideAverage)
            {
                return _sideAxisFunction != null;
            }

            return false;
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

        /// <summary>
        /// 获取当前集合中第一个符合指定类型的函数模板，如果没有此类型，返回null
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public FunctionTemplate? First(FunctionTemplateFlags flags)
        {
            return _templates.Find(t => t.Flag == flags);
        }

        void Add(FunctionTemplate functionTemplate)
        {
            _templates.Add(functionTemplate);
        }

        /// <summary>
        /// 移除所有符合指定类型的函数模板数据
        /// </summary>
        /// <param name="flags"></param>
        public void RemoveAll(FunctionTemplateFlags flags)
        {
            _templates.RemoveAll(t => t.Flag == flags);
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
            template.SetFunctionParameterValue(_field.Export(), TemplateParameterUsage.ManipulateFieldName);
            template.SetFunctionParameterValue(title, TemplateParameterUsage.ManipulateLabelText);
            Add(template);
        }

        /// <summary>
        /// 向模板集合末尾追加修改轴表达式的函数模板
        /// </summary>
        /// <param name="axis"></param>
        public void AppendAxisFunction(Axis axis)
        {
            if (_axisFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_axisFunction.Clone();
            template.SetFunctionParameterValue(_field.Export(), TemplateParameterUsage.ManipulateFieldName);
            template.SetFunctionParameterValue(axis, TemplateParameterUsage.ManipulateSideAxis);
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
            template.SetFunctionParameterValue(_field.Export(), TemplateParameterUsage.ManipulateFieldName);
            template.SetFunctionParameterValue(code, TemplateParameterUsage.ManipulateCategoryName);
            template.SetFunctionParameterValue(label, TemplateParameterUsage.ManipulateLabelText);
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
            template.SetFunctionParameterValue(code, TemplateParameterUsage.ManipulateCategoryName);
            template.SetFunctionParameterValue(label, TemplateParameterUsage.ManipulateLabelText);
            Add(template);
        }

        /// <summary>
        /// 向模板集合末尾追加配置平均提及计算的函数模板
        /// </summary>
        /// <param name="skipCodes"></param>
        /// <param name="rowLabel"></param>
        /// <param name="decimals"></param>
        public void AppendAverageMentionFunction(string skipCodes = "", string rowLabel = "", int decimals = 2)
        {
            if (_sideAxisFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_sideAxisFunction.Clone();
            template.SetFunctionParameterValue(skipCodes, TemplateParameterUsage.ManipulateExclude);
            template.SetFunctionParameterValue(rowLabel, TemplateParameterUsage.ManipulateLabelText);
            template.SetFunctionParameterValue(decimals.ToString(), TemplateParameterUsage.ManipulateDecimals);
        }

        /// <summary>
        /// 向模板集合末尾追加修改单个码号Factor的函数模板
        /// </summary>
        /// <param name="code"></param>
        /// <param name="factor"></param>
        public void AppendSingleFactorFunction(string code, double factor)
        {
            if (_singleCodeFactorFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_singleCodeFactorFunction.Clone();
            template.SetFunctionParameterValue(_field.Export(), TemplateParameterUsage.ManipulateFieldName);
            template.SetFunctionParameterValue(code, TemplateParameterUsage.ManipulateCategoryName);
            template.SetFunctionParameterValue(factor.ToString(), TemplateValueType.Number, TemplateParameterUsage.ManipulateFactor);
            Add(template);
        }

        /// <summary>
        /// 向模板集合末尾追加添加连续Factor的函数模板
        /// </summary>
        /// <param name="startFactor"></param>
        /// <param name="stepFactor"></param>
        /// <param name="excludeCodes"></param>
        /// <param name="appendFactorLabel"></param>
        public void AppendSequentialFactorFunction(double startFactor = 1, double stepFactor = 1, string excludeCodes = "", bool appendFactorLabel = true)
        {
            if (_sequentialCodeFacotrFunction == null)
            {
                return;
            }
            var template = (FunctionTemplate)_sequentialCodeFacotrFunction.Clone();
            template.SetFunctionParameterValue(_field.Export(), TemplateParameterUsage.ManipulateFieldName);
            template.SetFunctionParameterValue(startFactor.ToString(), TemplateParameterUsage.ManipulateSequentialFactorStart);
            template.SetFunctionParameterValue(stepFactor.ToString(), TemplateParameterUsage.ManipulateSequentialFactorStep);
            if (!string.IsNullOrEmpty(excludeCodes))
            {
                template.SetFunctionParameterValue(excludeCodes, TemplateParameterUsage.ManipulateExclude);
            }
            template.SetFunctionParameterValue(appendFactorLabel ? "True" : "False", TemplateParameterUsage.ManipulateSequentialFactorAppendLabel);
            Add(template);
        }

        /// <summary>
        /// 配置当前函数模板中的轴表达式
        /// </summary>
        /// <param name="axis"></param>
        public void SetAxis(Axis axis)
        {
            _axis = axis;
            if (_axisFunction == null)
            {
                return;
            }
            FunctionTemplate? template = _templates.Find(t => t.Flag == FunctionTemplateFlags.ManipulateSideAxis);
            if (template == null)
            {
                template = (FunctionTemplate)_axisFunction.Clone();
                Add(template);
            }
            template.SetFunctionParameterValue(axis, TemplateValueType.String, TemplateParameterUsage.ManipulateSideAxis);
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
            _axis.AppendText();
            _axis.AppendBase(config?.Get(SpecConfigKeys.AXIS_BASE_LABEL), baseFilter);
            _axis.AppendText();
            _axis.AppendCategoryRange();
            _axis.AppendText();
            _axis.AppendSubTotal(config?.Get(SpecConfigKeys.AXIS_SIGMA_LABEL) ?? "");
            if (addAverage && _axisInsertaverageMentionFunction != null)
            {
                _axisInsertaverageMentionFunction.SetFunctionParameterValue(averageVariable, TemplateValueType.String, TemplateParameterUsage.ManipulateRebaseMeanVariable);
                _axis.AppendInsertFunction(_sideAxisFunction);
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
            var axisFunction = _templates.Find(f => f.Flag == FunctionTemplateFlags.ManipulateSideAxis);
            if (axisFunction != null)
            {
                var parameter = axisFunction.Parameters[TemplateParameterUsage.ManipulateSideAxis];
                if (parameter != null)
                {
                    parameter.SetValue(_axis.ToString());
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
