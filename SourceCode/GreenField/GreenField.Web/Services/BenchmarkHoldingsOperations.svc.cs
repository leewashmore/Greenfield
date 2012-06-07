using System.ServiceModel;
using System.ServiceModel.Activation;
using GreenField.Web.DimensionEntitiesService;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using System;
using System.Configuration;
using GreenField.DataContracts;
using System.Collections.Generic;
using GreenField.Web.Helpers;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using GreenField.DAL;
using System.ServiceModel.Description;
using System.Data.Objects;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BenchmarkHoldingsOperations
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
                List<SectorBreakdownData> result = new List<SectorBreakdownData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                    return result;

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;

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
                    decimal? portfolioWeight = (record.DIRTY_VALUE_PC / netPortfolioValuation) * 100;

                    //Retrieve Benchmark Weight 
                    decimal? benchmarkWeight = (Convert.ToDecimal(benchmarkData.Where(a => a.ISSUE_NAME == record.ISSUE_NAME).Select(a => a.BENCHMARK_WEIGHT).FirstOrDefault()));

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
                List<RegionBreakdownData> result = new List<RegionBreakdownData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                    return result;

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;

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
                    decimal? portfolioWeight = (record.DIRTY_VALUE_PC / netPortfolioValuation) * 100;

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
                List<TopHoldingsData> result = new List<TopHoldingsData>();

                if (portfolioSelectionData == null || effectiveDate == null)
                    return result;

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

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
                    decimal? portfolioWeight = (record.DIRTY_VALUE_PC / sumMarketValuePortfolio) * 100;

                    //Calculate Benchmark Weight - if null look for data in GF_BENCHMARK_HOLDINGS
                    GF_BENCHMARK_HOLDINGS specificHolding = DimensionEntity.GF_BENCHMARK_HOLDINGS
                            .Where(rec => rec.ISSUE_NAME == record.ISSUE_NAME && 
                                   rec.BENCHMARK_ID == record.BENCHMARK_ID && 
                                   rec.PORTFOLIO_DATE == record.PORTFOLIO_DATE )
                            .FirstOrDefault();
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
                List<IndexConstituentsData> result = new List<IndexConstituentsData>();

               if (portfolioSelectionData == null || effectiveDate == null)
                    return result;

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;

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
                    List<GF_BENCHMARK_HOLDINGS> data = entity.GF_BENCHMARK_HOLDINGS
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

        /// <summary>
        /// Service to return data for PortfolioDetailsUI
        /// </summary>
        /// <param name="objPortfolioIdentifier">Portfolio IDentifier</param>
        /// <param name="effectiveDate">Selected Date</param>
        /// <param name="objGetBenchmark">bool to check whether to get Benchmark data or not</param>
        /// <returns>List of type Portfolio Details Data</returns>
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

                if (objPortfolioIdentifier.PortfolioId == null)
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
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();             
                if (portfolioSelectionData == null || effectiveDate == null || filterType == null || filterValue == null)
                    return result;
                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

               
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
                                        where p.ASHEMM_PROP_REGION_CODE==filterValue
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
                                        where p.ISO_COUNTRY_CODE==filterValue
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
                                        where p.GICS_INDUSTRY_NAME==filterValue
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
                            case "Show Everything":
                               var v=  from p in data                                      
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
                                        continue;
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
                List<HoldingsPercentageData> result = new List<HoldingsPercentageData>();
                if (portfolioSelectionData == null || effectiveDate == null || filterType == null || filterValue == null)
                    return result;
                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

              
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
                                        where p.ASHEMM_PROP_REGION_CODE==filterValue
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
                                        where p.ISO_COUNTRY_CODE==filterValue
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
                                        where p.GICS_INDUSTRY_NAME==filterValue
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
                                        where p.GICS_SECTOR_NAME==filterValue
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, BenchmarkSum = g.Sum(a => a.BENCHMARK_WEIGHT) };
                                var d = from p in portfolioData
                                        where p.GICS_SECTOR_NAME == filterValue
                                        group p by p.ASHEMM_PROP_REGION_CODE into g
                                        select new { SectorName = g.Key, PortfolioSum = g.Sum(a => a.DIRTY_VALUE_PC) };

                                if (n == null || d == null)
                                    return result;

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
                                        continue;
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

        

        #endregion

        #region Heat Map Operation Contract

        /// <summary>
        /// Retrieves Heat Map Data for a particular portfolio and date
        /// </summary>
        /// <param name="fundSelectionData">Contains Selected Fund Data</param>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<HeatMapData> RetrieveHeatMapData(PortfolioSelectionData fundSelectionData, DateTime effectiveDate)
        {
            if (fundSelectionData == null || effectiveDate == null)
                throw new ArgumentNullException(ServiceFaultResourceManager.GetString("ServiceNullArgumentException").ToString());

            List<HeatMapData> result = new List<HeatMapData>();
            List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIBUTION> data = DimensionEntity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == fundSelectionData.PortfolioId && t.TO_DATE == effectiveDate && t.NODE_NAME == "Country").ToList();
            if (data == null || data.Count == 0)
                return result;
            for (int i = 0; i < data.Count; i++)
            {
                HeatMapData entry = new HeatMapData();
                if (data[i].AGG_LVL_1 == null)
                    continue;
                entry.CountryID = data[i].AGG_LVL_1;
                entry.CountryYTD = data[i].F_POR_ASH_RC_CTN_YTD;
                Decimal? diff = data[i].F_POR_ASH_RC_CTN_YTD - data[i].F_BM1_ASH_RC_CTN_YTD;
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
                        else
                            if (data[i].F_POR_ASH_RC_CTN_YTD == null || data[i].F_BM1_ASH_RC_CTN_YTD == null)
                            {
                                entry.CountryPerformance = PerformanceGrade.NO_RELATION;
                                entry.CountryYTD = Convert.ToDecimal(0);
                            }
                result.Add(entry);
            }
            return result;
        }
        #endregion

        #region Relative Performance

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
             List<AttributionData> result = new List<AttributionData>();
            if (portfolioSelectionData == null || effectiveDate == null)
                return result;
            //checking if the service is down
            bool isServiceUp;
            isServiceUp = CheckServiceAvailability.ServiceAvailability();

           if (!isServiceUp)

                throw new Exception();

            List<DimensionEntitiesService.GF_PERF_DAILY_ATTRIBUTION> attributionData = DimensionEntity.GF_PERF_DAILY_ATTRIBUTION.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate).ToList();
            if (attributionData.Count == 0 || attributionData == null)
                return result;
            try
            {
                for (int i = 0; i < attributionData.Count; i++)
                {
                    AttributionData entry = new AttributionData();
                    entry.Country = attributionData[i].COUNTRY;
                    entry.CountryName = attributionData[i].COUNTRY_NAME;
                    entry.PorRcAvgWgt1w = attributionData[i].POR_RC_AVG_WGT_1W;
                    entry.Bm1RcAvgWgt1w = attributionData[i].BM1_RC_AVG_WGT_1W;
                    entry.FPorAshRcCtn1w = attributionData[i].F_POR_ASH_RC_CTN_1W;
                    entry.FBm1AshRcCtn1w = attributionData[i].F_BM1_ASH_RC_CTN_1W;
                    entry.FBm1AshAssetAlloc1w = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1W;
                    entry.FBm1AshSecSelec1w = attributionData[i].F_BM1_ASH_SEC_SELEC_1W;
                    entry.PorRcAvgWgt1d = attributionData[i].POR_RC_AVG_WGT_1D;
                    entry.Bm1RcAvgWgt1d = attributionData[i].BM1_RC_AVG_WGT_1D;
                    entry.FPorAshRcCtn1d = attributionData[i].F_POR_ASH_RC_CTN_1D;
                    entry.FBm1AshRcCtn1d = attributionData[i].F_BM1_ASH_RC_CTN_1D;
                    entry.FBm1AshAssetAlloc1d = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1D;
                    entry.FBm1AshSecSelec1d = attributionData[i].F_BM1_ASH_SEC_SELEC_1D;
                    entry.PorRcAvgWgtMtd = attributionData[i].POR_RC_AVG_WGT_MTD;
                    entry.Bm1RcAvgWgtMtd = attributionData[i].BM1_RC_AVG_WGT_MTD;
                    entry.FPorAshRcCtnMtd = attributionData[i].F_POR_ASH_RC_CTN_MTD;
                    entry.FBm1AshRcCtnMtd = attributionData[i].F_BM1_ASH_RC_CTN_MTD;
                    entry.FBm1AshAssetAllocMtd = attributionData[i].F_BM1_ASH_ASSET_ALLOC_MTD;
                    entry.FBm1AshSecSelecMtd = attributionData[i].F_BM1_ASH_SEC_SELEC_MTD;
                    entry.PorRcAvgWgtQtd = attributionData[i].POR_RC_AVG_WGT_QTD;
                    entry.Bm1RcAvgWgtQtd = attributionData[i].BM1_RC_AVG_WGT_QTD;
                   entry.FPorAshRcCtnQtd = attributionData[i].F_POR_ASH_RC_CTN_QTD;
                    entry.FBm1AshRcCtnQtd = attributionData[i].F_BM1_ASH_RC_CTN_QTD;
                    entry.FBm1AshAssetAllocQtd = attributionData[i].F_BM1_ASH_ASSET_ALLOC_QTD;
                    entry.FBm1AshSecSelecQtd = attributionData[i].F_BM1_ASH_SEC_SELEC_QTD;
                    entry.PorRcAvgWgtYtd = attributionData[i].POR_RC_AVG_WGT_YTD;
                    entry.Bm1RcAvgWgtYtd = attributionData[i].BM1_RC_AVG_WGT_YTD;
                    entry.FPorAshRcCtnYtd = attributionData[i].F_POR_ASH_RC_CTN_YTD;
                    entry.FBm1AshRcCtnYtd = attributionData[i].F_BM1_ASH_RC_CTN_YTD;
                    entry.FBm1AshAssetAllocYtd = attributionData[i].F_BM1_ASH_ASSET_ALLOC_YTD;
                    entry.FBm1AshSecSelecYtd = attributionData[i].F_BM1_ASH_SEC_SELEC_YTD;
                    entry.PorRcAvgWgt1y = attributionData[i].POR_RC_AVG_WGT_1Y;
                    entry.Bm1RcAvgWgt1y = attributionData[i].BM1_RC_AVG_WGT_1Y;
                    entry.FPorAshRcCtn1y = attributionData[i].F_POR_ASH_RC_CTN_1Y;
                    entry.FBm1AshRcCtn1y = attributionData[i].F_BM1_ASH_RC_CTN_1Y;
                    entry.FBm1AshAssetAlloc1y = attributionData[i].F_BM1_ASH_ASSET_ALLOC_1Y;
                    entry.FBm1AshSecSelec1y = attributionData[i].F_BM1_ASH_SEC_SELEC_1Y;                   
                    //entry.PorRcAvgWgt5y = attributionData[i].POR_RC_AVG_WGT_5Y;
                    //entry.Bm1RcAvgWgt5y = attributionData[i].BM1_RC_AVG_WGT_5Y;
                    //entry.FPorAshRcCtn5y = attributionData[i].F_POR_ASH_RC_CTN_5Y;
                    //entry.FBm1AshRcCtn5y = attributionData[i].F_BM1_ASH_RC_CTN_5Y;
                    //entry.FBm1AshAssetAlloc5y = attributionData[i].F_BM1_ASH_ASSET_ALLOC_5Y;
                    //entry.FBm1AshSecSelec5y = attributionData[i].F_BM1_ASH_SEC_SELEC_5Y;
                    //entry.PorRcAvgWgtSi = attributionData[i].POR_RC_AVG_WGT_SI;
                    //entry.Bm1RcAvgWgtSi = attributionData[i].BM1_RC_AVG_WGT_SI;
                    //entry.FPorAshRcCtnSi = attributionData[i].F_POR_ASH_RC_CTN_SI;
                    //entry.FBm1AshRcCtnSi = attributionData[i].F_BM1_ASH_RC_CTN_SI;
                    //entry.FBm1AshAssetAllocSi = attributionData[i].F_BM1_ASH_ASSET_ALLOC_SI;
                    //entry.FBm1AshSecSelecSi = attributionData[i].F_BM1_ASH_SEC_SELEC_SI;
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
                return result;

            //checking if the service is down
            bool isServiceUp;
            isServiceUp = CheckServiceAvailability.ServiceAvailability();

            if (!isServiceUp)
                throw new Exception();

          
            //List<DimensionEntitiesService.GF_PERF_TOPLEVELSTATS> riskReturnData = DimensionEntity.GF_PERF_TOPLEVELSTATS.Where(t => t.PORTFOLIO == portfolioSelectionData.PortfolioId && t.TO_DATE == effectiveDate && t.CURRENCY == "USD" && t.RETURN_TYPE == "Gross").ToList();

            List<DimensionEntitiesService.GF_PERF_TOPLEVELSTATS> riskReturnData =  (from p in DimensionEntity.GF_PERF_TOPLEVELSTATS
                                                                                   where p.PORTFOLIO == portfolioSelectionData.PortfolioId 
                                                                                   && p.TO_DATE == effectiveDate.Date
                                                                                   && p.CURRENCY == "USD" && p.RETURN_TYPE == "Gross"
                                                                                    select p).ToList<GF_PERF_TOPLEVELSTATS>();
            //DimensionEntitiesService.Entities entity = DimensionEntity;

            //var data = from p in entity.GF_PERF_TOPLEVELSTATS
            //           where p.TO_DATE == Convert.ToDateTime("12/31/2006")
            //           && p.PORTFOLIO == "ABPEQ"
            //           && p.CURRENCY == "USD"
            //           && p.RETURN_TYPE == "Gross"
            //           select p;

            //var oneyearData = (from p in data
            //                  where p.PORTYPE.StartsWith("Benchmark") && p.YEAR == "01 Year"
            //                  select p.RC_ALPHA).First();

            if (riskReturnData == null || riskReturnData.Count == 0)
                return result;
            try
            {
                PortfolioRiskReturnData entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Alpha";
                entry.BenchMarkValue1 = riskReturnData.Where(t => t.PORTYPE.StartsWith("Benchmark") && t.YEAR == "01 Year").Select(t => t.RC_ALPHA).FirstOrDefault();                
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.PORTYPE.StartsWith("Benchmark") && t.YEAR == "03 Year").Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_ALPHA)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_ALPHA)).FirstOrDefault();
               entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_ALPHA)).FirstOrDefault();
                result.Add(entry);
                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Beta";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_BETA)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_BETA)).FirstOrDefault();

                result.Add(entry);
                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Information Ratio";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_INFORMATION)).FirstOrDefault();
                result.Add(entry);
                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Sharpe Ratio";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_SHARPE)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_SHARPE)).FirstOrDefault();
                result.Add(entry);
                entry = new PortfolioRiskReturnData();
                entry.DataPointName = "Standard Deviation";
                entry.BenchMarkValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.BenchMarkValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE.StartsWith("Benchmark") && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue1 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "01 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue2 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "03 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue3 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "05 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue4 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "10 Year").Select(t => t.RC_VOL)).FirstOrDefault();
                entry.PortfolioValue5 = (riskReturnData.Where(t => t.CURRENCY == "USD" && t.PORTYPE == "Portfolio" && t.RETURN_TYPE == "Gross" && t.YEAR == "Since Incep").Select(t => t.RC_VOL)).FirstOrDefault();
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

               // List<GF_PERF_DAILY_ATTRIBUTION> s1 = data.Where(t => t.POR_RC_MARKET_VALUE < 0).ToList();

        #endregion

    }
}
