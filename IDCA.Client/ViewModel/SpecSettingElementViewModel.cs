
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class SpecSettingElementViewModel : ObservableObject
    {

        public SpecSettingElementViewModel()
        {
        }

        string _fieldName = string.Empty;
        public string FieldName
        {
            get { return _fieldName; }
            set { SetProperty(ref _fieldName, value); }
        }

        string _tableTitle = string.Empty;
        public string TableTitle
        {
            get { return _tableTitle; }
            set { SetProperty(ref _tableTitle, value); }
        }

        string _baseLabel = string.Empty;
        public string BaseLabel
        {
            get { return _baseLabel; }
            set { SetProperty(ref _baseLabel, value); }
        }

        string _baseFilter = string.Empty;
        public string BaseFilter
        {
            get { return _baseFilter; }
            set { SetProperty(ref _baseFilter, value); }
        }

        SpecSettingTableViewModel? _parentObject;
        /// <summary>
        /// 当前对象的父级对象集合，用于响应移除此对象的方法。此数据应由父级对象初始化。
        /// </summary>
        public SpecSettingTableViewModel? ParentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }

        int _index = -1;
        /// <summary>
        /// 当前对象在父级对象中的索引位置。此对象应由父级对象初始化。
        /// </summary>
        public int Index { get => _index; set => _index = value; }

        public ICommand RemoveElementCommand => new RelayCommand(RemoveThisElement);

        void RemoveThisElement()
        {
            if (_index < 0 || _parentObject == null)
            {
                return;
            }
            _parentObject.RemoveElementAt(Index);
        }

    }
}
