using IDCA.Bll;
using IDCA.Bll.MDM;
using IDCA.Bll.Template;

namespace IDCA.Client.Singleton
{
    public class GlobalConfig
    {
        public GlobalConfig() 
        {
            _mdmDocument = new MDMDocument();
            _config = new Config();
            _tableSettingCollection = new TableSettingCollection(_mdmDocument, _config);
            LoadConfigs();
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

        string _fileName = string.Empty;
        /// <summary>
        /// 获取或配置当前项目的名称
        /// </summary>
        public string FileName { get => _fileName; set => _fileName = value; }

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

        readonly MDMDocument _mdmDocument;
        readonly TableSettingCollection _tableSettingCollection;
        /// <summary>
        /// 当前项目的MDM文档对象，此对象需要手动初始化
        /// </summary>
        public MDMDocument MDMDocument => _mdmDocument;
        /// <summary>
        /// 当前项目的表格配置集合
        /// </summary>
        public TableSettingCollection TableSettings => _tableSettingCollection;

    }
}
