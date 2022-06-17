
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

        /// <summary>
        /// 此类参数值为Job.ini文件中的项目根目录（DPGM文件夹）
        /// </summary>
        JobProjectPath = 1011,
        /// <summary>
        /// 此类参数值为Job.ini文件中的元数据文件名（.mdd/.ddf文件文件名）
        /// </summary>
        JobMetaDataName = 1021,
        /// <summary>
        /// 此类参数值为Job.ini文件中的.mdd文件上下文类型
        /// </summary>
        JobContextType = 1031,
        /// <summary>
        /// 此类参数值为Job.ini文件中的.mdd文件语言3字符短码
        /// </summary>
        JobLanguageShortCode = 1041,
        /// <summary>
        /// 此类参数值为Job.ini文件中的.mdd文件语言对应的语言长码
        /// </summary>
        JobLanguageLongCode = 1051,
        /// <summary>
        /// 此类参数值为Run.mrs文件中的项目描述
        /// </summary>
        JobDescription = 1061,
        
        /// <summary>
        /// 此类参数值为Manipulate文件或Tab文件内使用的MDMDocument变量
        /// </summary>
        ManipulateMDMDocument = 2012,
        /// <summary>
        /// 此类参数值为Manipulate文件或Tab文件内使用的上下文类型，可为字符串、宏或变量
        /// </summary>
        ManipulateContextType = 2022,
        /// <summary>
        /// 此类参数值为Manipulate文件或Tab文件内使用的语言类型，可为字符串、宏或变量
        /// </summary>
        ManipulateLanguageType = 2032,
        /// <summary>
        /// 此类参数值为Manipulate文件中使用的变量名，仅指当前级别的变量，不包括上级或下级变量
        /// </summary>
        ManipulateFieldName = 2042,
        /// <summary>
        /// 此类参数值为Manipulate文件中需要修改的Category类型数据的Name属性值
        /// </summary>
        ManipulateCategoryName = 2052,
        /// <summary>
        /// 此类参数值为Manipulate文件中修改的描述值
        /// </summary>
        ManipulateLabelText = 2062,
        /// <summary>
        /// 此类参数值为MDMDocument内Field的轴表达式
        /// </summary>
        ManipulateSideAxis = 2072,
        
        /// <summary>
        /// 此类参数值为Rebase函数使用的Base行描述
        /// </summary>
        RebaseBaseText = 2082,
        /// <summary>
        /// 此类参数值为Rebase函数是否添加均值，需要是true或false
        /// </summary>
        RebaseMean = 2092,
        /// <summary>
        /// 此类参数值为Rebase函数添加的均值计算变量名
        /// </summary>
        RebaseMeanVariable = 2102,

        /// <summary>
        /// 此类参数值为向轴表达式添加均值计算时插入函数的均值计算变量名
        /// </summary>
        ManipulateFunctionMeanVariable = 2112,

        /// <summary>
        /// 此类参数值为Tab文件中使用的TableDocument对象
        /// </summary>
        TableTableDocument = 3012,
        /// <summary>
        /// 此类参数值为Tab文件中添加表格函数的表头变量名
        /// </summary>
        TableTopVariableName = 3022,
        /// <summary>
        /// 此类参数值为Tab文件中添加表格函数的表侧变量名
        /// </summary>
        TableSideVariableName = 3032,
        /// <summary>
        /// 此类参数值为Tab文件中添加表格函数的标题文本
        /// </summary>
        TableTitleText = 3042,
        /// <summary>
        /// 此类参数值为Tab文件中添加表格函数的Base文本描述
        /// </summary>
        TableBaseText = 3052,
        /// <summary>
        /// 此类参数值为Tab文件中添加表格函数的Table Filter
        /// </summary>
        TableFilterText = 3062,
        /// <summary>
        /// 此类参数值为Tab文件中额外添加的描述文本
        /// </summary>
        TableLabelText = 3072,
        /// <summary>
        /// 此类参数值为Tab文件中添加GridSlice Table时需要指定出示的表头码号
        /// </summary>
        TableGridSliceShowCodes = 3082,
        /// <summary>
        /// 此类参数值为Tab文件中添加GridSlice Table时需要添加的额外表侧轴表达式
        /// </summary>
        TableGridSliceAdditionAxis = 3092,
        /// <summary>
        /// 此类参数值为Tab文件中表格名称开头表格标记，一般Grid表格为"TG"，其他为"T"
        /// </summary>
        TableTypeSpecifyWord = 3102,

        ///// <summary>
        ///// 此类参数值为脚本中单个变量声明
        ///// </summary>
        //ScriptDeclaration = 401,
        ///// <summary>
        ///// 此类参数值为脚本中For循环的循环下限
        ///// </summary>
        //ScriptLowerBoundary = 402,
        ///// <summary>
        ///// 此类参数值为脚本中For循环的循环上限
        ///// </summary>
        //ScriptUpperBoundary = 403,
        ///// <summary>
        ///// 此类参数值为脚本中For循环或ForEach循环使用的变量名
        ///// </summary>
        //ScriptForVariable = 404,
        ///// <summary>
        ///// 此类参数值为脚本中ForEach循环中的集合对象
        ///// </summary>
        //ScriptCollection = 405,
        ///// <summary>
        ///// 此类参数值为脚本中If语句中的条件判断部分
        ///// </summary>
        //ScriptTest = 406,
        ///// <summary>
        ///// 此类参数值为脚本中的各种Block语句
        ///// </summary>
        //ScriptBody = 407,
        ///// <summary>
        ///// 
        ///// </summary>
        //ScriptLoopVariables = 408,
        //ScriptTopField = 409,
        //ScriptLevelField = 410,
        //ScriptObjectName = 411,
        //ScriptFunctionTemplate = 412,
        //ScriptBinaryLeft = 413,
        //ScriptBinaryRight = 414,

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
            _value = new TemplateValue();
            _parameters = parent;
            _usage = TemplateParameterUsage.None;
        }

        string _name;
        TemplateValue _value;
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
        public void SetValue(TemplateValue value)
        {
            _value = value;
        }

        /// <summary>
        /// 设置参数的值，并配置值类型为字符串类型
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            _value.Value = value;
            _value.ValueType = TemplateValueType.String;
        }

        /// <summary>
        /// 设置参数的值和值类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        public void SetValue(string value, TemplateValueType valueType)
        {
            _value.ValueType = valueType;
            _value.Value = value;
        }

        /// <summary>
        /// 获取当前存储的参数值
        /// </summary>
        /// <returns></returns>
        public TemplateValue GetValue()
        {
            return _value;
        }

        ///// <summary>
        ///// 获取当前模板参数值的字符串类型值
        ///// </summary>
        ///// <returns></returns>
        //public OutType? GetValue<OutType>()
        //{
        //    return _value is OutType outValue ? outValue : default;
        //}

        public object Clone()
        {
            TemplateParameter clone = new(_parameters);
            clone.SetValue((TemplateValue)_value.Clone());
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
