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
    /// View model class for ViewPasswordResetForm
    /// </summary>
    [Export]
    public class ViewModelPasswordResetForm : NotificationObject, INavigationAware
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
        public ViewModelPasswordResetForm(IManageLogins manageLogins, IRegionManager regionManager, ILoggerFacade logger)
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
                //field validation state to valid on every set operation
                LoginIdState = FieldState.ValidField;
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
                SecurityQuestionState = FieldState.ValidField;
                //field validation state to valid on every set operation
                RaisePropertyChanged(() => this.SecurityQuestionText);
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
                SecurityAnswerState = FieldState.ValidField;
                //field validation state to valid on every set operation
                RaisePropertyChanged(() => this.SecurityAnswerText);
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
                InvalidSecurityAnswerPopupIsOpen = value == FieldState.InvalidField;
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
        /// Property binding to InvalidSecurityAnswerPopup IsOpen attribute
        /// </summary>
        private bool _invalidSecurityAnswerPopupIsOpen = false;
        public bool InvalidSecurityAnswerPopupIsOpen
        {
            get { return _invalidSecurityAnswerPopupIsOpen; }
            set
            {
                if (_invalidSecurityAnswerPopupIsOpen != value)
                {
                    if (value == true)
                        SwitchOffPopups();
                    _invalidSecurityAnswerPopupIsOpen = value;
                    RaisePropertyChanged(() => this.InvalidSecurityAnswerPopupIsOpen);
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
                    _invalidLoginIdPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidPasswordResetLoginIdError"));
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
        /// Property binding to InvalidSecurityAnswerPopup Child attribute
        /// </summary>
        private UIElement _invalidSecurityAnswerPopupChild;
        public UIElement InvalidSecurityAnswerPopupChild
        {
            get
            {
                if (_invalidSecurityAnswerPopupChild == null)
                    _invalidSecurityAnswerPopupChild = new ErrorMessage(ErrorResourceManager.GetString("InvalidPasswordResetSecurityAnswerError"));
                return _invalidSecurityAnswerPopupChild;
            }
            set
            {
                if (_invalidSecurityAnswerPopupChild != value)
                {
                    _invalidSecurityAnswerPopupChild = value;
                    RaisePropertyChanged(() => this.InvalidSecurityAnswerPopupChild);
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
        /// Property binding to Reset button click event or Login Id/SecurityAnswer Enter Press event
        /// </summary>        
        public ICommand ResetCommand
        {
            get { return new DelegateCommand<object>(ResetCommandMethod); }
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
        /// Reset button click event or Login Id/SecurityAnswer Enter Press event implementation - validate errors, navigate to notification screen
        /// </summary>
        /// <param name="param"></param>
        private void ResetCommandMethod(object param)
        {
            try
            {
                if (_manageLogins != null)
                {
                    if (ErrorValidation())
                    {
                        #region GetUser Service Call
                        _manageLogins.GetUser(LoginIdText, false, (membershipUser) =>
                        {
                            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            Logging.LogLoginBeginMethod(_logger, methodNamespace, LoginIdText);

                            try
                            {
                                if (membershipUser != null)
                                {
                                    Logging.LogLoginMethodParameter(_logger, methodNamespace, membershipUser, 1, _loginIdText);

                                    #region ResetPassword Service Call
                                    _manageLogins.ResetPassword(LoginIdText, SecurityAnswerText, (password) =>
                                    {
                                        string resetMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                        Logging.LogLoginBeginMethod(_logger, resetMethodNamespace, LoginIdText);

                                        try
                                        {
                                            if (password != null)
                                            {
                                                Logging.LogLoginMethodParameter(_logger, resetMethodNamespace, password, 1, _loginIdText);
                                                Logging.LogAccountPasswordReset(_logger, LoginIdText);
                                                MessageBox.Show(password); // Password displayed as messagebox, to be sent as email alert later
                                                ResourceManager NotificationManager = new ResourceManager(typeof(Notifications));
                                                NotificationText = NotificationManager.GetString("PasswordResetNotification").Replace("[LoginId]", LoginIdText);

                                                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewNotifications", UriKind.Relative));
                                            }
                                            else
                                            {
                                                Logging.LogLoginMethodParameterNull(_logger, resetMethodNamespace, 1, LoginIdText);
                                                SecurityAnswerState = FieldState.InvalidField;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                            Logging.LogLoginException(_logger, ex);
                                        }

                                        Logging.LogLoginEndMethod(_logger, resetMethodNamespace, LoginIdText);
                                    });
                                    #endregion
                                }
                                else
                                {
                                    Logging.LogLoginMethodParameterNull(_logger, methodNamespace, 1, LoginIdText);
                                    LoginIdState = FieldState.InvalidField;
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
            bool SecurityQuestionValidation = SecurityQuestionText != null;
            bool SecurityAnswerValidation = SecurityAnswerText != string.Empty;
            bool missingFieldValidation = LoginValidation && SecurityQuestionValidation && SecurityAnswerValidation;

            #endregion

            #region Field State Updation

            MissingFieldPopupIsOpen = false;

            LoginIdState = LoginValidation ? FieldState.ValidField : FieldState.MissingField;
            SecurityQuestionState = SecurityQuestionValidation ? FieldState.ValidField : FieldState.MissingField;
            SecurityAnswerState = SecurityAnswerValidation ? FieldState.ValidField : FieldState.MissingField;

            MissingFieldPopupIsOpen = missingFieldValidation ? false : true;

            #endregion

            return missingFieldValidation;
        }

        /// <summary>
        /// Validate field states
        /// </summary>
        /// <returns>True/False</returns>
        private bool ErrorValidation()
        {
            bool missingFieldValidation = MissingFieldValidation();
            bool errorValidation = (LoginIdState == FieldState.ValidField) &&
                                   (SecurityQuestionState == FieldState.ValidField) &&
                                   (SecurityAnswerState == FieldState.ValidField);
            return missingFieldValidation && errorValidation;
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
            InvalidSecurityAnswerPopupIsOpen = false;
        }

        /// <summary>
        /// Default seting parameters
        /// </summary>
        private void DefaultSettings()
        {
            LoginIdText = String.Empty;
            LoginIdState = FieldState.ValidField;
            SecurityQuestionText = null;
            SecurityQuestionState = FieldState.ValidField;
            SecurityAnswerText = String.Empty;
            SecurityAnswerState = FieldState.ValidField;
        }
        #endregion
    }
}
