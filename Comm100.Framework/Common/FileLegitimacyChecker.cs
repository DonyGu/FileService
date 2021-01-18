using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Comm100.Framework.Common
{
    /// <summary>
    /// 文件合法性检查器
    /// </summary>
    public class FileLegitimacyChecker
    {
        /// <summary>
        /// 最多解压层数
        /// </summary>
        private const int unZipLimitLevel = 3;
        /// <summary>
        /// 当前前服务所在路径
        /// </summary>
        private static string serverPath = "";// HttpContext.Current.Server.MapPath("temp");
        /// <summary>
        /// 非法文件后缀
        /// </summary>
        string[] blackList;
        /// <summary>
        /// 压缩文件后缀
        /// </summary>
        string[] zipFileExtension = { ".zip" };
        /// <summary>
        /// 设置一个默认文件夹，避免删除了其他文件
        /// </summary>
        string temprorayFolderName = "xx_x_xx";
        string temprorayPath = "C:\\";
        /// <summary>
        /// 构造方法
        /// </summary>
        public FileLegitimacyChecker(string[] blackList, string _temprorayPath = null)
        {
            this.blackList = blackList;
            if (string.IsNullOrEmpty(_temprorayPath))
            {
                serverPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\temp";
                string dateString = DateTime.UtcNow.ToString("yyyy-MM-dd");
                temprorayPath = String.Format("{0}\\{1}", serverPath, dateString);
            }
            else
                temprorayPath = _temprorayPath;
        }

        #region 私有内容
        /// <summary>
        /// 文件合法类型
        /// </summary>
        enum FileLegitimacy : uint
        {
            /// <summary>
            /// 合法
            /// </summary>
            legitimate,
            /// <summary>
            /// 非法
            /// </summary>
            wrongful,
            /// <summary>
            /// zip 待确定
            /// </summary>
            zip
            ///// <summary>
            ///// rar 待确定
            ///// </summary>
            //rar
        }

        /// <summary>
        /// zip 文件结构体，用来保存压缩包解压出来的文件信息。
        /// </summary>
        struct ZipFile
        {
            /// <summary>
            /// 文件全路径
            /// </summary>
            public string filePath;
            /// <summary>
            /// 文件类型
            /// </summary>
            public FileLegitimacy fileType;

            public ZipFile(string _filePath, FileLegitimacy _fileType)
            {
                filePath = _filePath;
                fileType = _fileType;
            }
        }

        /// <summary>
        /// 通过解压Zip压缩包文件获取文件列表，包括文件夹内的所有文件
        /// </summary>
        /// <param name="sourcePath">待解压的压缩文件</param>
        /// <param name="fileLists">被解压出来的文件列表</param>
        /// <returns>解压是否成功</returns>
        private bool DecompressZipFile(string sourcePath, ref List<string> fileLists)
        {
            try
            {
                string extractPath = FileHelper.GetPathFromFullFileName(sourcePath);
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(sourcePath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(extractPath + "\\" + directoryName);
                        }
                        if (fileName != String.Empty)
                        {
                            string targetFilePath = extractPath + "\\" + directoryName + "\\" + fileName;
                            //if (fileName.ToUpper().EndsWith(".ZIP"))//|| fileName.ToUpper().EndsWith(".RAR")
                            //{
                            ZipInputStreamToFile(s, targetFilePath);
                            //}
                            fileLists.Add(targetFilePath);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// zip 解压文件,将ZipInputStream 对应的文件解压为filePath 
        /// </summary>
        /// <param name="stream">压缩的文件流</param>
        /// <param name="filePath">解压后存放的文件路径，现在做法是在工程目录下创建临时文件夹做解压操作，使用完成后删除。</param>
        /// <returns>解压是否成功</returns>
        private bool ZipInputStreamToFile(ZipInputStream stream, string filePath)
        {
            using (FileStream streamWriter = File.Create(filePath))
            {
                int size = 2048;
                byte[] data = new byte[2048];
                while (true)
                {
                    size = stream.Read(data, 0, data.Length);
                    if (size > 0)
                    {
                        streamWriter.Write(data, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 检查文件的合法性
        /// </summary>
        /// <param name="fullFileName">待检查的文件完整名（带路径）</param>
        /// <returns>文件是否合法</returns>
        private FileLegitimacy FileNameLegitimacyCheck(string fullFileName)
        {
            var fileData = FileHelper.File2Byte(fullFileName, 2);
            return FileNameLegitimacyCheck(fullFileName, fileData);
        }

        /// <summary>
        /// 检查文件的合法性
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileData">文件数据</param>
        /// <returns></returns>
        private FileLegitimacy FileNameLegitimacyCheck(string fileName, byte[] fileData)
        {
            //先检查文件的实际类型是否合法，如果获取不到文件的真实类型就不做这个检查
            var realExtension = FileHelper.GetFileExtensionByFileData(fileData);
            var isGetTheRealExtension = !string.IsNullOrEmpty(realExtension);
            if (isGetTheRealExtension &&
                FileNameLegitimacyCheckByExtension(realExtension) == FileLegitimacy.wrongful)
            {
                return FileLegitimacy.wrongful;
            }

            //检查文件的扩展名是否合法.
            var fileExtension = FileHelper.GetFileExtension(fileName);
            return FileNameLegitimacyCheckByExtension(fileExtension);
        }


        /// <summary>
        /// 检查后缀名是否合法
        /// </summary>
        /// <param name="fileExtension">后缀名</param>
        /// <returns>文件合法类型</returns>
        private FileLegitimacy FileNameLegitimacyCheckByExtension(string fileExtension)
        {
            if (blackList != null && blackList.Any(b => b.Replace("*", "").ToUpper() == fileExtension))
            {
                return FileLegitimacy.wrongful;
            }
            return (fileExtension == zipFileExtension[0].ToUpper()) ? FileLegitimacy.zip : FileLegitimacy.legitimate;
        }

        /// <summary>
        /// 检查文件列表的合法性
        /// </summary>
        /// <param name="files"></param>
        /// <param name="unZipLevelCounter"></param>
        /// <returns></returns>
        private bool FileListLegitimacyCheck(List<string> files, int unZipLevelCounter)
        {
            var zipFiles = new List<ZipFile>();

            foreach (var file in files)
            {
                var fileLegitimacy = FileNameLegitimacyCheck(file);
                if (fileLegitimacy == FileLegitimacy.wrongful)
                    return false;
                else if (fileLegitimacy != FileLegitimacy.legitimate)
                {
                    zipFiles.Add(new ZipFile(file, fileLegitimacy));
                }
            }

            return zipFiles.Count <= 0 || ZipFileListLegitimacyCheck(zipFiles, unZipLevelCounter);
        }

        /// <summary>
        /// 检查压缩文件列表的有效性，如果文件解压失败，也认为文件无效！
        /// </summary>
        /// <param name="zipFiles"></param>
        /// <returns></returns>
        private bool ZipFileListLegitimacyCheck(List<ZipFile> zipFiles, int unZipLevelCounter)
        {
            unZipLevelCounter++;
            if (unZipLevelCounter > unZipLimitLevel)
                return false;
            foreach (ZipFile zipFile in zipFiles)
            {
                var filePaths = new List<string>();

                bool isUnZipSuccess = DecompressZipFile(zipFile.filePath, ref filePaths);
                return isUnZipSuccess ?
                    FileListLegitimacyCheck(filePaths, unZipLevelCounter)
                    : isUnZipSuccess;

            }
            return true;
        }

        /// <summary>
        /// 创建并返回临时路径
        /// </summary>
        /// <returns></returns>
        private string GetTemporaryFolderPath()
        {
            temprorayFolderName = Guid.NewGuid().ToString();
            string temPath = temprorayPath + "\\" + temprorayFolderName;
            if (!Directory.Exists(temPath))
                Directory.CreateDirectory(temPath);
            return temPath;
        }


        /// <summary>
        /// 删除用于解压的临时文件夹，在删除当前操作的目录外，也要删除48小时前创建的可能遗留下来没有删除的文件夹。
        /// </summary>
        /// <param name="temPath">当前操作临时文件的临时文件夹</param>
        /// <returns></returns>
        private bool DeleteTemporaryFolders(string temPath)
        {
            try
            {
                FileHelper.DeleteFolderWithFiles(temPath);
                serverPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\temp";
                List<string> directorys = FileHelper.GetFirstLevelDirectory(serverPath);
                DateTime beforYesterday = DateTime.Now.AddDays(-2);
                foreach (var dir in directorys)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    //string folderName = dirInfo.Name;
                    //string[] aDate = folderName.Split('-');
                    //DateTime createDate = new DateTime(Convert.ToInt32(aDate[0]), Convert.ToInt32(aDate[1]), Convert.ToInt32(aDate[2]));
                    if (dirInfo.CreationTimeUtc < beforYesterday)
                    {
                        FileHelper.DeleteFolderWithFiles(dir);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 检查文件流的合法性
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public bool FileLegitimacyCheck(string fileName, byte[] fileContent)
        {
            string fileExtension = FileHelper.GetFileExtension(fileName);
            if (!(FileNameLegitimacyCheckByExtension(fileExtension) == FileLegitimacy.wrongful))
            {
                string temPath = GetTemporaryFolderPath();
                try
                {
                    string fileTargetPath = temPath + "\\" + Guid.NewGuid().ToString() + Path.GetExtension(fileName).ToLower();
                    int unZipLevelCounter = 0;
                    FileHelper.SaveFileByteToFile(fileTargetPath, fileContent);
                    List<string> fileList = new List<string>();// { fileTargetPath };
                    fileList.Add(fileTargetPath);
                    return FileListLegitimacyCheck(fileList, unZipLevelCounter);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"FileLegitimacyCheck:{ex.Message}");
                    return false;
                }
                finally
                {
                    DeleteTemporaryFolders(temPath);
                }
            }
            return false;

        }

        /// <summary>
        /// 检查已经保存到服务器本地文件的合法性,解压的临时文件应该放到临时文件夹中去，与原文件夹无关。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool FileLegitimacyCheck(string filePath)
        {
            bool result = false;
            string fileExtension = FileHelper.GetFileExtension(filePath);
            if (!(FileNameLegitimacyCheckByExtension(fileExtension) == FileLegitimacy.wrongful))
            {
                string temPath = GetTemporaryFolderPath();
                try
                {
                    string fileTargetPath = temPath + "\\" + FileHelper.GetFileNameFromFilePath(filePath);// Path.GetFileName(filePath);//
                    if (!File.Exists(fileTargetPath))
                        File.Copy(filePath, fileTargetPath);
                    int unZipLevelCounter = 0;
                    List<string> fileList = new List<string>();
                    fileList.Add(fileTargetPath);
                    result = FileListLegitimacyCheck(fileList, unZipLevelCounter);
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    DeleteTemporaryFolders(temPath);
                }
            }
            return result;
        }

        /// <summary>
        /// 检查已经保存到服务器本地文件的合法性,解压的临时文件应该放到临时文件夹中去，与原文件夹无关。
        /// 文件本身没有后缀名，displayName带后缀名。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="disPlayName"></param>
        /// <returns></returns>
        public bool FileLegitimacyCheck(string filePath, string disPlayName)
        {
            // 先检查文件的扩展名是否合法.
            var fileExtension = FileHelper.GetFileExtension(disPlayName);
            var isLegitimacy = FileNameLegitimacyCheckByExtension(fileExtension) != FileLegitimacy.wrongful;
            if (!isLegitimacy) { return false; }

            //检查文件的实际类型是否合法(文件的实际类型和扩展名可能是不相同的)
            var temPath = GetTemporaryFolderPath();
            try
            {
                var fileTargetPath = temPath + "\\" + Guid.NewGuid().ToString() + Path.GetExtension(disPlayName).ToLower();
                File.Copy(filePath, fileTargetPath);
                var unZipLevelCounter = 0;
                var fileList = new List<string> { fileTargetPath }; // { fileTargetPath };

                isLegitimacy = FileListLegitimacyCheck(fileList, unZipLevelCounter);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Error check file legitimacy.");
                isLegitimacy = false;
            }
            finally
            {
                DeleteTemporaryFolders(temPath);
            }
            return isLegitimacy;
        }

        public bool FileLegitimacyCheckNew(string disPlayName)
        {
            bool result = false;
            string fileExtension = FileHelper.GetFileExtension(disPlayName);
            if (!(FileNameLegitimacyCheckByExtension(fileExtension) == FileLegitimacy.wrongful))
            {
                return true;
            }
            return result;
        }
        #endregion
    }
}
