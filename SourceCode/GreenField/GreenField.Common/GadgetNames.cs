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
        public const string SECURITY_OVERVIEW = "Company Overview";
        public const string INTERNAL_RESEARCH_PRICING_DETAILED = "Holdings and Positioning";
        public const string HOLDINGS_CHART_EXTENTION = "Trade History";
        public const string BENCHMARK_RELATIVE_PERFORMANCE = "Relative Performance";
        public const string EXTERNAL_RESEARCH_BASIC_DATA = "Market Data";
        public const string PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION_SUMMARY = "Fair Value";
        public const string INTERNAL_RESEARCH_CONSESUS_ESTIMATE_SUMMARY = "Comparison with Consensus";
        public const string GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC = "Summary Financials and Valuations";
        #endregion

        #region Snapshot company Profile
        //public const string INTERNAL_RESEARCH_COMPANY_PROFILE_REPORT = "Company Profile Report Internal Research 4.3";
        #endregion

        #region Snapshot Tear sheet
        //public const string PORTAL_ENHANCEMENTS_TEAR_SHEET = "Company Meeting Notes Portal Enhancements 4.2";
        #endregion
        #endregion

        #region Financials
        #region Financials Summary
        public const string EXTERNAL_RESEARCH_FUNDAMENTALS_SUMMARY = "Fundamental Summary";
        #endregion

        #region Financials Income statement
        public const string EXTERNAL_RESEARCH_INCOME_STATEMENT = "Income Statement";
        #endregion

        #region Financials Balance Sheet
        public const string EXTERNAL_RESEARCH_BALANCE_SHEET = "Balance Sheet";
        #endregion

        #region Financials Cash Flow Statement
        public const string EXTERNAL_RESEARCH_CASH_FLOW = "Cash Flow Statement";
        #endregion

        #region Financials Statistics
        public const string INTERNAL_RESEARCH_FINSTAT_REPORT = "Finstat Report";
        #endregion
        #endregion

        #region Estimates
        #region Estimates Consensus
        //public const string EXTERNAL_RESEARCH_CONSENSUS_OVERVIEW = "Consensus Overview External Research 4.1.5.1";
        //public const string EXTERNAL_RESEARCH_CONSENSUS_RECOMMENDATION = "Consensus Recommendation External Research 4.1.5.2";
        public const string EXTERNAL_RESEARCH_CONSENSUS_TARGET_PRICE = "Consensus Target Price";
        public const string EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES = "Consensus Median Estimates";
        public const string EXTERNAL_RESEARCH_CONSENSUS_VALUATIONS = "Consensus Valuations";
        #endregion

        #region Estimates Details
        public const string EXTERNAL_RESEARCH_CONSENSUS_DETAIL = "Consensus Estimates Detail";
        #endregion

        #region Estimates Comparison
        //public const string EXTERNAL_RESEARCH_CONSENSUS_COMPARISON_CHART = "Consensus Vs. EMM Comparison Chart External Research 4.4.4";
        //public const string EXTERNAL_RESEARCH_CONSENSUS_ESTIMATES_SUMMARY = "Consensus Estimates Summary External Research 4.1.5.6";
        #endregion
        #endregion

        #region Valuation
        #region Valuation DCF
        //HOLDINGS_DISCOUNTED_CASH_FLOW is not a gadget but used to signify union on following gadgets
        public const string HOLDINGS_DISCOUNTED_CASH_FLOW = "Discounted Cash Flow Holdings";
        public const string HOLDINGS_DISCOUNTED_CASH_FLOW_ASSUMPTIONS = "Assumptions";
        public const string HOLDINGS_DISCOUNTED_CASH_FLOW_TERMINAL_VALUE_CALCULATIONS = "Terminal Value Calculations";
        public const string HOLDINGS_DISCOUNTED_CASH_FLOW_SUMMARY = "DCF Summary";
        public const string HOLDINGS_DISCOUNTED_CASH_FLOW_SENSIVITY = "Sensitivity";
        public const string HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_EPS = "FORWARD EPS";
        public const string HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_BVPS = "FORWARD BVPS";        
        public const string HOLDINGS_FREE_CASH_FLOW = "Free Cash Flow";
        #endregion

        #region Valuation Fair Value
        public const string PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION = "Fair Value Composition";
        #endregion
        #endregion

        #region Documents
        public const string PORTAL_ENHANCEMENTS_DOCUMENTS = "Documents";
        #endregion

        #region Charting
        #region Charting Price/Comparison
        public const string SECURITY_REFERENCE_PRICE_COMPARISON = "Closing/Gross Price";
        #endregion

        #region Charting Unrealized Gain Loss
        public const string SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS = "Unrealized Gain/Loss";
        #endregion

        #region Charting Context
        public const string EXTERNAL_RESEARCH_SCATTER_CHART_BANK = "Scatter Charts Bank";
        public const string EXTERNAL_RESEARCH_SCATTER_CHART_INDUSTRIAL = "Scatter Charts Industrial";
        public const string EXTERNAL_RESEARCH_SCATTER_CHART_INSURANCE = "Scatter Charts Insurance";
        public const string EXTERNAL_RESEARCH_SCATTER_CHART_UTILITY = "Scatter Charts Utility";
        #endregion

        #region Charting Valuation
        //public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_BANK = "Historical Valuation Charts Bank";
        //public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INDUSTRIAL = "Historical Valuation Charts Industrial";
        //public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_INSURANCE = "Historical Valuation Charts Insurance";
        //public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_UTILITY = "Historical Valuation Charts Utility";
        public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PREVENUE = "Historical Valuation Charts P/Revenue";
        public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_EVEBITDA = "Historical Valuation Charts EV/EBITDA";
        public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PCE = "Historical Valuation Charts P/CE";
        public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE = "Historical Valuation Charts P/E";
        public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PBV = "Historical Valuation Charts P/BV";
        public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_FCFYield = "Historical Valuation Charts FCF Yield";
        public const string EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_DividendYield = "Historical Valuation Charts Dividend Yield";
        #endregion
        #endregion

        #region Corporate Governance
        #region Corporate Governance Questionnaire
        //public const string PORTFOLIO_ENRICHMENT_SCREEN_MOCKUP = "Screen Mockup Portfolio Enrichment 4.1.3";
        #endregion

        #region Corporate Governance Report
        //public const string PORTFOLIO_ENRICHMENT_REPORT = "Report Portfolio Enrichment 4.1.4";
        #endregion
        #endregion 
        #endregion        

        #region Markets
        #region Snapshot
        #region Snapshot Summary
        public const string MODELS_FX_MACRO_ECONOMICS_EM_DATA_REPORT = "Emerging Markets Data Report Models, FX and Macro Economic Data 4.6";
        #endregion

        #region Snapshot Market Performance
        public const string BENCHMARKS_MARKET_PERFORMANCE_SNAPSHOT = "Market Performance Snapshot";
        #endregion

        #region Snapshot Internal Vs. Model Valuation
        //public const string MODELS_FX_MACRO_ECONOMICS_INTERNAL_MODELS_EVALUATION_REPORT = "Internal Vs. Models Evaluation Report Models, FX and Macro Economic Data 4.5"; 
        #endregion 
        #endregion

        #region MacroEconomics
        #region MacroEconomics EM Summary
        public const string MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_DATA_REPORT = "Summary of Macro Database Key Data Report Models, FX and Macro Economic Data 4.4";
        #endregion

        #region MacroEconomics Country Summary
        public const string MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_ANNUAL_DATA_REPORT = "Summary of Macro Database Key Annual Data Report Models, FX and Macro Economic Data 4.3";
        #endregion 
        #endregion

        #region Commodities Summary
        public const string MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN = "Commodity Index ";//Returns Models, FX and Macro Economic Data 4.2.2"; 
        #endregion
        #endregion

        #region Portfolio
        #region Snapshot
        public const string HOLDINGS_TOP_TEN_HOLDINGS = "Top 10 Holdings";
        public const string HOLDINGS_RELATIVE_RISK = "Relative Risk";
        public const string HOLDINGS_REGION_BREAKDOWN = "Region Breakdown";
        public const string HOLDINGS_VALUATION_QUALITY_GROWTH_MEASURES = "Valuation, Quality, & Growth Measures";
        public const string HOLDINGS_SECTOR_BREAKDOWN = "Sector Breakdown";
        public const string HOLDINGS_RISK_RETURN = "Risk/Return";
        public const string HOLDINGS_ASSET_ALLOCATION = "Asset Allocation";
        public const string HOLDINGS_MARKET_CAPITALIZATION = "Market Capitalization";
        #endregion

        #region Holdings
        public const string HOLDINGS_PORTFOLIO_DETAILS_UI = "Portfolio Details User Interface"; 
        #endregion

        #region Performance Summary
        public const string PERFORMANCE_HEAT_MAP = "Heat Map";
        public const string PERFORMANCE_GRAPH = "Performance Graph";
        public const string PERFORMANCE_GRID = "Performance Grid";
        #endregion

        #region Performance Attribution
        public const string PERFORMANCE_ATTRIBUTION = "Attribution"; 
        #endregion

        #region Performance Relative Performance
        public const string PERFORMANCE_RELATIVE_PERFORMANCE = "Excess Contribution";
        public const string PERFORMANCE_COUNTRY_ACTIVE_POSITION = "Country Active Position";
        public const string PERFORMANCE_SECTOR_ACTIVE_POSITION = "Sector Active Position";
        public const string PERFORMANCE_SECURITY_ACTIVE_POSITION = "Security Active Position";
        public const string PERFORMANCE_CONTRIBUTOR_DETRACTOR = "Contributor Detractor"; 
        #endregion

        #region Benchmark Summary
        public const string BENCHMARKS_MULTILINE_BENCHMARK = "Multi-line Benchmarks UI Component";
        public const string BENCHMARK_TOP_TEN_CONSTITUENTS = "Top Benchmark Securities";
        public const string BENCHMARK_HOLDINGS_SECTOR_PIECHART = "Holdings PieChart For Sector";
        public const string BENCHMARK_HOLDINGS_REGION_PIECHART = "Holdings PieChart For Region";
        #endregion

        #region Benchmark Components
        public const string BENCHMARK_INDEX_CONSTITUENTS = "Index Constituents"; 
        #endregion
        #endregion

        #region Screening
        #region Quarterly Comparison
        public const string QUARTERLY_RESULTS_COMPARISON = "Quarterly Results Comparison"; 
        #endregion

        #region Stock
        public const string CUSTOM_SCREENING_TOOL = "Custom Screening Tool";
        #endregion
        #endregion

        #region Investment Committee
        public const string ICPRESENTATION_CREATE_EDIT = "Investment Committee Presentation Upload/Edit";
        public const string ICPRESENTATION_VOTE = "Investment Committee Presentation Voting Screen";
        //public const string ICPRESENTATION_PRE_MEETING_REPORT = "IC SRD 4.2.1";
        public const string ICPRESENTATION_MEETING_MINUTES = "IC Admin Meeting Minutes";
        public const string ICPRESENTATION_SUMMARY_REPORT = "Investment Committee Summary Report";
        //public const string ICPRESENTATION_METRICS_REPORT = "IC SRD 4.2.3";
        public const string ICPRESENTATION_PRESENTATIONS = "Investment Committee(IC) Presentation Overview";
        public const string ICPRESENTATION_UPLOAD_EDIT = "Investment Committee Presentation Upload/Edit";
        public const string ICPRESENTATION_PRESENTATIONS_NEW = "Investment Committee Presenation - New";
        public const string ICPRESENTATION_PRESENTATIONS_DECISION_ENTRY = "IC Admin Decision Entry Screen";        
        #endregion

        #region Admin
        public const string ADMIN_CHANGE_DATE = "Change Meeting Config Date";
        #endregion
    }
}
