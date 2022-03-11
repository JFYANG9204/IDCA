using IDCA.Bll.MDMDocument;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IDCA.Test
{
    [TestClass]
    public class MDMDocumentTest
    {
        [TestMethod]
        public void LoadDocument()
        {
            string path = @"D:\Program\C#\IDCA\IDCA.Test\Features\77403614_WT_FINAL.mdd";
            MDMDocument document = new();
            document.Open(path);
            var memo = GC.GetTotalMemory(true);
            Console.WriteLine("End");
        }
    }
}