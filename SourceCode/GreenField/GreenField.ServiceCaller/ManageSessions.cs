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
using GreenField.DataContracts;

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
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.GetSessionAsync();
            client.GetSessionCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault>;
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
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.SetSessionAsync(sessionVariable);
            client.SetSessionCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault>;
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
            };
        }

        /// <summary>
        /// Clears "Session" instance to CurrentSession
        /// </summary>
        /// <param name="callback">True/False</param>
        public void ClearSession(Action<bool> callback)
        {
            SessionOperationsClient client = new SessionOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.ClearSessionAsync();
            client.ClearSessionCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SessionDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(false);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(false);
                }
            };
        }
    }
}
