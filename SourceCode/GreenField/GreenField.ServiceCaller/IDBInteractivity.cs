using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;


namespace GreenField.ServiceCaller
{
    /// <summary>
    /// Service Caller Interface for Security Reference Data and Holdings, Benchmark & Performance Data.
    /// </summary>
    public interface IDBInteractivity
    {
        #region Build1
        void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback);

        void RetrieveSecurityOverviewData(EntitySelectionData entitySelectionData, Action<SecurityOverviewData> callback);

        void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback);

        void RetrieveUnrealizedGainLossData(EntitySelectionData entityIdentifier, DateTime startDateTime, DateTime endDateTime, String frequencyInterval, Action<List<UnrealizedGainLossData>> callback);

        void RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyInterval, Action<List<PricingReferenceData>> callback);

        #endregion

        #region Build2

        void RetrievePortfolioSelectionData(Action<List<PortfolioSelectionData>> callback);

        void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback);

        void RetrieveMarketCapitalizationData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<MarketCapitalizationData> callback);

        void RetrieveAssetAllocationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback);

        void RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback);

        void RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<RegionBreakdownData>> callback);

        void RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback);

        void RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback);

        void RetrieveHoldingsPercentageData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback);

        void RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData fundmarkSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback);

        void RetrieveTopBenchmarkSecuritiesData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback);

        void RetrievePortfolioRiskReturnData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback);

        void RetrieveMarketSnapshotSelectionData(string userName, Action<List<MarketSnapshotSelectionData>> callback);

        void RetrieveMarketSnapshotPreference(string userName, string snapshotName, Action<List<MarketSnapshotPreference>> callback);

        void RetrieveMarketPerformanceSnapshotData(List<MarketSnapshotPreference> marketSnapshotPreference, Action<List<MarketPerformanceSnapshotData>> callback);

        void AddMarketSnapshotGroupPreference(int snapshotPreferenceId, string groupName, Action<bool> callback);

        void RemoveMarketSnapshotGroupPreference(int groupPreferenceId, Action<bool> callback);

        void AddMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference, Action<bool> callback);

        void RemoveMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference, Action<bool> callback);

        /// <summary>
        /// Save changes in a specific user snapshot
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="marketSnapshotSelectionData">Snapshot details</param>
        /// <param name="createEntityPreferenceInfo">Snapshot preference for entities that are to be created</param>
        /// <param name="updateEntityPreferenceInfo">Snapshot preference for entities that are to be updated</param>
        /// <param name="deleteEntityPreferenceInfo">Snapshot preference for entities that are to be removed</param>
        /// <param name="deleteGroupPreferenceInfo">Group preference Ids for entity groups that are to be removed</param>
        /// <param name="createGroupPreferenceInfo">Group names for entity groups that are to be created</param>
        /// <param name="callback">Callback method that takes List of MarketSnapshotPreference as its argument</param>
        void SaveMarketSnapshotPreference(string userName, MarketSnapshotSelectionData marketSnapshotSelectionData, List<MarketSnapshotPreference> createEntityPreferenceInfo, List<MarketSnapshotPreference> updateEntityPreferenceInfo
            , List<MarketSnapshotPreference> deleteEntityPreferenceInfo, List<int> deleteGroupPreferenceInfo, List<string> createGroupPreferenceInfo, Action<List<MarketSnapshotPreference>> callback);

        /// <summary>
        /// Save changes to a new snapshot specified by user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="snapshotName">Snapshot name</param>
        /// <param name="snapshotPreference">Snapshot preference details</param>
        /// <param name="callback">Callback Method that takes List of MarketSnapshotSelectionData as it's argument</param>
        void SaveAsMarketSnapshotPreference(string userName, string snapshotName, List<MarketSnapshotPreference> snapshotPreference, Action<MarketSnapshotSelectionData> callback);

        void RemoveMarketSnapshotPreference(string userName, string snapshotName, Action<bool?> callback);

        void RetrieveFilterSelectionData(DateTime? effectiveDate, Action<List<FilterSelectionData>> callback);

        void RetrieveRelativePerformanceData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceData>> callback);

        void RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback);

        void RetrieveRelativePerformanceSecurityData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null);

        void RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null);

        void RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null);

        void RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null);

        void RetrievePerformanceGraphData(String name, Action<List<PerformanceGraphData>> callback);

        void RetrievePerformanceGridData(String name, Action<List<PerformanceGridData>> callback);

        void RetrieveAttributionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<AttributionData>> callback);

        void RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<HeatMapData>> callback);

        void RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime objSelectedDate, bool objGetBenchmark, Action<List<PortfolioDetailsData>> callback);

        void RetrieveBenchmarkChartReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate, Action<List<BenchmarkChartReturnData>> callback);

        void RetrieveBenchmarkGridReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate, Action<List<BenchmarkGridReturnData>> callback);
        #endregion

    }
}
