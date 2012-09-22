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
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelFairValueComposition : NotificationObject
    {
        #region PRIVATE FIELDS
        //MEF Singletons

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
        public ILoggerFacade _logger;


        /// <summary>
        /// Private member to store basic data gadget visibilty
        /// </summary>
        private Visibility _freeCashFlowGadgetVisibility = Visibility.Collapsed;
        /// <summary>
        /// Private member to store Selected Security ID
        /// </summary>
        private EntitySelectionData _securitySelectionData = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Stores fcf arranged data
        /// </summary>
        private List<FairValueCompositionSummaryData> _fairValueCompositionData;
        public List<FairValueCompositionSummaryData> FairValueCompositionData
        {
            get
            {
                return _fairValueCompositionData;
            }
            set
            {
                if (FairValueCompositionData != value)
                {
                    _fairValueCompositionData = value;
                    RaisePropertyChanged(() => this.FairValueCompositionData);
                }
            }
        }

        private List<Measure> _measuresData;
        public List<Measure> MeasuresData
        {
            get
            {
                if (_measuresData == null)
                {
                    _measuresData = new List<Measure>();
                }
                return _measuresData;
            }
            set
            {
                _measuresData = value;
                this.RaisePropertyChanged(() => this.MeasuresData);
            }
        }       



        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                }
                if (_securitySelectionData != null && IsActive)
                {
                    if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                    }
                }
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
        }
        #endregion

        #region CONSTRUCTOR
        public ViewModelFairValueComposition(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            MeasuresData = GetMeasureList();

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
            if (_securitySelectionData != null && IsActive)
            {
                if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                {
                    CallingWebMethod();
                }
            }

        }

        #endregion

        #region EVENTHANDLERS
        ///// <summary>
        ///// event to handle data
        ///// </summary>
        //public event RetrieveFairValueCompositionSummaryDataCompletedEventHandler RetrieveFairValueCompositionSummaryDataCompleteEvent;

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
                        CallingWebMethod();
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
        private void RetrieveFairValueCompositionSummaryDataCallbackMethod(List<FairValueCompositionSummaryData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //MeasuresList = GetMeasureList();
                FairValueCompositionData = new List<FairValueCompositionSummaryData>();
                List<FairValueCompositionSummaryData> temp = new List<FairValueCompositionSummaryData>();
                //FreeCashFlowGadgetVisibility = Visibility.Collapsed;
                
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    temp.Add(new FairValueCompositionSummaryData()
                    {
                        SOURCE = "Primary Analyst",
                        MEASURE = result.Select(a => a.SOURCE).Contains("Primary Analyst") ? result.Where(a => a.SOURCE == "Primary Analyst").Select(a => a.MEASURE).FirstOrDefault() : null,
                        BUY = result.Select(a => a.SOURCE).Contains("Primary Analyst") ? result.Where(a => a.SOURCE == "Primary Analyst").Select(a => a.BUY).FirstOrDefault() : null,
                        SELL = result.Select(a => a.SOURCE).Contains("Primary Analyst") ? result.Where(a => a.SOURCE == "Primary Analyst").Select(a => a.SELL).FirstOrDefault() : null,
                        UPSIDE = result.Select(a => a.SOURCE).Contains("Primary Analyst") ? result.Where(a => a.SOURCE == "Primary Analyst").Select(a => a.UPSIDE).FirstOrDefault() : null,
                        DATE = result.Select(a => a.SOURCE).Contains("Primary Analyst") ? result.Where(a => a.SOURCE == "Primary Analyst").Select(a => a.DATE).FirstOrDefault() : null,
                        DATA_ID = result.Select(a => a.SOURCE).Contains("Primary Analyst") ? result.Where(a => a.SOURCE == "Primary Analyst").Select(a => a.DATA_ID).FirstOrDefault() : null,
                    });
                    temp.Add(new FairValueCompositionSummaryData()
                    {
                        SOURCE = "Industry Analyst",
                        MEASURE = result.Select(a => a.SOURCE).Contains("Industry Analyst") ? result.Where(a => a.SOURCE == "Industry Analyst").Select(a => a.MEASURE).FirstOrDefault() : null,
                        BUY = result.Select(a => a.SOURCE).Contains("Industry Analyst") ? result.Where(a => a.SOURCE == "Industry Analyst").Select(a => a.BUY).FirstOrDefault() : null,
                        SELL = result.Select(a => a.SOURCE).Contains("Industry Analyst") ? result.Where(a => a.SOURCE == "Industry Analyst").Select(a => a.SELL).FirstOrDefault() : null,
                        UPSIDE = result.Select(a => a.SOURCE).Contains("Industry Analyst") ? result.Where(a => a.SOURCE == "Industry Analyst").Select(a => a.UPSIDE).FirstOrDefault() : null,
                        DATE = result.Select(a => a.SOURCE).Contains("Industry Analyst") ? result.Where(a => a.SOURCE == "Industry Analyst").Select(a => a.DATE).FirstOrDefault() : null,
                        DATA_ID = result.Select(a => a.SOURCE).Contains("Industry Analyst") ? result.Where(a => a.SOURCE == "Industry Analyst").Select(a => a.DATA_ID).FirstOrDefault() : null,
                    });
                    if (result != null && result.Count > 0)
                    {
                        foreach (FairValueCompositionSummaryData item in result)
                        {
                            if (item.SOURCE == "Primary Analyst" || item.SOURCE == "Industry Analyst")
                            { continue; }
                            else
                            {
                                temp.Add(item);
                            }
                        }
                    }
                    FairValueCompositionData = temp;
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

        #region SERVICE CALL METOHD
        /// <summary>
        /// Calls web service method
        /// </summary>
        private void CallingWebMethod()
        {
            if (_securitySelectionData != null && IsActive)
            {
                _dbInteractivity.RetrieveFairValueCompostionSummary(_securitySelectionData, RetrieveFairValueCompositionSummaryDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

        }
        #endregion

        #region EventUnSubscribe

        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion

        #region Helper Method

        public List<Measure> GetMeasureList()
        {
            MeasuresData.Add(new Measure() { DataId = 236, Measures = "Forward Dividend Yield" });
            MeasuresData.Add(new Measure() { DataId = 198, Measures = "Forward EV/EBITDA" });
            MeasuresData.Add(new Measure() { DataId = 246, Measures = "Forward EV/EBITDA relative to Country" });
            MeasuresData.Add(new Measure() { DataId = 247, Measures = "Forward EV/EBITDA relative to Industry" });
            MeasuresData.Add(new Measure() { DataId = 248, Measures = "Forward EV/EBITDA relative to Country Industry" });
            MeasuresData.Add(new Measure() { DataId = 237, Measures = "Forward EV/Revenue" });
            MeasuresData.Add(new Measure() { DataId = 238, Measures = "Forward P/NAV" });
            MeasuresData.Add(new Measure() { DataId = 239, Measures = "Forward P/Appraisal Value" });
            MeasuresData.Add(new Measure() { DataId = 188, Measures = "Forward P/BV" });
            MeasuresData.Add(new Measure() { DataId = 249, Measures = "Forward P/BV relative to Country" });
            MeasuresData.Add(new Measure() { DataId = 250, Measures = "Forward P/BV relative to Industry" });
            MeasuresData.Add(new Measure() { DataId = 251, Measures = "Forward P/BV relative to Country Industry" });
            MeasuresData.Add(new Measure() { DataId = 189, Measures = "Forward P/CE" });
            MeasuresData.Add(new Measure() { DataId = 187, Measures = "Forward P/E" });
            MeasuresData.Add(new Measure() { DataId = 252, Measures = "Forward P/E relative to Country" });
            MeasuresData.Add(new Measure() { DataId = 253, Measures = "Forward P/E relative to Industry" });
            MeasuresData.Add(new Measure() { DataId = 254, Measures = "Forward P/E relative to Country Industry" });
            MeasuresData.Add(new Measure() { DataId = 241, Measures = "Forward P/E to 2 Year Growth" });
            MeasuresData.Add(new Measure() { DataId = 242, Measures = "Forward P/E to 3 Year Growth" });
            MeasuresData.Add(new Measure() { DataId = 245, Measures = "Forward P/Embedded Value" });
            MeasuresData.Add(new Measure() { DataId = 197, Measures = "Forward P/Revenue" });

            return MeasuresData;
        }

        #endregion
    }
}
