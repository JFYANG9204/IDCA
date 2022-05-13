
using IDCA.Client.Singleton;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{

    public delegate void RemoveTableSettingElementEventHandler(int index);

    public class TableSettingViewModel : ObservableObject
    {
        public TableSettingViewModel() 
        {
        }

        ObservableCollection<string> _topbreaks = new();
        public ObservableCollection<string> Topbreaks
        {
            get { return _topbreaks; }
            set { SetProperty(ref _topbreaks, value); }
        }

        ObservableCollection<string> _tableNames = new();
        public ObservableCollection<string> TableNames
        {
            get { return _tableNames; }
            set { SetProperty(ref _tableNames, value); }
        }

        ObservableCollection<TableSettingElementViewModel> _elementList = new();
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

        ObservableCollection<HeaderInfo> _headerInfos = new();
        public ObservableCollection<HeaderInfo> HeaderInfos
        {
            get { return _headerInfos; }
            set { SetProperty(ref _headerInfos, value); }
        }

        public class HeaderInfo : ObservableObject
        {
            public HeaderInfo() { }

            string _name = string.Empty;
            public string Name
            {
                get { return _name; }
                set { SetProperty(ref _name, value); }
            }

            bool _checked = false;
            public bool Checked
            {
                get { return _checked; }
                set { SetProperty(ref _checked, value); }
            }

        }
    }

    public class TableSettingElementViewModel : ObservableObject
    {
        public TableSettingElementViewModel() { }

        RemoveTableSettingElementEventHandler? _removing = null;
        public event RemoveTableSettingElementEventHandler Removing
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

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(_variableName);
            }
        }


        public ICommand AxisSettingCommand => new RelayCommand(ShowAxisSettingDialog);
        void ShowAxisSettingDialog()
        {
            GlobalConfig.Instance.CurrentTableSettingIndex = Index;
            Common.WindowManager.ShowDialog("AxisSettingWindow");
        }

        public ICommand RemoveCommand => new RelayCommand(Remove);
        void Remove()
        {
            _removing?.Invoke(Index);
        }

    }



}
