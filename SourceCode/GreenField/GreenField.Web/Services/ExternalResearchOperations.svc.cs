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
                    CurrencyName = currencyName
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
            , FinancialStatementFiscalType fiscalType, FinancialStatementStatementType statementType, String currency)
        {
            try
            {
                string _dataSource = EnumUtils.ToString(dataSource);
                string _periodType = EnumUtils.ToString(periodType);
                string _fiscalType = EnumUtils.ToString(fiscalType);
                string _statementType = EnumUtils.ToString(statementType);

                ExternalResearchEntities entity = new ExternalResearchEntities();

                List<FinancialStatementData> result = null;

                result = entity.Get_Statement(issuerID, _dataSource, _periodType, _fiscalType, _statementType, currency).ToList();

                if (result == null)
                    return null;

                if (result.Count().Equals(0))
                    return result;

                string recordCurrency = result.First().CURRENCY;

                if (recordCurrency.ToUpper() != "USD")
                {
                    DateTime lastMonthEndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                    Decimal? fxRate = entity.FX_RATES.Where(record => record.CURRENCY == recordCurrency &&
                        record.FX_DATE == lastMonthEndDate).FirstOrDefault().FX_RATE;

                    if (fxRate != null && fxRate != 0)
                    {
                        int netRecords = result.Count();
                        for (int i = 0; i < netRecords; i++)
                        {
                            result.Add(new FinancialStatementData()
                            {
                                AMOUNT = result[i].AMOUNT / fxRate,
                                AMOUNT_TYPE = result[i].AMOUNT_TYPE,
                                BOLD_FONT = result[i].BOLD_FONT,
                                CALCULATION_DIAGRAM = result[i].CALCULATION_DIAGRAM,
                                CURRENCY = "USD",
                                REPORTED_MONTH = result[i].REPORTED_MONTH,
                                DATA_DESC = result[i].DATA_DESC,
                                DATA_ID = result[i].DATA_ID,
                                DECIMALS = result[i].DECIMALS,
                                GROUP_NAME = result[i].GROUP_NAME,
                                PERIOD = result[i].PERIOD,
                                PERIOD_TYPE = result[i].PERIOD_TYPE,
                                ROOT_SOURCE = result[i].ROOT_SOURCE,
                                ROOT_SOURCE_DATE = result[i].ROOT_SOURCE_DATE,
                                SORT_ORDER = result[i].SORT_ORDER
                            });
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
        /// Gets BAsic Data
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
                if (entitySelectionData == null)
                    return null;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

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
                //basicData.Beta = data.BETA;

                //AverageVolume= basicData.;
                //MarketCapitalization= basicData.;
                //SharesOutstanding= basicData.;
                //Beta= basicData.;
                //BarraBeta = basicData .;               
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
        public List<TargetPriceCEData> RetrieveTargetPriceData()
        {
            List<TargetPriceCEData> result = new List<TargetPriceCEData>();
            TargetPriceCEData data = new TargetPriceCEData();
            data.ConsensusRecommendation = "BUY";
            data.CurrentPrice = 35.56.ToString();
            data.High = Convert.ToDecimal(12.34);
            data.LastUpdate = DateTime.Today;
            data.Low = 13;
            data.MedianTargetPrice = 13.56.ToString();
            data.NoOfEstimates = 3;
            data.StandardDeviation = Convert.ToDecimal(13);
            data.Ticker = "INDIA";
            result.Add(data);
            return result;
        }




        #endregion

    }
}
