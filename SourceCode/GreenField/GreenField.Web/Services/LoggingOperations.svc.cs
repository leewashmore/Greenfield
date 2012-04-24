using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;

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

        #region Properties
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }
        #endregion

        #region Operation Contracts
        /// <summary>
        /// Log to File
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category">Debug/Info/Warn/Exception</param>
        /// <param name="priority">Low/Medium/High</param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void LogToFile(string message, string category, string priority)
        {
            try
            {
                _loggingOperations.LogToFile(message, category, priority);
            }
            catch (Exception ex)
            {
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get Logging Level
        /// </summary>
        /// <returns>Logging Level</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Int32 GetLoggingLevel()
        {
            try
            {
                return _loggingOperations.GetLoggingLevel();
            }
            catch (Exception ex)
            {
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion
    }
}
