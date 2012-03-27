using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.ObjectModel;

namespace GreenField.ServiceCaller
{
    public interface IDBInteractivity
    {
        void GetMessage(Action<String> callback);

        void RetrieveDetailedEstimates(String companyName, String periodType, String estimateType,
            Action<List<DetailedEstimates_Result>> callback);

        void RetrieveConsensusEstimates(String companyName, String periodType, Action<List<ConsensusEstimates_Result>> callback);

        void RetrieveCompaniesList(Action<List<GetCompanies_Result>> callback);

        void RetrieveDimensionDataListView(Action<List<String>> callback);

        void RetrieveDimensionDataForSelectedView(String viewName, Action<List<HoldingsData>> callback);

        void RetrievePerformanceDataForSelectedView(String viewName, Action<List<PerformanceData>> callback);

        void RetrievePerformanceDataListView(Action<List<String>> callback);

        void RetrieveReferenceDataForSelectedView(String viewName, Action<List<ReferenceData>> callback);

        void RetrieveReferenceDataListView(Action<List<String>> callback);

        void RetrieveAggregateDataListView(Action<List<String>> callback);

        void RetrieveAggregateDataForSelectedView(String portfolioName, Action<List<AggregatedData>> callback);

        void RetrievePortfolioNames(String viewName, Action<List<String>> callback);

        void RetrieveSecurityReferenceData(Action<List<SecurityOverviewData>> callback);

        void RetrieveSecurityReferenceDataByTicker(String ticker, Action<SecurityOverviewData> callback);

        void RetrieveEntitySelectionData(Action<List<EntitySelectionData>> callback);

        void RetrieveFundSelectionData(Action<List<FundSelectionData>> callback);

        void RetrieveBenchmarkSelectionData(Action<List<BenchmarkSelectionData>> callback);

        void RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyInterval, bool chartEntityTypes, Action<List<PricingReferenceData>> callback);

        void RetrieveMarketCapitalizationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<MarketCapitalizationData> callback);

        void RetrieveAssetAllocationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<AssetAllocationData>> callback);

        void RetrieveSectorBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<SectorBreakdownData>> callback);

        void RetrieveRegionBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RegionBreakdownData>> callback);

        void RetrieveTopHoldingsData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopHoldingsData>> callback);

        void RetrieveIndexConstituentsData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<IndexConstituentsData>> callback);

        void RetrieveHoldingsPercentageData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<HoldingsPercentageData>> callback);

        void RetrieveTopBenchmarkSecuritiesData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<TopBenchmarkSecuritiesData>> callback);
        
        void RetrievePortfolioRiskReturnData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<PortfolioRiskReturnData>> callback);

        void RetrieveUserPreferenceBenchmarkData(string userName, Action<List<UserBenchmarkPreference>> callback);        
        
        void RetrieveUnrealizedGainLossData(String entityIdentifier, DateTime startDateTime, DateTime endDateTime, String frequencyInterval ,Action<List<UnrealizedGainLossData>> callback);
        
        void RetrieveMorningSnapshotData(List<UserBenchmarkPreference> userBenchmarkPreference, Action<List<MorningSnapshotData>> callback);

        void AddUserPreferenceBenchmarkGroup(string userName, string groupName, Action<bool> callback);

        void RemoveUserPreferenceBenchmarkGroup(string userName, string groupName, Action<bool> callback);

        void AddUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference, Action<bool> callback);

        void RemoveUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference, Action<bool> callback);

        void RetrieveRelativePerformanceData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceData>> callback);

        void RetrieveRelativePerformanceSectorData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, Action<List<RelativePerformanceSectorData>> callback);               
        
    }
}
