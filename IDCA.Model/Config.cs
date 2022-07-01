
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
        public T Get<T>(ConfigInfo<T> info)
        {
            return _configs.ContainsKey(info.Name) && _configs[info.Name] is T value ? value : info.DefaultValue;
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
        /// 配置指定ConfigInfo对应的配置值，如果对应键不存在，将创建新的键值对。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="value"></param>
        public void Set<T>(ConfigInfo<T> info, T value)
        {
            if (value != null)
            {
                Set(info.Name, value);
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
        public void Synchronize(object properties)
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

    /// <summary>
    /// 保存配置的基础信息，包括配置的名称和默认值，用于配置的查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ConfigInfo<T>
    {
        public ConfigInfo(string name, T defaultValue)
        {
            _name = name;
            _defaultValue = defaultValue;
        }

        readonly string _name;
        readonly T _defaultValue;

        public string Name => _name;

        public T DefaultValue => _defaultValue;
    }

    public class SpecConfigDefaultValue
    {
        public const string TEMPLATE_ROOTPATH = "";
        public const string METADATA_CATEGORICAL_LABEL = "_";
        // Axis
        public const bool AXIS_ADD_SIGMA = true;
        public const string AXIS_BASE_LABEL = "Base : ";
        public const string AXIS_SIGMA_LABEL = "Sigma";
        public const string AXIS_NET_AHEAD_LABEL = "Net.";
        public const bool AXIS_NET_INSERT_EMPTYLINE = true;
        public const int AXIS_NPS_TOP_BOX = 2;
        public const int AXIS_NPS_BOTTOM_BOX = 7;
        public const string AXIS_MEAN_LABEL = "AVERAGE";
        public const string AXIS_STDDEV_LABEL = "STANDARD DEVIATION";
        public const string AXIS_STDERR_LABEL = "STANDARD ERROR";
        public const int AXIS_TOP_BOTTOM_BOX_POSITION = 1;
        public const int AXIS_NET_TYPE = 0;
        public const string AXIS_AVERAGE_MENTION_LABEL = "AVERAGE MENTION";
        public const bool AXIS_AVERAGE_MENTION_BLANKLINE = true;
        public const int AXIS_AVERAGE_MENTION_DECIMALS = 2;
        // Table
        public const string TABLE_SUMMARY_LABEL = " - Summary";
        public const string TABLE_SETTING_NET_LABEL_CODE_SEPARATER = ":";
        public const string TABLE_SETTING_NET_CODE_SEPARATER = ",";
        public const string TABLE_SETTING_NET_RANGE_SEPARATER = "-";
    }

    public class SpecConfigKeys
    {
        public static readonly ConfigInfo<string> GLOBAL_TEMPLATE_ROOTPATH = new(nameof(GLOBAL_TEMPLATE_ROOTPATH), SpecConfigDefaultValue.TEMPLATE_ROOTPATH);
        public static readonly ConfigInfo<string> METADATA_CATEGORICAL_LABEL = new(nameof(METADATA_CATEGORICAL_LABEL), SpecConfigDefaultValue.METADATA_CATEGORICAL_LABEL);
        // Axis Config
        public static readonly ConfigInfo<bool> AXIS_ADD_SIGMA = new(nameof(AXIS_ADD_SIGMA), SpecConfigDefaultValue.AXIS_ADD_SIGMA);
        public static readonly ConfigInfo<string> AXIS_BASE_LABEL = new(nameof(AXIS_BASE_LABEL), SpecConfigDefaultValue.AXIS_BASE_LABEL);
        public static readonly ConfigInfo<string> AXIS_SIGMA_LABEL = new(nameof(AXIS_SIGMA_LABEL), SpecConfigDefaultValue.AXIS_SIGMA_LABEL);
        public static readonly ConfigInfo<string> AXIS_NET_AHEAD_LABEL = new(nameof(AXIS_NET_AHEAD_LABEL), SpecConfigDefaultValue.AXIS_NET_AHEAD_LABEL);
        public static readonly ConfigInfo<bool> AXIS_NET_INSERT_EMPTYLINE = new(nameof(AXIS_NET_INSERT_EMPTYLINE), SpecConfigDefaultValue.AXIS_NET_INSERT_EMPTYLINE);
        public static readonly ConfigInfo<int> AXIS_NPS_TOP_BOX = new(nameof(AXIS_NPS_TOP_BOX), SpecConfigDefaultValue.AXIS_NPS_TOP_BOX);
        public static readonly ConfigInfo<int> AXIS_NPS_BOTTOM_BOX = new(nameof(AXIS_NPS_BOTTOM_BOX), SpecConfigDefaultValue.AXIS_NPS_BOTTOM_BOX);
        public static readonly ConfigInfo<string> AXIS_MEAN_LABEL = new(nameof(AXIS_MEAN_LABEL), SpecConfigDefaultValue.AXIS_MEAN_LABEL);
        public static readonly ConfigInfo<string> AXIS_STDDEV_LABEL = new(nameof(AXIS_STDDEV_LABEL), SpecConfigDefaultValue.AXIS_STDDEV_LABEL);
        public static readonly ConfigInfo<string> AXIS_STDERR_LABEL = new(nameof(AXIS_STDERR_LABEL), SpecConfigDefaultValue.AXIS_STDERR_LABEL);
        /// <summary>
        /// 配置Top/Bottom Box的位置，其int值需要与AxisTopBottomBoxPosition的值相同
        /// </summary>
        public static readonly ConfigInfo<int> AXIS_TOP_BOTTOM_BOX_POSITION = new(nameof(AXIS_TOP_BOTTOM_BOX_POSITION), SpecConfigDefaultValue.AXIS_TOP_BOTTOM_BOX_POSITION);
        public static readonly ConfigInfo<int> AXIS_NET_TYPE = new(nameof(AXIS_NET_TYPE), SpecConfigDefaultValue.AXIS_NET_TYPE);
        public static readonly ConfigInfo<string> AXIS_AVERAGE_MENTION_LABEL = new(nameof(AXIS_AVERAGE_MENTION_LABEL), SpecConfigDefaultValue.AXIS_AVERAGE_MENTION_LABEL);
        public static readonly ConfigInfo<bool> AXIS_AVERAGE_MENTION_BLANKLINE = new(nameof(AXIS_AVERAGE_MENTION_BLANKLINE), SpecConfigDefaultValue.AXIS_AVERAGE_MENTION_BLANKLINE);
        public static readonly ConfigInfo<int> AXIS_AVERAGE_MENTION_DECIMALS = new(nameof(AXIS_AVERAGE_MENTION_DECIMALS), SpecConfigDefaultValue.AXIS_AVERAGE_MENTION_DECIMALS);
        // Table Setting
        public static readonly ConfigInfo<string> TABLE_SUMMARY_LABEL = new(nameof(TABLE_SUMMARY_LABEL), SpecConfigDefaultValue.TABLE_SUMMARY_LABEL);
        public static readonly ConfigInfo<string> TABLE_SETTING_NET_LABEL_CODE_SEPARATER = new(nameof(TABLE_SETTING_NET_LABEL_CODE_SEPARATER), SpecConfigDefaultValue.TABLE_SETTING_NET_LABEL_CODE_SEPARATER);
        public static readonly ConfigInfo<string> TABLE_SETTING_NET_CODE_SEPARATER = new(nameof(TABLE_SETTING_NET_CODE_SEPARATER), SpecConfigDefaultValue.TABLE_SETTING_NET_CODE_SEPARATER);
        public static readonly ConfigInfo<string> TABLE_SETTING_NET_RANGE_SEPARATER = new(nameof(TABLE_SETTING_NET_RANGE_SEPARATER), SpecConfigDefaultValue.TABLE_SETTING_NET_RANGE_SEPARATER);
    }

}
