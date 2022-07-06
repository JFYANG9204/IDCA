using System;

namespace IDCA.Client.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelMarkAttribute : Attribute
    {

        public ViewModelMarkAttribute(SettingViewType settingViewType, string fullTypeName, string namespaceName)
        {
            _settingViewType = settingViewType;
            _fullTypeName = fullTypeName;
            _namespaceName = namespaceName;
        }

        SettingViewType _settingViewType;

        public SettingViewType SettingViewType
        {
            get => _settingViewType;
            set => _settingViewType = value;
        }

        string _fullTypeName;

        public string FullTypeName
        {
            get => _fullTypeName;
            set => _fullTypeName = value;
        }

        string _namespaceName;

        public string NamespaceName
        {
            get => _namespaceName;
            set => _namespaceName = value;
        }

    }

    public enum SettingViewType
    {
        TableSetting,
        HeaderSetting,
        Overview
    }

}
