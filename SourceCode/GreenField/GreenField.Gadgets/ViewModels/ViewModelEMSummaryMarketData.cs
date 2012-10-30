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
using GreenField.Common;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using System.Collections.Generic;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelEMSummaryMarketData : NotificationObject
    {
        #region PrivateMembers
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelEMSummaryMarketData(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            dbInteractivity.RetrieveEMSummaryMarketData("ABPEQ", RetrieveEMSummaryDataCallbackMethod);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property that stores the data from the web service
        /// </summary>
        private List<EMSummaryMarketData> emSummaryMarketDataInfo;
        public List<EMSummaryMarketData> EmSummaryMarketDataInfo
        {
            get
            {
                if (emSummaryMarketDataInfo == null)
                    emSummaryMarketDataInfo = new List<EMSummaryMarketData>();
                return emSummaryMarketDataInfo;
            }
            set
            {
                if (emSummaryMarketDataInfo != value)
                {
                    emSummaryMarketDataInfo = value;
                    RaisePropertyChanged(() => this.EmSummaryMarketDataInfo);
                }
            }
        }
        #endregion

        #region CallbackMethod
        public void RetrieveEMSummaryDataCallbackMethod(List<EMSummaryMarketData> result)
        {
           
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    EmSummaryMarketDataInfo = result;
                    RetrieveEMSummaryDataCompletedEvent(new RetrieveEMSummaryDataCompleteEventArgs() { EMSummaryInfo = result });
                }
                else
                {
                    EmSummaryMarketDataInfo = new List<EMSummaryMarketData>();
                    RetrieveEMSummaryDataCompletedEvent(new RetrieveEMSummaryDataCompleteEventArgs() { EMSummaryInfo = result });
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

        #region Events
        /// <summary>
        /// Event for the Retrieval of Data 
        /// </summary>
        public event RetrieveEMSummaryDataCompleteEventHandler RetrieveEMSummaryDataCompletedEvent;     
        #endregion
    }
}
