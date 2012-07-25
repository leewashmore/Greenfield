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
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using GreenField.DataContracts;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using System.Linq;
using GreenField.Gadgets.Helpers;
using Microsoft.Practices.Prism.Commands;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelFinancialStatements : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private FinancialStatementType _financialStatementType;
        #endregion

        #region Constructor
        public ViewModelFinancialStatements(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            _financialStatementType = (FinancialStatementType)param.AdditionalInfo;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            ExternalResearchVisibility = _financialStatementType == FinancialStatementType.FUNDAMENTAL_SUMMARY ? Visibility.Collapsed : Visibility.Visible;

            //Event Subscription - PeriodColumnNavigationEvent
            PeriodColumns.PeriodColumnNavigate += new PeriodColumnNavigationEvent(PeriodColumns_PeriodColumnNavigate);

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
        #region IsActive
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
                    if ((EntitySelectionInfo != null) && _isActive)
                    {
                        RaisePropertyChanged(() => this.EntitySelectionInfo);
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        _dbInteractivity.RetrieveIssuerReferenceData(EntitySelectionInfo, RetrieveIssuerReferenceDataCallbackMethod);
                    }
                }
            }
        } 
        #endregion

        #region UI Fields
        /// <summary>
        /// Sets the visibility of External Research Grid
        /// </summary>
        private Visibility _externalResearchVisibility;
        public Visibility ExternalResearchVisibility
        {
            get { return _externalResearchVisibility; }
            set
            {
                _externalResearchVisibility = value;
                RaisePropertyChanged(() => this.ExternalResearchVisibility);
            }
        }        
        #endregion

        #region Financial Statement Information
        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> _financialStatementDisplayInfo;
        public List<PeriodColumnDisplayData> FinancialStatementDisplayInfo
        {
            get { return _financialStatementDisplayInfo; }
            set
            {
                _financialStatementDisplayInfo = value;
                RaisePropertyChanged(() => this.FinancialStatementDisplayInfo);                
            }
        }

        /// <summary>
        /// Pivoted Financial Information to be dispayed on grid for External Research Data
        /// </summary>
        private List<PeriodColumnDisplayData> _financialStatementExtDisplayInfo;
        public List<PeriodColumnDisplayData> FinancialStatementExtDisplayInfo
        {
            get { return _financialStatementExtDisplayInfo; }
            set
            {
                _financialStatementExtDisplayInfo = value;
                RaisePropertyChanged(() => this.FinancialStatementExtDisplayInfo);
            }
        }

        /// <summary>
        /// Unpivoted Financial Information received from stored procedure
        /// </summary>
        private List<FinancialStatementData> _financialStatementInfo;
        public List<FinancialStatementData> FinancialStatementInfo
        {
            get
            {
                if (_financialStatementInfo == null)
                    _financialStatementInfo = new List<FinancialStatementData>();
                return _financialStatementInfo;
            }
            set
            {
                if (_financialStatementInfo != value)
                {
                    _financialStatementInfo = value;
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
        private PeriodRecord _periodRecord;
        public PeriodRecord PeriodRecord
        {
            get
            {
                if (_periodRecord == null)
                    _periodRecord = PeriodColumns.SetPeriodRecord();
                return _periodRecord;
            }
            set { _periodRecord = value; }
        }

        /// <summary>
        /// Stores period column headers
        /// </summary>
        private List<String> _periodColumnHeader;
        public List<String> PeriodColumnHeader
        {
            get
            {
                if (_periodColumnHeader == null)
                    _periodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord);
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
                    CurrencyInfo = new ObservableCollection<String> { IssuerReferenceInfo.CurrencyCode };
                    if (IssuerReferenceInfo.CurrencyCode != "USD")
                        CurrencyInfo.Add("USD");

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
        private FinancialStatementDataSource _selectedDataSource = FinancialStatementDataSource.REUTERS;
        public FinancialStatementDataSource SelectedDataSource
        {
            get { return _selectedDataSource; }
            set
            {
                if (_selectedDataSource != value)
                {
                    _selectedDataSource = value;
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
                RetrieveFinancialStatementData();
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
                if(IsActive)
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

        /// <summary>
        /// PeriodColumnNavigationEvent Event Handler
        /// </summary>
        /// <param name="e">PeriodColumnNavigationEventArg</param>
        private void PeriodColumns_PeriodColumnNavigate(PeriodColumnNavigationEventArg e)
        {
            //Validate namespace before implementation
            if (e.PeriodColumnNamespace == GetType().FullName && IsActive)
            {
                BusyIndicatorNotification(true, "Retrieving data for updated period range");
                Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(incrementFactor: Iterator, defaultHistoricalYearCount: 3
                    , defaultHistoricalQuarterCount: 4, netColumnCount: 6, isQuarterImplemented: true);
                FinancialStatementDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>
                    (FinancialStatementInfo.Where(record => record.IsConsensus == "N").ToList(), out periodRecord, periodRecord, subGroups: null, updatePeriodRecord: true);
                if (_financialStatementType != FinancialStatementType.FUNDAMENTAL_SUMMARY)
                {
                    FinancialStatementExtDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>
                        (FinancialStatementInfo.Where(record => record.IsConsensus == "Y").ToList(), out periodRecord, periodRecord, updatePeriodRecord: false); 
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

        /// <summary>
        /// RetrieveFinancialStatementData Callback Method - Retrieves unpivoted financial information
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveFinancialStatementDataCallbackMethod(List<FinancialStatementData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    FinancialStatementInfo = result;
                }
                else
                {
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
        #endregion

        #region Helper Methods
        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        private void RetrieveFinancialStatementData()
        {
            if (IssuerReferenceInfo != null)
            {
                BusyIndicatorNotification(true, "Retrieving Financial Statement Data for the selected security");
                FinancialStatementInfo = new List<FinancialStatementData>();
                _dbInteractivity.RetrieveFinancialStatementData(IssuerReferenceInfo.IssuerId, SelectedDataSource, SelectedPeriodType, SelectedFiscalType,
                            _financialStatementType, SelectedCurrency, RetrieveFinancialStatementDataCallbackMethod);
            }
        }

        public void SetFinancialStatementDisplayInfo()
        {
            if (FinancialStatementInfo.Count.Equals(0))
            {
                FinancialStatementDisplayInfo = new List<PeriodColumnDisplayData>();
                FinancialStatementExtDisplayInfo = new List<PeriodColumnDisplayData>();
                BusyIndicatorNotification();
                return;
            }

            BusyIndicatorNotification(true, "Updating Financial Statement Information based on selected preference");
            
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(Iterator);
            FinancialStatementDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>
                (FinancialStatementInfo.Where(record => record.IsConsensus == "N").ToList(), out periodRecord, periodRecord);

            if (_financialStatementType != FinancialStatementType.FUNDAMENTAL_SUMMARY)
            {
                FinancialStatementExtDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo<FinancialStatementData>
                        (FinancialStatementInfo.Where(record => record.IsConsensus == "Y").ToList(), out periodRecord, periodRecord, updatePeriodRecord: false); 
            }

            PeriodRecord = periodRecord;
            PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord);
            
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
