using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.ServiceModel.Activation;
using GreenField.Web.DataContracts;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Class implementing Login Operation Contracts
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LoginOperations
    {
        #region Fields
        /// <summary>
        /// Logging Service Instance
        /// </summary>
        private LoggingOperations loggingOperations = new LoggingOperations(); 
        #endregion

        #region Operation Contracts
        #region Membership
        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool ValidateUser(string username, string password)
        {
            try
            {
                return Membership.ValidateUser(username, password);
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <returns>MembershipCreateStatus</returns>
        [OperationContract]
        public string CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved)
        {
            try
            {
                MembershipCreateStatus status = new MembershipCreateStatus();
                Membership.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, out status);

                switch (status)
                {
                    case MembershipCreateStatus.DuplicateEmail:
                        return "DuplicateEmail";
                    case MembershipCreateStatus.DuplicateProviderUserKey:
                        return "DuplicateProviderUserKey";
                    case MembershipCreateStatus.DuplicateUserName:
                        return "DuplicateUserName";
                    case MembershipCreateStatus.InvalidAnswer:
                        return "InvalidAnswer";
                    case MembershipCreateStatus.InvalidEmail:
                        return "InvalidEmail";
                    case MembershipCreateStatus.InvalidPassword:
                        return "InvalidPassword";
                    case MembershipCreateStatus.InvalidProviderUserKey:
                        return "InvalidProviderUserKey";
                    case MembershipCreateStatus.InvalidQuestion:
                        return "InvalidQuestion";
                    case MembershipCreateStatus.InvalidUserName:
                        return "InvalidUserName";
                    case MembershipCreateStatus.ProviderError:
                        return "ProviderError";
                    case MembershipCreateStatus.Success:
                        return "Success";
                    case MembershipCreateStatus.UserRejected:
                        return "UserRejected";
                    default:
                        break;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return string.Empty;
            }
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                return Membership.Provider.ChangePassword(username, oldPassword, newPassword);
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns>New Password</returns>
        [OperationContract]
        public string ResetPassword(string username, string answer)
        {
            try
            {
                return Membership.Provider.ResetPassword(username, answer);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Update Approval Status for Membership User
        /// </summary>
        /// <param name="membershipUserInfo">MembershipUserInfo</param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool UpdateApprovalForUser(MembershipUserInfo membershipUserInfo)
        {
            try
            {
                MembershipUser membershipUser = Membership.GetUser(membershipUserInfo.UserName, membershipUserInfo.IsOnline);
                membershipUser.IsApproved = membershipUserInfo.IsApproved;
                if (membershipUser != null)
                    Membership.UpdateUser(membershipUser);
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Update Approval Status for multiple Membership Users
        /// </summary>
        /// <param name="users"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool UpdateApprovalForUsers(MembershipUserInfo[] users)
        {
            try
            {
                foreach (MembershipUserInfo user in users)
                {
                    MembershipUser membershipUser = Membership.GetUser(user.UserName, user.IsOnline);
                    membershipUser.IsApproved = user.IsApproved;
                    if (membershipUser != null)
                        Membership.UpdateUser(membershipUser);
                }

                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="username"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool DeleteUser(string username)
        {
            try
            {
                return Membership.DeleteUser(username);
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Delete multiple Membership Users
        /// </summary>
        /// <param name="username"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool DeleteUsers(string[] usernames)
        {
            try
            {
                foreach (string item in usernames)
                {
                    if (!Membership.DeleteUser(item))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Get Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns>MembershipUserInfo</returns>
        [OperationContract]
        public MembershipUserInfo GetUser(string username, bool userIsOnline)
        {
            try
            {
                MembershipUser user = Membership.GetUser(username, userIsOnline);
                return user != null ? ConvertMembershipUser(user) : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of MembershipUserInfo</returns>
        [OperationContract]
        public List<MembershipUserInfo> GetAllUsers()
        {
            MembershipUserCollection membershipUserCollection = new MembershipUserCollection();
            List<MembershipUserInfo> membershipUserInfo = new List<MembershipUserInfo>();

            try
            {
                membershipUserCollection = Membership.GetAllUsers();
                foreach (MembershipUser user in membershipUserCollection)
                    membershipUserInfo.Add(ConvertMembershipUser(user));
                return membershipUserInfo;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return null;
            }
            
        }

        /// <summary>
        /// Unlock Membership User
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool UnlockUser(string userName)
        {
            try
            {
                return Membership.Provider.UnlockUser(userName);
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Unlock multiple Membership Users
        /// </summary>
        /// <param name="userNames"></param>
        /// <returns></returns>
        [OperationContract]
        public bool UnlockUsers(string[] userNames)
        {
            try
            {
                foreach (string userName in userNames)
                    Membership.Provider.UnlockUser(userName);
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }
        #endregion

        #region Roles
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>Roles</returns>
        [OperationContract]
        public string[] GetAllRoles()
        {
            try
            {
                return Roles.GetAllRoles();
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return null;
            }
            
        }

        /// <summary>
        /// Create role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool CreateRole(string roleName)
        {
            try
            {
                Roles.CreateRole(roleName);
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Get Roles for Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Roles</returns>
        [OperationContract]
        public string[] GetRolesForUser(string username)
        {
            try
            {
                return Roles.GetRolesForUser(username);                
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return null;
            }
            
        }

        /// <summary>
        /// Remove Membership users from Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            try
            {
                Roles.RemoveUsersFromRoles(usernames, roleNames);
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Add Membership Users to Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            try
            {
                Roles.AddUsersToRoles(usernames, roleNames);
                return true;
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }

        /// <summary>
        /// Update Membership User Roles
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="addRoleNames"></param>
        /// <param name="deleteRoleNames"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool UpdateUserRoles(string userName, string[] addRoleNames, string[] deleteRoleNames)
        {
            bool addRolesValidation = true;
            if (addRoleNames.Count() > 0)
            {
                try
                {
                    Roles.AddUserToRoles(userName, addRoleNames);
                }
                catch (Exception ex)
                {
                    loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                    addRolesValidation = false;
                }
            }

            bool deleteRolesValidation = true;
            if (deleteRoleNames.Count() > 0)
            {
                try
                {
                    Roles.RemoveUserFromRoles(userName, deleteRoleNames);
                }
                catch (Exception ex)
                {
                    loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                    deleteRolesValidation = false;
                }
            }

            return addRolesValidation && deleteRolesValidation;
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="username"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        public bool DeleteRole(string username, bool throwOnPopulatedRole)
        {
            try
            {
                return Roles.DeleteRole(username, throwOnPopulatedRole);
            }
            catch (Exception ex)
            {
                loggingOperations.LogToFile("User : " + (System.Web.HttpContext.Current.Session["Session"] as Session).UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", "Medium");
                return false;
            }
        }
        #endregion 
        #endregion

        #region Helper
        /// <summary>
        /// Convert System.Web.Security.MembershipUser to GreenField.Web.DataContracts.MembershipUserInfo
        /// </summary>
        /// <param name="user">MembershipUser</param>
        /// <returns>MembershipUserInfo</returns>
        private MembershipUserInfo ConvertMembershipUser(MembershipUser user)
        {
            return new MembershipUserInfo
            {
                UserName = user.UserName,
                Email = user.Email,
                IsApproved = user.IsApproved,
                IsLockedOut = user.IsLockedOut,
                IsOnline = user.IsOnline,
                Comment = user.Comment,
                CreateDate = user.CreationDate,
                LastActivityDate = user.LastActivityDate,
                LastLockOutDate = user.LastLockoutDate,
                LastLogInDate = user.LastLoginDate,
                ProviderUserKey = user.ProviderUserKey.ToString(),
                ProviderName = user.ProviderName,
                PasswordQuestion = user.PasswordQuestion,
                LastPassWordChangedDate = user.LastPasswordChangedDate
            };
        } 
        #endregion
    }
}
