
using IDCA.Bll.Template;

namespace IDCA.Bll.SpecDocument
{
    public class Table : SpecObject
    {
        public Table(SpecObject parent) : base(parent)
        {
            _type = TableType.None;
            _field = new FieldExpressionTemplate();
        }

        readonly FieldExpressionTemplate _field;

        /// <summary>
        /// 设置最上级变量名
        /// </summary>
        /// <param name="variable">Top Level的变量名</param>
        public void SetTopLevelField(string variable)
        {
            _field.SetTopField(variable);
        }

        /// <summary>
        /// 向Field级别列表末尾添加新的下级别变量
        /// </summary>
        /// <param name="codeName">'[]'中的内容</param>
        /// <param name="variable">变量名，[..].后的内容</param>
        /// <param name="isCategorical">codeName是否是Categorical类型</param>
        public void PushLevelField(string codeName, string variable, bool isCategorical)
        {
            _field.PushLevel(codeName, variable, isCategorical);
        }

        TableType _type;
        /// <summary>
        /// 当前Table对象的表格类型
        /// </summary>
        public TableType Type { get => _type; set => _type = value; }

        Axis? _headerAxis = null;

        /// <summary>
        /// 创建表头变量的轴表达式
        /// </summary>
        /// <returns></returns>
        public Axis CreateHeaderAxis()
        {
            return _headerAxis = new Axis(this, AxisType.Normal);
        }

        Axis? _sideAxis = null;

        /// <summary>
        /// 创建表侧的轴表达式
        /// </summary>
        /// <param name="type">轴表达式类型</param>
        /// <returns></returns>
        public Axis CreateSideAxis(AxisType type)
        {
            return _sideAxis = new Axis(this, type);
        }




    }

    public enum TableType
    {
        None,
        Normal,
        Grid,
        GridSlice,
    }

}
