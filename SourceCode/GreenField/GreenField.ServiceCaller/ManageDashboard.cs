using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.ServiceCaller.DashboardDefinitions;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using System.Collections.ObjectModel;
using System.ServiceModel;


namespace GreenField.ServiceCaller
{
    [Export(typeof(IManageDashboard))]
    public class ManageDashboard : IManageDashboard
    {
        #region Fields

        /// <summary>
        /// Logging Service Instance
        /// </summary>
        /// 

        private ILoggerFacade _logger;

        #endregion

        [ImportingConstructor]
        public ManageDashboard(ILoggerFacade logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Retrieving Personalised Layout
        /// </summary>
        /// <param name="objUserID">UserName of User</param>
        /// <param name="callback"> DashboardPreferences</param>
        public void GetDashboardPreferenceByUserName(string userName, Action<List<tblDashboardPreference>> callback)
        {
            DashboardOperationsClient client = new DashboardOperationsClient();
            client.GetDashboardPreferenceByUserNameAsync(userName);
            client.GetDashboardPreferenceByUserNameCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                            callback(e.Result.ToList());
                        else
                            callback(null);
                    }
                }
                else if (e.Error is FaultException<ServiceFault>)
                {
                    FaultException<ServiceFault> fault = e.Error as FaultException<ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// Storing Personalised Layout
        /// </summary>
        /// <param name="dashBoardPreference">tblDashboardPreference collection</param>
        /// <param name="callback">True/False</param>
        public void SetDashboardPreference(ObservableCollection<tblDashboardPreference> dashBoardPreference, string userName, Action<bool?> callback)
        {
            DashboardOperationsClient client = new DashboardOperationsClient();
            client.SetDashboardPreferenceAsync(dashBoardPreference, userName);
            client.SetDashboardPreferenceCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<ServiceFault>)
                {
                    FaultException<ServiceFault> fault = e.Error as FaultException<ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }
    }
}
