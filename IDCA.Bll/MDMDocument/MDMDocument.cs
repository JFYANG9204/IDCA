
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace IDCA.Bll.MDMDocument
{
    public class MDMDocument : IMDMDocument
    {
        public MDMDocument()
        {
            _properties = new Properties(this);
            _templates = new Properties(this);
            _dataSources = new DataSources(this);
            _labels = new Labels(this, this, "");
            _variables = new Variables(this);
            _fields = new Fields(this);
            _types = new Types(this);
            _pages = new Pages(this);
            _systemRoutings = new Routings(this);
            _routings = new Routings(this);
            _mapping = new Mapping(this);
            _languages = new Languages(this, "");
            _contexts = new Contexts(this, "");
            _labelTypes = new Contexts(this, "");
            _routingContexts = new Contexts(this, "");
            _scriptTypes = new Contexts(this, "");
            _categoryMap = new CategoryMap(this);
            _saveLogs = new SaveLogs(this);
        }

        string _url = string.Empty;
        string _createVersion = string.Empty;
        string _lastVersion = string.Empty;
        string _id = string.Empty;
        string _dataVersion = string.Empty;
        string _dataSubVersion = string.Empty;
        bool _systemVariable = false;
        bool _dbFilterValidation = false;
        string _context = string.Empty;
        string _language = string.Empty;
        string _script = string.Empty;
        string _xml = string.Empty;

        readonly Properties _properties;
        readonly Properties _templates;
        readonly DataSources _dataSources;
        readonly Labels _labels;
        readonly Variables _variables;
        readonly Fields _fields;
        readonly Types _types;
        readonly Pages _pages;
        readonly Routings _systemRoutings;
        readonly Routings _routings;
        readonly Mapping _mapping;
        readonly Languages _languages;
        readonly Contexts _contexts;
        readonly Contexts _labelTypes;
        readonly Contexts _routingContexts;
        readonly Contexts _scriptTypes;
        readonly CategoryMap _categoryMap;
        readonly List<string> _atoms = new();
        readonly SaveLogs _saveLogs;

        XDocument _xmlDocument = new();

        public string Url => _url;
        public string CreateVersion { get => _createVersion; private set => _createVersion = value; }
        public string LastVersion { get => _lastVersion; private set => _lastVersion = value; }
        public string Id { get => _id; private set => _id = value; }
        public string DataVersion { get => _dataVersion; private set => _dataVersion = value; }
        public string DataSubVersion { get => _dataSubVersion; private set => _dataSubVersion = value; }
        public bool SystemVariable { get => _systemVariable; private set => _systemVariable = value; }
        public bool DbFilterValidation { get => _dbFilterValidation; private set => _dbFilterValidation = value; }
        public string Context { get => _context; private set => _context = value; }
        public string Language { get => _language; private set => _language = value; }

        public IMDMObject Parent => this;
        public IMDMDocument Document => this;

        public Properties Properties => _properties;
        public Properties Templates => _templates;
        public DataSources DataSources => _dataSources;
        public Labels Labels => _labels;
        public Variables Variables => _variables;
        public Fields Fields => _fields;
        public Types Types => _types;
        public Pages Pages => _pages;
        public Routings SystemRoutings => _systemRoutings;
        public Routings Routings => _routings;
        public Mapping Mapping => _mapping;
        public Languages Languages => _languages;
        public Contexts Contexts => _contexts;
        public Contexts LabelTypes => _labelTypes;
        public Contexts RoutingContexts => _routingContexts;
        public Contexts ScriptTypes => _scriptTypes;
        public CategoryMap CategoryMap => _categoryMap;
        public List<string> Atoms => _atoms;
        public SaveLogs SaveLogs => _saveLogs;
        public string Script => _script;
        public string Xml => _xml;
        public MDMObjectType ObjectType => MDMObjectType.Unknown;


        public void Clear()
        {
            _properties.Clear();
            _templates.Clear();
            
        }

        public void Close()
        {
            Clear();
        }

        public void Open(string path)
        {
            _url = path;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            _xmlDocument = XDocument.Load(path);
            _xml = _xmlDocument.ToString();
            XElement? _xmlRoot = _xmlDocument.Root;

            if (_xmlRoot is null)
            {
                throw new Exception("文档格式不符合标准，未找到XML根节点。");
            }

            XElement? _xmlMDM = _xmlRoot.Element("{http://www.spss.com/mr/dm/metadatamodel/Arc 3/2000-02-04}metadata");

            if (_xmlMDM is null)
            {
                throw new Exception("文档格式不符合标准，未找到MDM根节点。");
            }

            // 载入基础属性
            _createVersion = XmlHelper.ReadPropertyStringValue(_xmlMDM, "mdm_createversion");
            _lastVersion = XmlHelper.ReadPropertyStringValue(_xmlMDM, "mdm_lastversion");
            _id = XmlHelper.ReadPropertyStringValue(_xmlMDM, "id");
            _dataVersion = XmlHelper.ReadPropertyStringValue(_xmlMDM, "data_version");
            _dataSubVersion = XmlHelper.ReadPropertyStringValue(_xmlMDM, "data_sub_version");
            _systemVariable = XmlHelper.ReadPropertyBoolValue(_xmlMDM, "systemvariable");
            _dbFilterValidation = XmlHelper.ReadPropertyBoolValue(_xmlMDM, "dbfiltervalidation");

            // 载入数据源配置
            XmlHelper.ReadDataSources(_dataSources, _xmlMDM);
            // 载入属性配置
            XmlHelper.ReadProperties(_properties, _xmlMDM);
            XmlHelper.ReadProperties(_templates, _xmlMDM);
            // 载入标签、上下文等基础配置
            XmlHelper.ReadLabels(_labels, _xmlMDM);
            XmlHelper.ReadLanguages(_languages, _xmlMDM);
            XmlHelper.ReadContexts(_contexts, _xmlMDM);
            XmlHelper.ReadContexts(_labelTypes, _xmlMDM);
            XmlHelper.ReadContexts(_scriptTypes, _xmlMDM);
            XmlHelper.ReadAtoms(_atoms, _xmlMDM);
            XmlHelper.ReadCategoryMap(_categoryMap, _xmlMDM);
            // 载入定义
            XElement? _xmlDefine = _xmlMDM.Element("definition");
            if (_xmlDefine != null)
            {
                XmlHelper.ReadTypes(_types, _xmlDefine);
                XmlHelper.ReadGloablVariables(_variables, _xmlDefine);
                XmlHelper.ReadGlobalPages(_pages, _xmlDefine);
            }
            // 载入Field定义
            XElement? _xmlDesign = _xmlMDM.Element("design");
            if (_xmlDesign != null)
            {
                XmlHelper.ReadClassFields(_fields, _xmlDesign);
            }
        }
    }
}
