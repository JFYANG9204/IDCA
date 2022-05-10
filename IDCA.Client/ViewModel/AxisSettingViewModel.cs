using IDCA.Model;
using IDCA.Model.Spec;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class AxisSettingViewModel : ObservableObject
    {
        public AxisSettingViewModel() 
        {
        }


        string _axisExpression = string.Empty;
        public string AxisExpression
        {
            get { return _axisExpression; }
            set { SetProperty(ref _axisExpression, value); }
        }

        static readonly string[] _axisDefaultElements =
        {
            "所有Category元素(..)",
            "插入变量或函数",
            "文本行(text)",
            "基数(base)",
            "未加权的基数(unweightedbase)",
            "有效的基数(effectivebase)",
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

        ObservableCollection<string> _axisAvalibleElements = new(_axisDefaultElements);
        public ObservableCollection<string> AxisAvalibleElements
        {
            get { return _axisAvalibleElements; }
            set { SetProperty(ref _axisAvalibleElements, value); }
        }

        ObservableCollection<string> _axisCurrentElements = new();
        public ObservableCollection<string> AxisCurrentElements
        {
            get { return _axisCurrentElements; }
            set { SetProperty(ref _axisCurrentElements, value); }
        }

        int _axisAvalibleElementsSelectIndex = 0;
        public int AxisAvalibleElementsSelectIndex
        {
            get { return _axisAvalibleElementsSelectIndex; }
            set { SetProperty(ref _axisAvalibleElementsSelectIndex, value); }
        }

        int _axisCurrentElementsSelectIndex = 0;
        public int AxisCurrentElementsSelectIndex
        {
            get { return _axisCurrentElementsSelectIndex; }
            set { 
                SetProperty(ref _axisCurrentElementsSelectIndex, value);
                UpdateElementSettings();
            }
        }

        readonly List<ObservableCollection<AxisElementDetailSettingViewModel>> _axisElementDetailList = new();
        readonly List<ObservableCollection<AxisTailingElementSettingViewModel>> _axisTailingElementList = new();

        void UpdateElementSettings()
        {
            if (_axisCurrentElementsSelectIndex < 0 ||
                _axisCurrentElementsSelectIndex >= _axisTailingElementList.Count ||
                _axisCurrentElementsSelectIndex >= _axisElementDetailList.Count)
            {
                return;
            }
            CurrentAxisTailingElementSetting = _axisTailingElementList[_axisCurrentElementsSelectIndex];
            CurrentAxisElementDetailSetting = _axisElementDetailList[_axisCurrentElementsSelectIndex];
        }

        ObservableCollection<AxisTailingElementSettingViewModel> _currentAxisTailingElementSetting = new();
        public ObservableCollection<AxisTailingElementSettingViewModel> CurrentAxisTailingElementSetting
        {
            get { return _currentAxisTailingElementSetting; }
            set { SetProperty(ref _currentAxisTailingElementSetting, value); }
        }

        ObservableCollection<AxisElementDetailSettingViewModel> _currentAxisElementDetailSetting = new();
        public ObservableCollection<AxisElementDetailSettingViewModel> CurrentAxisElementDetailSetting
        {
            get { return _currentAxisElementDetailSetting; }
            set { SetProperty(ref _currentAxisElementDetailSetting, value); }
        }

        Axis? _axis = null;

        public void InitAxis(Axis axis)
        {
            _axis = axis;
        }

        public Axis? Axis => _axis;

        void UpdateAxisText()
        {
            if (_axis != null)
            {
                AxisExpression = _axis.ToString();
            }
        }

        void AppendAxisElement()
        {
            if (_axis == null)
            {
                return;
            }

            switch (_axisAvalibleElementsSelectIndex)
            {
                case 0:  _axis.AppendAllCategory();           break;
                case 1:  _axis.AppendInsertFunction();        break;
                case 2:  _axis.AppendTextElement();           break;
                case 3:  _axis.AppendBaseElement();           break;
                case 4:  _axis.AppendUnweightedBaseElement(); break;
                case 5:  _axis.AppendEffectiveBaseElement();  break;
                case 6:  _axis.AppendExpression();            break;
                case 7:  _axis.AppendNumeric();               break;
                case 8:  _axis.AppendDerived();               break;
                case 9:  _axis.AppendMean();                  break;
                case 10: _axis.AppendStdErr();                break;
                case 11: _axis.AppendStdDev();                break;
                case 12: _axis.AppendTotal();                 break;
                case 13: _axis.AppendSubTotal();              break;
                case 14: _axis.AppendMin();                   break;
                case 15: _axis.AppendMax();                   break;
                case 16: _axis.AppendNet();                   break;
                case 17: _axis.AppendCombine();               break;
                case 18: _axis.AppendSum();                   break;
                case 19: _axis.AppendMedian();                break;
                case 20: _axis.AppendPercentile();            break;
                case 21: _axis.AppendMode();                  break;
                case 22: _axis.AppendNtd();                   break;
                default:                                      break;
            }
        }

        void InitSingleTailingElement(string label, Type type, AxisElementSuffixType suffixType, params string[] values)
        {
            AxisTailingElementSettingViewModel element = new();
            element.TailingElementLabel = label;
            element.ValueType = type;
            element.Type = suffixType;
            foreach (var v in values)
            {
                element.TailingElementValue.Add(v);
            }
            CurrentAxisTailingElementSetting.Add(element);
        }

        void InitTailingElements()
        {
            InitSingleTailingElement("CalculationScope", typeof(AxisElementSuffixCalculationScope), AxisElementSuffixType.CalculationScope, "AllElements", "PrecedingElements");
            InitSingleTailingElement("CountsOnly", typeof(bool), AxisElementSuffixType.CountsOnly);
            InitSingleTailingElement("Decimals", typeof(int), AxisElementSuffixType.Decimals);
            InitSingleTailingElement("Factor", typeof(double), AxisElementSuffixType.Factor);
            InitSingleTailingElement("IsFixed", typeof(bool), AxisElementSuffixType.IsFixed);
            InitSingleTailingElement("IsHidden", typeof(bool), AxisElementSuffixType.IsHidden);
            InitSingleTailingElement("IsHiddenWhenColumn", typeof(bool), AxisElementSuffixType.IsHiddenWhenColumn);
            InitSingleTailingElement("IsHiddenWhenRow", typeof(bool), AxisElementSuffixType.IsHiddenWhenRow);
            InitSingleTailingElement("IncludeInBase", typeof(bool), AxisElementSuffixType.IncludeInBase);
            InitSingleTailingElement("IsUnweighted", typeof(bool), AxisElementSuffixType.IsUnweighted);
            InitSingleTailingElement("Multiplier", typeof(string), AxisElementSuffixType.Multiplier);
            InitSingleTailingElement("Weight", typeof(string), AxisElementSuffixType.Weight);
        }

        void InitSingleDetailElement(string label, bool canSelect)
        {
            AxisElementDetailSettingViewModel element = new()
            {
                Label = label,
                CanSelectVariable = canSelect
            };
            CurrentAxisElementDetailSetting.Add(element);
        }

        void InitLabeledElement()
        {
            InitSingleDetailElement("标签", false);
        }

        void InitDetailElements()
        {
            AppendAxisElement();
            switch (_axisAvalibleElementsSelectIndex)
            {
                case 0:
                    InitSingleDetailElement("排除码号", false);
                    break;
                case 1:
                    break;
                case 2:
                case 12:
                case 13:
                case 22:
                    InitLabeledElement();
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 8:
                    InitLabeledElement();
                    InitSingleDetailElement("筛选器条件", false);
                    break;
                case 7:
                case 9:
                case 10:
                case 11:
                case 14:
                case 15:
                case 18:
                case 19:
                case 21:
                    InitLabeledElement();
                    InitSingleDetailElement("数值变量名", true);
                    InitSingleDetailElement("筛选器条件", false);
                    break;
                case 16:
                case 17:
                    InitLabeledElement();
                    InitSingleDetailElement("码号", false);
                    break;
                case 20:
                    InitLabeledElement();
                    InitSingleDetailElement("数值变量名", true);
                    InitSingleDetailElement("临界值", false);
                    InitSingleDetailElement("筛选器条件", false);
                    break;
                default:
                    break;
            }
        }

        void AddAndInitDetailElements()
        {
            _axisElementDetailList.Add(new());
            CurrentAxisElementDetailSetting = _axisElementDetailList[^1];
            InitDetailElements();
        }

        void AddAndInitTailingElements()
        {
            _axisTailingElementList.Add(new());
            CurrentAxisTailingElementSetting = _axisTailingElementList[^1];
            InitTailingElements();
        }

        void AddElement()
        {
            if (_axisAvalibleElementsSelectIndex >= 0 &&
                _axisAvalibleElementsSelectIndex < _axisAvalibleElements.Count)
            {
                AxisCurrentElements.Add(_axisAvalibleElements[_axisAvalibleElementsSelectIndex]);
                AxisCurrentElementsSelectIndex = AxisCurrentElements.Count - 1;
                AddAndInitDetailElements();
                AddAndInitTailingElements();
                UpdateAxisText();
            }
        }
        public ICommand AddElementCommand => new RelayCommand(AddElement);

        void RemoveElement()
        {
            if (_axisCurrentElementsSelectIndex >= 0 && 
                _axisCurrentElements.Count > 0 &&
                _axisCurrentElementsSelectIndex < _axisCurrentElements.Count)
            {
                _axisCurrentElements.RemoveAt(_axisCurrentElementsSelectIndex);
                _axis?.RemoveAt(_axisCurrentElementsSelectIndex);
                _axisTailingElementList.RemoveAt(_axisCurrentElementsSelectIndex);
                _axisElementDetailList.RemoveAt(_axisCurrentElementsSelectIndex);
                UpdateAxisText();
            }
        }
        public ICommand RemoveElementCommand => new RelayCommand(RemoveElement);

        public void Swap(int sourceIndex, int targetIndex)
        {
            CollectionHelper.Swap(_axisCurrentElements, sourceIndex, targetIndex);
            CollectionHelper.Swap(_axisTailingElementList, sourceIndex, targetIndex);
            CollectionHelper.Swap(_axisElementDetailList, sourceIndex, targetIndex);
            _axis?.Swap(sourceIndex, targetIndex);
        }

        void MoveUp()
        {
            Swap(_axisCurrentElementsSelectIndex, _axisCurrentElementsSelectIndex - 1);
            _axisCurrentElementsSelectIndex--;
            UpdateAxisText();
        }
        public ICommand MoveUpCommand => new RelayCommand(MoveUp);

        void MoveDown()
        {
            Swap(_axisCurrentElementsSelectIndex, _axisCurrentElementsSelectIndex + 1);
            _axisCurrentElementsSelectIndex++;
            UpdateAxisText();
        }
        public ICommand MoveDownCommand => new RelayCommand(MoveDown);

    }

    public class AxisTailingElementSettingViewModel : ObservableObject
    {
        public AxisTailingElementSettingViewModel() { }

        AxisElementSuffixType _type = AxisElementSuffixType.None;
        public AxisElementSuffixType Type
        {
            get => _type;
            set => _type = value;
        }

        bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }

        string _tailingElementLabel = string.Empty;
        public string TailingElementLabel
        {
            get { return _tailingElementLabel; }
            set { SetProperty(ref _tailingElementLabel, value); }
        }

        ObservableCollection<string> _tailingElementValue = new();
        public ObservableCollection<string> TailingElementValue
        {
            get { return _tailingElementValue; }
            set { SetProperty(ref _tailingElementValue, value); }
        }

        int _tailingElementValueSelectedIndex = 0;
        public int TailingElementValueSelectedIndex
        {
            get { return _tailingElementValueSelectedIndex; }
            set { SetProperty(ref _tailingElementValueSelectedIndex, value); }
        }

        string _textValue = string.Empty;
        public string TextValue
        {
            get { return _textValue; }
            set { SetProperty(ref _textValue, value); }
        }

        Visibility _textBoxVisiblity = Visibility.Hidden;
        public Visibility TextBoxVisiblity
        {
            get { return _textBoxVisiblity; }
            set { SetProperty(ref _textBoxVisiblity, value); }
        }

        Visibility _comboBoxVisibility = Visibility.Visible;
        public Visibility ComboBoxVisibility
        {
            get { return _comboBoxVisibility; }
            set { SetProperty(ref _comboBoxVisibility, value); }
        }

        bool _isTextBoxValue = false;
        public bool IsTextBoxValue
        {
            get { return _isTextBoxValue; }
            set 
            { 
                if (value)
                {
                    TextBoxVisiblity = Visibility.Visible;
                    ComboBoxVisibility = Visibility.Hidden;
                }
                else
                {
                    TextBoxVisiblity = Visibility.Hidden;
                    ComboBoxVisibility = Visibility.Visible;
                }
                _isTextBoxValue = value;
            }
        }

        Type _valueType = typeof(bool);
        public Type ValueType
        {
            get { return _valueType; }
            set
            {
                _valueType = value;
                if (_valueType.Equals(typeof(bool)))
                {
                    TailingElementValue.Add("true");
                    TailingElementValue.Add("false");
                }
                else if (_valueType.Equals(typeof(string)) || _valueType.Equals(typeof(int)) || _valueType.Equals(typeof(double)))
                {
                    IsTextBoxValue = true;
                }
            }
        }

        public void InitFromObject(AxisElementSuffix? suffixObject)
        {
            if (suffixObject == null)
            {
                return;
            }

            switch (suffixObject.Type)
            {
                case AxisElementSuffixType.None:
                    break;
                case AxisElementSuffixType.CalculationScope:
                    IsTextBoxValue = false;
                    TailingElementValue.Clear();
                    TailingElementValue.Add("AllElements");
                    TailingElementValue.Add("PrecedingElements");
                    ValueType = typeof(AxisElementSuffixCalculationScope);
                    if (suffixObject.Value is AxisElementSuffixCalculationScope scope)
                    {
                        if (scope == AxisElementSuffixCalculationScope.AllElements)
                        {
                            TailingElementValueSelectedIndex = 0;
                        }
                        else
                        {
                            TailingElementValueSelectedIndex = 1;
                        }
                    }
                    else
                    {
                        TailingElementValueSelectedIndex = 0;
                    }
                    break;
                case AxisElementSuffixType.CountsOnly:
                case AxisElementSuffixType.IsFixed:
                case AxisElementSuffixType.IsHidden:
                case AxisElementSuffixType.IsHiddenWhenColumn:
                case AxisElementSuffixType.IsHiddenWhenRow:
                case AxisElementSuffixType.IncludeInBase:
                case AxisElementSuffixType.IsUnweighted:
                    IsTextBoxValue = false;
                    TailingElementValue.Clear();
                    TailingElementValue.Add("true");
                    TailingElementValue.Add("false");
                    ValueType = typeof(bool);
                    if (suffixObject.Value is bool b)
                    {
                        TailingElementValueSelectedIndex = b ? 0 : 1;
                    }
                    else
                    {
                        TailingElementValueSelectedIndex = 0;
                    }
                    break;
                case AxisElementSuffixType.Decimals:
                    IsTextBoxValue = true;
                    ValueType = typeof(int);
                    TextValue = suffixObject.Value.ToString() ?? "";
                    break;
                case AxisElementSuffixType.Factor:
                    IsTextBoxValue = true;
                    ValueType = typeof(double);
                    TextValue = suffixObject.Value.ToString() ?? "";
                    break;
                case AxisElementSuffixType.Multiplier:
                case AxisElementSuffixType.Weight:
                    IsTextBoxValue = true;
                    ValueType = typeof(string);
                    TextValue = suffixObject.Value.ToString() ?? "";
                    break;
                default:
                    break;
            }
        }

        public void UpdateToObject(AxisElementSuffix suffixObject)
        {
            if (Type != suffixObject.Type)
            {
                return;
            }

            if (Type.Equals(typeof(bool)))
            {
                suffixObject.Value = TailingElementValueSelectedIndex == 0;
            }
            else if (Type.Equals(typeof(AxisElementSuffixCalculationScope)))
            {
                suffixObject.Value = TailingElementValueSelectedIndex == 0 ? "AllElements" : "PrecedingElements";
            }
            else if (Type.Equals(typeof(int)) && IsTextBoxValue && int.TryParse(TextValue, out int iValue))
            {
                suffixObject.Value = iValue;
            }
            else if (Type.Equals(typeof(double)) && IsTextBoxValue && double.TryParse(TextValue, out double dValue))
            {
                suffixObject.Value = dValue;
            }
        }

    }

    public class AxisElementDetailSettingViewModel : ObservableObject
    {

        public AxisElementDetailSettingViewModel() { }

        string _label = "";
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        string _value = "";
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        bool _canSelectVariable = false;
        public bool CanSelectVariable
        {
            get => _canSelectVariable;
            set => _canSelectVariable = value;
        }

    }


}
