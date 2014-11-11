using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ViewPresentationVote
    /// </summary>
    public class ViewModelPresentationVote : NotificationObject
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

        #region Voting Screen
        /// <summary>
        /// Stores reference data for vote types
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
        /// Stores reference data for pfv measure types
        /// </summary>
        public List<String> PFVTypeInfo
        {
            get
            {
                return new List<string> 
                { 
                    PFVType.FORWARD_DIVIDEND_YIELD,
                    PFVType.FORWARD_EV_EBITDA,
                    //PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_COUNTRY,
                    //PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY,
                    //PFVType.FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY,
                    //PFVType.FORWARD_EV_REVENUE,
                    PFVType.FORWARD_P_NAV,
                    //PFVType.FORWARD_P_APPRAISAL_VALUE,
                    PFVType.FORWARD_P_BV,
                    //PFVType.FORWARD_P_BV_RELATIVE_TO_COUNTRY,
                    //PFVType.FORWARD_P_BV_RELATIVE_TO_INDUSTRY,
                    //PFVType.FORWARD_P_BV_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY,
                    PFVType.FORWARD_P_CE,
                    PFVType.FORWARD_P_E,
                    //PFVType.FORWARD_P_E_RELATIVE_TO_COUNTRY,
                    //PFVType.FORWARD_P_E_RELATIVE_TO_INDUSTRY,
                    //PFVType.FORWARD_P_E_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY,
                    //PFVType.FORWARD_P_E_TO_2_YEAR_EARNINGS_GROWTH,
                    //PFVType.FORWARD_P_E_TO_3_YEAR_EARNINGS_GROWTH,
                    //PFVType.FORWARD_P_EMBEDDED_VALUE,
                    //PFVType.FORWARD_P_REVENUE
                };
            }
        }

        /// <summary>
        /// Stores overview information for the selected presentation
        /// </summary>
        private ICPresentationOverviewData selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return selectedPresentationOverviewInfo; }
            set
            {
                selectedPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
                if (value != null && dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving documentation related to selected presentation");
                    dbInteractivity.RetrievePresentationAttachedFileDetails(value.PresentationID
                        , RetrievePresentationAttachedFileDetailsCallbackMethod);
                }
            }
        }

        /// <summary>
        /// Stores voter information for presenter and all voting members
        /// </summary>
        private List<VoterInfo> presentationMeetingVoterInfo;
        public List<VoterInfo> PresentationMeetingVoterInfo
        {
            get { return presentationMeetingVoterInfo; }
            set
            {
                presentationMeetingVoterInfo = value;
                RaisePropertyChanged(() => this.PresentationMeetingVoterInfo);
            }
        }

        /// <summary>
        /// Stores pre-meeting voting information for presenter and all voting members
        /// </summary>
        private List<VoterInfo> presentationPreMeetingVoterInfo;
        public List<VoterInfo> PresentationPreMeetingVoterInfo
        {
            get { return presentationPreMeetingVoterInfo; }
            set
            {
                presentationPreMeetingVoterInfo = value;
                RaisePropertyChanged(() => this.PresentationPreMeetingVoterInfo);
            }
        }

        /// <summary>
        /// Stores pre-meeting voting information for selected voting member
        /// </summary>
        private VoterInfo selectedPresentationPreMeetingVoterInfo;
        public VoterInfo SelectedPresentationPreMeetingVoterInfo
        {
            get { return selectedPresentationPreMeetingVoterInfo; }
            set
            {
                selectedPresentationPreMeetingVoterInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationPreMeetingVoterInfo);
                if (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
                    IsVoteEnabled = value != null;
            }
        }

        /// <summary>
        /// Stores commenting information for selected presentation
        /// </summary>
        private List<CommentInfo> selectedPresentationCommentInfo;
        public List<CommentInfo> SelectedPresentationCommentInfo
        {
            get { return selectedPresentationCommentInfo; }
            set
            {
                selectedPresentationCommentInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationCommentInfo);
            }
        }

        /// <summary>
        /// Stores file information on presentation's powerpoint document
        /// </summary>
        private FileMaster selectedPresentationPowerpointDocument;
        public FileMaster SelectedPresentationPowerpointDocument
        {
            get { return selectedPresentationPowerpointDocument; }
            set
            {
                selectedPresentationPowerpointDocument = value;
                RaisePropertyChanged(() => this.SelectedPresentationPowerpointDocument);
            }
        }

        /// <summary>
        /// Stores file information on presentation's ic packet document
        /// </summary>
        private FileMaster selectedPresentationICPacketDocument;
        public FileMaster SelectedPresentationICPacketDocument
        {
            get { return selectedPresentationICPacketDocument; }
            set
            {
                selectedPresentationICPacketDocument = value;
                RaisePropertyChanged(() => this.SelectedPresentationICPacketDocument);
            }
        }

        /// <summary>
        /// True if blogging section is enabled
        /// </summary>
        private Boolean isBlogEnabled = true;
        public Boolean IsBlogEnabled
        {
            get { return isBlogEnabled; }
            set
            {
                isBlogEnabled = value;
                RaisePropertyChanged(() => this.IsBlogEnabled);
            }
        }

        /// <summary>
        /// Stores upload comment information
        /// </summary>
        private String uploadCommentInfo;
        public String UploadCommentInfo
        {
            get { return uploadCommentInfo; }
            set
            {
                uploadCommentInfo = value;
                RaisePropertyChanged(() => this.UploadCommentInfo);
                RaisePropertyChanged(() => this.AddCommentCommand);
            }
        }

        /// <summary>
        /// Stores true if voter selection section is enabled
        /// </summary>
        private Boolean isVoterEnabled = true;
        public Boolean IsVoterEnabled
        {
            get { return isVoterEnabled; }
            set
            {
                isVoterEnabled = value;
                RaisePropertyChanged(() => this.IsVoterEnabled);
            }
        }

        /// <summary>
        /// Stores true if vote section is enabled
        /// </summary>
        private Boolean isVoteEnabled = true;
        public Boolean IsVoteEnabled
        {
            get { return isVoteEnabled; }
            set
            {
                isVoteEnabled = value;
                RaisePropertyChanged(() => this.IsVoteEnabled);
            }
        }

        /// <summary>
        /// Stores visibility of preview report button
        /// </summary>
        private Visibility previewReportVisibility = Visibility.Collapsed;
        public Visibility PreviewReportVisibility
        {
            get { return previewReportVisibility; }
            set
            {
                previewReportVisibility = value;
                RaisePropertyChanged(() => this.PreviewReportVisibility);
            }
        }

        /// <summary>
        /// Stores download stream for the pre meeting voting report preview
        /// </summary>
        private Stream downloadStream;
        public Stream DownloadStream
        {
            get { return downloadStream; }
            set
            {
                downloadStream = value;
                if (value != null && dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Downloading generated pre-meeting voting report...");
                    dbInteractivity.GeneratePreMeetingVotingReport(SelectedPresentationOverviewInfo.PresentationID
                        , GeneratePreMeetingVotingReportCallbackMethod);
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

        /// <summary>
        /// Add Comment Command
        /// </summary>
        public ICommand AddCommentCommand
        {
            get { return new DelegateCommand<object>(AddCommentCommandMethod, AddCommentCommandValidationMethod); }
        }

        /// <summary>
        /// Refresh Comment Command
        /// </summary>
        public ICommand RefreshCommentCommand
        {
            get { return new DelegateCommand<object>(RefreshCommentCommandMethod); }
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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelPresentationVote(DashboardGadgetParam param)
        {
            this.dbInteractivity = param.DBInteractivity;
            this.logger = param.LoggerFacade;
            this.eventAggregator = param.EventAggregator;
            this.regionManager = param.RegionManager;
        }
        #endregion        

        #region ICommand Methods
        #region SubmitCommand
        /// <summary>
        /// SubmitCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool SubmitCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null
                || PresentationPreMeetingVoterInfo == null)
            {
                return false;
            }
            if (SelectedPresentationOverviewInfo.StatusType != StatusType.READY_FOR_VOTING)
            {
                if (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
                {
                    return false;
                }
            }
            else
            {
                if (!(PresentationPreMeetingVoterInfo.Any(record => record.Name.ToLower() == UserSession.SessionManager.SESSION.UserName.ToLower()) ||
                    (SelectedPresentationOverviewInfo.Presenter.ToLower() == UserSession.SessionManager.SESSION.UserName.ToLower()) ||
                    UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// SubmitCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SubmitCommandMethod(object param)
        {
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
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Updating Pre-Meeting Voting Information");
                dbInteractivity.UpdatePreMeetingVoteDetails(UserSession.SessionManager.SESSION.UserName, PresentationMeetingVoterInfo
                    , UpdatePreMeetingVoteDetailsCallbackMethod);
            }
        } 
        #endregion

        #region AddCommentCommand
        /// <summary>
        /// AddCommentCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool AddCommentCommandValidationMethod(object param)
        {
            return UploadCommentInfo != null && UploadCommentInfo != String.Empty;
        }

        /// <summary>
        /// AddCommentCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void AddCommentCommandMethod(object param)
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving updated blog information related to selected presentation");
                dbInteractivity.SetPresentationComments(UserSession.SessionManager.SESSION.UserName
                    , SelectedPresentationOverviewInfo.PresentationID
                    , UploadCommentInfo, SetPresentationCommentsCallbackMethod);
            }
        } 
        #endregion

        #region RefreshComment
        /// <summary>
        /// RefreshComment execution method
        /// </summary>
        /// <param name="param"></param>
        private void RefreshCommentCommandMethod(object param)
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving updated blog information related to selected presentation");
                dbInteractivity.RetrievePresentationComments(SelectedPresentationOverviewInfo.PresentationID
                    , RetrievePresentationCommentsCallbackMethod);
            }
        } 
        #endregion
        #endregion

        #region CallBack Methods
        /// <summary>
        /// RetrievePresentationVoterData Callback Method
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
                    
                    PresentationMeetingVoterInfo = result;
                    PresentationPreMeetingVoterInfo = result.Where(record => record.PostMeetingFlag == false).OrderBy(record => record.Name).ToList();

                    if (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
                    {
                        PresentationMeetingVoterInfo = PresentationMeetingVoterInfo
                            .Where(record => record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower()).ToList();
                        PresentationPreMeetingVoterInfo = PresentationPreMeetingVoterInfo
                            .Where(record => record.Name.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower()).ToList();
                        IsVoterEnabled = true;
                        IsVoteEnabled = false;
                    }
                   
                    if (result.Any(record => record.Name.ToLower() == UserSession.SessionManager.SESSION.UserName))
                    {
                        SelectedPresentationPreMeetingVoterInfo = result
                            .Where(record => record.Name.ToLower() == UserSession.SessionManager.SESSION.UserName).FirstOrDefault();
                        IsVoterEnabled = false;
                    }
                    if (UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter)
                    {
                        IsVoterEnabled = false;
                        IsVoteEnabled = false;
                    }
                    if (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_CHIEF_EXECUTIVE) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_VOTING_MEMBER))
                    {
                        IsVoterEnabled = false;
                        IsVoteEnabled = false;
                    }
                    if (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_VOTING_MEMBER) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_NON_VOTING_MEMBER) &&
                        !UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_CHIEF_EXECUTIVE))
                    {
                        IsBlogEnabled = false;
                    }
                    if (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_NON_VOTING_MEMBER))
                    {
                        SelectedPresentationPreMeetingVoterInfo = result
                            .Where(record => record.Name.ToLower() == SelectedPresentationOverviewInfo.Presenter).FirstOrDefault();
                    }
                    if (!(UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN) ||
                        SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING))
                    {
                        IsVoterEnabled = false;
                        IsVoteEnabled = false;                        
                        IsBlogEnabled = false;
                    }
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

        /// <summary>
        /// RetrievePresentationAttachedFileDetails Callback Method
        /// </summary>
        /// <param name="result">List of FileMaster</param>
        private void RetrievePresentationAttachedFileDetailsCallbackMethod(List<FileMaster> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SelectedPresentationPowerpointDocument = result
                        .Where(record => record.Category == UploadDocumentType.POWERPOINT_PRESENTATION).FirstOrDefault();

                    SelectedPresentationICPacketDocument = result
                        .Where(record => record.Category == UploadDocumentType.IC_PACKET).FirstOrDefault();

                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving updated blog information related to selected presentation");
                        dbInteractivity.RetrievePresentationComments(SelectedPresentationOverviewInfo.PresentationID
                            , RetrievePresentationCommentsCallbackMethod);
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
        /// RetrievePresentationComments Callback Method
        /// </summary>
        /// <param name="result">List of CommentInfo</param>
        private void RetrievePresentationCommentsCallbackMethod(List<CommentInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SelectedPresentationCommentInfo = result;
                    if (SelectedPresentationOverviewInfo != null && dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Voting information for the selected presentation");
                        dbInteractivity.RetrievePresentationVoterData(SelectedPresentationOverviewInfo.PresentationID, RetrievePresentationVoterDataCallbackMethod);
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
        /// SetPresentationComments Callback Method
        /// </summary>
        /// <param name="result">List of CommentInfo</param>
        private void SetPresentationCommentsCallbackMethod(List<CommentInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SelectedPresentationCommentInfo = result;
                    UploadCommentInfo = null;
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
        /// UpdatePreMeetingVoteDetails Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        private void UpdatePreMeetingVoteDetailsCallbackMethod(Boolean? result)
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
                        Prompt.ShowDialog("Input submission successfully completed");
                        regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardICVoteDecision", UriKind.Relative));
                        
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while submitting input form.");
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
        /// GeneratePreMeetingVotingReport Callback Method
        /// </summary>
        /// <param name="result">Preview file byte stream</param>
        private void GeneratePreMeetingVotingReportCallbackMethod(Byte[] result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    DownloadStream.Write(result, 0, result.Length);
                    DownloadStream.Close();
                    DownloadStream = null;
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while downloading the preview report from server.");
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
        /// Initialize view
        /// </summary>
        public void Initialize()
        {
            if (IsActive)
            {
                SelectedPresentationOverviewInfo = ICNavigation.Fetch(ICNavigationInfo.PresentationOverviewInfo) as ICPresentationOverviewData;
            //    ViewPluginFlagEnumeration flag = (ViewPluginFlagEnumeration)ICNavigation.Fetch(ICNavigationInfo.ViewPluginFlagEnumerationInfo);

                if (UserSession.SessionManager.SESSION != null)
                {
                    if (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
                    {
                        PreviewReportVisibility = Visibility.Visible;
                    }
                    else
                    {
                        PreviewReportVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Raise vote type update
        /// </summary>
        /// <param name="vote">vote type</param>
        public void RaiseUpdateVoteType(String vote)
        {
            if (vote == null)
            {
                return;
            }
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

        /// <summary>
        /// Dispose event subscriptions
        /// </summary>
        public void Dispose()
        {
        }
        #endregion
    }
}
