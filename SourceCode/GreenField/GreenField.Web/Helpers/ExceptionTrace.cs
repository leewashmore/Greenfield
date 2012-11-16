using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using GreenField.DataContracts;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Logging server side exceptions
    /// </summary>
    public static class ExceptionTrace
    {
        #region Fields
        /// <summary>
        /// Logging Service Instance
        /// </summary>
        private static GreenField.Logging.LoggingOperations loggingOperations = new Logging.LoggingOperations();
        #endregion

        #region Public Methods
        /// <summary>
        /// logs server side exceptions
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException();
            }
            string userName = null;

            if (System.Web.HttpContext.Current.Session["Session"] != null)
            {
                userName = (System.Web.HttpContext.Current.Session["Session"] as Session).UserName;
            }

            if (userName == null)
            {
                userName = "Null";
            }
            loggingOperations.LogToFile("|User[(" + userName.Replace(Environment.NewLine, " ")
                + ")]|Type[(Exception"
                + ")]|Message[(" + ex.Message.Replace(Environment.NewLine, " ")
                + ")]|StackTrace[(" + StackTraceToString(ex)
                + ")]", "Exception", "Medium");
        }

        public static void LogInfo(string input, string type, string message)
        {
            string userName = null;

            if (System.Web.HttpContext.Current.Session["Session"] != null)
            {
                userName = (System.Web.HttpContext.Current.Session["Session"] as Session).UserName;
            }

            if (userName == null)
            {
                userName = "Null";
            }

            loggingOperations.LogToFile("|User[(" + userName.Replace(Environment.NewLine, " ")
                + ")]|Type[(" + type.Replace(Environment.NewLine, " ")
                + ")]|Message[(" + message.Replace(Environment.NewLine, " ")
                + ")]|Input[(" + input.ToString().Replace(Environment.NewLine, " ")
                + ")]|TimeStamp[(" + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss,fff").Replace(Environment.NewLine, " ")
                + ")]", "Debug", "None");
        }
        #endregion

        #region Methods
        /// <summary>
        /// designs string output for exception's stack trace
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string StackTraceToString(Exception exception)
        {
            StringBuilder sb = new StringBuilder(256);
            var frames = new System.Diagnostics.StackTrace(exception).GetFrames();
            //ignore current StackTraceToString method
            for (int i = 0; i < frames.Length; i++)
            {
                var currFrame = frames[i];
                var method = currFrame.GetMethod();
                sb.Append(string.Format("{0}|{1} || ", method.ReflectedType != null ? method.ReflectedType.FullName : string.Empty, method.Name));
            }
            return sb.ToString();
        }        
        #endregion
    }

    
}