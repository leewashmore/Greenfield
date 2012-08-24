using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Regions;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Views;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using GreenField.Gadgets.Helpers;



namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPresentations : NotificationObject
    {

        #region Fields
        private IRegionManager _regionManager;
       // private ManageMeetings _manageMeetings;
        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
            }
        }
        #endregion

        #region Constructor
        public ViewModelPresentations(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;
            
            RetrievePresentationOverviewData();
        }
        #endregion

        #region Properties

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool _busyIndicatorIsBusy = false;
        public bool BusyIndicatorIsBusy
        {
            get { return _busyIndicatorIsBusy; }
            set
            {
                _busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string _busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return _busyIndicatorContent; }
            set
            {
                _busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion        

        private List<ICPresentationOverviewData> _iCPresentationOverviewInfo;
        public List<ICPresentationOverviewData> ICPresentationOverviewInfo
        {
            get { return _iCPresentationOverviewInfo; }
            set 
            {
                _iCPresentationOverviewInfo = value; 
                RaisePropertyChanged(() => this.ICPresentationOverviewInfo);
            }
        }

        private ICPresentationOverviewData _selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return _selectedPresentationOverviewInfo; }
            set
            {
                _selectedPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
                ICNavigation.Update(ICNavigationInfo.PresentationOverviewInfo, value);                
                SelectionRaisePropertyChanged();
            }
        }                        

        #region ICommand Properties
        public ICommand ChangeDateCommand
        {
            get { return new DelegateCommand<object>(ChangeDateCommandMethod, ChangeDateCommandValidationMethod); }
        }

        public ICommand DecisionEntryCommand
        {
            get { return new DelegateCommand<object>(DecisionEntryCommandMethod, DecisionEntryCommandValidationMethod); }
        }
     
        public ICommand UploadCommand
        {
            get { return new DelegateCommand<object>(UploadCommandMethod, UploadCommandValidationMethod); }
        }

        public ICommand EditCommand
        {
            get { return new DelegateCommand<object>(EditCommandMethod, ChangeDateCommandValidationMethod); }
        }
      
        public ICommand WithdrawCommand
        {
            get { return new DelegateCommand<object>(WithdrawCommandMethod, EditCommandValidationMethod); }
        }

        public ICommand ViewCommand
        {
            get { return new DelegateCommand<object>(ViewCommandMethod, ViewCommandValidationMethod); }
        }    

        public ICommand NewCommand
        {
            get { return new DelegateCommand<object>(NewCommandMethod); }
        }
        #endregion        

        #endregion

        #region ICommand Methods
        private bool ChangeDateCommandValidationMethod(object param)
        {
            return true;
        }

        private void ChangeDateCommandMethod(object param)
        {
            ChildViewPresentationDateChangeEdit childViewPresentationDateChangeEdit = new ChildViewPresentationDateChangeEdit();
            childViewPresentationDateChangeEdit.Show();
        }

        private bool DecisionEntryCommandValidationMethod(object param)
        {
            return true;
        }

        private void DecisionEntryCommandMethod(object param)
        {
            
        }

        private bool UploadCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null 
                || SelectedPresentationOverviewInfo == null)
                return false;

            bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool statusValidation = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS;
            return userRoleValidation && statusValidation;
        }

        private void UploadCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Upload);
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_EDIT_PRESENTATION);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeEditPresentations", UriKind.Relative));
        }

        private bool EditCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;

            bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool statusValidation = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS
                || SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING;

            return userRoleValidation && statusValidation;
        }

        private void EditCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Edit);
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_EDIT_PRESENTATION);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeEditPresentations", UriKind.Relative));
        }

        private bool ICPPresentationsWithdrawItemValidation(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;

            bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool statusValidation = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS
                || SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING;

            return userRoleValidation && statusValidation;
        }

        private void WithdrawCommandMethod(object param)
        {
            
        }   

        private bool ViewCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;

            bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool statusValidation = UserSession.SessionManager.SESSION.Roles.Contains("IC_MEMBER_VOTING")
                ? SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING
                : SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS
                    || SelectedPresentationOverviewInfo.StatusType == StatusType.WITHDRAWN;

            return userRoleValidation && statusValidation;            
        }

        private void ViewCommandMethod(object param)
        {
            bool userRoleValidation = UserSession.SessionManager.SESSION.Roles.Contains("IC_MEMBER_VOTING");

            if (userRoleValidation && SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING)
                ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Vote);
            else
                ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.View);
                        
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_VOTE);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeVote", UriKind.Relative));
        }

        private void NewCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Create);
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_NEW_PRESENTATION);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeNew", UriKind.Relative));
        }
        #endregion
        
        #region Helper Methods

        private void RetrievePresentationOverviewData()
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving Presentation Overview Information...");
                _dbInteractivity.RetrievePresentationOverviewData(RetrievePresentationOverviewDataCallbackMethod);
            }
        }
        
        private void SelectionRaisePropertyChanged()
        {
            RaisePropertyChanged(() => this.EditCommand);
            RaisePropertyChanged(() => this.UploadCommand);
            RaisePropertyChanged(() => this.NewCommand);
            RaisePropertyChanged(() => this.WithdrawCommand);
            RaisePropertyChanged(() => this.ViewCommand);
            RaisePropertyChanged(() => this.ChangeDateCommand);
            RaisePropertyChanged(() => this.DecisionEntryCommand);
        }        

        #endregion

        #region CallBack Methods
        private void RetrievePresentationOverviewDataCallbackMethod(List<ICPresentationOverviewData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    ICPresentationOverviewInfo = result;
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }            
        }        
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
           
        }

        #endregion   

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }
    }
}
