using IDCA.Model;
using IDCA.Model.Spec;
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
    public class AxisSettingViewModel : ObservableObject
    {
        public AxisSettingViewModel(Axis axis) 
        {
            _axis = axis;
            _availableElements = new ObservableCollection<AvailableElement>();
            for (int i = 0; i < _axisDefaultElements.Length; i++)
            {
                var ele = _axisDefaultElements[i];
                var type = (AxisElementType)(i + 1);
                _availableElements.Add(new AvailableElement(ele, type, e => AppendTreeNode(e)));
            }
            _availableSelectedIndex = -1;
            _tree = new ObservableCollection<AxisTreeNode>();
            _nodeNames = new List<string>();
            LoadFromAxis(Axis);
        }

        Axis _axis;
        public Axis Axis
        {
            get { return _axis; }
        }

        string _axisExpression = string.Empty;
        public string AxisExpression
        {
            get { return _axisExpression; }
            set 
            { 
                SetProperty(ref _axisExpression, value);
                //_axis.FromString(value);
            }
        }



        readonly List<string> _nodeNames;
        public List<string> NodeNames => _nodeNames;

        public void UpdateAxisExpression()
        {
            AxisExpression = _axis.ToString();
        }

        static readonly string[] _axisDefaultElements =
        {
            "Category元素",
            "Category区间",
            "插入变量或函数",
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
            public string Name
            {
                get { return _name; }
                set { SetProperty(ref _name, value); }
            }

            string _scriptName;
            public string ScriptName
            {
                get { return _scriptName; }
                set { _scriptName = value; }
            }

            AxisElementType _type;
            public AxisElementType Type
            {
                get { return _type; }
                set { _type = value; }
            }

            readonly Action<AvailableElement> _adding;
            public ICommand AddCommand => new RelayCommand(() => _adding(this));
        }

        ObservableCollection<AvailableElement> _availableElements;
        public ObservableCollection<AvailableElement> AvailableElements
        {
            get => _availableElements;
            set => SetProperty(ref _availableElements, value);
        }

        int _availableSelectedIndex;
        public int AvailableSelectedIndex
        {
            get => _availableSelectedIndex;
            set => SetProperty(ref _availableSelectedIndex, value);
        }

        AxisTreeNode? _selectedNode;
        public AxisTreeNode? SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
        }

        ObservableCollection<AxisTreeNode> _tree;
        public ObservableCollection<AxisTreeNode> Tree
        {
            get { return _tree; }
            set { SetProperty(ref _tree, value); }
        }

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
                UpdateAxisExpression();
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
            _nodeNames.RemoveAll(n => n.Equals(node.Name, StringComparison.OrdinalIgnoreCase));
            UpdateAxisExpression();
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
            UpdateAxisExpression();
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

        public void ApplyToAxis()
        {
            if (_axis == null)
            {
                return;
            }

            foreach (var node in _tree)
            {
                var element = _axis.NewObject();
                node.ApplyToAxisElement(element);
                _axis.Add(element);
            }
        }


        public void LoadFromAxis(Axis axis)
        {
            _axis = axis;
            _tree.Clear();
            foreach (AxisElement element in axis)
            {
                var node = new AxisTreeNode(this, element, OnSelectedChanged);
                node.LoadFromAxisElement(element);
                node.Removing += RemoveTreeNode;
                node.MovingUp += n => MoveTreeNode(n);
                node.MovingDown += n => MoveTreeNode(n, true);
                _tree.Add(node);
            }
            _axisExpression = axis.ToString();
        }

    }

    public class AxisElementSuffixViewModel : ObservableObject
    {

        public AxisElementSuffixViewModel(AxisElementSuffixType type)
        {
            _type = type;
            _name = string.Empty;
            _text = string.Empty;
            _selectedIndex = 0;
            _selections = new ObservableCollection<string>();
            _checked = false;
            _isTextBox = true;
            _isComboBox = false;
        }

        Action<AxisElementSuffixViewModel>? _checkChanged;
        public event Action<AxisElementSuffixViewModel>? CheckChanged
        {
            add { _checkChanged += value; }
            remove { _checkChanged -= value; }
        }

        Action<AxisElementSuffixViewModel>? _valueChanged;
        public event Action<AxisElementSuffixViewModel>? ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        readonly AxisElementSuffixType _type;
        public AxisElementSuffixType Type => _type;

        string _name;
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
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty(ref _text, value);
                if (_checked)
                {
                    _valueChanged?.Invoke(this);
                }
            }
        }

        int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set 
            { 
                SetProperty(ref _selectedIndex, value);
                if (_checked)
                {
                    _valueChanged?.Invoke(this);
                }
            }
        }

        ObservableCollection<string> _selections;
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
        public bool Checked
        {
            get { return _checked; }
            set 
            { 
                SetProperty(ref _checked, value);
                _checkChanged?.Invoke(this);
            }
        }

        bool _isTextBox;
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

        public void AddSelections(params string[] seletions)
        {
            foreach (var item in seletions)
            {
                _selections.Add(item);
            }
        }

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

        public void LoadFromAxisElement(AxisElementSuffix? suffix)
        {
            if (suffix == null || !_name.Equals(suffix.Type.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Checked = true;
            Text = suffix.Value.ToString() ?? string.Empty;

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

    }

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
        public event Func<AxisElementDetailViewModel, bool> BeforeValueChanged
        {
            add { _beforeValueChanged += value; }
            remove { _beforeValueChanged -= value; }
        }

        Action<AxisElementDetailViewModel>? _valueChanged;
        public event Action<AxisElementDetailViewModel> ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        AxisElementDetailType _type;
        public AxisElementDetailType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        int _indexOfElement = -1;
        public int IndexOfElement
        {
            get { return _indexOfElement; }
            set { _indexOfElement = value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        string _text;
        public string Text
        {
            get { return _text; }
            set 
            {
                if (_beforeValueChanged == null || _beforeValueChanged(this))
                {
                    SetProperty(ref _text, value);
                    _valueChanged?.Invoke(this);
                }
            }
        }

        bool _isTextBox;
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
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set 
            { 
                SetProperty(ref _selectedIndex, value);
                _valueChanged?.Invoke(this);
            }
        }

        ObservableCollection<string> _selections;
        public ObservableCollection<string> Selections
        {
            get { return _selections; }
            set { SetProperty(ref _selections, value); }
        }

        public void SetSelections(params string[] selections)
        {
            foreach (var s in selections)
            {
                _selections.Add(s);
            }
        }

        public string GetValue()
        {
            if (_isComboBox)
            {
                return _selectedIndex > 0 && _selectedIndex < _selections.Count ?
                    _selections[_selectedIndex] : string.Empty;
            }
            else
            {
                return _text;
            }
        }

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
                    SetSelections("true", "false");
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
                    
                    if (_indexOfElement > 0 && _indexOfElement < element.Template.Count)
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

    public class AxisTreeNode : ObservableObject
    {
        public AxisTreeNode(AxisSettingViewModel root, AxisElement element)
        {
            _root = root;
            _axisElement = element;
            _elementType = element.Template.ElementType;
            _allowChildren = false;
            _elementType = AxisElementType.None;
            _name = string.Empty;
            _children = new ObservableCollection<AxisTreeNode>();
            _suffixes = new ObservableCollection<AxisElementSuffixViewModel>();
            _details = new ObservableCollection<AxisElementDetailViewModel>();
            // 填入初始化的后缀配置
            // CalculationScope=AllElements|PrecedingElements
            InitSuffixSelectionItem(AxisElementSuffixType.CalculationScope, "AllElements", "PrecedingElements");
            // CountsOnly=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.CountsOnly, "true", "false");
            // Decimals=DecimalPlaces
            InitSuffixTextBoxItem(AxisElementSuffixType.Decimals);
            // Factor=FactorValue
            InitSuffixTextBoxItem(AxisElementSuffixType.Factor);
            // IsFixed=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsFixed, "true", "false");
            // IsHidden=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsHidden, "true", "false");
            // IsHiddenWhenColumn = True | False
            InitSuffixSelectionItem(AxisElementSuffixType.IsHiddenWhenColumn, "true", "false");
            // IsHiddenWhenRow=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsHiddenWhenRow, "true", "false");
            // IncludeInBase=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IncludeInBase, "true", "false");
            // IsUnweighted=True|False
            InitSuffixSelectionItem(AxisElementSuffixType.IsUnweighted, "true", "false");
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
        public AxisElement AxisElement => _axisElement;

        readonly AxisSettingViewModel _root;
        public AxisSettingViewModel Root => _root;

        Action<AxisTreeNode, bool>? _selectedChanged;
        public Action<AxisTreeNode, bool>? SelectedChanged
        {
            get => _selectedChanged;
            set => _selectedChanged = value;
        }

        AxisElementType _elementType;
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
        public event Action<AxisTreeNode>? Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        Action<AxisTreeNode>? _movingUp;
        public event Action<AxisTreeNode>? MovingUp
        {
            add { _movingUp += value; }
            remove { _removing -= value; }
        }

        Action<AxisTreeNode>? _movingDown;
        public event Action<AxisTreeNode>? MovingDown
        {
            add { _movingDown += value; }
            remove { _movingDown -= value; }
        }

        Action<AxisTreeNode>? _addingChild;
        public event Action<AxisTreeNode>? AddingChild
        {
            add { _addingChild += value; }
            remove { _addingChild -= value; }
        }

        Action<AxisTreeNode>? _renamed;
        public event Action<AxisTreeNode>? Renamed
        {
            add { _renamed += value; }
            remove { _renamed -= value; }
        }

        bool _allowChildren;
        public bool AllowChildren
        {
            get { return _allowChildren; }
            set { SetProperty(ref _allowChildren, value); }
        }

        void AddChild()
        {
            _addingChild?.Invoke(this);
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

        void OnSuffixChecked(AxisElementSuffixViewModel viewModel)
        {
            var existSuffix = _axisElement.Suffix[viewModel.Type];
            if (viewModel.Checked)
            {
                if (existSuffix != null)
                {
                    existSuffix.Value = viewModel.GetValue();
                }
                else
                {
                    var suffix = _axisElement.Suffix.Append(viewModel.Type);
                    suffix.Value = viewModel.GetValue();
                }
            }
            else
            {
                if (existSuffix != null)
                {
                    _axisElement.Suffix.RemoveIf(e => e.Type == viewModel.Type);
                }
            }
            _root.UpdateAxisExpression();
        }

        void OnSuffixValueChanged(AxisElementSuffixViewModel viewModel)
        {
            if (viewModel.Checked)
            {
                var suffix = _axisElement.Suffix[viewModel.Type];
                if (suffix != null)
                {
                    suffix.Value = viewModel.GetValue();
                    _root.UpdateAxisExpression();
                }
            }
        }

        void InitSuffixTextBoxItem(AxisElementSuffixType type)
        {
            var element = new AxisElementSuffixViewModel(type)
            {
                Name = type.ToString(),
                IsComboBox = false,
                IsTextBox = true
            };
            element.CheckChanged += OnSuffixChecked;
            element.ValueChanged += OnSuffixValueChanged;
            _suffixes.Add(element);
        }

        void InitSuffixSelectionItem(AxisElementSuffixType type, params string[] selections)
        {
            var element = new AxisElementSuffixViewModel(type)
            {
                Name = type.ToString(),
                IsComboBox = true,
                IsTextBox = false,
            };
            element.AddSelections(selections);
            element.CheckChanged += OnSuffixChecked;
            element.ValueChanged += OnSuffixValueChanged;
            _suffixes.Add(element);
        }

        bool ValidateNodeName(AxisElementDetailViewModel detail)
        {
            return !Root.NodeNames.Exists(n => n.Equals(detail.Name, StringComparison.OrdinalIgnoreCase));
        }

        void OnDetailTextValueChanged(AxisElementDetailViewModel viewModel)
        {
            switch (viewModel.Type)
            {
                case AxisElementDetailType.Name:
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
            _root.UpdateAxisExpression();
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

        public void InitDetail(AxisElementType type)
        {
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
        public string Name 
        { 
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                _axisElement.Name = value;
                _renamed?.Invoke(this);
            }
        }

        ObservableCollection<AxisTreeNode> _children;
        public ObservableCollection<AxisTreeNode> Children
        {
            get { return _children; }
            set { SetProperty(ref _children, value); }
        }

        ObservableCollection<AxisElementSuffixViewModel> _suffixes;
        public ObservableCollection<AxisElementSuffixViewModel> Suffixes
        {
            get => _suffixes;
            set => SetProperty(ref _suffixes, value);
        }

        ObservableCollection<AxisElementDetailViewModel> _details;
        public ObservableCollection<AxisElementDetailViewModel> Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }

        public void PushChild(AxisTreeNode node)
        {
            _children.Add(node);
            var parameter = _axisElement.Template.NewParameter();
            parameter.SetValue(node.AxisElement);
            _axisElement.Template.PushParameter(parameter);
            node.Parent = this;
        }

        public void PopChild()
        {
            if (_children.Count > 0)
            {
                _children.RemoveAt(_children.Count - 1);
                _axisElement.Template.RemoveAt(_axisElement.Template.Count - 1);
            }
        }

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

        public bool SwapChildren(int index1, int index2)
        {
            bool result = CollectionHelper.Swap(_children, index1, index2);
            if (result)
            {
                _axisElement.Template.Swap(index1, index2);
            }
            return result;
        }

        public bool MoveChildUp(int index)
        {
            return SwapChildren(index, index - 1);
        }

        public bool MoveChildDown(int index)
        {
            return SwapChildren(index, index + 1);
        }

        public IEnumerable<AxisElementSuffixViewModel> GetSelectedSuffix()
        {
            return _suffixes.Where(suffix => suffix.Checked);
        }

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
            _root.NodeNames.Add(element.Name);

            detail.LoadFromAxisElement(element);
            Details.Add(detail);
        }

        public void LoadFromAxisElement(AxisElement element)
        {
            _name = element.Name;
            _elementType = element.Template.ElementType;
            // Detail
            Details.Clear();
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
                        child.LoadFromAxisElement(subElement);
                        PushChild(child);
                    }
                }
            }
        }

    }

}
