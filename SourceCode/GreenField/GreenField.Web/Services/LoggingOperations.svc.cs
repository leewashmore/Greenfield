using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Class implementing Logging Operation Contracts
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LoggingOperations
    {
        #region Fields
        /// <summary>
        /// GreenField.Logging.LoggingOperation Instance instance
        /// </summary>
        private GreenField.Logging.LoggingOperations _loggingOperations = new Logging.LoggingOperations();
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
            _loggingOperations.LogToFile(message, category, priority);
        }

        /// <summary>
        /// Get Logging Level
        /// </summary>
        /// <returns>Logging Level</returns>
        [OperationContract]
        public Int32 GetLoggingLevel()
        {
            return _loggingOperations.GetLoggingLevel();
        }
        #endregion
    }
}
