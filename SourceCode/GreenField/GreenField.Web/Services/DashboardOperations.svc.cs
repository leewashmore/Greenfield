﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using GreenField.DAL;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DashboardOperations
    {
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        /// <summary>
        /// Retrieve User Dashboard Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> Dashboard Preferences</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Store User Dashboard Preference
        /// </summary>
        /// <param name="objUserID"></param>
        /// <param name="objPersistData"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? SetDashboardPreference(ObservableCollection<tblDashboardPreference> dashBoardPreference, string userName)
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }



    }
}