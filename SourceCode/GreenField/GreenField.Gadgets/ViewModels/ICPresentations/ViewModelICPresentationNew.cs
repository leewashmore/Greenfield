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
using GreenField.ServiceCaller;
using GreenField.Gadgets.Models;
using GreenField.Common;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Logging;
using GreenField.UserSession;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelICPresentationNew : NotificationObject
    {

        #region Fields
        private IRegionManager _regionManager { get; set; }
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
        #endregion

        #region Constructor
        public ViewModelICPresentationNew(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;

            // _dbInteractivity.GetPresentations(GetPresentationsCallBackMethod);
            _entitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            _portfolioSelectionInfo = param.DashboardGadgetPayload.PortfolioSelectionData;

            //Subscription to SecurityReferenceSet event
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
            }
        }
        #endregion

        #region Properties
        #region IsActive
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

                if (value)
                {
                    Initialize();
                    HandleSecurityReferenceSet(_entitySelectionInfo);
                }
            }
        }
        #endregion

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

        #region Binded
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
                        Presenter = SessionManager.SESSION.UserName.ToLower(),
                        CreatedBy = SessionManager.SESSION.UserName.ToLower(),
                        CreatedOn = DateTime.UtcNow,
                        ModifiedBy = SessionManager.SESSION.UserName.ToLower(),
                        ModifiedOn = DateTime.UtcNow
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

        private EntitySelectionData _entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get { return _entitySelectionInfo; }
            set
            {
                if (_entitySelectionInfo != value)
                {
                    _entitySelectionInfo = value;
                    RaisePropertyChanged(() => EntitySelectionInfo);
                }
            }
        }

        private PortfolioSelectionData _portfolioSelectionInfo;

        public PortfolioSelectionData PortfolioSelectionInfo
        {
            get { return _portfolioSelectionInfo; }
            set
            {
                if (_portfolioSelectionInfo != value)
                {
                    _portfolioSelectionInfo = value;
                    RaisePropertyChanged(() => PortfolioSelectionInfo);
                }
            }
        }
        #endregion

        #region ICommand
        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }
        #endregion

        #endregion

        #region ICommand Methods
        private void SubmitCommandMethod(object param)
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Setting up presentation details..");
                _dbInteractivity.CreatePresentation(UserSession.SessionManager.SESSION.UserName, ICPresentationOverviewInfo, CreatePresentationCallBackMethod);
            }
        }

        private bool SubmitCommandValidationMethod(object param)
        {
            Boolean selectionValidation = _entitySelectionInfo != null && _portfolioSelectionInfo != null;
            Boolean dataValidation = ICPresentationOverviewInfo.SecurityBuyRange != null
                && ICPresentationOverviewInfo.SecurityPFVMeasure != String.Empty
                && ICPresentationOverviewInfo.SecuritySellRange != null
                && ICPresentationOverviewInfo.SecurityRecommendation != null
                && ICPresentationOverviewInfo.SecurityRecommendation != String.Empty
                && ICPresentationOverviewInfo.YTDRet_Absolute != null
                && ICPresentationOverviewInfo.YTDRet_Absolute.Count() > 1
                && ICPresentationOverviewInfo.YTDRet_RELtoEM != null
                && ICPresentationOverviewInfo.YTDRet_RELtoEM.Count() > 1
                && ICPresentationOverviewInfo.YTDRet_RELtoLOC != null
                && ICPresentationOverviewInfo.YTDRet_RELtoLOC.Count() > 1;

            return selectionValidation && dataValidation;
        }
        #endregion

        #region Callback
        private void CreatePresentationCallBackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATIONS);
                    _regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteePresentations");
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
                    RaisePropertyChanged(() => this.SubmitCommand);
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
                    _entitySelectionInfo = entitySelectionData;

                    if (IsActive && _entitySelectionInfo != null && PortfolioSelectionInfo != null)
                    {
                        RaisePropertyChanged(() => this.SubmitCommand);
                        HandlePortfolioReferenceSet(PortfolioSelectionInfo);
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

        /// <summary>
        /// Event handler for PortfolioSelection changed Event
        /// </summary>
        /// <param name="PortfolioSelectionData"></param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, portfolioSelectionData, 1);
                    _portfolioSelectionInfo = portfolioSelectionData;

                    //Argument Null Exception
                    if (IsActive && _entitySelectionInfo != null && PortfolioSelectionInfo != null)
                    {
                        RaisePropertyChanged(() => this.SubmitCommand);
                        BusyIndicatorNotification(true, "Retrieving security reference data for '" + _entitySelectionInfo.LongName + " (" + _entitySelectionInfo.ShortName + ")'");
                        _dbInteractivity.RetrieveSecurityDetails(_entitySelectionInfo, ICPresentationOverviewInfo, _portfolioSelectionInfo, RetrieveSecurityDetailsCallBackMethod);
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
        }
        #endregion

        #region Helper Methods
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        public void Initialize()
        {
            MeetingInfo meetingInfo = ICNavigation.Fetch(ICNavigationInfo.MeetingInfo) as MeetingInfo;
            if (meetingInfo != null)
            {
                ICPresentationOverviewInfo = new ICPresentationOverviewData()
                {
                    AcceptWithoutDiscussionFlag = true,
                    StatusType = StatusType.IN_PROGRESS,
                    Presenter = SessionManager.SESSION.UserName.ToLower(),
                    CreatedBy = SessionManager.SESSION.UserName.ToLower(),
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = SessionManager.SESSION.UserName.ToLower(),
                    ModifiedOn = DateTime.UtcNow,
                    MeetingDateTime = meetingInfo.MeetingDateTime,
                    CommitteeRangeEffectiveThrough = meetingInfo.MeetingDateTime.Date.AddMonths(3),
                    MeetingClosedDateTime = meetingInfo.MeetingClosedDateTime,
                    MeetingVotingClosedDateTime = meetingInfo.MeetingVotingClosedDateTime
                };
            }
        }

        public void RaiseICPresentationOverviewInfoChanged(Decimal valueYTDAbs, Decimal valueYTDReltoLoc, Decimal valueYTDReltoEM)
        {
            ICPresentationOverviewInfo.YTDRet_Absolute = String.Format("{0:n4}", valueYTDAbs) + "%";
            ICPresentationOverviewInfo.YTDRet_RELtoEM = String.Format("{0:n4}", valueYTDReltoLoc) + "%";
            ICPresentationOverviewInfo.YTDRet_RELtoLOC = String.Format("{0:n4}", valueYTDReltoEM) + "%";
            RaisePropertyChanged(() => this.SubmitCommand);
        }

        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
        }

        #endregion

    }
}