using System;

namespace IDCA.Client.ViewModel
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class AxisDescriptionAttribute : Attribute
    {

        public AxisDescriptionAttribute()
        {
            _controlType = ValueSettingControlType.TextBox;
            _description = string.Empty;
            _selections = Array.Empty<string>();
            _indexOfParent = 0;
        }

        public AxisDescriptionAttribute(string description, ValueSettingControlType controlType) : this()
        {
            _description = description;
            _controlType = controlType;
        }

        string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        ValueSettingControlType _controlType;

        public ValueSettingControlType ControlType
        {
            get { return _controlType; }
            set { _controlType = value; }
        }

        //bool _isTextBox;
        //public bool IsTextBox
        //{
        //    get { return _isTextBox; }
        //    set { _isTextBox = value; }
        //}
        //
        //bool _isComboBox;
        //public bool IsComboBox
        //{
        //    get { return _isComboBox; }
        //    set { _isComboBox = value; }
        //}

        string[] _selections;
        public string[] Selections
        {
            get { return _selections; }
            set { _selections = value; }
        }

        int _indexOfParent;
        public int IndexOfParent
        {
            get { return _indexOfParent; }
            set { _indexOfParent = value; }
        }

    }
}
