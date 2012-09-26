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
        /// <param name="issuerId"></param>
        /// <param name="periodType"></param>
        /// <param name="currency"></param>
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

                if (data == null)
                    return result;
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

                    if (previousYearQuarterAmount == null || previousYearQuarterAmount == 0)
                        temp.YOYGrowth = 0;
                    else
                        temp.YOYGrowth = (temp.YOYGrowth / previousYearQuarterAmount - 1) * 100;
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
        /// <param name="issuerId"></param>
        /// <param name="periodType"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
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
                    return result;

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
        /// Get data for Finstat Gadget
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="securityId"></param>
        /// <param name="dataSource"></param>
        /// <param name="fiscalType"></param>
        /// <param name="currency"></param>
        /// <param name="yearRange"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FinstatDetailData> RetrieveFinstatData(string issuerId, string securityId, FinancialStatementDataSource dataSource, FinancialStatementFiscalType fiscalType, String currency, Int32 yearRangeStart)
        {
            try
            {
                string _dataSource = EnumUtils.ToString(dataSource);
                string _fiscalType = EnumUtils.ToString(fiscalType);

                ExternalResearchEntities entity = new ExternalResearchEntities();

                List<FinstatDetail> data = new List<FinstatDetail>();
                List<FinstatDetailData> result = new List<FinstatDetailData>();

                data = entity.GetFinstatDetail(issuerId, securityId, _dataSource, _fiscalType, currency).ToList();
                if (data == null || data.Count() == 0)
                    return result;

                #region DataSource group
                List<int> distinctPeriodYear = data.Select(a => a.PERIOD_YEAR).Distinct().ToList();
                List<FinstatDetail> distinctRootSource = data.Where(a => a.ROOT_SOURCE != null).OrderBy(a => a.PERIOD_YEAR).ThenBy(a => a.DATA_SOURCE).ToList();


                foreach (int item in distinctPeriodYear)
                {
                    FinstatDetailData temp = new FinstatDetailData();
                    temp.GroupDescription = "Data Source";
                    temp.Description = "Source";
                    temp.PeriodType = "A";
                    temp.Amount = _dataSource;
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
                    tempData.Amount = (isRootSourceMixed.Count > 1) ? "MIXED" : _dataSource;
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
                        decimal? year1 = 0, year2 = 0, year3 = 0, year4 = 0, year5 = 0, year6 = 0;

                        decimal year1Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR - 3
                                                                                && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year1 = (year1Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year1Value));

                        decimal year2Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR - 2
                                                                                && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year2 = (year2Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year2Value));

                        decimal year3Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR - 1
                                                                               && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year3 = (year3Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year3Value));

                        decimal year4Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR
                                                                               && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year4 = (year4Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year4Value));
                        decimal year5Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 1
                                                                               && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year5 = (year5Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year5Value));
                        decimal year6Value = Convert.ToDecimal(data.Where(a => a.PERIOD_YEAR == data[i].PERIOD_YEAR + 2
                                                                               && a.DATA_DESC == data[i].DATA_DESC && a.GROUP_NAME == data[i].GROUP_NAME).Select(a => a.AMOUNT).FirstOrDefault());
                        year6 = (year6Value == 0) ? 0 : (Convert.ToDecimal(1.0 / 3.0) * ((decimal)1 / year6Value));

                        if (year1 != 0 && year2 != 0 && year3 != 0 && year1 != null && year2 != null && year3 != null)
                            temp.HarmonicFirst = Convert.ToDecimal((1 / (year1 + year2 + year3)) * data[i].MULTIPLIER);
                        if (year4 != 0 && year5 != 0 && year6 != 0 && year4 != null && year5 != null && year6 != null)
                            temp.HarmonicSecond = Convert.ToDecimal((1 / (year4 + year5 + year6)) * data[i].MULTIPLIER);
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
                    FinstatDetailData tempData = new FinstatDetailData();
                    tempData.GroupDescription = "Economic & Market Data";
                    tempData.Description = Convert.ToString(item.FIELD);
                    tempData.PeriodYear = Convert.ToInt32(item.YEAR1);
                    tempData.AmountType = "A";
                    tempData.PeriodType = "A";
                    tempData.BoldFont = "N";
                    tempData.IsPercentage = "Y";
                    tempData.RootSource = _dataSource;
                    tempData.RootSourceDate = DateTime.Now;
                    tempData.Amount = Math.Round((Convert.ToDecimal(item.VALUE) * 100), 1);
                    result.Add(tempData);
                }
                #endregion

                #region Relative Analysis Data
                List<FinstatRelativeAnalysisData> relativeData = entity.GetFinstatRelativeAnalysisData(issuerId, securityId, _dataSource, _fiscalType).ToList();

                #region direct data
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
                    //tempData.Multiplier = item.MULTIPLIER;
                    tempData.IsPercentage = item.PERCENTAGE;
                    if (item.VALUE == "step1")
                    {
                        tempData.BoldFont = "Y";
                        tempData.Description = item.DATA_ID == 44 ? "Net Income" :
                                                   item.DATA_ID == 166 ? "P/E" : item.DATA_ID == 164 ? "P/BV" : item.DATA_ID == 133 ? "ROE" : "";
                    }
                    else if (item.VALUE == "step2")
                    {
                        tempData.BoldFont = "N";
                        tempData.Description = item.DATA_ID == 11 ? "Consensus Net Income" :
                                                   item.DATA_ID == 166 ? "Consensus P/E" : item.DATA_ID == 164 ? "Consensus P/BV" : item.DATA_ID == 19 ? "Consensus ROE" : "";
                    }
                    else if (item.VALUE == "step3")
                    {
                        tempData.BoldFont = "N";
                        tempData.Description = item.DATA_ID == 166 ? "Country P/E" :
                                                   item.DATA_ID == 164 ? "Country P/BV" : item.DATA_ID == 133 ? "Country ROE" : "";
                    }
                    else if (item.VALUE == "step5")
                    {
                        tempData.BoldFont = "N";
                        tempData.Description = item.DATA_ID == 166 ? "Industry P/E" :
                                                   item.DATA_ID == 164 ? "Industry P/BV" : item.DATA_ID == 133 ? "Industry ROE" : "";
                    }
                    result.Add(tempData);
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
                            decimal countryPE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step3" && a.DATA_ID == 166).Select(a => a.AMOUNT).FirstOrDefault());
                            if (countryPE != 0)
                                record.Amount = Math.Round((item.AMOUNT / countryPE), 2);
                            break;
                        case 164:
                            record.Description = "Relative Country P/BV";
                            decimal countryPBV = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step3" && a.DATA_ID == 164).Select(a => a.AMOUNT).FirstOrDefault());
                            if (countryPBV != 0)
                                record.Amount = Math.Round((item.AMOUNT / countryPBV), 2);
                            break;
                        case 133:
                            record.Description = "Relative Country ROE";
                            decimal countryROE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step3" && a.DATA_ID == 133).Select(a => a.AMOUNT).FirstOrDefault());
                            if (countryROE != 0)
                                record.Amount = Math.Round((item.AMOUNT / countryROE), 2);
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
                            decimal industryPE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step5" && a.DATA_ID == 166).Select(a => a.AMOUNT).FirstOrDefault());
                            if (industryPE != 0)
                                record.Amount = Math.Round((item.AMOUNT / industryPE), 2);
                            break;
                        case 164:
                            record.Description = "Relative Industry P/BV";
                            decimal industryPBV = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step5" && a.DATA_ID == 164).Select(a => a.AMOUNT).FirstOrDefault());
                            if (industryPBV != 0)
                                record.Amount = Math.Round((item.AMOUNT / industryPBV), 2);
                            break;
                        case 133:
                            record.Description = "Relative Industry ROE";
                            decimal industryROE = Convert.ToDecimal(relativeData.Where(a => a.VALUE == "step5" && a.DATA_ID == 133).Select(a => a.AMOUNT).FirstOrDefault());
                            if (industryROE != 0)
                                record.Amount = Math.Round((item.AMOUNT / industryROE), 2);
                            break;
                        default:
                            break;
                    }
                }

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
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);
                decimal previousYearQuarterAmount;
                ExternalResearchEntities entity = new ExternalResearchEntities();

                data = entity.GetConsensusEstimateData(issuerId, "REUTERS", _periodType, "FISCAL", currency).ToList();
                List<int> dataDesc = new List<int>() { 17, 7, 11, 8, 18, 19 };

                if (data == null)
                    return new List<ConsensusEstimateMedian>();
                if (data.Count == 0)
                    return new List<ConsensusEstimateMedian>();

                data = data.OrderBy(record => record.ESTIMATE_DESC).ThenByDescending(record => record.PERIOD_YEAR).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    if (dataDesc.Contains(data[i].ESTIMATE_ID))
                    {
                        ConsensusEstimateMedian temp = new ConsensusEstimateMedian();

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

                        if (data[i].ESTIMATE_ID == 18 || data[i].ESTIMATE_ID == 19)
                            temp.Actual = Convert.ToString(Math.Round(Convert.ToDecimal(data[i].ACTUAL), 2)) + "%";
                        else
                            temp.Actual = Convert.ToString(Math.Round(Convert.ToDecimal(data[i].ACTUAL), 2));

                        temp.YOYGrowth = data[i].AMOUNT;
                        temp.Variance = data[i].AMOUNT == 0 ? null : ((data[i].ASHMOREEMM_AMOUNT / data[i].AMOUNT) - 1) * 100;

                        previousYearQuarterAmount = data.Where(a => a.ESTIMATE_DESC == data[i].ESTIMATE_DESC && a.PERIOD_YEAR == (data[i].PERIOD_YEAR - 1)
                            && a.PERIOD_TYPE == data[i].PERIOD_TYPE).Select(a => a.AMOUNT).FirstOrDefault();

                        if (previousYearQuarterAmount == null || previousYearQuarterAmount == 0)
                            temp.YOYGrowth = 0;
                        else
                            temp.YOYGrowth = (temp.YOYGrowth / previousYearQuarterAmount - 1) * 100;

                        result.Add(temp);
                    }
                }
                //if (result != null)
                //    result = ConsensusEstimateCalculations.CalculateEPSValues(result, periodType);

                //if (result != null)
                //    result = ConsensusEstimateCalculations.CalculateNetIncomeValues(result, periodType);

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
                string _periodType = EnumUtils.ToString(periodType).Substring(0, 1);
                decimal previousYearQuarterAmount;
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

                data = entity.GetConsensusEstimatesValuation(issuerId, "REUTERS", _periodType, "FISCAL", currency, null, null, securityId).ToList();
                List<int> dataDesc = new List<int>() { 166, 170, 171, 164, 192, 172 };

                if (data == null)
                    return new List<ConsensusEstimatesValuations>();
                if (data.Count == 0)
                    return new List<ConsensusEstimatesValuations>();

                data = data.OrderBy(record => record.ESTIMATE_DESC).ThenByDescending(record => record.PERIOD_YEAR).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    if (dataDesc.Contains(data[i].ESTIMATE_ID))
                    {
                        ConsensusEstimatesValuations temp = new ConsensusEstimatesValuations();

                        if (data[i].ESTIMATE_ID == 170)
                            temp.SortOrder = 1;
                        else if (data[i].ESTIMATE_ID == 171)
                            temp.SortOrder = 2;
                        else if (data[i].ESTIMATE_ID == 166)
                            temp.SortOrder = 3;
                        else if (data[i].ESTIMATE_ID == 172)
                            temp.SortOrder = 4;
                        else if (data[i].ESTIMATE_ID == 164)
                            temp.SortOrder = 5;
                        else if (data[i].ESTIMATE_ID == 192)
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

                        temp.YOYGrowth = data[i].AMOUNT;
                        temp.Variance = data[i].AMOUNT == 0 ? null : ((data[i].ASHMOREEMM_AMOUNT / data[i].AMOUNT) - 1) * 100;

                        previousYearQuarterAmount = data.Where(a => a.ESTIMATE_DESC == data[i].ESTIMATE_DESC && a.PERIOD_YEAR == (data[i].PERIOD_YEAR - 1)
                                && a.PERIOD_TYPE == data[i].PERIOD_TYPE).Select(a => a.AMOUNT).FirstOrDefault();

                        if (previousYearQuarterAmount == null || previousYearQuarterAmount == 0)
                            temp.YOYGrowth = 0;
                        else
                            temp.YOYGrowth = (temp.YOYGrowth / previousYearQuarterAmount - 1) * 100;

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
        public List<PRevenueData> RetrievePRevenueData(EntitySelectionData entitySelectionData, string chartTitle)
        {
            try
            {
                List<PRevenueData> result = new List<PRevenueData>();
                List<GetPRevenueData_Result> resultDB = new List<GetPRevenueData_Result>();
                List<GetEV_EBITDAData_Result> resultDB_EV_EBITDA = new List<GetEV_EBITDAData_Result>();
                ExternalResearchEntities extResearch = new ExternalResearchEntities();

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

                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities research = new ExternalResearchEntities();
                result = research.GetDataForPeriodGadgets(_dataSource, _fiscalType, cCurrency, issuerId, securityId.ToString()).ToList();
                foreach (GreenField.DAL.COASpecificData item in result)
                {
                    GreenField.DataContracts.COASpecificData entry = new GreenField.DataContracts.COASpecificData();
                    entry.Amount = item.Amount;
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
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData> RetrieveValuationGrowthData(PortfolioSelectionData selectedPortfolio, DateTime? effectiveDate, String filterType, String filterValue, bool lookThruEnabled)
        {
            try
            {
                if (selectedPortfolio == null || effectiveDate == null || filterType == null || filterValue == null)
                    return new List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData>();
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
                //List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> dataBenchmark = new List<GF_BENCHMARK_HOLDINGS>();


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
                    throw new Exception("Services are not available");


                List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData> result = new List<GreenField.DataContracts.DataContracts.ValuationQualityGrowthData>();
                List<GreenField.DAL.ValuationQualityGrowthData> storedProcResult = new List<GreenField.DAL.ValuationQualityGrowthData>();

                if (lookThruEnabled == false)
                {
                    #region Look Thru Disabled
                    switch (filterType)
                    {
                        case "Region":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ASHEMM_PROP_REGION_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ASHEMM_PROP_REGION_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Country":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ISO_COUNTRY_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ISO_COUNTRY_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Industry":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_INDUSTRY_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_INDUSTRY_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Sector":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_SECTOR_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_SECTOR_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Show Everything":
                            {
                                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolio != null && dataPortfolio.Count > 0)
                                {
                                    benchmarkId = dataPortfolio.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.BENCHMARK_WEIGHT > 0)
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
                    storedProcResult = research.usp_GetDataForValuationQualityGrowth(_issuerIDPortfolio, _securityIDPortfolio, _issuerIDBenchmark, _securityIDBenchmark).ToList();


                    #region Calculating values for all Data Id's for Portfolio View and Benchmark View
                    //******************Combining Portfolio weight and Benchmark weight for all securities from Portfolio View,Benchmark View and Amount from Data Base****************


                    List<String> dinstinctIssuerIdsForPortfolio = new List<String>();
                    List<String> dinstinctSecurityIdsForPortfolio = new List<String>();
                    List<String> dinstinctIssuerIdsForBenchmark = new List<String>();
                    List<String> dinstinctSecurityIdsForBenchmark = new List<String>();
                    List<String> distinctSecurityNamesForPortfolio = new List<String>();
                    List<String> distinctSecurityNamesForBenchmark = new List<String>();


                    if (storedProcResult != null && storedProcResult.Count() > 0)
                    {

                        foreach (int dataId in storedProcResult.Select(t => t.DataId).Distinct())
                        {
                            dinstinctIssuerIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio").Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio").Select(t => t.SecurityId).Distinct().ToList();
                            dinstinctIssuerIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark").Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark").Select(t => t.SecurityId).Distinct().ToList();
                            foreach (String secId in dinstinctSecurityIdsForPortfolio)
                            {
                                if (listForPortfolio.ContainsKey(secId))
                                    distinctSecurityNamesForPortfolio.Add(listForPortfolio[secId]);
                            }

                            foreach (String secId in dinstinctSecurityIdsForBenchmark)
                            {
                                if (listForBenchmark.ContainsKey(secId))
                                    distinctSecurityNamesForBenchmark.Add(listForBenchmark[secId]);
                            }

                            foreach (String s in dinstinctIssuerIdsForPortfolio)
                            {
                                foreach (GF_PORTFOLIO_HOLDINGS row in dataPortfolio.Where(t => t.ISSUER_ID == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.IssuerId = s;
                                    objPort.SecurityId = listForPortfolio.FirstOrDefault(t => t.Value == row.ISSUE_NAME).Key;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForPortfolio.FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForBenchmark.FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ASHEMM_PROP_REGION_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ASHEMM_PROP_REGION_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Country":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ISO_COUNTRY_CODE == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.ISO_COUNTRY_CODE == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Industry":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_INDUSTRY_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_INDUSTRY_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Sector":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_SECTOR_NAME == filterValue && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.GICS_SECTOR_NAME == filterValue && t.BENCHMARK_WEIGHT > 0)
                                   .ToList();
                                }
                            }
                            break;

                        case "Show Everything":
                            {
                                dataPortfolioLookThruDis = entity.GF_PORTFOLIO_LTHOLDINGS
                                .Where(t => t.PORTFOLIO_ID == selectedPortfolio.PortfolioId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.DIRTY_VALUE_PC > 0)
                                .ToList();
                                if (dataPortfolioLookThruDis != null && dataPortfolioLookThruDis.Count > 0)
                                {
                                    benchmarkId = dataPortfolioLookThruDis.FirstOrDefault().BENCHMARK_ID.ToString();
                                    dataBenchmark = entity.GF_BENCHMARK_HOLDINGS
                                   .Where(t => t.BENCHMARK_ID == benchmarkId && t.PORTFOLIO_DATE == effectiveDate.Value.Date && t.BENCHMARK_WEIGHT > 0)
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
                    storedProcResult = research.usp_GetDataForValuationQualityGrowth(_issuerIDPortfolio, _securityIDPortfolio, _issuerIDBenchmark, _securityIDBenchmark).ToList();


                    #region Calculating values for all Data Id's for Portfolio View and Benchmark View
                    //******************Combining Portfolio weight and Benchmark weight for all securities from Portfolio View,Benchmark View and Amount from Data Base****************
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
                            dinstinctIssuerIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio").Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForPortfolio = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Portfolio").Select(t => t.SecurityId).Distinct().ToList();
                            dinstinctIssuerIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark").Select(t => t.IssuerId).Distinct().ToList();
                            dinstinctSecurityIdsForBenchmark = storedProcResult.Where(t => t.DataId == dataId && t.AmountType == "Benchmark").Select(t => t.SecurityId).Distinct().ToList();
                            foreach (String secId in dinstinctSecurityIdsForPortfolio)
                            {
                                if (listForPortfolio.ContainsKey(secId))
                                    distinctSecurityNamesForPortfolio.Add(listForPortfolio[secId]);
                            }

                            foreach (String secId in dinstinctSecurityIdsForBenchmark)
                            {
                                if (listForBenchmark.ContainsKey(secId))
                                    distinctSecurityNamesForBenchmark.Add(listForBenchmark[secId]);
                            }

                            foreach (String s in dinstinctIssuerIdsForPortfolio)
                            {
                                foreach (GF_PORTFOLIO_LTHOLDINGS row in dataPortfolioLookThruDis.Where(t => t.ISSUER_ID == s).ToList())
                                {
                                    CalculatedValuesForValuation objPort = new CalculatedValuesForValuation();
                                    objPort.IssuerId = s;
                                    objPort.SecurityId = listForPortfolio.FirstOrDefault(t => t.Value == row.ISSUE_NAME).Key;
                                    objPort.DataId = dataId;
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForPortfolio.FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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
                                    objPort.Amount = storedProcResult.Where(t => t.IssuerId == s && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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
                                    objPort.Amount = storedProcResult.Where(t => t.SecurityId == (listForBenchmark.FirstOrDefault(m => m.Value == s).Key) && t.DataId == dataId).Select(t => t.Amount).FirstOrDefault();
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

        private void CalculateHarmonicMeanPortfolio(List<CalculatedValuesForValuation> filteredByDataIdList, String description, ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? initialSumDirtyValuePC = 0, Decimal? harmonicMeanPortfolio = 0)
        {
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumDirtyValuePC = initialSumDirtyValuePC + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumDirtyValuePC != 0)
                    row.PortfolioPercent = (row.PortfolioPercent / initialSumDirtyValuePC);
                if (row.Amount != 0)
                {
                    row.InverseAmount = 1 / row.Amount;
                }
                row.MultipliedValue = row.PortfolioPercent * row.InverseAmount;
                harmonicMeanPortfolio = harmonicMeanPortfolio + row.MultipliedValue;
            }



            entry.Description = description;
            if (harmonicMeanPortfolio != 0)
                entry.Portfolio = 1 / harmonicMeanPortfolio;
        }


        private void CalculateHarmonicMeanBenchmark(List<CalculatedValuesForValuation> filteredByDataIdList, ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? harmonicMeanBenchmark = 0)
        {

            Decimal? initialSumBenchmarkWeight = 0;
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumBenchmarkWeight = initialSumBenchmarkWeight + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumBenchmarkWeight != 0)
                    row.PortfolioPercent = (row.PortfolioPercent / initialSumBenchmarkWeight);
                if (row.Amount != 0)
                {
                    row.InverseAmount = 1 / row.Amount;
                }
                row.MultipliedValue = row.PortfolioPercent * row.InverseAmount;
                harmonicMeanBenchmark = harmonicMeanBenchmark + row.MultipliedValue;
            }


            if (harmonicMeanBenchmark != 0)
                entry.Benchmark = 1 / harmonicMeanBenchmark;
            if (entry.Benchmark != 0)
                entry.Relative = entry.Portfolio / entry.Benchmark;
        }


        private void CalculateHarmonicMeanPortfolioROE(List<CalculatedValuesForValuation> filteredByDataIdList, String description, ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? initialSumDirtyValuePC = 0, Decimal? harmonicMeanPortfolio = 0)
        {
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumDirtyValuePC = initialSumDirtyValuePC + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumDirtyValuePC != 0)
                    row.PortfolioPercent = (row.PortfolioPercent / initialSumDirtyValuePC);
                if (row.Amount != 0)
                {
                    row.InverseAmount = 1 / row.Amount;
                }
                row.MultipliedValue = row.PortfolioPercent * row.Amount;
                harmonicMeanPortfolio = harmonicMeanPortfolio + row.MultipliedValue;
            }

            entry.Description = description;
            entry.Portfolio = harmonicMeanPortfolio * 100;
        }


        private void CalculateHarmonicMeanBenchmarkROE(List<CalculatedValuesForValuation> filteredByDataIdList, ref GreenField.DataContracts.DataContracts.ValuationQualityGrowthData entry, Decimal? harmonicMeanBenchmark = 0)
        {

            Decimal? initialSumBenchmarkWeight = 0;
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                initialSumBenchmarkWeight = initialSumBenchmarkWeight + row.PortfolioPercent;
            }
            foreach (CalculatedValuesForValuation row in filteredByDataIdList)
            {
                if (initialSumBenchmarkWeight != 0)
                    row.PortfolioPercent = (row.PortfolioPercent / initialSumBenchmarkWeight);
                if (row.Amount != 0)
                {
                    row.InverseAmount = 1 / row.Amount;
                }
                row.MultipliedValue = row.PortfolioPercent * row.Amount;
                harmonicMeanBenchmark = harmonicMeanBenchmark + row.MultipliedValue;
            }


            entry.Benchmark = harmonicMeanBenchmark * 100;
            if (entry.Benchmark != 0)
                entry.Relative = entry.Portfolio / entry.Benchmark;
        }
        #endregion

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<tblCompanyInfo> RetrieveCompanyData()
        {
            try
            {
                ReutersEntities entity = new ReutersEntities();
                return entity.tblCompanyInfoes.ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        ///// <summary>
        ///// Get data for Consensus Estimate Detailed gadget
        ///// </summary>
        ///// <param name="issuerId"></param>
        ///// <param name="periodType"></param>
        ///// <param name="currency"></param>
        ///// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<SecurityOverviewData> GetSecurityUniverse(PortfolioSelectionData portfolio, BenchmarkSelectionData benchmark, String currency)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionTrace.LogException(ex);
        //        string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
        //        throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
        //    }
        //}


    }
}
