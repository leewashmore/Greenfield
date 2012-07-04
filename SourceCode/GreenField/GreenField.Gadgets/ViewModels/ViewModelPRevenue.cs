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
using GreenField.Common;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using System.Linq;
using GreenField.DataContracts.DataContracts;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Controls.Charting;


namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPRevenue : NotificationObject
    {
        #region Fields

        //MEF Singletons

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// Details of selected Security
        /// </summary>
        private EntitySelectionData _securitySelectionData;

        ///// <summary>
        ///// Private member to store P/Revenue data
        ///// </summary>
        //private List<PRevenueData> _pRevenueDataInfo;

        /// <summary>
        /// Stores visibility of Gadget
        /// </summary>
        private Visibility _pRevenueDataGadgetVisibility = Visibility.Collapsed;

        /// <summary>
        /// Stores Chart data
        /// </summary>
        private  RangeObservableCollection<PRevenueData> _pRevenuePlottedData;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">MEF Eventaggregator instance</param>
        public ViewModelPRevenue(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (_securitySelectionData != null )
            {                
                _dbInteractivity.RetrievePRevenueData(_securitySelectionData, RetrievePRevenueDataCallbackMethod );
                BusyIndicatorStatus = true;
            }
            if (_eventAggregator != null)
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe((HandleSecurityReferenceSet));
        }
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Details of Selected Security
        /// </summary>
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return _securitySelectionData;
            }
            set
            {
                _securitySelectionData = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        public RangeObservableCollection<PRevenueData> PRevenuePlottedData
        {
            get
            {
                if (_pRevenuePlottedData == null)
                    _pRevenuePlottedData = new RangeObservableCollection<PRevenueData>();
                return _pRevenuePlottedData;
            }
            set
            {
                _pRevenuePlottedData = value;
                RaisePropertyChanged(() => this.PRevenuePlottedData);
            }

        }
        /// <summary>
        /// ChartArea property bound to ChartArea of dgPRevenue 
        /// </summary>
        private ChartArea _chartArea;
        public ChartArea ChartArea
        {
            get
            {
                return this._chartArea;
            }
            set
            {
                this._chartArea = value;
            }
        }

        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return _busyIndicatorStatus;
            }
            set
            {
                _busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }/// <summary>
        /// Minimum Value for X-Axis of Chart
        /// </summary>
        private decimal _axisYMinValue;
        public decimal AxisYMinValue
        {
            get { return _axisYMinValue; }
            set
            {
                _axisYMinValue = value;
                this.RaisePropertyChanged(() => this.AxisYMinValue);
            }
        }

        /// <summary>
        /// Maximum Value for X-Axis of Chart
        /// </summary>
        private decimal _axisYMaxValue;
        public decimal AxisYMaxValue
        {
            get { return _axisYMaxValue; }
            set
            {
                _axisYMaxValue = value;
                this.RaisePropertyChanged(() => this.AxisYMaxValue);
            }
        }

        /// <summary>
        /// Step size of XAxis of Chart
        /// </summary>
        private int _axisYStep;
        public int AxisYStep
        {
            get { return _axisYStep; }
            set
            {
                _axisYStep = value;

            }
        }
        ///// <summary>
        ///// Stores data for Basic data grid
        ///// </summary>
        //public List<PRevenueData> PRevenueDataInfo
        //{
        //    get { return _pRevenueDataInfo; }
        //    set
        //    {
        //        if (_pRevenueDataInfo != value)
        //        {
        //            _pRevenueDataInfo = value;
        //            RaisePropertyChanged(() => this.PRevenueDataInfo);
        //        }
        //    }
        //}
        public Visibility PRevenueDataGadgetVisibility
        {
            get { return _pRevenueDataGadgetVisibility; }
            set
            {
                _pRevenueDataGadgetVisibility = value;
                RaisePropertyChanged(() => this.PRevenueDataGadgetVisibility);
            }
        }

        #endregion

        #region EVENTS
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler PRevenueDataLoadEvent;

        #endregion

        #region EVENTHANDLERS
        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _securitySelectionData = entitySelectionData;

                    if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                    {
                        if (PRevenueDataLoadEvent != null)
                            PRevenueDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrievePRevenueData(entitySelectionData, RetrievePRevenueDataCallbackMethod);
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
        }
        #endregion

        #region CALLBACK METHOD
        /// <summary>
        /// Callback method that assigns value to the BAsicDataInfo property
        /// </summary>
        /// <param name="result">basic data </param>
        private void RetrievePRevenueDataCallbackMethod(List<PRevenueData> pRevenueData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (pRevenueData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, pRevenueData, 1);
                    //PRevenuePlottedData.Clear();                    
                    PRevenuePlottedData.AddRange(pRevenueData.ToList());
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

        #region EventUnSubscribe

        /// <summary>
        /// Dsiposing off Events and Event Subscribers
        /// </summary>
        public void Dispose()
        {
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
                
            }
        }

        #endregion
    }
}
