﻿using System;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using GreenField.DataContracts;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;


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

        /// <summary>
        /// Set static class "Session" to CurrentSession
        /// </summary>
        /// <param name="sessionVariable">Session</param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool ClearSession()
        {
            try
            {
               // HttpContext.Current.Session["Session"] = null;
                HttpContext.Current.Session.RemoveAll();
                return true;

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
