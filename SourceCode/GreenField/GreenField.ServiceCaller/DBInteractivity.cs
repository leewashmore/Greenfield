using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.ObjectModel;
using System.Windows;


namespace GreenField.ServiceCaller
{
    [Export(typeof(IDBInteractivity))]
    public class DBInteractivity : IDBInteractivity
    {
        public void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveSecurityReferenceDataAsync();
            client.RetrieveSecurityReferenceDataCompleted += (se, e) =>
                {
                    if (callback != null)
                        callback(e.Result.ToList());
                };
        }

        public void RetrieveSecurityReferenceDataByTicker(string ticker, Action<SecurityOverviewData> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveSecurityReferenceDataByTickerAsync(ticker);
            client.RetrieveSecurityReferenceDataByTickerCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveEntitySelectionDataAsync();
            client.RetrieveEntitySelectionDataCompleted += (se, e) =>
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

        public void RetrieveFundSelectionData(Action<List<FundSelectionData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveFundSelectionDataAsync();
            client.RetrieveFundSelectionDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrievePricingReferenceDataAsync(entityIdentifiers.ToList(), startDateTime, endDateTime, totalReturnCheck, frequencyInterval);
            client.RetrievePricingReferenceDataCompleted += (se, e) =>
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

        public void RetrieveMarketCapitalizationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<MarketCapitalizationData> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveMarketCapitalizationDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveMarketCapitalizationDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RetrieveAssetAllocationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveAssetAllocationDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveAssetAllocationDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveSectorBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveSectorBreakdownDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveSectorBreakdownDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveRegionBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RegionBreakdownData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveRegionBreakdownDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveRegionBreakdownDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveTopHoldingsData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveTopHoldingsDataAsync(fundSelectionData, benchmarkSelectionData, effectiveDate);
            client.RetrieveTopHoldingsDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveIndexConstituentsData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveIndexConstituentsDataAsync(benchmarkSelectionData, effectiveDate);
            client.RetrieveIndexConstituentsDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }       

        public void RetrieveUserPreferenceBenchmarkData(string userName, Action<List<UserBenchmarkPreference>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveUserPreferenceBenchmarkDataAsync(userName);
            client.RetrieveUserPreferenceBenchmarkDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void RetrieveMorningSnapshotData(List<UserBenchmarkPreference> userBenchmarkPreference, Action<List<MorningSnapshotData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveMorningSnapshotDataAsync(userBenchmarkPreference);
            client.RetrieveMorningSnapshotDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }

        public void AddUserPreferenceBenchmarkGroup(string userName, string groupName, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.AddUserPreferenceBenchmarkGroupAsync(userName, groupName);
            client.AddUserPreferenceBenchmarkGroupCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RemoveUserPreferenceBenchmarkGroup(string userName, string groupName, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RemoveUserPreferenceBenchmarkGroupAsync(userName, groupName);
            client.RemoveUserPreferenceBenchmarkGroupCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void AddUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.AddUserPreferenceBenchmarkAsync(userName, userBenchmarkPreference);
            client.AddUserPreferenceBenchmarkCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }

        public void RemoveUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference, Action<bool> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RemoveUserPreferenceBenchmarkAsync(userName, userBenchmarkPreference);
            client.RemoveUserPreferenceBenchmarkCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }
       
        public void RetriveValuesForFilters(String filterType, Action<List<String>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveValuesForFiltersAsync(filterType);
            client.RetrieveValuesForFiltersCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result);
            };
        }
        
        public void RetrieveRelativePerformanceData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        public void RetrieveRelativePerformanceSectorData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        public void RetrieveRelativePerformanceSecurityData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        #region Interaction Method for Performance Graph Gadget
        /// <summary>
        /// Method that calls the RetrievePerformanceGraphData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="name">Name of the fund</param>
        /// <param name="callback"></param>
        public void RetrievePerformanceGraphData(String name, Action<List<PerformanceGraphData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrievePerformanceGraphDataAsync(name);
            client.RetrievePerformanceGraphDataCompleted+= (se, e) =>
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
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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
        /// <param name="fundSelectionData">Object of type FundSelectionData Class containg the fund selection data</param>
        /// <param name="effectiveDate">Effective date as selected by the user</param>
        /// <param name="filterType">The filter type selected by the user</param>
        /// <param name="filterValue">The filter value selected by the user</param>
        /// <param name="callback"></param>   
        public void RetrieveHoldingsPercentageDataForRegion(FundSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveHoldingsPercentageDataForRegionAsync(fundSelectionData, effectiveDate, filterType, filterValue);
            client.RetrieveHoldingsPercentageDataForRegionCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
            };
        }
        
        
        /// <summary>
        /// Method that calls the  RetrieveHoldingsPercentageData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="fundSelectionData">Object of type FundSelectionData Class containg the fund selection data</param>
        /// <param name="effectiveDate">Effective date as selected by the user</param>
        /// <param name="filterType">The filter type selected by the user</param>
        /// <param name="filterValue">The filter value selected by the user</param>
        /// <param name="callback"></param>  
        public void RetrieveHoldingsPercentageData(FundSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveHoldingsPercentageDataAsync(fundSelectionData, effectiveDate, filterType, filterValue);
            client.RetrieveHoldingsPercentageDataCompleted += (se, e) =>
            {
                if (callback != null)
                    callback(e.Result.ToList());
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
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        #region Interaction Method for Theoretical Unrealized Gain Loss Gadget

        /// <summary>
        /// Method that calls the Unrealized Gain Loss service method and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="entityIdentifier">Unique Identifier for each security</param>
        /// <param name="startDateTime">Start Date time of the time period selected</param>
        /// <param name="endDateTime">End Date time of the time period selected</param>
        /// <param name="frequencyInterval">frequency Interval selected</param>
        /// <param name="callback"></param>
        public void RetrieveUnrealizedGainLossData(string entityIdentifier, DateTime startDateTime, DateTime endDateTime, string frequencyInterval, Action<List<UnrealizedGainLossData>> callback)
        {

            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        #region Interaction Methods for Benchmark

        /// <summary>
        /// Method that calls the RetrieveTopBenchmarkSecuritiesData method of the service and provides interation between the Viewmodel and Service.
        /// </summary>
        /// <param name="benchmarkSelectionData">object containing Benchmark Selection Data</param>
        /// <param name="effectiveDate">Effective Date selected by the user</param>
        /// <param name="callback">callback</param>
        public void RetrieveTopBenchmarkSecuritiesData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
            client.RetrieveTopBenchmarkSecuritiesDataAsync(benchmarkSelectionData, effectiveDate);
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
        public void RetrievePortfolioRiskReturnData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        public void RetrieveRelativePerformanceCountryActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        public void RetrieveRelativePerformanceSectorActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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

        public void RetrieveRelativePerformanceSecurityActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null)
        {
            ProxyDataDefinitions.ProxyDataOperationsClient client = new ProxyDataDefinitions.ProxyDataOperationsClient();
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
    }
}
