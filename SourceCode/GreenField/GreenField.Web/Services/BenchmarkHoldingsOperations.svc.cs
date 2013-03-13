using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Caching;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Xml;
using System.Xml.Serialization;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.DAL;
using System.Xml.Serialization;
using System.Collections;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for BenchmarkHoldings
    /// </summary>
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BenchmarkHoldingsOperations
    {
        /// <summary>
        /// Instance of DimensionService
        /// </summary>
        private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                {
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
                }

                return dimensionEntity;
            }
        }

        /// <summary>
        /// Returns the cache folder specified in web.config
        /// </summary>
        private String cacheFolder;
        public String CacheFolder
        {
            get
            {
                if (String.IsNullOrEmpty(cacheFolder))
                {
                    cacheFolder = ConfigurationManager.AppSettings["CacheDirectory"];
                }

                return cacheFolder;
            }
        }

        /// <summary>
        /// Returns the portfolioName from which the available
        /// dates needs to be found
        /// </summary>
        private String portfolioName;
        public String PortfolioName
        {
            get
            {
                if (String.IsNullOrEmpty(portfolioName))
                {
                    portfolioName = ConfigurationManager.AppSettings["PortfolioName"];
                }

                return portfolioName;
            }
        }

        /// <summary>
        /// Fault Resource manager
        /// </summary>
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

        /// <summary>
        /// Retrieve list of Portfolio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PortfolioSelectionData> RetrievePortfolioSelectionData()
        {
            try
            {
                // use cache if available
                var fromCache = (List<PortfolioSelectionData>)new DefaultCacheProvider().Get(CacheKeyNames.PortfolioSelectionDataCache);
                if (fromCache != null)
                    return fromCache;

                // otherwise fetch the data and cache it
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

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

                new DefaultCacheProvider().Set(CacheKeyNames.PortfolioSelectionDataCache, result, Int32.Parse(ConfigurationManager.AppSettings["SecuritiesCacheTime"]));

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
        /// <param name="isExCashSecurity">bool</param>
        /// <param name="lookThruEnabled">bool</param>
        /// <returns>list of sector breakdown data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<SectorBreakdownData> RetrieveSectorBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity,
            bool lookThruEnabled)
        {
            try
            {
                List<SectorBreakdownData> result = new List<SectorBreakdownData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                { return result; }

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                { throw new Exception(); }

                if (lookThruEnabled)
                {
                    #region Look - thru enabled
                    //Retrieve GF_PORTFOLIO_HOLDINGS data SECURITYTHEMECODE
                    List<GF_PORTFOLIO_LTHOLDINGS> data = isExCashSecurity
                                                  ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                           && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                           && record.SECURITYTHEMECODE != "CASH").ToList()
                                                  : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                           && record.PORTFOLIO_DATE == effectiveDate.Date).ToList();
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    Decimal? netPortfolioValuation = data.Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));

                    if (netPortfolioValuation == 0 || netPortfolioValuation == null)
                    { throw new InvalidOperationException(); }

                    //Retrieve the Id of benchmark associated with the Portfolio
                    List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkId.Count != 1)
                    { throw new InvalidOperationException(); }

                    List<GF_BENCHMARK_HOLDINGS> benchmarkData = DimensionEntity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                    foreach (GF_PORTFOLIO_LTHOLDINGS record in data)
                    {
                        if (record.DIRTY_VALUE_PC == null)
                        { continue; }

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (record.DIRTY_VALUE_PC / netPortfolioValuation) * 100;

                        //Retrieve Benchmark Weight 
                        decimal? benchmarkWeight = (Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == record.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT)
                                                                                                                                    .FirstOrDefault()));
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

                    #region BenchmarkSecurities
                    List<string> portfolioSecurityID = data.Select(a => a.ASEC_SEC_SHORT_NAME).ToList();
                    List<GF_BENCHMARK_HOLDINGS> onlyBenchmarkSecurities = benchmarkData.Where(a => !portfolioSecurityID.Contains(a.ASEC_SEC_SHORT_NAME)).ToList();
                    foreach (GF_BENCHMARK_HOLDINGS item in onlyBenchmarkSecurities)
                    {

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = 0;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = item.BENCHMARK_WEIGHT;
                        //Calculate Active Position
                        decimal? activePosition = portfolioWeight - benchmarkWeight;

                        result.Add(new SectorBreakdownData()
                        {
                            Sector = item.GICS_SECTOR_NAME,
                            Industry = item.GICS_INDUSTRY_NAME,
                            Security = item.ISSUE_NAME,
                            PortfolioShare = portfolioWeight,
                            BenchmarkShare = benchmarkWeight,
                            ActivePosition = activePosition
                        });
                    }
                    #endregion
                    return result;
                    #endregion
                }
                else
                {
                    #region Look - thru disabled
                    //Retrieve GF_PORTFOLIO_HOLDINGS data SECURITYTHEMECODE
                    List<GF_PORTFOLIO_HOLDINGS> data = isExCashSecurity
                                                    ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                             && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                             && record.SECURITYTHEMECODE != "CASH").ToList()
                                                     : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                             && record.PORTFOLIO_DATE == effectiveDate.Date).ToList();
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    Decimal? netPortfolioValuation = data.Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));

                    if (netPortfolioValuation == 0 || netPortfolioValuation == null)
                    { throw new InvalidOperationException(); }

                    //Retrieve the Id of benchmark associated with the Portfolio
                    List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkId.Count != 1)
                    { throw new InvalidOperationException(); }

                    List<GF_BENCHMARK_HOLDINGS> benchmarkData = DimensionEntity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                    foreach (GF_PORTFOLIO_HOLDINGS record in data)
                    {
                        if (record.DIRTY_VALUE_PC == null)
                        { continue; }

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (record.DIRTY_VALUE_PC / netPortfolioValuation) * 100;

                        //Retrieve Benchmark Weight 
                        decimal? benchmarkWeight = (Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == record.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT)
                                                                                                                                 .FirstOrDefault()));
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

                    #region BenchmarkSecurities

                    List<string> portfolioSecurityID = data.Select(a => a.ASEC_SEC_SHORT_NAME).ToList();
                    List<GF_BENCHMARK_HOLDINGS> onlyBenchmarkSecurities = benchmarkData.Where(a => !portfolioSecurityID.Contains(a.ASEC_SEC_SHORT_NAME)).ToList();
                    foreach (GF_BENCHMARK_HOLDINGS item in onlyBenchmarkSecurities)
                    {

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = 0;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = item.BENCHMARK_WEIGHT;
                        //Calculate Active Position
                        decimal? activePosition = portfolioWeight - benchmarkWeight;

                        result.Add(new SectorBreakdownData()
                        {
                            Sector = item.GICS_SECTOR_NAME,
                            Industry = item.GICS_INDUSTRY_NAME,
                            Security = item.ISSUE_NAME,
                            PortfolioShare = portfolioWeight,
                            BenchmarkShare = benchmarkWeight,
                            ActivePosition = activePosition
                        });
                    }
                    #endregion
                    return result;
                    #endregion
                }
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
        /// <param name="isExCashSecurity">bool</param>
        /// <param name="lookThruEnabled">bool</param>
        /// <returns>list of region breakdown data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RegionBreakdownData> RetrieveRegionBreakdownData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity,
            bool lookThruEnabled)
        {
            try
            {
                List<RegionBreakdownData> result = new List<RegionBreakdownData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                { return result; }

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                { throw new Exception(); }

                if (lookThruEnabled)
                {
                    #region Look-thru enabled
                    //Retrieve GF_PORTFOLIO_HOLDINGS data SECURITYTHEMECODE
                    List<GF_PORTFOLIO_LTHOLDINGS> data = isExCashSecurity
                                                   ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                                       && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                                       && record.SECURITYTHEMECODE != "CASH").ToList()
                                                   : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                              && record.PORTFOLIO_DATE == effectiveDate.Date).ToList();
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    Decimal? netPortfolioValuation = data.Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));

                    if (netPortfolioValuation == 0 || netPortfolioValuation == null)
                    { throw new InvalidOperationException(); }

                    //Retrieve the Id of benchmark associated with the Portfolio
                    List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkId.Count != 1)
                    { throw new InvalidOperationException(); }

                    List<GF_BENCHMARK_HOLDINGS> benchmarkData = DimensionEntity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                    foreach (GF_PORTFOLIO_LTHOLDINGS record in data)
                    {
                        if (record.DIRTY_VALUE_PC == null)
                        { continue; }

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (record.DIRTY_VALUE_PC / netPortfolioValuation) * 100;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == record.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT)
                                                                                                                                .FirstOrDefault());
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
                    #region BenchmarkSecurities
                    List<string> portfolioSecurityID = data.Select(a => a.ASEC_SEC_SHORT_NAME).ToList();
                    List<GF_BENCHMARK_HOLDINGS> onlyBenchmarkSecurities = benchmarkData.Where(a => !portfolioSecurityID.Contains(a.ASEC_SEC_SHORT_NAME)).ToList();
                    foreach (GF_BENCHMARK_HOLDINGS item in onlyBenchmarkSecurities)
                    {

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = 0;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = item.BENCHMARK_WEIGHT;
                        //Calculate Active Position
                        decimal? activePosition = portfolioWeight - benchmarkWeight;

                        result.Add(new RegionBreakdownData()
                        {
                            Region = item.ASHEMM_PROP_REGION_NAME,
                            Country = item.COUNTRYNAME,
                            Security = item.ISSUE_NAME,
                            PortfolioShare = portfolioWeight,
                            BenchmarkShare = benchmarkWeight,
                            ActivePosition = activePosition
                        });
                    }
                    #endregion
                    return result;
                    #endregion
                }
                else
                {
                    #region Look-thru disabled
                    //Retrieve GF_PORTFOLIO_HOLDINGS data SECURITYTHEMECODE
                    List<GF_PORTFOLIO_HOLDINGS> data = isExCashSecurity
                                                     ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                              && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                              && record.SECURITYTHEMECODE != "CASH").ToList()
                                                     : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                              && record.PORTFOLIO_DATE == effectiveDate.Date).ToList();
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    Decimal? netPortfolioValuation = data.Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));

                    if (netPortfolioValuation == 0 || netPortfolioValuation == null)
                    { throw new InvalidOperationException(); }

                    //Retrieve the Id of benchmark associated with the Portfolio
                    List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkId.Count != 1)
                    { throw new InvalidOperationException(); }

                    List<GF_BENCHMARK_HOLDINGS> benchmarkData = DimensionEntity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                    foreach (GF_PORTFOLIO_HOLDINGS record in data)
                    {
                        if (record.DIRTY_VALUE_PC == null)
                        { continue; }

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (record.DIRTY_VALUE_PC / netPortfolioValuation) * 100;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == record.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT)
                                                                                                                                    .FirstOrDefault());
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

                    #region BenchmarkSecurities
                    List<string> portfolioSecurityID = data.Select(a => a.ASEC_SEC_SHORT_NAME).ToList();
                    List<GF_BENCHMARK_HOLDINGS> onlyBenchmarkSecurities = benchmarkData.Where(a => !portfolioSecurityID.Contains(a.ASEC_SEC_SHORT_NAME)).ToList();
                    foreach (GF_BENCHMARK_HOLDINGS item in onlyBenchmarkSecurities)
                    {

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = 0;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = item.BENCHMARK_WEIGHT;
                        //Calculate Active Position
                        decimal? activePosition = portfolioWeight - benchmarkWeight;

                        result.Add(new RegionBreakdownData()
                        {
                            Region = item.ASHEMM_PROP_REGION_NAME,
                            Country = item.COUNTRYNAME,
                            Security = item.ISSUE_NAME,
                            PortfolioShare = portfolioWeight,
                            BenchmarkShare = benchmarkWeight,
                            ActivePosition = activePosition
                        });
                    }

                    #endregion
                    return result;
                    #endregion
                }
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
        /// <param name="isExCashSecurity">bool</param>
        /// <param name="lookThruEnabled">bool</param>
        /// <returns>list of top holdings data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<TopHoldingsData> RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity,
            bool lookThruEnabled)
        {
            try
            {
                List<TopHoldingsData> result = new List<TopHoldingsData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                { return result; }

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                { throw new Exception(); }

                decimal sumMarketValuePortfolio = 0;
                if (lookThruEnabled)
                {
                    #region Look - Through Enabled
                    //get the summation of DIRTY_VALUE_PC used to calculate the holding's PortfolioShare
                    sumMarketValuePortfolio = isExCashSecurity
                                                  ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                            && t.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                            && t.SECURITYTHEMECODE != "CASH").ToList()
                                                                                                            .Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC))
                                                  : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                            && t.PORTFOLIO_DATE == effectiveDate.Date).ToList()
                                                                                                            .Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));
                    //if sum of DIRTY_VALUE_PC for criterion is zero, empty set is returned
                    if (sumMarketValuePortfolio == 0)
                    { return result; }

                    //Retrieve GF_LTPORTFOLIO_HOLDINGS data for top ten holdings based on DIRTY_VALUE_PC and SECURITYTHEMECODE

                    List<GF_PORTFOLIO_LTHOLDINGS> data = isExCashSecurity
                                                  ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                 && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                 && record.SECURITYTHEMECODE != "CASH")
                                                                                                 .OrderByDescending(record => record.DIRTY_VALUE_PC).Take(10).ToList()
                                                  : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                  && record.PORTFOLIO_DATE == effectiveDate.Date)
                                                                                                 .OrderByDescending(record => record.DIRTY_VALUE_PC).Take(10).ToList();
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    foreach (GF_PORTFOLIO_LTHOLDINGS record in data)
                    {
                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (record.DIRTY_VALUE_PC / sumMarketValuePortfolio) * 100;

                        //Calculate Benchmark Weight - if null look for data in GF_BENCHMARK_HOLDINGS
                        GF_BENCHMARK_HOLDINGS specificHolding = DimensionEntity.GF_BENCHMARK_HOLDINGS.Where(rec => rec.ISSUE_NAME == record.ISSUE_NAME
                                                                                                      && rec.BENCHMARK_ID == record.BENCHMARK_ID
                                                                                                      && rec.PORTFOLIO_DATE == record.PORTFOLIO_DATE).FirstOrDefault();
                        decimal? benchmarkWeight = specificHolding != null ? Convert.ToDecimal(specificHolding.BENCHMARK_WEIGHT) : Convert.ToDecimal(null);

                        //Calculate Active Position
                        decimal? activePosition = portfolioWeight - benchmarkWeight;

                        result.Add(new TopHoldingsData()
                        {
                            Ticker = record.TICKER,
                            Holding = record.ISSUE_NAME,
                            MarketValue = record.DIRTY_VALUE_PC,
                            PortfolioShare = portfolioWeight,
                            BenchmarkShare = benchmarkWeight,
                            ActivePosition = activePosition
                        });
                    }
                    return result;
                    #endregion
                }
                else
                {
                    #region Look - Through disabled
                    //get the summation of DIRTY_VALUE_PC used to calculate the holding's PortfolioShare on the basis of SECURITYTHEMECODE
                    sumMarketValuePortfolio = isExCashSecurity
                                                  ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                          && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                          && record.SECURITYTHEMECODE != "CASH").ToList()
                                                                                                          .Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC))
                                                  : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                          && t.PORTFOLIO_DATE == effectiveDate.Date).ToList()
                                                                                                          .Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));
                    //if sum of DIRTY_VALUE_PC for criterion is zero, empty set is returned
                    if (sumMarketValuePortfolio == 0)
                    { return result; }

                    //Retrieve GF_PORTFOLIO_HOLDINGS data for top ten holdings based on DIRTY_VALUE_PC and SECURITYTHEMECODE
                    List<GF_PORTFOLIO_HOLDINGS> data = isExCashSecurity
                                                  ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                && record.SECURITYTHEMECODE != "CASH")
                                                                                                .OrderByDescending(record => record.DIRTY_VALUE_PC).Take(10).ToList()
                                                  : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                 && record.PORTFOLIO_DATE == effectiveDate.Date)
                                                                                                .OrderByDescending(record => record.DIRTY_VALUE_PC).Take(10).ToList();
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    foreach (GF_PORTFOLIO_HOLDINGS record in data)
                    {
                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (record.DIRTY_VALUE_PC / sumMarketValuePortfolio) * 100;

                        //Calculate Benchmark Weight - if null look for data in GF_BENCHMARK_HOLDINGS
                        GF_BENCHMARK_HOLDINGS specificHolding = DimensionEntity.GF_BENCHMARK_HOLDINGS.Where(rec => rec.ISSUE_NAME == record.ISSUE_NAME
                                                                                                     && rec.BENCHMARK_ID == record.BENCHMARK_ID
                                                                                                     && rec.PORTFOLIO_DATE == record.PORTFOLIO_DATE).FirstOrDefault();
                        decimal? benchmarkWeight = specificHolding != null ? Convert.ToDecimal(specificHolding.BENCHMARK_WEIGHT) : Convert.ToDecimal(null);

                        //Calculate Active Position
                        decimal? activePosition = portfolioWeight - benchmarkWeight;

                        result.Add(new TopHoldingsData()
                        {
                            Ticker = record.TICKER,
                            Holding = record.ISSUE_NAME,
                            MarketValue = record.DIRTY_VALUE_PC,
                            PortfolioShare = portfolioWeight,
                            BenchmarkShare = benchmarkWeight,
                            ActivePosition = activePosition
                        });
                    }
                    return result;
                    #endregion
                }
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
        /// <param name="lookThruEnabled">bool</param>
        /// <returns>list of index constituents data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<IndexConstituentsData> RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool lookThruEnabled)
        {
            try
            {
                List<IndexConstituentsData> result = new List<IndexConstituentsData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                { return result; }

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                { throw new Exception(); }

                if (lookThruEnabled)
                {
                    #region Look-thru enabled
                    GF_PORTFOLIO_LTHOLDINGS benchmarkRow = DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                      && t.PORTFOLIO_DATE.Equals(effectiveDate.Date)).FirstOrDefault();
                    //Return empty set if PORTFOLIO_ID and PORTFOLIO_DATE combination does not exist
                    if (benchmarkRow == null)
                    { return result; }

                    string benchmarkId = benchmarkRow.BENCHMARK_ID;
                    if (benchmarkId != null)
                    {
                        List<GF_BENCHMARK_HOLDINGS> data = DimensionEntity.GF_BENCHMARK_HOLDINGS
                            .Where(t => (t.BENCHMARK_ID == benchmarkId) && (t.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                        if (data.Count.Equals(0))
                        { return result; }

                        if (data != null)
                        {
                            foreach (DimensionEntitiesService.GF_BENCHMARK_HOLDINGS record in data)
                            {
                                //calculate sum of BENCHMARK_WEIGHT for a country
                                string country = record.COUNTRYNAME;
                                object sumBenchmarkWeightCountry = data.Where(t => t.COUNTRYNAME == country).Sum(t => t.BENCHMARK_WEIGHT);

                                //calculte sum of BENCHMARK_WEIGHT for a industry
                                string industry = record.GICS_INDUSTRY_NAME;
                                object sumBenchmarkWeightIndustry = data.Where(t => t.GICS_INDUSTRY_NAME == industry && t.COUNTRYNAME == country)
                                    .Sum(t => t.BENCHMARK_WEIGHT);
                                if (sumBenchmarkWeightCountry != null && sumBenchmarkWeightIndustry != null)
                                {
                                    result.Add(new IndexConstituentsData()
                                    {
                                        ConstituentName = record.ISSUE_NAME,
                                        BenchmarkId = record.BENCHMARK_ID,
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
                    #endregion
                }
                else
                {
                    #region Look-thru disabled
                    GF_PORTFOLIO_HOLDINGS benchmarkRow = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                      && t.PORTFOLIO_DATE.Equals(effectiveDate.Date)).FirstOrDefault();
                    //Return empty set if PORTFOLIO_ID and PORTFOLIO_DATE combination does not exist
                    if (benchmarkRow == null)
                    { return result; }

                    string benchmarkId = benchmarkRow.BENCHMARK_ID;
                    if (benchmarkId != null)
                    {
                        List<GF_BENCHMARK_HOLDINGS> data = DimensionEntity.GF_BENCHMARK_HOLDINGS
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
                                object sumBenchmarkWeightIndustry = data.Where(t => t.GICS_INDUSTRY_NAME == industry && t.COUNTRYNAME == country)
                                    .Sum(t => t.BENCHMARK_WEIGHT);
                                if (sumBenchmarkWeightCountry != null && sumBenchmarkWeightIndustry != null)
                                {
                                    result.Add(new IndexConstituentsData()
                                    {
                                        ConstituentName = record.ISSUE_NAME,
                                        BenchmarkId = record.BENCHMARK_ID,
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
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// web method for relative risk index exposures
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData object</param>
        /// <param name="effectiveDate">Effective Date</param>
        /// <param name="isExCashSecurity">bool</param>
        /// <param name="lookThruEnabled">bool</param>
        /// <param name="filterType">string</param>
        /// <param name="filterValue">string</param>
        /// <returns>List of RiskIndexExposures data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RiskIndexExposuresData> RetrieveRiskIndexExposuresData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate,
            bool isExCashSecurity, bool lookThruEnabled, string filterType, string filterValue)
        {
            try
            {
                List<RiskIndexExposuresData> result = new List<RiskIndexExposuresData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                { return result; }

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                { throw new Exception(); }

                #region Local Variables
                decimal? portfolioMomentum = 0;
                decimal? portfolioVolatility = 0;
                decimal? portfolioValue = 0;
                decimal? portfolioSize = 0;
                decimal? portfolioSizeNonLinear = 0;
                decimal? portfolioGrowth = 0;
                decimal? portfolioLiquidity = 0;
                decimal? portfolioLeverage = 0;
                decimal? benchmarkMomentum = 0;
                decimal? benchmarkVolatility = 0;
                decimal? benchmarkValue = 0;
                decimal? benchmarkSize = 0;
                decimal? benchmarkSizeNonLinear = 0;
                decimal? benchmarkGrowth = 0;
                decimal? benchmarkLiquidity = 0;
                decimal? benchmarkLeverage = 0;
                #endregion

                if (lookThruEnabled)
                {
                    #region Look thru enabled
                    //Retrieve GF_PORTFOLIO_HOLDINGS data 
                    List<GF_PORTFOLIO_LTHOLDINGS> data = GetFilteredRiskIndexListWithLookThru(portfolioSelectionData, effectiveDate, isExCashSecurity, filterType,
                                                                                                filterValue);
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    //get the summation of DIRTY_VALUE_PC used to calculate the holding's PortfolioShare 
                    decimal? sumMarketValuePortfolio = data.Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));

                    //if sum of DIRTY_VALUE_PC for criterion is zero, empty set is returned
                    if (sumMarketValuePortfolio == 0 && sumMarketValuePortfolio == null)
                    { return result; }

                    //Retrieve the Id of benchmark associated with the Portfolio
                    List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkId.Count != 1)
                    { throw new InvalidOperationException(); }

                    List<GF_BENCHMARK_HOLDINGS> benchmarkData = DimensionEntity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                    foreach (GF_PORTFOLIO_LTHOLDINGS item in data)
                    {
                        if (item.DIRTY_VALUE_PC == null)
                        { continue; }

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (item.DIRTY_VALUE_PC / sumMarketValuePortfolio) * 100;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == item.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT)
                                                                                                                              .FirstOrDefault());
                        portfolioMomentum += portfolioWeight * item.BARRA_RISK_FACTOR_MOMENTUM;
                        portfolioVolatility += portfolioWeight * item.BARRA_RISK_FACTOR_VOLATILITY;
                        portfolioValue += portfolioWeight * item.BARRA_RISK_FACTOR_VALUE;
                        portfolioSize += portfolioWeight * item.BARRA_RISK_FACTOR_SIZE;
                        portfolioSizeNonLinear += portfolioWeight * item.BARRA_RISK_FACTOR_SIZE_NONLIN;
                        portfolioGrowth += portfolioWeight * item.BARRA_RISK_FACTOR_GROWTH;
                        portfolioLiquidity += portfolioWeight * item.BARRA_RISK_FACTOR_LIQUIDITY;
                        portfolioLeverage += portfolioWeight * item.BARRA_RISK_FACTOR_LEVERAGE;

                        benchmarkMomentum += benchmarkWeight * item.BARRA_RISK_FACTOR_MOMENTUM;
                        benchmarkVolatility += benchmarkWeight * item.BARRA_RISK_FACTOR_VOLATILITY;
                        benchmarkValue += benchmarkWeight * item.BARRA_RISK_FACTOR_VALUE;
                        benchmarkSize += benchmarkWeight * item.BARRA_RISK_FACTOR_SIZE;
                        benchmarkSizeNonLinear += benchmarkWeight * item.BARRA_RISK_FACTOR_SIZE_NONLIN;
                        benchmarkGrowth += benchmarkWeight * item.BARRA_RISK_FACTOR_GROWTH;
                        benchmarkLiquidity += benchmarkWeight * item.BARRA_RISK_FACTOR_LIQUIDITY;
                        benchmarkLeverage += benchmarkWeight * item.BARRA_RISK_FACTOR_LEVERAGE;
                    }
                    #endregion
                }
                else
                {
                    #region Look thru disabled
                    //Retrieve GF_PORTFOLIO_HOLDINGS data 
                    List<GF_PORTFOLIO_HOLDINGS> data = GetFilteredRiskIndexListWithoutLookThru(portfolioSelectionData, effectiveDate, isExCashSecurity, filterType
                                                                                                    , filterValue);
                    if (data == null || data.Count.Equals(0))
                    { return result; }

                    //get the summation of DIRTY_VALUE_PC used to calculate the holding's PortfolioShare 
                    decimal? sumMarketValuePortfolio = data.Sum(t => Convert.ToDecimal(t.DIRTY_VALUE_PC));

                    //if sum of DIRTY_VALUE_PC for criterion is zero, empty set is returned
                    if (sumMarketValuePortfolio == 0 && sumMarketValuePortfolio == null)
                    { return result; }

                    //Retrieve the Id of benchmark associated with the Portfolio
                    List<string> benchmarkId = data.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkId.Count != 1)
                    { throw new InvalidOperationException(); }

                    List<GF_BENCHMARK_HOLDINGS> benchmarkData = DimensionEntity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();

                    foreach (GF_PORTFOLIO_HOLDINGS item in data)
                    {
                        if (item.DIRTY_VALUE_PC == null)
                        { continue; }

                        //Calculate Portfolio Weight
                        decimal? portfolioWeight = (item.DIRTY_VALUE_PC / sumMarketValuePortfolio) * 100;

                        //Retrieve Benchmark Weight
                        decimal? benchmarkWeight = Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == item.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT)
                                                                                  .FirstOrDefault());
                        portfolioMomentum += portfolioWeight * item.BARRA_RISK_FACTOR_MOMENTUM;
                        portfolioVolatility += portfolioWeight * item.BARRA_RISK_FACTOR_VOLATILITY;
                        portfolioValue += portfolioWeight * item.BARRA_RISK_FACTOR_VALUE;
                        portfolioSize += portfolioWeight * item.BARRA_RISK_FACTOR_SIZE;
                        portfolioSizeNonLinear += portfolioWeight * item.BARRA_RISK_FACTOR_SIZE_NONLIN;
                        portfolioGrowth += portfolioWeight * item.BARRA_RISK_FACTOR_GROWTH;
                        portfolioLiquidity += portfolioWeight * item.BARRA_RISK_FACTOR_LIQUIDITY;
                        portfolioLeverage += portfolioWeight * item.BARRA_RISK_FACTOR_LEVERAGE;

                        benchmarkMomentum += benchmarkWeight * item.BARRA_RISK_FACTOR_MOMENTUM;
                        benchmarkVolatility += benchmarkWeight * item.BARRA_RISK_FACTOR_VOLATILITY;
                        benchmarkValue += benchmarkWeight * item.BARRA_RISK_FACTOR_VALUE;
                        benchmarkSize += benchmarkWeight * item.BARRA_RISK_FACTOR_SIZE;
                        benchmarkSizeNonLinear += benchmarkWeight * item.BARRA_RISK_FACTOR_SIZE_NONLIN;
                        benchmarkGrowth += benchmarkWeight * item.BARRA_RISK_FACTOR_GROWTH;
                        benchmarkLiquidity += benchmarkWeight * item.BARRA_RISK_FACTOR_LIQUIDITY;
                        benchmarkLeverage += benchmarkWeight * item.BARRA_RISK_FACTOR_LEVERAGE;
                    }
                    #endregion
                }
                result.Add(new RiskIndexExposuresData()
                {
                    EntityType = "Portfolio",
                    Momentum = portfolioMomentum,
                    Volatility = portfolioVolatility,
                    Value = portfolioValue,
                    Size = portfolioSize,
                    SizeNonLinear = portfolioSizeNonLinear,
                    Growth = portfolioGrowth,
                    Liquidity = portfolioLiquidity,
                    Leverage = portfolioLeverage
                });
                result.Add(new RiskIndexExposuresData()
                {
                    EntityType = "Benchmark",
                    Momentum = benchmarkMomentum,
                    Volatility = benchmarkVolatility,
                    Value = benchmarkValue,
                    Size = benchmarkSize,
                    SizeNonLinear = benchmarkSizeNonLinear,
                    Growth = benchmarkGrowth,
                    Liquidity = benchmarkLiquidity,
                    Leverage = benchmarkLeverage
                });
                result.Add(new RiskIndexExposuresData()
                {
                    EntityType = "Relative",
                    Momentum = portfolioMomentum - benchmarkMomentum,
                    Volatility = portfolioVolatility - benchmarkVolatility,
                    Value = portfolioValue - benchmarkValue,
                    Size = portfolioSize - benchmarkSize,
                    SizeNonLinear = portfolioSizeNonLinear - benchmarkSizeNonLinear,
                    Growth = portfolioGrowth - benchmarkGrowth,
                    Liquidity = portfolioLiquidity - benchmarkLiquidity,
                    Leverage = portfolioLeverage - benchmarkLeverage
                });
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
        public List<FilterSelectionData> RetrieveFilterSelectionData(PortfolioSelectionData selectedPortfolio, DateTime? effectiveDate)
        {
            try
            {
                /* TODO
                
                // use cache if available
                var fromCache = (List<FilterSelectionData>)new DefaultCacheProvider().Get(CacheKeyNames.FilterSelectionDataCache);
                if (fromCache != null)
                    return fromCache;
                 */

                // otherwise fetch the data and cache it
                if (selectedPortfolio == null)
                    return new List<FilterSelectionData>();
                if (effectiveDate == null)
                    return new List<FilterSelectionData>();

                List<FilterSelectionData> result = new List<FilterSelectionData>();

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> data = DimensionEntity.GF_PORTFOLIO_HOLDINGS
                    .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date)
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

                //new DefaultCacheProvider().Set(CacheKeyNames.FilterSelectionDataCache, result, Int32.Parse(ConfigurationManager.AppSettings["SecuritiesCacheTime"]));

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

        /// <summary>
        /// Service to return data for PortfolioDetailsUI
        /// </summary>
        /// <param name="objPortfolioIdentifier">Portfolio IDentifier</param>
        /// <param name="effectiveDate">Selected Date</param>
        /// <param name="objGetBenchmark">bool to check whether to get Benchmark data or not</param>
        /// <returns>List of type Portfolio Details Data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PortfolioDetailsData> RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled, bool excludeCash = false, bool objGetBenchmark = false)
        {
#if DEBUG
            // Stopwatch
            Stopwatch swRetrievePortfolioDetailsData = new Stopwatch();
            DateTime timeRetrievePortfolioDetailsData = new DateTime();
            Stopwatch swGF_PORTFOLIO_LTHOLDINGS = new Stopwatch();
            DateTime timeGF_PORTFOLIO_LTHOLDINGS = new DateTime();
            Stopwatch swGF_BENCHMARK_HOLDINGS = new Stopwatch();
            DateTime timeGF_BENCHMARK_HOLDINGS = new DateTime();
            Stopwatch swAddPortfolioSecurities = new Stopwatch();
            DateTime timeAddPortfolioSecurities = new DateTime();
            Stopwatch swRetrieveExternalResearchData = new Stopwatch();
            DateTime timeRetrieveExternalResearchData = new DateTime();

            swRetrievePortfolioDetailsData.Start();
            timeRetrievePortfolioDetailsData = DateTime.Now;
#endif

            // do operations 
            try
            {
                List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();
                if ((objPortfolioIdentifier == null) || (effectiveDate == null))
                {
                    return result;
                }
                if (objPortfolioIdentifier.PortfolioId == null)
                {
                    return result;
                }
                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData;
                List<DimensionEntitiesService.GF_PORTFOLIO_LTHOLDINGS> dimensionPortfolioLTHoldingsData;
                List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData;
                Boolean isFiltered = false;


                dimensionPortfolioLTHoldingsData = null;
                if (lookThruEnabled)
                {
                    #region LookThru
                    if (excludeCash)
                    {
                        if (filterType != null && filterValue != null)
                        {
                            
                            isFiltered = true;
                            dimensionPortfolioHoldingsData = null;
                            switch (filterType)
                            {
                                case "Region":
                                        dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                        && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") &&(a.ASHEMM_PROP_REGION_CODE==filterValue)).ToList();
                                        break;
                                case "Country":
                                        dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                        && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") && (a.ISO_COUNTRY_CODE == filterValue)).ToList();
                                        
                                        break;
                                case "Industry":
                                     dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                        && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") && (a.GICS_INDUSTRY_NAME == filterValue)).ToList();
                                        
                                        break;
                                case "Sector":
                                     dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                        && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") && (a.GICS_SECTOR_NAME == filterValue) ).ToList();
                                        
                                        break;
                                case "Show Everything":
                                        dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                        && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                        isFiltered = false; //filter type should be set to false
                                        break;


                            }
                        }
                        else
                        {
                            dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                           && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                        }
                    }
                    else
                    {
#if DEBUG
                        swGF_PORTFOLIO_LTHOLDINGS.Start();
#endif

                        if (filterType != null && filterValue != null)
                        {
                            isFiltered = true;
                            dimensionPortfolioHoldingsData = null;
                            switch (filterType)
                            {
                                case "Region":
                                    dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                    && (a.PORTFOLIO_DATE == effectiveDate.Date)  && (a.ASHEMM_PROP_REGION_CODE == filterValue)).ToList();
                                    break;
                                case "Country":
                                    dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                    && (a.PORTFOLIO_DATE == effectiveDate.Date)  && (a.ISO_COUNTRY_CODE == filterValue)).ToList();
                                    break;
                                case "Industry":
                                    dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                       && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.GICS_INDUSTRY_NAME == filterValue)).ToList();
                                    break;
                                case "Sector":
                                    dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                       && (a.PORTFOLIO_DATE == effectiveDate.Date)  && (a.GICS_SECTOR_NAME == filterValue)).ToList();
                                    break;
                                case "Show Everything":
                                    dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                    && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    isFiltered = false; //filter type should be set to false
                                    break;


                            }
                        }
                        else
                        {
                            dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                        }
#if DEBUG
                        swGF_PORTFOLIO_LTHOLDINGS.Stop();
                        timeGF_PORTFOLIO_LTHOLDINGS = DateTime.Now;
#endif
                    }

                    //if service returned empty set
                    if (dimensionPortfolioLTHoldingsData.Count == 0)
                    {
                        return result;
                    }
                    //retrieve the id of benchmark associated with the portfolio
                    List<string> benchmarkIdLT = dimensionPortfolioLTHoldingsData.Select(a => a.BENCHMARK_ID).Distinct().ToList();
                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkIdLT.Count != 1)
                    {
                        throw new InvalidOperationException("More than 1 Benchmark is assigned to the Selected Portfolio" + objPortfolioIdentifier.PortfolioId.ToUpper().ToString());
                    }
                      dimensionBenchmarkHoldingsData = null;
                    if (excludeCash)
                    {
                        if (filterType != null && filterValue != null)
                        {
                            isFiltered = true;

                            switch (filterType)
                            {
                                case "Region":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") && (a.ASHEMM_PROP_REGION_CODE == filterValue)).ToList();
                                    break;
                                case "Country":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") && (a.ISO_COUNTRY_CODE == filterValue)).ToList();
                                    break;
                                case "Industry":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") && (a.GICS_INDUSTRY_NAME == filterValue)).ToList();
                                    break;
                                case "Sector":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH") && (a.GICS_SECTOR_NAME == filterValue)).ToList();
                                    break;
                                case "Show Everything":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    isFiltered = false; //filter type should be set to false
                                    break;

                            }
                        }
                        else
                        {
                            dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                            (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                        }
                        
                    }
                    else
                    {
#if DEBUG
                        swGF_BENCHMARK_HOLDINGS.Start();
#endif
                        if (filterType != null && filterValue != null)
                        {
                            isFiltered = true;

                            switch (filterType)
                            {
                                case "Region":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.ASHEMM_PROP_REGION_CODE == filterValue)).ToList();
                                    break;
                                case "Country":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.ISO_COUNTRY_CODE == filterValue)).ToList();
                                    break;
                                case "Industry":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.GICS_INDUSTRY_NAME == filterValue)).ToList();
                                    break;
                                case "Sector":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.GICS_SECTOR_NAME == filterValue)).ToList();
                                    break;
                                case "Show Everything":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                                    (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                                    isFiltered = false; //filter type should be set to false
                                    break;

                            }
                        }
                        else
                        {
                            dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) &&
                            (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                        }

                       // dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.
                         //              Where(a => (a.BENCHMARK_ID == benchmarkIdLT.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
#if DEBUG
                        swGF_BENCHMARK_HOLDINGS.Stop();
                        timeGF_BENCHMARK_HOLDINGS = DateTime.Now;
#endif
                    }
                    result = PortfolioDetailsCalculations.AddPortfolioLTSecurities(dimensionPortfolioLTHoldingsData, dimensionBenchmarkHoldingsData,isFiltered);

                    #region BenchmarkSecurities

                    if (objGetBenchmark)
                    {
                        List<string> portfolioSecurityID = dimensionPortfolioLTHoldingsData.Select(a => a.ASEC_SEC_SHORT_NAME).ToList();
                        List<GF_BENCHMARK_HOLDINGS> onlyBenchmarkSecurities = dimensionBenchmarkHoldingsData.Where(a => !portfolioSecurityID.Contains(a.ASEC_SEC_SHORT_NAME)).ToList();
                        result = PortfolioDetailsCalculations.AddBenchmarkSecurities(result, onlyBenchmarkSecurities);
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    #region NonLookThru
                    if (excludeCash)
                    {
                        if (filterType != null && filterValue != null)
                        {
                            isFiltered = true;
                            dimensionPortfolioHoldingsData = null;
                            switch (filterType)
                            {
                                case "Region":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() && a.ASHEMM_PROP_REGION_CODE== filterValue)
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                                case "Country":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() && a.ISO_COUNTRY_CODE== filterValue)
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                                case "Industry":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() && a.GICS_INDUSTRY_NAME == filterValue)
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;

                                case "Sector":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() && a.GICS_SECTOR_NAME == filterValue)
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                                case "Show Everything":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() )
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    isFiltered = false; //filter type should be set to false
                                    break;
                                default: dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                            }
                        }
                        else
                        {

                            dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                    && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                        }
                    }
                    else
                    {
#if DEBUG
                        swGF_PORTFOLIO_LTHOLDINGS.Start();
#endif
                        if (filterType != null && filterValue != null)
                        {
                            isFiltered = true;
                            dimensionPortfolioHoldingsData = null;
                            switch (filterType)
                            {
                                case "Region":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                    && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.ASHEMM_PROP_REGION_CODE== filterValue)).ToList();
                                    break;
                                case "Country":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() && a.ISO_COUNTRY_CODE == filterValue)
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                                    break;
                                case "Industry":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() && a.GICS_INDUSTRY_NAME == filterValue)
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                                    break;

                                case "Sector":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper() && a.GICS_SECTOR_NAME == filterValue)
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                                    break;
                                case "Show Everything":
                                    dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                                    isFiltered = false; //filter type should be set to false
                                    break;
                                default: dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                      && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper())
                                    && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                            
                        }
#if DEBUG
                        swGF_PORTFOLIO_LTHOLDINGS.Stop();
                        timeGF_PORTFOLIO_LTHOLDINGS = DateTime.Now;
#endif
                    }

                    if (dimensionPortfolioHoldingsData == null)
                    {
                        return new List<PortfolioDetailsData>();
                    }
                    //if service returned empty set
                    if (dimensionPortfolioHoldingsData.Count == 0)
                    {
                        return result;
                    }
                    //retrieve the id of benchmark associated with the portfolio
                    List<string> benchmarkId = dimensionPortfolioHoldingsData.Select(a => a.BENCHMARK_ID).Distinct().ToList();
                    //if the database doesn't return a single benchmark for a portfolio
                    if (benchmarkId.Count != 1)
                    {
                        throw new InvalidOperationException("More than 1 Benchmark is found for the Selected Portfolio: " + objPortfolioIdentifier.PortfolioId);
                    }
                    dimensionBenchmarkHoldingsData = null;
                    if (excludeCash)
                    {
                        
                        if (filterType != null && filterValue != null)
                        {
                            isFiltered = true;
                            switch (filterType)
                            {
                                case "Region":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.ASHEMM_PROP_REGION_CODE == filterValue)
                                    && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                                case "Country":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.ISO_COUNTRY_CODE == filterValue)
                                    && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                                case "Industry":
                                     dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.GICS_INDUSTRY_NAME == filterValue)
                                    && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                                case "Sector":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.GICS_SECTOR_NAME == filterValue)
                                    && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    break;
                                case "Show Everything":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) 
                                    && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                                    isFiltered = false; //filter type should be set to false
                                    break;
                        
                            }
                        }

                        else
                        {
                            dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)
                           && (a.SECURITYTHEMECODE.ToUpper() != "CASH")).ToList();
                        }
                    }
                    else
                    {
#if DEBUG
                        swGF_BENCHMARK_HOLDINGS.Start();
#endif
                        if (filterType != null && filterValue != null)
                        {
                            isFiltered = true;
                            switch (filterType)
                            {
                                case "Region":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.ASHEMM_PROP_REGION_CODE == filterValue)).ToList();
                                    break;
                                case "Country":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.ISO_COUNTRY_CODE == filterValue)).ToList();
                                    break;
                                case "Industry":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.GICS_INDUSTRY_NAME == filterValue)).ToList();
                                    break;
                                case "Sector":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.GICS_SECTOR_NAME == filterValue)).ToList();
                                    break;
                                case "Show Everything":
                                    dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                                    isFiltered = false; //filter type should be set to false
                                    break;

                            }
                        }
                        else
                        {

                            dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.
                                    Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                        }
#if DEBUG
                        swGF_BENCHMARK_HOLDINGS.Stop();
                        timeGF_BENCHMARK_HOLDINGS = DateTime.Now;
#endif
                    }
#if DEBUG
                    swAddPortfolioSecurities.Start();
#endif
                    result = PortfolioDetailsCalculations.AddPortfolioSecurities(dimensionPortfolioHoldingsData, dimensionBenchmarkHoldingsData, isFiltered);
#if DEBUG
                    swAddPortfolioSecurities.Stop();
                    timeAddPortfolioSecurities = DateTime.Now;
#endif
                    // set portfolio for each record to current portfolio
                    result.ForEach(r =>
                    {
                        r.PortfolioPath = objPortfolioIdentifier.PortfolioId;
                        r.PfcHoldingPortfolio = objPortfolioIdentifier.PortfolioId;
                    });
                    
                    

                    #region BenchmarkSecurities

                    if (objGetBenchmark)
                    {
                        List<string> portfolioSecurityID = dimensionPortfolioHoldingsData.Select(a => a.ASEC_SEC_SHORT_NAME).ToList();
                        List<GF_BENCHMARK_HOLDINGS> onlyBenchmarkSecurities = dimensionBenchmarkHoldingsData.Where(a => !portfolioSecurityID.Contains(a.ASEC_SEC_SHORT_NAME)).ToList();
                        result = PortfolioDetailsCalculations.AddBenchmarkSecurities(result, onlyBenchmarkSecurities);
                    }

                    #endregion

                    #endregion
                }

#if DEBUG
                Trace.WriteLine(string.Format("\n1. _____________________________________________________________________________________"));
                Trace.WriteLine(string.Format("{1}: RetrievePortfolioDetailsData for {0}\n", objPortfolioIdentifier.PortfolioId, timeRetrievePortfolioDetailsData.ToString()));
                Trace.WriteLine(string.Format("{1}: 1. Dimension: GF_PORTFOLIO_LTHOLDINGS = {0} seconds.", (swGF_PORTFOLIO_LTHOLDINGS.ElapsedMilliseconds / 1000.00).ToString(), timeGF_PORTFOLIO_LTHOLDINGS.ToString()));
                Trace.WriteLine(string.Format("{1}: 2. Dimension: GF_BENCHMARK_HOLDINGS = {0} seconds.", (swGF_BENCHMARK_HOLDINGS.ElapsedMilliseconds / 1000.00).ToString(), timeGF_BENCHMARK_HOLDINGS.ToString()));
                Trace.WriteLine(string.Format("{1}: 3. AddPortfolioSecurities = {0} seconds.", (swAddPortfolioSecurities.ElapsedMilliseconds / 1000.00).ToString(), timeAddPortfolioSecurities.ToString()));

                swRetrieveExternalResearchData.Start();
                //Trace.WriteLine(string.Format("{0}: Passed to RetrieveExternalResearchData", DateTime.Now));
                //XMLStringValue(result);
#endif
                result = RetrieveExternalResearchData(result, effectiveDate, filterType, filterValue, lookThruEnabled,excludeCash, objGetBenchmark);
#if DEBUG
                //Trace.WriteLine(string.Format("{0}: returned from RetrieveExternalResearchData", DateTime.Now));
                //Trace.WriteLine("");
                //XMLStringValue(result);

                swRetrieveExternalResearchData.Stop();
                timeRetrieveExternalResearchData = DateTime.Now;
                // StopWatch
                swRetrievePortfolioDetailsData.Stop();
                Trace.WriteLine(string.Format("{1}: 4. RetrieveExternalResearchData = {0} seconds.", (swRetrieveExternalResearchData.ElapsedMilliseconds / 1000.00).ToString(), timeRetrieveExternalResearchData.ToString()));
                Trace.WriteLine(string.Format("\n{1}: Total time = {0} seconds.", (swRetrievePortfolioDetailsData.ElapsedMilliseconds / 1000.00).ToString(), DateTime.Now.ToString()));
#endif
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

    

        private static void XMLStringValue(List<PortfolioDetailsData> result)
        {
            XmlSerializer XmlS = new XmlSerializer(result.GetType());
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = new XmlTextWriter(sw);
            XmlS.Serialize(tw, result);
            Trace.Write(sw.ToString());
        }

        /// <summary>
        /// Method to Retreive Asset Allocation Data
        /// </summary>
        /// <param name="portfolioSelectionData">Details of Selected Portfolio</param>
        /// <param name="effectiveDate">The Selected Date</param>
        /// <returns>List of AssetAllocationData</returns>
        [OperationContract]
        public List<AssetAllocationData> RetrieveAssetAllocationData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool lookThruEnabled, bool excludeCash)
        {
            try
            {
                List<AssetAllocationData> result = new List<AssetAllocationData>();
                //arguement null exception
                if ((portfolioSelectionData == null) || (effectiveDate == null))
                {
                    return result;
                }
                DimensionEntitiesService.Entities entity = DimensionEntity;
                //arguement null exception
                if (entity == null)
                {
                    return result;
                }
                List<GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData;
                List<GF_PORTFOLIO_LTHOLDINGS> dimensionPortfolioLTHoldingsData;
                if (lookThruEnabled)
                {
                    #region LookThruEnabled

                    if (excludeCash)
                    {
                        dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.
                                                            Where(a => (a.PORTFOLIO_ID.ToUpper().Trim() == portfolioSelectionData.PortfolioId.ToUpper().Trim())
                                                                && (a.PORTFOLIO_DATE == effectiveDate.Date) && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).ToList();
                    }
                    else
                    {
                        dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.
                                                            Where(a => (a.PORTFOLIO_ID.ToUpper().Trim() == portfolioSelectionData.PortfolioId.ToUpper().Trim())
                                                                && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                    }

                    if (dimensionPortfolioHoldingsData == null)
                    {
                        return result;
                    }
                    if (dimensionPortfolioHoldingsData.Count == 0)
                    {
                        return result;
                    }
                    //retrieve the id of benchmark associated with the portfolio
                    List<string> benchmarkId = dimensionPortfolioHoldingsData.Select(a => a.BENCHMARK_ID).Distinct().ToList();
                    //if the database doesn't return a single benchmark for a portfolio
                    if (benchmarkId.Count != 1)
                    {
                        throw new InvalidOperationException();
                    }
                    List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && ((a.PORTFOLIO_DATE) == effectiveDate.Date)).ToList();
                    result = AssetAllocationCalculations.CalculateAssetAllocationValues(dimensionPortfolioHoldingsData, dimensionBenchmarkHoldingsData, portfolioSelectionData);

                    #endregion
                }
                else
                {
                    #region LookThruDisabled

                    if (excludeCash)
                    {
                        dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.
                                                Where(a => (a.PORTFOLIO_ID == portfolioSelectionData.PortfolioId) && (a.PORTFOLIO_DATE == effectiveDate.Date)
                                                    && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).ToList();
                    }
                    else
                    {
                        dimensionPortfolioLTHoldingsData = entity.GF_PORTFOLIO_LTHOLDINGS.
                                                Where(a => (a.PORTFOLIO_ID == portfolioSelectionData.PortfolioId) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                    }

                    if (dimensionPortfolioLTHoldingsData.Count == 0)
                    {
                        return result;
                    }
                    //retrieve the id of benchmark associated with the portfolio
                    List<string> benchmarkId = dimensionPortfolioLTHoldingsData.Select(a => a.BENCHMARK_ID).Distinct().ToList();

                    //If the DataBase doesn't return a single Benchmark for a Portfolio
                    if (benchmarkId.Count != 1)
                    {
                        throw new InvalidOperationException();
                    }
                    List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData = entity.GF_BENCHMARK_HOLDINGS.
                        Where(a => (a.BENCHMARK_ID == benchmarkId.First()) && ((a.PORTFOLIO_DATE) == effectiveDate.Date)).ToList();
                    result = AssetAllocationCalculations.CalculateAssetAllocationValuesLT(dimensionPortfolioLTHoldingsData, dimensionBenchmarkHoldingsData, portfolioSelectionData);

                    #endregion
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

        #region HelperMethods

      /*  /// <summary>
        /// Method to retrieve External Research Data for Portfolio Details
        /// </summary>
        /// <param name="portfolioDetailsData">Collection of PortfolioDetailsData</param>
        /// <returns>Collection of PortfolioDetailsData</returns>
        private List<PortfolioDetailsData> RetrieveExternalResearchData(List<PortfolioDetailsData> portfolioDetailsData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled, bool excludeCash = false, bool objGetBenchmark=false)
        {
            try
            {
#if DEBUG
                // Stopwatch
                Stopwatch swRetrieveExt = new Stopwatch();
                DateTime timeRetrieveExt = new DateTime();
                Stopwatch swPortfolio_Security_Targets_Union = new Stopwatch();
                DateTime timePortfolio_Security_Targets_Union = new DateTime();
                Stopwatch swGF_SECURITY_BASEVIEW_Local = new Stopwatch();
                DateTime timeGF_SECURITY_BASEVIEW_Local = new DateTime();
                Stopwatch swGetPortfolioDetailsExternalData = new Stopwatch();
                DateTime timeGetPortfolioDetailsExternalData = new DateTime();
                Stopwatch swRetrieveSecurityReferenceData = new Stopwatch();
                DateTime timeRetrieveSecurityReferenceData = new DateTime();
                Stopwatch swGetPortfolioDetailsFairValue = new Stopwatch();
                DateTime timeGetPortfolioDetailsFairValue = new DateTime();
                
                swRetrieveExt.Start();
                timeRetrieveExt = DateTime.Now;
#endif

                var portfolios = portfolioDetailsData.Select(x => x.PfcHoldingPortfolio).Distinct().ToList();
                var externalResearchEntities = new GreenField.DAL.ExternalResearchEntities();
#if DEBUG
                swPortfolio_Security_Targets_Union.Start();
#endif
                var targets = externalResearchEntities.Portfolio_Security_Targets_Union.Where(x => portfolios.Contains(x.PORTFOLIO_ID)).ToList();
                
#if DEBUG
                swPortfolio_Security_Targets_Union.Stop();
                timePortfolio_Security_Targets_Union = DateTime.Now;
                swGF_SECURITY_BASEVIEW_Local.Start();
#endif
                //var securities = externalResearchEntities.GF_SECURITY_BASEVIEW_Local.ToList();

#if DEBUG
                swGF_SECURITY_BASEVIEW_Local.Stop();
                timeGF_SECURITY_BASEVIEW_Local = DateTime.Now;
#endif
                
                //List<SecurityBaseviewData> securityData = new List<SecurityBaseviewData>();
#if DEBUG
                swRetrieveSecurityReferenceData.Start();
#endif
                //securityData = RetrieveSecurityReferenceData(securities);
#if DEBUG
                swRetrieveSecurityReferenceData.Stop();
                timeRetrieveSecurityReferenceData = DateTime.Now;
#endif
                ExternalResearchEntities entity = new ExternalResearchEntities() { CommandTimeout = 5000 };
                List<string> securityAsecSecShortName = portfolioDetailsData.Select(a => a.AsecSecShortName).ToList();
                List<PortfolioDetailsExternalData> externalData = new List<PortfolioDetailsExternalData>();
                List<FAIR_VALUE> fairValueData = new List<FAIR_VALUE>();
                int check = 1;
                StringBuilder securityIDPortfolio = new StringBuilder();
                StringBuilder issuerIDPortfolio = new StringBuilder();

                SecurityReferenceOperations securityReferenceOperations = new SecurityReferenceOperations();
                List<EntitySelectionData> newSecurities = securityReferenceOperations.RetrieveSecuritiesData();

                foreach (String asecSecShortName in securityAsecSecShortName)
                {
                    //SecurityBaseviewData securityDetails = securityData.Where(record => record.IssueName == issueName).FirstOrDefault();
                    var securityDetails = newSecurities.Where(record => record.ShortName == asecSecShortName).FirstOrDefault();

                    if (securityDetails != null)
                    {
                        check = 0;
                        securityIDPortfolio.Append(",'" + securityDetails.SecurityId + "'");
                        issuerIDPortfolio.Append(",'" + securityDetails.IssuerId + "'");
                        if (portfolioDetailsData.Where(a => a.AsecSecShortName == asecSecShortName).FirstOrDefault() != null)
                        {
                            portfolioDetailsData.Where(a => a.AsecSecShortName == asecSecShortName).FirstOrDefault().SecurityId = Convert.ToString(securityDetails.SecurityId);
                        }
                    }
                }
                issuerIDPortfolio = check == 0 ? issuerIDPortfolio.Remove(0, 1) : null;
                securityIDPortfolio = check == 0 ? securityIDPortfolio.Remove(0, 1) : null;
                string _issuerIDPortfolio = issuerIDPortfolio == null ? null : issuerIDPortfolio.ToString();
                string _securityIDPortfolio = securityIDPortfolio == null ? null : securityIDPortfolio.ToString();

#if DEBUG
                swGetPortfolioDetailsExternalData.Start();
#endif
                externalData = entity.GetPortfolioDetailsExternalData(_issuerIDPortfolio, _securityIDPortfolio).ToList();
#if DEBUG
                swGetPortfolioDetailsExternalData.Stop();
                timeGetPortfolioDetailsExternalData = DateTime.Now;

                swGetPortfolioDetailsFairValue.Start();
#endif
                fairValueData = GetPortfolioDetailsFairValue(_securityIDPortfolio);
#if DEBUG
                swGetPortfolioDetailsFairValue.Stop();
                timeGetPortfolioDetailsFairValue = DateTime.Now;
#endif

                if (fairValueData == null)
                {
                    fairValueData = new List<FAIR_VALUE>();
                }
                foreach (PortfolioDetailsData item in portfolioDetailsData)
                {
                    item.MarketCap = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 185).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 185).FirstOrDefault().Amount;

                    item.ForwardPE = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 187).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 187).FirstOrDefault().Amount;

                    item.ForwardPBV = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 188).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 188).FirstOrDefault().Amount;

                    item.ForwardEB_EBITDA = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 198).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 198).FirstOrDefault().Amount;

                    item.RevenueGrowthCurrentYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == DateTime.Today.Year).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == DateTime.Today.Year).FirstOrDefault().Amount * 100M;

                    item.RevenueGrowthNextYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault() ==
                        null ? null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault().Amount * 100M;

                    item.NetIncomeGrowthCurrentYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ? null :
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.NetIncomeGrowthNextYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault().Amount * 100M;

                    item.ROE = externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 133 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 133 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.NetDebtEquity = externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 149 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 149 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount;

                    item.FreecashFlowMargin =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 146 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 146 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.Upside = fairValueData.Where(a => a.SECURITY_ID == item.SecurityId).FirstOrDefault() == null ?
                        null : (fairValueData.Where(a => a.SECURITY_ID == item.SecurityId).FirstOrDefault().UPSIDE as decimal?) * 100M;

                    var security = newSecurities.Where(x => x.ShortName == item.AsecSecShortName).FirstOrDefault();

                    item.AshEmmModelWeight = 0;
                    if (security != null)
                    {

                        var target = targets.Where(x => x.SECURITY_ID == security.SecurityId && x.PORTFOLIO_ID == item.PfcHoldingPortfolio);
                        if (target != null)
                            item.AshEmmModelWeight = target.Sum(x => x.TARGET_PCT);

                        if (item.PfcHoldingPortfolio != item.PortfolioPath)
                        {
                            var securityPortfolios = item.PortfolioPath.Split(',');
                            for (int i = securityPortfolios.Count() - 2; i >= 0; i--)
                            {
                                security = newSecurities.Where(x => x.LOOK_THRU_FUND == securityPortfolios[i + 1]).FirstOrDefault();
                                if (security != null)
                                {
                                    target = targets.Where(x => x.SECURITY_ID == security.SecurityId && x.PORTFOLIO_ID == securityPortfolios[i]);
                                    if (target != null)
                                        item.AshEmmModelWeight = item.AshEmmModelWeight * target.Sum(x => x.TARGET_PCT);
                                }
                                else
                                {
                                    throw new ApplicationException("Unknown look through fund security (LOOK_THRU_FUND: " + securityPortfolios[i] + ")");
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    throw new ApplicationException("Unknown security (short name: " + item.AsecSecShortName + ")");
                    //}
                }
#if DEBUG
                // StopWatch
                swRetrieveExt.Stop();
                Trace.WriteLine(string.Format("\t\t\t{0}: RetrieveExternalResearchData start\n", timeRetrieveExt.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 1. AIMS_Main: Portfolio_Security_Targets_Union = {0} seconds.", (swPortfolio_Security_Targets_Union.ElapsedMilliseconds / 1000.00).ToString(), timePortfolio_Security_Targets_Union.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 2. AIMS_Main: GF_SECURITY_BASEVIEW_Local = {0} seconds.", (swGF_SECURITY_BASEVIEW_Local.ElapsedMilliseconds / 1000.00).ToString(), timeGF_SECURITY_BASEVIEW_Local.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 3. RetrieveSecurityReferenceData = {0} seconds.", (swRetrieveSecurityReferenceData.ElapsedMilliseconds / 1000.00).ToString(), timeRetrieveSecurityReferenceData.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 4. AIMS_Main: GetPortfolioDetailsExternalData = {0} seconds.", (swGetPortfolioDetailsExternalData.ElapsedMilliseconds / 1000.00).ToString(), timeGetPortfolioDetailsExternalData.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 5. AIMS_Main: GetPortfolioDetailsFairValue = {0} seconds.", (swGetPortfolioDetailsFairValue.ElapsedMilliseconds / 1000.00).ToString(), timeGetPortfolioDetailsFairValue.ToString()));
                Trace.WriteLine(string.Format("\n\t\t\t{1}: Total time = {0} seconds.", (swRetrieveExt.ElapsedMilliseconds / 1000.00).ToString(), DateTime.Now.ToString()));
#endif

                return portfolioDetailsData;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        */




        private List<PortfolioDetailsData> RetrieveExternalResearchData(List<PortfolioDetailsData> portfolioDetailsData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled, bool excludeCash = false, bool objGetBenchmark = false)
        {
            try
            {


                var portfolios = portfolioDetailsData.Select(x => x.PortfolioId).Distinct().ToList();
                var securityWithPortfolioPath = portfolioDetailsData.Select(x => new { x.AsecSecShortName, x.PortfolioPath }).Distinct().ToList();


                var externalResearchEntities = new GreenField.DAL.ExternalResearchEntities();
                //var targets = externalResearchEntities.Portfolio_Security_Targets_Union.Where(x => portfolios.Contains(x.PORTFOLIO_ID)).ToList();
                //var targets = externalResearchEntities.Portfolio_Target_Security_Baseview.Where(x => portfolios.Contains(x.PORTFOLIO_ID)).ToList();
                var targets = new List<Portfolio_Security_Target_Baseview>();
                var portfolioSecuritiesBaseviewList = externalResearchEntities.Portfolio_Security_Target_Baseview.ToList();
                decimal sumTargetPct = 0;
                #region    CalculatelookthruNumbers 
                //this region is used to get the look through numbers which will be used to calculate model%
                Hashtable lookthruHash =new Hashtable();
                if (lookThruEnabled)
                {
                    Hashtable ht = new Hashtable();
                    foreach (var a in securityWithPortfolioPath)
                    {
                        if (ht.ContainsKey(a.AsecSecShortName))
                        {
                            string pfPortPath = (string)ht[a.AsecSecShortName];
                            if (a.PortfolioPath.Length > pfPortPath.Length)
                            {
                                ht[a.AsecSecShortName] = a.PortfolioPath;
                            }

                        }
                        else
                        {
                            ht[a.AsecSecShortName] = a.PortfolioPath;
                        }

                    }
                    lookthruHash = CalculateLookThruNumbers(ht, filterType, filterValue, portfolioSecuritiesBaseviewList);
                }
                #endregion



                if (filterType != null && filterValue != null)
                {
                    switch (filterType)
                    {
                        case "Region":
                            targets = portfolioSecuritiesBaseviewList.Where(x => portfolios.Contains(x.PORTFOLIO_ID) && x.ASHEMM_PROPRIETARY_REGION_CODE == filterValue).ToList();
                            break;
                        case "Country":
                            targets = portfolioSecuritiesBaseviewList.Where(x => portfolios.Contains(x.PORTFOLIO_ID) && x.ISO_COUNTRY_CODE == filterValue).ToList();
                            break;
                        case "Industry":
                            targets = portfolioSecuritiesBaseviewList.Where(x => portfolios.Contains(x.PORTFOLIO_ID) && x.GICS_INDUSTRY_NAME == filterValue).ToList();
                            break;
                        case "Sector":
                            targets = portfolioSecuritiesBaseviewList.Where(x => portfolios.Contains(x.PORTFOLIO_ID) && x.GICS_SECTOR_NAME == filterValue).ToList();
                            break;
                        case "Show Everything":
                            targets = portfolioSecuritiesBaseviewList.Where(x => portfolios.Contains(x.PORTFOLIO_ID)).ToList();
                            break;
                    }
                }
                else
                {
                    targets = portfolioSecuritiesBaseviewList.Where(x => portfolios.Contains(x.PORTFOLIO_ID)).ToList();
                }
                    sumTargetPct = targets.Sum(x => x.TARGET_PCT);

#if DEBUG
                    Trace.WriteLine(sumTargetPct);
#endif         

                ExternalResearchEntities entity = new ExternalResearchEntities() { CommandTimeout = 0 };
                List<string> securityAsecSecShortName = portfolioDetailsData.Select(a => a.AsecSecShortName).ToList();
                List<PortfolioDetailsExternalData> externalData = new List<PortfolioDetailsExternalData>();
                List<FAIR_VALUE> fairValueData = new List<FAIR_VALUE>();
                int check = 1;
                StringBuilder securityIDPortfolio = new StringBuilder();
                StringBuilder issuerIDPortfolio = new StringBuilder();

                SecurityReferenceOperations securityReferenceOperations = new SecurityReferenceOperations();
                List<EntitySelectionData> newSecurities = securityReferenceOperations.RetrieveSecuritiesData();

                foreach (String asecSecShortName in securityAsecSecShortName)
                {
                    var securityDetails = newSecurities.Where(record => record.ShortName == asecSecShortName).FirstOrDefault();

                    if (securityDetails != null)
                    {
                        check = 0;
                        securityIDPortfolio.Append(",'" + securityDetails.SecurityId + "'");
                        issuerIDPortfolio.Append(",'" + securityDetails.IssuerId + "'");
                        if (portfolioDetailsData.Where(a => a.AsecSecShortName == asecSecShortName).FirstOrDefault() != null)
                        {
                            portfolioDetailsData.Where(a => a.AsecSecShortName == asecSecShortName).FirstOrDefault().SecurityId = Convert.ToString(securityDetails.SecurityId);
                        }
                    }
                }
                issuerIDPortfolio = check == 0 ? issuerIDPortfolio.Remove(0, 1) : null;
                securityIDPortfolio = check == 0 ? securityIDPortfolio.Remove(0, 1) : null;
                string _issuerIDPortfolio = issuerIDPortfolio == null ? null : issuerIDPortfolio.ToString();
                string _securityIDPortfolio = securityIDPortfolio == null ? null : securityIDPortfolio.ToString();

                externalData = entity.GetPortfolioDetailsExternalData(_issuerIDPortfolio, _securityIDPortfolio).ToList();
                fairValueData = GetPortfolioDetailsFairValue(_securityIDPortfolio);

                if (fairValueData == null)
                {
                    fairValueData = new List<FAIR_VALUE>();
                }
               
                foreach (PortfolioDetailsData item in portfolioDetailsData)
                {
                    item.MarketCap = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 185).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 185).FirstOrDefault().Amount;

                    item.ForwardPE = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 187).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 187).FirstOrDefault().Amount;

                    item.ForwardPBV = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 188).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 188).FirstOrDefault().Amount;

                    item.ForwardEB_EBITDA = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 198).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 198).FirstOrDefault().Amount;

                    item.RevenueGrowthCurrentYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == DateTime.Today.Year).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == DateTime.Today.Year).FirstOrDefault().Amount * 100M;

                    item.RevenueGrowthNextYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault() ==
                        null ? null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault().Amount * 100M;

                    item.NetIncomeGrowthCurrentYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ? null :
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.NetIncomeGrowthNextYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault().Amount * 100M;

                    item.ROE = externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 133 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 133 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.NetDebtEquity = externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 149 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 149 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount;

                    item.FreecashFlowMargin =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 146 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 146 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.Upside = fairValueData.Where(a => a.SECURITY_ID == item.SecurityId).FirstOrDefault() == null ?
                        null : (fairValueData.Where(a => a.SECURITY_ID == item.SecurityId).FirstOrDefault().UPSIDE as decimal?) * 100M;
                    
                   // var security = newSecurities.Where(x => x.ShortName == item.AsecSecShortName).FirstOrDefault();
                    item.IssuerName = externalResearchEntities.GF_SECURITY_BASEVIEW_Local.Where(x => x.ASEC_SEC_SHORT_NAME == item.AsecSecShortName).FirstOrDefault() == null ? 
                        null : externalResearchEntities.GF_SECURITY_BASEVIEW_Local.Where(x => x.ASEC_SEC_SHORT_NAME == item.AsecSecShortName).FirstOrDefault().ISSUER_NAME;
                    if (!lookThruEnabled)
                    {

                        if (filterType != null && filterValue != null)
                        {
                            if (!filterType.Equals("Show Everything")) //for everything reweight target%
                            {
                                if (sumTargetPct != 0)
                                {
                                    var target = targets.Where(x => x.ASEC_SEC_SHORT_NAME == item.AsecSecShortName);
                                    item.AshEmmModelWeight = target.Sum(x => x.TARGET_PCT) / sumTargetPct;

                                }
                                else
                                {
                                    item.AshEmmModelWeight = 0;
                                }
                            }
                            else //for show everything display as it is
                            {
                                var target = targets.Where(x => x.ASEC_SEC_SHORT_NAME == item.AsecSecShortName);
                                item.AshEmmModelWeight = target.Sum(x => x.TARGET_PCT);
                            }




                        }
                        else
                        {
                            var target = targets.Where(x => x.ASEC_SEC_SHORT_NAME == item.AsecSecShortName);
                            item.AshEmmModelWeight = target.Sum(x => x.TARGET_PCT);
                        }
                    }
                    else
                    {
                        decimal lookthrutargetProduct = 1;
                        if (item.PfcHoldingPortfolio != item.PortfolioPath)
                        {
                            var securityPortfolioPath = item.PortfolioPath.Split(',');
                            
                             for (int i = 0; i < securityPortfolioPath.Count() - 1; i++)
                             {      var portfolioId = securityPortfolioPath[i];
                                    var lookthruportfolioId = securityPortfolioPath[i + 1];
                                    var target = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == portfolioId && x.LOOK_THRU_FUND == lookthruportfolioId).ToList();
                                    if (target != null)
                                        lookthrutargetProduct = lookthrutargetProduct * target.Sum(x => x.TARGET_PCT);
                             }
                        }
   
                            List<decimal> lookthruNumbers =(List<decimal>)lookthruHash[item.AsecSecShortName];
                            if (lookthruNumbers != null)
                            {
                                ///lookthrutargetProduct
                                decimal lookthrutargetSum = lookthruNumbers[1];                                                    ///
                                var targetSecurity = portfolioSecuritiesBaseviewList.Where(x => x.ASEC_SEC_SHORT_NAME == item.AsecSecShortName && x.PORTFOLIO_ID == item.PfcHoldingPortfolio).ToList();
                                if (targetSecurity != null)
                                {
                                    if ((lookthrutargetSum + sumTargetPct) != 0)
                                    {
                                        item.AshEmmModelWeight = targetSecurity.Sum(x => x.TARGET_PCT) * lookthrutargetProduct / (lookthrutargetSum + sumTargetPct);
                                    }
                                    else
                                    {
                                        item.AshEmmModelWeight = 0;
                                    }

                                }
                                else
                                {
                                    item.AshEmmModelWeight = 0;
                                }
                            }
                        
                        

                           
                     }


                   
            
                }

                return portfolioDetailsData;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        private Hashtable CalculateLookThruNumbers(Hashtable ht, String filterType, String filterValue, List<Portfolio_Security_Target_Baseview> portfolioSecuritiesBaseviewList)
        {
            Hashtable lookthruHash = new Hashtable();
            var externalResearchEntities = new GreenField.DAL.ExternalResearchEntities();
            //var portfolioSecuritiesBaseviewList = externalResearchEntities.Portfolio_Security_Target_Baseview.ToList();
            foreach (DictionaryEntry entry in ht)
            {
                

                if (entry.Value != null)
                {
                    var securityPortfolioPath = entry.Value.ToString().Split(',');
                    decimal lookthrutargetSum = 0;
                    decimal lookthrutargetProduct = 1;
                    var targetLookThruFundSecurityList = new List<Portfolio_Security_Target_Baseview>();
                    if (securityPortfolioPath.Count() > 1)
                    {
                        for (int i = 0; i < securityPortfolioPath.Count() - 1; i++)
                        {

                            var portfolioId = securityPortfolioPath[i];
                            var lookthruportfolioId = securityPortfolioPath[i + 1];
                            //var target = externalResearchEntities.Portfolio_Security_Target_Baseview.Where(x => x.PORTFOLIO_ID == portfolioId && x.LOOK_THRU_FUND == lookthruportfolioId).ToList();
                            var target = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == portfolioId && x.LOOK_THRU_FUND == lookthruportfolioId).ToList();

                            if (filterType != null && filterValue != null)
                            {
                                switch (filterType)
                                {
                                    case "Region":
                                        targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == lookthruportfolioId && x.ASHEMM_PROPRIETARY_REGION_CODE == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                        break;
                                    case "Country":
                                        targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == lookthruportfolioId && x.ISO_COUNTRY_CODE == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                        break;
                                    case "Industry":
                                        targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == lookthruportfolioId && x.GICS_INDUSTRY_NAME == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                        break;
                                    case "Sector":
                                        targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == lookthruportfolioId && x.GICS_SECTOR_NAME == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                        break;
                                    case "Show Everything":
                                        targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == lookthruportfolioId).ToList();
                                        break;
                                }
                            }


                            if (target != null)
                                lookthrutargetProduct = lookthrutargetProduct * target.Sum(x => x.TARGET_PCT);


                            if (targetLookThruFundSecurityList != null)
                            {


                                lookthrutargetSum = lookthrutargetSum + lookthrutargetProduct * targetLookThruFundSecurityList.Sum(x => x.TARGET_PCT);
                            }

                        }
                    }
                    else
                    {
                        var portfolioId = securityPortfolioPath[0];
                        if (filterType != null && filterValue != null)
                        {
                            switch (filterType)
                            {
                                case "Region":
                                    targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == portfolioId && x.ASHEMM_PROPRIETARY_REGION_CODE == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                    break;
                                case "Country":
                                    targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == portfolioId && x.ISO_COUNTRY_CODE == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                    break;
                                case "Industry":
                                    targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == portfolioId && x.GICS_INDUSTRY_NAME == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                    break;
                                case "Sector":
                                    targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == portfolioId && x.GICS_SECTOR_NAME == filterValue && x.LOOK_THRU_FUND == null).ToList();
                                    break;
                                case "Show Everything":
                                    targetLookThruFundSecurityList = portfolioSecuritiesBaseviewList.Where(x => x.PORTFOLIO_ID == portfolioId).ToList();
                                    break;
                            }

                            if (targetLookThruFundSecurityList != null)
                            {
                                lookthrutargetSum = lookthrutargetSum + lookthrutargetProduct * targetLookThruFundSecurityList.Sum(x => x.TARGET_PCT);
                            }

                        }
                    }

                    List<decimal> lookthruNumbers = new List<decimal>();
                    lookthruNumbers.Add(lookthrutargetProduct);
                    lookthruNumbers.Add(lookthrutargetSum);
                    lookthruHash.Add(entry.Key, lookthruNumbers);

                }
            }

            return lookthruHash;
        }


        /// <summary>
        /// Method to retrieve External Research Data for Portfolio Details
        /// </summary>
        /// <param name="portfolioDetailsData">Collection of PortfolioDetailsData</param>
        /// <returns>Collection of PortfolioDetailsData</returns>
        private List<PortfolioDetailsData> RetrieveExternalResearchDataOld(List<PortfolioDetailsData> portfolioDetailsData)
        {
            try
            {
#if DEBUG
                // Stopwatch
                Stopwatch swRetrieveExt = new Stopwatch();
                DateTime timeRetrieveExt = new DateTime();
                Stopwatch swPortfolio_Security_Targets_Union = new Stopwatch();
                DateTime timePortfolio_Security_Targets_Union = new DateTime();
                Stopwatch swGF_SECURITY_BASEVIEW_Local = new Stopwatch();
                DateTime timeGF_SECURITY_BASEVIEW_Local = new DateTime();
                Stopwatch swGetPortfolioDetailsExternalData = new Stopwatch();
                DateTime timeGetPortfolioDetailsExternalData = new DateTime();
                Stopwatch swRetrieveSecurityReferenceData = new Stopwatch();
                DateTime timeRetrieveSecurityReferenceData = new DateTime();
                Stopwatch swGetPortfolioDetailsFairValue = new Stopwatch();
                DateTime timeGetPortfolioDetailsFairValue = new DateTime();

                swRetrieveExt.Start();
                timeRetrieveExt = DateTime.Now;
#endif

                var portfolios = portfolioDetailsData.Select(x => x.PfcHoldingPortfolio).Distinct().ToList();
                var externalResearchEntities = new GreenField.DAL.ExternalResearchEntities();
#if DEBUG
                swPortfolio_Security_Targets_Union.Start();
#endif
                var targets = externalResearchEntities.Portfolio_Security_Targets_Union.Where(x => portfolios.Contains(x.PORTFOLIO_ID)).ToList();
#if DEBUG
                swPortfolio_Security_Targets_Union.Stop();
                timePortfolio_Security_Targets_Union = DateTime.Now;
                swGF_SECURITY_BASEVIEW_Local.Start();
#endif
                var securities = externalResearchEntities.GF_SECURITY_BASEVIEW_Local.ToList();

                //Trace.WriteLine(string.Format("{0}: returned from GF_SECURITY_BASEVIEW_Local", DateTime.Now));
                //Trace.WriteLine("");
                //XMLStringValue(securities);

#if DEBUG
                swGF_SECURITY_BASEVIEW_Local.Stop();
                timeGF_SECURITY_BASEVIEW_Local = DateTime.Now;
#endif

                List<SecurityBaseviewData> securityData = new List<SecurityBaseviewData>();
#if DEBUG
                swRetrieveSecurityReferenceData.Start();
#endif
                securityData = RetrieveSecurityReferenceData(securities);
#if DEBUG
                swRetrieveSecurityReferenceData.Stop();
                timeRetrieveSecurityReferenceData = DateTime.Now;
#endif
                ExternalResearchEntities entity = new ExternalResearchEntities() { CommandTimeout = 5000 };
                List<string> securityNames = portfolioDetailsData.Select(a => a.IssueName).ToList();
                List<PortfolioDetailsExternalData> externalData = new List<PortfolioDetailsExternalData>();
                List<FAIR_VALUE> fairValueData = new List<FAIR_VALUE>();
                int check = 1;
                StringBuilder securityIDPortfolio = new StringBuilder();
                StringBuilder issuerIDPortfolio = new StringBuilder();

                foreach (String issueName in securityNames)
                {
                    SecurityBaseviewData securityDetails = securityData.Where(record => record.IssueName == issueName).FirstOrDefault();
                    if (securityDetails != null)
                    {
                        check = 0;
                        securityIDPortfolio.Append(",'" + securityDetails.SecurityId + "'");
                        issuerIDPortfolio.Append(",'" + securityDetails.IssuerId + "'");
                        if (portfolioDetailsData.Where(a => a.IssueName == issueName).FirstOrDefault() != null)
                        {
                            portfolioDetailsData.Where(a => a.IssueName == issueName).FirstOrDefault().SecurityId = Convert.ToString(securityDetails.SecurityId);
                        }
                    }
                }
                issuerIDPortfolio = check == 0 ? issuerIDPortfolio.Remove(0, 1) : null;
                securityIDPortfolio = check == 0 ? securityIDPortfolio.Remove(0, 1) : null;
                string _issuerIDPortfolio = issuerIDPortfolio == null ? null : issuerIDPortfolio.ToString();
                string _securityIDPortfolio = securityIDPortfolio == null ? null : securityIDPortfolio.ToString();

#if DEBUG
                swGetPortfolioDetailsExternalData.Start();
#endif
                externalData = entity.GetPortfolioDetailsExternalData(_issuerIDPortfolio, _securityIDPortfolio).ToList();
#if DEBUG
                swGetPortfolioDetailsExternalData.Stop();
                timeGetPortfolioDetailsExternalData = DateTime.Now;

                swGetPortfolioDetailsFairValue.Start();
#endif
                fairValueData = GetPortfolioDetailsFairValue(_securityIDPortfolio);
#if DEBUG
                swGetPortfolioDetailsFairValue.Stop();
                timeGetPortfolioDetailsFairValue = DateTime.Now;
#endif

                if (fairValueData == null)
                {
                    fairValueData = new List<FAIR_VALUE>();
                }
                foreach (PortfolioDetailsData item in portfolioDetailsData)
                {
                    item.MarketCap = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 185).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 185).FirstOrDefault().Amount;

                    item.ForwardPE = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 187).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 187).FirstOrDefault().Amount;

                    item.ForwardPBV = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 188).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 188).FirstOrDefault().Amount;

                    item.ForwardEB_EBITDA = externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 198).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.SecurityId == item.SecurityId && a.DataId == 198).FirstOrDefault().Amount;

                    item.RevenueGrowthCurrentYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == DateTime.Today.Year).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == DateTime.Today.Year).FirstOrDefault().Amount * 100M;

                    item.RevenueGrowthNextYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault() ==
                        null ? null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 178 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault().Amount * 100M;

                    item.NetIncomeGrowthCurrentYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ? null :
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.NetIncomeGrowthNextYear =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 177 && a.PeriodYear == (DateTime.Today.Year + 1)).FirstOrDefault().Amount * 100M;

                    item.ROE = externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 133 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 133 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.NetDebtEquity = externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 149 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 149 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount;

                    item.FreecashFlowMargin =
                        externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 146 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault() == null ?
                        null : externalData.Where(a => a.IssuerId == item.IssuerId && a.DataId == 146 && a.PeriodYear == (DateTime.Today.Year)).FirstOrDefault().Amount * 100M;

                    item.Upside = fairValueData.Where(a => a.SECURITY_ID == item.SecurityId).FirstOrDefault() == null ?
                        null : (fairValueData.Where(a => a.SECURITY_ID == item.SecurityId).FirstOrDefault().UPSIDE as decimal?) * 100M;

                    var security = securities.Where(x => x.ASEC_SEC_SHORT_NAME == item.AsecSecShortName).FirstOrDefault();
                    item.AshEmmModelWeight = 0;
                    if (security != null)
                    {

                        var target = targets.Where(x => x.SECURITY_ID == security.SECURITY_ID && x.PORTFOLIO_ID == item.PfcHoldingPortfolio);
                        if (target != null)
                            item.AshEmmModelWeight = target.Sum(x => x.TARGET_PCT);

                        if (item.PfcHoldingPortfolio != item.PortfolioPath)
                        {
                            var securityPortfolios = item.PortfolioPath.Split(',');
                            for (int i = securityPortfolios.Count() - 2; i >= 0; i--)
                            {
                                security = securities.Where(x => x.LOOK_THRU_FUND == securityPortfolios[i + 1]).FirstOrDefault();
                                if (security != null)
                                {
                                    target = targets.Where(x => x.SECURITY_ID == security.SECURITY_ID && x.PORTFOLIO_ID == securityPortfolios[i]);
                                    if (target != null)
                                        item.AshEmmModelWeight = item.AshEmmModelWeight * target.Sum(x => x.TARGET_PCT);
                                }
                                else
                                {
                                    throw new ApplicationException("Unknown look through fund security (LOOK_THRU_FUND: " + securityPortfolios[i] + ")");
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    throw new ApplicationException("Unknown security (short name: " + item.AsecSecShortName + ")");
                    //}
                }
#if DEBUG
                // StopWatch
                swRetrieveExt.Stop();
                Trace.WriteLine(string.Format("OLD. _____________________________________________________________________________________"));
                Trace.WriteLine(string.Format("\t\t\t{0}: RetrieveExternalResearchData start\n", timeRetrieveExt.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 1. AIMS_Main: Portfolio_Security_Targets_Union = {0} seconds.", (swPortfolio_Security_Targets_Union.ElapsedMilliseconds / 1000.00).ToString(), timePortfolio_Security_Targets_Union.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 2. AIMS_Main: GF_SECURITY_BASEVIEW_Local = {0} seconds.", (swGF_SECURITY_BASEVIEW_Local.ElapsedMilliseconds / 1000.00).ToString(), timeGF_SECURITY_BASEVIEW_Local.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 3. RetrieveSecurityReferenceData = {0} seconds.", (swRetrieveSecurityReferenceData.ElapsedMilliseconds / 1000.00).ToString(), timeRetrieveSecurityReferenceData.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 4. AIMS_Main: GetPortfolioDetailsExternalData = {0} seconds.", (swGetPortfolioDetailsExternalData.ElapsedMilliseconds / 1000.00).ToString(), timeGetPortfolioDetailsExternalData.ToString()));
                Trace.WriteLine(string.Format("\t\t\t{1}: 5. AIMS_Main: GetPortfolioDetailsFairValue = {0} seconds.", (swGetPortfolioDetailsFairValue.ElapsedMilliseconds / 1000.00).ToString(), timeGetPortfolioDetailsFairValue.ToString()));
                Trace.WriteLine(string.Format("\n\t\t\t{1}: Total time = {0} seconds.", (swRetrieveExt.ElapsedMilliseconds / 1000.00).ToString(), DateTime.Now.ToString()));
#endif

                return portfolioDetailsData;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Method to Retrieve Fairvalues
        /// </summary>
        /// <param name="securityIds">List of Securities</param>
        /// <returns>Collection of Fair_Value</returns>
        private List<FAIR_VALUE> GetPortfolioDetailsFairValue(string securityIds)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                List<FAIR_VALUE> result = new List<FAIR_VALUE>();

                result = entity.GetPortfolioDetailsFairValue(securityIds).ToList();
                if (result == null)
                {
                    return new List<FAIR_VALUE>();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                return null;
            }
        }

        /// <summary>
        /// Method to Retrieve all securities from GF_SECURITY_BASEVIEW
        /// </summary>
        /// <returns>List of SecurityBaseviewData</returns>
        private List<SecurityBaseviewData> RetrieveSecurityReferenceData(List<GreenField.DAL.GF_SECURITY_BASEVIEW_Local> securities)
        {
            try
            {
                //DimensionEntitiesService.Entities entity = DimensionEntity;
                //List<DimensionEntitiesService.GF_SECURITY_BASEVIEW> data;
                //if (securities == null)
                //{
                //    var data = entity.GF_SECURITY_BASEVIEW.ToList();
                //    List<SecurityBaseviewData> result = new List<SecurityBaseviewData>();
                //    foreach (DimensionEntitiesService.GF_SECURITY_BASEVIEW record in data)
                //    {
                //        result.Add(new SecurityBaseviewData()
                //        {
                //            IssueName = record.ISSUE_NAME,
                //            Ticker = record.TICKER,
                //            Country = record.ISO_COUNTRY_CODE,
                //            Sector = record.GICS_SECTOR_NAME,
                //            Industry = record.GICS_INDUSTRY_NAME,
                //            SubIndustry = record.GICS_SUB_INDUSTRY_NAME,
                //            PrimaryAnalyst = record.ASHMOREEMM_PRIMARY_ANALYST,
                //            Currency = record.TRADING_CURRENCY,
                //            FiscalYearend = record.FISCAL_YEAR_END,
                //            Website = record.WEBSITE,
                //            Description = record.BLOOMBERG_DESCRIPTION,
                //            SecurityId = record.SECURITY_ID,
                //            IssuerId = record.ISSUER_ID
                //        });
                //    }

                //    return result;
                //}
                //else
                
                var data = securities.ToList();
                List<SecurityBaseviewData> result = new List<SecurityBaseviewData>();
                foreach (var record in data)
                {
                    result.Add(new SecurityBaseviewData()
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
                        Description = record.BLOOMBERG_DESCRIPTION,
                        SecurityId = Int32.Parse(record.SECURITY_ID),
                        IssuerId = record.ISSUER_ID
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

        #endregion

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
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled)
        {
            try
            {
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                if (portfolioSelectionData == null || effectiveDate == null || filterType == null || filterValue == null)
                {
                    return result;
                }
                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception();
                }
                HoldingsPercentageData entry = new HoldingsPercentageData();
                decimal? sumForBenchmarks = 0;
                decimal? sumForPortfolios = 0;
                if (lookThruEnabled)
                {
                    #region lookThru Enabled
                    List<DimensionEntitiesService.GF_PORTFOLIO_LTHOLDINGS> portfolioData = DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).ToList();
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
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var k = from p in portfolioData
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in q)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in q)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }

                                    break;
                                case "Country":
                                    var l = from p in data
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var c = from p in portfolioData
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in l)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in l)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Industry":
                                    var m = from p in data
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };

                                    var s = from p in portfolioData
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in m)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in m)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Sector":
                                    var n = from p in data
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.GICS_INDUSTRY_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var d = from p in portfolioData
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.GICS_INDUSTRY_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in n)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in n)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Show Everything":
                                    var v = from p in data
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };

                                    var h = from p in portfolioData
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in v)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in v)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    foreach (var a in h)
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
                                    foreach (var a in h)
                                    {
                                        if (sumForPortfolios == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region lookThru Disabled
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
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var k = from p in portfolioData
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in q)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in q)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Country":
                                    var l = from p in data
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var c = from p in portfolioData
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in l)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in l)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Industry":
                                    var m = from p in data
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };

                                    var s = from p in portfolioData
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in m)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in m)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }

                                    break;
                                case "Sector":
                                    var n = from p in data
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.GICS_INDUSTRY_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var d = from p in portfolioData
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.GICS_INDUSTRY_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in n)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in n)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Show Everything":
                                    var v = from p in data
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };

                                    var h = from p in portfolioData
                                            group p by p.GICS_SECTOR_NAME into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in v)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in v)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    foreach (var a in h)
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
                                    foreach (var a in h)
                                    {
                                        if (sumForPortfolios == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion
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
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool lookThruEnabled)
        {
            try
            {
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                if (portfolioSelectionData == null || effectiveDate == null || filterType == null || filterValue == null)
                    return result;
                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception();
                }
                HoldingsPercentageData entry = new HoldingsPercentageData();
                decimal? sumForBenchmarks = 0;
                decimal? sumForPortfolios = 0;
                if (lookThruEnabled)
                {
                    #region Look Thru Enabled
                    List<DimensionEntitiesService.GF_PORTFOLIO_LTHOLDINGS> portfolioData = DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).ToList();
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
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.ISO_COUNTRY_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var k = from p in portfolioData
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.ISO_COUNTRY_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                    foreach (var a in q)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }

                                    foreach (var a in q)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Country":
                                    var l = from p in data
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var c = from p in portfolioData
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                    foreach (var a in l)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in l)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Industry":
                                    var m = from p in data
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var s = from p in portfolioData
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                    foreach (var a in m)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in m)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Sector":
                                    var n = from p in data
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var d = from p in portfolioData
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                    if (n == null || d == null)
                                    {
                                        return result;
                                    }
                                    foreach (var a in n)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in n)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;

                                case "Show Everything":
                                    var v = from p in data
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };

                                    var h = from p in portfolioData
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in v)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in v)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    foreach (var a in h)
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
                                    foreach (var a in h)
                                    {
                                        if (sumForPortfolios == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion
                }

                else
                {
                    #region Look Thru Disabled
                    List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> portfolioData = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate ).ToList();
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
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.ISO_COUNTRY_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var k = from p in portfolioData
                                            where p.ASHEMM_PROP_REGION_CODE == filterValue
                                            group p by p.ISO_COUNTRY_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                    foreach (var a in q)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in q)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Country":
                                    var l = from p in data
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var c = from p in portfolioData
                                            where p.ISO_COUNTRY_CODE == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };


                                    foreach (var a in l)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }

                                    foreach (var a in l)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Industry":
                                    var m = from p in data
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var s = from p in portfolioData
                                            where p.GICS_INDUSTRY_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                    foreach (var a in m)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in m)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                case "Sector":
                                    var n = from p in data
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                    var d = from p in portfolioData
                                            where p.GICS_SECTOR_NAME == filterValue
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };
                                    if (n == null || d == null)
                                    {
                                        return result;
                                    }
                                    foreach (var a in n)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in n)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
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
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;

                                case "Show Everything":
                                    var v = from p in data
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };

                                    var h = from p in portfolioData
                                            group p by p.ASHEMM_PROP_REGION_CODE into g
                                            select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                    foreach (var a in v)
                                    {
                                        sumForBenchmarks = sumForBenchmarks + a.BenchmarkSum;
                                    }
                                    foreach (var a in v)
                                    {
                                        if (sumForBenchmarks == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForBenchmarkSum(entry, sumForBenchmarks, a.SectorName, a.BenchmarkSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    foreach (var a in h)
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
                                    foreach (var a in h)
                                    {
                                        if (sumForPortfolios == 0)
                                        {
                                            continue;
                                        }
                                        CalculatesPercentageForPortfolioSum(entry, sumForPortfolios, a.SectorName, a.PortfolioSum, benchmarkId, ref result, effectiveDate);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion
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
        /// <param name="benchmarkReturn">Portfolio Weight</param>
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
        /// <param name="benchmarkReturn">Portfolio Weight</param>
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
            if (portfolioSelectionData == null || effectiveDate == null)
                return result;
            //checking if the service is down
            bool isServiceUp;
            isServiceUp = CheckServiceAvailability.ServiceAvailability();
            if (!isServiceUp)
            {
                throw new Exception();
            }
            List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIBUTION> topTenBenchmarkData = DimensionEntity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate && t.NODE_NAME == "Security ID" && t.BM1_RC_WGT_EOD != null && t.BM1_RC_WGT_EOD > 0).OrderByDescending(t => t.BM1_RC_WGT_EOD).ToList();
            IEqualityComparer<GF_PERF_DAILY_ATTRIBUTION> customComparer = new GreenField.Web.Services.PerformanceOperations.GF_PERF_DAILY_ATTRIBUTION_Comparer();
            topTenBenchmarkData = topTenBenchmarkData.Distinct(customComparer).Take(10).ToList();
            if (topTenBenchmarkData.Count == 0 || topTenBenchmarkData == null)
            {
                return result;
            }
            try
            {
                for (int i = 0; i < topTenBenchmarkData.Count; i++)
                {
                    TopBenchmarkSecuritiesData entry = new TopBenchmarkSecuritiesData();
                    entry.IssuerName = topTenBenchmarkData[i].SEC_NAME.ToString();
                    entry.Weight = Convert.ToDecimal(topTenBenchmarkData[i].BM1_RC_WGT_EOD) * 100;
                    entry.OneDayReturn = Convert.ToDecimal(topTenBenchmarkData[i].BM1_RC_TWR_1D) * 100;
                    entry.WTD = Convert.ToDecimal(topTenBenchmarkData[i].BM1_RC_TWR_1W) * 100;
                    entry.MTD = Convert.ToDecimal(topTenBenchmarkData[i].BM1_RC_TWR_MTD) * 100;
                    entry.QTD = Convert.ToDecimal(topTenBenchmarkData[i].BM1_RC_TWR_QTD) * 100;
                    entry.YTD = Convert.ToDecimal(topTenBenchmarkData[i].BM1_RC_TWR_YTD) * 100;
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
        /// Retrieve list of dates available in portfolio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DateTime> RetrieveAvailableDatesInPortfolios()
        {
            try
            {
                // use cache if available
                var fromCache = (List<DateTime>)new DefaultCacheProvider().Get(CacheKeyNames.AvailableDatesInPortfoliosCache);
                if (fromCache != null)
                    return fromCache;

                // otherwise fetch the data and cache it
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                List<DateTime> availableDateList = new List<DateTime>();
                FileCacheManager cacheManager = new FileCacheManager(CacheFolder);

                string availableDates = cacheManager["Dates"];

                if (String.IsNullOrEmpty(availableDates))
                {
                    List<DateTime?> dateList = GetAvailablePortolioDates();
                    availableDates = GenerateDateString(dateList);
                    StoreDateInCache(cacheManager, "Dates", availableDates);
                }

                availableDateList = GetDateListFromString(availableDates);
                availableDateList.Sort();

                new DefaultCacheProvider().Set(CacheKeyNames.AvailableDatesInPortfoliosCache, availableDateList, Int32.Parse(ConfigurationManager.AppSettings["SecuritiesCacheTime"]));

                return availableDateList;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region Heat Map Operation Contract

        /// <summary>
        /// Retrieves Heat Map Data for a particular portfolio and date
        /// </summary>
        /// <param name="fundSelectionData">Contains Selected Fund Data</param>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<HeatMapData> RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate, String period)
        {
            if (fundSelectionData == null || effectiveDate == null)
            {
                throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());
            }
            List<HeatMapData> result = new List<HeatMapData>();
            List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIBUTION> data = DimensionEntity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == fundSelectionData.PortfolioId && t.TO_DATE == effectiveDate && t.NODE_NAME == "Country").ToList();
            if (data == null || data.Count == 0)
            {
                return result;
            }
            //implementing portfolio inception check  
            System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
            dateInfo.ShortDatePattern = "dd/MM/yyyy";
            DateTime portfolioInceptionDate = Convert.ToDateTime(data.Select(a => a.POR_INCEPTION_DATE).FirstOrDefault(), dateInfo);
            if (period != "1D" && period != "1W")
            {
                bool isValid = InceptionDateChecker.ValidateInceptionDate(period, portfolioInceptionDate, effectiveDate);
                if (!isValid)
                {
                    return result;
                }
            }
            for (int i = 0; i < data.Count; i++)
            {
                HeatMapData entry = new HeatMapData();
                if (data[i].AGG_LVL_1 == null)
                    continue;
                switch (period)
                {
                    case "YTD":
                        entry.CountryID = data[i].AGG_LVL_1;
                        entry.CountryYTD = data[i].ADJ_RTN_POR_RC_TWR_YTD;
                        entry.BenchmarkYTD = data[i].BM1_RC_TWR_YTD; 
                        Decimal? diff = data[i].ADJ_RTN_POR_RC_TWR_YTD - data[i].BM1_RC_TWR_YTD;
                        CalculateHeatMapDiff(diff, ref entry);
                        if (data[i].ADJ_RTN_POR_RC_TWR_YTD == null && data[i].BM1_RC_TWR_YTD == null)
                        {
                            entry.CountryPerformance = PerformanceGrade.NO_RELATION;
                            entry.CountryYTD = Convert.ToDecimal(0);
                        }
                        result.Add(entry);
                        break;
                    case "MTD":
                        entry.CountryID = data[i].AGG_LVL_1;
                        entry.CountryYTD = data[i].ADJ_RTN_POR_RC_TWR_MTD;
                        entry.BenchmarkYTD = data[i].BM1_RC_TWR_MTD;
                        Decimal? diff1 = data[i].ADJ_RTN_POR_RC_TWR_MTD - data[i].BM1_RC_TWR_MTD;
                        CalculateHeatMapDiff(diff1, ref entry);
                        if (data[i].ADJ_RTN_POR_RC_TWR_MTD == null && data[i].BM1_RC_TWR_MTD == null)
                        {
                            entry.CountryPerformance = PerformanceGrade.NO_RELATION;
                            entry.CountryYTD = Convert.ToDecimal(0);
                            entry.BenchmarkYTD = Convert.ToDecimal(0);
                        }
                        result.Add(entry);
                        break;
                    case "1D":
                        entry.CountryID = data[i].AGG_LVL_1;
                        entry.CountryYTD = data[i].ADJ_RTN_POR_RC_TWR_1D;
                        entry.BenchmarkYTD = data[i].BM1_RC_TWR_1D;
                        Decimal? diff2 = data[i].ADJ_RTN_POR_RC_TWR_1D - data[i].BM1_RC_TWR_1D;
                        CalculateHeatMapDiff(diff2, ref entry);
                        if (data[i].ADJ_RTN_POR_RC_TWR_1D == null && data[i].BM1_RC_TWR_1D == null)
                        {
                            entry.CountryPerformance = PerformanceGrade.NO_RELATION;
                            entry.CountryYTD = Convert.ToDecimal(0);
                            entry.BenchmarkYTD = Convert.ToDecimal(0);
                        }
                        result.Add(entry);
                        break;
                    case "1W":
                        entry.CountryID = data[i].AGG_LVL_1;
                        entry.CountryYTD = data[i].ADJ_RTN_POR_RC_TWR_1W;
                        entry.BenchmarkYTD = data[i].BM1_RC_TWR_1W;
                        Decimal? diff3 = data[i].ADJ_RTN_POR_RC_TWR_1W - data[i].BM1_RC_TWR_1W;
                        CalculateHeatMapDiff(diff3, ref entry);
                        if (data[i].ADJ_RTN_POR_RC_TWR_1W == null && data[i].BM1_RC_TWR_1W == null)
                        {
                            entry.CountryPerformance = PerformanceGrade.NO_RELATION;
                            entry.CountryYTD = Convert.ToDecimal(0);
                            entry.BenchmarkYTD = Convert.ToDecimal(0);
                        }
                        result.Add(entry);
                        break;
                    case "QTD":
                        entry.CountryID = data[i].AGG_LVL_1;
                        entry.CountryYTD = data[i].ADJ_RTN_POR_RC_TWR_QTD;
                        entry.BenchmarkYTD = data[i].BM1_RC_TWR_QTD;
                        Decimal? diff4 = data[i].ADJ_RTN_POR_RC_TWR_QTD - data[i].BM1_RC_TWR_QTD;
                        CalculateHeatMapDiff(diff4, ref entry);
                        if (data[i].ADJ_RTN_POR_RC_TWR_QTD == null && data[i].BM1_RC_TWR_QTD == null)
                        {
                            entry.CountryPerformance = PerformanceGrade.NO_RELATION;
                            entry.CountryYTD = Convert.ToDecimal(0);
                            entry.BenchmarkYTD = Convert.ToDecimal(0);
                        }
                        result.Add(entry);
                        break;
                    case "1Y":
                        entry.CountryID = data[i].AGG_LVL_1;
                        entry.CountryYTD = data[i].ADJ_RTN_POR_RC_TWR_1Y;
                        entry.BenchmarkYTD = data[i].BM1_RC_TWR_1Y;
                        Decimal? diff5 = data[i].ADJ_RTN_POR_RC_TWR_1Y - data[i].BM1_RC_TWR_1Y;
                        CalculateHeatMapDiff(diff5, ref entry);
                        if (data[i].ADJ_RTN_POR_RC_TWR_1Y == null && data[i].BM1_RC_TWR_1Y == null)
                        {
                            entry.CountryPerformance = PerformanceGrade.NO_RELATION;
                            entry.CountryYTD = Convert.ToDecimal(0);
                            entry.BenchmarkYTD = Convert.ToDecimal(0);
                        }
                        result.Add(entry);
                        break;
                    default:
                        HeatMapData entry1 = new HeatMapData();
                        result.Add(entry1);
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates Heat Map Difference
        /// </summary>
        /// <param name="diff"></param>
        /// <param name="entry"></param>
        private void CalculateHeatMapDiff(Decimal? diff, ref HeatMapData entry)
        {
            if (diff > Convert.ToDecimal(0.05))
            {
                entry.CountryPerformance = PerformanceGrade.OVER_PERFORMING;
            }
            else
                if (diff < Convert.ToDecimal(-0.05))
                {
                    entry.CountryPerformance = PerformanceGrade.UNDER_PERFORMING;
                }
                else
                    if (diff >= Convert.ToDecimal(-0.05) && diff <= Convert.ToDecimal(0.05))
                    {
                        entry.CountryPerformance = PerformanceGrade.FLAT_PERFORMING;
                    }
        }
        #endregion

        #region Attribution and Risk return

        /// <summary>
        /// Retrieves Attribution Data for a particular composite/fund and Effective Date
        /// Filtering data based on fund name
        /// </summary>
        /// <param name="portfolioSelectionData">Portfolio Data that contains the name of the selected portfolio</param>
        /// <param name="effectiveDate">Selected Effective Date</param>
        /// <returns>List of Attribution Data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<AttributionData> RetrieveAttributionData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String nodeName)
        {
            List<AttributionData> result = new List<AttributionData>();
            if (portfolioSelectionData == null || effectiveDate == null)
            {
                return result;
            }
            //checking if the service is down
            bool isServiceUp;
            isServiceUp = CheckServiceAvailability.ServiceAvailability();
            if (!isServiceUp)
            {
                throw new Exception();
            }
            EqualityComparer<GF_PERF_DAILY_ATTRIBUTION> customComparer = new GreenField.Web.Services.PerformanceOperations.GF_PERF_DAILY_ATTRIBUTION_Comparer();
            List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIBUTION> attributionData = new List<GF_PERF_DAILY_ATTRIBUTION>();
            switch (nodeName)
            {
                case "Country":
                    attributionData = DimensionEntity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate &&
                        t.NODE_NAME == "Country").ToList();
                    attributionData = attributionData.Distinct(customComparer).ToList();
                    break;
                case "Sector":
                    attributionData = DimensionEntity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate &&
                        t.NODE_NAME == "GICS Level 1").ToList();
                    attributionData = attributionData.Distinct(customComparer).ToList();
                    break;
                case "Security":
                    attributionData = DimensionEntity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate &&
                        t.NODE_NAME == "Security ID" && t.SEC_INV_THEME == "EQUITY").ToList();
                    attributionData = attributionData.Distinct(customComparer).ToList();
                    break;
                default:
                    attributionData = new List<GF_PERF_DAILY_ATTRIBUTION>();
                    break;
            }
            if (attributionData.Count == 0 || attributionData == null)
            {
                return result;
            }
            try
            {
                for (int i = 0; i < attributionData.Count; i++)
                {
                    AttributionData entry = new AttributionData();
                    entry.Country = attributionData[i].AGG_LVL_1;
                    entry.CountryName = attributionData[i].AGG_LVL_1_LONG_NAME;
                    entry.PorRcAvgWgt1w = attributionData[i].POR_RC_AVG_WGT_1W;
                    entry.Bm1RcAvgWgt1w = attributionData[i].BM1_RC_AVG_WGT_1W;
                    entry.FPorAshRcCtn1w = attributionData[i].ADJ_RTN_POR_RC_TWR_1W;
                    entry.BM1_RC_TWR_1W = attributionData[i].BM1_RC_TWR_1W;
                    entry.PorTopTwr1w = attributionData[i].POR_TOP_RC_TWR_1W;
                    entry.BM1TopTwr1w =attributionData[i].BM1_TOP_RC_TWR_1W;
                    entry.FBm1AshAssetAlloc1w = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1W;
                    entry.FBm1AshSecSelec1w = attributionData[i].F_BM1_ASH_SEC_SELEC_1W;
                    entry.PorRcAvgWgt1d = attributionData[i].POR_RC_AVG_WGT_1D;
                    entry.Bm1RcAvgWgt1d = attributionData[i].BM1_RC_AVG_WGT_1D;
                    entry.FPorAshRcCtn1d = attributionData[i].ADJ_RTN_POR_RC_TWR_1D;
                    entry.BM1_RC_TWR_1D = attributionData[i].BM1_RC_TWR_1D;
                    entry.PorTopTwr1d = attributionData[i].POR_TOP_RC_TWR_1D;
                    entry.BM1TopTwr1d = attributionData[i].BM1_TOP_RC_TWR_1D;
                    entry.FBm1AshAssetAlloc1d = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1D;
                    entry.FBm1AshSecSelec1d = attributionData[i].F_BM1_ASH_SEC_SELEC_1D;
                    entry.PorRcAvgWgtMtd = attributionData[i].POR_RC_AVG_WGT_MTD;
                    entry.Bm1RcAvgWgtMtd = attributionData[i].BM1_RC_AVG_WGT_MTD;
                    entry.FPorAshRcCtnMtd = attributionData[i].ADJ_RTN_POR_RC_TWR_MTD;
                    entry.BM1_RC_TWR_MTD = attributionData[i].BM1_RC_TWR_MTD;
                    entry.PorTopTwrMtd = attributionData[i].POR_TOP_RC_TWR_MTD;
                    entry.BM1TopTwrMtd = attributionData[i].BM1_TOP_RC_TWR_MTD;
                    entry.FBm1AshAssetAllocMtd = attributionData[i].F_BM1_ASH_ASSET_ALLOC_MTD;
                    entry.FBm1AshSecSelecMtd = attributionData[i].F_BM1_ASH_SEC_SELEC_MTD;
                    entry.PorRcAvgWgtQtd = attributionData[i].POR_RC_AVG_WGT_QTD;
                    entry.Bm1RcAvgWgtQtd = attributionData[i].BM1_RC_AVG_WGT_QTD;
                    entry.FPorAshRcCtnQtd = attributionData[i].ADJ_RTN_POR_RC_TWR_QTD;
                    entry.BM1_RC_TWR_QTD = attributionData[i].BM1_RC_TWR_QTD;
                    entry.PorTopTwrQtd = attributionData[i].POR_TOP_RC_TWR_QTD;
                    entry.BM1TopTwrQtd = attributionData[i].BM1_TOP_RC_TWR_QTD;
                    entry.FBm1AshAssetAllocQtd = attributionData[i].F_BM1_ASH_ASSET_ALLOC_QTD;
                    entry.FBm1AshSecSelecQtd = attributionData[i].F_BM1_ASH_SEC_SELEC_QTD;
                    entry.PorRcAvgWgtYtd = attributionData[i].POR_RC_AVG_WGT_YTD;
                    entry.Bm1RcAvgWgtYtd = attributionData[i].BM1_RC_AVG_WGT_YTD;
                    entry.FPorAshRcCtnYtd = attributionData[i].ADJ_RTN_POR_RC_TWR_YTD;
                    entry.BM1_RC_TWR_YTD = attributionData[i].BM1_RC_TWR_YTD;
                    entry.FBm1AshAssetAllocYtd = attributionData[i].F_BM1_ASH_ASSET_ALLOC_YTD;
                    entry.FBm1AshSecSelecYtd = attributionData[i].F_BM1_ASH_SEC_SELEC_YTD;
                    entry.PorTopTwrYTD = attributionData[i].POR_TOP_RC_TWR_YTD;
                    entry.BM1TopTwrYtd = attributionData[i].BM1_TOP_RC_TWR_YTD;
                    entry.PorRcAvgWgt1y = attributionData[i].POR_RC_AVG_WGT_1Y;
                    entry.Bm1RcAvgWgt1y = attributionData[i].BM1_RC_AVG_WGT_1Y;
                    entry.FPorAshRcCtn1y = attributionData[i].ADJ_RTN_POR_RC_TWR_1Y;
                    entry.BM1_RC_TWR_1Y = attributionData[i].BM1_RC_TWR_1Y;
                    entry.FBm1AshAssetAlloc1y = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1Y;
                    entry.FBm1AshSecSelec1y = attributionData[i].F_BM1_ASH_SEC_SELEC_1Y;
                    entry.PorTopTwr1y = attributionData[i].POR_TOP_RC_TWR_1Y;
                    entry.BM1TopTwr1y = attributionData[i].BM1_TOP_RC_TWR_1Y;
                    entry.PorInceptionDate = attributionData[i].POR_INCEPTION_DATE;
                    entry.EffectiveDate = attributionData[i].TO_DATE; 
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
            List<PortfolioRiskReturnData> result = new List<PortfolioRiskReturnData>();
            if (portfolioSelectionData == null || effectiveDate == null)
            {
                return result;
            }
            //checking if the service is down
            bool isServiceUp;
            isServiceUp = CheckServiceAvailability.ServiceAvailability();

            if (!isServiceUp)
                throw new Exception();
            List<DimensionEntitiesService.GF_PERF_TOPLEVELSTATS> riskReturnData = (from p in DimensionEntity.GF_PERF_TOPLEVELSTATS
                                                                                   where p.PORTFOLIO == portfolioSelectionData.PortfolioId
                                                                                   && p.TO_DATE == effectiveDate.Date
                                                                                   && p.CURRENCY == "USD"
                                                                                   && p.RETURN_TYPE == "Gross"
                                                                                   select p).ToList<GF_PERF_TOPLEVELSTATS>();

            if (riskReturnData == null || riskReturnData.Count == 0)
            {
                return result;
            }
            try
            {
                PortfolioRiskReturnData entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Alpha";
                entry.BenchMarkValue1 = riskReturnData.Where(t => t.PORTYPE.StartsWith("Benchmark") && t.YEAR == "01 Year").Select(t => t.RC_ALPHA)
                    .FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.PORTYPE.StartsWith("Benchmark") && t.YEAR == "03 Year").Select(t => t.RC_ALPHA))
                    .FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);

                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Beta";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);

                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Standard Deviation";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);

                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Sharpe Ratio";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);

                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Information Ratio";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);

                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Tracking Error";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_TRACKERROR)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);

                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Correlation";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_CORRELATION)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
                result.Add(entry);

                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "R^Square";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep")
                    .Select(t => t.RC_R2)).FirstOrDefault();
                entry.PorInceptionDate = riskReturnData[0].POR_INCEPTION_DATE;
                entry.EffectiveDate = effectiveDate;
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

        #region Helper Method Risk Index
        public List<GF_PORTFOLIO_HOLDINGS> GetFilteredRiskIndexListWithoutLookThru(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, string filterType, string filterValue)
        {
            List<GF_PORTFOLIO_HOLDINGS> tempList = new List<GF_PORTFOLIO_HOLDINGS>();
            if (portfolioSelectionData != null && effectiveDate != null)
                switch (filterType)
                {
                    case "Region":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.ASHEMM_PROP_REGION_CODE == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.ASHEMM_PROP_REGION_CODE == filterValue).ToList();
                        break;
                    case "Country":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.ISO_COUNTRY_CODE == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.ISO_COUNTRY_CODE == filterValue).ToList();
                        break;
                    case "Sector":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.GICS_SECTOR_NAME == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.GICS_SECTOR_NAME == filterValue).ToList();
                        break;
                    case "Industry":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.GICS_INDUSTRY_NAME == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.GICS_INDUSTRY_NAME == filterValue).ToList();
                        break;
                    case "Show Everything":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY").ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date).ToList();
                        break;

                    default:
                        break;
                }
            return tempList;
        }

        public List<GF_PORTFOLIO_LTHOLDINGS> GetFilteredRiskIndexListWithLookThru(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, bool isExCashSecurity, string filterType, string filterValue)
        {
            List<GF_PORTFOLIO_LTHOLDINGS> tempList = new List<GF_PORTFOLIO_LTHOLDINGS>();
            if (portfolioSelectionData != null && effectiveDate != null)
                switch (filterType)
                {
                    case "Region":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.ASHEMM_PROP_REGION_CODE == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.ASHEMM_PROP_REGION_CODE == filterValue).ToList();
                        break;
                    case "Country":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.ISO_COUNTRY_CODE == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.ISO_COUNTRY_CODE == filterValue).ToList();
                        break;
                    case "Sector":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.GICS_SECTOR_NAME == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.GICS_SECTOR_NAME == filterValue).ToList();
                        break;
                    case "Industry":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY"
                                                                                                               && record.GICS_INDUSTRY_NAME == filterValue).ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.GICS_INDUSTRY_NAME == filterValue).ToList();
                        break;
                    case "Show Everything":
                        tempList = isExCashSecurity ? DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date
                                                                                                               && record.SECURITYTHEMECODE != "CASH"
                                                                                                               && record.SECURITYTHEMECODE != "LOC_CCY").ToList()
                                                    : DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolioSelectionData.PortfolioId
                                                                                                               && record.PORTFOLIO_DATE == effectiveDate.Date).ToList();
                        break;

                    default:
                        break;
                }
            return tempList;
        }
        #endregion

        #region Helper Methods Portfolio Date Retrieval

        private List<DateTime> GetDateListFromString(string availableDates)
        {
            List<DateTime> dateList = new List<DateTime>();
            string[] dateArr = availableDates.Split(new char[] { ',' });

            foreach (string date in dateArr)
            {
                DateTime availableDate = new DateTime();

                bool isDateConverted = DateTime.TryParse(date, out availableDate);

                if (isDateConverted)
                {
                    dateList.Add(availableDate);
                }
            }
            return dateList;
        }

        private List<DateTime?> GetAvailablePortolioDates()
        {
            Entities entity = DimensionEntity;
            List<GF_PORTFOLIO_HOLDINGS> selectedPortfolioDetails = (from p in entity.GF_PORTFOLIO_HOLDINGS
                                                                    where p.PORTFOLIO_ID == PortfolioName
                                                                    select p).ToList();

            List<DateTime?> dateList = (from p in selectedPortfolioDetails
                                        select p.PORTFOLIO_DATE).Distinct().ToList();
            return dateList;
        }

        private void StoreDateInCache(FileCacheManager cacheManager, string key, string dateString)
        {
            if (cacheManager != null)
            {
                cacheManager.SetCacheItem(key, dateString);
            }
        }

        private String GenerateDateString(List<DateTime?> dateList)
        {
            StringBuilder dateString = new StringBuilder();

            if (dateList != null)
            {
                for (int i = 0; i < dateList.Count; i++)
                {
                    if (dateList[i].HasValue)
                    {
                        dateString.Append(dateList[i].Value.Date.ToShortDateString());

                        if (i < dateList.Count - 1)
                        {
                            dateString.Append(",");
                        }
                    }
                }
            }

            return dateString.ToString();
        }

        #endregion

    }
}