using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using GreenField.DAL;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;
using System.Diagnostics;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for External Research
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ExternalResearchOperations
    {
        /// <summary>
        /// Fault Resource Manager
        /// </summary>
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        /// <summary>
        /// Instance of DimensionService
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
                String industryName = securityDetails.GICS_INDUSTRY_NAME;
                int? securityID = securityDetails.SECURITY_ID;
                String issueName = securityDetails.ISSUE_NAME;
                String subIndustryName = securityDetails.GICS_SUB_INDUSTRY_NAME;
                String ticker = securityDetails.TICKER;
                String currency = securityDetails.TRADING_CURRENCY;
                String primaryAnalyst = securityDetails.ASHMOREEMM_PRIMARY_ANALYST;
                String industryAnalyst = securityDetails.ASHMOREEMM_INDUSTRY_ANALYST;

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
                    IndustryName = industryName,
                    SecurityId = securityID,
                    IssueName = issueName,
                    SubIndustryName = subIndustryName,
                    Ticker = ticker,
                    TradingCurrency = currency,
                    PrimaryAnalyst = primaryAnalyst,
                    IndustryAnalyst = industryAnalyst
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
        /// <param name="issuerId">string</param>
        /// <param name="periodType">FinancialStatementPeriodType</param>
        /// <param name="currency">String</param>
        /// <returns>list of ConsensusEstimateDetail</returns>
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

                if (data == null)
                { return result; }

                decimal previousYearQuarterAmount;
                data = data.OrderBy(record => record.ESTIMATE_DESC).ThenByDescending(record => record.PERIOD_YEAR).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    ConsensusEstimateDetail temp = new ConsensusEstimateDetail();
                    if (data[i].ESTIMATE_ID == 17)
                        temp.SortOrder = 1;
                    else if (data[i].ESTIMATE_ID == 7)
                        temp.SortOrder = 2;
                    else if (data[i].ESTIMATE_ID == 11)
                        temp.SortOrder = 3;
                    else if (data[i].ESTIMATE_ID == 8)
                        temp.SortOrder = 4;
                    else if (data[i].ESTIMATE_ID == 18)
                        temp.SortOrder = 5;
                    else if (data[i].ESTIMATE_ID == 19)
                        temp.SortOrder = 6;

                    temp.IssuerId = data[i].ISSUER_ID;
                    temp.EstimateId = data[i].ESTIMATE_ID;
                    temp.Description = data[i].ESTIMATE_DESC;
                    temp.Period = data[i].Period;
                    temp.AmountType = data[i].AMOUNT_TYPE;
                    temp.PeriodYear = data[i].PERIOD_YEAR;
                    temp.PeriodType = data[i].PERIOD_TYPE;
                    temp.Amount = data[i].AMOUNT;
                    temp.AshmoreEmmAmount = data[i].ASHMOREEMM_AMOUNT;
                    temp.NumberOfEstimates = data[i].NUMBER_OF_ESTIMATES;
                    temp.High = data[i].HIGH;
                    temp.Low = data[i].LOW;
                    temp.StandardDeviation = data[i].STANDARD_DEVIATION;
                    temp.SourceCurrency = data[i].SOURCE_CURRENCY;
                    temp.DataSource = data[i].DATA_SOURCE;
                    temp.DataSourceDate = data[i].DATA_SOURCE_DATE;
                    temp.Actual = data[i].ACTUAL;
                    temp.ConsensusMedian = data[i].AMOUNT;
                    temp.YOYGrowth = data[i].AMOUNT;
                    temp.Variance = data[i].AMOUNT == 0 ? null : ((data[i].ASHMOREEMM_AMOUNT / data[i].AMOUNT) - 1) * 100;

                    previousYearQuarterAmount = data.Where(a => a.ESTIMATE_DESC == data[i].ESTIMATE_DESC && a.PERIOD_YEAR == (data[i].PERIOD_YEAR - 1)
                        && a.PERIOD_TYPE == data[i].PERIOD_TYPE).Select(a => a.AMOUNT).FirstOrDefault();

                    if (previousYearQuarterAmount == 0)
                    {
                        temp.YOYGrowth = 0;
                    }
                    else
                    {
                        temp.YOYGrowth = (temp.YOYGrowth / previousYearQuarterAmount - 1) * 100;
                    }
                    result.Add(temp);
                }
                return result.OrderBy(a => a.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get data for Broker Details In Consensus Detail gadget
        /// </summary>
        /// <param name="issuerId">string</param>
        /// <param name="periodType">FinancialStatementPeriodType</param>
        /// <param name="currency">String</param>
        /// <returns>list of ConsensusEstimateDetail</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<ConsensusEstimateDetail> RetrieveConsensusEstimateDetailedBrokerData(string issuerId, FinancialStatementPeriodType periodType, String currency)
        {
            try
            {
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);
                ExternalResearchEntities entity = new ExternalResearchEntities();
                entity.CommandTimeout = 100;
                List<BrokerDetailData> data = new List<BrokerDetailData>();
                List<ConsensusEstimateDetail> result = new List<ConsensusEstimateDetail>();

                data = entity.GetBrokerDetail(issuerId, null, _periodType, currency).ToList();

                if (data == null)
                { return result; }

                List<BrokerDetailData> requiredBrokerDetailsList = new List<BrokerDetailData>();

                #region Mapping Broker's Estimate_Type with Consensus Detail's Estimate_Desc
                foreach (BrokerDetailData item in data)
                {
                    switch (item.EstimateType)
                    {
                        case "EPS":
                            item.EstimateType = "Earnings Per Share (Pre Exceptional)";
                            item.fPeriodEnd = item.fPeriodEnd.Substring(0, 4);
                            requiredBrokerDetailsList.Add(item);
                            break;
                        case "EBITDA":
                            item.fPeriodEnd = item.fPeriodEnd.Substring(0, 4);
                            requiredBrokerDetailsList.Add(item);
                            break;
                        case "NTP":
                            item.EstimateType = "Net Income (Pre Exceptional)";
                            item.fPeriodEnd = item.fPeriodEnd.Substring(0, 4);
                            requiredBrokerDetailsList.Add(item);
                            break;
                        case "ROE":
                            item.EstimateType = "Return on Equity";
                            item.fPeriodEnd = item.fPeriodEnd.Substring(0, 4);
                            requiredBrokerDetailsList.Add(item);
                            break;
                        case "ROA":
                            item.EstimateType = "Return on Assets";
                            item.fPeriodEnd = item.fPeriodEnd.Substring(0, 4);
                            requiredBrokerDetailsList.Add(item);
                            break;
                        case "REVENUE":
                            item.EstimateType = "Revenue";
                            item.fPeriodEnd = item.fPeriodEnd.Substring(0, 4);
                            requiredBrokerDetailsList.Add(item);
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                for (int i = 0; i < requiredBrokerDetailsList.Count; i++)
                {
                    ConsensusEstimateDetail temp = new ConsensusEstimateDetail();
                    if (data[i].EstimateType == "Revenue")
                        temp.SortOrder = 1;
                    else if (data[i].EstimateType == "EBITDA")
                        temp.SortOrder = 2;
                    else if (data[i].EstimateType == "Net Income (Pre Exceptional)")
                        temp.SortOrder = 3;
                    else if (data[i].EstimateType == "Earnings Per Share (Pre Exceptional)")
                        temp.SortOrder = 4;
                    else if (data[i].EstimateType == "Return on Assets")
                        temp.SortOrder = 5;
                    else if (data[i].EstimateType == "Return on Equity")
                        temp.SortOrder = 6;
                    temp.Description = requiredBrokerDetailsList[i].EstimateType;
                    temp.GroupDescription = requiredBrokerDetailsList[i].broker_name;
                    temp.AmountType = "A";
                    temp.PeriodYear = Convert.ToInt32(requiredBrokerDetailsList[i].fPeriodEnd);
                    temp.PeriodType = _periodType;
                    temp.Amount = Convert.ToDecimal(requiredBrokerDetailsList[i].Amount);
                    temp.ReportedCurrency = requiredBrokerDetailsList[i].Reported_Currency;
                    temp.LastUpdateDate = requiredBrokerDetailsList[i].Last_Update_Date.Date.ToShortDateString();
                    result.Add(temp);
                }
                return result.OrderBy(a => a.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get data for Finstat Gadget  //example of usage of Multiplier
        /// </summary>
        /// <param name="issuerId">string</param>
        /// <param name="securityId">string</param>
        /// <param name="dataSource">FinancialStatementDataSource</param>
        /// <param name="fiscalType">FinancialStatementFiscalType</param>
        /// <param name="currency">String</param>
        /// <param name="yearRange">Int32</param>
        /// <returns>list of FinstatDetailData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FinstatDetailData> RetrieveFinstatData(string issuerId, string securityId, FinancialStatementDataSource dataSource,
            FinancialStatementFiscalType fiscalType, String currency, Int32 yearRangeStart)
        {
            try
            {
                string _dataSource = EnumUtils.ToString(dataSource);
                string _fiscalType = EnumUtils.ToString(fiscalType);
                ExternalResearchEntities entity = new ExternalResearchEntities();
                entity.CommandTimeout = 300;
                List<FinstatDetail> data = new List<FinstatDetail>();
                List<FinstatDetailData> result = new List<FinstatDetailData>();

                data = entity.GetFinstatDetail(issuerId, securityId, _dataSource, _fiscalType, currency).ToList();
                if (data == null || data.Count() == 0)
                { return result; }

                #region DataSource group
                List<int> distinctPeriodYear = data.Select(a => a.PERIOD_YEAR).Distinct().ToList();
                List<FinstatDetail> distinctRootSource = data.Where(a => a.ROOT_SOURCE != null && a.DATA_SOURCE != null)
                    .OrderBy(a => a.PERIOD_YEAR).ThenBy(a => a.DATA_SOURCE).ToList();

                foreach (int item in distinctPeriodYear)
                {
                    

                    FinstatDetailData temp = new FinstatDetailData();
                    temp.GroupDescription = "Data Source";
                    temp.Description = "Source";
                    temp.PeriodType = "A";
                    List<string> isDataSourceMixed = distinctRootSource.Where(a => a.PERIOD_YEAR == item).Select(a => a.DATA_SOURCE).Distinct().ToList();
                    temp.Amount = (isDataSourceMixed.Count > 1) ? "MIXED" : distinctRootSource.Where(a => a.PERIOD_YEAR == item)
                        .Select(a => a.DATA_SOURCE).FirstOrDefault(); ;
                    //temp.Amount = _dataSource;
                    temp.RootSource = _dataSource;
                    temp.RootSourceDate = DateTime.Now;
                    temp.PeriodYear = item;
                    temp.AmountType = "A";
                    temp.BoldFont = "N";
                    temp.IsPercentage = "N";
                    result.Add(temp);

                    FinstatDetailData tempData = new FinstatDetailData();
                    tempData.GroupDescription = "Data Source";
                    tempData.Description = "Root Source";
                    tempData.PeriodType = "A";
                    List<string> isRootSourceMixed = distinctRootSource.Where(a => a.PERIOD_YEAR == item).Select(a => a.ROOT_SOURCE).Distinct().ToList();
                    tempData.Amount = (isRootSourceMixed.Count > 1) ? "MIXED" : distinctRootSource.Where(a => a.PERIOD_YEAR == item)
                        .Select(a => a.ROOT_SOURCE).FirstOrDefault();
                    tempData.RootSource = _dataSource;
                    tempData.RootSourceDate = DateTime.Now;
                    tempData.PeriodYear = item;
                    tempData.AmountType = "A";
                    tempData.BoldFont = "N";
                    tempData.IsPercentage = "N";
                    result.Add(tempData);
                }
                #endregion

                #region Preparing display data for group names
                for (int i = 0; i < data.Count(); i++)
                {
                    FinstatDetailData temp = new FinstatDetailData();
                    temp.Amount = Convert.ToDecimal(data[i].AMOUNT * data[i].MULTIPLIER);
                    temp.BoldFont = data[i].BOLD_FONT;
                    temp.Description = data[i].DATA_DESC;
                    temp.Decimals = data[i].DECIMALS;
                    temp.GroupDescription = data[i].GROUP_NAME;
                    temp.Harmonic = data[i].HARMONIC;
                    temp.IsPercentage = data[i].PERCENTAGE;
                    temp.PeriodYear = Convert.ToInt32(data[i].PERIOD_YEAR);
                    temp.SortOrder = data[i].SORT_ORDER;
                    temp.AmountType = "A";
                    temp.PeriodType = "A";
                    temp.RootSource = data[i].ROOT_SOURCE;
                    temp.RootSourceDate = Convert.ToDateTime(data[i].ROOT_SOURCE_DATE);
                    if (data[i].HARMONIC == "Y")
                    {
                       /* decimal? year1 = 0, year2 = 0, year3 = 0, year4 = 0, year5 = 0, year6 = 0;

                        decimal year1Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR - 2
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year1 = (year1Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year1Value));
                        decimal year2Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR - 1
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year2 = (year2Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year2Value));
                        decimal year3Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year3 = (year3Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year3Value));
                        decimal year4Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 1
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year4 = (year4Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year4Value));
                        decimal year5Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 2
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year5 = (year5Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year5Value));
                        decimal year6Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 3
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year6 = (year6Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year6Value));

                        if (year1 != 0 && year2 != 0 && year3 != 0 && year1 != null && year2 != null && year3 != null)
                        {
                            temp.HarmonicFirst = Convert.ToDecimal((1 / (year1 + year2 + year3)) * data[i].MULTIPLIER);
                        }
                        if (year4 != 0 && year5 != 0 && year6 != 0 && year4 != null && year5 != null && year6 != null)
                        {
                            temp.HarmonicSecond = Convert.ToDecimal((1 / (year4 + year5 + year6)) * data[i].MULTIPLIER);
                        }*/

                        // Do not get confused . Even though we check for the flag HARMONIC=Y  , we use only Simple Average not Harmonic Mean. This is validated by Gerred Howe. Picked up from HeadStrong code 
                        //and modified it  - Akhtar (06/27/2013)
                        List<decimal?> listFirst = new List<decimal?>();
                        
                        listFirst.Add(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR - 2
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        listFirst.Add(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR - 1
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        listFirst.Add(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        temp.HarmonicFirst = GroupCalculations.SimpleAverage(listFirst) * data[i].MULTIPLIER;
                        List<decimal?> listSecond = new List<decimal?>();
                        listSecond.Add(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 1
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        listSecond.Add(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 2
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        listSecond.Add(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 3
                                                    && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        temp.HarmonicSecond = GroupCalculations.SimpleAverage(listSecond) * data[i].MULTIPLIER;
                    }
                    result.Add(temp);
                }
                #endregion

                #region Economic & Market Data
                List<FinstatEconomicMarketData> economicData = entity.GetFinstatEconomicMarketData(issuerId, securityId, _dataSource, _fiscalType, currency).ToList();

                //Preparing Exchange Rates
                List<string> distinctYears = economicData.Select(a => a.PERIOD_YEAR).Distinct().ToList();
                foreach (string item in distinctYears)
                {
                    FinstatDetailData tempData = new FinstatDetailData();
                    tempData.GroupDescription = "Economic & Market Data";
                    tempData.Description = "Exchange Rate (Year-End)";
                    tempData.PeriodYear = Convert.ToInt32(item);
                    tempData.AmountType = "A";
                    tempData.PeriodType = "A";
                    tempData.BoldFont = "N";
                    tempData.IsPercentage = "N";
                    tempData.RootSource = _dataSource;
                    tempData.RootSourceDate = DateTime.Now;
                    tempData.Amount = Convert.ToDecimal(economicData.Where(a => a.PERIOD_YEAR == item).Select(a => a.FX_RATE).FirstOrDefault());
                    result.Add(tempData);

                    FinstatDetailData temp = new FinstatDetailData();
                    temp.GroupDescription = "Economic & Market Data";
                    temp.Description = "Exchange Rate (Average)";
                    temp.PeriodYear = Convert.ToInt32(item);
                    temp.AmountType = "A";
                    temp.PeriodType = "A";
                    temp.BoldFont = "N";
                    temp.IsPercentage = "N";
                    temp.RootSource = _dataSource;
                    temp.RootSourceDate = DateTime.Now;
                    temp.Amount = Convert.ToDecimal(economicData.Where(a => a.PERIOD_YEAR == item).Select(a => a.AVG12MonthRATE).FirstOrDefault());
                    result.Add(temp);
                }

                //Preparing Inflation and ST Interest Rate
                foreach (FinstatEconomicMarketData item in economicData)
                {
                    
                    if (item.FIELD != null)
                    {
                        FinstatDetailData tempData = new FinstatDetailData();
                        tempData.GroupDescription = "Economic & Market Data";
                        tempData.Description = Convert.ToString(item.FIELD)
                            .Replace("INFLATION_PCT", "Inflation %")
                            .Replace("ST_INTEREST_RATE", "ST Interest Rate");
                        tempData.PeriodYear = Convert.ToInt32(item.YEAR1);
                        tempData.AmountType = "A";
                        tempData.PeriodType = "A";
                        tempData.BoldFont = "N";
                        tempData.IsPercentage = "Y";
                        tempData.RootSource = _dataSource;
                        tempData.RootSourceDate = DateTime.Now;
                        if (!String.IsNullOrEmpty(item.FIELD) && (item.FIELD.Contains("INFLATION_PCT") || item.FIELD.Contains("ST_INTEREST_RATE")))
                            tempData.Decimals = 1;
                        tempData.Amount = Math.Round((Convert.ToDecimal(item.VALUE) * 100), 1);
                        result.Add(tempData); 
                    }
                }
                #endregion

                #region Relative Analysis Data
        /*        List<FinstatRelativeAnalysisData> relativeData = entity.GetFinstatRelativeAnalysisData(issuerId, securityId, _dataSource, _fiscalType).ToList();
                List<FinstatDetailData> relativeResultSet = new List<FinstatDetailData>();                

                #region direct data
                //inserting dummy data


                foreach (FinstatRelativeAnalysisData item in relativeData)
                {
                    FinstatDetailData tempData = new FinstatDetailData();
                    tempData.GroupDescription = "Relative Analysis (in USD)";
                    tempData.PeriodYear = item.PERIOD_YEAR;
                    tempData.Amount = Convert.ToDecimal(item.AMOUNT * item.MULTIPLIER);
                    tempData.AmountType = "A";
                    tempData.PeriodType = "A";
                    tempData.RootSource = _dataSource;
                    tempData.RootSourceDate = DateTime.Now;
                    tempData.Decimals = item.DECIMALS;
                    tempData.IsPercentage = item.PERCENTAGE;
                    if (item.VALUE == "step1")
                    {
                        tempData.BoldFont = "Y";
                        tempData.Description = item.DATA_ID == 44 ? "Net Income" :
                                                   item.DATA_ID == 166 ? "P/E" : item.DATA_ID == 164 ? "P/BV" : item.DATA_ID == 133 ? "ROE" : "";
                        tempData.SortOrder = item.DATA_ID == 44 ? 5000 :
                                                   item.DATA_ID == 166 ? 5002 : item.DATA_ID == 164 ? 5008 : item.DATA_ID == 133 ? 5014 : 6000;
                    }
                    else if (item.VALUE == "step2")
                    {
                        tempData.BoldFont = "N";
                        tempData.Description = item.DATA_ID == 11 ? "Consensus Net Income" :
                                             item.DATA_ID == 166 ? "Consensus P/E" : item.DATA_ID == 164 
                                             ? "Consensus P/BV" : item.DATA_ID == 19 ? "Consensus ROE" : "";
                        tempData.SortOrder = item.DATA_ID == 11 ? 5001 :
                                             item.DATA_ID == 166 ? 5003 : item.DATA_ID == 164 ? 5009 : item.DATA_ID == 19 ? 5015 : 6000;
                    }
                    else if (item.VALUE == "step3")
                    {
                        tempData.BoldFont = "N";
                        tempData.Description = item.DATA_ID == 166 ? "Country P/E" :
                                                   item.DATA_ID == 164 ? "Country P/BV" : item.DATA_ID == 133 ? "Country ROE" : "";
                        tempData.SortOrder = item.DATA_ID == 166 ? 5004 :
                                                   item.DATA_ID == 164 ? 5010 : item.DATA_ID == 133 ? 5016 : 6000;
                    }
                    else if (item.VALUE == "step5")
                    {
                        tempData.BoldFont = "N";
                        tempData.Description = item.DATA_ID == 166 ? "Industry P/E" :
                                                   item.DATA_ID == 164 ? "Industry P/BV" : item.DATA_ID == 133 ? "Industry ROE" : "";
                        tempData.SortOrder = item.DATA_ID == 166 ? 5006 :
                                                   item.DATA_ID == 164 ? 5012 : item.DATA_ID == 133 ? 5018 : 6000;
                    }

                    relativeResultSet.Add(tempData);
                }
                #endregion

                //remaining data
                List<FinstatRelativeAnalysisData> step1Data = new List<FinstatRelativeAnalysisData>();
                step1Data = relativeData.Where(a => a.VALUE == "step1" && a.DATA_ID != 44).ToList();

                foreach (FinstatRelativeAnalysisData item in step1Data)
                {
                    FinstatDetailData record = new FinstatDetailData();
                    record.GroupDescription = "Relative Analysis (in USD)";
                    record.AmountType = "A";
                    record.PeriodType = "A";
                    record.RootSource = _dataSource;
                    record.RootSourceDate = DateTime.Now;
                    record.BoldFont = "N";
                    record.IsPercentage = "N";
                    record.PeriodYear = item.PERIOD_YEAR;
                    switch (item.DATA_ID)
                    {
                        case 166:
                            record.Description = "Relative Country P/E";
                            decimal countryPE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step3" && a.DATA_ID == 166
                                && a.PERIOD_YEAR == item.PERIOD_YEAR).Select(a => a.AMOUNT).FirstOrDefault());
                            if (countryPE != 0)
                            {
                                record.SortOrder = 5005;
                                record.Decimals = 1;
                                record.Amount = Math.Round((item.AMOUNT / countryPE), 2);
                                relativeResultSet.Add(record);
                            }
                            
                            break;
                        case 164:
                            record.Description = "Relative Country P/BV";
                            decimal countryPBV = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step3" && a.DATA_ID == 164
                                && a.PERIOD_YEAR == item.PERIOD_YEAR).Select(a => a.AMOUNT).FirstOrDefault());
                            if (countryPBV != 0)
                            {
                                record.SortOrder = 5011;
                                record.Decimals = 1;
                                record.Amount = Math.Round((item.AMOUNT / countryPBV), 2);
                                relativeResultSet.Add(record);
                            }
                            
                            break;
                        case 133:
                            record.Description = "Relative Country ROE";
                            decimal countryROE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step3" && a.DATA_ID == 133
                                && a.PERIOD_YEAR == item.PERIOD_YEAR).Select(a => a.AMOUNT).FirstOrDefault());
                            if (countryROE != 0)
                            {
                                record.SortOrder = 5017;
                                record.Decimals = 1;
                                record.Amount = Math.Round((item.AMOUNT / countryROE), 2);
                                relativeResultSet.Add(record);
                            }                            
                            break;
                        default:
                            break;
                    }

                }

                
                foreach (FinstatRelativeAnalysisData item in step1Data)
                {
                    FinstatDetailData record = new FinstatDetailData();
                    record.GroupDescription = "Relative Analysis (in USD)";
                    record.PeriodYear = item.PERIOD_YEAR;
                    record.AmountType = "A";
                    record.PeriodType = "A";
                    record.BoldFont = "N";
                    record.IsPercentage = "N";
                    record.RootSource = _dataSource;
                    record.RootSourceDate = DateTime.Now;
                    switch (item.DATA_ID)
                    {
                        case 166:
                            record.Description = "Relative Industry P/E";
                            decimal industryPE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step5" && a.DATA_ID == 166
                                && a.PERIOD_YEAR == item.PERIOD_YEAR).Select(a => a.AMOUNT).FirstOrDefault());
                            if (industryPE != 0)
                            {
                                record.SortOrder = 5007;
                                record.Decimals = 1;
                                record.Amount = Math.Round((item.AMOUNT / industryPE), 2);
                                relativeResultSet.Add(record);
                            }
                            break;
                        case 164:
                            record.Description = "Relative Industry P/BV";
                            decimal industryPBV = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step5" && a.DATA_ID == 164
                                && a.PERIOD_YEAR == item.PERIOD_YEAR).Select(a => a.AMOUNT).FirstOrDefault());
                            if (industryPBV != 0)
                            {
                                record.SortOrder = 5013;
                                record.Decimals = 1;
                                record.Amount = Math.Round((item.AMOUNT / industryPBV), 2);
                                relativeResultSet.Add(record);
                            }
                            break;
                        case 133:
                            record.Description = "Relative Industry ROE";
                            decimal industryROE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step5" && a.DATA_ID == 133
                                && a.PERIOD_YEAR == item.PERIOD_YEAR).Select(a => a.AMOUNT).FirstOrDefault());
                            if (industryROE != 0)
                            {
                                record.SortOrder = 5019;
                                record.Decimals = 1;
                                record.Amount = Math.Round((item.AMOUNT / industryROE), 2); 
                                relativeResultSet.Add(record);
                            }
                            break;
                        default:
                            break;
                    }
                }
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Net Income", _dataSource, 5000, "Y"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Consensus Net Income", _dataSource, 5001, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("P/E", _dataSource, 5002, "Y"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Consensus P/E", _dataSource, 5003, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Country P/E", _dataSource, 5004, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Relative Country P/E", _dataSource, 5005, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Industry P/E", _dataSource, 5006, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Relative Industry P/E", _dataSource, 5007, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("P/BV", _dataSource, 5008, "Y"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Consensus P/BV", _dataSource, 5009, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Country P/BV", _dataSource, 5010, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Relative Country P/BV", _dataSource, 5011, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Industry P/BV", _dataSource, 5012, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Relative Industry P/BV", _dataSource, 5013, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("ROE", _dataSource, 5014, "Y"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Consensus ROE", _dataSource, 5015, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Country ROE", _dataSource, 5016, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Relative Country ROE", _dataSource, 5017, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Industry ROE", _dataSource, 5018, "N"));
                relativeResultSet.Add(GetFinstatDetailRelativeSampleData("Relative Industry ROE", _dataSource, 5019, "N"));
                relativeResultSet = relativeResultSet.OrderBy(g => g.SortOrder).ToList();
                result.AddRange(relativeResultSet);*/
                #endregion                

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }



        private FinstatDetailData GetFinstatDetailRelativeSampleData(String description, String dataSource, int sortOrder
            , String boldFont)
        {
            FinstatDetailData record = new FinstatDetailData();
            record.GroupDescription = "Relative Analysis (in USD)";
            record.Description = description;
            record.AmountType = "A";
            record.PeriodType = "A";
            record.RootSource = dataSource;
            record.RootSourceDate = DateTime.Now;
            record.BoldFont = boldFont;
            record.IsPercentage = "N";
            record.Decimals = 2;
            record.IsPercentage = "N";
            record.PeriodYear = 2300;
            record.SortOrder = sortOrder;
            return record;
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
                {
                    return null;
                }

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }

                //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (data == null)
                {
                    return null;
                }

                BasicData basicData = new BasicData();
                basicData.WeekRange52Low = data.FIFTYTWO_WEEK_LOW;
                basicData.WeekRange52High = data.FIFTYTWO_WEEK_HIGH;
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
                    
                    if (decimal.TryParse(data.BETA, out convertedString))
                    {
                        basicData.Beta = convertedString;
                    }
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
                resultDB = extResearch.ExecuteStoreQuery<GetBasicData_Result>("exec GetBasicData @SecurityID={0}", 
                    Convert.ToString(data.SECURITY_ID)).ToList();
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
            {
                return new List<TargetPriceCEData>();
            }
            DimensionEntitiesService.Entities dimensionEntity = DimensionEntity;
            List<GF_SECURITY_BASEVIEW> securityData = (dimensionEntity.GF_SECURITY_BASEVIEW.
                Where(a => a.ISSUE_NAME.ToUpper().Trim() == entitySelectionData.LongName.ToUpper().Trim()).ToList());
            if (securityData == null)
                return result;
            string XRef = securityData.Select(a => a.XREF).FirstOrDefault();
            if (XRef == null)
            {
                return result;
            }
            List<GetTargetPrice_Result> dbResult = new List<GetTargetPrice_Result>();
            ExternalResearchEntities entity = new ExternalResearchEntities();
            dbResult = entity.GetTargetPrice(XRef).ToList();
            if (dbResult == null)
            {
                return result;
            }
            if (dbResult.Count == 0)
            {
                return result;
            }
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
                data.CurrentPriceDate = Convert.ToDateTime(item.CurrentPriceDate);
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
            List<ConsensusEstimateMedianData> data = new List<ConsensusEstimateMedianData>();
            try
            {
                if (issuerId == null)
                {
                    return new List<ConsensusEstimateMedian>();
                }
                if (currency == null)
                {
                    return new List<ConsensusEstimateMedian>();
                }
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);
                decimal previousYearQuarterAmount;
                ExternalResearchEntities entity = new ExternalResearchEntities();
                data = entity.GetConsensusEstimateData(issuerId, "REUTERS", _periodType, "FISCAL", currency).ToList();
                List<int> dataDesc = new List<int>() { 17, 7, 11, 8, 18, 19 };

                if (data == null)
                {
                    return new List<ConsensusEstimateMedian>();
                }
                if (data.Count == 0)
                {
                    return new List<ConsensusEstimateMedian>();
                }
                data = data.OrderBy(record => record.ESTIMATE_DESC).ThenByDescending(record => record.PERIOD_YEAR).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    if (dataDesc.Contains(data[i].ESTIMATE_ID))
                    {
                        ConsensusEstimateMedian temp = new ConsensusEstimateMedian();
                        if (data[i].ESTIMATE_ID == 17)
                        {
                            temp.SortOrder = 1;
                        }
                        else if (data[i].ESTIMATE_ID == 7)
                        {
                            temp.SortOrder = 2;
                        }
                        else if (data[i].ESTIMATE_ID == 11)
                        {
                            temp.SortOrder = 3;
                        }
                        else if (data[i].ESTIMATE_ID == 8)
                        {
                            temp.SortOrder = 4;
                        }
                        else if (data[i].ESTIMATE_ID == 18)
                        {
                            temp.SortOrder = 5;
                        }
                        else if (data[i].ESTIMATE_ID == 19)
                        {
                            temp.SortOrder = 6;
                        }
                        temp.IssuerId = data[i].ISSUER_ID;
                        temp.EstimateId = data[i].ESTIMATE_ID;
                        temp.Description = data[i].ESTIMATE_DESC;
                        temp.Period = data[i].Period;
                        temp.AmountType = data[i].AMOUNT_TYPE;
                        temp.PeriodYear = data[i].PERIOD_YEAR;
                        temp.PeriodType = data[i].PERIOD_TYPE;
                        temp.Amount = data[i].AMOUNT;
                        temp.AshmoreEmmAmount = data[i].ASHMOREEMM_AMOUNT;
                        temp.NumberOfEstimates = data[i].NUMBER_OF_ESTIMATES;
                        temp.High = data[i].HIGH;
                        temp.Low = data[i].LOW;
                        temp.StandardDeviation = data[i].STANDARD_DEVIATION;
                        temp.SourceCurrency = data[i].SOURCE_CURRENCY;
                        temp.DataSource = data[i].DATA_SOURCE;
                        temp.DataSourceDate = data[i].DATA_SOURCE_DATE;

                        if (data[i].ESTIMATE_ID == 18 || data[i].ESTIMATE_ID == 19)
                        {
                            temp.Actual = Convert.ToString(Math.Round(Convert.ToDecimal(data[i].ACTUAL), 2)) + "%";
                        }
                        else
                        {
                            temp.Actual = (Math.Round(Convert.ToDecimal(data[i].ACTUAL), 2)).ToString("N");
                        }
                        temp.YOYGrowth = data[i].AMOUNT;
                        temp.Variance = data[i].AMOUNT == 0 ? null : ((data[i].ASHMOREEMM_AMOUNT / data[i].AMOUNT) - 1) * 100;
                        previousYearQuarterAmount = data.Where(a => a.ESTIMATE_DESC == data[i].ESTIMATE_DESC && a.PERIOD_YEAR == (data[i].PERIOD_YEAR - 1)
                            && a.PERIOD_TYPE == data[i].PERIOD_TYPE).Select(a => a.AMOUNT).FirstOrDefault();
                        if (previousYearQuarterAmount == null || previousYearQuarterAmount == 0)
                        {
                            temp.YOYGrowth = 0;
                        }
                        else
                        {
                            temp.YOYGrowth = (temp.YOYGrowth / previousYearQuarterAmount - 1) * 100;
                        }
                        result.Add(temp);
                    }
                }
                return result.OrderBy(a => a.SortOrder).ToList();
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
        public List<ConsensusEstimatesValuations> RetrieveConsensusEstimatesValuationData(string issuerId, string longName, FinancialStatementPeriodType periodType, string currency)
        {
            List<ConsensusEstimatesValuations> result = new List<ConsensusEstimatesValuations>();
            List<ConsensusEstimateValuation> data = new List<ConsensusEstimateValuation>();
            try
            {
                if (issuerId == null)
                {
                    return new List<ConsensusEstimatesValuations>();
                }
                if (longName == null)
                {
                    return new List<ConsensusEstimatesValuations>();
                }
                if (currency == null)
                {
                    return new List<ConsensusEstimatesValuations>();
                }
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);
                decimal? previousYearQuarterAmount;
                ExternalResearchEntities entity = new ExternalResearchEntities();
                GF_SECURITY_BASEVIEW securityData = DimensionEntity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUE_NAME == longName).FirstOrDefault();
                string securityId;
                if (securityData == null)
                {
                    securityId = null;
                }
                else
                {
                    securityId = Convert.ToString(securityData.SECURITY_ID);
                }
                //data = entity.GetConsensusEstimatesValuation(issuerId, "REUTERS", _periodType, "FISCAL", currency, null, null, securityId).ToList();
                data = entity.GetConsensusEstimatesValuation(issuerId, "REUTERS", "CONSENSUS", _periodType, "FISCAL", currency, null, null, securityId).ToList();
                List<int> dataDesc = new List<int>() { 166, 170, 171, 164, 192, 172 };

                if (data == null)
                {
                    return new List<ConsensusEstimatesValuations>();
                }
                if (data.Count == 0)
                {
                    return new List<ConsensusEstimatesValuations>();
                }
                data = data.OrderBy(record => record.ESTIMATE_DESC).ThenByDescending(record => record.PERIOD_YEAR).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    if (dataDesc.Contains(data[i].ESTIMATE_ID))
                    {
                        ConsensusEstimatesValuations temp = new ConsensusEstimatesValuations();
                        if (data[i].ESTIMATE_ID == 170)
                        {
                            temp.SortOrder = 1;
                        }
                        else if (data[i].ESTIMATE_ID == 171)
                        {
                            temp.SortOrder = 2;
                        }
                        else if (data[i].ESTIMATE_ID == 166)
                        {
                            temp.SortOrder = 3;
                        }
                        else if (data[i].ESTIMATE_ID == 172)
                        {
                            temp.SortOrder = 4;
                        }
                        else if (data[i].ESTIMATE_ID == 164)
                        {
                            temp.SortOrder = 5;
                        }
                        else if (data[i].ESTIMATE_ID == 192)
                        {
                            temp.SortOrder = 6;
                        }
                        temp.IssuerId = data[i].ISSUER_ID;
                        temp.EstimateId = data[i].ESTIMATE_ID;
                        temp.Description = data[i].ESTIMATE_DESC;
                        temp.Period = data[i].Period;
                        temp.AmountType = data[i].AMOUNT_TYPE;
                        temp.PeriodYear = data[i].PERIOD_YEAR;
                        temp.PeriodType = data[i].PERIOD_TYPE;
                        temp.Amount = data[i].AMOUNT;
                        temp.AshmoreEmmAmount = data[i].ASHMOREEMM_AMOUNT;
                        temp.NumberOfEstimates = data[i].NUMBER_OF_ESTIMATES;
                        temp.High = data[i].HIGH;
                        temp.Low = data[i].LOW;
                        temp.StandardDeviation = data[i].STANDARD_DEVIATION;
                        temp.SourceCurrency = data[i].SOURCE_CURRENCY;
                        temp.DataSource = data[i].DATA_SOURCE;
                        temp.DataSourceDate = data[i].DATA_SOURCE_DATE;
                        temp.Actual = data[i].ACTUAL;
                        temp.YOYGrowth = data[i].AMOUNT;
                        temp.Variance = data[i].AMOUNT == 0 ? null : ((data[i].ASHMOREEMM_AMOUNT / data[i].AMOUNT) - 1) * 100;
                        previousYearQuarterAmount = data.Where(a => a.ESTIMATE_DESC == data[i].ESTIMATE_DESC && a.PERIOD_YEAR == (data[i].PERIOD_YEAR - 1)
                                && a.PERIOD_TYPE == data[i].PERIOD_TYPE).Select(a => a.AMOUNT).FirstOrDefault();
                        if (previousYearQuarterAmount == null || previousYearQuarterAmount == 0)
                        {
                            temp.YOYGrowth = 0;
                        }
                        else
                        {
                            temp.YOYGrowth = (temp.YOYGrowth / previousYearQuarterAmount - 1) * 100;
                        }
                        result.Add(temp);
                    }
                }
                return result.OrderBy(a => a.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }


        #endregion

        #region Consensus Estimates Summary Gadget
        /// <summary>
        /// Retrieve data for Consensus Estimates Summary Gadget (aka. Comparison with Consensus Gadget)
        /// </summary>
        /// <param name="entityIdentifier">Security identifier selected by the user</param>
        /// <returns>Returns data in the list of type ConsensusEstimatesSummaryData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData> RetrieveConsensusEstimatesSummaryData
            (EntitySelectionData entityIdentifier)
        {
            try
            {
                List<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData> result = new
                    List<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData>();
                ExternalResearchEntities research = new ExternalResearchEntities();
                research.CommandTimeout = 5000;
                result = research.ExecuteStoreQuery<GreenField.DataContracts.DataContracts.ConsensusEstimatesSummaryData>
                    ("exec GetConsensusEstimatesSummaryData @Security={0}", entityIdentifier.InstrumentID).ToList();
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
                research.CommandTimeout = 3000;
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
        public List<PRevenueData> RetrievePRevenueData(EntitySelectionData entitySelectionData, string chartTitle)
        {
            try
            {
                List<PRevenueData> result = new List<PRevenueData>();
                List<GetPRevenueData_Result> resultDB = new List<GetPRevenueData_Result>();
                List<GetEV_EBITDAData_Result> resultDB_EV_EBITDA = new List<GetEV_EBITDAData_Result>();
                ExternalResearchEntities extResearch = new ExternalResearchEntities() { CommandTimeout = 5000 };

                if (entitySelectionData == null)
                    return null;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW svcData = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (svcData == null)
                    return null;
                //execute store proc giving securityId as an input parameter
                int? securityId = svcData.SECURITY_ID;

                if (chartTitle == "EV/EBITDA")
                {
                    resultDB_EV_EBITDA = extResearch.ExecuteStoreQuery<GetEV_EBITDAData_Result>("exec Get_EV_EBITDA @SecurityID={0},@issuerId={1},@chartTitle={2}", Convert.ToString(svcData.SECURITY_ID), svcData.ISSUER_ID, chartTitle).ToList();

                    //Comparer to fetch only unique rows
                    if (resultDB_EV_EBITDA != null && resultDB_EV_EBITDA.Count > 1)
                    {
                        IEqualityComparer<GetEV_EBITDAData_Result> comparerEV_EBITDA = new HistoricalValEV_EBITDACompararer();
                        resultDB_EV_EBITDA = resultDB_EV_EBITDA.Distinct(comparerEV_EBITDA).ToList();
                    }

                    for (int _index = 0; _index < resultDB_EV_EBITDA.Count; _index++)
                    {
                        PRevenueData data = new PRevenueData();
                        decimal? sumNetDebt = null;
                        decimal? sumEBITDA = null;
                        data.PeriodLabel = resultDB_EV_EBITDA[_index].PeriodLabel;

                        if ((resultDB_EV_EBITDA[_index].USDPrice == null || resultDB_EV_EBITDA[_index].USDPrice == 0) || (resultDB_EV_EBITDA[_index].Shares_Outstanding == null || resultDB_EV_EBITDA[_index].Shares_Outstanding == 0))
                        {
                            data.PRevenueVal = null;
                        }
                        else
                        {
                            //Sum of Amount if 4 quarters exist
                            if (_index + 1 < resultDB_EV_EBITDA.Count && _index + 2 < resultDB_EV_EBITDA.Count && _index + 3 < resultDB_EV_EBITDA.Count)
                            {
                                if ((resultDB_EV_EBITDA[_index].NetDebt != null && resultDB_EV_EBITDA[_index + 1].NetDebt != null && resultDB_EV_EBITDA[_index + 2].NetDebt != null && resultDB_EV_EBITDA[_index + 3].NetDebt != null)
                                    || (resultDB_EV_EBITDA[_index].EBITDA != null && resultDB_EV_EBITDA[_index + 1].EBITDA != null && resultDB_EV_EBITDA[_index + 2].EBITDA != null && resultDB_EV_EBITDA[_index + 3].EBITDA != null))
                                {
                                    sumNetDebt = resultDB_EV_EBITDA[_index].NetDebt + resultDB_EV_EBITDA[_index + 1].NetDebt + resultDB_EV_EBITDA[_index + 2].NetDebt + resultDB_EV_EBITDA[_index + 3].NetDebt;
                                    sumEBITDA = resultDB_EV_EBITDA[_index].EBITDA + resultDB_EV_EBITDA[_index + 1].EBITDA + resultDB_EV_EBITDA[_index + 2].EBITDA + resultDB_EV_EBITDA[_index + 3].EBITDA;
                                }
                                else
                                {
                                    sumNetDebt = null;
                                    sumEBITDA = null;
                                }
                                if ((sumNetDebt == null || sumNetDebt == 0) || (sumEBITDA == null || sumEBITDA == 0))
                                {
                                    data.PRevenueVal = null;
                                }
                                else
                                {
                                    decimal? EV = null;
                                    decimal? EBITDA = null;
                                    EV = (resultDB_EV_EBITDA[_index].USDPrice * resultDB_EV_EBITDA[_index].Shares_Outstanding) / sumNetDebt;
                                    EBITDA = sumEBITDA;
                                    data.PRevenueVal = EV / EBITDA;
                                }
                            }
                        }//end - if
                        result.Add(data);
                    }//end for loop
                }
                else
                {
                    resultDB = extResearch.ExecuteStoreQuery<GetPRevenueData_Result>("exec Get_PRevenue @SecurityID={0},@issuerId={1},@chartTitle={2}", Convert.ToString(svcData.SECURITY_ID), svcData.ISSUER_ID, chartTitle).ToList();//, Convert.ToString(data.SECURITY_ID)).ToList();
                    //Comparer to fetch only unique rows
                    if (resultDB != null && resultDB.Count > 1)
                    {
                        IEqualityComparer<GetPRevenueData_Result> comparer = new HistoricalValuationCompararer();
                        resultDB = resultDB.Distinct(comparer).ToList();
                    }
                    for (int _index = 0; _index < resultDB.Count; _index++)
                    {
                        PRevenueData data = new PRevenueData();
                        decimal? sumAmount = null;

                        data.PeriodLabel = resultDB[_index].PeriodLabel;

                        if ((resultDB[_index].USDPrice == null || resultDB[_index].USDPrice == 0) || (resultDB[_index].Shares_Outstanding == null || resultDB[_index].Shares_Outstanding == 0))
                        {
                            data.PRevenueVal = null;
                        }
                        else
                        {
                            //Sum of Amount if 4 quarters exist
                            if (_index + 1 < resultDB.Count && _index + 2 < resultDB.Count && _index + 3 < resultDB.Count)
                            {
                                if (resultDB[_index].Amount != null && resultDB[_index + 1].Amount != null && resultDB[_index + 2].Amount != null && resultDB[_index + 3].Amount != null)
                                    sumAmount = resultDB[_index].Amount + resultDB[_index + 1].Amount + resultDB[_index + 2].Amount + resultDB[_index + 3].Amount;
                            }
                            else
                            {
                                sumAmount = null;
                            }

                            if (sumAmount == null || sumAmount == 0)
                                data.PRevenueVal = null;
                            else
                            {
                                if (chartTitle == "FCF Yield" || chartTitle == "Dividend Yield")
                                    data.PRevenueVal = sumAmount / (resultDB[_index].USDPrice * resultDB[_index].Shares_Outstanding);
                                else
                                    data.PRevenueVal = (resultDB[_index].USDPrice * resultDB[_index].Shares_Outstanding) / sumAmount;
                            }

                        }
                        result.Add(data);
                    }//end for loop

                }//end - if

                result = HistoricalValuationCalculations.CalculateAvg(result);
                result = HistoricalValuationCalculations.CalculateStdDev(result);

                for (int _index = result.Count; _index > 0; _index--)
                {
                    if (result[_index - 1].PRevenueVal == null)
                    {
                        result[_index - 1].Average = null;
                        result[_index - 1].StdDevMinus = null;
                        result[_index - 1].StdDevPlus = null;
                    }
                    else
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
                List<RatioComparisonData> result = entity.RetrieveRatioComparisonData(contextSecurityXML).ToList();
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

        #region Gadget With Period Columns
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<GreenField.DataContracts.COASpecificData> RetrieveCOASpecificData(String issuerId, int? securityId, FinancialStatementDataSource cSource, FinancialStatementFiscalType cFiscalType, String cCurrency)
        {
            try
            {
                string _dataSource = EnumUtils.ToString(cSource);
                string _fiscalType = EnumUtils.ToString(cFiscalType);
                List<GreenField.DAL.COASpecificData> result = new List<GreenField.DAL.COASpecificData>();
                List<GreenField.DataContracts.COASpecificData> mainResult = new List<GreenField.DataContracts.COASpecificData>();
                ExternalResearchEntities research = new ExternalResearchEntities();
                research.CommandTimeout = 5000;
                result = research.GetDataForPeriodGadgets(_dataSource, _fiscalType, cCurrency, issuerId, securityId.ToString()).ToList();  //Retrieve data for Summary of Financials and Valuations gadget.
                foreach (GreenField.DAL.COASpecificData item in result)
                {
                    GreenField.DataContracts.COASpecificData entry = new GreenField.DataContracts.COASpecificData();
                    entry.Amount = item.Amount * item.Multiplier;  //add in Multiplier logic
                    entry.AmountType = item.AmountType;
                    entry.DataSource = item.DataSource;
                    entry.Decimals = item.Decimals;
                    entry.Description = item.Description;
                    entry.GroupDescription = item.GridDesc;
                    entry.GridId = item.GridId;
                    entry.IsPercentage = item.IsPercentage;
                    entry.PeriodType = item.Period_Type;
                    entry.PeriodYear = item.PeriodYear;
                    entry.RootSource = item.RootSource;
                    entry.ShowGrid = item.ShowGrid;
                    entry.SortOrder = item.SortOrder;
                    entry.Multiplier = item.Multiplier;
                    mainResult.Add(entry);
                }
                return mainResult;

            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion

        #region Valuation,Quality and Growth
        /// <summary>
        /// Retrieves Valuation quality growth data from the database
        /// </summary>
        /// <param name="selectedPortfolio">Portfolio selected</param>
        /// <param name="effectiveDate">Effective Date selected</param>
        /// <param name="filterType">Filter Type selected</param>
        /// <param name="filterValue">Filter value selected</param>
        /// <param name="lookThruEnabled">Look through check box</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData> RetrieveValuationGrowthData
            (PortfolioSelectionData selectedPortfolio, DateTime? effectiveDate, String filterType, String filterValue, bool lookThruEnabled)
        {
            try
            {
                if (selectedPortfolio == null || effectiveDate == null || filterType == null || filterValue == null)
                {
                    return new List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData>();
                }
                StringBuilder issuerIDPortfolio = new StringBuilder();
                StringBuilder securityIDPortfolio = new StringBuilder();
                StringBuilder issuerIDBenchmark = new StringBuilder();
                StringBuilder securityIDBenchmark = new StringBuilder();
                List<String> distinctSecuritiesForPortfolio = new List<String>();
                List<String> distinctSecuritiesForBenchmark = new List<String>();
                Dictionary<String, String> listForPortfolio = new Dictionary<string, string>();
                Dictionary<String, String> listForBenchmark = new Dictionary<string, string>();
                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> dataPortfolio = new List<GF_PORTFOLIO_HOLDINGS>();
                List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> dataBenchmark = new List<GF_BENCHMARK_HOLDINGS>();
                List<DimensionEntitiesService.GF_PORTFOLIO_LTHOLDINGS> dataPortfolioLookThruDis = new List<GF_PORTFOLIO_LTHOLDINGS>();
                //lists for filtering all data according to Data Id's
                List<CalculatedValuesForValuation> valuesPortForAllDataIds = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForAllDataIds = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForPRevenue = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForPRevenue = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForEVEBITDA = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForEVEBITDA = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForPE = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForPE = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForPCE = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForPCE = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForPBV = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForPBV = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForROE = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForROE = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForDividendYield = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForDividendYield = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForRevGrowth = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForRevGrowth = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesPortForNetGrowth = new List<CalculatedValuesForValuation>();
                List<CalculatedValuesForValuation> valuesBenchForNetGrowth = new List<CalculatedValuesForValuation>();
                int check = 1;
                int checkBen = 1;
                String benchmarkId = null;
                DimensionEntitiesService.Entities entity = DimensionEntity;
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData> result = new
                    List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData>();
                List<GreenField.DAL.ValuationQualityGrowthData> storedProcResult = new List<GreenField.DAL.ValuationQualityGrowthData>();

                if (lookThruEnabled == false)
                {
                    #region Look Thru Disabled
                    switch (filterType)
                    {
                        case "Region":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE ==
                                    effectiveDate.Value.Date && t.ASHEMM_PROP_REGION_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.ASHEMM_PROP_REGION_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Country":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE ==
                                    effectiveDate.Value.Date && t.ISO_COUNTRY_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.ISO_COUNTRY_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Industry":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE ==
                                    effectiveDate.Value.Date && t.GICS_INDUSTRY_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.GICS_INDUSTRY_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Sector":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                    t.GICS_SECTOR_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.GICS_SECTOR_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Show Everything":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                    t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    if (dataPortfolio != null && dataPortfolio.Count() > 0)
                    {
                        distinctSecuritiesForPortfolio = dataPortfolio.Select(record => record.ISSUE_NAME).Distinct().ToList();
                    }
                    if (dataBenchmark != null && dataBenchmark.Count() > 0)
                    {
                        distinctSecuritiesForBenchmark = dataBenchmark.Select(record => record.ISSUE_NAME).Distinct().ToList();
                    }
                    foreach (String issueName in distinctSecuritiesForPortfolio)
                    {
                        GF_SECURITY_BASEVIEW securityDetails = entity.GF_SECURITY_BASEVIEW
                         .Where(record => record.ISSUE_NAME == issueName).FirstOrDefault();
                        if (securityDetails != null)
                        {
                            check = 0;
                            issuerIDPortfolio.Append(",'" + securityDetails.ISSUER_ID + "'");
                            securityIDPortfolio.Append(",'" + securityDetails.SECURITY_ID + "'");
                            listForPortfolio.Add(securityDetails.SECURITY_ID.ToString(), securityDetails.ISSUE_NAME);
                        }
                    }
                    issuerIDPortfolio = check == 0 ? issuerIDPortfolio.Remove(0, 1) : null;
                    securityIDPortfolio = check == 0 ? securityIDPortfolio.Remove(0, 1) : null;

                    foreach (String issueName in distinctSecuritiesForBenchmark)
                    {
                        GF_SECURITY_BASEVIEW securityDetails = entity.GF_SECURITY_BASEVIEW
                         .Where(record => record.ISSUE_NAME == issueName).FirstOrDefault();
                        if (securityDetails != null)
                        {
                            checkBen = 0;
                            issuerIDBenchmark.Append(",'" + securityDetails.ISSUER_ID + "'");
                            securityIDBenchmark.Append(",'" + securityDetails.SECURITY_ID + "'");
                            listForBenchmark.Add(securityDetails.SECURITY_ID.ToString(), securityDetails.ISSUE_NAME);
                        }
                    }
                    issuerIDBenchmark = checkBen == 0 ? issuerIDBenchmark.Remove(0, 1) : null;
                    securityIDBenchmark = checkBen == 0 ? securityIDBenchmark.Remove(0, 1) : null;

                    string _issuerIDPortfolio = issuerIDPortfolio == null ? null : issuerIDPortfolio.ToString();
                    string _issuerIDBenchmark = issuerIDBenchmark == null ? null : issuerIDBenchmark.ToString();
                    string _securityIDBenchmark = securityIDBenchmark == null ? null : securityIDBenchmark.ToString();
                    string _securityIDPortfolio = securityIDPortfolio == null ? null : securityIDPortfolio.ToString();

                    ExternalResearchEntities research = new ExternalResearchEntities();
                    research.CommandTimeout = 5000;
                    storedProcResult = research.usp_GetDataForValuationQualityGrowth(_issuerIDPortfolio, _securityIDPortfolio,
                        _issuerIDBenchmark, _securityIDBenchmark).ToList();

                    #region Calculating values for all Data Id's for Portfolio View and Benchmark View
                    //Combining Portfolio weight and Benchmark weight for all securities from Portfolio View,Benchmark View and 
                    //Amount from Data Base
                    if (storedProcResult != null && storedProcResult.Count() > 0)
                    {
                        foreach (int dataId in storedProcResult.Select(t => t.DataId).Distinct())
                        {
                            List<String> dinstinctIssuerIdsForPortfolio = new List<String>();
                            List<String> dinstinctSecurityIdsForPortfolio = new List<String>();
                            List<String> dinstinctIssuerIdsForBenchmark = new List<String>();
                            List<String> dinstinctSecurityIdsForBenchmark = new List<String>();
                            List<String> distinctSecurityNamesForPortfolio = new List<String>();
                            List<String> distinctSecurityNamesForBenchmark = new List<String>();
                            dinstinctIssuerIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio")
                                .Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio")
                                .Select(t => t.SecurityId).Distinct().ToList();
                            dinstinctIssuerIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark")
                                .Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark")
                                .Select(t => t.SecurityId).Distinct().ToList();
                            foreach (String secId in dinstinctSecurityIdsForPortfolio)
                            {
                                if (listForPortfolio.ContainsKey(secId))
                                {
                                    distinctSecurityNamesForPortfolio.Add(listForPortfolio[secId]);
                                }
                            }
                            foreach (String secId in dinstinctSecurityIdsForBenchmark)
                            {
                                if (listForBenchmark.ContainsKey(secId))
                                {
                                    distinctSecurityNamesForBenchmark.Add(listForBenchmark[secId]);
                                }
                            }
                            foreach (String s in dinstinctIssuerIdsForPortfolio)
                            {
                                foreach (GF_PORTFOLIO_HOLDINGS row in dataPortfolio.Where(t => t.ISSUER_ID == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.IssuerId = s;
                                    objPort.SecurityId = listForPortfolio.FirstOrDefault(t => t.Value == row.ISSUE_NAME).Key;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId).Select(t => t.Amount)
                                        .FirstOrDefault();
                                    objPort.PortfolioPercent = row.DIRTY_VALUE_PC;
                                    valuesPortForAllDataIds.Add(objPort);
                                }
                            }
                            foreach (String s in distinctSecurityNamesForPortfolio)
                            {
                                foreach (GF_PORTFOLIO_HOLDINGS row in dataPortfolio.Where(t => t.ISSUE_NAME == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.SecurityId = listForPortfolio.FirstOrDefault(t => t.Value == s).Key;
                                    objPort.IssuerId = row.ISSUER_ID;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForPortfolio
                                        .FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
                                    objPort.PortfolioPercent = row.DIRTY_VALUE_PC;
                                    valuesPortForAllDataIds.Add(objPort);
                                }
                            }
                            foreach (String s in dinstinctIssuerIdsForBenchmark)
                            {
                                foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmark.Where(t => t.ISSUER_ID == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.IssuerId = s;
                                    objPort.SecurityId = listForBenchmark.FirstOrDefault(t => t.Value == row.ISSUE_NAME).Key;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId)
                                        .Select(t => t.Amount).FirstOrDefault();
                                    objPort.PortfolioPercent = row.BENCHMARK_WEIGHT;
                                    valuesBenchForAllDataIds.Add(objPort);
                                }
                            }
                            foreach (String s in distinctSecurityNamesForBenchmark)
                            {
                                foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmark.Where(t => t.ISSUE_NAME == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.SecurityId = listForBenchmark.FirstOrDefault(t => t.Value == s).Key;
                                    objPort.IssuerId = row.ISSUER_ID;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForBenchmark
                                        .FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
                                    objPort.PortfolioPercent = row.BENCHMARK_WEIGHT;
                                    valuesBenchForAllDataIds.Add(objPort);
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    #region Look Thru Enabled
                    switch (filterType)
                    {
                        case "Region":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE ==
                                    effectiveDate.Value.Date && t.ASHEMM_PROP_REGION_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.ASHEMM_PROP_REGION_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Country":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE ==
                                    effectiveDate.Value.Date && t.ISO_COUNTRY_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.ISO_COUNTRY_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Industry":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE ==
                                    effectiveDate.Value.Date && t.GICS_INDUSTRY_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.GICS_INDUSTRY_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Sector":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                    t.GICS_SECTOR_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.GICS_SECTOR_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        case "Show Everything":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                    t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date &&
                                       t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count() > 0)
                    {
                        distinctSecuritiesForPortfolio = dataPortfolioLookThruDis.Select(record => record.ISSUE_NAME).Distinct().ToList();
                    }
                    if (dataBenchmark != null && dataBenchmark.Count() > 0)
                    {
                        distinctSecuritiesForBenchmark = dataBenchmark.Select(record => record.ISSUE_NAME).Distinct().ToList();
                    }
                    foreach (String issueName in distinctSecuritiesForPortfolio)
                    {
                        GF_SECURITY_BASEVIEW securityDetails = entity.GF_SECURITY_BASEVIEW
                         .Where(record => record.ISSUE_NAME == issueName).FirstOrDefault();
                        if (securityDetails != null)
                        {
                            check = 0;
                            issuerIDPortfolio.Append(",'" + securityDetails.ISSUER_ID + "'");
                            securityIDPortfolio.Append(",'" + securityDetails.SECURITY_ID + "'");
                            listForPortfolio.Add(securityDetails.SECURITY_ID.ToString(), securityDetails.ISSUE_NAME);
                        }
                    }
                    issuerIDPortfolio = check == 0 ? issuerIDPortfolio.Remove(0, 1) : null;
                    securityIDPortfolio = check == 0 ? securityIDPortfolio.Remove(0, 1) : null;
                    foreach (String issueName in distinctSecuritiesForBenchmark)
                    {
                        GF_SECURITY_BASEVIEW securityDetails = entity.GF_SECURITY_BASEVIEW
                         .Where(record => record.ISSUE_NAME == issueName).FirstOrDefault();
                        if (securityDetails != null)
                        {
                            checkBen = 0;
                            issuerIDBenchmark.Append(",'" + securityDetails.ISSUER_ID + "'");
                            securityIDBenchmark.Append(",'" + securityDetails.SECURITY_ID + "'");
                            listForBenchmark.Add(securityDetails.SECURITY_ID.ToString(), securityDetails.ISSUE_NAME);
                        }
                    }
                    issuerIDBenchmark = checkBen == 0 ? issuerIDBenchmark.Remove(0, 1) : null;
                    securityIDBenchmark = checkBen == 0 ? securityIDBenchmark.Remove(0, 1) : null;

                    string _issuerIDPortfolio = issuerIDPortfolio == null ? null : issuerIDPortfolio.ToString();
                    string _issuerIDBenchmark = issuerIDBenchmark == null ? null : issuerIDBenchmark.ToString();
                    string _securityIDBenchmark = securityIDBenchmark == null ? null : securityIDBenchmark.ToString();
                    string _securityIDPortfolio = securityIDPortfolio == null ? null : securityIDPortfolio.ToString();

                    ExternalResearchEntities research = new ExternalResearchEntities();
                    research.CommandTimeout = 5000;
                    storedProcResult = research.usp_GetDataForValuationQualityGrowth(_issuerIDPortfolio, _securityIDPortfolio,
                        _issuerIDBenchmark, _securityIDBenchmark).ToList();

                    #region Calculating values for all Data Id's for Portfolio View and Benchmark View
                    //Combining Portfolio weight and Benchmark weight for all securities from Portfolio View,
                    //Benchmark View and Amount from Data Base
                    List<String> dinstinctIssuerIdsForPortfolio = new List<String>();
                    List<String> dinstinctSecurityIdsForPortfolio = new List<String>();
                    List<String> dinstinctIssuerIdsForBenchmark = new List<String>();
                    List<String> dinstinctSecurityIdsForBenchmark = new List<String>();
                    List<String> distinctSecurityNamesForPortfolio = new List<string>();
                    List<String> distinctSecurityNamesForBenchmark = new List<string>();
                    if (storedProcResult != null && storedProcResult.Count() > 0)
                    {
                        foreach (int dataId in storedProcResult.Select(t => t.DataId).Distinct())
                        {
                            dinstinctIssuerIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio")
                                .Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio")
                                .Select(t => t.SecurityId).Distinct().ToList();
                            dinstinctIssuerIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark")
                                .Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark")
                                .Select(t => t.SecurityId).Distinct().ToList();
                            foreach (String secId in dinstinctSecurityIdsForPortfolio)
                            {
                                if (listForPortfolio.ContainsKey(secId))
                                {
                                    distinctSecurityNamesForPortfolio.Add(listForPortfolio[secId]);
                                }
                            }
                            foreach (String secId in dinstinctSecurityIdsForBenchmark)
                            {
                                if (listForBenchmark.ContainsKey(secId))
                                {
                                    distinctSecurityNamesForBenchmark.Add(listForBenchmark[secId]);
                                }
                            }
                            foreach (String s in dinstinctIssuerIdsForPortfolio)
                            {
                                foreach (GF_PORTFOLIO_LTHOLDINGS row in dataPortfolioLookThruDis.Where(t => t.ISSUER_ID == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.IssuerId = s;
                                    objPort.SecurityId = listForPortfolio.FirstOrDefault(t => t.Value == row.ISSUE_NAME).Key;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId).Select(t => t.Amount)
                                        .FirstOrDefault();
                                    objPort.PortfolioPercent = row.DIRTY_VALUE_PC;
                                    valuesPortForAllDataIds.Add(objPort);
                                }
                            }
                            foreach (String s in distinctSecurityNamesForPortfolio)
                            {
                                foreach (GF_PORTFOLIO_LTHOLDINGS row in dataPortfolioLookThruDis.Where(t => t.ISSUE_NAME == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.SecurityId = listForPortfolio.FirstOrDefault(t => t.Value == s).Key;
                                    objPort.IssuerId = row.ISSUER_ID;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForPortfolio
                                        .FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
                                    objPort.PortfolioPercent = row.DIRTY_VALUE_PC;
                                    valuesPortForAllDataIds.Add(objPort);
                                }
                            }
                            foreach (String s in dinstinctIssuerIdsForBenchmark)
                            {
                                foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmark.Where(t => t.ISSUER_ID == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.IssuerId = s;
                                    objPort.SecurityId = listForBenchmark.FirstOrDefault(t => t.Value == row.ISSUE_NAME).Key;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId)
                                        .Select(t => t.Amount).FirstOrDefault();
                                    objPort.PortfolioPercent = row.BENCHMARK_WEIGHT;
                                    valuesBenchForAllDataIds.Add(objPort);
                                }
                            }
                            foreach (String s in distinctSecurityNamesForBenchmark)
                            {
                                foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmark.Where(t => t.ISSUE_NAME == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.SecurityId = listForBenchmark.FirstOrDefault(t => t.Value == s).Key;
                                    objPort.IssuerId = row.ISSUER_ID;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForBenchmark
                                        .FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
                                    objPort.PortfolioPercent = row.BENCHMARK_WEIGHT;
                                    valuesBenchForAllDataIds.Add(objPort);
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                }
                if (valuesPortForAllDataIds != null && valuesPortForAllDataIds.Count() > 0)
                {
                    valuesPortForPRevenue = valuesPortForAllDataIds.Where(t => t.DataId == 197).ToList();
                    valuesPortForEVEBITDA = valuesPortForAllDataIds.Where(t => t.DataId == 198).ToList();
                    valuesPortForPE = valuesPortForAllDataIds.Where(t => t.DataId == 187).ToList();
                    valuesPortForPCE = valuesPortForAllDataIds.Where(t => t.DataId == 189).ToList();
                    valuesPortForPBV = valuesPortForAllDataIds.Where(t => t.DataId == 188).ToList();
                    valuesPortForROE = valuesPortForAllDataIds.Where(t => t.DataId == 200).ToList();
                    valuesPortForDividendYield = valuesPortForAllDataIds.Where(t => t.DataId == 236).ToList();
                    valuesPortForRevGrowth = valuesPortForAllDataIds.Where(t => t.DataId == 201).ToList();
                    valuesPortForNetGrowth = valuesPortForAllDataIds.Where(t => t.DataId == 202).ToList();
                }
                if (valuesBenchForAllDataIds != null && valuesBenchForAllDataIds.Count() > 0)
                {
                    valuesBenchForPRevenue = valuesBenchForAllDataIds.Where(t => t.DataId == 197).ToList();
                    valuesBenchForEVEBITDA = valuesBenchForAllDataIds.Where(t => t.DataId == 198).ToList();
                    valuesBenchForPE = valuesBenchForAllDataIds.Where(t => t.DataId == 187).ToList();
                    valuesBenchForPCE = valuesBenchForAllDataIds.Where(t => t.DataId == 189).ToList();
                    valuesBenchForPBV = valuesBenchForAllDataIds.Where(t => t.DataId == 188).ToList();
                    valuesBenchForROE = valuesBenchForAllDataIds.Where(t => t.DataId == 200).ToList();
                    valuesBenchForDividendYield = valuesBenchForAllDataIds.Where(t => t.DataId == 236).ToList();
                    valuesBenchForRevGrowth = valuesBenchForAllDataIds.Where(t => t.DataId == 201).ToList();
                    valuesBenchForNetGrowth = valuesBenchForAllDataIds.Where(t => t.DataId == 202).ToList();
                }
                GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolio(valuesPortForPRevenue, "Forward P/Revenue", ref entry);
                CalculateHarmonicMeanBenchmark(valuesBenchForPRevenue, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolio(valuesPortForEVEBITDA, "Forward EV/EBITDA", ref entry);
                CalculateHarmonicMeanBenchmark(valuesBenchForEVEBITDA, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolio(valuesPortForPE, "Forward P/E", ref entry);
                CalculateHarmonicMeanBenchmark(valuesBenchForPE, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolio(valuesPortForPCE, "Forward P/CE", ref entry);
                CalculateHarmonicMeanBenchmark(valuesBenchForPCE, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolio(valuesPortForPBV, "Forward P/BV", ref entry);
                CalculateHarmonicMeanBenchmark(valuesBenchForPBV, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolioROE(valuesPortForROE, "Forward ROE", ref entry);
                CalculateHarmonicMeanBenchmarkROE(valuesBenchForROE, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolioROE(valuesPortForDividendYield, "Forward Dividend Yield", ref entry);
                CalculateHarmonicMeanBenchmarkROE(valuesBenchForDividendYield, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolioROE(valuesPortForRevGrowth, "Forward Revenue Growth", ref entry);
                CalculateHarmonicMeanBenchmarkROE(valuesBenchForRevGrowth, ref entry);
                result.Add(entry);
                entry = new GreenField.DataContracts.DataContracts.ValuationQualityGrowthData();
                CalculateHarmonicMeanPortfolioROE(valuesPortForNetGrowth, "Forward Net Income Growth", ref entry);
                CalculateHarmonicMeanBenchmarkROE(valuesBenchForNetGrowth, ref entry);
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

        /// <summary>
        /// Calculates Harmonic mean for portfolio data id's less than 200
        /// </summary>
        /// <param name="filteredByDataIdList">filtered by data Id list</param>
        /// <param name="description">description to be added to the grid</param>
        /// <param name="entry">object of type ValuationQualityGrowthData </param>
        /// <param name="initialSumDirtyValuePC">variable to store the sum of all portfolio weights</param>
        /// <param name="harmonicMeanPortfolio">Harmonic mean</param>
        private void CalculateHarmonicMeanPortfolio(List<CalculatedValuesForValuation> filteredByDataIdList, String description,
            ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? initialSumDirtyValuePC = 0,
            Decimal? harmonicMeanPortfolio = 0)
        {
            Decimal? portWeight = 0;
            Decimal? multipliedValue = 0;
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumDirtyValuePC = initialSumDirtyValuePC + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumDirtyValuePC != 0)
                {
                    portWeight = (row.PortfolioPercent / initialSumDirtyValuePC);
                }
                if (row.Amount != 0)
                {
                    row.InverseAmount = 1 / row.Amount;
                }
                multipliedValue = portWeight * row.InverseAmount;
                harmonicMeanPortfolio = harmonicMeanPortfolio + multipliedValue;
            }
            entry.Description = description;
            if (harmonicMeanPortfolio != 0)
            {
                entry.Portfolio = 1 / harmonicMeanPortfolio;
            }
        }

        /// <summary>
        /// Calculates Harmonic mean for benchmark data id's less than 200
        /// </summary>
        /// <param name="filteredByDataIdList">filtered by data Id list</param>
        /// <param name="entry">object of type ValuationQualityGrowthData </param>
        /// <param name="harmonicMeanBenchmark">Harmonic mean</param>
        private void CalculateHarmonicMeanBenchmark(List<CalculatedValuesForValuation> filteredByDataIdList,
            ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? harmonicMeanBenchmark = 0)
        {
            Decimal? benchWeight = 0;
            Decimal? multipliedValue = 0;
            Decimal? initialSumBenchmarkWeight = 0;
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumBenchmarkWeight = initialSumBenchmarkWeight + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumBenchmarkWeight != 0)
                {
                    benchWeight = (row.PortfolioPercent / initialSumBenchmarkWeight);
                }
                if (row.Amount != 0)
                {
                    row.InverseAmount = 1 / row.Amount;
                }
                multipliedValue = benchWeight * row.InverseAmount;
                harmonicMeanBenchmark = harmonicMeanBenchmark + multipliedValue;
            }

            if (harmonicMeanBenchmark != 0)
            {
                entry.Benchmark = 1 / harmonicMeanBenchmark;
            }
            if (Convert.ToDecimal(entry.Benchmark) != 0)
            {
                entry.Relative = Convert.ToDecimal(entry.Portfolio) / Convert.ToDecimal(entry.Benchmark);
            }
        }

        /// <summary>
        /// Calculates Harmonic mean for portfolio data id's greater than 200
        /// </summary>
        /// <param name="filteredByDataIdList">filtered by data Id list</param>
        /// <param name="description">description to be added to the grid</param>
        /// <param name="entry">object of type ValuationQualityGrowthData </param>
        /// <param name="initialSumDirtyValuePC">variable to store the sum of all portfolio weights</param>
        /// <param name="harmonicMeanPortfolio">Harmonic mean</param>
        private void CalculateHarmonicMeanPortfolioROE(List<CalculatedValuesForValuation> filteredByDataIdList, String description, ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? initialSumDirtyValuePC = 0, Decimal? harmonicMeanPortfolio = 0)
        {
            Decimal? portWeight = 0;
            Decimal? multipliedValue = 0;
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumDirtyValuePC = initialSumDirtyValuePC + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumDirtyValuePC != 0)
                {
                    portWeight = (row.PortfolioPercent / initialSumDirtyValuePC);
                }
                multipliedValue = portWeight * row.Amount;
                harmonicMeanPortfolio = harmonicMeanPortfolio + multipliedValue;
            }
            entry.Description = description;
            entry.Portfolio = harmonicMeanPortfolio * 100;
        }


        /// <summary>
        /// Calculates Harmonic mean for benchmark data id's greater than 200
        /// </summary>
        /// <param name="filteredByDataIdList">filtered by data Id list</param>
        /// <param name="entry">object of type ValuationQualityGrowthData </param>
        /// <param name="harmonicMeanBenchmark">Harmonic mean</param>
        private void CalculateHarmonicMeanBenchmarkROE(List<CalculatedValuesForValuation> filteredByDataIdList,
            ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? harmonicMeanBenchmark = 0)
        {
            Decimal? benchWeight = 0;
            Decimal? multipliedValue = 0;
            Decimal? initialSumBenchmarkWeight = 0;
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumBenchmarkWeight = initialSumBenchmarkWeight + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumBenchmarkWeight != 0)
                {
                    benchWeight = (row.PortfolioPercent / initialSumBenchmarkWeight);
                }
                multipliedValue = benchWeight * row.Amount;
                harmonicMeanBenchmark = harmonicMeanBenchmark + multipliedValue;
            }
            entry.Benchmark = harmonicMeanBenchmark * 100;
            if (Convert.ToDecimal(entry.Benchmark) != 0)
            {
                entry.Relative = Convert.ToDecimal(entry.Portfolio) / Convert.ToDecimal(entry.Benchmark);
            }
            entry.Portfolio = Math.Round(Convert.ToDecimal(entry.Portfolio), 1) + "%";
            entry.Benchmark = Math.Round(Convert.ToDecimal(harmonicMeanBenchmark * 100), 1) + "%";
        }
        #endregion

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<String> RetrieveCompanyData()
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<String> result = entity.GF_SECURITY_BASEVIEW.OrderBy(record => record.ISSUER_NAME).ToList()
                    .Select(record => record.ISSUER_NAME).ToList().Distinct().ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region Summary of Emerging Market Data
        /// <summary>
        /// Retrieve data for Market data gadget
        /// </summary>
        /// <param name="lastBusinessDate">last business date available in the view </param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<EMSummaryMarketData> RetrieveEmergingMarketData(String selectedPortfolio)
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities research = new ExternalResearchEntities();
                research.CommandTimeout = 5000;
                Decimal? benchmarkWeight = 0;
                String benId = null;
                List<String> benchmarkIds = new List<string>();
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                List<EMSummaryMarketBenchmarkData> emBenchData = new List<EMSummaryMarketBenchmarkData>();
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS
                    .Where(record => record.PORTFOLIO_ID == selectedPortfolio).
                    OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                {
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                    {
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);
                    }
                 benId = lastBusinessRecord.BENCHMARK_ID;
                }    
                //gathering the data from GF_BENCHMARK_HOLDINGS
                List<GF_BENCHMARK_HOLDINGS> dataBenchmarkHoldings = new List<GF_BENCHMARK_HOLDINGS>();
                dataBenchmarkHoldings = dimensionEntity.GF_BENCHMARK_HOLDINGS.Where(record => record.BENCHMARK_ID == benId
                                                         && record.PORTFOLIO_DATE == lastBusinessDate
                                                          && record.BENCHMARK_WEIGHT > 0).ToList();
                //gathering data from GF_PERF_DAILY_ATTRIBUTION
                List<GF_PERF_DAILY_ATTRIBUTION> attributionData = new List<GF_PERF_DAILY_ATTRIBUTION>();
                attributionData = RetrieveBenchmarkYTDReturns(selectedPortfolio, lastBusinessDate);

                var benchData = dataBenchmarkHoldings != null ? (from p in dataBenchmarkHoldings
                                                                 select new
                                                                 {
                                                                     IssuerId = p.ISSUER_ID,
                                                                     AsecShortName = p.ASEC_SEC_SHORT_NAME,
                                                                     IssueName = p.ISSUE_NAME,
                                                                     Region = p.ASHEMM_PROP_REGION_CODE,
                                                                     Country = p.ISO_COUNTRY_CODE,
                                                                     Sector = p.GICS_SECTOR,
                                                                     Industry = p.GICS_INDUSTRY,
                                                                     Weight = p.BENCHMARK_WEIGHT
                                                                 }).Distinct() : null;

                List<String> asecShortNames = benchData != null ? (from p in benchData
                                                                   select p.AsecShortName).ToList() : null;
                //retrieve security Id's              
                List<SecurityData> securityData = RetrieveSecurityIds(asecShortNames);
                List<String> distinctSecurityId = securityData.Select(record => record.SecurityId).ToList();
                //List<String> distinctIssuerId = securityData.Select(record => record.IssuerId).ToList();

                String securityIds = StringBuilder(distinctSecurityId);
                String yearList = (DateTime.Now.Year).ToString() + "," + (DateTime.Now.Year + 1).ToString();
                //String _issuerIds = StringBuilder(distinctIssuerId);

                foreach (String asec in securityData.Select(record => record.AsecShortName).ToList())
                {
                    EMSummaryMarketBenchmarkData obj = new EMSummaryMarketBenchmarkData();
                    obj.AsecShortName = benchData.Where(t => t.AsecShortName == asec).Select(t => t.AsecShortName)
                        .FirstOrDefault();
                    obj.IssuerId = benchData.Where(t => t.AsecShortName == asec).Select(t => t.IssuerId).FirstOrDefault();
                    obj.IssueName = benchData.Where(t => t.AsecShortName == asec).Select(t => t.IssueName).FirstOrDefault();
                    obj.CountryCode = benchData.Where(t => t.AsecShortName == asec).Select(t => t.Country).FirstOrDefault();
                    obj.BenWeight = Convert.ToDecimal(benchData.Where(t => t.AsecShortName == asec).Select(t => t.Weight).FirstOrDefault());
                    emBenchData.Add(obj);
                }
                //calling the stored procedure from the database
                List<EMSummaryMarketData> resultList = new List<EMSummaryMarketData>();
                List<EMSummaryMarketData> tempResultListForGroups = new List<EMSummaryMarketData>();
                List<EMSumCountryData> wholeData = research.usp_GetCountryDataForEMMarketData().ToList();
                List<EMSumCountryData> countryData = wholeData.Where(t => t.Type == "C").ToList();
                List<EMSumCountryData> groupData = wholeData.Where(t => t.Type == "G").ToList();
                List<String> countryCodes = wholeData.Where(t => t.Type == "C").Select(t => t.CountryCode).ToList();
                String countries = StringBuilder(countryCodes);
                List<FXData> fxData = research.usp_GetCurrencyDataForCountries(countries).ToList();
                List<MacroEMData> macroData = research.usp_GetMacroDataEMSummary(countries).ToList();
                List<EMData> emSummaryData = research.usp_GetDataForEMData(securityIds, yearList).ToList();
                List<EMSummFinalData> emFinalData = new List<EMSummFinalData>();
                DateTime dateQ1Year1 = new DateTime(DateTime.Now.Year,3,31,0,0,0);
                DateTime dateQ2Year1 = new DateTime(DateTime.Now.Year, 6, 30,0, 0, 0);
                DateTime dateQ3Year1 = new DateTime(DateTime.Now.Year, 9, 30, 0, 0, 0);
                DateTime dateQ4Year1 = new DateTime(DateTime.Now.Year, 12, 31, 0, 0, 0);
                DateTime dateQ1Year2 = new DateTime(DateTime.Now.Year + 1, 3, 31, 0, 0, 0);
                DateTime dateQ2Year2 = new DateTime(DateTime.Now.Year + 1, 6, 30, 0, 0, 0);
                DateTime dateQ3Year2 = new DateTime(DateTime.Now.Year + 1, 9, 30, 0, 0, 0);
                DateTime dateQ4Year2 = new DateTime(DateTime.Now.Year + 1, 12, 31, 0, 0, 0);
                foreach (EMSumCountryData row in countryData)
                {
                    EMSummaryMarketData obj = new EMSummaryMarketData();
                    obj.Region = row.RegionName;
                    obj.Country = row.CountryName;
                    obj.BenchmarkId = benId;
                    obj.PortfolioDate = lastBusinessDate;
                    if (emBenchData != null)
                    {
                        obj.BenchmarkWeight = emBenchData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()).
                            Sum(t => t.BenWeight);
                            
                    }
                    if (attributionData != null)
                    {
                        obj.YTDReturns = attributionData.Where(t => t.AGG_LVL_1.Trim().ToLower() == row.CountryCode.Trim().ToLower()).
                            Sum(t => t.BM1_RC_TWR_YTD);
                            
                    }
                    if (emSummaryData != null)
                    {
                        foreach (int dataId in emSummaryData.Select(t => t.DataId).Distinct().ToList())
                        {
                            foreach (int year in emSummaryData.Where(t => t.DataId == dataId).Select(t => t.DataYear).Distinct().ToList())
                            {
                                foreach (String d in emSummaryData.Where(t => t.DataId == dataId && t.DataYear == year).
                                    Select(t => t.DataType).Distinct().ToList())
                                {
                                    List<EMSummHarmonicData> emHarmonicData = new List<EMSummHarmonicData>();
                                    List<EMData> emFilteredData = new List<EMData>();
                                    EMSummFinalData emFinData = new EMSummFinalData();
                                    emFilteredData = emSummaryData.Where(t => t.DataId == dataId && t.DataYear == year && t.DataType == d)
                                        .ToList();
                                    foreach (EMData emData in emFilteredData)
                                    {
                                        EMSummHarmonicData summHarDataObj = new EMSummHarmonicData();
                                        summHarDataObj.IssuerId = emData.IssuerId;
                                        summHarDataObj.SecurityId = emData.SecurityId;
                                        String secId = summHarDataObj.SecurityId;
                                        String asecShrtName = securityData.Where(record => record.SecurityId == secId)
                                            .Select(record => record.AsecShortName).FirstOrDefault();
                                        summHarDataObj.PeriodYear = year;
                                        summHarDataObj.DataId = dataId;
                                        summHarDataObj.Country = row.CountryCode;
                                        summHarDataObj.DataType = d;
                                        summHarDataObj.BenWeight = Convert.ToDecimal(emBenchData
                                            .Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower() &&
                                            t.AsecShortName == asecShrtName).Select(t => t.BenWeight).FirstOrDefault());
                                        summHarDataObj.Amount = emData.Amount;
                                        summHarDataObj.InvAmount = emData.Amount != 0 ? (1 / emData.Amount) : 0;
                                        emHarmonicData.Add(summHarDataObj);
                                    }
                                    Decimal harmonicMean = CalculateHarmonicMeanBenchmark(emHarmonicData);
                                    emFinData.DataId = dataId;
                                    emFinData.DataType = d;
                                    emFinData.PeriodYear = year;
                                    emFinData.CountryCode = row.CountryCode;
                                    emFinData.HarmonicMean = harmonicMean;
                                    emFinalData.Add(emFinData);
                                }
                            }
                        }
                    }
                    if (emFinalData != null)
                    {
                        obj.PECurYear = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.PECurYearCon = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year && t.DataType == "C"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.PENextYear = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType == "W"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.PENextYearCon = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType == "C"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.USDEarCurYear = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                          .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.USDEarCurYearCon = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year && t.DataType == "C"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.USDEarNextYear = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType
                            == "W" && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.USDEarNextYearCon = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType
                            == "C" && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.PBVCurYear = emFinalData.Where(t => t.DataId == 164 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                          .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.DYCurYear = emFinalData.Where(t => t.DataId == 192 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                        obj.ROECurYear = emFinalData.Where(t => t.DataId == 133 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                            && t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower())
                            .Select(t => t.HarmonicMean).FirstOrDefault();
                    }
                    if (fxData != null)
                    {
                        obj.FxY1Q1 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ1Year1).
                            Select(t => t.FX_RATE).FirstOrDefault();
                        obj.FxY1Q2 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ2Year1).
                            Select(t => t.FX_RATE).FirstOrDefault();
                        obj.FxY1Q3 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ3Year1).
                            Select(t => t.FX_RATE).FirstOrDefault();
                        obj.FxY1Q4 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ4Year1).
                            Select(t => t.FX_RATE).FirstOrDefault();
                        obj.FxY2Q1 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ1Year2).
                            Select(t => t.FX_RATE).FirstOrDefault();
                        obj.FxY2Q2 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ2Year2).
                            Select(t => t.FX_RATE).FirstOrDefault();
                        obj.FxY2Q3 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ3Year2).
                            Select(t => t.FX_RATE).FirstOrDefault();
                        obj.FxY2Q4 = fxData.Where(t => t.COUNTRY_CODE.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.FX_DATE == dateQ4Year2).
                            Select(t => t.FX_RATE).FirstOrDefault();
                    }
                    if (macroData != null)
                    {
                        obj.GdpY0 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                            && t.Year1 == DateTime.Now.Year - 1 && t.Field.Trim() == "REAL_GDP_GROWTH_RATE")
                            .Select(t => t.Value).FirstOrDefault();
                        obj.GdpY1 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year && t.Field.Trim() == "REAL_GDP_GROWTH_RATE")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.GdpY2 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year + 1 && t.Field.Trim() == "REAL_GDP_GROWTH_RATE")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.InflationY0 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year - 1 && t.Field.Trim() == "INFLATION_PCT")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.InflationY1 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year && t.Field.Trim() == "INFLATION_PCT")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.InflationY2 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year + 1 && t.Field.Trim() == "INFLATION_PCT")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.StInterestY0 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year - 1 && t.Field.Trim() == "ST_INTEREST_RATE")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.StInterestY1 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year && t.Field.Trim() == "ST_INTEREST_RATE")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.CurrAccountY0 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year - 1 && t.Field.Trim() == "CURRENT_ACCOUNT_PCT_GDP")
                           .Select(t => t.Value).FirstOrDefault();
                        obj.CurrAccountY1 = macroData.Where(t => t.CountryCode.Trim().ToLower() == row.CountryCode.Trim().ToLower()
                           && t.Year1 == DateTime.Now.Year && t.Field.Trim() == "CURRENT_ACCOUNT_PCT_GDP")
                           .Select(t => t.Value).FirstOrDefault();
                        resultList.Add(obj);
                    }
                }
                foreach (String group in groupData.Select(t => t.CountryName).Distinct())
                {
                    EMSummaryMarketData obj = new EMSummaryMarketData();
                    obj.Region = groupData.Where(t => t.CountryName == group).Select(t => t.RegionName).FirstOrDefault();
                    obj.Country = group;
                    foreach (String cou in groupData.Where(t => t.CountryName == group).Select(t => t.CountryCode).Distinct())
                    {
                        benchmarkWeight = benchmarkWeight + emBenchData.Where(t => t.CountryCode.Trim().ToLower() == cou.Trim().ToLower())
                            .Sum(t => t.BenWeight);                              
                    }
                    obj.BenchmarkWeight = benchmarkWeight;
                    tempResultListForGroups.Add(obj);
                }
                foreach (int dataId in emSummaryData.Select(t => t.DataId).Distinct().ToList())
                {
                    foreach (int year in emSummaryData.Where(t => t.DataId == dataId).Select(t => t.DataYear).Distinct().ToList())
                    {
                        foreach (String d in emSummaryData.Where(t => t.DataId == dataId && t.DataYear == year).
                            Select(t => t.DataType).Distinct().ToList())
                        {
                            List<EMSummHarmonicData> emHarmonicData = new List<EMSummHarmonicData>();
                            List<EMData> emFilteredData = new List<EMData>();
                            EMSummFinalData emFinData = new EMSummFinalData();
                            emFilteredData = emSummaryData.Where(t => t.DataId == dataId && t.DataYear == year && t.DataType == d)
                                .ToList();
                            foreach (String group in groupData.Select(t => t.CountryName).Distinct())
                            {                               
                                foreach (String cou in groupData.Where(t => t.CountryName == group).Select(t => t.CountryCode).Distinct())
                                {
                                    foreach (EMData emData in emFilteredData)
                                    {
                                        EMSummHarmonicData summHarDataObj = new EMSummHarmonicData();
                                        summHarDataObj.IssuerId = emData.IssuerId;
                                        summHarDataObj.SecurityId = emData.SecurityId;
                                        String secId = summHarDataObj.SecurityId;
                                        String asecShrtName = securityData.Where(record => record.SecurityId == secId)
                                            .Select(record => record.AsecShortName).FirstOrDefault();
                                        summHarDataObj.PeriodYear = year;
                                        summHarDataObj.DataId = dataId;
                                        summHarDataObj.Country = cou;
                                        summHarDataObj.DataType = d;
                                        summHarDataObj.BenWeight = Convert.ToDecimal(emBenchData.Where(t => t.CountryCode.Trim().ToLower() == cou.Trim().ToLower() &&
                                            t.AsecShortName == asecShrtName).Select(t => t.BenWeight).FirstOrDefault());
                                        summHarDataObj.Amount = emData.Amount;
                                        summHarDataObj.InvAmount = emData.Amount != 0 ? (1 / emData.Amount) : 0;
                                        emHarmonicData.Add(summHarDataObj);
                                    }
                                }
                                Decimal harmonicMean = CalculateHarmonicMeanBenchmark(emHarmonicData);
                                emFinData.DataId = dataId;
                                emFinData.DataType = d;
                                emFinData.PeriodYear = year;
                                emFinData.CountryCode = group;
                                emFinData.HarmonicMean = harmonicMean;
                                emFinalData.Add(emFinData);
                            }
                        }
                    }
                }
                foreach (EMSummaryMarketData row in tempResultListForGroups)
                {
                    row.PECurYear = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                       .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.PECurYearCon = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year && t.DataType == "C"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.PENextYear = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType == "W"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.PENextYearCon = emFinalData.Where(t => t.DataId == 166 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType == "C"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.USDEarCurYear = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                      .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.USDEarCurYearCon = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year && t.DataType == "C"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.USDEarNextYear = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType == "W"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.USDEarNextYearCon = emFinalData.Where(t => t.DataId == 177 && t.PeriodYear == DateTime.Now.Year + 1 && t.DataType ==
                        "C" && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.PBVCurYear = emFinalData.Where(t => t.DataId == 164 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                      .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.DYCurYear = emFinalData.Where(t => t.DataId == 192 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    row.ROECurYear = emFinalData.Where(t => t.DataId == 133 && t.PeriodYear == DateTime.Now.Year && t.DataType == "W"
                        && t.CountryCode.Trim().ToLower() == row.Country.Trim().ToLower())
                        .Select(t => t.HarmonicMean).FirstOrDefault();
                    resultList.Add(row);
                }
                return resultList;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                Trace.WriteLine(ex.StackTrace);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieve security Id's according to asecShortNames from GF_SECURITY_BASEVIEW
        /// </summary>
        /// <param name="asecShortNames"></param>
        /// <returns></returns>
        private List<SecurityData> RetrieveSecurityIds(List<String> asecShortNames)
        {
            List<SecurityData> secData = new List<SecurityData>();

            foreach (String asec in asecShortNames)
            {
                GF_SECURITY_BASEVIEW item = (dimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.ASEC_SEC_SHORT_NAME == asec)
                    .FirstOrDefault());
                if (item != null)
                {
                    secData.Add(new SecurityData()
                    {
                        SecurityId = item.SECURITY_ID.ToString(),
                        IssuerId = item.ISSUER_ID,
                        IssueName = item.ISSUE_NAME,
                        AsecShortName = item.ASEC_SEC_SHORT_NAME
                    });
                }
            }
            return secData;
        }

        /// <summary>
        /// String builder that adds ' between Id's
        /// </summary>
        /// <param name="param">String of Id's</param>
        /// <returns></returns>
        private string StringBuilder(List<String> param)
        {
            StringBuilder var = new StringBuilder();
            int check = 1;
            foreach (String item in param)
            {
                check = 0;
                var.Append(",'" + item + "'");
            }
            var = check == 0 ? var.Remove(0, 1) : null;
            string result = var == null ? null : var.ToString();
            return result;
        }

        /// <summary>
        /// Calculates harmonic mean
        /// </summary>
        /// <param name="filteredList">filtered list according to various groups formed</param>
        /// <returns></returns>
        private Decimal CalculateHarmonicMeanBenchmark(List<EMSummHarmonicData> filteredList)
        {
            Decimal? initialSumBenchmarkWeight = 0;
            Decimal? multipliedAmount = 0;
            Decimal? totalAmount = 0;
            Decimal? harmonicMean = 0;
            Decimal? benchWeight = 0;

            initialSumBenchmarkWeight = filteredList.Sum(t => t.BenWeight);

            foreach (EMSummHarmonicData row in filteredList)
            {
                benchWeight = initialSumBenchmarkWeight != 0 ? (row.BenWeight / initialSumBenchmarkWeight) : 0;
                multipliedAmount = row.InvAmount * benchWeight;
                totalAmount = totalAmount + multipliedAmount;
            }
            harmonicMean = totalAmount != 0 ? 1 / totalAmount : 0;
            return Convert.ToDecimal(harmonicMean);
        }

        private List<GF_PERF_DAILY_ATTRIBUTION> RetrieveBenchmarkYTDReturns(String selectedPortfolio,
            DateTime lastBusinessDate)
        {
            DimensionEntitiesService.Entities entity = DimensionEntity;
            List<GF_PERF_DAILY_ATTRIBUTION> dataDailyAttribution = new List<GF_PERF_DAILY_ATTRIBUTION>();
            dataDailyAttribution = entity.GF_PERF_DAILY_ATTRIBUTION.Where(record => record.PORTFOLIO == selectedPortfolio
                                                          && record.TO_DATE == lastBusinessDate && record.NODE_NAME == "Country").ToList();
            return dataDailyAttribution;
        }
        #endregion

        #region Investment Context Data
        /// <summary>
        /// Retrieve Investment Context Data
        /// </summary>
        /// <param name="issuerId">issuer id from the view </param>
        /// <param name="context"> Retrieve the data group by country, industry or both</param>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<List<InvestmentContextDetailsData>> RetrieveInvestmentContextData(String issuerId, String context)
        {
            List<List<InvestmentContextDetailsData>> icdData = new List<List<InvestmentContextDetailsData>>(); ;
            if (context == "Country")
            {
                icdData.Add(getInvestmentContextByCountry(issuerId, context));
            }
            else if(context == "Industry")
            {
                icdData.Add(getInvestmentContextByIndustry(issuerId, context));
            }
            else if (context == "Both") //default this is what called from the gadget
            {
                icdData.Add(getInvestmentContextByCountry(issuerId, "Country"));
                icdData.Add(getInvestmentContextByIndustry(issuerId, "Industry"));
            }
            return icdData;
        }


        private List<InvestmentContextDetailsData> getInvestmentContextByCountry(String issuerId,String context)
        {
            ExternalResearchEntities entity = new ExternalResearchEntities();
            //call the stored procedure and get the investment context data for the given issuer and the context "country"
            entity.CommandTimeout = 600;
            List<InvestmentContextData> getInvestmentContextResult = entity.getInvestmentContext(issuerId, context).ToList();

            var DistinctDataId = getInvestmentContextResult.Select(a => a.DataId).Distinct().ToList();
            List<SecurityDataIdScrub> securityDataIdScrub = new List<SecurityDataIdScrub>();
            //store the data in the scrub class to be scrubbed
            foreach (InvestmentContextData icd in getInvestmentContextResult)
            {
                securityDataIdScrub.Add(new SecurityDataIdScrub()
                {
                    DataId = icd.DataId,
                    IssuerId = icd.issuer_id,
                    IssuerName = icd.issuer_name,
                    IsoCountryCode = icd.iso_country_code,
                    SecurityId = icd.SecurityId,
                    GICS_Industry = icd.gics_industry,
                    GICS_Industry_Name = icd.gics_industry_name,
                    GICS_Sector = icd.gics_sector,
                    GICS_Sector_Name = icd.gics_sector_name,
                    PeriodYear = icd.period_year,
                    OriginalValue = icd.value,
                    ScrubbedValue = null
                });
            }

            //scrub the data for each data id
            DataScrubber ds = new DataScrubber();
            for (int i = 0; i < DistinctDataId.Count; i++)
            {
                ds.DoScrubbing(securityDataIdScrub, DistinctDataId[i], "Range");
            }

            // Rearrange the data in the format to be displayed
            List<InvestmentContextDetailsData> icdList = RearrangeData(securityDataIdScrub, context);

            // get the list of issuer ids that you want to display
            List<string> issuerList = getListOfIssuersToDisplay(icdList, issuerId);
            // get the report sum/average line
            List<InvestmentContextDetailsData> finalICD = getTotalLine(icdList, context);
            // get the sum/average for each sector
            List<InvestmentContextDetailsData> icdGroupedResult = GroupBySector(finalICD);
            // remove the issuers which are not part of the top 100 issuer list based on the market cap.
            List<InvestmentContextDetailsData> icdSectorChildren = icdGroupedResult[0].children;
            foreach (var icd in icdSectorChildren)
            {
                List<InvestmentContextDetailsData> icdDetailsData = icd.children;
                icdDetailsData.RemoveAll(x => !issuerList.Contains(x.IssuerId));
            }
            
            return icdGroupedResult;

        }


        private List<InvestmentContextDetailsData> getInvestmentContextByIndustry(String issuerId,String context)
        {
            ExternalResearchEntities entity = new ExternalResearchEntities();
            //call the stored procedure and get the investment context data for the given issuer and the context "country"
            entity.CommandTimeout = 600;
            List<InvestmentContextData> getInvestmentContextResult = entity.getInvestmentContext(issuerId, context).ToList();

            var DistinctDataId = getInvestmentContextResult.Select(a => a.DataId).Distinct().ToList();
            List<SecurityDataIdScrub> securityDataIdScrub = new List<SecurityDataIdScrub>();
            //store the data in the scrub class to be scrubbed
            foreach (InvestmentContextData icd in getInvestmentContextResult)
            {
                securityDataIdScrub.Add(new SecurityDataIdScrub()
                {
                    DataId = icd.DataId,
                    IssuerId = icd.issuer_id,
                    IssuerName = icd.issuer_name,
                    IsoCountryCode = icd.iso_country_code,
                    SecurityId = icd.SecurityId,
                    GICS_Industry = icd.gics_industry,
                    GICS_Industry_Name = icd.gics_industry_name,
                    GICS_Sector = icd.gics_sector,
                    GICS_Sector_Name = icd.gics_sector_name,
                    PeriodYear = icd.period_year,
                    OriginalValue = icd.value,
                    ScrubbedValue = null
                });
            }
            //scrub the data for each data id
            DataScrubber ds = new DataScrubber();
            for (int i = 0; i < DistinctDataId.Count; i++)
            {
                ds.DoScrubbing(securityDataIdScrub, DistinctDataId[i], "Range");
            }
            // Rearrange the data in the format to be displayed
            List<InvestmentContextDetailsData> icdList = RearrangeData(securityDataIdScrub, context);
            // get the list of issuer ids that you want to display
            List<string> issuerList = getListOfIssuersToDisplay(icdList,issuerId);
            // get the report sum/average line
            List<InvestmentContextDetailsData> finalICD = getTotalLine(icdList,context);
            // remove the issuers which are not part of the top 100 issuer list based on the market cap.
            List<InvestmentContextDetailsData> icdChildren = finalICD[0].children;
            icdChildren.RemoveAll(x => !issuerList.Contains(x.IssuerId));
            return finalICD;

        }

        
       

        private List<InvestmentContextDetailsData> RearrangeData(List<SecurityDataIdScrub> securityDataIdScrub, string context)
        {
            //rearrange data 
            var securityIdList = securityDataIdScrub.Select(a => a.SecurityId).Distinct().ToList();
            List<InvestmentContextDetailsData> investmentcontextDetailList = new List<InvestmentContextDetailsData>();
            ExternalResearchEntities entity = new ExternalResearchEntities();
            DateTime? lastBusinessDate = entity.GF_COMPOSITE_LTHOLDINGS.Where(x => x.PORTFOLIO_ID == "EQYALL").OrderByDescending(x => x.PORTFOLIO_DATE).Select(x=>x.PORTFOLIO_DATE).FirstOrDefault();
            List<GF_COMPOSITE_LTHOLDINGS> gfCompositeHoldingList = entity.GF_COMPOSITE_LTHOLDINGS.Where(x => x.PORTFOLIO_DATE == lastBusinessDate && x.PORTFOLIO_ID == "EQYALL").ToList();
            string issuerid = null;
            foreach (var security in securityIdList)
            {
                issuerid = securityDataIdScrub.Where(a => a.SecurityId == security).Select(a => a.IssuerId).FirstOrDefault();
                investmentcontextDetailList.Add(new InvestmentContextDetailsData()
                {
                    IssuerId                = securityDataIdScrub.Where(a=>a.SecurityId==security).Select(a=>a.IssuerId).FirstOrDefault(),
                    IssuerName              = securityDataIdScrub.Where(a => a.SecurityId == security).Select(a => a.IssuerName).FirstOrDefault(),
                    IsoCountryCode          = securityDataIdScrub.Where(a => a.SecurityId == security).Select(a => a.IsoCountryCode).FirstOrDefault(),
                    GicsSectorCode          = securityDataIdScrub.Where(a => a.SecurityId == security).Select(a => a.GICS_Sector).FirstOrDefault(),
                    GicsSectorName          = securityDataIdScrub.Where(a => a.SecurityId == security).Select(a => a.GICS_Sector_Name).FirstOrDefault(),
                    GicsIndustryCode        = securityDataIdScrub.Where(a => a.SecurityId == security).Select(a => a.GICS_Industry).FirstOrDefault(),
                    GicsIndustryName        = securityDataIdScrub.Where(a => a.SecurityId == security).Select(a => a.GICS_Industry_Name).FirstOrDefault(),
                    MarketValue             = gfCompositeHoldingList.Where(x => x.PORTFOLIO_DATE == lastBusinessDate && x.ISSUER_ID == issuerid).Sum(x => x.DIRTY_VALUE_PC) == 0 ? null : gfCompositeHoldingList.Where(x => x.PORTFOLIO_DATE == lastBusinessDate && x.ISSUER_ID == issuerid).Sum(x => x.DIRTY_VALUE_PC)/1000000,
                    MarketValueScrubbed = gfCompositeHoldingList.Where(x => x.PORTFOLIO_DATE == lastBusinessDate && x.ISSUER_ID == issuerid).Sum(x => x.DIRTY_VALUE_PC) == 0 ? null : gfCompositeHoldingList.Where(x => x.PORTFOLIO_DATE == lastBusinessDate && x.ISSUER_ID == issuerid).Sum(x => x.DIRTY_VALUE_PC) / 1000000,
                    MarketCap               = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 185).Select(a => a.OriginalValue).FirstOrDefault(),
                    MarketCapScrubbed       = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 185).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 185).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 185).Select(a => a.ScrubbedValue).FirstOrDefault(), //Market Cap Scrubbed
                    ForwardPE               = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 187).Select(a => a.OriginalValue).FirstOrDefault(), //Forward PE
                                              
                    ForwardPEScrubbed       = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 187).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 187).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 187).Select(a => a.ScrubbedValue).FirstOrDefault(), //Forward PE Scrubbed

                    ForwardPBV              = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 188).Select(a => a.OriginalValue).FirstOrDefault(), //Forward PBV
                                              
                    ForwardPBVScrubbed      = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 188).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 188).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 188).Select(a => a.ScrubbedValue).FirstOrDefault(), //Forward PBV Scrubbed

                    PECurrentYear           = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.OriginalValue).FirstOrDefault(), 

                    PECurrentYearScrubbed   = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.ScrubbedValue).FirstOrDefault(), //PE Current Year Scrubbed


                    PENextYear              = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.OriginalValue).FirstOrDefault(), //PE Next year
                                              

                    PENextYearScrubbed      = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 166 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.ScrubbedValue).FirstOrDefault(), //PE Next year Scrubbed

                    PBVCurrentYear          = 
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.OriginalValue).FirstOrDefault(), //PBV Current Year

                    PBVCurrentYearScrubbed  = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.ScrubbedValue).FirstOrDefault(), //PBV Current Year Scrubbed

                    PBVNextYear = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.OriginalValue).FirstOrDefault(), //PBV Next Year

                    PBVNextYearScrubbed    = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                             securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.OriginalValue).FirstOrDefault() :
                                             securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 164 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.ScrubbedValue).FirstOrDefault(), //PBV Next Year Scrubbed

                    EB_EBITDA_CurrentYear   =
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.OriginalValue).FirstOrDefault(),//EV/ EBITDA Current Year
                                              

                    EB_EBITDA_CurrentYearScrubbed = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year)).Select(a => a.ScrubbedValue).FirstOrDefault(), //EV/ EBITDA Current Year scrubbed

                    EB_EBITDA_NextYear      =
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.OriginalValue).FirstOrDefault(), //EV/EBITDA Next Year
                                              

                    EB_EBITDA_NextYearScrubbed = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                                                  securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.OriginalValue).FirstOrDefault() :
                                                                  securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 193 && a.PeriodYear == (DateTime.Today.Year + 1)).Select(a => a.ScrubbedValue).FirstOrDefault(), //EV/EBITDA Next Year Scrubbed

                    DividendYield           = 
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 192).Select(a => a.OriginalValue).FirstOrDefault() , //Div Yield

                    DividendYieldScrubbed   = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 192).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 192).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 192).Select(a => a.ScrubbedValue).FirstOrDefault(), //Div Yield Scrubbed


                    ROE                     = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 133).Select(a => a.OriginalValue).FirstOrDefault() ,
                                              //ROE
                    ROEScrubbed            = securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 133).Select(a => a.ScrubbedValue).FirstOrDefault() == null ?
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 133).Select(a => a.OriginalValue).FirstOrDefault() :
                                              securityDataIdScrub.Where(a => a.SecurityId == security && a.DataId == 133).Select(a => a.ScrubbedValue).FirstOrDefault() //ROE Scrubbed

                    
                });
            }

            return investmentcontextDetailList;
        }


        private List<InvestmentContextDetailsData> getTotalLine(List<InvestmentContextDetailsData> icdList,String context)
        {
            //take sum/average for the total line
            List<InvestmentContextDetailsData> allTotalLine = new List<InvestmentContextDetailsData>();
            InvestmentContextDetailsData averageLine = new InvestmentContextDetailsData()
            {
                GicsSectorCode = null,
                GicsSectorName =context+" Average",
                MarketValue = icdList.Sum(x => x.MarketValueScrubbed),
                MarketCap = icdList.Sum(x => x.MarketCapScrubbed),
                ForwardPE = GroupCalculations.SimpleAverage(icdList.Select(x => x.ForwardPEScrubbed).ToList()),
                ForwardPBV = GroupCalculations.SimpleAverage(icdList.Select(x => x.ForwardPBVScrubbed).ToList()),
                PECurrentYear = GroupCalculations.SimpleAverage(icdList.Select(x => x.PECurrentYearScrubbed).ToList()),
                PENextYear = GroupCalculations.SimpleAverage(icdList.Select(x => x.PENextYearScrubbed).ToList()),
                PBVCurrentYear = GroupCalculations.SimpleAverage(icdList.Select(x => x.PBVCurrentYearScrubbed).ToList()),
                PBVNextYear = GroupCalculations.SimpleAverage(icdList.Select(x => x.PBVNextYearScrubbed).ToList()),
                EB_EBITDA_CurrentYear = GroupCalculations.SimpleAverage(icdList.Select(x => x.EB_EBITDA_CurrentYearScrubbed).ToList()),
                EB_EBITDA_NextYear = GroupCalculations.SimpleAverage(icdList.Select(x => x.EB_EBITDA_NextYearScrubbed).ToList()),
                DividendYield = GroupCalculations.SimpleAverage(icdList.Select(x => x.DividendYieldScrubbed).ToList()),
                ROE = GroupCalculations.SimpleAverage(icdList.Select(x => x.ROEScrubbed).ToList()),
                children = icdList
            };
            allTotalLine.Add(averageLine);
            InvestmentContextDetailsData icdMedianLine = new InvestmentContextDetailsData()
            {
                GicsSectorCode = null,
                GicsSectorName = context + " Median",
                MarketValue = null,
                MarketCap = null,
                ForwardPE = GroupCalculations.Median(icdList.Where(x => x.ForwardPEScrubbed.HasValue).Select(x => x.ForwardPEScrubbed).ToList()),
                ForwardPBV = GroupCalculations.Median(icdList.Where(x => x.ForwardPBVScrubbed.HasValue).Select(x => x.ForwardPBVScrubbed).ToList()),
                PECurrentYear = GroupCalculations.Median(icdList.Where(x => x.PECurrentYearScrubbed.HasValue).Select(x => x.PECurrentYearScrubbed).ToList()),
                PENextYear = GroupCalculations.Median(icdList.Where(x => x.PENextYearScrubbed.HasValue).Select(x => x.PENextYearScrubbed).ToList()),
                PBVCurrentYear = GroupCalculations.Median(icdList.Where(x => x.PBVCurrentYearScrubbed.HasValue).Select(x => x.PBVCurrentYearScrubbed).ToList()),
                PBVNextYear = GroupCalculations.Median(icdList.Where(x => x.PBVNextYearScrubbed.HasValue).Select(x => x.PBVNextYearScrubbed).ToList()),
                EB_EBITDA_CurrentYear = GroupCalculations.Median(icdList.Where(x=>x.EB_EBITDA_CurrentYearScrubbed.HasValue).Select(x => x.EB_EBITDA_CurrentYearScrubbed).ToList()),
                EB_EBITDA_NextYear = GroupCalculations.Median(icdList.Where(x => x.EB_EBITDA_NextYearScrubbed.HasValue).Select(x => x.EB_EBITDA_NextYearScrubbed).ToList()),
                DividendYield = GroupCalculations.Median(icdList.Where(x => x.DividendYieldScrubbed.HasValue).Select(x => x.DividendYieldScrubbed).ToList()),
                ROE = GroupCalculations.Median(icdList.Where(x => x.ROEScrubbed.HasValue).Select(x => x.ROEScrubbed).ToList()),
                children = null
            };
             allTotalLine.Add(icdMedianLine);

             InvestmentContextDetailsData icdWeightedAverageLine = new InvestmentContextDetailsData()
             {
                 GicsSectorCode = null,
                 GicsSectorName = context + " Weighted Average",
                 MarketValue = null,
                 MarketCap = null,
                 ForwardPE = GroupCalculations.WeightedAverage(icdList.Where(x => x.ForwardPEScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.ForwardPEScrubbed.HasValue).Select(x => x.ForwardPEScrubbed).ToList(), icdList.Where(x => x.ForwardPEScrubbed.HasValue).Sum(x => x.MarketCap)),
                 ForwardPBV = GroupCalculations.WeightedAverage(icdList.Where(x => x.ForwardPBVScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.ForwardPBVScrubbed.HasValue).Select(x => x.ForwardPBVScrubbed).ToList(), icdList.Where(x => x.ForwardPBVScrubbed.HasValue).Sum(x => x.MarketCap)),
                 PECurrentYear = GroupCalculations.WeightedAverage(icdList.Where(x => x.PECurrentYearScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.PECurrentYearScrubbed.HasValue).Select(x => x.PECurrentYearScrubbed).ToList(), icdList.Where(x => x.PECurrentYearScrubbed.HasValue).Sum(x => x.MarketCap)),
                 PENextYear = GroupCalculations.WeightedAverage(icdList.Where(x => x.PENextYearScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.PENextYearScrubbed.HasValue).Select(x => x.PENextYearScrubbed).ToList(), icdList.Where(x => x.PENextYearScrubbed.HasValue).Sum(x => x.MarketCap)),
                 PBVCurrentYear = GroupCalculations.WeightedAverage(icdList.Where(x => x.PBVCurrentYearScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.PBVCurrentYearScrubbed.HasValue).Select(x => x.PBVCurrentYearScrubbed).ToList(), icdList.Where(x => x.PBVCurrentYearScrubbed.HasValue).Sum(x => x.MarketCap)),
                 PBVNextYear = GroupCalculations.WeightedAverage(icdList.Where(x => x.PBVNextYearScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.PBVNextYearScrubbed.HasValue).Select(x => x.PBVNextYearScrubbed).ToList(), icdList.Where(x => x.PBVNextYearScrubbed.HasValue).Sum(x => x.MarketCap)),
                 EB_EBITDA_CurrentYear = GroupCalculations.WeightedAverage(icdList.Where(x => x.EB_EBITDA_CurrentYearScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.EB_EBITDA_CurrentYearScrubbed.HasValue).Select(x => x.EB_EBITDA_CurrentYearScrubbed).ToList(), icdList.Where(x => x.EB_EBITDA_CurrentYearScrubbed.HasValue).Sum(x => x.MarketCap)),
                 EB_EBITDA_NextYear = GroupCalculations.WeightedAverage(icdList.Where(x => x.EB_EBITDA_NextYearScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.EB_EBITDA_NextYearScrubbed.HasValue).Select(x => x.EB_EBITDA_NextYearScrubbed).ToList(), icdList.Where(x => x.EB_EBITDA_NextYearScrubbed.HasValue).Sum(x => x.MarketCap)),
                 DividendYield = GroupCalculations.WeightedAverage(icdList.Where(x => x.DividendYieldScrubbed.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.DividendYieldScrubbed.HasValue).Select(x => x.DividendYieldScrubbed).ToList(), icdList.Where(x => x.DividendYieldScrubbed.HasValue).Sum(x => x.MarketCap)),
                 ROE = GroupCalculations.WeightedAverage(icdList.Where(x => x.ROEScrubbed.HasValue && x.MarketCap.HasValue).Select(x => x.MarketCap).ToList(), icdList.Where(x => x.ROEScrubbed.HasValue && x.MarketCap.HasValue).Select(x => x.ROEScrubbed).ToList(), icdList.Where(x => x.ROEScrubbed.HasValue).Sum(x => x.MarketCap)),
                 children = null
             };
             allTotalLine.Add(icdWeightedAverageLine);
            return allTotalLine;
        }


        private List<InvestmentContextDetailsData> GroupBySector(List<InvestmentContextDetailsData> finalICD)
        { //take the sum/average for the group headers
            var result = new List<InvestmentContextDetailsData>();
            var query = from d in finalICD[0].children
                        group d by d.GicsSectorCode into grp
                        select grp;
            var groups = query.ToList();
            foreach (var group in groups)
            {
                var main = group.Where(x => x.GicsSectorCode == group.Key).FirstOrDefault();
                if (group.Count() > 1)
                {
                    var icd = new InvestmentContextDetailsData
                    {
                        GicsSectorCode= main.GicsSectorCode,
                        GicsSectorName= main.GicsSectorName,
                        MarketValue = group.Sum(x => x.MarketValueScrubbed),
                        MarketCap = group.Sum(x => x.MarketCapScrubbed) ,
                        ForwardPE = GroupCalculations.SimpleAverage(group.Select(x => x.ForwardPEScrubbed).ToList()),
                        ForwardPBV = GroupCalculations.SimpleAverage(group.Select(x => x.ForwardPBVScrubbed).ToList()),
                        PECurrentYear = GroupCalculations.SimpleAverage(group.Select(x => x.PECurrentYearScrubbed).ToList()),
                        PENextYear = GroupCalculations.SimpleAverage(group.Select(x => x.PENextYearScrubbed).ToList()),
                        PBVCurrentYear = GroupCalculations.SimpleAverage(group.Select(x => x.PBVCurrentYearScrubbed).ToList()),
                        PBVNextYear = GroupCalculations.SimpleAverage(group.Select(x => x.PBVNextYearScrubbed).ToList()),
                        EB_EBITDA_CurrentYear = GroupCalculations.SimpleAverage(group.Select(x => x.EB_EBITDA_CurrentYearScrubbed).ToList()),
                        EB_EBITDA_NextYear = GroupCalculations.SimpleAverage(group.Select(x => x.EB_EBITDA_NextYearScrubbed).ToList()),
                        DividendYield = GroupCalculations.SimpleAverage(group.Select(x => x.DividendYieldScrubbed).ToList()),
                        ROE = GroupCalculations.SimpleAverage(group.Select(x => x.ROEScrubbed).ToList()),
                        children=group.ToList()
                    };
                    result.Add(icd);
                }
            }
            finalICD[0].children = result;
            return finalICD;

        }

        private List<string> getListOfIssuersToDisplay(List<InvestmentContextDetailsData> icdList,string issuerId)
        {
            List<string> issuerList = new List<string>();
            // Get all the issuer that has the market value from the composite
            issuerList = icdList.Where(x => x.MarketValue.HasValue && x.IssuerId != issuerId).Select(x => x.IssuerId).ToList();
            issuerList.Add(issuerId);
            
            //since we will be displaying only 100 issuers . Subtract the remaining issuers that you want to display. if there are 40 issuers that has the market value then remaining issuers that you want to display is 60
            int remainingIssuerCount = 100 - issuerList.Count();
            //handle for odd numbers. its ok to display 101 issuers
            if (remainingIssuerCount / 2 != 0)
            {
                remainingIssuerCount = remainingIssuerCount + 1;
            }

            //get the market cap of the selected issuer
            decimal? mktcap = icdList.Where(x => x.IssuerId == issuerId).Select(x => x.MarketCapScrubbed).FirstOrDefault();
            
            int upperRange =0;
            int lowerRange =0;
            upperRange = remainingIssuerCount/2; // get the upper range . 
            lowerRange = remainingIssuerCount/2; //get the lower range
            //get the top n(upper range) of issuers whose market cap is greater than the selected issuer's market cap
            List<string> issHigherMktCap = icdList.Where(x => x.MarketCapScrubbed >= mktcap && !issuerList.Contains(x.IssuerId)).OrderByDescending(x=>x.MarketCapScrubbed).Take(upperRange).Select(x=>x.IssuerId).ToList();
            issuerList.AddRange(issHigherMktCap);

            //if the number of issuers returned are less than the upperrange then the retrieve the remaining issuers whose market cap is less than the selected issuers mkt cap.
            if (issHigherMktCap.Count() < upperRange)
            {
                lowerRange = lowerRange + (upperRange - issHigherMktCap.Count());
            }


            List<string> issLowerMktCap = icdList.Where(x => x.MarketCapScrubbed < mktcap && !issuerList.Contains(x.IssuerId)).OrderByDescending(x => x.MarketCapScrubbed).Take(lowerRange).Select(x => x.IssuerId).ToList();
            issuerList.AddRange(issLowerMktCap);

            //if the number of issuers returned are less than the lowerrange then try to the retrieve the remaining issuers whose market cap is greater than the selected issuers mkt cap.
            if (issLowerMktCap.Count() < lowerRange)
            {
                upperRange = lowerRange - issHigherMktCap.Count();
                issHigherMktCap = icdList.Where(x => x.MarketCapScrubbed >= mktcap && !issuerList.Contains(x.IssuerId)).OrderByDescending(x => x.MarketCapScrubbed).Take(upperRange).Select(x => x.IssuerId).ToList();
                issuerList.AddRange(issHigherMktCap);
            }

           // issuerList.AddRange(issLowerMktCap);

            return issuerList;
           
        }


        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DATA_MASTER> RetrieveDataMaster()
        {
            ExternalResearchEntities entity = new ExternalResearchEntities();
            List<DATA_MASTER> DataMasterList = entity.DATA_MASTER.ToList();
            return DataMasterList;
        }

        #endregion



    }
}
