using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace GreenField.IssuerShares.Server
{
    public static class Settings
    {
        public static Int32 RecordsPerBulk {  get { return GreenField.IssuerShares.Server.Properties.Settings.Default.RecordsPerBulk; } }
        public static Int32 RecordsPerChunk{  get { return GreenField.IssuerShares.Server.Properties.Settings.Default.RecordsPerChunk; } }
        public static Int32 MaxNumberOfDaysToStopExtrapolatingAfter { get { return GreenField.IssuerShares.Server.Properties.Settings.Default.MaxNumberOfDaysToStopExtrapolatingAfter; } }
        public static Int32? NumberOfDaysAgoToStartLoadingFrom { get { return GreenField.IssuerShares.Server.Properties.Settings.Default.NumberOfDaysAgoToStartLoadingFrom; } }
        public static Int32 NumberOfDaysBeforeLoadingDateToBeGuaranteeFromHittingGap { get { return GreenField.IssuerShares.Server.Properties.Settings.Default.NumberOfDaysBeforeLoadingDateToBeGuaranteeFromHittingGap; } }
        public static String ConnectionToAims { get { return ConfigurationManager.ConnectionStrings["Aims"].ConnectionString; } }
        public static String ConnectionToAimsEntities { get { return ConfigurationManager.ConnectionStrings["AimsEntities"].ConnectionString; } }
        public static String ODataServiceUri { get { return ConfigurationManager.AppSettings["DimensionWebService"]; } }
    }
}
