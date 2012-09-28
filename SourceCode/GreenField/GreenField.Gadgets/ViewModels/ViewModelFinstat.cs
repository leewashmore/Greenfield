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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.Helpers;

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
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private EntitySelectionData _entitySelectionData;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelFinstat(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            PeriodColumns.PeriodColumnNavigate += (e) =>
            {
                if (e.PeriodColumnNamespace == GetType().FullName)
                {                    
                    Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                    SetFinstatDetailDisplayInfo();                    
                }
            };

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }

            if (_entitySelectionData != null)
            {
                HandleSecurityReferenceSetEvent(_entitySelectionData);
            }
        } 
        #endregion

        #region Properties
       
        #region Security Information
        /// <summary>
        /// IssueName Property
        /// </summary>
        private string _issueName;
        public string IssueName
        {
            get { return _issueName; }
            set
            {
                if (_issueName != value)
                    _issueName = value;
                RaisePropertyChanged(() => this.IssueName);
            }
        }

        /// <summary>
        /// Ticker Property
        /// </summary>
        private string _ticker;
        public string Ticker
        {
            get { return _ticker; }
            set
            {
                if (_ticker != value)
                    _ticker = value;
                RaisePropertyChanged(() => this.Ticker);
            }
        }

        /// <summary>
        /// Country Property
        /// </summary>
        private string _country;
        public string Country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                    _country = value;
                RaisePropertyChanged(() => this.Country);
            }
        }

        /// <summary>
        /// Sector Property
        /// </summary>
        private string _sector;
        public string Sector
        {
            get { return _sector; }
            set
            {
                if (_sector != value)
                    _sector = value;
                RaisePropertyChanged(() => this.Sector);
            }
        }

        /// <summary>
        /// Industry Property
        /// </summary>
        private string _industry;
        public string Industry
        {
            get { return _industry; }
            set
            {
                if (_industry != value)
                    _industry = value;
                RaisePropertyChanged(() => this.Industry);
            }
        }

        /// <summary>
        /// SubIndustry Property
        /// </summary>
        private string _subIndustry;
        public string SubIndustry
        {
            get { return _subIndustry; }
            set
            {
                if (_subIndustry != value)
                    _subIndustry = value;
                RaisePropertyChanged(() => this.SubIndustry);
            }
        }

        /// <summary>
        /// PrimaryAnalyst Property
        /// </summary>
        private string _primaryAnalyst;
        public string PrimaryAnalyst
        {
            get { return _primaryAnalyst; }
            set
            {
                if (_primaryAnalyst != value)
                    _primaryAnalyst = value;
                RaisePropertyChanged(() => this.PrimaryAnalyst);
            }
        }

        /// <summary>
        /// Currency Property
        /// </summary>
        private string _currency;
        public string Currency
        {
            get { return _currency; }
            set
            {
                if (_currency != value)
                    _currency = value;
                RaisePropertyChanged(() => this.Currency);
            }
        }

        /// <summary>
        /// PrimaryAnalyst Property
        /// </summary>
        private string _industryAnalyst;
        public string IndustryAnalyst
        {
            get { return _industryAnalyst; }
            set
            {
                if (_industryAnalyst != value)
                    _industryAnalyst = value;
                RaisePropertyChanged(() => this.IndustryAnalyst);
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

        #region Finstat Detail Information
        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> _finstatDetailDisplayInfo;
        public List<PeriodColumnDisplayData> FinstatDetailDisplayInfo
        {
            get { return _finstatDetailDisplayInfo; }
            set
            {
                _finstatDetailDisplayInfo = value;
                RaisePropertyChanged(() => this.FinstatDetailDisplayInfo);
            }
        }

        /// <summary>
        /// Unpivoted FinstatDetail Information received from stored procedure
        /// </summary>
        private List<FinstatDetailData> _finstatDetailInfo;
        public List<FinstatDetailData> FinstatDetailInfo
        {
            get
            {
                if (_finstatDetailInfo == null)
                    _finstatDetailInfo = new List<FinstatDetailData>();
                return _finstatDetailInfo;
            }
            set
            {
                if (_finstatDetailInfo != value)
                {
                    _finstatDetailInfo = value;
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
        private PeriodRecord _periodRecord;
        public PeriodRecord PeriodRecord
        {
            get
            {
                if (_periodRecord == null)
                    _periodRecord = PeriodColumns.SetPeriodRecord(Iterator, defaultHistoricalYearCount: 4, netColumnCount: 7, isQuarterImplemented: false);
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
                        PeriodIsYearly = true
                        //PeriodIsYearly = SelectedPeriodType == FinancialStatementPeriodType.ANNUAL
                    });
                }
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
                    if (_entitySelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        _dbInteractivity.RetrieveIssuerReferenceData(_entitySelectionData, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        }

        #region Year Range Start Selection

        private List<Int32> _startYearRange = new List<Int32>() { (Convert.ToInt32(DateTime.Today.AddYears(-4).Year)) };
        public List<Int32> StartYearRange
        {
            get
            {
                return _startYearRange;                
            }
            set
            {
                if (_startYearRange != value)
                {
                    _startYearRange = value;
                    RaisePropertyChanged(() => StartYearRange);
                }
            }
        }


        private Int32 _selectedyearRange = Convert.ToInt32(DateTime.Now.AddYears(-4).Year);
        public Int32 SelectedYearRange
        {
            get { return _selectedyearRange; }
            set
            {
                if (_selectedyearRange != value)
                {
                    _selectedyearRange = value;
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
        private string _reportRunDate = DateTime.Now.Date.ToShortDateString();
        public string ReportRunDate
        {
            get { return _reportRunDate; }
            set { _reportRunDate = value; }
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
        private FinancialStatementDataSource _selectedDataSource = FinancialStatementDataSource.PRIMARY;
        public FinancialStatementDataSource SelectedDataSource
        {
            get { return _selectedDataSource; }
            set
            {
                if (_selectedDataSource != value)
                {
                    _selectedDataSource = value;
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
        private FinancialStatementFiscalType _selectedFiscalType = FinancialStatementFiscalType.FISCAL;
        public FinancialStatementFiscalType SelectedFiscalType
        {
            get { return _selectedFiscalType; }
            set
            {
                if (_selectedFiscalType != value)
                {
                    if (_selectedFiscalType != value)
                    {
                        _selectedFiscalType = value;
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
                RetrieveFinstatData();
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
                _issuerReferenceInfo = value;
                if (value != null)
                {
                    CurrencyInfo = new ObservableCollection<String> { "USD" };
                    if (IssuerReferenceInfo.CurrencyCode != "USD")
                        CurrencyInfo.Add(IssuerReferenceInfo.CurrencyCode);
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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    _entitySelectionData = result;

                    if (_entitySelectionData != null && IsActive)
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
        
        #region CallBack Methods
        /// <summary>
        /// Callback method for Security Overview Service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">IssuerReferenceData Collection</param>
        public void RetrieveIssuerReferenceDataCallbackMethod(IssuerReferenceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
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
                    Prompt.ShowDialog("No Issuer linked to the entity " + _entitySelectionData.LongName + " (" + _entitySelectionData.ShortName + " : " + _entitySelectionData.InstrumentID + ")");
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

        /// <summary>
        /// CallBack method for service method call
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveFinstatDetailDataCallbackMethod(List<FinstatDetailData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                   // FinstatDetailInfo = new List<FinstatDetailData>();
                    FinstatDetailInfo = result;
                    StartYearRange = new List<Int32>(FinstatDetailInfo.OrderBy(a => a.PeriodYear).Select(a => a.PeriodYear).Distinct().ToList());
                    StartYearRange.Remove(4000);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                BusyIndicatorNotification();
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }

            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Helper Methods
        public void RetrieveFinstatData()
        {
            if (IssuerReferenceInfo != null && IsActive)
            {
                _dbInteractivity.RetrieveFinstatDetailData(IssuerReferenceInfo.IssuerId,
                                                            (IssuerReferenceInfo.SecurityId).ToString(),
                                                            SelectedDataSource, SelectedFiscalType,
                                                            SelectedCurrency, SelectedYearRange,
                                                            RetrieveFinstatDetailDataCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Data based on selected security");
            }
        }

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

        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
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