using System;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.ServiceCaller
{
    public static class ServiceLog
    {
        public static void LogServiceCall(ILoggerFacade logger, string methodNamespace, DateTime timeStamp,
                                          String userName)
        {
            if (logger != null)
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                           + ")]|Type[(ServiceCall"
                           + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                           + ")]|TimeStamp[(" +
                           timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                           + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogServiceCallback(ILoggerFacade logger, string methodNamespace, DateTime timeStamp,
                                              string userName)
        {
            if (logger != null)
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                           + ")]|Type[(ServiceCallback"
                           + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                           + ")]|TimeStamp[(" +
                           timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                           + ")]", Category.Info, Priority.None);
            }
        }

        //TODO: add error details
        public static void LogServiceClientReceivedData(ILoggerFacade logger, string methodNamespace, Exception e, DateTime time,
                                                        long startTime, long endTime, string userName)
        {
            if (logger != null)
            {
                logger.Log(
                    string.Format("|User[({0})]|Type[(Client Received Data TimeSpan:{1})]|MethodNameSpace[({2})] - {3}|Time[({4})]",
                                  userName.Replace(Environment.NewLine, " "), new TimeSpan(endTime - startTime).ToString(),
                                  methodNamespace.Replace(Environment.NewLine, " "), e == null ? "Sucessful" : "Failed",
                                  time.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")),
                    Category.Warn, Priority.None);
            }
        }

        public static void LogServiceCallLogin(ILoggerFacade logger, string methodNamespace, DateTime timeStamp,
                                               string userName)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + userName.Replace(Environment.NewLine, " ")
                           + ")]|Type[(LoginServiceCall"
                           + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                           + ")]|TimeStamp[(" +
                           timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                           + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogServiceCallbackLogin(ILoggerFacade logger, string methodNamespace, DateTime timeStamp,
                                                   string userName)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + userName.Replace(Environment.NewLine, " ")
                           + ")]|Type[(LoginServiceCallback"
                           + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                           + ")]|TimeStamp[(" +
                           timeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                           + ")]", Category.Info, Priority.None);
            }
        }
    }
}