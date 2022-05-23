using IDCA.Model;
using IDCA.Model.Spec;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            string fieldText = "A1[{_1}].Slice[..].Column[{].";
            field.FromString(fieldText);
            string export = field.Export();
            Console.WriteLine(export);
            Assert.AreEqual(export, "A1[{_1}].Slice[..].Column");
        }

        [TestMethod]
        public void AxisTest()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Features\Templates.xml");
            SpecDocument spec = new("", path, new());
            var manipulation = spec.Manipulations.NewObject();
            manipulation.Axis.AppendTextElement();
            manipulation.Axis.AppendBaseElement("Base : Total Respondent", "true");
            manipulation.Axis.AppendTextElement();
            manipulation.Axis.AppendAllCategory();
            manipulation.Axis.AppendTextElement();
            manipulation.Axis.AppendSubTotal("Sigma");
            manipulation.Axis.AppendMean("Mean").Suffix.AppendIsHidden(true);
            var result = manipulation.Axis.ToString();
            Console.WriteLine(result);
        }

        [TestMethod]
        public void TypeTest()
        {
            Type keyType = typeof(SpecConfigKeys);
            var propertyInfos = keyType.GetFields();
            foreach (var propertyInfo in propertyInfos)
            {
                Console.WriteLine(propertyInfo.Name);
            }
            Console.WriteLine("End");
        }

        [TestMethod]
        public void AxisFromStringTest()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Features\Templates.xml");
            SpecDocument spec = new("", path, new());
            var manipulation = spec.Manipulations.NewObject();
            string axisExpression = "{e1 '' text(), base 'Base : Total Respondent' base('True'), e2 '' text(), .., e3 '' text(), sigma 'Sigma' subtotal(), mean 'Mean' mean() [Ishidden=True, Isfixed=True]}";
            manipulation.Axis.FromString(axisExpression);
            Console.WriteLine(manipulation.Axis.ToString());
        }

    }
}
