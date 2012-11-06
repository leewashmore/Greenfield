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
    /// View model for ViewModelFinstat class
    /// </summary>
    public class ViewModelFinstat : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
        private EntitySelectionData entitySelectionData;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelFinstat(DashboardGadgetParam param)
        {
            logger = param.LoggerFacade;
            dbInteractivity = param.DBInteractivity;
            eventAggregator = param.EventAggregator;
            entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            PeriodColumns.PeriodColumnNavigate += (e) =>
            {
                if (e.PeriodColumnNamespace == GetType().FullName)
                {                    
                    Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                    SetFinstatDetailDisplayInfo();                    
                }
            };

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }

            if (entitySelectionData != null)
            {
                HandleSecurityReferenceSetEvent(entitySelectionData);
            }
        } 
        #endregion

        #region Properties       
        #region Security Information
        /// <summary>
        /// IssueName Property
        /// </summary>
        private string issueName;
        public string IssueName
        {
            get { return issueName; }
            set
            {
                if (issueName != value)
                {
                    issueName = value;
                    RaisePropertyChanged(() => this.IssueName);
                }
            }
        }

        /// <summary>
        /// Ticker Property
        /// </summary>
        private string ticker;
        public string Ticker
        {
            get { return ticker; }
            set
            {
                if (ticker != value)
                {
                    ticker = value;
                    RaisePropertyChanged(() => this.Ticker);
                }
            }
        }

        /// <summary>
        /// Country Property
        /// </summary>
        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                if (country != value)
                {
                    country = value;
                    RaisePropertyChanged(() => this.Country);
                }
            }
        }

        /// <summary>
        /// Sector Property
        /// </summary>
        private string sector;
        public string Sector
        {
            get { return sector; }
            set
            {
                if (sector != value)
                {
                    sector = value;
                    RaisePropertyChanged(() => this.Sector);
                }
            }
        }

        /// <summary>
        /// Industry Property
        /// </summary>
        private string industry;
        public string Industry
        {
            get { return industry; }
            set
            {
                if (industry != value)
                {
                    industry = value;
                    RaisePropertyChanged(() => this.Industry);
                }
            }
        }

        /// <summary>
        /// SubIndustry Property
        /// </summary>
        private string subIndustry;
        public string SubIndustry
        {
            get { return subIndustry; }
            set
            {
                if (subIndustry != value)
                {
                    subIndustry = value;
                    RaisePropertyChanged(() => this.SubIndustry);
                }
            }
        }

        /// <summary>
        /// PrimaryAnalyst Property
        /// </summary>
        private string primaryAnalyst;
        public string PrimaryAnalyst
        {
            get { return primaryAnalyst; }
            set
            {
                if (primaryAnalyst != value)
                {
                    primaryAnalyst = value;
                    RaisePropertyChanged(() => this.PrimaryAnalyst);
                }
            }
        }

        /// <summary>
        /// Currency Property
        /// </summary>
        private string currency;
        public string Currency
        {
            get { return currency; }
            set
            {
                if (currency != value)
                {
                    currency = value;
                    RaisePropertyChanged(() => this.Currency);
                }
            }
        }

        /// <summary>
        /// PrimaryAnalyst Property
        /// </summary>
        private string industryAnalyst;
        public string IndustryAnalyst
        {
            get { return industryAnalyst; }
            set
            {
                if (industryAnalyst != value)
                {
                    industryAnalyst = value;
                    RaisePropertyChanged(() => this.IndustryAnalyst);
                }
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

        #region Finstat Detail Information
        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> finstatDetailDisplayInfo;
        public List<PeriodColumnDisplayData> FinstatDetailDisplayInfo
        {
            get { return finstatDetailDisplayInfo; }
            set
            {
                finstatDetailDisplayInfo = value;
                RaisePropertyChanged(() => this.FinstatDetailDisplayInfo);
            }
        }

        /// <summary>
        /// Unpivoted FinstatDetail Information received from stored procedure
        /// </summary>
        private List<FinstatDetailData> finstatDetailInfo;
        public List<FinstatDetailData> FinstatDetailInfo
        {
            get
            {
                if (finstatDetailInfo == null)
                { finstatDetailInfo = new List<FinstatDetailData>(); }
                return finstatDetailInfo;
            }
            set
            {
                if (finstatDetailInfo != value)
                {
                    finstatDetailInfo = value;
                    SetFinstatDetailDisplayInfo();
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
                { periodRecord = PeriodColumns.SetPeriodRecord(Iterator, defaultHistoricalYearCount: 4, netColumnCount: 7, isQuarterImplemented: false); }
                return periodRecord;
            }
            set { periodRecord = value; }
        }
        #endregion

        #region Period Column Headers
        /// <summary>
        /// Stores period column headers
        /// </summary>
        private List<String> periodColumnHeaderInfo;
        public List<String> PeriodColumnHeader
        {
            get
            {
                if (periodColumnHeaderInfo == null)
                { periodColumnHeaderInfo = PeriodColumns.SetColumnHeaders(PeriodRecord, false); }
                return periodColumnHeaderInfo;
            }
            set
            {
                periodColumnHeaderInfo = value;
                RaisePropertyChanged(() => this.PeriodColumnHeader);
                if (value != null)
                {
                    PeriodColumns.RaisePeriodColumnUpdateCompleted(new PeriodColumnUpdateEventArg()
                    {
                        PeriodColumnNamespace = GetType().FullName,
                        PeriodColumnHeader = value,
                        PeriodRecord = PeriodRecord,
                        PeriodIsYearly = true
                    });
                }
            }
        }
        #endregion

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get{ return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (entitySelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        dbInteractivity.RetrieveIssuerReferenceData(entitySelectionData, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        }

        #region Year Range Start Selection
        /// <summary>
        /// property to contain start year range
        /// </summary>
        private List<Int32> startYearRange = new List<Int32>() { (Convert.ToInt32(DateTime.Today.AddYears(-4).Year)) };
        public List<Int32> StartYearRange
        {
            get{ return startYearRange; }
            set
            {
                if (startYearRange != value)
                {
                    startYearRange = value;
                    RaisePropertyChanged(() => StartYearRange);
                }
            }
        }

        /// <summary>
        /// property to conatin selected year range
        /// </summary>
        private Int32 selectedyearRange = Convert.ToInt32(DateTime.Now.AddYears(-4).Year);
        public Int32 SelectedYearRange
        {
            get { return selectedyearRange; }
            set
            {
                if (selectedyearRange != value)
                {
                    selectedyearRange = value;
                    RaisePropertyChanged(() => SelectedYearRange);
                    Iterator = value - DateTime.Today.Year + 4;
                    SetFinstatDetailDisplayInfo();
                }
            }
        } 
        #endregion

        /// <summary>
        /// Displays reprt run date
        /// </summary>
        private string reportRunDate = DateTime.Now.Date.ToShortDateString();
        public string ReportRunDate
        {
            get { return reportRunDate; }
            set { reportRunDate = value; }
        }

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
                    RetrieveFinstatData();
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
                        RetrieveFinstatData();
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
                RetrieveFinstatData();
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
                    { CurrencyInfo.Add(IssuerReferenceInfo.CurrencyCode); }
                    SelectedCurrency = "USD";
                }
            }
        }

        #endregion
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Entity Selection Data
        /// </summary>
        /// <param name="entitySelectionData">EntitySelectionData</param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    entitySelectionData = result;

                    if (entitySelectionData != null && IsActive)
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
        
        #region CallBack Methods
        /// <summary>
        /// Callback method for Security Overview Service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">IssuerReferenceData Collection</param>
        public void RetrieveIssuerReferenceDataCallbackMethod(IssuerReferenceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    IssuerReferenceInfo = result;
                    IssueName = result.IssueName;
                    Ticker = result.Ticker;
                    Country = result.CountryName;
                    Sector = result.SectorName;
                    Industry = result.IndustryName;
                    SubIndustry = result.SubIndustryName;
                    Currency = result.TradingCurrency;
                    PrimaryAnalyst = result.PrimaryAnalyst;
                    IndustryAnalyst = result.IndustryAnalyst;
                }
                else
                {
                    BusyIndicatorNotification();
                    Prompt.ShowDialog("No Issuer linked to the entity " + entitySelectionData.LongName + " (" + entitySelectionData.ShortName + " : "
                        + entitySelectionData.InstrumentID + ")");
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
        /// CallBack method for service method call
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveFinstatDetailDataCallbackMethod(List<FinstatDetailData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    FinstatDetailInfo = result;
                    StartYearRange = new List<Int32>(FinstatDetailInfo.OrderBy(a => a.PeriodYear).Select(a => a.PeriodYear).Distinct().ToList());
                    StartYearRange.Remove(4000);
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                BusyIndicatorNotification();
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// method to retrieve data for gadget
        /// </summary>
        public void RetrieveFinstatData()
        {
            if (IssuerReferenceInfo != null && IsActive)
            {
                dbInteractivity.RetrieveFinstatDetailData(IssuerReferenceInfo.IssuerId,
                                                            (IssuerReferenceInfo.SecurityId).ToString(),
                                                            SelectedDataSource, SelectedFiscalType,
                                                            SelectedCurrency, SelectedYearRange,
                                                            RetrieveFinstatDetailDataCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Data based on selected security");
            }
        }

        /// <summary>
        /// method to create display data
        /// </summary>
        public void SetFinstatDetailDisplayInfo()
        {
            BusyIndicatorNotification(true, "Updating information based on selected preference");
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(Iterator, defaultHistoricalYearCount: 4, netColumnCount: 7, isQuarterImplemented: false);
            FinstatDetailDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo(FinstatDetailInfo, out periodRecord,
                periodRecord, uniqueByGroupDesc: true, additionalFirstDescPropertyName: "HarmonicFirst", additionalSecondDescPropertyName: "HarmonicSecond");
            PeriodRecord = periodRecord;
            PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord, displayPeriodType: false);
            BusyIndicatorNotification();
        }

        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
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