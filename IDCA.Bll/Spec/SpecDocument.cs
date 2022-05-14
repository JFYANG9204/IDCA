
using IDCA.Model.MDM;
using IDCA.Model.Template;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDCA.Model.Spec
{
    public class SpecDocument : SpecObject
    {

        public SpecDocument(string projectPath, Config config) : base()
        {
            _projectPath = projectPath;
            _objectType = SpecObjectType.Document;
            _manipulations = new Manipulations(this);
            _scripts = new ScriptCollection(this);
            _templates = new TemplateCollection();
            _config = config;
            _dmsMetadata = new MetadataCollection(this, config);
            _metadata = new MetadataCollection(this, config);
        }

        public SpecDocument(string projectPath, string templateXmlPath, Config config) : this(projectPath, config)
        {
            _templates.Load(templateXmlPath);
        }

        readonly Config _config;
        readonly string _projectPath;

        List<FileTemplate>? _libraryFiles;
        List<FileTemplate>? _otherUsefulFiles;
        
        FileTemplate? _mddManipulationFile;
        FileTemplate? _onNextCaseFile;
        FileTemplate? _dmsMetadataFile;
        FileTemplate? _metadataFile;

        MDMDocument? _mdmDocument;
        /// <summary>
        /// 对应的MDM文档对象
        /// </summary>
        public MDMDocument? MDMDocument
        {
            get
            {
                if (_mdmDocument == null)
                {
                    Logger.Warning("MDMDocumentNotInitialized", ExceptionMessages.SpecMDMIsNotInitialized);
                }
                return _mdmDocument;
            }
        }

        /// <summary>
        /// 修改当前MDM文档的上下文类型
        /// </summary>
        /// <param name="context"></param>
        public void SetContext(string context)
        {
            _mdmDocument?.SetContext(context);
        }

        /// <summary>
        /// 修改当前MDM文档的语言类型
        /// </summary>
        /// <param name="language"></param>
        public void SetLanguage(string language)
        {
            _mdmDocument?.SetLanguage(language);
        }

        /// <summary>
        /// 当前文档的配置信息
        /// </summary>
        public Config Config => _config;

        readonly MetadataCollection _dmsMetadata;
        readonly MetadataCollection _metadata;

        /// <summary>
        /// 初始化当前Spec文档，需要提供已载入的MDM文档对象
        /// </summary>
        /// <param name="mdm">MDM文档对象</param>
        public void Init(MDMDocument mdm)
        {
            _mdmDocument = mdm;
            _libraryFiles = _templates.Library;
            _otherUsefulFiles = _templates.OtherUsefulFile;
            _mddManipulationFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.ManipulationFile);
            _onNextCaseFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.OnNextCaseFile);
            _dmsMetadataFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.DmsMetadataFile);
            _metadataFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.MetadataFile);
        }
        /// <summary>
        /// 初始化当前Spec文档，需要同时提供已完成初始化的MDM文档对象和模板集合定义文件路径
        /// </summary>
        /// <param name="mdm"></param>
        /// <param name="templateXmlPath"></param>
        public void Init(MDMDocument mdm, string templateXmlPath)
        {
            Init(mdm);
            _templates.Load(templateXmlPath);
        }

        readonly Dictionary<string, Tables> _globalTables = new();
        /// <summary>
        /// 创建新的表格配置集合，如果不提供名称，将以"Tab"+索引序号自动命名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Tables NewCollection(string name = "")
        {
            var tables = new Tables(this);
            string tabName = name.ToLower();
            if (string.IsNullOrEmpty(tabName) || _globalTables.ContainsKey(tabName))
            {
                int index = _globalTables.Count;
                while (_globalTables.ContainsKey($"tab{index}"))
                {
                    index++;
                }
                tables.Name = $"tab{index}";
            }
            else
            {
                tables.Name = tabName;
            }
            _globalTables.Add(tables.Name, tables);
            return tables;
        }
        /// <summary>
        /// 获取固定索引位置的表格配置集合，如果索引越限，返回Null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Tables? GetTables(int index)
        {
            if (index < 0 || index >= _globalTables.Count)
            {
                return null;
            }
            return _globalTables.Values.ElementAt(index);
        }
        /// <summary>
        /// 根据表格文件名获取表格集合对象，如果名称不存在，返回Null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Tables? GetTables(string name)
        {
            string lowerName = name.ToLower();
            return _globalTables.ContainsKey(lowerName) ? _globalTables[lowerName] : null;
        }
        /// <summary>
        /// 移除特定名称的表格集合对象
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTables(string name)
        {
            if (_globalTables.ContainsKey(name.ToLower()))
            {
                _globalTables.Remove(name.ToLower());
            }
        }
        /// <summary>
        /// 移除特定索引位置的表格集合对象
        /// </summary>
        /// <param name="index"></param>
        public void RemoveTablesAt(int index)
        {
            if (index < 0 || index >= _globalTables.Count)
            {
                return;
            }
            _globalTables.Remove(_globalTables.Keys.ElementAt(index));
        }

        readonly TemplateCollection _templates;
        /// <summary>
        /// 当前文档的模板集合
        /// </summary>
        public TemplateCollection Templates => _templates;

        readonly Manipulations _manipulations;
        /// <summary>
        /// 当前文档的MDM修改集合
        /// </summary>
        public Manipulations Manipulations => _manipulations;

        readonly ScriptCollection _scripts;
        /// <summary>
        /// 当前文档的赋值脚本语句集合
        /// </summary>
        public ScriptCollection Scripts => _scripts;

        /// <summary>
        /// 执行当前脚本配置，写入文件内容
        /// </summary>
        public void Exec()
        {
            _libraryFiles?.ForEach(file => FileHelper.WriteToFile(_projectPath, file));
            _otherUsefulFiles?.ForEach(file => FileHelper.WriteToFile(_projectPath, file));
            FileHelper.WriteToFile(_projectPath, _dmsMetadataFile, _dmsMetadata.Export());
            FileHelper.WriteToFile(_projectPath, _metadataFile, _metadata.Export());
            FileHelper.WriteToFile(_projectPath, _onNextCaseFile, _scripts.Export());
            FileHelper.WriteToFile(_projectPath, _mddManipulationFile, _manipulations.Export());
            CollectionHelper.ForEach(_globalTables.Values, tables => FileHelper.WriteToFile(Path.Combine(_projectPath, tables.Name), tables.Export()));
        }

    }
}
