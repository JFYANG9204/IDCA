﻿using IDCA.Bll.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace IDCA.Test
{
    [TestClass]
    public class TemplateTest
    {
        [TestMethod]
        public void LoadTemplate()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Features\Templates.xml");
            TemplateCollection loader = new();
            loader.Load(path);
            FunctionTemplate? function = loader.TryGet<FunctionTemplate, FunctionTemplateFlags>(FunctionTemplateFlags.TableGridSlice);
            string result = function?.Exec() ?? string.Empty;
            Console.WriteLine("End");
        }
    }
}
