
using IDCA.Bll.MDMDocument;
using IDCA.Bll.Template;
using System.Collections.Generic;

namespace IDCA.Bll.SpecDocument
{
    /// <summary>
    /// SpecDocument对象创建的表格集合
    /// </summary>
    public class Tables : SpecObjectCollection<Table>
    {
        public Tables(SpecDocument spec) : base(spec, collection => new Table(collection))
        {
        }

        public Table? this[string name] => _nameCache.ContainsKey(name) ? _nameCache[name] : null;

        readonly Dictionary<string, Table> _nameCache = new();

        /// <summary>
        /// 创建新的Table对象，并返回
        /// </summary>
        /// <returns></returns>
        public Table NewTable()
        {
            var table = NewObject();
            table.Name = $"T{Count + 1}";
            Add(table);
            if (!_nameCache.ContainsKey(table.Name))
            {
                _nameCache.Add(table.Name, table);
            }
            return table;
        }

    }

    /// <summary>
    /// 根据SpecDocument载入的信息创建的单个表格类
    /// 所有类型的表格都由此类创建
    /// </summary>
    public class Table : SpecObject
    {
        public Table(SpecObject parent) : base(parent)
        {
            _type = TableType.None;
            _field = new FieldExpressionTemplate();
        }

        string _name = string.Empty;
        /// <summary>
        /// 表格的对象名称
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        readonly FieldExpressionTemplate _field;
        FunctionTemplate? _manipulateTitle;
        FunctionTemplate? _manipulateLabel;
        FunctionTemplate? _manipulateAxisExpression;
        FunctionTemplate? _addTable;
        FunctionTemplate? _addGrid;
        FunctionTemplate? _addGridSlice;
        FunctionTemplate? _addMeanSummary;
        FunctionTemplate? _addReponseSummary;

        Field? _mdmField;

        /// <summary>
        /// 从MDM文档的Field集合中读取符合当前Table变量名称的原始数据定义
        /// </summary>
        /// <param name="fields"></param>
        public void LoadField(Fields? fields)
        {
            if (fields == null)
            {
                Logger.Error(Messages.MDMFieldIsEmpty, $"Table:{_name}");
                return;
            }

            if (_field.Parameters.Count == 0 || string.IsNullOrEmpty(_field.TopField))
            {
                Logger.Error(Messages.TableFieldIsNotSetted, $"Table:{_name}");
                return;
            }

            Field? find = fields[_field.TopField];

            if (find is null)
            {
                Logger.Error(Messages.TableFieldIsNotFound, $"Table:{_name}");
                return;
            }

            _mdmField = find;
        }

        /// <summary>
        /// 从已载入完成的模板集合中载入所需要的模板
        /// </summary>
        /// <param name="collection">已载入完成的模板集合</param>
        public void LoadTemplate(TemplateCollection collection)
        {
            _manipulateTitle = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateTitleLabel);
            _manipulateLabel = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideLabel);
            _manipulateAxisExpression = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.ManipulateSideAxis);
            _addTable = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableNormal);
            _addGrid = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableGrid);
            _addGridSlice = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableGridSlice);
            _addMeanSummary = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableMeanSummary);
            _addReponseSummary = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableResponseSummary);
        }

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

        string _title = "Null";
        /// <summary>
        /// 当前表格的标题
        /// </summary>
        public string Title { get => _title; set => _title = value; }

        string _banner = "banner";
        /// <summary>
        /// 当前表格表头变量名，用于非Grid类型表格
        /// </summary>
        public string Banner { get => _banner; set => _banner = value; }

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
        GridAndSlice,
        MeanSummary,
        ResponseSummary,
    }

}
