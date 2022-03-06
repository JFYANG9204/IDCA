

namespace IDCA.Bll
{

    public class ConfigNames
    {

    }

    public interface IConfig
    {
        /// <summary>
        /// 获取指定类型的特定名称的配置信息，如果键值不存在或类型不匹配，返回null
        /// </summary>
        /// <typeparam name="T">指定配置信息的类型</typeparam>
        /// <param name="key">配置名称</param>
        /// <returns>配置值</returns>
        T TryGet<T>(string key);
    }
}
