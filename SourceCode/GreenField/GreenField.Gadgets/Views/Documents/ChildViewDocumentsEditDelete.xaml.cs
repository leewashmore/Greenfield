using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.DataContracts;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using Microsoft.Practices.Prism.Logging;
using System.IO;

namespace GreenField.Gadgets.Views.Documents
{
    public partial class ChildViewDocumentsEditDelete : ChildWindow
    {

        IDBInteractivity _dBInteractivity;
        ILoggerFacade _logger;

        public ChildViewDocumentsEditDelete(IDBInteractivity dBInteractivity, ILoggerFacade logger)
        {
            _dBInteractivity = dBInteractivity;
            _logger = logger;

            InitializeComponent();

            if (_dBInteractivity != null)
            {
                _dBInteractivity.RetrieveDocumentsDataForUser(UserSession.SessionManager.SESSION.UserName, RetrieveDocumentsDataForUserCallbackMethod);
            }
        }

        private List<DocumentCatalogData> _documentCatalogInfo;
        public List<DocumentCatalogData> DocumentCatalogInfo
        {
            get { return _documentCatalogInfo; }
            set 
            {
                _documentCatalogInfo = value;
                this.dgEditDeleteFile.ItemsSource = value;
            }
        }
        

        private void RetrieveDocumentsDataForUserCallbackMethod(List<DocumentCatalogData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    DocumentCatalogInfo = result;
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

