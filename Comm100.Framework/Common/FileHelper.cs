using Comm100.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Comm100.Framework.Common
{
    public class FileHelper
    {
        public static bool CheckFileNameLegitimacy(string fileName, byte[] content, string[] blackList)
        {
            string fileExtension = GetFileExtension(fileName);
            string realExtension = GetFileExtensionByFileData(content);
            if (!string.IsNullOrEmpty(realExtension))
            {
                if (!CheckFileNameLegitimacyCheckByExtension(realExtension, blackList))
                {
                    return false;
                }
            }
            return CheckFileNameLegitimacyCheckByExtension(fileExtension, blackList);
        }

        private static bool CheckFileNameLegitimacyCheckByExtension(string fileExtension, string[] blackList)
        {
            if (blackList != null)
            {
                foreach (var item in blackList)
                {
                    if (item.Replace("*", "").ToUpper() == fileExtension)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool FileExtensionCompare(string fileExtension, string realExtension)
        {
            switch (realExtension)
            {
                case "":
                    return true;
                case ".EXE":
                    return fileExtension.Equals(".EXE") || fileExtension.Equals(".COM") || fileExtension.Equals(".DLL");
                case ".DOC":
                    return fileExtension.Equals(".DOC") || fileExtension.Equals(".XLS") || fileExtension.Equals(".PPT")
                        || fileExtension.Equals("WPS") || fileExtension.Equals(".MSG");
                case ".DOCX":
                    return fileExtension.Equals(".DOCX") || fileExtension.Equals(".PPTX")
                        || fileExtension.Equals(".XLSX") || fileExtension.Equals(".ZIP");
                case ".ASP":
                    return fileExtension.Equals(".ASPX") || fileExtension.Equals("ASP") || fileExtension.Equals(".SQL")
                        || fileExtension.Equals(".CSV") || fileExtension.Equals(".TXT");
                case ".HTM":
                    return fileExtension.Equals(".HTM") || fileExtension.Equals("HTML");
                case ".MDB":
                    return fileExtension.Equals(".MDB") || fileExtension.Equals(".ACCDB");
                case ".RDP":
                    return fileExtension.Equals(".RDP") || fileExtension.Equals(".SQL") || fileExtension.Equals(".CSV");
                case ".TXT":
                    return fileExtension.Equals(".TXT") || fileExtension.Equals(".SQL") || fileExtension.Equals("CSV");
                case ".JPG":
                case ".GIF":
                case ".PNG":
                case "BMP":
                    return fileExtension.Equals(".JPG") || fileExtension.Equals(".JPEG") || fileExtension.Equals(".GIF")
                        || fileExtension.Equals(".PNG") || fileExtension.Equals("BMP");
                default:
                    return fileExtension.Equals(realExtension);
            }
        }

        /// <summary>
        /// 从完整的文件路径中获取文件所在文件夹路径
        /// </summary>
        /// <param name="filePath">完整文件名</param>
        /// <returns>文件所在文件夹路径</returns>
        public static string GetPathFromFullFileName(string filePath)
        {
            int endIndex = filePath.LastIndexOf('\\');
            if (endIndex != -1)
            {
                return filePath.Substring(0, endIndex);
            }
            throw new Exception("Incorrect file path.");
        }

        public static string GetFileExtensionByFileData(byte[] content)
        {
            string fileClass = "";
            try
            {
                int length = Math.Min(2, content.Length);
                for (int i = 0; i < length; i++)
                {
                    fileClass += content[i].ToString();
                }
                return GetFileExtensionWithFileClassStream(fileClass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetFileExtensionWithFileClassStream(string fileClass)
        {
            string fileExtension = "";
            switch (fileClass)
            {
                case "255216":
                    fileExtension = ".JPG";
                    break;
                case "7173":
                    fileExtension = ".GIF";
                    break;
                case "13780":
                    fileExtension = ".PNG";
                    break;
                case "6677":
                    fileExtension = ".BMP";
                    break;
                case "208207":
                    fileExtension = ".DOC";//xis,ppt,wps
                    break;
                case "8075":
                    fileExtension = ".DOCX";//ptx,xlsx,zip
                    break;
                case "8297":
                    fileExtension = ".RAR";
                    break;
                case "7790":
                    fileExtension = ".EXE";
                    break;
                case "3780":
                    fileExtension = ".PDF";
                    break;
                case "4946":
                case "104116":
                case "5150":
                case "210187":
                    fileExtension = ".TXT";
                    break;
                case "239187":
                    fileExtension = ".ASP";//aspx,asp,sql
                    break;
                case "6063":
                    fileExtension = ".XML";
                    break;
                case "6033":
                    fileExtension = ".HTM";
                    break;
                case "4742":
                case "119105":
                    fileExtension = ".JS";
                    break;
                case "01":
                    fileExtension = ".MDB";
                    break;
                case "5566":
                    fileExtension = ".PSD";
                    break;
                case "255254":
                    fileExtension = ".RDP";
                    break;
                case "10056":
                    fileExtension = ".DT";
                    break;
                case "64101":
                    fileExtension = ".BAT";
                    break;
                case "4059":
                    fileExtension = ".SGF";
                    break;
                case "46105":
                    fileExtension = ".CSS";
                    break;
                case "117115":
                    fileExtension = ".CS";
                    break;
                case "7384":
                    fileExtension = ".CHM";
                    break;
                case "70105":
                    fileExtension = ".LOG";
                    break;
                case "8269":
                    fileExtension = ".REG";
                    break;
                case "6395":
                    fileExtension = ".HLP";
                    break;
                case "48130":
                    fileExtension = ".P12";
                    break;
                default:
                    break;
            }
            return fileExtension;
        }


        /// <summary>
        /// 从文件读取byte数据
        /// </summary>
        /// <param name="filePath">完整文件路径</param>
        /// <param name="maxDataLength">读取最大长度，如果为默认的0 则，不限制</param>
        /// <returns>byte数据</returns>
        public static byte[] File2Byte(string filePath, long maxDataLength = 0)
        {
            FileStream fileStream = null;
            byte[] fileByte = new byte[0];
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(fileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);
                long byteLength = (maxDataLength < 1) ? r.BaseStream.Length :
                    ((maxDataLength > r.BaseStream.Length) ? r.BaseStream.Length : maxDataLength);
                fileByte = r.ReadBytes((int)byteLength);
                return fileByte;
            }
            catch
            {
                return fileByte;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        public static String GetFileExtension(String fileName)
        {
            int startIndex = fileName.LastIndexOf('.');
            int fileNameSplit = fileName.LastIndexOf('\\');
            if (fileNameSplit < startIndex)
            {
                return fileName.Substring(startIndex, (fileName.Length - startIndex)).ToUpper();
            }
            throw new FileNotAllowedException();
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileNameFromFilePath(string filePath)
        {
            int startIndex = filePath.LastIndexOf('\\') + 1;
            if (startIndex != -1)
            {
                return Guid.NewGuid().ToString() + Path.GetExtension(filePath).ToLower(); //filePath.Substring(startIndex, filePath.Length - startIndex).ToLower();
            }
            throw new Exception("Please make sure that the file name is included in the file path.");
        }

        /// <summary>
        /// 递归删除临时文件及文件夹,包括文件夹本身
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DeleteFolderWithFiles(string path)
        {
            try
            {
                string[] files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                string[] folders = Directory.GetDirectories(path);
                foreach (var folder in folders)
                {
                    DeleteFolderWithFiles(folder);
                }
                Directory.Delete(path);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取目标文件夹下第一层文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetFirstLevelDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            List<string> directorys = new List<string>();
            foreach (DirectoryInfo folder in dir.GetDirectories())
            {
                directorys.Add(folder.FullName);
            }
            return directorys;
        }

        /// <summary>
        /// 将文件字节数组保存到服务器本地
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <param name="temprorayFolderName"></param>
        /// <returns></returns>
        public static bool SaveFileByteToFile(string fileName, byte[] fileContent)
        {
            using (MemoryStream m = new MemoryStream(fileContent))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    try
                    {
                        m.WriteTo(fs);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
