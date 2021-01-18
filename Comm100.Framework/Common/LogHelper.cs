using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Common
{
    public class LogHelper
    {
        public static Logger logger = LogManager.GetLogger("FileService");

        public static void Error(string message)
        {
            logger.Error(message);
        }

        public static void Debug(string message)
        {
            logger.Debug(message);
        }

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Error(Exception exp)
        {
            logger.Error(exp);
        }
        public static void Error(Exception exp, string message)
        {
            logger.Error(exp, message);
        }

        public static Action CatchException(Action fn)
        {
            return () =>
            {
                try
                {
                    fn();
                }
                catch (Exception exp)
                {
                    Error(exp);
                }
            };
        }
    }
}
