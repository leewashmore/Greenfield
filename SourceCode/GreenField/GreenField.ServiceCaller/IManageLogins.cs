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
using GreenField.ServiceCaller.LoginDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.DataContracts;


namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Interface exposing Service LoginOperations Methods
    /// </summary>
    public interface IManageLogins
    {
        #region Service Caller Method Declarations
        #region Membership
        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="callback">True/False</param>
        void ValidateUser(string username, string password, Action<bool?> callback);

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
        void CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, Action<string> callback);

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="callback">True/False</param>
        void ChangePassword(string username, string oldPassword, string newPassword, Action<bool?> callback);

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <param name="callback">New Password</param>
        void ResetPassword(string username, string answer, Action<string> callback);

        /// <summary>
        /// Update Approval Status for Membership User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback">True/False</param>
        void UpdateApprovalForUser(MembershipUserInfo user, Action<bool?> callback);

        /// <summary>
        /// Update Approval Status for multiple Membership Users
        /// </summary>
        /// <param name="users"></param>
        /// <param name="callback">True/False</param>
        void UpdateApprovalForUsers(ObservableCollection<MembershipUserInfo> user, Action<bool?> callback);

        /// <summary>
        /// Unlock Membership  User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="callback">True/False</param>
        void UnlockUser(string userName, Action<bool?> callback);

        /// <summary>
        /// Unlock multiple Membership  Users
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="callback">True/False</param>
        void UnlockUsers(ObservableCollection<string> userNames, Action<bool?> callback);

        /// <summary>
        /// Get Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <param name="callback">MembershipUser</param>
        void GetUser(string username, bool userIsOnline, Action<MembershipUserInfo> callback);

        /// <summary>
        /// Get all Membership Users
        /// </summary>
        /// <param name="callback">Users</param>
        void GetAllUsers(Action<List<MembershipUserInfo>> callback);

        /// <summary>
        /// Delete Membership User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callback">True/False</param>
        void DeleteUser(string usernames, Action<bool?> callback);

        /// <summary>
        /// Delete multiple  Membership Users
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callback">True/False</param>
        void DeleteUsers(ObservableCollection<string> usernames, Action<bool?> callback);
        #endregion

        #region Roles
        /// <summary>
        /// Create Role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="callback">True/False</param>
        void CreateRole(string roleName, Action<bool?> callback);

        /// <summary>
        /// Get all Roles
        /// </summary>
        /// <param name="callback">Roles</param>
        void GetAllRoles(Action<List<string>> callback);

        /// <summary>
        /// Get Roles for Membership User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="callback">Roles</param>
        void GetRolesForUser(string userName, Action<List<string>> callback);

        /// <summary>
        /// Remove Membership Users from Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <param name="callback">True/False</param>
        void RemoveUsersFromRoles(ObservableCollection<string> usernames, ObservableCollection<string> roleNames, Action<bool?> callback);

        /// <summary>
        /// Add Membership Users to Roles
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="roleNames"></param>
        /// <param name="callback"></param>
        void AddUsersToRoles(ObservableCollection<string> usernames, ObservableCollection<string> roleNames, Action<bool?> callback);

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="username"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <param name="callback">True/False</param>
        void DeleteRole(string username, bool throwOnPopulatedRole, Action<bool?> callback);

        /// <summary>
        /// Update Membership User Roles
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="addRoleNames"></param>
        /// <param name="deleteRoleNames"></param>
        /// <param name="callback">True/False</param>
        void UpdateUserRoles(string userName, ObservableCollection<string> addRoleNames, ObservableCollection<string> deleteRoleNames, Action<bool?> callback);
        #endregion 
        #endregion        
    }
}
