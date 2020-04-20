using System;

namespace Comm100.Framework.Exceptions
{
    public class FileKeyExistsException :  Exception
    {
        public FileKeyExistsException() : base("filekey already exisits.") { }
    }
}