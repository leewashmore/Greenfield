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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelEstimates : NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// Instance of Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData _entitySelectionData;

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Selected Date
        /// </summary>
        private DateTime? _effectiveDate;


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">Dashboard Gadget Payload</param>
        public ViewModelEstimates(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;

            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
        }

        #endregion
    }
}
