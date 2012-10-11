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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.IO;
using System.Reflection;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model class for ViewPresentationDecisionEntry
    /// </summary>
    public class ViewModelPresentationDecisionEntry : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Region Manager
        /// </summary>
        private IRegionManager regionManager;

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
                }
            }
        } 
        #endregion

        #region Presentation details
        /// <summary>
        /// Stores true if security is held in portfolios
        /// </summary>
        public Boolean isSecurityHeld { get; set; }

        /// <summary>
        /// Stores presentation overview details
        /// </summary>
        private ICPresentationOverviewData selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return selectedPresentationOverviewInfo; }
            set
            {
                if (value != null)
                {
                    if (value.AcceptWithoutDiscussionFlag == null)
                    {
                        value.AcceptWithoutDiscussionFlag = true;
                        value.CommitteePFVMeasure = value.SecurityPFVMeasure;
                        value.CommitteeBuyRange = value.SecurityBuyRange;
                        value.CommitteeSellRange = value.SecuritySellRange;
                        value.CommitteeRecommendation = value.SecurityRecommendation;
                    }

                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Voting information for the selected presentation");
                        dbInteractivity.RetrievePresentationVoterData(value.PresentationID, RetrievePresentationVoterDataCallbackMethod);
                    }
                }
                selectedPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
                IsAcceptWithoutDiscussionChecked = value.AcceptWithoutDiscussionFlag;
            }
        } 
        #endregion

        #region Interface interacting properties
        /// <summary>
        /// Stores true if IC Decision panel is enabled
        /// </summary>
        private Boolean _isICDecisionEnable;
        public Boolean IsICDecisionEnable
        {
            get { return _isICDecisionEnable; }
            set
            {
                _isICDecisionEnable = value;
                RaisePropertyChanged(() => this.IsICDecisionEnable);
            }
        }

        /// <summary>
        /// Stores true if Accept without discussion checkbox is checked
        /// </summary>
        private Boolean? _isAcceptWithoutDiscussionChecked = true;
        public Boolean? IsAcceptWithoutDiscussionChecked
        {
            get { return _isAcceptWithoutDiscussionChecked; }
            set
            {
                _isAcceptWithoutDiscussionChecked = value;
                RaisePropertyChanged(() => this.IsAcceptWithoutDiscussionChecked);
                if (value != null)
                {
                    IsICDecisionEnable = !Convert.ToBoolean(value);
                    SelectedPresentationOverviewInfo.AcceptWithoutDiscussionFlag = Convert.ToBoolean(value);

                    if (Convert.ToBoolean(value))
                    {
                        SelectedPresentationOverviewInfo.CommitteePFVMeasure = SelectedPresentationOverviewInfo.SecurityPFVMeasure;
                        SelectedPresentationOverviewInfo.CommitteePFVMeasureValue = SecurityPFVMeasureCurrentPrices != null
                            ? SecurityPFVMeasureCurrentPrices[SelectedPresentationOverviewInfo.SecurityPFVMeasure] : null;
                        SelectedPresentationOverviewInfo.CommitteeBuyRange = SelectedPresentationOverviewInfo.SecurityBuyRange;
                        SelectedPresentationOverviewInfo.CommitteeSellRange = SelectedPresentationOverviewInfo.SecuritySellRange;
                        SelectedPresentationOverviewInfo.CommitteeRecommendation = SelectedPresentationOverviewInfo.SecurityRecommendation;
                    }
                    RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
                }
            }
        } 
        #endregion

        #region IC Decision input
        /// <summary>
        /// Stores vote type reference
        /// </summary>
        public List<String> VoteTypeInfo
        {
            get
            {
                return new List<string> 
                { 
                    VoteType.AGREE,
                    VoteType.ABSTAIN,
                    VoteType.MODIFY
                };
            }
        }                

        /// <summary>
        /// Stores PFV Measure type reference 
        /// </summary>
        public List<String> PFVTypeInfo
        {
            get
            {
                return new List<string> 
                { 
                    PFVType.FORWARD_DIVIDEND_YIELD,
                    PFVType.FORWARD_EV_EBITDA,
                    PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_COUNTRY,
                    PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY,
                    PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY,
                    PFVType.FORWARD_EV_REVENUE,
                    PFVType.FORWARD_P_NAV,
                    PFVType.FORWARD_P_APPRAISAL_VALUE,
                    PFVType.FORWARD_P_BV,
                    PFVType.FORWARD_P_BV_RELATIVE_TO_COUNTRY,
                    PFVType.FORWARD_P_BV_RELATIVE_TO_INDUSTRY,
                    PFVType.FORWARD_P_BV_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY,
                    PFVType.FORWARD_P_CE,
                    PFVType.FORWARD_P_E,
                    PFVType.FORWARD_P_E_RELATIVE_TO_COUNTRY,
                    PFVType.FORWARD_P_E_RELATIVE_TO_INDUSTRY,
                    PFVType.FORWARD_P_E_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY,
                    PFVType.FORWARD_P_E_TO_2_YEAR_EARNINGS_GROWTH,
                    PFVType.FORWARD_P_E_TO_3_YEAR_EARNINGS_GROWTH,
                    PFVType.FORWARD_P_EMBEDDED_VALUE,
                    PFVType.FORWARD_P_REVENUE
                };
            }
        }

        /// <summary>
        /// Stores PFV Measure types as keys with value equal to their current system value
        /// </summary>
        private Dictionary<String, Decimal?> _securityPFVMeasureCurrentPrices;
        public Dictionary<String, Decimal?> SecurityPFVMeasureCurrentPrices
        {
            get { return _securityPFVMeasureCurrentPrices; }
            set
            {
                _securityPFVMeasureCurrentPrices = value;
                if (value != null)
                {
                    SelectedPresentationOverviewInfo.CommitteePFVMeasureValue = value[SelectedPresentationOverviewInfo.CommitteePFVMeasure];
                }
            }
        } 
        #endregion

        #region Voter Information
        /// <summary>
        /// Stores presentation voter information
        /// </summary>
        private List<VoterInfo> _presentationVoterInfo;
        public List<VoterInfo> PresentationVoterInfo
        {
            get { return _presentationVoterInfo; }
            set { _presentationVoterInfo = value; }
        }

        /// <summary>
        /// Stores presentation pre-meeting voter information 
        /// </summary>
        private List<VoterInfo> _presentationPreMeetingVoterInfo;
        public List<VoterInfo> PresentationPreMeetingVoterInfo
        {
            get { return _presentationPreMeetingVoterInfo; }
            set
            {
                _presentationPreMeetingVoterInfo = value;
                RaisePropertyChanged(() => this.PresentationPreMeetingVoterInfo);
            }
        }

        /// <summary>
        /// Stores presentation post-meeting voter information 
        /// </summary>
        private List<VoterInfo> _presentationPostMeetingVoterInfo;
        public List<VoterInfo> PresentationPostMeetingVoterInfo
        {
            get { return _presentationPostMeetingVoterInfo; }
            set
            {
                _presentationPostMeetingVoterInfo = value;
                RaisePropertyChanged(() => this.PresentationPostMeetingVoterInfo);
            }
        } 
        #endregion

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelPresentationDecisionEntry(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            regionManager = param.RegionManager;
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

        #region ICommand Methods
        /// <summary>
        /// SubmitCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SubmitCommandMethod(object param)
        {
            if (SecurityPFVMeasureCurrentPrices == null)
            {
                Prompt.ShowDialog("Decision Entry form could not be submitted"
                + " owing to unavailability of current P/FV Measure prices for the selected security");
                return;
            }
            if (SecurityPFVMeasureCurrentPrices[SelectedPresentationOverviewInfo.CommitteePFVMeasure] == null)
            {
                Prompt.ShowDialog("Decision Entry form could not be submitted owing to unavailability"
                + " of current P/FV Measure price for the selected security and P/FV measure in IC Decision section");
                return;
            }
            if (SelectedPresentationOverviewInfo.CommitteePFVMeasure == null
                || SelectedPresentationOverviewInfo.CommitteeBuyRange == null
                || SelectedPresentationOverviewInfo.CommitteeSellRange == null
                || SelectedPresentationOverviewInfo.CommitteePFVMeasureValue == null)
            {
                Prompt.ShowDialog("'Modify' Vote input has not been supplemented with valid"
                + " P/FV Measure, Buy and Sell Range for one or more voting members");
                return;
            }
            foreach (VoterInfo info in PresentationPostMeetingVoterInfo)
            {
                if (info.VoteType == VoteType.MODIFY)
                {
                    if (info.VoterPFVMeasure == null || info.VoterBuyRange == null || info.VoterSellRange == null)
                    {
                        Prompt.ShowDialog("'Modify' Vote input has not been supplemented with valid"
                        + " P/FV Measure, Buy and Sell Range for one or more voting members");
                        return;
                    }
                }
            }
            if (dbInteractivity != null)
            {
                dbInteractivity.UpdateDecisionEntryDetails(UserSession.SessionManager.SESSION.UserName, SelectedPresentationOverviewInfo
                    , PresentationPostMeetingVoterInfo, UpdateDecisionEntryDetailsCallbackMethod);
            }
        } 
        #endregion

        #region CallBack Methods
        /// <summary>
        /// RetrievePresentationVoterData callback method
        /// </summary>
        /// <param name="result">List of VoterInfo</param>
        private void RetrievePresentationVoterDataCallbackMethod(List<VoterInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    PresentationVoterInfo = ListUtils.GetDeepCopy<VoterInfo>(result);
                    PresentationPreMeetingVoterInfo = result
                        .Where(record => record.PostMeetingFlag == false && record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower())
                        .OrderBy(record => record.Name).ToList();
                    PresentationPostMeetingVoterInfo = result
                        .Where(record => record.PostMeetingFlag == true && record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower())
                        .OrderBy(record => record.Name).ToList();

                    foreach (VoterInfo postMeetingVoterInfo in PresentationPostMeetingVoterInfo)
                    {
                        if (postMeetingVoterInfo.VoteType == null)
                        {
                            VoterInfo preMeetingVoterInfo = PresentationPreMeetingVoterInfo
                                .Where(record => record.Name == postMeetingVoterInfo.Name).FirstOrDefault();
                            if(preMeetingVoterInfo != null)
                            {
                                postMeetingVoterInfo.VoteType = preMeetingVoterInfo.VoteType;
                                postMeetingVoterInfo.VoterPFVMeasure = preMeetingVoterInfo.VoterPFVMeasure;
                                postMeetingVoterInfo.VoterBuyRange = preMeetingVoterInfo.VoterBuyRange;
                                postMeetingVoterInfo.VoterSellRange = preMeetingVoterInfo.VoterSellRange;
                                postMeetingVoterInfo.VoterRecommendation = preMeetingVoterInfo.VoterRecommendation;
                            }
                        }                        
                    }
                    isSecurityHeld = SelectedPresentationOverviewInfo.CurrentHoldings.ToLower() == "yes";

                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieve current P/FV Measure values related to presented security...");
                        dbInteractivity.RetrieveCurrentPFVMeasures(PFVTypeInfo, SelectedPresentationOverviewInfo.SecurityTicker
                            , RetrieveCurrentPFVMeasuresCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);                
            }
        }

        /// <summary>
        /// RetrieveCurrentPFVMeasures callback method
        /// </summary>
        /// <param name="result">Dictionary od string key and nullable decimal values</param>
        private void RetrieveCurrentPFVMeasuresCallbackMethod(Dictionary<String, Decimal?> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SecurityPFVMeasureCurrentPrices = result;
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
        /// UpdateDecisionEntryDetails callback method
        /// </summary>
        /// <param name="result">Nullable Boolean</param>
        private void UpdateDecisionEntryDetailsCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (result == true)
                    {
                        Prompt.ShowDialog("Decision Entry successfully completed");
                        regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteePresentations");
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while submitting Decision Entry form.");
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
        /// Updates ICDecision Recommendation based on selected PFV Measure, buy and sell range
        /// </summary>
        /// <param name="selectedPFVMeasure">PFV Measure</param>
        /// <param name="buyRange">BuyRange</param>
        /// <param name="sellRange">Sell Range</param>
        public void UpdateICDecisionRecommendation(String selectedPFVMeasure, Decimal buyRange, Decimal sellRange)
        {
            if (selectedPFVMeasure == null || SecurityPFVMeasureCurrentPrices == null)
                return;

            Decimal? CurrentPFVMeasurePrice = SecurityPFVMeasureCurrentPrices[selectedPFVMeasure];

            if (CurrentPFVMeasurePrice == null)
            {
                Prompt.ShowDialog("Current P/FV measure value is not available to evaluate recommendation");
                return;
            }
            SelectedPresentationOverviewInfo.CommitteeBuyRange = Convert.ToSingle(buyRange);
            SelectedPresentationOverviewInfo.CommitteeSellRange = Convert.ToSingle(sellRange);
            SelectedPresentationOverviewInfo.CommitteePFVMeasureValue = Convert.ToDecimal(CurrentPFVMeasurePrice);

            Decimal lowerLimit = buyRange <= sellRange ? buyRange : sellRange;
            Decimal upperLimit = buyRange > sellRange ? buyRange : sellRange;

            if (isSecurityHeld)
            {
                if (CurrentPFVMeasurePrice <= upperLimit)
                {
                    SelectedPresentationOverviewInfo.CommitteeRecommendation = RecommendationType.HOLD;
                }
                else if (CurrentPFVMeasurePrice > upperLimit)
                {
                    SelectedPresentationOverviewInfo.CommitteeRecommendation = RecommendationType.SELL;
                }
            }
            else
            {
                if (CurrentPFVMeasurePrice <= lowerLimit)
                {
                    SelectedPresentationOverviewInfo.CommitteeRecommendation = RecommendationType.BUY;
                }
                else if (CurrentPFVMeasurePrice > lowerLimit)
                {
                    SelectedPresentationOverviewInfo.CommitteeRecommendation = RecommendationType.WATCH;
                }
            }

            RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
        }

        /// <summary>
        /// Initializes view
        /// </summary>
        public void Initialize()
        {
            PresentationVoterInfo = null;
            PresentationPreMeetingVoterInfo = null;
            PresentationPostMeetingVoterInfo = null;
            isSecurityHeld = false;
            SecurityPFVMeasureCurrentPrices = null;            
            SelectedPresentationOverviewInfo = ICNavigation.Fetch(ICNavigationInfo.PresentationOverviewInfo) as ICPresentationOverviewData;
        }

        public void RaiseUpdateFinalVoteType(VoterInfo voterInfo)
        {
            if (voterInfo == null)
                return;

            RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);

            if (voterInfo.VoteType == VoteType.AGREE)
            {
                VoterInfo bindedItem = PresentationPostMeetingVoterInfo.Where(record => record.VoterID == voterInfo.VoterID).FirstOrDefault();
                bindedItem.VoterPFVMeasure = SelectedPresentationOverviewInfo.SecurityPFVMeasure;
                bindedItem.VoterBuyRange = SelectedPresentationOverviewInfo.SecurityBuyRange;
                bindedItem.VoterSellRange = SelectedPresentationOverviewInfo.SecuritySellRange;

                RaisePropertyChanged(() => this.PresentationPostMeetingVoterInfo);
            }
            else if (voterInfo.VoteType == VoteType.MODIFY)
            {
                VoterInfo origItem = PresentationVoterInfo.Where(record => record.VoterID == voterInfo.VoterID).FirstOrDefault();
                if (origItem == null)
                    return;

                if (origItem.VoteType != VoteType.MODIFY)
                    return;

                VoterInfo bindedItem = PresentationPostMeetingVoterInfo.Where(record => record.VoterID == voterInfo.VoterID).FirstOrDefault();
                if (bindedItem == null)
                    return;

                bindedItem.VoterPFVMeasure = origItem.VoterPFVMeasure;
                bindedItem.VoterBuyRange = origItem.VoterBuyRange;
                bindedItem.VoterSellRange = origItem.VoterSellRange;

                RaisePropertyChanged(() => this.PresentationPostMeetingVoterInfo);                 
            }
            else if (voterInfo.VoteType == VoteType.ABSTAIN)
            {
                VoterInfo bindedItem = PresentationPostMeetingVoterInfo.Where(record => record.VoterID == voterInfo.VoterID).FirstOrDefault();
                bindedItem.VoterPFVMeasure = null;
                bindedItem.VoterBuyRange = null;
                bindedItem.VoterSellRange = null;

                RaisePropertyChanged(() => this.PresentationPostMeetingVoterInfo);
            }
        }

        public void Dispose()
        {
        } 
        #endregion
    }
}
