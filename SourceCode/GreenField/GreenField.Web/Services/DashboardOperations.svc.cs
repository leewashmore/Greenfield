using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GreenField.DAL;
using System.ServiceModel.Activation;
using GreenField.Web.Services;
using System.Collections.ObjectModel;
using GreenField.Web.DataContracts;

namespace GreenField.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DashboardService" in code, svc and config file together.
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DashboardOperations
    {
        #region Fields

        /// <summary>
        /// Logging Service Instance
        /// </summary>
        private LoggingOperations loggingOperations = new LoggingOperations();

        #endregion

        /// <summary>
        /// Retrieve User Dashboard Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> Dashboard Preferences</returns>
        [OperationContract]
        public List<tblDashboardPreference> GetDashboardPreferenceByUserName(String userName)
        {
            List<tblDashboardPreference> result = new List<tblDashboardPreference>();
            try
            {
                ResearchEntities entity = new ResearchEntities();
                result = entity.GetDashBoardPreferenceByUserName(userName).ToList();
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
            }
            return result;
        }


        /// <summary>
        /// Store User Dashboard Preference
        /// </summary>
        /// <param name="objUserID"></param>
        /// <param name="objPersistData"></param>
        [OperationContract]
        public bool SetDashBoardPreference(ObservableCollection<tblDashboardPreference> dashBoardPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                foreach (tblDashboardPreference item in dashBoardPreference)
                {                    
                    entity.SetDashBoardPreference(item.UserName, item.GadgetViewClassName, item.GadgetViewModelClassName, item.GadgetName, item.GadgetState, item.PreferenceGroupID, item.GadgetPosition); 
                }
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }
    }
}