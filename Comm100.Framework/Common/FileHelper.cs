using Comm100.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Common
{
    public class FileHelper
    {
        public static bool CheckFileNameLegitimacy(string fileName, byte[] content, string[] blackList)
        {
            string fileExtension = getFileExtension(fileName);
            string realExtension = getFileExtensionByFileData(content);
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

        private static string getFileExtensionByFileData(byte[] content)
        {
            string fileClass = "";
            try
            {
                for (int i = 0; i < 2; i++)
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

        private static String getFileExtension(String fileName)
        {
            int startIndex = fileName.LastIndexOf('.');
            int fileNameSplit = fileName.LastIndexOf('\\');
            if (fileNameSplit < startIndex)
            {
                return fileName.Substring(startIndex, (fileName.Length - startIndex)).ToUpper();
            }
            throw new FileNotAllowedException();
        }
    }
}
