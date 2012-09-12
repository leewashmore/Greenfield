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
    public class ViewModelPresentationVote : NotificationObject
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
        #endregion

        #region Constructor
        public ViewModelPresentationVote(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;
        }
        #endregion

        #region Properties
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
                if (value)
                {
                    Initialize();
                }
            }
        }

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
                    BusyIndicatorNotification(true, "Retrieving documentation related to selected presentation");
                    _dbInteractivity.RetrievePresentationAttachedFileDetails(value.PresentationID
                        , RetrievePresentationAttachedFileDetailsCallbackMethod);
                }
            }
        }

        private List<VoterInfo> _presentationMeetingVoterInfo;
        public List<VoterInfo> PresentationMeetingVoterInfo
        {
            get { return _presentationMeetingVoterInfo; }
            set
            {
                _presentationMeetingVoterInfo = value;
                RaisePropertyChanged(() => this.PresentationMeetingVoterInfo);
            }
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

        private VoterInfo _selectedPresentationPreMeetingVoterInfo;
        public VoterInfo SelectedPresentationPreMeetingVoterInfo
        {
            get { return _selectedPresentationPreMeetingVoterInfo; }
            set
            {
                _selectedPresentationPreMeetingVoterInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationPreMeetingVoterInfo);
                VoteIsEnabled = value != null && UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN);
            }
        }

        private List<CommentInfo> _selectedPresentationCommentInfo;
        public List<CommentInfo> SelectedPresentationCommentInfo
        {
            get { return _selectedPresentationCommentInfo; }
            set
            {
                _selectedPresentationCommentInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationCommentInfo);
            }
        }

        private FileMaster _selectedPresentationPowerpointDocument;
        public FileMaster SelectedPresentationPowerpointDocument
        {
            get { return _selectedPresentationPowerpointDocument; }
            set
            {
                _selectedPresentationPowerpointDocument = value;
                RaisePropertyChanged(() => this.SelectedPresentationPowerpointDocument);
            }
        }

        private FileMaster _selectedPresentationICPacketDocument;
        public FileMaster SelectedPresentationICPacketDocument
        {
            get { return _selectedPresentationICPacketDocument; }
            set
            {
                _selectedPresentationICPacketDocument = value;
                RaisePropertyChanged(() => this.SelectedPresentationICPacketDocument);
            }
        }

        private Boolean _notesIsEnabled = false;
        public Boolean NotesIsEnabled
        {
            get { return _notesIsEnabled; }
            set
            {
                _notesIsEnabled = value;
                RaisePropertyChanged(() => this.NotesIsEnabled);
            }
        }

        private Boolean _blogIsEnabled = true;
        public Boolean BlogIsEnabled
        {
            get { return _blogIsEnabled; }
            set
            {
                _blogIsEnabled = value;
                RaisePropertyChanged(() => this.BlogIsEnabled);
            }
        }

        private String _uploadCommentInfo;
        public String UploadCommentInfo
        {
            get { return _uploadCommentInfo; }
            set
            {
                _uploadCommentInfo = value;
                RaisePropertyChanged(() => this.UploadCommentInfo);
                RaisePropertyChanged(() => this.AddCommentCommand);
            }
        }

        private Boolean _voterIsEnabled = true;
        public Boolean VoterIsEnabled
        {
            get { return _voterIsEnabled; }
            set
            {
                _voterIsEnabled = value;
                RaisePropertyChanged(() => this.VoterIsEnabled);
            }
        }

        private Boolean _voteIsEnabled = true;
        public Boolean VoteIsEnabled
        {
            get { return _voteIsEnabled; }
            set
            {
                _voteIsEnabled = value;
                RaisePropertyChanged(() => this.VoteIsEnabled);
            }
        }

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }

        public ICommand AddCommentCommand
        {
            get { return new DelegateCommand<object>(AddCommentCommandMethod, AddCommentCommandValidationMethod); }
        }

        public ICommand RefreshCommentCommand
        {
            get { return new DelegateCommand<object>(RefreshCommentCommandMethod); }
        }

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
        #endregion

        #region ICommand Methods
        private bool SubmitCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null
                || PresentationPreMeetingVoterInfo == null)
                return false;
            if (SelectedPresentationOverviewInfo.StatusType != StatusType.READY_FOR_VOTING)
            {
                if (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
                    return false;
            }
            else
            {
                if (!(PresentationPreMeetingVoterInfo.Any(record => record.Name == UserSession.SessionManager.SESSION.UserName) ||
                    (SelectedPresentationOverviewInfo.Presenter == UserSession.SessionManager.SESSION.UserName)))
                    return false;
            }
            return true;
        }

        private void SubmitCommandMethod(object param)
        {
            VoterInfo presenterVoterInfo = PresentationPreMeetingVoterInfo
                .Where(record => record.Name.ToLower() == SelectedPresentationOverviewInfo.Presenter).FirstOrDefault();

            foreach (VoterInfo info in PresentationMeetingVoterInfo)
            {
                if (info.VoteType == VoteType.MODIFY)
                {
                    if (info.VoterPFVMeasure == null || info.VoterBuyRange == null || info.VoterSellRange == null)
                    {
                        Prompt.ShowDialog("'Modify' Vote input has not been supplemented with valid P/FV Measure, Buy and Sell Range for one or more voting members");
                        return;
                    }
                }

                if (presenterVoterInfo != null)
                {
                    info.Notes = presenterVoterInfo.Notes;
                }


                if (info.Name.ToLower() == UserSession.SessionManager.SESSION.UserName && info.PostMeetingFlag == false)
                {
                    VoterInfo postMeetingVoterInfo = PresentationMeetingVoterInfo
                        .Where(record => record.Name.ToLower() == UserSession.SessionManager.SESSION.UserName && record.PostMeetingFlag == true)
                        .FirstOrDefault();

                    if (postMeetingVoterInfo != null)
                    {
                        postMeetingVoterInfo.VoteType = info.VoteType;
                        postMeetingVoterInfo.VoterPFVMeasure = info.VoterPFVMeasure;
                        postMeetingVoterInfo.VoterBuyRange = info.VoterBuyRange;
                        postMeetingVoterInfo.VoterSellRange = info.VoterSellRange;
                    }
                }
            }

            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Updating Pre-Meeting Voting Information");
                _dbInteractivity.UpdatePreMeetingVoteDetails(UserSession.SessionManager.SESSION.UserName, PresentationMeetingVoterInfo
                    , UpdatePreMeetingVoteDetailsCallbackMethod);
            }
        }

        private bool AddCommentCommandValidationMethod(object param)
        {
            return UploadCommentInfo != null && UploadCommentInfo != String.Empty;
        }

        private void AddCommentCommandMethod(object param)
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving updated blog information related to selected presentation");
                _dbInteractivity.SetPresentationComments(UserSession.SessionManager.SESSION.UserName, SelectedPresentationOverviewInfo.PresentationID
                    , UploadCommentInfo, SetPresentationCommentsCallbackMethod);
            }
        }

        private void RefreshCommentCommandMethod(object param)
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving updated blog information related to selected presentation");
                _dbInteractivity.RetrievePresentationComments(SelectedPresentationOverviewInfo.PresentationID
                    , RetrievePresentationCommentsCallbackMethod);
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
                    
                    PresentationMeetingVoterInfo = result;
                    PresentationPreMeetingVoterInfo = result.Where(record => record.PostMeetingFlag == false).OrderBy(record => record.Name).ToList();

                    if(UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
                    {
                        PresentationMeetingVoterInfo = PresentationMeetingVoterInfo
                            .Where(record => record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower()).ToList();
                        PresentationPreMeetingVoterInfo = PresentationPreMeetingVoterInfo
                            .Where(record => record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower()).ToList();
                        VoterIsEnabled = true;
                        VoteIsEnabled = false;
                    }
                    


                    if (result.Any(record => record.Name.ToLower() == UserSession.SessionManager.SESSION.UserName))
                    {
                        SelectedPresentationPreMeetingVoterInfo = result
                            .Where(record => record.Name.ToLower() == UserSession.SessionManager.SESSION.UserName).FirstOrDefault();
                        VoterIsEnabled = false;
                    }

                    if (UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter)
                    {
                        VoterIsEnabled = false;
                        VoteIsEnabled = false;
                        NotesIsEnabled = true;
                    }                    

                    if (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.VOTING_MEMBER))
                    {
                        VoterIsEnabled = false;
                        VoteIsEnabled = false;
                    }

                    if (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.VOTING_MEMBER) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.NON_VOTING_MEMBER) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_CHIEF_EXECUTIVE))
                    {
                        BlogIsEnabled = false;
                    }

                    if (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.NON_VOTING_MEMBER))
                    {
                        SelectedPresentationPreMeetingVoterInfo = result
                            .Where(record => record.Name.ToLower() == SelectedPresentationOverviewInfo.Presenter).FirstOrDefault();
                    }

                    if (!(UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN) ||
                        SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING))
                    {
                        VoterIsEnabled = false;
                        VoteIsEnabled = false;
                        NotesIsEnabled = false;
                        BlogIsEnabled = false;
                    }

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

        private void RetrievePresentationAttachedFileDetailsCallbackMethod(List<FileMaster> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SelectedPresentationPowerpointDocument = result
                        .Where(record => record.Category == UploadDocumentType.POWERPOINT_PRESENTATION).FirstOrDefault();

                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving updated blog information related to selected presentation");
                        _dbInteractivity.RetrievePresentationComments(SelectedPresentationOverviewInfo.PresentationID
                            , RetrievePresentationCommentsCallbackMethod);
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

        private void RetrievePresentationCommentsCallbackMethod(List<CommentInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SelectedPresentationCommentInfo = result;
                    if (SelectedPresentationOverviewInfo != null && _dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Voting information for the selected presentation");
                        _dbInteractivity.RetrievePresentationVoterData(SelectedPresentationOverviewInfo.PresentationID, RetrievePresentationVoterDataCallbackMethod);
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

        private void SetPresentationCommentsCallbackMethod(List<CommentInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SelectedPresentationCommentInfo = result;
                    UploadCommentInfo = null;
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

        private void UpdatePreMeetingVoteDetailsCallbackMethod(Boolean? result)
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
                        Prompt.ShowDialog("Input submission successfully completed");
                        _regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteePresentations");
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while submitting input form.");
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

        public void Initialize()
        {
            if (IsActive)
            {
                SelectedPresentationOverviewInfo = ICNavigation.Fetch(ICNavigationInfo.PresentationOverviewInfo) as ICPresentationOverviewData;
                ViewPluginFlagEnumeration flag = (ViewPluginFlagEnumeration)ICNavigation.Fetch(ICNavigationInfo.ViewPluginFlagEnumerationInfo);
            }
        }

        public void RaiseUpdateVoteType(String vote)
        {
            if (vote == null)
                return;

            if (SelectedPresentationPreMeetingVoterInfo != null && SelectedPresentationOverviewInfo != null)
            {
                if (vote == VoteType.AGREE)
                {
                    SelectedPresentationPreMeetingVoterInfo.VoterPFVMeasure = SelectedPresentationOverviewInfo.SecurityPFVMeasure;
                    SelectedPresentationPreMeetingVoterInfo.VoterBuyRange = SelectedPresentationOverviewInfo.SecurityBuyRange;
                    SelectedPresentationPreMeetingVoterInfo.VoterSellRange = SelectedPresentationOverviewInfo.SecuritySellRange;
                }
                else if (vote == VoteType.ABSTAIN)
                {
                    SelectedPresentationPreMeetingVoterInfo.VoterPFVMeasure = null;
                    SelectedPresentationPreMeetingVoterInfo.VoterBuyRange = null;
                    SelectedPresentationPreMeetingVoterInfo.VoterSellRange = null;
                }
            }

            RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
