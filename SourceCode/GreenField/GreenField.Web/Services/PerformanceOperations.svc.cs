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
    /// Service for Performance Operations
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PerformanceOperations
    {
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

        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        #region PerformanceServices

        /// <summary>
        /// Service Method for RelativePerformanceUI gadget
        /// </summary>
        /// <param name="objSelectedEntity">details of selected Portfolio & Security</param>
        /// <param name="objEffectiveDate">selected effective Date</param>
        /// <returns>List of RelativePerformanceUIData</returns>
        [OperationContract]
        public List<RelativePerformanceUIData> RetrieveRelativePerformanceUIData(Dictionary<string, string> objSelectedEntity, DateTime? objEffectiveDate)
        {
            //Null Arguement Check
            if ((objSelectedEntity == null) || (objEffectiveDate == null) || (objSelectedEntity.Count == 0))
                throw new Exception();

            //If dictionary object doesn't contains Security/Portfolio data, return empty set
            if (!objSelectedEntity.ContainsKey("SECURITY") || !objSelectedEntity.ContainsKey("PORTFOLIO"))
                return new List<RelativePerformanceUIData>();

            //Create new Entity for service
            DimensionEntitiesService.Entities entity = DimensionEntity;

            string securityName = objSelectedEntity.Where(a => a.Key == "SECURITY").First().Value;
            string portfolioName = objSelectedEntity.Where(a => a.Key == "PORTFOLIO").First().Value;
            List<string> countryName = new List<string>();
            List<string> SectorName = new List<string>();

            countryName = (entity.GF_PERF_DAILY_ATTRIBUTION.Where(a => a.SEC_NAME == securityName).ToList()).Select(a => a.COUNTRY_NAME).ToList();
            SectorName = (entity.GF_PERF_DAILY_ATTRIBUTION.Where(a => a.SEC_NAME == securityName).ToList()).Select(a => a.GICS_LVL1).ToList();

            List<RelativePerformanceUIData> result = new List<RelativePerformanceUIData>();
            return result;
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

                if (objSelectedEntities.ContainsKey("SECURITY"))
                    securityLongName = objSelectedEntities.Where(a => a.Key == "SECURITY").First().Value;
                if (objSelectedEntities.ContainsKey("PORTFOLIO"))
                    portfolioId = objSelectedEntities.Where(a => a.Key == "PORTFOLIO").First().Value;

                countryName = (entity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME == securityLongName).ToList()).Select(a => a.ASEC_SEC_COUNTRY_NAME).ToList();

                if (countryName.Count != 1)
                    throw new Exception("Single Security cannot have multiple countries");


                List<GF_PERF_MONTHLY_ATTRIBUTION> dimensionMonthlyPerfData = entity.GF_PERF_MONTHLY_ATTRIBUTION.
                    Where(a => a.PORTFOLIO == portfolioId
                        && ((a.AGG_LVL_1_LONG_NAME == securityLongName) || ((a.NODE_NAME.ToUpper() == "COUNTRY") && (a.COUNTRY_NAME == countryName.First())))
                        && a.TO_DATE >= startDate).ToList();

                //Checking contents of Data fetched from Dimension
                if (dimensionMonthlyPerfData == null || dimensionMonthlyPerfData.Count == 0)
                    return result;

                result = MultiLineBenchmarkUICalculations.RetrieveBenchmarkChartData(dimensionMonthlyPerfData);

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
        /// Method to retrieve data in Benchmark Grid
        /// </summary>
        /// <param name="objBenchmarkIdentifier"></param>
        /// <param name="objEffectiveDate"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<BenchmarkGridReturnData> RetrieveBenchmarkGridReturnData(Dictionary<string, string> objSelectedEntities)
        {
            List<BenchmarkGridReturnData> result = new List<BenchmarkGridReturnData>();
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;

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


                if (objSelectedEntities.ContainsKey("SECURITY"))
                    securityLongName = (objSelectedEntities.Where(a => a.Key == "SECURITY").First().Value);
                if (objSelectedEntities.ContainsKey("PORTFOLIO"))
                    portfolioId = (objSelectedEntities.Where(a => a.Key == "PORTFOLIO").First().Value);

                countryName = (entity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME.ToUpper() == securityLongName).ToList()).Select(a => a.ASEC_SEC_COUNTRY_NAME).ToList();


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
        public List<MarketSnapshotPreference> RetrieveMarketSnapshotPreference(string userName, string snapshotName)
        {
            try
            {
                if (userName == null)
                    return null;

                ResearchEntities entity = new ResearchEntities();
                List<MarketSnapshotPreference> userPreference = (entity.GetMarketSnapshotPreference(userName, snapshotName)).ToList<MarketSnapshotPreference>();
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
                entity.DeleteMarketSnapshotEntityPreference(marketSnapshotPreference.EntityPreferenceId);
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
        public List<MarketSnapshotPreference> SaveMarketSnapshotPreference(string userName, MarketSnapshotSelectionData marketSnapshotSelectionData
            , List<MarketSnapshotPreference> createEntityPreferenceInfo, List<MarketSnapshotPreference> updateEntityPreferenceInfo
            , List<MarketSnapshotPreference> deleteEntityPreferenceInfo, List<int> deleteGroupPreferenceInfo, List<string> createGroupPreferenceInfo)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                foreach (string groupName in createGroupPreferenceInfo)
                {
                    int groupPrefId = Convert.ToInt32(entity.SetMarketSnapshotGroupPreference(marketSnapshotSelectionData.SnapshotPreferenceId, groupName).FirstOrDefault());

                    foreach (MarketSnapshotPreference preference in createEntityPreferenceInfo)
                    {
                        if (preference.GroupName == groupName)
                        {
                            entity.SetMarketSnapshotEntityPreference(groupPrefId, preference.EntityName
                                , preference.EntityReturnType, preference.EntityType, preference.EntityOrder);
                        }
                    }
                }

                foreach (MarketSnapshotPreference preference in createEntityPreferenceInfo)
                {
                    if (!createGroupPreferenceInfo.Contains(preference.GroupName))
                    {
                        entity.SetMarketSnapshotEntityPreference(preference.GroupPreferenceID, preference.EntityName
                            , preference.EntityReturnType, preference.EntityType, preference.EntityOrder);
                    }
                }

                foreach (int groupPreferenceId in deleteGroupPreferenceInfo)
                {
                    entity.DeleteMarketSnapshotGroupPreference(groupPreferenceId);
                }

                foreach (MarketSnapshotPreference preference in deleteEntityPreferenceInfo)
                {
                    entity.DeleteMarketSnapshotEntityPreference(preference.EntityPreferenceId);
                }

                foreach (MarketSnapshotPreference preference in updateEntityPreferenceInfo)
                {
                    entity.UpdateMarketSnapshotEntityPreference(preference.GroupPreferenceID
                        , preference.EntityPreferenceId, preference.EntityOrder);
                }

                List<MarketSnapshotPreference> userPreference = (entity.GetMarketSnapshotPreference(userName
                    , marketSnapshotSelectionData.SnapshotName)).ToList<MarketSnapshotPreference>();

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
        public MarketSnapshotSelectionData SaveAsMarketSnapshotPreference(string userName, string snapshotName, List<MarketSnapshotPreference> snapshotPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                Decimal? snapshotID = entity.SetMarketSnapshotPreference(userName, snapshotName).FirstOrDefault();
                if (snapshotID == null)
                    return null;

                snapshotPreference = snapshotPreference
                    .OrderBy(record => record.GroupPreferenceID)
                    .ThenBy(record => record.EntityOrder)
                    .ToList();

                string insertedGroupName = String.Empty;
                Decimal? groupPreferenceId = 0;

                foreach (MarketSnapshotPreference preference in snapshotPreference)
                {
                    if (preference.GroupName != insertedGroupName)
                    {
                        groupPreferenceId = entity.SetMarketSnapshotGroupPreference(Convert.ToInt32(snapshotID), preference.GroupName).FirstOrDefault();
                        insertedGroupName = preference.GroupName;
                    }

                    entity.SetMarketSnapshotEntityPreference(Convert.ToInt32(groupPreferenceId), preference.EntityName, preference.EntityReturnType,
                        preference.EntityType, preference.EntityOrder);
                }

                MarketSnapshotSelectionData result = (entity.GetMarketSnapshotSelectionData(userName))
                    .ToList<MarketSnapshotSelectionData>()
                    .Where(record => record.SnapshotName == snapshotName)
                    .FirstOrDefault();
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


       

        [OperationContract]
        public EntitySelectionData TestService(PortfolioSelectionData obj, EntitySelectionData obje)
        {
            EntitySelectionData r = new EntitySelectionData();
            return r;
        }

    }
}
