using System;
using System.Text;
using System.Text.RegularExpressions;

namespace IDCA.Model
{
    internal class StringHelper
    {
        /// <summary>
        /// 获取字符串右侧的数字部分，如果最后一个字符不是数字，返回空字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string NumberAtRight(string source)
        {
            StringBuilder builder = new();
            int i = source.Length - 1;
            while (i >= 0)
            {
                char c = source[i];
                if (c >= '0' && c <= '9')
                {
                    builder.Insert(0, c);
                }
                else
                {
                    break;
                }
                i--;
            }
            return RemoveAheadZero(builder);
        }

        /// <summary>
        /// 移除字符串开头的0，如果所有字符都是0，返回"0"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string RemoveAheadZero(StringBuilder source)
        {
            int startLength = source.Length;
            while (source.Length > 1 && source[0] == '0')
            {
                source.Remove(0, 1);
            }
            return startLength > 0 && source.Length == 0 ? "0" : source.ToString();
        }

        /// <summary>
        /// 从字符串读取所有Field名称，返回Field名组成的数组
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal static string[] ReadFieldNames(string? fieldName)
        {
            string[] fields = Array.Empty<string>();
            if (string.IsNullOrEmpty(fieldName))
            {
                return fields;
            }
            StringBuilder builder = new();
            int i = 0;
            while (i < fieldName.Length)
            {
                char ch = fieldName[i];

                if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_' || ch == '@' || ch == '#' || (ch >= '0' && ch <= '9'))
                {
                    builder.Append(ch);
                }

                if (ch == '[')
                {
                    while (i <= fieldName.Length - 1 && ch != ']')
                    {
                        ch = fieldName[++i];
                    }
                    // 跳过']'
                    ch = fieldName[++i];
                }

                if (ch == '.' || i >= fieldName.Length - 1)
                {
                    Array.Resize(ref fields, fields.Length + 1);
                    fields[^1] = builder.ToString();
                    builder.Clear();
                }

                i++;
            }
            return fields;
        }
        /// <summary>
        /// 将字符串转换为16进制数字字符串值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string ConvertToHexString(string source)
        {
            StringBuilder result = new();
            byte[] buffer = Encoding.UTF8.GetBytes(source);
            for (int i = 0; i < buffer.Length; i++)
            {
                result.Append(Convert.ToString(buffer[i], 16));
            }
            return result.ToString();
        }
        /// <summary>
        /// 判断文本是否是整数
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static bool IsInteger(string source)
        {
            return Regex.IsMatch(source, @"[0-9]+");
        }
        /// <summary>
        /// 判断文本是否是数值，整数或小数
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static bool IsDigit(string source)
        {
            return Regex.IsMatch(source, @"[0-9.]+");
        }

        /// <summary>
        /// 判断文本是否是有效的MDM脚本名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool ValidateElementName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            return Regex.IsMatch(name, @"[a-zA-Z_$#@\u4e00-\u9fa5]{1}[a-zA-Z_0-9$#@\u4e00-\u9fa5]*");
        }

    }
}
