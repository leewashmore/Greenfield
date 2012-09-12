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
    /// <summary>
    /// DCF Operations
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DCFOperations
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
                    return new List<DCFAnalysisSummaryData>();

                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                #endregion

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                    return new List<DCFAnalysisSummaryData>();

                dbResult = entity.RetrieveDCFAnalysisSummaryData(issuerId, "PRIMARY", "A", "FISCAL", "USD").ToList();
                modelData = entity.GetDCFRiskFreeRate(Convert.ToString(securityDetails.ISO_COUNTRY_CODE)).FirstOrDefault();
                marketCap = Convert.ToDecimal(entity.GetDCFMarketCap(Convert.ToString(securityDetails.SECURITY_ID)).FirstOrDefault());

                DCFAnalysisSummaryData data = new DCFAnalysisSummaryData();

                data.SecurityId = securityDetails.ASEC_SEC_SHORT_NAME;
                data.IssuerId = securityDetails.ISSUER_ID;
                data.Beta = (securityDetails.BARRA_BETA == null) ?
                    (Convert.ToDecimal(securityDetails.BETA)) : (Convert.ToDecimal(securityDetails.BARRA_BETA));
                data.CostOfDebt = Convert.ToDecimal(securityDetails.WACC_COST_DEBT);
                data.MarginalTaxRate = dbResult.Where(a => a.DATA_ID == 232).Select(a => a.AMOUNT).FirstOrDefault();
                data.GrossDebt = dbResult.Where(a => a.DATA_ID == 90).Select(a => a.AMOUNT).FirstOrDefault();
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
                Dictionary<string, decimal> dataROIC_SDPR = new Dictionary<string, decimal>();
                List<DCFCashFlowData> cashFlowResult = new List<DCFCashFlowData>();
                string issuerId;

                ExternalResearchEntities entity = new ExternalResearchEntities();

                if (entitySelectionData == null)
                    return new List<DCFTerminalValueCalculationsData>();

                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                #endregion

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                    return new List<DCFTerminalValueCalculationsData>();

                //cashFlowResult = entity.GetDCFCashFlow(issuerId).ToList();

                decimal longTermGDPGrowth = Convert.ToDecimal(entity.GetDCFGDP(Convert.ToString(securityDetails.ISO_COUNTRY_CODE)).FirstOrDefault());
                DCFTerminalValueCalculationsData data = new DCFTerminalValueCalculationsData();

                dataROIC_SDPR = GetROIC(issuerId);

                if (dataROIC_SDPR.ContainsKey("ROIC"))
                {
                    data.SustainableROIC = dataROIC_SDPR.Where(a => a.Key == "ROIC").Select(a => a.Value).FirstOrDefault();
                }
                if (dataROIC_SDPR.ContainsKey("SDPR"))
                {
                    data.SustainableDividendPayoutRatio = dataROIC_SDPR.Where(a => a.Key == "SDPR").Select(a => a.Value).FirstOrDefault();
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

                //data.ISSUER_ID = "920028";
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
                //decimal? amount ;
                //item.FieldName = "Revenue Growth";
                //amount = Convert.ToDecimal(resultDB.Where(a => (a.PERIOD_YEAR == DateTime.Now.Year && a.FIELD_NAME == "Revenue Growth")).Select(a => a.AMOUNT));
                //if(amount != null)
                //item.ValueY0 = Convert.ToString(amount) + "%";

                //amount = Convert.ToDecimal(resultDB.Where(a => (a.PERIOD_YEAR == DateTime.Now.Year && a.FIELD_NAME == "Revenue Growth")).Select(a => a.AMOUNT));
                //if (amount != null)
                //    item.ValueY0 = Convert.ToString(amount) + "%";



                //    result.Add(item);
                //}
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
                    return new List<DCFCashFlowData>();

                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                #endregion

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                string issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                    return new List<DCFCashFlowData>();

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
                }

                decimal average = result.Where(a => a.PERIOD_YEAR < currentYear + 5).Select(a => Convert.ToDecimal(a.AMOUNT)).Average();

                for (int i = 5; i < 10; i++)
                {
                    result.Add(new DCFCashFlowData() { AMOUNT = 0, DISCOUNTING_FACTOR = 0, PERIOD_YEAR = currentYear + i });
                }

                foreach (DCFCashFlowData item in result)
                {
                    if (item.PERIOD_YEAR > currentYear + 4)
                    {
                        item.AMOUNT = average * Convert.ToDecimal(Math.Pow((0.99), Convert.ToDouble(item.PERIOD_YEAR - (currentYear + 4))));
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
                    return new List<DCFSummaryData>();

                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                #endregion

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                string issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                    return new List<DCFSummaryData>();

                dbResult = entity.GetDCFSummaryData(issuerId).ToList();
                dbResultShares = entity.GetDCF_NumberOfShares(Convert.ToString(securityDetails.SECURITY_ID)).ToList();

                DCFSummaryData data = new DCFSummaryData();

                data.Cash = dbResult.Where(a => a.DATA_ID == 255).Select(a => a.AMOUNT).FirstOrDefault();
                data.FVInvestments = dbResult.Where(a => a.DATA_ID == 258).Select(a => a.AMOUNT).FirstOrDefault();
                data.GrossDebt = dbResult.Where(a => a.DATA_ID == 256).Select(a => a.AMOUNT).FirstOrDefault();
                data.FVMinorities = dbResult.Where(a => a.DATA_ID == 257).Select(a => a.AMOUNT).FirstOrDefault();
                data.NumberOfShares = dbResult.Select(a => a.AMOUNT).FirstOrDefault();
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public decimal? RetrieveCurrentPriceData(EntitySelectionData entitySelectionData)
        {
            try
            {
                if (entitySelectionData == null)
                    return 0;

                #region ServiceAvailabilityChecker

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                #endregion

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();
                if (securityDetails == null)
                    return 0;

                return Convert.ToDecimal(securityDetails.CLOSING_PRICE);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }


        #region HelperMethods

        /// <summary>
        /// Method to fetch the values of ROIC
        /// </summary>
        /// <param name="issuerid">IssuerId of Selected Security</param>
        /// <returns>Nullable Decimal as ROIC</returns>
        private Dictionary<string, decimal> GetROIC(string issuerid)
        {
            ExternalResearchEntities entity = new ExternalResearchEntities();
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();


            List<decimal?> collectionROIC = new List<decimal?>();
            List<decimal?> collectionSustainableDividendPayoutRatio = new List<decimal?>();

            decimal? valueROIC;
            decimal? valueSustainableDividendPayoutRatio;
            int currentYear = DateTime.Today.Year;

            for (int i = 0; i < 5; i++)
            {
                currentYear = currentYear + i;
                valueROIC = entity.GetDCF_ROIC(issuerid, currentYear, "PRIMARY", "A", "FISCAL", "USD").Where(a => a.DATA_ID == 162)
                    .Select(a => a.AMOUNT).FirstOrDefault();
                collectionROIC.Add(valueROIC);
                valueSustainableDividendPayoutRatio = entity.GetDCF_ROIC(issuerid, currentYear, "PRIMARY", "A", "FISCAL", "USD").Where(a => a.DATA_ID == 141)
                    .Select(a => a.AMOUNT).FirstOrDefault();
                collectionSustainableDividendPayoutRatio.Add(valueSustainableDividendPayoutRatio);
            }

            if (collectionROIC.Any(a => a.Value != null))
            {
                result.Add("ROIC", Convert.ToDecimal(collectionROIC.Average()));
            }
            if (collectionSustainableDividendPayoutRatio.Any(a => a.Value != null))
            {
                result.Add("SDPR", Convert.ToDecimal(collectionSustainableDividendPayoutRatio.Average()));
            }

            return result;
        }

        #endregion

    }
}
