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
using System.Linq;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service Class for Holdings, Benchmark & Performance
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BenchmarkHoldingsPerformanceOperations
    {
        /// <summary>
        /// Entity object for Dimension Web Service
        /// </summary>
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

        [OperationContract]
        public void Temp(PeriodSelectionData data)
        {
        }

        /// <summary>
        /// Method to retrieve Portfolio Selection Data from GF_PPORTFOLIO_SELECTION
        /// </summary>
        /// <returns>List of PortfolioSelectionData</returns>
        [OperationContract]
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

        /// <summary>
        /// retrieving data for sector breakdown gadget
        /// </summary>
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of sector breakdown data</returns>
        [OperationContract]
        public List<SectorBreakdownData> RetrieveSectorBreakdownData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
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
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>list of region breakdown data</returns>
        [OperationContract]
        public List<RegionBreakdownData> RetrieveRegionBreakdownData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
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
        public List<TopHoldingsData> RetrieveTopHoldingsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {
                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> data = DimensionEntity.GF_PORTFOLIO_HOLDINGS
                                                                                            .Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).ToList();
                List<TopHoldingsData> result = new List<TopHoldingsData>();
                if (data != null)
                {
                    decimal? sumMarketValuePortfolio = data.Sum(t => t.DIRTY_VALUE_PC);
                    decimal? sumMarketValueBenchmark = data.Sum(t => t.DIRTY_VALUE_PC);
                    if (sumMarketValuePortfolio != 0 && sumMarketValueBenchmark != 0)
                    {
                        foreach (DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS record in data)
                        {
                            result.Add(new TopHoldingsData()
                            {
                                Ticker = record.TICKER,
                                Holding = record.PORTFOLIO_ID,
                                MarketValue = record.DIRTY_VALUE_PC,
                                PortfolioShare = record.DIRTY_VALUE_PC / sumMarketValuePortfolio,
                                BenchmarkShare = record.DIRTY_VALUE_PC / sumMarketValueBenchmark,
                                BetShare = (record.DIRTY_VALUE_PC / sumMarketValuePortfolio) / (record.DIRTY_VALUE_PC / sumMarketValueBenchmark)
                            });
                        }
                    }
                }

                //List<TopHoldingsData> result = new List<TopHoldingsData>();
                //DataTable dataTable = GetDataTable("Select * from tblHoldingsData");
                //object sumPortfolioWeight = dataTable.Compute("Sum(PORTFOLIO_WEIGHT)", "");
                //object sumBenchmarkWeight = dataTable.Compute("Sum(BENCHMARK_WEIGHT)", "");

                //foreach (DataRow row in dataTable.Rows)
                //{
                //    result.Add(new TopHoldingsData()
                //    {
                //        Ticker = row.Field<string>("TICKER"),
                //        Holding = row.Field<string>("ISSUE_NAME"),
                //        MarketValue = row.Field<Single?>("DIRTY_VALUE_PC"),
                //        PortfolioShare = row.Field<Single?>("PORTFOLIO_WEIGHT") / (sumPortfolioWeight as Single?),
                //        BenchmarkShare = row.Field<Single?>("BENCHMARK_WEIGHT") / (sumBenchmarkWeight as Single?),
                //        BetShare = row.Field<Single?>("PORTFOLIO_WEIGHT") - row.Field<Single?>("BENCHMARK_WEIGHT")
                //    });
                //}
                return result.OrderByDescending(t => t.MarketValue).ToList().Take(10).ToList();
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
        public List<IndexConstituentsData> RetrieveIndexConstituentsData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            try
            {
                //List<IndexConstituentsData> result = new List<IndexConstituentsData>();
                //DataTable dataTable = GetDataTable("Select * from tblBenchmarkData");
                //object sumMarketValue = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "");

                //foreach (DataRow row in dataTable.Rows)
                //{
                //    string country = row.Field<string>("ISO_COUNTRY_CODE");
                //    object sumMarketValueCountry = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "ISO_COUNTRY_CODE = '" + country + "'");

                //    string industry = row.Field<string>("GICS_INDUSTRY_NAME");
                //    object sumMarketValueIndustry = dataTable.Compute("Sum(DIRTY_VALUE_PC)", "GICS_INDUSTRY_NAME = '" + industry + "'");

                //    result.Add(new IndexConstituentsData()
                //    {
                //        ConstituentName = row.Field<string>("ISSUE_NAME"),
                //        Country = country,
                //        Region = row.Field<string>("ASHEMM_PROPRIETARY_REGION_CODE"),
                //        Sector = row.Field<string>("GICS_SECTOR_NAME"),
                //        Industry = industry,
                //        SubIndustry = row.Field<string>("GICS_SUB_INDUSTRY_NAME"),
                //        Weight = row.Field<Single?>("DIRTY_VALUE_PC") / (sumMarketValue as Single?),
                //        WeightCountry = row.Field<Single?>("DIRTY_VALUE_PC") / (sumMarketValueCountry as Single?),
                //        WeightIndustry = row.Field<Single?>("DIRTY_VALUE_PC") / (sumMarketValueIndustry as Single?),
                //    });
                //}
                List<IndexConstituentsData> result = new List<IndexConstituentsData>();

                string benchmarkId = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).FirstOrDefault().BENCHMARK_ID.ToString();
                if (benchmarkId != null)
                {
                    List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> data = DimensionEntity.GF_BENCHMARK_HOLDINGS.Where(t => t.BENCHMARK_ID == benchmarkId).ToList();
                    if (data != null)
                    {
                        object sumMarketValue = data.Sum(t => t.DIRTY_VALUE_PC);
                        if (sumMarketValue != null)
                        {
                            foreach (DimensionEntitiesService.GF_BENCHMARK_HOLDINGS record in data)
                            {
                                string country = record.ISO_COUNTRY_CODE;
                                object sumMarketValueCountry = data.Where(t => t.ISO_COUNTRY_CODE == country).Sum(t => t.DIRTY_VALUE_PC);

                                string industry = record.GICS_INDUSTRY_NAME;
                                object sumMarketValueIndustry = data.Where(t => t.GICS_INDUSTRY_NAME == industry).Sum(t => t.DIRTY_VALUE_PC);
                                if (sumMarketValueCountry != null || sumMarketValueIndustry == null || (decimal?)sumMarketValueCountry == 0 || (decimal?)sumMarketValueIndustry == 0)
                                {
                                    continue;
                                }
                                result.Add(new IndexConstituentsData()
                                {
                                    ConstituentName = record.ISSUE_NAME,
                                    Country = country,
                                    Region = record.ASHEMM_PROP_REGION_CODE,
                                    Sector = record.GICS_SECTOR_NAME,
                                    Industry = industry,
                                    SubIndustry = record.GICS_SUB_INDUSTRY_NAME,
                                    Weight = (record.DIRTY_VALUE_PC) / (decimal?)sumMarketValue,
                                    WeightCountry = (record.DIRTY_VALUE_PC) / (decimal?)sumMarketValueCountry,
                                    WeightIndustry = (record.DIRTY_VALUE_PC) / (decimal?)sumMarketValueIndustry
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

        /// <summary>
        /// Returns Portfolio Details Data
        /// </summary>
        /// <param name="objPortfolioIdentifier">Details of Selected Portfolio</param>
        /// <param name="objSelectedDate">Selected Date</param>
        /// <returns>List of PortfolioDetailsData</returns>
        [OperationContract]
        public List<PortfolioDetailsData> RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioIdentifier, DateTime objSelectedDate, bool objGetBenchmark = false)
        {
            try
            {
                List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();

                //Arguement Null Case, return Empty Set
                if ((objPortfolioIdentifier == null) || (objSelectedDate == null))
                    return result;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> dimensionServicePortfolioData =
                    entity.GF_PORTFOLIO_HOLDINGS.
                    Where(a => (a.PORTFOLIO_ID.ToUpper() == objPortfolioIdentifier.PortfolioId.ToUpper()) && (a.PORTFOLIO_DATE == objSelectedDate.Date)).ToList();

                //If Service returned empty set
                if (dimensionServicePortfolioData.Count == 0)
                    return result;

                foreach (GF_PORTFOLIO_HOLDINGS item in dimensionServicePortfolioData)
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
                    portfolioResult.SecurityType = item.SECURITY_TYPE;
                    portfolioResult.BalanceNominal = item.BALANCE_NOMINAL;
                    portfolioResult.DirtyValuePC = item.DIRTY_VALUE_PC;
                    portfolioResult.PortfolioDirtyValuePC = 0;
                    portfolioResult.AshEmmModelWeight = item.ASH_EMM_MODEL_WEIGHT;
                    portfolioResult.PortfolioWeight = 0;
                    portfolioResult.BenchmarkWeight = item.BENCHMARK_WEIGHT;
                    portfolioResult.MarketCapUSD = item.MARKET_CAP_IN_USD;
                    portfolioResult.ReAshEmmModelWeight = portfolioResult.AshEmmModelWeight;
                    portfolioResult.RePortfolioWeight = portfolioResult.PortfolioWeight;
                    portfolioResult.ReBenchmarkWeight = portfolioResult.BenchmarkWeight;
                    portfolioResult.ActivePosition = portfolioResult.PortfolioWeight - portfolioResult.BenchmarkWeight;
                    result.Add(portfolioResult);
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
        /// Method to retrieve data in Benchmark Chart
        /// </summary>
        /// <param name="objBenchmarkIdentifier"></param>
        /// <param name="objEffectiveDate"></param>
        /// <returns></returns>
        [OperationContract]
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

            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Method to retrieve data in Benchmark Grid
        /// </summary>
        /// <param name="objBenchmarkIdentifier"></param>
        /// <param name="objEffectiveDate"></param>
        /// <returns></returns>
        [OperationContract]
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
            catch
            {
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
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageData(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue)
        {

            try
            {
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                //List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                HoldingsPercentageData entry = new HoldingsPercentageData();
                //ResearchEntities research = new ResearchEntities();
                //holdingData = research.tblHoldingsDatas.ToList();
                decimal? sumForBenchmarks = 0;
                decimal? sumForPortfolios = 0;
                string benchmarkId = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).FirstOrDefault().BENCHMARK_ID.ToString();
                List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> data = DimensionEntity.GF_BENCHMARK_HOLDINGS.Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate).ToList();

                switch (filterType)
                {
                    case "Region":
                        var q = from p in data
                                where (p.ASHEMM_PROP_REGION_CODE.ToString()).Equals(filterValue)
                                group p by p.GICS_SECTOR_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                        foreach (var a in q)    
                        {
                            CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in q)
                        {
                            if (sumForBenchmarks == 0 )
                                continue;
                            CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                        }
                        break;
                    case "Country":
                        var l = from p in data
                                where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                group p by p.GICS_SECTOR_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

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
                        var m = from p in data
                                where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                group p by p.GICS_SECTOR_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                        foreach (var a in m)
                        {
                            CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in m)
                        {
                            if (sumForBenchmarks == 0 )
                                continue;
                            CalculatesPercentageForBenchmark(entry, sumForBenchmarks, sumForPortfolios, a.SectorName, a.BenchmarkSum, a.PortfolioSum, ref result);
                        }
                        break;
                    case "Sector":
                        var n = from p in data
                                where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                                group p by p.GICS_INDUSTRY_NAME into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                        foreach (var a in n)
                        {
                            CalculatesTotalSumForBenchmark(ref sumForBenchmarks, ref sumForPortfolios, a.BenchmarkSum, a.PortfolioSum);
                        }
                        foreach (var a in n)
                        {
                            if (sumForBenchmarks == 0 )
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
        public List<HoldingsPercentageData> RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue)
        {
            try
            {
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                //List<tblHoldingsData> holdingData = new List<tblHoldingsData>();
                HoldingsPercentageData entry = new HoldingsPercentageData();
                //ResearchEntities research = new ResearchEntities();
                //holdingData = research.tblHoldingsDatas.ToList();
                decimal? sumForBenchmarks = 0;
                decimal? sumForPortfolios = 0;
                string benchmarkId = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(t => t.PORTFOLIO_ID == portfolioSelectionData.PortfolioId && t.PORTFOLIO_DATE == effectiveDate).FirstOrDefault().BENCHMARK_ID.ToString();
                List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> data = DimensionEntity.GF_BENCHMARK_HOLDINGS.Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate).ToList();

                switch (filterType)
                {
                    case "Region":
                        var q = from p in data
                                where (p.ASHEMM_PROP_REGION_CODE.ToString()).Equals(filterValue)
                                group p by p.ISO_COUNTRY_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

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
                        var l = from p in data
                                where (p.ISO_COUNTRY_CODE.ToString()).Equals(filterValue)
                                group p by p.ASHEMM_PROP_REGION_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };


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
                        var m = from p in data
                                where (p.GICS_INDUSTRY_NAME.ToString()).Equals(filterValue)
                                group p by p.ASHEMM_PROP_REGION_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

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
                        var n = from p in data
                                where (p.GICS_SECTOR_NAME.ToString()).Equals(filterValue)
                                group p by p.ASHEMM_PROP_REGION_CODE into g
                                select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT), PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

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
        public void CalculatesTotalSumForBenchmark(ref decimal? sumForBenchmarks, ref decimal? sumForPortfolios, decimal? a, decimal? b)
        {
            sumForBenchmarks = sumForBenchmarks + a;
            sumForPortfolios = sumForPortfolios + b;
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
        public void CalculatesPercentageForBenchmark(HoldingsPercentageData entry, decimal? sumForBenchmarks, decimal? sumForPortfolios, String name, decimal? a, decimal? b, ref List<HoldingsPercentageData> result)
        {
            entry = new HoldingsPercentageData();
            entry.SegmentName = name;
            entry.BenchmarkWeight = (a/sumForBenchmarks) * 100;
            //entry.PortfolioWeight = (b/sumForPortfolios) * 100;
            entry.PortfolioWeight = 100;
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

        [OperationContract]
        public MarketCapitalizationData RetrieveMarketCapitalizationData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
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

                List<GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData = entity.GF_PORTFOLIO_HOLDINGS.Where(a => (a.PORTFOLIO_ID == portfolioSelectionData.PortfolioId) && (a.PORTFOLIO_DATE == effectiveDate.Date)).ToList();
                result = AssetAllocationCalculations.CalculateAssetAllocationValues(dimensionPortfolioHoldingsData, portfolioSelectionData);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        #endregion

        #region Market Performance Snapshot Operation Contracts

        /// <summary>
        /// retrieving list of market performance snapshots for particular user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>returns list of market performance snapshots</returns>
        [OperationContract]
        public List<MarketSnapshotSelectionData> RetrieveMarketSnapshotSelectionData(string userName)
        {
            try
            {
                if (userName != null)
                {
                    ResearchEntities entity = new ResearchEntities();
                    List<MarketSnapshotSelectionData> userPreference = (entity.GetMarketSnapshotSelectionData(userName)).ToList<MarketSnapshotSelectionData>();
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
        /// retrieving user preference for market performance snapshot gadget
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="snapshotName"></param>
        /// <returns>list of user preference of entities in market performance snapshot</returns>
        [OperationContract]
        public List<MarketSnapshotPreference> RetrieveMarketSnapshotPreference(string userName, string snapshotName)
        {
            try
            {
                if (userName != null)
                {
                    ResearchEntities entity = new ResearchEntities();
                    List<MarketSnapshotPreference> userPreference = (entity.GetMarketSnapshotPreference(userName, snapshotName)).ToList<MarketSnapshotPreference>();
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
        /// retrieving entity data for market performance snapshot gadget based on user preference
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        /// <returns>list of entity data for market performance snapshot</returns>
        [OperationContract]
        public List<MarketPerformanceSnapshotData> RetrieveMarketPerformanceSnapshotData(List<MarketSnapshotPreference> marketSnapshotPreference)
        {
            try
            {
                List<MarketPerformanceSnapshotData> result = new List<MarketPerformanceSnapshotData>();
                foreach (MarketSnapshotPreference preference in marketSnapshotPreference)
                {
                    if (preference.EntityName != null)
                    {
                        result.Add(new MarketPerformanceSnapshotData()
                        {
                            MarketSnapshotPreferenceInfo = preference,
                            DateToDateReturn = -0.1,
                            WeekToDateReturn = -0.1,
                            MonthToDateReturn = 4.4,
                            QuarterToDateReturn = 4.4,
                            YearToDateReturn = 7.4,
                            LastYearReturn = 4.6,
                            SecondLastYearReturn = 52.3,
                            ThirdLastYearReturn = -50.8
                        });
                    }
                    else
                    {
                        result.Add(new MarketPerformanceSnapshotData()
                        {
                            MarketSnapshotPreferenceInfo = preference
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
        /// adding new market performance snapshot created by user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="snapshotName"></param>
        [OperationContract]
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
                return false;
            }
        }

        /// <summary>
        /// updating the market performance snapshot name for a particular user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="snapshotName"></param>
        /// <param name="snapshotPreferenceId"></param>
        [OperationContract]
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
                return false;
            }
        }

        /// <summary>
        /// adding user preferred groups in market performance snapshot gadget
        /// </summary>
        /// <param name="snapshotPreferenceId"></param>
        /// <param name="groupName"></param>
        [OperationContract]
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
                return false;
            }
        }

        /// <summary>
        /// removing user preferred groups from market performance snapshot gadget
        /// </summary>
        /// <param name="grouppreferenceId"></param>
        [OperationContract]
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
                return false;
            }
        }

        /// <summary>
        /// adding user preferred entities in groups in market performance snapshot gadget
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        [OperationContract]
        public bool AddMarketSnapshotEntityPreference(MarketSnapshotPreference marketSnapshotPreference)
        {
            ResearchEntities entity = new ResearchEntities();
            try
            {
                entity.SetMarketSnapshotEntityPreference(marketSnapshotPreference.GroupPreferenceID,
                                                            marketSnapshotPreference.EntityName,
                                                                marketSnapshotPreference.EntityReturnType,
                                                                    marketSnapshotPreference.EntityOrder);
                return true;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        ///  removing user preferred entities from groups in market performance snapshot gadget
        /// </summary>
        /// <param name="marketSnapshotPreference"></param>
        [OperationContract]
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
                return false;
            }
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
        public List<PortfolioRiskReturnData> RetrievePortfolioRiskReturnData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
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

        #region Heat Map Operation Contract
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

        #region Relative Performance
        [OperationContract]
        public List<RelativePerformanceSectorData> RetrieveRelativePerformanceSectorData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
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
        /// <param name="fundSelectionData">PortfolioSelectionData object</param>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData object</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="countryID">(optional) ISO_COUNTRY_CODE; By default Null</param>
        /// <param name="sectorID">(optional) GICS_SECTOR; By default Null</param>
        /// <returns>List of RelativePerformanceActivePositionData objects</returns>
        [OperationContract]
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
                return null;
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
                return null;
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
                return null;
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
        public List<RelativePerformanceSecurityData> RetrieveRelativePerformanceSecurityData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID = null, int? sectorID = null, int order = 0, int? maxRecords = null)
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
        public List<RelativePerformanceData> RetrieveRelativePerformanceData(PortfolioSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate)
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


    }
}
