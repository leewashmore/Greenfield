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
            _countryCode = param.DashboardGadgetPayload.CountrySelectionData;

            if (_countryCode != null)
            {
                _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(_countryCode, RetrieveMacroEconomicDataEMSummaryCallbackMethod);
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
        public event DataRetrievalProgressIndicatorEventHandler macroDBKeyAnnualReportEMSummaryDataLoadedEvent;
        #endregion

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

        public void AddDataToFiveYearModels(int CurrentYear)
        {
            if (FiveYearMacroCountryData != null)
                FiveYearMacroCountryData.Clear();
            List<FiveYearDataModels> result = new List<FiveYearDataModels>();
            for (int i = 0; i < macroCountryData.Count; i++)
            {
                MacroDatabaseKeyAnnualReportData m = new MacroDatabaseKeyAnnualReportData();
                FiveYearDataModels entry = new FiveYearDataModels();
                entry.CATEGORY_NAME = macroCountryData[i].CATEGORY_NAME;
                entry.COUNTRY_NAME = macroCountryData[i].COUNTRY_NAME;
                entry.DESCRIPTION = macroCountryData[i].DESCRIPTION;
                entry.DISPLAY_TYPE = macroCountryData[i].DISPLAY_TYPE;
                entry.SORT_ORDER = macroCountryData[i].SORT_ORDER;
                entry.YEAR_ONE = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + CurrentYear);
                entry.YEAR_TWO = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 1));
                entry.YEAR_THREE = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 2));
                entry.YEAR_FOUR = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 3));
                entry.YEAR_FIVE = GetProperty<Decimal?>(macroCountryData[i], "YEAR_" + (CurrentYear + 4));
                result.Add(entry);
            }
            FiveYearMacroCountryData = result;

        }

        #region ICommand


        public ICommand MoveRightCommand
        {
            get { return new DelegateCommand<object>(MoveRightCommandMethod); }
        }


        public ICommand MoveLeftCommand
        {
            get { return new DelegateCommand<object>(MoveLeftCommandMethod); }
        }

        #endregion

        private void MoveRightCommandMethod(object param)
        {
            CurrentYear = CurrentYear + 1;
        }

        private void MoveLeftCommandMethod(object param)
        {
            CurrentYear = CurrentYear - 1;
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
                AddDataToFiveYearModels(value);
                RetrieveMacroEMSummaryDataCompletedEvent(new RetrieveMacroCountrySummaryDataCompleteEventArgs() { MacroInfo = MacroCountryData });
                RaisePropertyChanged(() => this.CurrentYear);
            }
        }

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

        public event RetrieveMacroCountrySummaryDataCompleteEventHandler RetrieveMacroEMSummaryDataCompletedEvent;


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
                        if (null != macroDBKeyAnnualReportEMSummaryDataLoadedEvent)
                            macroDBKeyAnnualReportEMSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(_countryCode, RetrieveMacroEconomicDataEMSummaryCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
