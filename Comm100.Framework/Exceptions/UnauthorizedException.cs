using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message= "Access denied") : base(message) { }

    }
}
