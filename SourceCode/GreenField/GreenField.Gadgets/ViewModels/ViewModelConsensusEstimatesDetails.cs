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
using Microsoft.Practices.Prism.Regions;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewModelConsensusEstimatesDetails class
    /// </summary>
    public class ViewModelConsensusEstimatesDetails : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        public IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        public ILoggerFacade _logger;
        //private EntitySelectionData _entitySelectionData;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelConsensusEstimatesDetails(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;

            PeriodColumns.PeriodColumnNavigate += (e) =>
            {
                if (e.PeriodColumnNamespace == GetType().FullName)
                {
                    Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                    SetConsensusEstimatePivotDisplayInfo();
                }
            };

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

        #region Properties
        private List<PeriodColumnGroupingDetail> _dataGrouping;
        public List<PeriodColumnGroupingDetail> DataGrouping
        {
            get
            {
                if (_dataGrouping == null)
                {
                    _dataGrouping = new List<PeriodColumnGroupingDetail>();
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "YOY", GroupPropertyName = "YOYGrowth", GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Consensus Median", GroupPropertyName = "ConsensusMedian", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "AshmoreEMM", GroupPropertyName = "AshmoreEmmAmount", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Variance%", GroupPropertyName = "Variance", GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Actual", GroupPropertyName = "Actual", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "# Of Estimates", GroupPropertyName = "NumberOfEstimates", GroupDataType = PeriodColumnGroupingType.INT });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "High", GroupPropertyName = "High", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Low", GroupPropertyName = "Low", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Std Dev", GroupPropertyName = "StandardDeviation", GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Last Update", GroupPropertyName = "DataSourceDate", GroupDataType = PeriodColumnGroupingType.SHORT_DATETIME });
                    _dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Reported Currency", GroupPropertyName = "SourceCurrency", GroupDataType = PeriodColumnGroupingType.STRING });
                }
                return _dataGrouping;
            }
        }


        #region Consensus Estimate Detail Information
        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> _consensusEstimateDetailDisplayInfo;
        public List<PeriodColumnDisplayData> ConsensusEstimateDetailDisplayInfo
        {
            get { return _consensusEstimateDetailDisplayInfo; }
            set
            {
                _consensusEstimateDetailDisplayInfo = value;
                RaisePropertyChanged(() => this.ConsensusEstimateDetailDisplayInfo);                
            }
        }

        /// <summary>
        /// Unpivoted ConsensusEstimatesDetail Information received from stored procedure
        /// </summary>
        private List<ConsensusEstimateDetail> _consensusEstimateDetailInfo;
        public List<ConsensusEstimateDetail> ConsensusEstimateDetailInfo
        {
            get
            {
                if (_consensusEstimateDetailInfo == null)
                    _consensusEstimateDetailInfo = new List<ConsensusEstimateDetail>();
                return _consensusEstimateDetailInfo;
            }
            set
            {
                if (_consensusEstimateDetailInfo != value)
                {
                    _consensusEstimateDetailInfo = value;                    
                }
            }
        }

        private List<ConsensusEstimateDetail> _brokerDetailUnpivotInfo;
        public List<ConsensusEstimateDetail> BrokerDetailUnpivotInfo
        {
            get
            {
                if (_brokerDetailUnpivotInfo == null)
                    _brokerDetailUnpivotInfo = new List<ConsensusEstimateDetail>();
                return _brokerDetailUnpivotInfo;
            }
            set
            {
                if (_brokerDetailUnpivotInfo != value)
                {
                    _brokerDetailUnpivotInfo = value;
                    SetConsensusEstimatePivotDisplayInfo();
                }
            }
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

                        SelectedCurrency = "USD";
                    }
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
                    RetrieveConsensusEstimatesDetailsData();
                }
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
            get { return _selectedCurrency; }
            set
            {            
                    _selectedCurrency = value;
                    RaisePropertyChanged(() => this.SelectedCurrency);
                    _eventAggregator.GetEvent<ConsensusEstimateDetailCurrencyChangeEvent>().Publish(new ChangedCurrencyInEstimateDetail()
                    {
                       CurrencyName = value
                    });
                    RetrieveConsensusEstimatesDetailsData();
            }
        }
        #endregion

        #region Security Information
        private EntitySelectionData _entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get { return _entitySelectionInfo; }
            set
            {
                _entitySelectionInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionInfo);
            }
        }
        #endregion

        #region Busy Indicator
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
                    if (EntitySelectionInfo != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        _dbInteractivity.RetrieveIssuerReferenceData(EntitySelectionInfo, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        }
        #endregion

        #region Event Handlers

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

        public void RetrieveConsensusEstimateDetailedDataCallbackMethod(List<ConsensusEstimateDetail> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ConsensusEstimateDetailInfo = result;
                    if (IssuerReferenceInfo != null && IsActive)
                    {
                        _dbInteractivity.RetrieveConsensusEstimateDetailedBrokerData(IssuerReferenceInfo.IssuerId, SelectedPeriodType, SelectedCurrency, RetrieveConsensusEstimateDetailedBrokerDataCallBackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
               // BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        public void RetrieveConsensusEstimateDetailedBrokerDataCallBackMethod(List<ConsensusEstimateDetail> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    BrokerDetailUnpivotInfo = result;
                }
                else
                {
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
        #endregion

        #region Helper Methods
        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        private void RetrieveConsensusEstimatesDetailsData()
        {
            if (IssuerReferenceInfo != null && IsActive)
            {
                _dbInteractivity.RetrieveConsensusEstimateDetailedData(IssuerReferenceInfo.IssuerId,
                                                                    SelectedPeriodType,
                                                                    SelectedCurrency,
                                                                    RetrieveConsensusEstimateDetailedDataCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Data based on selected security");
            }
        }

        public void SetConsensusEstimatePivotDisplayInfo()
        {
            BusyIndicatorNotification(true, "Updating information based on selected preference");

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(Iterator, defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
            List<PeriodColumnDisplayData> estimateDetailPivotInfo = PeriodColumns.SetPeriodColumnDisplayInfo(ConsensusEstimateDetailInfo, out periodRecord,
                periodRecord, subGroups: DataGrouping);
            List<PeriodColumnDisplayData> brokerDetailPivotInfo = PeriodColumns.SetPeriodColumnDisplayInfo(BrokerDetailUnpivotInfo, out periodRecord,
                periodRecord, uniqueByGroupDesc: true);
            estimateDetailPivotInfo.AddRange(brokerDetailPivotInfo);
            ConsensusEstimateDetailDisplayInfo = estimateDetailPivotInfo;

            PeriodRecord = periodRecord;
            PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord, displayPeriodType: false);
            BusyIndicatorNotification();
        }        

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }
        #endregion        
    }
}