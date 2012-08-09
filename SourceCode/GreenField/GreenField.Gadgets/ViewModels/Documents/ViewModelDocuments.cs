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
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        private IManageDocuments _manageDocuments;

        private ChildViewDocumentsUpload _uploadWindow;
        #endregion       

        #region Constructor
        public ViewModelDocuments(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _manageDocuments = param.ManageDocuments;
            _uploadWindow = new ChildViewDocumentsUpload(_dbInteractivity, _logger);
            //param.ManageAlerts.SendAlert(new List<String> { "Rahul.Vig@headstrong.com" }, new List<String> { "Akshay.Mathur2@headstrong.com" }
            //    , "This is a test mail", "Test", (Boolean? result) =>
            //{

            //});            

            _uploadWindow.Unloaded += new RoutedEventHandler(_uploadWindow_Unloaded);
            
        }

        void _uploadWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _manageDocuments.UploadDocument(_uploadWindow.UploadFileName, _uploadWindow.UploadFileByteStream, (result) => { MessageBox.Show(result.ToString()); });
        }

        #endregion

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

        private String _searchString;
        public String SearchString
        {
            get { return _searchString; }
            set { _searchString = value; }
        }

        public event ConstructDocumentSearchResultEventHandler ConstructDocumentSearchResultEvent;
        

        public ICommand DocumentSearchCommand
        {
            get { return new DelegateCommand<object>(DocumentSearchCommandMethod); }
        }

        public ICommand UploadDocumentCommand
        {
            get { return new DelegateCommand<object>(UploadDocumentCommandMethod); }
        }

        private void DocumentSearchCommandMethod(object param)
        {
            _dbInteractivity.RetrieveDocumentsData(SearchString, RetrieveDocumentsDataCallbackMethod);
        }

        private void UploadDocumentCommandMethod(object param)
        {
            _uploadWindow.Show();


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
            Logging.LogEndMethod(_logger, methodNamespace);
        }
    }
}
