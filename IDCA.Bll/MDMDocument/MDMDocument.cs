
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
        string _context = string.Empty;
        string _language = string.Empty;
        string _script = string.Empty;
        string _xml = string.Empty;

        readonly Properties _properties;
        readonly Properties _templates;
        readonly DataSources _dataSources;
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
        public string Context { get => _context; private set => _context = value; }
        public string Language { get => _language; private set => _language = value; }

        public IMDMObject Parent => this;
        public IMDMDocument Document => this;

        public IProperties<Property> Properties => _properties;
        public Properties Templates => _templates;
        public DataSources DataSources => _dataSources;
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


        }
    }
}
