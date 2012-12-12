using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// View Model for ViewFinancialStatements
    /// </summary>
    public class ViewModelFinancialStatements : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Service caller instance
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Loggerfacade instance
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// Financial Statement Type
        /// </summary>
        private FinancialStatementType financialStatementType;
        #endregion

        #region Properties
        #region IsActive
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
                    if ((EntitySelectionInfo != null) && isActive)
                    {
                        RaisePropertyChanged(() => this.EntitySelectionInfo);
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        dbInteractivity.RetrieveIssuerReferenceData(EntitySelectionInfo, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        }
        #endregion

        #region UI Fields
        /// <summary>
        /// Sets the visibility of External Research Grid
        /// </summary>
        private Visibility externalResearchVisibility;
        public Visibility ExternalResearchVisibility
        {
            get { return externalResearchVisibility; }
            set
            {
                externalResearchVisibility = value;
                RaisePropertyChanged(() => this.ExternalResearchVisibility);
            }
        }
        #endregion

        #region Financial Statement Information
        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> financialStatementDisplayInfo;
        public List<PeriodColumnDisplayData> FinancialStatementDisplayInfo
        {
            get { return financialStatementDisplayInfo; }
            set
            {
                financialStatementDisplayInfo = value;
                RaisePropertyChanged(() => this.FinancialStatementDisplayInfo);
            }
        }

        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid for External Research Data
        /// </summary>
        private List<PeriodColumnDisplayData> financialStatementExtDisplayInfo;
        public List<PeriodColumnDisplayData> FinancialStatementExtDisplayInfo
        {
            get { return financialStatementExtDisplayInfo; }
            set
            {
                financialStatementExtDisplayInfo = value;
                RaisePropertyChanged(() => this.FinancialStatementExtDisplayInfo);
            }
        }

        /// <summary>
        /// Unpivoted Financial Information received from stored procedure
        /// </summary>
        private List<FinancialStatementData> financialStatementInfo;
        public List<FinancialStatementData> FinancialStatementInfo
        {
            get
            {
                if (financialStatementInfo == null)
                {
                    financialStatementInfo = new List<FinancialStatementData>();
                }
                return financialStatementInfo;
            }
            set
            {
                if (financialStatementInfo != value)
                {
                    financialStatementInfo = value;
                    SetFinancialStatementDisplayInfo();
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
                    periodRecord = PeriodColumns.SetPeriodRecord();
                }
                return periodRecord;
            }
            set { periodRecord = value; }
        }

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
                    periodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord);
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
                issuerReferenceInfo = value;
                if (value != null)
                {
                    CurrencyInfo = new ObservableCollection<String> { "USD" };
                    if (IssuerReferenceInfo.CurrencyCode != "USD")
                    {
                        CurrencyInfo.Add(IssuerReferenceInfo.CurrencyCode);
                    }
                    SelectedCurrency = CurrencyInfo[0];
                }
            }
        }
        #endregion

        #region Data Source
        /// <summary>
        /// Stores FinancialStatementDataSource Enum Items
        /// </summary>
        public List<FinancialStatementDataSource> DataSourceInfo
        {
            get { return EnumUtils.GetEnumDescriptions<FinancialStatementDataSource>(); }
        }

        /// <summary>
        /// Stores selected FinancialStatementDataSource
        /// </summary>
        private FinancialStatementDataSource selectedDataSource = FinancialStatementDataSource.PRIMARY;
        public FinancialStatementDataSource SelectedDataSource
        {
            get { return selectedDataSource; }
            set
            {
                if (selectedDataSource != value)
                {
                    selectedDataSource = value;
                    RaisePropertyChanged(() => this.SelectedDataSource);
                    RetrieveFinancialStatementData();
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
                    RetrieveFinancialStatementData();
                }
            }
        }
        #endregion

        #region Calendarization Option
        /// <summary>
        /// Stores FinancialStatementFiscalType Enum Items
        /// </summary>
        public List<FinancialStatementFiscalType> FiscalTypeInfo
        {
            get { return EnumUtils.GetEnumDescriptions<FinancialStatementFiscalType>(); }
        }

        /// <summary>
        /// Stores selected FinancialStatementFiscalType
        /// </summary>
        private FinancialStatementFiscalType selectedFiscalType = FinancialStatementFiscalType.FISCAL;
        public FinancialStatementFiscalType SelectedFiscalType
        {
            get { return selectedFiscalType; }
            set
            {
                if (selectedFiscalType != value)
                {
                    if (selectedFiscalType != value)
                    {
                        selectedFiscalType = value;
                        RaisePropertyChanged(() => this.SelectedFiscalType);
                        RetrieveFinancialStatementData();
                    }
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
                RetrieveFinancialStatementData();
            }
        }
        #endregion

        #region Security Information
        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get { return entitySelectionInfo; }
            set
            {
                entitySelectionInfo = value;
                if (IsActive)
                {
                    RaisePropertyChanged(() => this.EntitySelectionInfo);
                }
            }
        }
        #endregion

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool isBusyIndicatorBusy = false;
        public bool IsBusyIndicatorBusy
        {
            get { return isBusyIndicatorBusy; }
            set
            {
                isBusyIndicatorBusy = value;
                RaisePropertyChanged(() => this.IsBusyIndicatorBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor that initializes the class
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelFinancialStatements(DashboardGadgetParam param)
        {
            logger = param.LoggerFacade;
            dbInteractivity = param.DBInteractivity;
            eventAggregator = param.EventAggregator;
            financialStatementType = (FinancialStatementType)param.AdditionalInfo;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            ExternalResearchVisibility = financialStatementType == FinancialStatementType.FUNDAMENTAL_SUMMARY ? Visibility.Collapsed : Visibility.Visible;

            //Event Subscription - PeriodColumnNavigationEvent
            PeriodColumns.PeriodColumnNavigate += new PeriodColumnNavigationEvent(PeriodColumns_PeriodColumnNavigate);

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

        #region Event Handlers
        /// <summary>
        /// Security change Event
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

        /// <summary>
        /// PeriodColumnNavigationEvent Event Handler
        /// </summary>
        /// <param name="e">PeriodColumnNavigationEventArg</param>
        private void PeriodColumns_PeriodColumnNavigate(PeriodColumnNavigationEventArg e)
        {
            //validate namespace before implementation
            if (e.PeriodColumnNamespace == GetType().FullName && IsActive)
            {
                BusyIndicatorNotification(true, "Retrieving data for updated period range");
                Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(incrementFactor: Iterator, defaultHistoricalYearCount: 3
                    , defaultHistoricalQuarterCount: 4, netColumnCount: 6, isQuarterImplemented: true);

                var nList = FinancialStatementInfo.Where(record => record.IsConsensus == "N").ToList();
                this.SetDecimals(nList, 1);

                FinancialStatementDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>(nList, out periodRecord, periodRecord, subGroups: null, updatePeriodRecord: true);
                if (financialStatementType != FinancialStatementType.FUNDAMENTAL_SUMMARY)
                {
                    var yList = FinancialStatementInfo.Where(record => record.IsConsensus == "Y").ToList();
                    this.SetDecimals(yList, 1);
                    FinancialStatementExtDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>(yList, out periodRecord, periodRecord, updatePeriodRecord: false);
                }
                PeriodRecord = periodRecord;
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord);
                BusyIndicatorNotification();
            }
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// RetrieveIssuerReferenceData Callback Method - assigns IssuerReferenceInfo and calls RetrieveFinancialStatementData
        /// </summary>
        /// <param name="result">IssuerReferenceData</param>
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
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
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
        /// RetrieveFinancialStatementData Callback Method - Retrieves unpivoted financial information
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveFinancialStatementDataCallbackMethod(List<FinancialStatementData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    FinancialStatementInfo = result;
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
        #endregion

        #region Helper Methods
        /// <summary>
        /// Event Subscribe
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        /// <summary>
        /// Service Call
        /// </summary>
        private void RetrieveFinancialStatementData()
        {
            if (IssuerReferenceInfo != null)
            {
                BusyIndicatorNotification(true, "Retrieving Financial Statement Data for the selected security");
                dbInteractivity.RetrieveFinancialStatementData(IssuerReferenceInfo.IssuerId, SelectedDataSource, SelectedPeriodType, SelectedFiscalType,
                            financialStatementType, SelectedCurrency, RetrieveFinancialStatementDataCallbackMethod);
            }
        }

        /// <summary>
        /// Sets financial statement display info
        /// </summary>
        public void SetFinancialStatementDisplayInfo()
        {
            BusyIndicatorNotification(true, "Updating Financial Statement Information based on selected preference");

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(Iterator);
            var nList = FinancialStatementInfo.Where(record => record.IsConsensus == "N").ToList();
            this.SetDecimals(nList, 1);
            FinancialStatementDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>(nList, out periodRecord, periodRecord);

            if (financialStatementType != FinancialStatementType.FUNDAMENTAL_SUMMARY)
            {
                var yList = FinancialStatementInfo.Where(record => record.IsConsensus == "Y").ToList();
                this.SetDecimals(yList, 1);
                FinancialStatementExtDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>(yList, out periodRecord, periodRecord, updatePeriodRecord: false);
            }
            PeriodRecord = periodRecord;
            PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord);
            BusyIndicatorNotification();
        }

        private void SetDecimals(List<FinancialStatementData> list, Int32 numberOfDecimals)
        {
            foreach (var item in list)
            {
                item.Decimals = numberOfDecimals;
            }
        }

        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="isBusyIndicatorVisible">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        private void BusyIndicatorNotification(bool isBusyIndicatorVisible = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            IsBusyIndicatorBusy = isBusyIndicatorVisible;
        }
        #endregion
    }
}
