using IDCA.Bll.MDM;
using System.Text;

namespace IDCA.Bll
{
    public class Setting
    {
    }

    public class NetLikeElement
    {
        public NetLikeElement(NetLikeType type)
        {
            _type = type;
            _name = string.Empty;
            _label = string.Empty;
            _codes = new();
        }

        readonly NetLikeType _type;
        /// <summary>
        /// 当前元素的类型，是Combine还是Net
        /// </summary>
        public NetLikeType Type => _type;

        string _name;
        string _label;
        readonly StringBuilder _codes;

        /// <summary>
        /// 当前Net或Combine元素的变量名
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// 当前Net或Combine元素的标签
        /// </summary>
        public string Label { get => _label; set => _label = value; }
        /// <summary>
        /// 当前Net或Combine元素所需要的码号
        /// </summary>
        public string Codes => _codes.ToString();
        /// <summary>
        /// 从字符串读取码号
        /// </summary>
        /// <param name="source"></param>
        public void FromString(Field field, string source)
        {
            _codes.Clear();

            // 如果field对象的Categories属性为null，由于缺少码号搜索的依据，直接返回
            if (field.Categories == null)
            {
                return;
            }

            string[] codes = source.Split(',');
            for (int i = 0; i < codes.Length; i++)
            {
                string code = codes[i];
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

                    foreach (Element element in field.Categories)
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
                    Element? exactElement = field.Categories[code];
                    if (exactElement != null)
                    {
                        codeName = exactElement.Name;
                    }
                    else
                    {
                        string sourceNumber = StringHelper.NumberAtRight(code);
                        foreach (Element element in field.Categories)
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

    }

    public enum NetLikeType
    {
        Net,
        Combine,
    }

}
