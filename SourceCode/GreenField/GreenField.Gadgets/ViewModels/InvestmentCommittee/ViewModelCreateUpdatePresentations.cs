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
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ViewCreateUpdatePresentations
    /// </summary>
    public class ViewModelCreateUpdatePresentations : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Region Manager MEF instance
        /// </summary>
        private IRegionManager regionManager;

        /// <summary>
        /// Event Aggregator MEF instance
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Service caller instance
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Logging MEF instance
        /// </summary>
        private ILoggerFacade logger;        
        #endregion

        #region Properties
        #region IsActive
        /// <summary>
        /// IsActive implementation
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
        #endregion

        #region Upload/Delete Document
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
                        UploadDocumentType.POWERPOINT_PRESENTATION, 
                        UploadDocumentType.FINSTAT_REPORT, 
                        UploadDocumentType.INVESTMENT_CONTEXT_REPORT, 
                        UploadDocumentType.DCF_MODEL, 
                        UploadDocumentType.ADDITIONAL_ATTACHMENT 
                    };
                }
                return uploadDocumentInfo;
            }
        }

        /// <summary>
        /// Stores selected upload document type
        /// </summary>
        private String selectedUploadDocumentInfo = UploadDocumentType.POWERPOINT_PRESENTATION;
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
        /// Download file stream
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
                    BusyIndicatorNotification(true, "Downloading generated IC Packet...");
                    dbInteractivity.GenerateICPacketReport(SelectedPresentationOverviewInfo.PresentationID, GenerateICPacketReportCallbackMethod);
                }
            }
        }
        #endregion

        #region Presentation and related documentation details
        /// <summary>
        /// Stores information concerning selected presentation
        /// </summary>
        private ICPresentationOverviewData selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return selectedPresentationOverviewInfo; }
            set
            {
                selectedPresentationOverviewInfo = value;
            }
        }

        /// <summary>
        /// Stores documentation information concerning the selected presentation
        /// </summary>
        private List<FileMaster> selectedPresentationDocumentationInfo;
        public List<FileMaster> SelectedPresentationDocumentationInfo
        {
            get { return selectedPresentationDocumentationInfo; }
            set
            {
                selectedPresentationDocumentationInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationDocumentationInfo);
                RaisePropertyChanged(() => this.SubmitCommand);
            }
        } 
        #endregion

        #region ICommand
        /// <summary>
        /// DeleteAttachedFile ICommand
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
        /// Submit Command
        /// </summary>
        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
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
        public ViewModelCreateUpdatePresentations(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            regionManager = param.RegionManager;
        }
        #endregion               

        #region ICommand Methods
        /// <summary>
        /// DeleteAttachedFileCommand execution Method
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
                        if (dbInteractivity != null)
                        {
                            BusyIndicatorNotification(true, "Deleting document...");
                            dbInteractivity.UpdatePresentationAttachedFileStreamData(UserSession.SessionManager.SESSION.UserName
                                , SelectedPresentationOverviewInfo.PresentationID, DeleteFileData, true
                                , UpdatePresentationAttachedFileStreamDataCallbackMethod);
                        }                       
                    }
                });     
            }
        }

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
        /// Icommand method for Upload Initialization
        /// </summary>
        /// <param name="param"></param>
        private void UploadCommandMethod(object param)
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Uploading document");
                String deleteUrl = String.Empty;
                FileMaster overwriteFileMaster = SelectedPresentationDocumentationInfo.Where(record => record.Category == SelectedUploadDocumentInfo)
                    .FirstOrDefault();
                if(overwriteFileMaster != null)
                {
                    deleteUrl = overwriteFileMaster.Location;
                }
                dbInteractivity.UploadDocument(UploadFileData.Name, UploadFileStreamData
                    , deleteUrl, UploadDocumentCallbackMethod);
            }
        }

        /// <summary>
        /// SubmitCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private Boolean SubmitCommandValidationMethod(object param)
        {
            if (SelectedPresentationDocumentationInfo == null)
                return false;
            return SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.POWERPOINT_PRESENTATION).Count() == 1
                && SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.INVESTMENT_CONTEXT_REPORT).Count() == 1
                && SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.FINSTAT_REPORT).Count() == 1
                && SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.DCF_MODEL).Count() == 1;
        }

        /// <summary>
        /// SubmitCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SubmitCommandMethod(object param)
        {
            if (SelectedPresentationOverviewInfo.StatusType != StatusType.READY_FOR_VOTING
                && SelectedPresentationOverviewInfo.MeetingDateTime > DateTime.UtcNow)
            {
                Prompt.ShowDialog("Please ensure that all changes have been made before submitting meeting presentation for voting", ""
                    , MessageBoxButton.OKCancel, (result) =>
                {
                    if (result == MessageBoxResult.OK)
                    {
                        SelectedPresentationOverviewInfo.StatusType = StatusType.READY_FOR_VOTING;

                        //update details
                        if (dbInteractivity != null)
                        {
                            BusyIndicatorNotification(true, "Updating selected presentation to status 'Ready for Voting'...");
                            dbInteractivity.SetICPPresentationStatus(GreenField.UserSession.SessionManager.SESSION.UserName
                                , SelectedPresentationOverviewInfo.PresentationID,
                                    StatusType.READY_FOR_VOTING, SetICPPresentationStatusCallbackMethod);
                        }
                    }
                });
            }
            else
            {
                ChildViewReSubmitPresentation dialog = new ChildViewReSubmitPresentation();
                dialog.Show();
                dialog.Unloaded += (se, e) =>
                {
                    if (dialog.DialogResult == true)
                    {
                        if (SelectedPresentationOverviewInfo != null && dbInteractivity != null)
                        {
                            BusyIndicatorNotification(true, "Resubmitting presentation...");
                            dbInteractivity.ReSubmitPresentation(UserSession.SessionManager.SESSION.UserName,
                                SelectedPresentationOverviewInfo, dialog.IsAlertChecked,
                                ReSubmitPresentationCallbackMethod);
                        }
                    }
                };
            }
        }
        #endregion        

        #region Callback Methods
        /// <summary>
        /// Assigns SelectedPresentationDocumentationInfo with documentation info related to SelectedPresentationOverviewInfo
        /// </summary>
        /// <param name="result">List of FileMaster information</param>
        private void RetrievePresentationAttachedDetailsCallback(List<FileMaster> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SelectedPresentationDocumentationInfo = result;
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
                            dbInteractivity.UpdatePresentationAttachedFileStreamData(UserSession.SessionManager.SESSION.UserName
                                , SelectedPresentationOverviewInfo.PresentationID, UploadFileData, false
                                , UpdatePresentationAttachedFileStreamDataCallbackMethod);
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
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// UpdatePresentationAttachedFileStreamData Callback Method
        /// </summary>
        /// <param name="result">True if successful else false</param>
        private void UpdatePresentationAttachedFileStreamDataCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    Initialize();
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
        /// UpdatePresentationAttachedFileStreamData Callback Method
        /// </summary>
        /// <param name="result">True if successful else false</param>
        private void SetICPPresentationStatusCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (SelectedPresentationOverviewInfo != null && dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Resubmitting presentation...");
                        dbInteractivity.ReSubmitPresentation(UserSession.SessionManager.SESSION.UserName,
                            SelectedPresentationOverviewInfo, false, ReSubmitPresentationCallbackMethod);
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
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// ReSubmitPresentation callback method
        /// </summary>
        /// <param name="result">True/False/Null</param>
        private void ReSubmitPresentationCallbackMethod(Boolean? result)
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
                eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATIONS);
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteePresentations");
            }
        }

        /// <summary>
        /// GenerateICPacketReport callback method
        /// </summary>
        /// <param name="result">IC Packet file byte stream</param>
        private void GenerateICPacketReportCallbackMethod(Byte[] result)
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
        /// Calls RetrievePresentationAttachedFileDetails to fetch updated document data
        /// </summary>
        public void RetrievePresentationAttachedDetails()
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving presentation attached file details...");
                dbInteractivity.RetrievePresentationAttachedFileDetails(SelectedPresentationOverviewInfo.PresentationID, RetrievePresentationAttachedDetailsCallback);
            }
        }

        /// <summary>
        /// Initializes view
        /// </summary>
        public void Initialize()
        {
            UploadFileData = null;
            UploadFileStreamData = null;
            SelectedUploadFileName = null;
            ICPresentationOverviewData presentationInfo = ICNavigation.Fetch(ICNavigationInfo.PresentationOverviewInfo) as ICPresentationOverviewData;
            if (presentationInfo != null)
            {
                SelectedPresentationOverviewInfo = presentationInfo;
                if (dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving updated upload documentation...");
                    dbInteractivity.RetrievePresentationAttachedFileDetails(SelectedPresentationOverviewInfo.PresentationID,
                        RetrievePresentationAttachedDetailsCallback);
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
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
        }
        #endregion
    }
}