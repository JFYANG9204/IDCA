

using System;
using System.Collections.Generic;
using System.Reflection;

namespace IDCA.Bll
{

    public class Config
    {
        public Config()
        {
        }

        readonly Dictionary<string, object> _configs = new();
        /// <summary>
        /// 获取指定类型的特定名称的配置信息，如果键值不存在或类型不匹配，返回null
        /// </summary>
        /// <typeparam name="T">指定配置信息的类型</typeparam>
        /// <param name="key">配置名称</param>
        /// <returns>配置值</returns>
        public T? TryGet<T>(string key)
        {
            return _configs.ContainsKey(key) && _configs[key] is T value ? value : default;
        }
        /// <summary>
        /// 配置指定索引的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            if (_configs.ContainsKey(key))
            {
                _configs[key] = value;
            }
            else
            {
                _configs.Add(key, value);
            }
        }

        /// <summary>
        /// 清空当前的配置集合
        /// </summary>
        public void Clear()
        {
            _configs.Clear();
        }

        /// <summary>
        /// 从配置对象中的特定属性载入配置内容，键值为属性名，属性值为当前属性的值。
        /// </summary>
        public void TryLoad(object propertyObject, PropertyInfo? propertyInfo)
        {
            object? propertyValue = propertyInfo?.GetValue(propertyObject);
            if (propertyInfo == null || propertyValue == null)
            {
                return;
            }
            Set(propertyInfo.Name, propertyValue);
        }

        /// <summary>
        /// 尝试从对象中读取对应名称属性的值，需要配置键值和属性名相同
        /// </summary>
        public void TryLoad(object propertyObject, string propertyName)
        {
            Type type = propertyObject.GetType();
            TryLoad(propertyObject, type.GetProperty(propertyName));
        }

        /// <summary>
        /// 尝试从配置对象中载入配置，需要提供包含静态字段的值类型对象
        /// </summary>
        /// <param name="propertyObject"></param>
        /// <param name="keyType"></param>
        public void TryLoad(object propertyObject, Type keyType)
        {
            var fields = keyType.GetFields();
            foreach (var field in fields)
            {
                TryLoad(propertyObject, field.Name);
            }
        }
    }

    public class SpecConfigKeys
    {
        public const string MetadataCategoricalLabel = "MetadataCategoricalLabel";
        public const string AxisBaseLabel = "AxisBaseLabel";
        public const string AxisSigmaLabel = "AxisSigmaLabel";
        public const string AxisNetAheadLabel = "AxisNetAheadLabel";
        public const string AxisTopBottomBoxPositon = "AxisTopBottomBoxPositon";
        public const string AxisCombinePosition = "AxisCombinePosition";
    }

}
