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
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using GreenField.Common;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.LoginModule.ViewModel
{
    /// <summary>
    /// View model class for ViewNotifications
    /// </summary>
    [Export]
    public class ViewModelNotifications : NotificationObject, INavigationAware
    {
        #region Fields
        /// <summary>
        /// MEF singletons
        /// </summary>
        private IRegionManager _regionManager;
        private ILoggerFacade _logger;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ViewModelNotifications(IRegionManager regionManager, ILoggerFacade logger)
        {
            _regionManager = regionManager;
            _logger = logger;
        } 
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// Property binding to Notification TextBlock Text attribute
        /// </summary>
        private string _notificationText;
        public string NotificationText
        {
            get { return _notificationText; }
            set
            {
                if (_notificationText != value)
                    _notificationText = value;
                RaisePropertyChanged(() => this.NotificationText);
            }
        } 
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

        #region ICommand
        /// <summary>
        /// Property binding to Hyperlink 'Back to Login Screen' click event
        /// </summary>
        public ICommand CancelCommand
        {
            get { return new DelegateCommand<object>(CancelCommandMethod); }
        }        
        #endregion 
        #endregion

        #region ICommand Methods
        /// <summary>
        /// ICommand property implementing 'Back to Login Screen' Hyperlink click event - navigate to login screen
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
        /// NavigateTo Property set to true; NotificationText Property assigned from navigationContext
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            try
            {
                NavigateTo = true;
                NotificationText = navigationContext.NavigationService.Region.Context as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        } 
        #endregion        
    }
}
