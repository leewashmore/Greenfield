using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;


namespace GreenField.ServiceCaller
{
    public interface IDBInteractivity
    {
        #region Build1
        void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback);

        void RetrieveSecurityReferenceDataByTicker(String ticker, Action<SecurityOverviewData> callback);

        void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback);

        void RetrieveUnrealizedGainLossData(EntitySelectionData entityIdentifier, DateTime startDateTime, DateTime endDateTime, String frequencyInterval, Action<List<UnrealizedGainLossData>> callback);

        void RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyInterval, Action<List<PricingReferenceData>> callback);
        
        #endregion


        void RetrieveFundSelectionData(Action<List<FundSelectionData>> callback);

        void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback);

       

        void RetrieveMarketCapitalizationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<MarketCapitalizationData> callback);

        void RetrieveAssetAllocationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback);

        void RetrieveSectorBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback);

        void RetrieveRegionBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RegionBreakdownData>> callback);

        void RetrieveTopHoldingsData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback);

        void RetrieveIndexConstituentsData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback);

        void RetrieveHoldingsPercentageData(FundSelectionData fundSelectionData, DateTime effectiveDate,String filterType,String filterValue,Action<List<HoldingsPercentageData>> callback);

        void RetrieveHoldingsPercentageDataForRegion(FundSelectionData fundmarkSelectionData, DateTime effectiveDate, String filterType, String filterValue, Action<List<HoldingsPercentageData>> callback);

        void RetrieveTopBenchmarkSecuritiesData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback);
        
        void RetrievePortfolioRiskReturnData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback);

        void RetrieveMarketSnapshotSelectionData(string userName, Action<List<MarketSnapshotSelectionData>> callback);

        void RetrieveMarketSnapshotPreference(string userName, string snapshotName, Action<List<MarketSnapshotPreference>> callback);                    
        

        void RetrieveMarketPerformanceSnapshotData(List<MarketSnapshotPreference> marketSnapshotPreference, Action<List<MarketPerformanceSnapshotData>> callback);

        void AddMarketSnapshotGroupPreference(int snapshotPreferenceId, string groupName, Action<bool> callback);

        void RemoveMarketSnapshotGroupPreference(int groupPreferenceId, Action<bool> callback);

        void AddMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference, Action<bool> callback);

        void RemoveMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference, Action<bool> callback);


        void RetriveValuesForFilters(String filterType, Action<List<String>> callback);

        void RetrieveRelativePerformanceData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceData>> callback);

        void RetrieveRelativePerformanceSectorData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback);

        void RetrieveRelativePerformanceSecurityData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSecurityData>> callback, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null);

        void RetrieveRelativePerformanceCountryActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null);

        void RetrieveRelativePerformanceSectorActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null);

        void RetrieveRelativePerformanceSecurityActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceActivePositionData>> callback, string countryID = null, int? sectorID = null);

        void RetrievePerformanceGraphData(String name, Action<List<PerformanceGraphData>> callback);

        void RetrievePerformanceGridData(String name, Action<List<PerformanceGridData>> callback);

        void RetrieveAttributionData(String name, Action<List<AttributionData>> callback);

        void RetrieveHeatMapData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<HeatMapData>> callback);

        #region Build2

        void RetrievePortfolioDetailsData(string objPortfolioIdentifier, DateTime objSelectedDate, Action<List<PortfolioDetailsData>> callback);

        void RetrieveBenchmarkChartReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate, Action<List<BenchmarkChartReturnData>> callback);

        void RetrieveBenchmarkGridReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate, Action<List<BenchmarkGridReturnData>> callback);
        #endregion

    }
}
