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
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Collections.ObjectModel;
using GreenField.DataContracts;
using System.Collections.Generic;
using GreenField.DataContracts.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using System.Linq;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for COASpecific Gadgets With Period Columns
    /// </summary>
    public class ViewModelCOASpecific : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private String defaultGadgetDesc;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelCOASpecific(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;

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

        void PeriodColumns_PeriodColumnNavigate(PeriodColumnNavigationEventArg e)
        {
            if (e.PeriodColumnNamespace == GetType().FullName)           
            {               
                Iterator = e.PeriodColumnNavigationDirection == NavigationDirection.LEFT ? Iterator - 1 : Iterator + 1;
                SetCOASpecificDisplayInfo();
            }
        }
        #endregion

        #region Properties

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
                RetrieveCOASpecificData();

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
                if (IsActive)
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
                    RetrieveCOASpecificData();
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
                        RetrieveCOASpecificData();
                    }
                }
            }
        }
        #endregion

        #region COASpecificData List


        private List<String> coaSpecificGadgetNameInfo;
        public List<String> COASpecificGadgetNameInfo
        {
            get { return coaSpecificGadgetNameInfo; }
            set
            {
                if (coaSpecificGadgetNameInfo != value)
                {
                    coaSpecificGadgetNameInfo = value;
                    RaisePropertyChanged(() => this.COASpecificGadgetNameInfo);
                }
            }

        }

        private String selectedCOASpecificGadgetNameInfo;
        public String SelectedCOASpecificGadgetNameInfo
        {
            get { return selectedCOASpecificGadgetNameInfo; }
            set
            {
                if (selectedCOASpecificGadgetNameInfo != value)
                {
                    selectedCOASpecificGadgetNameInfo = value;
                    COASpecificFilteredInfo = new ObservableCollection<COASpecificData>(COASpecificInfo.Where(t => t.GroupDescription == value));
                    AddToComboBoxSeries.Clear();
                    RaisePropertyChanged(() => this.SelectedCOASpecificGadgetNameInfo);
                }
            }
        }


        private List<COASpecificData> coaSpecificInfo;
        public List<COASpecificData> COASpecificInfo
        {
            get { return coaSpecificInfo; }
            set
            {
                if (coaSpecificInfo != value)
                {
                    coaSpecificInfo = value;
                    COASpecificGadgetNameInfo = value.Select(t => t.GroupDescription).Distinct().ToList();
                    SelectedCOASpecificGadgetNameInfo = value.Select(t => t.GroupDescription).FirstOrDefault();                   
                    RaisePropertyChanged(() => this.COASpecificInfo);
                    SetCOASpecificDisplayInfo();
                }
            }

        }

        private ObservableCollection<COASpecificData> coaSpecificFilterdInfo = new ObservableCollection<COASpecificData>();
        public ObservableCollection<COASpecificData> COASpecificFilteredInfo
        {
            get { return coaSpecificFilterdInfo; }
            set
            {
                if (value != null)
                {
                    coaSpecificFilterdInfo = value;
                    List<String> defaultSeries = COASpecificFilteredInfo.Select(t => t.Description).Distinct().ToList();
                    ComparisonSeries.Clear();
                    foreach (String t in defaultSeries)
                    {
                        GadgetWithPeriodColumns entry = new GadgetWithPeriodColumns();
                        entry.GridId = null;
                        entry.GadgetName = null;
                        entry.GadgetDesc = t;
                        entry.Amount = null;
                        entry.PeriodYear = null;
                        ComparisonSeries.Add(entry);
                    }
                    RaisePropertyChanged(() => this.COASpecificFilteredInfo);
                }
            }
        }

        private ObservableCollection<GadgetWithPeriodColumns> comparisonSeries = new ObservableCollection<GadgetWithPeriodColumns>();
        public ObservableCollection<GadgetWithPeriodColumns> ComparisonSeries
        {
            get { return comparisonSeries; }
            set
            {
                comparisonSeries = value;
                RaisePropertyChanged(() => this.ComparisonSeries);
            }
        }

        private ObservableCollection<String> addToComboBoxSeries = new ObservableCollection<String>();
        public ObservableCollection<String> AddToComboBoxSeries
        {
            get { return addToComboBoxSeries; }
            set
            {
                addToComboBoxSeries = value;
                RaisePropertyChanged(() => this.AddToComboBoxSeries);
            }
        }

        /// <summary>
        /// Pivoted COA Specific  Information to be dispayed on grid
        /// </summary>
        private List<PeriodColumnDisplayData> _coaSpecificDisplayInfo;
        public List<PeriodColumnDisplayData> COASpecificDisplayInfo
        {
            get { return _coaSpecificDisplayInfo; }
            set
            {
                _coaSpecificDisplayInfo = value;
                RaisePropertyChanged(() => this.COASpecificDisplayInfo);
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
                        PeriodIsYearly = true
                    });
                }
            }
        }
        #endregion

        #region ICommand
        /// <summary>
        /// Delete Series from Chart
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod); }
        }

        /// <summary>
        /// Add to chart method
        /// </summary>
        public ICommand AddCommand
        {
            get { return new DelegateCommand<object>(AddCommandMethod); }
        }
        #endregion

        /// <summary>
        /// Stores selected Series From Combo Box
        /// </summary>
        private String _selectedSeriesCB;
        public String SelectedSeriesCB
        {
            get { return _selectedSeriesCB; }
            set
            {
                _selectedSeriesCB = value;
                RaisePropertyChanged(() => this.SelectedSeriesCB);
            }
        }

        #endregion

        #region CallbackMethods

        void RetrieveCOASpecificDataCallbackMethod(List<COASpecificData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    COASpecificInfo = result;                    
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

        #region Helper Methods

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

        private void RetrieveCOASpecificData()
        {
            if (IssuerReferenceInfo != null)
            {
                COASpecificInfo = new List<COASpecificData>();
                BusyIndicatorNotification(true, "Retrieving Data based on selected security");
                _dbInteractivity.RetrieveCOASpecificData(IssuerReferenceInfo.IssuerId, IssuerReferenceInfo.SecurityId, SelectedDataSource, SelectedFiscalType, SelectedCurrency, RetrieveCOASpecificDataCallbackMethod);
               
            }
        }

        public void SetCOASpecificDisplayInfo()
        {
            BusyIndicatorNotification(true, "Updating information based on selected preference");
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(Iterator, 3, 4, 6, false);
            COASpecificDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo(COASpecificInfo, out periodRecord, periodRecord, subGroups: null);
            PeriodRecord = periodRecord;
            PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord, true);
            BusyIndicatorNotification();
        }

        #endregion

        #region ICommand Methods

        /// <summary>
        /// Delete Series from Chart
        /// </summary>
        /// <param name="param"></param>
        private void DeleteCommandMethod(object param)
        {
            GadgetWithPeriodColumns a = param as GadgetWithPeriodColumns;
            List<COASpecificData> removeItem = new List<COASpecificData>();
            removeItem = COASpecificFilteredInfo.Where(w => w.Description == a.GadgetDesc).ToList();
            if (removeItem != null)
                foreach (COASpecificData r in removeItem)
                {
                    COASpecificFilteredInfo.Remove(r);
                }
            ComparisonSeries.Remove(a);
            AddToComboBoxSeries.Add(a.GadgetDesc);

        }

        /// <summary>
        /// Add to Chart Command Method
        /// </summary>
        /// <param name="param"></param>            
        private void AddCommandMethod(object param)
        {
            GadgetWithPeriodColumns entry = new GadgetWithPeriodColumns();

            if (SelectedSeriesCB != null)
            {
                entry.GridId = null;
                entry.GadgetName = null;
                entry.GadgetDesc = SelectedSeriesCB;
                entry.Amount = null;
                entry.PeriodYear = null;
                ComparisonSeries.Add(entry);
            }
            List<COASpecificData> addItem = new List<COASpecificData>();

            if (selectedCOASpecificGadgetNameInfo == null)
                addItem = (COASpecificInfo.Where(t => t.GroupDescription == defaultGadgetDesc && t.Description == SelectedSeriesCB)).ToList();
            else
                addItem = (COASpecificInfo.Where(t => t.GroupDescription == selectedCOASpecificGadgetNameInfo && t.Description == SelectedSeriesCB)).ToList();
            if (addItem != null)
                foreach (COASpecificData r in addItem)
                {
                    COASpecificFilteredInfo.Add(r);
                }
            AddToComboBoxSeries.Remove(SelectedSeriesCB);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler coaSpecificDataLoadedEvent;
        #endregion
    }
}
