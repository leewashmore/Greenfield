using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ExternalResearchDefinitions;

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
        public IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        public ILoggerFacade logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelConsensusEstimatesDetails(DashboardGadgetParam param)
        {
            logger = param.LoggerFacade;
            dbInteractivity = param.DBInteractivity;
            eventAggregator = param.EventAggregator;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;

            PeriodColumns.PeriodColumnNavigate += (e) =>
            {
                if (e.PeriodColumnNamespace == GetType().FullName)
                {
                    Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                    SetConsensusEstimatePivotDisplayInfo();
                }
            };

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }

            if (EntitySelectionInfo != null)
            {
                HandleSecurityReferenceSetEvent(EntitySelectionInfo);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// property to enable proper grouping in grid
        /// </summary>
        private List<PeriodColumnGroupingDetail> dataGrouping;
        public List<PeriodColumnGroupingDetail> DataGrouping
        {
            get
            {
                if (dataGrouping == null)
                {
                    dataGrouping = new List<PeriodColumnGroupingDetail>();
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "YOY",
                        GroupPropertyName = "YOYGrowth",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "Consensus Median",
                        GroupPropertyName = "ConsensusMedian",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "AshmoreEMM",
                        GroupPropertyName = "AshmoreEmmAmount",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "Variance%",
                        GroupPropertyName = "Variance",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "Actual",
                        GroupPropertyName = "Actual",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "# Of Estimates",
                        GroupPropertyName = "NumberOfEstimates",
                        GroupDataType = PeriodColumnGroupingType.INT
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "High",
                        GroupPropertyName = "High",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "Low",
                        GroupPropertyName = "Low",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "Std Dev",
                        GroupPropertyName = "StandardDeviation",
                        GroupDataType = PeriodColumnGroupingType.DECIMAL
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "Last Update",
                        GroupPropertyName = "DataSourceDate",
                        GroupDataType = PeriodColumnGroupingType.SHORT_DATETIME
                    });
                    dataGrouping.Add(new PeriodColumnGroupingDetail()
                    {
                        GroupDisplayName = "Reported Currency",
                        GroupPropertyName = "SourceCurrency",
                        GroupDataType = PeriodColumnGroupingType.STRING
                    });
                }
                return dataGrouping;
            }
        }

        #region Consensus Estimate Detail Information
        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> consensusEstimateDetailDisplayInfo;
        public List<PeriodColumnDisplayData> ConsensusEstimateDetailDisplayInfo
        {
            get { return consensusEstimateDetailDisplayInfo; }
            set
            {
                consensusEstimateDetailDisplayInfo = value;
                RaisePropertyChanged(() => this.ConsensusEstimateDetailDisplayInfo);
            }
        }

        /// <summary>
        /// Unpivoted ConsensusEstimatesDetail Information received from stored procedure
        /// </summary>
        private List<ConsensusEstimateDetail> consensusEstimateDetailInfo;
        public List<ConsensusEstimateDetail> ConsensusEstimateDetailInfo
        {
            get
            {
                if (consensusEstimateDetailInfo == null)
                { consensusEstimateDetailInfo = new List<ConsensusEstimateDetail>(); }
                return consensusEstimateDetailInfo;
            }
            set
            {
                if (consensusEstimateDetailInfo != value)
                {
                    consensusEstimateDetailInfo = value;
                }
            }
        }

        private List<ConsensusEstimateDetail> brokerDetailUnpivotInfo;
        public List<ConsensusEstimateDetail> BrokerDetailUnpivotInfo
        {
            get
            {
                if (brokerDetailUnpivotInfo == null)
                { brokerDetailUnpivotInfo = new List<ConsensusEstimateDetail>(); }
                return brokerDetailUnpivotInfo;
            }
            set
            {
                if (brokerDetailUnpivotInfo != value)
                {
                    brokerDetailUnpivotInfo = value;
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
        private IssuerReferenceData issuerReferenceInfo;
        public IssuerReferenceData IssuerReferenceInfo
        {
            get { return issuerReferenceInfo; }
            set
            {
                if (issuerReferenceInfo != value)
                {
                    issuerReferenceInfo = value;
                    if (value != null)
                    {
                        CurrencyInfo = new ObservableCollection<String> { IssuerReferenceInfo.CurrencyCode };
                        if (IssuerReferenceInfo.CurrencyCode != "USD")
                        {
                            CurrencyInfo.Add("USD");
                        }
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
        private PeriodRecord periodRecordInfo;
        public PeriodRecord PeriodRecord
        {
            get
            {
                if (periodRecordInfo == null)
                {
                    periodRecordInfo = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
                }
                return periodRecordInfo;
            }
            set { periodRecordInfo = value; }
        }
        #endregion

        #region Period Column Headers
        /// <summary>
        /// Stores period column headers
        /// </summary>
        private List<String> periodColumnHeader;
        public List<String> PeriodColumnHeader
        {
            get
            {
                if (periodColumnHeader == null)
                {
                    periodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord, false);
                }
                return periodColumnHeader;
            }
            set
            {
                periodColumnHeader = value;
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
        private FinancialStatementPeriodType selectedPeriodType = FinancialStatementPeriodType.ANNUAL;
        public FinancialStatementPeriodType SelectedPeriodType
        {
            get { return selectedPeriodType; }
            set
            {
                if (selectedPeriodType != value)
                {
                    selectedPeriodType = value;
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
        private ObservableCollection<String> currencyInfo = new ObservableCollection<string> { "USD" };
        public ObservableCollection<String> CurrencyInfo
        {
            get { return currencyInfo; }
            set
            {
                if (currencyInfo != value)
                {
                    currencyInfo = value;
                    RaisePropertyChanged(() => this.CurrencyInfo);
                }
            }
        }

        /// <summary>
        /// Stores selected currency
        /// </summary>
        private String selectedCurrency = "USD";
        public String SelectedCurrency
        {
            get { return selectedCurrency; }
            set
            {
                selectedCurrency = value;
                RaisePropertyChanged(() => this.SelectedCurrency);
                eventAggregator.GetEvent<ConsensusEstimateDetailCurrencyChangeEvent>().Publish(new ChangedCurrencyInEstimateDetail()
                {
                    CurrencyName = value
                });
                RetrieveConsensusEstimatesDetailsData();
            }
        }
        #endregion

        #region Security Information
        private EntitySelectionData entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get { return entitySelectionInfo; }
            set
            {
                entitySelectionInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionInfo);
            }
        }
        #endregion

        #region Busy Indicator
        /// <summary>
        /// if busy indicator is busy or not
        /// </summary>
        private bool busyIndicatorIsBusy;
        public bool BusyIndicatorIsBusy
        {
            get { return busyIndicatorIsBusy; }
            set
            {
                busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// content to show below busy indicator
        /// </summary>
        private string busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return busyIndicatorContent; }
            set
            {
                busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (EntitySelectionInfo != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        dbInteractivity.RetrieveIssuerReferenceData(EntitySelectionInfo, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// handles security changed event
        /// </summary>
        /// <param name="result"></param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    EntitySelectionInfo = result;

                    if (EntitySelectionInfo != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        dbInteractivity.RetrieveIssuerReferenceData(result, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
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

        #region Callback Methods
        /// <summary>
        /// callback method issuer reference data
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveIssuerReferenceDataCallbackMethod(IssuerReferenceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    IssuerReferenceInfo = result;
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + " (" + EntitySelectionInfo.ShortName + " : "
                                                                                                    + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// callback method for consensus estimate method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveConsensusEstimateDetailedDataCallbackMethod(List<ConsensusEstimateDetail> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ConsensusEstimateDetailInfo = result;
                    if (IssuerReferenceInfo != null && IsActive)
                    {
                        dbInteractivity.RetrieveConsensusEstimateDetailedBrokerData(IssuerReferenceInfo.IssuerId, SelectedPeriodType, SelectedCurrency,
                                                                                                        RetrieveConsensusEstimateDetailedBrokerDataCallBackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// callback method for consensus estimate broker detail method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveConsensusEstimateDetailedBrokerDataCallBackMethod(List<ConsensusEstimateDetail> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    BrokerDetailUnpivotInfo = result;
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        /// <summary>
        /// method to fetch data for gadget
        /// </summary>
        private void RetrieveConsensusEstimatesDetailsData()
        {
            if (IssuerReferenceInfo != null && IsActive)
            {
                dbInteractivity.RetrieveConsensusEstimateDetailedData(IssuerReferenceInfo.IssuerId,
                                                                    SelectedPeriodType,
                                                                    SelectedCurrency,
                                                                    RetrieveConsensusEstimateDetailedDataCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Data based on selected security");
            }
        }

        /// <summary>
        /// create display information
        /// </summary>
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

        /// <summary>
        /// method to set busy indicator status
        /// </summary>
        /// <param name="showBusyIndicator"></param>
        /// <param name="message"></param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
            { BusyIndicatorContent = message; }
            BusyIndicatorIsBusy = showBusyIndicator;
        }
        #endregion
    }
}