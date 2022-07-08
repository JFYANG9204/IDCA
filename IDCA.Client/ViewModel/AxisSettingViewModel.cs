using IDCA.Model;
using IDCA.Model.Spec;
using IDCA.Model.Template;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    /// <summary>
    /// AxisSettingView使用的ViewModel
    /// </summary>
    public class AxisSettingViewModel : ObservableObject
    {
        public AxisSettingViewModel(TableSettingElementViewModel parent, AxisOperator axisOperator) 
        {
            _parent = parent;
            _axisOperator = axisOperator;
            _axis = axisOperator.Axis;
            _availableElements = new ObservableCollection<AvailableElement>();
            for (int i = 0; i < _axisDefaultElements.Length; i++)
            {
                var ele = _axisDefaultElements[i];
                var type = (AxisElementType)(i + 2);
                _availableElements.Add(new AvailableElement(ele, type, e => AppendTreeNode(e)));
            }
            _availableSelectedIndex = -1;
            _topBottomBoxSelections = new ObservableCollection<CheckableItemViewModel>();
            _useAxisAsSideVariable = false;
            // Factor Sequence
            _addContinuousFactor = false;
            _factorSequenceSelectedIndex = 0;
            // Axis Operator
            _npsTopBox = _axisOperator.NpsTopBox.ToString();
            _npsBottomBox = _axisOperator.NpsBottomBox.ToString();
            _appendMean = false;
            _appendAverage = false;
            _averageSkipCodes = "";
            _averageDecimals = "";
            _meanVariable = string.Empty;
            _meanFilter = string.Empty;
            // 初始化Top/Bottom Box选项类别
            _topBottomBoxSelections = new ObservableCollection<CheckableItemViewModel>
            {
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop1BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop2BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop3BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop4BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop5BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop6BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisTop7BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom1BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom2BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom3BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom4BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom5BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom6BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisBottom7BoxName),
                CreateTableTopBottomBoxSelectionItem(ViewModelConstants.AxisNpsName)
            };
            _tree = new ObservableCollection<AxisTreeNode>();
            _nodeNames = new HashSet<string>();
            LoadFromAxis(Axis);
        }
        CheckableItemViewModel CreateTableTopBottomBoxSelectionItem(string label)
        {
            var vm = new CheckableItemViewModel(label, OnTableTopBottomBoxNameCheckedChanged);
            vm.Selected += () => UpdateTableTopBottomBoxName();
            return vm;
        }

        void UpdateTableTopBottomBoxName()
        {
            TopBottomBoxName = string.Join(',',
                _topBottomBoxSelections.Where(t => t.Checked)
                                            .Select(t => t.Name));
        }

        void OnTableTopBottomBoxNameCheckedChanged(CheckableItemViewModel _)
        {
            UpdateTableTopBottomBoxName();
        }


        readonly TableSettingElementViewModel _parent;
        readonly AxisOperator _axisOperator;
        Axis _axis;

        /// <summary>
        /// 当前窗口修改轴表达式对象
        /// </summary>
        public Axis Axis
        {
            get { return _axis; }
        }

        string _axisExpression = string.Empty;
        /// <summary>
        /// 当前轴表达式的文本内容
        /// </summary>
        public string AxisExpression
        {
            get { return _axisExpression; }
            set 
            { 
                SetProperty(ref _axisExpression, value);
                //_axis.FromString(value);
            }
        }

        readonly HashSet<string> _nodeNames;
        /// <summary>
        /// 当前轴包含的节点名称，用来判断元素名是否可用
        /// </summary>
        public HashSet<string> NodeNames => _nodeNames;
        /// <summary>
        /// 更新轴表达式文本内容
        /// </summary>
        public void UpdateAxisExpressionText()
        {
            AxisExpression = _axis.ToString();
        }

        ObservableCollection<CheckableItemViewModel> _topBottomBoxSelections;
        /// <summary>
        /// 表格轴表达式需要添加的Top/Bottom Box类型的勾选选项
        /// </summary>
        public ObservableCollection<CheckableItemViewModel> TopBottomBoxSelections
        {
            get { return _topBottomBoxSelections; }
            set { SetProperty(ref _topBottomBoxSelections, value); }
        }

        string _topBottomBoxName = string.Empty;
        /// <summary>
        /// 当前已选中的Top/Bottom Box的配置名，中间用逗号分隔
        /// </summary>
        public string TopBottomBoxName
        {
            get { return _topBottomBoxName; }
            set { SetProperty(ref _topBottomBoxName, value); }
        }

        /// <summary>
        /// 获取当前已选中的Box数值，如果未选中，返回null
        /// </summary>
        /// <returns></returns>
        public int[]? GetTopBottomBox()
        {
            var selectedBox = _topBottomBoxSelections.Where(s => s.Checked && s.Name != ViewModelConstants.AxisNpsName);
            if (!selectedBox.Any())
            {
                return null;
            }

            return selectedBox.Select(s =>
            {
                return s.Name switch
                {
                    ViewModelConstants.AxisTop1BoxName => 1,
                    ViewModelConstants.AxisTop2BoxName => 2,
                    ViewModelConstants.AxisTop3BoxName => 3,
                    ViewModelConstants.AxisTop4BoxName => 4,
                    ViewModelConstants.AxisTop5BoxName => 5,
                    ViewModelConstants.AxisTop6BoxName => 6,
                    ViewModelConstants.AxisTop7BoxName => 7,
                    ViewModelConstants.AxisBottom1BoxName => -1,
                    ViewModelConstants.AxisBottom2BoxName => -2,
                    ViewModelConstants.AxisBottom3BoxName => -3,
                    ViewModelConstants.AxisBottom4BoxName => -4,
                    ViewModelConstants.AxisBottom5BoxName => -5,
                    ViewModelConstants.AxisBottom6BoxName => -6,
                    ViewModelConstants.AxisBottom7BoxName => -7,
                    _ => 0
                };
            }).ToArray();
        }

        /// <summary>
        /// 判定当前是否选中了NPS
        /// </summary>
        /// <returns></returns>
        public bool SelectedNps()
        {
            return _topBottomBoxSelections.Any(e => e.Checked && e.Name == ViewModelConstants.AxisNpsName);
        }

        void UpdateAxisTopBottomBox()
        {
            var boxes = GetTopBottomBox();
            var nps = SelectedNps();
            if (boxes != null)
            {
                _axisOperator.CreateTopBottomBoxAxisExpression(_parent.BaseText, nps, boxes);
                LoadFromAxis(_axisOperator.Axis);
                return;
            }
            // 只添加NPS
            if (nps)
            {
                _axisOperator.CreateTopBottomBoxAxisExpression(_parent.BaseText, true);
                LoadFromAxis(_axisOperator.Axis);
                return;
            }

            _axisOperator.CreateBasicAxisExpression(_parent.BaseText);
            LoadFromAxis(_axisOperator.Axis);
        }

        /// <summary>
        /// 当前表格表侧Factor赋值的顺序，正序时递增，逆序时递减。
        /// </summary>
        public static readonly string[] FactorSequence = { "正序", "逆序" };

        /// <summary>
        /// 当前表格添加Net的类型
        /// </summary>
        public static readonly string[] TableNetTypeSelections =
        {
            "标准Net",
            "在具体选项前的Combine",
            "在Subtotal和具体选项之间的Combine",
            "放在最后的Combine",
        };

        int _netTypeSelectedIndex = 0;
        /// <summary>
        /// 当前表格添加Net的类型的选中索引
        /// </summary>
        public int NetTypeSelectedIndex
        {
            get { return _netTypeSelectedIndex; }
            set
            {
                SetProperty(ref _netTypeSelectedIndex, value);
                _axisOperator.NetType = (AxisNetType)value;
                _axisOperator.Update((AxisNetType)value);
                LoadFromAxis(_axisOperator.Axis);
            }
        }

        /// <summary>
        /// 获取当前配置的表格Net类型
        /// </summary>
        /// <returns></returns>
        public AxisNetType GetTableNetType()
        {
            return (AxisNetType)_netTypeSelectedIndex;
        }


        bool _appendMean;
        /// <summary>
        /// 添加计算均值，与AppendAverage互斥
        /// </summary>
        public bool AppendMean
        {
            get { return _appendMean; }
            set
            {
                SetProperty(ref _appendMean, value);
                _axisOperator.AppendMean = value;
                if (value)
                {
                    AppendAverage = false;
                    _axisOperator.AppendMeanStdDevStdErr();
                }
                else
                {
                    _axisOperator.RemoveMeanStdDevStdErr();
                }
                LoadFromAxis(_axis);
            }
        }

        bool _appendAverage;
        /// <summary>
        /// 添加计算平均提及，与AppendMean互斥
        /// </summary>
        public bool AppendAverage
        {
            get { return _appendAverage; }
            set
            {
                SetProperty(ref _appendAverage, value);
                _axisOperator.AppendAverage = value;
                
                if (value)
                {
                    AppendMean = false;
                    _axisOperator.AppendAverageMention();
                }
                else
                {
                    _axisOperator.RemoveAverageMention();
                }
                LoadFromAxis(_axis);
            }
        }

        string _averageSkipCodes;
        /// <summary>
        /// 添加Average时跳过的码号
        /// </summary>
        public string AverageSkipCodes
        {
            get { return _averageSkipCodes; }
            set 
            { 
                SetProperty(ref _averageSkipCodes, value);
                _axisOperator.AverageSkipCodes = value;
                _axisOperator.UpdateAverageSkipCodes(value);
                LoadFromAxis(_axis);
                UpdateAxisExpressionText();
            }
        }

        string _averageDecimals;
        /// <summary>
        /// 添加Average时保留的小数位数
        /// </summary>
        public string AverageDecimals
        {
            get { return _averageDecimals; }
            set
            {
                if (int.TryParse(value, out int i))
                {
                    SetProperty(ref _averageDecimals, value);
                    
                    var manipulation = _parent.Manipulation;
                    var func = manipulation?.First(FunctionTemplateFlags.ManipulateSideAverage);
                    if (func != null)
                    {
                        func.SetFunctionParameterValue(i, TemplateParameterUsage.ManipulateDecimals);
                    }
                    else
                    {
                        var derived = _axisOperator.First(e => e.Name.Equals(AxisConstants.AxisAverageDerivedName));
                        if (derived != null)
                        {
                            var suffix = derived.Suffix[AxisElementSuffixType.Decimals];
                            if (suffix == null)
                            {
                                derived.Suffix.AppendDecimals(i);
                            }
                            else
                            {
                                suffix.Value = i;
                            }
                        }
                    }

                    UpdateAxisExpressionText();
                }
            }
        }

        string _meanVariable;
        /// <summary>
        /// 添加计算均值时使用的变量
        /// </summary>
        public string MeanVariable
        {
            get { return _meanVariable; }
            set
            {
                SetProperty(ref _meanVariable, value);
                _axisOperator.MeanVariable = value;
                _axisOperator.UpdateMeanVariable(value);
                UpdateAxisExpressionText();
            }
        }

        string _meanFilter;
        /// <summary>
        /// 当前轴表达式添加均值时使用的Filter表达式
        /// </summary>
        public string MeanFilter
        {
            get { return _meanFilter; }
            set
            {
                SetProperty(ref _meanFilter, value);
                _axisOperator.MeanFilter = value;
                _axisOperator.UpdateMeanFilter(value);
                UpdateAxisExpressionText();
            }
        }

        bool _addContinuousFactor;
        /// <summary>
        /// 是否添加顺序（正序或逆序）的Factor赋值
        /// </summary>
        public bool AddContinuousFactor
        {
            get { return _addContinuousFactor; }
            set 
            { 
                SetProperty(ref _addContinuousFactor, value);
                if (_parent.Manipulation != null)
                {
                    if (value)
                    {
                        _parent.Manipulation.AppendSequentialFactorFunction(1, _factorSequenceSelectedIndex == 0 ? 1 : -1, _averageSkipCodes, true);
                    }
                    else
                    {
                        _parent.Manipulation.RemoveAll(FunctionTemplateFlags.ManipulateSetSequentialCodeFactor);
                    }
                }
            }
        }

        int _factorSequenceSelectedIndex;
        /// <summary>
        /// 当前表格Factor赋值顺序选择索引，修改时同步修改Axis对象配置
        /// </summary>
        public int FactorSequenceSelectedIndex
        {
            get { return _factorSequenceSelectedIndex; }
            set
            {
                SetProperty(ref _factorSequenceSelectedIndex, value);
                _axisOperator.IsTopBottomBoxReversed = value == 1;
                UpdateAxisTopBottomBox();
                if (_parent.Manipulation != null)
                {
                    var func = _parent.Manipulation.First(FunctionTemplateFlags.ManipulateSideAverage);
                    var parameter = func?.Parameters[TemplateParameterUsage.ManipulateSequentialFactorStep];
                    if (parameter != null)
                    {
                        parameter.GetValue().Value = value == 0 ? 1 : -1;
                    }
                }
            }
        }

        string _npsTopBox;
        /// <summary>
        /// Nps使用的Top Box包含的选项数量
        /// </summary>
        public string NpsTopBox
        {
            get { return _npsTopBox; }
            set
            {
                SetProperty(ref _npsTopBox, value);
                if (int.TryParse(value, out int iValue) && iValue > 0)
                {
                    _axisOperator.NpsTopBox = iValue;
                    UpdateAxisTopBottomBox();
                }
            }
        }

        string _npsBottomBox;
        /// <summary>
        /// Nps使用的Bottom Box包含的选项数量
        /// </summary>
        public string NpsBottomBox
        {
            get { return _npsBottomBox; }
            set
            {
                SetProperty(ref _npsBottomBox, value);
                if (int.TryParse(value, out int iValue) && iValue > 0)
                {
                    _axisOperator.NpsBottomBox = iValue;
                    UpdateAxisTopBottomBox();
                }
            }
        }

        bool _useAxisAsSideVariable;
        /// <summary>
        /// 是否使用轴表达式来添加Table
        /// </summary>
        public bool UseAxisAsSideVariable
        {
            get { return _useAxisAsSideVariable; }
            set
            {
                SetProperty(ref _useAxisAsSideVariable, value);
                if (value)
                {
                    _axisOperator.Axis.Type = AxisType.AxisVariable;
                }
                else
                {
                    _axisOperator.Axis.Type = AxisType.Normal;
                }
            }
        }


        static readonly string[] _axisDefaultElements =
        {
            "Category元素",
            "Category区间",
            "文本行(text)",
            "基数(base)",
            "未加权基数(unweightedbase)",
            "有效基数(effectivebase)",
            "表达式(expression)",
            "数值(numeric)",
            "自定义计算(derived)",
            "均值(mean)",
            "标准误差(stderr)",
            "标准差(stddev)",
            "总和(total)",
            "小计(subtotal)",
            "最小值(min)",
            "最大值(max)",
            "净值(net)",
            "合并(combine)",
            "求和(sum)",
            "中位数(median)",
            "百分比(percentile)",
            "模式(mode)",
            "Ntd(ntd)"
        };
        /// <summary>
        /// 轴表达式可用元素
        /// </summary>
        public class AvailableElement : ObservableObject
        {
            public AvailableElement(string name, AxisElementType type, Action<AvailableElement> adding)
            {
                _name = name;
                _type = type;
                _adding = adding;

                _scriptName = type switch
                {
                    AxisElementType.Category => "Category",
                    AxisElementType.CategoryRange => "CategoryRange",
                    AxisElementType.Text => "e",
                    AxisElementType.Base => "base",
                    AxisElementType.UnweightedBase => "UnweightedBase",
                    AxisElementType.EffectiveBase => "EffectiveBase",
                    AxisElementType.Expression => "Expression",
                    AxisElementType.Numeric => "Numeric",
                    AxisElementType.Derived => "Derived",
                    AxisElementType.Mean => "Mean",
                    AxisElementType.StdErr => "StdErr",
                    AxisElementType.StdDev => "StdDev",
                    AxisElementType.Total => "Total",
                    AxisElementType.SubTotal => "Subtotal",
                    AxisElementType.Min => "Min",
                    AxisElementType.Max => "Max",
                    AxisElementType.Net => "Net",
                    AxisElementType.Combine => "Combine",
                    AxisElementType.Sum => "Sum",
                    AxisElementType.Median => "Median",
                    AxisElementType.Percentile => "Percentile",
                    AxisElementType.Mode => "Mode",
                    AxisElementType.Ntd => "Ntd",
                    _ => string.Empty,
                };
            }

            string _name;
            /// <summary>
            /// 元素名
            /// </summary>
            public string Name
            {
                get { return _name; }
                set { SetProperty(ref _name, value); }
            }

            string _scriptName;
            /// <summary>
            /// 添加新元素时的默认名称前缀
            /// </summary>
            public string ScriptName
            {
                get { return _scriptName; }
                set { _scriptName = value; }
            }

            AxisElementType _type;
            /// <summary>
            /// 轴元素类型
            /// </summary>
            public AxisElementType Type
            {
                get { return _type; }
                set { _type = value; }
            }

            readonly Action<AvailableElement> _adding;
            /// <summary>
            /// 父级对象添加此类元素时执行的命令
            /// </summary>
            public ICommand AddCommand => new RelayCommand(() => _adding(this));
        }

        ObservableCollection<AvailableElement> _availableElements;
        /// <summary>
        /// 当前所有可用类型的轴元素
        /// </summary>
        public ObservableCollection<AvailableElement> AvailableElements
        {
            get => _availableElements;
            set => SetProperty(ref _availableElements, value);
        }

        int _availableSelectedIndex;
        /// <summary>
        /// 可用轴元素列表的选取索引
        /// </summary>
        public int AvailableSelectedIndex
        {
            get => _availableSelectedIndex;
            set => SetProperty(ref _availableSelectedIndex, value);
        }

        AxisTreeNode? _selectedNode;
        /// <summary>
        /// 当前轴已有元素树中的选中节点
        /// </summary>
        public AxisTreeNode? SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
        }

        ObservableCollection<AxisTreeNode> _tree;
        /// <summary>
        /// 当前已有元素构成的轴元素树
        /// </summary>
        public ObservableCollection<AxisTreeNode> Tree
        {
            get { return _tree; }
            set { SetProperty(ref _tree, value); }
        }
        /// <summary>
        /// 移动轴元素节点，可以向上或向下移动一个位置
        /// </summary>
        /// <param name="node">移动的节点</param>
        /// <param name="moveDown">是否向下移动</param>
        void MoveTreeNode(AxisTreeNode node, bool moveDown = false)
        {
            bool success;
            if (node.Parent != null)
            {
                int index = node.Parent.Children.IndexOf(node);
                if (moveDown)
                {
                    success = node.Parent.MoveChildDown(index);
                }
                else
                {
                    success = node.Parent.MoveChildUp(index);
                }
            }
            else
            {
                int index = _tree.IndexOf(node);
                int targetIndex = moveDown ? index + 1 : index - 1;
                success = CollectionHelper.Swap(_tree, index, targetIndex);
                if (success)
                {
                    _axis.Swap(index, targetIndex);
                }
            }

            if (success)
            {
                UpdateAxisExpressionText();
            }
        }

        //void MoveTreeNode(bool moveDown = false)
        //{
        //    if (_selectedNode == null)
        //    {
        //        return;
        //    }
        //
        //    var selected = _selectedNode;
        //    if (_selectedNode.Parent != null)
        //    {
        //        int index = _selectedNode.Parent.Children.IndexOf(_selectedNode);
        //        if (moveDown)
        //        {
        //            _selectedNode.Parent.MoveChildDown(index);
        //        }
        //        else
        //        {
        //            _selectedNode.Parent.MoveChildUp(index);
        //        }
        //    }
        //    else
        //    {
        //        int index = _tree.IndexOf(_selectedNode);
        //        CollectionHelper.Swap(_tree, index, moveDown ? index + 1 : index - 1);
        //    }
        //    selected.IsSelected = true;
        //}


        //void MoveUpTreeNode()
        //{
        //    MoveTreeNode();
        //    UpdateAxisExpression();
        //}
        //public ICommand MoveUpCommand => new RelayCommand(MoveUpTreeNode);
        //
        //void MoveDownTreeNode()
        //{
        //    MoveTreeNode(true);
        //    UpdateAxisExpression();
        //}
        //public ICommand MoveDownCommand => new RelayCommand(MoveDownTreeNode);
        /// <summary>
        /// 移除指定的树节点
        /// </summary>
        /// <param name="node"></param>
        void RemoveTreeNode(AxisTreeNode node)
        {
            if (node.Parent != null)
            {
                int index = node.Parent.Children.IndexOf(node);
                if (index > -1)
                {
                    node.Parent.RemoveChildAt(index);
                    node.Parent.AxisElement.Template.RemoveAt(index);
                }
            }
            else
            {
                int index = _tree.IndexOf(node);
                if (index > -1)
                {
                    _tree.RemoveAt(index);
                    _axis.RemoveAt(index);
                }
            }
            _nodeNames.RemoveWhere(n => n.Equals(node.Name, StringComparison.OrdinalIgnoreCase));
            UpdateAxisExpressionText();
        }

        //void RemoveTreeNode()
        //{
        //    if (_selectedNode == null)
        //    {
        //        return;
        //    }
        //
        //    if (_selectedNode.Parent != null)
        //    {
        //        int index = _selectedNode.Parent.Children.IndexOf(_selectedNode);
        //        if (index > -1)
        //        {
        //            _selectedNode.Parent.RemoveChildAt(index);
        //        }
        //    }
        //    else
        //    {
        //        int index = _tree.IndexOf(_selectedNode);
        //        if (index > -1)
        //        {
        //            _tree.RemoveAt(index);
        //        }
        //    }
        //    UpdateAxisExpression();
        //}
        //public ICommand RemoveTreeNodeCommand => new RelayCommand(RemoveTreeNode);

        void OnSelectedChanged(AxisTreeNode n, bool v)
        {
            if (v)
            {
                SelectedNode = n;
            }
        }
        /// <summary>
        /// 向当前树的选中节点后添加新的节点
        /// </summary>
        /// <param name="element">选中的可用元素类型</param>
        /// <param name="parentNode">父级节点，针对Net/Combine这种有子类的元素</param>
        void AppendTreeNode(AvailableElement element, AxisTreeNode? parentNode = null)
        {
            var axisElement = _axis.NewObject();
            axisElement.Template.ElementType = element.Type;
            axisElement.Name = $"{element.ScriptName}{_axis.CountIf(element.Type) + 1}";

            var node = new AxisTreeNode(this, axisElement, OnSelectedChanged);
            node.AddingChild += AppendTreeElementChildNode;
            node.Removing += RemoveTreeNode;
            node.MovingUp += n => MoveTreeNode(n);
            node.MovingDown += n => MoveTreeNode(n, true);
            node.Name = axisElement.Name;
            node.Parent = parentNode;

            if (parentNode != null)
            {
                var parameter = parentNode.AxisElement.Template.NewParameter();
                parameter.SetValue(axisElement);

                int currentIndex = _selectedNode == null ? -1 : parentNode.Children.IndexOf(_selectedNode);
                if (currentIndex == -1 || currentIndex == parentNode.Children.Count - 1)
                {
                    parentNode.Children.Add(node);
                    parentNode.AxisElement.Template.PushParameter(parameter);
                }
                else
                {
                    parentNode.Children.Insert(currentIndex + 1, node);
                    parentNode.AxisElement.Template.InsertParameter(currentIndex + 1, parameter);
                }

            }
            else
            {
                int currentIndex = _selectedNode == null ? -1 : _tree.IndexOf(_selectedNode);

                if (currentIndex == -1 || currentIndex == _tree.Count - 1)
                {
                    _tree.Add(node);
                    _axis.Add(axisElement);
                }
                else
                {
                    _tree.Insert(currentIndex + 1, node);
                    _axis.Insert(currentIndex + 1, axisElement);
                }

            }
            _nodeNames.Add(element.Name);
            UpdateAxisExpressionText();
        }

        void AppendTreeElementChildNode(AxisTreeNode node)
        {
            if (_availableSelectedIndex < 0 ||
                _availableSelectedIndex >= _availableElements.Count)
            {
                return;
            }
            AppendTreeNode(_availableElements[_availableSelectedIndex], node);
        }

        //void AppendTreeNode()
        //{
        //    if (_availableSelectedIndex < 0 || 
        //        _availableSelectedIndex >= _availableElements.Count)
        //    {
        //        return;
        //    }
        //
        //    AxisElementType type = (AxisElementType)(_availableSelectedIndex + 1);
        //    string name = _availableElements[_availableSelectedIndex].ScriptName;
        //
        //    var axisElement = _axis.AppendElement(type);
        //
        //    var node = new AxisTreeNode(this, axisElement, OnSelectedChanged);
        //    node.Removing += RemoveTreeNode;
        //    node.MovingUp += n => MoveTreeNode(n);
        //    node.MovingDown += n => MoveTreeNode(n, true);
        //    node.Name = name;
        //    if (_selectedNode == null)
        //    {
        //        _tree.Add(node);
        //    }
        //    else
        //    {
        //        int selectedIndex = _tree.IndexOf(_selectedNode);
        //        if (selectedIndex > -1)
        //        {
        //            _tree.Insert(selectedIndex + 1, node);
        //        }
        //    }
        //    UpdateAxisExpression();
        //}
        //public ICommand AppendTreeNodeCommand => new RelayCommand(AppendTreeNode);


        //void Confirm(object? sender)
        //{
        //    ApplyToAxis();
        //    WindowManager.CloseWindow(sender);
        //}
        //public ICommand ConfirmCommand => new RelayCommand<object?>(Confirm);

        //void Cancel(object? sender)
        //{
        //    WindowManager.CloseWindow(sender);
        //}
        //public ICommand CancelCommand => new RelayCommand<object?>(Cancel);

        //public void ApplyToAxis()
        //{
        //    if (_axis == null)
        //    {
        //        return;
        //    }
        //
        //    foreach (var node in _tree)
        //    {
        //        var element = _axis.NewObject();
        //        node.ApplyToAxisElement(element);
        //        _axis.Add(element);
        //    }
        //}

        /// <summary>
        /// 从已有的轴表达式对象载入UI数据
        /// </summary>
        /// <param name="axis"></param>
        public void LoadFromAxis(Axis axis)
        {
            _axis = axis;
            _tree.Clear();
            foreach (AxisElement element in axis)
            {
                var node = new AxisTreeNode(this, element, OnSelectedChanged);
                //node.LoadFromAxisElement(element);
                node.Removing += RemoveTreeNode;
                node.MovingUp += n => MoveTreeNode(n);
                node.MovingDown += n => MoveTreeNode(n, true);
                _tree.Add(node);
            }
            UpdateAxisExpressionText();
        }

    }
    /// <summary>
    /// AxisSettingView中，AxisElement后缀列表使用的ViewModel
    /// </summary>
    public class AxisElementSuffixViewModel : ObservableObject
    {

        public AxisElementSuffixViewModel(AxisTreeNode node, AxisElementSuffixType type, AxisElement axisElement)
        {
            _type = type;
            _name = string.Empty;
            _text = string.Empty;
            _selectedIndex = 0;
            _selections = new ObservableCollection<string>();
            _checked = false;
            _isTextBox = true;
            _isComboBox = false;
            _axisElement = axisElement;
            _node = node;
        }

        //Action<AxisElementSuffixViewModel>? _checkChanged;
        //public event Action<AxisElementSuffixViewModel>? CheckChanged
        //{
        //    add { _checkChanged += value; }
        //    remove { _checkChanged -= value; }
        //}
        //
        //Action<AxisElementSuffixViewModel>? _valueChanged;
        //public event Action<AxisElementSuffixViewModel>? ValueChanged
        //{
        //    add { _valueChanged += value; }
        //    remove { _valueChanged -= value; }
        //}

        readonly AxisElement _axisElement;
        readonly AxisElementSuffixType _type;
        /// <summary>
        /// 当前后缀所在的Axis元素树节点
        /// </summary>
        readonly AxisTreeNode _node;
        /// <summary>
        /// 后缀类型，可以在AxisElement中直接使用。
        /// 此类型只用于标注这个配置对应的元素类型，不代表AxisElement对象中一定有这个类型的配置。
        /// </summary>
        public AxisElementSuffixType Type => _type;

        string _name;
        /// <summary>
        /// 后缀名，应该是AxisElementSuffixType对应的名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        string _text;
        /// <summary>
        /// TextBox中填入的内容
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty(ref _text, value);
                UpdateAxisExpression();
            }
        }

        int _selectedIndex;
        /// <summary>
        /// 可选值选中的索引
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set 
            { 
                SetProperty(ref _selectedIndex, value);
                UpdateAxisExpression();
            }
        }

        ObservableCollection<string> _selections;
        /// <summary>
        /// 如果后缀值用ComboBox选取，此属性值是ComboBox的可选值
        /// </summary>
        public ObservableCollection<string> Selections
        {
            get
            {
                return _selections;
            }
            set
            {
                SetProperty(ref _selections, value);
            }
        }

        bool _checked;
        /// <summary>
        /// 当前后缀是否选中
        /// </summary>
        public bool Checked
        {
            get { return _checked; }
            set 
            { 
                SetProperty(ref _checked, value);
                UpdateAxisExpression();
            }
        }

        bool _isTextBox;
        /// <summary>
        /// 当前值用TextBox填入
        /// </summary>
        public bool IsTextBox
        {
            get
            {
                return _isTextBox;
            }
            set
            {
                SetProperty(ref _isTextBox, value);
            }
        }

        bool _isComboBox;
        /// <summary>
        /// 当前值用ComboBox选取
        /// </summary>
        public bool IsComboBox
        {
            get
            {
                return _isComboBox;
            }
            set
            {
                SetProperty(ref _isComboBox, value);
            }
        }
        /// <summary>
        /// 向当前值选项末尾添加新的值
        /// </summary>
        /// <param name="seletions"></param>
        public void AddSelections(params string[] seletions)
        {
            foreach (var item in seletions)
            {
                _selections.Add(item);
            }
        }
        /// <summary>
        /// 获取当前选中的值或填入的值
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            if (_isTextBox)
            {
                return _text;
            }
            else if (_selectedIndex >= 0 && _selectedIndex < _selections.Count)
            {
                return _selections[_selectedIndex];
            }
            return string.Empty;
        }
        /// <summary>
        /// 将当前配置应用到轴元素
        /// </summary>
        /// <param name="element"></param>
        public void ApplyToAxisElement(AxisElement? element)
        {
            if (element == null || !_checked)
            {
                return;
            }

            var suffix = element.Suffix.NewObject();
            suffix.Type = Converter.ConvertToAxisElementSuffixType(_name);
            suffix.Value = GetValue();
            element.Suffix.Add(suffix);
        }
        /// <summary>
        /// 从轴元素载入当前UI配置
        /// </summary>
        /// <param name="suffix"></param>
        public void LoadFromAxisElement(AxisElementSuffix? suffix)
        {
            if (suffix == null || !_name.Equals(suffix.Type.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _text = suffix.Value.ToString() ?? string.Empty;
            _checked = true;

            switch (suffix.Type)
            {
                case AxisElementSuffixType.CalculationScope:
                case AxisElementSuffixType.CountsOnly:
                case AxisElementSuffixType.IsFixed:
                case AxisElementSuffixType.IsHidden:
                case AxisElementSuffixType.IsHiddenWhenColumn:
                case AxisElementSuffixType.IsHiddenWhenRow:
                case AxisElementSuffixType.IncludeInBase:
                case AxisElementSuffixType.IsUnweighted:
                    IsComboBox = true;
                    break;

                case AxisElementSuffixType.Decimals:
                case AxisElementSuffixType.Factor:
                case AxisElementSuffixType.Multiplier:
                case AxisElementSuffixType.Weight:
                    IsTextBox = true;
                    break;

                case AxisElementSuffixType.None:
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(_text) && _isComboBox && _selections.Count > 0)
            {
                SelectedIndex = _selections.IndexOf(_text);
            }
        }

        void UpdateAxisExpression()
        {
            var suffix = _axisElement.Suffix[_type];
            if (!Checked && suffix != null)
            {
                _axisElement.Suffix.Remove(suffix);
                _node.Root.UpdateAxisExpressionText();
            }
            else if (Checked)
            {
                if (suffix != null)
                {
                    suffix.Value = GetValue();
                }
                else
                {
                    _axisElement.Suffix.Append(_type).Value = GetValue();
                }
                _node.Root.UpdateAxisExpressionText();
            }
        }
    }

    /// <summary>
    /// 存储AxisSettingView里AxisElement树节点详细配置的ViewModel。
    /// 存储配置值的控件可以是TextBox或者ComboBox。
    /// </summary>
    public class AxisElementDetailViewModel : ObservableObject
    {
        public AxisElementDetailViewModel()
        {
            _name = string.Empty;
            _text = string.Empty;
            _isTextBox = true;
            _isComboBox = false;
            _type = AxisElementDetailType.None;
            _selectedIndex = 1;
            _selections = new ObservableCollection<string>();
            _isReadOnly = false;
        }

        public AxisElementDetailViewModel(AxisElementDetailType type) : this()
        {
            var t = type.GetType();
            FieldInfo? info = t.GetField(type.ToString());
            if (info != null)
            {
                var attr = info.GetCustomAttributes<AxisDescriptionAttribute>();
                AxisDescriptionAttribute? desc;
                if (attr != null && (desc = attr.First()) != null)
                {
                    _name = desc.Description;
                    _isTextBox = desc.IsTextBox;
                    _isComboBox = desc.IsComboBox;
                    foreach (string s in desc.Selections)
                    {
                        _selections.Add(s);
                    }
                }
            }
            _type = type;
        }

        Func<AxisElementDetailViewModel, bool>? _beforeValueChanged;
        /// <summary>
        /// 在修改Text属性时，如果此事件不为null，会优先触发此事件来判断值是否可用。
        /// 如果返回true或者事件未配置，将修改成功，否则，将修改失败。
        /// </summary>
        public event Func<AxisElementDetailViewModel, bool> BeforeValueChanged
        {
            add { _beforeValueChanged += value; }
            remove { _beforeValueChanged -= value; }
        }

        Action<AxisElementDetailViewModel, string>? _valueChanged;
        /// <summary>
        /// 在修改Text值或SelectedIndex后，触发的事件。
        /// </summary>
        public event Action<AxisElementDetailViewModel, string> ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        AxisElementDetailType _type;
        /// <summary>
        /// 当前对象的详细配置类型
        /// </summary>
        public AxisElementDetailType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        int _indexOfElement = -1;
        /// <summary>
        /// 当前对象在AxisTreeNode中的索引位置
        /// </summary>
        public int IndexOfElement
        {
            get { return _indexOfElement; }
            set { _indexOfElement = value; }
        }

        bool _isReadOnly;
        /// <summary>
        /// 使用的控件是否是只读的
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }

        string _name;
        /// <summary>
        /// 当前详细设置的名称，显示在ComboBox或TextBox左边
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        string _text;
        /// <summary>
        /// 当前详细配置值控件是TextBox时，绑定TextBox内的文本内容。
        /// </summary>
        public string Text
        {
            get { return _text; }
            set 
            {
                if (_beforeValueChanged == null || _beforeValueChanged(this))
                {
                    string oldValue = _text;
                    SetProperty(ref _text, value);
                    _valueChanged?.Invoke(this, _text);
                }
            }
        }

        bool _isTextBox;
        /// <summary>
        /// 当前配置值控件是否是TextBox，和IsComboBox不能同时为true。
        /// </summary>
        public bool IsTextBox
        {
            get { return _isTextBox; }
            set
            {
                SetProperty(ref _isTextBox, value);
                if (value)
                {
                    IsComboBox = false;
                }
            }
        }

        bool _isComboBox;
        /// <summary>
        /// 当前配置值控件是否是ComboBox，和IsTextBox不能同时为true。
        /// </summary>
        public bool IsComboBox
        {
            get { return _isComboBox; }
            set
            {
                SetProperty(ref _isComboBox, value);
                if (value)
                {
                    IsTextBox = false;
                }
            }
        }

        int _selectedIndex;
        /// <summary>
        /// 绑定值选择ComboBox的已选择索引
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set 
            {
                string oldValue = GetValue();
                SetProperty(ref _selectedIndex, value);
                _valueChanged?.Invoke(this, oldValue);
            }
        }

        ObservableCollection<string> _selections;
        /// <summary>
        /// 绑定ComboBox内的选项内容。
        /// </summary>
        public ObservableCollection<string> Selections
        {
            get { return _selections; }
            set { SetProperty(ref _selections, value); }
        }
        /// <summary>
        /// 设置当前ComboBox控件的选项内容。
        /// </summary>
        /// <param name="selections"></param>
        public void SetSelections(params string[] selections)
        {
            foreach (var s in selections)
            {
                _selections.Add(s);
            }
        }
        /// <summary>
        /// 获取当前配置的字符串值，无论当前使用控件是TextBox还是ComboBox。
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            if (_isComboBox)
            {
                return _selectedIndex >= 0 && _selectedIndex < _selections.Count ?
                    _selections[_selectedIndex] : string.Empty;
            }
            else
            {
                return _text;
            }
        }
        /// <summary>
        /// 将当前详细配置应用到已有的轴元素中
        /// </summary>
        /// <param name="element"></param>
        public void ApplyToAxisElement(AxisElement element)
        {
            switch (_type)
            {
                case AxisElementDetailType.Name:
                    element.Name = _text;
                    break;
                case AxisElementDetailType.Description:
                    element.Description = _text;
                    break;

                case AxisElementDetailType.Exclude:
                    element.Exclude = bool.TryParse(GetValue(), out bool b) && b;
                    break;

                case AxisElementDetailType.Filter:
                case AxisElementDetailType.VariableName:
                case AxisElementDetailType.CutOff:
                    var parameter = element.Template.GetParameter(_indexOfElement);
                    if (parameter == null)
                    {
                        while (element.Template.Count < _indexOfElement + 1)
                        {
                            parameter = element.Template.NewParameter();
                            element.Template.PushParameter(parameter);
                        }
                    }
                    parameter?.SetValue(_text);
                    break;

                case AxisElementDetailType.None:
                default:
                    break;
            }
        }
        /// <summary>
        /// 从已有的轴元素对象读取配置内容。
        /// </summary>
        /// <param name="element"></param>
        public void LoadFromAxisElement(AxisElement element)
        {
            switch (_type)
            {
                case AxisElementDetailType.Name:
                    IsTextBox = true;
                    _name = ViewModelConstants.AxisElementDetailName;
                    _text = element.Name;
                    break;
                case AxisElementDetailType.Description:
                    IsTextBox = true;
                    _name = ViewModelConstants.AxisElementDetailDescription;
                    _text = string.IsNullOrEmpty(element.Description) ? string.Empty : element.Description;
                    break;
                case AxisElementDetailType.Exclude:
                    IsComboBox = true;
                    Selections.Clear();
                    SetSelections("True", "False");
                    SelectedIndex = element.Exclude ? 0 : 1;
                    break;
                case AxisElementDetailType.Filter:
                case AxisElementDetailType.VariableName:
                case AxisElementDetailType.CutOff:
                case AxisElementDetailType.CategoryUpperBoundary:
                case AxisElementDetailType.CategoryLowerBoundary:
                    IsTextBox = true;

                    if (_type == AxisElementDetailType.Filter)
                    {
                        _name = ViewModelConstants.AxisElementDetailFilter;
                    }
                    else if (_type == AxisElementDetailType.VariableName)
                    {
                        _name = ViewModelConstants.AxisElementDetailVariableName;
                    }
                    else if (_type == AxisElementDetailType.CategoryUpperBoundary)
                    {
                        _name = ViewModelConstants.AxisElementDetailCategoryUpperBoundary;
                    }
                    else if (_type == AxisElementDetailType.CategoryLowerBoundary)
                    {
                        _name = ViewModelConstants.AxisElementDetailCategoryLowerBoundary;
                    }
                    else
                    {
                        _name = ViewModelConstants.AxisElementDetailCutOff;
                    }
                    
                    if (_indexOfElement >= 0 && _indexOfElement < element.Template.Count)
                    {
                        var parameter = element.Template.GetParameter(_indexOfElement)?.GetValue().ToString();
                        _text = parameter ?? string.Empty;
                    }
                    break;

                case AxisElementDetailType.None:
                default:
                    break;
            }
        }

    }

    public enum AxisElementDetailType
    {
        [AxisDescription("")]
        None,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailType,
            IsComboBox = false,
            IsTextBox = true)]
        Type,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailName,
            IsComboBox = false,
            IsTextBox = true)]
        Name,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailDescription,
            IsComboBox = false,
            IsTextBox = true)]
        Description,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailExclude,
            IsComboBox = true,
            IsTextBox = false,
            Selections = new string[] { "true", "false" })]
        Exclude,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailFilter,
            IsComboBox = false,
            IsTextBox = true)]
        Filter,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailVariableName,
            IsComboBox = false,
            IsTextBox = true)]
        VariableName,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailCutOff,
            IsComboBox = false,
            IsTextBox = true)]
        CutOff,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailCategoryUpperBoundary,
            IsComboBox = false,
            IsTextBox = true)]
        CategoryUpperBoundary,
        [AxisDescription(
            ViewModelConstants.AxisElementDetailCategoryLowerBoundary,
            IsComboBox = false,
            IsTextBox = true)]
        CategoryLowerBoundary
    }

    /// <summary>
    /// AxisSettingView中Axis及其下级元素用TreeView表示出来，AxisTreeNode绑定
    /// TreeView内的基础节点。
    /// </summary>
    public class AxisTreeNode : ObservableObject
    {
        public AxisTreeNode(AxisSettingViewModel root, AxisElement element)
        {
            _root = root;
            _axisElement = element;
            _elementType = element.Template.ElementType;
            _allowChildren = _elementType == AxisElementType.Net || _elementType == AxisElementType.Combine;
            _elementType = AxisElementType.None;
            _name = string.Empty;
            _children = new ObservableCollection<AxisTreeNode>();
            _suffixes = new ObservableCollection<AxisElementSuffixViewModel>();
            _details = new ObservableCollection<AxisElementDetailViewModel>();
            // 填入初始化的后缀配置
            // CalculationScope=AllElements|PrecedingElements
            InitSuffixSelectionItem(AxisElementSuffixType.CalculationScope, "AllElements", "PrecedingElements");
            // CountsOnly=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.CountsOnly, "True", "False");
            // Decimals=DecimalPlaces
            InitSuffixTextBoxItem(AxisElementSuffixType.Decimals);
            // Factor=FactorValue
            InitSuffixTextBoxItem(AxisElementSuffixType.Factor);
            // IsFixed=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsFixed, "True", "False");
            // IsHidden=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsHidden, "True", "False");
            // IsHiddenWhenColumn = True | False
            InitSuffixSelectionItem(AxisElementSuffixType.IsHiddenWhenColumn, "True", "False");
            // IsHiddenWhenRow=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsHiddenWhenRow, "True", "False");
            // IncludeInBase=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IncludeInBase, "True", "False");
            // IsUnweighted=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsUnweighted, "True", "False");
            // Multiplier=MultiplierVariable
            InitSuffixTextBoxItem(AxisElementSuffixType.Multiplier);
            // Weight=WeightVariable
            InitSuffixTextBoxItem(AxisElementSuffixType.Weight);

            LoadFromAxisElement(_axisElement);
        }

        public AxisTreeNode(AxisSettingViewModel root, AxisElement axisElement, Action<AxisTreeNode, bool>? selectedChanged) : this(root, axisElement)
        {
            _selectedChanged = selectedChanged;
            _allowChildren = _elementType == AxisElementType.Combine || _elementType == AxisElementType.Net;
        }

        readonly AxisElement _axisElement;
        /// <summary>
        /// 树节点对应的Axis轴元素，修改ViewModel的时候会同步修改此对象的数据。
        /// </summary>
        public AxisElement AxisElement => _axisElement;

        readonly AxisSettingViewModel _root;
        /// <summary>
        /// 根节点，为存储整个轴的ViewModel。
        /// </summary>
        public AxisSettingViewModel Root => _root;

        Action<AxisTreeNode, bool>? _selectedChanged;
        /// <summary>
        /// 当TreeView选择这个节点或者取消选择这个节点时触发的事件。
        /// </summary>
        public Action<AxisTreeNode, bool>? SelectedChanged
        {
            get => _selectedChanged;
            set => _selectedChanged = value;
        }

        AxisElementType _elementType;
        /// <summary>
        /// 当前配置的轴元素类型。
        /// </summary>
        public AxisElementType ElementType
        {
            get { return _elementType; }
            set
            {
                _elementType = value;
                Details.Clear();
                InitDetail(value);
                _allowChildren = value == AxisElementType.Combine || value == AxisElementType.Net;
            }
        }

        bool _isSelected;
        /// <summary>
        /// 当前节点在TreeView中是否被选中。
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
                _selectedChanged?.Invoke(this, value);
            }
        }

        Action<AxisTreeNode>? _removing;
        /// <summary>
        /// 在父级对象（AxisSettingViewModel或AxisTreeNode）中移除此对象。
        /// 此回调由父级对象提供。
        /// </summary>
        public event Action<AxisTreeNode>? Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        Action<AxisTreeNode>? _movingUp;
        /// <summary>
        /// 在父级对象中（AxisSettingViewModel或AxisTreeNode）向上移动此对象一个位置
        /// 此回调由父级对象提供。
        /// </summary>
        public event Action<AxisTreeNode>? MovingUp
        {
            add { _movingUp += value; }
            remove { _removing -= value; }
        }

        Action<AxisTreeNode>? _movingDown;
        /// <summary>
        /// 在父级对象中（AxisSettingViewModel或AxisTreeNode）向下移动此对象一个位置
        /// 此回调由父级对象提供。
        /// </summary>
        public event Action<AxisTreeNode>? MovingDown
        {
            add { _movingDown += value; }
            remove { _movingDown -= value; }
        }

        Action<AxisTreeNode>? _addingChild;
        /// <summary>
        /// 当前对象添加下级节点，回调由父级对象提供。
        /// </summary>
        public event Action<AxisTreeNode>? AddingChild
        {
            add { _addingChild += value; }
            remove { _addingChild -= value; }
        }

        Action<AxisTreeNode>? _renamed;
        /// <summary>
        /// 当前对象重命名后触发的事件。
        /// </summary>
        public event Action<AxisTreeNode>? Renamed
        {
            add { _renamed += value; }
            remove { _renamed -= value; }
        }

        bool _allowChildren;
        /// <summary>
        /// 当前节点是否允许添加子节点。
        /// </summary>
        public bool AllowChildren
        {
            get { return _allowChildren; }
            set { SetProperty(ref _allowChildren, value); }
        }
        /// <summary>
        /// 向当前节点对象中添加下级节点对象，此方法使用AddingChild回调，
        /// 如果AddingChild回调未配置或AllowChild为false，将无法添加。
        /// </summary>
        void AddChild()
        {
            if (_allowChildren)
            {
                _addingChild?.Invoke(this);
            }
        }
        public ICommand AddChildCommand => new RelayCommand(AddChild);

        void Remove()
        {
            _removing?.Invoke(this);
        }
        public ICommand RemoveCommand => new RelayCommand(Remove);

        void MoveUp()
        {
            _movingUp?.Invoke(this);
        }
        public ICommand MoveUpCommand => new RelayCommand(MoveUp);

        void MoveDown()
        {
            _movingDown?.Invoke(this);
        }
        public ICommand MoveDownCommand => new RelayCommand(MoveDown);

        AxisTreeNode? _parent;
        public AxisTreeNode? Parent { get => _parent; set => _parent = value; }

        //void OnSuffixChecked(AxisElementSuffixViewModel viewModel)
        //{
        //    var existSuffix = _axisElement.Suffix[viewModel.Type];
        //    if (viewModel.Checked)
        //    {
        //        if (existSuffix != null)
        //        {
        //            existSuffix.Value = viewModel.GetValue();
        //        }
        //        else
        //        {
        //            var suffix = _axisElement.Suffix.Append(viewModel.Type);
        //            suffix.Value = viewModel.GetValue();
        //        }
        //    }
        //    else
        //    {
        //        if (existSuffix != null)
        //        {
        //            _axisElement.Suffix.RemoveIf(e => e.Type == viewModel.Type);
        //        }
        //    }
        //    _root.UpdateAxisExpression();
        //}
        //
        //void OnSuffixValueChanged(AxisElementSuffixViewModel viewModel)
        //{
        //    if (viewModel.Checked)
        //    {
        //        var suffix = _axisElement.Suffix[viewModel.Type];
        //        if (suffix != null)
        //        {
        //            suffix.Value = viewModel.GetValue();
        //            _root.UpdateAxisExpression();
        //        }
        //    }
        //}

        void InitSuffixTextBoxItem(AxisElementSuffixType type)
        {
            var element = new AxisElementSuffixViewModel(this, type, _axisElement)
            {
                Name = type.ToString(),
                IsComboBox = false,
                IsTextBox = true
            };
            _suffixes.Add(element);
        }

        void InitSuffixSelectionItem(AxisElementSuffixType type, params string[] selections)
        {
            var element = new AxisElementSuffixViewModel(this, type, _axisElement)
            {
                Name = type.ToString(),
                IsComboBox = true,
                IsTextBox = false,
            };
            element.AddSelections(selections);
            _suffixes.Add(element);
        }
        /// <summary>
        /// 判断当前输入的字符是否是作为元素名的有效值
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        bool ValidateNodeName(AxisElementDetailViewModel detail)
        {
            string value = detail.GetValue();
            return !Root.NodeNames.Contains(value.ToLower()) && StringHelper.ValidateElementName(value);
        }

        void OnDetailTextValueChanged(AxisElementDetailViewModel viewModel, string oldValue)
        {
            switch (viewModel.Type)
            {
                case AxisElementDetailType.Name:
                    Root.NodeNames.Remove(oldValue.ToLower());
                    Name = viewModel.GetValue();
                    break;
                case AxisElementDetailType.Description:
                    _axisElement.Description = viewModel.GetValue();
                    break;
                case AxisElementDetailType.Exclude:
                    _axisElement.Exclude = bool.TryParse(viewModel.GetValue(), out bool b) && b;
                    break;
                case AxisElementDetailType.Filter:
                case AxisElementDetailType.VariableName:
                case AxisElementDetailType.CutOff:
                case AxisElementDetailType.CategoryUpperBoundary:
                case AxisElementDetailType.CategoryLowerBoundary:
                    var parameter = _axisElement.Template.GetParameter(viewModel.IndexOfElement);
                    if (parameter == null)
                    {
                        while (_axisElement.Template.Count < viewModel.IndexOfElement + 1)
                        {
                            parameter = _axisElement.Template.NewParameter();
                            _axisElement.Template.PushParameter(parameter);
                        }
                    }
                    parameter?.SetValue(viewModel.GetValue());
                    break;
                case AxisElementDetailType.None:
                default:
                    break;
            }
            _root.UpdateAxisExpressionText();
        }

        void InitDetailElement(AxisElementDetailType type, int? indexOfElement = null)
        {
            var detail = new AxisElementDetailViewModel(type);
            if (indexOfElement != null && indexOfElement >= 0)
            {
                detail.IndexOfElement = (int)indexOfElement;
            }
            // 添加需要关联值的事件
            if (type == AxisElementDetailType.Name)
            {
                detail.BeforeValueChanged += ValidateNodeName;
            }
            detail.ValueChanged += OnDetailTextValueChanged;

            Details.Add(detail);
        }
        /// <summary>
        /// 初始化指定类型的详细配置对象。
        /// </summary>
        /// <param name="type"></param>
        public void InitDetail(AxisElementType type)
        {
            // 名称
            var typeDetail = new AxisElementDetailViewModel(AxisElementDetailType.Type)
            {
                IsReadOnly = true,
                Text = type.ToString()
            };
            Details.Add(typeDetail);

            switch (type)
            {
                case AxisElementType.InsertFunctionOrVariable:
                    break;

                case AxisElementType.CategoryRange:
                    // InitDetailElement(AxisElementDetailType.Description);
                    InitDetailElement(AxisElementDetailType.CategoryLowerBoundary, 0);
                    InitDetailElement(AxisElementDetailType.CategoryUpperBoundary, 1);
                    InitDetailElement(AxisElementDetailType.Exclude);
                    break;

                // 单个Filter条件
                case AxisElementType.Base:
                case AxisElementType.UnweightedBase:
                case AxisElementType.EffectiveBase:
                case AxisElementType.Expression:
                case AxisElementType.Derived:
                    InitDetailElement(AxisElementDetailType.Name);
                    InitDetailElement(AxisElementDetailType.Description);
                    InitDetailElement(AxisElementDetailType.Filter, 0);
                    InitDetailElement(AxisElementDetailType.Exclude);
                    break;

                // (Variable, 'Filter')
                case AxisElementType.Numeric:
                case AxisElementType.Mean:
                case AxisElementType.StdErr:
                case AxisElementType.StdDev:
                case AxisElementType.Min:
                case AxisElementType.Max:
                case AxisElementType.Sum:
                case AxisElementType.Median:
                case AxisElementType.Mode:
                    InitDetailElement(AxisElementDetailType.Name);
                    InitDetailElement(AxisElementDetailType.Description);
                    InitDetailElement(AxisElementDetailType.VariableName, 0);
                    InitDetailElement(AxisElementDetailType.Filter, 1);
                    InitDetailElement(AxisElementDetailType.Exclude);
                    break;

                case AxisElementType.Percentile:
                    InitDetailElement(AxisElementDetailType.Name);
                    InitDetailElement(AxisElementDetailType.Description);
                    InitDetailElement(AxisElementDetailType.VariableName, 0);
                    InitDetailElement(AxisElementDetailType.CutOff, 1);
                    InitDetailElement(AxisElementDetailType.Filter, 2);
                    InitDetailElement(AxisElementDetailType.Exclude);
                    break;

                case AxisElementType.Net:
                case AxisElementType.Combine:
                case AxisElementType.Category:
                case AxisElementType.Text:
                case AxisElementType.Total:
                case AxisElementType.SubTotal:
                case AxisElementType.None:
                case AxisElementType.Ntd:
                    InitDetailElement(AxisElementDetailType.Name);
                    InitDetailElement(AxisElementDetailType.Description);
                    InitDetailElement(AxisElementDetailType.Exclude);
                    break;

                default:
                    break;
            }
        }

        string _name;
        /// <summary>
        /// 当前节点名，对应AxisElement中的Name属性，在修改前会验证传入值，
        /// 当轴表达式中已存在此名称（不区分大小写）时，将修改失败。
        /// </summary>
        public string Name 
        { 
            get { return _name; }
            set
            {
                if (_root.NodeNames.Contains(value.ToLower()) || !AxisConstants.CheckName(value))
                {
                    return;
                }
                SetProperty(ref _name, value);
                _axisElement.Name = value;
                _renamed?.Invoke(this);
            }
        }

        ObservableCollection<AxisTreeNode> _children;
        /// <summary>
        /// 当前节点的子节点集合。
        /// </summary>
        public ObservableCollection<AxisTreeNode> Children
        {
            get { return _children; }
            set { SetProperty(ref _children, value); }
        }

        ObservableCollection<AxisElementSuffixViewModel> _suffixes;
        /// <summary>
        /// 当前节点的后缀配置集合。
        /// </summary>
        public ObservableCollection<AxisElementSuffixViewModel> Suffixes
        {
            get => _suffixes;
            set => SetProperty(ref _suffixes, value);
        }

        ObservableCollection<AxisElementDetailViewModel> _details;
        /// <summary>
        /// 当前节点的详细配置集合。
        /// </summary>
        public ObservableCollection<AxisElementDetailViewModel> Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }
        /// <summary>
        /// 向当前节点中添加子节点。
        /// </summary>
        /// <param name="node"></param>
        public void PushChild(AxisTreeNode node)
        {
            _children.Add(node);
            //var parameter = _axisElement.Template.NewParameter();
            //parameter.SetValue(node.AxisElement);
            //_axisElement.Template.PushParameter(parameter);
            node.Parent = this;
        }
        /// <summary>
        /// 移除当前子节点的最后一个。
        /// </summary>
        public void PopChild()
        {
            if (_children.Count > 0)
            {
                _children.RemoveAt(_children.Count - 1);
                _axisElement.Template.RemoveAt(_axisElement.Template.Count - 1);
            }
        }
        /// <summary>
        /// 移除指定位置的子节点。如果索引存在，将移除成功，返回true，否则返回false。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveChildAt(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                return false;
            }
            _children.RemoveAt(index);
            _axisElement.Template.RemoveAt(index);
            return true;
        }
        /// <summary>
        /// 交换两个索引位置的子节点数据。如果有任意一个索引不存在或者两个索引相同，将返回false；
        /// 否则将成功交换并返回true。
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public bool SwapChildren(int index1, int index2)
        {
            bool result = CollectionHelper.Swap(_children, index1, index2);
            if (result)
            {
                _axisElement.Template.Swap(index1, index2);
            }
            return result;
        }
        /// <summary>
        /// 将指定位置的子节点向前移动一个位置。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool MoveChildUp(int index)
        {
            return SwapChildren(index, index - 1);
        }
        /// <summary>
        /// 将指定位置的子节点向后移动一个位置。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool MoveChildDown(int index)
        {
            return SwapChildren(index, index + 1);
        }

        public IEnumerable<AxisElementSuffixViewModel> GetSelectedSuffix()
        {
            return _suffixes.Where(suffix => suffix.Checked);
        }
        /// <summary>
        /// 将当前的所有配置应用到已有的轴元素中。
        /// </summary>
        /// <param name="element"></param>
        public void ApplyToAxisElement(AxisElement? element)
        {
            if (element == null)
            {
                return;
            }

            element.Template.ElementType = _elementType;
            // Detail
            foreach (var detail in _details)
            {
                detail.ApplyToAxisElement(element);
            }
            // Net / Combine
            if (_elementType == AxisElementType.Net || _elementType == AxisElementType.Combine)
            {
                foreach (var child in _children)
                {
                    var childElement = new AxisElement(element);
                    child.ApplyToAxisElement(childElement);
                    var parameter = element.Template.NewParameter();
                    parameter.SetValue(childElement);
                    element.Template.PushParameter(parameter);
                }
            }
            // Suffix
            foreach (var suffix in _suffixes)
            {
                suffix.ApplyToAxisElement(element);
            }
        }

        void LoadSingleDetail(AxisElementDetailType type, AxisElement element, int indexOfElement = -1)
        {
            var detail = new AxisElementDetailViewModel(type);
            if (indexOfElement > -1)
            {
                detail.IndexOfElement = indexOfElement;
            }

            // 添加需要关联值的事件
            if (type == AxisElementDetailType.Name)
            {
                detail.BeforeValueChanged += ValidateNodeName;
            }
            detail.ValueChanged += OnDetailTextValueChanged;

            // 向节点名称列表中添加当前元素名
            _root.NodeNames.Add(element.Name.ToLower());

            detail.LoadFromAxisElement(element);
            Details.Add(detail);
        }
        /// <summary>
        /// 从已有的轴元素对象（AxisElement）中读取数据。读取前会清空以后的数据。
        /// </summary>
        /// <param name="element"></param>
        public void LoadFromAxisElement(AxisElement element)
        {
            _name = element.Name;
            _elementType = element.Template.ElementType;
            // Detail
            Details.Clear();

            Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Type)
            {
                IsReadOnly = true,
                Text = element.Template.ElementType.ToString()
            });

            switch (_elementType)
            {
                case AxisElementType.InsertFunctionOrVariable:
                    break;

                case AxisElementType.CategoryRange:
                    LoadSingleDetail(AxisElementDetailType.Description, element);
                    LoadSingleDetail(AxisElementDetailType.CategoryLowerBoundary, element, 0);
                    LoadSingleDetail(AxisElementDetailType.CategoryUpperBoundary, element, 1);
                    LoadSingleDetail(AxisElementDetailType.Exclude, element);
                    break;

                // 单个Filter条件
                case AxisElementType.Base:
                case AxisElementType.UnweightedBase:
                case AxisElementType.EffectiveBase:
                case AxisElementType.Expression:
                case AxisElementType.Derived:
                    LoadSingleDetail(AxisElementDetailType.Name, element);
                    LoadSingleDetail(AxisElementDetailType.Description, element);
                    LoadSingleDetail(AxisElementDetailType.Filter, element, 0);
                    LoadSingleDetail(AxisElementDetailType.Exclude, element);
                    break;

                // (Variable, 'Filter')
                case AxisElementType.Numeric:
                case AxisElementType.Mean:
                case AxisElementType.StdErr:
                case AxisElementType.StdDev:
                case AxisElementType.Min:
                case AxisElementType.Max:
                case AxisElementType.Sum:
                case AxisElementType.Median:
                case AxisElementType.Mode:
                    LoadSingleDetail(AxisElementDetailType.Name, element);
                    LoadSingleDetail(AxisElementDetailType.Description, element);
                    LoadSingleDetail(AxisElementDetailType.VariableName, element, 0);
                    LoadSingleDetail(AxisElementDetailType.Filter, element, 1);
                    LoadSingleDetail(AxisElementDetailType.Exclude, element);
                    break;

                case AxisElementType.Percentile:
                    LoadSingleDetail(AxisElementDetailType.Name, element);
                    LoadSingleDetail(AxisElementDetailType.Description, element);
                    LoadSingleDetail(AxisElementDetailType.VariableName, element, 0);
                    LoadSingleDetail(AxisElementDetailType.CutOff, element, 1);
                    LoadSingleDetail(AxisElementDetailType.Filter, element, 2);
                    LoadSingleDetail(AxisElementDetailType.Exclude, element);
                    break;

                case AxisElementType.Net:
                case AxisElementType.Combine:
                case AxisElementType.Category:
                case AxisElementType.Text:
                case AxisElementType.Total:
                case AxisElementType.SubTotal:
                case AxisElementType.None:
                case AxisElementType.Ntd:
                    LoadSingleDetail(AxisElementDetailType.Name, element);
                    LoadSingleDetail(AxisElementDetailType.Description, element);
                    LoadSingleDetail(AxisElementDetailType.Exclude, element);
                    break;

                default:
                    break;
            }
            // Suffix
            foreach (var suffix in Suffixes)
            {
                suffix.LoadFromAxisElement(element.Suffix.Find(s => s.Type.ToString().Equals(suffix.Name)));
            }
            // Template Net / Combine的子元素
            if ((_elementType == AxisElementType.Net || 
                _elementType == AxisElementType.Combine) &&
                element.Template.Count > 0)
            {
                for (int i = 0; i < element.Template.Count; i++)
                {
                    AxisElementParameter parameter = element.Template.GetParameter(i)!;
                    if (parameter.GetValue() is AxisElement subElement &&
                        _root is AxisSettingViewModel viewModel)
                    {
                        var child = new AxisTreeNode(_root, 
                            subElement,
                            (n, v) =>
                            {
                                if (v)
                                {
                                    viewModel.SelectedNode = n;
                                }
                            });
                        //child.LoadFromAxisElement(subElement);
                        PushChild(child);
                    }
                }
            }
        }

    }

}
