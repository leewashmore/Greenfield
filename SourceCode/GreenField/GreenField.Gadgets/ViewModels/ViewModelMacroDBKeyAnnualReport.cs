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
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.ModelFXDefinitions;
using System.Collections.Generic;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;

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
            
        }
        #endregion

        private List<MacroDatabaseKeyAnnualReportData> macroCountryData;
        public List<MacroDatabaseKeyAnnualReportData> MacroCountryData
        {
            get
            {
                if (macroCountryData == null)
                { 
                   _dbInteractivity.RetrieveMacroDatabaseKeyAnnualReportData("AR",RetrieveMacroEconomicDataCallbackMethod);
                   
                }
                return macroCountryData;
            }
            set
            {
                macroCountryData = value;
                RaisePropertyChanged(() => this.MacroCountryData);
            }
        }

        public void RetrieveMacroEconomicDataCallbackMethod(List<MacroDatabaseKeyAnnualReportData> result)
        {
            MacroCountryData = result;
        }
    }
}
