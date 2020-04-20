using System;

namespace Comm100.Framework.Exceptions
{
    public class FileKeyNotFoundException : Exception
    {
        public FileKeyNotFoundException() : base("fileKey not found.") { }
    }
}