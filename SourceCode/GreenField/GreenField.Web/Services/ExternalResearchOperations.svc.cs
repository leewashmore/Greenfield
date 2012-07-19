using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.Helpers;
using GreenField.DataContracts;
using GreenField.DAL;
using System.Data.Objects;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using GreenField.Web.DataContracts;
using System.Data;
using GreenField.DataContracts.DataContracts;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ExternalResearchOperations
    {
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

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
        [FaultContract(typeof(ServiceFault))]
        public string RetrieveIssuerId(EntitySelectionData entitySelectionData)
        {
            try
            {
                string result = DimensionEntity.GF_PORTFOLIO_HOLDINGS.Where(record =>
                        record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName)
                    .Select(record => record.ISSUER_ID).FirstOrDefault();

                return result == null ? String.Empty : result;
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
        public IssuerReferenceData RetrieveIssuerReferenceData(EntitySelectionData entitySelectionData)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                if (securityDetails == null)
                    return new IssuerReferenceData();

                String issuerId = securityDetails.ISSUER_ID;
                String countryCode = securityDetails.ISO_COUNTRY_CODE;
                String countryName = securityDetails.ASEC_SEC_COUNTRY_NAME;
                String regionCode = securityDetails.ASHEMM_PROPRIETARY_REGION_CODE;
                String sectorCode = securityDetails.GICS_SECTOR;
                String sectorName = securityDetails.GICS_SECTOR_NAME;
                String industryCode = securityDetails.GICS_INDUSTRY;
                String industryName = securityDetails.GICS_SUB_INDUSTRY_NAME;
                String currencyCode = null;
                String currencyName = null;

                External_Country_Master countryDetails = entity.External_Country_Master
                    .Where(record => record.COUNTRY_CODE == countryCode &&
                        record.COUNTRY_NAME == countryName)
                    .FirstOrDefault();

                if (countryDetails != null)
                {
                    currencyCode = countryDetails.CURRENCY_CODE;
                    currencyName = countryDetails.CURRENCY_NAME;
                }

                IssuerReferenceData result = new IssuerReferenceData()
                {
                    IssuerId = issuerId,
                    CountryCode = countryCode,
                    CountryName = countryName,
                    CurrencyCode = currencyCode,
                    CurrencyName = currencyName,
                    RegionCode = regionCode,
                    SectorCode = sectorCode,
                    SectorName = sectorName,
                    IndustryCode = industryCode,
                    IndustryName = industryName
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FinancialStatementData> RetrieveFinancialStatement(string issuerID, FinancialStatementDataSource dataSource, FinancialStatementPeriodType periodType
            , FinancialStatementFiscalType fiscalType, FinancialStatementType statementType, String currency)
        {
            try
            {
                string _dataSource = EnumUtils.ToString(dataSource);
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);
                string _fiscalType = EnumUtils.ToString(fiscalType);
                string _statementType = EnumUtils.ToString(statementType);

                ExternalResearchEntities entity = new ExternalResearchEntities();

                List<FinancialStatementData> result = null;

                result = entity.Get_Statement(issuerID, _dataSource, _periodType, _fiscalType, _statementType, currency).ToList();

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
        /// Get data for Consensus Estimate Detailed gadget
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="periodType"></param>
        /// <param name="currency"></param>
        /// <param name="currentYear"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<ConsensusEstimateDetail> RetrieveConsensusEstimateDetailedData(string issuerId, FinancialStatementPeriodType periodType, String currency)
        {
            try
            {
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);

                ExternalResearchEntities entity = new ExternalResearchEntities();

                List<ConsensusEstimateDetailData> data = new List<ConsensusEstimateDetailData>();
                List<ConsensusEstimateDetail> result = new List<ConsensusEstimateDetail>();

                data = entity.GetConsensusDetail(issuerId, "REUTERS", _periodType, "FISCAL", currency).ToList();

                ConsensusEstimateDetail temp = new ConsensusEstimateDetail();
                foreach (ConsensusEstimateDetailData item in data)
                {
                    temp = new ConsensusEstimateDetail();
                  temp.IssuerId = item.ISSUER_ID;
                  temp.EstimateId = item.ESTIMATE_ID;
                  temp.EstimateDesc = item.ESTIMATE_DESC;
                  temp.Period = item.Period;
                  temp.AmountType = item.AMOUNT_TYPE;
                  temp.PeriodYear = item.PERIOD_YEAR;
                  temp.PeriodType = item.PERIOD_TYPE;
                  temp.Amount = item.AMOUNT;
                  temp.AshmoreEmmAmount = item.ASHMOREEMM_AMOUNT;
                  temp.NumberOfEstimates = item.NUMBER_OF_ESTIMATES;
                  temp.High = item.HIGH;
                  temp.Low = item.LOW;
                  temp.StandardDeviation = item.STANDARD_DEVIATION;
                  temp.SourceCurrency = item.SOURCE_CURRENCY;
                  temp.DataSource = item.DATA_SOURCE;
                  temp.DataSourceDate = item.DATA_SOURCE_DATE;
                  item.ACTUAL = temp.Actual;
                  result.Add(temp);
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
        /// Gets Basic Data
        /// </summary>
        /// <param name="securityId"></param>
        /// <returns>Basic data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<BasicData> RetrieveBasicData(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<BasicData> result = new List<BasicData>();
                List<GetBasicData_Result> resultDB = new List<GetBasicData_Result>();
                ExternalResearchEntities extResearch = new ExternalResearchEntities();
                if (entitySelectionData == null)
                    return null;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (data == null)
                    return null;

                BasicData basicData = new BasicData();
                basicData.WeekRange52 = data.FIFTYTWO_WEEK_LOW - data.FIFTYTWO_WEEK_HIGH;
                basicData.AverageVolume = data.SECURITY_VOLUME_AVG_6M;
                basicData.SharesOutstanding = data.SHARES_OUTSTANDING;
                if (data.BARRA_BETA != null)
                {
                    basicData.Beta = data.BARRA_BETA;
                    basicData.BetaSource = "BARRA";
                }
                else
                {
                    decimal convertedString;
                    basicData.BetaSource = "BLOOMBERG";
                    //if (data.BETA != null && data.BETA != string.Empty)
                    //{
                    if (decimal.TryParse(data.BETA, out convertedString))
                        basicData.Beta = convertedString;

                    else
                    {
                        basicData.Beta = null;
                        LoggingOperations _logging = new LoggingOperations();
                        string userName = null;

                        if (System.Web.HttpContext.Current.Session["Session"] != null)
                        {
                            userName = (System.Web.HttpContext.Current.Session["Session"] as Session).UserName;
                        }
                        //Calling method to log error in text file
                        _logging.LogToFile("|User[(" + userName.Replace(Environment.NewLine, " ")
                         + ")]|Type[(Exception"
                         + ")]|Message[(" + "Conversion from string to decimal failed."
                         + ")]", "Exception", "Medium");
                    }
                }

                ////Retrieving data from Period Financials table
                resultDB = extResearch.ExecuteStoreQuery<GetBasicData_Result>("exec GetBasicData @SecurityID={0}", Convert.ToString(data.SECURITY_ID)).ToList();



                basicData.MarketCapitalization = resultDB[0].MARKET_CAPITALIZATION;
                basicData.EnterpriseValue = resultDB[0].ENTERPRISE_VALUE;
                result.Add(basicData);

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region ConsensusEstimatesGadgets

        /// <summary>
        /// Service Method for ConsensusEstimatesGadget-TargetPrice
        /// </summary>
        /// <returns>Collection of TargetPriceCEData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<TargetPriceCEData> RetrieveTargetPriceData(EntitySelectionData entitySelectionData)
        {
            List<TargetPriceCEData> result = new List<TargetPriceCEData>();
            TargetPriceCEData data = new TargetPriceCEData();

            if (entitySelectionData == null)
                return new List<TargetPriceCEData>();
            DimensionEntitiesService.Entities dimensionEntity = DimensionEntity;

            List<GF_SECURITY_BASEVIEW> securityData = (dimensionEntity.GF_SECURITY_BASEVIEW.
                Where(a => a.ISSUE_NAME.ToUpper().Trim() == entitySelectionData.LongName.ToUpper().Trim()).ToList());
            if (securityData == null)
                return result;

            string XRef = securityData.Select(a => a.XREF).FirstOrDefault();

            if (XRef == null)
                return result;

            List<GetTargetPrice_Result> dbResult = new List<GetTargetPrice_Result>();
            ExternalResearchEntities entity = new ExternalResearchEntities();
            dbResult = entity.GetTargetPrice(XRef).ToList();

            if (dbResult == null)
                return result;
            if (dbResult.Count == 0)
                return result;

            foreach (GetTargetPrice_Result item in dbResult)
            {
                data = new TargetPriceCEData();
                data.Ticker = (item.Ticker == null) ? "N/A" : item.Ticker;
                data.ConsensusRecommendation = item.MeanLabel;
                data.CurrentPrice = ((item.CurrentPrice == null) ? "N/A" : item.CurrentPrice.ToString()).ToString() +
                    "( " + ((item.Currency == null) ? "N/A" : (item.Currency.ToString())).ToString() + " )";
                data.MedianTargetPrice = ((item.Median == null) ? "N/A" : item.Median.ToString()) +
                    " ( " + ((item.TargetCurrency == null) ? "N/A" : item.TargetCurrency.ToString()) + " )";
                data.LastUpdate = Convert.ToDateTime(item.StartDate);
                data.NoOfEstimates = (item.NumOfEsts == null) ? "N/A" : (Convert.ToString(item.NumOfEsts));
                data.High = (item.High == null) ? "N/A" : (Convert.ToString(item.High));
                data.Low = (item.Low == null) ? "N/A" : (Convert.ToString(item.Low));
                data.StandardDeviation = (item.StdDev == null) ? "N/A" : (Convert.ToString(item.StdDev));
                result.Add(data);
            }

            return result;
        }

        /// <summary>
        /// Service Method for ConsensusEstimateGadget - Median
        /// </summary>
        /// <param name="issuerId">Issuer ID</param>
        /// <param name="periodType">Period Type: A/Q</param>
        /// <param name="currency">Selected Currency</param>
        /// <returns>Collection of ConsensusEstimateMedianData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<ConsensusEstimateMedian> RetrieveConsensusEstimatesMedianData(string issuerId, FinancialStatementPeriodType periodType, string currency)
        {
            List<ConsensusEstimateMedian> result = new List<ConsensusEstimateMedian>();
            List<ConsensusEstimateMedianData> dbResult = new List<ConsensusEstimateMedianData>();
            try
            {
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);

                ExternalResearchEntities entity = new ExternalResearchEntities();

                dbResult = entity.GetConsensusEstimateData(issuerId, _periodType, currency).ToList();

                ConsensusEstimateMedian data = new ConsensusEstimateMedian();

                foreach (ConsensusEstimateMedianData item in dbResult)
                {
                    data = new ConsensusEstimateMedian();
                    data.Amount = item.AMOUNT;
                    data.AmountType = item.AMOUNT_TYPE;
                    data.Currency = item.CURRENCY;
                    data.DataSource = item.DATA_SOURCE;
                    data.DataSourceDate = item.DATA_SOURCE_DATE;
                    data.EstimateDesc = item.ESTIMATE_DESC;
                    data.EstimateType = item.ESTIMATE_TYPE;
                    data.FiscalType = item.FISCAL_TYPE;
                    data.High = item.HIGH;
                    data.IssuerId = item.ISSUER_ID;
                    data.Low = item.LOW;
                    data.NumberOfEstimates = item.NUMBER_OF_ESTIMATES;
                    data.PeriodEndDate = item.PERIOD_END_DATE;
                    data.PeriodType = item.PERIOD_TYPE;
                    data.PeriodYear = item.PERIOD_YEAR;
                    data.SecrityId = item.SECURITY_ID;
                    data.SourceCurrency = item.SOURCE_CURRENCY;
                    data.StandardDeviation = item.STANDARD_DEVIATION;
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
        /// Service Method for ConsensusEstimateGadget- Valuations
        /// </summary>
        /// <param name="issuerId">Issuer Id for a Security</param>
        /// <param name="periodType">Period Type: A/Q</param>
        /// <param name="currency">Selected Currency</param>
        /// <returns>Collection of ConsensusEstimatesValuations Data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<ConsensusEstimatesValuations> RetrieveConsensusEstimatesValuationData(string issuerId, FinancialStatementPeriodType periodType, string currency)
        {
            List<ConsensusEstimatesValuations> result = new List<ConsensusEstimatesValuations>();
            List<ConsensusEstimateValuation> dbResult = new List<ConsensusEstimateValuation>();
            try
            {
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);

                ExternalResearchEntities entity = new ExternalResearchEntities();

                dbResult = entity.GetConsensusEstimatesValuation(issuerId, "REUTERS", _periodType, "FISCAL", currency, null, null).ToList();

                ConsensusEstimatesValuations data;
                foreach (ConsensusEstimateValuation item in dbResult)
                {
                    data = new ConsensusEstimatesValuations();
                    data.Amount = item.AMOUNT;
                    data.AmountType = item.AMOUNT_TYPE;
                    data.AshmoreEMMAmount = Convert.ToDecimal(item.ASHMOREEMM_AMOUNT);
                    data.DataSource = item.DATA_SOURCE;
                    data.DataSourceDate = item.DATA_SOURCE_DATE;
                    data.EstimateType = item.ESTIMATE_DESC;
                    data.EstimateId = Convert.ToString(item.ESTIMATE_ID);
                    data.High = item.HIGH;
                    data.Low = item.LOW;
                    data.IssuerId = item.ISSUER_ID;
                    data.NumberOfEstimates = item.NUMBER_OF_ESTIMATES;
                    data.Period = item.PERIOD;
                    data.PeriodType = item.PERIOD_TYPE;
                    data.PeriodYear = item.PERIOD_YEAR;
                    data.SourceCurrency = item.SOURCE_CURRENCY;
                    data.StandardDeviation = item.STANDARD_DEVIATION;
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


        #endregion

        /// <summary>
        /// Retrieve data for consensus Estimates Summary Gadget
        /// </summary>
        /// <param name="entityIdentifier">Security identifier selected by the user</param>
        /// <returns>Returns data in the list of type ConsensusEstimatesSummaryData</returns>
        /// 
        #region Consensus Estimates Summary Gadget
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData> RetrieveConsensusEstimatesSummaryData(EntitySelectionData entityIdentifier)
        {
            try
            {
                List<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData> result = new List<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData>();
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities research = new ExternalResearchEntities();
                result = research.ExecuteStoreQuery<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData>("exec GetConsensusEstimatesSummaryData @Security={0}", entityIdentifier.LongName).ToList();
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

        #region Quarterly Comparision Results
        /// <summary>
        /// Retrieves Data for Quarterly Comparison
        /// </summary>
        /// <param name="fieldValue">field as selected by the user</param>
        /// <param name="yearValue">year as selected by the user</param>
        /// <returns>Returns data in list of type QuarterlyResultsData </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<QuarterlyResultsData> RetrieveQuarterlyResultsData(String fieldValue, int yearValue)
        {
            try
            {
                int dataID;
                List<QuarterlyResultsData> result = new List<QuarterlyResultsData>();
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities research = new ExternalResearchEntities();
                if (fieldValue == "Revenue")
                    dataID = 11;
                else
                    dataID = 44;
                result = research.ExecuteStoreQuery<QuarterlyResultsData>("exec usp_GetQuarterlyResults @DataId={0}, @PeriodYear = {1}", dataID, yearValue).ToList();

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

        #region Historical Valuation Multiples Gadget
        /// <summary>
        /// Gets P/Revenue Data
        /// </summary>
        /// <param name="entitySelectionData"></param>
        /// <returns>P/Revenue Data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PRevenueData> RetrievePRevenueData(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<PRevenueData> result = new List<PRevenueData>();
                string issuerId = string.Empty;

                ExternalResearchEntities extResearch = new ExternalResearchEntities();
                if (entitySelectionData == null)
                    return null;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (data == null)
                    return null;
                issuerId = data.ISSUER_ID;
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

        #region Scatter Graph Gadget
        /// <summary>
        /// Gets Ratio Comparison Data
        /// </summary>
        /// <param name="contextSecurityXML">xml script for security list for a particular context</param>
        /// <returns>RatioComparisonData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RatioComparisonData> RetrieveRatioComparisonData(String contextSecurityXML)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                List<RatioComparisonData> result = entity.usp_RetrieveRatioComparisonData(contextSecurityXML).ToList();
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
        public List<GF_SECURITY_BASEVIEW> RetrieveRatioSecurityReferenceData(ScatterGraphContext context, IssuerReferenceData issuerDetails)
        {
            try
            {
                switch (context)
                {
                    case ScatterGraphContext.REGION:
                        return DimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.ASHEMM_PROPRIETARY_REGION_CODE == issuerDetails.RegionCode).ToList();
                    case ScatterGraphContext.COUNTRY:
                        return DimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.ISO_COUNTRY_CODE == issuerDetails.CountryCode).ToList();
                    case ScatterGraphContext.SECTOR:
                        return DimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.GICS_SECTOR == issuerDetails.SectorCode).ToList();
                    case ScatterGraphContext.INDUSTRY:
                        return DimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.GICS_INDUSTRY == issuerDetails.IndustryCode).ToList();
                    default:
                        return null;
                }
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
