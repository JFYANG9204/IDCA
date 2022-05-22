using IDCA.Model;
using IDCA.Model.MDM;
using IDCA.Model.Spec;
using IDCA.Model.Template;
using System.Collections.Generic;
using System.Linq;

namespace IDCA.Client.Singleton
{
    public class GlobalConfig
    {
        public GlobalConfig() 
        {
            _config = new Config();
            LoadConfigs();
            _templateDictionary = new TemplateDictionary();
            _mdmDocument = new MDMDocument();
            _specDocument = new SpecDocument(ProjectRootPath, _config);
            _tableSettings = new Dictionary<string, TableSettingCollection>();
            _currentTableSetting = NewTableSetting();
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

        readonly SpecDocument _specDocument;
        readonly MDMDocument _mdmDocument;
        TableSettingCollection _currentTableSetting;
        /// <summary>
        /// 当前Spec文档配置对象
        /// </summary>
        public SpecDocument SpecDocument => _specDocument;
        /// <summary>
        /// 当前项目的MDM文档对象，此对象需要手动初始化
        /// </summary>
        public MDMDocument MDMDocument => _mdmDocument;
        /// <summary>
        /// 项目当前配置界面对应的表格配置集合
        /// </summary>
        public TableSettingCollection CurrentTableSetting => _currentTableSetting;

        readonly Dictionary<string, TableSettingCollection> _tableSettings;
        /// <summary>
        /// 验证是否是可用的名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ValidateTableName(string name)
        {
            return !_tableSettings.ContainsKey(name.ToLower());
        }
        /// <summary>
        /// 尝试获取指定名称的表格配置集合，如果不存在，返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TableSettingCollection? TryGetTableSetting(string name)
        {
            string lowerName = name.ToLower();
            return _tableSettings.ContainsKey(lowerName) ? _tableSettings[lowerName] : null;
        }
        /// <summary>
        /// 创建新的表格配置集合，如果忽略名称，默认配置"tab+序号"的名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TableSettingCollection NewTableSetting(string name = "")
        {
            string tabName = name.ToLower();
            if (string.IsNullOrEmpty(name) || _tableSettings.ContainsKey(tabName))
            {
                tabName = $"tab{_tableSettings.Count + 1}";
            }
            var tables = _specDocument.NewCollection(name);
            var table = new TableSettingCollection(MDMDocument, Config, tables);
            _tableSettings.Add(tabName, table);
            return table;
        }

        /// <summary>
        /// 设置当前的表格配置集合
        /// </summary>
        /// <param name="name"></param>
        public void SetCurrentTableSetting(string name)
        {
            if (_tableSettings.ContainsKey(name))
            {
                _currentTableSetting = _tableSettings[name];
            }
        }
        /// <summary>
        /// 获取当前配置中的所有表格名称
        /// </summary>
        /// <returns></returns>
        public string[] GetTableNames()
        {
            return _tableSettings.Keys.ToArray();
        }

        int _currentTableSettingIndex = 0;
        /// <summary>
        /// 当前项目表格配置选定配置条目的索引
        /// </summary>
        public int CurrentTableSettingIndex { get => _currentTableSettingIndex; set => _currentTableSettingIndex = value; }

        string _mdmDocumentPath = string.Empty;
        /// <summary>
        /// 当前项目的MDM文档路径
        /// </summary>
        public string MdmDocumentPath { get => _mdmDocumentPath; set => _mdmDocumentPath = value; }

        int _templateSelectIndex = 0;
        /// <summary>
        /// 获取或配置当前模板集合的选中索引
        /// </summary>
        public int TemplateSelectIndex { get => _templateSelectIndex; set => _templateSelectIndex = value; }

        readonly TemplateDictionary _templateDictionary;
        /// <summary>
        /// 当前读取到的模板字典
        /// </summary>
        public TemplateDictionary TemplateDictionary => _templateDictionary;

    }
}
