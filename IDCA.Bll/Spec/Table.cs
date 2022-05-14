
using IDCA.Model.Template;
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
        
        public Table? this[string name] => _nameCache.ContainsKey(name) ? _nameCache[name] : null;

        readonly Dictionary<string, Table> _nameCache = new();
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
        public string Name { get => _name; set => _name = value; }

        /// <summary>
        /// 创建新的Table对象，并返回
        /// </summary>
        /// <returns></returns>
        public Table NewTable(TableType type = TableType.None)
        {
            var table = NewObject();
            table.Name = $"T{Count + 1}";
            table.Type = type;
            table.LoadTemplate(_templates);
            Add(table);
            if (!_nameCache.ContainsKey(table.Name))
            {
                _nameCache.Add(table.Name, table);
            }
            return table;
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

        string _name = string.Empty;
        /// <summary>
        /// 表格的对象名称
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        string _tableLabel = string.Empty;
        /// <summary>
        /// 表格的名称标签
        /// </summary>
        public string TableLabel { get => _tableLabel; set => _tableLabel = value; }

        string _tableBase = string.Empty;
        /// <summary>
        /// 表格的Base标签
        /// </summary>
        public string TableBase { get => _tableBase; set => _tableBase = value; }

        readonly FieldScript _field;
        FunctionTemplate? _template;

        /// <summary>
        /// 从已载入完成的模板集合中载入所需要的模板
        /// </summary>
        /// <param name="collection">已载入完成的模板集合</param>
        public void LoadTemplate(TemplateCollection collection)
        {
            switch (_type)
            {
                case TableType.Normal:
                    _template = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableNormal);
                    break;
                case TableType.Grid:
                    _template = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableGrid);
                    break;
                case TableType.GridSlice:
                    _template = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableGridSlice);
                    break;
                case TableType.MeanSummary:
                    _template = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableMeanSummary);
                    break;
                case TableType.ResponseSummary:
                    _template = collection.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableResponseSummary);
                    break;
                case TableType.None:
                default:
                    break;
            }
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

        string _title = "Null";
        /// <summary>
        /// 当前表格的标题
        /// </summary>
        public string Title { get => _title; set => _title = value; }

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

        /// <summary>
        /// 导入当前设置到字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            if (_template != null)
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

                _template.SetFunctionParameterValue(side, TemplateValueType.String, TemplateParameterUsage.TableSideVariableName);
                _template.SetFunctionParameterValue(banner, bannerType, TemplateParameterUsage.TableTopVariableName);
                _template.SetFunctionParameterValue(_tableLabel, TemplateValueType.String, TemplateParameterUsage.TableTitleText);
                _template.SetFunctionParameterValue(_tableBase, TemplateValueType.String, TemplateParameterUsage.TableBaseText);
                return _template.Exec();
            }

            return string.Empty;
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
