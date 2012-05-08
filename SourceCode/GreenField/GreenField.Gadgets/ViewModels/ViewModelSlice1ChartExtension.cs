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
using GreenField.Common.Helper;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelSlice1ChartExtension : NotificationObject
    {
        //MEF Singletons
        #region Fields

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// Details of Selected Portfolio
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;

        /// <summary>
        /// Selected Date
        /// </summary>
        private DateTime? _effectiveDate;

        /// <summary>
        /// Details of selected Security
        /// </summary>
        private EntitySelectionData _entitySelectionData;


        #endregion


        public ViewModelSlice1ChartExtension(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
        }
    }
}
