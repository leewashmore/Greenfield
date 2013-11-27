using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using GreenField.DAL;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;

namespace GreenField.Web.Services
{
    /// <summary>
    /// DCF Operations
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DCFOperations
    {
        /// <summary>
        /// Service FaultResource manager
        /// </summary>
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        /// <summary>
        /// Instance of Dimension Service
        /// </summary>
        /*private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));

                return dimensionEntity;
            }
        }*/


        private DimensionEntities dimensionEntity;
        public DimensionEntities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                {
                    dimensionEntity = new GreenField.DAL.DimensionEntities();
                }
                return dimensionEntity;
            }
        }
        #region AnalysisSummary

        /// <summary>
        /// Operation Contract for DCFAnalysisSummary
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns>Collection of DCFAnalysisSummaryData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DCFAnalysisSummaryData> RetrieveDCFAnalysisData(EntitySelectionData entitySelectionData)
        {
            try
            {
                string issuerId;
                List<DCFAnalysisSummaryData> result = new List<DCFAnalysisSummaryData>();
                List<DCFAnalysisSummaryData_Result> dbResult;
                MODEL_INPUTS_CTY modelData = new MODEL_INPUTS_CTY();
                decimal marketCap;
                ExternalResearchEntities entity = new ExternalResearchEntities();
                if (entitySelectionData == null)
                {
                    return new List<DCFAnalysisSummaryData>();
                }
                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                #endregion

                GreenField.DAL.GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                if (securityDetails == null)
                {
                    return new List<DCFAnalysisSummaryData>();
                }
                issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                {
                    return new List<DCFAnalysisSummaryData>();
                }
                dbResult = entity.RetrieveDCFAnalysisSummaryData(issuerId, "PRIMARY", "C", "FISCAL", "USD").ToList();
                modelData = entity.GetDCFRiskFreeRate(Convert.ToString(securityDetails.ISO_COUNTRY_CODE)).FirstOrDefault();
                marketCap = Convert.ToDecimal(entity.GetDCFMarketCap(Convert.ToString(securityDetails.SECURITY_ID)).FirstOrDefault());
                DCFAnalysisSummaryData data = new DCFAnalysisSummaryData();
                data.SecurityId = securityDetails.ASEC_SEC_SHORT_NAME;
                data.IssuerId = securityDetails.ISSUER_ID;
                data.Beta = (securityDetails.BARRA_BETA == null) ?
                    (Convert.ToDecimal(securityDetails.BETA)) : (Convert.ToDecimal(securityDetails.BARRA_BETA));
                data.CostOfDebt = Convert.ToDecimal(securityDetails.WACC_COST_DEBT);
                data.MarginalTaxRate = dbResult.Where(a => a.DATA_ID == 289 && a.PERIOD_TYPE.Trim() == "C").Select(a => a.AMOUNT).FirstOrDefault();
                data.GrossDebt = dbResult.Where(a => a.DATA_ID == 256 && a.PERIOD_TYPE.Trim() == "C").Select(a => a.AMOUNT).FirstOrDefault();
                data.MarketCap = Convert.ToDecimal(marketCap);
                if (modelData != null)
                {
                    data.RiskFreeRate = (modelData.RISK_FREE_RATE != null ? modelData.RISK_FREE_RATE : 0);
                    data.MarketRiskPremium = (modelData.RISK_PREM != null ? modelData.RISK_PREM : 0);
                }
                result.Add(data);
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

        #region TerminalValueCalculations

        /// <summary>
        /// Terminal Value Calculations
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns>List of DCFTerminalValueCalculationsData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DCFTerminalValueCalculationsData> RetrieveTerminalValueCalculationsData(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<DCFTerminalValueCalculationsData> result = new List<DCFTerminalValueCalculationsData>();
                Dictionary<string, decimal?> dataROIC_SDPR = new Dictionary<string, decimal?>();
                List<DCFCashFlowData> cashFlowResult = new List<DCFCashFlowData>();
                string issuerId;
                ExternalResearchEntities entity = new ExternalResearchEntities();
                if (entitySelectionData == null)
                {
                    return new List<DCFTerminalValueCalculationsData>();
                }
                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                #endregion

                GreenField.DAL.GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                {
                    return new List<DCFTerminalValueCalculationsData>();
                }
                decimal longTermGDPGrowth = Convert.ToDecimal(entity.GetDCFGDP(Convert.ToString(securityDetails.ISO_COUNTRY_CODE)).FirstOrDefault());
                DCFTerminalValueCalculationsData data = new DCFTerminalValueCalculationsData();
                dataROIC_SDPR = GetROIC(issuerId);
                if (dataROIC_SDPR.ContainsKey("ROIC"))
                {
                    data.SustainableROIC = dataROIC_SDPR.Where(a => a.Key == "ROIC").Select(a => a.Value).FirstOrDefault();
                }
                if (dataROIC_SDPR.ContainsKey("SDPR"))
                {
                    data.SustainableDividendPayoutRatio = Convert.ToDecimal(dataROIC_SDPR.Where(a => a.Key == "SDPR").Select(a => a.Value).FirstOrDefault());
                }
                data.LongTermNominalGDPGrowth = longTermGDPGrowth;
                result.Add(data);
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
        /// Gets FreCashFlows Data
        /// </summary>
        /// <param name="securityId"></param>
        /// <returns>FreCashFlows data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FreeCashFlowsData> RetrieveFreeCashFlowsData(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<FreeCashFlowsData> result = new List<FreeCashFlowsData>();
                List<GetFreeCashFlows_Result> resultDB = new List<GetFreeCashFlows_Result>();
                ExternalResearchEntities dcf_FreeCashFlows = new ExternalResearchEntities();

                if (entitySelectionData == null)
                    return null;

                DimensionEntities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                //Retrieving data from security view
                GreenField.DAL.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (data == null)
                    return null;
                ////Retrieving data from Period Financials table
                resultDB = dcf_FreeCashFlows.ExecuteStoreQuery<GetFreeCashFlows_Result>("exec GetFreeCashFlows @IssuerID={0}", data.ISSUER_ID).ToList();

                foreach (GetFreeCashFlows_Result record in resultDB)
                {
                    FreeCashFlowsData item = new FreeCashFlowsData();
                    item.FieldName = record.FIELD_NAME;
                    item.PeriodYear = record.PERIOD_YEAR;
                    item.Amount = record.AMOUNT;
                    result.Add(item);
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
        /// Service Method to Retrieve Cash Flow Values
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DCFCashFlowData> RetrieveCashFlows(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<DCFCashFlowData> result = new List<DCFCashFlowData>();
                List<DCFCashFlowData> dbResult = new List<DCFCashFlowData>();

                ExternalResearchEntities entity = new ExternalResearchEntities();

                if (entitySelectionData == null)
                {
                    return new List<DCFCashFlowData>();
                }
                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                #endregion

                GreenField.DAL.GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                string issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                {
                    return new List<DCFCashFlowData>();
                }
                dbResult = entity.GetDCFCashFlow(issuerId).OrderBy(a => a.PERIOD_YEAR).ToList();
                if (dbResult == null || dbResult.Count == 0)
                {
                    return new List<DCFCashFlowData>();
                }
                int currentYear = DateTime.Today.Year;
                for (int i = 0; i < 10; i++)
                {
                    if (dbResult.Where(a => a.PERIOD_YEAR == currentYear + i).FirstOrDefault() != null)
                    {
                        result.Add(dbResult.Where(a => a.PERIOD_YEAR == currentYear + i).FirstOrDefault());
                    }
                    else
                    {
                        result.Add(new DCFCashFlowData() { AMOUNT = 0, DISCOUNTING_FACTOR = 0, PERIOD_YEAR = currentYear + i });
                    }
                }
                decimal average = result.Where(a => a.PERIOD_YEAR < currentYear + 5).Select(a => Convert.ToDecimal(a.AMOUNT)).Sum() / 5;
                foreach (DCFCashFlowData item in result)
                {
                    if (item.PERIOD_YEAR > currentYear + 4)
                    {
                        item.AMOUNT = average * Convert.ToDecimal(Math.Pow((0.99), Convert.ToDouble(item.PERIOD_YEAR - (currentYear + 4))));
                    }
                    item.FREE_CASH_FLOW = item.AMOUNT;
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
        /// Retrieve data for DCFSummaryData 
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns>List of DCFSummaryData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DCFSummaryData> RetrieveSummaryData(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<DCFSummaryData> result = new List<DCFSummaryData>();
                List<DCFSummaryDBData> dbResult = new List<DCFSummaryDBData>();
                List<DCFSummaryDBData> dbResultShares = new List<DCFSummaryDBData>();
                ExternalResearchEntities entity = new ExternalResearchEntities();
                if (entitySelectionData == null)
                {
                    return new List<DCFSummaryData>();
                }
                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                #endregion

                GreenField.DAL.GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                string issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                {
                    return new List<DCFSummaryData>();
                }
                dbResult = entity.GetDCFSummaryData(issuerId).ToList();
                dbResultShares = entity.GetDCF_NumberOfShares(Convert.ToString(securityDetails.SECURITY_ID)).ToList();
                DCFSummaryData data = new DCFSummaryData();
                data.Cash = dbResult.Where(a => a.DATA_ID == 255).Select(a => a.AMOUNT).FirstOrDefault();
                data.FVInvestments = dbResult.Where(a => a.DATA_ID == 258).Select(a => a.AMOUNT).FirstOrDefault();
                data.GrossDebt = dbResult.Where(a => a.DATA_ID == 256).Select(a => a.AMOUNT).FirstOrDefault();
                data.FVMinorities = dbResult.Where(a => a.DATA_ID == 257).Select(a => a.AMOUNT).FirstOrDefault();
                data.NumberOfShares = dbResultShares.Select(a => a.AMOUNT).FirstOrDefault();
                result.Add(data);
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
        /// Method to fetch the Current Price
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public decimal? RetrieveCurrentPriceData(EntitySelectionData entitySelectionData)
        {
            try
            {
                if (entitySelectionData == null)
                {
                    return 0;
                }
                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                #endregion

                GreenField.DAL.GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();
                if (securityDetails == null)
                {
                    return 0;
                }
                return Convert.ToDecimal(securityDetails.CLOSING_PRICE);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Method to Fetch the Country Name of Selected Security
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns>Name of the Country</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public string RetrieveCountryName(EntitySelectionData entitySelectionData)
        {
            try
            {
                string countryName = string.Empty;
                GreenField.DAL.GF_SECURITY_BASEVIEW data = DimensionEntity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME == entitySelectionData.LongName).FirstOrDefault();
                if (data != null)
                {
                    countryName = data.ASEC_SEC_COUNTRY_NAME;
                }
                if (countryName == null)
                {
                    return string.Empty;
                }
                return countryName;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region FairValue

        /// <summary>
        /// Gets DCF FairValues
        /// </summary>
        /// <param name="securityId">Security ID of selected Security</param>
        /// <returns>Collection of Type Period_Financials</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PERIOD_FINANCIALS> RetrieveFairValue(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<PERIOD_FINANCIALS> result = new List<PERIOD_FINANCIALS>();
                GreenField.DAL.GF_SECURITY_BASEVIEW data = DimensionEntity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME == entitySelectionData.LongName).FirstOrDefault();
                if (data == null)
                {
                    return new List<PERIOD_FINANCIALS>();
                }
                int? securityId = int.Parse(data.SECURITY_ID);
                if (securityId == null)
                {
                    return new List<PERIOD_FINANCIALS>();
                }
                ExternalResearchEntities entity = new ExternalResearchEntities();
                result = entity.GetDCFFairValue(Convert.ToString(securityId)).ToList();
                if (result == null)
                {
                    return new List<PERIOD_FINANCIALS>();
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
        /// Service Method to Insert value in FAIR_VALUE
        /// </summary>
        /// <param name="entitySelectionData">selected Security</param>
        /// <param name="valueType">ValueType</param>
        /// <param name="fvMeasure">FV_MEasure</param>
        /// <param name="fvbuy">FV_BUY</param>
        /// <param name="fvSell">FV_SELL</param>
        /// <param name="currentMeasureValue">Current Measure Value</param>
        /// <param name="upside">Upside</param>
        /// <param name="updated">Updated</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool InsertFairValues(EntitySelectionData entitySelectionData, string valueType, int? fvMeasure, decimal? fvbuy, decimal? fvSell, decimal? currentMeasureValue, decimal? upside, DateTime? updated)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                GreenField.DAL.GF_SECURITY_BASEVIEW data = DimensionEntity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME == entitySelectionData.LongName).FirstOrDefault();
                if (data == null)
                {
                    return false;
                }
                int? securityId = int.Parse(data.SECURITY_ID);
                if (securityId == null)
                {
                    return false;
                }
                if (valueType != null)
                    valueType = valueType.ToUpper();

                entity.InsertDCFFairValue(Convert.ToString(securityId), valueType, fvMeasure, fvbuy, fvSell, currentMeasureValue, upside, updated,"C",0);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Service method to Delete Existing fair value records for a Security
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns>Result of the Operation</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public bool DeleteFairValues(EntitySelectionData entitySelectionData)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                GreenField.DAL.GF_SECURITY_BASEVIEW data = DimensionEntity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME == entitySelectionData.LongName).FirstOrDefault();
                if (data == null)
                {
                    return false;
                }
                int? securityId = int.Parse(data.SECURITY_ID);
                if (securityId == null)
                {
                    return false;
                }
                entity.DeleteDCFFairValueData(Convert.ToString(securityId));
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Method to fetch the values of ROIC
        /// </summary>
        /// <param name="issuerid">IssuerId of Selected Security</param>
        /// <returns>Nullable Decimal as ROIC</returns>
        private Dictionary<string, decimal?> GetROIC(string issuerid)
        {
            ExternalResearchEntities entity = new ExternalResearchEntities();
            Dictionary<string, decimal?> result = new Dictionary<string, decimal?>();
            List<decimal?> collectionROIC = new List<decimal?>();
            List<decimal?> collectionSustainableDividendPayoutRatio = new List<decimal?>();
            decimal? valueROIC;
            decimal? valueSustainableDividendPayoutRatio;
            int currentYear = DateTime.Today.Year;
            for (int i = 0; i < 5; i++)
            {
                List<DCF_ROICResult> res = entity.GetDCF_ROIC(issuerid, currentYear + i, "PRIMARY", "A", "FISCAL", "USD").ToList();
                if (res.Where(a => a.DATA_ID == 162 && a.FISCAL == "CALENDAR").ToList().Count > 0)
                {
                    valueROIC = res.Where(a => a.DATA_ID == 162 && a.FISCAL.ToUpper().Trim() == "CALENDAR").Select(a => a.AMOUNT).FirstOrDefault();
                }
                else
                {
                    valueROIC = null;
                }
                if (valueROIC != null)
                {
                    collectionROIC.Add(valueROIC);
                }
                else
                {
                    collectionROIC.Add(null);
                }
                valueSustainableDividendPayoutRatio = res.Where(a => a.DATA_ID == 141 && a.FISCAL.ToUpper().Trim() == "FISCAL")
                    .Select(a => a.AMOUNT).FirstOrDefault();
                if (valueSustainableDividendPayoutRatio != null)
                {
                    collectionSustainableDividendPayoutRatio.Add(valueSustainableDividendPayoutRatio);
                }
            }

            if (collectionROIC.All(a => a != null))
            {
                if (Convert.ToDecimal(collectionROIC.Average()) != 0)
                {
                    result.Add("ROIC", Convert.ToDecimal(collectionROIC.Average()));
                }
                else
                {
                    result.Add("ROIC", 0);
                }
            }
            else
            {
                result.Add("ROIC", null);
            }

            if (collectionSustainableDividendPayoutRatio.Any(a => a.Value != null))
            {
                if (Convert.ToDecimal(collectionSustainableDividendPayoutRatio.Average()) != 0)
                {
                    result.Add("SDPR", Convert.ToDecimal(collectionSustainableDividendPayoutRatio.Average()));
                }
                else
                {
                    result.Add("SDPR", Convert.ToDecimal(0.3333));
                }
            }
            else
            {
                result.Add("SDPR", Convert.ToDecimal(0.3333));
            }

            return result;
        }

        #endregion

    }
}