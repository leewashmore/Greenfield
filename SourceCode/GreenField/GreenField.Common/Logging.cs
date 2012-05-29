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
using System.Text;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Common
{
    public static class Logging
    {
        public static string StackTraceToString(Exception exception)
        {
            StringBuilder sb = new StringBuilder(256);
            var frames = new System.Diagnostics.StackTrace(exception).GetFrames();
            for (int i = 0; i < frames.Length; i++) /* Ignore current StackTraceToString method...*/
            {
                var currFrame = frames[i];
                var method = currFrame.GetMethod();
                sb.Append(string.Format("{0}|{1} || ", method.ReflectedType != null ? method.ReflectedType.FullName : string.Empty, method.Name));
            }
            return sb.ToString();
        }

        #region Methods
        public static void LogBeginMethod(ILoggerFacade logger, string methodNamespace, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | BeginMethod: [(" + methodNamespace + ")]", Category.Info, Priority.None);
                }
            }
            else
            {
                logger.Log("User : [(" + userName + ")]" + " | BeginMethod: [(" + methodNamespace + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogEndMethod(ILoggerFacade logger, string methodNamespace, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | EndMethod: [(" + methodNamespace + ")]", Category.Info, Priority.None);

                }
            }
            else
            {
                logger.Log("User : [(" + userName + ")]" + " | EndMethod: [(" + methodNamespace + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogMethodParameter(ILoggerFacade logger, object methodName, object parameter, int index, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(" + parameter.ToString() + ")]", Category.Debug, Priority.None);
                }
            }
            else
            {
                logger.Log("User : [(" + userName + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(" + parameter.ToString() + ")]", Category.Debug, Priority.None);
            }
        }

        public static void LogMethodParameterNull(ILoggerFacade logger, object methodName, int index, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(Null)]", Category.Debug, Priority.None);
                }
            }
            else
            {
                logger.Log("User : [(" + userName + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(Null)]", Category.Debug, Priority.None);
            }
        }

        public static void LogMethodParameterFalse(ILoggerFacade logger, object methodName, int index, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(False)]", Category.Debug, Priority.None);
                }
            }
            else
            {
                logger.Log("User : [(" + userName + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(False)]", Category.Debug, Priority.None);
            }
        }
        #endregion

        #region Exceptions
        public static void LogException(ILoggerFacade logger, Exception exception)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | Exception: [(" + exception.Message + ")] | StackTrace: [(" + StackTraceToString(exception) + ")]", Category.Exception, Priority.Medium);
            }
        }
        #endregion

        #region Session Management
        public static void LogSessionStart(ILoggerFacade logger)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | SessionBegin", Category.Info, Priority.None);
            }
        }

        public static void LogSessionClose(ILoggerFacade logger, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | SessionEnd", Category.Info, Priority.None);
                }
            }
            else
            {
                logger.Log("User : [(" + userName + ")]" + " | SessionEnd", Category.Info, Priority.None);
            }
        }
        #endregion

        #region Role Management
        public static void LogRoleCreate(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | RoleCreation [(" + roleName + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogRoleCreateFailed(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | RoleCreationFailed [(" + roleName + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogRoleDelete(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | RoleDeletion [(" + roleName + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogRoleDeleteFailed(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | RoleDeletionFailed [(" + roleName + ")]", Category.Info, Priority.None);
            }
        }
        #endregion

        #region User Management
        public static void LogLoginActivate(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountActivation [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginActivateFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountActivationFailed [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginBlock(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountBlock [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginBlockFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountBlockFailed [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginLockRelease(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountLockRelease [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginLockReleaseFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountLockReleaseFailed [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginDelete(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountDeletion [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginDeleteFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountDeletionFailed [(" + user + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleAssign(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountRoleAssignment [User:(" + user + ")|Role:(" + role + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleAssignFailed(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountRoleAssignmentFailed [User:(" + user + ")|Role:(" + role + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleRemove(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountRoleRemoval [User:(" + user + ")|Role:(" + role + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleRemoveFailed(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("User : [(" + SessionManager.SESSION.UserName + ")]" + " | AccountRoleRemovalFailed [User:(" + user + ")|Role:(" + role + ")]", Category.Info, Priority.None);
            }
        }
        #endregion

        #region Login Management
        public static void LogAccountInactiveLoginAttempt(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")] | AccountInactiveLoginAttempt", Category.Info, Priority.None);
            }
        }

        public static void LogAccountLockedLoginAttempt(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")] | AccountLockedLoginAttempt", Category.Info, Priority.None);
            }
        }

        public static void LogAccountInvalidLoginAttempt(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")] | AccountInvalidLoginAttempt", Category.Info, Priority.None);
            }
        }

        public static void LogLoginBeginMethod(ILoggerFacade logger, string methodNamespace, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")]" + " | BeginMethod: [(" + methodNamespace + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginEndMethod(ILoggerFacade logger, string methodNamespace, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")]" + " | EndMethod: [(" + methodNamespace + ")]", Category.Info, Priority.None);

            }
        }

        public static void LogLoginMethodParameter(ILoggerFacade logger, object methodName, object parameter, int index, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(" + parameter.ToString() + ")]", Category.Debug, Priority.None);
            }
        }

        public static void LogLoginMethodParameterNull(ILoggerFacade logger, object methodName, int index, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")]" + " | MethodParameter: [(" + methodName + ")] | Index: [(" + index.ToString() + ")] | Value: [(Null)]", Category.Debug, Priority.None);
            }
        }

        public static void LogLoginException(ILoggerFacade logger, Exception exception)
        {
            logger.Log("User : [(Null)] | Exception: [(" + exception.Message + ")] | StackTrace: [(" + StackTraceToString(exception) + ")]", Category.Exception, Priority.Medium);
        }

        public static void LogAccountRegister(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")] | AccountRegistration", Category.Info, Priority.None);
            }
        }

        public static void LogAccountPasswordChange(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")] | AccountPasswordChange", Category.Info, Priority.None);
            }
        }

        public static void LogAccountPasswordReset(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("LoginID : [(" + loginId + ")] | AccountPasswordReset", Category.Info, Priority.None);
            }
        }
        #endregion
    }
}
