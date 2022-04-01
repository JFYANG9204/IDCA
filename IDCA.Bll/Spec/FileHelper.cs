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

    }
}
