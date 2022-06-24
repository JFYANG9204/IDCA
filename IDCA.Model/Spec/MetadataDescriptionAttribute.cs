using System;

namespace IDCA.Model.Spec
{
    [AttributeUsage(AttributeTargets.All)]
    public class MetadataDescriptionAttribute : Attribute
    {

        public MetadataDescriptionAttribute(string description) : this(description, typeof(string), Array.Empty<string>())
        { 
        }

        public MetadataDescriptionAttribute(string description, Type expectType, string[] expectValues)
        {
            _description = description;
            _expectValueType = expectType;
            _expectValues = expectValues;
            _noValue = false;
        }

        string _description;
        /// <summary>
        /// 额外的文本描述
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        Type _expectValueType;
        /// <summary>
        /// 期望值类型
        /// </summary>
        public Type ExpectValueType
        {
            get => _expectValueType;
            set => _expectValueType = value;
        }

        string[] _expectValues;
        /// <summary>
        /// 期望的有效值
        /// </summary>
        public string[] ExpectValues
        {
            get => _expectValues;
            set => _expectValues = value;
        }

        bool _noValue;
        /// <summary>
        /// 是否不需要配置值
        /// </summary>
        public bool NoValue
        {
            get => _noValue;
            set => _noValue = value;
        }

    }
}
