﻿
using IDCA.Bll.Template;
using System.Collections.Generic;
using System.Text;

namespace IDCA.Bll.SpecDocument
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
        }
        
        public Table? this[string name] => _nameCache.ContainsKey(name) ? _nameCache[name] : null;

        readonly Dictionary<string, Table> _nameCache = new();
        readonly TemplateCollection _templates;

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
            _field = new FieldExpressionTemplate();
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

        readonly FieldExpressionTemplate _field;
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
                    side = _sideAxis.Type == AxisType.AxisVariable ? _sideAxis.ToString() : $"{_field.Exec()}{_sideAxis}";
                }
                else
                {
                    side = _field.Exec();
                }

                if (_headerAxis != null && _headerAxis.Type == AxisType.Normal)
                {
                    banner = _type == TableType.Grid ? $"{_field.TopField}{_headerAxis}" : $"{_banner}{_headerAxis}";
                }
                else
                {
                    banner = _type == TableType.Grid ? _field.TopField : ((_headerAxis != null && _headerAxis.Type == AxisType.AxisVariable) ? _headerAxis.ToString() : _banner);
                }

                _template.SetFunctionParameterValue(side, TemplateParameterUsage.TableSideVariableName);
                _template.SetFunctionParameterValue(banner, TemplateParameterUsage.TableTopVariableName);
                _template.SetFunctionParameterValue(_tableLabel, TemplateParameterUsage.TableTitleText);
                _template.SetFunctionParameterValue(_tableBase, TemplateParameterUsage.TableBaseText);
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
