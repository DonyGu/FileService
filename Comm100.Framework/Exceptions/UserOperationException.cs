using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Exceptions
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class UserOperationException : Exception
    {
        public UserOperationException() { }
        public UserOperationException(string message) : base(message) { }
        public UserOperationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
