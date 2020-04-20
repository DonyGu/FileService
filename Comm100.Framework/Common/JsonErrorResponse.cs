using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Common
{
    public class JsonErrorResponse
    {
        /// <summary>
        /// error code
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 生产环境的消息
        /// </summary>
        public string message { get; set; }
    }
}
