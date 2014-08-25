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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common.Helper;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using System.Collections.Generic;
using GreenField.DataContracts;
using System.Linq;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelMarketCapitalization : NotificationObject
    {
        #region Fields
        //MEF Singletons

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;

        /// <summary>
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData mktCapDataFilter;

        /// <summary>
        /// Private member to store mkt cap data
        /// </summary>
        private MarketCapitalizationData marketCapitalizationInfo;

        /// <summary>
        /// Private member to store effective date
        /// </summary>
        private DateTime? effectiveDate;

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool isExCashSecurity = false;

        /// <summary>
        /// Private member to enable or disble lookthru 
        /// </summary>

        private bool lookThruEnabled = false;

        /// <summary>
        /// Private member to store market cap gadget visibilty
        /// </summary>
        private Visibility marketCapGadgetVisibility = Visibility.Collapsed;


        #endregion

        #region Constructor
        public ViewModelMarketCapitalization(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;

            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;            
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            mktCapDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            IsExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;
                        
            if (effectiveDate != null && portfolioSelectionData != null && IsActive)// && _mktCapDataFilter != null)
            {
                CallingWebMethod();                
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);                
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Stores data for Mkt Cap grid
        /// </summary>
        public MarketCapitalizationData MarketCapitalizationInfo
        {
            get { return marketCapitalizationInfo; }
            set
            {
                if (marketCapitalizationInfo != value)
                {
                    marketCapitalizationInfo = value;
                    PopulateMarketCapitalizationDataDictionary();
                    RaisePropertyChanged(() => this.MarketCapitalizationInfo);
                }
            }
        }

       /// <summary>
       /// Stores Market cap gadget visibility 
       /// </summary>            
        public Visibility MarketCapGadgetVisibility
        {
            get
            {
                return marketCapGadgetVisibility; 
            }
            set
            {
                marketCapGadgetVisibility = value;
                RaisePropertyChanged(() => this.MarketCapGadgetVisibility);                
            }
        }
        
        /// <summary>
        /// Portfolio selected by user
        /// </summary>       
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
        ///Effective Date as selected by the user 
        /// </summary>       
        public DateTime? EffectiveDate
        {
            get
            {
                return effectiveDate;
            }
            set
            {
                if (effectiveDate != value)
                {
                    effectiveDate = value;
                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }

        /// <summary>
        /// Stores securities including or excluding cash type
        /// </summary>        
        public bool IsExCashSecurity
        {
            get { return isExCashSecurity; }
            set
            {
                if (isExCashSecurity != value)
                {
                    isExCashSecurity = value;                    
                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }
        /// <summary>
        /// Check to include LookThru or Not
        /// </summary>
        private bool enableLookThru;
        public bool EnableLookThru
        {
            get { return enableLookThru; }
            set
            {
                enableLookThru = value;
                this.RaisePropertyChanged(() => this.EnableLookThru);
            }
        }
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive 
        { 
            get{ return isActive; }
            set 
            {
                isActive = value;               
                if (effectiveDate != null && portfolioSelectionData != null && isActive)
                {
                    CallingWebMethod();                    
                }
            }
        }

        /// <summary>
        /// Holds the MarketCap Data points and corresponding values
        /// </summary>
        private List<MarketCapitalizationDataPoint> marketCapDataList;
        public List<MarketCapitalizationDataPoint> MarketCapDataList
        {
            get
            {
                if (marketCapDataList == null)
                {
                    marketCapDataList = new List<MarketCapitalizationDataPoint>();
                }
                return marketCapDataList;
            }
            set
            {
                marketCapDataList = value;
                RaisePropertyChanged(() => this.MarketCapDataList);
            }
        }

        #endregion
        #endregion

        #region Event
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler MarketCapitalizationDataLoadEvent;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, PortfolioSelectionData, 1);
                    portfolioSelectionData = PortfolioSelectionData;
                    if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                    {
                        CallingWebMethod();                       
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
        /// Assigns UI Field Properties based on Selected Effective Date
        /// </summary>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        public void HandleEffectiveDateSet(DateTime effectiveDateValue)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectiveDate, 1);
                    effectiveDate = effectiveDateValue;
                    if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                    {
                        CallingWebMethod();                       
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pairs consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, filterSelectionData, 1);
                    mktCapDataFilter = filterSelectionData;
                    if (MarketCapitalizationDataLoadEvent != null)
                            MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    if (effectiveDate != null && portfolioSelectionData != null && mktCapDataFilter != null && IsActive)
                    {
                        CallingWebMethod();                        
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

        #region Callback Methods

        /// <summary>
        /// Callback method that assigns value to the MarketCapitalizationInfo property
        /// </summary>
        /// <param name="result">contains the market capitalization data </param>
        private void RetrieveMarketCapitalizationDataCallbackMethod(List<MarketCapitalizationData> marketCapitalizationData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                MarketCapitalizationInfo = null;
                MarketCapGadgetVisibility = Visibility.Collapsed;
                if (marketCapitalizationData != null && marketCapitalizationData.Count > 0)
                {
                    MarketCapGadgetVisibility = Visibility.Visible;
                    Logging.LogMethodParameter(logger, methodNamespace, marketCapitalizationData, 1);
                    MarketCapitalizationInfo = marketCapitalizationData.FirstOrDefault();
                    this.RaisePropertyChanged(() => this.MarketCapitalizationInfo);
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
                if (MarketCapitalizationDataLoadEvent != null)
                    MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        public void HandleExCashSecuritySetEvent(bool isExCashSec)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(logger, methodNamespace, isExCashSec, 1);
                IsExCashSecurity = isExCashSec;
                if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                {
                    CallingWebMethod();                    
                }

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
                EnableLookThru = enableLookThru;

                if (effectiveDate != null && portfolioSelectionData != null && IsActive)//&& _mktCapDataFilter != null)
                {
                    CallingWebMethod();                    
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

        #region SERVICE CALL METOHD
        /// <summary>
        /// Calls web service method
        /// </summary>
        private void CallingWebMethod()
        {
            if (MarketCapitalizationDataLoadEvent != null)
            {
                MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            }
            if (mktCapDataFilter != null)
            {
                dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), mktCapDataFilter.Filtertype, mktCapDataFilter.FilterValues, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
            }
            else
            {
                dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), null, null, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
            }

        }
        #endregion  

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            eventAggregator.GetEvent<MarketCapitalizationSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
            eventAggregator.GetEvent<ExCashSecuritySetEvent>().Unsubscribe(HandleExCashSecuritySetEvent);
            eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }

        #endregion

        #region private methods
        private void PopulateMarketCapitalizationDataDictionary()
        {
            if (MarketCapitalizationInfo != null)
            {
                List<MarketCapitalizationDataPoint> marketCapValues = new List<MarketCapitalizationDataPoint>();

                MarketCapitalizationDataPoint weightedAvg = new MarketCapitalizationDataPoint();
                weightedAvg.Name = "Weighted Average";
                weightedAvg.PortfolioValue = MarketCapitalizationInfo.PortfolioWtdAvg.ToString("N0");
                weightedAvg.BenchmarkValue = MarketCapitalizationInfo.BenchmarkWtdAvg.ToString("N0");
                marketCapValues.Add(weightedAvg);

                MarketCapitalizationDataPoint weightedMedian = new MarketCapitalizationDataPoint();
                weightedMedian.Name = "Weighted Median";
                weightedMedian.PortfolioValue = MarketCapitalizationInfo.PortfolioWtdMedian.ToString("N0");
                weightedMedian.BenchmarkValue = MarketCapitalizationInfo.BenchmarkWtdMedian.ToString("N0");
                marketCapValues.Add(weightedMedian);

                MarketCapitalizationDataPoint mega = new MarketCapitalizationDataPoint();
                mega.Name = String.Format("Mega > {0}", MarketCapitalizationInfo.LargeRange);
                mega.PortfolioValue = String.Format("{0}%", MarketCapitalizationInfo.PortfolioSumMegaRange.ToString("N2"));
                mega.BenchmarkValue = String.Format("{0}%", MarketCapitalizationInfo.BenchmarkSumMegaRange.ToString("N2"));
                marketCapValues.Add(mega);

                MarketCapitalizationDataPoint large = new MarketCapitalizationDataPoint();
                large.Name = String.Format("Large {0}-{1}", MarketCapitalizationInfo.MediumRange, MarketCapitalizationInfo.LargeRange);
                large.PortfolioValue = String.Format("{0}%", MarketCapitalizationInfo.PortfolioSumLargeRange.ToString("N2"));
                large.BenchmarkValue = String.Format("{0}%", MarketCapitalizationInfo.BenchmarkSumLargeRange.ToString("N2"));
                marketCapValues.Add(large);

                MarketCapitalizationDataPoint medium = new MarketCapitalizationDataPoint();
                medium.Name = String.Format("Medium {0}-{1}", MarketCapitalizationInfo.SmallRange, MarketCapitalizationInfo.MediumRange);
                medium.PortfolioValue = String.Format("{0}%", MarketCapitalizationInfo.PortfolioSumMediumRange.ToString("N2"));
                medium.BenchmarkValue = String.Format("{0}%", MarketCapitalizationInfo.BenchmarkSumMediumRange.ToString("N2"));
                marketCapValues.Add(medium);

                MarketCapitalizationDataPoint small = new MarketCapitalizationDataPoint();
                small.Name = String.Format("Small {0}-{1}", MarketCapitalizationInfo.MicroRange, MarketCapitalizationInfo.SmallRange);
                small.PortfolioValue = String.Format("{0}%", MarketCapitalizationInfo.PortfolioSumSmallRange.ToString("N2"));
                small.BenchmarkValue = String.Format("{0}%", MarketCapitalizationInfo.BenchmarkSumSmallRange.ToString("N2"));
                marketCapValues.Add(small);

                MarketCapitalizationDataPoint micro = new MarketCapitalizationDataPoint();
                micro.Name = String.Format("Micro < {0}", MarketCapitalizationInfo.MicroRange);
                micro.PortfolioValue = String.Format("{0}%", MarketCapitalizationInfo.PortfolioSumMicroRange.ToString("N2"));
                micro.BenchmarkValue = String.Format("{0}%", MarketCapitalizationInfo.BenchmarkSumMicroRange.ToString("N2"));
                marketCapValues.Add(micro);

                MarketCapitalizationDataPoint undefined = new MarketCapitalizationDataPoint();
                undefined.Name = "Undefined";
                undefined.PortfolioValue = String.Format("{0}%", MarketCapitalizationInfo.PortfolioSumUndefinedRange.ToString("N2"));
                undefined.BenchmarkValue = String.Format("{0}%", MarketCapitalizationInfo.BenchmarkSumUndefinedRange.ToString("N2"));
                marketCapValues.Add(undefined);               

                MarketCapDataList.AddRange(marketCapValues);
            }
        }
        #endregion
    }
}
