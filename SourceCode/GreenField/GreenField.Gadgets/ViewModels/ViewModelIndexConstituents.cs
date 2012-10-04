using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

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
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool lookThruEnabled = false;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive 
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if ((portfolioSelectionData != null) && (EffectiveDate != null) && isActive)
                    {
                        dbInteractivity.RetrieveIndexConstituentsData(portfolioSelectionData, Convert.ToDateTime(effectiveDateInfo), lookThruEnabled,
                            RetrieveIndexConstituentsDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetparam</param>
        public ViewModelIndexConstituents(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;
            if ((portfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
            {
                dbInteractivity.RetrieveIndexConstituentsData(portfolioSelectionData, Convert.ToDateTime(effectiveDateInfo),lookThruEnabled, 
                    RetrieveIndexConstituentsDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// contains all data to be displayed in the gadget
        /// </summary>
        private ObservableCollection<IndexConstituentsData> indexConstituentsInfo;
        public ObservableCollection<IndexConstituentsData> IndexConstituentsInfo
        {
            get { return indexConstituentsInfo; }
            set
            {
                if (indexConstituentsInfo != value)
                {
                    indexConstituentsInfo = value;
                    RaisePropertyChanged(() => this.IndexConstituentsInfo);
                }
            }
        }

        /// <summary>
        /// DashboardGadgetPayLoad field
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;
        public PortfolioSelectionData PortfolioSelectionData
        {
            get { return portfolioSelectionData; }
            set
            {
                if (portfolioSelectionData != value)
                {
                    portfolioSelectionData = value;
                    RaisePropertyChanged(() => PortfolioSelectionData);
                }
            }
        }

        /// <summary>
        /// effective date selected
        /// </summary>
        private DateTime? effectiveDateInfo;
        public DateTime? EffectiveDate
        {
            get { return effectiveDateInfo; }
            set
            {
                if (effectiveDateInfo != value)
                {
                    effectiveDateInfo = value;
                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }

        /// <summary>
        /// benchmarkId for portfolio selected
        /// </summary>
        private string benchmarkId;
        public string BenchmarkId
        {
            get { return benchmarkId; }
            set 
            {
                if (benchmarkId != value)
                {
                    benchmarkId = value;
                    RaisePropertyChanged(() => BenchmarkId);
                }
            }
        }

        /// <summary>
        /// property to contain status value for busy indicator of the gadget
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get { return busyIndicatorStatus; }
            set
            {
                if (busyIndicatorStatus != value)
                {
                    busyIndicatorStatus = value;
                    RaisePropertyChanged(() => BusyIndicatorStatus);
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectiveDate, 1);
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && PortfolioSelectionData != null && IsActive)
                    {
                        dbInteractivity.RetrieveIndexConstituentsData(portfolioSelectionData, Convert.ToDateTime(effectiveDateInfo),lookThruEnabled, 
                            RetrieveIndexConstituentsDataCallbackMethod);
                        BusyIndicatorStatus = true;
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

        /// <summary>
        /// Event Handler to subscribed event 'PortfolioReferenceSetEvent'
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portfolioSelectionData, 1);
                    PortfolioSelectionData = portfolioSelectionData;
                    if (EffectiveDate != null && PortfolioSelectionData != null && IsActive)
                    {
                        dbInteractivity.RetrieveIndexConstituentsData(portfolioSelectionData, Convert.ToDateTime(effectiveDateInfo), lookThruEnabled, 
                            RetrieveIndexConstituentsDataCallbackMethod);
                        BusyIndicatorStatus = true;
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

        /// <summary>
        /// Event Handler for LookThru Status
        /// </summary>
        /// <param name="enableLookThru">True: LookThru Enabled/False: LookThru Disabled</param>
        public void HandleLookThruReferenceSetEvent(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(logger, methodNamespace, enableLookThru, 1);
                lookThruEnabled = enableLookThru;

                if (EffectiveDate != null && PortfolioSelectionData != null && IsActive)
                {
                    dbInteractivity.RetrieveIndexConstituentsData(portfolioSelectionData, Convert.ToDateTime(effectiveDateInfo), lookThruEnabled,
                        RetrieveIndexConstituentsDataCallbackMethod);
                    BusyIndicatorStatus = true;
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

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveIndexConstituentsData service call
        /// </summary>
        /// <param name="indexConstituentsData">IndexConstituentsData collection</param>
        private void RetrieveIndexConstituentsDataCallbackMethod(List<IndexConstituentsData> indexConstituentsData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (indexConstituentsData != null && indexConstituentsData.Count != 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, indexConstituentsData, 1);
                    IndexConstituentsInfo = new ObservableCollection<IndexConstituentsData>(indexConstituentsData);
                    BenchmarkId = IndexConstituentsInfo.ElementAt(0).BenchmarkId.ToString();
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
            finally
            {
                BusyIndicatorStatus = false;
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion             

        #region Dispose Method
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }
        #endregion
    }   
}
