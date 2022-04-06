using IDCA.Bll.MDM;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDCA.Bll
{
    public class Setting
    {
        public Setting(MDMDocument document, Config config)
        {
            _MDM = document;
            _config = config;
        }

        readonly MDMDocument _MDM;
        readonly Config _config;
        readonly List<TableSetting> _settings = new();

        /// <summary>
        /// 当前配置集合中的元素数量
        /// </summary>
        public int Count => _settings.Count;

        /// <summary>
        /// 创建新的表格配置对象，并添加进当前集合中
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public TableSetting? NewTableSetting(string field)
        {
            Field? mdmField = _MDM.Fields[field];
            if (mdmField == null)
            {
                Logger.Warning("MDMFieldIsNotFound", ExceptionMessages.TableFieldIsNotSetted, field);
                return null;
            }
            TableSetting setting = new(this, mdmField, _config);
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
        public TableSetting(Setting setting, Field field, Config config)
        {
            _setting = setting;
            _field = field;
            _config = config;
            _name = $"TS{setting.Count + 1}";
        }

        readonly Setting _setting;
        readonly Field _field;
        TableType _type = TableType.Normal;
        readonly Config _config;
        readonly string _name;
        string _tableTitle = string.Empty;
        string _baseLabel = string.Empty;
        NetLikeSettingElement[] _net = Array.Empty<NetLikeSettingElement>();

        /// <summary>
        /// 当前配置元素的父级Setting集合对象
        /// </summary>
        public Setting Setting => _setting;
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
        public string TableTitle { get => _tableTitle; set => _tableTitle = value; }
        /// <summary>
        /// 当前表格配置的基数行标签
        /// </summary>
        public string BaseLabel { get => _baseLabel; set => _baseLabel = value; }
        /// <summary>
        /// 当前表格配置的MDMField对象
        /// </summary>
        public Field Field => _field;
        /// <summary>
        /// 当前表格配置的Net/Combine配置集合
        /// </summary>
        public NetLikeSettingElement[] Net => _net;
        /// <summary>
        /// 向当前的Net集合中添加新的元素并将其返回
        /// </summary>
        /// <returns></returns>
        public NetLikeSettingElement NewNetElement()
        {
            NetLikeSettingElement element = new(this, _config);
            Array.Resize(ref _net, _net.Length + 1);
            _net[^1] = element;
            return element;
        }

    }

    public enum NetLikeType
    {
        StandardNet,
        CombineAhead,
        CombineAfterAll,
        CombineAfterSum,
    }

    public class NetLikeSettingElement
    {
        public NetLikeSettingElement(TableSetting parent, Config config)
        {
            _parent = parent;
            _type = NetLikeType.StandardNet;
            _name = string.Empty;
            _label = string.Empty;
            _codes = new();
            _field = parent.Field;
            _config = config;
        }

        readonly TableSetting _parent;
        /// <summary>
        /// 当前NetLike配置的父级表格配置对象
        /// </summary>
        public TableSetting Parent => _parent;

        NetLikeType _type;
        /// <summary>
        /// 当前元素的类型，是Combine还是Net
        /// </summary>
        public NetLikeType Type => _type;

        string _name;
        string _label;
        readonly StringBuilder _codes;
        readonly Field _field;
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
        public void FromString(string label, string source, bool isCombine = false)
        {
            _codes.Clear();

            // 如果field对象的Categories属性为null，由于缺少码号搜索的依据，直接返回
            if (_field.Categories == null)
            {
                return;
            }

            _name = $"n{_parent.Net.Length + 1}";
            string? netAheadLabel = _config.TryGet<string>(SpecConfigKeys.AxisNetAheadLabel);
            _label = $"{(string.IsNullOrEmpty(netAheadLabel) ? "" : $"{netAheadLabel}.")}{label}";

            if (isCombine)
            {
                _type = _config.TryGet<NetLikeType>(SpecConfigKeys.AxisCombinePosition);
            }

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
            _codes.Clear();

            if (_field.Categories == null || box <= 0 || box >= _field.Categories.Count)
            {
                Logger.Warning("SettingCodeError", ExceptionMessages.SettingNetLikeRangeInvalid, $"{(bottom ? "Bottom" : "Top")} {box} Box");
                return;
            }

            _name = $"{(bottom ? "b" : "t")}{box}b";
            string? netAheadLabel = _config.TryGet<string>(SpecConfigKeys.AxisNetAheadLabel);
            _label = $"{(string.IsNullOrEmpty(netAheadLabel) ? "" : $"{netAheadLabel}.")}{(bottom ? "Bottom" : "Top")} {box} Box";
            _type = _config.TryGet<NetLikeType>(SpecConfigKeys.AxisTopBottomBoxPositon);

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
