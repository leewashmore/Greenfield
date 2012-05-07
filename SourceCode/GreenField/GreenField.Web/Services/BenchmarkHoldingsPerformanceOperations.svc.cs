using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using GreenField.Web.Helpers;
using GreenField.DAL;
using System.ServiceModel.Activation;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using System.Data.Objects;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BenchmarkHoldingsPerformanceOperations
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
        public void Temp(PeriodSelectionData data)
        {
        }

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PortfolioSelectionData> RetrievePortfolioSelectionData()
        {
            try
            {
                List<PortfolioSelectionData> result = new List<PortfolioSelectionData>();

                DimensionEntitiesService.Entities entity = DimensionEntity;

                List<GF_PORTFOLIO_SELECTION> dimensionFundList = entity.GF_PORTFOLIO_SELECTION.ToList();

                foreach (GF_PORTFOLIO_SELECTION item in dimensionFundList)
                {
                    PortfolioSelectionData data = new PortfolioSelectionData();
                    data.PortfolioId = item.PORTFOLIO_ID;
                    data.PortfolioThemeSubGroupId = item.PORTFOLIO_THEME_SUBGROUP_CODE;
                    data.PortfolioThemeSubGroupName = item.PORTFOLIO_THEME_SUBGROUP_NAME;
                    result.Add(data);
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// retrieving data for sector breakdown gadget
        /// </summary>
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of sector breakdown data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<SectorBreakdownData> RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<SectorBreakdownData> result = new List<SectorBreakdownData>();

                List<GF_PORTFOLIO_HOLDINGS> data = entity.GF_PORTFOLIO_HOLDINGS
                    .Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                        && record.PORTFOLIO_DATE == effectiveDate.Date)
                        .ToList();

                if (data.Count.Equals(0))
                    return result;

                Decimal? netPortfolioValuation = data.Sum(record => Convert.ToDecimal(record.DIRTY_VALUE_PC));

                if (netPortfolioValuation == 0 || netPortfolioValuation == null)
                    throw new InvalidOperationException();

                //Retrieve the Id of benchmark associated with the Portfolio
                List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                //If the DataBase doesn't return a single Benchmark for a Portfolio
                if (benchmarkId.Count != 1)
                    throw new InvalidOperationException();

                List<GF_BENCHMARK_HOLDINGS> benchmarkData = entity.GF_BENCHMARK_HOLDINGS.
                    Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();


                foreach (GF_PORTFOLIO_HOLDINGS record in data)
                {
                    if (record.DIRTY_VALUE_PC == null)
                        continue;

                    //Calculate Portfolio Weight
                    decimal? portfolioWeight = record.DIRTY_VALUE_PC / netPortfolioValuation;

                    //Retrieve Benchmark Weight 
                    decimal? benchmarkWeight = Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == record.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT).FirstOrDefault());

                    //Calculate Active Position
                    decimal? activePosition = portfolioWeight - benchmarkWeight;

                    result.Add(new SectorBreakdownData()
                    {
                        Sector = record.GICS_SECTOR_NAME,
                        Industry = record.GICS_INDUSTRY_NAME,
                        Security = record.ISSUE_NAME,
                        PortfolioShare = portfolioWeight,
                        BenchmarkShare = benchmarkWeight,
                        ActivePosition = activePosition
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
        /// retrieving data for region breakdown gadget
        /// </summary>
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of region breakdown data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RegionBreakdownData> RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<RegionBreakdownData> result = new List<RegionBreakdownData>();

                List<GF_PORTFOLIO_HOLDINGS> data = entity.GF_PORTFOLIO_HOLDINGS
                    .Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                        && record.PORTFOLIO_DATE == effectiveDate.Date)
                        .ToList();

                if (data.Count.Equals(0))
                    return result;

                Decimal? netPortfolioValuation = data.Sum(record => Convert.ToDecimal(record.DIRTY_VALUE_PC));

                if (netPortfolioValuation == 0 || netPortfolioValuation == null)
                    throw new InvalidOperationException();

                //Retrieve the Id of benchmark associated with the Portfolio
                List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                //If the DataBase doesn't return a single Benchmark for a Portfolio
                if (benchmarkId.Count != 1)
                    throw new InvalidOperationException();

                List<GF_BENCHMARK_HOLDINGS> benchmarkData = entity.GF_BENCHMARK_HOLDINGS.
                    Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                foreach (GF_PORTFOLIO_HOLDINGS record in data)
                {
                    if (record.DIRTY_VALUE_PC == null)
                        continue;

                    //Calculate Portfolio Weight
                    decimal? portfolioWeight = record.DIRTY_VALUE_PC / netPortfolioValuation;

                    //Retrieve Benchmark Weight
                    decimal? benchmarkWeight = Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == record.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT).FirstOrDefault());

                    //Calculate Active Position
                    decimal? activePosition = portfolioWeight - benchmarkWeight;

                    result.Add(new RegionBreakdownData()
                    {
                        Region = record.ASHEMM_PROP_REGION_NAME,
                        Country = record.COUNTRYNAME,
                        Security = record.ISSUE_NAME,
                        PortfolioShare = portfolioWeight,
                        BenchmarkShare = benchmarkWeight,
                        ActivePosition = activePosition
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
        /// retrieving  data for TopHoldings gadget
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of top holdings data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<TopHoldingsData> RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                List<TopHoldingsData> result = new List<TopHoldingsData>();

                //get the summation of DIRTY_VALUE_PC used to calculate the holding's PortfolioShare
                decimal sumMarketValuePortfolio = DimensionEntity.GF_PORTFOLIO_HOLDINGS
                    .Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                        && t.PORTFOLIO_DATE == effectiveDate.Date)
                    .ToList()
                    .Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));

                //if sum of DIRTY_VALUE_PC for criterion is zero, empty set is returned
                if (sumMarketValuePortfolio == 0)
                    return result;

                //Retrieve GF_PORTFOLIO_HOLDINGS data for top ten holdings based on DIRTY_VALUE_PC
                List<GF_PORTFOLIO_HOLDINGS> data = DimensionEntity.GF_PORTFOLIO_HOLDINGS
                    .Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                        && record.PORTFOLIO_DATE == effectiveDate.Date)
                    .OrderByDescending(record => record.DIRTY_VALUE_PC)
                    .Take(10)
                    .ToList();

                if (data == null)
                    throw new InvalidOperationException(ServiceFaultResourceManager.GetString("ServiceNullResultSet").ToString());

                foreach (GF_PORTFOLIO_HOLDINGS record in data)
                {
                    //Calculate Portfolio Weight
                    decimal? portfolioWeight = record.DIRTY_VALUE_PC / sumMarketValuePortfolio;

                    //Calculate Benchmark Weight - if null look for data in GF_BENCHMARK_HOLDINGS
                    GF_BENCHMARK_HOLDINGS specificHolding = DimensionEntity.GF_BENCHMARK_HOLDINGS
                            .Where(rec => rec.TICKER == record.TICKER)
                            .FirstOrDefault();
                    decimal? benchmarkWeight = specificHolding != null ? Convert.ToDecimal(specificHolding.BENCHMARK_WEIGHT) : Convert.ToDecimal(null);


                    //Calculate Active Position
                    decimal? activePosition = portfolioWeight - benchmarkWeight;

                    result.Add(new TopHoldingsData()
                    {
                        Ticker = record.TICKER,
                        Holding = record.PORTFOLIO_ID,
                        MarketValue = record.DIRTY_VALUE_PC,
                        PortfolioShare = portfolioWeight,
                        BenchmarkShare = benchmarkWeight,
                        ActivePosition = activePosition
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
        /// retrieving  data for index constituent gadget
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of index constituents data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<IndexConstituentsData> RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {

                if (portfolioSelectionData == null || effectiveDate == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<IndexConstituentsData> result = new List<IndexConstituentsData>();

                GF_PORTFOLIO_HOLDINGS benchmarkRow = DimensionEntity.GF_PORTFOLIO_HOLDINGS
                    .Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                        && t.PORTFOLIO_DATE.Equals(effectiveDate.Date))
                    .FirstOrDefault();

                //Return empty set if PORTFOLIO_ID and PORTFOLIO_DATE combination does not exist
                if (benchmarkRow == null)
                    return result;

                string benchmarkId = benchmarkRow.BENCHMARK_ID;

                if (benchmarkId != null)
                {
                    List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> data = entity.GF_BENCHMARK_HOLDINGS
                        .Where(t => (t.BENCHMARK_ID == benchmarkId) && (t.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                    if (data != null)
                    {
                        foreach (DimensionEntitiesService.GF_BENCHMARK_HOLDINGS record in data)
                        {
                            //calculte sum of BENCHMARK_WEIGHT for a country
                            string country = record.COUNTRYNAME;
                            object sumBenchmarkWeightCountry = data.Where(t => t.COUNTRYNAME == country).Sum(t => t.BENCHMARK_WEIGHT);

                            //calculte sum of BENCHMARK_WEIGHT for a industry
                            string industry = record.GICS_INDUSTRY_NAME;
                            object sumBenchmarkWeightIndustry = data.Where(t => t.GICS_INDUSTRY_NAME == industry && t.COUNTRYNAME == country).Sum(t => t.BENCHMARK_WEIGHT);
                            if (sumBenchmarkWeightCountry != null && sumBenchmarkWeightIndustry != null)
                            {
                                result.Add(new IndexConstituentsData()
                                {
                                    ConstituentName = record.ISSUE_NAME,
                                    Country = country + " (" + record.ISO_COUNTRY_CODE + ")",
                                    Region = record.ASHEMM_PROP_REGION_CODE,
                                    Sector = record.GICS_SECTOR_NAME,
                                    Industry = industry,
                                    SubIndustry = record.GICS_SUB_INDUSTRY_NAME,
                                    Weight = record.BENCHMARK_WEIGHT,
                                    WeightCountry = (record.BENCHMARK_WEIGHT) / (decimal?)sumBenchmarkWeightCountry,
                                    WeightIndustry = (record.BENCHMARK_WEIGHT) / (decimal?)sumBenchmarkWeightIndustry
                                });
                            }
                        }
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
        /// Retrieves the filter values for a selected filter type
        /// </summary>
        /// <param name="filterType">Filter Type seleted by the user</param>
        /// <param name="effectiveDate">Effective Date selected by the user </param>
        /// <returns>HoldingsFilterSelectionData Object</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FilterSelectionData> RetrieveFilterSelectionData(DateTime? effectiveDate)
        {
            try
            {
                List<FilterSelectionData> result = new List<FilterSelectionData>();

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> data = DimensionEntity.GF_PORTFOLIO_HOLDINGS
                    .Where(t => t.PORTFOLIO_DATE == effectiveDate.Value.Date)
                    .ToList();

                List<FilterSelectionData> distinctRegions = data
                            .Select(record => new FilterSelectionData() { Filtertype = "Region", FilterValues = record.ASHEMM_PROP_REGION_CODE == null ? String.Empty : record.ASHEMM_PROP_REGION_CODE })
                            .Distinct()
                            .OrderBy(record => record.FilterValues)
                            .ToList();
                result.AddRange(distinctRegions);

                List<FilterSelectionData> distinctCountries = data
                    .Select(record => new FilterSelectionData() { Filtertype = "Country", FilterValues = record.ISO_COUNTRY_CODE == null ? String.Empty : record.ISO_COUNTRY_CODE })
                    .Distinct()
                    .OrderBy(record => record.FilterValues)
                    .ToList();
                result.AddRange(distinctCountries);

                List<FilterSelectionData> distinctSectors = data
                    .Select(record => new FilterSelectionData() { Filtertype = "Sector", FilterValues = record.GICS_SECTOR_NAME == null ? String.Empty : record.GICS_SECTOR_NAME })
                    .Distinct()
                    .OrderBy(record => record.FilterValues)
                    .ToList();
                result.AddRange(distinctSectors);

                List<FilterSelectionData> distinctIndustries = data
                    .Select(record => new FilterSelectionData() { Filtertype = "Industry", FilterValues = record.GICS_INDUSTRY_NAME == null ? String.Empty : record.GICS_INDUSTRY_NAME })
                    .Distinct()
                    .OrderBy(record => record.FilterValues)
                    .ToList();
                result.AddRange(distinctIndustries);

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region Build2 Services

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PortfolioDetailsData> RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime effectiveDate, bool objGetBenchmark = false)
        {
            try
            {
                List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();

                //Arguement Null Case, return Empty Set
                if ((objPortfolioIdentifier == null) || (effectiveDate == null))
                    return result;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData =
                    entity.GF_PORTFOLIO_HOLDINGS
                    .Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();

                //If Service returned empty set
                if (dimensionPortfolioHoldingsData.Count == 0)
                    return result;

                //Retrieve the Id of benchmark associated with the Portfolio
                List<string> benchmarkId = dimensionPortfolioHoldingsData.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                //If the DataBase doesn't return a single Benchmark for a Portfolio
                if (benchmarkId.Count != 1)
                    throw new InvalidOperationException();

                List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.
                    Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                List<GF_BENCHMARK_HOLDINGS> asb = dimensionBenchmarkHoldingsData.OrderBy(a => a.ISSUE_NAME).ToList();


                foreach (GF_PORTFOLIO_HOLDINGS item in dimensionPortfolioHoldingsData)
                {
                    PortfolioDetailsData portfolioResult = new PortfolioDetailsData();
                    portfolioResult.AsecSecShortName = item.ASEC_SEC_SHORT_NAME;
                    portfolioResult.IssueName = item.ISSUE_NAME;
                    portfolioResult.Ticker = item.TICKER;
                    portfolioResult.ProprietaryRegionCode = item.ASHEMM_PROP_REGION_CODE;
                    portfolioResult.IsoCountryCode = item.ISO_COUNTRY_CODE;
                    portfolioResult.SectorName = item.GICS_SECTOR_NAME;
                    portfolioResult.IndustryName = item.GICS_INDUSTRY_NAME;
                    portfolioResult.SubIndustryName = item.GICS_SUB_INDUSTRY_NAME;
                    portfolioResult.MarketCapUSD = item.MARKET_CAP_IN_USD;
                    portfolioResult.SecurityType = item.SECURITY_TYPE;
                    portfolioResult.BalanceNominal = item.BALANCE_NOMINAL;
                    portfolioResult.DirtyValuePC = item.DIRTY_VALUE_PC;
                    portfolioResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                                Where(a => a.ISSUE_NAME == portfolioResult.IssueName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                                Where(a => a.ISSUE_NAME == portfolioResult.IssueName).FirstOrDefault().BENCHMARK_WEIGHT);
                    portfolioResult.AshEmmModelWeight = item.ASH_EMM_MODEL_WEIGHT;
                    result.Add(portfolioResult);
                }

                result = PortfolioDetailsCalculations.CalculatePortfolioDetails(result);

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                return null;
                //throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Method to retrieve data in Benchmark Chart
        /// </summary>
        /// <param name="objBenchmarkIdentifier"></param>
        /// <param name="objEffectiveDate"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<BenchmarkChartReturnData> RetrieveBenchmarkChartReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate)
        {
            try
            {
                List<BenchmarkChartReturnData> result = new List<BenchmarkChartReturnData>();
                if ((objBenchmarkIdentifier != null) && (objEffectiveDate != null))
                {
                    Random random = new Random();

                    for (int i = 0; i < 365; i++)
                    {
                        BenchmarkChartReturnData data = new BenchmarkChartReturnData();
                        data.FromDate = DateTime.Now.AddDays(-182 + i);
                        data.InstrumentID = 10020.ToString();
                        data.IssueName = "MSCI Standard";
                        data.Ticker = "MSCI";
                        data.Type = "Benchmark";
                        data.DailyReturn = random.Next(5, 40);

                        result.Add(data);

                        data.FromDate = DateTime.Now.AddDays(-182 + i);
                        data.InstrumentID = 10021.ToString();
                        data.IssueName = "MSCI Brazil";
                        data.Ticker = "MSCIB";
                        data.DailyReturn = random.Next(5, 40);
                        data.Type = "Benchmark";
                        result.Add(data);
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
        /// Method to retrieve data in Benchmark Grid
        /// </summary>
        /// <param name="objBenchmarkIdentifier"></param>
        /// <param name="objEffectiveDate"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<BenchmarkGridReturnData> RetrieveBenchmarkGridReturnData(List<BenchmarkSelectionData> objBenchmarkIdentifier, DateTime objEffectiveDate)
        {
            List<BenchmarkGridReturnData> result = new List<BenchmarkGridReturnData>();
            try
            {
                if ((objBenchmarkIdentifier != null) && (objEffectiveDate != null))
                {
                    Random random = new Random();

                    BenchmarkGridReturnData data = new BenchmarkGridReturnData();
                    data.InstrumentID = 10020.ToString();
                    data.IssueName = "MSCI Standard";
                    data.MTD = random.Next(10, 100) / 10;
                    data.PreviousYearData = random.Next(10, 100) / 10;
                    data.QTD = random.Next(10, 100) / 10;
                    data.ThreePreviousYearData = random.Next(10, 100) / 10;
                    data.Ticker = "MSCI";
                    data.TwoPreviousYearData = random.Next(10, 100) / 10;
                    data.YTD = random.Next(10, 100) / 10;
                    data.Type = "Benchmark";
                    result.Add(data);

                    data.InstrumentID = 10021.ToString();
                    data.IssueName = "MSCI Brazil";
                    data.MTD = random.Next(10, 100) / 10;
                    data.PreviousYearData = random.Next(10, 100) / 10;
                    data.QTD = random.Next(10, 100) / 10;
                    data.ThreePreviousYearData = random.Next(10, 100) / 10;
                    data.Ticker = "MSCIB";
                    data.TwoPreviousYearData = random.Next(10, 100) / 10;
                    data.Type = "Benchmark";
                    data.YTD = random.Next(10, 100) / 10;
                    result.Add(data);
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
        /// Method to Retreive Asset Allocation Data
        /// </summary>
        /// <param name="portfolioSelectionData">Details of Selected Portfolio</param>
        /// <param name="effectiveDate">The Selected Date</param>
        /// <returns>List of AssetAllocationData</returns>
        [OperationContract]
        public List<AssetAllocationData> RetrieveAssetAllocationData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<AssetAllocationData> result = new List<AssetAllocationData>();

                //Arguement Null Exception
                if ((portfolioSelectionData == null) || (effectiveDate == null))
                    return result;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                //Arguement Null Exception
                if (entity == null)
                    return result;

                List<GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.
                    Where(a => (a.PORTFOLIO_ID == portfolioSelectionData.PortfolioId) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                if (dimensionPortfolioHoldingsData.Count == 0)
                    return result;

                //Retrieve the Id of benchmark associated with the Portfolio
                List<string> benchmarkId = dimensionPortfolioHoldingsData.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                //If the DataBase doesn't return a single Benchmark for a Portfolio
                if (benchmarkId.Count != 1)
                    throw new InvalidOperationException();

                List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.
                    Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && ((a.PORTFOLIO_DATE) == effectiveDate.Date)).ToList();
                result = AssetAllocationCalculations.CalculateAssetAllocationValues(dimensionPortfolioHoldingsData, dimensionBenchmarkHoldingsData, portfolioSelectionData);
                return result;
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
        [FaultContract(typeof(ServiceFault))]
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue)
        {

            try
            {

                if (portfolioSelectionData == null || effectiveDate == null || filterType == null || filterValue == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                HoldingsPercentageData entry = new HoldingsPercentageData();
                decimal? sumForBenchmarks = 0;
                decimal? sumForPortfolios = 0;

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> portfolioData = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).ToList();
                if (portfolioData.Count == 0 || portfolioData == null)
                {
                    return result;
                }
                String benchmarkId = portfolioData[0].BENCHMARK_ID.ToString();
                if (benchmarkId != null)
                {

                    List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> data = DimensionEntity.GF_BENCHMARK_HOLDINGS.Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate).ToList();
                    if (data != null || data.Count != 0)
                    {
                        switch (filterType)
                        {
                            case "Region":
                                var q = from p in data
                                        where (p.ASHEMM_PROP_REGION_CODE.ToString()).Equals(filterValue)
                                        group p by p.GICS_SECTOR_NAME into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var k = from p in portfolioData
                                        where (p.ASHEMM_PROP_REGION_CODE.ToString()).Equals(filterValue)
                                        group p by p.GICS_SECTOR_NAME into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                foreach (var a in q)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }

                                foreach (var a in q)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }
                                foreach (var a in k)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in k)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }

                                break;
                            case "Country":
                                var l = from p in data
                                        where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                        group p by p.GICS_SECTOR_NAME into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var c = from p in portfolioData
                                        where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                        group p by p.GICS_SECTOR_NAME into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                foreach (var a in l)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }

                                foreach (var a in l)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }

                                foreach (var a in c)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in c)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }
                                break;
                            case "Industry":
                                var m = from p in data
                                        where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                        group p by p.GICS_SECTOR_NAME into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };

                                var s = from p in portfolioData
                                        where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                        group p by p.GICS_SECTOR_NAME into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                foreach (var a in m)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }
                                foreach (var a in m)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }
                                foreach (var a in s)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in s)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }

                                break;
                            case "Sector":
                                var n = from p in data
                                        where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                                        group p by p.GICS_INDUSTRY_NAME into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var d = from p in portfolioData
                                        where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                                        group p by p.GICS_INDUSTRY_NAME into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                foreach (var a in n)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }
                                foreach (var a in n)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }
                                foreach (var a in d)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in d)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }

                                break;
                            default:
                                break;
                        }
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
        /// Retrieves Holdings data for showing pie chart for region allocation
        /// </summary>
        /// <param name="fundSelectionData">Contains Selected Fund Data</param>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        /// <param name="filterType">The Filter type selected by the user</param>
        /// <param name="filterValue">The Filter value selected by the user</param>
        /// <returns>List of HoldingsPercentageData </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue)
        {
            try
            {
                if (portfolioSelectionData == null || effectiveDate == null || filterType == null || filterValue == null)
                    throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                HoldingsPercentageData entry = new HoldingsPercentageData();
                decimal? sumForBenchmarks = 0;
                decimal? sumForPortfolios = 0;
                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> portfolioData = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).ToList();
                if (portfolioData.Count == 0 || portfolioData == null)
                    return result;
                String benchmarkId = portfolioData[0].BENCHMARK_ID.ToString();
                if (benchmarkId != null)
                {
                    List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> data = DimensionEntity.GF_BENCHMARK_HOLDINGS.Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate).ToList();
                    if (data != null || data.Count != 0)
                    {
                        switch (filterType)
                        {
                            case "Region":
                                var q = from p in data
                                        where (p.ASHEMM_PROP_REGION_CODE.ToString()).Equals(filterValue)
                                        group p by p.ISO_COUNTRY_CODE into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var k = from p in portfolioData
                                        where (p.ASHEMM_PROP_REGION_CODE.ToString()).Equals(filterValue)
                                        group p by p.ISO_COUNTRY_CODE into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                foreach (var a in q)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }

                                foreach (var a in q)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }
                                foreach (var a in k)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in k)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }
                                break;
                            case "Country":
                                var l = from p in data
                                        where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var c = from p in portfolioData
                                        where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };


                                foreach (var a in l)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }

                                foreach (var a in l)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }

                                foreach (var a in c)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in c)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }
                                break;
                            case "Industry":
                                var m = from p in data
                                        where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var s = from p in portfolioData
                                        where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };


                                foreach (var a in m)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }
                                foreach (var a in m)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }
                                foreach (var a in s)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in s)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }
                                break;
                            case "Sector":
                                var n = from p in data
                                        where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var d = from p in portfolioData
                                        where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                foreach (var a in n)
                                {
                                    sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                }
                                foreach (var a in n)
                                {
                                    if (sumForBenchmarks == 0)
                                        continue;
                                    CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                }
                                foreach (var a in d)
                                {
                                    sumForPortfolios = sumForPortfolios + a.PortfolioSum;
                                }
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (result[i].PortfolioWeight.Equals(null))
                                    {
                                        result[i].PortfolioWeight = 0;
                                    }
                                }
                                foreach (var a in d)
                                {
                                    if (sumForPortfolios == 0)
                                        continue;
                                    CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                }
                                break;
                            default:
                                break;
                        }
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
        /// Calculates total of Benchmark Weight and total Portfolio Weight
        /// </summary>
        /// <param name="sumForBenchmarks">Stores the sum of Benchmark Weight</param>
        /// <param name="sumForPortfolios">Stores the sum of Portfolio Weight</param>
        /// <param name="a">Benchmark Weight</param>
        /// <param name="b">Portfolio Weight</param>
        private void CalculatesPercentageForPortfolioSum(HoldingsPercentageData entry, decimal? sumForPortfolios, String name, decimal? b, String benchmarkName, ref List<HoldingsPercentageData> result, DateTime effectiveDate)
        {
            var segmentValue = (from p in result
                                where p.SegmentName == name
                                select p).FirstOrDefault();

            if (segmentValue != null)
            {
                if (String.IsNullOrWhiteSpace(segmentValue.SegmentName))
                {
                    segmentValue.SegmentName = "Unknown";
                }
                segmentValue.PortfolioWeight = (b / sumForPortfolios) * 100;
            }
            else
            {

                entry = new HoldingsPercentageData();
                entry.PortfolioWeight = (b / sumForPortfolios) * 100;
                entry.BenchmarkWeight = 0;
                if (String.IsNullOrWhiteSpace(name))
                {
                    entry.SegmentName = "Unknown";
                }
                else
                {
                    entry.SegmentName = name;
                }
                entry.BenchmarkName = benchmarkName;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);
            }
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
        private void CalculatesPercentageForBenchmarkSum(HoldingsPercentageData entry, decimal? sumForBenchmarks, String name, decimal? a, String benchmarkName, ref List<HoldingsPercentageData> result, DateTime effectiveDate)
        {
            entry = new HoldingsPercentageData();
            if (String.IsNullOrWhiteSpace(name))
            {
                entry.SegmentName = "Unknown";
            }
            else
            {
                entry.SegmentName = name;
            }
            entry.BenchmarkWeight = (a / sumForBenchmarks) * 100;
            entry.BenchmarkName = benchmarkName;
            entry.EffectiveDate = effectiveDate;
            result.Add(entry);
        }
        #endregion

        /// <summary>
        /// Retrieves Top Benchmark Securities data 
        /// </summary>
        /// <param name="benchmarkSelectionData">Contains Selected Benchmark Data </param>
        /// <param name="effectiveDate">Effective Date selected by user</param>
        /// <returns>returns list of Top Ten Benchmarks </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<TopBenchmarkSecuritiesData> RetrieveTopBenchmarkSecuritiesData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
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

        #region Performance

        #region MARKET CAPITALIZATION METHODS
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
                if (filterType != null && filterValue != null)
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
                        default:
                            break;
                    }                    
                }

                for (int _index = 0; _index < dimensionServicePortfolioData.Count; _index++)
                {
                    MarketCapitalizationData mktCapData = new MarketCapitalizationData();
                    mktCapData.PortfolioDirtyValuePC = dimensionServicePortfolioData[_index].DIRTY_VALUE_PC;
                    mktCapData.MarketCapitalInUSD = dimensionServicePortfolioData[_index].MARKET_CAP_IN_USD;
                    mktCapData.SecurityThemeCode = dimensionServicePortfolioData[_index].SECURITYTHEMECODE;
                    mktCapData.Benchmark_ID = dimensionServicePortfolioData[_index].BENCHMARK_ID;
                    //mktCapData.Portfolio_ID = dimensionServicePortfolioData[_index].PORTFOLIO_ID;
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
                if (filterType != null && filterValue != null)
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
                        default:
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
                     if (portfolioSelectionData == null || effectiveDate == null )//|| filterType == null || filterValue == null)
                         throw new ArgumentNullException(ServiceFaultResourceManager.GetString(GreenfieldConstants.SERVICE_NULL_ARG_EXC_MSG).ToString());

                     List<MarketCapitalizationData> mktCap = new List<MarketCapitalizationData>();
                     MarketCapitalizationData mktCapData = new MarketCapitalizationData();
                
                    //Consolidated list Portfolio and benchmark Data
                     mktCap = RetrievePortfolioMktCapData(portfolioSelectionData, effectiveDate, filterType, filterValue, isExCashSecurity);

                     if (mktCap.Count == 0)
                         return mktCap;

                    //weighted avg for portfolio                 
                    mktCapData.PortfolioWtdAvg = MarketCapitalizationCalculations.CalculatePortfolioWeightedAvg(mktCap);                

                    //weighted median for portfolio
                    mktCapData.PortfolioWtdMedian = MarketCapitalizationCalculations.CalculatePortfolioWeightedMedian(mktCap);               
                
                    //ranges for portfolio
                    List<MarketCapitalizationData>  lstmktCapPortfolio = MarketCapitalizationCalculations.CalculateSumPortfolioRanges(mktCap);
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

                    mktCap.Add(mktCapData);
                    return mktCap;  

            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString(GreenfieldConstants.NETWORK_FAULT_ECX_MSG).ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
             

        }
        #endregion

        #endregion

        

       

        #region Heat Map Operation Contract
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
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

        #region Relative Performance
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceSectorData> RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate)
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves Country Level Active Position Data for a particular composite/fund, benchmark and effective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceCountryActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null)
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves Sector Level Active Position Data for a particular composite/fund, benchmark and effective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceSectorActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null)
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves Security Level Active Position Data for a particular composite/fund, benchmark and effective date.
        /// Filtering data filtering based on ISO_COUNTRY_CODE, GICS_SECTOR and record restriction handled through optional arguments
        /// </summary>
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceActivePositionData> RetrieveRelativePerformanceSecurityActivePositionData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null)
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
        public List<RelativePerformanceSecurityData> RetrieveRelativePerformanceSecurityData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null)
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RelativePerformanceData> RetrieveRelativePerformanceData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate)
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
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves Performance graph data for a particular composite/fund.
        /// Filtering data based on the fund name.
        /// </summary>
        /// <param name="nameOfFund">Name of the selected fund</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
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
        /// Filtering data based on the fund name and Effective date.
        /// </summary>
        /// <param name="portfolioSelectionData">Portfolio Data that contains the name of the selected portfolio</param>
        /// <param name="effectiveDate">Selected Effective Date</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PerformanceGridData> RetrievePerformanceGridData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            if (portfolioSelectionData == null || effectiveDate == null)
            throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());
           List<PerformanceGridData> result = new List<PerformanceGridData>();
           DimensionEntitiesService.GF_PERF_MONTHLY_ATTRIBUTION performanceData = DimensionEntity.GF_PERF_MONTHLY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate).FirstOrDefault();           
           if (performanceData == null)
           return result;
           String portfolioID = performanceData.PORTFOLIO;
           String benchmarkID =  DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioID).FirstOrDefault().BENCHMARK_ID;
            try
            {
                {
                    PerformanceGridData entry = new PerformanceGridData();
                    entry.Name = portfolioID;
                    entry.MTD = performanceData.POR_TOP_QC_TWR_1M;
                    entry.QTD = performanceData.POR_TOP_RC_TWR_3M;
                    entry.YTD = performanceData.POR_TOP_RC_TWR_YTD;
                    entry.FIRST_YEAR = performanceData.POR_TOP_RC_TWR_1Y;
                    entry.THIRD_YEAR = performanceData.POR_TOP_RC_TWR_3Y_ANN;
                    entry.FIFTH_YEAR = performanceData.POR_TOP_RC_TWR_5Y_ANN;
                    entry.TENTH_YEAR = performanceData.POR_TOP_RC_TWR_SI_ANN;
                    result.Add(entry);
                    entry = new PerformanceGridData();
                    entry.Name = benchmarkID;
                    entry.MTD = performanceData.BM1_TOP_RC_TWR_1M;
                    entry.QTD = performanceData.BM1_TOP_RC_TWR_3M;
                    entry.YTD = performanceData.BM1_TOP_RC_TWR_YTD;
                    entry.FIRST_YEAR = performanceData.BM1_TOP_RC_TWR_1Y;
                    entry.THIRD_YEAR = performanceData.BM1_TOP_RC_TWR_3Y_ANN;
                    entry.FIFTH_YEAR = performanceData.BM1_TOP_RC_TWR_5Y_ANN;
                    entry.TENTH_YEAR = performanceData.BM1_TOP_RC_TWR_SI_ANN;
                    result.Add(entry);
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
        /// Retrieves Attribution Data for a particular composite/fund and Effective Date
        /// Filtering data based on fund name
        /// </summary>
        /// <param name="portfolioSelectionData">Portfolio Data that contains the name of the selected portfolio</param>
        /// <param name="effectiveDate">Selected Effective Date</param>
        /// <returns>List of Attribution Data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<AttributionData> RetrieveAttributionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            if (portfolioSelectionData == null || effectiveDate == null)
                throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());
            List<AttributionData> result = new List<AttributionData>();
            List<DimensionEntitiesService.GF_PERF_MONTHLY_ATTRIBUTION> attributionData = DimensionEntity.GF_PERF_MONTHLY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate).ToList();
            if (attributionData.Count == 0 || attributionData == null)
                return result;
            try
            {
                for (int i = 0; i < attributionData.Count; i++)
                {
                    AttributionData entry = new AttributionData();
                    entry.COUNTRY = attributionData[i].COUNTRY;
                    entry.COUNTRY_NAME = attributionData[i].COUNTRY_NAME;
                    entry.POR_RC_AVG_WGT_1M = attributionData[i].POR_RC_AVG_WGT_1M;
                    entry.BM1_RC_AVG_WGT_1M = attributionData[i].BM1_RC_AVG_WGT_1M;
                    entry.F_POR_ASH_RC_CTN_1M = attributionData[i].F_POR_ASH_RC_CTN_1M;
                    entry.F_BM1_ASH_RC_CTN_1M = attributionData[i].F_BM1_ASH_RC_CTN_1M;
                    entry.F_BM1_ASH_ASSET_ALLOC_1M = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1M;
                    entry.F_BM1_ASH_SEC_SELEC_1M = attributionData[i].F_BM1_ASH_SEC_SELEC_1M;
                    entry.POR_RC_AVG_WGT_3M = attributionData[i].POR_RC_AVG_WGT_3M;
                    entry.BM1_RC_AVG_WGT_3M = attributionData[i].BM1_RC_AVG_WGT_3M;
                    entry.F_POR_ASH_RC_CTN_3M = attributionData[i].F_POR_ASH_RC_CTN_3M;
                    entry.F_BM1_ASH_ASSET_ALLOC_3M = attributionData[i].F_BM1_ASH_ASSET_ALLOC_3M;
                    entry.F_BM1_ASH_SEC_SELEC_3M = attributionData[i].F_BM1_ASH_SEC_SELEC_3M;
                    entry.POR_RC_AVG_WGT_6M = attributionData[i].POR_RC_AVG_WGT_6M;
                    entry.BM1_RC_AVG_WGT_6M = attributionData[i].BM1_RC_AVG_WGT_6M;
                    entry.F_POR_ASH_RC_CTN_6M = attributionData[i].F_POR_ASH_RC_CTN_6M;
                    entry.F_BM1_ASH_RC_CTN_6M = attributionData[i].F_BM1_ASH_RC_CTN_6M;
                    entry.F_BM1_ASH_ASSET_ALLOC_6M = attributionData[i].F_BM1_ASH_ASSET_ALLOC_6M;
                    entry.F_BM1_ASH_SEC_SELEC_6M = attributionData[i].F_BM1_ASH_SEC_SELEC_6M;
                    entry.POR_RC_AVG_WGT_YTD = attributionData[i].POR_RC_AVG_WGT_YTD;
                    entry.BM1_RC_AVG_WGT_YTD = attributionData[i].BM1_RC_AVG_WGT_YTD;
                    entry.F_POR_ASH_RC_CTN_YTD = attributionData[i].F_POR_ASH_RC_CTN_YTD;
                    entry.F_BM1_ASH_RC_CTN_YTD = attributionData[i].F_BM1_ASH_RC_CTN_YTD;
                    entry.F_BM1_ASH_ASSET_ALLOC_YTD = attributionData[i].F_BM1_ASH_ASSET_ALLOC_YTD;
                    entry.F_BM1_ASH_SEC_SELEC_YTD = attributionData[i].F_BM1_ASH_SEC_SELEC_YTD;
                    entry.POR_RC_AVG_WGT_1Y = attributionData[i].POR_RC_AVG_WGT_1Y;
                    entry.BM1_RC_AVG_WGT_1Y = attributionData[i].BM1_RC_AVG_WGT_1Y;
                    entry.F_POR_ASH_RC_CTN_1Y = attributionData[i].F_POR_ASH_RC_CTN_1Y;
                    entry.F_BM1_ASH_RC_CTN_1Y = attributionData[i].F_BM1_ASH_RC_CTN_1Y;
                    entry.F_BM1_ASH_ASSET_ALLOC_1Y = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1Y;
                    entry.F_BM1_ASH_SEC_SELEC_1Y = attributionData[i].F_BM1_ASH_SEC_SELEC_1Y;
                    entry.POR_RC_AVG_WGT_3Y = attributionData[i].POR_RC_AVG_WGT_3Y;
                    entry.BM1_RC_AVG_WGT_3Y = attributionData[i].BM1_RC_AVG_WGT_3Y;
                    entry.F_POR_ASH_RC_CTN_3Y = attributionData[i].F_POR_ASH_RC_CTN_3Y;
                    entry.F_BM1_ASH_RC_CTN_3Y = attributionData[i].F_BM1_ASH_RC_CTN_3Y;
                    entry.F_BM1_ASH_ASSET_ALLOC_3Y = attributionData[i].F_BM1_ASH_ASSET_ALLOC_3Y;
                    entry.F_BM1_ASH_SEC_SELEC_3Y = attributionData[i].F_BM1_ASH_SEC_SELEC_3Y;
                    entry.POR_RC_AVG_WGT_5Y = attributionData[i].POR_RC_AVG_WGT_5Y;
                    entry.BM1_RC_AVG_WGT_5Y = attributionData[i].BM1_RC_AVG_WGT_5Y;
                    entry.F_POR_ASH_RC_CTN_5Y = attributionData[i].F_POR_ASH_RC_CTN_5Y;
                    entry.F_BM1_ASH_RC_CTN_5Y = attributionData[i].F_BM1_ASH_RC_CTN_5Y;
                    entry.F_BM1_ASH_ASSET_ALLOC_5Y = attributionData[i].F_BM1_ASH_ASSET_ALLOC_5Y;
                    entry.F_BM1_ASH_SEC_SELEC_5Y = attributionData[i].F_BM1_ASH_SEC_SELEC_5Y;
                    entry.POR_RC_AVG_WGT_SI = attributionData[i].POR_RC_AVG_WGT_SI;
                    entry.BM1_RC_AVG_WGT_SI = attributionData[i].BM1_RC_AVG_WGT_SI;
                    entry.F_POR_ASH_RC_CTN_SI = attributionData[i].F_POR_ASH_RC_CTN_SI;
                    entry.F_BM1_ASH_RC_CTN_SI = attributionData[i].F_BM1_ASH_RC_CTN_SI;
                    entry.F_BM1_ASH_ASSET_ALLOC_SI = attributionData[i].F_BM1_ASH_ASSET_ALLOC_SI;
                    entry.F_BM1_ASH_SEC_SELEC_SI = attributionData[i].F_BM1_ASH_SEC_SELEC_SI;
                    result.Add(entry);
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
        /// Retrieves Portfolio Risk Return Data
        /// </summary>
        /// <param name="fundSelectionData">Contains Selected Fund Data</param>
        /// <param name="benchmarkSelectionData">Contains Selected Benchmark Data </param>
        /// <param name="effectiveDate">Effective Date selected by user</param>
        /// <returns>returns List of PortfolioRiskReturnData containing Portfolio Risk Return Data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PortfolioRiskReturnData> RetrievePortfolioRiskReturnData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            if (portfolioSelectionData == null || effectiveDate == null)
                throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());
            List<PortfolioRiskReturnData> result = new List<PortfolioRiskReturnData>();
           List<DimensionEntitiesService.GF_PERF_TOPLEVELSTATS> riskReturnData = DimensionEntity.GF_PERF_TOPLEVELSTATS.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate).ToList();
            if (riskReturnData == null)
                return result;            
            try
            {
                PortfolioRiskReturnData entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Alpha";
                entry.BenchMarkValue = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Benchmark1" && t.RETURN_TYPE=="Net").Select(t => t.RC_ALPHA).FirstOrDefault());
                entry.PortfolioValue =(riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Net").Select(t => t.RC_ALPHA).FirstOrDefault());
                result.Add(entry);
                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Beta";
                entry.BenchMarkValue = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Benchmark1" && t.RETURN_TYPE == "Net").Select(t => t.RC_BETA).FirstOrDefault());
                entry.PortfolioValue = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Net").Select(t => t.RC_BETA).FirstOrDefault());
                result.Add(entry);
                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Information Ratio";
                entry.BenchMarkValue = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Benchmark1" && t.RETURN_TYPE == "Net").Select(t => t.RC_INFORMATION).FirstOrDefault());
                entry.PortfolioValue = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Net").Select(t => t.RC_INFORMATION).FirstOrDefault());
                result.Add(entry);
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
