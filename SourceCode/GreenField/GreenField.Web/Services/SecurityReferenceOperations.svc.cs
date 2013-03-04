using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for Security Reference
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SecurityReferenceOperations
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

        private class BenchmarkSelectionData : IEquatable<BenchmarkSelectionData>
        {
            public String BenchmarkId { get; set; }
            public String BenchmarkName { get; set; }

            public bool Equals(BenchmarkSelectionData other)
            {
                if (Object.ReferenceEquals(other, null)) return false;
                if (Object.ReferenceEquals(this, other)) return true;
                //return BenchmarkId.Equals(other.BenchmarkId) && BenchmarkName.Equals(other.BenchmarkName);
                return BenchmarkId.Equals(other.BenchmarkId);
            }

            public override int GetHashCode()
            {
                int hashBenchmarkId = BenchmarkId.GetHashCode();
                //int hashBenchmarkName = BenchmarkName.GetHashCode();
                //return hashBenchmarkId ^ hashBenchmarkName;
                return hashBenchmarkId;
            }
        }

        /// <summary>
        /// Service Fault Resource manager
        /// </summary>
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        /// <summary>
        /// retrieving the security data for security overview
        /// </summary>
        /// <returns>list of security overview data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<SecurityOverviewData> RetrieveSecurityReferenceData()
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_SECURITY_BASEVIEW> data = entity.GF_SECURITY_BASEVIEW.ToList();

                List<SecurityOverviewData> result = new List<SecurityOverviewData>();
                foreach (DimensionEntitiesService.GF_SECURITY_BASEVIEW record in data)
                {
                    result.Add(new SecurityOverviewData()
                    {
                        IssueName = record.ISSUE_NAME,
                        Ticker = record.TICKER,
                        Country = record.ISO_COUNTRY_CODE,
                        Sector = record.GICS_SECTOR_NAME,
                        Industry = record.GICS_INDUSTRY_NAME,
                        SubIndustry = record.GICS_SUB_INDUSTRY_NAME,
                        PrimaryAnalyst = record.ASHMOREEMM_PRIMARY_ANALYST,
                        Currency = record.TRADING_CURRENCY,
                        FiscalYearend = record.FISCAL_YEAR_END,
                        Website = record.WEBSITE,
                        Description = record.BLOOMBERG_DESCRIPTION
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
        /// retrieving the security data on ticker filter
        /// </summary>
        /// <returns>list of security overview data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public SecurityOverviewData RetrieveSecurityOverviewData(EntitySelectionData entitySelectionData)
        {
            try
            {
                if (entitySelectionData == null)
                { return new SecurityOverviewData(); }

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                { throw new Exception("Services are not available"); }

                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (data == null)
                { return new SecurityOverviewData(); }

                SecurityOverviewData result = new SecurityOverviewData()
                {
                    IssueName = data.ISSUE_NAME,
                    Ticker = data.TICKER,
                    Country = data.ISO_COUNTRY_CODE,
                    Sector = data.GICS_SECTOR_NAME,
                    Industry = data.GICS_INDUSTRY_NAME,
                    SubIndustry = data.GICS_SUB_INDUSTRY_NAME,
                    PrimaryAnalyst = data.ASHMOREEMM_PRIMARY_ANALYST,
                    Currency = data.TRADING_CURRENCY,
                    FiscalYearend = data.FISCAL_YEAR_END,
                    Website = data.WEBSITE,
                    Description = data.BLOOMBERG_DESCRIPTION
                };

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
        /// Retrieving the Pricing Reference Data for selected Entity for the gadget ClosingPriceChart.
        /// </summary>
        /// <param name="entityIdentifiers">Selected Security/Commodity/Index</param>
        /// <param name="startDateTime">start time for the chart</param>
        /// <param name="endDateTime">end time for the chart</param>
        /// <param name="totalReturnCheck">Check to include TotalPriceReturn or TotalGrossReturn</param>
        /// <param name="frequencyDuration">Frequency Duration </param>
        /// <returns>List of Pricing Reference Data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PricingReferenceData> RetrievePricingReferenceData(ObservableCollection<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyDuration)
        {
            try
            {
                if (entityIdentifiers == null || frequencyDuration == null)
                {
                    return new List<PricingReferenceData>();
                }
                decimal objAdjustedDollarPrice = 0;
                decimal objPreviousDailySpotFx = 0;
                decimal objIndexedPrice = 0;
                decimal objReturn = 0;

                decimal curPrice = 0;
                decimal curReturn = 0;
                decimal calculatedPrice = 0;
                string entityType = "";
                string entityInstrumentID = "";
                DateTime startDate = Convert.ToDateTime(startDateTime);
                DateTime endDate = Convert.ToDateTime(endDateTime);
                int sortingID = 0;

                //List Containing the names of Securities/Commodities/Indexes to be added
                List<string> entityNames = (from p in entityIdentifiers
                                            select p.InstrumentID).ToList();

                List<PricingReferenceData> pricingDataResult = new List<PricingReferenceData>();

                DimensionEntitiesService.Entities entity = DimensionEntity;

                //Plotting a Single Line Chart
                #region SingleLineChart

                if (entityIdentifiers.Count() == 1)
                {
                    entityInstrumentID = Convert.ToString(entityIdentifiers[0].InstrumentID);
                    entityType = Convert.ToString(entityIdentifiers[0].Type);

                    DateTime webServiceStartTime = DateTime.Now;

                    List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData = entity.GF_PRICING_BASEVIEW
                    .Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >= startDate) && (r.FROMDATE < endDate))
                    .OrderByDescending(res => res.FROMDATE).ToList();

                    DateTime webServiceEndTime = DateTime.Now;

                    // Calcluating the values of curPrice,curReturn,calculatedPrice
                    if (dimensionServicePricingData.Count != 0)
                    {
                        bool dataNotFound = true;
                        while (dataNotFound)
                        {
                            if ((dimensionServicePricingData[0].DAILY_CLOSING_PRICE == null) || (dimensionServicePricingData[0].DAILY_PRICE_RETURN == null) || (dimensionServicePricingData[0].DAILY_GROSS_RETURN == null))
                            {
                                dimensionServicePricingData.RemoveAt(0);
                            }
                            else
                            {
                                dataNotFound = false;
                            }
                        }
                        curPrice = Convert.ToDecimal(dimensionServicePricingData[0].DAILY_CLOSING_PRICE);
                        curReturn = (totalReturnCheck) ? (Convert.ToDecimal(dimensionServicePricingData[0].DAILY_GROSS_RETURN)) : (Convert.ToDecimal(dimensionServicePricingData[0].DAILY_PRICE_RETURN));
                        calculatedPrice = curPrice;

                        foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                        {
                            PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                            objPricingReferenceData.Type = pricingItem.TYPE;
                            objPricingReferenceData.Ticker = pricingItem.TICKER + ((totalReturnCheck) ? " (Total)" : "");
                            objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                            objPricingReferenceData.FromDate = (DateTime)pricingItem.FROMDATE;
                            objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                            objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                            objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                            objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                            objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                            objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);
                            objPricingReferenceData.SortingID = sortingID;
                            //Checking if the Item is the first item in the list
                            if ((pricingItem.INSTRUMENT_ID == dimensionServicePricingData[0].INSTRUMENT_ID) && (pricingItem.FROMDATE == dimensionServicePricingData[0].FROMDATE))
                            {
                                // if it is the first item in the list then simply save the value of calculated price
                                objPricingReferenceData.IndexedPrice = calculatedPrice;
                            }
                            else
                            {
                                //if it is not the first item then executing the logic.
                                calculatedPrice = (curPrice / ((curReturn / 100) + 1));
                                curPrice = calculatedPrice;
                                objPricingReferenceData.IndexedPrice = calculatedPrice;
                                curReturn = (totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN));
                            }
                            pricingDataResult.Add(objPricingReferenceData);
                        }
                    }
                }

                #endregion

                //Plotting a Multi-Line Comparison Chart
                #region MultiLineChart

                if (entityIdentifiers.Count() > 1)
                {
                    foreach (EntitySelectionData item in entityIdentifiers)
                    {
                        if (Convert.ToString(item.Type).ToUpper() == "SECURITY")
                        {
                            sortingID = ++sortingID;
                            entityInstrumentID = Convert.ToString(item.InstrumentID);


                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate) && (r.DAILY_SPOT_FX != 0)).OrderByDescending(res => res.FROMDATE).ToList();


                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    if (pricingItem.DAILY_SPOT_FX == 0)
                                        continue;

                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER + ((totalReturnCheck) ? " (Total)" : "");
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = (DateTime)pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);
                                    objPricingReferenceData.SortingID = sortingID;

                                    //Checking if the current object is first in the series
                                    if (pricingItem.FROMDATE == dimensionServicePricingData[0].FROMDATE)
                                    {
                                        objPricingReferenceData.AdjustedDollarPrice = (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / objPricingReferenceData.DailySpotFX);
                                    }
                                    else
                                    {
                                        objPricingReferenceData.AdjustedDollarPrice =
                                            objAdjustedDollarPrice / ((1 + (objReturn / 100)) * (Convert.ToDecimal(pricingItem.DAILY_SPOT_FX) / objPreviousDailySpotFx));
                                    }
                                    objAdjustedDollarPrice = objPricingReferenceData.AdjustedDollarPrice;
                                    objPreviousDailySpotFx = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                                    pricingDataResult.Add(objPricingReferenceData);
                                }
                            }
                        }

                        else if ((Convert.ToString(item.Type).ToUpper() == "COMMODITY") || ((Convert.ToString(item.Type).ToUpper() == "INDEX")) || ((Convert.ToString(item.Type).ToUpper() == "CURRENCY")))
                        {
                            sortingID = ++sortingID;
                            entityInstrumentID = Convert.ToString(item.InstrumentID);
                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate)).OrderBy(res => res.FROMDATE).ToList();

                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    if (pricingItem.DAILY_SPOT_FX == 0)
                                        continue;
                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER;
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = (DateTime)pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);
                                    objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                                    objPricingReferenceData.AdjustedDollarPrice =
                                        (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / Convert.ToDecimal(pricingItem.DAILY_SPOT_FX));
                                    objPricingReferenceData.SortingID = sortingID;
                                    pricingDataResult.Add(objPricingReferenceData);
                                }
                            }
                        }

                        pricingDataResult = pricingDataResult.OrderBy(r => r.FromDate).ToList();

                        if ((pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID))).ToList().Count() > 0)
                        {
                            pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).FirstOrDefault().IndexedPrice = 100;
                        }

                        foreach (PricingReferenceData objPricingDataResult in pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).ToList())
                        {
                            if (objPricingDataResult.FromDate == pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).First().FromDate)
                            {
                                objAdjustedDollarPrice = objPricingDataResult.AdjustedDollarPrice;
                                objIndexedPrice = objPricingDataResult.IndexedPrice;
                            }
                            else
                            {
                                objPricingDataResult.IndexedPrice = (objPricingDataResult.AdjustedDollarPrice / objAdjustedDollarPrice) * objIndexedPrice;
                                objIndexedPrice = objPricingDataResult.IndexedPrice;
                                objAdjustedDollarPrice = objPricingDataResult.AdjustedDollarPrice;
                            }
                        }
                    }

                    foreach (PricingReferenceData item in pricingDataResult)
                    {
                        item.IndexedPrice = item.IndexedPrice - 100;
                    }
                }

                #endregion

                #region FilterDataAccordingToFrequency

                List<DateTime> endDates = new List<DateTime>();
                endDates = (from p in pricingDataResult
                            select p.FromDate).Distinct().ToList();

                List<DateTime> allEndDates = new List<DateTime>();

                allEndDates = FrequencyCalculator.RetrieveDatesAccordingToFrequency(endDates, startDateTime, endDateTime, frequencyDuration);

                List<PricingReferenceData> result = new List<PricingReferenceData>();

                if (frequencyDuration != "Daily")
                {
                    foreach (EntitySelectionData item in entityIdentifiers)
                    {
                        List<PricingReferenceData> individualSeriesResult = RetrievePricingDataAccordingFrequency(pricingDataResult.Where(r => r.InstrumentID == item.InstrumentID).OrderBy(r => r.FromDate).ToList(), allEndDates);
                        result.AddRange(individualSeriesResult);
                    }

                }
                else
                {
                    result = pricingDataResult;
                }
                #endregion

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
        /// retrieve list of securities for security selector
        /// </summary>
        /// <returns>list of entity selection data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<EntitySelectionData> RetrieveEntitySelectionData()
        {
            try
            {
                // use cache if available
                var fromCache = (List<EntitySelectionData>)new DefaultCacheProvider().Get(CacheKeyNames.EntitySelectionDataCache);
                if (fromCache != null)
                    return fromCache;

                // otherwise fetch the data and cache it
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");


                List<DimensionEntitiesService.GF_SELECTION_BASEVIEW> data = DimensionEntity.GF_SELECTION_BASEVIEW.ToList();
#if DEBUG
                Stopwatch swDimensionSec = new Stopwatch();
                DateTime timedimensionSec = new DateTime();
                Stopwatch swLocalSec = new Stopwatch();
                DateTime timelocalSec = new DateTime();

                swDimensionSec.Start();
#endif

                //List<DimensionEntitiesService.GF_SECURITY_BASEVIEW> securities2 = DimensionEntity.GF_SECURITY_BASEVIEW.ToList();

#if DEBUG
                swDimensionSec.Stop();
                timedimensionSec = DateTime.Now;

                swLocalSec.Start();
#endif

                var securities = new GreenField.DAL.ExternalResearchEntities().GF_SECURITY_BASEVIEW_Local.ToList();

#if DEBUG
                swLocalSec.Stop();
                timelocalSec = DateTime.Now;

                Trace.WriteLine(string.Format("{1}: 1. DimensionEntity.GF_SECURITY_BASEVIEW = {0} seconds.", (swDimensionSec.ElapsedMilliseconds / 1000.00).ToString(), timedimensionSec.ToString()));
                Trace.WriteLine(string.Format("{1}: 2. GF_SECURITY_BASEVIEW_Local = {0} seconds.", (swLocalSec.ElapsedMilliseconds / 1000.00).ToString(), timelocalSec.ToString()));
#endif

                List<EntitySelectionData> result = new List<EntitySelectionData>();
                if (data != null)
                {
                    foreach (DimensionEntitiesService.GF_SELECTION_BASEVIEW record in data)
                    {
                        var security = securities.Where(sec => sec.ASEC_SEC_SHORT_NAME == record.INSTRUMENT_ID).FirstOrDefault();
                    
                        result.Add(new EntitySelectionData()
                        {
                            SortOrder = EntityTypeSortOrder.GetSortOrder(record.TYPE),
                            ShortName = record.SHORT_NAME == null ? String.Empty : record.SHORT_NAME,
                            LongName = record.LONG_NAME == null ? String.Empty : record.LONG_NAME,
                            InstrumentID = record.INSTRUMENT_ID == null ? String.Empty : record.INSTRUMENT_ID,
                            Type = record.TYPE == null ? String.Empty : record.TYPE,
                            SecurityType = record.SECURITY_TYPE == null ? String.Empty : record.SECURITY_TYPE,
                            SecurityId = security != null ? security.SECURITY_ID : null,
                            IssuerId = security != null ? security.ISSUER_ID : null,
                            LOOK_THRU_FUND = security != null ? security.LOOK_THRU_FUND : null
                        });
                    }
                }

                List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIB_DIST_BM> benchmarkData = DimensionEntity.GF_PERF_DAILY_ATTRIB_DIST_BM.ToList();
                if (benchmarkData != null)
                {
                    foreach (DimensionEntitiesService.GF_PERF_DAILY_ATTRIB_DIST_BM benchmark in benchmarkData)
                    {
                        result.Add(new EntitySelectionData()
                        {

                            SortOrder = EntityTypeSortOrder.GetSortOrder("BENCHMARK"),
                            ShortName = benchmark.BM == null ? String.Empty : benchmark.BM,
                            LongName = benchmark.BMNAME == null ? String.Empty : benchmark.BMNAME,
                            InstrumentID = benchmark.BM == null ? String.Empty : benchmark.BM,
                            Type = "BENCHMARK",
                            SecurityType = null
                        });
                    }
                }

                new DefaultCacheProvider().Set(CacheKeyNames.EntitySelectionDataCache, result, Int32.Parse(ConfigurationManager.AppSettings["SecuritiesCacheTime"]));

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
        /// retrieve list of securities for security selector
        /// </summary>
        /// <returns>list of entity selection data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<EntitySelectionData> RetrieveSecuritiesData()
        {
            try
            {
                // use cache if available
                var fromCache = (List<EntitySelectionData>)new DefaultCacheProvider().Get(CacheKeyNames.SecurityDataCache);
                if (fromCache != null)
                    return fromCache;

                // otherwise fetch the data and cache it
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

#if DEBUG
                Stopwatch swLocalSec = new Stopwatch();
                DateTime timelocalSec = new DateTime();
                swLocalSec.Start();
#endif

                var securities = new GreenField.DAL.ExternalResearchEntities().GF_SECURITY_BASEVIEW_Local.ToList();

                List<EntitySelectionData> result = new List<EntitySelectionData>();
                foreach (var security in securities)
                {
                    result.Add(new EntitySelectionData()
                        {
                            ShortName = security.ASEC_SEC_SHORT_NAME,
                            SecurityType = security.SECURITY_TYPE,
                            SecurityId = security.SECURITY_ID,
                            IssuerId = security.ISSUER_ID,
                            LOOK_THRU_FUND = security.LOOK_THRU_FUND
                        });
                }
#if DEBUG
                swLocalSec.Stop();
                timelocalSec = DateTime.Now;
                Trace.WriteLine(string.Format("{1}: 2. GF_SECURITY_BASEVIEW_Local = {0} seconds.", (swLocalSec.ElapsedMilliseconds / 1000.00).ToString(), timelocalSec.ToString()));
#endif

                new DefaultCacheProvider().Set(CacheKeyNames.SecurityDataCache, result, Int32.Parse(ConfigurationManager.AppSettings["SecuritiesCacheTime"]));
                return result;
                
                /*

                                if (data != null)
                                {
                                    foreach (DimensionEntitiesService.GF_SECURITY_BASEVIEW record in data)
                                    {
                                        var security = securities.Where(sec => sec.ASEC_SEC_SHORT_NAME == record.INSTRUMENT_ID).FirstOrDefault();

                                        result.Add(new EntitySelectionData()
                                        {
                                            SortOrder = EntityTypeSortOrder.GetSortOrder(record.TYPE),
                                            ShortName = record.SHORT_NAME == null ? String.Empty : record.SHORT_NAME,
                                            LongName = record.LONG_NAME == null ? String.Empty : record.LONG_NAME,
                                            InstrumentID = record.INSTRUMENT_ID == null ? String.Empty : record.INSTRUMENT_ID,
                                            Type = record.TYPE == null ? String.Empty : record.TYPE,
                                            SecurityType = record.SECURITY_TYPE == null ? String.Empty : record.SECURITY_TYPE,
                                            SecurityId = security != null ? security.SECURITY_ID : null,
                                            IssuerId = security != null ? security.ISSUER_ID : null,
                                            LOOK_THRU_FUND = security != null ? security.LOOK_THRU_FUND : null
                                        });
                                    }
                                }

                                List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIB_DIST_BM> benchmarkData = DimensionEntity.GF_PERF_DAILY_ATTRIB_DIST_BM.ToList();
                                if (benchmarkData != null)
                                {
                                    foreach (DimensionEntitiesService.GF_PERF_DAILY_ATTRIB_DIST_BM benchmark in benchmarkData)
                                    {
                                        result.Add(new EntitySelectionData()
                                        {

                                            SortOrder = EntityTypeSortOrder.GetSortOrder("BENCHMARK"),
                                            ShortName = benchmark.BM == null ? String.Empty : benchmark.BM,
                                            LongName = benchmark.BMNAME == null ? String.Empty : benchmark.BMNAME,
                                            InstrumentID = benchmark.BM == null ? String.Empty : benchmark.BM,
                                            Type = "BENCHMARK",
                                            SecurityType = null
                                        });
                                    }
                                }
                                 */

            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        
        /// <summary>
        /// retrieve list of securities for security selector
        /// </summary>
        /// <returns>list of entity selection data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<EntitySelectionData> RetrieveEntitySelectionWithBenchmarkData()
        {
            try
            {
                List<EntitySelectionData> result = new List<EntitySelectionData>();

                //List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIB_DIST_BM> benchmarkSelectionData = DimensionEntity.GF_PERF_DAILY_ATTRIB_DIST_BM
                //    .OrderBy(record => record.BM).ToList();


                //    .Where(record => record.TO_DATE == Convert.ToDateTime("30/4/2012"))
                //    .Select(record => new BenchmarkSelectionData()
                //    {
                //        BenchmarkId = record.BM,
                //        BenchmarkName = record.BMNAME
                //    })
                //    .ToList();

                //benchmarkSelectionData = benchmarkSelectionData.Distinct().ToList();

                //if (benchmarkSelectionData != null)
                //{
                //    foreach (BenchmarkSelectionData record in benchmarkSelectionData)
                //    {
                //        result.Add(new EntitySelectionData()
                //        {

                //            SortOrder = EntityTypeSortOrder.GetSortOrder("BENCHMARK"),
                //            ShortName = record.BenchmarkId,
                //            LongName = record.BenchmarkName,
                //            InstrumentID = null,
                //            Type = "BENCHMARK",
                //            SecurityType = null
                //        });
                //    }
                //}

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region Pricing Chart Helper Methods

        /// <summary>
        /// Method to Retrieve Data Filtered according to Frequency.
        /// If for a particular day , data is not present then the data for 1-day before is considered.
        /// </summary>
        /// <param name="objPricingData"></param>
        /// <param name="objEndDates"></param>
        /// <returns></returns>
        private List<PricingReferenceData> RetrievePricingDataAccordingFrequency(List<PricingReferenceData> objPricingData, List<DateTime> objEndDates)
        {
            try
            {
                List<PricingReferenceData> resultFrequency = new List<PricingReferenceData>();
                List<DateTime> EndDates = objEndDates;

                foreach (DateTime item in EndDates)
                {
                    int i = 1;
                    bool dateObjectFound = true;

                    if (objPricingData.Any(r => r.FromDate.Date == item.Date))
                    {
                        resultFrequency.Add(objPricingData.Where(r => r.FromDate == item.Date).First());
                        dateObjectFound = false;
                        continue;
                    }
                    else
                    {
                        dateObjectFound = true;
                    }

                    while (dateObjectFound)
                    {
                        //Checking Data for 1-Day before
                        bool objDataFoundDec = objPricingData.Any(r => r.FromDate.Date == item.AddDays(-i).Date);
                        if (objDataFoundDec)
                        {
                            resultFrequency.Add(objPricingData.Where(r => r.FromDate.Date == item.AddDays(-i).Date).First());
                            dateObjectFound = false;
                        }
                        else
                        {
                            i++;
                            //If data for 30 days before doesn't exist, then move to next Date.
                            if (i > 30)
                            {
                                dateObjectFound = false;
                                continue;
                            }
                        }
                    }
                }
                return resultFrequency.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method to Retrieve Data Filtered according to Frequency.
        /// If for a particular day , data is not present then the data for 1-day before is considered.
        /// </summary>
        /// <param name="objUnrealizedGainLossData"></param>
        /// <param name="objEndDates"></param>
        /// <returns></returns>
        private List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(List<UnrealizedGainLossData> objUnrealizedGainLossData, List<DateTime> objEndDates)
        {
            try
            {
                List<UnrealizedGainLossData> resultFrequency = new List<UnrealizedGainLossData>();

                List<DateTime> EndDates = objEndDates;
                foreach (DateTime item in EndDates)
                {
                    int i = 1;
                    bool dateObjectFound = true;

                    if (objUnrealizedGainLossData.Any(r => r.FromDate.Date == item.Date))
                    {
                        resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate.Date == item.Date).First());
                        dateObjectFound = false;
                        continue;
                    }
                    else
                    {
                        dateObjectFound = true;
                    }

                    while (dateObjectFound)
                    {
                        bool objDataFoundDec = objUnrealizedGainLossData.Any(r => r.FromDate.Date == item.AddDays(-i).Date);
                        if (objDataFoundDec)
                        {
                            resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate.Date == item.AddDays(-i).Date).First());
                            dateObjectFound = false;
                        }
                        else
                        {
                            i++;
                            if (i > 30)
                            {
                                dateObjectFound = false;
                                continue;
                            }
                        }
                    }
                }
                return resultFrequency.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        #endregion

        #region Connection String Methods
        private string GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"ND1DDYYB6Q1\SQLEXPRESS";
            builder.InitialCatalog = "AshmoreEMMPOC";
            builder.UserID = "sa";
            builder.Password = "India@123";
            builder.MultipleActiveResultSets = true;
            return builder.ConnectionString;
        }

        private DataTable GetDataTable(string queryString)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable dataTable = new DataTable();
                dataTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

                try
                {
                    sqlDataAdapter.Fill(dataTable);
                    connection.Close();
                }
                catch (Exception)
                {

                    return null;
                }

                return dataTable;
            }
        }
        #endregion

        #region Unrealized Gain Loss Operation contract
        /// <summary>
        /// Retrieves the Theoretical Unrealized Gain Loss Data for selected Entity.
        /// </summary>
        /// <param name="entityIdentifier">Ticker for the security</param>
        /// <param name="startDateTime">Start Date of the Time Period that is selected</param>
        /// <param name="endDateTime">End Date of the Time Period that is selected</param>       
        /// <param name="frequencyInterval">Frequency Duration selected</param>       
        /// <returns>List of UnrealozedGainLossData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(EntitySelectionData entityIdentifier, DateTime startDateTime, DateTime endDateTime, string frequencyInterval)
        {
            try
            {
                List<UnrealizedGainLossData> result = new List<UnrealizedGainLossData>();

                if (entityIdentifier == null || entityIdentifier.ShortName == null || endDateTime < startDateTime)
                {
                    return result;
                }
                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_PRICING_BASEVIEW> resultSet
                    = entity.GF_PRICING_BASEVIEW
                        .Where(record => (record.INSTRUMENT_ID == entityIdentifier.InstrumentID))
                        .OrderByDescending(record => record.FROMDATE)
                        .ToList();

                int noOfRows = resultSet.Count();
                if (noOfRows < 90)
                {
                    return result;
                }

                //Calculating the Adjusted price for a security and storing it in the list.
                List<UnrealizedGainLossData> adjustedPriceResult = UnrealizedGainLossCalculations.CalculateAdjustedPrice(resultSet);

                //Calculating the Moving Average for a security and storing it in the list.
                List<UnrealizedGainLossData> movingAverageResult = UnrealizedGainLossCalculations.CalculateMovingAverage(adjustedPriceResult);

                //Calculating the Ninety Day Weight for a security and storing it in the list.
                List<UnrealizedGainLossData> ninetyDayWtResult = UnrealizedGainLossCalculations.CalculateNinetyDayWtAvg(movingAverageResult);

                //Calculating the Cost for a security and storing it in the list.
                List<UnrealizedGainLossData> costResult = UnrealizedGainLossCalculations.CalculateCost(ninetyDayWtResult);

                //Calculating the Weighted Average Cost for a security and storing it in the list.
                List<UnrealizedGainLossData> wtAvgCostResult = UnrealizedGainLossCalculations.CalculateWtAvgCost(costResult);

                //Calculating the Unrealized Gain loss for a security and storing it in the list.
                List<UnrealizedGainLossData> unrealizedGainLossResult = UnrealizedGainLossCalculations.CalculateUnrealizedGainLoss(wtAvgCostResult);

                //Filtering the list according to the time period selected
                List<UnrealizedGainLossData> timeFilteredUnrealizedGainLossResult
                    = unrealizedGainLossResult
                        .Where(record => (record.FromDate >= startDateTime) && (record.FromDate < endDateTime))
                        .ToList();

                //Filtering the list according to the frequency selected.
                List<DateTime> EndDates
                    = timeFilteredUnrealizedGainLossResult
                        .Select(record => record.FromDate)
                        .ToList();

                //Calculating the date points based on Data Frequency
                List<DateTime> allEndDates = FrequencyCalculator.RetrieveDatesAccordingToFrequency(EndDates, startDateTime, endDateTime, frequencyInterval);
                result = UnrealizedGainLossCalculations.RetrieveUnrealizedGainLossData(timeFilteredUnrealizedGainLossResult, allEndDates);
                return result;
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