using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{

    /// <summary>
    /// 用来绑定可勾选的数据的View Model类
    /// </summary>
    public class CheckableItemViewModel : ObservableObject
    {

        public CheckableItemViewModel()
        {
        }

        public CheckableItemViewModel(string name)
        {
            _name = name;
        }

        public CheckableItemViewModel(string name, Action<CheckableItemViewModel> onCheckedChanged)
        {
            _name = name;
            _checkedChanged += onCheckedChanged;
        }

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
            set 
            { 
                SetProperty(ref _checked, value);
                _checkedChanged?.Invoke(this);
            }
        }

        Action<CheckableItemViewModel>? _checkedChanged;
        public event Action<CheckableItemViewModel> CheckedChanged
        {
            add { _checkedChanged += value; }
            remove { _checkedChanged -= value; }
        }

        Action? _selected;
        public event Action Selected
        {
            add { _selected += value; }
            remove { _selected -= value; }
        }

        void OnSelected()
        {
            _selected?.Invoke();
        }
        public ICommand OnSelectedCommand => new RelayCommand(OnSelected);

    }
}
