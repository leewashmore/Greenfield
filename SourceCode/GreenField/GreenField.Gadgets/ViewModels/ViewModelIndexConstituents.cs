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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.ObjectModel;
using GreenField.Common;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewIndexConstituents class
    /// </summary>
    public class ViewModelIndexConstituents : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;
        #endregion

        #region Constructor
         /// <summary>
         /// Constructor
         /// </summary>
         /// <param name="param">DashboardGadgetparam</param>
         public ViewModelIndexConstituents(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (EffectiveDate != null && _portfolioSelectionData != null)
            {
                _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, _effectiveDate, RetrieveIndexConstituentsDataCallbackMethod);
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            }
        } 
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// contains all data to be displayed in the gadget
        /// </summary>
        private ObservableCollection<IndexConstituentsData> _indexConstituentsInfo;
        public ObservableCollection<IndexConstituentsData> IndexConstituentsInfo
        {
            get { return _indexConstituentsInfo; }
            set
            {
                if (_indexConstituentsInfo != value)
                {
                    _indexConstituentsInfo = value;
                    RaisePropertyChanged(() => this.IndexConstituentsInfo);
                }
            }
        }

        /// <summary>
        /// effective date selected
        /// </summary>
        private DateTime _effectiveDate;
        public DateTime EffectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                if (_effectiveDate != value)
                {
                    _effectiveDate = value;
                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }      
  
        #endregion
        #endregion

        #region Event Handlers

        /// <summary>
        /// Event Handler to subscribed event 'EffectiveDateSet'
        /// </summary>
        /// <param name="effectiveDate">DateTime</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, effectiveDate, 1);
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && _portfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, _effectiveDate, RetrieveIndexConstituentsDataCallbackMethod);
                        if (IndexConstituentDataLoadEvent != null)
                            IndexConstituentDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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

        /// <summary>
        /// Event Handler to subscribed event 'PortfolioReferenceSetEvent'
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, portfolioSelectionData, 1);
                    _portfolioSelectionData = portfolioSelectionData;
                    if (EffectiveDate != null && _portfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, _effectiveDate, RetrieveIndexConstituentsDataCallbackMethod);
                        if (IndexConstituentDataLoadEvent != null)
                            IndexConstituentDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method for RetrieveIndexConstituentsData service call
        /// </summary>
        /// <param name="indexConstituentsData">IndexConstituentsData collection</param>
        private void RetrieveIndexConstituentsDataCallbackMethod(List<IndexConstituentsData> indexConstituentsData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (indexConstituentsData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, indexConstituentsData, 1);
                    IndexConstituentsInfo = new ObservableCollection<IndexConstituentsData>(indexConstituentsData);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (IndexConstituentDataLoadEvent != null)
                    IndexConstituentDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        } 
        #endregion         

        #region Event
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler IndexConstituentDataLoadEvent;

        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
        }
        
        #endregion
    }

   
}
