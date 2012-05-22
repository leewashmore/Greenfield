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
using GreenField.Common.Helper;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelCommodityIndex : NotificationObject
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
        /// <summary>
        /// Private member object of CommodityResult for Commodity
        /// </summary>
        private List<FXCommodityData> _commodityData;

        #endregion
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelCommodityIndex(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _dbInteractivity.RetrieveCommodityData(RetrieveCommodityDataCallbackMethod);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Stores Commodity Data
        /// </summary>
        public List<FXCommodityData> CommodityData
        {
            get
            {                            
                return _commodityData;
            }
            set
            {
                _commodityData = value;
                RaisePropertyChanged(() => this.CommodityData);
            }
        }
        #endregion

        #region Event
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler CommodityDataLoadEvent;

        #endregion
        #region Callback Method
        /// <summary>
        /// Callback method that assigns value to the Commodity property
        /// </summary>
        /// <param name="result">contains the Commodity data </param>
        public void RetrieveCommodityDataCallbackMethod(List<FXCommodityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {

                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CommodityData = result;
                    this.RaisePropertyChanged(() => this.CommodityData);
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
        #endregion

    }
}
