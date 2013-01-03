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
using Microsoft.Practices.Prism.Logging;

namespace GreenField.ServiceCaller
{
    public static class ServiceLog
    {
        public static void LogServiceCall(ILoggerFacade logger, string methodNamespace, DateTime timeStamp, String userName)
        {
            if (logger != null)
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                                + ")]|Type[(ServiceCall"
                                + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                                + ")]|TimeStamp[(" + timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                                + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogServiceCallback(ILoggerFacade logger, string methodNamespace, DateTime timeStamp, string userName)
        {
            if (logger != null)
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                                + ")]|Type[(ServiceCallback"
                                + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                                + ")]|TimeStamp[(" + timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                                + ")]", Category.Info, Priority.None); 
            }
        }

        public static void LogServiceCallLogin(ILoggerFacade logger, string methodNamespace, DateTime timeStamp, string userName)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + userName.Replace(Environment.NewLine, " ")
                                + ")]|Type[(LoginServiceCall"
                                + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                                + ")]|TimeStamp[(" + timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                                + ")]", Category.Info, Priority.None); 
            }
        }

        public static void LogServiceCallbackLogin(ILoggerFacade logger, string methodNamespace, DateTime timeStamp, string userName)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + userName.Replace(Environment.NewLine, " ")
                                + ")]|Type[(LoginServiceCallback"
                                + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                                + ")]|TimeStamp[(" + timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                                + ")]", Category.Info, Priority.None); 
            }
        }
    }
}
