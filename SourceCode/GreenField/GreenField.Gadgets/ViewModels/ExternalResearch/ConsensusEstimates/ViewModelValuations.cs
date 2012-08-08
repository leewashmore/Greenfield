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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelValuations : NotificationObject
    {
        #region PrivateFields

        /// <summary>
        /// Instance of Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Selected Date
        /// </summary>
        private DateTime? _effectiveDate;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelValuations(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;

            PeriodColumns.PeriodColumnNavigate += (e) =>
            {
                if (e.PeriodColumnNamespace == GetType().FullName)
                {
                    BusyIndicatorNotification(true, "Retrieving data for updated time span");
                    Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                    PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(Iterator, defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
                    ConsensusEstimateDetailDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo(ConsensusEstimateDetailInfo, out periodRecord,
                        periodRecord, subGroups: DataGrouping);
                    PeriodRecord = periodRecord;
                    PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord, displayPeriodType: false);
                    BusyIndicatorNotification();
                }
            };


            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }

            if (EntitySelectionInfo != null)
            {
                HandleSecurityReferenceSetEvent(EntitySelectionInfo);
            }
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData _entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get
            {
                return _entitySelectionInfo;
            }
            set
            {
                _entitySelectionInfo = value;
                this.RaisePropertyChanged(() => this.EntitySelectionInfo);
            }
        }

        private List<PeriodColumnGroupingDetail> _dataGrouping;
        public List<PeriodColumnGroupingDetail> DataGrouping
        {
            get
            {
                if (_dataGrouping == null)
                {
                    _dataGrouping = new List<PeriodColumnGroupingDetail>();
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "YOY", GroupPropertyName = "YOYGrowth", GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "AshmoreEMM", GroupPropertyName = "AshmoreEmmAmount", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Variance%", GroupPropertyName = "Variance", GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Actual", GroupPropertyName = "Actual", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "# of Estimates", GroupPropertyName = "NumberOfEstimates", GroupDataType = PeriodColumnGroupingType.INT });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "High", GroupPropertyName = "High", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Low", GroupPropertyName = "Low", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Standard Deviation", GroupPropertyName = "StandardDeviation", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Last Update", GroupPropertyName = "DataSourceDate", GroupDataType = PeriodColumnGroupingType.SHORT_DATETIME });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Reported Currency", GroupPropertyName = "SourceCurrency", GroupDataType = PeriodColumnGroupingType.STRING });
                }
                return _dataGrouping;
            }
        }


        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> _consensusEstimateDetailDisplayInfo;
        public List<PeriodColumnDisplayData> ConsensusEstimateDetailDisplayInfo
        {
            get
            {
                if (_consensusEstimateDetailDisplayInfo == null)
                    _consensusEstimateDetailDisplayInfo = new List<PeriodColumnDisplayData>();
                return _consensusEstimateDetailDisplayInfo;
            }
            set
            {
                _consensusEstimateDetailDisplayInfo = value;
                RaisePropertyChanged(() => this.ConsensusEstimateDetailDisplayInfo);
            }
        }

        /// <summary>
        /// ConsensusEstimates Detailed Info
        /// </summary>
        private List<ConsensusEstimatesValuations> _consensusEstimateDetailInfo;
        public List<ConsensusEstimatesValuations> ConsensusEstimateDetailInfo
        {
            get
            {
                return _consensusEstimateDetailInfo;
            }
            set
            {
                _consensusEstimateDetailInfo = value;
                SetConsensusEstimateMedianDisplayInfo();
            }
        }

        #region DasboardActivityStatus

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
                if (_isActive)
                {
                    if (EntitySelectionInfo != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        _dbInteractivity.RetrieveIssuerReferenceData(EntitySelectionInfo, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        }

        #endregion

        #region Busy Indicator
        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorIsBusy;
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
        /// Busy Indicator Content
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

        #region Currency Option
        /// <summary>
        /// Stores Reported issuer domicile currency and "USD"
        /// </summary>
        private ObservableCollection<String> _currencyInfo = new ObservableCollection<string> { "USD" };
        public ObservableCollection<String> CurrencyInfo
        {
            get { return _currencyInfo; }
            set
            {
                if (_currencyInfo != value)
                {
                    _currencyInfo = value;
                    RaisePropertyChanged(() => this.CurrencyInfo);
                }
            }
        }

        /// <summary>
        /// Stores selected currency
        /// </summary>
        private String _selectedCurrency = "USD";
        public String SelectedCurrency
        {
            get
            {
                return _selectedCurrency;
            }
            set
            {
                _selectedCurrency = value;
                RaisePropertyChanged(() => this.SelectedCurrency);
                RetrieveConsensusEstimatesValuationData();
            }

        }
        #endregion

        #region Period Type
        /// <summary>
        /// Stores FinancialStatementPeriodType Enum Items
        /// </summary>
        public List<FinancialStatementPeriodType> PeriodTypeInfo
        {
            get { return EnumUtils.GetEnumDescriptions<FinancialStatementPeriodType>(); }
        }

        /// <summary>
        /// Stores selected FinancialStatementPeriodType
        /// </summary>
        private FinancialStatementPeriodType _selectedPeriodType = FinancialStatementPeriodType.ANNUAL;
        public FinancialStatementPeriodType SelectedPeriodType
        {
            get { return _selectedPeriodType; }
            set
            {
                if (_selectedPeriodType != value)
                {
                    _selectedPeriodType = value;
                    RaisePropertyChanged(() => this.SelectedPeriodType);
                    RetrieveConsensusEstimatesValuationData();
                }
            }
        }
        #endregion

        #region Period Column Headers
        /// <summary>
        /// Stores period column headers
        /// </summary>
        private List<String> _periodColumnHeader;
        public List<String> PeriodColumnHeader
        {
            get
            {
                if (_periodColumnHeader == null)
                    _periodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord, false);
                return _periodColumnHeader;
            }
            set
            {
                _periodColumnHeader = value;
                RaisePropertyChanged(() => this.PeriodColumnHeader);
                if (value != null)
                {
                    PeriodColumns.RaisePeriodColumnUpdateCompleted(new PeriodColumnUpdateEventArg()
                    {
                        PeriodColumnNamespace = GetType().FullName,
                        PeriodColumnHeader = value,
                        PeriodRecord = PeriodRecord,
                        PeriodIsYearly = SelectedPeriodType == FinancialStatementPeriodType.ANNUAL
                    });
                }
            }
        }
        #endregion

        #region Period Information
        /// <summary>
        /// Iteration Count
        /// </summary>
        public Int32 Iterator { get; set; }

        /// <summary>
        /// Period Record storing period information based on iteration
        /// </summary>
        private PeriodRecord _periodRecord;
        public PeriodRecord PeriodRecord
        {
            get
            {
                if (_periodRecord == null)
                    _periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
                return _periodRecord;
            }
            set { _periodRecord = value; }
        }
        #endregion

        #region Issuer Details

        /// <summary>
        /// Stores Issuer related data
        /// </summary>
        /// 
        private IssuerReferenceData _issuerReferenceInfo;
        public IssuerReferenceData IssuerReferenceInfo
        {
            get { return _issuerReferenceInfo; }
            set
            {
                if (_issuerReferenceInfo != value)
                {
                    _issuerReferenceInfo = value;
                    if (value != null)
                    {
                        CurrencyInfo = new ObservableCollection<String> { IssuerReferenceInfo.CurrencyCode };
                        if (IssuerReferenceInfo.CurrencyCode != "USD")
                            CurrencyInfo.Add("USD");

                        SelectedCurrency = CurrencyInfo[0];
                    }
                }
            }
        }
        #endregion


        #endregion

        #region EventHandlers

        /// <summary>
        /// Security Change Event
        /// </summary>
        /// <param name="result"></param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    EntitySelectionInfo = result;

                    if (EntitySelectionInfo != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        _dbInteractivity.RetrieveIssuerReferenceData(result, RetrieveIssuerReferenceDataCallbackMethod);
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region Callback Methods

        /// <summary>
        /// IssuerReferenceData callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveIssuerReferenceDataCallbackMethod(IssuerReferenceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    IssuerReferenceInfo = result;
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// ConsensusEstimateData callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveConsensusEstimateDataCallbackMethod(List<ConsensusEstimatesValuations> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ConsensusEstimateDetailInfo = result;
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
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// Event Unsubscribe
        /// </summary>
        public void Dispose()
        {
            //Implement Dispose Here
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// 
        /// </summary>
        public void SetConsensusEstimateMedianDisplayInfo()
        {
            BusyIndicatorNotification(true, "Updating information based on selected preference");

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(Iterator, defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
            ConsensusEstimateDetailDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo(ConsensusEstimateDetailInfo, out periodRecord, periodRecord, subGroups: DataGrouping);
            PeriodRecord = periodRecord;
            PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord, false);

            BusyIndicatorNotification();
        }

        /// <summary>
        /// Busy Indicator Notification
        /// </summary>
        /// <param name="showBusyIndicator"></param>
        /// <param name="message"></param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }

        /// <summary>
        /// Web Service Call Method
        /// </summary>
        private void RetrieveConsensusEstimatesValuationData()
        {
            try
            {
                if (IssuerReferenceInfo != null)
                {
                    if (SelectedCurrency != null)
                    {
                        if (IssuerReferenceInfo.IssuerId != null)
                        {
                            _dbInteractivity.RetrieveConsensusEstimatesValuationsData
                                (IssuerReferenceInfo.IssuerId, SelectedPeriodType, SelectedCurrency, RetrieveConsensusEstimateDataCallbackMethod);
                            BusyIndicatorNotification(true, "Retrieving data for the selected Security");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
