using System.Text;

namespace IDCA.Bll
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
            while (source.Length > 0 && source[0] == '0')
            {
                source.Remove(0, 1);
            }
            return startLength > 0 && source.Length == 0 ? "0" : source.ToString();
        }


    }
}
