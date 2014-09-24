using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;
using GreenField.UserSession;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model class for ViewICPresentationNew
    /// </summary>
    public class ViewModelICPresentationNew : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Region Manager
        /// </summary>
        private IRegionManager regionManager { get; set; }

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade logger;        
        #endregion

        #region Properties
        #region IsActive
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (value)
                {
                    Initialize();
                    HandleSecurityReferenceSet(EntitySelectionInfo);
                }
            }
        }
        #endregion

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool isBusyIndicatorBusy = false;
        public bool IsBusyIndicatorBusy
        {
            get { return isBusyIndicatorBusy; }
            set
            {
                isBusyIndicatorBusy = value;
                RaisePropertyChanged(() => this.IsBusyIndicatorBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return busyIndicatorContent; }
            set
            {
                busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion

        #region Binded
        /// <summary>
        /// Stores presentation overview information received from the overview screen
        /// </summary>
        private ICPresentationOverviewData iCPresentationOverviewInfo;
        public ICPresentationOverviewData ICPresentationOverviewInfo
        {
            get
            {
                if (iCPresentationOverviewInfo == null)
                {
                    iCPresentationOverviewInfo = new ICPresentationOverviewData()
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
                return iCPresentationOverviewInfo;
            }
            set
            {
                iCPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.ICPresentationOverviewInfo);
            }
        }

        /// <summary>
        /// Stores entity selected from the toolbox
        /// </summary>
        private EntitySelectionData entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get { return entitySelectionInfo; }
            set
            {
                if (entitySelectionInfo != value)
                {
                    entitySelectionInfo = value;
                    RaisePropertyChanged(() => EntitySelectionInfo);
                }
            }
        }

        /// <summary>
        /// Stores portfolio selection information
        /// </summary>
        private PortfolioSelectionData portfolioSelectionInfo;
        public PortfolioSelectionData PortfolioSelectionInfo
        {
            get { return portfolioSelectionInfo; }
            set
            {
                if (portfolioSelectionInfo != value)
                {
                    portfolioSelectionInfo = value;
                    RaisePropertyChanged(() => PortfolioSelectionInfo);
                }
            }
        }
        #endregion

        #region ICommand
        /// <summary>
        /// Submit Command
        /// </summary>
        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelICPresentationNew(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            regionManager = param.RegionManager;

            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            PortfolioSelectionInfo = param.DashboardGadgetPayload.PortfolioSelectionData;

            //Subscription to SecurityReferenceSet event
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
            }
        }
        #endregion        

        #region ICommand Methods
        /// <summary>
        /// SubmitCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool SubmitCommandValidationMethod(object param)
        {
            Boolean selectionValidation = entitySelectionInfo != null && portfolioSelectionInfo != null;
            Boolean dataValidation = ICPresentationOverviewInfo.SecurityBuyRange != null
                && ICPresentationOverviewInfo.SecurityPFVMeasure != String.Empty
                && ICPresentationOverviewInfo.SecuritySellRange != null
                && ICPresentationOverviewInfo.FVCalc != null
                && ICPresentationOverviewInfo.SecurityRecommendation != null
                && ICPresentationOverviewInfo.SecurityRecommendation != String.Empty
                && ICPresentationOverviewInfo.YTDRet_Absolute != null
                && ICPresentationOverviewInfo.YTDRet_Absolute.Count() > 1
                && ICPresentationOverviewInfo.YTDRet_RELtoEM != null
                && ICPresentationOverviewInfo.YTDRet_RELtoEM.Count() > 1
                && ICPresentationOverviewInfo.YTDRet_RELtoLOC != null
                && ICPresentationOverviewInfo.YTDRet_RELtoLOC.Count() > 1
                && ICPresentationOverviewInfo.MeetingClosedDateTime != null;

            return selectionValidation && dataValidation;
        }

        /// <summary>
        /// SubmitCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SubmitCommandMethod(object param)
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Setting up presentation details..");
                dbInteractivity.CreatePresentation(UserSession.SessionManager.SESSION.UserName, ICPresentationOverviewInfo, CreatePresentationCallBackMethod);
            }
        }        
        #endregion

        #region Callback
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void CreatePresentationCallBackMethod(Int64? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result> 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);

                   /* eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATIONS);
                    ICNavigation.Update(ICNavigationInfo.MeetingInfo, iCPresentationOverviewInfo);
                    regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardICPresentation", UriKind.Relative);*/
                    iCPresentationOverviewInfo.PresentationID = (long)result;
                    ICNavigation.Update(ICNavigationInfo.PresentationOverviewInfo, iCPresentationOverviewInfo);
                    eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATIONS);
                    regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardICPresentation", UriKind.Relative));

                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    ICPresentationOverviewInfo = result;
                    RaisePropertyChanged(() => this.SubmitCommand);
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                    EntitySelectionInfo = entitySelectionData;

                    if (IsActive && EntitySelectionInfo != null && PortfolioSelectionInfo != null)
                    {
                        RaisePropertyChanged(() => this.SubmitCommand);
                        HandlePortfolioReferenceSet(PortfolioSelectionInfo);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Event handler for PortfolioSelection changed Event
        /// </summary>
        /// <param name="PortfolioSelectionData">PortfolioSelectionData</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portfolioSelectionData, 1);
                    PortfolioSelectionInfo = portfolioSelectionData;

                    if (IsActive && entitySelectionInfo != null && PortfolioSelectionInfo != null)
                    {
                        RaisePropertyChanged(() => this.SubmitCommand);
                        BusyIndicatorNotification(true, "Retrieving security reference data for '" 
                            + entitySelectionInfo.LongName + " (" + entitySelectionInfo.ShortName + ")'");
                        dbInteractivity.RetrieveSecurityDetails(entitySelectionInfo, ICPresentationOverviewInfo
                            , portfolioSelectionInfo, RetrieveSecurityDetailsCallBackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="isBusyIndicatorVisible">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        private void BusyIndicatorNotification(bool isBusyIndicatorVisible = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            IsBusyIndicatorBusy = isBusyIndicatorVisible;
        }

        /// <summary>
        /// Initializes view
        /// </summary>
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

        /// <summary>
        /// Updates YTD Ret values in presentation overview based on input values
        /// </summary>
        /// <param name="valueYTDAbs">YTDRet_Absolute</param>
        /// <param name="valueYTDReltoLoc">YTDRet_RELtoLOC</param>
        /// <param name="valueYTDReltoEM">YTDRet_RELtoEM</param>
        public void RaiseICPresentationOverviewInfoChanged(Decimal valueYTDAbs, Decimal valueYTDReltoLoc, Decimal valueYTDReltoEM)
        {
            ICPresentationOverviewInfo.YTDRet_Absolute = String.Format("{0:n2}", valueYTDAbs) + "%";
            ICPresentationOverviewInfo.YTDRet_RELtoLOC = String.Format("{0:n2}", valueYTDReltoEM) + "%";
            ICPresentationOverviewInfo.YTDRet_RELtoEM = String.Format("{0:n2}", valueYTDReltoLoc) + "%";            
            RaisePropertyChanged(() => this.SubmitCommand);
        }
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
        }
        #endregion
    }
}