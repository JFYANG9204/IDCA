
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.Template
{
    
    public enum TemplateParameterUsage
    {
        None = 0,

        JobProjectPath = 101,
        JobMetaDataName = 102,
        JobContextType = 103,
        JobLanguageType = 104,
        
        ManipulateMDMDocument = 201,
        ManipulateContextType = 202,
        ManipulateLanguageType = 203,
        ManipulateVariableName = 204,
        ManipulateCodeName = 205,
        ManipulateLabelText = 206,
        ManipulateValue = 207,

        TableTopVariableName = 301,
        TableSideVariableName = 302,
        TableTitleText = 303,
        TableBaseText = 304,
        TableFilterText = 305,
        TableExtraSettings = 306,

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
        /// 尝试向当前值集合末尾添加新的参数对象
        /// </summary>
        /// <param name="value"></param>
        public void TryPushParam(object value, TemplateParameterUsage usage)
        {
            if (_value is null)
            {
                _value = new TemplateParameters(_parameters.Template);
            }

            if (_value is TemplateParameters parameters)
            {
                var param = parameters.NewObject();
                param.Usage = usage;
                param.SetValue(value);
                parameters.Add(param);
            }
        }

        /// <summary>
        /// 获取当前模板参数值的字符串类型值
        /// </summary>
        /// <returns></returns>
        public OutType? GetValue<OutType>()
        {
            return _value is OutType outValue ? outValue : default;
        }

        /// <summary>
        /// 尝试获取特定类型的参数值列表，如果没有符合类型的值，返回空列表
        /// </summary>
        /// <typeparam name="OutType">需要匹配的参数值类型</typeparam>
        /// <returns></returns>
        public OutType[] TryGetArray<OutType>(TemplateParameterUsage? usage = null)
        {
            List<OutType> array = new();
            if (_value is TemplateParameters parameters)
            {
                parameters.All(param =>
                {
                    var value = param.GetValue<OutType>();
                    if (value != null)
                    {
                        array.Add(value);
                    }
                }, usage);
            }
            return array.ToArray();
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
        /// 创建新的模板参数对象
        /// </summary>
        /// <returns></returns>
        public TemplateParameter NewObject()
        {
            return new(this);
        }

        public IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        
        public TemplateParameter? GetValue(TemplateParameterUsage usage)
        {
            if (!_usageCache.ContainsKey(usage))
            {
                return default;
            }
            return _usageCache[usage];
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
