using System;

namespace Comm100.Framework.Exceptions
{
    public class FileNotAllowedException : Exception
    {
        public FileNotAllowedException() : base("The file format is not supported. ") { }
    }
}