
using System;

namespace IDCA.Bll.Template
{
    public class Template : ITemplate
    {
        protected Template(TemplateType type)
        {
            _type = type;
            _parameters = new(this);
            _content = string.Empty;
        }

        readonly TemplateType _type;
        readonly TemplateParameters<TemplateParameter> _parameters;
        string _content;

        public TemplateType Type => _type;
        public ITemplateParameters<TemplateParameter> Parameters => _parameters;
        public string Content { get => _content; set => _content = value; }

        public virtual string Exec()
        {
            string result = _content;
            foreach (TemplateParameter param in _parameters)
            {
                result = result.Replace(param.ToString(), param.GetValue(), StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }

    public class FileTemplate : Template, IFileTemplate
    {
        public FileTemplate() : base(TemplateType.File) 
        {
            _directory = "";
            _fileName = "";
        }

        string _directory;
        string _fileName;

        public string Directory { get => _directory; set => _directory = value; }
        public string FileName { get => _fileName; set => _fileName = value; }
    }


    public class ScriptTemplate : Template, IScriptTemplate
    {
        public ScriptTemplate() : base(TemplateType.Script)
        {
            _flag = ScriptTemplateFlags.None;
            _indentLevel = 0;
        }

        ScriptTemplateFlags _flag;
        int _indentLevel;

        public ScriptTemplateFlags Flag { get => _flag; set => _flag = value; }
        public int IndentLevel { get => _indentLevel; set => _indentLevel = value; }

        public override string Exec()
        {
            string result = string.Empty;
            if (_flag == ScriptTemplateFlags.IfStatement && Parameters.Count >= 2)
            {
                var parameters = new (string, string)[Parameters.Count];
                for (int i = 0; i < Parameters.Count; i++)
                {
                    parameters[i] = Parameters[i].GetTupleValue();
                }
            }
            return result;
        }
    }

}
