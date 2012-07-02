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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using System.Collections.Generic;
using GreenField.DataContracts.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelQuarterlyResultsComparison : NotificationObject
    {
        #region Fields

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;
        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelQuarterlyResultsComparison(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            if(YearValue !=null && FieldValue !=null)
            _dbInteractivity.RetrieveQuarterlyResultsData(FieldValue, YearValue, RetrieveQuarterlyResultsDataCallbackMethod);
        }
        #endregion

        #region Properties      

        public List<String> FieldValues 
        {
            get { return new List<String> { "Net Income", "Revenue"}; }            
        }        

        public List<int>  YearValues
        {
          //  get { return new List<int> {DateTime.Now.Year,DateTime.Now.Year+1,DateTime.Now.Year+2}; } 
            get { return new List<int> { 2002,2003,2004}; }
        }

      
        private int yearValue;
        public int YearValue
        {
            get 
            { 
                return yearValue;
            }

            set 
            {
                if (yearValue != value)
                {
                    yearValue = value;
                    if (FieldValue != null)
                        if (null != quarterlyResultsComoarisonDataLoadedEvent)
                            quarterlyResultsComoarisonDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveQuarterlyResultsData(FieldValue, yearValue, RetrieveQuarterlyResultsDataCallbackMethod);
                    RaisePropertyChanged(() => this.YearValue);
                }
            }
        }

        private String fieldValue;
        public String FieldValue
        {
            get
            {
                return fieldValue;
            }

            set
            {
                if (fieldValue != value)
                {
                    fieldValue = value;
                    if(YearValue != 0)
                        if (null != quarterlyResultsComoarisonDataLoadedEvent)
                            quarterlyResultsComoarisonDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveQuarterlyResultsData(fieldValue, YearValue, RetrieveQuarterlyResultsDataCallbackMethod);
                    RaisePropertyChanged(() => this.FieldValue);
                }
            }
        }

      
        private List<QuarterlyResultsData> quarterlyResultsInfo;
        public List<QuarterlyResultsData> QuarterlyResultsInfo
        {
            get
            {
                return quarterlyResultsInfo;
            }

            set
            {
                 quarterlyResultsInfo = value;
                 RaisePropertyChanged(() => this.QuarterlyResultsInfo);
                
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler quarterlyResultsComoarisonDataLoadedEvent;
        #endregion

        #region CallbackMethods
        private void RetrieveQuarterlyResultsDataCallbackMethod(List<QuarterlyResultsData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    QuarterlyResultsInfo = result;
                    if (null != quarterlyResultsComoarisonDataLoadedEvent)
                        quarterlyResultsComoarisonDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    quarterlyResultsComoarisonDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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






    }
}
