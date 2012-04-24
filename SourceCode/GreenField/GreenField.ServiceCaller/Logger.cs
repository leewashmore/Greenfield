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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Reflection;

namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Class for interacting with Service LoggingOperations
    /// </summary>
    [Export(typeof(ILoggerFacade))]
    public class Logger : ILogger
    {
        #region Fields
        #region Log Levels
        private static Int32 DEBUG_LEVEL = 5;
        private static Int32 INFO_LEVEL = 4;
        private static Int32 WARN_LEVEL = 3;
        private static Int32 ERROR_LEVEL = 2;
        private static Int32 FATAL_LEVEL = 1;
        private static Int32 DEFAULT_LEVEL = 0;
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor - Initialize LogLevel from Web Config
        /// </summary>
        public Logger()
        {
            if (LogLevel == DEFAULT_LEVEL)
            {
                LoggingDefinitions.LoggingOperationsClient client = new LoggingDefinitions.LoggingOperationsClient();
                client.GetLoggingLevelAsync();
                client.GetLoggingLevelCompleted += (s, ea) =>
                {
                    try
                    {
                        LogLevel = ea.Result;
                    }
                    catch (TargetInvocationException ex)
                    {
                        MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
                    }
                };
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property that gets LogLevel from Web Config 
        /// </summary>
        private static Int32 _logLevel = DEFAULT_LEVEL;
        public static Int32 LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = value;
            }
        }
        #endregion

        #region Service Caller Methods
        /// <summary>
        /// Method that filters the messages to be logged
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category">Debug/Info/Warn/Exception</param>
        /// <param name="priority">Low/Medium/High</param>
        public void Log(string message, Category category, Priority priority)
        {
            LoggingDefinitions.LoggingOperationsClient client = new LoggingDefinitions.LoggingOperationsClient();

            #region Async Call
            //Async Call based on Category Type, LogLevel and Priority
            if (LogLevel == DEFAULT_LEVEL)
                client.LogToFileAsync(message, Category.Info.ToString(), Priority.Low.ToString());

            else if (category == Category.Debug && LogLevel >= DEBUG_LEVEL)
                client.LogToFileAsync(message, category.ToString(), priority.ToString());

            else if (category == Category.Info && LogLevel >= INFO_LEVEL)
                client.LogToFileAsync(message, category.ToString(), priority.ToString());

            else if (category == Category.Exception && LogLevel >= WARN_LEVEL && priority == Priority.Low)
                client.LogToFileAsync(message, category.ToString(), priority.ToString());

            else if (category == Category.Exception && LogLevel >= ERROR_LEVEL && priority == Priority.Medium)
                client.LogToFileAsync(message, category.ToString(), priority.ToString());

            else if (category == Category.Exception && LogLevel >= FATAL_LEVEL && priority == Priority.High)
                client.LogToFileAsync(message, category.ToString(), priority.ToString());
            #endregion

            #region Completion Event
            client.LogToFileCompleted += (se, e) =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.InnerException.Message);
                }
            };
            #endregion
        }
        #endregion
    }
}
