﻿using IDCA.Model;
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
        public AxisSettingViewModel() 
        {
            _availableElements = new ObservableCollection<AvailableElement>();
            for (int i = 0; i < _axisDefaultElements.Length; i++)
            {
                var ele = _axisDefaultElements[i];
                var type = (AxisElementType)(i + 1);
                _availableElements.Add(new AvailableElement(ele, type, e => AppendTreeNode(e)));
            }
            _availableSelectedIndex = -1;
            _tree = new ObservableCollection<AxisTreeNode>();
        }

        Axis? _axis;
        public Axis? Axis
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
                _axis?.FromString(value);
            }
        }

        void UpdateAxisExpression()
        {
            ApplyToAxis();
            AxisExpression = _axis?.ToString() ?? "{}";
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
            }

            string _name;
            public string Name
            {
                get { return _name; }
                set { SetProperty(ref _name, value); }
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
                success = CollectionHelper.Swap(_tree, index, moveDown ? index + 1 : index - 1);
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
                }
            }
            else
            {
                int index = _tree.IndexOf(node);
                if (index > -1)
                {
                    _tree.RemoveAt(index);
                }
            }
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
            var node = new AxisTreeNode(this, element.Type, OnSelectedChanged);
            node.AddingChild += AppendTreeElementChildNode;
            node.Removing += RemoveTreeNode;
            node.MovingUp += n => MoveTreeNode(n);
            node.MovingDown += n => MoveTreeNode(n, true);
            node.Name = element.Name;
            if (parentNode != null)
            {
                parentNode.Children.Add(node);
            }
            else
            {
                _tree.Add(node);
            }
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

        void AppendTreeNode()
        {
            if (_availableSelectedIndex < 0 || 
                _availableSelectedIndex >= _availableElements.Count)
            {
                return;
            }

            AxisElementType type = (AxisElementType)(_availableSelectedIndex + 1);
            string name = _availableElements[_availableSelectedIndex].Name;
            var node = new AxisTreeNode(this, type, OnSelectedChanged);
            node.Removing += RemoveTreeNode;
            node.MovingUp += n => MoveTreeNode(n);
            node.MovingDown += n => MoveTreeNode(n, true);
            node.Name = name;
            if (_selectedNode == null || !(
                _selectedNode.ElementType == AxisElementType.Net ||
                _selectedNode.ElementType == AxisElementType.Combine))
            {
                _tree.Add(node);
            }
            else
            {
                _selectedNode.Children.Add(node);
            }
            UpdateAxisExpression();
        }
        public ICommand AppendTreeNodeCommand => new RelayCommand(AppendTreeNode);


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
            foreach (AxisElement element in axis)
            {
                var node = new AxisTreeNode(this);
                node.LoadFromAxisElement(element);
                _tree.Add(node);
            }
        }

    }

    public class AxisElementSuffixViewModel : ObservableObject
    {

        public AxisElementSuffixViewModel()
        {
            _name = string.Empty;
            _text = string.Empty;
            _selectedIndex = 0;
            _selections = new ObservableCollection<string>();
            _checked = false;
            _isTextBox = true;
            _isComboBox = false;
        }

        string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        string _text;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        ObservableCollection<string> _selections;
        public ObservableCollection<string> Selections
        {
            get => _selections;
            set => SetProperty(ref _selections, value);
        }

        bool _checked;
        public bool Checked
        {
            get => _checked;
            set => SetProperty(ref _checked, value);
        }

        bool _isTextBox;
        public bool IsTextBox
        {
            get => _isTextBox;
            set => SetProperty(ref _isTextBox, value);
        }

        bool _isComboBox;
        public bool IsComboBox
        {
            get => _isComboBox;
            set => SetProperty(ref _isComboBox, value);
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
            else if (_selectedIndex > 0 && _selectedIndex < _selections.Count)
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

        AxisElementDetailType _type;
        public AxisElementDetailType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        int _indexOfElement = 0;
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
            set { SetProperty(ref _text, value); }
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
            set { SetProperty(ref _selectedIndex, value); }
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
        public AxisTreeNode(ObservableObject root)
        {
            _root = root;
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
        }

        public AxisTreeNode(ObservableObject root, AxisElementType type, Action<AxisTreeNode, bool>? selectedChanged) : this(root)
        {
            InitDetail(type);
            _elementType = type;
            _selectedChanged = selectedChanged;
            _allowChildren = type == AxisElementType.Combine || type == AxisElementType.Net;
        }

        readonly ObservableObject _root;
        public ObservableObject Root => _root;

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

        void InitSuffixTextBoxItem(AxisElementSuffixType type)
        {
            var element = new AxisElementSuffixViewModel()
            {
                Name = type.ToString(),
                IsComboBox = false,
                IsTextBox = true
            };
            _suffixes.Add(element);
        }

        void InitSuffixSelectionItem(AxisElementSuffixType type, params string[] selections)
        {
            var element = new AxisElementSuffixViewModel()
            {
                Name = type.ToString(),
                IsComboBox = true,
                IsTextBox = false,
            };
            element.AddSelections(selections);
            _suffixes.Add(element);
        }

        public void InitDetail(AxisElementType type)
        {
            switch (type)
            {
                case AxisElementType.InsertFunctionOrVariable:
                    break;

                case AxisElementType.CategoryRange:
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Description));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.CategoryLowerBoundary) { IndexOfElement = 0 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.CategoryUpperBoundary) { IndexOfElement = 1 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Exclude));
                    break;

                // 单个Filter条件
                case AxisElementType.Base:
                case AxisElementType.UnweightedBase:
                case AxisElementType.EffectiveBase:
                case AxisElementType.Expression:
                case AxisElementType.Derived:
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Name));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Description));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Filter) { IndexOfElement = 0 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Exclude));
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
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Name));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Description));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.VariableName) { IndexOfElement = 0 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Filter) { IndexOfElement = 1 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Exclude));
                    break;

                case AxisElementType.Percentile:
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Name));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Description));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.VariableName) { IndexOfElement = 0 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.CutOff) { IndexOfElement = 1 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Filter) { IndexOfElement = 2 });
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Exclude));
                    break;

                case AxisElementType.Net:
                case AxisElementType.Combine:
                case AxisElementType.Category:
                case AxisElementType.Text:
                case AxisElementType.Total:
                case AxisElementType.SubTotal:
                case AxisElementType.None:
                case AxisElementType.Ntd:
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Name));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Description));
                    Details.Add(new AxisElementDetailViewModel(AxisElementDetailType.Exclude));
                    break;

                default:
                    break;
            }
        }

        string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

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
            node.Parent = this;
        }

        public void PopChild()
        {
            if (_children.Count > 0)
            {
                _children.RemoveAt(_children.Count - 1);
            }
        }

        public bool RemoveChildAt(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                return false;
            }
            _children.RemoveAt(index);
            return true;
        }

        public bool SwapChildren(int index1, int index2)
        {
            return CollectionHelper.Swap(_children, index1, index2);
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
            detail.LoadFromAxisElement(element);
            Details.Add(detail);
        }

        public void LoadFromAxisElement(AxisElement element)
        {
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
                            subElement.Template.ElementType,
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
