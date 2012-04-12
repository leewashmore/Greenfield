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
using System.Text;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.Generic;


namespace GreenField.Common
{
    public static class MembershipCreateStatus
    {
        public static string DUPLICATE_EMAIL = "DuplicateEmail";
        public static string DUPLICATE_PROVIDERUSERKEY = "DuplicateProviderUserKey";
        public static string DUPLICATE_USERNAME = "DuplicateUserName";
        public static string INVALID_ANSWER = "InvalidAnswer";
        public static string INVALID_EMAIL = "InvalidEmail";
        public static string INVALID_PASSWORD = "InvalidPassword";
        public static string INVALID_PROVIDERUSERKEY = "InvalidProviderUserKey";
        public static string INVALID_QUESTION = "InvalidQuestion";
        public static string PROVIDER_ERROR = "ProviderError";
        public static string SUCCESS = "Success";
        public static string USER_REJECTED = "UserRejected";
    }

    public static class Application
    {
        public static string APPLICATION_NAME = "GreenField";
    }

    public static class RegionNames
    {
        public static string MAIN_REGION = "MainRegion";
    }

    public static class GadgetNames
    {
        public static string SECURITY_OVERVIEW = "Security Overview";
        public static string PRICING = "Closing/Gross Price";
        public static string UNREALIZED_GAINLOSS = "Unrealized Gain/Loss";
        public static string REGION_BREAKDOWN = "Region Breakdown";
        public static string SECTOR_BREAKDOWN = "Sector Breakdown";
        public static string INDEX_CONSTITUENTS = "Index Constituents";
        public static string MARKET_CAPITALIZATION = "Market Capitalization";
        public static string TOP_HOLDINGS = "Top 10 Holdings";
        public static string ASSET_ALLOCATION = "Asset Allocation";
        public static string HOLDINGS_PIECHART = "Holdings PieChart";
        public static string PORTFOLIO_RISK_RETURNS = "Portfolio Risk Returns";
        public static string TOP_BENCHMARK_SECURITIES = "Top Benchmark Securities";
        public static string TOP_CONTRIBUTOR = "Top 5 Contributors";
        
        public static string COUNTRY_ACTIVE_POSITION = "Country Active Position (Relative Performance)";
        public static string SECTOR_ACTIVE_POSITION = "Sector Active Position (Relative Performance)";
        public static string TOP_DETRACTOR = "Top 5 Detractors";
        public static string CONTRIBUTOR_DETRACTOR = "All Securities";
        public static string PERFORMANCE_GRAPH = "Performance Graph";       
        public static string ATTRIBUTION = "Attribution";
        public static string PERFORMANCE_GRID = "Performance Grid";

        public static string EXTERNAL_RESEARCH_PRICING = "Pricing Information External Research 4.1.3.1 / 4.4.3";
        public static string INTERNAL_RESEARCH_PRICING_DETAILED = "Pricing Detailed Internal Research 5.1.1.1";
        public static string EXTERNAL_RESEARCH_VALUATIONS = "Valuation Information External Research 4.1.3.8 / 4.4.3";
        public static string HOLDINGS_CHART_EXTENTION = "Chart Extention Holdings 4.4.1.1";
        public static string EXTERNAL_RESEARCH_GROWTH = "Growth Information External Research 4.1.3.9 / 4.4.3";
        public static string BENCHMARK_RELATIVE_PERFORMANCE = "Relative Performance Benchmarks 4.1.1.5";
        public static string EXTERNAL_RESEARCH_MARGINS = "Margins Information External Research 4.1.3.2 / 4.4.3";
        public static string EXTERNAL_RESEARCH_BASIC_DATA = "Basic Data Information External Research 4.1.4.1";
        public static string EXTERNAL_RESEARCH_LEVERAGE_CAPITAL_FINANCIAL_STRENGTH = "Leverage/Capital Structure / Financial Strength External Research 4.1.3.3 / 4.1.3.4 / 4.4.3";
        public static string PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION = "Fair Value Composition Summary Portfolio Construction 4.2.2";
        public static string EXTERNAL_RESEARCH_ASSET_QUALITY_CASH_FLOW = "Asset Quality / Cash Flow Information External Research 4.1.3.5 / 4.1.3.6 / 4.4.3";
        public static string INTERNAL_RESEARCH_VALUATIONS_DETAILED = "Valuations Detailed Internal Research 5.1.1.2";
        public static string EXTERNAL_RESEARCH_PROFITABILITY = "Profitability Information External Research 4.1.3.7 / 4.4.3";
    }

    public static class EntityTypes
    {
        public static string SECURITY = "SECURITY";
    }

    public static class LogLevel
    {
        public static Int32 DEBUG_LEVEL = 5;
        public static Int32 INFO_LEVEL = 4;
        public static Int32 WARN_LEVEL = 3;
        public static Int32 ERROR_LEVEL = 2;
        public static Int32 FATAL_LEVEL = 1;
    }

    public delegate void DataRetrievalProgressIndicatorEventHandler(DataRetrievalProgressIndicatorEventArgs e);

    public class DataRetrievalProgressIndicatorEventArgs : EventArgs
    {
        public bool ShowBusy { get; set; }
    }

    public delegate void RelativePerformanceGridBuildEventHandler(RelativePerformanceGridBuildEventArgs e);
    public class RelativePerformanceGridBuildEventArgs : EventArgs
    {
        public List<RelativePerformanceSectorData> RelativePerformanceSectorInfo { get; set; }
        public List<RelativePerformanceData> RelativePerformanceInfo { get; set; }
    }

    public class RelativePerformanceGridCellData
    {
        public string CountryID { get; set; }
        public int? SectorID { get; set; }        
    }

      public static class BenchmarkReturnTypes
    {
        public static string TotalReturnType = "Total(Gross)";
        public static string NetReturnType = "( Net Return )";
        public static string PriceReturnType = "( Price Return )";
    }

    
      public static class HoldingsPercentageSegmentClassifier
      {
          public static int SECTOR = 0;
          public static int REGIOM = 1;
      }

      



}
