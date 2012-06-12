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
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.ModelFXDefinitions;
using System.Collections.Generic;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.Models;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelMacroDBKeyAnnualReport : NotificationObject
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


        #endregion
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelMacroDBKeyAnnualReport(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _countryCode = param.DashboardGadgetPayload.CountrySelectionData;

            if (_countryCode != null)
            {
                _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportData(_countryCode, RetrieveMacroEconomicDataCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<CountrySelectionSetEvent>().Subscribe(HandleCountryReferenceSetEvent);
            }

        }
        #endregion
        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler macroDBKeyAnnualReportCountryDataLoadedEvent;
        #endregion

        private List<MacroDatabaseKeyAnnualReportData> macroCountryData;
        public List<MacroDatabaseKeyAnnualReportData> MacroCountryData
        {
            get
            {
                if (macroCountryData == null)
                    macroCountryData = new List<MacroDatabaseKeyAnnualReportData>();
                return macroCountryData;
            }
            set
            {
                macroCountryData = value;
                //today =  System.DateTime.Now.Year;  
                if (macroCountryData != null)
                    AddDataToFiveYearModels(CurrentYear);
                RaisePropertyChanged(() => this.MacroCountryData);


            }
        }

        public void AddDataToFiveYearModels(int CurrentYear)
        {
            if (FiveYearMacroCountryData != null)
                FiveYearMacroCountryData.Clear();
            List<FiveYearDataModels> result = new List<FiveYearDataModels>();
            for (int i = 0; i < macroCountryData.Count; i++)
            {
                MacroDatabaseKeyAnnualReportData m = new MacroDatabaseKeyAnnualReportData();
                FiveYearDataModels entry = new FiveYearDataModels();
                entry.CategoryName = macroCountryData[i].CATEGORY_NAME;
                entry.CountryName = macroCountryData[i].COUNTRY_NAME;
                entry.Description = macroCountryData[i].DESCRIPTION;
                entry.DisplayType = macroCountryData[i].DISPLAY_TYPE;
                entry.SortOrder = macroCountryData[i].SORT_ORDER;
                entry.YearOne = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + CurrentYear);
                entry.YearTwo = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 1));
                entry.YearThree = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 2));
                entry.YearFour = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 3));
                entry.YearFive = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 4));
                if (entry.YearOne == null)
                    entry.YearOne = 0;
                if (entry.YearTwo == null)
                    entry.YearTwo = 0;
                if (entry.YearThree == null)
                    entry.YearThree = 0;
                if (entry.YearFour == null)
                    entry.YearFour = 0;
                if (entry.YearFive == null)
                    entry.YearFive = 0;
                entry.FiveYearAvg = (entry.YearOne + entry.YearTwo + entry.YearThree + entry.YearFour + entry.YearFive) / 5;
                result.Add(entry);
            }
            FiveYearMacroCountryData = result;

        }

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
                if (macroCountryData != null)
                    AddDataToFiveYearModels(value);
                RetrieveMacroDataCompletedEvent(new RetrieveMacroCountrySummaryDataCompleteEventArgs() { MacroInfo = MacroCountryData });
                RaisePropertyChanged(() => this.CurrentYear);
            }
        }

        #region ICommand

        public ICommand LeftNavigationClick
        {
            get
            {
                return new DelegateCommand<object>(MoveLeftCommandMethod);
            }
        }

        public ICommand RightNavigationClick
        {
            get
            {
                return new DelegateCommand<object>(MoveRightCommandMethod);
            }
        }


        public ICommand MoveRightCommand
        {
            get { return new DelegateCommand<object>(MoveRightCommandMethod); }
        }


        public ICommand MoveLeftCommand
        {
            get { return new DelegateCommand<object>(MoveLeftCommandMethod); }
        }

        #endregion

        public void MoveRightCommandMethod(object param)
        {
            CurrentYear = CurrentYear + 1;
        }

        public void MoveLeftCommandMethod(object param)
        {
            CurrentYear = CurrentYear - 1;
        }

        public void RetrieveMacroEconomicDataCallbackMethod(List<MacroDatabaseKeyAnnualReportData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            if (result != null && result.Count > 0)
            {
                Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                MacroCountryData = result;
                if (null != macroDBKeyAnnualReportCountryDataLoadedEvent)
                    macroDBKeyAnnualReportCountryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                RetrieveMacroDataCompletedEvent(new RetrieveMacroCountrySummaryDataCompleteEventArgs() { MacroInfo = result });
            }
            else
            {
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                if (null != macroDBKeyAnnualReportCountryDataLoadedEvent)
                    macroDBKeyAnnualReportCountryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }


        }

        public event RetrieveMacroCountrySummaryDataCompleteEventHandler RetrieveMacroDataCompletedEvent;


        public void HandleCountryReferenceSetEvent(String CountryData)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (CountryData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, CountryData, 1);
                    _countryCode = CountryData;

                    if (_countryCode != null)
                    {
                        if (null != macroDBKeyAnnualReportCountryDataLoadedEvent)
                            macroDBKeyAnnualReportCountryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportData(_countryCode, RetrieveMacroEconomicDataCallbackMethod);
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

        public static T GetProperty<T>(MacroDatabaseKeyAnnualReportData m, string propertyName)
        {
            var theProperty = m.GetType().GetProperty(propertyName);
            if (theProperty == null)
                throw new ArgumentException("object does not have an " + propertyName + " property", "m");
            if (theProperty.PropertyType.FullName != typeof(T).FullName)
                throw new ArgumentException("object has an Id property, but it is not of type " + typeof(T).FullName, "m");
            return (T)theProperty.GetValue(m, null);
        }

    }
}
