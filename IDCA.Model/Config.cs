
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IDCA.Model
{

    public class Config
    {
        public Config()
        {
            _configs = new Dictionary<string, object>();
        }

        readonly Dictionary<string, object> _configs;
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

        /// <summary>
        /// 将当前配置的值更新到配置对象中，需要配置对象包含同名属性
        /// </summary>
        /// <param name="properties"></param>
        public void UpdateToSettings(object properties)
        {
            var type = properties.GetType();
            foreach (var key in _configs.Keys)
            {
                var propertyInfo = type.GetProperty(key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(properties, _configs[key]);
                }
            }
        }

    }

    /// <summary>
    /// 轴表达式Net类型，用于标记Net的组织方式，影响轴表达式的出示顺序
    /// </summary>
    public enum AxisNetType
    {
        /// <summary>
        /// 标准的Net组织方式，用Net类型的轴元素出示各个元素，从上到下顺序排列
        /// </summary>
        StandardNet,
        /// <summary>
        /// 所有的Net以Combine类型的轴元素放在具体Category元素之前
        /// </summary>
        CombineBeforeAllCategory,
        /// <summary>
        /// 所有的Net以Combine类型的轴元素放在具体Category元素和最后的小计之间
        /// </summary>
        CombineBetweenAllCategoryAndSigma,
        /// <summary>
        /// 所有的Net以Combine类型的轴元素放在小计之后
        /// </summary>
        CombineAfterSigma
    }

    public enum AxisTopBottomBoxPosition
    {
        BeforeAllCategory = 0,
        BetweenAllCategoryAndSigma = 1,
        AfterSigma = 2
    }

    public class SpecConfigKeys
    {
        public const string TemplateRootPath = "TemplateRootPath";
        public const string MetadataCategoricalLabel = "MetadataCategoricalLabel";
        // Axis Config
        public const string AxisAddSigma = "AxisAddSigma";
        public const string AxisBaseLabel = "AxisBaseLabel";
        public const string AxisSigmaLabel = "AxisSigmaLabel";
        public const string AxisNetAheadLabel = "AxisNetAheadLabel";
        public const string AxisNetInsertEmptyLine = "AxisNetInsertEmptyLine";
        public const string AxisNpsTopBox = "AxisNpsTopBox";
        public const string AxisNpsBottomBox = "AxisNpsBottomBox";
        /// <summary>
        /// 配置Top/Bottom Box的位置，其int值需要与AxisTopBottomBoxPosition的值相同
        /// </summary>
        public const string AxisTopBottomBoxPositon = "AxisTopBottomBoxPositon";
        public const string AxisCombinePosition = "AxisCombinePosition";
        public const string AxisAverageMentionLabel = "AxisAverageMentionLabel";
        public const string AxisAverageMentionBlankRow = "AxisAverageMentionBlankRow";
        public const string AxisAverageMentionDecimals = "AxisAverageMentionDecimals";
        public const string TableSummaryLabel = "TableSummaryLabel";
        // Table Setting
        public const string TableSettingNetLabelCodeSeparater = "TableSettingNetLabelCodeSeparater";
        public const string TableSettingNetCodeSeparater = "TableSettingNetCodeSeparater";
        public const string TableSettingNetCodeRangeSeparater = "TableSettingNetCodeRangeSeparater";
    }

}
