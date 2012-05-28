using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.DataContracts;
using System.Web;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;
using System.Resources;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Class implementing Session Operation Contracts
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SessionOperations
    {

        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        #region Operation Contracts
        /// <summary>
        /// Get static class "Session" from CurrentSession
        /// </summary>
        /// <returns>Session</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Session GetSession()
        {
            try
            {
                return System.Web.HttpContext.Current.Session["Session"] as Session;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Set static class "Session" to CurrentSession
        /// </summary>
        /// <param name="sessionVariable">Session</param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? SetSession(Session sessionVariable)
        {
            try
            {
                if (sessionVariable != null)
                {
                    HttpContext.Current.Session["Session"] = sessionVariable;
                    return true;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion
    }
}
