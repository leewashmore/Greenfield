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
using GreenField.DataContracts;


namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelCreateUpdatePresentations : NotificationObject
    {

        #region Fields
        public IRegionManager _regionManager { private get; set; }
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        #endregion

        #region Constructor
        public ViewModelCreateUpdatePresentations(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;
        }

        #endregion       
        
        #region Properties
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

        #region Upload Document Type
        /// <summary>
        /// Stores the list of upload document type
        /// </summary>
        private List<String> _uploadDocumentInfo;
        public List<String> UploadDocumentInfo
        {
            get
            {
                if (_uploadDocumentInfo == null)
                {
                    _uploadDocumentInfo = new List<string> 
                    {
                        UploadDocumentType.POWERPOINT_PRESENTATION, 
                        UploadDocumentType.FINSTAT_REPORT, 
                        UploadDocumentType.INVESTMENT_CONTEXT_REPORT, 
                        UploadDocumentType.DCF_MODEL, 
                        UploadDocumentType.ADDITIONAL_ATTACHMENT 
                    };
                }
                return _uploadDocumentInfo;
            }
        }

        /// <summary>
        /// Stores selected upload document type
        /// </summary>
        private String _selectedUploadDocumentInfo = UploadDocumentType.POWERPOINT_PRESENTATION;
        public String SelectedUploadDocumentInfo
        {
            get { return _selectedUploadDocumentInfo; }
            set
            {
                _selectedUploadDocumentInfo = value;
                UploadFileData = null;
                UploadFileStreamData = null;
                SelectedUploadFileName = null;
                RaisePropertyChanged(() => this.SelectedUploadDocumentInfo);
            }
        } 
        #endregion

        /// <summary>
        /// Stores fileName of the browsed file
        /// </summary>
        private String _selectedUploadFileName;
        public String SelectedUploadFileName
        {
            get { return _selectedUploadFileName; }
            set
            {
                _selectedUploadFileName = value;                
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
        /// Stores information concerning selected presentation
        /// </summary>
        private ICPresentationOverviewData _selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return _selectedPresentationOverviewInfo; }
            set
            {
                _selectedPresentationOverviewInfo = value;
            }
        }

        /// <summary>
        /// Stores documentation information concerning the selected presentation
        /// </summary>
        private List<FileMaster> _selectedPresentationDocumentationInfo;
        public List<FileMaster> SelectedPresentationDocumentationInfo
        {
            get { return _selectedPresentationDocumentationInfo; }
            set
            {
                _selectedPresentationDocumentationInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationDocumentationInfo);
                RaisePropertyChanged(() => this.SubmitCommand);
            }
        }

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
            get { return new DelegateCommand<object>(UploadCommandMethod, UploadCommandMethodValidationMethod); }
        }

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
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

        #endregion       

        #region ICommand Methods

        private void DeleteAttachedFileCommandMethod(object param)
        {
            if (param is FileMaster)
            {
                DeleteFileData = param as FileMaster;
                Prompt.ShowDialog(messageText: "This action will permanently delete attachment from system. Do you wish to continue?", buttonType: MessageBoxButton.OKCancel, messageBoxResult: (result) =>
                {
                    if (result == MessageBoxResult.OK)
                    {
                        if (_dbInteractivity != null)
                        {
                            BusyIndicatorNotification(true, "Deleting document...");
                            _dbInteractivity.UpdatePresentationAttachedFileStreamData(UserSession.SessionManager.SESSION.UserName
                                , SelectedPresentationOverviewInfo.PresentationID, DeleteFileData, true, UpdatePresentationAttachedFileStreamDataCallbackMethod);
                        }                       
                    }
                });     
            }
        }

        private Boolean UploadCommandMethodValidationMethod(object param)
        {
            return UploadFileStreamData != null && UploadFileData != null;
        }

        /// <summary>
        /// Icommand method for Upload Initialization
        /// </summary>
        /// <param name="param"></param>
        private void UploadCommandMethod(object param)
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Uploading document");
                String deleteUrl = String.Empty;
                FileMaster overwriteFileMaster = SelectedPresentationDocumentationInfo.Where(record => record.Category == SelectedUploadDocumentInfo)
                    .FirstOrDefault();
                if(overwriteFileMaster != null)
                {
                    deleteUrl = overwriteFileMaster.Location;
                }
                _dbInteractivity.UploadDocument(UploadFileData.Name, UploadFileStreamData
                    , deleteUrl, UploadDocumentCallbackMethod);
            }
        }

        private Boolean SubmitCommandValidationMethod(object param)
        {
            if (SelectedPresentationDocumentationInfo == null)
                return false;
            return SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.POWERPOINT_PRESENTATION).Count() == 1
                && SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.INVESTMENT_CONTEXT_REPORT).Count() == 1
                && SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.FINSTAT_REPORT).Count() == 1
                && SelectedPresentationDocumentationInfo.Where(record => record.Category == UploadDocumentType.DCF_MODEL).Count() == 1;
        }

        private void SubmitCommandMethod(object param)
        {
            Prompt.ShowDialog("Please ensure that all changes have been made before finalizing meeting presentations", "", MessageBoxButton.OKCancel, (result) =>
            {
                if (result == MessageBoxResult.OK)
                {
                    SelectedPresentationOverviewInfo.StatusType = StatusType.READY_FOR_VOTING;
                    //update details

                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Updating selected presentation to status 'Ready for Voting'...");
                        _dbInteractivity.SetICPPresentationStatus(GreenField.UserSession.SessionManager.SESSION.UserName, SelectedPresentationOverviewInfo.PresentationID,
                                StatusType.READY_FOR_VOTING, SetICPPresentationStatusCallbackMethod);
                    }
                }
            });


        }

        #endregion

        #region Helper Methods

        public void RetrievePresentationAttachedDetails()
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving presentation attached file details...");
                _dbInteractivity.RetrievePresentationAttachedFileDetails(SelectedPresentationOverviewInfo.PresentationID, RetrievePresentationAttachedDetailsCallback); 
            }

        }

        public void Initialize()
        {
            UploadFileData = null;
            UploadFileStreamData = null;
            SelectedUploadFileName = null;
            ICPresentationOverviewData presentationInfo = ICNavigation.Fetch(ICNavigationInfo.PresentationOverviewInfo) as ICPresentationOverviewData;
            if (presentationInfo != null)
            {
                SelectedPresentationOverviewInfo = presentationInfo;
                if (_dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving updated upload documentation...");
                    _dbInteractivity.RetrievePresentationAttachedFileDetails(SelectedPresentationOverviewInfo.PresentationID, 
                        RetrievePresentationAttachedDetailsCallback);
                }
            }            
        }

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

        #endregion

        #region Callback Methods
        /// <summary>
        /// Assigns SelectedPresentationDocumentationInfo with documentation info related to SelectedPresentationOverviewInfo
        /// </summary>
        /// <param name="result">List of FileMaster information</param>
        private void RetrievePresentationAttachedDetailsCallback(List<FileMaster> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SelectedPresentationDocumentationInfo = result;
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
        /// Callback method for UploadDocument service call
        /// </summary>
        /// <param name="result">Server location url. Empty String if unsuccessful</param>
        private void UploadDocumentCallbackMethod(String result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (result != String.Empty)
                    {
                        UploadFileData.Location = result;
                        if (_dbInteractivity != null)
                        {
                            if (SelectedUploadDocumentInfo != UploadDocumentType.ADDITIONAL_ATTACHMENT)
                            {
                                UploadFileData.Name = SelectedUploadFileName;
                            }

                            BusyIndicatorNotification(true, "Uploading document");
                            _dbInteractivity.UpdatePresentationAttachedFileStreamData(UserSession.SessionManager.SESSION.UserName
                                , SelectedPresentationOverviewInfo.PresentationID, UploadFileData, false, UpdatePresentationAttachedFileStreamDataCallbackMethod);
                        }
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// UpdatePresentationAttachedFileStreamData Callback Method
        /// </summary>
        /// <param name="result">True if successful else false</param>
        private void UpdatePresentationAttachedFileStreamDataCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    Initialize();
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// UpdatePresentationAttachedFileStreamData Callback Method
        /// </summary>
        /// <param name="result">True if successful else false</param>
        private void SetICPPresentationStatusCallbackMethod(Boolean? result)
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

        private void UpdatePresentationAttachedFileStreamDataDeleteAttachedFileCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Initialize();
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
            Logging.LogEndMethod(_logger, methodNamespace);
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
