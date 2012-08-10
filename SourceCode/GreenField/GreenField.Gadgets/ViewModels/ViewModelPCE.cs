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
    public class ViewModelPCE : NotificationObject
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

        /// <summary>
        /// Stores Chart data
        /// </summary>
        private RangeObservableCollection<PRevenueData> _PCEPlottedData;

        /// <summary>
        /// Stores chart title
        /// </summary>
        private string _chartTitle = "P/CE";
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">MEF Eventaggregator instance</param>
        public ViewModelPCE(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            CallingWebMethod();
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

        public RangeObservableCollection<PRevenueData> PCEPlottedData
        {
            get
            {
                if (_PCEPlottedData == null)
                    _PCEPlottedData = new RangeObservableCollection<PRevenueData>();
                return _PCEPlottedData;
            }
            set
            {
                _PCEPlottedData = value;
                RaisePropertyChanged(() => this.PCEPlottedData);
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
        /// <summary>
        /// Minimum Value for X-Axis of Chart
        /// </summary>
        private decimal _axisXMinValue;
        public decimal AxisXMinValue
        {
            get { return _axisXMinValue; }
            set
            {
                _axisXMinValue = value;
                this.RaisePropertyChanged(() => this.AxisXMinValue);
            }
        }

        /// <summary>
        /// Maximum Value for X-Axis of Chart
        /// </summary>
        private decimal _axisXMaxValue;
        public decimal AxisXMaxValue
        {
            get { return _axisXMaxValue; }
            set
            {
                _axisXMaxValue = value;
                this.RaisePropertyChanged(() => this.AxisXMaxValue);
            }
        }

        /// <summary>
        /// Step size of XAxis of Chart
        /// </summary>
        private int _axisXStep;
        public int AxisXStep
        {
            get { return _axisXStep; }
            set
            {
                _axisXStep = value;

            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
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
                _isActive = value;
                CallingWebMethod();
            }
        }

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
                if (entitySelectionData != null && IsActive)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _securitySelectionData = entitySelectionData;

                    CallingWebMethod();
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
        private void RetrievePCEDataCallbackMethod(List<PRevenueData> pRevenueData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (pRevenueData != null && IsActive)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, pRevenueData, 1);
                    PCEPlottedData.Clear();
                    PCEPlottedData.AddRange(pRevenueData.ToList());
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
            finally { BusyIndicatorStatus = false; }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region WEB SERVICE CALL
        private void CallingWebMethod()
        {
            if (_securitySelectionData != null && IsActive)
            {
                _dbInteractivity.RetrievePRevenueData(_securitySelectionData, _chartTitle, RetrievePCEDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
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

