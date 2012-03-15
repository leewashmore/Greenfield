using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.Web.DataContracts;
using System.Web;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Class implementing Session Operation Contracts
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SessionOperations
    {
        #region Fields
        /// <summary>
        /// Logging Service Instance
        /// </summary>
        private LoggingOperations loggingOperations = new LoggingOperations();
        #endregion

        #region Operation Contracts
        /// <summary>
        /// Get static class "Session" from CurrentSession
        /// </summary>
        /// <returns>Session</returns>
        [OperationContract]
        public Session GetSession()
        {
            try
            {
                return System.Web.HttpContext.Current.Session["Session"] as Session;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return null;
            }
        }

        /// <summary>
        /// Set static class "Session" to CurrentSession
        /// </summary>
        /// <param name="sessionVariable">Session</param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool SetSession(Session sessionVariable)
        {
            try
            {
                if (sessionVariable != null)
                    HttpContext.Current.Session["Session"] = sessionVariable;
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        } 
        #endregion
    }
}
