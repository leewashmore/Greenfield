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
            try
            {
                DashboardOperationsClient client = new DashboardOperationsClient();
                client.GetDashboardPreferenceByUserNameAsync(userName);
                client.GetDashboardPreferenceByUserNameCompleted += (se, e) =>
                {
                    try
                    {
                        if (callback != null)
                            callback(e.Result.ToList());
                    }
                    catch (Exception ex)
                    {
                        _logger.Log("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                        MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.Log("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Storing Personalised Layout
        /// </summary>
        /// <param name="dashBoardPreference">tblDashboardPreference collection</param>
        /// <param name="callback">True/False</param>
        public void SetDashBoardPreference(ObservableCollection<tblDashboardPreference> dashBoardPreference, Action<bool> callback)
        {
            try
            {
                DashboardOperationsClient client = new DashboardOperationsClient();
                client.SetDashBoardPreferenceAsync(dashBoardPreference);
                client.SetDashBoardPreferenceCompleted += (se, e) =>
                {
                    if (callback != null)
                        callback(e.Result);
                };

            }
            catch (Exception ex)
            {
                _logger.Log("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }
        }
    }
}
