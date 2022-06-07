
namespace IDCA.Model
{
    public class ExceptionMessages
    {
        // MDM 部分
        public const string MDMFieldIsEmpty = "错误：MDM文档载入错误，未载入Fields集合。";
        public const string MDMFieldIsNotFound = "错误：无法在MDM文档中找到名为'{0}'的Field。";
        // Table 部分
        public const string TableFieldIsNotSetted = "错误：未设定Table对象的Field信息，无法载入MDM文档Field对象。";
        public const string TableFieldInvalid = "错误：Field'{0}'格式无效。";
        // Tamplate 部分
        public const string TemplateRootFolderIsNotExist = "错误：模板根文件夹不存在，所需路径：{0}";
        public const string TemplateIsNotFind = "错误：未找到用于{0}的函数模板。";
        public const string TemplateDefinitionXmlFileIsNotExist = "警告：模板'{0}'缺少定义文件Template.xml。";
        public const string TemplateNameCannotBeEmpty = "错误：模板'{0}'中XML定义文件中的模板名不可为空。";
        // File
        public const string FileWriteError = "错误：文本写入错误，路径：{0}";
        // Spec
        public const string SpecMDMIsNotInitialized = "警告：MDM文档未初始化。";
        // Settings
        public const string SettingNetLikeRangeInvalid = "警告：Net/Combine码号配置格式错误，错误字符：'{0}'已跳过。";
        public const string SettingTopBottomBoxInvalid = "警告：Top/Bottom Box配置无效，Box值不能为0或大于Categorical列表数量。";
    }

    public class Messages
    {
        public const string FileWriteSuccess = "文件：'{0}'写入完成";
    }
}
