using System;
using System.Text;

namespace IDCA.Bll.Spec
{

    public class ScriptCollection : SpecObject
    {
        public ScriptCollection(SpecDocument document) : base(document)
        {
            _objectType = SpecObjectType.Collection;
        }

        Script[] _scripts = Array.Empty<Script>();

        /// <summary>
        /// 向当前集合的末尾添加已存在的Script对象
        /// </summary>
        /// <param name="script"></param>
        public void Add(Script script)
        {
            Array.Resize(ref _scripts, _scripts.Length + 1);
            _scripts[^1] = script;
        }

        /// <summary>
        /// 创建新的Script对象，并将其加入到当前集合，最后返回新创建的对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Script NewScript(ScriptType type)
        {
            Script script = type switch
            {
                ScriptType.Field => new FieldScript(this),
                ScriptType.CallExpression => new CallExpressionScript(this),
                ScriptType.BinaryExpression => new BinaryExpressionScript(this),
                ScriptType.UnaryExpression => new UnaryExpressionScript(this),
                ScriptType.BlockStatement => new BlockStatementScript(this),
                ScriptType.IfStatement => new IfStatementScript(this),
                ScriptType.ForStatement => new ForStatementScript(this),
                ScriptType.ForEachStatement => new ForEachStatementScript(this),
                ScriptType.WhileStatement => new WhileStatementScript(this),
                ScriptType.DoWhileStatement => new DoWhileStatementScript(this),
                ScriptType.DoUntilStatement => new DoUntilStatementScript(this),
                _ => new EmptyScript(this),
            };
            Add(script);
            return script;
        }

        /// <summary>
        /// 导出集合的所有内容到单个字符串
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            StringBuilder builder = new();

            foreach (Script script in _scripts)
            {
                builder.AppendLine();
                builder.AppendLine($"'***************{script.Info}***************");
                builder.AppendLine(script.Export());
                builder.AppendLine();
            }

            return builder.ToString();
        }

    }

}
