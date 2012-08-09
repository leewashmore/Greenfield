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
using GreenField.ServiceCaller.AlertDefinitions;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using System.Collections.ObjectModel;
using System.ServiceModel;
using GreenField.UserSession;


namespace GreenField.ServiceCaller
{
    [Export(typeof(IManageAlerts))]
    public class ManageAlerts : IManageAlerts
    {
        #region Fields

        [Import]
        public ILoggerFacade LoggerFacade { get; set; }

        #endregion

        //[ImportingConstructor]
        //public ManageAlerts(ILoggerFacade logger)
        //{
        //    LoggerFacade = logger;
        //}

        public void SendAlert(List<String> messageTo, List<String> messageCc, String messageBody, String messageSubject, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            //ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            
            AlertOperationsClient client = new AlertOperationsClient();
            client.SendAlertAsync(messageTo, messageCc, messageBody, messageSubject);
            client.SendAlertCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.AlertDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.AlertDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.AlertDefinitions.ServiceFault>;
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
                //ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                //    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
    }
}
