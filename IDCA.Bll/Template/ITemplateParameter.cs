
using System.Collections;

namespace IDCA.Bll.Template
{
    public interface ITemplateParameter
    {
        /// <summary>
        /// 变量名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 变量数据类型
        /// </summary>
        public TemplateParameterValueType ValueType { get; }
        /// <summary>
        /// 变量的值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 转换为字符串类型的模板参数替换模板，格式为：$[variable]
        /// </summary>
        /// <returns>转换后的结果</returns>
        string ToString();
        /// <summary>
        /// 获取当前模板参数值的字符串类型值
        /// </summary>
        /// <returns></returns>
        string GetValue();
        /// <summary>
        /// 获取元组格式的模板参数值，如果Value属性的类型不是元组类型，则返回空元组
        /// </summary>
        /// <returns></returns>
        (string, string) GetTupleValue();
    }

    public enum TemplateParameterValueType
    {
        String,
        Categorical,
        Variable,
        Number,
        Expression,
        Script,
    }

    public interface ITemplateParameters<T> : IEnumerable
    {
        /// <summary>
        /// 根据数值索引当前位置的元素信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
        /// <summary>
        /// 当前集合中元素的数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 模板参数集合所在的模板对象
        /// </summary>
        ITemplate Template { get; }
        /// <summary>
        /// 创建新的模板参数对象
        /// </summary>
        /// <returns></returns>
        T NewObject();
        /// <summary>
        /// 将模板参数对象添加进集合
        /// </summary>
        /// <param name="templateParameter">需要添加的模板参数对象</param>
        void Add(T templateParameter);
        /// <summary>
        /// 从特定起始索引开始，获取对应的参数数组
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        string[] GetParameters(int startIndex);
    }

}
