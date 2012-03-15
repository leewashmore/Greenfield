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
using System.Text.RegularExpressions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using System.Resources;
using GreenField.LoginModule.Resources;
using System.Globalization;
using System.Collections;
using GreenField.LoginModule.Controls;
using Microsoft.Practices.Prism.Commands;
using GreenField.Common;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.LoginModule.ViewModel
{
    /// <summary>
    /// View model class for ViewPasswordChangeForm
    /// </summary>
    [Export]
    public class ViewModelPasswordChangeForm : NotificationObject, INavigationAware
    {
        #region Fields
        /// <summary>
        /// Visualizations for field validation states
        /// </summary>
        private static Brush VALID_BRUSH = new SolidColorBrush(Colors.Black);
        private static Brush INVALID_BRUSH = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Enumeration for field validation states
        /// </summary>
        public enum FieldState { MissingField, InvalidField, ValidField };

        /// <summary>
        /// MEF singletons
        /// </summary>
        private IManageLogins _manageLogins;
        private IRegionManager _regionManager;
        private ILoggerFacade _logger;
        #endregion

        #region Contructor
        [ImportingConstructor]
        public ViewModelPasswordChangeForm(IManageLogins manageLogins, IRegionManager regionManager, ILoggerFacade logger)
        {
            _manageLogins = manageLogins;
            _regionManager = regionManager;
            _logger = logger;
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
                InvalidCredentialsPopupIsOpen = value == FieldState.InvalidField ? true : false;
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
                InvalidCredentialsPopupIsOpen = value == FieldState.InvalidField ? true : false;
            }
        }

        /// <summary>
        /// Property binding NewPassword PasswordBlock Text property
        /// </summary>
        private string _newPasswordText = string.Empty;
        public string NewPasswordText
        {
            get { return _newPasswordText; }
            set
            {
                if (_newPasswordText != value)
                    _newPasswordText = value;
                RaisePropertyChanged(() => this.NewPasswordText);
                //field validation state validation on every set operation
                NewPasswordValidation();
            }
        }

        /// <summary>
        /// Property storing NewPassword field validation state
        /// </summary>
        private FieldState _newPasswordState = FieldState.ValidField;
        public FieldState NewPasswordState
        {
            get { return _newPasswordState; }
            set
            {
                _newPasswordState = value;
                //Visualization changes based on field validation state
                NewPasswordBorderBrush = value != FieldState.ValidField ? INVALID_BRUSH : VALID_BRUSH;
                InvalidNewPasswordPopupIsOpen = value == FieldState.InvalidField ? true : false;
            }
        }

        /// <summary>
        /// Property binding ConfirmPassword PasswordBlock Text property
        /// </summary>
        private string _confirmPasswordText = string.Empty;
        public string ConfirmPasswordText
        {
            get { return _confirmPasswordText; }
            set
            {
                if (_confirmPasswordText != value)
                    _confirmPasswordText = value;
                RaisePropertyChanged(() => this.ConfirmPasswordText);
                //field validation state validation on every set operation
                ConfirmationPasswordValidation();
            }
        }

        /// <summary>
        /// Property storing ConfirmPassword field validation state
        /// </summary>
        private FieldState _confirmPasswordState = FieldState.ValidField;
        public FieldState ConfirmPasswordState
        {
            get { return _confirmPasswordState; }
            set
            {
                _confirmPasswordState = value;
                //Visualization changes based on field validation state
                ConfPasswordBorderBrush = value != FieldState.ValidField ? INVALID_BRUSH : VALID_BRUSH;
                InvalidConfPasswordPopupIsOpen = value == FieldState.InvalidField ? true : false;
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

        /// <summary>
        /// Property binding to the NewPassword PasswordBox BorderBrush and NewPassword TextBlock Foreground attributes
        /// </summary>
        private Brush _newPasswordBorderBrush = VALID_BRUSH;
        public Brush NewPasswordBorderBrush
        {
            get { return _newPasswordBorderBrush; }
            set
            {
                if (_newPasswordBorderBrush != value)
                {
                    _newPasswordBorderBrush = value;
                    RaisePropertyChanged(() => this.NewPasswordBorderBrush);
                }
            }
        }

        /// <summary>
        /// Property binding to the ConfirmPassword PasswordBox BorderBrush and ConfirmPassword TextBlock Foreground attributes
        /// </summary>
        private Brush _confPasswordBorderBrush = VALID_BRUSH;
        public Brush ConfPasswordBorderBrush
        {
            get { return _confPasswordBorderBrush; }
            set
            {
                if (_confPasswordBorderBrush != value)
                {
                    _confPasswordBorderBrush = value;
                    RaisePropertyChanged(() => this.ConfPasswordBorderBrush);
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
        /// Property binding to InvalidNewPasswordPopup IsOpen attribute
        /// </summary>
        private bool _invalidNewPasswordPopupIsOpen = false;
        public bool InvalidNewPasswordPopupIsOpen
        {
            get { return _invalidNewPasswordPopupIsOpen; }
            set
            {
                if (_invalidNewPasswordPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _invalidNewPasswordPopupIsOpen = value;
                    RaisePropertyChanged(() => this.InvalidNewPasswordPopupIsOpen);
                }
            }
        }

        /// <summary>
        /// Property binding to InvalidConfPasswordPopup IsOpen attribute
        /// </summary>
        private bool _invalidConfPasswordPopupIsOpen = false;
        public bool InvalidConfPasswordPopupIsOpen
        {
            get { return _invalidConfPasswordPopupIsOpen; }
            set
            {
                if (_invalidConfPasswordPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _invalidConfPasswordPopupIsOpen = value;
                    RaisePropertyChanged(() => this.InvalidConfPasswordPopupIsOpen);
                }
            }
        }

        /// <summary>
        /// Property binding to ResetErrorPopup IsOpen attribute
        /// </summary>
        private bool _resetErrorPopupIsOpen = false;
        public bool ResetErrorPopupIsOpen
        {
            get { return _resetErrorPopupIsOpen; }
            set
            {
                if (_resetErrorPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _resetErrorPopupIsOpen = value;
                    RaisePropertyChanged(() => this.ResetErrorPopupIsOpen);
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
        /// Property binding to MissingFieldPopup Child attribute
        /// </summary>
        private UIElement _missingFieldPopupChild;
        public UIElement MissingFieldPopupChild
        {
            get 
            {
                if (_missingFieldPopupChild == null)
                    _missingFieldPopupChild = new ErrorMessage(ErrorResourceManager.GetString("MissingPasswordChangeFieldsError"));
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
        /// Property binding to InvalidNewPasswordPopup Child attribute
        /// </summary>
        private UIElement _invalidNewPasswordPopupChild;
        public UIElement InvalidNewPasswordPopupChild
        {
            get
            {
                if (_invalidNewPasswordPopupChild == null)
                    _invalidNewPasswordPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidPasswordError"));
                return _invalidNewPasswordPopupChild;
            }
            set
            {
                if (_invalidNewPasswordPopupChild != value)
                {
                    _invalidNewPasswordPopupChild = value;
                    RaisePropertyChanged(() => this.InvalidNewPasswordPopupChild);
                }
            }
        }

        /// <summary>
        /// Property binding to InvalidConfPasswordPopup Child attribute
        /// </summary>
        private UIElement _invalidConfPasswordPopupChild;
        public UIElement InvalidConfPasswordPopupChild
        {
            get
            {
                if (_invalidConfPasswordPopupChild == null)
                    _invalidConfPasswordPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidConfPasswordError"));
                return _invalidConfPasswordPopupChild;
            }
            set
            {
                if (_invalidConfPasswordPopupChild != value)
                {
                    _invalidConfPasswordPopupChild = value;
                    RaisePropertyChanged(() => this.InvalidConfPasswordPopupChild);
                }
            }
        }

        /// <summary>
        /// Property binding to ResetErrorPopup Child attribute
        /// </summary>
        private UIElement _resetErrorPopupChild;
        public UIElement ResetErrorPopupChild
        {
            get
            {
                if (_resetErrorPopupChild == null)
                    _resetErrorPopupChild = new ErrorMessage(ErrorResourceManager.GetString("PasswordResetError"));
                return _resetErrorPopupChild;
            }
            set
            {
                if (_resetErrorPopupChild != value)
                {
                    _resetErrorPopupChild = value;
                    RaisePropertyChanged(() => this.ResetErrorPopupChild);
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

        #region Notification
        /// <summary>
        /// Property to store notification text
        /// </summary>
        public string NotificationText { get; set; } 
        #endregion

        #region ICommands
        /// <summary>
        /// Property binding to Hyperlink 'Back to Login Screen' click event
        /// </summary>
        public ICommand CancelCommand
        {
            get { return new DelegateCommand<object>(CancelCommandMethod); }
        }

        /// <summary>
        /// Property binding to Update button click event or Login ID/Password/NewPassword/ConfPassword Enter Press event
        /// </summary>        
        public ICommand UpdateCommand
        {
            get { return new DelegateCommand<object>(UpdateCommandMethod); }
        }
        #endregion
        #endregion

        #region ICommand Methods
        /// <summary>
        /// Hyperlink 'Back to Login Screen' click event implementation - navigate to login screen
        /// </summary>
        /// <param name="param"></param>
        private void CancelCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewLoginForm", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        }

        /// <summary>
        /// Update button click event or Login ID/Password/NewPassword/ConfPassword Enter Press event implementation - validate errors, navigate to notification screen
        /// </summary>
        /// <param name="param"></param>
        private void UpdateCommandMethod(object param)
        {
            try
            {
                if (_manageLogins != null)
                {
                    if (ErrorValidation())
                    {
                        #region ValidateUser Service Call
                        _manageLogins.ValidateUser(LoginIdText, PasswordText, (credentialValidation) =>
                        {
                            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            Logging.LogLoginBeginMethod(_logger, methodNamespace, LoginIdText);
                            if (credentialValidation)
                            {
                                Logging.LogLoginMethodParameter(_logger, methodNamespace, credentialValidation, 1, _loginIdText);

                                #region ChangePassword Service Call
                                _manageLogins.ChangePassword(LoginIdText, PasswordText, NewPasswordText, (resetValidation) =>
                                {
                                    string changeMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                    Logging.LogLoginBeginMethod(_logger, changeMethodNamespace, LoginIdText);

                                    if (resetValidation)
                                    {
                                        Logging.LogLoginMethodParameter(_logger, changeMethodNamespace, resetValidation, 1, _loginIdText);
                                        Logging.LogAccountPasswordChange(_logger, LoginIdText);
                                        ResourceManager NotificationManager = new ResourceManager(typeof(Notifications));
                                        NotificationText = NotificationManager.GetString("PasswordChangeNotification").Replace("[LoginID]", LoginIdText);
                                        _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewNotifications", UriKind.Relative));
                                    }
                                    else
                                    {
                                        Logging.LogLoginMethodParameterNull(_logger, changeMethodNamespace, 1, LoginIdText);
                                        ResetErrorPopupIsOpen = true;
                                    }
                                    Logging.LogLoginEndMethod(_logger, changeMethodNamespace, LoginIdText);
                                });
                                #endregion
                            }
                            else
                            {
                                Logging.LogLoginMethodParameterNull(_logger, methodNamespace, 1, LoginIdText);
                                LoginIdState = FieldState.InvalidField;
                                PasswordState = FieldState.InvalidField;
                            }
                            Logging.LogLoginEndMethod(_logger, methodNamespace, LoginIdText);
                        });
                        #endregion
                    } 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
        /// NavigateTo Property set to false, Assign NotificationText value to navigationContext
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            try
            {
                navigationContext.NavigationService.Region.Context = NotificationText;
                NavigateTo = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
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
        /// Validate Missing Fields
        /// </summary>
        /// <returns>True/False</returns>
        private bool MissingFieldValidation()
        {
            #region Validation Conditions

            bool LoginValidation = LoginIdText != string.Empty;
            bool PasswordValidation = PasswordText != string.Empty;
            bool NewPasswordValidation = NewPasswordText != string.Empty;
            bool ConfirmPasswordValidation = ConfirmPasswordText != string.Empty;
            bool missingFieldValidation = LoginValidation && PasswordValidation && NewPasswordValidation && ConfirmPasswordValidation;

            #endregion

            #region Field State Updation

            MissingFieldPopupIsOpen = false;

            LoginIdState = LoginValidation ? FieldState.ValidField : FieldState.MissingField;
            PasswordState = PasswordValidation ? FieldState.ValidField : FieldState.MissingField;
            NewPasswordState = NewPasswordValidation ? FieldState.ValidField : FieldState.MissingField;
            ConfirmPasswordState = ConfirmPasswordValidation ? FieldState.ValidField : FieldState.MissingField;
            
            MissingFieldPopupIsOpen = missingFieldValidation ? false : true;

            #endregion

            return missingFieldValidation;
        }

        /// <summary>
        /// Validate Login ID Field
        /// </summary>
        /// <returns>True/False</returns>
        private bool LoginValidation()
        {
            if (LoginIdText != string.Empty)
            {
                #region Validation Conditions

                string pattern = "^[a-zA-Z0-9]+$";
                Match match = Regex.Match(LoginIdText, pattern);
                bool loginValidation = match.Success;

                #endregion

                #region Field State Updation

                LoginIdState = loginValidation ? FieldState.ValidField : FieldState.InvalidField;

                #endregion

                return loginValidation;
            }

            LoginIdState = FieldState.ValidField;
            return true;
        }

        /// <summary>
        /// Validate ConfirmPassword Field
        /// </summary>
        /// <returns>True/False</returns>
        private bool ConfirmationPasswordValidation()
        {
            if (ConfirmPasswordText != string.Empty)
            {

                #region Validation Conditions

                bool confirmPasswordValidation = NewPasswordText == ConfirmPasswordText;

                #endregion

                #region Field State Updation

                ConfirmPasswordState = confirmPasswordValidation ? FieldState.ValidField : FieldState.InvalidField;

                #endregion

                return confirmPasswordValidation;
            }

            ConfirmPasswordState = FieldState.ValidField;
            return true;
        }

        /// <summary>
        /// Validate NewPassword Field
        /// </summary>
        /// <returns>True/False</returns>
        private bool NewPasswordValidation()
        {
            if (NewPasswordText != string.Empty)
            {
                #region Validation Conditions

                bool newPasswordValidation = NewPasswordText.Length >= 7 && NewPasswordText.Length <= 50 && !(NewPasswordText.Contains(" "));

                #endregion

                #region Field State Updation

                NewPasswordState = newPasswordValidation ? FieldState.ValidField : FieldState.InvalidField;

                #endregion

                return newPasswordValidation;
            }

            NewPasswordState = FieldState.ValidField;
            return true;
        }

        /// <summary>
        /// Validate field states
        /// </summary>
        /// <returns>True/False</returns>
        private bool ErrorValidation()
        {
            MissingFieldValidation();
            LoginValidation();
            NewPasswordValidation();
            ConfirmationPasswordValidation();            

            bool errorValidation = (LoginIdState == FieldState.ValidField) &&
                                   (PasswordState == FieldState.ValidField) &&
                                   (NewPasswordState == FieldState.ValidField) &&
                                   (ConfirmPasswordState == FieldState.ValidField);
            return errorValidation;
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
            InvalidNewPasswordPopupIsOpen = false;
            InvalidConfPasswordPopupIsOpen = false;
            ResetErrorPopupIsOpen = false;
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
            NewPasswordText = String.Empty;
            NewPasswordState = FieldState.ValidField;
            ConfirmPasswordText = String.Empty;
            ConfirmPasswordState = FieldState.ValidField;
        }
        #endregion
    }
}
