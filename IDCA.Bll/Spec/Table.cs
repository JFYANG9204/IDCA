
using IDCA.Model.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDCA.Model.Spec
{
    /// <summary>
    /// SpecDocument对象创建的表格集合
    /// </summary>
    public class Tables : SpecObjectCollection<Table>
    {
        public Tables(SpecDocument spec) : base(spec, collection => new Table(collection))
        {
            _document = spec;
            _templates = spec.Templates;
            _topBreaks = new List<string>();
        }

        Func<Tables, string, bool>? _rename;
        public event Func<Tables, string, bool>? Rename
        {
            add { _rename += value; }
            remove { _rename -= value; }
        }

        private bool OnRename(string originName)
        {
            return _rename == null || _rename.Invoke(this, originName);
        }

        readonly TemplateCollection _templates;

        readonly List<string> _topBreaks;
        /// <summary>
        /// 当前Table集合需要对应的表头列表
        /// </summary>
        public List<string> TopBreaks => _topBreaks;

        string _name = string.Empty;
        /// <summary>
        /// 当前表格配置集合的名称，应该对应mrs文件名
        /// </summary>
        public string Name 
        {
            get => _name;
            set 
            {
                if (!_name.Equals(value, StringComparison.OrdinalIgnoreCase) && OnRename(value))
                {
                    _name = value;
                }
            }
        }

        /// <summary>
        /// 创建新的Table对象，并返回
        /// </summary>
        /// <returns></returns>
        public Table NewTable(TableType type = TableType.None)
        {
            var table = NewObject();
            table.Name = $"T{Count + 1}";
            table.Type = type;
            table.IndexAt = Count;
            table.LoadTemplate(_templates);
            Add(table);
            return table;
        }

        /// <summary>
        /// 将索引位置的Table对象在集合中向前移动一个位置
        /// </summary>
        /// <param name="index"></param>
        public void MoveUp(int index)
        {
            if (index <= 0 || index >= Count)
            {
                return;
            }
            var table = _items[index];
            table.IndexAt--;
            Swap(index, index - 1);
        }

        /// <summary>
        /// 将索引位置的Table对象在集合中向后移动一个位置
        /// </summary>
        /// <param name="index"></param>
        public void MoveDown(int index)
        {
            if (index < 0 || index >= Count - 1)
            {
                return;
            }
            var table = _items[index];
            table.IndexAt--;
            Swap(index, index + 1);
        }

        /// <summary>
        /// 导出当前集合中所有的Table配置到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            StringBuilder builder = new();

            foreach (var item in _items)
            {
                builder.AppendLine(item.Export());
            }

            return builder.ToString();
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
            _objectType = SpecObjectType.Table;
            _type = TableType.None;
            _field = new FieldScript(this);
        }

        int _indexAt = -1;
        /// <summary>
        /// 当前对象在集合中的索引，需要创建时初始化
        /// </summary>
        internal int IndexAt { get => _indexAt; set => _indexAt = value; }

        string _name = string.Empty;
        /// <summary>
        /// 表格的对象名称
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        string _tableTitle = string.Empty;
        /// <summary>
        /// 表格的名称标签
        /// </summary>
        public string TableTitle { get => _tableTitle; set => _tableTitle = value; }

        string _tableBase = string.Empty;
        /// <summary>
        /// 表格的Base标签
        /// </summary>
        public string TableBase { get => _tableBase; set => _tableBase = value; }

        readonly FieldScript _field;
        FunctionTemplate? _tableFunctionTemplate;
        FunctionTemplate? _tableFilterTemplate;
        FunctionTemplate? _tableLabelTemplate;

        /// <summary>
        /// 从已载入完成的模板集合中载入所需要的模板
        /// </summary>
        /// <param name="collection">已载入完成的模板集合</param>
        public void LoadTemplate(TemplateCollection collection)
        {
            switch (_type)
            {
                case TableType.Normal:
                    _tableFunctionTemplate = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableNormal);
                    break;
                case TableType.Grid:
                    _tableFunctionTemplate = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableGrid);
                    break;
                case TableType.GridSlice:
                    _tableFunctionTemplate = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableGridSlice);
                    break;
                case TableType.MeanSummary:
                    _tableFunctionTemplate = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableMeanSummary);
                    break;
                case TableType.ResponseSummary:
                    _tableFunctionTemplate = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableResponseSummary);
                    break;
                case TableType.None:
                default:
                    break;
            }
            _tableFilterTemplate = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableFilter);
            _tableLabelTemplate = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableLabel);
        }

        /// <summary>
        /// 向Field级别列表末尾添加新的下级别变量
        /// </summary>
        /// <param name="codeName">'[]'中的内容</param>
        /// <param name="variable">变量名，[..].后的内容</param>
        /// <param name="isCategorical">codeName是否是Categorical类型</param>
        public void PushLevelField(string variable, string codeName = "", bool isCategorical = false)
        {
            _field.PushLevelField(variable, codeName, isCategorical);
        }

        TableType _type;
        /// <summary>
        /// 当前Table对象的表格类型
        /// </summary>
        public TableType Type { get => _type; set => _type = value; }

        string _titleInTableFile = "Null";
        /// <summary>
        /// 当前表格在Table.mrs文件中的标题
        /// </summary>
        public string TitleInTableFile { get => _titleInTableFile; set => _titleInTableFile = value; }

        string _baseInTableFile = string.Empty;
        /// <summary>
        /// 当前表格在Table.mrs文件中的Base标签
        /// </summary>
        public string BaseInTableFile { get => _baseInTableFile; set => _baseInTableFile = value; }

        string _filterInTableFile = string.Empty;
        /// <summary>
        /// 当前表格在Table.mrs文件中的筛选器条件
        /// </summary>
        public string FilterInTableFile { get => _filterInTableFile; set => _filterInTableFile = value; }

        string _labelInTableFile = string.Empty;
        /// <summary>
        /// 当前表格在Table.mrs文件中的额外追加标签
        /// </summary>
        public string LabelInTableFile { get => _labelInTableFile; set => _labelInTableFile = value; }

        readonly TemplateValue _banner = new();
        /// <summary>
        /// 设置当前Table的表头定义数据和数据类型
        /// </summary>
        /// <param name="banner"></param>
        /// <param name="valueType"></param>
        public void SetBanner(string banner, TemplateValueType valueType)
        {
            _banner.Value = banner;
            _banner.ValueType = valueType;
        }

        Axis? _headerAxis = null;
        /// <summary>
        /// 当前表格的表头轴表达式配置
        /// </summary>
        public Axis? HeaderAxis => _headerAxis;
        /// <summary>
        /// 创建表头变量的轴表达式
        /// </summary>
        /// <returns></returns>
        public Axis CreateHeaderAxis()
        {
            return _headerAxis = new Axis(this, AxisType.Normal);
        }
        /// <summary>
        /// 配置当前的表头轴表达式配置，只能配置父级对象为此对象的轴对象
        /// </summary>
        /// <param name="axis"></param>
        public void SetHeaderAxis(Axis axis)
        {
            if (axis.Parent == this)
            {
                _headerAxis = axis;
            }
        }

        Axis? _sideAxis = null;
        /// <summary>
        /// 当前表格的表侧轴表达式配置
        /// </summary>
        public Axis? SideAxis => _sideAxis;
        /// <summary>
        /// 创建表侧的轴表达式
        /// </summary>
        /// <param name="type">轴表达式类型</param>
        /// <returns></returns>
        public Axis CreateSideAxis(AxisType type)
        {
            return _sideAxis = new Axis(this, type);
        }
        /// <summary>
        /// 配置当前表侧轴表达式配置，只能配置父级对象为此对象的轴对象
        /// </summary>
        /// <param name="axis"></param>
        public void SetSideAxis(Axis axis)
        {
            if (axis.Parent == this)
            {
                _sideAxis = axis;
            }
        }
        /// <summary>
        /// 导入当前设置到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            var result = new StringBuilder();
            if (_tableFunctionTemplate != null)
            {
                string side, banner;
                if (_sideAxis != null)
                {
                    side = _sideAxis.Type == AxisType.AxisVariable ? _sideAxis.ToString() : $"{_field.Export()}{_sideAxis}";
                }
                else
                {
                    side = _field.Export();
                }

                TemplateValueType bannerType = _type == TableType.Grid ? TemplateValueType.String : _banner.ValueType;
                if (_headerAxis != null && _headerAxis.Type == AxisType.Normal)
                {
                    banner = _type == TableType.Grid ? $"{_field.TopLevel}{_headerAxis}" : $"{_banner}{_headerAxis}";
                }
                else
                {
                    banner = _type == TableType.Grid ? _field.TopLevel : ((_headerAxis != null && _headerAxis.Type == AxisType.AxisVariable) ? _headerAxis.ToString() : _banner.ToString());
                }
                // table function side parameter
                _tableFunctionTemplate.SetFunctionParameterValue(
                    side, 
                    TemplateValueType.String, 
                    TemplateParameterUsage.TableSideVariableName);
                // table function top parameter
                _tableFunctionTemplate.SetFunctionParameterValue(
                    banner,
                    bannerType, 
                    TemplateParameterUsage.TableTopVariableName);
                // table function title parameter
                _tableFunctionTemplate.SetFunctionParameterValue(
                    _tableTitle, 
                    TemplateValueType.String, 
                    TemplateParameterUsage.TableTitleText);
                // table function base text parameter
                _tableFunctionTemplate.SetFunctionParameterValue(
                    _tableBase, 
                    TemplateValueType.String, 
                    TemplateParameterUsage.TableBaseText);
                result.AppendLine(_tableFunctionTemplate.Exec());
                // table label
                if (!string.IsNullOrEmpty(_labelInTableFile) && 
                    _tableLabelTemplate != null)
                {
                    // 表格类型参数，用以区分Grid和非Grid表格
                    _tableLabelTemplate.SetFunctionParameterValue(
                        _type == TableType.Grid ? "TG" : "T",
                        TemplateValueType.String,
                        TemplateParameterUsage.TableTypeSpecifyWord);
                    // 表格追加标签内容
                    _tableLabelTemplate.SetFunctionParameterValue(
                        _labelInTableFile,
                        TemplateValueType.String,
                        TemplateParameterUsage.TableLabelText);
                    result.AppendLine(_tableLabelTemplate.Exec());
                }
                // table filter
                if (!string.IsNullOrEmpty(_filterInTableFile) &&
                    _tableFilterTemplate != null)
                {
                    // 添加Filter条件
                    _tableFilterTemplate.SetFunctionParameterValue(
                        _filterInTableFile,
                        TemplateValueType.String,
                        TemplateParameterUsage.TableFilterText);
                    result.AppendLine(_tableFilterTemplate.Exec());
                }
            }

            return result.ToString();
        }

    }

    public enum TableType
    {
        None,
        Normal,
        Grid,
        GridSlice,
        MeanSummary,
        ResponseSummary,
    }

}
