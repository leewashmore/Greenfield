using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GreenField.DAL;
using System.Data;
using GreenField.Web.DataContracts;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using GreenField.Web.Helpers;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using System.Drawing;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ProxyDataOperations
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

        /// <summary>
        /// retrieving the security data for security overview
        /// </summary>
        /// <returns>list of security overview data</returns>
        [OperationContract]
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
                    FiscalYearEnd = record.FISCAL_YEAR_END,
                    Website = record.WEBSITE,
                    Description = record.BLOOMBERG_DESCRIPTION
                });
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// retrieving the security data on ticker filter
        /// </summary>
        /// <returns>list of security overview data</returns>
        [OperationContract]
        public SecurityOverviewData RetrieveSecurityReferenceDataByTicker(string ticker)
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW.Where(o => o.TICKER == ticker).FirstOrDefault();

                if (data == null)
                    return new SecurityOverviewData();

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
                    FiscalYearEnd = data.FISCAL_YEAR_END,
                    Website = data.WEBSITE,
                    Description = data.BLOOMBERG_DESCRIPTION
                };

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieving the Pricing Reference Data for selected Entity.
        /// </summary>
        /// <param name="entityIdentifiers"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="totalReturnCheck"></param>
        /// <param name="frequencyDuration"></param>
        /// <param name="chartEntityTypes"></param>
        /// <returns>List of PricingReferenceData</returns>
        [OperationContract]
        public List<PricingReferenceData> RetrievePricingReferenceData(List<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyDuration)
        {
            try
            {
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
                            objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                            objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                            objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                            objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                            objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                            objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                            objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);

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
                        if (Convert.ToString(item.Type) == "SECURITY")
                        {
                            entityInstrumentID = Convert.ToString(item.InstrumentID);

                            
                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate)).OrderByDescending(res => res.FROMDATE).ToList();


                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER + ((totalReturnCheck) ? " (Total)" : "");
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);

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

                        else if ((Convert.ToString(item.Type) == "COMMODITY") || ((Convert.ToString(item.Type) == "INDEX")) || ((Convert.ToString(item.Type) == "CURRENCY")))
                        {
                            entityInstrumentID = Convert.ToString(item.InstrumentID);
                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate)).OrderBy(res => res.FROMDATE).ToList();

                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER;
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);
                                    objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                                    objPricingReferenceData.AdjustedDollarPrice =
                                        (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / Convert.ToDecimal(pricingItem.DAILY_SPOT_FX));
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
                return null;
            }
        }

        /// <summary>
        /// retrieve list of securities for security selector
        /// </summary>
        /// <returns>list of entity selection data</returns>
        [OperationContract]
        public List<EntitySelectionData> RetrieveEntitySelectionData()
        {
            try
            {
                List<DimensionEntitiesService.GF_SELECTION_BASEVIEW> data = DimensionEntity.GF_SELECTION_BASEVIEW.ToList();
                List<EntitySelectionData> result = new List<EntitySelectionData>();
                if (data != null)
                {
                    foreach (DimensionEntitiesService.GF_SELECTION_BASEVIEW record in data)
                    {
                        result.Add(new EntitySelectionData()
                        {
                            SortOrder = EntityTypeSortOrder.GetSortOrder(record.TYPE),
                            ShortName = record.SHORT_NAME,
                            LongName = record.LONG_NAME,
                            InstrumentID = record.INSTRUMENT_ID,
                            Type = record.TYPE,
                            SecurityType = record.SECURITY_TYPE
                        });
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
        
        [OperationContract]
        public List<FundSelectionData> RetrieveFundSelectionData()
        {
            try
            {
                List<FundSelectionData> result = new List<FundSelectionData>();

                for (int i = 0; i < 10; i++)
                {
                    result.Add(new FundSelectionData()
                    {
                        Category = i % 2 == 0 ? "Funds" : "Composites",
                        Name = i % 2 == 0 ? "Fund " + (i + 1).ToString() : "Composite " + (i + 1).ToString()
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        [OperationContract]
        public List<BenchmarkSelectionData> RetrieveBenchmarkSelectionData()
        {
            try
            {
                List<BenchmarkSelectionData> result = new List<BenchmarkSelectionData>();

                result.Add(new BenchmarkSelectionData() { Name = "EM Emerging Markets" });
                result.Add(new BenchmarkSelectionData() { Name = "IMI Emerging Markets" });
                result.Add(new BenchmarkSelectionData() { Name = "Indonesia" });
                result.Add(new BenchmarkSelectionData() { Name = "India" });
                result.Add(new BenchmarkSelectionData() { Name = "China" });

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        [OperationContract]
        public MarketCapitalizationData RetrieveMarketCapitalizationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                return new MarketCapitalizationData()
                    {
                        MegaLowerLimit = "100 Billion",
                        LargeLowerLimit = "10 Billion",
                        MediumLowerLimit = "2 Billion",
                        SmallLowerLimit = "250 Million",

                        PortfolioWeightedAverage = 20340,
                        BenchmarkWeightedAverage = 32450,
                        PortfolioWeightedMedian = 9123,
                        BenchmarkWeightedMedian = 13678,
                        PortfolioMegaShare = 44.9,
                        BenchmarkMegaShare = 39.6,
                        PortfolioLargeShare = 39.6,
                        BenchmarkLargeShare = 32.5,
                        PortfolioMediumShare = 15.1,
                        BenchmarkMediumShare = 11.1,
                        PortfolioSmallShare = 0.5,
                        BenchmarkSmallShare = 0,
                        PortfolioMicroShare = 0,
                        BenchmarkMicroShare = 0
                    };
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        [OperationContract]
        public List<AssetAllocationData> RetrieveAssetAllocationData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                //List<AssetAllocationData> result = new List<AssetAllocationData>();
                //result.Add(new AssetAllocationData() { Country = "Mideast Regional", PortfolioShare = 4.4, ModelShare = 4.5, BenchmarkShare = 0, BetShare = 4.5 });
                //result.Add(new AssetAllocationData() { Country = "Ex-South Africa", PortfolioShare = 1.9, ModelShare = 2.0, BenchmarkShare = 0.6, BetShare = 1.4 });
                //result.Add(new AssetAllocationData() { Country = "Cash", PortfolioShare = 0.7, ModelShare = 0.7, BenchmarkShare = 0, BetShare = 0.7 });
                //result.Add(new AssetAllocationData() { Country = "Russia", PortfolioShare = 6.6, ModelShare = 6.6, BenchmarkShare = 6.1, BetShare = 0.5 });
                //result.Add(new AssetAllocationData() { Country = "Mexico", PortfolioShare = 4.5, ModelShare = 4.4, BenchmarkShare = 4.1, BetShare = 0.3 });
                //result.Add(new AssetAllocationData() { Country = "Korea", PortfolioShare = 15.6, ModelShare = 15.3, BenchmarkShare = 15.1, BetShare = 0.2 });
                //return result;
                List<AssetAllocationData> result = new List<AssetAllocationData>();
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                object sumPortfolioWeight = dataTable.Compute("Sum(PORTFOLIO_WEIGHT)", "");
                object sumBenchmarkWeight = dataTable.Compute("Sum(BENCHMARK_WEIGHT)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new AssetAllocationData()
                    {
                        Country = row.Field<string>("ISO_COUNTRY_CODE"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT") / (sumPortfolioWeight as Single?),
                        ModelShare = row.Field<Single?>("ASH_EMM_MODEL_WEIGHT"),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?),
                        BetShare = (row.Field<Single?>("ASH_EMM_MODEL_WEIGHT")) - (row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?))
                    });
                }

                return result;

            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// retrieving data for sector breakdown gadget
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of sector breakdown data</returns>
        [OperationContract]
        public List<SectorBreakdownData> RetrieveSectorBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<SectorBreakdownData> result = new List<SectorBreakdownData>();
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                object sumPortfolioWeight = dataTable.Compute("Sum(PORTFOLIO_WEIGHT)", "");
                object sumBenchmarkWeight = dataTable.Compute("Sum(BENCHMARK_WEIGHT)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new SectorBreakdownData()
                    {
                        Sector = row.Field<string>("GICS_SECTOR_NAME"),
                        Industry = row.Field<string>("GICS_INDUSTRY_NAME"),
                        Security = row.Field<string>("ISSUE_NAME"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT") / (sumPortfolioWeight as Single?),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?),
                        BetShare = row.Field<Single?>("PORTFOLIO_WEIGHT") - row.Field<Single?>("BENCHMARK_WEIGHT")
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// retrieving data for region breakdown gadget
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of region breakdown data</returns>
        [OperationContract]
        public List<RegionBreakdownData> RetrieveRegionBreakdownData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<RegionBreakdownData> result = new List<RegionBreakdownData>();
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                object sumPortfolioWeight = dataTable.Compute("Sum(PORTFOLIO_WEIGHT)", "");
                object sumBenchmarkWeight = dataTable.Compute("Sum(BENCHMARK_WEIGHT)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new RegionBreakdownData()
                    {
                        Region = row.Field<string>("ASHEMM_PROPRIETARY_REGION_CODE"),
                        Country = row.Field<string>("ISO_COUNTRY_CODE"),
                        Security = row.Field<string>("ISSUE_NAME"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT") / (sumPortfolioWeight as Single?),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?),
                        BetShare = row.Field<Single?>("PORTFOLIO_WEIGHT") - row.Field<Single?>("BENCHMARK_WEIGHT")
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// retrieving  data for index constituent gadget
        /// </summary>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of top holdings data</returns>
        [OperationContract]
        public List<TopHoldingsData> RetrieveTopHoldingsData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                //List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> data = DimensionEntity.GF_PORTFOLIO_HOLDINGS.ToList();
                //List<TopHoldingsData> result = new List<TopHoldingsData>();

                //if (data != null)
                //{
                //    decimal? sumMarketValuePortfolio = data.Sum(t => t.DIRTY_VALUE_PC);
                //    decimal? sumMarketValueBenchmark = data.Sum(t => t.DIRTY_VALUE_PC);
                //    foreach (DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS record in data)
                //    {
                //        result.Add(new TopHoldingsData()
                //        {
                //            Ticker = record.TICKER,
                //            //Holding = record
                //            MarketValue = record.DIRTY_VALUE_PC,
                //            //PortfolioShare = record.DIRTY_VALUE_PC / sumMarketValuePortfolio,
                //            //BenchmarkShare = record.DIRTY_VALUE_PC / sumMarketValueBenchmark,
                //            //BetShare = (record.DIRTY_VALUE_PC / sumMarketValuePortfolio) / (record.DIRTY_VALUE_PC / sumMarketValueBenchmark)
                //        });
                //    }
                //}
                //return result;

                List<TopHoldingsData> result = new List<TopHoldingsData>();
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                object sumPortfolioWeight = dataTable.Compute("Sum(PORTFOLIO_WEIGHT)", "");
                object sumBenchmarkWeight = dataTable.Compute("Sum(BENCHMARK_WEIGHT)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new TopHoldingsData()
                    {
                        Ticker = row.Field<string>("TICKER"),
                        Holding = row.Field<string>("ISSUE_NAME"),
                        MarketValue = row.Field<Single?>("DIRTY_VALUE_PC"),
                        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT") / (sumPortfolioWeight as Single?),
                        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?),
                        BetShare = row.Field<Single?>("PORTFOLIO_WEIGHT") - row.Field<Single?>("BENCHMARK_WEIGHT")
                    });
                }
                return result.OrderByDescending( t => t.MarketValue).ToList().Take(10).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// retrieving  data for index constituent gadget
        /// </summary>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of index constituents data</returns>
        [OperationContract]
        public List<IndexConstituentsData> RetrieveIndexConstituentsData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<IndexConstituentsData> result = new List<IndexConstituentsData>();
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                object sumMarketValue = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "");

                foreach (DataRow row in dataTable.Rows)
                {
                    string country = row.Field<string>("ISO_COUNTRY_CODE");
                    object sumMarketValueCountry = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "ISO_COUNTRY_CODE = '" + country + "'");

                    string industry = row.Field<string>("GICS_INDUSTRY_NAME");
                    object sumMarketValueIndustry = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "GICS_INDUSTRY_NAME = '" + industry + "'");

                    result.Add(new IndexConstituentsData()
                    {
                        ConstituentName = row.Field<string>("ISSUE_NAME"),
                        Country = country,
                        Region = row.Field<string>("ASHEMM_PROPRIETARY_REGION_CODE"),
                        Sector = row.Field<string>("GICS_SECTOR_NAME"),
                        Industry = industry,
                        SubIndustry = row.Field<string>("GICS_SUB_INDUSTRY_NAME"),
                        Weight = row.Field<Single?>("DIRTY_VALUE_PC") / (sumMarketValue as Single?),
                        WeightCountry = row.Field<Single?>("DIRTY_VALUE_PC") / (sumMarketValueCountry as Single?),
                        WeightIndustry = row.Field<Single?>("DIRTY_VALUE_PC") / (sumMarketValueIndustry as Single?),
                        //DailyReturnUSD = row.Field<string>("ISSUE_NAME")
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
      
        [OperationContract]
        public List<String> RetrieveValuesForFilters(String filterType)
        {
            try
            {
                List<String> result = new List<String>();
                List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                ResearchEntities research = new ResearchEntities();
                holdingData = research.tblHoldingsDatas.ToList();
                switch (filterType)
                {
                    case "Region":
                        result = (from p in holdingData select p.ASHEMM_PROPRIETARY_REGION_CODE.ToString()).Distinct().ToList();
                        break;
                    case "Country":
                        result = (from p in holdingData select p.ISO_COUNTRY_CODE.ToString()).Distinct().ToList();
                        break;
                    case "Industry":
                        result = (from p in holdingData select p.GICS_INDUSTRY_NAME.ToString()).Distinct().ToList();
                        break;
                    case "Sector":
                        result = (from p in holdingData select p.GICS_SECTOR_NAME.ToString()).Distinct().ToList();
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
        #region Build2 Services

        ///// </summary>
        ///// <param name="objPortfolioIdentifier">Selected Portfolio</param>
        ///// <returns>List of PortfolioDetailsData</returns>
        //[OperationContract]
        //public List<PortfolioDetailsData> RetrievePortfolioDetailsData(string objPortfolioIdentifier, DateTime objSelectedDate)
        //{
        //    List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();
        //    try
        //    {
        //        Random random = new Random();
        //        for (int i = 0; i < 5; i++)
        //        {
        //            result.Add(new PortfolioDetailsData()
        //            {
        //                EntityTicker = "TCS IN",
        //                EntityName = "TATA CONSULTANCY SVCS LTD",
        //                Type = "Security",
        //                Country = "India",
        //                Shares = 345565,
        //                Price = random.Next(200, 1000),
        //                Currency = "INR",
        //                Value = 0.019995,
        //                TargetPerc = random.Next(10, 30),
        //                PortfolioPerc = random.Next(10, 30),
        //                BenchmarkPerc = random.Next(10, 30),
        //                BetPerc = random.Next(0, 20),
        //                Upside = random.Next(10, 30),
        //                YTDReturn = random.Next(0, 30),
        //                MarketCap = 100000000,
        //                PE_FWD = 0.5,
        //                PE_Fair = 0.7,
        //                PBE_Fair = 0.23,
        //                PBE_FWD = 0.456,
        //                EVEBITDA_FWD = 2344786,
        //                EVEBITDA_Fair = 2277648,
        //                SalesGrowthCurrentYear = 12.34,
        //                SalesGrowthNextYear = 23.56,
        //                NetIncomeGrowthCurrentYear = 17.897,
        //                NetIncomeGrowthNextYear = 19.56,
        //                NetDebtEquityCurrentYear = 21.876,
        //                FreeFlowCashMarginCurrentYear = -18.987
        //            });
        //        }

        //        for (int i = 0; i < 5; i++)
        //        {
        //            result.Add(new PortfolioDetailsData()
        //            {
        //                EntityTicker = "PBR/A US",
        //                EntityName = "PETROBRAS - PETROLEO BRASs",
        //                Type = "Security",
        //                Country = "USA",
        //                Shares = random.Next(20000, 50000),
        //                Price = random.Next(200, 700),
        //                Currency = "USD",
        //                Value = 1,
        //                TargetPerc = random.Next(10, 30),
        //                PortfolioPerc = random.Next(10, 30),
        //                BenchmarkPerc = random.Next(10, 30),
        //                BetPerc = random.Next(0, 20),
        //                Upside = random.Next(10, 30),
        //                YTDReturn = random.Next(0, 30),
        //                MarketCap = 1000000000,
        //                PE_FWD = 0.5,
        //                PE_Fair = 0.7,
        //                PBE_Fair = 0.23,
        //                PBE_FWD = 0.456,
        //                EVEBITDA_FWD = 2344786,
        //                EVEBITDA_Fair = 2277648,
        //                SalesGrowthCurrentYear = 12.34,
        //                SalesGrowthNextYear = 23.56,
        //                NetIncomeGrowthCurrentYear = 17.897,
        //                NetIncomeGrowthNextYear = 19.56,
        //                NetDebtEquityCurrentYear = 21.876,
        //                FreeFlowCashMarginCurrentYear = -18.987
        //            });
        //        }
        //        for (int i = 0; i < 5; i++)
        //        {
        //            result.Add(new PortfolioDetailsData()
        //            {
        //                EntityTicker = "MSCI US",
        //                EntityName = "Morgon Stanley Common Index USA",
        //                Type = "Index",
        //                Country = "USA",
        //                Shares = random.Next(20000, 50000),
        //                Price = random.Next(200, 700),
        //                Currency = "USD",
        //                Value = 1,
        //                TargetPerc = random.Next(10, 30),
        //                PortfolioPerc = random.Next(10, 30),
        //                BenchmarkPerc = random.Next(10, 30),
        //                BetPerc = random.Next(0, 20),
        //                Upside = random.Next(10, 30),
        //                YTDReturn = random.Next(0, 30),
        //                MarketCap = 500000000,
        //                PE_FWD = 0.45,
        //                PE_Fair = 0.17,
        //                PBE_Fair = 0.83,
        //                PBE_FWD = 0.856,
        //                EVEBITDA_FWD = 4344786,
        //                EVEBITDA_Fair = 8277648,
        //                SalesGrowthCurrentYear = 22.34,
        //                SalesGrowthNextYear = 17.56,
        //                NetIncomeGrowthCurrentYear = 9.897,
        //                NetIncomeGrowthNextYear = 2.56,
        //                NetDebtEquityCurrentYear = 8.876,
        //                FreeFlowCashMarginCurrentYear = -9.987
        //            });
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        #endregion

        #region Morning Snapshot Operation Contracts

        /// <summary>
        /// retrieving user preference for morning snapshot gadget
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>list of user benchmark preference</returns>
        [OperationContract]
        public List<UserBenchmarkPreference> RetrieveUserPreferenceBenchmarkData(string userName)
        {
            try
            {
                if (userName != null)
                {
                    ResearchEntities entity = new ResearchEntities();
                    List<UserBenchmarkPreference> userPreference = (entity.GetUserBenchmarkPreference(userName)).ToList<UserBenchmarkPreference>();
                    return userPreference;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// retrieving benchmark data for morning snapshot gadget based on user preference
        /// </summary>
        /// <param name="userBenchmarkPreference"></param>
        /// <returns>list of benchmark data for morning snapshot</returns>
        [OperationContract]
        public List<MorningSnapshotData> RetrieveMorningSnapshotData(List<UserBenchmarkPreference> userBenchmarkPreference)
        {

            try
            {
                List<MorningSnapshotData> result = new List<MorningSnapshotData>();
                foreach (UserBenchmarkPreference preference in userBenchmarkPreference)
                {
                    if (preference.BenchmarkName != null)
                    {
                        result.Add(new MorningSnapshotData()
                        {
                            MorningSnapshotPreferenceInfo = preference,
                            DTD = -0.1,
                            WTD = -0.1,
                            MTD = 4.4,
                            QTD = 4.4,
                            YTD = 7.4,
                            PreviousYearPrice = 4.6,
                            IIPreviousYearPrice = 52.3,
                            IIIPreviousYearPrice = -50.8
                        });
                    }
                    else
                    {
                        result.Add(new MorningSnapshotData()
                        {
                            MorningSnapshotPreferenceInfo = preference
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// adding user preferred groups in morning snapshot gadget
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        [OperationContract]
        public bool AddUserPreferenceBenchmarkGroup(string userName, string groupName)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.SetUserGroupPreference(userName, groupName);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// removing user preferred groups from morning snapshot gadget
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        [OperationContract]
        public bool RemoveUserPreferenceBenchmarkGroup(string userName, string groupname)
        {
            try
            {
                ResearchEntities entity = new ResearchEntities();
                entity.DeleteUserGroupPreference(userName, groupname);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// adding user preferred benchmarks in groups in morning snapshot gadget
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        [OperationContract]
        public bool AddUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.SetUserBenchmarkPreference(userName, userBenchmarkPreference.GroupName, userBenchmarkPreference.BenchmarkName, userBenchmarkPreference.BenchmarkReturnType);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// removing user preferred benchmarks from groups in morning snapshot gadget
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        [OperationContract]
        public bool RemoveUserPreferenceBenchmark(string userName, UserBenchmarkPreference userBenchmarkPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.DeleteUserBenchmarkPreference(userName, userBenchmarkPreference.GroupName, userBenchmarkPreference.BenchmarkName);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }
        
        #endregion

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

        #region Relative Performance
        [OperationContract]
        public List<RelativePerformanceSectorData> RetrieveRelativePerformanceSectorData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                List<RelativePerformanceSectorData> result = new List<RelativePerformanceSectorData>();
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new RelativePerformanceSectorData()
                    {
                        SectorID = row.Field<int>("GICS_SECTOR"),
                        SectorName = row.Field<string>("GICS_SECTOR_NAME")
                    });
                }
                result = result.Distinct().ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves Country Level Active Position Data for a particular composite/fund, benchmark and effective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceCountryActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null)
        {
            try
            {
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                List<string> countryCodes = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    countryCodes.Add(row.Field<string>("ISO_COUNTRY_CODE"));
                }
                countryCodes = countryCodes.Distinct().ToList();

                string query = "Select * From tblHoldingsData";
                string queryWhereCondition = String.Empty;

                if (countryID == null && sectorID == null)
                {
                    queryWhereCondition = String.Empty;
                }

                else if (countryID == null && sectorID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where GICS_SECTOR = " + sectorID.ToString();
                }

                else if (sectorID == null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where ISO_COUNTRY_CODE = '" + countryID + "'";
                }

                else if (sectorID != null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where ISO_COUNTRY_CODE = '" + countryID + "' And GICS_SECTOR = " + sectorID.ToString();
                }

                query = query + queryWhereCondition;
                dataTable = GetDataTable(query);
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
                    double MarketValue = 0;
                    double FundWeight = 0;
                    double BenchmarkWeight = 0;

                    record.Entity = countryCode.ToString();
                    DataTable countrySpecificData = new DataTable();
                    EnumerableRowCollection<DataRow> rowCollection = dataTable.AsEnumerable().Where(row => row.Field<string>("ISO_COUNTRY_CODE") == countryCode);
                    if (rowCollection.Count() > 0)
                    {
                        countrySpecificData = dataTable.AsEnumerable().Where(row => row.Field<string>("ISO_COUNTRY_CODE") == countryCode).CopyToDataTable();

                        foreach (DataRow row in countrySpecificData.Rows)
                        {
                            MarketValue = MarketValue + (double)(row.Field<Single?>("MARKET_CAP_IN_USD") == null ? 0 : row.Field<Single?>("MARKET_CAP_IN_USD"));
                            FundWeight = FundWeight + (double)(row.Field<Single?>("PORTFOLIO_WEIGHT") == null ? 0 : row.Field<Single?>("PORTFOLIO_WEIGHT") * 100);
                            BenchmarkWeight = BenchmarkWeight + (double)(row.Field<Single?>("BENCHMARK_WEIGHT") == null ? 0 : row.Field<Single?>("BENCHMARK_WEIGHT") * 100);
                        }

                        record.MarketValue = MarketValue;
                        record.FundWeight = FundWeight;
                        record.BenchmarkWeight = BenchmarkWeight;
                        record.ActivePosition = FundWeight - BenchmarkWeight;

                        result.Add(record);
                    }
                }

                return result.OrderByDescending(t => t.ActivePosition).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves Sector Level Active Position Data for a particular composite/fund, benchmark and effective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceSectorActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null)
        {
            try
            {
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                List<RelativePerformanceSectorData> sectorCodes = new List<RelativePerformanceSectorData>();
                foreach (DataRow row in dataTable.Rows)
                {
                    sectorCodes.Add(new RelativePerformanceSectorData()
                    {
                        SectorID = row.Field<int>("GICS_SECTOR"),
                        SectorName = row.Field<string>("GICS_SECTOR_NAME")
                    });
                }
                sectorCodes = sectorCodes.Distinct().ToList();

                string query = "Select * From tblHoldingsData";
                string queryWhereCondition = String.Empty;

                if (countryID == null && sectorID == null)
                {
                    queryWhereCondition = String.Empty;
                }

                else if (countryID == null && sectorID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where GICS_SECTOR = " + sectorID.ToString();
                }

                else if (sectorID == null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where ISO_COUNTRY_CODE = '" + countryID + "'";
                }

                else if (sectorID != null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where ISO_COUNTRY_CODE = '" + countryID + "' And GICS_SECTOR = " + sectorID.ToString();
                }

                query = query + queryWhereCondition;
                dataTable = GetDataTable(query);
                List<RelativePerformanceActivePositionData> result = new List<RelativePerformanceActivePositionData>();

                foreach (RelativePerformanceSectorData sector in sectorCodes)
                {
                    if (sectorID != null)
                    {
                        if (!sector.SectorID.Equals(sectorID))
                        {
                            continue;
                        }
                    }

                    RelativePerformanceActivePositionData record = new RelativePerformanceActivePositionData();
                    double MarketValue = 0;
                    double FundWeight = 0;
                    double BenchmarkWeight = 0;

                    record.Entity = sector.SectorName.ToString();
                    DataTable sectorSpecificData = new DataTable();
                    EnumerableRowCollection<DataRow> rowCollection = dataTable.AsEnumerable().Where(row => row.Field<int>("GICS_SECTOR") == sector.SectorID);
                    if (rowCollection.Count() > 0)
                    {
                        sectorSpecificData = dataTable.AsEnumerable().Where(row => row.Field<int>("GICS_SECTOR") == sector.SectorID).CopyToDataTable();

                        foreach (DataRow row in sectorSpecificData.Rows)
                        {
                            MarketValue = MarketValue + (double)(row.Field<Single?>("MARKET_CAP_IN_USD") == null ? 0 : row.Field<Single?>("MARKET_CAP_IN_USD"));
                            FundWeight = FundWeight + (double)(row.Field<Single?>("PORTFOLIO_WEIGHT") == null ? 0 : row.Field<Single?>("PORTFOLIO_WEIGHT") * 100);
                        BenchmarkWeight = BenchmarkWeight + (double)(row.Field<Single?>("BENCHMARK_WEIGHT") == null ? 0 : row.Field<Single?>("BENCHMARK_WEIGHT") * 100);                        
                        }

                        record.MarketValue = MarketValue;
                        record.FundWeight = FundWeight;
                        record.BenchmarkWeight = BenchmarkWeight;
                        record.ActivePosition = FundWeight - BenchmarkWeight;

                        result.Add(record);
                    }
                }

                return result.OrderByDescending(t => t.ActivePosition).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves Security Level Active Position Data for a particular composite/fund, benchmark and effective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceSecurityActivePositionData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null)
        {
            try
            {
                string query = "Select * From tblHoldingsData";
                string queryWhereCondition = String.Empty;

                if (countryID == null && sectorID == null)
                {
                    queryWhereCondition = String.Empty;
                }

                else if (countryID == null && sectorID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where GICS_SECTOR = " + sectorID.ToString();
                }

                else if (sectorID == null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where ISO_COUNTRY_CODE = '" + countryID + "'";
                }

                else if (sectorID != null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + " Where ISO_COUNTRY_CODE = '" + countryID + "' And GICS_SECTOR = " + sectorID.ToString();
                }

                query = query + queryWhereCondition;

                DataTable dataTable = GetDataTable(query);
                List<RelativePerformanceActivePositionData> result = new List<RelativePerformanceActivePositionData>();

                foreach (DataRow row in dataTable.Rows)
                {
                    double? fundWeight = (double?)(row.Field<Single?>("PORTFOLIO_WEIGHT") != null ? row.Field<Single?>("PORTFOLIO_WEIGHT") * 100 : null);
                    double? benchmarkWeight = (double?)(row.Field<Single?>("BENCHMARK_WEIGHT") != null ? row.Field<Single?>("BENCHMARK_WEIGHT") * 100 : null);
                    double? activePosition = null;
                    if (fundWeight == null && benchmarkWeight != null)
                        activePosition = benchmarkWeight * -1;
                    else if (fundWeight != null && benchmarkWeight == null)
                        activePosition = fundWeight;
                    else if (fundWeight != null && benchmarkWeight != null)
                        activePosition = fundWeight - benchmarkWeight;

                    result.Add(new RelativePerformanceActivePositionData()
                    {
                        Entity = row.Field<string>("ISSUE_NAME"),
                        MarketValue = (double?)(row.Field<Single?>("MARKET_CAP_IN_USD")),
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
                return null;
            }
        }

        /// <summary>
        /// Retrieves Security Level Relative Performance Data for a particular composite/fund, benchmark and efective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <param name="order">(optional)1 for Ascending - data ordering - By default descending</param>
        /// <param name="maxRecords">(optional) Maximum number of records to be retrieved - By default Null</param>
        /// <returns>List of RetrieveRelativePerformanceSecurityData objects</returns>
        [OperationContract]
        public List<RelativePerformanceSecurityData> RetrieveRelativePerformanceSecurityData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null)
        {


            try
            {
                DataTable dataTable = new DataTable();
                string query = "Select " + (maxRecords == null ? "*" : "Top " + maxRecords.ToString() + " *") + " From tblHoldingsData ";
                string queryWhereCondition = "Where ";

                if (countryID == null && sectorID == null)
                {
                    queryWhereCondition = String.Empty;
                }

                else if (countryID == null && sectorID != null)
                {
                    queryWhereCondition = queryWhereCondition + "GICS_SECTOR = " + sectorID.ToString();
                }

                else if (sectorID == null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + "ISO_COUNTRY_CODE = '" + countryID + "'";
                }

                else if (sectorID != null && countryID != null)
                {
                    queryWhereCondition = queryWhereCondition + "ISO_COUNTRY_CODE = '" + countryID + "' And GICS_SECTOR = " + sectorID.ToString();
                }

                query = query + queryWhereCondition + " Order By DIRTY_VALUE_PC " + (order == 1 ? "Asc" : "Desc");


                dataTable = GetDataTable(query);

                int alpha = 2;
                List<RelativePerformanceSecurityData> result = new List<RelativePerformanceSecurityData>();
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new RelativePerformanceSecurityData()
                    {
                        SecurityName = row.Field<string>("ISSUE_NAME"),
                        SecurityCountryID = row.Field<string>("ISO_COUNTRY_CODE"),
                        SecuritySectorName = row.Field<string>("GICS_SECTOR_NAME"),
                        SecurityAlpha = alpha++,
                        SecurityActivePosition = (double)
                        (row.Field<Single?>("PORTFOLIO_WEIGHT") == null ? 0 : row.Field<Single?>("PORTFOLIO_WEIGHT") * 100
                        - row.Field<Single?>("BENCHMARK_WEIGHT") == null ? 0 : row.Field<Single?>("BENCHMARK_WEIGHT") * 100)
                    });
                }
                return order == 1 ? result.OrderBy(e => e.SecurityAlpha).ToList() : result.OrderByDescending(e => e.SecurityAlpha).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
        
        [OperationContract]
        public List<RelativePerformanceData> RetrieveRelativePerformanceData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            try
            {
                DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                List<string> countryCodes = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    countryCodes.Add(row.Field<string>("ISO_COUNTRY_CODE"));
                }
                countryCodes = countryCodes.Distinct().ToList();

                List<RelativePerformanceSectorData> sectors = new List<RelativePerformanceSectorData>();
                foreach (DataRow row in dataTable.Rows)
                {
                    sectors.Add(new RelativePerformanceSectorData()
                    {
                        SectorID = row.Field<int>("GICS_SECTOR"),
                        SectorName = row.Field<string>("GICS_SECTOR_NAME")
                    });
                }
                sectors = sectors.Distinct().ToList();


                List<RelativePerformanceData> result = new List<RelativePerformanceData>();
                foreach (string countryCode in countryCodes)
                {
                    double? aggcsAlpha = 0.0;
                    double? aggcsPortfolioShare = 0.0;
                    double? aggcsBenchmarkShare = 0.0;
                    List<RelativePerformanceCountrySpecificData> sectorSpecificData = new List<RelativePerformanceCountrySpecificData>();
                    foreach (RelativePerformanceSectorData sectorData in sectors)
                    {
                        double? aggAlpha = 0.0;
                        double? aggPortfolioShare = 0.0;
                        double? aggBenchmarkShare = 0.0;
                        DataTable specificData = GetDataTable("Select * from tblHoldingsData where ISO_COUNTRY_CODE = '" + countryCode + "' and GICS_SECTOR = " + sectorData.SectorID.ToString());


                        foreach (DataRow row in specificData.Rows)
                        {
                            if (row.Field<Single?>("BENCHMARK_WEIGHT") != null)
                            {
                                aggPortfolioShare = aggPortfolioShare + (double)row.Field<Single>("PORTFOLIO_WEIGHT") * 100;
                                aggBenchmarkShare = aggBenchmarkShare + (double)row.Field<Single>("BENCHMARK_WEIGHT") * 100;
                                aggAlpha = aggAlpha + 2;
                            }
                        }

                        if (aggPortfolioShare > 0 || aggBenchmarkShare > 0)
                        {
                            sectorSpecificData.Add(new RelativePerformanceCountrySpecificData()
                                        {
                                            SectorID = sectorData.SectorID,
                                            SectorName = sectorData.SectorName,
                                            Alpha = aggAlpha,
                                            PortfolioShare = aggPortfolioShare,
                                            BenchmarkShare = aggBenchmarkShare,
                                            ActivePosition = aggPortfolioShare - aggBenchmarkShare,
                                        });
                        }
                        else
                        {
                            sectorSpecificData.Add(new RelativePerformanceCountrySpecificData()
                            {
                                SectorID = sectorData.SectorID,
                                SectorName = sectorData.SectorName,
                                Alpha = null,
                                PortfolioShare = null,
                                BenchmarkShare = null,
                                ActivePosition = null,
                            });
                        }

                        aggcsAlpha = aggcsAlpha + aggAlpha;
                        aggcsPortfolioShare = aggcsPortfolioShare + aggPortfolioShare;
                    aggcsBenchmarkShare = aggcsBenchmarkShare + aggBenchmarkShare;                    
                    }

                    if (sectorSpecificData.Count > 0)
                    {
                        result.Add(new RelativePerformanceData()
                        {
                            CountryID = countryCode,
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
                return null;
            }
        }
      
        /// <summary>
        /// Retrieves Performance graph data for a particular composite/fund.
        /// Filtering data based on the fund name.
        /// </summary>
        /// <param name="nameOfFund">Name of the selected fund</param>
        /// <returns></returns>
        [OperationContract]
        public List<PerformanceGraphData> RetrievePerformanceGraphData(String nameOfFund)
        {
            List<PerformanceGraphData> result = new List<PerformanceGraphData>();
            try
            {
                if (nameOfFund != null)
                {
                    List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                    PerformanceGraphData entry = new PerformanceGraphData();
                    ResearchEntities research = new ResearchEntities();
                    holdingData = research.tblHoldingsDatas.ToList();
                    result.Add(new PerformanceGraphData() { PORTFOLIO_ID = "P1", BENCHMARK_ID = "B1", PORTFOLIO_PERFORMANCE = 33.3, BENCHMARK_PERFORMANCE = 58.6, EFFECTIVE_DATE = new DateTime(2011, 12, 31), MTD = 23, QTD = 29, YTD = 13, FIRST_YEAR = 12, THIRD_YEAR = 10, FIFTH_YEAR = 07, TENTH_YEAR = 19 });
                    result.Add(new PerformanceGraphData() { PORTFOLIO_ID = "P2", BENCHMARK_ID = "B2", PORTFOLIO_PERFORMANCE = 38.3, BENCHMARK_PERFORMANCE = 68.6, EFFECTIVE_DATE = new DateTime(2011, 10, 14), MTD = 13, QTD = 19, YTD = 23, FIRST_YEAR = 15, THIRD_YEAR = 17, FIFTH_YEAR = 09, TENTH_YEAR = 39 });
                    result.Add(new PerformanceGraphData() { PORTFOLIO_ID = "P3", BENCHMARK_ID = "B3", PORTFOLIO_PERFORMANCE = 31.5, BENCHMARK_PERFORMANCE = 53.9, EFFECTIVE_DATE = new DateTime(2011, 09, 13), MTD = 24, QTD = 28, YTD = 19, FIRST_YEAR = 15, THIRD_YEAR = 11, FIFTH_YEAR = 16, TENTH_YEAR = 19 });
                    result.Add(new PerformanceGraphData() { PORTFOLIO_ID = "P4", BENCHMARK_ID = "B4", PORTFOLIO_PERFORMANCE = 39.9, BENCHMARK_PERFORMANCE = 78.6, EFFECTIVE_DATE = new DateTime(2011, 08, 29), MTD = 25, QTD = 26, YTD = 15, FIRST_YEAR = 13, THIRD_YEAR = 10, FIFTH_YEAR = 07, TENTH_YEAR = 19 });
                }
                return result;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
      
        /// <summary>
        /// Retrieves Performance grid data for a particular composite/fund.
        /// Filtering data based on the fund name.
        /// </summary>
        /// <param name="nameOfFund">Name of the selected fund</param>
        /// <returns></returns>
        [OperationContract]
        public List<PerformanceGridData> RetrievePerformanceGridData(String nameOfFund)
        {
            List<PerformanceGridData> result = new List<PerformanceGridData>();
            try
            {
                if (nameOfFund != null)
                {
                    List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                    PerformanceGridData entry = new PerformanceGridData();
                    ResearchEntities research = new ResearchEntities();
                    holdingData = research.tblHoldingsDatas.ToList();
                    result.Add(new PerformanceGridData() { MTD = 23, QTD = 29, YTD = 13, FIRST_YEAR = 12, THIRD_YEAR = 10, FIFTH_YEAR = 07, TENTH_YEAR = 19 });
                    result.Add(new PerformanceGridData() { MTD = 13, QTD = 19, YTD = 23, FIRST_YEAR = 15, THIRD_YEAR = 17, FIFTH_YEAR = 09, TENTH_YEAR = 39 });
                    result.Add(new PerformanceGridData() { MTD = 24, QTD = 28, YTD = 19, FIRST_YEAR = 15, THIRD_YEAR = 11, FIFTH_YEAR = 16, TENTH_YEAR = 19 });
                    result.Add(new PerformanceGridData() { MTD = 25, QTD = 26, YTD = 15, FIRST_YEAR = 13, THIRD_YEAR = 10, FIFTH_YEAR = 07, TENTH_YEAR = 19 });
                }
                return result;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }

        }

        /// <summary>
        /// Retrieves Attribution Data for a particular composite/fund
        /// Filtering data based on fund name
        /// </summary>
        /// <param name="nameOfFund">Name of the selected fund</param>
        /// <returns></returns>
        [OperationContract]
        public List<AttributionData> RetrieveAttributionData(String nameOfFund)
        {
            List<AttributionData> result = new List<AttributionData>();
            try
            {
                if (nameOfFund != null)
                {
                    List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                    AttributionData entry = new AttributionData();
                    ResearchEntities research = new ResearchEntities();
                    holdingData = research.tblHoldingsDatas.ToList();

                    foreach (tblHoldingsData d in holdingData)
                    {
                        entry = new AttributionData();
                        entry.COUNTRY_ID = d.ISO_COUNTRY_CODE;
                        entry.PORTFOLIO_WEIGHT = Convert.ToDouble(d.PORTFOLIO_WEIGHT);
                        entry.BENCHMARK_WEIGHT = Convert.ToDouble(d.BENCHMARK_WEIGHT);
                        result.Add(entry);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
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
        public List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(EntitySelectionData entityIdentifier, DateTime startDateTime, DateTime endDateTime, string frequencyInterval)
        {
            try
            {
                List<UnrealizedGainLossData> result = new List<UnrealizedGainLossData>();

                if (entityIdentifier == null || entityIdentifier.ShortName == null || endDateTime < startDateTime)
                    return result;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                List<DimensionEntitiesService.GF_PRICING_BASEVIEW> resultSet
                    = entity.GF_PRICING_BASEVIEW
                        .Where(record => (record.TICKER == entityIdentifier.ShortName))
                        .OrderByDescending(record => record.FROMDATE)
                        .ToList();

                int noOfRows = resultSet.Count();

                if (noOfRows < 90)
                    return result;

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
                return null;
            }
        }
        #endregion

        #region HoldingPieChart Operation Contracts
        /// <summary>
        /// Retrieves Holdings data for showing pie chart for sector allocation
        /// </summary>
        /// <param name="fundSelectionData">Contains Selected Fund Data</param>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        /// <param name="filterType">The Filter type selected by the user</param>
        /// <param name="filterValue">The Filter value selected by the user</param>
        /// <returns>List of HoldingsPercentageData </returns>
        [OperationContract]
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(FundSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue)
        {
           
            try
            {
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                HoldingsPercentageData entry = new HoldingsPercentageData();
                ResearchEntities research = new ResearchEntities();
                holdingData = research.tblHoldingsDatas.ToList();
            Double sumForBenchmarks = 0;
            Double sumForPortfolios = 0;

                switch (filterType)
                {
                    case "Region":
                        var q = from p in holdingData
                                where (p.ASHEMM_PROPRIETARY_REGION_CODE.ToString()).Equals(filterValue)
                                group p by p.GICS_SECTOR_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };
                    
                        foreach (var a in q)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in q)
                        {
                        if (sumForBenchmarks == 0)
                        continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                        }
                        break;
                    case "Country":
                        var l = from p in holdingData
                                where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                group p by p.GICS_SECTOR_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };

                     foreach (var a in l)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in l)
                        {
                        if (sumForBenchmarks == 0)
                            continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                    }                    
                        break;
                    case "Industry":
                        var m = from p in holdingData
                                where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                group p by p.GICS_SECTOR_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };
                    
                        foreach (var a in m)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in m)
                        {
                        if (sumForBenchmarks == 0)
                        continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                    }  
                        break;
                    case "Sector":
                        var n = from p in holdingData
                                where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                            group p by p.GICS_INDUSTRY_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };
                    
                        foreach (var a in n)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in n)
                        {
                        if (sumForBenchmarks == 0)
                        continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                    }  
                    
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves Holdings data for showing pie chart for region allocation
        /// </summary>
        /// <param name="fundSelectionData">Contains Selected Fund Data</param>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        /// <param name="filterType">The Filter type selected by the user</param>
        /// <param name="filterValue">The Filter value selected by the user</param>
        /// <returns>List of HoldingsPercentageData </returns>
        [OperationContract]
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageDataForRegion(FundSelectionData fundSelectionData, DateTime effectiveDate, String filterType, String filterValue)
        {
            try
            {
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                HoldingsPercentageData entry = new HoldingsPercentageData();
                ResearchEntities research = new ResearchEntities();
                holdingData = research.tblHoldingsDatas.ToList();
            Double sumForBenchmarks = 0;
            Double sumForPortfolios = 0;

                switch (filterType)
                {
                    case "Region":
                        var q = from p in holdingData
                                where (p.ASHEMM_PROPRIETARY_REGION_CODE.ToString()).Equals(filterValue)
                                group p by p.ISO_COUNTRY_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };
                    
                        foreach (var a in q)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in q)
                        {
                        if (sumForBenchmarks == 0)
                            continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                        }
                        break;
                    case "Country":
                        var l = from p in holdingData
                                where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                group p by p.ASHEMM_PROPRIETARY_REGION_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };

                   
                        foreach (var a in l)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in l)
                        {
                        if (sumForBenchmarks == 0)
                            continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                        }
                        break;
                    case "Industry":
                        var m = from p in holdingData
                                where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                group p by p.ASHEMM_PROPRIETARY_REGION_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };
                   
                        foreach (var a in m)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in m)
                        {
                        if (sumForBenchmarks == 0)
                            continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                        }
                        break;
                    case "Sector":
                        var n = from p in holdingData
                                where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                                group p by p.ASHEMM_PROPRIETARY_REGION_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.PORTFOLIO_WEIGHT) };
                   
                        foreach (var a in n)
                        {
                        CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in n)
                        {
                        if (sumForBenchmarks == 0)
                        continue;
                        CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                        }
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

       /// <summary>
       /// Calculates total of Benchmark Weight and total Portfolio Weight
       /// </summary>
       /// <param name="sumForBenchmarks">Stores the sum of Benchmark Weight</param>
       /// <param name="sumForPortfolios">Stores the sum of Portfolio Weight</param>
       /// <param name="a">Benchmark Weight</param>
       /// <param name="b">Portfolio Weight</param>
        public void CalculatesTotalSumForBenchmark(ref Double sumForBenchmarks, ref Double sumForPortfolios,float? a,float? b)
        {
            sumForBenchmarks = sumForBenchmarks + Convert.ToDouble(a);
            sumForPortfolios = sumForPortfolios + Convert.ToDouble(b);
        }

        /// <summary>
        /// Calculates the percentage contribution for Benchmark and Portfolio.
        /// </summary>
        /// <param name="entry">Object of type HoldingsPercentageData</param>
        /// <param name="sumForBenchmarks">Contains sum of Benchmark Weights</param>
        /// <param name="sumForPortfolios">Contains sum of Benchmark Weights</param>
        /// <param name="name">Contains the name of the segment</param>
        /// <param name="a">Benchmark Weight</param>
        /// <param name="b">Portfolio Weight</param>
        /// <param name="result">List of HoldingsPercentageData </param>
        public void CalculatesPercentageForBenchmark(HoldingsPercentageData entry, Double sumForBenchmarks, Double sumForPortfolios, String name, float? a, float? b, ref List<HoldingsPercentageData> result)
        {
            entry = new HoldingsPercentageData();
            entry.SegmentName = name;            
            entry.BenchmarkWeight = (Convert.ToDouble(a) / sumForBenchmarks) * 100;
            entry.PortfolioWeight = (Convert.ToDouble(b) / sumForPortfolios) * 100;                     
            result.Add(entry);
        }

        #endregion

        #region Benchmark

        /// <summary>
        /// Retrieves Top Benchmark Securities data 
        /// </summary>
        /// <param name="benchmarkSelectionData">Contains Selected Benchmark Data </param>
        /// <param name="effectiveDate">Effective Date selected by user</param>
        /// <returns>returns list of Top Ten Benchmarks </returns>
        [OperationContract]
        public List<TopBenchmarkSecuritiesData> RetrieveTopBenchmarkSecuritiesData(BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {
            List<TopBenchmarkSecuritiesData> result = new List<TopBenchmarkSecuritiesData>();
            List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
            List<tblHoldingsData> top10HoldingData = new List<tblHoldingsData>();
            TopBenchmarkSecuritiesData entry = new TopBenchmarkSecuritiesData();
            ResearchEntities research = new ResearchEntities();
            holdingData = research.tblHoldingsDatas.ToList();
            top10HoldingData = (from p in holdingData orderby p.BENCHMARK_WEIGHT descending select p).Take(10).ToList();

            foreach (tblHoldingsData item in top10HoldingData)
            {
                entry = new TopBenchmarkSecuritiesData();
                entry.Weight = Convert.ToDouble(item.BENCHMARK_WEIGHT);
                entry.IssuerName = item.ISSUE_NAME;
                result.Add(entry);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Retrieves Portfolio Risk Return Data
        /// </summary>
        /// <param name="fundSelectionData">Contains Selected Fund Data</param>
        /// <param name="benchmarkSelectionData">Contains Selected Benchmark Data </param>
        /// <param name="effectiveDate">Effective Date selected by user</param>
        /// <returns>returns List of PortfolioRiskReturnData containing Portfolio Risk Return Data</returns>
        [OperationContract]
        public List<PortfolioRiskReturnData> RetrievePortfolioRiskReturnData(FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
        {            
            try
            {
                List<PortfolioRiskReturnData> portfolioRiskReturnValues = new List<PortfolioRiskReturnData>();

                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Expected Return",
                    PortfolioValue = 18.1.ToString(),
                    BenchMarkValue = 15.3.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Alpha",
                    PortfolioValue = 1.8.ToString(),
                    BenchMarkValue = "N/A"
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Beta",
                    PortfolioValue = 0.95.ToString(),
                    BenchMarkValue = "N/A"
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Standard Deviation",
                    PortfolioValue = 15.1.ToString(),
                    BenchMarkValue = 15.7.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Sharpe Ratio",
                    PortfolioValue = 0.18.ToString(),
                    BenchMarkValue = 0.13.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Information Ratio",
                    PortfolioValue = 1.81.ToString()
                });
                portfolioRiskReturnValues.Add(new PortfolioRiskReturnData()
                {
                    DataPointName = "Turnover Ratio",
                    PortfolioValue = 11.14.ToString()
                });
                return portfolioRiskReturnValues;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        #region Heat Map Data Contract
        [OperationContract]
        public List<HeatMapData> RetrieveHeatMapData()
        {
            List<HeatMapData> result = new List<HeatMapData>();             
            HeatMapData entry = new HeatMapData();
            ResearchEntities research = new ResearchEntities();
            result.Add(new HeatMapData() { CountryID = "RU", CountryPerformance = PerformanceGrade.OVER_PERFORMING, CountryYTD = 90 });
            result.Add(new HeatMapData() { CountryID = "IN", CountryPerformance = PerformanceGrade.OVER_PERFORMING, CountryYTD = 95 });
            result.Add(new HeatMapData() { CountryID = "AF", CountryPerformance = PerformanceGrade.FLAT_PERFORMING, CountryYTD = 10 });
            result.Add(new HeatMapData() { CountryID = "AU", CountryPerformance = PerformanceGrade.UNDER_PERFORMING, CountryYTD = 20 });
            return result;         
        }         
        #endregion

            
        }       
    }

