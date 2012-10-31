using System;
using System.Linq;
using System.Collections.Generic;
using GreenField.Common;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;

namespace GreenField.App.Helpers
{
    /// <summary>
    /// Stores information that can be used to create a gadget object
    /// </summary>
    public class GadgetInfo
    {
        #region Properties
        /// <summary>
        /// Display Name for the gadget
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// Type of the view class for the gadget
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// Type of the view model class for the gadget
        /// </summary>
        public Type ViewModelType { get; set; } 
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public GadgetInfo()
        {
        }

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="displayName">Display Name for the gadget</param>
        /// <param name="viewType">Type of the view class for the gadget</param>
        /// <param name="viewModelType">Type of the view model class for the gadget</param>
        public GadgetInfo(String displayName, Type viewType, Type viewModelType)
        {
            DisplayName = displayName;
            ViewType = viewType;
            ViewModelType = viewModelType;
        } 
        #endregion
    }

    /// <summary>
    /// Class for storing gadget listings for dashboard
    /// </summary>
    public static class GadgetListing
    {
        /// <summary>
        /// Stores gadget listings for dashboard
        /// </summary>
        private static List<GadgetInfo> info;
        public static List<GadgetInfo> Info 
        {
            get
            {
                if(info == null)
                {
                    info = new List<GadgetInfo>();
                    info.Add(new GadgetInfo(GadgetNames.BENCHMARK_HOLDINGS_REGION_PIECHART, typeof(ViewHoldingsPieChartRegion), typeof(ViewModelHoldingsPieChartRegion)));
                    info.Add(new GadgetInfo(GadgetNames.BENCHMARK_HOLDINGS_SECTOR_PIECHART, typeof(ViewHoldingsPieChart), typeof(ViewModelHoldingsPieChart)));
                    info.Add(new GadgetInfo(GadgetNames.BENCHMARK_INDEX_CONSTITUENTS, typeof(ViewIndexConstituents), typeof(ViewModelIndexConstituents)));
                    info.Add(new GadgetInfo(GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE, typeof(ViewRelativePerformanceUI), typeof(ViewModelRelativePerformanceUI)));
                    info.Add(new GadgetInfo(GadgetNames.BENCHMARK_TOP_TEN_CONSTITUENTS, typeof(ViewTopBenchmarkSecurities), typeof(ViewModelTopBenchmarkSecurities)));
                    info.Add(new GadgetInfo(GadgetNames.BENCHMARKS_MARKET_PERFORMANCE_SNAPSHOT, typeof(ViewMarketPerformanceSnapshot), typeof(ViewModelMarketPerformanceSnapshot)));
                    info.Add(new GadgetInfo(GadgetNames.BENCHMARKS_MULTILINE_BENCHMARK, typeof(ViewMultiLineBenchmark), typeof(ViewModelMultiLineBenchmark)));
                    info.Add(new GadgetInfo(GadgetNames.CUSTOM_SCREENING_TOOL, typeof(ViewCustomScreeningTool), typeof(ViewModelCustomScreeningTool)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_ASSET_QUALITY_CASH_FLOW, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_BALANCE_SHEET, typeof(ViewFinancialStatements), typeof(ViewModelFinancialStatements)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_BASIC_DATA, typeof(ViewBasicData), typeof(ViewModelBasicData)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CASH_FLOW, typeof(ViewFinancialStatements), typeof(ViewModelFinancialStatements)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_COMPARISON_CHART, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_DETAIL, typeof(ViewConsensusEstimatesDetails), typeof(ViewModelConsensusEstimatesDetails)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_ESTIMATES_SUMMARY, typeof(ViewConsensusEstimateSummary), typeof(ViewModelConsensusEstimateSummary)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES, typeof(ViewEstimates), typeof(ViewModelEstimates)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_OVERVIEW, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_RECOMMENDATION, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_TARGET_PRICE, typeof(ViewTargetPrice), typeof(ViewModelTargetPrice)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_VALUATIONS, typeof(ViewValuations), typeof(ViewModelValuations)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_FUNDAMENTALS_SUMMARY, typeof(ViewFinancialStatements), typeof(ViewModelFinancialStatements)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_GROWTH, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_BANK, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_DividendYield, typeof(ViewDividendYield), typeof(ViewModelDividendYield)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_EVEBITDA, typeof(ViewEVEBITDA), typeof(ViewModelEVEBITDA)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_FCFYield, typeof(ViewFCFYield), typeof(ViewModelFCFYield)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INDUSTRIAL, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INSURANCE, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PBV, typeof(ViewPBV), typeof(ViewModelPBV)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PCE, typeof(ViewPCE), typeof(ViewModelPCE)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE, typeof(ViewPE), typeof(ViewModelPE)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PREVENUE, typeof(ViewPRevenue), typeof(ViewModelPRevenue)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_UTILITY, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_INCOME_STATEMENT, typeof(ViewFinancialStatements), typeof(ViewModelFinancialStatements)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_LEVERAGE_CAPITAL_FINANCIAL_STRENGTH, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_MARGINS, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_PRICING, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_PROFITABILITY, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_BANK, typeof(ViewScatterGraph), typeof(ViewModelScatterGraph)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INDUSTRIAL, typeof(ViewScatterGraph), typeof(ViewModelScatterGraph)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INSURANCE, typeof(ViewScatterGraph), typeof(ViewModelScatterGraph)));
                    info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_UTILITY, typeof(ViewScatterGraph), typeof(ViewModelScatterGraph)));
                    //info.Add(new GadgetInfo(GadgetNames.EXTERNAL_RESEARCH_VALUATIONS, typeof(ViewValuations), typeof(ViewModelValuations)));
                    info.Add(new GadgetInfo(GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC, typeof(ViewCOASpecific), typeof(ViewModelCOASpecific)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_ASSET_ALLOCATION, typeof(ViewAssetAllocation), typeof(ViewModelAssetAllocation)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_CHART_EXTENTION, typeof(ViewSlice1ChartExtension), typeof(ViewModelSlice1ChartExtension)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.HOLDINGS_FREE_CASH_FLOW, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_MARKET_CAPITALIZATION, typeof(ViewMarketCapitalization), typeof(ViewModelMarketCapitalization)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_PORTFOLIO_DETAILS_UI, typeof(ViewPortfolioDetails), typeof(ViewModelPortfolioDetails)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_REGION_BREAKDOWN, typeof(ViewRegionBreakdown), typeof(ViewModelRegionBreakdown)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_RELATIVE_RISK, typeof(ViewRiskIndexExposures), typeof(ViewModelRiskIndexExposures)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_RISK_RETURN, typeof(ViewPortfolioRiskReturns), typeof(ViewModelPortfolioRiskReturns)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_SECTOR_BREAKDOWN, typeof(ViewSectorBreakdown), typeof(ViewModelSectorBreakdown)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS, typeof(ViewTopHoldings), typeof(ViewModelTopHoldings)));
                    info.Add(new GadgetInfo(GadgetNames.HOLDINGS_VALUATION_QUALITY_GROWTH_MEASURES, typeof(ViewValuationQualityGrowth), typeof(ViewModelValuationQualityGrowth)));
                    //info.Add(new GadgetInfo(GadgetNames.INTERNAL_RESEARCH_COMPANY_PROFILE_REPORT, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.INTERNAL_RESEARCH_CONSESUS_ESTIMATE_SUMMARY, typeof(ViewConsensusEstimateSummary), typeof(ViewModelConsensusEstimateSummary)));
                    info.Add(new GadgetInfo(GadgetNames.INTERNAL_RESEARCH_FINSTAT_REPORT, typeof(ViewFinstat), typeof(ViewModelFinstat)));
                    info.Add(new GadgetInfo(GadgetNames.INTERNAL_RESEARCH_PRICING_DETAILED, typeof(ViewCompositeFund), typeof(ViewModelCompositeFund)));
                    //info.Add(new GadgetInfo(GadgetNames.INTERNAL_RESEARCH_VALUATIONS_DETAILED, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN, typeof(ViewCommodityIndex), typeof(ViewModelCommodityIndex)));
                    info.Add(new GadgetInfo(GadgetNames.MODELS_FX_MACRO_ECONOMICS_EM_DATA_REPORT, typeof(ViewEMSummaryMarketData), typeof(ViewModelEMSummaryMarketData))); // Pending
                    //info.Add(new GadgetInfo(GadgetNames.MODELS_FX_MACRO_ECONOMICS_INTERNAL_MODELS_EVALUATION_REPORT, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_ANNUAL_DATA_REPORT, typeof(ViewMacroDBKeyAnnualReport), typeof(ViewModelMacroDBKeyAnnualReport)));
                    info.Add(new GadgetInfo(GadgetNames.MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_DATA_REPORT, typeof(ViewMacroDBKeyAnnualReportEMSummary), typeof(ViewModelMacroDBKeyAnnualReportEMSummary)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_ATTRIBUTION, typeof(ViewAttribution), typeof(ViewModelAttribution)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_CONTRIBUTOR_DETRACTOR, typeof(ViewContributorDetractor), typeof(ViewModelContributorDetractor)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION, typeof(ViewRelativePerformanceCountryActivePosition), typeof(ViewModelRelativePerformanceCountryActivePosition)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_GRAPH, typeof(ViewPerformanceGadget), typeof(ViewModelPerformanceGadget)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_GRID, typeof(ViewPerformanceGrid), typeof(ViewModelPerformanceGrid)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_HEAT_MAP, typeof(ViewHeatMap), typeof(ViewModelHeatMap)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_RELATIVE_PERFORMANCE, typeof(ViewRelativePerformance), typeof(ViewModelRelativePerformance)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION, typeof(ViewRelativePerformanceSectorActivePosition), typeof(ViewModelRelativePerformanceSectorActivePosition)));
                    info.Add(new GadgetInfo(GadgetNames.PERFORMANCE_SECURITY_ACTIVE_POSITION, typeof(ViewRelativePerformanceSecurityActivePosition), typeof(ViewModelRelativePerformanceSecurityActivePosition)));
                    //info.Add(new GadgetInfo(GadgetNames.PORTAL_ENHANCEMENTS_DOCUMENTS, typeof(ViewDocuments), typeof(ViewModelDocuments)));
                    //info.Add(new GadgetInfo(GadgetNames.PORTAL_ENHANCEMENTS_TEAR_SHEET, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION, typeof(ViewFairValueComposition), typeof(ViewModelFairValueComposition)));
                    info.Add(new GadgetInfo(GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION_SUMMARY, typeof(ViewFairValueCompositionSummary), typeof(ViewModelFairValueCompositionSummary)));
                    //info.Add(new GadgetInfo(GadgetNames.PORTFOLIO_ENRICHMENT_REPORT, typeof(string), typeof(string)));
                    //info.Add(new GadgetInfo(GadgetNames.PORTFOLIO_ENRICHMENT_SCREEN_MOCKUP, typeof(string), typeof(string)));
                    info.Add(new GadgetInfo(GadgetNames.QUARTERLY_RESULTS_COMPARISON, typeof(ViewQuarterlyResultsComparison), typeof(ViewModelQuarterlyResultsComparison)));
                    info.Add(new GadgetInfo(GadgetNames.SECURITY_OVERVIEW, typeof(ViewSecurityOverview), typeof(ViewModelSecurityOverview)));
                    info.Add(new GadgetInfo(GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON, typeof(ViewClosingPriceChart), typeof(ViewModelClosingPriceChart)));
                    info.Add(new GadgetInfo(GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS, typeof(ViewUnrealizedGainLoss), typeof(ViewModelUnrealizedGainLoss)));
                }
                info = info.OrderBy(record => record.DisplayName).ToList();
                return info;
            }
        }        
    }

}
