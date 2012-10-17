using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.ServiceModel.Activation;
using GreenField.DataContracts;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;
using System.Resources;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Class implementing Login Operation Contracts
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LoginOperations
    {
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        #region Operation Contracts
        #region Membership
        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? ValidateUser(string username, string password)
        {
            try
            {
                return Membership.ValidateUser(username, password);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
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
        [FaultContract(typeof(ServiceFault))]
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

                return null;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
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
        [FaultContract(typeof(ServiceFault))]
        public bool? ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                return Membership.Provider.ChangePassword(username, oldPassword, newPassword);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns>New Password</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public string ResetPassword(string username, string answer)
        {
            try
            {
                return Membership.Provider.ResetPassword(username, answer);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Update Approval Status for Membership User
        /// </summary>
        /// <param name="membershipUserInfo">MembershipUserInfo</param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? UpdateApprovalForUser(MembershipUserInfo membershipUserInfo)
        {
            try
            {
                MembershipUser membershipUser = Membership.GetUser(membershipUserInfo.UserName, membershipUserInfo.IsOnline);
                membershipUser.IsApproved = membershipUserInfo.IsApproved;
                if (membershipUser != null)
                {
                    Membership.UpdateUser(membershipUser);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Update Approval Status for multiple Membership Users
        /// </summary>
        /// <param name="users"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? UpdateApprovalForUsers(MembershipUserInfo[] users)
        {
            try
            {
                foreach (MembershipUserInfo user in users)
                {
                    MembershipUser membershipUser = Membership.GetUser(user.UserName, user.IsOnline);
                    membershipUser.IsApproved = user.IsApproved;
                    if (membershipUser != null)
                    {
                        Membership.UpdateUser(membershipUser);                        
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="username"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? DeleteUser(string username)
        {
            try
            {
                return Membership.DeleteUser(username);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Delete multiple Membership Users
        /// </summary>
        /// <param name="username"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? DeleteUsers(string[] usernames)
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
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns>MembershipUserInfo</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public MembershipUserInfo GetUser(string username, bool userIsOnline)
        {
            try
            {
                MembershipUser user = Membership.GetUser(username, userIsOnline);
                return user != null ? ConvertMembershipUser(user) : null;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of MembershipUserInfo</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MembershipUserInfo> GetAllUsers()
        {
            try
            {
                MembershipUserCollection membershipUserCollection = new MembershipUserCollection();
                List<MembershipUserInfo> membershipUserInfo = new List<MembershipUserInfo>();

                membershipUserCollection = Membership.GetAllUsers();
                foreach (MembershipUser user in membershipUserCollection)
                    membershipUserInfo.Add(ConvertMembershipUser(user));
                return membershipUserInfo;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Unlock Membership User
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? UnlockUser(string userName)
        {
            try
            {
                return Membership.Provider.UnlockUser(userName);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Unlock multiple Membership Users
        /// </summary>
        /// <param name="userNames"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? UnlockUsers(string[] userNames)
        {
            try
            {
                foreach (string userName in userNames)
                    Membership.Provider.UnlockUser(userName);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion

        #region Roles
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>Roles</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public string[] GetAllRoles()
        {
            try
            {
                return Roles.GetAllRoles();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Create role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? CreateRole(string roleName)
        {
            try
            {
                Roles.CreateRole(roleName);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get Roles for Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Roles</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public string[] GetRolesForUser(string username)
        {
            try
            {
                return Roles.GetRolesForUser(username);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Remove Membership users from Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            try
            {
                Roles.RemoveUsersFromRoles(usernames, roleNames);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Add Membership Users to Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            try
            {
                Roles.AddUsersToRoles(usernames, roleNames);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
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
        [FaultContract(typeof(ServiceFault))]
        public bool? UpdateUserRoles(string userName, string[] addRoleNames, string[] deleteRoleNames)
        {
            try
            {
                bool addRolesValidation = true;
                string[] userRoles = Roles.GetRolesForUser(userName);
                if (addRoleNames.Count() > 0)
                {
                    foreach (string addRole in addRoleNames)
                    {
                        if(userRoles.Contains(addRole))
                            continue;
                        Roles.AddUserToRoles(userName, new string[] { addRole });
                    }                    
                }

                bool deleteRolesValidation = true;
                if (deleteRoleNames.Count() > 0)
                {
                    foreach (string deleteRole in deleteRoleNames)
                    {
                        if(userRoles.Contains(deleteRole))
                            Roles.RemoveUserFromRoles(userName, new string[] { deleteRole });
                    }                         
                }

                return addRolesValidation && deleteRolesValidation;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="username"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <returns>True/False</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool? DeleteRole(string username, bool throwOnPopulatedRole)
        {
            try
            {
                return Roles.DeleteRole(username, throwOnPopulatedRole);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
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
            if (user == null)
                return null;

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
                LastLockoutDate = user.LastLockoutDate,
                LastLogInDate = user.LastLoginDate,
                ProviderUserKey = user.ProviderUserKey.ToString(),
                ProviderName = user.ProviderName,
                PasswordQuestion = user.PasswordQuestion,
                LastPasswordChangedDate = user.LastPasswordChangedDate
            };
        }
        #endregion
    }
}
