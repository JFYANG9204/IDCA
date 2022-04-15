
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class SpecSettingTableViewModel : ObservableObject
    {
        public SpecSettingTableViewModel() 
        {
            _tableElements = new();
            _tableElements.Add(new SpecSettingElementViewModel());
        }

        ObservableCollection<SpecSettingElementViewModel> _tableElements;

        public ObservableCollection<SpecSettingElementViewModel> TableElements
        {
            get { return _tableElements; }
            set { SetProperty(ref _tableElements, value); }
        }

        public ICommand AppendNewElement => new RelayCommand(NewElement);
        /// <summary>
        /// 移除固定位置的子类元素
        /// </summary>
        /// <param name="index"></param>
        public void RemoveElementAt(int index)
        {
            if (index < 0 || index >= _tableElements.Count)
            {
                return;
            }
            _tableElements.RemoveAt(index);
        }


        public void NewElement()
        {
            var element = new SpecSettingElementViewModel
            {
                ParentObject = this,
                Index = _tableElements.Count
            };
            _tableElements.Add(element);
        }

    }
}
