using IDCA.Model.Spec;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{

    /// <summary>
    /// VariableSettingWindow窗口使用的后台数据，需要绑定Metadata对象，修改窗口
    /// 内容时会同步修改Metadata对象数据。
    /// </summary>
    public class VariableSettingViewModel : ObservableObject
    {

        public VariableSettingViewModel(Metadata metadata)
        {
            _metadata = metadata;
            _name = metadata.Name;
            // 属性初始化
            _properties = new ObservableCollection<MetadataPropertyViewModel>();
            foreach (var property in metadata.Properties)
            {
                _properties.Add(new MetadataPropertyViewModel(property));
            }
            // Category初始化
            _categories = new ObservableCollection<MetadataCategoryViewModel>();
            foreach (var category in metadata.Categories)
            {
                _categories.Add(new MetadataCategoryViewModel(category));
            }
            // Fields
            _fields = new ObservableCollection<VariableSettingViewModel>();
            _fieldTypeSelectedIndex = 0;
            _fieldTypeSelectedItem = "";
        }

        readonly Metadata _metadata;
        /// <summary>
        /// 当前窗口修改的元数据对象
        /// </summary>
        public Metadata Metadata => _metadata;

        Action<VariableSettingViewModel>? _removing;
        /// <summary>
        /// 移除当前所有配置时触发的事件，用于父级对象删除此Metadata类的配置信息
        /// </summary>
        public event Action<VariableSettingViewModel> Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        Func<string, bool>? _beforeRenamed;
        /// <summary>
        /// 修改变量名前验证名称是否有效的回调
        /// 如果此事件为null或者返回true，将可以成功修改名称；
        /// 如果此事件返回false，将修改失败。
        /// </summary>
        public event Func<string, bool> BeforeRenamed
        {
            add { _beforeRenamed += value; }
            remove { _beforeRenamed -= value; }
        }

        Action<VariableSettingViewModel>? _renamed;
        /// <summary>
        /// 成功修改变量名后触发的事件，此事件应该由创建此对象的父级对象配置
        /// </summary>
        public event Action<VariableSettingViewModel> Renamed
        {
            add { _renamed += value; }
            remove { _renamed -= value; }
        }

        string _name;
        /// <summary>
        /// 当前的变量名，修改前会调用BeforeRenamed回调判断传入值是否可用。
        /// 如果BeforeRenamed未定义或者返回true，将可以成功修改。
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_beforeRenamed == null || _beforeRenamed(value))
                {
                    SetProperty(ref _name, value);
                    _metadata.Name = value;
                    _renamed?.Invoke(this);
                }
            }
        }

        string[] _fieldTypes =
        {
            "Long",
            "Double",
            "Text",
            "Info",
            "Categorical",
        };
        /// <summary>
        /// 当前可用的变量类型
        /// </summary>
        public string[] FieldTypes
        {
            get { return _fieldTypes; }
            set { SetProperty(ref _fieldTypes, value); }
        }

        string _fieldTypeSelectedItem;
        /// <summary>
        /// Field变量类型的选中项
        /// </summary>
        public string FieldTypeSelectedItem
        {
            get { return _fieldTypeSelectedItem; }
            set { SetProperty(ref _fieldTypeSelectedItem, value); }
        }

        int _fieldTypeSelectedIndex;
        /// <summary>
        /// 当前选中的变量类型索引
        /// </summary>
        public int FieldTypeSelectedIndex
        {
            get { return _fieldTypeSelectedIndex; }
            set 
            { 
                SetProperty(ref _fieldTypeSelectedIndex, value);
                _metadata.Type = (MetadataType)_fieldTypeSelectedIndex;
            }
        }

        ObservableCollection<MetadataPropertyViewModel> _properties;
        /// <summary>
        /// 当前Metadata对应的所有属性配置集合
        /// </summary>
        public ObservableCollection<MetadataPropertyViewModel> Properties
        {
            get { return _properties; }
            set { SetProperty(ref _properties, value); }
        }

        /// <summary>
        /// 判断修改的属性名称是否可用，可用返回true，不可用返回false。
        /// 此方法用于MetadataPropertyViewModel回调使用。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ValidatePropertyName(string name)
        {
            return _metadata.GetProperty(name) == null && !string.IsNullOrEmpty(name);
        }

        /// <summary>
        /// 移除属性配置，如果给定的值不在当前列表中，将不做任何操作
        /// </summary>
        /// <param name="property"></param>
        void RemoveProperty(MetadataPropertyViewModel property)
        {
            _properties.Remove(property);
            _metadata.RemoveProperty(property.Name);
        }

        /// <summary>
        /// 创建新的Property，新的对象初始默认命名，需要后续手动修改
        /// </summary>
        public void NewProperty()
        {
            string propName = "Property1";
            int count = 1;
            while (!ValidatePropertyName(propName))
            {
                propName = $"Property{++count}";
            }
            var metadataProperty = _metadata.NewProperty(propName);
            var vm = new MetadataPropertyViewModel(metadataProperty);
            vm.BeforeRenamed += ValidatePropertyName;
            vm.Removing += RemoveProperty;
            _properties.Add(vm);
        }
        /// <summary>
        /// 用于响应添加新Property按钮操作的命令
        /// </summary>
        public ICommand NewPropertyCommand => new RelayCommand(NewProperty);

        // Categorical

        ObservableCollection<MetadataCategoryViewModel> _categories;
        /// <summary>
        /// 当前所有MetadataCategory类型对象的配置
        /// </summary>
        public ObservableCollection<MetadataCategoryViewModel> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }
        /// <summary>
        /// 移除指定的Categorical类型配置，如果不存在，将不做任何操作
        /// </summary>
        /// <param name="categorical"></param>
        void RemoveCategorical(MetadataCategoryViewModel categorical)
        {
            _categories.Remove(categorical);
            _metadata.RemoveCategorical(categorical.Name);
        }

        MetadataCategoryViewModel? _selectedCategory;
        /// <summary>
        /// 当前选中的Category元素
        /// </summary>
        public MetadataCategoryViewModel? SelectedCategory
        {
            get { return _selectedCategory; }
            set { SetProperty(ref _selectedCategory, value); }
        }

        /// <summary>
        /// 添加新的Categorical类型配置进当前列表，用于添加命令的响应
        /// </summary>
        void NewCategorical()
        {
            var vm = new MetadataCategoryViewModel(_metadata.NewCategorical());
            vm.Removing += RemoveCategorical;
            _categories.Add(vm);
        }
        public ICommand NewCategoricalCommand => new RelayCommand(NewCategorical);

        // Parent Node

        VariableSettingViewModel? _parent;
        /// <summary>
        /// 此对象的父级对象，当此对象为Top[..].Slice或Top[..].Sub[..].Slice中的Sub或Slice时，
        /// 此属性应当不为null，如果为Top，此属性为null
        /// </summary>
        public VariableSettingViewModel? Parent
        {
            get => _parent;
            set => _parent = value;
        }

        // Sub Fields
        ObservableCollection<VariableSettingViewModel> _fields;
        /// <summary>
        /// 当前对象的下一级对象
        /// </summary>
        public ObservableCollection<VariableSettingViewModel> Fields
        {
            get { return _fields; }
            set { SetProperty(ref _fields, value); }
        }

        VariableSettingViewModel? _selectedField;
        /// <summary>
        /// 下级变量树中选中的节点
        /// </summary>
        public VariableSettingViewModel? SelectedField
        {
            get { return _selectedField; }
            set { SetProperty(ref _selectedField, value); }
        }

        Action<VariableSettingViewModel, bool>? _selected;
        /// <summary>
        /// 当此节点选中时触发的事件
        /// </summary>
        public event Action<VariableSettingViewModel, bool>? Selected
        {
            add { _selected += value; }
            remove { _selected -= value; }
        }

        bool _isSelected = false;
        /// <summary>
        /// 当前节点是否已选中
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            { 
                SetProperty(ref _isSelected, value);
                _selected?.Invoke(this, value);
            }
        }

        void OnFieldSelected(VariableSettingViewModel vm, bool selected)
        {
            if (selected)
            {
                SelectedField = vm;
            }
        }

        /// <summary>
        /// 判断变量名是否可用，如果可用返回true，不可用返回false
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ValidateFieldName(string name)
        {
            return !_fields.Where(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Any();
        }
        /// <summary>
        /// 此方法用于下级变量的回调。
        /// 当下级变量触发删除时间时，将执行此方法，删除集合中的对应数据。
        /// </summary>
        /// <param name="viewModel"></param>
        void RemoveField(VariableSettingViewModel viewModel)
        {
            _fields.Remove(viewModel);
            _metadata.RemoveField(viewModel.Name);
            if (_fields.Count == 0)
            {
                if (_metadata.Type == MetadataType.CategoricalLoop)
                {
                    _metadata.Type = MetadataType.Categorical;
                }
                else if (_metadata.Type == MetadataType.NumericLoop)
                {
                    _metadata.Type = MetadataType.Long;
                }
            }
        }

        void NewField()
        {
            string name = "Slice";
            int count = 1;
            while (!ValidateFieldName(name))
            {
                name = $"Slice{++count}";
            }
            var field = _metadata.NewField(MetadataType.Categorical, name);
            var vm = new VariableSettingViewModel(field);
            vm.BeforeRenamed += ValidateFieldName;
            vm.Removing += RemoveField;
            if (_selected == null)
            {
                vm.Selected += OnFieldSelected;
            }
            else
            {
                vm.Selected += _selected;
            }
            _fields.Add(vm);
            if (_parent != null)
            {
                if (_parent.Metadata.Type == MetadataType.Categorical)
                {
                    _parent.Metadata.Type = MetadataType.CategoricalLoop;
                }
                else if (_parent.Metadata.Type == MetadataType.Long)
                {
                    _parent.Metadata.Type = MetadataType.NumericLoop;
                }
            }
        }
        /// <summary>
        /// 向当前元数据中追加新的下级变量时执行的命令
        /// </summary>
        public ICommand NewFieldCommand => new RelayCommand(NewField);

        void RemoveSelf()
        {
            _removing?.Invoke(this);
        }
        /// <summary>
        /// 移除当前对象自身时执行的命令，回调应该由父级对象提供。
        /// </summary>
        public ICommand RemoveSelfCommand => new RelayCommand(RemoveSelf);

    }

    /// <summary>
    /// VariableSettingViewModel使用的Categorical类型详细配置数据，
    /// 此配置修改时，会同步修改Metadata中的数据内容。
    /// </summary>
    public class MetadataCategoryViewModel : ObservableObject
    {

        public MetadataCategoryViewModel(MetadataCategorical categorical)
        {
            _categorical = categorical;

            _name = categorical.Name;
            _description = categorical.Description ?? string.Empty;
            _properties = new ObservableCollection<MetadataPropertyViewModel>();
            // 初始化Property对象
            foreach (var property in categorical.Properties)
            {
                var vm = new MetadataPropertyViewModel(property);
                vm.BeforeRenamed += ValidatePropertyName;
                _properties.Add(vm);
            }
            // Suffix
            _suffixViewModels = new ObservableCollection<MetadataCategoricalSuffixViewModel>
            {
                new MetadataCategoricalSuffixViewModel(_categorical, MetadataCategoricalSuffixType.ElementType),
                new MetadataCategoricalSuffixViewModel(_categorical, MetadataCategoricalSuffixType.Exclusive),
                new MetadataCategoricalSuffixViewModel(_categorical, MetadataCategoricalSuffixType.Expression),
                new MetadataCategoricalSuffixViewModel(_categorical, MetadataCategoricalSuffixType.Factor),
                new MetadataCategoricalSuffixViewModel(_categorical, MetadataCategoricalSuffixType.Fix),
                new MetadataCategoricalSuffixViewModel(_categorical, MetadataCategoricalSuffixType.Keycode),
                new MetadataCategoricalSuffixViewModel(_categorical, MetadataCategoricalSuffixType.NoFilter)
            };

        }

        readonly MetadataCategorical _categorical;
        /// <summary>
        /// 当前修改的元数据分类选项对象
        /// </summary>
        public MetadataCategorical Categorical => _categorical;

        ObservableCollection<MetadataCategoricalSuffixViewModel> _suffixViewModels;
        /// <summary>
        /// 当前Categorical对象的后缀对象
        /// </summary>
        public ObservableCollection<MetadataCategoricalSuffixViewModel> Suffixes
        {
            get { return _suffixViewModels; }
            set { SetProperty(ref _suffixViewModels, value); }
        }

        Action<MetadataCategoryViewModel>? _removing;
        /// <summary>
        /// 此对象移除时触发的事件，用于配合父级对象删除集合中的对象。
        /// </summary>
        public event Action<MetadataCategoryViewModel> Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        void Remove()
        {
            _removing?.Invoke(this);
        }
        /// <summary>
        /// 此对象内的控件触发移除自身时响应的命令
        /// </summary>
        public ICommand RemoveCommand => new RelayCommand(Remove);

        ObservableCollection<MetadataPropertyViewModel> _properties;
        /// <summary>
        /// 当前Categorical对应的所有属性配置集合
        /// </summary>
        public ObservableCollection<MetadataPropertyViewModel> Properties
        {
            get { return _properties; }
            set { SetProperty(ref _properties, value); }
        }

        /// <summary>
        /// 判断修改的属性名称是否可用，可用返回true，不可用返回false。
        /// 此方法用于MetadataPropertyViewModel回调使用。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ValidatePropertyName(string name)
        {
            return _categorical.GetProperty(name) == null;
        }

        /// <summary>
        /// 移除属性配置，如果给定的值不在当前列表中，将不做任何操作
        /// </summary>
        /// <param name="property"></param>
        void RemoveProperty(MetadataPropertyViewModel property)
        {
            _properties.Remove(property);
            _categorical.RemoveProperty(property.Name);
        }

        /// <summary>
        /// 创建新的Property，新的对象初始默认命名，需要后续手动修改
        /// </summary>
        void NewProperty()
        {
            string propName = "Property1";
            int count = 1;
            while (!ValidatePropertyName(propName))
            {
                propName = $"Property{++count}";
            }
            var metadataProperty = _categorical.NewProperty(propName);
            var vm = new MetadataPropertyViewModel(metadataProperty);
            vm.BeforeRenamed += ValidatePropertyName;
            vm.Removing += RemoveProperty;
            _properties.Add(vm);
        }
        /// <summary>
        /// 用于响应添加新Property按钮操作的命令
        /// </summary>
        public ICommand NewPropertyCommand => new RelayCommand(NewProperty);

        string _name;
        /// <summary>
        /// 当前Categorical的变量名，设置名称时会检查重名，如果修改的名称
        /// 已存在，将修改失败，不变更当前对象名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if ((_categorical.Parent is Metadata metadata &&
                    metadata.GetCategorical(value) == null) ||
                    _categorical.Parent == null)
                {
                    SetProperty(ref _name, value);
                    _categorical.Name = value;
                }
            }
        }

        string _description;
        /// <summary>
        /// 当前Categorical对象的描述，可为null，第一次修改后不再为null
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                _categorical.Description = value;
            }
        }


    }

    /// <summary>
    /// 用于配置Metadata相关对象的属性配置
    /// </summary>
    public class MetadataPropertyViewModel : ObservableObject
    {
        /// <summary>
        /// 创建MetadataPropertyViewModel对象，并从已存在的Property对象中读取已有内容
        /// </summary>
        /// <param name="property">已有的MetadataProperty对象，绑定后此对象值修改时会同步修改MetadataProperty对象的对应值</param>
        public MetadataPropertyViewModel(MetadataProperty property)
        {
            _property = property;
            _name = _property.Name;
            _value = _property.Value;
            _isString = _property.IsString;
        }

        readonly MetadataProperty _property;
        /// <summary>
        /// 当前属性配置对应的Spec对象
        /// </summary>
        public MetadataProperty Property => _property;

        Func<string, bool>? _breforeRenamed;
        /// <summary>
        /// 用于检查修改的属性名是否可用，此回调由父级对象提供，值可用返回true，不可用返回false。
        /// </summary>
        public event Func<string, bool> BeforeRenamed
        {
            add { _breforeRenamed += value; }
            remove { _breforeRenamed -= value; }
        }

        Action<MetadataPropertyViewModel>? _removing;
        /// <summary>
        /// 当此ViewModel控制的控件控制移除此对象控件时触发的事件，
        /// 用于父级控件移除子控件的回调。
        /// </summary>
        public event Action<MetadataPropertyViewModel> Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        void Remove()
        {
            _removing?.Invoke(this);
        }
        /// <summary>
        /// 当此控件内的按钮移除此控件时触发的命令。
        /// </summary>
        public ICommand RemoveCommand => new RelayCommand(Remove);

        string _name;
        /// <summary>
        /// 当前属性的名称，此属性修改时会通过BeforeRenamed回调检查输入值是否可用，
        /// 如果BeforeRenamed回调返回false，将修改失败；
        /// 如果BeforeRenamed回调未设置，将一直成功修改。
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_breforeRenamed == null || _breforeRenamed(value))
                {
                    _property.Name = value;
                    SetProperty(ref _name, value);
                }
            }
        }

        string _value;
        /// <summary>
        /// Property对象配置的值，可修改，不需要检查值
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _property.Value = value;
                SetProperty(ref _value, value);
            }
        }

        bool _isString;
        /// <summary>
        /// 当前的属性值是否是字符串格式，可修改
        /// </summary>
        public bool IsString
        {
            get
            {
                return _isString;
            }
            set
            {
                _property.IsString = value;
                SetProperty(ref _isString, value);
            }
        }

    }

    /// <summary>
    /// 用于控制Categorical的各种后缀配置的ViewModel
    /// </summary>
    public class MetadataCategoricalSuffixViewModel : ObservableObject
    {
        public MetadataCategoricalSuffixViewModel(MetadataCategorical categorical, MetadataCategoricalSuffixType type)
        {
            _categorical = categorical;
            _isChecked = categorical.GetSuffix(type) != null;
            _type = type;
            _value = categorical.GetSuffix(type) ?? string.Empty;

            _isCombobox = false;

            var attr = type.GetType()
                           .GetField(type.ToString())?
                           .GetCustomAttributes<MetadataDescriptionAttribute>()
                           .First();
            if (attr != null)
            {
                _name = attr.Description;
                _noValue = attr.NoValue;
                if (attr.ExpectValues.Length > 0)
                {
                    _isCombobox = true;
                    _valueSelections = new string[attr.ExpectValues.Length];
                    attr.ExpectValues.CopyTo(_valueSelections, 0);
                    _valueSelection = _valueSelections[0];
                }
                else
                {
                    _valueSelection = string.Empty;
                    _valueSelections = Array.Empty<string>();
                }
            }
            else
            {
                _name = "Suffix";
                _valueSelection = string.Empty;
                _valueSelections = Array.Empty<string>();
                _noValue = false;
            }
            _isTextBox = !_isCombobox && !NoValue;
        }

        readonly MetadataCategorical _categorical;
        readonly MetadataCategoricalSuffixType _type;

        bool _isChecked;
        /// <summary>
        /// 当前的后缀类型是否选中，控制是否添加进Categorical类型的后缀集合中
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetProperty(ref _isChecked, value);
                if (value)
                {
                    _categorical.SetSuffix(_type, _value);
                }
                else
                {
                    _categorical.SetSuffix(_type, null);
                }
            }
        }

        string _name;
        /// <summary>
        /// 当前后缀配置的名称，此名称只会影响UI上的标识，不会影响MetadataCategorical对象的内容。
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        string _value;
        /// <summary>
        /// 当前存储的后缀值，此值可以是空字符串，会同步更新MetadataCategorical类型对象中的值。
        /// 注意：此值不会影响后缀在MetadataCategorical内的存在与否。
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                SetProperty(ref _value, value);
                if (_isChecked)
                {
                    _categorical.SetSuffix(_type, value);
                }
            }
        }

        bool _isCombobox;
        /// <summary>
        /// 用于标记修改值的方式，如果为true，将出示为ComboBox，和IsTextBox互斥
        /// </summary>
        public bool IsCombobox
        {
            get { return _isCombobox; }
            set 
            { 
                SetProperty(ref _isCombobox, value);
                if (value)
                {
                    _isTextBox = false;
                }
            }
        }

        bool _isTextBox;
        /// <summary>
        /// 用于标记修改值的方式，如果为true，将出示TextBox，和IsComboBox互斥
        /// </summary>
        public bool IsTextBox
        {
            get { return _isTextBox; }
            set 
            { 
                SetProperty(ref _isTextBox, value);
                if (value)
                {
                    _isCombobox = false;
                }
            }
        }

        string[] _valueSelections;
        /// <summary>
        /// 如果有限定值，在UI上将以ComboBox下拉选项出示，此属性存储下拉选项内容
        /// </summary>
        public string[] ValueSelections
        {
            get { return _valueSelections; }
            set { SetProperty(ref _valueSelections, value); }
        }

        string _valueSelection;
        /// <summary>
        /// 如果当前值配置方法为ComboBox选取值，此属性绑定选中值
        /// </summary>
        public string ValueSelection
        {
            get { return _valueSelection; }
            set
            {
                SetProperty(ref _valueSelection, value);
                Value = value;
            }
        }

        bool _noValue;
        /// <summary>
        /// 是否不需要配置值
        /// </summary>
        public bool NoValue
        {
            get { return _noValue; }
            set { SetProperty(ref _noValue, value); }
        }

    }



}
