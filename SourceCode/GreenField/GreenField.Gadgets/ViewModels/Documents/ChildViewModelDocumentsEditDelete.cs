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
        public ChildViewModelDocumentsEditDelete(IDBInteractivity dBInteractivity, ILoggerFacade logger)
        {
            _dbInteractivity = dBInteractivity;
            _logger = logger;
            
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
                        SelectedCompanyInfo = CompanyInfo.Where(record => record.Name == value.DocumentCompanyName
                            && record.Ticker == value.DocumentSecurityTicker).FirstOrDefault();
                        DocumentMetags = value.DocumentCatalogData.FileMetaTags;                        
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

        public List<DocumentCategoryType> CategoryType
        {
            get { return EnumUtils.GetEnumDescriptions<DocumentCategoryType>(); }
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
        
        private List<tblCompanyInfo> _companyInfo;
        public List<tblCompanyInfo> CompanyInfo
        {
            get { return _companyInfo; }
            set
            {
                _companyInfo = value;
                RaisePropertyChanged(() => this.CompanyInfo);
            }
        }

        private tblCompanyInfo _selectedCompanyInfo;
        public tblCompanyInfo SelectedCompanyInfo
        {
            get { return _selectedCompanyInfo; }
            set
            {
                _selectedCompanyInfo = value;
                RaisePropertyChanged(() => this.SelectedCompanyInfo);
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

        public ICommand DeleteCommand 
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod); }
        }
        #endregion

        #region ICommand Methods
        private void DeleteCommandMethod(object param)
        {
            BusyIndicatorNotification(true, "Deleting document from repository...");
            _dbInteractivity.DeleteDocument(SelectedDocumentCatagoricalInfo.DocumentCatalogData.FilePath, DeleteDocumentCallbackMethod);
        }
        #endregion


        #region Callback Method
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

                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving available company information...");
                        _dbInteractivity.RetrieveCompanyData(RetrieveCompanyDataCallbackMethod);
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

        private void RetrieveCompanyDataCallbackMethod(List<tblCompanyInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CompanyInfo = result;
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
