
using IDCA.Bll.Template;
using System.Collections.Generic;

namespace IDCA.Bll.SpecDocument
{
    public class SpecDocument : SpecObject
    {
        public SpecDocument(string projectPath) : base()
        {
            _projectPath = projectPath;
            _objectType = SpecObjectType.Document;
            _tables = new Tables(this);
            _templates = new TemplateCollection();
        }

        string _projectPath;
        string _context = string.Empty;
        string _language = string.Empty;

        List<FileTemplate>? _libraryFiles;
        List<FileTemplate>? _otherUsefulFiles;
        
        FileTemplate? _mddManipulationFile;
        FileTemplate? _tabFile;
        FileTemplate? _onNextCaseFile;

        MDMDocument.MDMDocument? _mdmDocument;

        /// <summary>
        /// 初始化当前Spec文档，需要提供已载入的MDM文档对象和模板集合对象
        /// </summary>
        /// <param name="mdm">MDM文档对象</param>
        /// <param name="templates">模板集合对象</param>
        public void Init(MDMDocument.MDMDocument mdm, TemplateCollection templates)
        {
            _mdmDocument = mdm;
            _libraryFiles = templates.Library;
            _otherUsefulFiles = templates.OtherUsefulFile;
            _mddManipulationFile = templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.ManipulationFile);
            _tabFile = templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.TableFile);
            _onNextCaseFile = templates.TryGet<FileTemplate, FileTemplateFlags>(FileTemplateFlags.OnNextCaseFile);
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



    }
}
