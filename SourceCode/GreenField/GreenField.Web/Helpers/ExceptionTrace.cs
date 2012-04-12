using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using GreenField.Web.DataContracts;

namespace GreenField.Web.Helpers
{
    public static class ExceptionTrace
    {
        #region Fields
        /// <summary>
        /// Logging Service Instance
        /// </summary>
        private static GreenField.Logging.LoggingOperations _loggingOperations = new Logging.LoggingOperations();
        #endregion

        private static string StackTraceToString(Exception exception)
        {
            StringBuilder sb = new StringBuilder(256);
            var frames = new System.Diagnostics.StackTrace(exception).GetFrames();
            /* Ignore current StackTraceToString method...*/
            for (int i = 0; i < frames.Length; i++)
            {
                var currFrame = frames[i];
                var method = currFrame.GetMethod();
                sb.Append(string.Format("{0}|{1} || ", method.ReflectedType != null ? method.ReflectedType.FullName : string.Empty, method.Name));
            }
            return sb.ToString();
        }

        public static void LogException(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException();

            string userName = (System.Web.HttpContext.Current.Session["Session"] as Session).UserName;

            if (userName == null)
                throw new InvalidOperationException();

            _loggingOperations.LogToFile("User : " + userName + "\nMessage: " + ex.Message + "\nStackTrace: " + StackTraceToString(ex), "Exception", "Medium");
        }
    }
}