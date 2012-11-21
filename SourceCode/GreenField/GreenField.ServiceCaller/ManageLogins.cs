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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.LoginDefinitions;
using System.Reflection;
using System.ServiceModel;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Logging;
using GreenField.UserSession;

namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Class for interacting with Service LoginOperations - implements ASP Membership Provider APIs through WCF
    /// </summary>
    [Export(typeof(IManageLogins))]
    public class ManageLogins : IManageLogins
    {
        [Import]
        public ILoggerFacade LoggerFacade { get; set; }

        #region Service Caller Method Definitions
        #region Membership
        /// <summary>
        /// Validate User credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="callback">True/False</param>
        public void ValidateUser(string username, string password, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.ValidateUserAsync(username, password, callback);
            client.ValidateUserCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Create Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="callback">MembershipCreateStatus</param>
        public void CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, Action<string> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.CreateUserAsync(username, password, email, passwordQuestion, passwordAnswer, isApproved, callback);
            client.CreateUserCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result.ToString());
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="callback">True/False</param>
        public void ChangePassword(string username, string oldPassword, string newPassword, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.ChangePasswordAsync(username, oldPassword, newPassword, callback);
            client.ChangePasswordCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <param name="callback">New Password</param>
        public void ResetPassword(string username, string answer, Action<string> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.ResetPasswordAsync(username, answer, callback);
            client.ResetPasswordCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Update Approval Status for Membership User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback">True/False</param>
        public void UpdateApprovalForUser(MembershipUserInfo user, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.UpdateApprovalForUserAsync(user, callback);
            client.UpdateApprovalForUserCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// Update Approval Status for multiple Membership Users
        /// </summary>
        /// <param name="users"></param>
        /// <param name="callback">True/False</param>
        public void UpdateApprovalForUsers(ObservableCollection<MembershipUserInfo> users, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.UpdateApprovalForUsersAsync(users, callback);
            client.UpdateApprovalForUsersCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Unlock Membership  User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="callback">True/False</param>
        public void UnlockUser(string userName, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.UnlockUserAsync(userName);
            client.UnlockUserCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Unlock multiple Membership  Users
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="callback">True/False</param>
        public void UnlockUsers(ObservableCollection<string> userNames, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.UnlockUsersAsync(userNames);
            client.UnlockUsersCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Get Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <param name="callback">MembershipUser</param>
        public void GetUser(string username, bool userIsOnline, Action<MembershipUserInfo> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.GetUserAsync(username, userIsOnline);
            client.GetUserCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Get all Membership Users
        /// </summary>
        /// <param name="callback">Users</param>
        public void GetAllUsers(Action<System.Collections.Generic.List<MembershipUserInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.GetAllUsersAsync();
            client.GetAllUsersCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }

                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Delete Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callback">True/False</param>
        public void DeleteUser(string username, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.DeleteUserAsync(username);
            client.DeleteUserCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Delete multiple Membership Users
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callback">True/False</param>
        public void DeleteUsers(ObservableCollection<string> usernames, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.DeleteUsersAsync(usernames);
            client.DeleteUsersCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Roles
        /// <summary>
        /// Create Role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="callback">True/False</param>
        public void CreateRole(string roleName, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.CreateRoleAsync(roleName, callback);
            client.CreateRoleCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Get all Roles
        /// </summary>
        /// <param name="callback">Roles</param>
        public void GetAllRoles(Action<System.Collections.Generic.List<string>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.GetAllRolesAsync();
            client.GetAllRolesCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Get Roles for Membership User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="callback">Roles</param>
        public void GetRolesForUser(string userName, Action<List<string>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.GetRolesForUserAsync(userName);
            client.GetRolesForUserCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Remove Membership Users from Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <param name="callback">True/False</param>
        public void RemoveUsersFromRoles(ObservableCollection<string> usernames, ObservableCollection<string> roleNames, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.RemoveUsersFromRolesAsync(usernames, roleNames);
            client.RemoveUsersFromRolesCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Add Membership Users to Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <param name="callback"></param>
        public void AddUsersToRoles(ObservableCollection<string> usernames, ObservableCollection<string> roleNames, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.AddUsersToRolesAsync(usernames, roleNames);
            client.AddUsersToRolesCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="username"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <param name="callback">True/False</param>
        public void DeleteRole(string username, bool throwOnPopulatedRole, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.DeleteRoleAsync(username, throwOnPopulatedRole, callback);
            client.DeleteRoleCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Update Membership User Roles
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="addRoleNames"></param>
        /// <param name="deleteRoleNames"></param>
        /// <param name="callback">True/False</param>
        public void UpdateUserRoles(string userName, ObservableCollection<string> addRoleNames, ObservableCollection<string> deleteRoleNames, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            LoginDefinitions.LoginOperationsClient client = new LoginDefinitions.LoginOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            client.UpdateUserRolesAsync(userName, addRoleNames, deleteRoleNames);
            client.UpdateUserRolesCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.LoginDefinitions.ServiceFault>;
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
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime()
                    , SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion
        #endregion
    }
}
