
using IDCA.Client.Singleton;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class TableSettingViewModel : ObservableObject
    {
        public TableSettingViewModel() 
        {
        }

        ObservableCollection<TableSettingElementViewModel> _elementList = new();
        public ObservableCollection<TableSettingElementViewModel> ElementList
        {
            get { return _elementList; }
            set { SetProperty(ref _elementList, value); }
        }

        public void Push()
        {
            var element = new TableSettingElementViewModel
            {
                Index = ElementList.Count
            };
            ElementList.Add(element);
        }

    }

    public class TableSettingElementViewModel : ObservableObject
    {
        public TableSettingElementViewModel() { }

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
            Common.WindowManager.ShowDialog("SpecAxisSettingDialog");
        }

    }


}
