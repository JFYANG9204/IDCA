using IDCA.Bll.Template;

namespace IDCA.Client.Singleton
{
    public class GlobalConfig
    {
        public GlobalConfig() { }

        private readonly static GlobalConfig _instance = new();
        public static GlobalConfig Instance => _instance;


        string _projectRootPath = string.Empty;
        /// <summary>
        /// 获取或配置当前配置中的项目根目录
        /// </summary>
        public string ProjectRootPath { get => _projectRootPath; set => _projectRootPath = value; }

        TemplateCollection? _templates;
        /// <summary>
        /// 获取或配置当前项目的模板类型，未配置时值为null
        /// </summary>
        public TemplateCollection? Templates { get => _templates; set => _templates = value; }


    }
}
