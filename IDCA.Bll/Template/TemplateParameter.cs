
using System;
using System.Collections;
using System.Collections.Generic;

namespace IDCA.Bll.Template
{
    public class TemplateParameter
    {
        public TemplateParameter()
        {
            _name = "";
            _value = "";
        }

        string _name;
        object _value;

        /// <summary>
        /// 变量名
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// 变量的值
        /// </summary>
        public object Value { get => _value; set => _value = value; }

        /// <summary>
        /// 转换为字符串类型的模板参数替换模板，格式为：$[variable]
        /// </summary>
        /// <returns>转换后的结果</returns>
        public override string ToString()
        {
            return $"$[{_name}]";
        }

        /// <summary>
        /// 获取当前模板参数值的字符串类型值
        /// </summary>
        /// <returns></returns>
        public OutType? GetValue<OutType>()
        {
            Type type = typeof(OutType);
            Type valueType = _value.GetType();

            if (type.Equals(typeof(string)) && _value is Template template)
            {
                return (OutType)(object)template.Exec();
            }

            if (type.Equals(valueType))
            {
                return (OutType?)_value;
            }

            return default;
        }

        /// <summary>
        /// 获取元组格式的模板参数值，如果Value属性的类型不是元组类型，则返回空元组
        /// </summary>
        /// <returns></returns>
        public (OutType1, OutType2) GetTupleValue<OutType1, OutType2>()
        {
            if (_value is (OutType1 item1, OutType2 item2))
            {
                return new(item1, item2);
            }
            return new();
        }

    }

    public class TemplateParameters<T> where T : TemplateParameter, new()
    {
        internal TemplateParameters(Template template)
        {
            _template = template;
        }

        readonly Template _template;
        readonly List<T> _parameters = new();

        /// <summary>
        /// 根据数值索引当前位置的元素信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
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
        /// 模板参数集合所在的模板对象
        /// </summary>
        public Template Template => _template;
        /// <summary>
        /// 当前集合中元素的数量
        /// </summary>
        public int Count => _parameters.Count;

        /// <summary>
        /// 将模板参数对象添加进集合
        /// </summary>
        /// <param name="templateParameter">需要添加的模板参数对象</param>
        public void Add(T templateParameter)
        {
            _parameters.Add(templateParameter);
        }

        /// <summary>
        /// 创建新的模板参数对象
        /// </summary>
        /// <returns></returns>
        public T NewObject()
        {
            return new();
        }

        public IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        /// <summary>
        /// 从特定起始索引开始，获取对应的参数数组
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public OutType?[] GetParameters<OutType>(int startIndex)
        {
            if (_parameters.Count == 0 || startIndex >= _parameters.Count)
            {
                return Array.Empty<OutType?>();
            }
            OutType?[] result = new OutType[_parameters.Count - startIndex];
            int count = 0;
            for (int i = startIndex; i < _parameters.Count; i++)
            {
                result[count++] = _parameters[i].GetValue<OutType>();
            }
            return result;
        }

        /// <summary>
        /// 从特定起始索引开始，获取对应的元组参数数组
        /// </summary>
        /// <typeparam name="OutType1">元组第一个元素类型</typeparam>
        /// <typeparam name="OutType2">元组第二个元组类型</typeparam>
        /// <param name="startIndex">起始索引</param>
        /// <returns>模板参数元组数组</returns>
        public (OutType1, OutType2)[] GetTupleParameters<OutType1, OutType2>(int startIndex)
        {
            if (_parameters.Count == 0 || startIndex >= _parameters.Count)
            {
                return Array.Empty<(OutType1, OutType2)>();
            }
            (OutType1, OutType2)[] result = new (OutType1, OutType2)[_parameters.Count - startIndex];
            int count = 0;
            for (int i = startIndex; i < _parameters.Count; i++)
            {
                result[count++] = _parameters[i].GetTupleValue<OutType1, OutType2>();
            }
            return result;
        }

    }

}
