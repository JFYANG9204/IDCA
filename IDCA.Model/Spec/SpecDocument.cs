
using IDCA.Model.MDM;
using IDCA.Model.Template;
using System;
using System.Collections.Generic;
using System.IO;

namespace IDCA.Model.Spec
{
    public class SpecDocument : SpecObject
    {

        public SpecDocument(string projectPath, TemplateCollection template, Config config) : base()
        {
            _globalTables = new List<Tables>();
            _projectPath = projectPath;
            _objectType = SpecObjectType.Document;
            _manipulations = new Manipulations(this);
            _scripts = new ScriptCollection(this);
            _templates = template;
            _config = config;
            _dmsMetadata = new MetadataCollection(this, config);
            _metadata = new MetadataCollection(this, config);
        }

        public SpecDocument(Config config) : this("", new TemplateCollection(), config)
        {
        }

        public SpecDocument(string projectPath, Config config) : this(projectPath, new TemplateCollection(), config)
        {
        }

        public SpecDocument(string projectPath, string templateXmlPath, Config config) : this(projectPath, config)
        {
            _templates.Load(templateXmlPath);
        }

        readonly Config _config;

        string _projectPath;
        /// <summary>
        /// 当前项目的根目录
        /// </summary>
        public string ProjectPath
        {
            get { return _projectPath; }
            set { _projectPath = value; }
        }

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

        // Header部分更新触发事件
        Action<Metadata>? _headerAdded;
        Action<Metadata>? _headerRemoved;
        Action<Metadata>? _headerChanged;
        /// <summary>
        /// 当表头变量添加进集合时触发的事件
        /// </summary>
        public event Action<Metadata> HeaderAdded
        {
            add { _headerAdded += value; }
            remove { _headerAdded -= value; }
        }
        /// <summary>
        /// 当表头变量被移除时触发的事件
        /// </summary>
        public event Action<Metadata> HeaderRemoved
        {
            add { _headerRemoved += value; }
            remove { _headerRemoved -= value; }
        }
        /// <summary>
        /// 当表头发生变化时（添加或删除）触发的事件
        /// </summary>
        public event Action<Metadata> HeaderChanged
        {
            add { _headerChanged += value; }
            remove { _headerChanged -= value; }
        }

        void OnHeaderAdded(Metadata metadata)
        {
            _headerAdded?.Invoke(metadata);
        }

        void OnHeaderRemoved(Metadata metadata)
        {
            _headerRemoved?.Invoke(metadata);
        }

        void OnHeaderChanged(Metadata metadata)
        {
            _headerChanged?.Invoke(metadata);
        }

        // Table部分更新触发事件
        Action<Tables>? _tablesRemoved;
        Action<Tables>? _tablesAdded;
        Action<Tables>? _tablesChanged;
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
        /// <summary>
        /// 表格集合发生变动时触发的事件
        /// </summary>
        public event Action<Tables>? TablesChanged
        {
            add { _tablesChanged += value; }
            remove { _tablesChanged -= value; }
        }

        void OnTablesRemoved(Tables tables)
        {
            _tablesRemoved?.Invoke(tables);
        }

        void OnTablesAdded(Tables tables)
        {
            _tablesAdded?.Invoke(tables);
        }

        void OnTablesChanged(Tables tables)
        {
            _tablesChanged?.Invoke(tables);
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

        /// <summary>
        /// 判断是否是可用的名称，不区分大小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns>如果可用返回true，已存在相同名称返回false</returns>
        public bool ValidateTablesName(string name)
        {
            return !_globalTables.Exists(t => t.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 判断是否是可用的表头名称，不区分大小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ValidateHeaderName(string name)
        {
            return _metadata[name] == null;
        }

        bool OnTablesRename(Tables tables, string newName)
        {
            if (!ValidateTablesName(newName))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建新的表头变量并添加到元数据集合，如果忽略name参数，将以"Topbreak_"+索引自动命名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Metadata NewHeader(string name = "")
        {
            string headerName = name;
            if (string.IsNullOrEmpty(headerName))
            {
                headerName = $"TopBreak_{_metadata.Count + 1}";
            }
            var metadata = _metadata.NewMetadata(headerName, MetadataType.Categorical);
            OnHeaderAdded(metadata);
            OnHeaderChanged(metadata);
            return metadata;
        }

        /// <summary>
        /// 创建新的表格配置集合，如果不提供名称，将以"Tab"+索引序号自动命名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Tables NewTables(string name = "")
        {
            var tables = new Tables(this);
            string tabName = name.ToLower();
            if (string.IsNullOrEmpty(tabName) || !ValidateTablesName(tabName))
            {
                int index = _globalTables.Count + 1;
                while (!ValidateTablesName($"Tab_{index}"))
                {
                    index++;
                }
                tables.Name = $"Tab_{index}";
            }
            else
            {
                tables.Name = tabName;
            }
            tables.Rename += OnTablesRename;
            _globalTables.Add(tables);
            OnTablesAdded(tables);
            OnTablesChanged(tables);
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
        /// 获取指定名称的表头变量，不区分大小写。如果不存在，返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Metadata? GetHeader(string name)
        {
            return _metadata[name];
        }
        /// <summary>
        /// 移除特定名称的表格集合对象
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTables(string name)
        {
            if (!ValidateTablesName(name))
            {
                int index = _globalTables.FindIndex(table => table.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (index > -1)
                {
                    var tables = _globalTables[index];
                    _globalTables.RemoveAt(index);
                    OnTablesRemoved(tables);
                    OnTablesChanged(tables);
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
            OnTablesRemoved(tables);
            OnTablesChanged(tables);
        }

        /// <summary>
        /// 移除特定名称的表头变量
        /// </summary>
        /// <param name="name"></param>
        public void RemoveHeader(string name)
        {
            var header = _metadata[name];
            if (header != null)
            {
                _metadata.Remove(name);
                OnHeaderRemoved(header);
                OnHeaderChanged(header);
            }
        }

        /// <summary>
        /// 获取当前所有表头变量的变量名列表
        /// </summary>
        /// <returns></returns>
        public string[] GetHeaderNames()
        {
            string[] result = new string[_metadata.Count];
            int i = 0;
            foreach (Metadata metadata in _metadata)
            {
                result[i] = metadata.Name;
                i++;
            }
            return result;
        }

        /// <summary>
        /// 获取当前所有表格配置的文件名
        /// </summary>
        /// <returns></returns>
        public string[] GetTablesNames()
        {
            string[] result = new string[_globalTables.Count];
            for (int i = 0; i < _globalTables.Count; i++)
            {
                result[i] = _globalTables[i].Name;
            }
            return result;
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

        /// <summary>
        /// 清空集合中的所有配置对象
        /// </summary>
        public void Clear()
        {
            _dmsMetadata.Clear();
            _manipulations.Clear();
            _metadata.Clear();
            _globalTables.Clear();
            _scripts.Clear();

        }

    }
}
