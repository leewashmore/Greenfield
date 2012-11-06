using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Views.Documents;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ViewDocuments
    /// </summary>
    public class ViewModelDocuments : NotificationObject
    {
        #region Fields
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// ChildViewDocumentsUpload instance
        /// </summary>
        private ChildViewDocumentsUpload uploadWindow;        
        #endregion       

        #region Properties
        #region Events
        public event ConstructDocumentSearchResultEventHandler ConstructDocumentSearchResultEvent;  
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

        #region Service caller
        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        public IDBInteractivity DbInteractivity { get; set; } 
        #endregion

        #region Is Active Implementation
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
                    if (DbInteractivity != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving document meta-tag information...");
                        DbInteractivity.GetDocumentsMetaTags(GetDocumentsMetaTagsCallBackMethod);
                    }
                }
            }
        }
        #endregion

        #region Documents Data
        /// <summary>
        /// Stores document categorical data returned from search query
        /// </summary>
        private List<DocumentCategoricalData> documentCategoricalInfo;
        public List<DocumentCategoricalData> DocumentCategoricalInfo
        {
            get { return documentCategoricalInfo; }
            set
            {
                if (documentCategoricalInfo != value)
                {
                    documentCategoricalInfo = value;
                    ConstructDocumentSearchResultEvent(value);
                }
            }
        }
        #endregion

        #region Reference data
        /// <summary>
        /// Stores reference company information
        /// </summary>
        public List<String> CompanyInfo { get; set; }

        /// <summary>
        /// Stores reference user information
        /// </summary>
        public List<MembershipUserInfo> UserInfo { get; set; } 
        #endregion

        #region Auto Complete Search Box
        /// <summary>
        /// Stores the list of metatags 
        /// </summary>
        private List<string> searchStringInfo;
        public List<string> SearchStringInfo
        {
            get { return searchStringInfo; }
            set
            {
                searchStringInfo = value;
                RaisePropertyChanged(() => this.SearchStringInfo);
            }
        }

        /// <summary>
        /// Stores the list of metatags 
        /// </summary>
        private List<string> metaTagsInfo;
        public List<string> MetaTagsInfo
        {
            get
            {
                if (metaTagsInfo == null)
                {
                    metaTagsInfo = new List<string>();
                }
                return metaTagsInfo;
            }
            set
            {
                metaTagsInfo = value;
                RaisePropertyChanged(() => this.MetaTagsInfo);
                SearchStringInfo = value;
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines SearchStringInfo based on the text entered
        /// </summary>
        private string securitySearchText;
        public string SearchStringText
        {
            get { return securitySearchText; }
            set
            {
                securitySearchText = value;
                RaisePropertyChanged(() => this.SearchStringText);
                if (value != null)
                {
                    if (value != String.Empty && SearchStringInfo != null)
                    {
                        SearchStringInfo = MetaTagsInfo
                                    .Where(record => record.ToLower().Contains(value.ToLower()))
                                    .ToList();
                    }
                    else
                    {
                        SearchStringInfo = MetaTagsInfo;
                    }
                }
            }
        }
        #endregion

        #region ICommands
        /// <summary>
        /// Document Search Command
        /// </summary>
        public ICommand DocumentSearchCommand
        {
            get { return new DelegateCommand<object>(DocumentSearchCommandMethod); }
        }

        /// <summary>
        /// Document Upload Command
        /// </summary>
        public ICommand DocumentUploadCommand
        {
            get { return new DelegateCommand<object>(DocumentUploadCommandMethod); }
        }

        /// <summary>
        /// Document Edit Delete Command
        /// </summary>
        public ICommand DocumentEditDeleteCommand
        {
            get { return new DelegateCommand<object>(DocumentEditDeleteCommandMethod); }
        }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelDocuments(DashboardGadgetParam param)
        {
            this.eventAggregator = param.EventAggregator;
            this.DbInteractivity = param.DBInteractivity;
            this.logger = param.LoggerFacade;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// uploadWindow Unloaded EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        void uploadWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (uploadWindow.DialogResult == true)
            {
                if (DbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Uploading Document...");
                    DbInteractivity.UploadDocument(uploadWindow.UploadFileName, uploadWindow.UploadFileByteStream,
                        String.Empty, UploadDocumentCallbackMethod);
                }
            }
        }         
        #endregion        

        #region ICommand Methods
        /// <summary>
        /// DocumentSearchCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void DocumentSearchCommandMethod(object param)
        {
            if (SearchStringText != null && DbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving Search Results...");
                DbInteractivity.RetrieveDocumentsData(SearchStringText, RetrieveDocumentsDataCallbackMethod);
            }
        }

        /// <summary>
        /// DocumentUploadCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void DocumentUploadCommandMethod(object param)
        {
            if (uploadWindow == null)
            {
                uploadWindow = new ChildViewDocumentsUpload(DbInteractivity, logger, CompanyInfo, UserInfo);
            }
            uploadWindow.Show();
        }

        /// <summary>
        /// DocumentEditDeleteCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void DocumentEditDeleteCommandMethod(object param)
        {
            ChildViewDocumentsEditDelete editDeleteWindow = new ChildViewDocumentsEditDelete(DbInteractivity, logger, CompanyInfo);
            editDeleteWindow.Show();
            editDeleteWindow.Unloaded += (se, e) =>
            {
                if (SearchStringText != null && DbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving Search Results...");
                    DbInteractivity.RetrieveDocumentsData(SearchStringText, RetrieveDocumentsDataCallbackMethod);
                }
            };
        } 
        #endregion

        #region Callback Methods
        /// <summary>
        /// UploadDocument Callback Method
        /// </summary>
        /// <param name="result">usl of uploaded document</param>
        private void UploadDocumentCallbackMethod(String result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result != String.Empty)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (DbInteractivity != null)
                    {
                        DbInteractivity.SetUploadFileInfo(UserSession.SessionManager.SESSION.UserName, uploadWindow.UploadFileName
                            , result, uploadWindow.UploadFileCompanyInfo, null, null
                            , EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(uploadWindow.UploadFileType)
                            , uploadWindow.UploadFileTags, uploadWindow.UploadFileNotes, SetUploadFileInfoCallbackMethod);
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
        /// SetUploadFileInfo Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        private void SetUploadFileInfoCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (uploadWindow.UserAlertEmails != null && uploadWindow.UserAlertEmails.Count != 0)
                    {
                        BusyIndicatorNotification(true, "Updation messaging queue...");
                        String emailTo = String.Join("|", uploadWindow.UserAlertEmails.ToArray());
                        String emailSubject = "Document Upload Alert";
                        String emailMessageBody = "Document upload notification. Please find the details below:\n"
                            + "Name - " + uploadWindow.UploadFileName + "\n"
                            + "Company - " + uploadWindow.UploadFileCompanyInfo + "\n"
                            + "Type - " + EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(uploadWindow.UploadFileType) + "\n"
                            + "Tags - " + uploadWindow.UploadFileTags + "\n"
                            + "Notes - " + uploadWindow.UploadFileNotes;

                        DbInteractivity.SetMessageInfo(emailTo, null, emailSubject, emailMessageBody, null
                            , UserSession.SessionManager.SESSION.UserName, SetMessageInfoCallbackMethod);
                    }
                    else if (SearchStringText != null && DbInteractivity != null)
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
        /// SetMessageInfo Callback Method for comments
        /// </summary>
        /// <param name="result">True if successful</param>
        public void SetMessageInfoCallbackMethod_Comment(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
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
            }
        }

        /// <summary>
        /// SetMessageInfo Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        private void SetMessageInfoCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
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
        /// RetrieveDocumentsData Callback Method
        /// </summary>
        /// <param name="result">DocumentCategoricalData</param>
        private void RetrieveDocumentsDataCallbackMethod(List<DocumentCategoricalData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    DocumentCategoricalInfo = result;
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
        /// GetDocumentsMetaTags CallBackMethod
        /// </summary>
        /// <param name="result">Metatags</param>
        private void GetDocumentsMetaTagsCallBackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    MetaTagsInfo = result;
                    if (DbInteractivity != null && (CompanyInfo == null || UserInfo == null))
                    {
                        BusyIndicatorNotification(true, "Retrieving Company Information...");
                        DbInteractivity.RetrieveCompanyData(RetrieveCompanyDataCallbackMethod);
                    }
                    else
                    {
                        if (uploadWindow == null)
                        {
                            uploadWindow = new ChildViewDocumentsUpload(DbInteractivity, logger, CompanyInfo, UserInfo);
                            uploadWindow.Unloaded += new RoutedEventHandler(uploadWindow_Unloaded);
                            BusyIndicatorNotification();
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
        /// RetrieveCompanyData Callback Method
        /// </summary>
        /// <param name="result">list of issuers</param>
        private void RetrieveCompanyDataCallbackMethod(List<String> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CompanyInfo = result;
                    if (DbInteractivity != null && UserInfo == null)
                    {
                        BusyIndicatorNotification(true, "Retrieving User Information...");
                        DbInteractivity.GetAllUsers(GetAllUsersCallbackMethod);
                    }
                    else
                    {
                        if (uploadWindow == null)
                        {
                            uploadWindow = new ChildViewDocumentsUpload(DbInteractivity, logger, CompanyInfo, UserInfo);
                            uploadWindow.Unloaded += new RoutedEventHandler(uploadWindow_Unloaded);
                            BusyIndicatorNotification();
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
        /// GetAllUsers Callback Method
        /// </summary>
        /// <param name="result">MembershipUserInfo</param>
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
                    if (uploadWindow == null)
                    {
                        uploadWindow = new ChildViewDocumentsUpload(DbInteractivity, logger, CompanyInfo, UserInfo);
                        uploadWindow.Unloaded += new RoutedEventHandler(uploadWindow_Unloaded);                        
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
        /// SetDocumentComment Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        public void SetDocumentCommentCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (SearchStringText != null && DbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Updating Search Results...");
                        DbInteractivity.RetrieveDocumentsData(SearchStringText, RetrieveDocumentsDataCallbackMethod);
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
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Helper Methods
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
        #endregion        

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            uploadWindow.Unloaded -= new RoutedEventHandler(uploadWindow_Unloaded);
        }
        #endregion   
    }
}
