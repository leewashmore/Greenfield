using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using GreenField.Web.Helpers.Service_Faults;
using System.Resources;
using GreenField.DataContracts.DataContracts;
using GreenField.Web.Helpers;
using GreenField.DataContracts;
using GreenField.DAL;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CustomScreeningToolOperations" in code, svc and config file together.
	public class CustomScreeningToolOperations
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

        /// <summary>
        /// Retrieving custom controls selection list depending upon parameter which contains name of the control
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<string> RetrieveCustomControlsList(string parameter)
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                List<string> result = new List<string>();

                DimensionEntitiesService.Entities entity = DimensionEntity;

                switch (parameter)
                {
                    case "Region":
                        result = entity.GF_SECURITY_BASEVIEW.Select(record => record.ASEC_SEC_COUNTRY_ZONE_NAME).Distinct().ToList();
                        break;
                    case "Country":
                        result = entity.GF_SECURITY_BASEVIEW.Select(record => record.ASEC_SEC_COUNTRY_NAME).Distinct().ToList();
                        break;
                    case "Sector":
                        result = entity.GF_SECURITY_BASEVIEW.Select(record => record.GICS_SECTOR_NAME).Distinct().ToList();
                        break;
                    case "Industry":
                        result = entity.GF_SECURITY_BASEVIEW.Select(record => record.GICS_INDUSTRY_NAME).Distinct().ToList();
                        break;
                    default:
                        break;
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
        /// Retrieving Security Reference Tab Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrieveSecurityReferenceTabDataPoints()
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                List<CustomSelectionData> result = new List<CustomSelectionData>();

               CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
               List<SCREENING_DISPLAY_REFERENCE> data = new List<SCREENING_DISPLAY_REFERENCE>();

               data = entity.SCREENING_DISPLAY_REFERENCE.OrderBy(record => record.DATA_DESC).ToList();
               
               if (data == null || data.Count < 1)
                   return result;

               foreach (SCREENING_DISPLAY_REFERENCE item in data)
               {
                   result.Add(new CustomSelectionData()
                   { 
                       ScreeningId = item.SCREENING_ID,
                       DataDescription = item.DATA_DESC,
                       LongDescription = item.LONG_DESC
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
        /// Retrieving Period Financials Tab Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrievePeriodFinancialsTabDataPoints()
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                List<CustomSelectionData> result = new List<CustomSelectionData>();

                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<FinancialTabDataDescriptions> data = new List<FinancialTabDataDescriptions>();

                data = entity.GetFinancialTabDataDescriptions("Period").ToList();
               
                if (data == null || data.Count < 1)
                    return result;

                foreach (FinancialTabDataDescriptions item in data)
                {
                    result.Add(new CustomSelectionData()
                    {
                        ScreeningId = item.SCREENING_ID,
                        DataDescription = item.DATA_DESC,
                       // LongDescription = item.LONG_DESC
                       Quaterly = item.QUARTERLY,
                       Annual = item.ANNUAL
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
        /// Retrieving Current Financials Tab Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrieveCurrentFinancialsTabDataPoints()
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                List<CustomSelectionData> result = new List<CustomSelectionData>();

                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<FinancialTabDataDescriptions> data = new List<FinancialTabDataDescriptions>();

                if (data == null || data.Count < 1)
                    return result;

                data = entity.GetFinancialTabDataDescriptions("Current").ToList();

                foreach (FinancialTabDataDescriptions item in data)
                {
                    result.Add(new CustomSelectionData()
                    {
                        ScreeningId = item.SCREENING_ID,
                        DataDescription = item.DATA_DESC,
                        // LongDescription = item.LONG_DESC
                        Quaterly = item.QUARTERLY,
                        Annual = item.ANNUAL
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
        /// Retrieving Fair Value Tab Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrieveFairValueTabDataPoints()
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                List<CustomSelectionData> result = new List<CustomSelectionData>();

                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<SCREENING_DISPLAY_FAIRVALUE> data = new List<SCREENING_DISPLAY_FAIRVALUE>();

                data = entity.SCREENING_DISPLAY_FAIRVALUE.OrderBy(record => record.DATA_DESC).ToList();

                if (data == null || data.Count < 1)
                    return result;

                foreach (SCREENING_DISPLAY_FAIRVALUE item in data)
                {
                    result.Add(new CustomSelectionData()
                    {
                        ScreeningId = item.SCREENING_ID,
                        DataDescription = item.DATA_DESC,
                        LongDescription = item.LONG_DESC
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
        /// Retrieving security data based on selected data points
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="benchmark"></param>
        /// <param name="region"></param>
        /// <param name="country"></param>
        /// <param name="sector"></param>
        /// <param name="industry"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomScreeningSecurityData> RetrieveSecurityData(PortfolioSelectionData portfolio, 
            BenchmarkSelectionData benchmark, String region, String country, String sector, String industry)
        {
            try
            {
                List<CustomScreeningSecurityData> result = new List<CustomScreeningSecurityData>();

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                result = RetrieveSecurityDetailsList(portfolio, benchmark, region, country, sector, industry);



                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region Helper Methods

        public List<CustomScreeningSecurityData> RetrieveSecurityDetailsList(PortfolioSelectionData portfolio,
            BenchmarkSelectionData benchmark, String region, String country, String sector, String industry)
        {
            DimensionEntitiesService.Entities entity = DimensionEntity;            
            
            List<GF_SECURITY_BASEVIEW> securitiesFromCustomControls = new List<GF_SECURITY_BASEVIEW>();
            List<CustomScreeningSecurityData> securityList = new List<CustomScreeningSecurityData>();

            if (portfolio != null)
            {
                List<GF_PORTFOLIO_HOLDINGS> securitiesFromPortfolio = new List<GF_PORTFOLIO_HOLDINGS>();
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);

                securitiesFromPortfolio = entity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolio.PortfolioId
                                                                                 && record.PORTFOLIO_DATE == lastBusinessDate
                                                                                 && (record.A_SEC_INSTR_TYPE == "Equity" || record.A_SEC_INSTR_TYPE == "GDR/ADR")
                                                                                 && record.DIRTY_VALUE_PC > 0).Distinct().ToList();
                if (securitiesFromPortfolio == null)
                    return securityList;
                foreach (GF_PORTFOLIO_HOLDINGS item in securitiesFromPortfolio)
	            {
		             securityList.Add(new CustomScreeningSecurityData()
                     {
                         SecurityId = item.ASEC_SEC_SHORT_NAME,
                         IssuerId = item.ISSUER_ID,
                         IssueName = item.ISSUE_NAME
                     });
	            }
                return securityList;
            }
            else if (benchmark != null)
            {
                List<GF_BENCHMARK_HOLDINGS> securitiesFromBenchmark = new List<GF_BENCHMARK_HOLDINGS>();
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_BENCHMARK_HOLDINGS lastBusinessRecord = entity.GF_BENCHMARK_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);

                securitiesFromBenchmark = entity.GF_BENCHMARK_HOLDINGS.Where(record => record.BENCHMARK_ID == benchmark.InstrumentID
                                                                                        && record.PORTFOLIO_DATE == lastBusinessDate).Distinct().ToList();
                if (securitiesFromBenchmark == null)
                    return securityList;
                foreach (GF_BENCHMARK_HOLDINGS item in securitiesFromBenchmark)
                {
                    securityList.Add(new CustomScreeningSecurityData()
                    {
                        SecurityId = item.ASEC_SEC_SHORT_NAME,
                        IssuerId = item.ISSUER_ID,
                        IssueName = item.ISSUE_NAME
                    });
                }
                return securityList;
            }
            else if (region != null)
            {
                List<GF_SECURITY_BASEVIEW> securitiesInRegion = new List<GF_SECURITY_BASEVIEW>();
                securitiesInRegion = entity.GF_SECURITY_BASEVIEW.Where(record => record.ASEC_SEC_COUNTRY_ZONE_NAME == region).Distinct().ToList();
                if (securitiesInRegion != null)
                    securitiesFromCustomControls.AddRange(securitiesInRegion);
            }
            else if (country != null)
            {
                List<GF_SECURITY_BASEVIEW> securitiesInCountry = new List<GF_SECURITY_BASEVIEW>();
                securitiesInCountry = entity.GF_SECURITY_BASEVIEW.Where(record => record.ASEC_SEC_COUNTRY_NAME == country).Distinct().ToList();
                if (securitiesInCountry != null)
                    securitiesFromCustomControls.AddRange(securitiesInCountry);
            }
            else if (sector != null)
            {
                List<GF_SECURITY_BASEVIEW> securitiesInSector = new List<GF_SECURITY_BASEVIEW>();
                securitiesInSector = entity.GF_SECURITY_BASEVIEW.Where(record => record.GICS_SECTOR_NAME == sector).Distinct().ToList();
                if (securitiesInSector != null)
                    securitiesFromCustomControls.AddRange(securitiesInSector);
            }
            else if (industry != null)
            {
                List<GF_SECURITY_BASEVIEW> securitiesInIndustry = new List<GF_SECURITY_BASEVIEW>();
                securitiesInIndustry = entity.GF_SECURITY_BASEVIEW.Where(record => record.GICS_INDUSTRY_NAME == industry).Distinct().ToList();
                if (securitiesInIndustry != null)
                    securitiesFromCustomControls.AddRange(securitiesInIndustry);
            }
            if (securitiesFromCustomControls == null)
                return securityList;

            securitiesFromCustomControls = securitiesFromCustomControls.Distinct().ToList();
            foreach (GF_SECURITY_BASEVIEW item in securitiesFromCustomControls)
            {
                securityList.Add(new CustomScreeningSecurityData()
                {
                    SecurityId = item.ASEC_SEC_SHORT_NAME,
                    IssuerId = item.ISSUER_ID,
                    IssueName = item.ISSUE_NAME
                });
            }
            return securityList;
        }
        #endregion
     	}
}