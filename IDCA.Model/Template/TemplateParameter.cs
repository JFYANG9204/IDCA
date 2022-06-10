
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Model.Template
{
    
    // TemplateParameterUsage通过四位十进制值的最后一位来确定值的类别：
    // 1 - 文件参数
    // 2 - 函数参数


    public enum TemplateParameterUsage
    {
        None = 0,

        JobProjectPath = 1011,
        JobMetaDataName = 1021,
        JobContextType = 1031,
        JobLanguageShortCode = 1041,
        JobLanguageLongCode = 1051,
        JobDescription = 1061,
        
        ManipulateMDMDocument = 2012,
        ManipulateContextType = 2022,
        ManipulateLanguageType = 2032,
        ManipulateFieldName = 2042,
        ManipulateCodeName = 2052,
        ManipulateLabelText = 2062,
        ManipulateSideAxis = 2072,
        ManipulateRebaseBaseText = 2082,
        ManipulateRebaseAverage = 2092,
        ManipulateRebaseAverageVariable = 2102,

        TableTableDocument = 3012,
        TableTopVariableName = 3022,
        TableSideVariableName = 3032,
        TableTitleText = 3042,
        TableBaseText = 3052,
        TableFilterText = 3062,
        TableLabelText = 3072,
        TableGridSliceShowCodes = 3082,
        TableGridSliceAdditionAxis = 3092,
        TableTypeSpecifyWord = 3102,

        ScriptDeclaration = 401,
        ScriptLowerBoundary = 402,
        ScriptUpperBoundary = 403,
        ScriptForVariable = 404,
        ScriptCollection = 405,
        ScriptTest = 406,
        ScriptBody = 407,
        ScriptLoopVariables = 408,
        ScriptTopField = 409,
        ScriptLevelField = 410,
        ScriptObjectName = 411,
        ScriptFunctionTemplate = 412,
        ScriptBinaryLeft = 413,
        ScriptBinaryRight = 414,

        FunctionTemplate = 501,
        FunctionName = 502,

        MetadataTemplate = 601,
        MetadataName = 602,
        MetadataLabel = 603,
        MetadataCategorical = 604,
        MetadataSubField = 605,
    }

    public class TemplateParameter : ICloneable
    {
        public TemplateParameter(TemplateParameters parent)
        {
            _name = "";
            _value = new object[1];
            _parameters = parent;
            _usage = TemplateParameterUsage.None;
        }

        string _name;
        object _value;
        readonly TemplateParameters _parameters;
        TemplateParameterUsage _usage;

        /// <summary>
        /// 变量名
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// 所在的参数集合对象
        /// </summary>
        public TemplateParameters TemplateParameters => _parameters;
        /// <summary>
        /// 参数用处
        /// </summary>
        public TemplateParameterUsage Usage { get => _usage; set => _usage = value; }
        /// <summary>
        /// 当前保存的参数值数量
        /// </summary>
        //public int Count => _value.Length;

        /// <summary>
        /// 转换为字符串类型的模板参数替换模板，格式为：$[variable]
        /// </summary>
        /// <returns>转换后的结果</returns>
        public override string ToString()
        {
            return $"$[{_name}]";
        }

        /// <summary>
        /// 设置参数的值，允许保存多个值
        /// </summary>
        /// <param name="value">设置的参数值</param>
        public void SetValue(object value)
        {
            _value = value;
        }

        /// <summary>
        /// 获取当前存储的参数值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// 获取当前模板参数值的字符串类型值
        /// </summary>
        /// <returns></returns>
        public OutType? GetValue<OutType>()
        {
            return _value is OutType outValue ? outValue : default;
        }

        public object Clone()
        {
            TemplateParameter clone = new(_parameters);
            if (_value is ICloneable cloneable)
            {
                clone.SetValue(cloneable.Clone());
            }
            else
            {
                clone.SetValue(_value);
            }
            clone.Usage = _usage;
            clone.Name = _name;
            return clone;
        }
    }

    public class TemplateParameters : ICloneable
    {
        internal TemplateParameters(Template template)
        {
            _template = template;
        }

        readonly Template _template;
        readonly List<TemplateParameter> _parameters = new();
        readonly Dictionary<TemplateParameterUsage, TemplateParameter> _usageCache = new();

        /// <summary>
        /// 根据数值索引当前位置的元素信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TemplateParameter this[int index]
        {
            get
            {
                if (index >= _parameters.Count || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return _parameters[index];
            }
        }

        /// <summary>
        /// 依据参数用途获取指定的模板参数，如果不存在该用途的参数，返回null
        /// </summary>
        /// <param name="usage"></param>
        /// <returns></returns>
        public TemplateParameter? this[TemplateParameterUsage usage] => _usageCache.ContainsKey(usage) ? _usageCache[usage] : null;

        /// <summary>
        /// 模板参数集合所在的模板对象
        /// </summary>
        public Template Template => _template;
        /// <summary>
        /// 当前集合中元素的数量
        /// </summary>
        public int Count => _parameters.Count;

        /// <summary>
        /// 将模板参数对象添加进集合，相同用途的参数值可以存在多个，但是按用途索引时只会所引到最后一个
        /// </summary>
        /// <param name="templateParameter">需要添加的模板参数对象</param>
        public void Add(TemplateParameter templateParameter)
        {
            _parameters.Add(templateParameter);
            if (!_usageCache.ContainsKey(templateParameter.Usage))
            {
                _usageCache.Add(templateParameter.Usage, templateParameter);
            }
            else
            {
                _usageCache[templateParameter.Usage] = templateParameter;
            }
        }

        /// <summary>
        /// 遍历列表中的所有元素，对所有元素执行回调函数
        /// </summary>
        /// <param name="callback"></param>
        public void All(Action<TemplateParameter> callback, TemplateParameterUsage? usage = null)
        {
            foreach (TemplateParameter param in _parameters)
            {
                if (usage == null || param.Usage == usage)
                {
                    callback(param);
                }
            }
        }

        /// <summary>
        /// 遍历列表中的所有元素，对所有元素执行回调函数，回调函数接收索引值
        /// </summary>
        /// <param name="callback"></param>
        public void All(Action<TemplateParameter, int> callback, TemplateParameterUsage? usage = null)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (usage == null || _parameters[i].Usage == usage)
                {
                    callback(_parameters[i], i);
                }
            }
        }

        /// <summary>
        /// 遍历列表中的所有元素，对所有元素执行回调函数，需要提供元素判断函数
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="verify"></param>
        public void All(Action<TemplateParameter> callback, Func<TemplateParameter, bool> verify)
        {
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (verify(_parameters[i]))
                {
                    callback(_parameters[i]);
                }
            }
        }

        /// <summary>
        /// 创建新的模板参数对象
        /// </summary>
        /// <returns></returns>
        public TemplateParameter NewObject()
        {
            return new TemplateParameter(this);
        }

        public IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public object Clone()
        {
            TemplateParameters clone = new(_template);
            foreach (var item in _parameters)
            {
                TemplateParameter clonedItem = (TemplateParameter)item.Clone();
                clone._parameters.Add(clonedItem);
                clone._usageCache.Add(clonedItem.Usage, clonedItem);
            }
            return clone;
        }
    }

}
