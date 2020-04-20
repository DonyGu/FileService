using log4net;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Common
{
    public class InitRepository
    {
        public static ILoggerRepository LoggerRepository { get; set; }
    }
    public class LogHelper
    {
        //全局异常错误记录持久化
        private static readonly ILog logError = LogManager.GetLogger(InitRepository.LoggerRepository.Name, "loggerError");
        //自定义操作记录
        private static readonly ILog logInfo = LogManager.GetLogger(InitRepository.LoggerRepository.Name, "loggerInfo");
        #region 全局异常错误记录持久化
        /// <summary>
        /// 全局异常错误记录持久化
        /// </summary>
        /// <param name="throwMsg"></param>
        /// <param name="ex"></param>
        public static void ErrorLog(string throwMsg, Exception ex)
        {
            string errorMsg = string.Format("[ThrowMessage]:{0} \r\n[ExceptionType]:{1} \r\n[ExceptionMessage]:{2} \r\n[Stack]:{3}", new object[] { throwMsg,
                ex.GetType().Name, ex.Message, ex.StackTrace });
            logError.Error(errorMsg);
        }
        public static void ErrorLog(string throwMsg)
        {
            string errorMsg = string.Format("[ThrowMessage]:{0} \r\n", new object[] { throwMsg });
            logError.Error(errorMsg);
        }
        #endregion
        #region 自定义操作记录
        /// <summary>
        /// 自定义操作记录
        /// </summary>
        /// <param name="throwMsg"></param>
        public static void WriteLog(string throwMsg)
        {
            string errorMsg = string.Format("[ThrowMessage]:{0} \r\n", new object[] { throwMsg });
            logError.Info(errorMsg);
        }
        #endregion
    }
}
