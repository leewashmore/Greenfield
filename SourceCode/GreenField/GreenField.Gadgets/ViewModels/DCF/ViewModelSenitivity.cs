using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using System.Collections.Generic;
using Telerik.Windows.Controls.Charting;
using System.Collections.ObjectModel;
using GreenField.DataContracts;
using System.Linq;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model Sensitivity
    /// </summary>
    public class ViewModelSenitivity:NotificationObject
    {
        #region PrivateVariables

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

        #endregion

        public ViewModelSenitivity(DashboardGadgetParam param)
        {

        }


        #region PropertyDeclaration

        public ILoggerFacade Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        private EntitySelectionData _entitySelectionData;

        public EntitySelectionData EntitySelectionData
        {
            get 
            {
                return _entitySelectionData; 
            }
            set
            {
                _entitySelectionData = value; 
            //this.RaisePropertyChanged(()=>this.
            }
        }
        


        #endregion

    }
}
