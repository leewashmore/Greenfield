using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.Helpers;
using GreenField.DAL;
using GreenField.Web.DataContracts;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service class for Performance Operations
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PerformanceOperations
    {
        #region PropertyDeclaration

        private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));

                return dimensionEntity;
            }
        }

        #endregion

        #region FaultResourceManager

        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        #endregion

        #region PerformanceServices

        /// <summary>
        /// Service Method for RelativePerformanceUI gadget
        /// </summary>
        /// <param name="objSelectedEntity">details of selected Portfolio & Security</param>
        /// <param name="objEffectiveDate">selected effective Date</param>
        /// <returns>List of RelativePerformanceUIData</returns>
        [OperationContract]
        public List<RelativePerformanceUIData> RetrieveRelativePerformanceUIData(Dictionary<string, string> objSelectedEntity, DateTime objEffectiveDate)
        {
            try
            {
                //Null Arguement Check
                if ((objSelectedEntity == null) || (objEffectiveDate == null) || (objSelectedEntity.Count == 0))
                    return new List<RelativePerformanceUIData>();

                //If dictionary object doesn't contains Security/Portfolio data, return empty set
                if (!objSelectedEntity.ContainsKey("SECURITY") || !objSelectedEntity.ContainsKey("PORTFOLIO"))
                    return new List<RelativePerformanceUIData>();

                List<RelativePerformanceUIData> result = new List<RelativePerformanceUIData>();

                //Create new Entity for service
                DimensionEntitiesService.Entities entity = DimensionEntity;

                List<string> countryName = new List<string>();
                List<string> benchmarkName = new List<string>();
                List<string> sectorName = new List<string>();

                string securityName = objSelectedEntity.Where(a => a.Key == "SECURITY").First().Value;
                string portfolioName = objSelectedEntity.Where(a => a.Key == "PORTFOLIO").First().Value;

                List<GF_SECURITY_BASEVIEW> securityBaseData = (entity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME.ToUpper().Trim() == securityName.ToUpper().Trim()).ToList());

                countryName = securityBaseData.Select(a => a.ASEC_SEC_COUNTRY_NAME).ToList();
                sectorName = securityBaseData.Select(a => a.GICS_SECTOR_NAME).ToList();
                benchmarkName = (entity.GF_PERF_DAILY_ATTRIBUTION.Where(a => a.PORTFOLIO.ToUpper().Trim() == portfolioName.ToUpper().Trim() && a.BMNAME != null).Take(1).ToList()).
                    Select(a => a.BMNAME).Distinct().ToList();


                //if (benchmarkName == null)
                //    throw new Exception("No Benchmark is found for the selected Portfolio");

                //if (benchmarkName.Count == 0)
                //    throw new Exception("No Benchmark is allotted for the selected Portfolio");
                //else if (benchmarkName.Count > 1)
                //    throw new Exception("More then 1 Benchmark is allotted to selected Portfolio ");

                //if (countryName == null)
                //    throw new Exception("No Country is found for the selected security");

                //if (countryName.Count == 0)
                //    throw new Exception("No Country is allotted for the selected Security");
                //else if (countryName.Count > 1)
                //    throw new Exception("More then 1 country is allotted to selected security ");

                //if (sectorName == null)
                //    throw new Exception("No Sector is Allotted for the selected security");

                //if (sectorName.Count == 0)
                //    throw new Exception("No Sector is allotted for the selected Security");
                //else if (sectorName.Count > 1)
                //    throw new Exception("More then 1 sector is allotted to selected security ");

                if (benchmarkName == null)
                    return result;
                if (benchmarkName.Count != 1)
                    return result;
                if (countryName == null)
                    return result;
                if (countryName.Count != 1)
                    return result;
                if (sectorName == null)
                    return result;
                if (sectorName.Count != 1)
                    return result;

                List<GF_PERF_DAILY_ATTRIBUTION> dimensionDailyPerfData = entity.GF_PERF_DAILY_ATTRIBUTION.Where(a =>
                    ((a.AGG_LVL_1_LONG_NAME.ToUpper().Trim() == securityName.ToUpper().Trim()) || (a.NODE_NAME.ToUpper().Trim() == "COUNTRY" && a.AGG_LVL_1_LONG_NAME.ToUpper().Trim() == countryName.First().ToUpper().Trim()) || (a.PORTFOLIO.ToUpper().Trim() == portfolioName.ToUpper().Trim() && a.NODE_NAME.ToUpper().Trim() == "GICS LEVEL 5" && a.AGG_LVL_1_LONG_NAME.ToUpper().Trim() == sectorName.First().ToUpper().Trim()))
                    && a.TO_DATE == objEffectiveDate.Date).ToList();

                result = RelativePerformanceUICalculations.CalculateRelativePerformanceUIData(dimensionDailyPerfData);

                if (result == null)
                    throw new InvalidOperationException
                        ("Method Name: CalculateRelativePerformanceUIData, Class: GreenField.Web.Helpers.RelativePerformanceUICalculations, Result Null Exception");

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Method to retrieve data in Benchmark Chart
        /// </summary>
        /// <param name="objBenchmarkIdentifier"></param>
        /// <param name="objStartDate"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<BenchmarkChartReturnData> RetrieveBenchmarkChartReturnData(Dictionary<string, string> objSelectedEntities)
        {
            try
            {
                List<BenchmarkChartReturnData> result = new List<BenchmarkChartReturnData>();

                //Arguement null Exception
                if (objSelectedEntities == null)
                    return result;
                if (!objSelectedEntities.ContainsKey("SECURITY") || (!objSelectedEntities.ContainsKey("PORTFOLIO")))
                    return result;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                string securityLongName = "";
                string portfolioId = "";
                DateTime startDate = DateTime.Today.AddYears(-1);
                List<string> countryName;
                List<string> benchmarkName;

                if (objSelectedEntities.ContainsKey("SECURITY"))
                    securityLongName = objSelectedEntities.Where(a => a.Key == "SECURITY").First().Value;
                if (objSelectedEntities.ContainsKey("PORTFOLIO"))
                    portfolioId = objSelectedEntities.Where(a => a.Key == "PORTFOLIO").First().Value;

                countryName = (entity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME == securityLongName).ToList()).Select(a => a.ASEC_SEC_COUNTRY_NAME).Distinct().ToList();
                benchmarkName = (entity.GF_PORTFOLIO_HOLDINGS.Where(a => a.PORTFOLIO_ID == portfolioId).ToList()).Select(a => a.BENCHMARK_ID).Distinct().ToList();

                //if (benchmarkName == null)
                //    throw new Exception("No Benchmark is found for the selected Portfolio");

                //if (benchmarkName.Count == 0)
                //    throw new Exception("No Benchmark is allotted for the selected Portfolio");
                //else if (benchmarkName.Count > 1)
                //    throw new Exception("More then 1 Benchmark is allotted to selected Portfolio ");

                //if (countryName == null)
                //    throw new Exception("No Country is found for the selected security");

                //if (countryName.Count == 0)
                //    throw new Exception("No Country is allotted for the selected Security");
                //else if (countryName.Count > 1)
                //    throw new Exception("More then 1 country is allotted to selected security ");

                if (benchmarkName == null)
                    return result;
                if (benchmarkName.Count != 1)
                    return result;
                if (countryName == null)
                    return result;
                if (countryName.Count != 1)
                    return result;

                List<GF_PERF_DAILY_ATTRIBUTION> dimensionDailyPerfData = entity.GF_PERF_DAILY_ATTRIBUTION.
                    Where(a => a.PORTFOLIO == portfolioId
                            && ((a.AGG_LVL_1_LONG_NAME == securityLongName) || ((a.NODE_NAME.ToUpper() == "COUNTRY") && (a.COUNTRY_NAME == countryName.First())))
                            && a.TO_DATE >= startDate).ToList();

                //Checking contents of Data fetched from Dimension
                if (dimensionDailyPerfData == null || dimensionDailyPerfData.Count == 0)
                    return result;

                result = MultiLineBenchmarkUICalculations.RetrieveBenchmarkChartData(dimensionDailyPerfData);

                if (result == null)
                    throw new InvalidOperationException();

                return result;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Method to retrieve data in Multi-Line Benchmark UI Grid
        /// </summary>
        /// <param name="objSelectedEntities"> Selected Security & Portfolio</param>
        /// <returns>List of BenchmarkGridReturnData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<BenchmarkGridReturnData> RetrieveBenchmarkGridReturnData(Dictionary<string, string> objSelectedEntities)
        {
            List<BenchmarkGridReturnData> result = new List<BenchmarkGridReturnData>();
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;

                if (objSelectedEntities == null)
                    return result;
                if (!objSelectedEntities.ContainsKey("SECURITY") || (!objSelectedEntities.ContainsKey("PORTFOLIO")))
                    return result;


                #region CalculatingStartDate

                DateTime firstDayPreviousMonth;
                DateTime firstDayCurrentMonth;
                DateTime currentDate = DateTime.Today;

                DateTime startDatePreviousYear = new DateTime(currentDate.Year - 1, 12, 1);
                DateTime endDatePreviousYear = new DateTime(currentDate.Year - 1, 12, 31);

                DateTime startDateTwoPreviousYear = new DateTime(currentDate.Year - 2, 12, 1);
                DateTime endDateTwoPreviousYear = new DateTime(currentDate.Year - 2, 12, 31);

                DateTime startDateThreePreviousYear = new DateTime(currentDate.Year - 3, 12, 1);
                DateTime endDateThreePreviousYear = new DateTime(currentDate.Year - 3, 12, 31);

                if (currentDate.Month == 1)
                    firstDayPreviousMonth = new DateTime(currentDate.Year - 1, 12, 1);
                else
                    firstDayPreviousMonth = new DateTime(currentDate.Year, currentDate.Month - 1, 1);

                firstDayCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                DateTime startDate = firstDayPreviousMonth;
                DateTime endDate = firstDayCurrentMonth;

                #endregion

                string securityLongName = "";
                string portfolioId = "";
                List<string> countryName;
                List<string> benchmarkName = new List<string>();


                if (objSelectedEntities.ContainsKey("SECURITY"))
                    securityLongName = (objSelectedEntities.Where(a => a.Key == "SECURITY").First().Value);
                if (objSelectedEntities.ContainsKey("PORTFOLIO"))
                    portfolioId = (objSelectedEntities.Where(a => a.Key == "PORTFOLIO").First().Value);

                countryName = (entity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME.ToUpper() == securityLongName).ToList()).Select(a => a.ASEC_SEC_COUNTRY_NAME).ToList();
                benchmarkName = (entity.GF_PORTFOLIO_HOLDINGS.Where(a => a.PORTFOLIO_ID == portfolioId).ToList()).Select(a => a.BENCHMARK_ID).ToList();


                //if (benchmarkName == null)
                //    throw new Exception("No Benchmark is found for the selected Portfolio");

                //if (benchmarkName.Count == 0)
                //    throw new Exception("No Benchmark is allotted for the selected Portfolio");

                //else if (benchmarkName.Count > 1)
                //    throw new Exception("More then 1 Benchmark is allotted to selected Portfolio ");

                //if (countryName == null)
                //    throw new Exception("No Country is found for the selected security");

                //if (countryName.Count == 0)
                //    throw new Exception("No Country is allotted for the selected Security");

                //else if (countryName.Count > 1)
                //    throw new Exception("More then 1 country is allotted to selected security ");

                if (benchmarkName == null)
                    return result;
                if (benchmarkName.Count != 1)
                    return result;
                if (countryName == null)
                    return result;
                if (countryName.Count != 1)
                    return result;

                List<GF_PERF_DAILY_ATTRIBUTION> dimensionPerfDailyData = entity.GF_PERF_DAILY_ATTRIBUTION.
                    Where(a => a.PORTFOLIO.ToUpper() == portfolioId &&
                        ((a.AGG_LVL_1_LONG_NAME.ToUpper() == securityLongName.ToUpper()) ||
                        ((a.NODE_NAME.ToUpper() == "COUNTRY") && (a.COUNTRY_NAME.ToUpper() == countryName.First().ToUpper())))
                        && ((a.TO_DATE > startDate && a.TO_DATE <= endDate) || (a.TO_DATE > startDatePreviousYear && a.TO_DATE <= endDatePreviousYear) || (a.TO_DATE > startDateTwoPreviousYear && a.TO_DATE <= endDateTwoPreviousYear) || (a.TO_DATE > startDateThreePreviousYear && a.TO_DATE <= endDateThreePreviousYear))).ToList();

                if (dimensionPerfDailyData == null)
                    throw new Exception("Service returned Null");

                if (dimensionPerfDailyData.Count() != 0)
                    result = MultiLineBenchmarkUICalculations.RetrieveBenchmarkGridData(dimensionPerfDailyData);

                if (result == null)
                    throw new InvalidOperationException();

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Service for Chart Extension Data
        /// </summary>
        /// <param name="objSelectedSecurity">Selected Security</param>
        /// <param name="objSelectedPortfolio">Selected Portfolio</param>
        /// <param name="objStartDate">start Date for the Chart</param>
        /// <returns>Collection of Chart Extension Data</returns>
        [OperationContract]
        public List<ChartExtensionData> RetrieveChartExtensionData(Dictionary<string, string> objSelectedEntities, DateTime objStartDate)
        {
            //Arguement null check
            if (objSelectedEntities == null || objStartDate == null)
                return new List<ChartExtensionData>();
            List<ChartExtensionData> result = new List<ChartExtensionData>();


            string longName = "";
            string portfolioID = "";

            //Create new Entity for service
            DimensionEntitiesService.Entities entity = DimensionEntity;

            bool isServiceUp;
            if (objSelectedEntities.ContainsKey("SECURITY"))
                longName = objSelectedEntities.Where(a => a.Key == "SECURITY").First().Value;
            else
                return new List<ChartExtensionData>();

            if (objSelectedEntities.ContainsKey("PORTFOLIO"))
                portfolioID = objSelectedEntities.Where(a => a.Key == "PORTFOLIO").First().Value;



            if (longName != null && longName != "")
            {
                #region ServiceAvailabilityChecker

                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                    throw new Exception();

                #endregion

                List<GF_PRICING_BASEVIEW> dimensionSecurityPrice = entity.GF_PRICING_BASEVIEW.
                    Where(a => (a.ISSUE_NAME == longName) && (a.FROMDATE >= objStartDate.Date)).OrderByDescending(a => a.FROMDATE).ToList();
                result = ChartExtensionCalculations.CalculateSecurityPricing(dimensionSecurityPrice);
            }

            if (portfolioID != null && portfolioID != "")
            {

                #region ServiceAvailabilityChecker

                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                    throw new Exception();

                #endregion

                List<GF_TRANSACTIONS> dimensionTransactionData = entity.GF_TRANSACTIONS.
                    Where(a => ((a.TRANSACTION_CODE.ToUpper() == "BUY") || (a.TRANSACTION_CODE.ToUpper() == "SELL")) && (a.PORTFOLIO_ID == portfolioID)
                        && (a.SEC_NAME == longName) && (a.TRADE_DATE >= Convert.ToDateTime(objStartDate.Date))).ToList();
                result = ChartExtensionCalculations.CalculateTransactionValues(dimensionTransactionData, result);
            }

            return result;
        }

        #endregion

        #region Market Performance Snapshot Operation Contracts

        /// <summary>
        /// retrieving list of market performance snapshots for particular user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>returns list of market performance snapshots</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MarketSnapshotSelectionData> RetrieveMarketSnapshotSelectionData(string userName)
        {
            try
            {
                if (userName == null)
                    return null;

                ResearchEntities entity = new ResearchEntities();
                List<MarketSnapshotSelectionData> userPreference = (entity.GetMarketSnapshotSelectionData(userName))
                    .OrderBy(record => record.SnapshotName)
                    .ToList<MarketSnapshotSelectionData>();

                return userPreference;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// retrieving user preference for market performance snapshot gadget
        /// </summary>
        /// <param name="userName">user name/id</param>
        /// <param name="snapshotName">snapshot name</param>
        /// <returns>list of user preference of entities in market performance snapshot</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MarketSnapshotPreference> RetrieveMarketSnapshotPreference(int snapshotPreferenceId)
        {
            try
            {
                ResearchEntities entity = new ResearchEntities();
                List<MarketSnapshotPreference> userPreference = (entity.GetMarketSnapshotPreference(snapshotPreferenceId)).ToList<MarketSnapshotPreference>();
                return userPreference.OrderBy(record => record.GroupPreferenceID).ThenBy(record => record.EntityOrder).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        private Decimal? GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(string instrumentId, string returnType, DateTime recordDate, out DateTime returnDate)
        {
            DimensionEntitiesService.Entities entity = DimensionEntity;
            GF_PRICING_BASEVIEW pricingRecord = null;
            int iteration = 0;
            while (pricingRecord == null)
            {
                iteration++;
                if (iteration > 4)
                    break;
                recordDate = recordDate.AddDays(-1);
                pricingRecord = entity.GF_PRICING_BASEVIEW
                            .Where(record => record.INSTRUMENT_ID == instrumentId
                                && record.FROMDATE == Convert.ToDateTime(recordDate.ToString())).FirstOrDefault();
            }
            returnDate = recordDate;

            Decimal? pricingRecordValue = null;
            if (pricingRecord != null)
            {
                switch (returnType)
                {
                    case "Price":
                        pricingRecordValue = pricingRecord.DAILY_PRICE_RETURN;
                        break;
                    case "Total":
                        pricingRecordValue = pricingRecord.DAILY_GROSS_RETURN;
                        break;
                    default:
                        pricingRecordValue = pricingRecord.DAILY_CLOSING_PRICE;
                        break;
                }
            }

            return pricingRecordValue;
        }

        /// <summary>
        /// retrieving entity data for market performance snapshot gadget based on user preference
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <returns>list of entity data for market performance snapshot</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MarketPerformanceSnapshotData> RetrieveMarketPerformanceSnapshotData(List<MarketSnapshotPreference> marketSnapshotPreference)
        {
            try
            {
                List<MarketPerformanceSnapshotData> result = new List<MarketPerformanceSnapshotData>();
                DimensionEntitiesService.Entities entity = DimensionEntity;

                foreach (MarketSnapshotPreference preference in marketSnapshotPreference)
                {
                    string entityInstrumentId = entity.GF_SELECTION_BASEVIEW
                        .Where(record => record.LONG_NAME == preference.EntityName
                            && record.TYPE == preference.EntityType)
                        .FirstOrDefault()
                        .INSTRUMENT_ID;

                    #region Today's Pricing Data
                    DateTime presentBusinessDate = DateTime.Today;
                    Decimal? presentBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, presentBusinessDate, out presentBusinessDate);
                    #endregion

                    #region Last Date Pricing Data
                    DateTime lastBusinessDate = presentBusinessDate;
                    Decimal? lastBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastBusinessDate, out lastBusinessDate);
                    #endregion

                    #region Second Last Date Pricing Data
                    DateTime secondLastBusinessDate = lastBusinessDate;
                    Decimal? secondLastBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, secondLastBusinessDate, out secondLastBusinessDate);
                    #endregion

                    #region Last Week Date Pricing Data
                    DateTime lastWeekBusinessDate = DateTime.Today.AddDays(-7);
                    Decimal? lastWeekBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastWeekBusinessDate, out lastWeekBusinessDate);
                    #endregion

                    #region Last Month Date Pricing Data
                    DateTime lastMonthBusinessDate = DateTime.Today.AddMonths(-1);
                    Decimal? lastMonthBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastMonthBusinessDate, out lastMonthBusinessDate);
                    #endregion

                    #region Last Quarter Date Pricing Data
                    DateTime lastQuarterBusinessDate = DateTime.Today.AddMonths(-3);
                    Decimal? lastQuarterBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastQuarterBusinessDate, out lastQuarterBusinessDate);
                    #endregion

                    #region Last Year Date Pricing Data
                    DateTime lastYearBusinessDate = new DateTime(DateTime.Today.Year - 1, 12, 31);
                    Decimal? lastYearBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastYearBusinessDate, out lastYearBusinessDate);
                    #endregion

                    #region Second Last Year Date Pricing Data
                    DateTime secondLastYearBusinessDate = new DateTime(DateTime.Today.Year - 2, 12, 31);
                    Decimal? secondLastYearBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, secondLastYearBusinessDate, out secondLastYearBusinessDate);
                    #endregion

                    #region Third Last Year Date Pricing Data
                    DateTime thirdLastYearBusinessDate = new DateTime(DateTime.Today.Year - 3, 12, 31);
                    Decimal? thirdLastYearBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, thirdLastYearBusinessDate, out thirdLastYearBusinessDate);
                    #endregion

                    #region Fourth Last Year Date Pricing Data
                    DateTime fourthLastYearBusinessDate = new DateTime(DateTime.Today.Year - 4, 12, 31);
                    Decimal? fourthLastYearBusinessDatePrice = GetPerformanceDataDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, fourthLastYearBusinessDate, out fourthLastYearBusinessDate);
                    #endregion

                    result.Add(new MarketPerformanceSnapshotData()
                    {
                        MarketSnapshotPreferenceInfo = preference,
                        DateToDateReturn = ((lastBusinessDatePrice - secondLastBusinessDatePrice) / (secondLastBusinessDatePrice == 0 ? null : secondLastBusinessDatePrice)) * 100,
                        WeekToDateReturn = ((presentBusinessDatePrice - lastWeekBusinessDatePrice) / (lastWeekBusinessDatePrice == 0 ? null : lastWeekBusinessDatePrice)) * 100,
                        MonthToDateReturn = ((presentBusinessDatePrice - lastMonthBusinessDatePrice) / (lastMonthBusinessDatePrice == 0 ? null : lastMonthBusinessDatePrice)) * 100,
                        QuarterToDateReturn = ((presentBusinessDatePrice - lastQuarterBusinessDatePrice) / (lastQuarterBusinessDatePrice == 0 ? null : lastQuarterBusinessDatePrice)) * 100,
                        YearToDateReturn = ((presentBusinessDatePrice - lastYearBusinessDatePrice) / (lastYearBusinessDatePrice == 0 ? null : lastYearBusinessDatePrice)) * 100,
                        LastYearReturn = ((lastYearBusinessDatePrice - secondLastYearBusinessDatePrice) / (secondLastYearBusinessDatePrice == 0 ? null : secondLastYearBusinessDatePrice)) * 100,
                        SecondLastYearReturn = ((secondLastYearBusinessDatePrice - thirdLastYearBusinessDatePrice) / (thirdLastYearBusinessDatePrice == 0 ? null : thirdLastYearBusinessDatePrice)) * 100,
                        ThirdLastYearReturn = ((thirdLastYearBusinessDatePrice - fourthLastYearBusinessDatePrice) / (fourthLastYearBusinessDatePrice == 0 ? null : fourthLastYearBusinessDatePrice)) * 100
                    });

                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// adding new market performance snapshot created by user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="snapshotName"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool AddMarketSnapshotPerformance(string userId, string snapshotName)
        {
            try
            {
                ResearchEntities entity = new ResearchEntities();
                entity.SetMarketSnapshotPreference(userId, snapshotName);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// updating the market performance snapshot name for a particular user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="snapshotName"></param>
        /// <param name="snapshotPreferenceId"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool UpdateMarketSnapshotPerformance(string userId, string snapshotName, int snapshotPreferenceId)
        {
            try
            {
                ResearchEntities entity = new ResearchEntities();
                entity.UpdateMarketSnapshotPreference(userId, snapshotName, snapshotPreferenceId);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// adding user preferred groups in market performance snapshot gadget
        /// </summary>
        /// <param name="snapshotPreferenceId"></param>
        /// <param name="groupName"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool AddMarketSnapshotGroupPreference(int snapshotPreferenceId, string groupName)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.SetMarketSnapshotGroupPreference(snapshotPreferenceId, groupName);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// removing user preferred groups from market performance snapshot gadget
        /// </summary>
        /// <param name="grouppreferenceId"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool RemoveMarketSnapshotGroupPreference(int groupPreferenceId)
        {
            try
            {
                ResearchEntities entity = new ResearchEntities();
                entity.DeleteMarketSnapshotGroupPreference(groupPreferenceId);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// adding user preferred entities in groups in market performance snapshot gadget
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool AddMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.SetMarketSnapshotEntityPreference(marketSnapshotPreference.GroupPreferenceID,
                                                            marketSnapshotPreference.EntityName,
                                                                marketSnapshotPreference.EntityReturnType,
                                                                    marketSnapshotPreference.EntityType,
                                                                        marketSnapshotPreference.EntityOrder);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        ///  removing user preferred entities from groups in market performance snapshot gadget
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool RemoveMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                int? affectedRows = entity.DeleteMarketSnapshotEntityPreference(marketSnapshotPreference.EntityPreferenceId).FirstOrDefault();
                if (affectedRows == null || affectedRows == 0)
                    return false;
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        ///  save user preference in market performance snapshot gadget
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MarketSnapshotPreference> SaveMarketSnapshotPreference(int snapshotPreferenceId, string updateXML)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                int? result = entity.UpdateMarketPerformanceSnapshot(snapshotPreferenceId, updateXML).FirstOrDefault();

                switch (result)
                {
                    case 0:
                        break;
                    case 1:
                        throw new NotImplementedException("An error occured while creating groups within the specified snapshot");
                    case 2:
                        throw new NotImplementedException("An error occured while creating entities for inserted groups within the specified snapshot");
                    case 3:
                        throw new NotImplementedException("An error occured while deleting groups within the specified snapshot");
                    case 4:
                        throw new NotImplementedException("An error occured while creating entities within the specified snapshot");
                    case 5:
                        throw new NotImplementedException("An error occured while deleting entities within the specified snapshot");
                    case 6:
                        throw new NotImplementedException("An error occured while updating entities within the specified snapshot");
                    default:
                        break;
                }

                List<MarketSnapshotPreference> userPreference = (entity.GetMarketSnapshotPreference(snapshotPreferenceId))
                    .ToList<MarketSnapshotPreference>();

                return userPreference;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        ///  save new user preference in market performance snapshot gadget
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public PopulatedMarketPerformanceSnapshotData SaveAsMarketSnapshotPreference(string userName, string snapshotName, string updateXML)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                PopulatedMarketPerformanceSnapshotData result = new PopulatedMarketPerformanceSnapshotData();

                Decimal? snapshotID = entity.SetMarketSnapshotPreference(userName, snapshotName).FirstOrDefault();
                if (snapshotID == null)
                    return null;

                int? status = entity.UpdateMarketPerformanceSnapshot(Convert.ToInt32(snapshotID), updateXML).FirstOrDefault();

                switch (status)
                {
                    case 0:
                        break;
                    case 1:
                        throw new NotImplementedException("An error occured while creating groups within the specified snapshot");
                    case 2:
                        throw new NotImplementedException("An error occured while creating entities for inserted groups within the specified snapshot");
                    case 3:
                        throw new NotImplementedException("An error occured while deleting groups within the specified snapshot");
                    case 4:
                        throw new NotImplementedException("An error occured while creating entities within the specified snapshot");
                    case 5:
                        throw new NotImplementedException("An error occured while deleting entities within the specified snapshot");
                    case 6:
                        throw new NotImplementedException("An error occured while updating entities within the specified snapshot");
                    default:
                        break;
                }

                //snapshotPreference = snapshotPreference
                //    .OrderBy(record => record.GroupPreferenceID)
                //    .ThenBy(record => record.EntityOrder)
                //    .ToList();

                //string insertedGroupName = String.Empty;
                //Decimal? groupPreferenceId = 0;



                //foreach (MarketSnapshotPreference preference in snapshotPreference)
                //{
                //    if (preference.GroupName != insertedGroupName)
                //    {
                //        groupPreferenceId = entity.SetMarketSnapshotGroupPreference(Convert.ToInt32(snapshotID), preference.GroupName).FirstOrDefault();
                //        insertedGroupName = preference.GroupName;
                //    }

                //    entity.SetMarketSnapshotEntityPreference(Convert.ToInt32(groupPreferenceId), preference.EntityName, preference.EntityReturnType,
                //        preference.EntityType, preference.EntityOrder);
                //}



                MarketSnapshotSelectionData marketSnapshotSelectionData = RetrieveMarketSnapshotSelectionData(userName)
                    .Where(record => record.SnapshotName == snapshotName).FirstOrDefault();

                List<MarketSnapshotPreference> marketSnapshotPreference = RetrieveMarketSnapshotPreference(Convert.ToInt32(snapshotID));
                List<MarketPerformanceSnapshotData> marketPerformanceSnapshotData = RetrieveMarketPerformanceSnapshotData(marketSnapshotPreference);

                result.MarketSnapshotSelectionInfo = marketSnapshotSelectionData;
                result.MarketPerformanceSnapshotInfo = marketPerformanceSnapshotData;
                return result;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }


        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool RemoveMarketSnapshotPreference(string userName, string snapshotName)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.DeleteMarketSnapshotPreference(userName, snapshotName);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion

        #region Relative Performance Gadgets
        /// <summary>
        /// Retrieves list of sector information for a particular composite/fund and effective date.
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>List of RelativePerformanceSectorData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceSectorData> RetrieveRelativePerformanceSectorData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<RelativePerformanceSectorData> result = new List<RelativePerformanceSectorData>();

                List<GF_PERF_DAILY_ATTRIBUTION> data = entity.GF_PERF_DAILY_ATTRIBUTION.Where(t =>
                                                                                        t.PORTFOLIO == portfolioSelectionData.PortfolioId &&
                                                                                        t.TO_DATE == Convert.ToDateTime(effectiveDate) &&
                                                                                        t.NODE_NAME == "Security ID" &&
                                                                                        t.POR_RC_MARKET_VALUE != 0 &&
                                                                                        t.COUNTRY != null &&
                                                                                        t.GICS_LVL1 != null).ToList();
                foreach (GF_PERF_DAILY_ATTRIBUTION record in data)
                {
                    result.Add(new RelativePerformanceSectorData()
                    {
                        SectorId = record.GICS_LVL1,
                        SectorName = record.GICS_LVL1
                    });
                }
                result = result.Distinct().ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves relative performance data for a particular composite/fund, effective date and period.
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="period">Period</param>
        /// <returns>List of RelativePerformanceData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceData> RetrieveRelativePerformanceData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null || period == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<GF_PERF_DAILY_ATTRIBUTION> dailyData = RetrieveRelativePerformanceDailyData(portfolioSelectionData, effectiveDate, null, null);

                if (dailyData == null)
                    return null;

                List<string> countryCodes = new List<string>();
                foreach (GF_PERF_DAILY_ATTRIBUTION record in dailyData)
                {
                    countryCodes.Add(record.COUNTRY);
                }
                countryCodes = countryCodes.Distinct().ToList();

                List<RelativePerformanceSectorData> sectors = new List<RelativePerformanceSectorData>();
                foreach (GF_PERF_DAILY_ATTRIBUTION record in dailyData)
                {
                    sectors.Add(new RelativePerformanceSectorData()
                    {
                        SectorId = record.GICS_LVL1,
                        SectorName = record.GICS_LVL1
                    });
                }
                sectors = sectors.Distinct().ToList();


                List<RelativePerformanceData> result = new List<RelativePerformanceData>();
                foreach (string countryCode in countryCodes)
                {
                    decimal? aggcsAlpha = 0;
                    decimal? aggcsPortfolioShare = 0;
                    decimal? aggcsBenchmarkShare = 0;
                    List<RelativePerformanceCountrySpecificData> sectorSpecificData = new List<RelativePerformanceCountrySpecificData>();
                    foreach (RelativePerformanceSectorData sectorData in sectors)
                    {
                        decimal? aggAlpha = 0;
                        decimal? aggPortfolioShare = 0;
                        decimal? aggBenchmarkShare = 0;
                        List<GF_PERF_DAILY_ATTRIBUTION> specificData = dailyData.Where(t => t.COUNTRY == countryCode && t.GICS_LVL1 == sectorData.SectorName).ToList();

                        foreach (GF_PERF_DAILY_ATTRIBUTION row in specificData)
                        {
                            aggPortfolioShare = (aggPortfolioShare + RetrieveRelativePerformancePortfolioWeight(row, period));
                            aggBenchmarkShare = (aggBenchmarkShare + RetrieveRelativePerformanceBenchmarkWeight(row, period));
                            aggAlpha = RetrieveRelativePerformanceAlphaValue(row, period);

                        }

                        sectorSpecificData.Add(new RelativePerformanceCountrySpecificData()
                        {
                            SectorId = sectorData.SectorId,
                            SectorName = sectorData.SectorName,
                            Alpha = aggAlpha,
                            PortfolioShare = aggPortfolioShare,
                            BenchmarkShare = aggBenchmarkShare,
                            ActivePosition = Convert.ToDecimal(aggPortfolioShare - aggBenchmarkShare),
                        });

                        aggcsAlpha = aggcsAlpha + aggAlpha;
                        aggcsPortfolioShare = aggcsPortfolioShare + aggPortfolioShare;
                        aggcsBenchmarkShare = aggcsBenchmarkShare + aggBenchmarkShare;
                    }

                    if (sectorSpecificData.Count > 0)
                    {
                        result.Add(new RelativePerformanceData()
                        {
                            CountryId = countryCode,
                            RelativePerformanceCountrySpecificInfo = sectorSpecificData,
                            AggregateCountryAlpha = aggcsAlpha,
                            AggregateCountryPortfolioShare = aggcsPortfolioShare,
                            AggregateCountryBenchmarkShare = aggcsBenchmarkShare,
                            AggregateCountryActivePosition = aggcsPortfolioShare - aggcsBenchmarkShare,
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        ///  Retrieves Country Level Active Position Data for a particular composite/fund, effective date and period.
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="period">Period</param>
        /// <param name="countryID">(optional) COUNTRY; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, string countryID = null, string sectorID = null)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null || period == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<GF_PERF_DAILY_ATTRIBUTION> data = RetrieveRelativePerformanceDailyData(portfolioSelectionData, effectiveDate, countryID, sectorID);

                List<string> countryCodes = new List<string>();
                foreach (GF_PERF_DAILY_ATTRIBUTION row in data)
                {
                    countryCodes.Add(row.COUNTRY);
                }
                countryCodes = countryCodes.Distinct().ToList();

                List<RelativePerformanceActivePositionData> result = new List<RelativePerformanceActivePositionData>();

                foreach (string countryCode in countryCodes)
                {
                    if (countryID != null)
                    {
                        if (!countryCode.Equals(countryID.ToString()))
                        {
                            continue;
                        }
                    }

                    RelativePerformanceActivePositionData record = new RelativePerformanceActivePositionData();
                    decimal? MarketValue = 0;
                    decimal? FundWeight = 0;
                    decimal? BenchmarkWeight = 0;

                    record.Entity = countryCode.ToString();
                    List<GF_PERF_DAILY_ATTRIBUTION> countrySpecificData = data.Where(row => row.COUNTRY == countryCode).ToList();

                    foreach (GF_PERF_DAILY_ATTRIBUTION row in countrySpecificData)
                    {
                        MarketValue = MarketValue + ((row.POR_RC_MARKET_VALUE) == null ? 0 : row.POR_RC_MARKET_VALUE);
                        FundWeight = FundWeight + (RetrieveRelativePerformancePortfolioWeight(row, period));
                        BenchmarkWeight = BenchmarkWeight + (RetrieveRelativePerformanceBenchmarkWeight(row, period));
                    }

                    record.MarketValue = MarketValue;
                    record.FundWeight = FundWeight;
                    record.BenchmarkWeight = BenchmarkWeight;
                    record.ActivePosition = Convert.ToDecimal(FundWeight - BenchmarkWeight);

                    result.Add(record);
                }

                return result.OrderByDescending(t => t.ActivePosition).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        ///  Retrieves Sector Level Active Position Data for a particular composite/fund, effective date and period.
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="period">Period</param>
        /// <param name="countryID">(optional) COUNTRY; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, string countryID = null, string sectorID = null)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null || period == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<GF_PERF_DAILY_ATTRIBUTION> data = RetrieveRelativePerformanceDailyData(portfolioSelectionData, effectiveDate, countryID, sectorID);

                List<RelativePerformanceSectorData> sectorCodes = new List<RelativePerformanceSectorData>();
                foreach (GF_PERF_DAILY_ATTRIBUTION row in data)
                {
                    sectorCodes.Add(new RelativePerformanceSectorData()
                    {
                        SectorId = row.GICS_LVL1,
                        SectorName = row.GICS_LVL1
                    });
                }
                sectorCodes = sectorCodes.Distinct().ToList();

                List<RelativePerformanceActivePositionData> result = new List<RelativePerformanceActivePositionData>();

                foreach (RelativePerformanceSectorData sector in sectorCodes)
                {
                    if (sectorID != null)
                    {
                        if (!sector.SectorId.Equals(sectorID))
                        {
                            continue;
                        }
                    }

                    RelativePerformanceActivePositionData record = new RelativePerformanceActivePositionData();
                    decimal? MarketValue = 0;
                    decimal? FundWeight = 0;
                    decimal? BenchmarkWeight = 0;

                    record.Entity = sector.SectorName.ToString();

                    List<GF_PERF_DAILY_ATTRIBUTION> sectorSpecificData = data.Where(row => row.GICS_LVL1 == sector.SectorId).ToList();

                    foreach (GF_PERF_DAILY_ATTRIBUTION row in sectorSpecificData)
                    {
                        MarketValue = MarketValue + ((row.POR_RC_MARKET_VALUE) == null ? 0 : row.POR_RC_MARKET_VALUE);
                        FundWeight = FundWeight + (RetrieveRelativePerformancePortfolioWeight(row, period));
                        BenchmarkWeight = BenchmarkWeight + (RetrieveRelativePerformanceBenchmarkWeight(row, period));
                    }

                    record.MarketValue = MarketValue;
                    record.FundWeight = FundWeight;
                    record.BenchmarkWeight = BenchmarkWeight;
                    record.ActivePosition = Convert.ToDecimal(FundWeight - BenchmarkWeight);

                    result.Add(record);
                }

                return result.OrderByDescending(t => t.ActivePosition).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        ///  Retrieves Security Level Active Position Data for a particular composite/fund, effective date and period.
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="period">Period</param>
        /// <param name="countryID">(optional) COUNTRY; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, string countryID = null, string sectorID = null)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null || period == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<GF_PERF_DAILY_ATTRIBUTION> data = RetrieveRelativePerformanceDailyData(portfolioSelectionData, effectiveDate, countryID, sectorID);

                List<RelativePerformanceActivePositionData> result = new List<RelativePerformanceActivePositionData>();

                foreach (GF_PERF_DAILY_ATTRIBUTION row in data)
                {
                    decimal? fundWeight = (RetrieveRelativePerformancePortfolioWeight(row, period));
                    decimal? benchmarkWeight = (RetrieveRelativePerformanceBenchmarkWeight(row, period));
                    decimal? activePosition = Convert.ToDecimal(fundWeight - benchmarkWeight);

                    result.Add(new RelativePerformanceActivePositionData()
                    {
                        Entity = row.SEC_NAME,
                        MarketValue = row.POR_RC_MARKET_VALUE,
                        FundWeight = fundWeight,
                        BenchmarkWeight = benchmarkWeight,
                        ActivePosition = activePosition
                    });
                }

                return result.OrderByDescending(t => t.ActivePosition).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves Security Level Relative Performance Data for a particular composite/fund, benchmark and efective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <param name="order">(optional)1 for Ascending - data ordering - By default descending</param>
        /// <param name="maxRecords">(optional) Maximum number of records to be retrieved - By default Null</param>
        /// <returns>List of RetrieveRelativePerformanceSecurityData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceSecurityData> RetrieveRelativePerformanceSecurityData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string period, string countryID = null, string sectorID = null)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null || period == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<GF_PERF_DAILY_ATTRIBUTION> data = RetrieveRelativePerformanceDailyData(portfolioSelectionData, effectiveDate, countryID, sectorID);

                List<RelativePerformanceSecurityData> result = new List<RelativePerformanceSecurityData>();
                foreach (GF_PERF_DAILY_ATTRIBUTION row in data)
                {
                    result.Add(new RelativePerformanceSecurityData()
                    {
                        SecurityName = row.SEC_NAME,
                        SecurityCountryId = row.COUNTRY,
                        SecuritySectorName = row.GICS_LVL1,
                        SecurityMarketValue = row.POR_RC_MARKET_VALUE,
                        SecurityAlpha = RetrieveRelativePerformanceAlphaValue(row, period)
                    });
                }
                return result.OrderByDescending(e => e.SecurityAlpha).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region Relative Performance Helper Methods
        /// <summary>
        /// retrieving data from GF_PERF_DAILY_ATTRIBUTION view for relative performance gadgets
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="country"></param>
        /// <param name="sector"></param>
        /// <returns>GF_PERF_DAILY_ATTRIBUTION Collection</returns>
        private List<GF_PERF_DAILY_ATTRIBUTION> RetrieveRelativePerformanceDailyData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, string country = null, string sector = null)
        {
            DimensionEntitiesService.Entities entity = DimensionEntity;
            List<GF_PERF_DAILY_ATTRIBUTION> dailyData = new List<GF_PERF_DAILY_ATTRIBUTION>();
            if (country == null && sector == null)
            {
                dailyData = entity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId &&
                                                                   t.TO_DATE == Convert.ToDateTime(effectiveDate) &&
                                                                   t.NODE_NAME == "Security ID" &&
                                                                   t.POR_RC_MARKET_VALUE != 0 &&
                                                                   t.COUNTRY != null &&
                                                                   t.GICS_LVL1 != null).ToList();
            }

            else if (country == null && sector != null)
            {
                dailyData = entity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId &&
                                                                    t.TO_DATE == Convert.ToDateTime(effectiveDate) &&
                                                                    t.NODE_NAME == "Security ID" &&
                                                                    t.POR_RC_MARKET_VALUE != 0 &&
                                                                    t.COUNTRY != null &&
                                                                    t.GICS_LVL1 == sector).ToList();
            }

            else if (sector == null && country != null)
            {
                dailyData = entity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId &&
                                                                   t.TO_DATE == Convert.ToDateTime(effectiveDate) &&
                                                                   t.NODE_NAME == "Security ID" &&
                                                                   t.POR_RC_MARKET_VALUE != 0 &&
                                                                   t.COUNTRY == country &&
                                                                   t.GICS_LVL1 != null).ToList();
            }

            else if (sector != null && country != null)
            {
                dailyData = entity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId &&
                                                                   t.TO_DATE == Convert.ToDateTime(effectiveDate) &&
                                                                  t.NODE_NAME == "Security ID" &&
                                                                   t.POR_RC_MARKET_VALUE != 0 &&
                                                                   t.COUNTRY == country &&
                                                                   t.GICS_LVL1 == sector).ToList();
            }
            return dailyData;
        }

        /// <summary>
        /// retrieving alpha values based on period selected for relative performance gadget
        /// </summary>
        /// <param name="row"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private decimal? RetrieveRelativePerformanceAlphaValue(GF_PERF_DAILY_ATTRIBUTION row, string period)
        {
            decimal? alpha = 0;
            switch (period)
            {
                case "1D":
                    alpha = row.F_POR_ASH_RC_CTN_1D - row.F_BM1_ASH_RC_CTN_1D;
                    break;

                case "1W":
                    alpha = row.F_POR_ASH_RC_CTN_1W - row.F_BM1_ASH_RC_CTN_1W;
                    break;

                case "MTD":
                    alpha = row.F_POR_ASH_RC_CTN_MTD - row.F_BM1_ASH_RC_CTN_MTD;
                    break;

                case "YTD":
                    alpha = row.F_POR_ASH_RC_CTN_YTD - row.F_BM1_ASH_RC_CTN_YTD;
                    break;

                case "QTD":
                    alpha = row.F_POR_ASH_RC_CTN_QTD - row.F_BM1_ASH_RC_CTN_QTD;
                    break;

                case "1Y":
                    alpha = row.F_POR_ASH_RC_CTN_1Y - row.F_BM1_ASH_RC_CTN_1Y;
                    break;

                default:
                    break;
            }
            return alpha;
        }

        /// <summary>
        /// retrieving benchmark weights based on period selected for relative performance gadget
        /// </summary>
        /// <param name="row"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private decimal? RetrieveRelativePerformanceBenchmarkWeight(GF_PERF_DAILY_ATTRIBUTION row, string period)
        {
            decimal? benchmarkWeight = 0;
            switch (period)
            {
                case "1D":
                    benchmarkWeight = row.BM1_RC_AVG_WGT_1D;
                    break;

                case "1W":
                    benchmarkWeight = row.BM1_RC_AVG_WGT_1W;
                    break;

                case "MTD":
                    benchmarkWeight = row.BM1_RC_AVG_WGT_MTD;
                    break;

                case "YTD":
                    benchmarkWeight = row.BM1_RC_AVG_WGT_YTD;
                    break;

                case "QTD":
                    benchmarkWeight = row.BM1_RC_AVG_WGT_QTD;
                    break;

                case "1Y":
                    benchmarkWeight = row.BM1_RC_AVG_WGT_1Y;
                    break;

                default:
                    break;
            }
            return benchmarkWeight;
        }

        /// <summary>
        /// retrieving portfolio weights based on period selected for relative performance gadget
        /// </summary>
        /// <param name="row"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private decimal? RetrieveRelativePerformancePortfolioWeight(GF_PERF_DAILY_ATTRIBUTION row, string period)
        {
            decimal? portfolioWeight = 0;
            switch (period)
            {
                case "1D":
                    portfolioWeight = row.POR_RC_AVG_WGT_1D;
                    break;

                case "1W":
                    portfolioWeight = row.POR_RC_AVG_WGT_1W;
                    break;

                case "MTD":
                    portfolioWeight = row.POR_RC_AVG_WGT_MTD;
                    break;

                case "YTD":
                    portfolioWeight = row.POR_RC_AVG_WGT_YTD;
                    break;

                case "QTD":
                    portfolioWeight = row.POR_RC_AVG_WGT_QTD;
                    break;

                case "1Y":
                    portfolioWeight = row.POR_RC_AVG_WGT_1Y;
                    break;

                default:
                    break;
            }
            return portfolioWeight;
        }

        #endregion

        #endregion

        #region Market Capitalization Methods
        /// <summary>
        /// Retrieves consolidated data for portfolio and benchmark
        /// </summary>
        /// <param name="selPortfolioID">Contains Selected Portfolio ID</param>
        /// <param name="selPortfolioDate">Effective Date selected by user</param>
        /// <returns>Consolidated list of portfolio and benchmark</returns>        
        private List<MarketCapitalizationData> RetrievePortfolioMktCapData(PortfolioSelectionData portfolio_ID, DateTime effectiveDate, String filterType, String filterValue, bool isExCashSecurity)
        {

            List<MarketCapitalizationData> result = new List<MarketCapitalizationData>();
            //List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> filteredResult = new List<GF_PORTFOLIO_HOLDINGS>();
            DimensionEntitiesService.Entities entity = DimensionEntity;
            List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> dimensionServicePortfolioData = null;

            if (entity.GF_PORTFOLIO_HOLDINGS == null && entity.GF_BENCHMARK_HOLDINGS.Count() == 0)
                return null;

            if (isExCashSecurity)
            {
                dimensionServicePortfolioData = entity.GF_PORTFOLIO_HOLDINGS
                        .Where(portfolioList => (portfolioList.PORTFOLIO_ID == portfolio_ID.PortfolioId)
                            && (portfolioList.PORTFOLIO_DATE == effectiveDate)
                            && (portfolioList.SECURITYTHEMECODE.ToUpper() != GreenfieldConstants.CASH)).ToList();
            }
            else
            {
                dimensionServicePortfolioData = entity.GF_PORTFOLIO_HOLDINGS
                        .Where(portfolioList => (portfolioList.PORTFOLIO_ID == portfolio_ID.PortfolioId)
                            && (portfolioList.PORTFOLIO_DATE == effectiveDate)).ToList();
            }

            if (dimensionServicePortfolioData == null || dimensionServicePortfolioData.Count == 0)
                return result;


            //Applying filters
            if (filterType != null)//&& filterValue != null)
            {
                switch (filterType)
                {
                    case GreenfieldConstants.REGION:
                        dimensionServicePortfolioData = dimensionServicePortfolioData.Where(list => (list.ASHEMM_PROP_REGION_CODE == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.COUNTRY:
                        dimensionServicePortfolioData = dimensionServicePortfolioData.Where(list => (list.ISO_COUNTRY_CODE == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.INDUSTRY:
                        dimensionServicePortfolioData = dimensionServicePortfolioData.Where(list => (list.GICS_INDUSTRY_NAME == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.SECTOR:
                        dimensionServicePortfolioData = dimensionServicePortfolioData.Where(list => (list.GICS_SECTOR_NAME == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.SHOW_EVERYTHING:
                        dimensionServicePortfolioData = entity.GF_PORTFOLIO_HOLDINGS
                        .Where(portfolioList => (portfolioList.PORTFOLIO_ID == portfolio_ID.PortfolioId)
                            && (portfolioList.PORTFOLIO_DATE == effectiveDate)).ToList();

                        break;
                    //default:
                    //    break;
                }
            }

            for (int _index = 0; _index < dimensionServicePortfolioData.Count; _index++)
            {
                MarketCapitalizationData mktCapData = new MarketCapitalizationData();
                mktCapData.PortfolioDirtyValuePC = dimensionServicePortfolioData[_index].DIRTY_VALUE_PC;
                mktCapData.MarketCapitalInUSD = dimensionServicePortfolioData[_index].MARKET_CAP_IN_USD;
                mktCapData.SecurityThemeCode = dimensionServicePortfolioData[_index].SECURITYTHEMECODE;
                mktCapData.Benchmark_ID = dimensionServicePortfolioData[_index].BENCHMARK_ID;
                mktCapData.Portfolio_ID = dimensionServicePortfolioData[_index].PORTFOLIO_ID;
                mktCapData.AsecSecShortName = dimensionServicePortfolioData[_index].ASEC_SEC_SHORT_NAME;
                mktCapData.BenchmarkWeight = 0;

                result.Add(mktCapData);
            }
            List<MarketCapitalizationData> _portfolioBenchmarkData = RetrieveBenchmarkMktCapData(result, effectiveDate, filterType, filterValue, isExCashSecurity);
            return _portfolioBenchmarkData;

        }

        /// <summary>
        /// Retrieves bechmark data
        /// </summary>
        /// <param name="portfolioData"></param>
        /// <param name="effectiveDate"></param>
        /// <returns>bechmark data</returns>
        private List<MarketCapitalizationData> RetrieveBenchmarkMktCapData(List<MarketCapitalizationData> portfolioData, DateTime effectiveDate, String filterType, String filterValue, bool isExCashSecurity)
        {

            //List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> filteredResult = new List<GF_BENCHMARK_HOLDINGS>();
            DimensionEntitiesService.Entities entity = DimensionEntity;
            List<GF_BENCHMARK_HOLDINGS> dimensionServiceBenchmarkData = null;

            //Retrieve the Id of benchmark associated with the Portfolio
            List<string> benchmarkId = portfolioData.Select(a => a.Benchmark_ID).Distinct().ToList();

            //If the DataBase doesn't return a single Benchmark for a Portfolio
            if (benchmarkId == null || benchmarkId.Count != 1)
                throw new InvalidOperationException();

            if (isExCashSecurity)
            {
                dimensionServiceBenchmarkData = entity.GF_BENCHMARK_HOLDINGS.
                    Where(benchMarklist => (benchMarklist.BENCHMARK_ID == benchmarkId.First())
                    && (benchMarklist.PORTFOLIO_DATE == effectiveDate.Date)
                    && (benchMarklist.SECURITYTHEMECODE.ToUpper() != GreenfieldConstants.CASH)).ToList();
            }
            else
            {
                dimensionServiceBenchmarkData = entity.GF_BENCHMARK_HOLDINGS.
                    Where(benchMarklist => (benchMarklist.BENCHMARK_ID == benchmarkId.First())
                    && (benchMarklist.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
            }

            if (dimensionServiceBenchmarkData.Count < 1)
                return null;

            //Applying filters
            if (filterType != null)//&& filterValue != null)
            {
                switch (filterType)
                {
                    case GreenfieldConstants.REGION:
                        dimensionServiceBenchmarkData = dimensionServiceBenchmarkData.Where(list => (list.ASHEMM_PROP_REGION_CODE == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.COUNTRY:
                        dimensionServiceBenchmarkData = dimensionServiceBenchmarkData.Where(list => (list.ISO_COUNTRY_CODE == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.INDUSTRY:
                        dimensionServiceBenchmarkData = dimensionServiceBenchmarkData.Where(list => (list.GICS_INDUSTRY_NAME == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.SECTOR:
                        dimensionServiceBenchmarkData = dimensionServiceBenchmarkData.Where(list => (list.GICS_SECTOR_NAME == filterValue)).ToList();

                        break;
                    case GreenfieldConstants.SHOW_EVERYTHING:
                        dimensionServiceBenchmarkData = entity.GF_BENCHMARK_HOLDINGS.
                            Where(benchMarklist => (benchMarklist.BENCHMARK_ID == benchmarkId.First())
                            && (benchMarklist.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                        break;

                }
            }
            //if (portfolioData.Capacity < (portfolioData.Count + dimensionServiceBenchmarkData.Count))
            //    portfolioData.Capacity = portfolioData.Count + dimensionServiceBenchmarkData.Count;
            //Add benchmark wieghts if ASEC_SEC_SHORT_NAME does not exist in portfolio list
            foreach (GF_BENCHMARK_HOLDINGS benchmarkData in dimensionServiceBenchmarkData)
            {
                var existingPortfolio = from p in portfolioData
                                        where p.AsecSecShortName.ToLower() == benchmarkData.ASEC_SEC_SHORT_NAME.ToLower()
                                        select p;

                if (existingPortfolio.Count() == 0)
                {
                    MarketCapitalizationData mktCapData = new MarketCapitalizationData();

                    mktCapData.MarketCapitalInUSD = benchmarkData.MARKET_CAP_IN_USD;
                    mktCapData.SecurityThemeCode = benchmarkData.SECURITYTHEMECODE;
                    mktCapData.BenchmarkWeight = benchmarkData.BENCHMARK_WEIGHT;
                    mktCapData.AsecSecShortName = benchmarkData.ASEC_SEC_SHORT_NAME;

                    portfolioData.Add(mktCapData);
                }
                //for (int i = portfolioData.Count - 1; i >= 0; i--)
                //{
                //    if (portfolioData[i].AsecSecShortName != benchmarkData.ASEC_SEC_SHORT_NAME)
                //    {
                //        MarketCapitalizationData mktCapData = new MarketCapitalizationData();

                //        mktCapData.MarketCapitalInUSD = benchmarkData.MARKET_CAP_IN_USD;
                //        mktCapData.SecurityThemeCode = benchmarkData.SECURITYTHEMECODE;
                //        mktCapData.BenchmarkWeight = benchmarkData.BENCHMARK_WEIGHT;
                //        mktCapData.AsecSecShortName = benchmarkData.ASEC_SEC_SHORT_NAME;

                //        portfolioData.Add(mktCapData);
                //        break;
                //    }
                //}

                //foreach (MarketCapitalizationData mktCapPortfolioData in portfolioData)
                //{
                //    if (mktCapPortfolioData.AsecSecShortName != benchmarkData.ASEC_SEC_SHORT_NAME)
                //    {
                //        MarketCapitalizationData mktCapData = new MarketCapitalizationData();

                //        mktCapData.MarketCapitalInUSD = benchmarkData.MARKET_CAP_IN_USD;
                //        mktCapData.SecurityThemeCode = benchmarkData.SECURITYTHEMECODE;
                //        mktCapData.BenchmarkWeight = benchmarkData.BENCHMARK_WEIGHT;
                //        mktCapData.AsecSecShortName = benchmarkData.ASEC_SEC_SHORT_NAME;

                //        portfolioData.Add(mktCapData);
                //    }
                //}                    
            }

            return portfolioData;

        }

        /// <summary>
        /// Retrieves portfolio and benchmark data for market capitalization grid
        /// </summary>
        /// <param name="portfolioSelectionData">Contains Selected Fund Data</param>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        /// <param name="filterType">The Filter type selected by the user</param>
        /// <param name="filterValue">The Filter value selected by the user</param>
        /// <returns>List of MarketCapitalizationData </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MarketCapitalizationData> RetrieveMarketCapitalizationData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool isExCashSecurity)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null)//|| filterType == null || filterValue == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString(GreenfieldConstants.SERVICE_NULL_ARG_EXC_MSG).ToString());

                List<MarketCapitalizationData> mktCap = new List<MarketCapitalizationData>();
                List<MarketCapitalizationData> result = new List<MarketCapitalizationData>();
                MarketCapitalizationData mktCapData = new MarketCapitalizationData();

                //Consolidated list Portfolio and benchmark Data
                mktCap = RetrievePortfolioMktCapData(portfolioSelectionData, effectiveDate, filterType, filterValue, isExCashSecurity);

                if (mktCap == null || mktCap.Count == 0)
                    return mktCap;

                //weighted avg for portfolio                 
                mktCapData.PortfolioWtdAvg = MarketCapitalizationCalculations.CalculatePortfolioWeightedAvg(mktCap);

                //weighted median for portfolio
                mktCapData.PortfolioWtdMedian = MarketCapitalizationCalculations.CalculatePortfolioWeightedMedian(mktCap);

                //ranges for portfolio
                List<MarketCapitalizationData> lstmktCapPortfolio = MarketCapitalizationCalculations.CalculateSumPortfolioRanges(mktCap);
                mktCapData.PortfolioSumMegaRange = lstmktCapPortfolio[0].PortfolioSumMegaRange;
                mktCapData.PortfolioSumLargeRange = lstmktCapPortfolio[0].PortfolioSumLargeRange;
                mktCapData.PortfolioSumMediumRange = lstmktCapPortfolio[0].PortfolioSumMediumRange;
                mktCapData.PortfolioSumSmallRange = lstmktCapPortfolio[0].PortfolioSumSmallRange;
                mktCapData.PortfolioSumMicroRange = lstmktCapPortfolio[0].PortfolioSumMicroRange;
                mktCapData.PortfolioSumUndefinedRange = lstmktCapPortfolio[0].PortfolioSumUndefinedRange;

                //Lower and upper limits range values are coming from portfolio list
                mktCapData.LargeRange = lstmktCapPortfolio[0].LargeRange;
                mktCapData.MediumRange = lstmktCapPortfolio[0].MediumRange;
                mktCapData.SmallRange = lstmktCapPortfolio[0].SmallRange;
                mktCapData.MicroRange = lstmktCapPortfolio[0].MicroRange;
                mktCapData.UndefinedRange = lstmktCapPortfolio[0].UndefinedRange;


                //weighted avg for benchmark
                mktCapData.BenchmarkWtdAvg = MarketCapitalizationCalculations.CalculateBenchmarkWeightedAvg(mktCap);

                //weighted median for benchmark
                mktCapData.BenchmarkWtdMedian = MarketCapitalizationCalculations.CalculateBenchmarkWeightedMedian(mktCap);

                //ranges for benchmark
                List<MarketCapitalizationData> lstmktCapBenchmark = MarketCapitalizationCalculations.CalculateSumBenchmarkRanges(mktCap);
                mktCapData.BenchmarkSumMegaRange = lstmktCapBenchmark[0].BenchmarkSumMegaRange;
                mktCapData.BenchmarkSumLargeRange = lstmktCapBenchmark[0].BenchmarkSumLargeRange;
                mktCapData.BenchmarkSumMediumRange = lstmktCapBenchmark[0].BenchmarkSumMediumRange;
                mktCapData.BenchmarkSumSmallRange = lstmktCapBenchmark[0].BenchmarkSumSmallRange;
                mktCapData.BenchmarkSumMicroRange = lstmktCapBenchmark[0].BenchmarkSumMicroRange;
                mktCapData.BenchmarkSumUndefinedRange = lstmktCapBenchmark[0].BenchmarkSumUndefinedRange;

                result.Add(mktCapData);
                return result;

            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString(GreenfieldConstants.NETWORK_FAULT_ECX_MSG).ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }


        }
        #endregion

    }
}
