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
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewSectorBreakDown
    /// </summary>
    public class ViewModelSectorBreakdown : NotificationObject
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

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool isExCashSecurity = false;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool lookThruEnabled = false;                
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelSectorBreakdown(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            portfolioSelectionDataInfo = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            isExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
            {
                dbInteractivity.RetrieveSectorBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),isExCashSecurity,lookThruEnabled, 
                                                                                                                        RetrieveSectorBreakdownDataCallbackMethod);
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
        /// contains data for the grid in the gadget
        /// </summary>
        private ObservableCollection<SectorBreakdownData> sectorBreakdownInfo;
        public ObservableCollection<SectorBreakdownData> SectorBreakdownInfo
        {
            get { return sectorBreakdownInfo; }
            set
            {
                if (sectorBreakdownInfo != value)
                {
                    sectorBreakdownInfo = value;
                    RaisePropertyChanged(() => this.SectorBreakdownInfo);
                }
            }
        }

        /// <summary>
        /// contains data for the chart in the gadget
        /// </summary>
        private ObservableCollection<SectorSpecificData> sectorSpecificInfo;
        public ObservableCollection<SectorSpecificData> SectorSpecificInfo
        {
            get { return sectorSpecificInfo; }
            set
            {
                if (sectorSpecificInfo != value)
                {
                    sectorSpecificInfo = value;
                    RaisePropertyChanged(() => this.SectorSpecificInfo);
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
        /// Property to contain status value for busy indicator of the gadget
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
                        dbInteractivity.RetrieveSectorBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                                                                                                        lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
                         dbInteractivity.RetrieveSectorBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),isExCashSecurity,
                                                                                                        lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
                        dbInteractivity.RetrieveSectorBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                                                                                                          lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
                if (isExCashSecurity != isExCashSec)
                {
                    isExCashSecurity = isExCashSec;

                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveSectorBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                                                                                                        lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
                if (lookThruEnabled != enableLookThru)
                {
                    lookThruEnabled = enableLookThru;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveSectorBreakdownData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                                                                                                        lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
        /// Callback method for RetrieveSectorBreakdownData service call
        /// </summary>
        /// <param name="sectorBreakdownData">SectorBreakdownData collection</param>
        private void RetrieveSectorBreakdownDataCallbackMethod(List<SectorBreakdownData> sectorBreakdownData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (sectorBreakdownData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, sectorBreakdownData, 1);
                    List<SectorBreakdownData> templist = new List<SectorBreakdownData>(sectorBreakdownData);
                    foreach (SectorBreakdownData item in templist)
                    {
                        if (item.Sector == "Not classified" || item.Sector == null || item.Sector == string.Empty || item.Sector == " ")
                            item.Sector = CapitalizeFirstLetterAfterSpace("Not classified");

                        if(item.Industry == null || item.Industry == string.Empty || item.Industry == " ")
                            item.Industry = CapitalizeFirstLetterAfterSpace("Not classified");
                    }
                    SectorBreakdownInfo = new ObservableCollection<SectorBreakdownData>(templist);                   
                    SectorSpecificInfo = new ObservableCollection<SectorSpecificData>();
                    foreach (SectorBreakdownData item in SectorBreakdownInfo)
                    {                       
                        if (SectorSpecificInfo.Where(i => i.Sector == item.Sector).Count().Equals(0))
                        {
                            SectorSpecificInfo.Add(new SectorSpecificData()
                            {
                                Sector = item.Sector,
                                PortfolioShare = SectorBreakdownInfo.Where(t => t.Sector == item.Sector).Sum(r => r.PortfolioShare),
                                BenchmarkShare = SectorBreakdownInfo.Where(t => t.Sector == item.Sector).Sum(r => r.BenchmarkShare)
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
        /// Scan through the letters, checking for spaces and convert the lowercase letters to uppercaes which follow spaces
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
    }
}
