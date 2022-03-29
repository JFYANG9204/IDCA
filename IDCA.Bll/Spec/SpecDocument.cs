
using IDCA.Bll.MDM;
using IDCA.Bll.Template;
using System.Collections.Generic;

namespace IDCA.Bll.Spec
{
    public class SpecDocument : SpecObject
    {
        public SpecDocument(string projectPath, string templateXmlPath) : base()
        {
            _projectPath = projectPath;
            _objectType = SpecObjectType.Document;
            _tables = new Tables(this);
            _manipulations = new Manipulations(this);
            _assignments = new Assignments(this);
            _templates = new TemplateCollection();
            _templates.Load(templateXmlPath);
        }

        string _projectPath;
        string _context = string.Empty;
        string _language = string.Empty;

        List<FileTemplate>? _libraryFiles;
        List<FileTemplate>? _otherUsefulFiles;
        
        FileTemplate? _mddManipulationFile;
        FileTemplate? _tabFile;
        FileTemplate? _onNextCaseFile;

        MDMDocument? _mdmDocument;

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

        readonly Assignments _assignments;
        /// <summary>
        /// 当前文档的赋值脚本语句集合
        /// </summary>
        public Assignments Assignments => _assignments;

    }
}
