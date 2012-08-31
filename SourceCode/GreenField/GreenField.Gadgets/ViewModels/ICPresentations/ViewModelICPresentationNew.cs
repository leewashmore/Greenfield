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
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using GreenField.Gadgets.Models;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Logging;
//using GreenField.DataContracts;
using GreenField.UserSession;
using GreenField.DataContracts;


namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelICPresentationNew : NotificationObject
    {

        #region Fields

        public IRegionManager _regionManager { private get; set; }

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

        private EntitySelectionData _entitySelectionData;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        #endregion

        #region Constructor
        public ViewModelICPresentationNew(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;

            FetchMeetingInfo();

            // _dbInteractivity.GetPresentations(GetPresentationsCallBackMethod);
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            //Subscription to SecurityReferenceSet event
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);

            //EntitySelectionData handling
            if (_entitySelectionData != null)
            {
                HandleSecurityReferenceSet(_entitySelectionData);
            }

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

        private ICPresentationOverviewData _iCPresentationOverviewInfo;
        public ICPresentationOverviewData ICPresentationOverviewInfo
        {
            get
            {
                if (_iCPresentationOverviewInfo == null)
                {
                    _iCPresentationOverviewInfo = new ICPresentationOverviewData()
                    {
                        AcceptWithoutDiscussionFlag = true,
                        StatusType = StatusType.IN_PROGRESS,
                        Presenter = SessionManager.SESSION.UserName,
                    };
                }
                return _iCPresentationOverviewInfo;
            }
            set
            {
                _iCPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.ICPresentationOverviewInfo);
            }
        }
        #endregion


        public ICommand SaveCommand
        {
            get { return new DelegateCommand<object>(SaveCommandMethod, SaveCommandValidationMethod); }
        }

        #region ICommand Methods
        private void SaveCommandMethod(object param)
        {
            if (_dbInteractivity != null)
            {
                _dbInteractivity.CreatePresentation(UserSession.SessionManager.SESSION.UserName, ICPresentationOverviewInfo, CreatePresentationCallBackMethod);             
            }           
        }

        private bool SaveCommandValidationMethod(object param)
        {
            return true;
        }
        #endregion

        #region Callback
        private void CreatePresentationCallBackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (result == true)
                    {
                        Prompt.ShowDialog("Presentation successfully created");
                        _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATIONS);
                        _regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteePresentations");
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Callback method for Security Overview Service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="securityOverviewData">SecurityOverviewData Collection</param>
        private void RetrieveSecurityDetailsCallBackMethod(ICPresentationOverviewData result)
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
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Assigns UI Field Properties based on Entity Selection Data
        /// </summary>
        /// <param name="entitySelectionData">EntitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _entitySelectionData = entitySelectionData;

                    if (IsActive && _entitySelectionData != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving security reference data for '" + entitySelectionData.LongName + " (" + entitySelectionData.ShortName + ")'");
                        _dbInteractivity.RetrieveSecurityDetails(entitySelectionData, ICPresentationOverviewInfo, RetrieveSecurityDetailsCallBackMethod);                        
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
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Helper Methods
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }
        public void FetchMeetingInfo()
        {
            MeetingInfo meetingInfo = ICNavigation.Fetch(ICNavigationInfo.MeetingInfo) as MeetingInfo;
            if (meetingInfo != null)
            {
                ICPresentationOverviewInfo.MeetingDateTime = meetingInfo.MeetingDateTime;
                ICPresentationOverviewInfo.MeetingClosedDateTime = meetingInfo.MeetingClosedDateTime;
                ICPresentationOverviewInfo.MeetingVotingClosedDateTime = meetingInfo.MeetingVotingClosedDateTime;                
            }
        }

        //public void Initialize()
        //{
        //    if (_dbInteractivity != null && IsActive)
        //    {
        //        BusyIndicatorNotification(true, "Retrieving Presentation Overview Information...");
        //        FetchMeetingInfo();                

        //        //Subscription to SecurityReferenceSet event
        //        _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);

        //        //EntitySelectionData handling
        //        if (_entitySelectionData != null)
        //        {
        //            HandleSecurityReferenceSet(_entitySelectionData);
        //        }
        //    }
        //}

        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

    }
}