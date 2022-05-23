using IDCA.Model.MDM;
using IDCA.Model.Spec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDCA.Model
{
    public class TableSettingCollection
    {
        public TableSettingCollection(MDMDocument document, Config config, Tables specTables)
        {
            _MDM = document;
            _config = config;
            _specTables = specTables;
            _settings = new List<TableSetting>();
        }

        readonly MDMDocument _MDM;
        readonly Config _config;
        readonly List<TableSetting> _settings;
        readonly Tables _specTables;

        /// <summary>
        /// 当前集合在SpecDocument对象中所对应的Tables对象
        /// </summary>
        public Tables SpecTables => _specTables;
        /// <summary>
        /// 当前配置集合中的元素数量
        /// </summary>
        public int Count => _settings.Count;

        /// <summary>
        /// 当前配置的MDM文档
        /// </summary>
        public MDMDocument MDMDocument => _MDM;

        /// <summary>
        /// 创建新的表格配置对象，并添加进当前集合中
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public TableSetting? NewTableSetting(string field = "")
        {
            Field? mdmField = null;
            if (!string.IsNullOrEmpty(field))
            {
                mdmField = _MDM.Fields[field];
                if (mdmField == null)
                {
                    Logger.Warning("MDMFieldIsNotFound", ExceptionMessages.TableFieldIsNotSetted, field);
                    return null;
                }
            }
            var setting = new TableSetting(this, mdmField, _config, SpecTables);
            Add(setting);
            return setting;
        }

        /// <summary>
        /// 向当前集合的末尾追加新的元素
        /// </summary>
        /// <param name="setting"></param>
        public void Add(TableSetting setting)
        {
            _settings.Add(setting);
        }

        /// <summary>
        /// 移除特定索引位置的元素
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _settings.Count)
            {
                return;
            }
            _settings.RemoveAt(index);
        }

        /// <summary>
        /// 移除特定名称的配置对象
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            int index = -1;
            for (int i = 0; i < _settings.Count; i++)
            {
                if (_settings[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }
            if (index > -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// 将原始索引数据和移动到目标索引位置，两个位置的数据将交换位置
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        public void Swap(int sourceIndex, int targetIndex)
        {
            CollectionHelper.Swap(_settings, sourceIndex, targetIndex);
            SpecTables.Swap(sourceIndex, targetIndex);
        }

        /// <summary>
        /// 将指定索引的值向前移动一个位置
        /// </summary>
        /// <param name="itemIndex"></param>
        public void MoveUp(int itemIndex)
        {
            if (itemIndex > _settings.Count || itemIndex <= 0)
            {
                return;
            }
            Swap(itemIndex, itemIndex - 1);
        }

        /// <summary>
        /// 将指定索引的值向后移动一个位置
        /// </summary>
        /// <param name="itemIndex"></param>
        public void MoveDown(int itemIndex)
        {
            if (itemIndex >= _settings.Count - 1)
            {
                return;
            }
            Swap(itemIndex, itemIndex + 1);
        }

    }

    public enum TableType
    {
        Normal,
        Grid,
        GridSlice,
        ResponseSummary,
        MeanSummary
    }

    public class TableSetting
    {
        public TableSetting(TableSettingCollection setting, Field? field, Config config, Tables tables)
        {
            _setting = setting;
            _field = field;
            _config = config;
            _name = $"TS{setting.Count + 1}";
            _table = tables.NewTable(Spec.TableType.Normal);
            _tempAxis = new Axis(_table, AxisType.Normal);
            _tableAxisNetType = TableAxisNetType.StandardNet;
            _net = new List<NetLikeSettingElement>();
        }

        readonly TableSettingCollection _setting;
        Field? _field;
        TableType _type = TableType.Normal;
        readonly Config _config;
        readonly string _name;
        string _tableTitle = string.Empty;
        string _baseLabel = string.Empty;
        string _baseFilter = string.Empty;
        string _tableFilter = string.Empty;
        List<NetLikeSettingElement> _net;
        readonly Table _table;
        TableAxisNetType _tableAxisNetType;
        bool _addSigma = true;
        bool _addNps = false;
        int _npsTopBox = 2;
        int _npsBottomBox = 7;

        // 事件回调
        Action? _tableTitleChanged;
        Action? _tableFilterChanged;
        Action? _baseLabelChanged;
        Action? _baseFilterChanged;

        public event Action? TableTitleChanged
        {
            add { _tableTitleChanged += value; }
            remove { _tableTitleChanged -= value; }
        }

        public event Action? TableFilterChanged
        {
            add { _tableFilterChanged += value; }
            remove { _tableFilterChanged -= value; }
        }

        public event Action? BaseLabelChanged
        {
            add { _baseLabelChanged += value; }
            remove { _baseLabelChanged -= value; }
        }

        public event Action? BaseFilterChanged
        {
            add { _baseFilterChanged += value; }
            remove { _baseFilterChanged -= value; }
        }

        readonly Axis _tempAxis;
        /// <summary>
        /// 用于保存临时修改信息的轴表达式对象
        /// </summary>
        public Axis TemporaryAxis => _tempAxis;
        /// <summary>
        /// 当前配置元素的父级Setting集合对象
        /// </summary>
        public TableSettingCollection Setting => _setting;
        /// <summary>
        /// 当前配置所对应的SpecDocument配置元素
        /// </summary>
        public Table Table => _table;
        /// <summary>
        /// 当前配置元素的对象名称
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// 当前表格配置的表格类型
        /// </summary>
        public TableType Type { get => _type; set => _type = value; }
        /// <summary>
        /// 当前表格配置的表格标题
        /// </summary>
        public string TableTitle
        {
            get
            {
                return _tableTitle;
            }
            set
            {
                _tableTitle = value;
                _tableTitleChanged?.Invoke();
            }
        }
        /// <summary>
        /// 当前表格配置的基数行标签
        /// </summary>
        public string BaseLabel
        {
            get
            {
                return _baseLabel;
            }
            set
            {
                _baseLabel = value;
                _baseLabelChanged?.Invoke();
            }
        }
        /// <summary>
        /// 当前表格轴表达式中base()元素中的参数内容
        /// </summary>
        public string BaseFilter
        {
            get
            {
                return _baseFilter;
            }
            set
            {
                _baseFilter = value;
                _baseFilterChanged?.Invoke();
            }
        }
        /// <summary>
        /// 当前表格在Table.mrs中的额外筛选器条件
        /// </summary>
        public string TableFilter 
        { 
            get
            {
                return _tableFilter;
            }
            set
            {
                _tableFilter = value;
                _tableFilterChanged?.Invoke();
            } 
        }
        /// <summary>
        /// 当前表格配置的MDMField对象
        /// </summary>
        public Field? Field { get => _field; set => _field = value; }
        /// <summary>
        /// 当前表格配置的Net/Combine配置集合
        /// </summary>
        public List<NetLikeSettingElement> Net => _net;
        /// <summary>
        /// 表格表侧轴表达式元素的Net类型，用于配置Net使用net元素还是combine元素以及出示位置
        /// </summary>
        public TableAxisNetType TableAxisNetType { get => _tableAxisNetType; set => _tableAxisNetType = value; }
        /// <summary>
        /// 表格表侧轴表达式是否在末尾添加subtotal()作为小计
        /// </summary>
        public bool AddSigma { get => _addSigma; set => _addSigma = value; }
        /// <summary>
        /// 表格是否出示NPS
        /// </summary>
        public bool AddNps { get => _addNps; set => _addNps = value; }
        /// <summary>
        /// 当前表格计算NPS的TopBox选项数量
        /// </summary>
        public int NpsTopBox { get => _npsTopBox; set => _npsTopBox = value; }
        /// <summary>
        /// 当前表格计算NPS的BottomBox选项数量
        /// </summary>
        public int NpsBottomBox { get => _npsBottomBox; set => _npsBottomBox = value; }
        /// <summary>
        /// 向当前的Net集合中添加新的元素并将其返回
        /// </summary>
        /// <returns></returns>
        public NetLikeSettingElement NewNetElement()
        {
            var element = new NetLikeSettingElement(this, _config);
            _net.Add(element);
            return element;
        }
        /// <summary>
        /// 创建基础的表侧轴表达式
        /// </summary>
        public void CreateBaseAxisExpression()
        {
            _tempAxis.Clear();
            _tempAxis.AppendTextElement();
            _tempAxis.AppendBaseElement(_baseLabel, _baseFilter);
            _tempAxis.AppendTextElement();
            _tempAxis.AppendAllCategory();
            _tempAxis.AppendTextElement();
            _tempAxis.AppendSubTotal(_config.TryGet<string>(SpecConfigKeys.AxisSigmaLabel) ?? "");
        }
        /// <summary>
        /// 从当前MDM文档配置当前配置的Field对象
        /// </summary>
        /// <param name="field">Field名称</param>
        public void LoadField(string field)
        {
            var mdmField = _setting.MDMDocument.Fields[field];
            if (mdmField != null)
            {
                _field = mdmField;
            }
        }

        void ApplyTopBottomBox()
        {
            _net.Where(ele => ele.IsTopBottomBox)
                .ToList()
                .ForEach(ele => _tempAxis.AppendNamedCombine(ele.Name, ele.Label, ele.Codes));
        }

        void ApplyNps()
        {
            if (!_addNps)
            {
                return;
            }

            if (_net.Find(ele => ele.Name.Equals($"t{_npsTopBox}b", StringComparison.OrdinalIgnoreCase)) == null)
            {
                var npsTopBoxNet = new NetLikeSettingElement(this, _config);
                npsTopBoxNet.FromTopBottomBox(_npsTopBox);
                var npsTopBoxElement = _tempAxis.AppendNamedCombine(npsTopBoxNet.Name, npsTopBoxNet.Label, npsTopBoxNet.Codes);
                npsTopBoxElement.Suffix.AppendIsHidden(true);
                npsTopBoxElement.Suffix.AppendIsFixed(true);
            }

            if (_net.Find(ele => ele.Name.Equals($"b{_npsTopBox}b", StringComparison.OrdinalIgnoreCase)) == null)
            {
                var npsBottomBoxNet = new NetLikeSettingElement(this, _config);
                npsBottomBoxNet.FromTopBottomBox(_npsBottomBox);
                var npsBottomBoxElement = _tempAxis.AppendNamedCombine(npsBottomBoxNet.Name, npsBottomBoxNet.Label, npsBottomBoxNet.Codes);
                npsBottomBoxElement.Suffix.AppendIsHidden(true);
                npsBottomBoxElement.Suffix.AppendIsFixed(true);
            }

            _tempAxis.AppendNamedDerived("nps", $"NPS(T{_npsTopBox}B-B{_npsBottomBox}B)", $"t{_npsTopBox}b-b{_npsBottomBox}b");
        }

        void ApplyMaybeNetElements()
        {
            if (_net.Count == 0)
            {
                _tempAxis.AppendAllCategory();
                return;
            }

            int topBottomCount = _net.Count(ele => ele.IsTopBottomBox);

            if (_tableAxisNetType == TableAxisNetType.StandardNet)
            {
                bool insertEmptyLine = _config.TryGet<bool>(SpecConfigKeys.AxisNetInsertEmptyLine);
                for (int i = 0; i < _net.Count; i++)
                {
                    var netElement = _net[i];
                    _tempAxis.AppendNet(netElement.Label, netElement.Codes);
                    if (insertEmptyLine && i < _net.Count - 1)
                    {
                        _tempAxis.AppendTextElement();
                    }
                }
            }
            else
            {
                AxisTopBottomBoxPosition position = 
                    Converter.ConvertToAxisTopBottomBoxPosition(
                        _config.TryGet<int>(SpecConfigKeys.AxisTopBottomBoxPositon));

                if (topBottomCount > 0 && position == AxisTopBottomBoxPosition.BeforeAllCategory)
                {
                    ApplyTopBottomBox();
                    ApplyNps();
                    var subtotal = _tempAxis.AppendSubTotal();
                    subtotal.Suffix.AppendIsHidden(true);
                    _tempAxis.AppendTextElement();
                }
                
                if (_tableAxisNetType == TableAxisNetType.CombineBeforeAllCategory)
                {

                }

                _tempAxis.AppendAllCategory();
                _tempAxis.AppendTextElement();

                if (topBottomCount > 0 && position == AxisTopBottomBoxPosition.BetweenAllCategoryAndSigma)
                {
                    ApplyTopBottomBox();
                    ApplyNps();
                    var beforeSigmaSubtotal = _tempAxis.AppendSubTotal();
                    beforeSigmaSubtotal.Suffix.AppendIsHidden(true);
                    var exactSigamNet = _tempAxis.AppendNet("", "..");
                    exactSigamNet.Suffix.AppendIsHidden(true);
                }

                if (_addSigma)
                {
                    _tempAxis.AppendSubTotal(_config.TryGet<string>(SpecConfigKeys.AxisSigmaLabel) ?? "Sigma");
                }

                if (topBottomCount > 0 && position == AxisTopBottomBoxPosition.AfterSigma)
                {
                    if (_tempAxis[^1].Template.ElementType != AxisElementType.Text)
                    {
                        _tempAxis.AppendTextElement();
                    }
                    ApplyTopBottomBox();
                    ApplyNps();
                }

            }

        }

        /// <summary>
        /// 应用当前保存的轴表达式配置
        /// </summary>
        public void ApplyAxis()
        {
            Axis sideAxis = _table.SideAxis ?? _table.CreateSideAxis(_tempAxis.Type);
            sideAxis.FromAxis(_tempAxis);
        }
        /// <summary>
        /// 将当前配置应用到Table对象中
        /// </summary>
        public void Apply()
        {
            _table.TableTitle = _tableTitle;
            _table.TableBase = _baseLabel;
            ApplyAxis();
        }

    }

    public enum TableAxisNetType
    {
        StandardNet,
        CombineBeforeAllCategory,
        CombineBetweenAllCategoryAndSigma,
        CombineAfterSigma
    }

    public class NetLikeSettingElement
    {
        public NetLikeSettingElement(TableSetting parent, Config config)
        {
            _parent = parent;
            _name = string.Empty;
            _label = string.Empty;
            _codes = new StringBuilder();
            _field = parent.Field;
            _config = config;
        }

        readonly TableSetting _parent;
        /// <summary>
        /// 当前NetLike配置的父级表格配置对象
        /// </summary>
        public TableSetting Parent => _parent;

        bool _isTopBottomBox = false;
        /// <summary>
        /// 当前的Net配置的是否是Top/Bottom Box
        /// </summary>
        public bool IsTopBottomBox => _isTopBottomBox;

        string _name;
        string _label;
        readonly StringBuilder _codes;
        readonly Field? _field;
        readonly Config _config;

        /// <summary>
        /// 当前Net或Combine元素的变量名
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// 当前Net或Combine元素的标签
        /// </summary>
        public string Label => _label;
        /// <summary>
        /// 当前Net或Combine元素所需要的码号
        /// </summary>
        public string Codes => _codes.ToString();
        /// <summary>
        /// 从字符串读取码号
        /// </summary>
        /// <param name="source"></param>
        public void FromString(string label, string source)
        {
            _codes.Clear();

            // 如果field对象的Categories属性为null，由于缺少码号搜索的依据，直接返回
            if (_field?.Categories == null)
            {
                return;
            }

            _name = $"n{_parent.Net.Count + 1}";
            string? netAheadLabel = _config.TryGet<string>(SpecConfigKeys.AxisNetAheadLabel);
            _label = $"{(string.IsNullOrEmpty(netAheadLabel) ? "" : $"{netAheadLabel}.")}{label}";

            string[] codes = source.Split(',');
            for (int i = 0; i < codes.Length; i++)
            {
                string code = codes[i].Trim();
                if (string.IsNullOrEmpty(code))
                {
                    continue;
                }
                string[] range = code.Split('-');
                if (range.Length == 2)
                {

                    // 由于MDM文档的Category顺序和人填写的顺序可能存在差异，此处不允许按照
                    // 完整Category.Name属性值来填写区间，只允许数值区间

                    string lower = StringHelper.NumberAtRight(range[0].Trim());
                    string upper = StringHelper.NumberAtRight(range[1].Trim());

                    if (!int.TryParse(lower, out int lowerValue) || !int.TryParse(upper, out int upperValue))
                    {
                        Logger.Warning("SettingCodeError", ExceptionMessages.SettingNetLikeRangeInvalid, code);
                        continue;
                    }

                    foreach (Element element in _field.Categories)
                    {
                        string numberAtRight = StringHelper.NumberAtRight(element.Name);
                        if (string.IsNullOrEmpty(numberAtRight) || !int.TryParse(numberAtRight, out int categoryValue))
                        {
                            continue;
                        }
                        if (categoryValue >= lowerValue && categoryValue <= upperValue)
                        {
                            _codes.Append($"{(_codes.Length > 0 ? "," : "")}{element.Name}");
                        }
                    }

                }
                else if (range.Length == 1)
                {
                    string? codeName = null;
                    Element? exactElement = _field.Categories[code];
                    if (exactElement != null)
                    {
                        codeName = exactElement.Name;
                    }
                    else
                    {
                        string sourceNumber = StringHelper.NumberAtRight(code);
                        foreach (Element element in _field.Categories)
                        {
                            if (StringHelper.NumberAtRight(element.Name).Equals(sourceNumber))
                            {
                                codeName = element.Name;
                                break;
                            }
                        }
                    }
                    if (codeName == null)
                    {
                        Logger.Warning("SettingCodeError", ExceptionMessages.SettingNetLikeRangeInvalid, code);
                    }
                    else
                    {
                        _codes.Append($"{(_codes.Length > 0 ? "," : "")}{codeName}");
                    }
                }
            }
        }

        /// <summary>
        /// 添加Top/Bottom Box类型的Net元素，此方法默认将类型转变为Combine
        /// </summary>
        /// <param name="box">Top/Bottom码号的数量</param>
        /// <param name="bottom">添加BottomBox，如果是false，添加TopBox</param>
        /// <param name="reverse">码号顺序是否反转，如果是true，TopBox从第一个开始，否则从最后一个开始</param>
        public void FromTopBottomBox(int box, bool bottom = false, bool reverse = false)
        {
            _isTopBottomBox = true;

            _codes.Clear();

            if (_field?.Categories == null || box <= 0 || box >= _field.Categories.Count)
            {
                Logger.Warning("SettingCodeError", ExceptionMessages.SettingNetLikeRangeInvalid, $"{(bottom ? "Bottom" : "Top")} {box} Box");
                return;
            }

            _name = $"{(bottom ? "b" : "t")}{box}b";
            string? netAheadLabel = _config.TryGet<string>(SpecConfigKeys.AxisNetAheadLabel);
            _label = $"{(string.IsNullOrEmpty(netAheadLabel) ? "" : $"{netAheadLabel}.")}{(bottom ? "Bottom" : "Top")} {box} Box";

            if (reverse)
            {
                int count = box;
                while (count >= 0)
                {
                    int index = _field.Categories.Count - 1 - count;
                    _codes.Append($"{(_codes.Length == 0 ? "" : ",")}{_field.Categories[index]!.Name}");
                    count--;
                }
            }
            else
            {
                for (int i = 0; i < box; i++)
                {
                    _codes.Append($"{(_codes.Length == 0 ? "" : ",")}{_field.Categories[i]!.Name}");
                }
            }


        }

    }

}
