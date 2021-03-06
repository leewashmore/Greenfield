﻿using System;
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
using GreenField.Common;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Collections.Generic;
using GreenField.Gadgets.Models;
using System.Linq;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelFundamentalSummary : NotificationObject
    {

        #region PrivateVariables
        //MEF Singletons

        /// <summary>
        /// Instance of DBInteractivity
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData _entitySelectionData;

        private FinancialStatementType _financialStatementType;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelFundamentalSummary(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            _financialStatementType = (FinancialStatementType)param.AdditionalInfo;

            PeriodColumns.PeriodColumnNavigate += (e) =>
            {
                if (e.PeriodColumnNamespace == GetType().FullName)
                {
                    BusyIndicatorNotification(true, "Retrieving data for updated time span");
                    PeriodRecord periodRecord = new PeriodRecord();

                    FinancialStatementDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo(FinancialStatementInfo, out periodRecord,
                        PeriodColumns.SetPeriodRecord(e.PeriodColumnNavigationDirection == PeriodColumns.NavigationDirection.LEFT
                            ? --Iterator : ++Iterator));

                    PeriodRecord = periodRecord;
                    PeriodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord);
                    BusyIndicatorNotification();
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

        #region PropertyDeclaration

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
            }
        }


        #region Properties
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
                FinancialStatementExtDisplayInfo = value
                    .Where(record => record.DATA_ID == 147 ||
                        record.DATA_ID == 149 ||
                        record.DATA_ID == 132 ||
                        record.DATA_ID == 133)
                    .ToList();
            }
        }

        private List<PeriodColumnDisplayData> _financialStatementExtDisplayInfo;
        public List<PeriodColumnDisplayData> FinancialStatementExtDisplayInfo
        {
            get { return _financialStatementExtDisplayInfo; }
            set
            {
                if (_financialStatementExtDisplayInfo != value)
                {
                    _financialStatementExtDisplayInfo = value;
                    RaisePropertyChanged(() => this.FinancialStatementExtDisplayInfo);
                }
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
                    _periodColumnHeader = PeriodColumns.SetColumnHeaders(PeriodRecord);
                return _periodColumnHeader;
            }
            set
            {
                _periodColumnHeader = value;
                RaisePropertyChanged(() => this.PeriodColumnHeader);
                if (value != null)
                {
                    PeriodColumns.RaisePeriodColumnUpdateCompleted(new PeriodColumns.PeriodColumnUpdateEventArg()
                    {
                        PeriodColumnNamespace = GetType().FullName,
                        EntitySelectionData = EntitySelectionInfo,
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
                if (_selectedCurrency != value)
                {
                    if (_selectedCurrency != value)
                    {
                        _selectedCurrency = value;
                        RaisePropertyChanged(() => this.SelectedCurrency);
                        RetrieveFinancialStatementData();
                    }
                }
            }
        }
        #endregion

        #region Security Information
        /// <summary>
        /// Selected Security from Toolbar
        /// </summary>
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
        /// Busy indicator Content
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
                    if (EntitySelectionInfo != null)
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

        private void RetrieveFinancialStatementData()
        {
            if (IssuerReferenceInfo != null)
            {
                _dbInteractivity.RetrieveFinancialStatementData(IssuerReferenceInfo.IssuerId, SelectedDataSource, SelectedPeriodType, SelectedFiscalType,
                            _financialStatementType, SelectedCurrency, RetrieveFinancialStatementDataCallbackMethod);
            }
        }

        public void SetFinancialStatementDisplayInfo()
        {
            BusyIndicatorNotification(true, "Updating Financial Statement Information based on selected preference");

            PeriodRecord periodRecord = new PeriodRecord();
            FinancialStatementDisplayInfo = PeriodColumns.SetPeriodColumnDisplayInfo(FinancialStatementInfo, out periodRecord,
                PeriodColumns.SetPeriodRecord(Iterator));
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
