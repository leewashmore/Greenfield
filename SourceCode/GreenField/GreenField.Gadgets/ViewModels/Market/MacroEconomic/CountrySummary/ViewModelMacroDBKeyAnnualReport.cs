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
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.ServiceCaller;
using GreenField.Gadgets.Models;
using GreenField.DataContracts;
using GreenField.Common;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelMacroDBKeyAnnualReport : NotificationObject
    {
        #region Fields
        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;
        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;
        /// <summary>
        /// Country Code of the country selected
        /// </summary>
        private String countryCode;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelMacroDBKeyAnnualReport(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            countryCode = param.DashboardGadgetPayload.CountrySelectionData;
            if (countryCode != null)
            {
                dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportData(countryCode, RetrieveMacroEconomicDataCallbackMethod);
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<CountrySelectionSetEvent>().Subscribe(HandleCountryReferenceSetEvent);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler macroDBKeyAnnualReportCountryDataLoadedEvent;
        /// <summary>
        /// Event for the notification of callback
        /// </summary>
        public event RetrieveMacroCountrySummaryDataCompleteEventHandler RetrieveMacroDataCompletedEvent;
        #endregion

        #region Properties
        #region UI
        /// <summary>
        /// List containing the full macro country data
        /// </summary>
        private List<MacroDatabaseKeyAnnualReportData> macroCountryData;
        public List<MacroDatabaseKeyAnnualReportData> MacroCountryData
        {
            get
            {
                if (macroCountryData == null)
                {
                    macroCountryData = new List<MacroDatabaseKeyAnnualReportData>();
                }
                return macroCountryData;
            }
            set
            {
                macroCountryData = value;
                if (macroCountryData != null)
                {
                    AddDataToFiveYearModels(CurrentYear);
                }
                RaisePropertyChanged(() => this.MacroCountryData);
            }
        }
        
        /// <summary>
        /// List containing data binded to the grid
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
        /// Property that stores the current year value
        /// </summary>
        private int currentYear = System.DateTime.Now.Year;
        public int CurrentYear
        {
            get
            {
                return currentYear;
            }
            set
            {
                currentYear = value;
                if (macroCountryData != null)
                {
                    AddDataToFiveYearModels(value);
                }
                RetrieveMacroDataCompletedEvent(new RetrieveMacroCountrySummaryDataCompleteEventArgs() { MacroInfo = MacroCountryData });
                RaisePropertyChanged(() => this.CurrentYear);
            }
        }

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
                if (isActive != value)
                {
                    isActive = value;
                    if (countryCode != null && IsActive)
                    {
                        if (null != macroDBKeyAnnualReportCountryDataLoadedEvent)
                        {
                            macroDBKeyAnnualReportCountryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportData(countryCode, RetrieveMacroEconomicDataCallbackMethod);
                    }
                }
            }
        }
        #endregion

        #region ICommand
        /// <summary>
        /// On Left Arrow click 
        /// </summary>
        public ICommand LeftNavigationClick
        {
            get
            {
                return new DelegateCommand<object>(MoveLeftCommandMethod);
            }
        }

        /// <summary>
        /// On right Arrow click
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

        #endregion
        #endregion

        #region Command Methods
        /// <summary>
        /// Move right command method
        /// </summary>
        /// <param name="param">param</param>
        public void MoveRightCommandMethod(object param)
        {
            CurrentYear = CurrentYear + 1;
        }
        /// <summary>
        /// Move Left command method
        /// </summary>
        /// <param name="param">param</param>
        public void MoveLeftCommandMethod(object param)
        {
            CurrentYear = CurrentYear - 1;
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method after the result list gets populated
        /// </summary>
        /// <param name="result">result</param>
        public void RetrieveMacroEconomicDataCallbackMethod(List<MacroDatabaseKeyAnnualReportData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            if (result != null && result.Count > 0)
            {
                Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                MacroCountryData = result;
                if (null != macroDBKeyAnnualReportCountryDataLoadedEvent)
                {
                    macroDBKeyAnnualReportCountryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                RetrieveMacroDataCompletedEvent(new RetrieveMacroCountrySummaryDataCompleteEventArgs() { MacroInfo = result });
            }
            else
            {
                Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                if (null != macroDBKeyAnnualReportCountryDataLoadedEvent)
                {
                    macroDBKeyAnnualReportCountryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handler called when a user selects a particular country
        /// </summary>
        /// <param name="CountryData">Country selected by the user</param>
        public void HandleCountryReferenceSetEvent(String CountryData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (CountryData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, CountryData, 1);
                    countryCode = CountryData;

                    if (countryCode != null && IsActive)
                    {
                        if (null != macroDBKeyAnnualReportCountryDataLoadedEvent)
                        {
                            macroDBKeyAnnualReportCountryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportData(countryCode, RetrieveMacroEconomicDataCallbackMethod);
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

        #region Private Method
        /// <summary>
        /// Gets the property value by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns></returns>
        public static T GetProperty<T>(MacroDatabaseKeyAnnualReportData m, string propertyName)
        {
            var theProperty = m.GetType().GetProperty(propertyName);
            if (theProperty == null)
            {
                throw new ArgumentException("object does not have an " + propertyName + " property", "m");
            }
            if (theProperty.PropertyType.FullName != typeof(T).FullName)
            {
                throw new ArgumentException("object has an Id property, but it is not of type " + typeof(T).FullName, "m");
            }
            return (T)theProperty.GetValue(m, null);
        }

        /// <summary>
        /// Filling the FiveYearDataModels type list
        /// </summary>
        /// <param name="CurrentYear">Value of Current System Year</param>
        public void AddDataToFiveYearModels(int CurrentYear)
        {
            if (FiveYearMacroCountryData != null)
            {
                FiveYearMacroCountryData.Clear();
            }
            if (CurrentYear >= 2024 || CurrentYear <= 1989)
            {
                return;
            }
            List<FiveYearDataModels> result = new List<FiveYearDataModels>();
            for (int i = 0; i < macroCountryData.Count; i++)
            {
                int valueOfCurrentYear = DateTime.Now.Year;
                MacroDatabaseKeyAnnualReportData m = new MacroDatabaseKeyAnnualReportData();
                FiveYearDataModels entry = new FiveYearDataModels();
                entry.CategoryName = macroCountryData[i].CATEGORY_NAME;
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
                {
                    Value1 = 0;
                }
                if (Value2 == null)
                {
                    Value2 = 0;
                }
                if (Value3 == null)
                {
                    Value3 = 0;
                }
                if (Value4 == null)
                {
                    Value4 = 0;
                }
                if (Value5 == null)
                {
                    Value5 = 0;
                }
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
            eventAggregator.GetEvent<CountrySelectionSetEvent>().Unsubscribe(HandleCountryReferenceSetEvent);        
        }
        #endregion
    }
}
