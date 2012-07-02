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
using GreenField.UserSession;


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
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(_logger, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION.UserName);

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
                else if (e.Error is FaultException<GreenField.ServiceCaller.DashboardDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DashboardDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DashboardDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(_logger, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION.UserName);
            };
        }

        /// <summary>
        /// Storing Personalised Layout
        /// </summary>
        /// <param name="dashBoardPreference">tblDashboardPreference collection</param>
        /// <param name="callback">True/False</param>
        public void SetDashboardPreference(ObservableCollection<tblDashboardPreference> dashBoardPreference, string userName, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(_logger, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION.UserName);

            DashboardOperationsClient client = new DashboardOperationsClient();
            client.SetDashboardPreferenceAsync(dashBoardPreference, userName);
            client.SetDashboardPreferenceCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DashboardDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DashboardDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DashboardDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(_logger, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION.UserName);
            };
        }
    }
}
