
using IDCA.Bll.MDM;
using IDCA.Bll.Template;
using System.Collections.Generic;

namespace IDCA.Bll.Spec
{
    public class SpecDocument : SpecObject
    {
        public SpecDocument(string projectPath, string templateXmlPath, Config config) : base()
        {
            _projectPath = projectPath;
            _objectType = SpecObjectType.Document;
            _tables = new Tables(this);
            _manipulations = new Manipulations(this);
            _scripts = new ScriptCollection(this);
            _templates = new TemplateCollection();
            _templates.Load(templateXmlPath);
            _config = config;
            _dmsMetadata = new(this, config);
            _metadata = new(this, config);
        }

        readonly Config _config;
        readonly string _projectPath;
        string _context = string.Empty;
        string _language = string.Empty;

        List<FileTemplate>? _libraryFiles;
        List<FileTemplate>? _otherUsefulFiles;
        
        FileTemplate? _mddManipulationFile;
        FileTemplate? _tabFile;
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
        /// 当前文档的配置信息
        /// </summary>
        public Config Config => _config;

        readonly MetadataCollection _dmsMetadata;
        readonly MetadataCollection _metadata;

        /// <summary>
        /// 初始化当前Spec文档，需要提供已载入的MDM文档对象和模板集合对象
        /// </summary>
        /// <param name="mdm">MDM文档对象</param>
        public void Init(MDMDocument mdm)
        {
            _mdmDocument = mdm;
            _libraryFiles = _templates.Library;
            _otherUsefulFiles = _templates.OtherUsefulFile;
            _mddManipulationFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.ManipulationFile);
            _tabFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.TableFile);
            _onNextCaseFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.OnNextCaseFile);
            _dmsMetadataFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.DmsMetadataFile);
            _metadataFile = _templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.MetadataFile);
            _context = mdm.Context;
            _language = mdm.Language;
        }

        readonly Tables _tables;
        /// <summary>
        /// 当前文档的表格集合
        /// </summary>
        public Tables Tables => _tables;

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
        }

    }
}
