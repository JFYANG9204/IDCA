using System;
using System.Collections.Generic;
using System.Text;

namespace IDCA.Model.Spec
{

    public enum MetadataType
    {
        None,
        Long,
        Double,
        Text,
        Info,
        Categorical,
        NumericLoop,
        CategoricalLoop,
    }

    public enum MetadataRangeType
    {
        None,
        Any,
        Exact,
    }

    public class Metadata : SpecObject
    {
        public Metadata(SpecObject parent, Config config, string name) : base(parent)
        {
            _objectType = SpecObjectType.Metadata;
            _config = config;
            _name = name;
            _fields = new Dictionary<string, Metadata>();
            _properties = new Dictionary<string, MetadataProperty>();
        }

        readonly Config _config;
        MetadataType _type = MetadataType.None;
        /// <summary>
        /// 当前Metadata数据类型
        /// </summary>
        public MetadataType Type { get => _type; set => _type = value; }

        int _indentLevel = 0;
        /// <summary>
        /// 当前对象的脚本缩进级别
        /// </summary>
        public int IndentLevel { get => _indentLevel; set => _indentLevel = value; }

        readonly string _name;
        /// <summary>
        /// 当前元数据的变量名，为了避免命名冲突，名称不可修改
        /// </summary>
        public string Name => _name;

        string? _description = string.Empty;
        /// <summary>
        /// 当前元数据的标签描述
        /// </summary>
        public string? Description { get => _description; set => _description = value; }

        MetadataCategorical[] _categories = Array.Empty<MetadataCategorical>();
        /// <summary>
        /// 创建新的Categorical类型的变量并添加进当前集合
        /// </summary>
        /// <returns></returns>
        public MetadataCategorical NewCategorical(string? name = null)
        {
            Array.Resize(ref _categories, _categories.Length + 1);
            string categoricalName = name ?? $"{_config.TryGet<string>(SpecConfigKeys.MetadataCategoricalLabel) ?? "V"}{_categories.Length}";
            var categorical = new MetadataCategorical(this, categoricalName);
            _categories[^1] = categorical;
            return categorical;
        }

        string? _lowerBoundary;
        string? _upperBoundary;
        MetadataRangeType _rangeType = MetadataRangeType.None;
        /// <summary>
        /// 配置当前对象的区间上下限
        /// </summary>
        /// <param name="lbound"></param>
        public void SetBoundary(MetadataRangeType rangeType, string? lbound = null, string? ubound = null)
        {
            _rangeType = rangeType;
            _lowerBoundary = lbound;
            _upperBoundary = ubound;
        }

        readonly Dictionary<string, Metadata> _fields;
        /// <summary>
        /// 创建新的下级变量，将其添加进当前集合并返回，由于变量名不可重复，
        /// 变量名相同时，会修改对应名称的变量值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Metadata NewField(MetadataType type, string name)
        {
            var field = new Metadata(this, _config, name) { Type = type, IndentLevel = _indentLevel + 1 };
            if (_fields.ContainsKey(name.ToLower()))
            {
                _fields[name.ToLower()] = field;
            }
            else
            {
                _fields.Add(name.ToLower(), field);
            }
            return field;
        }

        readonly Dictionary<string, MetadataProperty> _properties;
        /// <summary>
        /// 创建新的元数据属性配置并添加进集合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isString"></param>
        /// <returns></returns>
        public MetadataProperty NewProperty(string name, string? value = null, bool isString = false)
        {
            var property = new MetadataProperty(this, name, isString);
            if (value is not null)
            {
                property.Value = value;
            }

            if (_properties.ContainsKey(name.ToLower()))
            {
                _properties[name.ToLower()] = property;
            }
            else
            {
                _properties.Add(name.ToLower(), property);
            }
            return property;
        }

        /// <summary>
        /// 将当前配置导出为字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            var builder = new StringBuilder();
            var indent = new string(' ', IndentLevel * 4);
            builder.Append($"{indent}{_name}");
            if (_description is not null)
            {
                builder.Append($" \"{_description}\"");
            }
            // Properties
            builder.AppendLine();
            if (_properties.Count > 0)
            {
                int count = 0;
                builder.AppendLine($"{new string(' ', IndentLevel * 8)}[");
                foreach (var property in _properties.Values)
                {
                    builder.AppendLine($"{new string(' ', IndentLevel * 12)}{property}{(++count == _properties.Count ? "," : "")}");
                }
                builder.AppendLine($"{new string(' ', IndentLevel * 8)}]");
            }
            // type
            switch (_type)
            {
                case MetadataType.Long:
                    builder.Append($"{indent}long");
                    break;
                case MetadataType.Double:
                    builder.Append($"{indent}double");
                    break;
                case MetadataType.Text:
                    builder.Append($"{indent}text");
                    break;
                case MetadataType.Info:
                    builder.Append($"{indent}info");
                    break;
                case MetadataType.Categorical:
                    builder.Append($"{indent}categorical");
                    break;
                case MetadataType.NumericLoop:
                case MetadataType.CategoricalLoop:
                    builder.AppendLine($"{indent}loop");
                    break;
                case MetadataType.None:
                default:
                    break;
            }
            // range
            if (!(string.IsNullOrEmpty(_lowerBoundary) && string.IsNullOrEmpty(_upperBoundary)) &&
                !(_type == MetadataType.CategoricalLoop || _type == MetadataType.Info) &&
                _rangeType != MetadataRangeType.None)
            {
                builder.Append('[');
                switch (_rangeType)
                {
                    case MetadataRangeType.Any:
                        builder.Append($"{_lowerBoundary}..{_upperBoundary}");
                        break;
                    case MetadataRangeType.Exact:
                        builder.Append(_lowerBoundary ?? _upperBoundary);
                        break;
                    default:
                        break;
                }
                builder.Append(']');
            }
            builder.AppendLine();
            // Categorical
            if (_type == MetadataType.CategoricalLoop || _type == MetadataType.Categorical)
            {
                builder.AppendLine($"{indent}{{");
                for (int i = 0; i < _categories.Length; i++)
                {
                    MetadataCategorical category = _categories[i];
                    builder.AppendLine($"{indent}{indent}{category}{(i == _categories.Length - 1 ? "" : ",")}");
                }
                builder.Append($"{indent}}}");
                if (_type == MetadataType.CategoricalLoop)
                {
                    builder.Append(" field -");
                    builder.AppendLine();
                }
            }
            // Sub Fields
            if (_type == MetadataType.NumericLoop || _type == MetadataType.CategoricalLoop)
            {
                builder.AppendLine($"{indent}(");
                foreach (Metadata sub in _fields.Values)
                {
                    builder.AppendLine(sub.Export());
                }
                builder.AppendLine($"{indent}) expand");
            }
            // End
            builder.Append(';');
            builder.AppendLine();
            return builder.ToString();
        }
    }

    public class MetadataProperty : SpecObject
    {
        internal MetadataProperty(SpecObject parent, string name, bool isString) : base(parent)
        {
            _objectType = SpecObjectType.MetadataProperty;
            _name = name;
            _isString = isString;
        }

        readonly string _name;
        readonly bool _isString;
        string _value = string.Empty;

        /// <summary>
        /// 元数据属性的属性名，属性名不可修改
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// 元数据属性的值
        /// </summary>
        public string Value { get => _value; set => _value = value; }

        public override string ToString()
        {
            return $"{_name} = {(_isString ? $"\"{_value}\"" : _value)}";
        }

    }

    public enum MetadataCategoricalSuffixType
    {
        ElementType,
        Exclusive,
        Expression,
        Factor,
        Fix,
        Keycode,
        NoFilter,
    }

    public class MetadataCategorical : SpecObject
    {
        internal MetadataCategorical(SpecObject parent, string name) : base(parent)
        {
            _objectType = SpecObjectType.MetadataCategorical;
            _name = name;
        }

        readonly string _name;
        string? _description;
        /// <summary>
        /// 当前Categorical对象的变量名，为了避免冲突，变量名不可修改
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// 当前Categorical对象的标签描述
        /// </summary>
        public string? Description { get => _description; set => _description = value; }

        string? _listName;
        /// <summary>
        /// 配置当前引用的列表名称
        /// </summary>
        /// <param name="listName"></param>
        public void SetListName(string listName)
        {
            _listName = listName;
        }

        readonly Dictionary<MetadataCategoricalSuffixType, string> _suffix = new();

        /// <summary>
        /// 配置后缀类型和数据值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetSuffix(MetadataCategoricalSuffixType type, string value = "")
        {
            if (_suffix.ContainsKey(type))
            {
                _suffix[type] = value;
            }
            else
            {
                _suffix.Add(type, value);
            }
        }

        public override string ToString()
        {
            // 如果是List，忽略其他配置
            if (!string.IsNullOrEmpty(_listName))
            {
                return $"use {_listName}";
            }
            var builder = new StringBuilder();
            builder.Append($"{_name}{(_description is null ? "" : $" {_description}")}");
            if (_suffix.Count > 0)
            {
                foreach (KeyValuePair<MetadataCategoricalSuffixType, string> suffix in _suffix)
                {
                    switch (suffix.Key)
                    {
                        case MetadataCategoricalSuffixType.ElementType:
                            builder.Append($" elementtype(\"{suffix.Value}\")");
                            break;
                        case MetadataCategoricalSuffixType.Exclusive:
                            builder.Append(" exclusive");
                            break;
                        case MetadataCategoricalSuffixType.Expression:
                            builder.Append($" expression(\"{suffix.Value}\")");
                            break;
                        case MetadataCategoricalSuffixType.Factor:
                            builder.Append($" factor(\"{suffix.Value}\")");
                            break;
                        case MetadataCategoricalSuffixType.Fix:
                            builder.Append(" fix");
                            break;
                        case MetadataCategoricalSuffixType.Keycode:
                            builder.Append($" keycode(\"{suffix.Value}\")");
                            break;
                        case MetadataCategoricalSuffixType.NoFilter:
                            builder.Append(" nofilter");
                            break;
                        default:
                            break;
                    }
                }
            }
            return builder.ToString();
        }

    }

    public class MetadataCollection : SpecObject
    {
        public MetadataCollection(SpecDocument document, Config config) : base(document)
        {
            _objectType = SpecObjectType.Collection;
            _config = config;
        }

        readonly Config _config;
        readonly Dictionary<string, Metadata> _fields = new();

        /// <summary>
        /// 当前集合中的元素数量
        /// </summary>
        public int Count => _fields.Count;

        /// <summary>
        /// 移除特定名称的元数据对象
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            string lowerName = name.ToLower();
            if (_fields.ContainsKey(lowerName))
            {
                _fields.Remove(lowerName);
            }
        }

        /// <summary>
        /// 创建新的Metadata对象，将其加入当前集合并返回，
        /// 由于变量名不可重复，如果name参数已存在，则会创建新的Metadata对象，
        /// 并将原始数据覆盖。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Metadata NewMetadata(string name = "", MetadataType type = MetadataType.None)
        {
            string metadataName = name;
            if (string.IsNullOrEmpty(metadataName))
            {
                metadataName = $"Metadata{Count + 1}";
            }
            var metadata = new Metadata(this, _config, metadataName) { Type = type };
            string lowerName = metadataName.ToLower();
            if (_fields.ContainsKey(lowerName))
            {
                _fields[lowerName] = metadata;
            }
            else
            {
                _fields.Add(lowerName, metadata);
            }
            return metadata;
        }

        /// <summary>
        /// 将当前集合内容导出到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            var builder = new StringBuilder();
            foreach (Metadata metadata in _fields.Values)
            {
                builder.AppendLine();
                builder.AppendLine($"'***************{metadata.Name}***************");
                builder.AppendLine(metadata.Export());
                builder.AppendLine($"'***************      End      ***************");
                builder.AppendLine();
            }
            return builder.ToString();
        }

    }


}
