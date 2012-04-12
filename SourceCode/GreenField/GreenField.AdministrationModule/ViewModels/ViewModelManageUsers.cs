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
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.LoginDefinitions;
using Microsoft.Practices.Prism.Commands;
using System.Linq;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;

namespace GreenField.AdministrationModule.ViewModels
{
    /// <summary>
    /// View model class for ViewManageUsers
    /// </summary>
    [Export]
    public class ViewModelManageUsers : NotificationObject, INavigationAware
    {
        #region Fields
        /// <summary>
        /// MEF Singleton
        /// </summary>
        private IManageLogins _manageLogins;
        private ILoggerFacade _logger;

        /// <summary>
        /// User specific role collection - DB instance
        /// </summary>
        private ObservableCollection<string> DbUserRoles;

        /// <summary>
        /// All Roles minus user specific role collection - DB instance
        /// </summary>
        private ObservableCollection<string> DbAvailableRoles;
        #endregion

        #region Contructor
        [ImportingConstructor]
        public ViewModelManageUsers(IManageLogins manageLogins, ILoggerFacade logger)
        {
            try
            {
                _manageLogins = manageLogins;
                _logger = logger;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }
        #endregion

        # region Properties
        #region Mambership Users
        /// <summary>
        /// Membership user collection
        /// </summary>
        private ObservableCollection<MembershipUserInfo> _membershipUserInfo;
        public ObservableCollection<MembershipUserInfo> MembershipUserInfo
        {
            get
            {
                if (_membershipUserInfo == null && _manageLogins != null)
                    _manageLogins.GetAllUsers(GetAllUsersCallBackMethod);
                return _membershipUserInfo;
            }
            set
            {
                if (_membershipUserInfo != value)
                {
                    _membershipUserInfo = value;
                    RaisePropertyChanged(() => this.MembershipUserInfo);
                }
            }
        }

        /// <summary>
        /// Multiple select Membership user collection
        /// </summary>
        private ObservableCollection<MembershipUserInfo> _selectedMembershipUsers;
        public ObservableCollection<MembershipUserInfo> SelectedMembershipUsers
        {
            get { return _selectedMembershipUsers; }
            set
            {
                if (_selectedMembershipUsers != value)
                {
                    _selectedMembershipUsers = value;
                    RaisePropertyChanged(() => this.SelectedMembershipUsers);
                    RaisePropertyChanged(() => this.ApproveCommand);
                    RaisePropertyChanged(() => this.ReleaseLockoutCommand);
                    RaisePropertyChanged(() => this.DeleteCommand);

                    if (value.Count == 1)
                    {
                        SelectedMembershipUser = value[0];
                        if (_manageLogins != null)
                        {
                            _manageLogins.GetRolesForUser(value[0].UserName, GetRolesForUserCallBackMethod);
                        }
                    }
                    else
                    {
                        SelectedMembershipUser = null;
                        UserRoles.Clear();
                        AvailableRoles.Clear();
                    }
                }

            }
        }

        /// <summary>
        /// Single select/top select Membership user
        /// </summary>
        private MembershipUserInfo _selectedMembershipUser;
        public MembershipUserInfo SelectedMembershipUser
        {
            get { return _selectedMembershipUser; }
            set
            {
                if (_selectedMembershipUser != value)
                {
                    _selectedMembershipUser = value;
                    RaisePropertyChanged(() => this.SelectedMembershipUser);
                }
            }
        }
        #endregion

        #region Roles
        /// <summary>
        /// User specific role collection - UI instance
        /// </summary>
        private ObservableCollection<string> _userRoles = new ObservableCollection<string>();
        public ObservableCollection<string> UserRoles
        {
            get { return _userRoles; }
            set
            {
                if (_userRoles != value)
                {
                    _userRoles = value;
                    RaisePropertyChanged(() => this.UserRoles);
                    RaisePropertyChanged(() => this.AddAllRolesCommand);
                    RaisePropertyChanged(() => this.RemoveAllRolesCommand);
                }
            }
        }

        /// <summary>
        /// User specific selected role - UI instance
        /// </summary>
        private string _selectedUserRole;
        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set
            {
                if (_selectedUserRole != value)
                {
                    _selectedUserRole = value;
                    RaisePropertyChanged(() => this.SelectedUserRole);
                    RaisePropertyChanged(() => this.RemoveRoleCommand);
                }
            }
        }

        /// <summary>
        /// All Roles minus user specific role collection - UI instance
        /// </summary>
        private ObservableCollection<string> _availableRoles = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                if (_availableRoles != value)
                {
                    _availableRoles = value;
                    RaisePropertyChanged(() => this.AvailableRoles);
                    RaisePropertyChanged(() => this.AddAllRolesCommand);
                    RaisePropertyChanged(() => this.RemoveAllRolesCommand);
                }
            }
        }

        /// <summary>
        /// All Roles minus user specific selected role - UI instance
        /// </summary>
        private string _selectedAvailableRole;
        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set
            {
                if (_selectedAvailableRole != value)
                {
                    _selectedAvailableRole = value;
                    RaisePropertyChanged(() => this.SelectedAvailableRole);
                    RaisePropertyChanged(() => this.AddRoleCommand);
                }
            }
        }
        #endregion

        # region ICommand
        /// <summary>
        /// RadGrid SelectedItems change event handling
        /// </summary>
        public ICommand SelectedItemsChangedCommand
        {
            get { return new DelegateCommand<object>(SelectedItemsChangedCommandMethod); }
        }

        /// <summary>
        /// Change approval status event handling
        /// </summary>
        public ICommand ApproveCommand
        {
            get { return new DelegateCommand<object>(ApproveCommandMethod, ValidateSelection); }
        }

        /// <summary>
        /// Release Lockout event handling
        /// </summary>
        public ICommand ReleaseLockoutCommand
        {
            get { return new DelegateCommand<object>(ReleaseLockoutCommandMethod, ValidateSelection); }
        }

        /// <summary>
        /// Delete user event handling
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod, ValidateSelection); }
        }

        /// <summary>
        /// Add Role to user event handling
        /// </summary>
        public ICommand AddRoleCommand
        {
            get { return new DelegateCommand<object>(AddRoleCommandMethod, AddRoleCommandValidation); }
        }

        /// <summary>
        /// Add all roles to user event handling
        /// </summary>
        public ICommand AddAllRolesCommand
        {
            get { return new DelegateCommand<object>(AddAllRolesCommandMethod, AddAllRolesCommandValidation); }
        }

        /// <summary>
        /// Remeve Role from user event handling
        /// </summary>
        public ICommand RemoveRoleCommand
        {
            get { return new DelegateCommand<object>(RemoveRoleCommandMethod, RemoveRoleCommandValidation); }
        }

        /// <summary>
        /// Remove all roles from user event handling
        /// </summary>
        public ICommand RemoveAllRolesCommand
        {
            get { return new DelegateCommand<object>(RemoveAllRolesCommandMethod, RemoveAllRolesCommandValidation); }
        }

        /// <summary>
        /// Commit Role change to database event handling
        /// </summary>
        public ICommand UserRoleSaveChangesCommand
        {
            get { return new DelegateCommand<object>(UserRoleSaveChangesCommandMethod); }
        }
        #endregion
        #endregion

        #region ICommand Methods
        #region Mambership User
        /// <summary>
        /// Validate selection in MembershipUser RadgridView before approve/Release lockout/Delete are implemented
        /// </summary>
        /// <param name="param">SenderInfo</param>
        /// <returns>True/False</returns>
        private bool ValidateSelection(object param)
        {
            return SelectedMembershipUsers != null ? SelectedMembershipUsers.Count > 0 ? true : false : false;
        }

        /// <summary>
        /// Change approval status event handling
        /// </summary>
        private void ApproveCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            
            try
            {
                if (_manageLogins != null)
                {
                    string confirmationMessage = String.Empty;
                    int blockedLoginCount = SelectedMembershipUsers.Where(user => user.IsApproved == false).Count();
                    List<MembershipUserInfo> blockedLogins = SelectedMembershipUsers.Where(user => user.IsApproved == false).ToList();
                    if (blockedLoginCount > 0)
                    {
                        int selectedUserIndex = 0;
                        confirmationMessage = "Approve following Login(s) - ";
                        if (blockedLoginCount == 1)
                            confirmationMessage = confirmationMessage + "'" + blockedLogins[0].UserName + "' ?";
                        else
                            foreach (MembershipUserInfo user in blockedLogins)
                                confirmationMessage = ++selectedUserIndex < blockedLoginCount
                                    ? confirmationMessage = confirmationMessage + "'" + user.UserName + "', "
                                    : confirmationMessage = confirmationMessage + " and '" + user.UserName + "' ?";
                    }

                    int approvedLoginCount = SelectedMembershipUsers.Where(user => user.IsApproved == true).Count();
                    List<MembershipUserInfo> approvedLogins = SelectedMembershipUsers.Where(user => user.IsApproved == true).ToList();
                    if (approvedLoginCount > 0)
                    {
                        int selectedUserIndex = 0;
                        confirmationMessage = confirmationMessage + (confirmationMessage.Length > 0 ? " and " : "") + "Block following Login(s) - ";
                        if (approvedLoginCount == 1)
                            confirmationMessage = confirmationMessage + "'" + approvedLogins[0].UserName + "' ?";
                        else
                            foreach (MembershipUserInfo user in approvedLogins)
                                confirmationMessage = ++selectedUserIndex < approvedLoginCount
                                    ? confirmationMessage = confirmationMessage + "'" + user.UserName + "', "
                                    : confirmationMessage = confirmationMessage + " and '" + user.UserName + "' ?";
                    }

                    if (MessageBox.Show(confirmationMessage, "Approve Logins", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (MembershipUserInfo user in SelectedMembershipUsers)
                        {
                            user.IsApproved = !(user.IsApproved);
                        }

                        #region UpdateApprovalForUsers Service Call
                        _manageLogins.UpdateApprovalForUsers(SelectedMembershipUsers, (result) =>
                        {
                            string updateApprovalMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            Logging.LogBeginMethod(_logger, updateApprovalMethodNamespace);

                            try
                            {
                                if (result != null)
                                {
                                    Logging.LogMethodParameter(_logger, updateApprovalMethodNamespace, result, 1);
                                    if ((bool)result)
                                    {
                                        foreach (MembershipUserInfo selectedUser in SelectedMembershipUsers)
                                        {
                                            if (selectedUser.IsApproved)
                                                Logging.LogLoginActivate(_logger, selectedUser.UserName);
                                            else
                                                Logging.LogLoginBlock(_logger, selectedUser.UserName);
                                        }
                                    }
                                    else
                                    {
                                        foreach (MembershipUserInfo selectedUser in SelectedMembershipUsers)
                                        {
                                            if (selectedUser.IsApproved)
                                                Logging.LogLoginActivateFailed(_logger, selectedUser.UserName);
                                            else
                                                Logging.LogLoginBlockFailed(_logger, selectedUser.UserName);
                                            MembershipUserInfo.Where(u => u.UserName == selectedUser.UserName).First().IsApproved = !(selectedUser.IsApproved);

                                        }
                                    }
                                }
                                else
                                {
                                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                Logging.LogLoginException(_logger, ex);
                            }
                        }); 
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);

        }

        /// <summary>
        /// Release Lockout event handling
        /// </summary>
        private void ReleaseLockoutCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (_manageLogins != null)
                {
                    ObservableCollection<string> lockedLogins = new ObservableCollection<string>();
                    foreach (MembershipUserInfo user in SelectedMembershipUsers)
                    {
                        if (user.IsLockedOut == true)
                        {
                            lockedLogins.Add(user.UserName);
                        }
                    }

                    string confirmationMessage = String.Empty;
                    int lockedLoginCount = lockedLogins.Count;

                    if (lockedLoginCount > 0)
                    {
                        int selectedUserIndex = 0;
                        confirmationMessage = "Release Lockout for following Login(s) - ";
                        if (lockedLoginCount == 1)
                            confirmationMessage = confirmationMessage + "'" + lockedLogins[0] + "' ?";
                        else
                            foreach (string user in lockedLogins)
                                confirmationMessage = ++selectedUserIndex < lockedLoginCount
                                    ? confirmationMessage = confirmationMessage + "'" + user + "', "
                                    : confirmationMessage = confirmationMessage + " and '" + user + "' ?";

                        if (MessageBox.Show(confirmationMessage, "Release Lockout", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            #region UnlockUsers Service Call
                            _manageLogins.UnlockUsers(lockedLogins, (result) =>
                            {
                                string unlockUsersMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                Logging.LogBeginMethod(_logger, unlockUsersMethodNamespace);
                                try
                                {
                                    if (result != null)
                                    {
                                        Logging.LogMethodParameter(_logger, unlockUsersMethodNamespace, result, 1);
                                        if ((bool)result)
                                        {
                                            foreach (MembershipUserInfo selectedUser in SelectedMembershipUsers)
                                                MembershipUserInfo.Where(u => u.UserName == selectedUser.UserName && lockedLogins.Contains(selectedUser.UserName)).First().IsLockedOut = false;
                                            foreach (string item in lockedLogins)
                                                _logger.Log(String.Format("Account '{0}' unlocked by {1}",
                                                    item, SessionManager.SESSION.UserName), Category.Info, Priority.None);
                                        }
                                    }
                                    else
                                    {
                                        Logging.LogMethodParameterNull(_logger, unlockUsersMethodNamespace, 1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                    Logging.LogLoginException(_logger, ex);
                                }
                                Logging.LogEndMethod(_logger, methodNamespace);
                            }); 
                            #endregion
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Locked Logins", "Release Lockout", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Delete user event handling
        /// </summary>
        private void DeleteCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (_manageLogins != null)
                {
                    int selectedUserIndex = 0;
                    string confirmationMessage = "Remove following user(s) - ";
                    if (SelectedMembershipUsers.Count == 1)
                        confirmationMessage = confirmationMessage + "'" + ((MembershipUserInfo)SelectedMembershipUsers[0]).UserName + "' ?";
                    else
                        foreach (MembershipUserInfo user in SelectedMembershipUsers)
                            confirmationMessage = ++selectedUserIndex < SelectedMembershipUsers.Count
                                ? confirmationMessage + "'" + user.UserName + "', "
                                : confirmationMessage + " and '" + user.UserName + "' ?";

                    if (MessageBox.Show(confirmationMessage, "Delete Users", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        ObservableCollection<string> deleteUsers = new ObservableCollection<string>();

                        foreach (MembershipUserInfo user in SelectedMembershipUsers)
                            deleteUsers.Add(user.UserName);

                        _manageLogins.DeleteUsers(deleteUsers, (result) =>
                            {
                                string deleteUsersMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                Logging.LogBeginMethod(_logger, deleteUsersMethodNamespace);
                                try
                                {
                                    if (result != null)
                                    {
                                        Logging.LogMethodParameter(_logger, deleteUsersMethodNamespace, result, 1);
                                        if ((bool)result)
                                        {
                                            foreach (string user in deleteUsers)
                                            {
                                                Logging.LogLoginDelete(_logger, user);
                                            }
                                            _manageLogins.GetAllUsers(GetAllUsersCallBackMethod);

                                            if (UserRoles != null)
                                            {
                                                UserRoles.Clear();
                                            }
                                            if (AvailableRoles != null)
                                            {
                                                AvailableRoles.Clear();
                                            }
                                            if (SelectedMembershipUser != null)
                                            {
                                                SelectedMembershipUser = null;
                                            }
                                        }
                                        else
                                        {
                                            foreach (string user in deleteUsers)
                                            {
                                                Logging.LogLoginDeleteFailed(_logger, user);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Logging.LogMethodParameterNull(_logger, deleteUsersMethodNamespace, 1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                    Logging.LogLoginException(_logger, ex);
                                }
                                Logging.LogEndMethod(_logger, methodNamespace);
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// RadGrid SelectedItems change event handling
        /// </summary>
        private void SelectedItemsChangedCommandMethod(object param)
        {
            try
            {
                ObservableCollection<MembershipUserInfo> SelectedUsers = new ObservableCollection<MembershipUserInfo>();
                Collection<object> selectedUsers = param as Collection<object>;
                foreach (MembershipUserInfo user in selectedUsers)
                    SelectedUsers.Add(user);

                SelectedMembershipUsers = SelectedUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }
        #endregion

        #region Roles
        /// <summary>
        /// Add Role to user selection validation
        /// </summary>
        private bool AddRoleCommandValidation(object param)
        {
            return SelectedAvailableRole != null;
        }

        /// <summary>
        /// Add all roles to user event handling
        /// </summary>
        private void AddRoleCommandMethod(object param)
        {
            try
            {
                UserRoles.Add(SelectedAvailableRole);
                AvailableRoles.Remove(SelectedAvailableRole);
                RaisePropertyChanged(() => this.RemoveAllRolesCommand);
                RaisePropertyChanged(() => this.AddAllRolesCommand);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        /// <summary>
        /// Add all roles to user selection validation
        /// </summary>
        private bool AddAllRolesCommandValidation(object param)
        {
            try
            {
                if (AvailableRoles != null)
                    return AvailableRoles.Count > 0;                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);                
            }

            return false;
        }

        /// <summary>
        /// Add all roles to user event handling
        /// </summary>
        private void AddAllRolesCommandMethod(object param)
        {
            try
            {
                foreach (string item in AvailableRoles)
                    UserRoles.Add(item);
                AvailableRoles.Clear();
                RaisePropertyChanged(() => this.RemoveAllRolesCommand);
                RaisePropertyChanged(() => this.AddAllRolesCommand);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        /// <summary>
        /// Remeve Role from user selection validation
        /// </summary>
        private bool RemoveRoleCommandValidation(object param)
        {
            return SelectedUserRole != null;
        }

        /// <summary>
        /// Remeve Role from user event handling
        /// </summary>
        private void RemoveRoleCommandMethod(object param)
        {
            try
            {
                AvailableRoles.Add(SelectedUserRole);
                UserRoles.Remove(SelectedUserRole);
                RaisePropertyChanged(() => this.RemoveAllRolesCommand);
                RaisePropertyChanged(() => this.AddAllRolesCommand);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        /// <summary>
        /// Remove all roles from user selection validation
        /// </summary>
        private bool RemoveAllRolesCommandValidation(object param)
        {
            try
            {
                if (UserRoles != null)
                    return UserRoles.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            return false;
        }

        /// <summary>
        /// Remove all roles from user event handling
        /// </summary>
        private void RemoveAllRolesCommandMethod(object param)
        {
            try
            {
                foreach (string item in UserRoles)
                    AvailableRoles.Add(item);
                UserRoles.Clear();
                RaisePropertyChanged(() => this.RemoveAllRolesCommand);
                RaisePropertyChanged(() => this.AddAllRolesCommand);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        /// <summary>
        /// Commit Role change to database event handling
        /// </summary>
        private void UserRoleSaveChangesCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (_manageLogins != null)
                {
                    ObservableCollection<string> additionRoles = new ObservableCollection<string>();
                    ObservableCollection<string> deletionRoles = new ObservableCollection<string>();

                    foreach (string role in DbUserRoles)
                        if (!(UserRoles.Any(item => item == role)))
                            deletionRoles.Add(role);

                    foreach (string role in UserRoles)
                        if (!(DbUserRoles.Any(item => item == role)))
                            additionRoles.Add(role);


                    #region UpdateUserRoles Service Call
                    _manageLogins.UpdateUserRoles(SelectedMembershipUser.UserName, additionRoles, deletionRoles, (result) =>
                    {
                        string updateUserRolesMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                        Logging.LogBeginMethod(_logger, updateUserRolesMethodNamespace);
                        try
                        {
                            if (result != null)
                            {
                                Logging.LogMethodParameter(_logger, updateUserRolesMethodNamespace, result, 1);
                                if ((bool)result)
                                {
                                    foreach (string role in additionRoles)
                                    {
                                        Logging.LogLoginRoleAssign(_logger, SelectedMembershipUser.UserName, role);
                                    }

                                    foreach (string role in deletionRoles)
                                    {
                                        Logging.LogLoginRoleRemove(_logger, SelectedMembershipUser.UserName, role);
                                    }
                                }
                                else
                                {
                                    foreach (string role in additionRoles)
                                    {
                                        Logging.LogLoginRoleAssignFailed(_logger, SelectedMembershipUser.UserName, role);
                                    }

                                    foreach (string role in deletionRoles)
                                    {
                                        Logging.LogLoginRoleRemoveFailed(_logger, SelectedMembershipUser.UserName, role);
                                    }

                                    UserRoles = DbUserRoles;
                                    AvailableRoles = DbAvailableRoles;
                                }
                            }
                            else
                            {
                                Logging.LogMethodParameterNull(_logger, updateUserRolesMethodNamespace, 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                            Logging.LogLoginException(_logger, ex);
                        }
                        Logging.LogEndMethod(_logger, methodNamespace);
                    }); 
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion
        #endregion

        #region Callback Methods
        #region Membership User
        /// <summary>
        /// GetAllUsers callback Methods
        /// </summary>
        /// <param name="val">List of MembershipUserInfo class objects</param>
        private void GetAllUsersCallBackMethod(List<MembershipUserInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            if (result != null)
            {
                try
                {
                    if (result.Count > 0)
                        MembershipUserInfo = new ObservableCollection<MembershipUserInfo>(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }
            else
            {
                MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
            }

            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Roles
        /// <summary>
        /// GetRolesForUser callback Methods
        /// </summary>
        /// <param name="val">List of Roles</param>
        private void GetRolesForUserCallBackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            if (result != null)
            {
                try
                {
                    UserRoles = new ObservableCollection<string>(result);
                    DbUserRoles = new ObservableCollection<string>(result);
                    _manageLogins.GetAllRoles(GetAllRolesCallBackMethod);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }
            else
            {
                MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
            }

            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GetAllRoles callback Methods
        /// </summary>
        /// <param name="val">List of Roles</param>
        private void GetAllRolesCallBackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            if (result != null)
            {
                try
                {
                    if (UserRoles != null)
                    {
                        if (AvailableRoles != null)
                        {
                            AvailableRoles = new ObservableCollection<string>(result.Where(userRole => !(UserRoles.Contains(userRole))));
                        }                        

                        if (DbAvailableRoles != null)
                        {
                            DbAvailableRoles = new ObservableCollection<string>(result.Where(userRole => !(UserRoles.Contains(userRole))));
                        } 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }
            else
            {
                MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
            }

            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion
        #endregion

        #region INavigation Aware Methods
        /// <summary>
        /// Validation before navigation - Always True
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns>True</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// On Navigation from view - no implementation
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// On Navigation from view - refresh user list
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            try
            {
                if (SelectedMembershipUsers != null)
                {
                    if (_manageLogins != null)
                    {
                        _manageLogins.GetAllUsers(GetAllUsersCallBackMethod);
                    }
                    SelectedMembershipUser = null;
                    SelectedUserRole = null;
                    SelectedAvailableRole = null;
                    UserRoles.Clear();
                    AvailableRoles.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }
        #endregion
    }
}
