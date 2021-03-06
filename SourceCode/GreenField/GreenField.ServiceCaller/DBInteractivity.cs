﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.CustomScreeningDefinitions;
using GreenField.ServiceCaller.DCFDefinitions;
using GreenField.ServiceCaller.DocumentWorkSpaceDefinitions;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using GreenField.ServiceCaller.FairValueDefinitions;
using GreenField.ServiceCaller.MeetingDefinitions;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.UserSession;

namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Service Caller class for Security Reference Data and Holdings, Benchmark & Performance Data.
    /// </summary>
    [Export(typeof(IDBInteractivity))]
    public class DBInteractivity : IDBInteractivity
    {
        #region Fields
        /// <summary>
        /// Logging Service Instance
        /// </summary>
        /// 
        [Import]
        public ILoggerFacade LoggerFacade { get; set; }
        #endregion

        #region Build1
        /// <summary>
        /// service call method for security gadget
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSecurityReferenceDataAsync();
            client.RetrieveSecurityReferenceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }

                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for security overview gadget
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveSecurityOverviewData(EntitySelectionData entitySelectionData, Action<SecurityOverviewData> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                : "Unspecified");

            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSecurityOverviewDataAsync(entitySelectionData);
            client.RetrieveSecurityOverviewDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ?
                                                                                                                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveEntitySelectionDataAsync();
            client.RetrieveEntitySelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveEntitySelectionWithBenchmarkData(Action<List<EntitySelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveEntitySelectionWithBenchmarkDataAsync();
            client.RetrieveEntitySelectionWithBenchmarkDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service Caller Method for Closing Price Chart
        /// </summary>
        /// <param name="entityIdentifiers">List of Securities</param>
        /// <param name="startDateTime">Start Date</param>
        /// <param name="endDateTime">Chart End Date</param>
        /// <param name="totalReturnCheck">Total Return :True/False</param>
        /// <param name="frequencyInterval">Frequency Interval</param>
        /// <param name="chartEntityTypes">Type of entities added to Chart</param>
        /// <param name="callback">Collection of PricingReferenceData</param>
        public void RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime,
            bool totalReturnCheck, string frequencyInterval, Action<List<PricingReferenceData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePricingReferenceDataAsync(entityIdentifiers, startDateTime, endDateTime, totalReturnCheck, frequencyInterval);
            client.RetrievePricingReferenceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #region Interaction Method for Theoretical Unrealized Gain Loss Gadget

        /// <summary>
        /// Method that calls the Unrealized Gain Loss service method and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="entityIdentifier">Unique Identifier for each security</param>
        /// <param name="startDateTime">Start Date time of the time period selected</param>
        /// <param name="endDateTime">End Date time of the time period selected</param>
        /// <param name="frequencyInterval">frequency Interval selected</param>
        /// <param name="callback"></param>
        public void RetrieveUnrealizedGainLossData(EntitySelectionData entityIdentifier, DateTime startDateTime, DateTime endDateTime, string frequencyInterval, Action<List<UnrealizedGainLossData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveUnrealizedGainLossDataAsync(entityIdentifier, startDateTime, endDateTime, frequencyInterval);
            client.RetrieveUnrealizedGainLossDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #endregion

        #region Build2

        public void RetrievePortfolioSelectionData(Action<List<PortfolioSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePortfolioSelectionDataAsync();
            client.RetrievePortfolioSelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveAvailableDatesInPortfolios(Action<List<DateTime>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveAvailableDatesInPortfoliosAsync();
            client.RetrieveAvailableDatesInPortfoliosCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #region Slice 2

        public void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveBenchmarkSelectionDataAsync();
            client.RetrieveBenchmarkSelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (callback != null)
                {
                    if (e.Error == null)
                    {
                        if (callback != null)
                        {
                            if (e.Result != null)
                            {
                                callback(e.Result.ToList());
                            }
                            else
                            {
                                callback(null);
                            }
                        }
                    }
                    else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                    {
                        FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                            = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                        Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                        if (callback != null)
                            callback(null);
                    }
                    else
                    {
                        Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                        if (callback != null)
                            callback(null);
                    }
                    ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
                }
            };
        }

        ///<summary>
        /// Service Caller method for AssetAllocation gadget
        /// </summary>
        /// <param name="fundSelectionData">Selected Portfolio</param>
        /// <param name="effectiveDate">selected Date</param>
        /// <param name="callback">List of AssetAllocationData</param>
        public void RetrieveAssetAllocationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, bool lookThru, bool excludeCash, Action<List<AssetAllocationData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveAssetAllocationDataAsync(fundSelectionData, effectiveDate, lookThru, excludeCash);
            client.RetrieveAssetAllocationDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for sector breakdown gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="isExCashSecurity"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        public void RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, bool lookThruEnabled,
            Action<List<SectorBreakdownData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                            : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSectorBreakdownDataAsync(portfolioSelectionData, effectiveDate, isExCashSecurity, lookThruEnabled);
            client.RetrieveSectorBreakdownDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                    ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for region breakdown gadget
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData</param>
        /// <param name="effectiveDate">DateTime</param>
        /// <param name="isExCashSecurity">bool</param>
        /// <param name="lookThruEnabled">bool</param>
        /// <param name="callback"></param>
        public void RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, bool lookThruEnabled,
            Action<List<RegionBreakdownData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                                : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRegionBreakdownDataAsync(portfolioSelectionData, effectiveDate, isExCashSecurity, lookThruEnabled);
            client.RetrieveRegionBreakdownDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                    ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for top holdings gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="isExCashSecurity"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        public void RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, bool lookThruEnabled,
            Action<List<TopHoldingsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                    : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveTopHoldingsDataAsync(portfolioSelectionData, effectiveDate, isExCashSecurity, lookThruEnabled);
            client.RetrieveTopHoldingsDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                    ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for index constituent gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        public void RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool lookThruEnabled,
            Action<List<IndexConstituentsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                    : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveIndexConstituentsDataAsync(portfolioSelectionData, effectiveDate, lookThruEnabled);
            client.RetrieveIndexConstituentsDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                    ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for RiskIndexExposures gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="isExCashSecurity"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="filterType"></param>
        /// <param name="filterValue"></param>
        /// <param name="callback"></param>
        public void RetrieveRiskIndexExposuresData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity,
            bool lookThruEnabled, string filterType, string filterValue, Action<List<RiskIndexExposuresData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                                    : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRiskIndexExposuresDataAsync(portfolioSelectionData, effectiveDate, isExCashSecurity, lookThruEnabled, filterType, filterValue);
            client.RetrieveRiskIndexExposuresDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Method that calls the  RetrieveHoldingsPercentageDataForRegion method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="fundSelectionData">Object of type PortfolioSelectionData Class containg the fund selection data</param>
        /// <param name="effectiveDate">Effective date as selected by the user</param>
        /// <param name="filterType">The filter type selected by the user</param>
        /// <param name="filterValue">The filter value selected by the user</param>
        /// <param name="callback"></param>   
        public void RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled, Action<List<HoldingsPercentageData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveHoldingsPercentageDataForRegionAsync(fundSelectionData, effectiveDate, filterType, filterValue, lookThruEnabled);
            client.RetrieveHoldingsPercentageDataForRegionCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Method that calls the  RetrieveHoldingsPercentageData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="fundSelectionData">Object of type PortfolioSelectionData Class containg the fund selection data</param>
        /// <param name="effectiveDate">Effective date as selected by the user</param>
        /// <param name="filterType">The filter type selected by the user</param>
        /// <param name="filterValue">The filter value selected by the user</param>
        /// <param name="callback"></param>  
        public void RetrieveHoldingsPercentageData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled, Action<List<HoldingsPercentageData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveHoldingsPercentageDataAsync(fundSelectionData, effectiveDate, filterType, filterValue, lookThruEnabled);
            client.RetrieveHoldingsPercentageDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Market Performance Gadget
        public void RetrieveBenchmarkFilterSelectionData(String benchmarkCode, String BenchmarkName, String filterType, Action<List<BenchmarkFilterSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveBenchmarkFilterSelectionDataAsync(benchmarkCode, BenchmarkName, filterType);
            client.RetrieveBenchmarkFilterSelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for retrieving list of market performance snapshots for a user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="callback"></param>
        public void RetrieveMarketSnapshotSelectionData(string userName, Action<List<MarketSnapshotSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMarketSnapshotSelectionDataAsync(userName);
            client.RetrieveMarketSnapshotSelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// service call method for retrieving user preference of entities in “Market Performance Snapshot”
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="snapshotName"></param>
        /// <param name="callback"></param>
        public void RetrieveMarketSnapshotPreference(int snapshotPreferenceId, Action<List<MarketSnapshotPreference>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMarketSnapshotPreferenceAsync(snapshotPreferenceId);
            client.RetrieveMarketSnapshotPreferenceCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call method for “Market Performance Snapshot”
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void RetrieveMarketSnapshotPerformanceData(List<MarketSnapshotPreference> marketSnapshotPreference, Action<List<MarketSnapshotPerformanceData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMarketSnapshotPerformanceDataAsync(marketSnapshotPreference);
            client.RetrieveMarketSnapshotPerformanceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        ///  service call method to save changes in user snapshot entity from “Market Performance Snapshot”
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void SaveMarketSnapshotPreference(string updateXML, Action<List<MarketSnapshotPreference>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SaveMarketSnapshotPreferenceAsync(updateXML);
            client.SaveMarketSnapshotPreferenceCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        ///  service call method to save changes in user snapshot entity from “Market Performance Snapshot” as a new snapshot
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void SaveAsMarketSnapshotPreference(string updateXML, Action<PopulatedMarketSnapshotPerformanceData> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SaveAsMarketSnapshotPreferenceAsync(updateXML);
            client.SaveAsMarketSnapshotPreferenceCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RemoveMarketSnapshotPreference(string userName, string snapshotName, Action<bool?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RemoveMarketSnapshotPreferenceAsync(userName, snapshotName);
            client.RemoveMarketSnapshotPreferenceCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        /// <summary>
        /// Retrieves filter values for a selected filter type by calling the service
        /// </summary>
        /// <param name="filterType">Filter Type selected by the user</param>
        /// <param name="effectiveDate">Effected Date selected by the user</param>
        /// <param name="callback">callback method</param>
        public void RetrieveFilterSelectionData(PortfolioSelectionData selectedPortfolio, DateTime? effectiveDate, Action<List<FilterSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFilterSelectionDataAsync(selectedPortfolio, effectiveDate);
            client.RetrieveFilterSelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service caller method to retrieve PortfolioDetails Data
        /// </summary>
        /// <param name="objPortfolioIdentifier">Portfolio Identifier</param>
        /// <param name="objSelectedDate">Selected Date</param>
        /// <param name="callback">collection of Portfolio Details Data</param>
        public void RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime objSelectedDate, String filterType, String filterValue, bool lookThruEnabled, bool excludeCash, bool objGetBenchmark, Action<List<PortfolioDetailsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePortfolioDetailsDataAsync(objPortfolioIdentifier, objSelectedDate, filterType, filterValue, lookThruEnabled, excludeCash, objGetBenchmark);
            client.RetrievePortfolioDetailsDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service caller method to retrieve Benchmark Return Data for MultiLineBenchmarkUI- Chart
        /// </summary>
        /// <param name="objSelectedEntities">Details of Selected Portfolio & Security</param>
        /// <param name="callback">List of BenchmarkChartReturnData</param>
        public void RetrieveBenchmarkChartReturnData(Dictionary<string, string> objSelectedEntities, Action<List<BenchmarkChartReturnData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveBenchmarkChartReturnDataAsync(objSelectedEntities);
            client.RetrieveBenchmarkChartReturnDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service caller method to retrieve Benchmark Return Data for MultiLineBenchmarkUI- Chart 
        /// </summary>
        /// <param name="objSelectedEntites">Details of Selected Portfolio & Security</param>
        /// <param name="callback">List of BenchmarkGridReturnData</param>
        public void RetrieveBenchmarkGridReturnData(Dictionary<string, string> objSelectedEntites, Action<List<BenchmarkGridReturnData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveBenchmarkGridReturnDataAsync(objSelectedEntites);
            client.RetrieveBenchmarkGridReturnDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #region Interaction Method for Attribution Gadget
        /// <summary>
        /// Method that calls the RetrieveAttributionData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="portfolioSelectionData">Contains the selected portfolio</param>
        /// <param name="effectiveDate">Contains the selected effective date</param>
        /// <param name="callback">callback</param>
        public void RetrieveAttributionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String nodeName, 
            Action<List<AttributionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveAttributionDataAsync(portfolioSelectionData, effectiveDate, nodeName);
            client.RetrieveAttributionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Interaction Methods for Benchmark
        /// <summary>
        /// Method that calls the RetrieveTopBenchmarkSecuritiesData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="benchmarkSelectionData">object containing Benchmark Selection Data</param>
        /// <param name="effectiveDate">Effective Date selected by the user</param>
        /// <param name="callback">callback</param>
        public void RetrieveTopBenchmarkSecuritiesData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate,
            Action<List<TopBenchmarkSecuritiesData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveTopBenchmarkSecuritiesDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveTopBenchmarkSecuritiesDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Method that calls the RetrievePortfolioRiskReturnData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="fundSelectionData">object containing Fund Selection Data</param>
        /// <param name="benchmarkSelectionData">object containing Benchmark Selection Data</param>
        /// <param name="effectiveDate">Effective Date selected by the user</param>
        /// <param name="callback">callback</param>
        public void RetrievePortfolioRiskReturnData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, 
            Action<List<PortfolioRiskReturnData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePortfolioRiskReturnDataAsync(portfolioSelectionData, effectiveDate, effectiveDate);
            client.RetrievePortfolioRiskReturnDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #endregion

        #region Interaction Method for Heat Map

        /// <summary>
        /// Calls the web service and retrieves data for heat map
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="period"></param>
        /// <param name="callback"></param>
        public void RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String period, 
            Action<List<HeatMapData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveHeatMapDataAsync(fundSelectionData, effectiveDate, period);
            client.RetrieveHeatMapDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #endregion

        #region Slice3

        /// <summary>
        /// Service Caller Method for Relative Performance UI Data
        /// </summary>
        /// <param name="objSelectedEntity">Data of Selected Entities</param>
        /// <param name="objEffectiveDate">Selected Date</param>
        /// <param name="callback">List of Relative Performance Data</param>
        public void RetrieveRelativePerformanceUIData(Dictionary<string, string> objSelectedEntity, DateTime objEffectiveDate, Action<List<RelativePerformanceUIData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRelativePerformanceUIDataAsync(objSelectedEntity, objEffectiveDate);
            client.RetrieveRelativePerformanceUIDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service Caller Method for Chart Extension Gadget
        /// </summary>
        /// <param name="objSelectedEntities">Collection of selected Portfolio and/or Securitie</param>
        /// <param name="objEffectiveDate">Selected Date</param>
        /// <param name="callback">Collection of Chart Extension Data</param>
        public void RetrieveChartExtensionData(Dictionary<string, string> objSelectedEntities, DateTime objEffectiveDate, Action<List<ChartExtensionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveChartExtensionDataAsync(objSelectedEntities, objEffectiveDate);
            client.RetrieveChartExtensionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveCountrySelectionData(Action<List<CountrySelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ModelFXOperationsClient client = new ModelFXOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCountrySelectionDataAsync();
            client.RetrieveCountrySelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call to retrieve relative performance gadget data 
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="period"></param>
        /// <param name="callback"></param>
        public void RetrieveRelativePerformanceData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String period,
                                                                                Action<List<RelativePerformanceData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                                    : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRelativePerformanceDataAsync(portfolioSelectionData, effectiveDate, period);
            client.RetrieveRelativePerformanceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        /// <summary>
        /// service call to retrieve relative performance sector data list
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="period"></param>
        /// <param name="callback"></param>
        public void RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate,
                                                                                                    Action<List<RelativePerformanceSectorData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                            : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRelativePerformanceSectorDataAsync(fundSelectionData, effectiveDate);
            client.RetrieveRelativePerformanceSectorDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call to retrieve relative performance security data 
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="period"></param>
        /// <param name="callback"></param>
        public void RetrieveRelativePerformanceSecurityData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period,
                        Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, string sectorID = null)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                            : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRelativePerformanceSecurityDataAsync(portfolioSelectionData, effectiveDate, period, countryID, sectorID);
            client.RetrieveRelativePerformanceSecurityDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call to retrieve relative performance country active position data 
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="period"></param>
        /// <param name="callback"></param>
        public void RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period,
            Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                                    : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRelativePerformanceCountryActivePositionDataAsync(portfolioSelectionData, effectiveDate, period, countryID, sectorID);
            client.RetrieveRelativePerformanceCountryActivePositionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                    ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call to retrieve relative performance sector active position data 
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="period"></param>
        /// <param name="callback"></param>
        public void RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period,
            Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                        : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRelativePerformanceSectorActivePositionDataAsync(portfolioSelectionData, effectiveDate, period, countryID, sectorID);
            client.RetrieveRelativePerformanceSectorActivePositionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call to retrieve relative performance security active position data 
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="period"></param>
        /// <param name="callback"></param>
        public void RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period,
            Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                            : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRelativePerformanceSecurityActivePositionDataAsync(portfolioSelectionData, effectiveDate, period, countryID, sectorID);
            client.RetrieveRelativePerformanceSecurityActivePositionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Method that calls the  RetrieveMarketCapitalizationData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="fundSelectionData">Object of type PortfolioSelectionData Class containg the fund selection data</param>
        /// <param name="effectiveDate">Effective date as selected by the user</param>
        /// <param name="filterType">The filter type selected by the user</param>
        /// <param name="filterValue">The filter value selected by the user</param>
        /// <param name="callback"></param>  
        public void RetrieveMarketCapitalizationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool isExCashSecurity, bool lookThruEnabled, Action<List<MarketCapitalizationData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMarketCapitalizationDataAsync(fundSelectionData, effectiveDate, filterType, filterValue, isExCashSecurity, lookThruEnabled);
            client.RetrieveMarketCapitalizationDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Method that calls the RetrievePerformanceGraphData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="name">Name of the fund</param>
        /// <param name="callback"></param>
        public void RetrievePerformanceGraphData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String period,
            String country, Action<List<PerformanceGraphData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePerformanceGraphDataAsync(fundSelectionData, effectiveDate, period, country);
            client.RetrievePerformanceGraphDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Method that calls the RetrievePerformanceGridData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="name">Name of the fund</param>
        /// <param name="callback"></param>
        public void RetrievePerformanceGridData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String country, 
            Action<List<PerformanceGridData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePerformanceGridDataAsync(portfolioSelectionData, effectiveDate, country);
            client.RetrievePerformanceGridDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.BenchmarkHoldingsDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ?
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #endregion

        #endregion

        #region Slice 4 - FX

        public void RetrieveCommodityData(string commodityID, Action<List<FXCommodityData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ModelFXOperationsClient client = new ModelFXOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCommodityDataAsync(commodityID);
            client.RetrieveCommodityDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Calls the web service and gets the MacroDatabaseKeyAnnualReportData
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="callback"></param>
        public void RetrieveMacroDatabaseKeyAnnualReportData(string countryName, Action<List<MacroDatabaseKeyAnnualReportData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            ModelFXOperationsClient client = new ModelFXOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMacroDatabaseKeyAnnualReportDataAsync(countryName);
            client.RetrieveMacroDatabaseKeyAnnualReportDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Calls the web service and gets the MacroDatabaseKeyAnnualReportDataEMSummary Data
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="countryValues"></param>
        /// <param name="callback"></param>
        public void RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(String countryName, List<String> countryValues, 
            Action<List<MacroDatabaseKeyAnnualReportData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            ModelFXOperationsClient client = new ModelFXOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMacroDatabaseKeyAnnualReportDataEMSummaryAsync(countryName, countryValues);
            client.RetrieveMacroDatabaseKeyAnnualReportDataEMSummaryCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveCommoditySelectionData(Action<List<FXCommodityData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ModelFXOperationsClient client = new ModelFXOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCommoditySelectionDataAsync();
            client.RetrieveCommoditySelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Retrieves list of regions
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveRegionSelectionData(Action<List<RegionSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            ModelFXOperationsClient client = new ModelFXOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRegionSelectionDataAsync();
            client.RetrieveRegionSelectionDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ModelFXDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Slice 5 - External Research

        public void RetrieveIssuerReferenceData(EntitySelectionData entitySelectionData, Action<IssuerReferenceData> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveIssuerReferenceDataAsync(entitySelectionData);
            client.RetrieveIssuerReferenceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveFinancialStatementData(string issuerID, FinancialStatementDataSource dataSource, FinancialStatementPeriodType periodType
            , FinancialStatementFiscalType fiscalType, FinancialStatementType statementType, String currency, Action<List<FinancialStatementData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFinancialStatementAsync(issuerID, dataSource, periodType, fiscalType, statementType, currency);
            client.RetrieveFinancialStatementCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveInvestmentContextData(string issuerID, string context, Action<List<List<InvestmentContextDetailsData>>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveInvestmentContextDataAsync(issuerID,context);
            //client.RetrieveFinancialStatementAsync(issuerID, dataSource, periodType, fiscalType, statementType, currency);
            client.RetrieveInvestmentContextDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                           var lists = e.Result.ToList();
                           List<List<InvestmentContextDetailsData>> icdList = new List<List<InvestmentContextDetailsData>>();
                           foreach (var list in lists)
                           {
                               List<InvestmentContextDetailsData> ll = list.ToList();
                               icdList.Add(ll);
                           }
                            callback(icdList);
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }



        public void RetrieveDataMaster(Action<List<DATA_MASTER>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveDataMasterAsync();
            //client.RetrieveFinancialStatementAsync(issuerID, dataSource, periodType, fiscalType, statementType, currency);
            client.RetrieveDataMasterCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Gets Basic data
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveBasicData(EntitySelectionData entitySelectionData, Action<List<BasicData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveBasicDataAsync(entitySelectionData);
            client.RetrieveBasicDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// Calls the Web service and gets COA Specific data
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="securityId"></param>
        /// <param name="cSource"></param>
        /// <param name="cFiscalType"></param>
        /// <param name="cCurrency"></param>
        /// <param name="callback"></param>
        public void RetrieveCOASpecificData(String issuerId, int? securityId, FinancialStatementDataSource cSource, 
            FinancialStatementFiscalType cFiscalType, String cCurrency, Action<List<COASpecificData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCOASpecificDataAsync(issuerId, securityId, cSource, cFiscalType, cCurrency);
            client.RetrieveCOASpecificDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), 
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Retrieves Valuation,Quality and Growth data
        /// </summary>
        /// <param name="selectedPortfolio"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="filterType"></param>
        /// <param name="filterValue"></param>
        /// <param name="lookThruEnabled"></param>
        /// <param name="callback"></param>
        public void RetrieveValuationGrowthData(PortfolioSelectionData selectedPortfolio, DateTime? effectiveDate, 
            String filterType, String filterValue, bool lookThruEnabled, Action<List<ValuationQualityGrowthData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveValuationGrowthDataAsync(selectedPortfolio, effectiveDate, filterType, filterValue, lookThruEnabled);
            client.RetrieveValuationGrowthDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), 
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveEMSummaryMarketData(String selectedPortfolio, Action<List<EMSummaryMarketData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, 
                System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), 
                SessionManager.SESSION != null ?
                SessionManager.SESSION.UserName : "Unspecified");
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveEmergingMarketDataAsync(selectedPortfolio);
            client.RetrieveEmergingMarketDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #region ConsensusEstimates Gadgets

        /// <summary>
        /// Service Caller Method to Retrieve Data for TargetPriceGadget(ConsensusEstimates)
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveTargetPriceData(EntitySelectionData entitySelectionData, Action<List<TargetPriceCEData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveTargetPriceDataAsync(entitySelectionData);
            client.RetrieveTargetPriceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (callback != null)
                {
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault =
                        e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// ServiceCaller Method for ConsesnsuEstimatesGadget- Median
        /// </summary>
        /// <param name="issuerId">Issuer ID</param>
        /// <param name="periodType">Selected Period Type</param>
        /// <param name="currency">Selected Currency</param>
        /// <param name="callback">Collection of ConsensusEstimateMedian</param>
        public void RetrieveConsensusEstimatesMedianData(string issuerId, FinancialStatementPeriodType periodType, String currency, Action<List<ConsensusEstimateMedian>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveConsensusEstimatesMedianDataAsync(issuerId, periodType, currency);
            client.RetrieveConsensusEstimatesMedianDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            };
        }

        /// <summary>
        /// ServiceCaller Method for ConsesnsuEstimatesGadget- Valuations
        /// </summary>
        /// <param name="issuerId">Issuer ID</param>
        /// <param name="periodType">Selected Period Type</param>
        /// <param name="currency">Selected Currency</param>
        /// <param name="callback">Collection of ConsensusEstimateValuations</param>
        public void RetrieveConsensusEstimatesValuationsData(string issuerId, string longName, FinancialStatementPeriodType periodType, String currency, Action<List<ConsensusEstimatesValuations>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveConsensusEstimatesValuationDataAsync(issuerId, longName, periodType, currency);
            client.RetrieveConsensusEstimatesValuationDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            };
        }

        #endregion

        /// <summary>
        /// service call for Consensus estimate detail data retrieval
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="periodType"></param>
        /// <param name="currency"></param>
        /// <param name="callback"></param>
        public void RetrieveConsensusEstimateDetailedData(string issuerId, FinancialStatementPeriodType periodType, String currency,
            Action<List<ConsensusEstimateDetail>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                    : "Unspecified");
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveConsensusEstimateDetailedDataAsync(issuerId, periodType, currency);
            client.RetrieveConsensusEstimateDetailedDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                    ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Calls the web service and retrieves Quarterly result data
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <param name="yearValue"></param>
        /// <param name="callback"></param>
        public void RetrieveQuarterlyResultsData(String fieldValue, int yearValue, Action<List<QuarterlyResultsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveQuarterlyResultsDataAsync(fieldValue, yearValue);
            client.RetrieveQuarterlyResultsDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), 
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrievePRevenueData(EntitySelectionData entitySelectionData, string chartTitle, Action<List<PRevenueData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePRevenueDataAsync(entitySelectionData, chartTitle);
            client.RetrievePRevenueDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }

            };

        }

        public void RetrieveRatioComparisonData(String contextSecurityXML, Action<List<RatioComparisonData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRatioComparisonDataAsync(contextSecurityXML);
            client.RetrieveRatioComparisonDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }

            };

        }

        public void RetrieveRatioSecurityReferenceData(ScatterGraphContext context, IssuerReferenceData issuerDetails, Action<List<GF_SECURITY_BASEVIEW>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveRatioSecurityReferenceDataAsync(context, issuerDetails);
            client.RetrieveRatioSecurityReferenceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }

            };

        }

        /// <summary>
        /// service call for Finstat detail data retrieval
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="securityId"></param>
        /// <param name="dataSource"></param>
        /// <param name="fiscalType"></param>
        /// <param name="currency"></param>
        /// <param name="yearRange"></param>
        /// <param name="callback"></param>
        public void RetrieveFinstatDetailData(string issuerId, string securityId, FinancialStatementDataSource dataSource,
                                               FinancialStatementFiscalType fiscalType, String currency, Int32 yearRange, Action<List<FinstatDetailData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                 : "Unspecified");
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFinstatDataAsync(issuerId, securityId, dataSource, fiscalType, currency, yearRange);
            client.RetrieveFinstatDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call for Consensus estimate detail broker data retrieval
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="periodType"></param>
        /// <param name="currency"></param>
        /// <param name="callback"></param>
        public void RetrieveConsensusEstimateDetailedBrokerData(string issuerId, FinancialStatementPeriodType periodType, String currency,
                                                                                                            Action<List<ConsensusEstimateDetail>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                            : "Unspecified");
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveConsensusEstimateDetailedBrokerDataAsync(issuerId, periodType, currency);
            client.RetrieveConsensusEstimateDetailedBrokerDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Internal Research

        /// <summary>
        /// Calls the web service and retrieves estimates summary data
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveConsensusEstimatesSummaryData(EntitySelectionData entitySelectionData, Action<List<ConsensusEstimatesSummaryData>> 
            callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                SessionManager.SESSION.UserName : "Unspecified");
            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveConsensusEstimatesSummaryDataAsync(entitySelectionData);
            client.RetrieveConsensusEstimatesSummaryDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? 
                    SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// service call for composite fund gadget
        /// </summary>
        /// <param name="entityIdentifiers">EntitySelectionData</param>
        /// <param name="portfolio">PortfolioSelectionData</param>
        /// <param name="callback"></param>
        public void RetrieveCompositeFundData(EntitySelectionData entityIdentifiers, PortfolioSelectionData portfolio, Action<List<CompositeFundData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                            : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCompositeFundDataAsync(entityIdentifiers, portfolio);
            client.RetrieveCompositeFundDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                    ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Investment Committee

        public void RetrieveMeetingInfoByPresentationStatus(string presentationStatus, Action<List<MeetingInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMeetingInfoByPresentationStatusAsync(presentationStatus);
            client.RetrieveMeetingInfoByPresentationStatusCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveMeetingMinuteDetails(Int64? meetingID, Action<List<MeetingMinuteData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMeetingMinuteDetailsAsync(meetingID);
            client.RetrieveMeetingMinuteDetailsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveMeetingAttachedFileDetails(Int64? meetingID, Action<List<FileMaster>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveMeetingAttachedFileDetailsAsync(meetingID);
            client.RetrieveMeetingAttachedFileDetailsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdateMeetingMinuteDetails(String userName, MeetingInfo meetingInfo, List<MeetingMinuteData> meetingMinuteData, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdateMeetingMinuteDetailsAsync(userName, meetingInfo, meetingMinuteData);
            client.UpdateMeetingMinuteDetailsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdateMeetingAttachedFileStreamData(String userName, Int64 meetingId, FileMaster fileMasterInfo, Boolean deletionFlag, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdateMeetingAttachedFileStreamDataAsync(userName, meetingId, fileMasterInfo, deletionFlag);
            client.UpdateMeetingAttachedFileStreamDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrievePresentationOverviewData(String userName,String status,Action<List<ICPresentationOverviewData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePresentationOverviewDataAsync(userName,status );
            client.RetrievePresentationOverviewDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrievePresentationVoterData(Int64 presentationId, Action<List<VoterInfo>> callback, Boolean includeICAdminInfo = false)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePresentationVoterDataAsync(presentationId, includeICAdminInfo);
            client.RetrievePresentationVoterDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveSecurityPFVMeasureCurrentPrices(String securityId, List<String> pfvTypeInfo, Action<Dictionary<String, Decimal?>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSecurityPFVMeasureCurrentPricesAsync(securityId, pfvTypeInfo);
            client.RetrieveSecurityPFVMeasureCurrentPricesCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdateDecisionEntryDetails(String userName, ICPresentationOverviewData presentationOverViewData, List<VoterInfo> voterInfo, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdateDecisionEntryDetailsAsync(userName, presentationOverViewData, voterInfo);
            client.UpdateDecisionEntryDetailsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void CreatePresentation(String userName, ICPresentationOverviewData presentationOverviewData,string template, Action<PresentationFile> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.CreatePresentationAsync(userName, presentationOverviewData,template);
            client.CreatePresentationCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void DeletePresentation(String userName, ICPresentationOverviewData presentationOverviewData, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.DeletePresentationAsync(userName, presentationOverviewData);
            client.DeletePresentationCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }


        public void DistributeICPacks(Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.DistributeICPacksAsync();
            client.DistributeICPacksCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }




        public void VotingClosed(string fromstatus, string tostatus, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.VotingClosedAsync(fromstatus, tostatus);
            client.VotingClosedCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }


        public void PublishDecision(string fromstatus, string tostatus, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.PublishDecisionAsync(fromstatus, tostatus);
            client.PublishDecisionCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }


        public void RetrieveSecurityDetails(EntitySelectionData entitySelectionData, ICPresentationOverviewData presentationOverviewData, PortfolioSelectionData portfolioData, Action<ICPresentationOverviewData> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSecurityDetailsAsync(entitySelectionData, presentationOverviewData, portfolioData);
            client.RetrieveSecurityDetailsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void GetAvailablePresentationDates(Action<List<MeetingInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GetAvailablePresentationDatesAsync();
            client.GetAvailablePresentationDatesCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdateMeetingConfigSchedule(String userName, MeetingConfigurationSchedule meetingConfigurationSchedule, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdateMeetingConfigScheduleAsync(userName, meetingConfigurationSchedule);
            client.UpdateMeetingConfigScheduleCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrievePresentationComments(Int64 presentationId, Action<List<CommentInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePresentationCommentsAsync(presentationId);
            client.RetrievePresentationCommentsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void SetPresentationComments(string userName, Int64 presentationId, String comment, Action<List<CommentInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SetPresentationCommentsAsync(userName, presentationId, comment);
            client.SetPresentationCommentsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdatePreMeetingVoteDetails(String userName, List<VoterInfo> voterInfo, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdatePreMeetingVoteDetailsAsync(userName, voterInfo);
            client.UpdatePreMeetingVoteDetailsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void SetMeetingPresentationStatus(String userName, Int64 meetingId, String status, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SetMeetingPresentationStatusAsync(userName, meetingId, status);
            client.SetMeetingPresentationStatusCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdateMeetingPresentationDate(String userName, Int64 presentationId, MeetingInfo meetingInfo, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdateMeetingPresentationDateAsync(userName, presentationId, meetingInfo);
            client.UpdateMeetingPresentationDateCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void GetMeetingConfigSchedule(Action<MeetingConfigurationSchedule> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GetMeetingConfigScheduleAsync();
            client.GetMeetingConfigScheduleCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrievePresentationAttachedFileDetails(Int64? presentationID, Action<List<FileMaster>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePresentationAttachedFileDetailsAsync(presentationID);
            client.RetrievePresentationAttachedFileDetailsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdatePresentationAttachedFileStreamData(String userName, Int64 presentationId, FileMaster fileMasterInfo, Boolean deletionFlag, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdatePresentationAttachedFileStreamDataAsync(userName, presentationId, fileMasterInfo, deletionFlag);
            client.UpdatePresentationAttachedFileStreamDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void SetICPPresentationStatus(String userName, Int64 presentationId, String status, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SetICPPresentationStatusAsync(userName, presentationId, status);
            client.SetICPPresentationStatusCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveCurrentPFVMeasures(List<String> PFVTypeInfo, String securityTicker, Action<Dictionary<String, Decimal?>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCurrentPFVMeasuresAsync(PFVTypeInfo, securityTicker);
            client.RetrieveCurrentPFVMeasuresCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void GetAllUsers(Action<List<MembershipUserInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GetAllUsersAsync();
            client.GetAllUsersCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void GetUsersByNames(List<String> userNames, Action<List<MembershipUserInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GetUsersByNamesAsync(userNames);
            client.GetUsersByNamesCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void SetMessageInfo(String emailTo, String emailCc, String emailSubject, String emailMessageBody, String emailAttachment
            , String userName, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SetMessageInfoAsync(emailTo, emailCc, emailSubject, emailMessageBody, emailAttachment, userName);
            client.SetMessageInfoCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

    /*    public void GenerateMeetingMinutesReport(Int64 meetingId, Action<Byte[]> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GenerateMeetingMinutesReportAsync(meetingId);
            client.GenerateMeetingMinutesReportCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        */
        public void GeneratePreMeetingVotingReport(Int64 presentationId, Action<Byte[]> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GeneratePreMeetingVotingReportAsync(presentationId);
            client.GeneratePreMeetingVotingReportCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveSummaryReportData(DateTime startDate, DateTime endDate, Action<List<SummaryReportData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSummaryReportDataAsync(startDate, endDate);
            client.RetrieveSummaryReportDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void GenerateICPacketReport(Int64 presentationId, Action<Byte[]> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GenerateICPacketReportAsync(presentationId);
            client.GenerateICPacketReportCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void ReSubmitPresentation(String userName, ICPresentationOverviewData presentationOverviewData, Boolean sendAlert, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            MeetingOperationsClient client = new MeetingOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.ReSubmitPresentationAsync(userName, presentationOverviewData, sendAlert);
            client.ReSubmitPresentationCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Slice-7 DCF

        /// <summary>
        /// Service Caller Method for DCFAnalysisData
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveDCFAnalysisData(EntitySelectionData entitySelectionData, Action<List<DCFAnalysisSummaryData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveDCFAnalysisDataAsync(entitySelectionData);
            client.RetrieveDCFAnalysisDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.ToList());
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                      = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service Caller Method for DCFTerminalValueCalculations
        /// </summary>
        /// <param name="entitySelectionData">SelectedSecurity</param>
        /// <param name="callback">List of type DCFTerminalValueCalculationsData</param>
        public void RetrieveDCFTerminalValueCalculationsData(EntitySelectionData entitySelectionData, Action<List<DCFTerminalValueCalculationsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveTerminalValueCalculationsDataAsync(entitySelectionData);
            client.RetrieveTerminalValueCalculationsDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.ToList());
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                      = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service Method to Retrieve values of CashFlows
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <param name="callback">List of DCFCashFlowData</param>
        public void RetrieveCashFlows(EntitySelectionData entitySelectionData, Action<List<DCFCashFlowData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCashFlowsAsync(entitySelectionData);
            client.RetrieveCashFlowsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.ToList());
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                      = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Retrieve Data for DCF summary
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveDCFSummaryData(EntitySelectionData entitySelectionData, Action<List<DCFSummaryData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSummaryDataAsync(entitySelectionData);
            client.RetrieveSummaryDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.ToList());
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                      = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Retrieve Current Price of Security
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <param name="callback"></param>
        public void RetrieveDCFCurrentPrice(EntitySelectionData entitySelectionData, Action<decimal?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCurrentPriceDataAsync(entitySelectionData);
            client.RetrieveCurrentPriceDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                      = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Gets Free Cash Flows Data
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveDCFFreeCashFlowsData(EntitySelectionData entitySelectionData, Action<List<FreeCashFlowsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFreeCashFlowsDataAsync(entitySelectionData);
            client.RetrieveFreeCashFlowsDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// Gets Free Cash Flows Data
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveDCFFairValueData(EntitySelectionData entitySelectionData, Action<List<PERIOD_FINANCIALS>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFairValueAsync(entitySelectionData);
            client.RetrieveFairValueCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// Insert DCF Fair Values
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <param name="valueType">Value Type</param>
        /// <param name="fvMeasure">FV_ Measure</param>
        /// <param name="fvbuy">FV_Buy</param>
        /// <param name="fvSell">FV_Sell</param>
        /// <param name="currentMeasureValue">Current Measure Value</param>
        /// <param name="upside">Upside</param>
        /// <param name="updated">Updated</param>
        /// <param name="callback">Result of the operation: true/False</param>
        public void InsertDCFFairValueData(EntitySelectionData entitySelectionData, string valueType, int? fvMeasure, decimal? fvbuy, decimal? fvSell, decimal? currentMeasureValue, decimal? upside, DateTime? updated, Action<bool> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.InsertFairValuesAsync(entitySelectionData, valueType, fvMeasure, fvbuy, fvSell, currentMeasureValue, upside, updated);
            client.InsertFairValuesCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result);
                        }
                        else
                        {
                            callback(false);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// Fetch DCF country name Service Caller Method
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <param name="callback">Returns name of the Country</param>
        public void FetchDCFCountryName(EntitySelectionData entitySelectionData, Action<string> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCountryNameAsync(entitySelectionData);
            client.RetrieveCountryNameCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result);
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Delete FairValues for DCF
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <param name="callback">Result of the Operation</param>
        public void DeleteDCFFairValue(EntitySelectionData entitySelectionData, Action<bool> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DCFOperationsClient client = new DCFOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.DeleteFairValuesAsync(entitySelectionData);
            client.DeleteFairValuesCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result);
                        }
                        else
                        {
                            callback(false);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DCFDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #endregion

        #region Portal Enhancement
        public void RetrieveDocumentsData(String searchString, Action<List<DocumentCategoricalData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveDocumentsDataAsync(searchString);
            client.RetrieveDocumentsDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UploadDocument(String fileName, Byte[] fileByteStream, String deleteFileUrl, Action<String> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UploadDocumentAsync(fileName, fileByteStream, deleteFileUrl);
            client.UploadDocumentCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToString());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveDocument(String fileName, Action<Byte[]> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveDocumentAsync(fileName);
            client.RetrieveDocumentCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result);
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void SetUploadFileInfo(String userName, String Name, String Location, String CompanyName, String SecurityName
            , String SecurityTicker, String Type, String MetaTags, String Comments, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SetUploadFileInfoAsync(userName, Name, Location, CompanyName, SecurityName, SecurityTicker, Type, MetaTags, Comments);
            client.SetUploadFileInfoCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void GetDocumentsMetaTags(Action<List<string>> callback, Boolean OnlyTags = false)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GetDocumentsMetaTagsAsync(OnlyTags);
            client.GetDocumentsMetaTagsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveCompanyData(Action<List<String>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            ExternalResearchOperationsClient client = new ExternalResearchOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCompanyDataAsync();
            client.RetrieveCompanyDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.ExternalResearchDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void RetrieveDocumentsDataForUser(String userName, Action<List<DocumentCategoricalData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveDocumentsDataForUserAsync(userName);
            client.RetrieveDocumentsDataForUserCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void SetDocumentComment(String userName, Int64 fileId, String comment, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SetDocumentCommentAsync(userName, fileId, comment);
            client.SetDocumentCommentCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void DeleteDocument(String fileName, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.DeleteDocumentAsync(fileName);
            client.DeleteDocumentCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result);
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void UpdateDocumentsDataForUser(Int64 fileId, String fileName, String userName, String metaTags, String companyInfo
            , String categoryType, String comment, Byte[] overwriteStream, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdateDocumentsDataForUserAsync(fileId, fileName, userName, metaTags, companyInfo, categoryType, comment, overwriteStream);
            client.UpdateDocumentsDataForUserCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        public void DeleteFileMasterRecord(Int64 fileId, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.DeleteFileMasterRecordAsync(fileId);
            client.DeleteFileMasterRecordCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result);
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                        callback(null);
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region Custom Screening Tool
        /// <summary>
        /// Service call for retrieving custom controls selection list depending upon parameter which contains name of the control
        /// </summary>
        /// <param name="parameter">string</param>
        /// <param name="callback"></param>
        public void RetrieveCustomControlsList(string parameter, Action<List<string>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                                    : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCustomControlsListAsync(parameter);
            client.RetrieveCustomControlsListCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null 
                                                                                                        ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call for retrieving security data based on selected data points
        /// </summary>
        /// <param name="portfolio">PortfolioSelectionData</param>
        /// <param name="benchmark">EntitySelectionData</param>
        /// <param name="region">String</param>
        /// <param name="country">String</param>
        /// <param name="sector">String</param>
        /// <param name="industry">String</param>
        /// <param name="userPreference">String</param>
        /// <param name="callback"></param>
        public void RetrieveSecurityData(PortfolioSelectionData portfolio, EntitySelectionData benchmark, String region, String country, String sector, 
            String industry, List<CSTUserPreferenceInfo> userPreference, Action<List<CustomScreeningSecurityData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                            : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());

            long startTime = DateTime.Now.Ticks;
            client.RetrieveSecurityDataAsync(portfolio, benchmark, region, country, sector, industry, userPreference);
            client.RetrieveSecurityDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null 
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call for retrieving Security Reference Tab Data Points List
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveSecurityReferenceTabDataPoints(Action<List<CustomSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName 
                                                                                                                                : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveSecurityReferenceTabDataPointsAsync();
            client.RetrieveSecurityReferenceTabDataPointsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                        ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call for retrieving Period Financials Tab Data Points List
        /// </summary>
        /// <param name="callback"></param>
        public void RetrievePeriodFinancialsTabDataPoints(Action<List<CustomSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName 
                                                                                                                                        : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrievePeriodFinancialsTabDataPointsAsync();
            client.RetrievePeriodFinancialsTabDataPointsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call for retrieving Current Financials Tab Data Points List
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveCurrentFinancialsTabDataPoints(Action<List<CustomSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName 
                                                                                                                                : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveCurrentFinancialsTabDataPointsAsync();
            client.RetrieveCurrentFinancialsTabDataPointsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null 
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call for retrieving Fair Value Tab Data Points List
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveFairValueTabDataPoints(Action<List<CustomSelectionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                            : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFairValueTabDataPointsAsync();
            client.RetrieveFairValueTabDataPointsCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null 
                                                                                                                   ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call to save user preferred Data Points List
        /// </summary>
        /// <param name="userPreference"></param>
        /// <param name="username"></param>
        /// <param name="callback"></param>
        public void SaveUserDataPointsPreference(string userPreference, string username, Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName 
                                                                                                                                            : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SaveUserDataPointsPreferenceAsync(userPreference, username);
            client.SaveUserDataPointsPreferenceCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null 
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call to retrieve stored user preference for custom screening data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callback"></param>
        public void GetCustomScreeningUserPreferences(string username, Action<List<CSTUserPreferenceInfo>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                        : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.GetCustomScreeningUserPreferencesAsync(username);
            client.GetCustomScreeningUserPreferencesCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null 
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call to update user preferred Data Points List
        /// </summary>
        /// <param name="userPreference"></param>
        /// <param name="username"></param>
        /// <param name="existingListname"></param>
        /// <param name="newListname"></param>
        /// <param name="accessibility"></param>
        /// <param name="callback"></param>
        public void UpdateUserDataPointsPreference(string userPreference, string username, string existingListname, string newListname, string accessibility,
            Action<Boolean?> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName 
                                                                                                                                        : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UpdateUserDataPointsPreferenceAsync(userPreference, username, existingListname, newListname, accessibility);
            client.UpdateUserDataPointsPreferenceCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                            ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service call for retrieving Fair Value Tab Source List
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveFairValueTabSource(Action<List<string>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName
                                                                                                                                            : "Unspecified");
            CustomScreeningToolOperationsClient client = new CustomScreeningToolOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFairValueTabSourceAsync();
            client.RetrieveFairValueTabSourceCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.MeetingDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    { callback(null); }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }
        #endregion

        #region ExcelModel

        /// <summary>
        /// Service Caller to retrieve Data for Excel Model-Download
        /// </summary>
        /// <param name="selectedSecurity">The Selected Security in Control</param>
        /// <param name="callback">Returns the Excel file as byte Array</param>
        public void RetrieveDocumentsData(EntitySelectionData selectedSecurity, Action<byte[]> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveStatementDataAsync(selectedSecurity);
            client.RetrieveStatementDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                      = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        /// <summary>
        /// Service caller for ModelExcel worksheet
        /// </summary>
        /// <param name="fileStream">FileStream</param>
        /// <param name="userName">UserName</param>
        /// <param name="callback">callback</param>
        public void UploadModelExcelSheet(string fileName, byte[] fileStream, string userName, Action<string, string> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            DocumentWorkspaceOperationsClient client = new DocumentWorkspaceOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.UploadExcelModelAsync(fileStream, userName);
            client.UploadExcelModelCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(fileName, e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault> fault
                      = e.Error as FaultException<GreenField.ServiceCaller.DocumentWorkSpaceDefinitions.ServiceFault>;
                    //Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(fileName, fault.Reason.ToString());
                    }
                }
                else
                {
                    var msg = e.Error.ToString();
                    //Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(fileName, msg);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };
        }

        #endregion

        #region FAIR VALUE
        /// <summary>
        /// Gets FAIR VALUE SUMMARY Data
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveFairValueCompostionSummary(EntitySelectionData entitySelectionData, Action<List<FairValueCompositionSummaryData>> callback)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            FairValueOperationsClient client = new FairValueOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFairValueCompostionSummaryAsync(entitySelectionData);
            client.RetrieveFairValueCompostionSummaryCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// Gets the updated UpSide value
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveFairValueDataWithNewUpside(EntitySelectionData entitySelectionData, FairValueCompositionSummaryData editedFairValueData
            , Action<FairValueCompositionSummaryData> callback)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            FairValueOperationsClient client = new FairValueOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFairValueDataWithNewUpsideAsync(entitySelectionData, editedFairValueData);
            client.RetrieveFairValueDataWithNewUpsideCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result);
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }

        /// <summary>
        /// Save the updated values
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void SaveUpdatedFairValueData(EntitySelectionData entitySelectionData, List<FairValueCompositionSummaryData> editedFairValueDataList
            , Action<List<FairValueCompositionSummaryData>> callback)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            FairValueOperationsClient client = new FairValueOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.SaveUpdatedFairValueDataAsync(entitySelectionData, editedFairValueDataList);
            client.SaveUpdatedFairValueDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                    SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }
        /// <summary>
        /// Gets FAIR VALUE SUMMARY Data
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <param name="callback"></param>
        public void RetrieveFairValueCompostionSummaryData(EntitySelectionData entitySelectionData, Action<List<FairValueCompositionSummaryData>> callback)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            ServiceLog.LogServiceCall(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(),
                SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");

            FairValueOperationsClient client = new FairValueOperationsClient();
            client.Endpoint.Behaviors.Add(new CookieBehavior());
            long startTime = DateTime.Now.Ticks;
            client.RetrieveFairValueCompostionSummaryDataAsync(entitySelectionData);
            client.RetrieveFairValueCompostionSummaryDataCompleted += (se, e) =>
            {
                long endTime = DateTime.Now.Ticks;
                ServiceLog.LogServiceClientReceivedData(LoggerFacade, methodNamespace, e.Error, DateTime.Now.ToUniversalTime(), startTime, endTime, SessionManager.SESSION != null
                                                                                                                ? SessionManager.SESSION.UserName : "Unspecified");
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        if (e.Result != null)
                        {
                            callback(e.Result.ToList());
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.FairValueDefinitions.ServiceFault>;
                    Prompt.ShowDialog(fault.Reason.ToString(), fault.Detail.Description, MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    Prompt.ShowDialog(e.Error.Message, e.Error.GetType().ToString(), MessageBoxButton.OK);
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                ServiceLog.LogServiceCallback(LoggerFacade, methodNamespace, DateTime.Now.ToUniversalTime(), SessionManager.SESSION != null ? SessionManager.SESSION.UserName : "Unspecified");
            };

        }
        #endregion


        public void RequestMonthEndDates(Action<List<DateTime>> callback)
        {
            var client = new PerformanceOperationsClient();
            client.GetLastDayOfMonthsCompleted += (s, e) =>
            {

#warning Left undone: take care of the exceptional situiation

                callback(e.Result);
            };
            client.GetLastDayOfMonthsAsync();
        }
    }
}