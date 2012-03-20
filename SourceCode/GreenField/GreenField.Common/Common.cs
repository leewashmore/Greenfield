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

    public static class BenchmarkReturnTypes
    {
        public static string TotalReturnType = "Total(Gross)";
        public static string NetReturnType = "( Net Return )";
        public static string PriceReturnType = "( Price Return )";
    }


}
