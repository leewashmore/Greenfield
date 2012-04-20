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
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;


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
        SNAPSHOT_REMOVE
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

    public delegate void RetrieveHeatMapDataCompleteEventHandler(RetrieveHeatMapDataCompleteEventArgs e);
    public class RetrieveHeatMapDataCompleteEventArgs : EventArgs
    {
        public List<HeatMapData> HeatMapInfo { get; set; }
    }

    public class RelativePerformanceGridCellData
    {
        public string CountryID { get; set; }
        public int? SectorID { get; set; }        
    }

      public static class EntityReturnType
    {
        public static string TotalReturnType = "Total(Gross)";
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

      



}
