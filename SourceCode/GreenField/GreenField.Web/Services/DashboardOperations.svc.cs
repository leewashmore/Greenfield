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
using GreenField.Web.Helpers;
using System.Data.Objects;

namespace GreenField.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DashboardService" in code, svc and config file together.
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DashboardOperations
    {


        /// <summary>
        /// Retrieve User Dashboard Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> Dashboard Preferences</returns>
        [OperationContract]
        public List<tblDashboardPreference> GetDashboardPreferenceByUserName(String userName)
        {
            try
            {
                
                ResearchEntities entity = new ResearchEntities();

                ObjectResult<tblDashboardPreference> resultSet = entity.GetDashBoardPreferenceByUserName(userName);
                if (resultSet != null)
                {
                    return resultSet.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }

        }

        /// <summary>
        /// Store User Dashboard Preference
        /// </summary>
        /// <param name="objUserID"></param>
        /// <param name="objPersistData"></param>
        [OperationContract]
        public bool SetDashboardPreference(ObservableCollection<tblDashboardPreference> dashBoardPreference, string userName)
        {

            try
            {
                ResearchEntities entity = new ResearchEntities();
                if (dashBoardPreference.Count > 0)
                {
                    foreach (tblDashboardPreference item in dashBoardPreference)
                    {
                        entity.SetDashBoardPreference(item.UserName, item.GadgetViewClassName, item.GadgetViewModelClassName, item.GadgetName, item.GadgetState, item.PreferenceGroupID, item.GadgetPosition);
                    }
                }
                else
                {
                    entity.SetDashBoardPreference(userName, "null", "null", "null", "null", "null", 0);
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }



    }
}