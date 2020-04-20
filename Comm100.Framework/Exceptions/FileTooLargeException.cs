using System;

namespace Comm100.Framework.Exceptions
{
    public class FileTooLargeException : Exception
    {
        public FileTooLargeException(int filesize) : base($"The attachment size exceeds the allowable limit of {GetLimit(filesize)}.") { }

        private static string GetLimit(int filesize)
        {
            if (filesize < 1024)
            {
                return $"{filesize} B";
            }
            else if (filesize < 1024 * 1024)
            {
                return $"{filesize / 1024} KB";
            }
            else if (filesize < 1024 * 1024 * 1024)
            {
                return $"{filesize / 1024 / 1024} MB";
            }
            else
            {
                return $"{filesize / 1024 / 1024 / 1024} GB";
            }
        }
    }
}