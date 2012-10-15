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
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ChildViewDocumentsEditDelete
    /// </summary>
    public class ChildViewModelDocumentsEditDelete : NotificationObject
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
        /// Service caller instance
        /// </summary>
        private IDBInteractivity dbInteractivity;
        #endregion

        #region Properties
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

        #region Document Search Box
        /// <summary>
        /// Stores document catagorical information of files uploaded by logged in user
        /// </summary>
        private List<DocumentCategoricalData> documentCatagoricalInfo;
        public List<DocumentCategoricalData> DocumentCatagoricalInfo
        {
            get { return documentCatagoricalInfo; }
            set
            {
                documentCatagoricalInfo = value;
                RaisePropertyChanged(() => this.DocumentCatagoricalInfo);
                DocumentBindedCatagoricalInfo = value;
            }
        }

        /// <summary>
        /// Stores document catagorical information bided to user interface
        /// </summary>
        private List<DocumentCategoricalData> documentBindedCatagoricalInfo;
        public List<DocumentCategoricalData> DocumentBindedCatagoricalInfo
        {
            get { return documentBindedCatagoricalInfo; }
            set
            {
                documentBindedCatagoricalInfo = value;
                RaisePropertyChanged(() => this.DocumentBindedCatagoricalInfo);
            }
        }

        /// <summary>
        /// Stores selected document information
        /// </summary>
        private DocumentCategoricalData selectedDocumentCatagoricalInfo;
        public DocumentCategoricalData SelectedDocumentCatagoricalInfo
        {
            get { return selectedDocumentCatagoricalInfo; }
            set
            {
                if (selectedDocumentCatagoricalInfo != value)
                {
                    selectedDocumentCatagoricalInfo = value;
                    RaisePropertyChanged(() => this.SelectedDocumentCatagoricalInfo);
                    if (value != null)
                    {
                        IsDocumentEditEnabled = true;
                        SelectedCategoryType = value.DocumentCategoryType;
                        SelectedCompanyInfo = CompanyInfo.Where(record => record == value.DocumentCompanyName).FirstOrDefault();
                        DocumentMetags = value.DocumentCatalogData.FileMetaTags;
                        DocumentNotes = null;
                        UploadStream = null;
                    }
                }
            }
        }

        /// <summary>
        /// Stores search string text
        /// </summary>
        private String searchStringText;
        public String SearchStringText
        {
            get { return searchStringText; }
            set
            {
                searchStringText = value;
                RaisePropertyChanged(() => this.SearchStringText);
                if (value != null)
                {
                    if (value != String.Empty && DocumentCatagoricalInfo != null)
                    {
                        DocumentBindedCatagoricalInfo = DocumentCatagoricalInfo
                            .Where(record => record.DocumentCatalogData.FileName.ToLower().Contains(value.ToLower()))
                            .ToList();
                    }
                    else
                    {
                        DocumentBindedCatagoricalInfo = DocumentCatagoricalInfo;
                    }
                }
            }
        }
        #endregion

        #region Document related information
        #region Category
        /// <summary>
        /// Categorical reference data
        /// </summary>
        private List<DocumentCategoryType> categoryType;
        public List<DocumentCategoryType> CategoryType
        {
            get
            {
                if (categoryType == null)
                {
                    categoryType = EnumUtils.GetEnumDescriptions<DocumentCategoryType>();
                    categoryType.Remove(DocumentCategoryType.IC_PRESENTATIONS);
                    categoryType.Remove(DocumentCategoryType.MODELS);
                    categoryType.Remove(DocumentCategoryType.BLOG);
                }
                return categoryType;
            }
        }

        /// <summary>
        /// Stores selected category type
        /// </summary>
        private DocumentCategoryType selectedCategoryType;
        public DocumentCategoryType SelectedCategoryType
        {
            get { return selectedCategoryType; }
            set
            {
                selectedCategoryType = value;
                RaisePropertyChanged(() => this.SelectedCategoryType);
            }
        } 
        #endregion

        #region Issuer
        /// <summary>
        /// Stores reference issuer information
        /// </summary>
        private List<String> companyInfo;
        public List<String> CompanyInfo
        {
            get { return companyInfo; }
            set
            {
                companyInfo = value;
                RaisePropertyChanged(() => this.CompanyInfo);
            }
        }

        /// <summary>
        /// Stores selected issuer information
        /// </summary>
        private String selectedCompanyInfo;
        public String SelectedCompanyInfo
        {
            get { return selectedCompanyInfo; }
            set
            {
                selectedCompanyInfo = value;
                RaisePropertyChanged(() => this.SelectedCompanyInfo);
            }
        } 
        #endregion

        #region Meta tags
        /// <summary>
        /// Stores documents metatag information
        /// </summary>
        private List<String> metaTagsInfo;
        public List<String> MetaTagsInfo
        {
            get { return metaTagsInfo; }
            set
            {
                metaTagsInfo = value;
                RaisePropertyChanged(() => this.MetaTagsInfo);
            }
        }

        /// <summary>
        /// Stores documents metatag
        /// </summary>
        private String documentMetags;
        public String DocumentMetags
        {
            get { return documentMetags; }
            set
            {
                documentMetags = value;
                RaisePropertyChanged(() => this.DocumentMetags);
            }
        } 
        #endregion

        #region Notes
        private String documentNotes;
        public String DocumentNotes
        {
            get { return documentNotes; }
            set
            {
                documentNotes = value;
                RaisePropertyChanged(() => this.DocumentNotes);
            }
        } 
        #endregion

        #region Overwrite document
        /// <summary>
        /// Stores true if document edit is enabled
        /// </summary>
        private Boolean isdocumentEditEnabled = false;
        public Boolean IsDocumentEditEnabled
        {
            get { return isdocumentEditEnabled; }
            set
            {
                isdocumentEditEnabled = value;
                RaisePropertyChanged(() => this.IsDocumentEditEnabled);
            }
        }

        /// <summary>
        /// Stores byte information of uploaded document
        /// </summary>
        private Byte[] uploadStream;
        public Byte[] UploadStream
        {
            get { return uploadStream; }
            set { uploadStream = value; }
        }

        /// <summary>
        /// Stores upload file name
        /// </summary>
        public String UploadFileName { get; set; }  
        #endregion
        #endregion

        #region ICommand
        /// <summary>
        /// Delete Command
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod); }
        }

        /// <summary>
        /// Save Command
        /// </summary>
        public ICommand SaveCommand
        {
            get { return new DelegateCommand<object>(SaveCommandMethod); }
        } 
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dBInteractivity">IDBInteractivity</param>
        /// <param name="logger">ILoggerFacade</param>
        /// <param name="companyInfo">Issuer list</param>
        public ChildViewModelDocumentsEditDelete(IDBInteractivity dBInteractivity, ILoggerFacade logger, List<String> companyInfo)
        {
            this.dbInteractivity = dBInteractivity;
            this.logger = logger;
            CompanyInfo = companyInfo;
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving user specific document information...");
                dbInteractivity.RetrieveDocumentsDataForUser(UserSession.SessionManager.SESSION.UserName, RetrieveDocumentsDataForUserCallbackMethod);
            }
        }
        #endregion        

        #region ICommand Methods
        /// <summary>
        /// DeleteCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void DeleteCommandMethod(object param)
        {
            Prompt.ShowDialog("Document will be permanently deleted from repository. Please confirm."
                , "Document Delete", MessageBoxButton.OKCancel, (result) =>
            {
                if (result == MessageBoxResult.OK)
                {
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Deleting document from repository...");
                        dbInteractivity.DeleteDocument(SelectedDocumentCatagoricalInfo.DocumentCatalogData.FilePath
                            , DeleteDocumentCallbackMethod);
                    }
                }
            });
        }

        /// <summary>
        /// SaveCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SaveCommandMethod(object param)
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Saving document chnages to repository...");
                dbInteractivity.UpdateDocumentsDataForUser(Convert.ToInt64(SelectedDocumentCatagoricalInfo.DocumentCatalogData.FileId)
                    , UploadFileName, UserSession.SessionManager.SESSION.UserName, DocumentMetags, SelectedCompanyInfo
                    , EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(SelectedCategoryType), DocumentNotes, UploadStream
                    , UpdateDocumentsDataForUserCallbackMethod);
            }
        }
        #endregion

        #region Callback Method
        /// <summary>
        /// UpdateDocumentsDataForUser Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        private void UpdateDocumentsDataForUserCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (dbInteractivity != null)
                    {
                        DocumentCatagoricalInfo = null;
                        IsDocumentEditEnabled = false;
                        SelectedCompanyInfo = null;
                        DocumentMetags = null;
                        DocumentNotes = null;
                        UploadStream = null;

                        BusyIndicatorNotification(true, "Retrieving user specific document information...");
                        dbInteractivity.RetrieveDocumentsDataForUser(UserSession.SessionManager.SESSION.UserName
                            , RetrieveDocumentsDataForUserCallbackMethod);
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
        /// DeleteDocument Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        private void DeleteDocumentCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Deleting document meta-data records...");
                        dbInteractivity.DeleteFileMasterRecord(Convert.ToInt64(SelectedDocumentCatagoricalInfo.DocumentCatalogData.FileId)
                            , DeleteFileMasterRecordCallbackMethod);
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
        /// DeleteFileMasterRecord Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        private void DeleteFileMasterRecordCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (dbInteractivity != null)
                    {
                        DocumentCatagoricalInfo = null;
                        IsDocumentEditEnabled = false;
                        SelectedCompanyInfo = null;
                        DocumentMetags = null;
                        DocumentNotes = null;
                        UploadStream = null;

                        BusyIndicatorNotification(true, "Updating user specific document information...");
                        dbInteractivity.RetrieveDocumentsDataForUser(UserSession.SessionManager.SESSION.UserName, RetrieveDocumentsDataForUserCallbackMethod);
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
        /// RetrieveDocumentsDataForUser Callback Method
        /// </summary>
        /// <param name="result">DocumentCategoricalData</param>
        private void RetrieveDocumentsDataForUserCallbackMethod(List<DocumentCategoricalData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    DocumentCatagoricalInfo = result.Where(record => record.DocumentCategoryType != DocumentCategoryType.IC_PRESENTATIONS
                        && record.DocumentCategoryType != DocumentCategoryType.MODELS && record.DocumentCategoryType != DocumentCategoryType.BLOG).ToList();
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