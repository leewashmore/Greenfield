using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewRegionBreakDown
    /// </summary>
    public class ViewModelRegionBreakdown : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;

        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private PortfolioSelectionData portfolioSelectionDataInfo;       
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelRegionBreakdown(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            portfolioSelectionDataInfo = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            IsExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            LookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
            {
                dbInteractivity.RetrieveRegionBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),isExCashSecurity,lookThruEnabled,
                                                                                                                        RetrieveRegionBreakdownDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool isExCashSecurity = false;
        public bool  IsExCashSecurity
        {
            get { return isExCashSecurity; }
            set
            {
                if (isExCashSecurity != value)
                {
                    isExCashSecurity = value;
                    RaisePropertyChanged(() => IsExCashSecurity);
                }
            }
        }        

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool lookThruEnabled = false;
        public bool LookThruEnabled
        {
            get { return lookThruEnabled; }
            set
            {
                if (lookThruEnabled != value)
                {
                    lookThruEnabled = value;
                    RaisePropertyChanged(() => LookThruEnabled);
                }
            }
        }

        /// <summary>
        /// contains data for the grid in the gadget
        /// </summary>
        private ObservableCollection<RegionBreakdownData> regionBreakdownInfo;
        public ObservableCollection<RegionBreakdownData> RegionBreakdownInfo
        {
            get { return regionBreakdownInfo; }
            set
            {
                if (regionBreakdownInfo != value)
                {
                    regionBreakdownInfo = value;
                    RaisePropertyChanged(() => this.RegionBreakdownInfo);
                }
            }
        }

        /// <summary>
        /// contains data for the chart in the gadget
        /// </summary>
        private ObservableCollection<RegionSpecificData> regionSpecificInfo;
        public ObservableCollection<RegionSpecificData> RegionSpecificInfo
        {
            get { return regionSpecificInfo; }
            set
            {
                if (regionSpecificInfo != value)
                {
                    regionSpecificInfo = value;
                    RaisePropertyChanged(() => this.RegionSpecificInfo);
                }
            }
        }

        /// <summary>
        /// property to contain effective date value from EffectiveDate Datepicker
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
                    RaisePropertyChanged(() => EffectiveDate);
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

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get{ return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && isActive)
                    {
                        dbInteractivity.RetrieveRegionBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                                                                                                        lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }        
        #endregion
        #endregion

        #region Event Handlers
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
                    portfolioSelectionDataInfo = portfolioSelectionData;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveRegionBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                                                                                                          lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
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
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveRegionBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                                                                                                          lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
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
        /// Event Handler to Check for Cash Securities
        /// </summary>
        /// <param name="isExCashSec"></param>
        public void HandleExCashSecuritySetEvent(bool isExCashSec)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(logger, methodNamespace, isExCashSec, 1);
                if (IsExCashSecurity != isExCashSec)
                {
                    IsExCashSecurity = isExCashSec;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveRegionBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                                                                                                          lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
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
                if (LookThruEnabled != enableLookThru)
                {
                    LookThruEnabled = enableLookThru;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveRegionBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                            lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
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
        /// Callback method for RetrieveRegionBreakdownData service call
        /// </summary>
        /// <param name="regionBreakdownData">RegionBreakdownData collection</param>
        private void RetrieveRegionBreakdownDataCallbackMethod(List<RegionBreakdownData> regionBreakdownData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (regionBreakdownData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, regionBreakdownData, 1);
                    List<RegionBreakdownData> templist = new List<RegionBreakdownData>(regionBreakdownData);
                    foreach (RegionBreakdownData item in templist)
                    {
                        if (item.Region == "Not classified" || item.Region == null || item.Region == string.Empty || item.Region == " ")
                        { item.Region = CapitalizeFirstLetterAfterSpace("Not classified"); }

                        if (item.Country == null || item.Country == string.Empty || item.Country == " ")
                        { item.Country = CapitalizeFirstLetterAfterSpace("Not classified"); }
                    }
                    RegionBreakdownInfo = new ObservableCollection<RegionBreakdownData>(templist);                   
                    RegionSpecificInfo = new ObservableCollection<RegionSpecificData>();
                    foreach (RegionBreakdownData item in RegionBreakdownInfo)
                    {                        
                        if (RegionSpecificInfo.Where(i => i.Region == item.Region).Count().Equals(0))
                        {
                            RegionSpecificInfo.Add(new RegionSpecificData()
                            {
                                Region = item.Region,
                                PortfolioShare = RegionBreakdownInfo.Where(t => t.Region == item.Region).Sum(r => r.PortfolioShare),
                                BenchmarkShare = RegionBreakdownInfo.Where(t => t.Region == item.Region).Sum(r => r.BenchmarkShare)
                            });
                        }
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
            finally
            {
                BusyIndicatorStatus = false;
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Method to scan through the letters, checking for spaces and convert the lowercase letters following spaces to uppercase.
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>string</returns>
        public string CapitalizeFirstLetterAfterSpace(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
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
            eventAggregator.GetEvent<ExCashSecuritySetEvent>().Unsubscribe(HandleExCashSecuritySetEvent);
            eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }
        #endregion

        //public ObservableCollection<RegionBreakdownData> ArrangeSortOrder(ObservableCollection<RegionBreakdownData> data)
        //{
        //    List<String> distinctRegions = data.OrderBy(a => a.Region)
        //        .Select(a => a.Region).Distinct().ToList();
        //    int regionCount = 1;
        //    foreach (String region in distinctRegions)
        //    {
        //        List<string> distinctCountries = data.Where(a => a.Region == region).OrderBy(a => a.Country)
        //                .Select(a => a.Country).Distinct().ToList();
        //        int countryCount = 1;
        //        foreach (String country in distinctCountries)
        //        {
        //            List<RegionBreakdownData> records = data.Where(a => a.Region == region && a.Country == country).ToList();
        //            foreach (RegionBreakdownData record in records)
        //            {
        //                record.RegionSortOrder = String.Format("{0}. {1}",regionCount.ToString("00"), region);
        //                record.CountrySortOrder = String.Format("{0}. {1}", countryCount.ToString("00"), country);
        //            }                    
        //            countryCount++;
        //        }
        //        regionCount++;
        //    }
        //    return data;
        //}
    }
}
