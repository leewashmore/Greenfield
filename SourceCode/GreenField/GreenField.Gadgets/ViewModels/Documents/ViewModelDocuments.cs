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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using GreenField.Gadgets.Views.Documents;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelDocuments : NotificationObject
    {
        #region Fields
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        private ChildViewDocumentsUpload _uploadWindow;

        
        #endregion       

        #region Constructor
        public ViewModelDocuments(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            DbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
        }
        #endregion

        void _uploadWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_uploadWindow.DialogResult == true)
            {
                if (DbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Uploading Document...");
                    DbInteractivity.UploadDocument(_uploadWindow.UploadFileName, _uploadWindow.UploadFileByteStream,
                        String.Empty, UploadDocumentCallbackMethod);
                }
            }
        }

        

        #region Properties
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

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        public IDBInteractivity DbInteractivity { get; set; }

        #region Is Active Implementation
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
                    if (_uploadWindow == null)
                    {
                        _uploadWindow = new ChildViewDocumentsUpload(DbInteractivity, _logger);
                        _uploadWindow.Unloaded += new RoutedEventHandler(_uploadWindow_Unloaded);
                    }

                    if (DbInteractivity != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving document meta-tag information...");
                        DbInteractivity.GetDocumentsMetaTags(GetDocumentsMetaTagsCallBack);
                    }                    
                }
            }
        }
        #endregion

        #region Documents Data
        private List<DocumentCategoricalData> _documentCategoricalInfo;
        public List<DocumentCategoricalData> DocumentCategoricalInfo
        {
            get { return _documentCategoricalInfo; }
            set
            {
                if (_documentCategoricalInfo != value)
                {
                    _documentCategoricalInfo = value;
                    ConstructDocumentSearchResultEvent(value);
                }
            }
        }
        #endregion

        #region Auto Complete Search Box
        /// <summary>
        /// Stores the list of metatags 
        /// </summary>
        private List<string> _searchStringInfo;
        public List<string> SearchStringInfo
        {
            get { return _searchStringInfo; }
            set
            {
                _searchStringInfo = value;
                RaisePropertyChanged(() => this.SearchStringInfo);
            }
        }

        /// <summary>
        /// Stores the list of metatags 
        /// </summary>
        private List<string> _metaTagsInfo;
        public List<string> MetaTagsInfo
        {
            get
            {
                if (_metaTagsInfo == null)
                    _metaTagsInfo = new List<string>();
                return _metaTagsInfo;
            }
            set
            {
                _metaTagsInfo = value;
                RaisePropertyChanged(() => this.MetaTagsInfo);
                SearchStringInfo = value;
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines SearchStringInfo based on the text entered
        /// </summary>
        private string _securitySearchText;
        public string SearchStringText
        {
            get { return _securitySearchText; }
            set
            {
                _securitySearchText = value;
                RaisePropertyChanged(() => this.SearchStringText);
                if (value != null)
                {
                    if (value != String.Empty && SearchStringInfo != null)
                        SearchStringInfo = MetaTagsInfo
                                    .Where(record => record.ToLower().Contains(value.ToLower()))
                                    .ToList();
                    else
                        SearchStringInfo = MetaTagsInfo;
                }
            }
        }
        #endregion

        #region ICommands
        public ICommand DocumentSearchCommand
        {
            get { return new DelegateCommand<object>(DocumentSearchCommandMethod); }
        }

        public ICommand DocumentUploadCommand
        {
            get { return new DelegateCommand<object>(DocumentUploadCommandMethod); }
        }

        public ICommand DocumentEditDeleteCommand
        {
            get { return new DelegateCommand<object>(DocumentEditDeleteCommandMethod); }
        }
        #endregion 
        #endregion

        #region ICommand Methods
        private void DocumentSearchCommandMethod(object param)
        {
            if (SearchStringText != null && DbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving Search Results...");
                DbInteractivity.RetrieveDocumentsData(SearchStringText, RetrieveDocumentsDataCallbackMethod);
            }
        }

        private void DocumentUploadCommandMethod(object param)
        {
            if (_uploadWindow == null)
            {
                _uploadWindow = new ChildViewDocumentsUpload(DbInteractivity, _logger);
            }
            _uploadWindow.Initialize();
            _uploadWindow.Show();
        }

        private void DocumentEditDeleteCommandMethod(object param)
        {
            ChildViewDocumentsEditDelete editDeleteWindow = new ChildViewDocumentsEditDelete(DbInteractivity, _logger);
            editDeleteWindow.Show();
        } 
        #endregion

        #region Callback Methods
        private void UploadDocumentCallbackMethod(String result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result != String.Empty)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (DbInteractivity != null)
                    {
                        DbInteractivity.SetUploadFileInfo(UserSession.SessionManager.SESSION.UserName, _uploadWindow.UploadFileName
                            , result, _uploadWindow.UploadFileCompanyInfo.Name, _uploadWindow.UploadFileCompanyInfo.Ticker
                            , EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(_uploadWindow.UploadFileType)
                            , _uploadWindow.UploadFileTags, _uploadWindow.UploadFileNotes, SetUploadFileInfoCallbackMethod);
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

        private void SetUploadFileInfoCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (SearchStringText != null && DbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Search Results...");
                        DbInteractivity.RetrieveDocumentsData(SearchStringText, RetrieveDocumentsDataCallbackMethod);
                    }
                    else
                    {
                        BusyIndicatorNotification();
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

        private void RetrieveDocumentsDataCallbackMethod(List<DocumentCategoricalData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    DocumentCategoricalInfo = result;
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

        private void GetDocumentsMetaTagsCallBack(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    MetaTagsInfo = result;
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

        public void SetDocumentCommentCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (SearchStringText != null && DbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Updating Search Results...");
                        DbInteractivity.RetrieveDocumentsData(SearchStringText, RetrieveDocumentsDataCallbackMethod);
                    }
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        public event ConstructDocumentSearchResultEventHandler ConstructDocumentSearchResultEvent;  

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _uploadWindow.Unloaded -= new RoutedEventHandler(_uploadWindow_Unloaded);
        }

        #endregion   
    }
}
