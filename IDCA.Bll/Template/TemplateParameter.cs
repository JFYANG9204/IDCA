
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IDCA.Bll.Template
{
    public class TemplateParameter : ITemplateParameter
    {
        public TemplateParameter()
        {
            _name = "";
            _value = "";
            _valueType = TemplateParameterValueType.String;
        }

        string _name;
        object _value;
        TemplateParameterValueType _valueType;

        public string Name { get => _name; set => _name = value; }
        public object Value { get => _value; set => _value = value; }
        public TemplateParameterValueType ValueType { get => _valueType; set => _valueType = value; }

        public override string ToString()
        {
            return $"$[{_name}]";
        }

        public string GetValue()
        {
            string result = string.Empty;
            if (_valueType == TemplateParameterValueType.String)
            {
                if (_value is string str)
                {
                    result = $"\"{str}\"";
                }
                else if (_value is string[] strArray)
                {
                    result = $"\"{strArray.Aggregate((left, right) => $"{left}\n{right}")}\"";
                }
            }
            else if (_valueType == TemplateParameterValueType.Number)
            {
                if (_value is string strValue)
                {
                    result = strValue;
                }
                else if (_value is int intValue)
                {
                    result = intValue.ToString();
                }
                else if (_value is double doubleValue)
                {
                    result = doubleValue.ToString();
                }
            }
            else if (_valueType == TemplateParameterValueType.Categorical && _value is string catStr)
            {
                result = $"{{{catStr}}}";
            }
            else if (_valueType == TemplateParameterValueType.Variable && _value is string varStr)
            {
                result = varStr;
            }
            else if (_valueType == TemplateParameterValueType.Expression && _value is ITemplate template)
            {
                result = template.Exec();
            }
            return result;
        }

        public (string, string) GetTupleValue()
        {
            if (_value is (ITemplateParameter item1, ITemplateParameter item2))
            {
                return new(item1.GetValue(), item2.GetValue());
            }
            return new();
        }

    }

    public class TemplateParameters<T> : ITemplateParameters<T> where T : ITemplateParameter, new()
    {
        internal TemplateParameters(ITemplate template)
        {
            _template = template;
        }

        readonly ITemplate _template;
        readonly List<T> _parameters = new();

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

        public ITemplate Template => _template;
        public int Count => _parameters.Count;

        public void Add(T templateParameter)
        {
            _parameters.Add(templateParameter);
        }

        public T NewObject()
        {
            return new();
        }

        public IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public string[] GetParameters(int startIndex)
        {
            if (_parameters.Count == 0 || startIndex >= _parameters.Count)
            {
                return Array.Empty<string>();
            }
            string[] result = new string[_parameters.Count - startIndex];
            int count = 0;
            for (int i = startIndex; i < _parameters.Count; i++)
            {
                result[count++] = _parameters[i].GetValue();
            }
            return result;
        }

    }

}
