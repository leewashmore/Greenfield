using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using GreenField.DashboardModule.Views;
using GreenField.DashBoardModule.Views.Screening;

namespace GreenField.DashboardModule
{
    [ModuleExport(typeof(DashboardModule))]
    public class DashboardModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public DashboardModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboard));

            #region Company
            #region Charting
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingClosingPrice));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingContext));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingUnrealizedGainLoss));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyChartingValuation));
            #endregion

            #region Corporate Governance
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyCorporateGovernanceQuestionnaire));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyCorporateGovernanceReport));
            #endregion

            #region Documents
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyDocuments));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyDocumentsLoad));
            #endregion

            #region Estimates
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyEstimatesComparison));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyEstimatesConsensus));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyEstimatesDetailed));
            #endregion

            #region Financials
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsBalanceSheet));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsCashFlow));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsFinStat));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsIncomeStatement));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsPeerComparison));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyFinancialsSummary));
            #endregion

            #region Snapshot
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanySnapshotCompanyProfile));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanySnapshotSummary));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanySnapshotTearSheet));
            #endregion

            #region Valuation
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyValuationDiscountedCashFlow));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCompanyValuationFairValue));
            #endregion 
            #endregion

            #region Markets
            #region Commodities
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardMarketsCommoditiesSummary));
            #endregion

            #region MacroEconomics
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardMarketsMacroEconomicsCountrySummary));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardMarketsMacroEconomicsEMSummary));            
            #endregion

            #region Snapshot
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardMarketsSnapshotInternalModelValuation));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardMarketsSnapshotMarketPerformance));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardMarketsSnapshotSummary));            
            #endregion           
            #endregion

            #region Portfolio
            #region Benchmark
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardPortfolioBenchmarkComponents));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardPortfolioBenchmarkSummary));
            #endregion

            #region Holdings
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardPortfolioHoldings));
            #endregion

            #region Performance
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardPortfolioPerformanceAttribution));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardPortfolioPerformanceRelativePerformance));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardPortfolioPerformanceSummary));
            #endregion

            #region Snapshot
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardPortfolioSnapshot));
            #endregion            
            #endregion

            #region Screening
            #region Quarterly Comparison
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardQuarterlyResultsComparison));
            #endregion

            #region Stock
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCustomScreeningTool));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardCustomScreeningToolNewDataList));           
            #endregion
            #endregion

            #region Investment Committee
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeCreataEditPresentations));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeVote));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteePreMeetingReport));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeMeetingMinutes));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeSummaryReport));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeMetricsReport));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteePresentations));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeEditPresentations));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeNew));
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardInvestmentCommitteeDecisionEntry)); 
            #endregion

            #region Admin
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardAdminInvestmentCommitteeChangeDate));
            #endregion


            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashboardTargetingBroadGlobalActive));
        }
    }
}
