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
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;
using System.Diagnostics;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for CustomScreeningTool and Composite Fund
    /// </summary>
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CustomScreeningToolOperations
    {
        /// <summary>
        /// Instance of DimensionService
        /// </summary>
      /*  private Entities dimensionEntity;
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
        /// <summary>
        /// Fault Resource manager
        /// </summary>
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
                {
                    throw new Exception("Services are not available");
                }

                List<string> result = new List<string>();
                List<string> temp = new List<string>();
                DimensionEntities entity = DimensionEntity;

                switch (parameter)
                {
                    case "Region":
                        temp = (from iry in entity.GF_SECURITY_BASEVIEW
                                select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.ASEC_SEC_COUNTRY_ZONE_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME)
                                .Distinct().ToList();
                        break;
                    case "Country":
                        temp = (from iry in entity.GF_SECURITY_BASEVIEW
                                select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.ASEC_SEC_COUNTRY_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME)
                                .Distinct().ToList();
                        break;
                    case "Sector":
                        temp = (from iry in entity.GF_SECURITY_BASEVIEW
                                select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.GICS_SECTOR_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME)
                               .Distinct().ToList();
                        break;
                    case "Industry":
                        temp = (from iry in entity.GF_SECURITY_BASEVIEW
                                select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.GICS_INDUSTRY_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME)
                                .Distinct().ToList();
                        break;
                    default:
                        break;
                }
                foreach (string item in temp)
                {
                    if (item == " " || item == null)
                    {
                        result.Add("Unknown");
                    }
                    else { result.Add(item); }
                }
                result = result.OrderBy(a => a).ToList();
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
                List<CustomSelectionData> result = new List<CustomSelectionData>();

                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<SCREENING_DISPLAY_REFERENCE> data = new List<SCREENING_DISPLAY_REFERENCE>();

                data = entity.SCREENING_DISPLAY_REFERENCE.OrderBy(record => record.DATA_DESC).ToList();

                if (data == null || data.Count < 1)
                {
                    return result;
                }

                foreach (SCREENING_DISPLAY_REFERENCE item in data)
                {
                    if (item.DATA_DESC != null && item.SCREENING_ID != null)
                    {
                        result.Add(new CustomSelectionData()
                        {
                            ScreeningId = item.SCREENING_ID,
                            DataDescription = item.DATA_DESC,
                            LongDescription = item.LONG_DESC,
                            DataColumn = item.TABLE_COLUMN,
                            ShortDescription = item.SHORT_COLUMN_DESC
                        });
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
        /// Retrieving Period Financials Tab Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrievePeriodFinancialsTabDataPoints()
        {
            try
            {
                List<CustomSelectionData> result = new List<CustomSelectionData>();
                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<FinancialTabDataDescriptions> data = new List<FinancialTabDataDescriptions>();

                data = entity.GetFinancialTabDataDescriptions("Period").ToList();

                if (data == null || data.Count < 1)
                {
                    return result;
                }

                foreach (FinancialTabDataDescriptions item in data)
                {
                    if (item.DATA_DESC != null && item.SCREENING_ID != null)
                    {
                        result.Add(new CustomSelectionData()
                        {
                            ScreeningId = item.SCREENING_ID,
                            DataDescription = item.DATA_DESC,
                            LongDescription = item.LONG_DESC,
                            Quaterly = item.QUARTERLY,
                            Annual = item.ANNUAL,
                            DataID = item.DATA_ID,
                            EstimateID = item.ESTIMATE_ID
                        });
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
        /// Retrieving Current Financials Tab Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrieveCurrentFinancialsTabDataPoints()
        {
            try
            {
                List<CustomSelectionData> result = new List<CustomSelectionData>();
                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<FinancialTabDataDescriptions> data = new List<FinancialTabDataDescriptions>();

                data = entity.GetFinancialTabDataDescriptions("Current").ToList();

                if (data == null || data.Count < 1)
                {
                    return result;
                }

                foreach (FinancialTabDataDescriptions item in data)
                {
                    if (item.DATA_DESC != null && item.SCREENING_ID != null)
                    {
                        result.Add(new CustomSelectionData()
                        {
                            ScreeningId = item.SCREENING_ID,
                            DataDescription = item.DATA_DESC,
                            LongDescription = item.LONG_DESC,
                            Quaterly = item.QUARTERLY,
                            Annual = item.ANNUAL,
                            DataID = item.DATA_ID,
                            EstimateID = item.ESTIMATE_ID
                        });
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
        /// Retrieving Fair Value Tab Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrieveFairValueTabDataPoints()
        {
            try
            {
                List<CustomSelectionData> result = new List<CustomSelectionData>();
                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<SCREENING_DISPLAY_FAIRVALUE> data = new List<SCREENING_DISPLAY_FAIRVALUE>();

                data = entity.SCREENING_DISPLAY_FAIRVALUE.OrderBy(record => record.DATA_DESC).ToList();

                if (data == null || data.Count < 1)
                {
                    return result;
                }

                foreach (SCREENING_DISPLAY_FAIRVALUE item in data)
                {
                    if (item.DATA_DESC != null && item.SCREENING_ID != null)
                    {
                        result.Add(new CustomSelectionData()
                        {
                            ScreeningId = item.SCREENING_ID,
                            DataDescription = item.DATA_DESC,
                            LongDescription = item.LONG_DESC,
                            DataColumn = item.TABLE_COLUMN
                        });
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
        /// Save user preferred Data Points List
        /// </summary>
        /// <param name="userPreference">string</param>
        /// <param name="username">string</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean? SaveUserDataPointsPreference(string userPreference, string username)
        {
            try
            {
                bool isSaveSuccessful = true;
                if (userPreference == null || username == null)
                {
                    return false;
                }

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }

                int? result;
                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();

                result = entity.SaveCustomScreeningDataPointsPreference(userPreference, username).FirstOrDefault();

                if (result < 0)
                {
                    isSaveSuccessful = false;
                }
                return isSaveSuccessful;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// retrieve stored user preference for custom screening data
        /// </summary>
        /// <param name="username">string</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CSTUserPreferenceInfo> GetCustomScreeningUserPreferences(string username)
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }

                List<CSTUserPreferenceInfo> result = new List<CSTUserPreferenceInfo>();
                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<CustomScreeningUserPreferences> data = new List<CustomScreeningUserPreferences>();

                data = entity.GetCustomScreeningUserPreferences(username).ToList();

                if (data == null || data.Count < 1)
                {
                    return result;
                }

                foreach (CustomScreeningUserPreferences item in data)
                {
                    int dataId = 0;
                    if (item.ScreeningId != null)
                    {
                        dataId = GetCustomScreeningDataId(item.ScreeningId);
                    }
                    result.Add(new CSTUserPreferenceInfo()
                    {
                        UserName = item.UserName,
                        ScreeningId = item.ScreeningId,
                        DataDescription = item.DataDescription,
                        ListName = item.ListName,
                        TableColumn = item.TableColumn,
                        Accessibility = item.Accessibilty,
                        DataSource = item.DataSource,
                        PeriodType = item.PeriodType,
                        YearType = item.YearType,
                        FromDate = Convert.ToInt32(item.FromDate),
                        ToDate = Convert.ToInt32(item.ToDate),
                        DataPointsOrder = Convert.ToInt32(item.DataPointsOrder),
                        DataID = dataId
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
        /// <param name="portfolio">PortfolioSelectionData</param>
        /// <param name="benchmark">EntitySelectionData</param>
        /// <param name="region">string</param>
        /// <param name="country">string</param>
        /// <param name="sector">string</param>
        /// <param name="industry">string</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomScreeningSecurityData> RetrieveSecurityData(PortfolioSelectionData portfolio,
            EntitySelectionData benchmark, String region, String country, String sector, String industry, List<CSTUserPreferenceInfo> userPreference)
        {
            string input = (portfolio == null ? "Portfolio=null" : ("BenchmarkId=" + (portfolio.BenchmarkId == null ? "null" : portfolio.BenchmarkId)
                    + ";PortfolioId=" + (portfolio.PortfolioId == null ? "null" : portfolio.PortfolioId)
                    + ";PortfolioThemeSubGroupId=" + (portfolio.PortfolioThemeSubGroupId == null ? "null" : portfolio.PortfolioThemeSubGroupId)
                    + ";PortfolioThemeSubGroupName=" + (portfolio.PortfolioThemeSubGroupName == null ? "null" : portfolio.PortfolioThemeSubGroupName)))
                    + ";SecurityInstrumentID=" + (benchmark == null || benchmark.InstrumentID == null ? "null" : benchmark.InstrumentID)
                    + ";Region=" + (region == null ? "null" : region)
                    + ";Country=" + (country == null ? "null" : country)
                    + ";Sector=" + (sector == null ? "null" : sector)
                    + ";Industry=" + (industry == null ? "null" : industry)
                    + ";CSTUserPreferenceInfoCount=" + (userPreference == null ? "null" : userPreference.Count.ToString());
            ExceptionTrace.LogInfo(input, "Start", "RetrieveSecurityData");

            try
            {
                List<CustomScreeningSecurityData> result = new List<CustomScreeningSecurityData>();

                if (userPreference.Count == 1 && userPreference[0].ScreeningId == null)
                {
                    return result;
                }

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception();
                }

                DimensionEntities entity = DimensionEntity;
                ExternalResearchEntities externalEntity = new ExternalResearchEntities();
                CustomScreeningToolEntities cstEntity = new CustomScreeningToolEntities();
                List<CustomScreeningSecurityData> securityList = new List<CustomScreeningSecurityData>();

                ExceptionTrace.LogInfo(input, "Start", "RetrieveSecurityDetailsList");
                securityList = RetrieveSecurityDetailsList(portfolio, benchmark, region, country, sector, industry);
                ExceptionTrace.LogInfo(input, "End", "RetrieveSecurityDetailsList");

                List<string> distinctSecurityId = securityList.Select(record => record.SecurityId).ToList();
                List<string> distinctIssuerId = securityList.Select(record => record.IssuerId).ToList();

                string _securityIds = StringBuilder(distinctSecurityId);
                string _issuerIds = StringBuilder(distinctIssuerId);

                #region Retrieving data Items

                if (userPreference != null)
                {
                    foreach (CSTUserPreferenceInfo item in userPreference)
                    {
                        if (item.ScreeningId != null)
                        {
                            //retrieving REF Data Items
                            if (item.ScreeningId.StartsWith("REF"))
                            {
                                SCREENING_DISPLAY_REFERENCE referenceData = cstEntity.SCREENING_DISPLAY_REFERENCE
                                    .Where(a => a.SCREENING_ID == item.ScreeningId).FirstOrDefault();

                                ExceptionTrace.LogInfo(";SecurityIds=" + (_securityIds == null ? "null" : _securityIds), "Start", "GetCustomScreeningREFData");
                                List<CustomScreeningREFData> data = cstEntity.GetCustomScreeningREFData(_securityIds).ToList();
                                ExceptionTrace.LogInfo(";SecurityIds=" + (_securityIds == null ? "null" : _securityIds), "End", "GetCustomScreeningREFData");

                                foreach (CustomScreeningREFData record in data)
                                {
                                    CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();

                                    fillData.SecurityId = record.SECURITY_ID;
                                    fillData.AsecShortName = securityList.Where(a => a.SecurityId == record.SECURITY_ID).Select(a => a.AsecShortName).FirstOrDefault(); //record.ASEC_SEC_SHORT_NAME;
                                    fillData.IssueName = securityList.Where(a => a.SecurityId == record.SECURITY_ID).Select(a => a.IssueName).FirstOrDefault();
                                    fillData.Type = referenceData.TABLE_COLUMN;
                                    fillData.Multiplier = referenceData.MULTIPLIER;
                                    object amount = fillData.Multiplier != null ? Convert.ToDecimal(record.GetType().GetProperty(fillData.Type)
                                        .GetValue(record, null)) * fillData.Multiplier : record.GetType().GetProperty(fillData.Type)
                                        .GetValue(record, null);
                                    fillData.Decimals = referenceData.DECIMAL;
                                    fillData.IsPercentage = referenceData.PERCENTAGE;
                                    amount = fillData.Decimals != null ? Math.Round(Convert.ToDecimal(amount), Convert.ToInt16(fillData.Decimals)) : amount;
                                    fillData.Value = fillData.IsPercentage != null ? fillData.IsPercentage.Contains("Y") ? Convert.ToString(amount) + "%" : Convert.ToString(amount)
                                                                                   : Convert.ToString(amount);

                                    result.Add(fillData);
                                }
                            }
                            //retrieving FIN Data Items
                            else if (item.ScreeningId.StartsWith("FIN"))
                            {
                                SCREENING_DISPLAY_PERIOD referenceData = cstEntity.SCREENING_DISPLAY_PERIOD
                                    .Where(a => a.SCREENING_ID == item.ScreeningId).FirstOrDefault();

                                List<CustomScreeningFINData> temp = new List<CustomScreeningFINData>();
                                if (item.PeriodType != null)
                                {
                                    if (item.PeriodType.StartsWith("A"))
                                    {
                                        cstEntity.CommandTimeout = 5000;
                                        ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                            + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                            + ";DataId=" + item.DataID.ToString()
                                            + ";PeriodType=" + (item == null || item.DataSource == null ? "null" : item.PeriodType.Substring(0, 1))
                                            + ";FromDate=" + (item == null || item.DataSource == null ? "null" : item.FromDate.ToString())
                                            + ";YearType=" + (item == null || item.DataSource == null ? "null" : item.YearType)
                                            + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                            , "Start", "GetCustomScreeningREFData");
                                        temp = cstEntity.GetCustomScreeningFINData(_issuerIds, _securityIds, item.DataID, item.PeriodType.Substring(0, 1),
                                            item.FromDate, item.YearType, item.DataSource).ToList();
                                        ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                            + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                            + ";DataId=" + item.DataID.ToString()
                                            + ";PeriodType=" + (item == null || item.DataSource == null ? "null" : item.PeriodType.Substring(0, 1))
                                            + ";FromDate=" + (item == null || item.DataSource == null ? "null" : item.FromDate.ToString())
                                            + ";YearType=" + (item == null || item.DataSource == null ? "null" : item.YearType)
                                            + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                            , "End", "GetCustomScreeningREFData");
                                    }
                                    else
                                    {
                                        cstEntity.CommandTimeout = 5000;
                                        ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                            + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                            + ";DataId=" + item.DataID.ToString()
                                            + ";PeriodType=" + (item == null || item.DataSource == null ? "null" : item.PeriodType.Substring(0, 1))
                                            + ";FromDate=" + (item == null || item.DataSource == null ? "null" : item.FromDate.ToString())
                                            + ";YearType=" + (item == null || item.DataSource == null ? "null" : item.YearType)
                                            + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                            , "Start", "GetCustomScreeningFINData");
                                        temp = cstEntity.GetCustomScreeningFINData(_issuerIds, _securityIds, item.DataID, item.PeriodType, item.FromDate,
                                            item.YearType, item.DataSource).ToList();
                                        ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                            + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                            + ";DataId=" + item.DataID.ToString()
                                            + ";PeriodType=" + (item == null || item.DataSource == null ? "null" : item.PeriodType.Substring(0, 1))
                                            + ";FromDate=" + (item == null || item.DataSource == null ? "null" : item.FromDate.ToString())
                                            + ";YearType=" + (item == null || item.DataSource == null ? "null" : item.YearType)
                                            + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                            , "End", "GetCustomScreeningFINData");
                                    }

                                    foreach (CustomScreeningFINData record in temp)
                                    {
                                        IEnumerable<CustomScreeningSecurityData> matchedSecurities = securityList.Where(a => a.IssuerId == record.IssuerId || a.SecurityId == record.SecurityId);

                                        foreach (CustomScreeningSecurityData customScreeningSecurityData in matchedSecurities)
                                        {
                                            CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();
                                            fillData.SecurityId = customScreeningSecurityData.SecurityId;
                                            fillData.AsecShortName = customScreeningSecurityData.AsecShortName;
                                            fillData.IssuerId = record.IssuerId;
                                            fillData.IssueName = customScreeningSecurityData.IssueName;
                                            fillData.Type = item.DataDescription;
                                            fillData.Multiplier = referenceData.MULTIPLIER;
                                            decimal _amount = fillData.Multiplier != null ? Convert.ToDecimal(record.Amount * fillData.Multiplier) : record.Amount;
                                            fillData.DataSource = item.DataSource;
                                            fillData.PeriodYear = record.PeriodYear;
                                            fillData.PeriodType = item.PeriodType;
                                            fillData.YearType = item.YearType;
                                            fillData.Decimals = referenceData.DECIMAL;
                                            fillData.IsPercentage = referenceData.PERCENTAGE;
                                            _amount = fillData.Decimals != null ? Math.Round(Convert.ToDecimal(_amount), Convert.ToInt16(fillData.Decimals)) : _amount;
                                            fillData.Value = fillData.IsPercentage != null ? fillData.IsPercentage.Contains("Y") ? Convert.ToString(_amount) + "%" : Convert.ToString(_amount)
                                                                                            : Convert.ToString(_amount);
                                            result.Add(fillData);
                                        }
                                    }
                                }
                            }
                            //retrieving CUR Data Items
                            else if (item.ScreeningId.StartsWith("CUR"))
                            {
                                SCREENING_DISPLAY_CURRENT referenceData = cstEntity.SCREENING_DISPLAY_CURRENT
                                    .Where(a => a.SCREENING_ID == item.ScreeningId).FirstOrDefault();

                                List<CustomScreeningCURData> temp = new List<CustomScreeningCURData>();
                                cstEntity.CommandTimeout = 5000;
                                ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                    + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                    + ";DataId=" + item.DataID.ToString()
                                    + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                    , "Start", "GetCustomScreeningCURData");
                                temp = cstEntity.GetCustomScreeningCURData(_issuerIds, _securityIds, item.DataID, item.DataSource).ToList();
                                ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                    + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                    + ";DataId=" + item.DataID.ToString()
                                    + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                    , "End", "GetCustomScreeningCURData");
                                foreach (CustomScreeningCURData record in temp)
                                {
                                    IEnumerable<CustomScreeningSecurityData> matchedSecurities = securityList.Where(a => a.IssuerId == record.IssuerId || a.SecurityId == record.SecurityId);

                                    foreach (CustomScreeningSecurityData customScreeningSecurityData in matchedSecurities)
                                    {
                                        CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();
                                        fillData.SecurityId = customScreeningSecurityData.SecurityId;
                                        fillData.IssuerId = record.IssuerId;
                                        fillData.AsecShortName = customScreeningSecurityData.AsecShortName;
                                        fillData.IssueName = customScreeningSecurityData.IssueName;
                                        fillData.Type = item.DataDescription;
                                        fillData.Multiplier = referenceData.MULTIPLIER;
                                        decimal _amount = fillData.Multiplier != null ? Convert.ToDecimal(record.Amount * fillData.Multiplier) : record.Amount;
                                        fillData.DataSource = item.DataSource;
                                        fillData.Decimals = referenceData.DECIMAL;
                                        fillData.IsPercentage = referenceData.PERCENTAGE;
                                        _amount = fillData.Decimals != null ? Math.Round(Convert.ToDecimal(_amount), Convert.ToInt16(fillData.Decimals)) : _amount;
                                        fillData.Value = fillData.IsPercentage != null ? fillData.IsPercentage.Contains("Y") ? Convert.ToString(_amount) + "%" : Convert.ToString(_amount)
                                                                                       : Convert.ToString(_amount);
                                        result.Add(fillData);
                                    }
                                }
                            }
                            //retrieving FVA Data Items
                            else if (item.ScreeningId.StartsWith("FVA"))
                            {
                                SCREENING_DISPLAY_FAIRVALUE referenceData = cstEntity.SCREENING_DISPLAY_FAIRVALUE
                                    .Where(a => a.SCREENING_ID == item.ScreeningId).FirstOrDefault();
                                cstEntity.CommandTimeout = 5000;
                                ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                    + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                    + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                    , "Start", "GetCustomScreeningFVAData");
                                List<CustomScreeningFVAData> fvaData = cstEntity.GetCustomScreeningFVAData(_securityIds, item.DataSource).ToList();
                                ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                                    + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds)
                                    + ";DataSource=" + (item == null || item.DataSource == null ? "null" : item.DataSource)
                                    , "End", "GetCustomScreeningFVAData");
                                foreach (CustomScreeningFVAData record in fvaData)
                                {
                                    CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();
                                    fillData.SecurityId = record.SECURITY_ID;
                                    fillData.IssueName = securityList.Where(a => a.SecurityId == record.SECURITY_ID).Select(a => a.IssueName).FirstOrDefault();
                                    fillData.AsecShortName = securityList.Where(a => a.SecurityId == record.SECURITY_ID).Select(a => a.AsecShortName).FirstOrDefault();
                                    fillData.Type = referenceData.TABLE_COLUMN;
                                    fillData.DataSource = item.DataSource;
                                    fillData.Multiplier = referenceData.MULTIPLIER;
                                    object amount = fillData.Multiplier != null ? Convert.ToDecimal(record.GetType().GetProperty(fillData.Type)
                                        .GetValue(record, null)) * fillData.Multiplier : record.GetType().GetProperty(fillData.Type)
                                        .GetValue(record, null);
                                    fillData.Decimals = referenceData.DECIMAL;
                                    fillData.IsPercentage = referenceData.PERCENTAGE;
                                    amount = fillData.Decimals != null ? Math.Round(Convert.ToDecimal(amount), Convert.ToInt16(fillData.Decimals)) : amount;
                                    fillData.Value = fillData.IsPercentage != null ? fillData.IsPercentage.Contains("Y") ? Convert.ToString(amount) + "%" : Convert.ToString(amount)
                                                                                   : Convert.ToString(amount);
                                    fillData.DataSource = item.DataSource;

                                    result.Add(fillData);
                                }
                            }
                        }
                    }
                    ExceptionTrace.LogInfo(input, "End", "GetCustomScreeningREFData");
                }

                #endregion

                #region Market Cap Data
                List<CustomScreeningMarketCap> marketCapData = new List<CustomScreeningMarketCap>();
                ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                    + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds), "Start", "GetCustomScreeningMarketCap");
                marketCapData = cstEntity.GetCustomScreeningMarketCap(_securityIds).ToList();
                ExceptionTrace.LogInfo("IssuerIds=" + (_issuerIds == null ? "null" : _issuerIds)
                    + ";SecurityIds=" + (_securityIds == null ? "null" : _securityIds), "End", "GetCustomScreeningMarketCap");

                if (marketCapData != null && marketCapData.Count != 0 && result != null)
                {
                    foreach (CustomScreeningSecurityData item in result)
                    {
                        item.MarketCapAmount = marketCapData.Where(a => a.SecurityId == item.SecurityId).Select(a => a.Amount).FirstOrDefault();
                    }
                }

                #endregion

                ExceptionTrace.LogInfo(input, "End", "RetrieveSecurityData");
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                Trace.WriteLine(ex.InnerException);
                Trace.WriteLine(ex.StackTrace);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Update user preferred Data Points List
        /// </summary>
        /// <param name="userPreference">string</param>
        /// <param name="username">string</param>
        /// <param name="existingListname">string</param>
        /// <param name="newListname">string</param>
        /// <param name="accessibility">string</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean? UpdateUserDataPointsPreference(string userPreference, string username, string existingListname, string newListname, string accessibility)
        {
            try
            {
                bool isSaveSuccessful = true;
                if (userPreference == null || username == null || existingListname == null || newListname == null || accessibility == null)
                {
                    return false;
                }

                int? result;
                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();

                result = entity.UpdateCustomScreeningDataPointsPreference(userPreference, username, existingListname, newListname, accessibility).FirstOrDefault();

                if (result < 0)
                {
                    isSaveSuccessful = false;
                }

                return isSaveSuccessful;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieving Fair Value Tab Source List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<string> RetrieveFairValueTabSource()
        {
            try
            {
                List<string> result = new List<string>();
                ExternalResearchEntities entity = new ExternalResearchEntities();

                result = entity.FAIR_VALUE.Select(a => a.VALUE_TYPE).Distinct().ToList();

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
        /// Retrieving Composite Fund Data (for Holdings and Positioning gadget)
        /// </summary>
        /// <param name="entityIdentifiers">EntitySelectionData</param>
        /// <param name="portfolio">PortfolioSelectionData</param>
        /// <returns>list of CompositeFundData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CompositeFundData> RetrieveCompositeFundData(EntitySelectionData entityIdentifiers, PortfolioSelectionData portfolio)
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }

                DimensionEntities entity = DimensionEntity;
                ExternalResearchEntities externalEntity = new ExternalResearchEntities();

                List<CompositeFundData> result = new List<CompositeFundData>();
                List<GreenField.DAL.GF_BENCHMARK_HOLDINGS> benchmarkData = new List<GreenField.DAL.GF_BENCHMARK_HOLDINGS>();
                List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> portfolioHoldingsDataAll = new List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS>();
                List<CompositeFundData> portfolioTargets = new List<CompositeFundData>();

                string issuerId = entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == entityIdentifiers.InstrumentID).FirstOrDefault() != null ?
                entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == entityIdentifiers.InstrumentID).FirstOrDefault().ISSUER_ID : null;

                // GF_PORTFOLIO_HOLDINGS data all
                DateTime lastBusinessDatePortfolio = GetLastBusinessDate("PORTFOLIO_HOLDINGS"); 
                portfolioHoldingsDataAll = entity.GF_PORTFOLIO_HOLDINGS.Where(a => a.PORTFOLIO_ID == portfolio.PortfolioId
                                 && a.PORTFOLIO_DATE == lastBusinessDatePortfolio).ToList();

               // GF_BENCHMARK_HOLDINGS data
                string benchmarkId = entity.GF_PORTFOLIO_HOLDINGS.Where(a => a.PORTFOLIO_ID == portfolio.PortfolioId).FirstOrDefault() != null ?
                    entity.GF_PORTFOLIO_HOLDINGS.Where(a => a.PORTFOLIO_ID == portfolio.PortfolioId).FirstOrDefault().BENCHMARK_ID : null;
                Dictionary<string, decimal> benchmarkCountryData = new Dictionary<string, decimal>();
                if (benchmarkId != null)
                {
                    DateTime lastBusinessDateBenchmark = GetLastBusinessDate("BENCHMARK_HOLDINGS");
                    benchmarkData = entity.GF_BENCHMARK_HOLDINGS.Where(record => record.BENCHMARK_ID == benchmarkId && record.PORTFOLIO_DATE == lastBusinessDateBenchmark).ToList();

                    List<string> countryInBenchmarkData = benchmarkData.Select(a => a.ISO_COUNTRY_CODE).Distinct().ToList();
                    foreach (string item in countryInBenchmarkData)
                    {
                        if (item != null)
                        {
                            decimal benchmarkSum = Convert.ToDecimal(benchmarkData.Where(a => a.ISO_COUNTRY_CODE == item).Sum(a => a.BENCHMARK_WEIGHT));
                            benchmarkCountryData.Add(item, benchmarkSum);
                        }
                    }
                }

                // issuer view checkbox is not checked 
                CompositeFundData rowSecurityLevel = FillResultSetCompositeFund(entityIdentifiers.InstrumentID, issuerId, portfolioTargets, 
                                                    benchmarkData, portfolioHoldingsDataAll, benchmarkCountryData, false);
                result.Add(rowSecurityLevel);

                // issuer view checkbox is checked
                CompositeFundData rowIssuerLevel = FillResultSetCompositeFund(entityIdentifiers.InstrumentID, issuerId, portfolioTargets, 
                                                   benchmarkData, portfolioHoldingsDataAll, benchmarkCountryData, true);
                result.Add(rowIssuerLevel);
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
        /// <summary>
        /// retrieving data id for custom screening tool on basis of screening id
        /// </summary>
        /// <param name="screeningId">screeningId</param>
        /// <returns>int data id</returns>
        public int GetCustomScreeningDataId(string screeningId)
        {
            int dataId = 0;
            string screeningIdStart = screeningId.Substring(0, 3);
            CustomScreeningToolEntities entity = new CustomScreeningToolEntities();

            switch (screeningIdStart)
            {
                case "FIN":
                    if (screeningId != null)
                    {
                        dataId = entity.SCREENING_DISPLAY_PERIOD.Where(a => a.SCREENING_ID == screeningId).Select(a => a.DATA_ID).FirstOrDefault();
                    }
                    break;
                case "CUR":
                    if (screeningId != null)
                    {
                        dataId = entity.SCREENING_DISPLAY_CURRENT.Where(a => a.SCREENING_ID == screeningId).Select(a => a.DATA_ID).FirstOrDefault();
                    }
                    break;
                default:
                    break;
            }
            return dataId;
        }

        /// <summary>
        /// retrieving from views list of securities for selection done in cuustom screening tool
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="benchmark"></param>
        /// <param name="region"></param>
        /// <param name="country"></param>
        /// <param name="sector"></param>
        /// <param name="industry"></param>
        /// <returns>list of securities</returns>
        public List<CustomScreeningSecurityData> RetrieveSecurityDetailsList(PortfolioSelectionData portfolio,
            EntitySelectionData benchmark, String region, String country, String sector, String industry)
        {
            try
            {
                DimensionEntities entity = DimensionEntity;
                ExternalResearchEntities externalEntity = new ExternalResearchEntities();
                List<GF_SECURITY_BASEVIEW_Local> securitiesFromCustomControls = new List<GF_SECURITY_BASEVIEW_Local>();
                List<CustomScreeningSecurityData> securityList = new List<CustomScreeningSecurityData>();

                if (portfolio != null)
                {
                    List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> securitiesFromPortfolio = new List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS>();
                    DateTime lastBusinessDate = GetLastBusinessDate("PORTFOLIO_HOLDINGS");
                    List<GF_SECURITY_BASEVIEW_Local> fullSecurityList = externalEntity.GF_SECURITY_BASEVIEW_Local.ToList();

                    securitiesFromPortfolio = entity.GF_PORTFOLIO_HOLDINGS.Where(record => record.PORTFOLIO_ID == portfolio.PortfolioId
                                                                                     && record.PORTFOLIO_DATE == lastBusinessDate
                                                                                     && (record.A_SEC_INSTR_TYPE == "Equity" || record.A_SEC_INSTR_TYPE == "GDR/ADR")
                                                                                     && record.DIRTY_VALUE_PC > 0).ToList();
                    if (securitiesFromPortfolio == null)
                        return securityList;

                    securitiesFromPortfolio = securitiesFromPortfolio.Distinct().ToList();
                    foreach (GreenField.DAL.GF_PORTFOLIO_HOLDINGS item in securitiesFromPortfolio)
                    {

                        GF_SECURITY_BASEVIEW_Local securityIdRow = item.ASEC_SEC_SHORT_NAME != null
                            ? externalEntity.GF_SECURITY_BASEVIEW_Local.Where(a => a.ASEC_SEC_SHORT_NAME == item.ASEC_SEC_SHORT_NAME).FirstOrDefault() : null;

                        securityList.Add(new CustomScreeningSecurityData()
                        {
                            SecurityId = securityIdRow != null ? (securityIdRow.SECURITY_ID).ToString() : null,
                            IssuerId = item.ISSUER_ID,
                            IssueName = item.ISSUE_NAME,
                            AsecShortName = securityIdRow != null ? securityIdRow.ASEC_SEC_SHORT_NAME.ToString() : null
                        });
                    }
                    return securityList;
                }
                else if (benchmark != null)
                {

                    List<GF_BENCHMARK_HOLDINGS_Local> securitiesFromBenchmark = new List<GF_BENCHMARK_HOLDINGS_Local>();
                    DateTime lastBusinessDate = GetLastBusinessDate("BENCHMARK_HOLDINGS");
                    List<GF_SECURITY_BASEVIEW_Local> fullSecurityList = externalEntity.GF_SECURITY_BASEVIEW_Local.ToList();
                    securitiesFromBenchmark = externalEntity.GF_BENCHMARK_HOLDINGS_Local.Where(record => record.BENCHMARK_ID == benchmark.InstrumentID).ToList();

                    if (securitiesFromBenchmark == null)
                        return securityList;

                    securitiesFromBenchmark = securitiesFromBenchmark.Distinct().ToList();
                    foreach (GF_BENCHMARK_HOLDINGS_Local item in securitiesFromBenchmark)
                    {
                        GF_SECURITY_BASEVIEW_Local securityIdRow = item.ASEC_SEC_SHORT_NAME != null
                            ? externalEntity.GF_SECURITY_BASEVIEW_Local.Where(a => a.ASEC_SEC_SHORT_NAME == item.ASEC_SEC_SHORT_NAME).FirstOrDefault() : null;
                        securityList.Add(new CustomScreeningSecurityData()
                        {
                            SecurityId = securityIdRow != null ? (securityIdRow.SECURITY_ID).ToString() : null,
                            IssuerId = item.ISSUER_ID,
                            IssueName = item.ISSUE_NAME,
                            AsecShortName = securityIdRow != null ? securityIdRow.ASEC_SEC_SHORT_NAME.ToString() : null
                        });
                    }
                    return securityList;
                }
                else
                {
                    string regionValue = String.IsNullOrEmpty(region) ? String.Empty : region;
                    string countryValue = String.IsNullOrEmpty(country) ? String.Empty : country;
                    string sectorValue = String.IsNullOrEmpty(sector) ? String.Empty : sector;
                    string industryValue = String.IsNullOrEmpty(industry) ? String.Empty : industry;

                    List<GF_SECURITY_BASEVIEW_Local> securitiesList = new List<GF_SECURITY_BASEVIEW_Local>();
                    securitiesList = (from p in externalEntity.GF_SECURITY_BASEVIEW_Local
                                      where p.ASEC_SEC_COUNTRY_ZONE_NAME.Contains(regionValue)
                                      && p.ASEC_SEC_COUNTRY_NAME.Contains(countryValue)
                                      && p.GICS_SECTOR_NAME.Contains(sectorValue)
                                      && p.GICS_INDUSTRY_NAME.Contains(industryValue)
                                      select p).ToList();
                    if (securitiesList != null)
                    {
                        securitiesList = securitiesList.Distinct().ToList();
                        securitiesFromCustomControls.AddRange(securitiesList);
                    }
                }
                if (securitiesFromCustomControls == null)
                {
                    return securityList;
                }

                foreach (GF_SECURITY_BASEVIEW_Local item in securitiesFromCustomControls)
                {
                    securityList.Add(new CustomScreeningSecurityData()
                    {
                        SecurityId = item.SECURITY_ID,
                        IssuerId = item.ISSUER_ID,
                        IssueName = item.ISSUE_NAME,
                        AsecShortName = item.ASEC_SEC_SHORT_NAME
                    });
                }
                return securityList;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// method to create comma separated string of list passed
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string StringBuilder(List<string> param)
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
        /// method to perform calculations for composite fund gadget (also Holdings and Positioning gadget)
        /// </summary>
        /// <param name="InstrumentID"></param>
        /// <param name="issuerId"></param>
        /// <param name="portfolioTargets"></param>
        /// <param name="portfolioCountryTargets"></param>
        /// <param name="benchmarkData"></param>
        /// <param name="portfolioHoldingsData"></param>
        /// <param name="benchmarkCountryData"></param>
        /// <param name="check"></param>
        /// <returns> calculated data for composite fund gadget</returns>
        public CompositeFundData FillResultSetCompositeFund(string InstrumentID, string issuerId, List<CompositeFundData> portfolioTargets,
            List<GreenField.DAL.GF_BENCHMARK_HOLDINGS> benchmarkData, List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> portfolioHoldingsDataAll,
            Dictionary<string, decimal> benchmarkCountryData, bool check)
        {
            DimensionEntities entity = DimensionEntity;

            Int32 securityId = entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID).FirstOrDefault() != null ?
                   Convert.ToInt32(entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID).FirstOrDefault().SECURITY_ID) : 0;
            string country = entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID).FirstOrDefault() != null ?
                entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID).FirstOrDefault().ISO_COUNTRY_CODE : null;

            CompositeFundData temp = new CompositeFundData();
            decimal targetSumBenchmark, value;
            decimal? objPercent = null, objHoldingInCountry = null, objBenchmarkWeight = null, objBenchmarkWeightInCountry = null;
 
            //Portfolio Holdings Percent
            objPercent = check ? Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ISSUER_ID == issuerId)
                      .Sum(a => a.DIRTY_VALUE_PC)) /
                      Convert.ToDecimal(portfolioHoldingsDataAll.Sum(a => a.DIRTY_VALUE_PC))
              : Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID)
                      .Select(a => a.DIRTY_VALUE_PC).FirstOrDefault()) /
                      Convert.ToDecimal(portfolioHoldingsDataAll.Sum(a => a.DIRTY_VALUE_PC));
            temp.PortfolioTarget = objPercent != null ? Math.Round(Convert.ToDecimal(objPercent) * 100, 2) : Math.Round(Convert.ToDecimal(0.00));


            //Portfolio Holdings in Country 
            objHoldingInCountry = check ? Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ISSUER_ID == issuerId).Sum(a => a.DIRTY_VALUE_PC)) /
                        Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ISO_COUNTRY_CODE == country).Sum(a => a.DIRTY_VALUE_PC))
                : Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID).Select(a => a.DIRTY_VALUE_PC).FirstOrDefault()) /
                        Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ISO_COUNTRY_CODE == country).Sum(a => a.DIRTY_VALUE_PC)) ;
                temp.PortfolioTargetInCountry = objHoldingInCountry != null ? Math.Round(Convert.ToDecimal(objHoldingInCountry) * 100, 2) : Math.Round(Convert.ToDecimal(0.00));

            //Holdins (mn)
            temp.Holdings = check ? Math.Round(Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ISSUER_ID == issuerId)
                                                                                             .Sum(a => a.DIRTY_VALUE_PC)) / Convert.ToDecimal(1000000), 1)
               : Math.Round(Convert.ToDecimal(portfolioHoldingsDataAll.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID)
                                                                             .Select(a => a.DIRTY_VALUE_PC).FirstOrDefault()) / Convert.ToDecimal(1000000), 1);


            if (benchmarkData.Count > 0)
            {
                objBenchmarkWeight = check
                    //? Convert.ToDecimal(benchmarkData.Where(a => a.ISSUER_ID == issuerId).Select(a => a.BENCHMARK_WEIGHT).FirstOrDefault()) / 100  //This was changed becayse it was not providing an accurate summed total for the issuers
                    ? Convert.ToDecimal(benchmarkData.Where(a => a.ISSUER_ID == issuerId).ToList().Sum(a => a.BENCHMARK_WEIGHT)) / 100
                    : Convert.ToDecimal(benchmarkData.Where(a => a.ASEC_SEC_SHORT_NAME == InstrumentID).Select(a => a.BENCHMARK_WEIGHT).FirstOrDefault()) / 100;
            }
            //Benchmark Weight 
            temp.BenchmarkWeight = objBenchmarkWeight != null ? Math.Round(Convert.ToDecimal(objBenchmarkWeight) * 100, 2) : Convert.ToDecimal(0.00);
            targetSumBenchmark = benchmarkCountryData.TryGetValue(country, out value) ? value / 100 : 0;
            
            //Benchmark Weight in Country 
            temp.BenchmarkWeightInCountry = Math.Round(0.00);
            if (targetSumBenchmark != 0)
            {
                objBenchmarkWeightInCountry = objBenchmarkWeight / targetSumBenchmark;
                temp.BenchmarkWeightInCountry = objBenchmarkWeightInCountry != null ? Math.Round(Convert.ToDecimal(objBenchmarkWeightInCountry) * 100, 2)
                    : Math.Round(Convert.ToDecimal(0.00));
            }


            temp.ActivePosition = Math.Round(0.00);
            temp.ActivePositionInCountry = Math.Round(0.00);

            if (objBenchmarkWeight != null && objPercent != null && objBenchmarkWeight != null)
            {
                //Active benchmark calc
                temp.ActivePosition = Math.Round(Convert.ToDecimal(objPercent - objBenchmarkWeight) * 100, 2);
            }

            if (objBenchmarkWeightInCountry != null && objHoldingInCountry != null && objBenchmarkWeightInCountry != null)
            {
                //Active position in country calc 
                temp.ActivePositionInCountry = Math.Round(Convert.ToDecimal(objHoldingInCountry - objBenchmarkWeightInCountry) * 100, 2);
            }
            return temp;
        }

        /// <summary>
        /// method to get last business date based on WCF view from which to fetch the last date
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns>last business date</returns>
        public DateTime GetLastBusinessDate(string viewName)
        {
            try
            {
                DimensionEntities entity = DimensionEntity;
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);

                switch (viewName)
                {
                    case "PORTFOLIO_HOLDINGS":
                        {
                            GreenField.DAL.GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE)
                                                                                                                .FirstOrDefault();
                            if (lastBusinessRecord != null)
                            {
                                if (lastBusinessRecord.PORTFOLIO_DATE != null)
                                {
                                    lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);
                                }
                            }
                        }
                        break;
                    case "BENCHMARK_HOLDINGS":
                        {
                            //This was causing issues as there are 3 records being posted to the WCF for current day.  Had to add a little filtering. 
                            //GF_BENCHMARK_HOLDINGS lastBusinessRecord = entity.GF_BENCHMARK_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                            GreenField.DAL.GF_BENCHMARK_HOLDINGS lastBusinessRecord =
                                entity.GF_BENCHMARK_HOLDINGS.Where(g => g.BENCHMARK_ID == "MSCI EM NET").OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                            if (lastBusinessRecord != null)
                            {
                                if (lastBusinessRecord.PORTFOLIO_DATE != null)
                                {
                                    lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                return lastBusinessDate;
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