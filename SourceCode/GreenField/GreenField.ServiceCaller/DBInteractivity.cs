using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.ObjectModel;
using System.Windows;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;



namespace GreenField.ServiceCaller
{
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
                    if (callback != null)
                    {
                        if (e.Result != null)
                            callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
                };
        }

        public void RetrieveSecurityReferenceDataByTicker(string ticker, Action<SecurityOverviewData> callback)
        {
            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.RetrieveSecurityReferenceDataByTickerAsync(ticker);
            client.RetrieveSecurityReferenceDataByTickerCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result);
                }
                else
                {
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
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result);
                }
                else
                {
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
            SecurityReferenceOperationsClient client = new SecurityReferenceOperationsClient();
            client.RetrievePricingReferenceDataAsync(entityIdentifiers.ToList(), startDateTime, endDateTime, totalReturnCheck, frequencyInterval);
            client.RetrievePricingReferenceDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
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
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }
        #endregion

        #endregion

        #region Build2 Interaction Methods
        public void RetrievePortfolioSelectionData(Action<List<PortfolioSelectionData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrievePortfolioSelectionDataAsync();
            client.RetrievePortfolioSelectionDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
                    callback(null);
                }
            };
        }


        public void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveBenchmarkSelectionDataAsync();
            client.RetrieveBenchmarkSelectionDataCompleted += (se, e) =>
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
            };
        }

        public void RetrieveMarketCapitalizationData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<MarketCapitalizationData> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveMarketCapitalizationDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveMarketCapitalizationDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result);
                }
                else
                {
                    callback(null);
                }
            };
        }

        public void RetrieveAssetAllocationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveAssetAllocationDataAsync(fundSelectionData, effectiveDate);
            client.RetrieveAssetAllocationDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
                    callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for sector breakdown gadget
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="benchmarkSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveSectorBreakdownDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveSectorBreakdownDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
                    callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for region breakdown gadget
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="benchmarkSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData,DateTime effectiveDate, Action<List<RegionBreakdownData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveRegionBreakdownDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveRegionBreakdownDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
                    callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for top holdings gadget
        /// </summary>
        /// <param name="fundSelectionData"></param>
        /// <param name="benchmarkSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveTopHoldingsDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveTopHoldingsDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
                    callback(null);
                }
            };
        }

        /// <summary>
        /// service call method for index constituent gadget
        /// </summary>
        /// <param name="benchmarkSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="callback"></param>
        public void RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveIndexConstituentsDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveIndexConstituentsDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
                    callback(null);
                }
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveMarketSnapshotSelectionDataAsync(userName);
            client.RetrieveMarketSnapshotSelectionDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveMarketSnapshotPreferenceAsync(userName, snapshotName);
            client.RetrieveMarketSnapshotPreferenceCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveMarketPerformanceSnapshotDataAsync(marketSnapshotPreference);
            client.RetrieveMarketPerformanceSnapshotDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.AddMarketSnapshotGroupPreferenceAsync(snapshotPreferenceId, groupName);
            client.AddMarketSnapshotGroupPreferenceCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result);
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RemoveMarketSnapshotGroupPreferenceAsync(groupPreferenceId);
            client.RemoveMarketSnapshotGroupPreferenceCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result);
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.AddMarketSnapshotEntityPreferenceAsync(marketSnapshotPreference);
            client.AddMarketSnapshotEntityPreferenceCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result);
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RemoveMarketSnapshotEntityPreferenceAsync(marketSnapshotPreference);
            client.RemoveMarketSnapshotEntityPreferenceCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result);
                }
                else
                {
                    callback(false);
                }
            };
        }
        #endregion

        public void RetriveValuesForFilters(String filterType, Action<List<String>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveValuesForFiltersAsync(filterType);
            client.RetrieveValuesForFiltersCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
                    callback(null);
                }
            };
        }

        public void RetrieveRelativePerformanceData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();

            client.RetrieveRelativePerformanceDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveRelativePerformanceDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        public void RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveRelativePerformanceSectorDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveRelativePerformanceSectorDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        public void RetrieveRelativePerformanceSecurityData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveRelativePerformanceSecurityDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate, countryID, sectorID, order, maxRecords);
            client.RetrieveRelativePerformanceSecurityDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrievePortfolioDetailsDataAsync(objPortfolioIdentifier, objSelectedDate, objGetBenchmark);
            client.RetrievePortfolioDetailsDataCompleted += (se, e) =>
            {
                if (e.Result != null)
                {
                    if (callback != null)
                        callback(e.Result.ToList());
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveBenchmarkChartReturnDataAsync(objBenchmarkIdentifier, objEffectiveDate);
            client.RetrieveBenchmarkChartReturnDataCompleted += (se, e) =>
            {
                if (e.Result != null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.ToList());
                    }
                }
                else
                {
                    callback(null);
                }
            };
        }

        public void RetrieveBenchmarkGridReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate, Action<List<BenchmarkGridReturnData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveBenchmarkGridReturnDataAsync(objBenchmarkIdentifier, objEffectiveDate);
            client.RetrieveBenchmarkGridReturnDataCompleted += (se, e) =>
            {
                if (e.Result != null)
                {
                    if (callback != null)
                    {
                        callback(e.Result.ToList());
                    }
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrievePerformanceGraphDataAsync(name);
            client.RetrievePerformanceGraphDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        #endregion

        #region Interaction Method for Attribution Gadget
        /// <summary>
        /// Method that calls the RetrieveAttributionData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="name">Name of the Fund</param>
        /// <param name="callback"></param>
        public void RetrieveAttributionData(String name, Action<List<AttributionData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveAttributionDataAsync(name);
            client.RetrieveAttributionDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveHoldingsPercentageDataForRegionAsync(fundSelectionData, effectiveDate, filterType, filterValue);
            client.RetrieveHoldingsPercentageDataForRegionCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveHoldingsPercentageDataAsync(fundSelectionData, effectiveDate, filterType, filterValue);
            client.RetrieveHoldingsPercentageDataCompleted += (se, e) =>
            {
                if (callback != null)
                {
                    if (e.Result != null)
                        callback(e.Result.ToList());
                }
                else
                {
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
        public void RetrievePerformanceGridData(string name, Action<List<PerformanceGridData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrievePerformanceGridDataAsync(name);
            client.RetrievePerformanceGridDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
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
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveTopBenchmarkSecuritiesDataAsync(portfolioSelectionData, effectiveDate);
            client.RetrieveTopBenchmarkSecuritiesDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
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
        public void RetrievePortfolioRiskReturnData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrievePortfolioRiskReturnDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrievePortfolioRiskReturnDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }
        public void RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveRelativePerformanceCountryActivePositionDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate, countryID, sectorID);
            client.RetrieveRelativePerformanceCountryActivePositionDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        public void RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveRelativePerformanceSectorActivePositionDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate, countryID, sectorID);
            client.RetrieveRelativePerformanceSectorActivePositionDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        public void RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveRelativePerformanceSecurityActivePositionDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate, countryID, sectorID);
            client.RetrieveRelativePerformanceSecurityActivePositionDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        #endregion

        #region Interaction Method for Heat Map

        public void RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<HeatMapData>> callback)
        {
            BenchmarkHoldingsPerformanceOperationsClient client = new BenchmarkHoldingsPerformanceOperationsClient();
            client.RetrieveHeatMapDataAsync();
            client.RetrieveHeatMapDataCompleted += (se, e) =>
            {
                if (callback != null)
                    if (e.Result != null)
                    {
                        callback(e.Result.ToList());
                    }
                    else
                    {
                        callback(null);
                    }
            };
        }

        #endregion

        #endregion
    }
}
