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
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.SessionDefinitions;
using System.Reflection;
using System.ServiceModel;

namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Class for interacting with Service SessionOperations
    /// </summary>
    [Export(typeof(IManageSessions))]
    public class ManageSessions : IManageSessions
    {
        /// <summary>
        /// Get "Session" instance from CurrentSession
        /// </summary>
        /// <param name="callback">Session</param>
        public void GetSession(Action<Session> callback)
        {
            SessionOperationsClient client = new SessionOperationsClient();
            client.GetSessionAsync();
            client.GetSessionCompleted += (se, e) =>
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

        /// <summary>
        /// Set "Session" instance to CurrentSession
        /// </summary>
        /// <param name="sessionVariable">Session</param>
        /// <param name="callback">True/False</param>
        public void SetSession(Session sessionVariable, Action<bool?> callback)
        {
            SessionOperationsClient client = new SessionOperationsClient();
            client.SetSessionAsync(sessionVariable);
            client.SetSessionCompleted += (se, e) =>
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
