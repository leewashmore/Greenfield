using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.Helpers;
using GreenField.Web.DataContracts;
using GreenField.DAL;

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

            countryName = entity.GF_PERF_DAILY_ATTRIBUTION.Where(a => a.SEC_NAME == securityName).Select(a => a.COUNTRY_NAME).ToList();
            SectorName = entity.GF_PERF_DAILY_ATTRIBUTION.Where(a => a.SEC_NAME == securityName).Select(a => a.GICS_LVL1).ToList();

            List<RelativePerformanceUIData> result = new List<RelativePerformanceUIData>();
            return result;
        }


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
                List<MarketSnapshotSelectionData> userPreference
                    = (entity.GetMarketSnapshotSelectionData(userName)).ToList<MarketSnapshotSelectionData>();

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

        private Decimal? GetPricingDataByInstrumentIdReturnTypeAndFromDate(string instrumentId, string returnType, DateTime recordDate, out DateTime returnDate)
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
                    Decimal? presentBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, presentBusinessDate, out presentBusinessDate);
                    #endregion

                    #region Last Date Pricing Data
                    DateTime lastBusinessDate = presentBusinessDate;
                    Decimal? lastBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastBusinessDate, out lastBusinessDate);
                    #endregion

                    #region Second Last Date Pricing Data
                    DateTime secondLastBusinessDate = lastBusinessDate;
                    Decimal? secondLastBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, secondLastBusinessDate, out secondLastBusinessDate);
                    #endregion

                    #region Last Week Date Pricing Data
                    DateTime lastWeekBusinessDate = DateTime.Today.AddDays(-7);
                    Decimal? lastWeekBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastWeekBusinessDate, out lastWeekBusinessDate);
                    #endregion

                    #region Last Month Date Pricing Data
                    DateTime lastMonthBusinessDate = DateTime.Today.AddMonths(-1);
                    Decimal? lastMonthBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastMonthBusinessDate, out lastMonthBusinessDate);
                    #endregion

                    #region Last Quarter Date Pricing Data
                    DateTime lastQuarterBusinessDate = DateTime.Today.AddMonths(-3);
                    Decimal? lastQuarterBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastQuarterBusinessDate, out lastQuarterBusinessDate);
                    #endregion

                    #region Last Year Date Pricing Data
                    DateTime lastYearBusinessDate = new DateTime(DateTime.Today.Year - 1, 12, 31);
                    Decimal? lastYearBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, lastYearBusinessDate, out lastYearBusinessDate);
                    #endregion

                    #region Second Last Year Date Pricing Data
                    DateTime secondLastYearBusinessDate = new DateTime(DateTime.Today.Year - 2, 12, 31);
                    Decimal? secondLastYearBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, secondLastYearBusinessDate, out secondLastYearBusinessDate);
                    #endregion

                    #region Third Last Year Date Pricing Data
                    DateTime thirdLastYearBusinessDate = new DateTime(DateTime.Today.Year - 3, 12, 31);
                    Decimal? thirdLastYearBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
                        , preference.EntityReturnType, thirdLastYearBusinessDate, out thirdLastYearBusinessDate);
                    #endregion

                    #region Fourth Last Year Date Pricing Data
                    DateTime fourthLastYearBusinessDate = new DateTime(DateTime.Today.Year - 4, 12, 31);
                    Decimal? fourthLastYearBusinessDatePrice = GetPricingDataByInstrumentIdReturnTypeAndFromDate(entityInstrumentId
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

                return null;
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
    }
}
