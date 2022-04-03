using IDCA.Bll.Spec;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace IDCA.Test
{
    [TestClass]
    public class SpecTest
    {
        [TestMethod]
        public void FieldFromString()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Features\Templates.xml");
            SpecDocument spec = new("", path, new());
            FieldScript field = (FieldScript)spec.Scripts.NewScript(ScriptType.Field);
            string fieldText = "A1[{_1}].Slice[2].Slice[";
            field.FromString(fieldText);
            string export = field.Export();
            System.Console.WriteLine(export);
        }
    }
}
