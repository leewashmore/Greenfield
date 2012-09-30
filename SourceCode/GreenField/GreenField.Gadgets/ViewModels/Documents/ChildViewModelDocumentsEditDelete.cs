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
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.IO;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ChildViewModelDocumentsEditDelete : NotificationObject
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


        private IDBInteractivity _dbInteractivity;
        #endregion       

        #region Constructor
        public ChildViewModelDocumentsEditDelete(IDBInteractivity dBInteractivity, ILoggerFacade logger, List<String> companyInfo)
        {
            _dbInteractivity = dBInteractivity;
            _logger = logger;
            CompanyInfo = companyInfo;
            
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving user specific document information...");
                _dbInteractivity.RetrieveDocumentsDataForUser(UserSession.SessionManager.SESSION.UserName, RetrieveDocumentsDataForUserCallbackMethod);
            }

        }
        #endregion

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

        #region Document Search Box
        private List<DocumentCategoricalData> _documentCatagoricalInfo;
        public List<DocumentCategoricalData> DocumentCatagoricalInfo
        {
            get { return _documentCatagoricalInfo; }
            set
            {
                _documentCatagoricalInfo = value;
                RaisePropertyChanged(() => this.DocumentCatagoricalInfo);
                DocumentBindedCatagoricalInfo = value;
            }
        }

        private List<DocumentCategoricalData> _documentBindedCatagoricalInfo;
        public List<DocumentCategoricalData> DocumentBindedCatagoricalInfo
        {
            get { return _documentBindedCatagoricalInfo; }
            set
            {
                _documentBindedCatagoricalInfo = value;
                RaisePropertyChanged(() => this.DocumentBindedCatagoricalInfo);
            }
        }

        private DocumentCategoricalData _selectedDocumentCatagoricalInfo;
        public DocumentCategoricalData SelectedDocumentCatagoricalInfo
        {
            get { return _selectedDocumentCatagoricalInfo; }
            set
            {
                if (_selectedDocumentCatagoricalInfo != value)
                {
                    _selectedDocumentCatagoricalInfo = value;
                    RaisePropertyChanged(() => this.SelectedDocumentCatagoricalInfo);
                    if (value != null)
                    {
                        DocumentEditEnabled = true;
                        SelectedCategoryType = value.DocumentCategoryType;
                        SelectedCompanyInfo = CompanyInfo.Where(record => record == value.DocumentCompanyName).FirstOrDefault();
                        DocumentMetags = value.DocumentCatalogData.FileMetaTags;
                        DocumentNotes = null;
                        UploadStream = null;
                    }
                }
            }
        }

        private String _searchStringText;
        public String SearchStringText
        {
            get { return _searchStringText; }
            set
            {
                _searchStringText = value;
                RaisePropertyChanged(() => this.SearchStringText);
                if (value != null)
                {
                    if (value != String.Empty && DocumentCatagoricalInfo != null)
                        DocumentBindedCatagoricalInfo = DocumentCatagoricalInfo
                                    .Where(record => record.DocumentCatalogData.FileName.ToLower().Contains(value.ToLower()))
                                    .ToList();
                    else
                        DocumentBindedCatagoricalInfo = DocumentCatagoricalInfo;
                }
            }
        } 
        #endregion

        private List<DocumentCategoryType> _categoryType;
        public List<DocumentCategoryType> CategoryType
        {
            get 
            {
                if (_categoryType == null)
                {
                    _categoryType = EnumUtils.GetEnumDescriptions<DocumentCategoryType>();
                    _categoryType.Remove(DocumentCategoryType.IC_PRESENTATIONS);
                    _categoryType.Remove(DocumentCategoryType.MODELS);
                }
                return _categoryType;
            }
        }

        private DocumentCategoryType _selectedCategoryType;
        public DocumentCategoryType SelectedCategoryType
        {
            get { return _selectedCategoryType; }
            set
            {
                _selectedCategoryType = value;
                RaisePropertyChanged(() => this.SelectedCategoryType);
            }
        }
        
        private List<String> _companyInfo;
        public List<String> CompanyInfo
        {
            get { return _companyInfo; }
            set
            {
                _companyInfo = value;
                RaisePropertyChanged(() => this.CompanyInfo);
            }
        }

        private String _selectedCompanyInfo;
        public String SelectedCompanyInfo
        {
            get { return _selectedCompanyInfo; }
            set
            {
                _selectedCompanyInfo = value;
                RaisePropertyChanged(() => this.SelectedCompanyInfo);
            }
        }

        private List<String> _metaTagsInfo;
        public List<String> MetaTagsInfo
        {
            get { return _metaTagsInfo; }
            set
            {
                _metaTagsInfo = value;
                RaisePropertyChanged(() => this.MetaTagsInfo);
            }
        }

        private String _documentMetags;
        public String DocumentMetags
        {
            get { return _documentMetags; }
            set
            {
                _documentMetags = value;
                RaisePropertyChanged(() => this.DocumentMetags);
            }
        }

        private String _documentNotes;
        public String DocumentNotes
        {
            get { return _documentNotes; }
            set
            {
                _documentNotes = value;
                RaisePropertyChanged(() => this.DocumentNotes);                
            }
        }

        private Boolean _documentEditEnabled = false;
        public Boolean DocumentEditEnabled
        {
            get { return _documentEditEnabled; }
            set
            {
                _documentEditEnabled = value;
                RaisePropertyChanged(() => this.DocumentEditEnabled);
            }
        }

        private Byte[] _uploadStream;
        public Byte[] UploadStream
        {
            get { return _uploadStream; }
            set { _uploadStream = value; }
        }

        
        

        public ICommand DeleteCommand 
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod); }
        }        

        public ICommand SaveCommand
        {
            get { return new DelegateCommand<object>(SaveCommandMethod); }
        }
        #endregion

        #region ICommand Methods
        private void DeleteCommandMethod(object param)
        {
            
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Deleting document from repository...");
                _dbInteractivity.DeleteDocument(SelectedDocumentCatagoricalInfo.DocumentCatalogData.FilePath, DeleteDocumentCallbackMethod); 
            }
        }

        private void SaveCommandMethod(object param)
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Saving document chnages to repository...");
                _dbInteractivity.UpdateDocumentsDataForUser(Convert.ToInt64(SelectedDocumentCatagoricalInfo.DocumentCatalogData.FileId)
                    , UserSession.SessionManager.SESSION.UserName, DocumentMetags, SelectedCompanyInfo
                    , EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(SelectedCategoryType), DocumentNotes, UploadStream
                    , UpdateDocumentsDataForUserCallbackMethod);
            }
        }
        #endregion

        #region Callback Method
        private void UpdateDocumentsDataForUserCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (_dbInteractivity != null)
                    {
                        DocumentCatagoricalInfo = null;
                        DocumentEditEnabled = false;
                        SelectedCompanyInfo = null;
                        DocumentMetags = null;
                        DocumentNotes = null;
                        UploadStream = null;

                        BusyIndicatorNotification(true, "Retrieving user specific document information...");
                        _dbInteractivity.RetrieveDocumentsDataForUser(UserSession.SessionManager.SESSION.UserName, RetrieveDocumentsDataForUserCallbackMethod);
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

        private void DeleteDocumentCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Deleting document meta-data records...");
                        _dbInteractivity.DeleteFileMasterRecord(Convert.ToInt64(SelectedDocumentCatagoricalInfo.DocumentCatalogData.FileId)
                            , DeleteFileMasterRecordCallbackMethod);
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

        private void DeleteFileMasterRecordCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Updating user specific document information...");
                        _dbInteractivity.RetrieveDocumentsDataForUser(UserSession.SessionManager.SESSION.UserName, RetrieveDocumentsDataForUserCallbackMethod);
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

        private void RetrieveDocumentsDataForUserCallbackMethod(List<DocumentCategoricalData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    DocumentCatagoricalInfo = result.Where(record => record.DocumentCategoryType != DocumentCategoryType.IC_PRESENTATIONS
                        && record.DocumentCategoryType != DocumentCategoryType.MODELS).ToList();
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
        #endregion

        #region Helper Methods
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
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
