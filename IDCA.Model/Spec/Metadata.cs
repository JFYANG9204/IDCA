using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IDCA.Model.Spec
{

    public enum MetadataType
    {
        None = -1,
        Long = 0,
        Double = 1,
        Text = 2,
        Info = 3,
        Categorical = 4,
        NumericLoop = 5,
        CategoricalLoop = 6,
    }

    public enum MetadataRangeType
    {
        None,
        Any,
        Exact,
    }

    /// <summary>
    /// Spec配置的元数据类型，此对象支持MDM文档中的常用数据类型，
    /// </summary>
    public class Metadata : SpecObject
    {
        public Metadata(SpecObject parent, Config config, string name) : base(parent)
        {
            _objectType = SpecObjectType.Metadata;
            _config = config;
            _name = name;
            _fields = new List<Metadata>();
            _properties = new List<MetadataProperty>();
            _categories = new List<MetadataCategorical>();
        }

        Func<string, bool>? _beforeRename;
        /// <summary>
        /// 在修改此对象Name属性值之前触发的判定回调函数，
        /// 因为在同一个Metadata集合中，Metadata名称不可重复，
        /// 此回调用于判定传入字符是否可用
        /// </summary>
        public event Func<string, bool> BeforeRename
        {
            add { _beforeRename += value; }
            remove { _beforeRename -= value; }
        }

        readonly Config _config;
        /// <summary>
        /// 当前使用的配置对象
        /// </summary>
        public Config Config => _config;

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

        string _name;
        /// <summary>
        /// 当前元数据的变量名
        /// </summary>
        public string Name 
        { 
            get { return _name; }
            set
            {
                if (_beforeRename == null || _beforeRename.Invoke(value))
                {
                    _name = value;
                }
            }
        }

        string? _description = string.Empty;
        /// <summary>
        /// 当前元数据的标签描述
        /// </summary>
        public string? Description { get => _description; set => _description = value; }

        readonly List<MetadataCategorical> _categories;
        /// <summary>
        /// 当前已存储的MetadataCategorical对象列表。
        /// </summary>
        public List<MetadataCategorical> Categories => _categories;
        /// <summary>
        /// 创建新的Categorical类型的变量并添加进当前集合
        /// </summary>
        /// <returns></returns>
        public MetadataCategorical NewCategorical(string? name = null)
        {
            string codeLabel = _config.Get<string>(SpecConfigKeys.METADATA_CATEGORICAL_LABEL) ?? "_";
            int count = _categories.Count + 1;
            string categoricalName = name ?? $"{codeLabel}{count}";
            while (_categories.Exists(e => e.Name.Equals(categoricalName, StringComparison.OrdinalIgnoreCase)))
            {
                categoricalName = $"{codeLabel}{++count}";
            }
            var categorical = new MetadataCategorical(this, categoricalName) { IndentLevel = _indentLevel + 1};
            _categories.Add(categorical);
            return categorical;
        }

        /// <summary>
        /// 移除指定名称的Category对象，名称不区分大小写
        /// </summary>
        /// <param name="name"></param>
        public void RemoveCategorical(string name)
        {
            _categories.RemoveAll(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取指定名称的Categorical对象，不区分大小写。如果不存在，返回Null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MetadataCategorical? GetCategorical(string name)
        {
            return _categories.Find(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
            if (_type == MetadataType.Categorical || _type == MetadataType.Long)
            {
                if (lbound == null)
                {
                    _lowerBoundary = null;
                }
                else if (int.TryParse(lbound, out _))
                {
                    _lowerBoundary = lbound;
                }

                if (ubound == null)
                {
                    _upperBoundary = null;
                }
                else if (int.TryParse(ubound, out _))
                {
                    _upperBoundary = ubound;
                }
            }
            else if (_type == MetadataType.Text)
            {
                _lowerBoundary = lbound;
                _upperBoundary = ubound;
            }
        }

        readonly List<Metadata> _fields;
        /// <summary>
        /// 创建新的下级变量，将其添加进当前集合并返回，由于变量名不可重复，
        /// 变量名相同时，会修改对应名称的变量值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Metadata NewField(MetadataType type, string name)
        {
            _fields.RemoveAll(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var field = new Metadata(this, _config, name) { Type = type, IndentLevel = _indentLevel + 1 };
            _fields.Add(field);
            return field;
        }
        /// <summary>
        /// 移除指定名称的下级变量，变量名不区分大小写。
        /// 如果变量名不存在，将不执行任何操作。
        /// </summary>
        /// <param name="name"></param>
        public void RemoveField(string name)
        {
            _fields.RemoveAll(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        readonly List<MetadataProperty> _properties;
        /// <summary>
        /// 当前配置的属性集合
        /// </summary>
        public List<MetadataProperty> Properties => _properties;
        /// <summary>
        /// 获取指定名称的属性配置，不区分大小写，如果不存在，将返回null。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MetadataProperty? GetProperty(string name)
        {
            return _properties.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 移除指定名称的属性配置，名称不区分大小写。如果名称不存在，将不做任何操作。
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProperty(string name)
        {
            _properties.RemoveAll(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 创建新的元数据属性配置并添加进集合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isString"></param>
        /// <returns></returns>
        public MetadataProperty NewProperty(string name, string? value = null, bool isString = false)
        {
            var property = _properties.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                property = new MetadataProperty(this, name, isString);
                _properties.Add(property);
            }

            if (value is not null)
            {
                property.Value = value;
            }

            return property;
        }

        Axis? _axis;
        /// <summary>
        /// 当前元数据的轴配置，可以为空，如果不为null，将在转换成字符串时插入到最后的';'之前
        /// </summary>
        public Axis? Axis => _axis;
        /// <summary>
        /// 创建当前元数据的轴表达式对象，如果已存在，会重新创建新的并替换原有数据
        /// </summary>
        /// <returns></returns>
        public Axis CreateAxis()
        {
            var axis = new Axis(this, AxisType.MetadataAxis);
            _axis = axis;
            return axis;
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
            if (_properties.Count > 0)
            {
                int count = 0;
                builder.AppendLine();
                builder.AppendLine($"{new string(' ', IndentLevel * 8)}[");
                foreach (var property in _properties)
                {
                    builder.AppendLine($"{new string(' ', IndentLevel * 12)}{property}{(++count < _properties.Count ? "," : "")}");
                }
                builder.Append($"{new string(' ', IndentLevel * 8)}]");
            }
            // type
            builder.AppendLine();
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
            // Categorical
            if (_type == MetadataType.CategoricalLoop || _type == MetadataType.Categorical)
            {
                builder.AppendLine();
                builder.AppendLine($"{indent}{{");
                for (int i = 0; i < _categories.Count; i++)
                {
                    MetadataCategorical category = _categories[i];
                    builder.AppendLine($"{indent}{indent}{category}{(i == _categories.Count - 1 ? "" : ",")}");
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
                foreach (Metadata sub in _fields)
                {
                    builder.AppendLine(sub.Export());
                }
                builder.Append($"{indent}) expand");
            }
            // End
            if (_axis != null && 
                _type != MetadataType.NumericLoop && 
                _type != MetadataType.CategoricalLoop)
            {
                builder.Append($" {_axis.ToString()}");
            }
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

        string _name;
        bool _isString;
        string _value = string.Empty;

        /// <summary>
        /// 元数据属性的属性名
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// 当前的属性值是否是字符串类型，影响转换成字符串后的值
        /// </summary>
        public bool IsString { get => _isString; set => _isString = value; }
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
        [MetadataDescription("ElementType", ExpectValues = new string[]
        {
            "Category",
            "AnalysisSubheading",
            "AnalysisBase",
            "AnalysisSubtotal",
            "AnalysisSummaryData",
            "AnalysisDerived",
            "AnalysisTotal",
            "AnalysisMean",
            "AnalysisStdDev",
            "AnalysisStdErr",
            "AnalysisSampleVariance",
            "AnalysisMinimun",
            "AnalysisMaximun",
            "AnalysisCategory"
        })]
        ElementType,
        [MetadataDescription("Exclusive", NoValue = true)]
        Exclusive,
        [MetadataDescription("Expression")]
        Expression,
        [MetadataDescription("Factor")]
        Factor,
        [MetadataDescription("Fix", NoValue = true)]
        Fix,
        [MetadataDescription("Keycode")]
        Keycode,
        [MetadataDescription("NoFilter", NoValue = true)]
        NoFilter,
    }

    public class MetadataCategorical : SpecObject
    {
        internal MetadataCategorical(SpecObject parent, string name) : base(parent)
        {
            _objectType = SpecObjectType.MetadataCategorical;
            _name = name;
            _properties = new Dictionary<string, MetadataProperty>();
            _indentLevel = 0;
            _suffix = new Dictionary<MetadataCategoricalSuffixType, string>();
        }

        int _indentLevel;
        /// <summary>
        /// 当前脚本的缩进级别
        /// </summary>
        public int IndentLevel
        {
            get => _indentLevel;
            set => _indentLevel = value;
        }

        string _name;
        string? _description;
        /// <summary>
        /// 当前Categorical对象的变量名
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        /// <summary>
        /// 当前Categorical对象的标签描述
        /// </summary>
        public string? Description { get => _description; set => _description = value; }

        string? _listName;
        /// <summary>
        /// 当前Category变量引用的列表名
        /// </summary>
        public string? ListName => _listName;
        /// <summary>
        /// 配置当前引用的列表名称
        /// </summary>
        /// <param name="listName"></param>
        public void SetListName(string listName)
        {
            _listName = listName;
        }
        /// <summary>
        /// 当前Category对象是否引用列表
        /// </summary>
        public bool UseList => !string.IsNullOrEmpty(_listName);

        readonly Dictionary<string, MetadataProperty> _properties;
        /// <summary>
        /// 当前配置的属性集合
        /// </summary>
        public ICollection<MetadataProperty> Properties => _properties.Values;
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
        /// 获取指定名称的属性配置，不区分大小写，如果不存在，将返回null。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MetadataProperty? GetProperty(string name)
        {
            string lowerName = name.ToLower();
            return _properties.ContainsKey(lowerName) ? _properties[lowerName] : null;
        }
        /// <summary>
        /// 移除指定名称的属性配置，名称不区分大小写。如果名称不存在，将不做任何操作。
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProperty(string name)
        {
            string lowerName = name.ToLower();
            if (_properties.ContainsKey(lowerName))
            {
                _properties.Remove(lowerName);
            }
        }

        readonly Dictionary<MetadataCategoricalSuffixType, string> _suffix;

        /// <summary>
        /// 配置后缀类型和数据值，如果配置值为null，将会移除对应类型的后缀配置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetSuffix(MetadataCategoricalSuffixType type, string? value = "")
        {
            if (value == null)
            {
                if (_suffix.ContainsKey(type))
                {
                    _suffix.Remove(type);
                }
                return;
            }

            if (_suffix.ContainsKey(type))
            {
                _suffix[type] = value;
            }
            else
            {
                _suffix.Add(type, value);
            }
        }

        /// <summary>
        /// 获取指定类型后缀的值，如果未配置，返回null
        /// 需要注意：如果配置的值是空字符串，此后缀是存在的，只有返回null时，后缀类型不存在。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string? GetSuffix(MetadataCategoricalSuffixType type)
        {
            return _suffix.ContainsKey(type) ? _suffix[type] : null;
        }

        public override string ToString()
        {

            //
            // SumN
            //     [
            //         CalculationType="SumN",
            //         Hidden = True,
            //         ExcludedFromSummaries = True,
            //         DataElement = ""
            //     ]
            //     elementType(AnalysisSummaryData)
            //

            // 如果是List，忽略其他配置
            if (!string.IsNullOrEmpty(_listName))
            {
                return $"use {_listName}";
            }
            var builder = new StringBuilder();
            builder.Append($"{_name}{(_description is null ? "" : $" \"{_description}\"")}");

            if (_properties.Count > 0)
            {
                builder.AppendLine();
                builder.Append($"{new string(' ', (_indentLevel + 1) * 4)}[");
                builder.AppendLine();
                int count = 0;
                foreach (var prop in _properties.Values)
                {
                    count++;
                    builder.AppendLine($"{new string(' ', (_indentLevel + 2) * 4)}{prop.ToString()}{(count == _properties.Count - 1 ? "" : ",")}");
                }
                builder.AppendLine($"{new string(' ', (_indentLevel + 1) * 4)}]");
            }

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

    public class MetadataCollection : SpecObject, IEnumerable
    {
        public MetadataCollection(SpecDocument document, Config config) : base(document)
        {
            _objectType = SpecObjectType.Collection;
            _config = config;
            _fields = new List<Metadata>();
        }

        readonly Config _config;
        readonly List<Metadata> _fields;

        /// <summary>
        /// 当前集合中的元素数量
        /// </summary>
        public int Count => _fields.Count;
        /// <summary>
        /// 获取指定名称的表头变量，不区分大小写，如果名称不存在，将返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Metadata? this[string name]
        {
            get
            {
                return _fields.Find(metadata => metadata.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 移除特定名称的元数据对象
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            _fields.RemoveAll(field => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
            if (string.IsNullOrEmpty(metadataName) || 
                _fields.Find(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null)
            {
                metadataName = $"Metadata{Count + 1}";
            }
            var metadata = new Metadata(this, _config, metadataName) { Type = type };
            metadata.BeforeRename += ValidateMetadataName;
            _fields.Add(metadata);
            return metadata;
        }

        bool ValidateMetadataName(string name)
        {
            return _fields.Find(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) == null;
        }

        /// <summary>
        /// 将当前集合内容导出到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            var builder = new StringBuilder();
            foreach (Metadata metadata in _fields)
            {
                builder.AppendLine();
                builder.AppendLine($"'***************{metadata.Name}***************");
                builder.AppendLine(metadata.Export());
                builder.AppendLine($"'***************      End      ***************");
                builder.AppendLine();
            }
            return builder.ToString();
        }

        /// <summary>
        /// 清空当前集合的所有内容
        /// </summary>
        public void Clear()
        {
            _fields.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return _fields.GetEnumerator();
        }
    }


}
