using IDCA.Model.MDM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace IDCA.Test
{
    [TestClass]
    public class MDMDocumentTest
    {
        [TestMethod]
        public void LoadDocument()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Features\77403614_WT_FINAL.mdd");
            MDMDocument document = new();
            document.Open(path);
            Field? c1 = document.Fields["Cloop[..].C1"];
            Assert.IsNotNull(c1);
            Console.WriteLine("End");
        }
    }
}