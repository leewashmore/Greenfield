using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GreenField.Common
{
    public static class GadgetNames
    {
        #region Company
        #region Snapshot
        #region Snapshot Summary
        public static string SECURITY_OVERVIEW = "Security Overview";
        public static string EXTERNAL_RESEARCH_PRICING = "Pricing Information External Research 4.1.3.1 / 4.4.3";
        public static string INTERNAL_RESEARCH_PRICING_DETAILED = "Pricing Detailed Internal Research 5.1.1.1";
        public static string EXTERNAL_RESEARCH_VALUATIONS = "Valuation Information External Research 4.1.3.8 / 4.4.3";
        public static string HOLDINGS_CHART_EXTENTION = "Chart Extention Holdings";
        public static string EXTERNAL_RESEARCH_GROWTH = "Growth Information External Research 4.1.3.9 / 4.4.3";
        public static string BENCHMARK_RELATIVE_PERFORMANCE = "Relative Performance Benchmarks";
        public static string EXTERNAL_RESEARCH_MARGINS = "Margins Information External Research 4.1.3.2 / 4.4.3";
        public static string EXTERNAL_RESEARCH_BASIC_DATA = "Basic Data";// Information External Research 4.1.4.1";
        public static string EXTERNAL_RESEARCH_LEVERAGE_CAPITAL_FINANCIAL_STRENGTH = "Leverage/Capital Structure / Financial Strength External Research 4.1.3.3 / 4.1.3.4 / 4.4.3";
        public static string PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION = "Fair Value Composition 4.2.2";
        public static string EXTERNAL_RESEARCH_ASSET_QUALITY_CASH_FLOW = "Asset Quality / Cash Flow Information External Research 4.1.3.5 / 4.1.3.6 / 4.4.3";
        public static string INTERNAL_RESEARCH_VALUATIONS_DETAILED = "Valuations Detailed Internal Research 5.1.1.2";
        public static string EXTERNAL_RESEARCH_PROFITABILITY = "Profitability Information External Research 4.1.3.7 / 4.4.3";
        public static string INTERNAL_RESEARCH_CONSESUS_ESTIMATE_SUMMARY = "Internal Research Consensus Estimates Summary";
        public static string GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC = "External Research Gadget with Period Columns COA Specific";
        #endregion

        #region Snapshot company Profile
        public static string INTERNAL_RESEARCH_COMPANY_PROFILE_REPORT = "Company Profile Report Internal Research 4.3";
        #endregion

        #region Snapshot Tear sheet
        public static string PORTAL_ENHANCEMENTS_TEAR_SHEET = "Company Meeting Notes Portal Enhancements 4.2";
        #endregion
        #endregion

        #region Financials
        #region Financials Summary
        public static string EXTERNAL_RESEARCH_FUNDAMENTALS_SUMMARY = "Fundamental Summary External Research 4.1.2.1";
        #endregion
        #region Financials Income statement
        public static string EXTERNAL_RESEARCH_INCOME_STATEMENT = "Income Statement";
        #endregion
        #region Financials Balance Sheet
        public static string EXTERNAL_RESEARCH_BALANCE_SHEET = "Balance Sheet";
        #endregion
        #region Financials Cash Flow Statement
        public static string EXTERNAL_RESEARCH_CASH_FLOW = "Cash Flow Statement";
        #endregion
        #region Financials Statistics
        public static string INTERNAL_RESEARCH_FINSTAT_REPORT = "Finstat Report Internal Research 4.2";
        #endregion
        #endregion

        #region Estimates
        #region Estimates Consensus
        public static string EXTERNAL_RESEARCH_CONSENSUS_OVERVIEW = "Consensus Overview External Research 4.1.5.1";
        public static string EXTERNAL_RESEARCH_CONSENSUS_RECOMMENDATION = "Consensus Recommendation External Research 4.1.5.2";
        public static string EXTERNAL_RESEARCH_CONSENSUS_TARGET_PRICE = "Consensus Target Price Research 4.1.5.3";
        public static string EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES = "Consensus Median Estimates External Research 4.1.5.4";
        public static string EXTERNAL_RESEARCH_CONSENSUS_VALUATIONS = "Consensus Valuations External Research 4.1.5.5";
        #endregion

        #region Estimates Details
        public static string EXTERNAL_RESEARCH_CONSENSUS_DETAIL = "Consensus Estimates Details(4.1.6.1)";
        #endregion

        #region Estimates Comparison
        public static string EXTERNAL_RESEARCH_CONSENSUS_COMPARISON_CHART = "Consensus Vs. EMM Comparison Chart External Research 4.4.4";
        public static string EXTERNAL_RESEARCH_CONSENSUS_ESTIMATES_SUMMARY = "Consensus Estimates Summary External Research 4.1.5.6";
        #endregion
        #endregion

        #region Valuation
        #region Valuation DCF
        public static string HOLDINGS_DISCOUNTED_CASH_FLOW = "DCF - Discounted Cash Flow Holdings 4.1";
        #endregion

        #region Valuation Fair Value
        public static string PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION_SUMMARY = "Fair Value Composition Summary Portfolio Construction 4.2.1";
        #endregion
        #endregion

        #region Documents
        public static string PORTAL_ENHANCEMENTS_DOCUMENTS = "Documents Portal Enhancements 4.1";
        #endregion

        #region Charting
        #region Charting Price/Comparison
        public static string SECURITY_REFERENCE_PRICE_COMPARISON = "Closing/Gross Price";
        #endregion

        #region Charting Unrealized Gain Loss
        public static string SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS = "Unrealized Gain/Loss";
        #endregion

        #region Charting Context
        public static string EXTERNAL_RESEARCH_SCATTER_CHART_BANK = "Scatter Charts Bank External Research 4.4.2";
        public static string EXTERNAL_RESEARCH_SCATTER_CHART_INDUSTRIAL = "Scatter Charts Industrial External Research 4.4.2";
        public static string EXTERNAL_RESEARCH_SCATTER_CHART_INSURANCE = "Scatter Charts Insurance External Research 4.4.2";
        public static string EXTERNAL_RESEARCH_SCATTER_CHART_UTILITY = "Scatter Charts Utility External Research 4.4.2";
        #endregion

        #region Charting Valuation
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_BANK = "Historical Valuation Charts Bank External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INDUSTRIAL = "Historical Valuation Charts Industrial External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INSURANCE = "Historical Valuation Charts Insurance External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_UTILITY = "Historical Valuation Charts Utility External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PREVENUE = "Historical Valuation Charts P/Revenue External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_EVEBITDA = "Historical Valuation Charts EV/EBITDA External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PCE = "Historical Valuation Charts P/CE External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE = "Historical Valuation Charts P/E External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PBV = "Historical Valuation Charts P/BV External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_FCFYield = "Historical Valuation Charts FCF Yield External Research 4.4.1";
        public static string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_DividendYield = "Historical Valuation Charts Dividend Yield External Research 4.4.1";
        #endregion
        #endregion

        #region Corporate Governance
        #region Corporate Governance Questionnaire
        public static string PORTFOLIO_ENRICHMENT_SCREEN_MOCKUP = "Screen Mockup Portfolio Enrichment 4.1.3";
        #endregion

        #region Corporate Governance Report
        public static string PORTFOLIO_ENRICHMENT_REPORT = "Report Portfolio Enrichment 4.1.4";
        #endregion
        #endregion 
        #endregion        

        #region Markets
        #region Snapshot Summary
        public static string MODELS_FX_MACRO_ECONOMICS_EM_DATA_REPORT = "Emerging Markets Data Report Models, FX and Macro Economic Data 4.6";
        #endregion

        #region Snapshot Market Performance
        public static string BENCHMARKS_MARKET_PERFORMANCE_SNAPSHOT = "Market Performance Snapshot"; 
        #endregion

        #region Snapshot Internal Vs. Model Valuation
        public static string MODELS_FX_MACRO_ECONOMICS_INTERNAL_MODELS_EVALUATION_REPORT = "Internal Vs. Models Evaluation Report Models, FX and Macro Economic Data 4.5"; 
        #endregion

        #region MacroEconomics EM Summary
        public static string MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_DATA_REPORT = "Summary of Macro Database Key Data Report Models, FX and Macro Economic Data 4.4"; 
        #endregion

        #region MacroEconomics Country Summary
        public static string MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_ANNUAL_DATA_REPORT = "Summary of Macro Database Key Annual Data Report Models, FX and Macro Economic Data 4.3"; 
        #endregion

        #region Commodities Summary
        public static string MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN = "Commodity Index ";//Returns Models, FX and Macro Economic Data 4.2.2"; 
        #endregion
        #endregion

        #region Portfolio
        #region Snapshot
        public static string HOLDINGS_TOP_TEN_HOLDINGS = "Top 10 Holdings";
        public static string HOLDINGS_RELATIVE_RISK = "Relative Risk";
        public static string HOLDINGS_REGION_BREAKDOWN = "Region Breakdown";
        public static string HOLDINGS_VALUATION_QUALITY_GROWTH_MEASURES = "Valuation, Quality & Growth Measures Fund Holdings and Discounted Cash Flows 4.2.1.7";
        public static string HOLDINGS_SECTOR_BREAKDOWN = "Sector Breakdown";
        public static string HOLDINGS_RISK_RETURN = "Portfolio Risk Returns";
        public static string HOLDINGS_ASSET_ALLOCATION = "Asset Allocation";
        public static string HOLDINGS_MARKET_CAPITALIZATION = "Market Capitalization";
        #endregion

        #region Holdings
        public static string HOLDINGS_PORTFOLIO_DETAILS_UI = "Portfolio Details User Interface"; 
        #endregion

        #region Performance Summary
        public static string PERFORMANCE_HEAT_MAP = "Heat Map";
        public static string PERFORMANCE_GRAPH = "Performance Graph";
        public static string PERFORMANCE_GRID = "Performance Grid";
        #endregion

        #region Performance Attribution
        public static string PERFORMANCE_ATTRIBUTION = "Attribution"; 
        #endregion

        #region Performance Relative Performance
        public static string PERFORMANCE_RELATIVE_PERFORMANCE = "Excess Contribution";
        public static string PERFORMANCE_COUNTRY_ACTIVE_POSITION = "Country Active Position (Relative Performance)";
        public static string PERFORMANCE_SECTOR_ACTIVE_POSITION = "Sector Active Position (Relative Performance)";
        public static string PERFORMANCE_SECURITY_ACTIVE_POSITION = "Security Active Position (Relative Performance)";
        public static string PERFORMANCE_CONTRIBUTOR_DETRACTOR = "All Securities"; 
        #endregion

        #region Benchmark Summary
        public static string BENCHMARKS_MULTILINE_BENCHMARK = "Multi-line Benchmarks UI Component";
        public static string BENCHMARK_TOP_TEN_CONSTITUENTS = "Top Benchmark Securities";
        public static string BENCHMARK_HOLDINGS_SECTOR_PIECHART = "Holdings PieChart For Sector";
        public static string BENCHMARK_HOLDINGS_REGION_PIECHART = "Holdings PieChart For Region";
        #endregion

        #region Benchmark Components
        public static string BENCHMARK_INDEX_CONSTITUENTS = "Index Constituents"; 
        #endregion
        #endregion

        #region Screening
        #region Quarterly Comparison
        public static string QUARTERLY_RESULTS_COMPARISON = "Quarterly Results Comparison"; 
        #endregion
        #endregion
    }
}
