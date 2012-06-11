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
            return sb.ToString().Replace(Environment.NewLine, " ");
        }

        #region Methods
        public static void LogBeginMethod(ILoggerFacade logger, string methodNamespace, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ") 
                        + ")]|Type[(BeginMethod"
                        + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ") 
                        + ")]", Category.Info, Priority.None);
                }
            }
            else
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(BeginMethod" 
                        + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                        + ")]", Category.Info, Priority.None);                
            }
        }

        public static void LogMethodParameter(ILoggerFacade logger, string methodNamespace, object parameter, int index, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(MethodParameter"
                        + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                        + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentValue[(" + parameter.ToString().Replace(Environment.NewLine, " ")
                        + ")]", Category.Debug, Priority.None);
                }
            }
            else
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(MethodParameter"
                        + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                        + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentValue[(" + parameter.ToString().Replace(Environment.NewLine, " ")
                        + ")]", Category.Debug, Priority.None);
            }
        }

        public static void LogMethodParameterNull(ILoggerFacade logger, object methodName, int index, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(MethodParameter"
                        + ")]|MethodNameSpace[(" + methodName.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentValue[(Null)]", Category.Debug, Priority.None);
                }
            }
            else
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(MethodParameter"
                        + ")]|MethodNameSpace[(" + methodName.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentValue[(Null)]", Category.Debug, Priority.None);
                }
            }
        }

        public static void LogEndMethod(ILoggerFacade logger, string methodNamespace, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(EndMethod"
                        + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ") 
                        + ")]", Category.Info, Priority.None);
                }
            }
            else
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(EndMethod"
                        + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                        + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogMethodParameterFalse(ILoggerFacade logger, object methodName, int index, string userName = "")
        {
            if (userName == "")
            {
                if (logger != null && SessionManager.SESSION != null)
                {
                    logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(MethodParameter"
                        + ")]|MethodNameSpace[(" + methodName.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentValue[(False)]", Category.Debug, Priority.None);
                }
            }
            else
            {
                logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                        + ")]|Type[(MethodParameter"
                        + ")]|MethodNameSpace[(" + methodName.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                        + ")]|ArgumentValue[(False)]", Category.Debug, Priority.None);
            }
        }
        #endregion

        #region Exceptions
        public static void LogException(ILoggerFacade logger, Exception exception)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(Exception"
                    + ")]|Message[(" + exception.Message.Replace(Environment.NewLine, " ") 
                    + ")]|StackTrace[(" + StackTraceToString(exception)
                    + ")]", Category.Exception, Priority.Medium);                
            }
        }
        #endregion

        #region Session Management
        public static void LogSessionStart(ILoggerFacade logger)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(SessionBegin"
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogSessionClose(ILoggerFacade logger, string userName = "")
        {
            logger.Log("|User[(" + userName.Replace(Environment.NewLine, " ")
                + ")]|Type[(SessionEnd"
                + ")]", Category.Info, Priority.None);
        }
        #endregion

        #region Role Management
        public static void LogRoleCreate(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(RoleCreation"
                    + ")]|Role[(" + roleName.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogRoleCreateFailed(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(RoleCreationFailed"
                    + ")]|Role[(" + roleName.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogRoleDelete(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(RoleDeletion"
                    + ")]|Role[(" + roleName.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogRoleDeleteFailed(ILoggerFacade logger, string roleName)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(RoleDeletionFailed"
                    + ")]|Role[(" + roleName.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }
        #endregion

        #region User Management
        public static void LogLoginActivate(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountActivation"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginActivateFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountActivationFailed"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginBlock(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountBlock"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginBlockFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountBlockFailed"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginLockRelease(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountLockRelease"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginLockReleaseFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountLockReleaseFailed"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginDelete(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountDeletion"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginDeleteFailed(ILoggerFacade logger, string user)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountDeletionFailed"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleAssign(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountRoleAssignment"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]|Role[(" + role.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleAssignFailed(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountRoleAssignmentFailed"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]|Role[(" + role.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleRemove(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountRoleRemoval"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]|Role[(" + role.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginRoleRemoveFailed(ILoggerFacade logger, string user, string role)
        {
            if (logger != null && SessionManager.SESSION != null)
            {
                logger.Log("|User[(" + SessionManager.SESSION.UserName.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountRoleRemovalFailed"
                    + ")]|Account[(" + user.Replace(Environment.NewLine, " ")
                    + ")]|Role[(" + role.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }
        #endregion

        #region Login Management
        public static void LogAccountInactiveLoginAttempt(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountInactiveLoginAttempt"
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogAccountLockedLoginAttempt(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountLockedLoginAttempt"
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogAccountInvalidLoginAttempt(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountInvalidLoginAttempt"
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginBeginMethod(ILoggerFacade logger, string methodNamespace, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(BeginMethod"
                    + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginEndMethod(ILoggerFacade logger, string methodNamespace, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(EndMethod"
                    + ")]|MethodNameSpace[(" + methodNamespace.Replace(Environment.NewLine, " ")
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogLoginMethodParameter(ILoggerFacade logger, object methodName, object parameter, int index, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(MethodParameter"
                    + ")]|MethodNameSpace[(" + methodName.ToString().Replace(Environment.NewLine, " ")
                    + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                    + ")]|ArgumentValue[(" + parameter.ToString().Replace(Environment.NewLine, " ")
                    + ")]", Category.Debug, Priority.None);
            }
        }

        public static void LogLoginMethodParameterNull(ILoggerFacade logger, object methodName, int index, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(MethodParameter"
                    + ")]|MethodNameSpace[(" + methodName.ToString().Replace(Environment.NewLine, " ")
                    + ")]|ArgumentIndex[(" + index.ToString().Replace(Environment.NewLine, " ")
                    + ")]|ArgumentValue[(Null)]", Category.Debug, Priority.None);
            }
        }

        public static void LogLoginException(ILoggerFacade logger, Exception exception)
        {
            logger.Log("|User[(Null)]|Exception[(" + exception.Message.Replace(Environment.NewLine, " ")
                + ")]|Type[(Exception"
                + ")]|StackTrace[(" + StackTraceToString(exception).Replace(Environment.NewLine, " ")
                + ")]", Category.Exception, Priority.Medium);
        }

        public static void LogAccountRegister(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountRegistration"
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogAccountPasswordChange(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountPasswordChange"
                    + ")]", Category.Info, Priority.None);
            }
        }

        public static void LogAccountPasswordReset(ILoggerFacade logger, string loginId)
        {
            if (logger != null)
            {
                logger.Log("|LoginID[(" + loginId.Replace(Environment.NewLine, " ")
                    + ")]|Type[(AccountPasswordReset"
                    + ")]", Category.Info, Priority.None);
            }
        }
        #endregion
    }
}
