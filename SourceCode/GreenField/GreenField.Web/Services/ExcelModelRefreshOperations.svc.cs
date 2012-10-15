using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using GreenField.DAL;
using GreenField.DataContracts;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service to Refresh ExcelModel Data
    /// </summary>
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ExcelModelRefreshOperations
    {
        /// <summary>
        /// Service fault Resource manager
        /// </summary>
        private ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        /// <summary>
        /// Dimension Service Entity
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
        /// Returns Data to Refresh ExcelModel Sheet
        /// </summary>
        /// <param name="selectedSecurity">Selected Security</param>
        /// <returns>object of type ExcelModelData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public ExcelModelData RetrieveStatementData(string issuerId)
        {
            List<ModelConsensusEstimatesData> resultConsensus = new List<ModelConsensusEstimatesData>();
            List<FinancialStatementDataModels> resultReuters = new List<FinancialStatementDataModels>();
            List<FinancialStatementData> resultStatement = new List<FinancialStatementData>();
            List<string> commodities = new List<string>();
            ExcelModelData modelData = new ExcelModelData();
            List<DataPointsModelUploadData> dataPointsExcelUpload = new List<DataPointsModelUploadData>();
            ModelReferenceDataPoints dataPointsModelReference = new ModelReferenceDataPoints();
            string currencyReuters = "";
            string currencyConsensus = string.Empty;
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                if (issuerId == null)
                {
                    throw new Exception("Issuer Id is not Valid");
                }
                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ISSUER_ID == issuerId).FirstOrDefault();
                if (securityDetails == null)
                {
                    throw new Exception("Issuer Id is not Valid");
                }
                External_Country_Master countryDetails = entity.External_Country_Master
                .Where(record => record.COUNTRY_CODE == securityDetails.ISO_COUNTRY_CODE &&
                 record.COUNTRY_NAME == securityDetails.ASEC_SEC_COUNTRY_NAME)
                .FirstOrDefault();
                string issuerID = issuerId;
                string currency = countryDetails.CURRENCY_CODE;
                if (currency != null)
                {
                    resultReuters = RetrieveFinancialData(issuerID, currency);
                    currencyReuters = currency;
                    currencyConsensus = currency;
                }
                if (resultReuters != null)
                {
                    resultReuters = resultReuters.Where(a => a.PeriodYear != 2300).ToList();
                }
                if (resultReuters == null || resultReuters.Count == 0)
                {
                    if (currency != "USD")
                    {
                        resultReuters = RetrieveFinancialData(issuerID, "USD");
                        currencyReuters = "USD";
                    }
                    else
                    {
                        resultReuters = new List<FinancialStatementDataModels>();
                    }
                }
                resultReuters = resultReuters.Where(a => a.PeriodYear != 2300).ToList();

                if (resultConsensus == null || resultConsensus.Count == 0)
                {
                    if (currency != "USD")
                    {
                        resultConsensus = RetrieveModelConsensusData(issuerID, "USD");
                        currencyConsensus = "USD";
                    }
                    else
                    {
                        resultConsensus = new List<ModelConsensusEstimatesData>();
                    }
                }
                
                if (resultReuters == null || resultReuters.Count == 0)
                {
                    throw new Exception("No Data Returned from server");
                }
                dataPointsExcelUpload = RetrieveModelUploadDataPoints(issuerID);
                commodities = entity.RetrieveCommodityForecasts().ToList();
                ExcelModelData excelModelData = new ExcelModelData();
                excelModelData.ModelReferenceData = dataPointsModelReference;
                excelModelData.ModelUploadDataPoints = dataPointsExcelUpload;
                excelModelData.Currencies = entity.RetrieveDistinctFXRates().ToList();
                excelModelData.Commodities = commodities;
                excelModelData.ReutersData = resultReuters;
                excelModelData.CurrencyReuters = currencyReuters;
                excelModelData.ConsensusEstimateData = resultConsensus;
                return excelModelData;
            }
            catch (Exception ex)
            {
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieve Reuters Data
        /// </summary>
        /// <param name="issuerId">Issuer Id of Security</param>
        /// <param name="currency">Currency</param>
        /// <returns>Collection of FinancialStatementDataModels</returns>
        private List<FinancialStatementDataModels> RetrieveFinancialData(string issuerId, string currency)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                List<FinancialStatementDataModels> resultReuters = new List<FinancialStatementDataModels>();
                List<FinancialStatementType> statementType = new List<FinancialStatementType>() { FinancialStatementType.INCOME_STATEMENT, FinancialStatementType.BALANCE_SHEET, FinancialStatementType.CASH_FLOW_STATEMENT };
                List<FinancialStatementPeriodType> periodType = new List<FinancialStatementPeriodType>() { FinancialStatementPeriodType.ANNUAL, FinancialStatementPeriodType.QUARTERLY };
                List<FinancialStatementDataModels> resultStatement = new List<FinancialStatementDataModels>();

                foreach (FinancialStatementType item in statementType)
                {
                    string statement = EnumUtils.ToString(item);
                    foreach (FinancialStatementPeriodType period in periodType)
                    {
                        resultStatement = entity.Get_Statement_Models(issuerId, "REUTERS", EnumUtils.ToString(period).Substring(0, 1), "FISCAL", statement, currency).ToList();
                        if (resultStatement != null)
                        {
                            resultReuters.AddRange(resultStatement);
                        }
                    }
                }
                return resultReuters;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                return null;
            }
        }

        /// <summary>
        /// Retrieve List of DataPoints for Model-Upload worksheet
        /// </summary>
        /// <param name="issuerId">Issuer Id of the Selected Security</param>
        /// <returns>List of DataPointsModelUploadData</returns>
        private List<DataPointsModelUploadData> RetrieveModelUploadDataPoints(string issuerId)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                List<DataPointsModelUploadData> result = new List<DataPointsModelUploadData>();
                result = entity.RetrieveDataPointsModelUpload(issuerId).ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                return null;
            }
        }

        /// <summary>
        /// Retrieve Consensus Data for the Selected Security
        /// </summary>
        /// <param name="issuerId">Issuer id of the selected security</param>
        /// <param name="currency">Selected currency</param>
        /// <returns>List of ModelConsensusEstimatesData</returns>
        private List<ModelConsensusEstimatesData> RetrieveModelConsensusData(string issuerId, string currency)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                List<ModelConsensusEstimatesData> resultConsensus = new List<ModelConsensusEstimatesData>();
                List<ModelConsensusEstimatesData> data = new List<ModelConsensusEstimatesData>();
                List<FinancialStatementType> statementType = new List<FinancialStatementType>() { FinancialStatementType.INCOME_STATEMENT, FinancialStatementType.BALANCE_SHEET, FinancialStatementType.CASH_FLOW_STATEMENT };
                List<FinancialStatementPeriodType> periodType = new List<FinancialStatementPeriodType>() { FinancialStatementPeriodType.ANNUAL, FinancialStatementPeriodType.QUARTERLY };

                foreach (FinancialStatementPeriodType item in periodType)
                {
                    data = entity.GetModelConsensusEstimates(issuerId, "REUTERS", EnumUtils.ToString(item).Substring(0, 1), "FISCAL", currency).ToList();
                    if (data != null)
                    {
                        resultConsensus.AddRange(data);
                    }
                }
                foreach (ModelConsensusEstimatesData item in resultConsensus)
                {
                    item.SortOrder = ReturnSortOrder(item.ESTIMATE_ID);
                }
                return resultConsensus.OrderBy(a => a.SortOrder).ThenBy(a => a.PERIOD_YEAR).ThenBy(a => a.PERIOD_TYPE).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                return null;
            }
        }

        /// <summary>
        /// Return the Sort Order in the grid
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        private int ReturnSortOrder(int dataId)
        {
            switch (dataId)
            {
                case 17:
                    return 0;
                case 7:
                    return 1;
                case 6:
                    return 2;
                case 14:
                    return 3;
                case 11:
                    return 4;
                case 8:
                    return 5;
                case 10:
                    return 6;
                case 1:
                    return 7;
                case 18:
                    return 8;
                case 19:
                    return 9;
                case 2:
                    return 10;
                case 3:
                    return 11;
                case 4:
                    return 12;
                default:
                    return 13;
            }
        }
    }
}
