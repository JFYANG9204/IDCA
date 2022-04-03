using IDCA.Bll.Template;
using System;
using System.IO;

namespace IDCA.Bll.Spec
{
    internal class FileHelper
    {
        /// <summary>
        /// 判断文件夹是否存在，如果不存在，创建新的文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void FolderExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 将字符串写入文本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteToFile(string filePath, string content)
        {
            try
            {
                FolderExist(Path.GetDirectoryName(filePath) ?? filePath);
                StreamWriter stream = File.CreateText(filePath);
                stream.WriteLine(content);
                stream.Close();
                Logger.Message(Messages.FileWriteSuccess, filePath);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, ExceptionMessages.FileWriteError, filePath);
            }
        }

        /// <summary>
        /// 向当前文件模板写入内容，并写入到具体文件
        /// </summary>
        /// <param name="template"></param>
        /// <param name="content"></param>
        public static void WriteToFile(string projectPath, FileTemplate? template, string content = "")
        {
            if (template == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(content))
            {
                template.AppendLine(content);
            }
            WriteToFile(Path.Combine(projectPath, template.Directory, template.FileName), template.Exec());
        }

    }
}
