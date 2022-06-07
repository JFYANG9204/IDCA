using IDCA.Client.Singleton;
using IDCA.Model.Spec;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IDCA.Client.ViewModel
{
    public class HeaderSettingViewModel : ObservableObject
    {

        public HeaderSettingViewModel(Metadata metadata)
        {
            _metadata = metadata;
            _headerName = metadata.Name;
            _elements = new ObservableCollection<HeaderSettingElementViewModel>();
        }

        readonly Metadata _metadata;

        TableSettingTreeNode? _node;
        public TableSettingTreeNode? Node
        {
            get { return _node; }
            set { SetProperty(ref _node, value); }
        }

        string _headerName;
        public string HeaderName
        {
            get { return _headerName; }
            set 
            {
                if (_beforeRenamed == null || _beforeRenamed(value))
                {
                    var oldeName = _headerName;
                    SetProperty(ref _headerName, value);
                    _metadata.Name = value;
                    _renamed?.Invoke(oldeName, value);
                }
            }
        }

        Action<string, string>? _renamed;
        public event Action<string, string>? Renamed
        {
            add { _renamed += value; }
            remove { _renamed -= value; }
        }

        Func<string, bool>? _beforeRenamed;
        public event Func<string, bool>? BeforeRenamed
        {
            add { _beforeRenamed += value; }
            remove { _beforeRenamed -= value; }
        }

        ObservableCollection<HeaderSettingElementViewModel> _elements;
        public ObservableCollection<HeaderSettingElementViewModel> Elements
        {
            get { return _elements; }
            set { SetProperty(ref _elements, value); }
        }

        void RemoveElement(HeaderSettingElementViewModel element)
        {
            if (element.IndexOfParent > 0 && element.IndexOfParent < _elements.Count)
            {
                _elements.RemoveAt(element.IndexOfParent);
            }
        }

        public void NewElement()
        {
            var element = new HeaderSettingElementViewModel(_elements.Count);
            element.Removing += RemoveElement;
            _elements.Add(element);
        }
        public ICommand NewElementCommand => new RelayCommand(NewElement);

        public static HeaderSettingViewModel Empty(TableSettingTreeNode node)
        {
            return new HeaderSettingViewModel(new Metadata(GlobalConfig.Instance.SpecDocument, GlobalConfig.Instance.Config, ""))
            {
                Node = node
            };
        }

    }


    public class HeaderSettingElementViewModel : ObservableObject
    {

        public HeaderSettingElementViewModel()
        {
            _indexOfParent = 0;
            _variableName = string.Empty;
            _description = string.Empty;
            _expression = string.Empty;
        }

        public HeaderSettingElementViewModel(int indexOfParent) : this()
        {
            _indexOfParent = indexOfParent;
        }

        int _indexOfParent;
        public int IndexOfParent
        {
            get { return _indexOfParent; }
            set { _indexOfParent = value; }
        }

        Action<HeaderSettingElementViewModel>? _removing;
        public event Action<HeaderSettingElementViewModel>? Removing
        {
            add { _removing += value; }
            remove { _removing -= value; }
        }

        string _variableName;
        public string VariableName
        {
            get { return _variableName; }
            set { SetProperty(ref _variableName, value); }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        string _expression;
        public string Expression
        {
            get { return _expression; }
            set { SetProperty(ref _expression, value); }
        }

        void Remove()
        {
            _removing?.Invoke(this);
        }
        public ICommand RemoveCommand => new RelayCommand(Remove);

    }


}
