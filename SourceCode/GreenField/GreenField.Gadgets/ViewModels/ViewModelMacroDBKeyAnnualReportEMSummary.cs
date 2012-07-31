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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.ServiceCaller.ModelFXDefinitions;
using System.Collections.Generic;
using GreenField.DataContracts;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelMacroDBKeyAnnualReportEMSummary : NotificationObject
    {
        
        #region Fields

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;
        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        private String _countryCode;

        private List<String> _countryNames;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelMacroDBKeyAnnualReportEMSummary(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _countryNames = param.DashboardGadgetPayload.RegionFXData;
            _countryCode = param.DashboardGadgetPayload.CountrySelectionData;

            if ( _countryNames!=null)
            {
                _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(_countryCode,_countryNames, RetrieveMacroEconomicDataEMSummaryCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                //_eventAggregator.GetEvent<CountrySelectionSetEvent>().Subscribe(HandleCountryReferenceSetEvent);
                _eventAggregator.GetEvent<RegionFXEvent>().Subscribe(HandleRegionCountryReferenceSetEvent);
            }
            
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler macroDBKeyAnnualReportEMSummaryDataLoadedEvent;
        public event RetrieveMacroCountrySummaryDataCompleteEventHandler RetrieveMacroEMSummaryDataCompletedEvent;   
        #endregion

        #region Properties

        #region UI
        /// <summary>
        /// Stores the complete Macro Country data
        /// </summary>
        private List<MacroDatabaseKeyAnnualReportData> macroCountryData;
        public List<MacroDatabaseKeyAnnualReportData> MacroCountryData
        {
            get
            {
                
                return macroCountryData;
            }
            set
            {
                macroCountryData = value;
                AddDataToFiveYearModels(CurrentYear);
                RaisePropertyChanged(() => this.MacroCountryData);
            }
        }
        /// <summary>
        /// Stores the Five Year data binded to the grid
        /// </summary>
        private List<FiveYearDataModels> fiveYearMacroCountryData;
        public List<FiveYearDataModels> FiveYearMacroCountryData
        {
            get
            {

                return fiveYearMacroCountryData;
            }
            set
            {
                fiveYearMacroCountryData = value;
                RaisePropertyChanged(() => this.FiveYearMacroCountryData);
            }
        }

        /// <summary>
        /// Stores the value of the current year
        /// </summary>
        private int _currentYear = System.DateTime.Now.Year;
        public int CurrentYear
        {
            get
            {
                return _currentYear;
            }

            set
            {
                _currentYear = value;
                AddDataToFiveYearModels(value);
                RetrieveMacroEMSummaryDataCompletedEvent(new RetrieveMacroCountrySummaryDataCompleteEventArgs() { MacroInfo = MacroCountryData });
                RaisePropertyChanged(() => this.CurrentYear);
            }
        }

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
                    if (_countryNames != null && IsActive)
                    {
                        if (null != macroDBKeyAnnualReportEMSummaryDataLoadedEvent)
                            macroDBKeyAnnualReportEMSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(_countryCode, _countryNames, RetrieveMacroEconomicDataEMSummaryCallbackMethod);
                    }
                }
            }
        }
        #endregion

        #region ICommand
        /// <summary>
        /// On left Arrow Click
        /// </summary>
        public ICommand LeftNavigationClick
        {
            get
            {
                return new DelegateCommand<object>(MoveLeftCommandMethod);
            }
        }
        /// <summary>
        /// On Right Arrow Click
        /// </summary>
        public ICommand RightNavigationClick
        {
            get
            {
                return new DelegateCommand<object>(MoveRightCommandMethod);
            }
        }

        /// <summary>
        /// Move Right
        /// </summary>
        public ICommand MoveRightCommand
        {
            get { return new DelegateCommand<object>(MoveRightCommandMethod); }
        }

        /// <summary>
        /// Move Left
        /// </summary>
        public ICommand MoveLeftCommand
        {
            get { return new DelegateCommand<object>(MoveLeftCommandMethod); }
        }

        #region ICommand Methods
        public void MoveRightCommandMethod(object param)
        {
            CurrentYear = CurrentYear + 1;
        }

        public void MoveLeftCommandMethod(object param)
        {
            CurrentYear = CurrentYear - 1;
        }
        #endregion
        #endregion

        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback Method when the result list gets populated
        /// </summary>
        /// <param name="result">result</param>
        public void RetrieveMacroEconomicDataEMSummaryCallbackMethod(List<MacroDatabaseKeyAnnualReportData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            if (result != null && result.Count > 0)
            {
                Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                MacroCountryData = result;
                if (null != macroDBKeyAnnualReportEMSummaryDataLoadedEvent)
                    macroDBKeyAnnualReportEMSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                RetrieveMacroEMSummaryDataCompletedEvent(new RetrieveMacroCountrySummaryDataCompleteEventArgs() { MacroInfo = result });
            }
            else
            {
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                if (null != macroDBKeyAnnualReportEMSummaryDataLoadedEvent)
                    macroDBKeyAnnualReportEMSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
        }   
        #endregion  

        #region Event handler
        /// <summary>
        /// Handler called when the user selects a region
        /// </summary>
        /// <param name="countryValues">The country Values for a selected region</param>
        public void HandleRegionCountryReferenceSetEvent(List<String> countryValues)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (countryValues != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, countryValues, 1);
                    _countryNames = countryValues;

                    if (_countryNames != null && IsActive )
                    {
                        if (null != macroDBKeyAnnualReportEMSummaryDataLoadedEvent)
                            macroDBKeyAnnualReportEMSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(_countryCode, _countryNames, RetrieveMacroEconomicDataEMSummaryCallbackMethod);
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

        #region ClassMethods

        /// <summary>
        /// Gets the property value by the property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns></returns>
        public static T GetProperty<T>(MacroDatabaseKeyAnnualReportData m, string propertyName)
        {
            var theProperty = m.GetType().GetProperty(propertyName);
            if (theProperty == null)
                throw new ArgumentException("object does not have an " + propertyName + " property", "m");
            if (theProperty.PropertyType.FullName != typeof(T).FullName)
                throw new ArgumentException("object has an Id property, but it is not of type " + typeof(T).FullName, "m");
            return (T)theProperty.GetValue(m, null);
        }
        /// <summary>
        /// Fills the FiveYearDataModels  type list
        /// </summary>
        /// <param name="CurrentYear">Value of the current Year</param>
        public void AddDataToFiveYearModels(int CurrentYear)
        {
            if (FiveYearMacroCountryData != null)
                FiveYearMacroCountryData.Clear();
            List<FiveYearDataModels> result = new List<FiveYearDataModels>();
            if (CurrentYear >= 2024 || CurrentYear <= 1989)
                return;
            for (int i = 0; i < macroCountryData.Count; i++)
            {
                MacroDatabaseKeyAnnualReportData m = new MacroDatabaseKeyAnnualReportData();
                FiveYearDataModels entry = new FiveYearDataModels();
                int valueOfCurrentYear = DateTime.Now.Year;
                entry.CategoryName = macroCountryData[i].COUNTRY_NAME;
                entry.CountryName = macroCountryData[i].COUNTRY_NAME;
                entry.Description = macroCountryData[i].DESCRIPTION;
                entry.DisplayType = macroCountryData[i].DISPLAY_TYPE;
                entry.SortOrder = macroCountryData[i].SORT_ORDER;
                entry.YearOne = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear - 3));
                entry.YearTwo = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear - 2));
                entry.YearThree = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear - 1));
                entry.YearFour = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear));
                entry.YearFive = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 1));
                entry.YearSix = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 2));
                Decimal? Value1 = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (valueOfCurrentYear - 4));
                Decimal? Value2 = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (valueOfCurrentYear - 3));
                Decimal? Value3 = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (valueOfCurrentYear - 2));
                Decimal? Value4 = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (valueOfCurrentYear - 1));
                Decimal? Value5 = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (valueOfCurrentYear));
                if (Value1 == null)
                    Value1 = 0;
                if (Value2 == null)
                    Value2 = 0;
                if (Value3 == null)
                    Value3 = 0;
                if (Value4 == null)
                    Value4 = 0;
                if (Value5 == null)
                    Value5 = 0;
                entry.FiveYearAvg = (Value1 + Value2 + Value3 + Value4 + Value5) / 5;
                result.Add(entry);
            }
            FiveYearMacroCountryData = result;

        }
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<RegionFXEvent>().Unsubscribe(HandleRegionCountryReferenceSetEvent);
        }

        #endregion
    }
}
