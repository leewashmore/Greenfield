using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using log4net;
using System.ServiceModel.Activation;

namespace GreenField.Web.Services
{
    /// <summary>
    /// lass implementing Logging Operation Contracts
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LoggingOperations
    {
        #region Fields
        /// <summary>
        /// ILog instance
        /// </summary>
        private static readonly ILog LOG = LogManager.GetLogger(typeof(LoggingOperations));

        #region Log Levels
        //Logging Levels
        private const Int32 DEBUG_LEVEL = 5;
        private const Int32 INFO_LEVEL = 4;
        private const Int32 WARN_LEVEL = 3;
        private const Int32 ERROR_LEVEL = 2;
        private const Int32 FATAL_LEVEL = 1;
        #endregion 
        #endregion

        #region Operation Contracts
        /// <summary>
        /// Log to File
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category">Debug/Info/Warn/Exception</param>
        /// <param name="priority">Low/Medium/High</param>
        [OperationContract]
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
        [OperationContract]
        public Int32 GetLoggingLevel()
        {
            if (LOG.IsDebugEnabled)
                return DEBUG_LEVEL;
            if (LOG.IsInfoEnabled)
                return INFO_LEVEL;
            if (LOG.IsWarnEnabled)
                return WARN_LEVEL;
            if (LOG.IsErrorEnabled)
                return ERROR_LEVEL;
            if (LOG.IsFatalEnabled)
                return FATAL_LEVEL;
            else
                return INFO_LEVEL;
        } 
        #endregion
    }
}
