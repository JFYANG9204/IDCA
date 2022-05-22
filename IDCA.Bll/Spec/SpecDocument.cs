
using IDCA.Model.MDM;
using IDCA.Model.Template;
using System;
using System.Collections.Generic;
using System.IO;

namespace IDCA.Model.Spec
{
    public class SpecDocument : SpecObject
    {

        public SpecDocument(string projectPath, Config config) : base()
        {
            _globalTables = new List<Tables>();
            _tableNames = new List<string>();

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

        Action<Tables>? _tablesRemoved;
        Action<Tables>? _tablesAdded;
        /// <summary>
        /// 表格集合移除时触发的事件
        /// </summary>
        public event Action<Tables>? TablesRemoved
        {
            add { _tablesRemoved += value; }
            remove { _tablesRemoved -= value; }
        }
        /// <summary>
        /// 新表格集合添加时触发的事件
        /// </summary>
        public event Action<Tables>? TablesAdded
        {
            add { _tablesAdded += value; }
            remove { _tablesAdded -= value; }
        }


        void OnTablesRemoved(Tables tables)
        {
            _tablesRemoved?.Invoke(tables);
        }

        void OnTablesAdded(Tables tables)
        {
            _tablesAdded?.Invoke(tables);
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

        readonly List<Tables> _globalTables;
        readonly List<string> _tableNames;

        /// <summary>
        /// 判断是否是可用的名称，不区分大小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns>如果可用返回true，已存在相同名称返回false</returns>
        public bool ValidateTablesName(string name)
        {
            return !_tableNames.Exists(ele => ele.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        void RemoveTablesName(string name)
        {
            _tableNames.RemoveAll(ele => ele.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        bool OnTablesRename(Tables tables, string newName)
        {
            if (!ValidateTablesName(newName))
            {
                return false;
            }
            RemoveTablesName(tables.Name);
            _tableNames.Add(newName);
            return true;
        }

        /// <summary>
        /// 创建新的表格配置集合，如果不提供名称，将以"Tab"+索引序号自动命名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Tables NewCollection(string name = "")
        {
            var tables = new Tables(this);
            string tabName = name.ToLower();
            if (string.IsNullOrEmpty(tabName) || !ValidateTablesName(tabName))
            {
                int index = _globalTables.Count;
                while (!ValidateTablesName($"tab{index}"))
                {
                    index++;
                }
                tables.Name = $"tab{index}";
            }
            else
            {
                tables.Name = tabName;
            }
            tables.Rename += OnTablesRename;
            _globalTables.Add(tables);
            OnTablesAdded(tables);
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
            return _globalTables[index];
        }
        /// <summary>
        /// 根据表格文件名获取表格集合对象，如果名称不存在，返回Null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Tables? GetTables(string name)
        {
            return _globalTables.Find(table => table.Name == name);
        }
        /// <summary>
        /// 移除特定名称的表格集合对象
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTables(string name)
        {
            if (!ValidateTablesName(name))
            {
                int index = _globalTables.FindIndex(table => table.Name == name);
                if (index > -1)
                {
                    var tables = _globalTables[index];
                    _globalTables.RemoveAt(index);
                    RemoveTablesName(name);
                    OnTablesRemoved(tables);
                }
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
            var tables = _globalTables[index];
            _globalTables.RemoveAt(index);
            RemoveTablesName(_globalTables[index].Name);
            OnTablesRemoved(tables);
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
            CollectionHelper.ForEach(_globalTables, tables => FileHelper.WriteToFile(Path.Combine(_projectPath, tables.Name), tables.Export()));
        }

    }
}
