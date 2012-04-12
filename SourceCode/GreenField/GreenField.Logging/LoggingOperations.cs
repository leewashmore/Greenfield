using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace GreenField.Logging
{
    public class LoggingOperations
    {
        #region Private Fields
        /// <summary>
        /// ILog instance
        /// </summary>
        private static readonly ILog LOG = LogManager.GetLogger(typeof(LoggingOperations));
        #endregion

        #region Application Programming Interface
        /// <summary>
        /// Log to File
        /// </summary>
        /// <param name="message">Logging Message</param>
        /// <param name="category">Debug/Info/Warn/Exception</param>
        /// <param name="priority">Low/Medium/High</param>
        public void LogToFile(string message, string category, string priority)
        {
            string logLevel = "Info";

            if (category.ToLower().Equals("debug"))
                logLevel = "Debug";

            if (category.ToLower().Equals("info"))
                logLevel = "Info";

            if (category.ToLower().Equals("exception") && priority.ToLower().Equals("low"))
                logLevel = "Warn";

            if (category.ToLower().Equals("exception") && priority.ToLower().Equals("medium"))
                logLevel = "Error";

            if (category.ToLower().Equals("exception") && priority.ToLower().Equals("high"))
                logLevel = "Fatal";

            if (category.ToLower().Equals("debug"))
                logLevel = "Debug";

            switch (logLevel)
            {
                case "Debug":
                    if (LOG.IsDebugEnabled)
                        LOG.Debug(message);
                    break;
                case "Info":
                    if (LOG.IsInfoEnabled)
                        LOG.Info(message);
                    break;
                case "Warn":
                    if (LOG.IsWarnEnabled)
                        LOG.Warn(message);
                    break;
                case "Error":
                    if (LOG.IsErrorEnabled)
                        LOG.Error(message);
                    break;
                case "Fatal":
                    if (LOG.IsFatalEnabled)
                        LOG.Fatal(message);
                    break;
                default:
                    LOG.Info(message);
                    break;
            }
        }

        /// <summary>
        /// Get Logging Level
        /// </summary>
        /// <returns>Logging Level</returns>
        public Int32 GetLoggingLevel()
        {
            if (LOG.IsDebugEnabled)
                return LogLevel.DEBUG_LEVEL;
            if (LOG.IsInfoEnabled)
                return LogLevel.INFO_LEVEL;
            if (LOG.IsWarnEnabled)
                return LogLevel.WARN_LEVEL;
            if (LOG.IsErrorEnabled)
                return LogLevel.ERROR_LEVEL;
            if (LOG.IsFatalEnabled)
                return LogLevel.FATAL_LEVEL;
            else
                return LogLevel.INFO_LEVEL;
        }
        #endregion
    }
}
