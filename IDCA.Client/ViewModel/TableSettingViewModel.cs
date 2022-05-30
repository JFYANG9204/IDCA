
using IDCA.Client.Singleton;
using IDCA.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class TableSettingViewModel : ObservableObject
    {

        public TableSettingViewModel(string tableName, List<string>? tableNames = null)
        {
            _elementList = new ObservableCollection<TableSettingElementViewModel>();
            _tableName = tableName;
            _globalTableNames = tableNames;
        }

        readonly List<string>? _globalTableNames;

        TableSettingTreeNode? _node;
        public TableSettingTreeNode? Node
        {
            get { return _node; }
            set { SetProperty(ref _node, value); }
        }

        Action<string>? _renamed;
        public event Action<string>? Renamed
        {
            add { _renamed += value; }
            remove { _renamed -= value; }
        }

        string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set 
            {
                bool exist = _globalTableNames != null && _globalTableNames.Exists(e => e.Equals(value, StringComparison.OrdinalIgnoreCase));
                if (!exist)
                {
                    SetProperty(ref _tableName, value);
                    _globalTableNames?.Add(value);
                    _renamed?.Invoke(value);
                }
            }
        }

        Action<TableSettingViewModel>? _removing;
        public event Action<TableSettingViewModel> Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        void Remove()
        {
            _removing?.Invoke(this);
        }
        public ICommand RemoveCommand => new RelayCommand(Remove);

        ObservableCollection<TableSettingElementViewModel> _elementList;
        public ObservableCollection<TableSettingElementViewModel> ElementList
        {
            get { return _elementList; }
            set { SetProperty(ref _elementList, value); }
        }

        public ICommand PushNewElementCommand => new RelayCommand(Push);
        void Push()
        {
            var element = new TableSettingElementViewModel
            {
                Index = ElementList.Count,
            };
            element.Removing += RemoveElementAt;
            ElementList.Add(element);
            GlobalConfig.Instance.CurrentTableSetting.NewTableSetting();
        }

        void RemoveElementAt(int index)
        {
            if (index < 0 || index >= _elementList.Count)
            {
                return;
            }
            _elementList.RemoveAt(index);
            for (int i = 0; i < _elementList.Count; i++)
            {
                _elementList[i].Index = i;
            }
        }

        public void LoadFromTableSettingCollection(TableSettingCollection tableSettings)
        {
            _tableName = tableSettings.SpecTables.Name;
            foreach (TableSetting setting in tableSettings)
            {
                var tableSettingViewModel = new TableSettingElementViewModel();
                tableSettingViewModel.LoadFromTableSetting(setting);
                _elementList.Add(tableSettingViewModel);
            }
        }

    }

    public class TableSettingElementViewModel : ObservableObject
    {
        public TableSettingElementViewModel() 
        {
            _axisViewModel = new AxisSettingViewModel();
        }

        readonly AxisSettingViewModel _axisViewModel;

        Action<int>? _removing = null;
        public event Action<int> Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        string _variableName = string.Empty;
        public string VariableName
        {
            get { return _variableName; }
            set { SetProperty(ref _variableName, value); }
        }

        string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        string _baseText = string.Empty;
        public string BaseText
        {
            get { return _baseText; }
            set { SetProperty(ref _baseText, value); }
        }

        string _baseFilter = string.Empty;
        public string BaseFilter
        {
            get { return _baseFilter; }
            set { SetProperty(ref _baseFilter, value); }
        }

        string _tableFilter = string.Empty;
        public string TableFilter
        {
            get { return _tableFilter; }
            set { SetProperty(ref _tableFilter, value); }
        }

        int _index = 0;
        public int Index { get => _index; set => _index = value; }

        public ICommand AxisSettingCommand => new RelayCommand(ShowAxisSettingDialog);
        void ShowAxisSettingDialog()
        {
            GlobalConfig.Instance.CurrentTableSettingIndex = Index;
            Common.WindowManager.ShowWindow("AxisSettingWindow", _axisViewModel);
        }

        public ICommand RemoveCommand => new RelayCommand(Remove);
        void Remove()
        {
            _removing?.Invoke(Index);
        }

        public void LoadFromTableSetting(TableSetting tableSetting)
        {
            _variableName = tableSetting.Field != null ? tableSetting.Field.FullName : string.Empty;
            _title = tableSetting.TableTitle;
            _baseText = tableSetting.BaseLabel;
            _baseFilter = tableSetting.BaseFilter;
            _tableFilter = tableSetting.TableFilter;
        }


    }



}
