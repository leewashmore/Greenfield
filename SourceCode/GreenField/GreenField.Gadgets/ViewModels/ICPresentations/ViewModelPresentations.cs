using System;
using System.Collections.Generic;
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
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model class for ViewPresentations
    /// </summary>
    public class ViewModelPresentations : NotificationObject
    {
        #region Fields
        /// <summary>
        /// RegionManager MEF instance
        /// </summary>
        private IRegionManager regionManager;
       
        /// <summary>
        /// Event Aggregator MEF instance
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Service Caller MEF instance
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Logging MEF instance
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// Stores user preference of sending alert on change date
        /// </summary>
        private Boolean isChangeDateAlertSelected = false;

        /// <summary>
        /// Stores original presentation date received from change date window
        /// </summary>
        private DateTime? originalPresentationDate = null;

        /// <summary>
        /// Stores updated presentation date received from change date window
        /// </summary>
        private DateTime? updatedPresentationDate = null;
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

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool isbusyIndicatorBusy = false;
        public bool IsBusyIndicatorBusy
        {
            get { return isbusyIndicatorBusy; }
            set
            {
                isbusyIndicatorBusy = value;
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
        /// Stores presentation overview data
        /// </summary>
        private List<ICPresentationOverviewData> iCPresentationOverviewInfo;
        public List<ICPresentationOverviewData> ICPresentationOverviewInfo
        {
            get { return iCPresentationOverviewInfo; }
            set
            {
                iCPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.ICPresentationOverviewInfo);
            }
        }

        /// <summary>
        /// Stores selected presentation overview data
        /// </summary>
        private ICPresentationOverviewData selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return selectedPresentationOverviewInfo; }
            set
            {
                selectedPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
                ICNavigation.Update(ICNavigationInfo.PresentationOverviewInfo, value);
                SelectionRaisePropertyChanged();
            }
        }

        /// <summary>
        /// Stores applicable future meeting dates
        /// </summary>
        private List<MeetingInfo> meetingInfoDates;
        public List<MeetingInfo> MeetingInfoDates
        {
            get { return meetingInfoDates; }
            set
            {
                if (value != null)
                {
                    meetingInfoDates = value;
                    RaisePropertyChanged(() => this.MeetingInfoDates);
                    RaisePropertyChanged(() => this.NewCommand);
                }
            }
        }

        /// <summary>
        /// Stores selected meeting date
        /// </summary>
        private MeetingInfo selectedMeetingInfoDate;
        public MeetingInfo SelectedMeetingInfoDate
        {
            get { return selectedMeetingInfoDate; }
            set
            {
                selectedMeetingInfoDate = value;
                RaisePropertyChanged(() => this.SelectedMeetingInfoDate);
                RaisePropertyChanged(() => this.NewCommand);

                if (value != null)
                {
                    ICNavigation.Update(ICNavigationInfo.MeetingInfo, value);
                }
            }
        }
        #endregion

        #region ICommand Properties
        /// <summary>
        /// Change date command
        /// </summary>
        public ICommand ChangeDateCommand
        {
            get { return new DelegateCommand<object>(ChangeDateCommandMethod, ChangeDateCommandValidationMethod); }
        }

        /// <summary>
        /// Decision entry command
        /// </summary>
        public ICommand DecisionEntryCommand
        {
            get { return new DelegateCommand<object>(DecisionEntryCommandMethod, DecisionEntryCommandValidationMethod); }
        }

        /// <summary>
        /// Upload command
        /// </summary>
        public ICommand UploadCommand
        {
            get { return new DelegateCommand<object>(UploadCommandMethod, UploadCommandValidationMethod); }
        }

        /// <summary>
        /// Edit command
        /// </summary>
        public ICommand EditCommand
        {
            get { return new DelegateCommand<object>(EditCommandMethod, EditCommandValidationMethod); }
        }

        /// <summary>
        /// Withdraw command
        /// </summary>
        public ICommand WithdrawCommand
        {
            get { return new DelegateCommand<object>(WithdrawCommandMethod, WithdrawCommandValidationMethod); }
        }

        /// <summary>
        /// View command
        /// </summary>
        public ICommand ViewCommand
        {
            get { return new DelegateCommand<object>(ViewCommandMethod, ViewCommandValidationMethod); }
        }

        /// <summary>
        /// New command
        /// </summary>
        public ICommand NewCommand
        {
            get { return new DelegateCommand<object>(NewCommandMethod, NewCommandValidationMethod); }
        }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelPresentations(DashboardGadgetParam param)
        {
            this.dbInteractivity = param.DBInteractivity;
            this.logger = param.LoggerFacade;
            this.eventAggregator = param.EventAggregator;
            this.regionManager = param.RegionManager;
        }
        #endregion        

        #region ICommand Methods
        #region Change Date
        /// <summary>
        /// ChangeDateCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool ChangeDateCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
            {
                return false;
            }
            return (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN)
                || UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_CHIEF_EXECUTIVE))
                && SelectedPresentationOverviewInfo.StatusType != StatusType.WITHDRAWN
                && SelectedPresentationOverviewInfo.StatusType != StatusType.FINAL;
        }

        /// <summary>
        /// ChangeDateCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void ChangeDateCommandMethod(object param)
        {
            List<DateTime> proposedMeetingDates = MeetingInfoDates.Select(record => record.MeetingDateTime.ToLocalTime().Date).ToList();

            if (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_CHIEF_EXECUTIVE))
            {
                proposedMeetingDates = proposedMeetingDates.OrderBy(record => record).ToList();
                for (int index = 0; index < proposedMeetingDates.Count; index++)
                {
                    if (proposedMeetingDates[index] < Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).Date)
                    {
                        proposedMeetingDates.RemoveAt(index);
                        index--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            ChildViewPresentationDateChangeEdit childViewPresentationDateChangeEdit = new ChildViewPresentationDateChangeEdit(proposedMeetingDates
                , SelectedPresentationOverviewInfo.MeetingDateTime);
            childViewPresentationDateChangeEdit.Show();

            childViewPresentationDateChangeEdit.Unloaded += (se, e) =>
            {
                if (childViewPresentationDateChangeEdit.DialogResult == true)
                {
                    if (childViewPresentationDateChangeEdit.SelectedPresentationDateTime.Date
                        == Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).Date)
                    {
                        return;
                    }
                    Prompt.ShowDialog("Confirm presentation DateTime from " +
                        Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).ToLocalTime().ToString("MM-dd-yyyy h:mm tt") +
                        " to " + childViewPresentationDateChangeEdit.SelectedPresentationDateTime
                            .Add(Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).TimeOfDay).ToLocalTime()
                            .ToString("MM-dd-yyyy h:mm tt"),
                        "Change Presentation Date", MessageBoxButton.OKCancel, (result) =>
                    {
                        if (result == MessageBoxResult.OK)
                        {
                            MeetingInfo meetingInfo = MeetingInfoDates
                                .Where(record => record.MeetingDateTime.ToLocalTime().Date == 
                                    childViewPresentationDateChangeEdit.SelectedPresentationDateTime.Date)
                                .FirstOrDefault();

                            if (dbInteractivity != null)
                            {
                                isChangeDateAlertSelected = childViewPresentationDateChangeEdit.AlertNotification;
                                originalPresentationDate = Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime);
                                updatedPresentationDate = childViewPresentationDateChangeEdit.SelectedPresentationDateTime
                                    .Add(Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).TimeOfDay);

                                BusyIndicatorNotification(true, "Updating Presentation date for the selected presentation...");
                                dbInteractivity.UpdateMeetingPresentationDate(UserSession.SessionManager.SESSION.UserName
                                    , SelectedPresentationOverviewInfo.PresentationID
                                    , meetingInfo, UpdateMeetingPresentationDateCallbackMethod);
                            }
                        }
                    });
                }
            };
        }        
        #endregion 

        #region Decision Entry
        /// <summary>
        /// DecisionEntryCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool DecisionEntryCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
            {
                return false;
            }
            return UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN)
                && SelectedPresentationOverviewInfo.StatusType == StatusType.CLOSED_FOR_VOTING;
        }

        /// <summary>
        /// DecisionEntryCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void DecisionEntryCommandMethod(object param)
        {
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteeDecisionEntry");
        } 
        #endregion

        #region Upload
        /// <summary>
        /// UploadCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool UploadCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
            {
                return false;
            }
            bool isUserRoleValidated = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool isStatusValided = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS;
            return isUserRoleValidated && isStatusValided;
        }

        /// <summary>
        /// UploadCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void UploadCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Upload);
            eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_EDIT_PRESENTATION);
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeEditPresentations", UriKind.Relative));
        } 
        #endregion

        #region Edit
        /// <summary>
        /// EditCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool EditCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
            {
                return false;
            }
            bool isUserRoleValidated = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter
                && !(UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN));
            bool isStatusValided = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS
                || SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING;

            return isUserRoleValidated && isStatusValided;
        }

        /// <summary>
        /// EditCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void EditCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Edit);
            eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_EDIT_PRESENTATION);
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeEditPresentations", UriKind.Relative));
        } 
        #endregion

        #region Withdraw
        /// <summary>
        /// WithdrawCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool WithdrawCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
            {
                return false;
            }
            bool isUserRoleValidated = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool isStatusValided = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS
                || SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING;

            return isUserRoleValidated && isStatusValided;
        }

        /// <summary>
        /// WithdrawCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void WithdrawCommandMethod(object param)
        {
            Prompt.ShowDialog("Please confirm withdrawal of selected presentation", "Confirmation", MessageBoxButton.OKCancel, (result) =>
            {
                if (result == MessageBoxResult.OK)
                {
                    if (dbInteractivity != null)
                    {
                        dbInteractivity.SetICPPresentationStatus(UserSession.SessionManager.SESSION.UserName,
                                   SelectedPresentationOverviewInfo.PresentationID, StatusType.WITHDRAWN, SetICPPresentationStatusCallbackMethod);
                    }
                }
            });
        }    
        #endregion

        #region View
        /// <summary>
        /// ViewCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool ViewCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
            {
                return false;
            }
            return SelectedPresentationOverviewInfo.StatusType != StatusType.IN_PROGRESS
                && SelectedPresentationOverviewInfo.StatusType != StatusType.WITHDRAWN;
        }

        /// <summary>
        /// ViewCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void ViewCommandMethod(object param)
        {
            bool userRoleValidation = UserSession.SessionManager.SESSION.Roles.Contains("IC_MEMBER_VOTING");

            if (userRoleValidation && SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING)
            {
                ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Vote);
            }
            else
            {
                ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.View);
            }
            eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_VOTE);
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeVote", UriKind.Relative));
        } 
        #endregion

        #region New
        /// <summary>
        /// NewCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool NewCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null)
            {
                return false;
            }
            return SelectedMeetingInfoDate != null && (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN));
        }

        /// <summary>
        /// NewCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void NewCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Create);
            eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_NEW_PRESENTATION);
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeNew", UriKind.Relative));
        } 
        #endregion
        #endregion
        
        #region CallBack Methods
        /// <summary>
        /// RetrievePresentationOverviewData callback method
        /// </summary>
        /// <param name="result">List of ICPresentationOverviewData</param>
        private void RetrievePresentationOverviewDataCallbackMethod(List<ICPresentationOverviewData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    ICPresentationOverviewInfo = result;
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving available meeting dates...");
                        dbInteractivity.GetAvailablePresentationDates(GetAvailablePresentationDatesCallbackMethod); 
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
                //BusyIndicatorNotification();
            }            
        }

        /// <summary>
        /// GetAvailablePresentationDates callback method
        /// </summary>
        /// <param name="result">List of MeetingInfo</param>
        private void GetAvailablePresentationDatesCallbackMethod(List<MeetingInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    MeetingInfoDates = result;
                    SelectedMeetingInfoDate = result.OrderBy(record => record.MeetingDateTime).FirstOrDefault();                    
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
        /// UpdateMeetingPresentationDate callback method
        /// </summary>
        /// <param name="result">True/False/Null</param>
        private void UpdateMeetingPresentationDateCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (SelectedPresentationOverviewInfo != null && dbInteractivity != null && isChangeDateAlertSelected)
                    {
                        BusyIndicatorNotification(true, "Retrieving presentation associated users...");
                        dbInteractivity.RetrievePresentationVoterData(SelectedPresentationOverviewInfo.PresentationID, RetrievePresentationVoterDataCallbackMethod, true);
                    }
                    else
                    {
                        Initialize();
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while updating presentation date");
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
                Initialize();
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);                
            }
        }

        /// <summary>
        /// SetICPPresentationStatus callback method
        /// </summary>
        /// <param name="result">True/False/Null</param>
        private void SetICPPresentationStatusCallbackMethod(Boolean? result)
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
                        Prompt.ShowDialog("Presentation was successfully withdrawn");
                        Initialize();
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while updating presentation date");
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

                    List<String> userNames = result.Where(record => record.PostMeetingFlag == false).Select(record => record.Name).ToList();
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving user credentials...");
                        dbInteractivity.GetUsersByNames(userNames, GetUsersByNamesCallbackMethod);
                    }
                    else
                    {
                        Initialize();
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
                Initialize();
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);                
            }
        }

        /// <summary>
        /// GetUsersByNames callback method
        /// </summary>
        /// <param name="result">List of MembershipUserInfo</param>
        private void GetUsersByNamesCallbackMethod(List<MembershipUserInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);

                    MembershipUserInfo presenterCredentials = result.Where(record => record.UserName.ToLower() 
                        == SelectedPresentationOverviewInfo.Presenter.ToLower()).FirstOrDefault();
                    if (presenterCredentials == null)
                        throw new Exception("Presenter credentials could not be retrieved. Alert notification was not successful.");

                    String emailtTo = presenterCredentials.Email;

                    String emailCc = String.Join("|", result.Where(record => record.UserName.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower())
                        .Select(record => record.Email).ToArray());

                    String messageSubject = "Presentation Date Change Notification – " + SelectedPresentationOverviewInfo.SecurityName;
                    String messageBody = "The Investment Committee Admin has changed the presentation date for "
                        + SelectedPresentationOverviewInfo.SecurityName + " from "
                        + Convert.ToDateTime(originalPresentationDate).ToString("MMMM dd, yyyy") + " UTC to "
                        + Convert.ToDateTime(updatedPresentationDate).ToString("MMMM dd, yyyy") 
                        + " UTC*. Please contact the Investment Committee Admin with any questions or concerns.";

                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Processing alert notification...");
                        dbInteractivity.SetMessageInfo(emailtTo, emailCc, messageSubject, messageBody, null
                            , UserSession.SessionManager.SESSION.UserName, SetMessageInfoCallbackMethod);                        
                    }
                    else
                    {
                        Initialize();   
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
                Initialize();
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
            }
        }

        /// <summary>
        /// SetMessageInfo callback method
        /// </summary>
        /// <param name="result">True/False/Null</param>
        private void SetMessageInfoCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);                    
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
                Initialize();
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Initialize view
        /// </summary>
        public void Initialize()
        {
            SelectedMeetingInfoDate = null;
            SelectionRaisePropertyChanged();
            if (dbInteractivity != null && IsActive)
            {
                BusyIndicatorNotification(true, "Retrieving Presentation Overview Information...");
                dbInteractivity.RetrievePresentationOverviewData(RetrievePresentationOverviewDataCallbackMethod);
            }
        }

        /// <summary>
        /// Revalidates execution commands
        /// </summary>
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
