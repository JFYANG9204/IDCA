using IDCA.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDCA.Test
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void ReadField()
        {
            string fieldText = "A1[{_1}].Slice[..].Column[{].";
            string[] fields = StringHelper.ReadFieldNames(fieldText);
            Assert.AreEqual(3, fields.Length);
            foreach (var item in fields)
            {
                System.Console.WriteLine(item);
            }
        }

        [TestMethod]
        public void Convert()
        {
            var e = AxisNetType.CombineBeforeAllCategory;
            var t = e.GetType();
            var f = t.GetField(e.ToString());
            System.Console.WriteLine(AxisNetType.CombineBetweenAllCategoryAndSigma.ToString());
        }

    }
}
