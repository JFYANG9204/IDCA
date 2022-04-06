

using System.Collections.Generic;

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

    public class TemplateConfigKeys
    {
        public const string FileTemplateEncoding = "FileTemplateEncoding";
    }

}
