using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// View-Model for ConsensusValuations
    /// </summary>
    public class ViewModelValuations : NotificationObject
    {
        #region PrivateFields

        /// <summary>
        /// Instance of Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// LoggerFacade
        /// </summary>
        private ILoggerFacade logger;
        public ILoggerFacade Logger
        {
            get
            {
                return logger;
            }
            set
            {
                logger = value;
            }
        }

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelValuations(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;

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

        #region PropertyDeclaration

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get
            {
                return entitySelectionInfo;
            }
            set
            {
                entitySelectionInfo = value;
                this.RaisePropertyChanged(() => this.EntitySelectionInfo);
            }
        }

        private List<PeriodColumnGroupingDetail> dataGrouping;
        public List<PeriodColumnGroupingDetail> DataGrouping
        {
            get
            {
                if (dataGrouping == null)
                {
                    dataGrouping = new List<PeriodColumnGroupingDetail>();
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Amount", 
                        GroupPropertyName = "Amount", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "YOY", 
                        GroupPropertyName = "YOYGrowth", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE });
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "AshmoreEMM",
                        GroupPropertyName = "AshmoreEmmAmount", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Variance%", 
                        GroupPropertyName = "Variance", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL_PERCENTAGE });

                    /* No data
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Actual", 
                        GroupPropertyName = "Actual", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "# of Estimates", 
                        GroupPropertyName = "NumberOfEstimates", 
                        GroupDataType = PeriodColumnGroupingType.INT });
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "High", 
                        GroupPropertyName = "High", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Low", 
                        GroupPropertyName = "Low", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL });
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Standard Deviation TODO3", 
                        GroupPropertyName = "StandardDeviation", 
                        GroupDataType = PeriodColumnGroupingType.DECIMAL })
                    */
                    
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Consensus Last Update", 
                        GroupPropertyName = "DataSourceDate", 
                        GroupDataType = PeriodColumnGroupingType.SHORT_DATETIME });
                    dataGrouping.Add(new PeriodColumnGroupingDetail() { GroupDisplayName = "Reported Currency", 
                        GroupPropertyName = "SourceCurrency", 
                        GroupDataType = PeriodColumnGroupingType.STRING });                    
                }
                return dataGrouping;
            }
        }
        
        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> consensusEstimateDetailDisplayInfo;
        public List<PeriodColumnDisplayData> ConsensusEstimateDetailDisplayInfo
        {
            get
            {
                if (consensusEstimateDetailDisplayInfo == null)
                {
                    consensusEstimateDetailDisplayInfo = new List<PeriodColumnDisplayData>();
                }
                return consensusEstimateDetailDisplayInfo;
            }
            set
            {
                consensusEstimateDetailDisplayInfo = value;
                RaisePropertyChanged(() => this.ConsensusEstimateDetailDisplayInfo);
            }
        }

        /// <summary>
        /// ConsensusEstimates Detailed Info
        /// </summary>
        private List<ConsensusEstimatesValuations> consensusEstimateDetailInfo;
        public List<ConsensusEstimatesValuations> ConsensusEstimateDetailInfo
        {
            get
            {
                return consensusEstimateDetailInfo;
            }
            set
            {
                consensusEstimateDetailInfo = value;
                SetConsensusEstimateMedianDisplayInfo();
            }
        }

        #region DasboardActivityStatus

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (isActive)
                {
                    if (EntitySelectionInfo != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        dbInteractivity.RetrieveIssuerReferenceData(EntitySelectionInfo, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        }

        #endregion

        #region Busy Indicator
        /// <summary>
        /// Busy Indicator Status
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
        /// Busy Indicator Content
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
            get
            {
                return selectedCurrency;
            }
            set
            {
                selectedCurrency = value;
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
                    RetrieveConsensusEstimatesValuationData();
                }
            }
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

        #region Period Information
        /// <summary>
        /// Iteration Count
        /// </summary>
        public Int32 Iterator { get; set; }

        /// <summary>
        /// Period Record storing period information based on iteration
        /// </summary>
        private PeriodRecord periodRecord;
        public PeriodRecord PeriodRecord
        {
            get
            {
                if (periodRecord == null)
                {
                    periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
                }
                return periodRecord;
            }
            set { periodRecord = value; }
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
                        CurrencyInfo = new ObservableCollection<String>();
                        CurrencyInfo.Add("USD");
                        if (IssuerReferenceInfo.CurrencyCode != "USD")
                        {
                            CurrencyInfo.Add(IssuerReferenceInfo.CurrencyCode);
                        }
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
        /// IssuerReferenceData callback Method
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
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + 
                        " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
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

        /// <summary>
        /// ConsensusEstimateData callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveConsensusEstimateDataCallbackMethod(List<ConsensusEstimatesValuations> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ConsensusEstimateDetailInfo = result;
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
            finally
            {
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// Event Unsubscribe
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);            
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Set Display Data
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
            {
                BusyIndicatorContent = message;
            }
            BusyIndicatorIsBusy = showBusyIndicator;
        }

        /// <summary>
        /// Web Service Call Method
        /// </summary>
        private void RetrieveConsensusEstimatesValuationData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (IssuerReferenceInfo != null)
                {
                    if (IssuerReferenceInfo.IssuerId == null)
                    {
                        throw new Exception("Unable to retrieve issuer reference data for the selected security");
                    }
                    dbInteractivity.RetrieveConsensusEstimatesValuationsData
                        (IssuerReferenceInfo.IssuerId, Convert.ToString(EntitySelectionInfo.LongName), SelectedPeriodType, string.IsNullOrEmpty(SelectedCurrency) ? "USD" : SelectedCurrency, RetrieveConsensusEstimateDataCallbackMethod);
                    BusyIndicatorNotification(true, "Updating information based on selected Security");
                }
                else
                {
                    throw new Exception("Issuer reference data not specified");
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        #endregion
    }
}
