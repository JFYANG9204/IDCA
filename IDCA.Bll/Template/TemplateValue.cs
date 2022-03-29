
using System;

namespace IDCA.Bll.Template
{

    /// <summary>
    /// 模板值类型，会影响文本替换的结果
    /// </summary>
    public enum TemplateValueType
    {
        /// <summary>
        /// 字符串类型，替换后会被双引号包裹
        /// </summary>
        String = 0,
        /// <summary>
        /// 变量名，不做修改
        /// </summary>
        Variable = 1,
        /// <summary>
        /// 分类变量值类型，替换后会被花括号包裹
        /// </summary>
        Categorical = 2,
        /// <summary>
        /// 数值，不做修改
        /// </summary>
        Number = 3,
        /// <summary>
        /// 任意表达式，不做修改
        /// </summary>
        Expression = 4,
    }

    /// <summary>
    /// 用于配置模板的变量值
    /// </summary>
    public class TemplateValue : ICloneable
    {

        public TemplateValue()
        {
        }

        public TemplateValue(string value, TemplateValueType valueType)
        {
            _value = value;
            _valueType = valueType;
        }

        TemplateValueType _valueType = TemplateValueType.String;
        string _value = string.Empty;
        /// <summary>
        /// 值类型
        /// </summary>
        public TemplateValueType ValueType { get => _valueType; set => _valueType = value; }
        /// <summary>
        /// 字符串类型的值
        /// </summary>
        public string Value { get => _value; set => _value = value; }

        public override string ToString()
        {
            return _valueType switch
            {
                TemplateValueType.String => $"\"{_value}\"",
                TemplateValueType.Categorical => $"{{{_value}}}",
                _ => _value
            };
        }

        public object Clone()
        {
            return new TemplateValue(_value, _valueType);
        }
    }
}
