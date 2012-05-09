using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.ObjectModel;
using System.Windows;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using System.ServiceModel;


namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Service Caller class for Security Reference Data and Holdings, Benchmark & Performance Data.
    /// </summary>
    [Export(typeof(IDBInteractivity))]
    public class DBInteractivity : IDBInteractivity
    {
        #region Build1

        /// <summary>
        /// service call method for security overview gadget
        /// </summary>
        /// <param name="callback"></param>
        /// <summary>
        /// service call method for security overview gadget
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback)
        {
            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.RetrieveSecurityReferenceDataAsync();
            client.RetrieveSecurityReferenceDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveSecurityOverviewData(EntitySelectionData entitySelectionData, Action<SecurityOverviewData> callback)
        {
            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.RetrieveSecurityOverviewDataAsync(entitySelectionData);
            client.RetrieveSecurityOverviewDataCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                        callback(e.Result);
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback)
        {
            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.RetrieveEntitySelectionDataAsync();
            client.RetrieveEntitySelectionDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// Service Caller Method for Closing Price Chart
        /// </summary>
        /// <param name="entityIdentifiers"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="totalReturnCheck"></param>
        /// <param name="frequencyInterval"></param>
        /// <param name="chartEntityTypes"></param>
        /// <param name="callback"></param>
        public void RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyInterval, Action<List<PricingReferenceData>> callback)
        {
            if (entityIdentifiers == null)
                throw new ArgumentNullException();

            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.RetrievePricingReferenceDataAsync(entityIdentifiers.ToList(), startDateTime, endDateTime, totalReturnCheck, frequencyInterval);
            client.RetrievePricingReferenceDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
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

            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.RetrieveUnrealizedGainLossDataAsync(entityIdentifier, startDateTime, endDateTime, frequencyInterval);
            client.RetrieveUnrealizedGainLossDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }
        #endregion

        #endregion

        #region Build2 Interaction Methods
        public void RetrievePortfolioSelectionData(Action<List<PortfolioSelectionData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrievePortfolioSelectionDataAsync();
            client.RetrievePortfolioSelectionDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveBenchmarkSelectionDataAsync();
            client.RetrieveBenchmarkSelectionDataCompleted += (se, e) =>
            {
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
                    else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                    {
                        FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                            = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                        MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                        if (callback != null)
                            callback(null);
                    }
                }
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
        public void RetrieveMarketCapitalizationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool isExCashSecurity, Action<List<MarketCapitalizationData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveMarketCapitalizationDataAsync(fundSelectionData, effectiveDate, filterType, filterValue, isExCashSecurity);
            client.RetrieveMarketCapitalizationDataCompleted += (se, e) =>
            {
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
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveAssetAllocationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveAssetAllocationDataAsync(fundSelectionData, effectiveDate);
            client.RetrieveAssetAllocationDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for sector breakdown gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveSectorBreakdownDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveSectorBreakdownDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for region breakdown gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<RegionBreakdownData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveRegionBreakdownDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveRegionBreakdownDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for top holdings gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveTopHoldingsDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveTopHoldingsDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for index constituent gadget
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveIndexConstituentsDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveIndexConstituentsDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        #region Market Performance Gadget
        /// <summary>
        /// service call method for retrieving list of market performance snapshots for a user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="callback"></param>
        public void RetrieveMarketSnapshotSelectionData(string userName, Action<List<MarketSnapshotSelectionData>> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.RetrieveMarketSnapshotSelectionDataAsync(userName);
            client.RetrieveMarketSnapshotSelectionDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };

        }

        /// <summary>
        /// service call method for retrieving user preference of entities in “Market Performance Snapshot”
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="snapshotName"></param>
        /// <param name="callback"></param>
        public void RetrieveMarketSnapshotPreference(string userName, string snapshotName, Action<List<MarketSnapshotPreference>> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.RetrieveMarketSnapshotPreferenceAsync(userName, snapshotName);
            client.RetrieveMarketSnapshotPreferenceCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for “Market Performance Snapshot”
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void RetrieveMarketPerformanceSnapshotData(List<MarketSnapshotPreference> marketSnapshotPreference, Action<List<MarketPerformanceSnapshotData>> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.RetrieveMarketPerformanceSnapshotDataAsync(marketSnapshotPreference);
            client.RetrieveMarketPerformanceSnapshotDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// service call method to add user preferred group in “Market Performance Snapshot”
        /// </summary>
        /// <param name="snapshotPreferenceId"></param>
        /// <param name="groupName"></param>
        /// <param name="callback"></param>
        public void AddMarketSnapshotGroupPreference(int snapshotPreferenceId, string groupName, Action<bool> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.AddMarketSnapshotGroupPreferenceAsync(snapshotPreferenceId, groupName);
            client.AddMarketSnapshotGroupPreferenceCompleted += (se, e) =>
            {
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
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(false);
                }
            };
        }

        /// <summary>
        ///  service call method to remove user preferred group from “Market Performance Snapshot”
        /// </summary>
        /// <param name="groupPreferenceId"></param>
        /// <param name="callback"></param>
        public void RemoveMarketSnapshotGroupPreference(int groupPreferenceId, Action<bool> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.RemoveMarketSnapshotGroupPreferenceAsync(groupPreferenceId);
            client.RemoveMarketSnapshotGroupPreferenceCompleted += (se, e) =>
            {
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
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(false);
                }
            };
        }

        /// <summary>
        /// service call method to add user preferred entity in “Market Performance Snapshot”
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void AddMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference, Action<bool> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.AddMarketSnapshotEntityPreferenceAsync(marketSnapshotPreference);
            client.AddMarketSnapshotEntityPreferenceCompleted += (se, e) =>
            {
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
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(false);
                }
            };
        }

        /// <summary>
        ///  service call method to remove user preferred entity from “Market Performance Snapshot”
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void RemoveMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference, Action<bool> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.RemoveMarketSnapshotEntityPreferenceAsync(marketSnapshotPreference);
            client.RemoveMarketSnapshotEntityPreferenceCompleted += (se, e) =>
            {
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
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(false);
                }
            };
        }

        /// <summary>
        ///  service call method to save changes in user snapshot entity from “Market Performance Snapshot”
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void SaveMarketSnapshotPreference(string userName, MarketSnapshotSelectionData marketSnapshotSelectionData, List<MarketSnapshotPreference> createEntityPreferenceInfo, List<MarketSnapshotPreference> updateEntityPreferenceInfo
            , List<MarketSnapshotPreference> deleteEntityPreferenceInfo, List<int> deleteGroupPreferenceInfo, List<string> createGroupPreferenceInfo, Action<List<MarketSnapshotPreference>> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.SaveMarketSnapshotPreferenceAsync(userName, marketSnapshotSelectionData, createEntityPreferenceInfo, updateEntityPreferenceInfo
                , deleteEntityPreferenceInfo, deleteGroupPreferenceInfo, createGroupPreferenceInfo);
            client.SaveMarketSnapshotPreferenceCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        ///  service call method to save changes in user snapshot entity from “Market Performance Snapshot” as a new snapshot
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <param name="callback"></param>
        public void SaveAsMarketSnapshotPreference(string userName, string snapshotName
            , List<MarketSnapshotPreference> snapshotPreference, Action<MarketSnapshotSelectionData> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.SaveAsMarketSnapshotPreferenceAsync(userName, snapshotName, snapshotPreference);
            client.SaveAsMarketSnapshotPreferenceCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RemoveMarketSnapshotPreference(string userName, string snapshotName, Action<bool?> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.RemoveMarketSnapshotPreferenceAsync(userName, snapshotName);
            client.RemoveMarketSnapshotPreferenceCompleted += (se, e) =>
            {
                if (e.Error == null)
                {
                    if (callback != null)
                    {
                        callback(e.Result);
                    }
                }
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.PerformanceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }
        #endregion



        /// <summary>
        /// Retrieves filter values for a selected filter type by calling the service
        /// </summary>
        /// <param name="filterType">Filter Type selected by the user</param>
        /// <param name="effectiveDate">Effected Date selected by the user</param>
        /// <param name="callback">callback method</param>
        public void RetrieveFilterSelectionData(DateTime? effectiveDate, Action<List<FilterSelectionData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveFilterSelectionDataAsync(effectiveDate);
            client.RetrieveFilterSelectionDataCompleted += (se, e) =>
            {
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveRelativePerformanceData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate,String period, Action<List<RelativePerformanceData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();

           client.RetrieveRelativePerformanceDataAsync(portfolioSelectionData, effectiveDate, period);
            client.RetrieveRelativePerformanceDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveRelativePerformanceSectorDataAsync(fundSelectionData, effectiveDate);
            client.RetrieveRelativePerformanceSectorDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveRelativePerformanceSecurityData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate,string period, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, string sectorID = null)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveRelativePerformanceSecurityDataAsync(portfolioSelectionData, effectiveDate,period, countryID, sectorID);
            client.RetrieveRelativePerformanceSecurityDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        #region Build2

        /// <summary>
        /// Service caller method to retrieve PortfolioDetails Data
        /// </summary>
        /// <param name="objPortfolioIdentifier">Portfolio Identifier</param>
        /// <param name="objSelectedDate">Date for which data is required</param>
        /// <param name="callback">collection of Portfolio Details Data</param>
        public void RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime objSelectedDate, bool objGetBenchmark, Action<List<PortfolioDetailsData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrievePortfolioDetailsDataAsync(objPortfolioIdentifier, objSelectedDate, objGetBenchmark);
            client.RetrievePortfolioDetailsDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// Service caller method to retrieve Benchmark Return Data for multiLine Benchmark Chart
        /// </summary>
        /// <param name="objBenchmarkIdentifier">Benchmark Identifier</param>
        /// <param name="objEffectiveDate">Effective Date for which Data is Required</param>
        /// <param name="callback">Collection of Benchmark Return Data</param>
        public void RetrieveBenchmarkChartReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate, Action<List<BenchmarkChartReturnData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveBenchmarkChartReturnDataAsync(objBenchmarkIdentifier, objEffectiveDate);
            client.RetrieveBenchmarkChartReturnDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveBenchmarkGridReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate, Action<List<BenchmarkGridReturnData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveBenchmarkGridReturnDataAsync(objBenchmarkIdentifier, objEffectiveDate);
            client.RetrieveBenchmarkGridReturnDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        #endregion

        #region Interaction Method for Performance Graph Gadget
        /// <summary>
        /// Method that calls the RetrievePerformanceGraphData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="name">Name of the fund</param>
        /// <param name="callback"></param>
        public void RetrievePerformanceGraphData(String name, Action<List<PerformanceGraphData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrievePerformanceGraphDataAsync(name);
            client.RetrievePerformanceGraphDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        #endregion

        #region Interaction Method for Attribution Gadget
        /// <summary>
        /// Method that calls the RetrieveAttributionData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="portfolioSelectionData">Contains the selected portfolio</param>
        /// <param name="effectiveDate">Contains the selected effective date</param>
        /// <param name="callback">callback</param>
        public void RetrieveAttributionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<AttributionData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveAttributionDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveAttributionDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }
        #endregion

        #region Interaction Methods for Holdings Pie Chart

        /// <summary>
        /// Method that calls the  RetrieveHoldingsPercentageDataForRegion method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="fundSelectionData">Object of type PortfolioSelectionData Class containg the fund selection data</param>
        /// <param name="effectiveDate">Effective date as selected by the user</param>
        /// <param name="filterType">The filter type selected by the user</param>
        /// <param name="filterValue">The filter value selected by the user</param>
        /// <param name="callback"></param>   
        public void RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveHoldingsPercentageDataForRegionAsync(fundSelectionData, effectiveDate, filterType, filterValue);
            client.RetrieveHoldingsPercentageDataForRegionCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
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
        public void RetrieveHoldingsPercentageData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveHoldingsPercentageDataAsync(fundSelectionData, effectiveDate, filterType, filterValue);
            client.RetrieveHoldingsPercentageDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }
        #endregion

        #region Interaction Method for Performance Grid Gadget
        /// <summary>
        /// Method that calls the RetrievePerformanceGridData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="name">Name of the fund</param>
        /// <param name="callback"></param>
        public void RetrievePerformanceGridData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<PerformanceGridData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrievePerformanceGridDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrievePerformanceGridDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
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
        public void RetrieveTopBenchmarkSecuritiesData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveTopBenchmarkSecuritiesDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveTopBenchmarkSecuritiesDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        /// <summary>
        /// Method that calls the RetrievePortfolioRiskReturnData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="fundSelectionData">object containing Fund Selection Data</param>
        /// <param name="benchmarkSelectionData">object containing Benchmark Selection Data</param>
        /// <param name="effectiveDate">Effective Date selected by the user</param>
        /// <param name="callback">callback</param>
        public void RetrievePortfolioRiskReturnData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrievePortfolioRiskReturnDataAsync(portfolioSelectionData, effectiveDate, effectiveDate);
            client.RetrievePortfolioRiskReturnDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }
        public void RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveRelativePerformanceCountryActivePositionDataAsync(portfolioSelectionData,effectiveDate,period, countryID, sectorID);
            client.RetrieveRelativePerformanceCountryActivePositionDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveRelativePerformanceSectorActivePositionDataAsync(portfolioSelectionData,effectiveDate,period, countryID, sectorID);
            client.RetrieveRelativePerformanceSectorActivePositionDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        public void RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveRelativePerformanceSecurityActivePositionDataAsync(portfolioSelectionData,effectiveDate,period, countryID, sectorID);
            client.RetrieveRelativePerformanceSecurityActivePositionDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        #endregion

        #region Interaction Method for Heat Map

        public void RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<HeatMapData>> callback)
        {
            BenchmarkHoldingsOperationsClient client = new BenchmarkHoldingsOperationsClient();
            client.RetrieveHeatMapDataAsync();
            client.RetrieveHeatMapDataCompleted += (se, e) =>
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
                else if (e.Error is FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>)
                {
                    FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault> fault
                        = e.Error as FaultException<GreenField.ServiceCaller.SecurityReferenceDefinitions.ServiceFault>;
                    MessageBox.Show(fault.Detail.Description + "\n" + fault.Reason.ToString());
                    if (callback != null)
                        callback(null);
                }
            };
        }

        #endregion

        #endregion

        #region Slice3 Interaction Methods

        /// <summary>
        /// Service Caller Method for Relative Performance UI Data
        /// </summary>
        /// <param name="objSelectedEntity">Data of Selected Entities</param>
        /// <param name="objEffectiveDate">Selected Date</param>
        /// <param name="callback">List of Relative Performance Data</param>
        public void RetrieveRelativePerformanceUIData(Dictionary<string, string> objSelectedEntity, DateTime? objEffectiveDate, Action<List<RelativePerformanceUIData>> callback)
        {
            PerformanceOperationsClient client = new PerformanceOperationsClient();
            client.RetrieveRelativePerformanceUIDataAsync(objSelectedEntity, objEffectiveDate);
            client.RetrieveRelativePerformanceUIDataCompleted += (se, e) =>
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
            };
        }

        #endregion
    }
}
