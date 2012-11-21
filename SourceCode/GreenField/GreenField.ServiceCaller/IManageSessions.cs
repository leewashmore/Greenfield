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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.SessionDefinitions;
using GreenField.DataContracts;

namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Interface exposing Service SessionOperations Methods
    /// </summary>
    public interface IManageSessions
    {
        /// <summary>
        /// Get "Session" instance from CurrentSession
        /// </summary>
        /// <param name="callback">Session</param>
        void GetSession(Action<Session> callback);

        /// <summary>
        /// Set "Session" instance to CurrentSession
        /// </summary>
        /// <param name="sessionVariable">Session</param>
        /// <param name="callback">True/False</param>
        void SetSession(Session sessionVariable, Action<bool?> callback);

        /// <summary>
        /// Clears "Session" instance to CurrentSession
        /// </summary>
        /// <param name="callback">True/False</param>
        void ClearSession(Action<bool> callback);
    }
}
