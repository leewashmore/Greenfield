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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using System.ComponentModel;


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

    public enum MarketPerformanceSnapshotActionType
    {
        SNAPSHOT_SAVE,
        SNAPSHOT_SAVE_AS,
        SNAPSHOT_ADD,
        SNAPSHOT_REMOVE,
        SNAPSHOT_PAGE_NAVIGATION
    }

    public enum ScatterChartDefaults
    {
        BANK,
        INDUSTRIAL,
        INSURANCE,
        UTILITY
    }

    public static class Application
    {
        public static string APPLICATION_NAME = "GreenField";
    }

    public static class RegionNames
    {
        public static string MAIN_REGION = "MainRegion";
    }

    public static class LogLevel
    {
        public static Int32 DEBUG_LEVEL = 5;
        public static Int32 INFO_LEVEL = 4;
        public static Int32 WARN_LEVEL = 3;
        public static Int32 ERROR_LEVEL = 2;
        public static Int32 FATAL_LEVEL = 1;
    }

    public static class EntityNodeType
    {
        public static String NONE = "None";
        public static String COUNTRY = "Country";
        public static String SECTOR = "Sector";
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
        public List<RelativePerformanceSecurityData> RelativePerformanceSecurityInfo { get; set; }
        public List<RelativePerformanceData> RelativePerformanceInfo { get; set; }
    }

    public delegate void RelativePerformanceToggledSectorGridBuildEventHandler(RelativePerformanceToggledSectorGridBuildEventArgs e);
    public class RelativePerformanceToggledSectorGridBuildEventArgs : EventArgs
    {
        public List<RelativePerformanceSecurityData> RelativePerformanceSecurityInfo { get; set; }
        public List<string> RelativePerformanceCountryNameInfo { get; set; }
    }

    public delegate void RetrieveHeatMapDataCompleteEventHandler(RetrieveHeatMapDataCompleteEventArgs e);
    public class RetrieveHeatMapDataCompleteEventArgs : EventArgs
    {
        public List<HeatMapData> HeatMapInfo { get; set; }
    }

    public delegate void RetrieveConsensusEstimatesSummaryCompleteEventHandler(RetrieveConsensusSummaryCompletedEventsArgs e);
     public class RetrieveConsensusSummaryCompletedEventsArgs : EventArgs
    {
        public List<ConsensusEstimatesSummaryData> ConsensusInfo { get; set; }
    }

    public delegate void RetrieveHeatMapSelectedCountryEventHandler(RetrieveHeatMapSelectedCountryCompletedEventArgs e);
    public class RetrieveHeatMapSelectedCountryCompletedEventArgs : EventArgs
    {
        public String SelectedCountry { get; set; }
    }

    public delegate void RetrieveMacroCountrySummaryDataCompleteEventHandler(RetrieveMacroCountrySummaryDataCompleteEventArgs e);
    public class RetrieveMacroCountrySummaryDataCompleteEventArgs : EventArgs
    {
        public List<MacroDatabaseKeyAnnualReportData> MacroInfo { get; set; }
    }

    public delegate void ConstructDocumentSearchResultEventHandler(List<DocumentCategoricalData> e);    

    public class RelativePerformanceGridCellData
    {
        public string CountryID { get; set; }
        public string SectorID { get; set; }
    }

    public static class EntityReturnType
    {
        public static string TotalReturnType = "Total";
        public static string NetReturnType = "Net";
        public static string PriceReturnType = "Price";
    }
    
    public static class EntityType
    {
        public const string SECURITY = "SECURITY";
        public const string BENCHMARK = "BENCHMARK";
        public const string INDEX = "INDEX";
        public const string COMMODITY = "COMMODITY";
        public const string CURRENCY = "CURRENCY";
    }

    public static class HoldingsPercentageSegmentClassifier
    {
        public static int SECTOR = 0;
        public static int REGIOM = 1;
    }

    public enum MarketPerformanceSnapshotActionTypes
    {
        ADDNEW,
        SAVE,
        SAVEAS,
        DELETE
    }
    public delegate void RetrieveCommodityDataCompleteEventHandler(RetrieveCommodityDataCompleteEventArgs e);
    public class RetrieveCommodityDataCompleteEventArgs : EventArgs
    {
        public List<GreenField.DataContracts.FXCommodityData> CommodityInfo { get; set; }
    }

    public class ChangedCurrencyInEstimateDetail
    {
        public string CurrencyName { get; set; }
    }

    #region IC PRESENTATION

    public static class StatusType
    {
        public static String IN_PROGRESS = "In Progress";
        public static String READY_FOR_VOTING = "Ready for Voting";
        public static String CLOSED_FOR_VOTING = "Closed for Voting";
        public static String FINAL = "Final";
        public static String WITHDRAWN = "Withdrawn";
    }

    public static class VoteType
    {
        public static String AGREE = "Agree";
        public static String MODIFY = "Modify";
        public static String ABSTAIN = "Abstain";
    }

    public static class MeetingAttachedFileType
    {
        public static String INDUSTRY_REPORT = "Industry Report";
        public static String OTHER_DOCUMENT = "Other Document";
    }

    public static class AttendanceType
    {
        public static String ATTENDED = "Attended";
        public static String VIDEO_CONFERENCE = "Video Conference";
        public static String TELE_CONFERENCE = "Tele Conference";
        public static String NOT_PRESENT = "Not Present";
    }

    public static class PFVType
    {
        public static String FORWARD_DIVIDEND_YIELD = "Forward Dividend Yield";
        public static String FORWARD_EV_EBITDA = "Forward EV/EBITDA";
        public static String FORWARD_EV_EBITDA_RELATIVE_TO_COUNTRY = "Forward EV/EBITDA relative to Country";
        public static String FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY = "Forward EV/EBITDA relative to Industry";
        public static String FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY = "Forward EV/EBITDA relative to Industry within Country";
        public static String FORWARD_EV_REVENUE = "Forward EV/Revenue";
        public static String FORWARD_P_NAV = "Forward P/NAV";
        public static String FORWARD_P_APPRAISAL_VALUE = "Forward P/Appraisal Value";
        public static String FORWARD_P_BV = "Forward P/BV";
        public static String FORWARD_P_BV_RELATIVE_TO_COUNTRY = "Forward P/BV relative to Country";
        public static String FORWARD_P_BV_RELATIVE_TO_INDUSTRY = "Forward P/BV relative to Industry";
        public static String FORWARD_P_BV_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY = "Forward P/BV relative to Industry within Country";
        public static String FORWARD_P_CE = "Forward P/CE";
        public static String FORWARD_P_E = "Forward P/E";
        public static String FORWARD_P_E_RELATIVE_TO_COUNTRY = "Forward P/E relative to Country";
        public static String FORWARD_P_E_RELATIVE_TO_INDUSTRY = "Forward P/E relative to Industry";
        public static String FORWARD_P_E_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY = "Forward P/E relative to Industry within Country";
        public static String FORWARD_P_E_TO_2_YEAR_EARNINGS_GROWTH = "Forward P/E to 2 Year Earnings Growth";
        public static String FORWARD_P_E_TO_3_YEAR_EARNINGS_GROWTH = "Forward P/E to 3 Year Earnings Growth";
        public static String FORWARD_P_EMBEDDED_VALUE = "Forward P/Embedded Value";
        public static String FORWARD_P_REVENUE = "Forward P/Revenue";
    }

    public static class RecommendationType
    {
        public static String HOLD = "Hold";
        public static String SELL = "Sell";
        public static String BUY = "Buy";
        public static String WATCH = "Watch";
    }

    public static class MemberGroups
    {
        public static String VOTINGMEMBER = "IC_MEMBER_VOTING";
        public static String NONVOTINGMEMBER = "IC_MEMBER_NONVOTING";
        public static String ADMIN = "IC_ADMIN";       
    }

    public enum VIEWVOTEFLAG { VIEW, VOTE } ;
    public enum ViewPluginFlagEnumeration { Create, Upload, Update, View, Vote, Edit, ChangeDate };
    #endregion
}
