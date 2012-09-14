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
    public class ViewModelPresentationDecisionEntry : NotificationObject
    {
        #region Fields
        private IRegionManager _regionManager;

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
                }
            }
        }

        #endregion

        #region Constructor
        public ViewModelPresentationDecisionEntry(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;
        }
        #endregion

        #region Properties
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

        private Boolean _iCDecisionIsEnable;
        public Boolean ICDecisionIsEnable
        {
            get { return _iCDecisionIsEnable; }
            set
            {
                _iCDecisionIsEnable = value;
                RaisePropertyChanged(() => this.ICDecisionIsEnable);
            }
        }

        private Dictionary<String, Decimal?> _securityPFVMeasureCurrentPrices;
        public Dictionary<String, Decimal?> SecurityPFVMeasureCurrentPrices
        {
            get
            {
                //if (_securityPFVMeasureCurrentPrices == null)
                //{
                //    _securityPFVMeasureCurrentPrices = new Dictionary<string, decimal?>();
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_DIVIDEND_YIELD, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_EV_EBITDA, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_COUNTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_EV_REVENUE, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_NAV, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_APPRAISAL_VALUE, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_BV, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_BV_RELATIVE_TO_COUNTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_BV_RELATIVE_TO_INDUSTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_BV_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_CE, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_E, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_E_RELATIVE_TO_COUNTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_E_RELATIVE_TO_INDUSTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_E_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_E_TO_2_YEAR_EARNINGS_GROWTH, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_E_TO_3_YEAR_EARNINGS_GROWTH, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_EMBEDDED_VALUE, null);
                //    _securityPFVMeasureCurrentPrices.Add(PFVType.FORWARD_P_REVENUE, null);

                //}
                return _securityPFVMeasureCurrentPrices;
            }
            set { _securityPFVMeasureCurrentPrices = value; }
        }

        Boolean SecurityIsHeld { get; set; }
                
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

        private Boolean? _acceptWithoutDiscussionIsChecked = true;
        public Boolean? AcceptWithoutDiscussionIsChecked
        {
            get { return _acceptWithoutDiscussionIsChecked; }
            set 
            {
                _acceptWithoutDiscussionIsChecked = value;
                RaisePropertyChanged(() => this.AcceptWithoutDiscussionIsChecked);
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
                if (value != null && _dbInteractivity != null)
                {
                    AcceptWithoutDiscussionIsChecked = value.AcceptWithoutDiscussionFlag;
                    ICDecisionIsEnable = !(Convert.ToBoolean(value.AcceptWithoutDiscussionFlag));
                    BusyIndicatorNotification(true, "Retrieving Voting information for the selected presentation");
                    _dbInteractivity.RetrievePresentationVoterData(value.PresentationID, RetrievePresentationVoterDataCallbackMethod);
                }
            }
        }

        private List<VoterInfo> _presentationVoterInfo;
        public List<VoterInfo> PresentationVoterInfo
        {
            get { return _presentationVoterInfo; }
            set { _presentationVoterInfo = value; }
        }

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

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod); }
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

        #region ICommand Methods
        private void SubmitCommandMethod(object param)
        {
            if (SelectedPresentationOverviewInfo.AcceptWithoutDiscussionFlag == false)
            {
                if (SecurityPFVMeasureCurrentPrices == null)
                {
                    Prompt.ShowDialog("Decision Entry form could not be submitted owing to unavailability of current P/FV Measure prices for the selected security");
                    return;
                }
                if (SecurityPFVMeasureCurrentPrices[SelectedPresentationOverviewInfo.CommitteePFVMeasure] == null)
                {
                    Prompt.ShowDialog("Decision Entry form could not be submitted owing to unavailability of current P/FV Measure price for the selected security and P/FV measure in IC Decision section");
                    return;
                }

                if (SelectedPresentationOverviewInfo.CommitteePFVMeasure == null
                    || SelectedPresentationOverviewInfo.CommitteeBuyRange == null
                    || SelectedPresentationOverviewInfo.CommitteeSellRange == null)
                {
                    Prompt.ShowDialog("'Modify' Vote input has not been supplemented with valid P/FV Measure, Buy and Sell Range for one or more voting members");
                    return;
                }
            }

            foreach (VoterInfo info in PresentationPostMeetingVoterInfo)
            {
                if (info.VoteType == VoteType.MODIFY)
                {
                    if (info.VoterPFVMeasure == null || info.VoterBuyRange == null || info.VoterSellRange == null)
                    {
                        Prompt.ShowDialog("'Modify' Vote input has not been supplemented with valid P/FV Measure, Buy and Sell Range for one or more voting members");
                        return;
                    }
                }
            }

            if (_dbInteractivity != null)
            {
                _dbInteractivity.UpdateDecisionEntryDetails(UserSession.SessionManager.SESSION.UserName, SelectedPresentationOverviewInfo
                    , PresentationPostMeetingVoterInfo, UpdateDecisionEntryDetailsCallbackMethod);
            }
        } 
        #endregion

        #region CallBack Methods
        private void RetrievePresentationVoterDataCallbackMethod(List<VoterInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    PresentationVoterInfo = ListUtils.GetDeepCopy<VoterInfo>(result);
                    PresentationPreMeetingVoterInfo = result
                        .Where(record => record.PostMeetingFlag == false && record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower())
                        .OrderBy(record => record.Name).ToList();
                    PresentationPostMeetingVoterInfo = result
                        .Where(record => record.PostMeetingFlag == true && record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower())
                        .OrderBy(record => record.Name).ToList();
                    SecurityIsHeld = SelectedPresentationOverviewInfo.CurrentHoldings.ToLower() == "yes";

                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieve current P/FV Measure values related to presented security...");
                        _dbInteractivity.RetrieveCurrentPFVMeasures(PFVTypeInfo, SelectedPresentationOverviewInfo.SecurityTicker
                            , RetrieveCurrentPFVMeasuresCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);                
            }
        }

        private void RetrieveCurrentPFVMeasuresCallbackMethod(Dictionary<String, Decimal?> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SecurityPFVMeasureCurrentPrices = result;
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

        private void UpdateDecisionEntryDetailsCallbackMethod(Boolean? result)
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
                        Prompt.ShowDialog("Decision Entry successfully completed");
                        _regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteePresentations");
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while submitting Decision Entry form.");
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

        #region Helper Methods
        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="showBusyIndicator">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        public void UpdateICDecisionAsPresented(Boolean iCDecisionIsEnable)
        {
            ICDecisionIsEnable = iCDecisionIsEnable;
            SelectedPresentationOverviewInfo.AcceptWithoutDiscussionFlag = !iCDecisionIsEnable;
            //if (SelectedPresentationOverviewInfo.AcceptWithoutDiscussionFlag == true)
            //{
            //    SelectedPresentationOverviewInfo.CommitteePFVMeasure = SelectedPresentationOverviewInfo.SecurityPFVMeasure;
            //    SelectedPresentationOverviewInfo.CommitteeBuyRange = SelectedPresentationOverviewInfo.SecurityBuyRange;
            //    SelectedPresentationOverviewInfo.CommitteeSellRange = SelectedPresentationOverviewInfo.SecuritySellRange;
            //    SelectedPresentationOverviewInfo.CommitteeRecommendation = SelectedPresentationOverviewInfo.SecurityRecommendation;
            //}

            if (!iCDecisionIsEnable)
            {
                SelectedPresentationOverviewInfo.CommitteePFVMeasure = SelectedPresentationOverviewInfo.SecurityPFVMeasure;
                SelectedPresentationOverviewInfo.CommitteeBuyRange = SelectedPresentationOverviewInfo.SecurityBuyRange;
                SelectedPresentationOverviewInfo.CommitteeSellRange = SelectedPresentationOverviewInfo.SecuritySellRange;
                SelectedPresentationOverviewInfo.CommitteeRecommendation = SelectedPresentationOverviewInfo.SecurityRecommendation;
            }
            RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
        }

        public void UpdateICDecisionRecommendation(String selectedPFVMeasure, Decimal buyRange, Decimal sellRange)
        {
            if (selectedPFVMeasure == null)
                return;

            Decimal? CurrentPFVMeasurePrice = SecurityPFVMeasureCurrentPrices[selectedPFVMeasure];

            if (CurrentPFVMeasurePrice == null)
            {
                Prompt.ShowDialog("Current P/FV measure value is not available to evaluate recommendation");
                return;
            }
            SelectedPresentationOverviewInfo.CommitteeBuyRange = Convert.ToSingle(buyRange);
            SelectedPresentationOverviewInfo.CommitteeSellRange = Convert.ToSingle(sellRange);


            Decimal lowerLimit = buyRange <= sellRange ? buyRange : sellRange;
            Decimal upperLimit = buyRange > sellRange ? buyRange : sellRange;

            if (SecurityIsHeld)
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

        public void Initialize()
        {
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
