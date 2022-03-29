using IDCA.Bll.MDM;
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
            Console.WriteLine("End");
        }
    }
}