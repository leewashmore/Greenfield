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
using System.Windows.Media;
using Microsoft.Practices.Prism.Modularity;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Resources;
using System.Globalization;
using System.Reflection;
using System.Collections;
using GreenField.LoginModule.Resources;
using GreenField.ServiceCaller;
using GreenField.Common;
using GreenField.LoginModule.Controls;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.LoginModule.ViewModel
{
    /// <summary>
    /// View model class for ViewRegisterForm
    /// </summary>
    [Export]
    public class ViewModelRegisterForm : NotificationObject, INavigationAware
    {
        #region Fields
        /// <summary>
        /// Visualizations for field validation states
        /// </summary>
        private static Brush VALId_BRUSH = new SolidColorBrush(Colors.Black);
        private static Brush INVALId_BRUSH = new SolidColorBrush(Colors.Red);

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
        public ViewModelRegisterForm(IManageLogins manageLogins, IRegionManager regionManager, ILoggerFacade logger)
        {
            _manageLogins = manageLogins;
            _regionManager = regionManager;
            _logger = logger;
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// Property binding Login Id TextBlock Text property
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
                //field validation on every set operation
                LoginValidation();
            }
        }

        /// <summary>
        /// Property storing Login Id field validation state
        /// </summary>
        private FieldState _loginIdState = FieldState.ValidField;
        public FieldState LoginIdState
        {
            get { return _loginIdState; }
            set
            {
                _loginIdState = value;
                //Visualization changes based on field validation state
                LoginBorderBrush = value != FieldState.ValidField ? INVALId_BRUSH : VALId_BRUSH;
                InvalidLoginIdPopupIsOpen = value == FieldState.InvalidField ? true : false;
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
                //field validation on every set operation
                PasswordValidation();
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
                PasswordBorderBrush = value != FieldState.ValidField ? INVALId_BRUSH : VALId_BRUSH;
                InvalidPasswordPopupIsOpen = value == FieldState.InvalidField ? true : false;
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
                //field validation on every set operation
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
                ConfPasswordBorderBrush = value != FieldState.ValidField ? INVALId_BRUSH : VALId_BRUSH;
                InvalidConfPasswordPopupIsOpen = value == FieldState.InvalidField ? true : false;
            }
        }

        /// <summary>
        /// Property binding Email PasswordBlock Text property
        /// </summary>
        private string _emailText = string.Empty;
        public string EmailText
        {
            get { return _emailText; }
            set
            {
                if (_emailText != value)
                    _emailText = value;
                RaisePropertyChanged(() => this.EmailText);
                //field validation on every set operation
                EmailValidation();
            }
        }

        /// <summary>
        /// Property storing Email field validation state
        /// </summary>
        private FieldState _emailState = FieldState.ValidField;
        public FieldState EmailState
        {
            get { return _emailState; }
            set
            {
                _emailState = value;
                //Visualization changes based on field validation state
                EmailBorderBrush = value != FieldState.ValidField ? INVALId_BRUSH : VALId_BRUSH;
                InvalidEmailPopupIsOpen = value == FieldState.InvalidField ? true : false;
            }
        }

        /// <summary>
        /// Property binding SecurityQuestion TextBlock Text property
        /// </summary>
        private string _securityQuestionText = null;
        public string SecurityQuestionText
        {
            get { return _securityQuestionText; }
            set
            {
                if (_securityQuestionText != value)
                    _securityQuestionText = value;
                RaisePropertyChanged(() => this.SecurityQuestionText);
                //field validation state to valid on every set operation
                SecurityQuestionState = FieldState.ValidField;                
            }
        }

        /// <summary>
        /// Property storing SecurityQuestion field validation state
        /// </summary>
        private FieldState _securityQuestionState = FieldState.ValidField;
        public FieldState SecurityQuestionState
        {
            get { return _securityQuestionState; }
            set
            {
                _securityQuestionState = value;
                //Visualization changes based on field validation state
                SecurityQuestionBorderBrush = value != FieldState.ValidField ? INVALId_BRUSH : VALId_BRUSH;
            }
        }

        /// <summary>
        /// Property binding SecurityAnswer TextBlock Text property
        /// </summary>
        private string _securityAnswerText = string.Empty;
        public string SecurityAnswerText
        {
            get { return _securityAnswerText; }
            set
            {
                if (_securityAnswerText != value)
                    _securityAnswerText = value;
                RaisePropertyChanged(() => this.SecurityAnswerText);
                //field validation state to valid on every set operation
                SecurityAnswerState = FieldState.ValidField;                
            }
        }

        /// <summary>
        /// Property storing SecurityAnswer field validation state
        /// </summary>
        private FieldState _securityAnswerState = FieldState.ValidField;
        public FieldState SecurityAnswerState
        {
            get { return _securityAnswerState; }
            set
            {
                _securityAnswerState = value;
                //Visualization changes based on field validation state
                SecurityAnswerBorderBrush = value != FieldState.ValidField ? INVALId_BRUSH : VALId_BRUSH;
            }
        }

        /// <summary>
        /// Property storing Security Question Data from resouce file
        /// </summary>
        private ObservableCollection<string> _securityQuestionsList;
        public ObservableCollection<string> SecurityQuestionsList
        {
            get
            {
                if (_securityQuestionsList == null)
                {
                    _securityQuestionsList = new ObservableCollection<string>();
                    ResourceSet rs = SecurityQuestions.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

                    IDictionaryEnumerator enumerator = rs.GetEnumerator();
                    while (enumerator.MoveNext())
                        _securityQuestionsList.Add(enumerator.Value.ToString());
                    //rs.Dispose();
                }
                return _securityQuestionsList;
            }
        }
        #endregion

        #region Visualization
        #region UI Field Brush
        /// <summary>
        /// Property binding to the Login Id Textbox BorderBrush and Login Id TextBlock Foreground attributes
        /// </summary>
        private Brush _loginBorderBrush = VALId_BRUSH;
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
        private Brush _passwordBorderBrush = VALId_BRUSH;
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
        /// Property binding to the ConfirmPassword PasswordBox BorderBrush and ConfirmPassword TextBlock Foreground attributes
        /// </summary>
        private Brush _confPasswordBorderBrush = VALId_BRUSH;
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

        /// <summary>
        /// Property binding to the Email Textbox BorderBrush and Email TextBlock Foreground attributes
        /// </summary>
        private Brush _emailBorderBrush = VALId_BRUSH;
        public Brush EmailBorderBrush
        {
            get { return _emailBorderBrush; }
            set
            {
                if (_emailBorderBrush != value)
                {
                    _emailBorderBrush = value;
                    RaisePropertyChanged(() => this.EmailBorderBrush);
                }
            }
        }

        /// <summary>
        /// Property binding to the SecurityQuestion ComboBox BorderBrush and SecurityQuestion TextBlock Foreground attributes
        /// </summary>
        private Brush _securityQuestionBorderBrush = VALId_BRUSH;
        public Brush SecurityQuestionBorderBrush
        {
            get { return _securityQuestionBorderBrush; }
            set
            {
                if (_securityQuestionBorderBrush != value)
                {
                    _securityQuestionBorderBrush = value;
                    RaisePropertyChanged(() => this.SecurityQuestionBorderBrush);
                }
            }
        }

        /// <summary>
        /// Property binding to the SecurityAnswer Textbox BorderBrush and SecurityAnswer TextBlock Foreground attributes
        /// </summary>
        private Brush _securityAnswerBorderBrush = VALId_BRUSH;
        public Brush SecurityAnswerBorderBrush
        {
            get { return _securityAnswerBorderBrush; }
            set
            {
                if (_securityAnswerBorderBrush != value)
                {
                    _securityAnswerBorderBrush = value;
                    RaisePropertyChanged(() => this.SecurityAnswerBorderBrush);
                }
            }
        }
        #endregion

        #region UI Field Invalid Error Popup
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
        /// Property binding to InvalidLoginIdPopup IsOpen attribute
        /// </summary>
        private bool _invalidLoginIdPopupIsOpen = false;
        public bool InvalidLoginIdPopupIsOpen
        {
            get { return _invalidLoginIdPopupIsOpen; }
            set
            {
                if (_invalidLoginIdPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _invalidLoginIdPopupIsOpen = value;
                    RaisePropertyChanged(() => this.InvalidLoginIdPopupIsOpen);
                }
            }
        }

        /// <summary>
        /// Property binding to InvalidPasswordPopup IsOpen attribute
        /// </summary>
        private bool _invalidPasswordPopupIsOpen = false;
        public bool InvalidPasswordPopupIsOpen
        {
            get { return _invalidPasswordPopupIsOpen; }
            set
            {
                if (_invalidPasswordPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _invalidPasswordPopupIsOpen = value;
                    RaisePropertyChanged(() => this.InvalidPasswordPopupIsOpen);
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
        /// Property binding to InvalidEmailPopup IsOpen attribute
        /// </summary>
        private bool _invalidEmailPopupIsOpen = false;
        public bool InvalidEmailPopupIsOpen
        {
            get { return _invalidEmailPopupIsOpen; }
            set
            {
                if (_invalidEmailPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _invalidEmailPopupIsOpen = value;
                    RaisePropertyChanged(() => this.InvalidEmailPopupIsOpen);
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
        /// Property binding to MissingFieldPopup Child attribute
        /// </summary>
        private UIElement _missingFieldPopupChild;
        public UIElement MissingFieldPopupChild
        {
            get 
            {
                if (_missingFieldPopupChild == null)
                    _missingFieldPopupChild = new ErrorMessage(ErrorResourceManager.GetString("MissingRegistrationFieldsError"));
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
        /// Property binding to InvalidLoginIdPopup Child attribute
        /// </summary>
        private UIElement _invalidLoginIdPopupChild;
        public UIElement InvalidLoginIdPopupChild
        {
            get
            {
                if (_invalidLoginIdPopupChild == null)
                    _invalidLoginIdPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidLoginIdError"));
                return _invalidLoginIdPopupChild;
            }
            set
            {
                if (_invalidLoginIdPopupChild != value)
                {
                    _invalidLoginIdPopupChild = value;
                    RaisePropertyChanged(() => this.InvalidLoginIdPopupChild);
                }
            }
        }

        /// <summary>
        /// Property binding to InvalidPasswordPopup Child attribute
        /// </summary>
        private UIElement _invalidPasswordPopupChild;
        public UIElement InvalidPasswordPopupChild
        {
            get
            {
                if (_invalidPasswordPopupChild == null)
                    _invalidPasswordPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidPasswordError"));
                return _invalidPasswordPopupChild;
            }
            set
            {
                if (_invalidPasswordPopupChild != value)
                {
                    _invalidPasswordPopupChild = value;
                    RaisePropertyChanged(() => this.InvalidPasswordPopupChild);
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
        /// Property binding to InvalidEmailPopup Child attribute
        /// </summary>
        private UIElement _invalidEmailPopupChild;
        public UIElement InvalidEmailPopupChild
        {
            get
            {
                if (_invalidEmailPopupChild == null)
                    _invalidEmailPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidEmailError"));
                return _invalidEmailPopupChild;
            }
            set
            {
                if (_invalidEmailPopupChild != value)
                {
                    _invalidEmailPopupChild = value;
                    RaisePropertyChanged(() => this.InvalidEmailPopupChild);
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
        /// Property binding to Register button click event or Login Id/Password/ConfPassword/Email/SecurityAnswer Enter Press event
        /// </summary>        
        public ICommand RegisterCommand
        {
            get { return new DelegateCommand<object>(RegisterCommandMethod); }
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
        /// Register button click event or Login Id/Password/ConfPassword/Email/SecurityAnswer
        /// Enter Press event implementation - validate errors, navigate to notification screen
        /// </summary>
        /// <param name="param"></param>
        private void RegisterCommandMethod(object param)
        {
            try
            {
                if (_manageLogins != null)
                {
                    if (ErrorValidation())
                    {
                        #region CreateUser Service Call
                        _manageLogins.CreateUser(LoginIdText, PasswordText, EmailText, SecurityQuestionText, SecurityAnswerText, false, (status) =>
                        {
                            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            Logging.LogLoginBeginMethod(_logger, methodNamespace, LoginIdText);

                            try
                            {
                                if (status != null)
                                {
                                    if (status == MembershipCreateStatus.SUCCESS)
                                    {
                                        Logging.LogLoginMethodParameter(_logger, methodNamespace, status, 1, LoginIdText);
                                        Logging.LogAccountRegister(_logger, LoginIdText);

                                        ResourceManager NotificationManager = new ResourceManager(typeof(Notifications));
                                        NotificationText = NotificationManager.GetString("RegisterNotification").Replace("[LoginId]", LoginIdText);

                                        _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewNotifications", UriKind.Relative));
                                    }
                                    else
                                    {
                                        Logging.LogLoginMethodParameter(_logger, methodNamespace, status, 1, LoginIdText);
                                        LoginIdState = FieldState.InvalidField;
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
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
                _logger.Log("User : " + SessionManager.SESSION.UserName +"\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
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
            bool ConfirmPasswordValidation = ConfirmPasswordText != string.Empty;
            bool EmailValidation = EmailText != string.Empty;
            bool SecurityQuestionValidation = SecurityQuestionText != null;
            bool SecurityAnswerValidation = SecurityAnswerText != string.Empty;
            bool missingFieldValidation = PasswordValidation && LoginValidation && ConfirmPasswordValidation
                && EmailValidation && SecurityQuestionValidation && SecurityAnswerValidation;

            #endregion

            #region Field State Updation

            MissingFieldPopupIsOpen = false;

            LoginIdState = LoginValidation ? FieldState.ValidField : FieldState.MissingField;
            PasswordState = PasswordValidation ? FieldState.ValidField : FieldState.MissingField;
            ConfirmPasswordState = ConfirmPasswordValidation ? FieldState.ValidField : FieldState.MissingField;
            EmailState = EmailValidation ? FieldState.ValidField : FieldState.MissingField;
            SecurityQuestionState = SecurityQuestionValidation ? FieldState.ValidField : FieldState.MissingField;
            SecurityAnswerState = SecurityAnswerValidation ? FieldState.ValidField : FieldState.MissingField;

            MissingFieldPopupIsOpen = missingFieldValidation ? false : true;

            #endregion

            return missingFieldValidation;
        }

        /// <summary>
        /// Validate Login Id Field
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

                bool confirmPasswordValidation = PasswordText == ConfirmPasswordText;

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
        /// Validate Password Field
        /// </summary>
        /// <returns>True/False</returns>
        private bool PasswordValidation()
        {
            if (PasswordText != string.Empty)
            {
                #region Validation Conditions

                bool passwordValidation = PasswordText.Length >= 7 && PasswordText.Length <= 50 && !(PasswordText.Contains(" "));

                #endregion

                #region Field State Updation

                PasswordState = passwordValidation ? FieldState.ValidField : FieldState.InvalidField;

                #endregion

                return passwordValidation;
            }

            PasswordState = FieldState.ValidField;
            return true;
        }

        /// <summary>
        /// Validate Email Field
        /// </summary>
        /// <returns>True/False</returns>
        private bool EmailValidation()
        {
            if (EmailText != string.Empty)
            {
                #region Validation Conditions

                string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" + @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]"
                            + @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
                Match match = Regex.Match(EmailText, pattern, RegexOptions.IgnoreCase);
                bool emailValidation = match.Success;

                #endregion

                #region Field State Updation

                EmailState = emailValidation ? FieldState.ValidField : FieldState.InvalidField;

                #endregion

                return emailValidation;
            }

            EmailState = FieldState.ValidField;
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
            PasswordValidation();
            ConfirmationPasswordValidation();
            EmailValidation();
            bool errorValidation = (LoginIdState == FieldState.ValidField) &&
                                   (PasswordState == FieldState.ValidField) &&
                                   (ConfirmPasswordState == FieldState.ValidField) &&
                                   (EmailState == FieldState.ValidField) &&
                                   (SecurityQuestionState == FieldState.ValidField) &&
                                   (SecurityAnswerState == FieldState.ValidField);
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
            InvalidLoginIdPopupIsOpen = false;
            InvalidPasswordPopupIsOpen = false;
            InvalidConfPasswordPopupIsOpen = false;
            InvalidEmailPopupIsOpen = false;
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
            ConfirmPasswordText = String.Empty;
            ConfirmPasswordState = FieldState.ValidField;
            EmailText = String.Empty;
            EmailState = FieldState.ValidField;
            SecurityQuestionText = null;
            SecurityQuestionState = FieldState.ValidField;
            SecurityAnswerText = String.Empty;
            SecurityAnswerState = FieldState.ValidField;
        }
        #endregion
    }
}
