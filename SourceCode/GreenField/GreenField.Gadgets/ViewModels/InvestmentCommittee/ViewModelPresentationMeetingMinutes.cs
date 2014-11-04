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
using GreenField.DataContracts;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ViewPresentationMeetingMinutes
    /// </summary>
    public class ViewModelPresentationMeetingMinutes : NotificationObject
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

        #region UserInfo
        /// <summary>
        /// Stores application user info
        /// </summary>
        List<MembershipUserInfo> UserInfo { get; set; }
        #endregion

        #region Meeting Minutes Data
        /// <summary>
        /// True when meeting date is selected
        /// </summary>
        private Boolean isControlsEnabled = false;
        public Boolean IsControlsEnabled
        {
            get { return isControlsEnabled; }
            set
            {
                isControlsEnabled = value;
                RaisePropertyChanged(() => this.IsControlsEnabled);
            }
        }

        /// <summary>
        /// Reference data for attendance types
        /// </summary>
        public List<String> AttendanceTypeInfo
        {
            get { return new List<string> { "Attended", "Video Conference", "Tele Conference", "Not Present" }; }
        }

        /// <summary>
        ///Stores information for closed for voting meetings
        /// </summary>
        private List<MeetingInfo> closedForVotingMeetingInfo;
        public List<MeetingInfo> ClosedForVotingMeetingInfo
        {
            get { return closedForVotingMeetingInfo; }
            set
            {
                closedForVotingMeetingInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingInfo);
            }
        }

        /// <summary>
        /// Stores information for the selected closed for voting meeting
        /// </summary>
        private MeetingInfo selectedClosedForVotingMeetingInfo;
        public MeetingInfo SelectedClosedForVotingMeetingInfo
        {
            get { return selectedClosedForVotingMeetingInfo; }
            set
            {
                selectedClosedForVotingMeetingInfo = value;
                RaisePropertyChanged(() => this.SelectedClosedForVotingMeetingInfo);
                if (value != null && dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving Presentation and Attendance details for the selected meeting");
                    dbInteractivity.RetrieveMeetingMinuteDetails(value.MeetingID, RetrieveMeetingMinuteDetailsCallbackMethod);
                }
            }
        }

        /// <summary>
        /// Stores meeting minutes information for the selected closed for voting meeting
        /// </summary>
        private List<MeetingMinuteData> closedForVotingMeetingMinuteInfo;
        public List<MeetingMinuteData> ClosedForVotingMeetingMinuteInfo
        {
            get { return closedForVotingMeetingMinuteInfo; }
            set
            {
                closedForVotingMeetingMinuteInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingMinuteInfo);
                RaisePropertyChanged(() => this.AddAttendeeCommand);
                if (value == null)
                {
                    IsControlsEnabled = false;
                    ClosedForVotingMeetingMinuteDistinctPresentationInfo = null;
                    ClosedForVotingMeetingMinuteDistinctAttendanceInfo = null;
                }
                else
                {
                    IsControlsEnabled = true;
                    List<Int64> distinctPresentationIds = value.Select(record => record.PresentationID).ToList()
                        .Distinct().ToList();
                    List<MeetingMinuteData> distinctPresentationRecords = new List<MeetingMinuteData>();
                    foreach (Int64 presentationId in distinctPresentationIds)
                    {
                        distinctPresentationRecords.Add(
                            value.Where(record => record.PresentationID == presentationId).FirstOrDefault());
                    }

                    ClosedForVotingMeetingMinuteDistinctPresentationInfo = distinctPresentationRecords;

                    List<String> distinctVoterNames = value.Where(record => record.Name != null).Select(record => record.Name).ToList()
                        .Distinct().ToList();
                    List<MeetingMinuteData> distinctVoterRecords = new List<MeetingMinuteData>();
                    foreach (String name in distinctVoterNames)
                    {
                        distinctVoterRecords.Add(
                            value.Where(record => record.Name == name).FirstOrDefault());
                    }
                    ClosedForVotingMeetingMinuteDistinctAttendanceInfo = distinctVoterRecords;
                }
            }
        }

        /// <summary>
        /// Stores meeting minutes information for distict presentations in the selected closed for voting meeting
        /// </summary>
        private List<MeetingMinuteData> closedForVotingMeetingMinuteDistinctPresentationInfo;
        public List<MeetingMinuteData> ClosedForVotingMeetingMinuteDistinctPresentationInfo
        {
            get { return closedForVotingMeetingMinuteDistinctPresentationInfo; }
            set
            {
                closedForVotingMeetingMinuteDistinctPresentationInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingMinuteDistinctPresentationInfo);
            }
        }

        /// <summary>
        /// Stores meeting minutes information for distict attendance in the selected closed for voting meeting
        /// </summary>
        private List<MeetingMinuteData> closedForVotingMeetingMinuteDistinctAttendanceInfo;
        public List<MeetingMinuteData> ClosedForVotingMeetingMinuteDistinctAttendanceInfo
        {
            get { return closedForVotingMeetingMinuteDistinctAttendanceInfo; }
            set
            {
                closedForVotingMeetingMinuteDistinctAttendanceInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingMinuteDistinctAttendanceInfo);
            }
        }
        #endregion

        #region Upload Document
        /// <summary>
        /// Stores the list of upload document type
        /// </summary>
        private List<String> uploadDocumentInfo;
        public List<String> UploadDocumentInfo
        {
            get
            {
                if (uploadDocumentInfo == null)
                {
                    uploadDocumentInfo = new List<string> 
                    {
                        UploadDocumentType.INDUSTRY_REPORT, 
                        UploadDocumentType.OTHER_DOCUMENT
                    };
                }
                return uploadDocumentInfo;
            }
        }

        /// <summary>
        /// Stores selected upload document type
        /// </summary>
        private String selectedUploadDocumentInfo = UploadDocumentType.INDUSTRY_REPORT;
        public String SelectedUploadDocumentInfo
        {
            get { return selectedUploadDocumentInfo; }
            set
            {
                selectedUploadDocumentInfo = value;
                UploadFileData = null;
                UploadFileStreamData = null;
                SelectedUploadFileName = null;
                RaisePropertyChanged(() => this.SelectedUploadDocumentInfo);
            }
        }

        /// <summary>
        /// Stores fileName of the browsed file
        /// </summary>
        private String selectedUploadFileName;
        public String SelectedUploadFileName
        {
            get { return selectedUploadFileName; }
            set
            {
                selectedUploadFileName = value;
                RaisePropertyChanged(() => this.SelectedUploadFileName);
                RaisePropertyChanged(() => this.UploadCommand);
            }
        }

        /// <summary>
        /// Stores Filemaster object for the upload file
        /// </summary>
        public FileMaster UploadFileData { get; set; }

        /// <summary>
        /// Stores Filemaster object for the deletion file
        /// </summary>
        public FileMaster DeleteFileData { get; set; }

        /// <summary>
        /// Stores fileStream object for the upload file
        /// </summary>
        public Byte[] UploadFileStreamData { get; set; }

        /// <summary>
        /// Stores download stream posted by user for saving preview
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
                    BusyIndicatorNotification(true, "Downloading generated meeting minutes report...");
                 //   dbInteractivity.GenerateMeetingMinutesReport(SelectedClosedForVotingMeetingInfo.MeetingID
                   //     , GenerateMeetingMinutesReportCallbackMethod);
                }
            }
        }

        /// <summary>
        /// Stores documentation information concerning the selected presentation
        /// </summary>
        private List<FileMaster> selectedMeetingDocumentationInfo;
        public List<FileMaster> SelectedMeetingDocumentationInfo
        {
            get { return selectedMeetingDocumentationInfo; }
            set
            {
                selectedMeetingDocumentationInfo = value;
                RaisePropertyChanged(() => this.SelectedMeetingDocumentationInfo);
            }
        }
        #endregion

        #region ICommands
        /// <summary>
        /// Add Attendee Command
        /// </summary>
        public ICommand AddAttendeeCommand
        {
            get { return new DelegateCommand<object>(AddAttendeeCommandMethod, AddAttendeeCommandValidationMethod); }
        }

        /// <summary>
        /// Delete Attendee Command
        /// </summary>
        public ICommand DeleteAttendeeCommand
        {
            get { return new DelegateCommand<object>(DeleteAttendeeCommandMethod); }
        }

        /// <summary>
        /// Delete Attached File Command
        /// </summary>
        public ICommand DeleteAttachedFileCommand
        {
            get { return new DelegateCommand<object>(DeleteAttachedFileCommandMethod); }
        }

        /// <summary>
        /// Upload Command
        /// </summary>
        public ICommand UploadCommand
        {
            get { return new DelegateCommand<object>(UploadCommandMethod, UploadCommandValidationMethod); }
        }

        /// <summary>
        /// Save Command
        /// </summary>
        public ICommand SaveCommand
        {
            get { return new DelegateCommand<object>(SaveCommandMethod); }
        }

        /// <summary>
        /// Submit Command
        /// </summary>
        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod); }
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
        public ViewModelPresentationMeetingMinutes(DashboardGadgetParam param)
        {
            this.dbInteractivity = param.DBInteractivity;
            this.logger = param.LoggerFacade;
            this.eventAggregator = param.EventAggregator;
            this.regionManager = param.RegionManager;
        }
        #endregion        

        #region ICommand Methods
        #region AddAttendeeCommand
        /// <summary>
        /// AddAttendeeCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private Boolean AddAttendeeCommandValidationMethod(object param)
        {
            return ClosedForVotingMeetingMinuteInfo != null && UserInfo != null;
        }

        /// <summary>
        /// AddAttendeeCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void AddAttendeeCommandMethod(object param)
        {
            List<String> validUsers = UserInfo.OrderBy(record => record.UserName)
                .Select(record => record.UserName.ToLower()).ToList();

            ChildViewPresentationMeetingMinutesAddAttendee cvPresentationMeetingMinutesAddAttendee =
                new ChildViewPresentationMeetingMinutesAddAttendee(validUsers);

            cvPresentationMeetingMinutesAddAttendee.Show();
            cvPresentationMeetingMinutesAddAttendee.Unloaded += (se, e) =>
            {
                if (cvPresentationMeetingMinutesAddAttendee.DialogResult.Equals(true))
                {
                    if (ClosedForVotingMeetingMinuteInfo.Any(record => record.Name.ToLower() ==
                        cvPresentationMeetingMinutesAddAttendee.SelectedUser))
                    {
                        return;
                    }
                    List<MeetingMinuteData> closedForVotingMeetingMinuteInfo = new List<MeetingMinuteData>(ClosedForVotingMeetingMinuteInfo);

                    foreach (MeetingMinuteData item in ClosedForVotingMeetingMinuteDistinctPresentationInfo)
                    {
                        closedForVotingMeetingMinuteInfo.Add(new MeetingMinuteData()
                        {
                            PresentationID = item.PresentationID,
                            Presenter = item.Presenter,
                            SecurityName = item.SecurityName,
                            SecurityTicker = item.SecurityTicker,
                            SecurityCountry = item.SecurityCountry,
                            SecurityIndustry = item.SecurityIndustry,
                            Name = cvPresentationMeetingMinutesAddAttendee.SelectedUser,
                            AttendanceType = cvPresentationMeetingMinutesAddAttendee.SelectedAttendanceType
                        });
                    }
                    ClosedForVotingMeetingMinuteInfo = closedForVotingMeetingMinuteInfo;
                }
            };
        } 
        #endregion

        #region DeleteAttendeeCommand
        /// <summary>
        /// DeleteAttendeeCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void DeleteAttendeeCommandMethod(object param)
        {
            if (param is MeetingMinuteData)
            {
                MeetingMinuteData deletionAttendeeData = param as MeetingMinuteData;
                ClosedForVotingMeetingMinuteInfo = ClosedForVotingMeetingMinuteInfo
                    .Where(record => record.Name != deletionAttendeeData.Name).ToList();
            }
        } 
        #endregion

        #region DeleteAttachedFileCommand
        /// <summary>
        /// DeleteAttachedFileCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void DeleteAttachedFileCommandMethod(object param)
        {
            if (param is FileMaster)
            {
                DeleteFileData = param as FileMaster;
                Prompt.ShowDialog(messageText: "This action will permanently delete attachment from system. Do you wish to continue?"
                    , buttonType: MessageBoxButton.OKCancel, messageBoxResult: (result) =>
                {
                    if (result == MessageBoxResult.OK)
                    {
                        BusyIndicatorNotification(true, "Deleting selected document...");
                        dbInteractivity.UpdateMeetingAttachedFileStreamData(GreenField.UserSession.SessionManager.SESSION.UserName
                            , SelectedClosedForVotingMeetingInfo.MeetingID, DeleteFileData, true, UpdateMeetingAttachedFileStreamDataCallbackMethod);
                    }
                });
            }
        } 
        #endregion

        #region UploadCommand
        /// <summary>
        /// UploadCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private Boolean UploadCommandValidationMethod(object param)
        {
            return UploadFileStreamData != null && UploadFileData != null;
        }

        /// <summary>
        /// UploadCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void UploadCommandMethod(object param)
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Uploading document");
                String deleteUrl = String.Empty;
                FileMaster overwriteFileMaster = SelectedMeetingDocumentationInfo
                    .Where(record => record.Category == SelectedUploadDocumentInfo).FirstOrDefault();
                if (overwriteFileMaster != null)
                {
                    deleteUrl = overwriteFileMaster.Location;
                }
                dbInteractivity.UploadDocument(UploadFileData.Name, UploadFileStreamData, deleteUrl, UploadDocumentCallbackMethod);
            }
        } 
        #endregion

        #region SaveCommand
        /// <summary>
        /// SaveCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SaveCommandMethod(object param)
        {
            foreach (MeetingMinuteData meetingMinuteData in ClosedForVotingMeetingMinuteDistinctAttendanceInfo)
            {
                List<MeetingMinuteData> voterSpecificMeetingMinuteData = ClosedForVotingMeetingMinuteInfo
                    .Where(record => record.Name == meetingMinuteData.Name).ToList();
                foreach (MeetingMinuteData voterMeetingMinuteData in voterSpecificMeetingMinuteData)
                {
                    voterMeetingMinuteData.AttendanceType = meetingMinuteData.AttendanceType;
                }
            }
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Updating Meeting Minute Details");
                dbInteractivity.UpdateMeetingMinuteDetails(GreenField.UserSession.SessionManager.SESSION.UserName, SelectedClosedForVotingMeetingInfo,
                    ClosedForVotingMeetingMinuteInfo, UpdateMeetingMinuteDetailsCallbackMethod);
            }
        } 
        #endregion

        #region SubmitCommand
        /// <summary>
        /// SubmitCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SubmitCommandMethod(object param)
        {
            Prompt.ShowDialog("Please ensure that all changes have been made before finalizing meeting presentations"
                , "", MessageBoxButton.OKCancel, (result) =>
            {
                if (result == MessageBoxResult.OK)
                {
                    if (dbInteractivity != null)
                    {
                        dbInteractivity.SetMeetingPresentationStatus(GreenField.UserSession.SessionManager.SESSION.UserName
                            , SelectedClosedForVotingMeetingInfo.MeetingID, StatusType.FINAL, SetMeetingPresentationStatusCallbackMethod);
                    }
                }
            });
        } 
        #endregion
        #endregion

        #region CallBack Methods
        /// <summary>
        /// RetrieveMeetingInfoByPresentationStatus Callback Method
        /// </summary>
        /// <param name="result">List of MeetingInfo</param>
        private void RetrieveMeetingInfoByPresentationStatusCallbackMethod(List<MeetingInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    ClosedForVotingMeetingInfo = result;
                    if (UserInfo == null && dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving application user info...");
                        dbInteractivity.GetAllUsers(GetAllUsersCallbackMethod);
                    }
                    else
                    {
                        BusyIndicatorNotification();
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
        /// GetAllUsers Callback Method
        /// </summary>
        /// <param name="result">List of MembershipUserInfo</param>
        private void GetAllUsersCallbackMethod(List<MembershipUserInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    UserInfo = result;
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
        /// RetrieveMeetingMinuteDetails Callback Method
        /// </summary>
        /// <param name="result">List of MeetingMinuteData</param>
        private void RetrieveMeetingMinuteDetailsCallbackMethod(List<MeetingMinuteData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    ClosedForVotingMeetingMinuteInfo = result.Where(record => record.Name.ToLower() != record.Presenter.ToLower()).ToList();
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Attached document details for the selected meeting");
                        dbInteractivity.RetrieveMeetingAttachedFileDetails(SelectedClosedForVotingMeetingInfo.MeetingID, RetrieveMeetingAttachedFileDetailsCallbackMethod);
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
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// RetrieveMeetingAttachedFileDetails Callback Method
        /// </summary>
        /// <param name="result">List of FileMaster</param>
        private void RetrieveMeetingAttachedFileDetailsCallbackMethod(List<FileMaster> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SelectedMeetingDocumentationInfo = result;
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
        /// Callback method for UploadDocument service call
        /// </summary>
        /// <param name="result">Server location url. Empty String if unsuccessful</param>
        private void UploadDocumentCallbackMethod(String result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (result != String.Empty)
                    {
                        UploadFileData.Location = result;
                        if (dbInteractivity != null)
                        {
                            if (SelectedUploadDocumentInfo != UploadDocumentType.ADDITIONAL_ATTACHMENT)
                            {
                                UploadFileData.Name = SelectedUploadFileName;
                            }

                            BusyIndicatorNotification(true, "Uploading document");
                            dbInteractivity.UpdateMeetingAttachedFileStreamData(UserSession.SessionManager.SESSION.UserName
                                , SelectedClosedForVotingMeetingInfo.MeetingID, UploadFileData, false
                                , UpdateMeetingAttachedFileStreamDataCallbackMethod);
                        }
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
        /// UpdateMeetingAttachedFileStreamData Callback Method
        /// </summary>
        /// <param name="result">True if successfully executed</param>
        private void UpdateMeetingAttachedFileStreamDataCallbackMethod(Boolean? result)
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
                Initialize();
            }
        }

        /// <summary>
        /// UpdateMeetingMinuteDetails Callback Method
        /// </summary>
        /// <param name="result">True if successfully executed</param>
        private void UpdateMeetingMinuteDetailsCallbackMethod(Boolean? result)
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
                        Prompt.ShowDialog("Meeting minutes for the selected meeting has been successfully saved");
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while submitting meeting minute details");
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
        /// SetMeetingPresentationStatus Callback Method
        /// </summary>
        /// <param name="result">True if successfully executed</param>
        private void SetMeetingPresentationStatusCallbackMethod(Boolean? result)
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
                        Prompt.ShowDialog("Meeting has been successfully finalized");
                        Initialize();
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while submitting meeting finalization submission form.");
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
        /// GenerateMeetingMinutesReport Callback Method
        /// </summary>
        /// <param name="result">Meeting minutes preview file byte stream</param>
        private void GenerateMeetingMinutesReportCallbackMethod(Byte[] result)
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
        /// Initialize view
        /// </summary>
        public void Initialize()
        {
            if (IsActive)
            {
                ClosedForVotingMeetingInfo = null;
                SelectedClosedForVotingMeetingInfo = null;
                ClosedForVotingMeetingMinuteInfo = null;
                ClosedForVotingMeetingMinuteDistinctPresentationInfo = null;
                ClosedForVotingMeetingMinuteDistinctAttendanceInfo = null;
                UploadFileData = null;
                DeleteFileData = null;
                UploadFileStreamData = null;
                SelectedMeetingDocumentationInfo = null;
                SelectedUploadFileName = null;

                if (dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving Meetings with 'Closed for Voting' Investment Committee presentation status");
                    dbInteractivity.RetrieveMeetingInfoByPresentationStatus("Voting Closed"
                        , RetrieveMeetingInfoByPresentationStatusCallbackMethod);
                }
            }
        }

        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="isBusyIndicatorVisible">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        public void BusyIndicatorNotification(bool isBusyIndicatorVisible = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            IsBusyIndicatorBusy = isBusyIndicatorVisible;
        }

        /// <summary>
        /// Disposes event subscriptions
        /// </summary>
        public void Dispose()
        {
        }               
        #endregion
    }
}