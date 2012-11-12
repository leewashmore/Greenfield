using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Modularity;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using GreenField.Common;
using GreenField.ServiceCaller;
using System.Windows.Browser;
using System.Resources;
using GreenField.LoginModule.Controls;
using GreenField.LoginModule.Resources;
using GreenField.ServiceCaller.LoginDefinitions;
using GreenField.ServiceCaller.SessionDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;

namespace GreenField.LoginModule.ViewModel
{
    /// <summary>
    /// View model class for ViewLoginForm
    /// </summary>
    [Export]
    public class ViewModelLoginForm : NotificationObject, INavigationAware
    {
        #region Fields
        /// <summary>
        /// Stores roles assigned to login user
        /// </summary>
        private List<string> UserRoles = new List<string>();

        /// <summary>
        /// Visualizations for field validation states
        /// </summary>
        private static Brush VALID_BRUSH = new SolidColorBrush(Colors.Black);
        private static Brush INVALID_BRUSH = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Enumeration for field validation states
        /// </summary>
        public enum FieldState { MissingField, InvalidField, LoginLocked, LoginUnapproved, ValidField };

        /// <summary>
        /// MEF singletons
        /// </summary>
        private IManageLogins _manageLogins;
        private IManageSessions _manageSessions;
        private IRegionManager _regionManager;
        private ILoggerFacade _logger;
        #endregion

        #region Contructor
        [ImportingConstructor]
        public ViewModelLoginForm(IManageLogins manageLogins, IManageSessions manageSessions, IRegionManager regionManager, ILoggerFacade logger)
        {
            _manageLogins = manageLogins;
            _manageSessions = manageSessions;
            _regionManager = regionManager;
            _logger = logger;

            try
            {
                if (_manageSessions != null)
                {
                    #region GetSession Service Call
                    _manageSessions.GetSession((result) =>
                    {
                        string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                        if (result != null)
                        {
                            try
                            {
                                Session session = result as Session;
                                Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1, result.UserName);
                                Logging.LogSessionClose(_logger, result.UserName);
                            }
                            catch (Exception ex)
                            {
                                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                Logging.LogException(_logger, ex);
                            }
                        }
                    });
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// Property binding Login ID TextBlock Text property
        /// </summary>
        private string _loginIdText = string.Empty;
        public string LoginIdText
        {
            get { return _loginIdText; }
            set
            {
                if (_loginIdText != value)
                    _loginIdText = value;
                RaisePropertyChanged(() => this.LoginIdText);
                //field validation state to valid on every set operation
                LoginIdState = FieldState.ValidField;
            }
        }

        /// <summary>
        /// Property storing Login ID field validation state
        /// </summary>
        private FieldState _loginIdState = FieldState.ValidField;
        public FieldState LoginIdState
        {
            get { return _loginIdState; }
            set
            {
                _loginIdState = value;

                //Visualization changes based on field validation state
                LoginBorderBrush = value != FieldState.ValidField ? INVALID_BRUSH : VALID_BRUSH;
                InvalidCredentialsPopupIsOpen = value == FieldState.InvalidField;
                MissingFieldPopupIsOpen = value == FieldState.MissingField;
                UnapprovedLoginPopupIsOpen = value == FieldState.LoginUnapproved;
                LockedOutLoginPopupIsOpen = value == FieldState.LoginLocked;
            }
        }

        /// <summary>
        /// Property binding Password PasswordBlock Text property
        /// </summary>
        private string _passwordText = string.Empty;
        public string PasswordText
        {
            get { return _passwordText; }
            set
            {
                if (_passwordText != value)
                    _passwordText = value;
                RaisePropertyChanged(() => this.PasswordText);
                //field validation state to valid on every set operation
                PasswordState = FieldState.ValidField;
            }
        }

        /// <summary>
        /// Property storing Password field validation state
        /// </summary>
        private FieldState _passwordState = FieldState.ValidField;
        public FieldState PasswordState
        {
            get { return _passwordState; }
            set
            {
                _passwordState = value;
                //Visualization changes based on field validation state
                PasswordBorderBrush = value != FieldState.ValidField ? INVALID_BRUSH : VALID_BRUSH;
                InvalidCredentialsPopupIsOpen = value == FieldState.InvalidField;
                MissingFieldPopupIsOpen = value == FieldState.MissingField;
                UnapprovedLoginPopupIsOpen = value == FieldState.LoginUnapproved;
                LockedOutLoginPopupIsOpen = value == FieldState.LoginLocked;
            }
        }
        #endregion

        #region Visualization
        #region UI Field Brush
        /// <summary>
        /// Property binding to the Login ID Textbox BorderBrush and Login ID TextBlock Foreground attributes
        /// </summary>
        private Brush _loginBorderBrush = VALID_BRUSH;
        public Brush LoginBorderBrush
        {
            get { return _loginBorderBrush; }
            set
            {
                if (_loginBorderBrush != value)
                {
                    _loginBorderBrush = value;
                    RaisePropertyChanged(() => this.LoginBorderBrush);
                }
            }
        }

        /// <summary>
        /// Property binding to the Password PasswordBox BorderBrush and Password TextBlock Foreground attributes
        /// </summary>
        private Brush _passwordBorderBrush = VALID_BRUSH;
        public Brush PasswordBorderBrush
        {
            get { return _passwordBorderBrush; }
            set
            {
                if (_passwordBorderBrush != value)
                {
                    _passwordBorderBrush = value;
                    RaisePropertyChanged(() => this.PasswordBorderBrush);
                }
            }
        }
        #endregion

        #region UI Field Invalid Error Popup
        /// <summary>
        /// Property binding to InvalidCredentialsPopup IsOpen attribute
        /// </summary>
        private bool _invalidCredentialsPopupIsOpen = false;
        public bool InvalidCredentialsPopupIsOpen
        {
            get { return _invalidCredentialsPopupIsOpen; }
            set
            {
                if (_invalidCredentialsPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _invalidCredentialsPopupIsOpen = value;
                    RaisePropertyChanged(() => this.InvalidCredentialsPopupIsOpen);
                }
            }
        }

        /// <summary>
        /// Property binding to MissingFieldPopup IsOpen attribute
        /// </summary>
        private bool _missingFieldPopupIsOpen = false;
        public bool MissingFieldPopupIsOpen
        {
            get { return _missingFieldPopupIsOpen; }
            set
            {
                if (_missingFieldPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _missingFieldPopupIsOpen = value;
                    RaisePropertyChanged(() => this.MissingFieldPopupIsOpen);
                }
            }
        }

        /// <summary>
        /// Property binding to UnapprovedLoginPopup IsOpen attribute
        /// </summary>
        private bool _unapprovedLoginPopupIsOpen = false;
        public bool UnapprovedLoginPopupIsOpen
        {
            get { return _unapprovedLoginPopupIsOpen; }
            set
            {
                if (_unapprovedLoginPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _unapprovedLoginPopupIsOpen = value;
                    RaisePropertyChanged(() => this.UnapprovedLoginPopupIsOpen);
                }
            }
        }

        /// <summary>
        /// Property binding to LockedOutLoginPopup IsOpen attribute
        /// </summary>
        private bool _lockedOutLoginPopupIsOpen = false;
        public bool LockedOutLoginPopupIsOpen
        {
            get { return _lockedOutLoginPopupIsOpen; }
            set
            {
                if (_lockedOutLoginPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _lockedOutLoginPopupIsOpen = value;
                    RaisePropertyChanged(() => this.LockedOutLoginPopupIsOpen);
                }
            }
        }

        #endregion

        #region Error Popup Childs
        /// <summary>
        /// Property storing ErrorMessage Resource
        /// </summary>
        public ResourceManager ErrorResourceManager
        {
            get { return new ResourceManager(typeof(ErrorMessages)); }
        }

        /// <summary>
        /// Property binding to InvalidCredentialsPopup Child attribute
        /// </summary>
        private UIElement _missingFieldPopupChild;
        public UIElement MissingFieldPopupChild
        {
            get
            {
                if (_missingFieldPopupChild == null)
                    _missingFieldPopupChild = new ErrorMessage(ErrorResourceManager.GetString("MissingLoginFieldsError"));
                return _missingFieldPopupChild;
            }
            set
            {

                if (_missingFieldPopupChild != value)
                {
                    _missingFieldPopupChild = value;
                    RaisePropertyChanged(() => this.MissingFieldPopupChild);
                }
            }
        }

        /// <summary>
        /// Property binding to MissingFieldPopup Child attribute
        /// </summary>
        private UIElement _invalidCredentialsPopupChild;
        public UIElement InvalidCredentialsPopupChild
        {
            get
            {
                if (_invalidCredentialsPopupChild == null)
                    _invalidCredentialsPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidCredentialsError"));
                return _invalidCredentialsPopupChild;
            }
            set
            {
                if (_invalidCredentialsPopupChild != value)
                {
                    _invalidCredentialsPopupChild = value;
                    RaisePropertyChanged(() => this.InvalidCredentialsPopupChild);
                }
            }
        }

        /// <summary>
        /// Property binding to UnapprovedLoginPopup Child attribute
        /// </summary>
        private UIElement _unapprovedLoginPopupChild;
        public UIElement UnapprovedLoginPopupChild
        {
            get
            {
                if (_unapprovedLoginPopupChild == null)
                    _unapprovedLoginPopupChild = new ErrorMessage(ErrorResourceManager.GetString("LoginUnapprovedError"));
                return _unapprovedLoginPopupChild;
            }
            set
            {

                if (_unapprovedLoginPopupChild != value)
                {
                    _unapprovedLoginPopupChild = value;
                    RaisePropertyChanged(() => this.UnapprovedLoginPopupChild);
                }
            }
        }

        /// <summary>
        /// Property binding to LockedOutLoginPopup Child attribute
        /// </summary>
        private UIElement _lockedOutLoginPopupChild;
        public UIElement LockedOutLoginPopupChild
        {
            get
            {
                if (_lockedOutLoginPopupChild == null)
                    _lockedOutLoginPopupChild = new ErrorMessage(ErrorResourceManager.GetString("LoginLockedOutError"));
                return _lockedOutLoginPopupChild;
            }
            set
            {
                if (_lockedOutLoginPopupChild != value)
                {
                    _lockedOutLoginPopupChild = value;
                    RaisePropertyChanged(() => this.LockedOutLoginPopupChild);
                }
            }
        }

        #endregion
        #endregion

        #region AnimationDataTrigger
        /// <summary>
        /// Property binded to DataTrigger that triggers flipping animation play
        /// </summary>
        private bool _navigateTo;
        public bool NavigateTo
        {
            get { return _navigateTo; }
            set
            {
                if (_navigateTo != value)
                {
                    _navigateTo = value;
                    RaisePropertyChanged(() => this.NavigateTo);
                }
            }
        }
        #endregion

        #region ICommands
        /// <summary>
        /// Property binding to Login button click event or Login ID/Password Enter Press event
        /// </summary>
        public ICommand LoginCommand
        {
            get { return new DelegateCommand<object>(LoginCommandMethod); }
        }

        /// <summary>
        /// Property binding to Register hyperlink click event
        /// </summary>
        public ICommand RegisterCommand
        {
            get { return new DelegateCommand<object>(RegisterCommandMethod); }
        }

        /// <summary>
        /// Property binding to Change Password hyperlink click event
        /// </summary
        public ICommand PasswordChangeCommand
        {
            get { return new DelegateCommand<object>(PasswordChangeCommandMethod); }
        }

        /// <summary>
        /// Property binding to Forgot Password hyperlink click event
        /// </summary
        public ICommand PasswordResetCommand
        {
            get { return new DelegateCommand<object>(PasswordResetCommandMethod); }
        }
        #endregion
        #endregion

        #region ICommand Methods
        /// <summary>
        /// Register Hyperlink click event implementation - navigate to ViewRegisterForm 
        /// </summary>
        /// <param name="param"></param>
        private void RegisterCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewRegisterForm", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        }

        /// <summary>
        /// Login button click event or Login ID/Password Enter Press event implementation - validate credentials
        /// </summary>
        /// <param name="param"></param>
        private void LoginCommandMethod(object param)
        {
            CredentialValidation();
        }

        /// <summary>
        /// Change Password Hyperlink click event implementation - navigate to ViewPasswordChangeForm 
        /// </summary>
        /// <param name="param"></param>
        private void PasswordChangeCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewPasswordChangeForm", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        }

        /// <summary>
        /// Forgot Password Hyperlink click event implementation - navigate to ViewPasswordResetForm 
        /// </summary>
        /// <param name="param"></param>
        private void PasswordResetCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewPasswordResetForm", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        }
        #endregion

        #region INavigationAware Methods
        /// <summary>
        /// Always true
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns></returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// NavigateTo Property set to false
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            NavigateTo = false;
        }

        /// <summary>
        /// NavigateTo Property set to true; default settings
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavigateTo = true;
            DefaultSettings();
        }

        #endregion

        #region Validation Methods
        /// <summary>
        /// Validate Login Credentials
        /// </summary>
        /// <returns>True/False</returns>
        private bool CredentialValidation()
        {
            bool credentialValidation = MissingValidations();
            try
            {
                if (_manageLogins != null)
                {
                    if (credentialValidation)
                    {
                        #region GetUser Service Call
                        _manageLogins.GetUser(LoginIdText, false, (user) =>
                        {
                            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            Logging.LogLoginBeginMethod(_logger, methodNamespace, LoginIdText);

                            try
                            {
                                if (user != null)
                                {
                                    Logging.LogLoginMethodParameter(_logger, methodNamespace, user, 1, LoginIdText);

                                    #region GetRolesForUser Service Call
                                    _manageLogins.GetRolesForUser(user.UserName, (userRoles) =>
                                    {
                                        string userRolesMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                        Logging.LogLoginBeginMethod(_logger, userRolesMethodNamespace, LoginIdText);

                                        try
                                        {
                                            if (userRoles != null)
                                            {
                                                Logging.LogLoginMethodParameter(_logger, userRolesMethodNamespace, userRoles, 1, _loginIdText);
                                                UserRoles = userRoles;

                                                #region ValidateUser Service Call
                                                _manageLogins.ValidateUser(LoginIdText, PasswordText, (result) =>
                                                {
                                                    string userValidationmethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                                    Logging.LogLoginBeginMethod(_logger, userValidationmethodNamespace, LoginIdText);

                                                    try
                                                    {
                                                        if (result != null)
                                                        {
                                                            Logging.LogLoginMethodParameter(_logger, userValidationmethodNamespace, result, 1, _loginIdText);
                                                            if ((bool)result)
                                                            {
                                                                LoadModule();
                                                            }
                                                            else
                                                            {
                                                                if (!user.IsApproved)
                                                                {
                                                                    Logging.LogAccountInactiveLoginAttempt(_logger, LoginIdText);
                                                                    LoginIdState = FieldState.LoginUnapproved;
                                                                    PasswordState = FieldState.LoginUnapproved;
                                                                }
                                                                else if (user.IsLockedOut)
                                                                {
                                                                    Logging.LogAccountLockedLoginAttempt(_logger, LoginIdText);
                                                                    LoginIdState = FieldState.LoginLocked;
                                                                    PasswordState = FieldState.LoginLocked;
                                                                }
                                                                else
                                                                {
                                                                    Logging.LogAccountInvalidLoginAttempt(_logger, LoginIdText);
                                                                    LoginIdState = FieldState.InvalidField;
                                                                    PasswordState = FieldState.InvalidField;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Logging.LogLoginMethodParameterNull(_logger, userValidationmethodNamespace, 1, LoginIdText);
                                                            LoginIdState = FieldState.InvalidField;
                                                            PasswordState = FieldState.InvalidField;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                                        Logging.LogLoginException(_logger, ex);
                                                    }

                                                    Logging.LogLoginEndMethod(_logger, userValidationmethodNamespace, LoginIdText);
                                                });
                                                #endregion
                                            }
                                            else
                                            {
                                                Logging.LogLoginMethodParameterNull(_logger, userRolesMethodNamespace, 1, LoginIdText);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                            Logging.LogLoginException(_logger, ex);
                                        }
                                        Logging.LogLoginEndMethod(_logger, userRolesMethodNamespace, LoginIdText);
                                    });
                                    #endregion
                                }
                                else
                                {
                                    Logging.LogLoginMethodParameterNull(_logger, methodNamespace, 1, LoginIdText);
                                    LoginIdState = FieldState.InvalidField;
                                    PasswordState = FieldState.InvalidField;
                                }
                            }
                            catch (Exception ex)
                            {
                                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                Logging.LogLoginException(_logger, ex);
                            }
                            Logging.LogLoginEndMethod(_logger, methodNamespace, LoginIdText);
                        });
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }

            credentialValidation = LoginIdState == FieldState.ValidField && PasswordState == FieldState.ValidField;
            return credentialValidation;
        }

        /// <summary>
        /// Validate Missing Fields
        /// </summary>
        /// <returns>True/False</returns>
        private bool MissingValidations()
        {
            try
            {
                #region Validation Conditions
                bool passwordValidation = PasswordText != string.Empty;
                bool loginValidation = LoginIdText != string.Empty;
                bool missingValidation = passwordValidation && loginValidation;
                #endregion

                #region Field State Updation
                PasswordState = passwordValidation ? FieldState.ValidField : FieldState.MissingField;
                LoginIdState = loginValidation ? FieldState.ValidField : FieldState.MissingField;
                MissingFieldPopupIsOpen = !(missingValidation);
                #endregion

                return missingValidation;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
                return false;
            }
        }
        #endregion

        #region Load Method
        /// <summary>
        /// Navigation to Main Application
        /// </summary>
        private void LoadModule()
        {
            try
            {
                //Session data posted to server creation
                Session sessionVariable = new Session
                    {
                        UserName = LoginIdText.ToLower(),
                        Roles = UserRoles
                    };

                _manageSessions.SetSession(sessionVariable, (result) =>
                {
                    string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    Logging.LogLoginBeginMethod(_logger, methodNamespace, LoginIdText.ToLower());

                    try
                    {
                        if (result != null)
                        {
                            String cookie = String.Format("{0}={1}", CookieEncription.Encript("UserName"), CookieEncription.Encript(LoginIdText.ToLower()));
                            List<String> roles = UserRoles.Select(g => CookieEncription.Encript(g)).ToList();
                            cookie = UserRoles.Count == 0 ? cookie : String.Format("{0},{1}={2}", cookie, CookieEncription.Encript("Roles"), String.Join("|", roles));
                            HtmlPage.Document.Cookies = cookie;                            
                            //HtmlPage.Document.SetProperty("cookie", "UserName=" + LoginIdText.ToLower());
                            if ((bool)result) HtmlPage.Window.Navigate(new Uri(@"HomePage.aspx", UriKind.Relative));
                        }
                        else
                        {
                            Logging.LogLoginMethodParameterNull(_logger, methodNamespace, 1, LoginIdText.ToLower());
                        }
                    }
                    catch (Exception ex)
                    {
                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogLoginException(_logger, ex);
                    }

                    Logging.LogLoginEndMethod(_logger, methodNamespace, LoginIdText.ToLower());
                });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Remove all Error notifications
        /// </summary>
        private void SwitchOffPopups()
        {
            MissingFieldPopupIsOpen = false;
            InvalidCredentialsPopupIsOpen = false;
            UnapprovedLoginPopupIsOpen = false;
            LockedOutLoginPopupIsOpen = false;
        }

        /// <summary>
        /// Default seting parameters
        /// </summary>
        private void DefaultSettings()
        {
            LoginIdText = String.Empty;
            LoginIdState = FieldState.ValidField;
            PasswordText = String.Empty;
            PasswordState = FieldState.ValidField;
        }
        #endregion
    }
}
