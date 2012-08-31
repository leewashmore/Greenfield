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
        public ViewModelCreateUpdatePresentations(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;

            
            //fetch presentation attached file info based on presentation id
           // RetrievePresentationAttachedDetails();
            //set PresentationAttachedFileInfo in callback
            //check enum for upload/edit
            //if upload set visibility to false or empty text block
            //if edit load the files in text blocks
            

        }

        #endregion       
        
        #region Properties

        private FileMaster _selectedPresentationPowerPoint;
        public FileMaster SelectedPresentationPowerPoint
        {
            get { return _selectedPresentationPowerPoint; }
            set
            {
                _selectedPresentationPowerPoint = value;
                RaisePropertyChanged(() => this.SelectedPresentationPowerPoint);
            }
        }

        private FileMaster _selectedPresentationFinStatReport;
        public FileMaster SelectedPresentationFinStatReport
        {
            get { return _selectedPresentationFinStatReport; }
            set
            {
                _selectedPresentationFinStatReport = value;
                RaisePropertyChanged(() => this.SelectedPresentationFinStatReport);
            }
        }

        private FileMaster _selectedPresentationInvestmentContext;
        public FileMaster SelectedPresentationInvestmentContext
        {
            get { return _selectedPresentationInvestmentContext; }
            set
            {
                _selectedPresentationInvestmentContext = value;
                RaisePropertyChanged(() => this.SelectedPresentationInvestmentContext);
            }
        }

        private FileMaster _selectedPresentationDCFReports;
        public FileMaster SelectedPresentationDCFReports
        {
            get { return _selectedPresentationDCFReports; }
            set
            {
                _selectedPresentationDCFReports = value;
                RaisePropertyChanged(() => this.SelectedPresentationDCFReports);
            }
        }

        public PresentationAttachedFileStreamData SelectedPowerPointFileStreamData { get; set; }
        public PresentationAttachedFileStreamData SelectedDeleteFileStreamData { get; set; }

        private ICPresentationOverviewData _selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return _selectedPresentationOverviewInfo; }
            set
            {
                _selectedPresentationOverviewInfo = value;
            }
        }

        private List<FileMaster> _selectedPresentationDocumentationInfo;
        public List<FileMaster> SelectedPresentationDocumentationInfo
        {
            get { return _selectedPresentationDocumentationInfo; }
            set
            {
                _selectedPresentationDocumentationInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationDocumentationInfo);
            }
        }

        #region ICommand

        public ICommand BrowsePowerpointCommand
        {
            get { return new DelegateCommand<object>(BrowsePowerPointCommandMethod, BrowsePowerPointCommandValidationMethod); }
        }

        public ICommand PowerPointHyperlinkCommand
        {
            get { return new DelegateCommand<object>(PowerPointHyperlinkCommandMethod); }
        }

        public ICommand DeleteAttachedFileCommand
        {
            get { return new DelegateCommand<object>(DeleteAttachedFileCommandMethod); }
        }

        public ICommand AddPowerPointCommand
        {
            get { return new DelegateCommand<object>(AddPowerPointCommandCommandMethod, AddPowerPointCommandCommandValidationMethod); }
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

        #endregion       

        #region ICommand Methods

        private Boolean BrowsePowerPointCommandValidationMethod(object param)
        {
            return SelectedPresentationPowerPoint != null && (SelectedPresentationPowerPoint.Name != string.Empty);
        }

        private void PowerPointHyperlinkCommandMethod(object param)
        {
            try
            {
                _dbInteractivity.RetrieveDocument(SelectedPresentationPowerPoint.Location, RetrieveDocumentCallback);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogLoginException(_logger, ex);
            }
        }

        private void BrowsePowerPointCommandMethod(object param)
        {
            //DB update ppt
            OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog() == true)
            {
                BusyIndicatorNotification(true, "Uploading file...");
                if (SelectedPresentationDocumentationInfo != null)
                {
                    if (SelectedPresentationDocumentationInfo
                                .Any(record => record.Name == dialog.File.Name))
                    {
                        Prompt.ShowDialog("File '" + dialog.File.Name + "' already exists as an attachment. Please change the name of the file and upload again.");
                        BusyIndicatorNotification();
                        return;
                    }
                }

                FileMaster presentationAttachedFileData
                    = new FileMaster()
                    {
                        Name = dialog.File.Name,
                        SecurityName = SelectedPresentationOverviewInfo.SecurityName,
                        SecurityTicker = SelectedPresentationOverviewInfo.SecurityTicker,
                        Type = EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(DocumentCategoryType.IC_PRESENTATIONS),
                        MetaTags = "Power Point Presentation;" + SelectedPresentationOverviewInfo.Presenter + SelectedPresentationOverviewInfo.MeetingDateTime
                    };

                FileStream fileStream = dialog.File.OpenRead();

                SelectedPowerPointFileStreamData
                    = new PresentationAttachedFileStreamData()
                    {
                        PresentationAttachedFileData = presentationAttachedFileData,
                        FileStream = ReadFully(fileStream)
                    };

                SelectedPresentationPowerPoint.Name = dialog.File.Name;
                //make a call to documnetworksaceoperations to get url
                _dbInteractivity.UploadDocument(SelectedPresentationPowerPoint.Name, SelectedPowerPointFileStreamData.FileStream, UploadPowerPointPresentationCallbackMethod);

                BusyIndicatorNotification();
            }
        }

        private void DeleteAttachedFileCommandMethod(object param)
        {
            if (param is PresentationAttachedFileStreamData)
            {
                SelectedDeleteFileStreamData = param as PresentationAttachedFileStreamData;
                Prompt.ShowDialog(messageText: "This action will permanently delete attachment from system. Do you wish to continue?", buttonType: MessageBoxButton.OKCancel, messageBoxResult: (result) =>
                {
                    if (result == MessageBoxResult.OK)
                    {
                        _dbInteractivity.UpdatePresentationAttachedFileStreamData(GreenField.UserSession.SessionManager.SESSION.UserName
                            , SelectedPresentationOverviewInfo.PresentationID, SelectedPowerPointFileStreamData.PresentationAttachedFileData.Location,
                            SelectedPowerPointFileStreamData, UpdatePresentationAttachedFileStreamDataDeleteAttachedFileCallbackMethod);
                    }
                });     
            }
        }

        private Boolean AddPowerPointCommandCommandValidationMethod(object param)
        {
            return SelectedPresentationPowerPoint != null && (SelectedPresentationPowerPoint.Name != string.Empty);
        }

        private void AddPowerPointCommandCommandMethod(object param)
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Uploading document");

                _dbInteractivity.UpdatePresentationAttachedFileStreamData(GreenField.UserSession.SessionManager.SESSION.UserName, SelectedPresentationOverviewInfo.PresentationID,
                    SelectedPowerPointFileStreamData.PresentationAttachedFileData.Location, SelectedPowerPointFileStreamData, UpdatePresentationAttachedFileStreamDataCallback);
            }
        }

        private void SubmitCommandMethod(object param)
        {
            Prompt.ShowDialog("Please ensure that all changes have been made before finalizing meeting presentations", "", MessageBoxButton.OKCancel, (result) =>
            {
                if (result == MessageBoxResult.OK)
                {
                    SelectedPresentationOverviewInfo.StatusType = StatusType.READY_FOR_VOTING;
                    //update details
                    
                    //if (_dbInteractivity != null)
                    //{
                    //    _dbInteractivity.SetMeetingPresentationStatus(GreenField.UserSession.SessionManager.SESSION.UserName, SelectedClosedForVotingMeetingInfo.MeetingID,
                    //            StatusType.READY_FOR_VOTING, SetMeetingPresentationStatusCallbackMethod);
                    //}
                }
            });


        }

        #endregion

        #region Helper Methods

        
        private Byte[] ReadFully(Stream input)
        {
            Byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public void RetrievePresentationAttachedDetails()
        {
            _dbInteractivity.RetrievePresentationAttachedFileDetails(SelectedPresentationOverviewInfo.PresentationID, RetrievePresentationAttachedDetailsCallback);

        }

        public void Initialize()
        {
            ICPresentationOverviewData presentationInfo = ICNavigation.Fetch(ICNavigationInfo.PresentationOverviewInfo) as ICPresentationOverviewData;
            if (presentationInfo != null)
            {
                SelectedPresentationOverviewInfo = presentationInfo;
                if (_dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving Documentation...");
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
                    SelectedPresentationPowerPoint = result.Where(record => record.MetaTags.Contains("Power Point Presentation")).FirstOrDefault();
                    SelectedPresentationFinStatReport = result.Where(record => record.MetaTags.Contains("Power Point Presentation")).FirstOrDefault();
                    SelectedPresentationInvestmentContext = result.Where(record => record.MetaTags.Contains("Power Point Presentation")).FirstOrDefault();
                    SelectedPresentationDCFReports = result.Where(record => record.MetaTags.Contains("Power Point Presentation")).FirstOrDefault();
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

        private void RetrieveDocumentCallback(Byte[] result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SelectedPowerPointFileStreamData.FileStream = result;

                    //open file for edit
                    //file to be saved locally and then opened for edit
                    //what happens when user edits and closes file, save? where? or call upload method?

                    BusyIndicatorNotification();
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

        private void UploadPowerPointPresentationCallbackMethod(string result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (result != null)
                    {  
                        //set the url
                        SelectedPowerPointFileStreamData.PresentationAttachedFileData.Location = result;

                        //if (_dbInteractivity != null)
                        //{
                        //    BusyIndicatorNotification(true, "Uploading document");
                            
                        //    _dbInteractivity.UpdatePresentationAttachedFileStreamData(SelectedPresentationOverviewInfo.Presenter, SelectedPresentationOverviewInfo.PresentationID,
                        //        SelectedPowerPointFileStreamData.PresentationAttachedFileData.Location, SelectedPowerPointFileStreamData, UpdatePresentationAttachedFileStreamDataCallback);                           
                        //}
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

        private void UpdatePresentationAttachedFileStreamDataCallback(Boolean? result)
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
                        SelectedPresentationPowerPoint = null;
                        SelectedPowerPointFileStreamData = null;
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

        private void UpdatePresentationAttachedFileStreamDataDeleteAttachedFileCallbackMethod(Boolean? result)
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
                        SelectedPresentationDocumentationInfo = SelectedPresentationDocumentationInfo
                            .Where(record => record != SelectedDeleteFileStreamData.PresentationAttachedFileData).ToList();
                        SelectedDeleteFileStreamData = null;
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
