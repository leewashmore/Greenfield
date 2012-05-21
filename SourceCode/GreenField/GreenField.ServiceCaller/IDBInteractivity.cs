using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.DataContracts;

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

        void RetrieveMarketCapitalizationData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool isExCashSecurity, Action<List<MarketCapitalizationData>> callback);

        void RetrieveAssetAllocationData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback);

        void RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback);

        void RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<RegionBreakdownData>> callback);

        void RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback);

        void RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback);

        void RetrieveHoldingsPercentageData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback);

        void RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData fundmarkSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback);

        void RetrieveTopBenchmarkSecuritiesData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback);

        void RetrievePortfolioRiskReturnData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback);

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

        void RetrieveRelativePerformanceData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String period, Action<List<RelativePerformanceData>> callback);

        void RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback);

        void RetrieveRelativePerformanceSecurityData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, string sectorID = null);

        void RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null);

        void RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null);

        void RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, string period, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, string sectorID = null);

        void RetrievePerformanceGraphData(String name, Action<List<PerformanceGraphData>> callback);

        void RetrievePerformanceGridData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<PerformanceGridData>> callback);

        void RetrieveAttributionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, Action<List<AttributionData>> callback);

        void RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, Action<List<HeatMapData>> callback);

        void RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime objSelectedDate, bool objGetBenchmark, Action<List<PortfolioDetailsData>> callback);

        void RetrieveBenchmarkChartReturnData(Dictionary<string, string> objSelectedEntites, Action<List<BenchmarkChartReturnData>> callback);

        void RetrieveBenchmarkGridReturnData(Dictionary<string, string> objSelectedEntites, Action<List<BenchmarkGridReturnData>> callback);


        #endregion

        #region Slice-3

        void RetrieveRelativePerformanceUIData(Dictionary<string, string> objSelectedEntity, DateTime? objEffectiveDate, Action<List<RelativePerformanceUIData>> callback);

        void RetrieveCountrySelectionData(Action<List<CountrySelectionData>> callback);

        void RetrieveMacroDatabaseKeyAnnualReportData(String countryName, Action<List<MacroDatabaseKeyAnnualReportData>> callback);

        void RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(String countryName, Action<List<MacroDatabaseKeyAnnualReportData>> callback);
        
        void RetrieveChartExtensionData(Dictionary<string, string> objSelectedEntities, DateTime objEffectiveDate, Action<List<ChartExtensionData>> callback);


        #endregion

        #region Slice 4 - FX

        void RetrieveCommodityData(Action<List<CommodityResult>> callback);

        #endregion

    }
}
