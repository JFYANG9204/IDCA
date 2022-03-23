
namespace IDCA.Bll.SpecDocument
{
    public class AxisElementTemplateFactory
    {
        /// <summary>
        /// 创建用于生成text()轴表达式元素的模板，此模板无参数需求
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static AxisElementTemplate CreateTextTemlate(SpecObject parent)
        {
            AxisElementTemplate template = new(parent);
            template.ElementType = AxisElementType.Text;
            return template;
        }




    }
}
