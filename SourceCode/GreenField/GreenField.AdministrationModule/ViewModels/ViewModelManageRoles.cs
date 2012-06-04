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
using System.Collections.Generic;
using GreenField.ServiceCaller;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using GreenField.AdministrationModule.Views;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;

namespace GreenField.AdministrationModule.ViewModels
{
    /// <summary>
    /// View model class for ViewManageRoles
    /// </summary>
    [Export]
    public class ViewModelManageRoles : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singleton
        /// </summary>
        private IManageLogins _manageLogins;
        private ILoggerFacade _logger;
        #endregion

        #region Contructor
        [ImportingConstructor]
        public ViewModelManageRoles(IManageLogins manageLogins, ILoggerFacade logger)
        {
            _manageLogins = manageLogins;
            _logger = logger;
        }
        #endregion

        # region Properties
        #region UI Fields
        /// <summary>
        /// All Roles Collection
        /// </summary>
        private ObservableCollection<string> _allRoles;
        public ObservableCollection<string> AllRoles
        {
            get
            {
                if (_allRoles == null && _manageLogins != null)
                    _manageLogins.GetAllRoles(GetAllRolesCallBackMethod);
                return _allRoles;
            }
            set
            {
                if (_allRoles != value)
                {
                    _allRoles = value;
                    RaisePropertyChanged(() => this.AllRoles);
                }
            }
        }

        /// <summary>
        /// Selected Roles Collection
        /// </summary>
        private ObservableCollection<string> _selectedRoles;
        public ObservableCollection<string> SelectedRoles
        {
            get { return _selectedRoles; }
            set
            {
                if (_selectedRoles != value)
                {
                    _selectedRoles = value;
                    RaisePropertyChanged(() => SelectedRoles);
                    RaisePropertyChanged(() => this.DeleteRoleCommand);
                }
            }
        }

        /// <summary>
        /// Create Role Name
        /// </summary>
        private string _newRoleName;
        public string NewRoleName
        {
            get { return _newRoleName; }
            set
            {
                if (_newRoleName != value)
                {
                    _newRoleName = value;
                    RaisePropertyChanged(() => NewRoleName);
                }
            }
        }
        #endregion

        # region ICommand
        /// <summary>
        /// ICommand- Create Role
        /// </summary>
        public ICommand CreateRoleCommand
        {
            get { return new DelegateCommand<object>(CreateRoleCommandMethod); }
        }

        /// <summary>
        /// ICommand - Delete Role
        /// </summary>
        public ICommand DeleteRoleCommand
        {
            get { return new DelegateCommand<object>(DeleteRoleCommandMethod, DeleteRoleCommandValidation); }
        }

        /// <summary>
        /// ICommand - SelectedItems Change
        /// </summary>
        public ICommand SelectedItemsChangedCommand
        {
            get { return new DelegateCommand<object>(SelectedItemsChangedCommandMethod); }
        }
        #endregion
        #endregion

        #region ICommand Methods
        /// <summary>
        /// CreateRoleCommand Execution Method - Create new role
        /// </summary>
        /// <param name="param"></param>
        private void CreateRoleCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                ChildCreateNewRole childCreateNewRole = new ChildCreateNewRole() { Roles = AllRoles };
                childCreateNewRole.Show();
                childCreateNewRole.Unloaded += (se, e) =>
                 {
                     if (childCreateNewRole.DialogResult == true)
                     {
                         NewRoleName = childCreateNewRole.txtEnterValue.Text;
                         if (_manageLogins != null)
                         {
                             #region CreateRole Service Call
                             _manageLogins.CreateRole(NewRoleName, (result) =>
                            {
                                string createRoleMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                Logging.LogBeginMethod(_logger, createRoleMethodNamespace);
                                try
                                {
                                    if (result != null)
                                    {
                                        Logging.LogMethodParameter(_logger, createRoleMethodNamespace, result, 1);
                                        if ((bool)result)
                                        {
                                            AllRoles.Add(NewRoleName);
                                            Logging.LogRoleCreate(_logger, NewRoleName);
                                        }
                                        else
                                        {
                                            Prompt.ShowDialog("Message: Role creation failed\nStacktrace: " + methodNamespace);
                                            Logging.LogRoleCreateFailed(_logger, NewRoleName);
                                        }
                                    }
                                    else
                                    {
                                        Logging.LogMethodParameterNull(_logger, createRoleMethodNamespace, 1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                    Logging.LogLoginException(_logger, ex);
                                }
                                Logging.LogEndMethod(_logger, methodNamespace);
                            });
                             #endregion
                         }
                     }
                 };
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Delete Role ICommand Implementation
        /// </summary>
        /// <param name="param"></param>
        private void DeleteRoleCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                int selectedUserIndex = 0;
                string confirmationMessage = "Remove following role(s) - ";
                if (SelectedRoles.Count == 1)
                    confirmationMessage = confirmationMessage + "'" + SelectedRoles[0] + "' ?";
                else
                    foreach (string role in SelectedRoles)
                        confirmationMessage = ++selectedUserIndex < SelectedRoles.Count
                            ? confirmationMessage = confirmationMessage + "'" + role + "', "
                            : confirmationMessage = confirmationMessage + " and '" + role + "' ?";

                Prompt.ShowDialog(confirmationMessage, "Delete Roles", MessageBoxButton.OKCancel, (messageResult) =>
                {
                    if (messageResult == MessageBoxResult.OK)
                    {
                        foreach (string role in SelectedRoles)
                        {
                            if (_manageLogins != null)
                            {
                                #region DeleteRole Service Call
                                _manageLogins.DeleteRole(role, false, (result) =>
                                {
                                    string deleteRoleMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                    Logging.LogBeginMethod(_logger, deleteRoleMethodNamespace);
                                    try
                                    {
                                        if (result != null)
                                        {
                                            Logging.LogMethodParameter(_logger, deleteRoleMethodNamespace, result, 1);
                                            if ((bool)result)
                                            {
                                                AllRoles.Remove(role);
                                                Logging.LogRoleDelete(_logger, role);
                                            }
                                            else
                                            {
                                                Prompt.ShowDialog("Message: Role deletion failed\nStacktrace: " + methodNamespace);
                                                Logging.LogRoleDeleteFailed(_logger, role);
                                            }
                                        }

                                        else
                                        {
                                            Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                        Logging.LogLoginException(_logger, ex);
                                    }
                                    Logging.LogEndMethod(_logger, methodNamespace);
                                });
                                #endregion
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Delete Role ICommand Validation
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool DeleteRoleCommandValidation(object param)
        {
            if (SelectedRoles != null)
                return SelectedRoles.Count > 0 ? true : false;
            return false;
        }

        /// <summary>
        /// SelectedItems ICommand Implementation
        /// </summary>
        /// <param name="param"></param>
        private void SelectedItemsChangedCommandMethod(object param)
        {
            try
            {
                ObservableCollection<string> _SelectedRoles = new ObservableCollection<string>();
                Collection<object> selectedRoles = param as Collection<object>;
                foreach (string role in selectedRoles)
                    _SelectedRoles.Add(role);

                SelectedRoles = _SelectedRoles;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }
        #endregion

        #region CallBackMethods
        /// <summary>
        /// GetAllRoles CallBack Method
        /// </summary>
        /// <param name="val">List of Roles</param>
        private void GetAllRolesCallBackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            if (result != null)
            {
                Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                try
                {
                    if (result.Count != 0)
                        AllRoles = new ObservableCollection<string>(result);
                }
                catch (Exception ex)
                {
                    Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }
            else
            {
                Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
            }

            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion
    }
}
