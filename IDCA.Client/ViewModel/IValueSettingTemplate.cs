

using System.Collections.Generic;

namespace IDCA.Client.ViewModel
{

    public enum ValueSettingControlType
    {
        ComboBox,
        TextBox
    }

    public interface IValueSettingTemplate
    {
        public string Name { get; set; }

        public ValueSettingControlType ControlType { get; }

        public IList<string> Selections { get; set; }

        public int SelectedIndex { get; set; }

        public string Text { get; set; }


    }
}
