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
            if(YearValue !=0 && FieldValue !=null)
            _dbInteractivity.RetrieveQuarterlyResultsData(FieldValue, YearValue, RetrieveQuarterlyResultsDataCallbackMethod);
        }
        #endregion

        #region Properties      
        /// <summary>
        /// List of field values binded to the field combo box
        /// </summary>
        public List<String> FieldValues 
        {
            get { return new List<String> { "Net Income", "Revenue"}; }            
        }        
        /// <summary>
        /// List of year values binded to the year combo box
        /// </summary>
        public List<int>  YearValues
        {
           get { return new List<int> {DateTime.Now.Year,DateTime.Now.Year+1,DateTime.Now.Year+2}; } 
          //  get { return new List<int> { 2002,2003,2004}; }
        }


        private String gridHeaderValue = "Net Income" + ">" + DateTime.Now.Year.ToString();
        public String GridHeaderValue
        {
            get
            {
                return gridHeaderValue;
            }

            set
            {
                if (gridHeaderValue != value)
                {
                    gridHeaderValue = value;
                    RaisePropertyChanged(() => this.GridHeaderValue);
                }
            }
        }

      /// <summary>
      /// Selected Year Value
      /// </summary>
        private int yearValue = DateTime.Now.Year;
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
                    GridHeaderValue = FieldValue + ">" + value.ToString();
                    if (FieldValue != null && IsActive)
                        if (null != quarterlyResultsComoarisonDataLoadedEvent)
                            quarterlyResultsComoarisonDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveQuarterlyResultsData(FieldValue, yearValue, RetrieveQuarterlyResultsDataCallbackMethod);
                    RaisePropertyChanged(() => this.YearValue);
                }
            }
        }

        /// <summary>
        /// Selected Field Value
        /// </summary>
        private String fieldValue = "Net Income";
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
                    GridHeaderValue = value + ">" + YearValue.ToString();
                    if(YearValue != 0 && IsActive)
                        if (null != quarterlyResultsComoarisonDataLoadedEvent)
                            quarterlyResultsComoarisonDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveQuarterlyResultsData(fieldValue, YearValue, RetrieveQuarterlyResultsDataCallbackMethod);
                    RaisePropertyChanged(() => this.FieldValue);
                }
            }
        }

      /// <summary>
      /// list that stores the Quarterly Comparison Info
      /// </summary>
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
                if (_isActive != value)
                {
                    _isActive = value;
                    if (YearValue != 0 && FieldValue != null && IsActive)
                    {
                        if (null != quarterlyResultsComoarisonDataLoadedEvent)
                            quarterlyResultsComoarisonDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveQuarterlyResultsData(FieldValue, YearValue, RetrieveQuarterlyResultsDataCallbackMethod);
                    }
                }
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
        /// <summary>
        /// Called when the result list gets populated with data 
        /// </summary>
        /// <param name="result">result</param>
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
