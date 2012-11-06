using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for RelativePerformanceUI gadget
    /// </summary>
    public class ViewModelRelativePerformanceUI : NotificationObject
    {
        #region Private Fields 

        /// <summary>
        /// Instance of Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Instance of Logger Facade
        /// </summary>
        public ILoggerFacade logger;

        /// <summary>
        /// Instance of Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of PortfolioSelectionData
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;

        /// <summary>
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData entitySelectionData;

        /// <summary>
        /// Selected Effective Date
        /// </summary>
        private DateTime? effectiveDate;      

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the class.
        /// </summary>
        /// <param name="param">DashboardGadget payload</param>
        public ViewModelRelativePerformanceUI(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;

            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (portfolioSelectionData != null)
            {
                HandleFundReferenceSet(portfolioSelectionData);
            }
            if (entitySelectionData != null)
            {
                HandleSecurityReferenceSet(entitySelectionData);
            }
            if (effectiveDate != null)
            {
                HandleEffectiveDateSet(Convert.ToDateTime(effectiveDate));
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            }
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return selectedSecurity;
            }
            set
            {
                selectedSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        /// <summary>
        /// Selected Portfolio
        /// </summary>
        private PortfolioSelectionData selectedPortfolio;
        public PortfolioSelectionData SelectedPortfolio
        {
            get
            {
                return selectedPortfolio;
            }
            set
            {
                selectedPortfolio = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolio);
            }
        }

        /// <summary>
        /// Stores the value of Security & Portfolio selected
        /// </summary>
        private Dictionary<string, string> selectedEntityValues;
        public Dictionary<string, string> SelectedEntityValues
        {
            get
            {
                if (selectedEntityValues == null)
                {
                    selectedEntityValues = new Dictionary<string, string>();
                }
                return selectedEntityValues;
            }
            set
            {
                selectedEntityValues = value;
                this.RaisePropertyChanged(() => this.SelectedEntityValues);
            }
        }

        /// <summary>
        /// Selected Date from the tool-bar
        /// </summary>
        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
                this.RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        /// <summary>
        /// Collection of type RelativePerformanceUIData, populates grid
        /// </summary>
        private RangeObservableCollection<RelativePerformanceUIData> relativePerformanceReturnData;
        public RangeObservableCollection<RelativePerformanceUIData> RelativePerformanceReturnData
        {
            get
            {
                if (relativePerformanceReturnData == null)
                {
                    relativePerformanceReturnData = new RangeObservableCollection<RelativePerformanceUIData>();
                }
                return relativePerformanceReturnData;
            }
            set
            {
                relativePerformanceReturnData = value;
                this.RaisePropertyChanged(() => this.RelativePerformanceReturnData);
            }
        }

        /// <summary>
        /// Status of Busy Indicator
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return busyIndicatorStatus;
            }
            set
            {
                busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
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
                isActive = value;
                if (SelectedSecurity != null && SelectedDate != null && SelectedPortfolio != null && SelectedEntityValues != null && isActive)
                {
                    dbInteractivity.RetrieveRelativePerformanceUIData(SelectedEntityValues, SelectedDate, RelativePerformanceUIDataCallbackMethod);
                    BusyIndicatorStatus = true;
                }
            }
        }

        #endregion
               
        #region EventHandlers

        /// <summary>
        /// Handle Fund Change Event
        /// </summary>
        /// <param name="PortfolioSelectionData">Details of Selected Portfolio</param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (PortfolioSelectionData != null)
                {
                    if (SelectedEntityValues.ContainsKey("PORTFOLIO"))
                    {
                        SelectedEntityValues.Remove("PORTFOLIO");
                    }
                    Logging.LogMethodParameter(logger, methodNamespace, PortfolioSelectionData, 1);
                    SelectedPortfolio = PortfolioSelectionData;
                    SelectedEntityValues.Add("PORTFOLIO", PortfolioSelectionData.PortfolioId);
                    if (SelectedSecurity != null && SelectedDate != null && SelectedPortfolio != null && SelectedEntityValues != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceUIData(SelectedEntityValues, SelectedDate, RelativePerformanceUIDataCallbackMethod);
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
        /// Handle Security Change Event
        /// </summary>
        /// <param name="PortfolioSelectionData">Details of Selected Security</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    if (entitySelectionData.InstrumentID == null)
                    {
                        throw new Exception("Security Data Cannot be Fetched for this Security");
                    }
                    if (SelectedEntityValues.ContainsKey("SECURITY"))
                    {
                        SelectedEntityValues.Remove("SECURITY");
                    }
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                    SelectedSecurity = entitySelectionData;
                    SelectedEntityValues.Add("SECURITY", entitySelectionData.LongName);
                    if (SelectedPortfolio != null && SelectedDate != null && SelectedSecurity != null && SelectedEntityValues != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceUIData(SelectedEntityValues, SelectedDate, RelativePerformanceUIDataCallbackMethod);
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
        /// Handle Date Change Event
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectiveDate, 1);
                    SelectedDate = effectiveDate;
                    if (SelectedDate != null && SelectedEntityValues != null && SelectedSecurity != null && SelectedPortfolio != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceUIData(SelectedEntityValues, SelectedDate, RelativePerformanceUIDataCallbackMethod);
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

        #endregion

        #region CallbackMethods

        /// <summary>
        /// Callback method, returns data from Service
        /// </summary>
        /// <param name="result">Collection of type RelativePerformanceUIData</param>
        private void RelativePerformanceUIDataCallbackMethod(List<RelativePerformanceUIData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    RelativePerformanceReturnData.Clear();
                    RelativePerformanceReturnData.AddRange(result);
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

        #region UnsubscribeEvents

        /// <summary>
        /// Unsubscribing the Events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
        }

        #endregion

    }
}