using IDCA.Model;
using IDCA.Model.Template;

namespace IDCA.Client.Singleton
{
    public class GlobalConfig
    {
        private GlobalConfig() 
        {
            _config = new Config();
            LoadConfigs();
            _templateDictionary = new TemplateDictionary();
        }

        /// <summary>
        /// 载入或重载入当前的配置值，会清空当前已存在的数据并重新读取
        /// </summary>
        public void LoadConfigs()
        {
            _config.Clear();
            _config.TryLoad(Properties.Settings.Default, typeof(SpecConfigKeys));
        }

        private readonly static GlobalConfig _instance = new();
        public static GlobalConfig Instance => _instance;

        readonly Config _config;
        /// <summary>
        /// 当前的全局配置
        /// </summary>
        public Config Config => _config;

        string _projectRootPath = string.Empty;
        /// <summary>
        /// 获取或配置当前配置中的项目根目录
        /// </summary>
        public string ProjectRootPath { get => _projectRootPath; set => _projectRootPath = value; }

        string _projectName = string.Empty;
        /// <summary>
        /// 获取或配置当前项目的名称
        /// </summary>
        public string ProjectName { get => _projectName; set => _projectName = value; }

        string _excelSettingFilePath = string.Empty;
        /// <summary>
        /// Excel配置文件，可选配置
        /// </summary>
        public string ExcelSettingFilePath { get => _excelSettingFilePath; set => _excelSettingFilePath = value; }

        TemplateCollection? _templates;
        /// <summary>
        /// 获取或配置当前项目的模板类型，未配置时值为null
        /// </summary>
        public TemplateCollection? Templates { get => _templates; set => _templates = value; }

        string _mdmDocumentPath = string.Empty;
        /// <summary>
        /// 当前项目的MDM文档路径
        /// </summary>
        public string MdmDocumentPath { get => _mdmDocumentPath; set => _mdmDocumentPath = value; }

        readonly TemplateDictionary _templateDictionary;
        /// <summary>
        /// 当前读取到的模板字典
        /// </summary>
        public TemplateDictionary TemplateDictionary => _templateDictionary;

    }
}
