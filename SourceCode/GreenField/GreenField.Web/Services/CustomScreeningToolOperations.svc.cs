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
using System.Data;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
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
                        result = (from iry in entity.GF_SECURITY_BASEVIEW
                                  select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.ASEC_SEC_COUNTRY_ZONE_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME).Distinct()
                                  .ToList();
                        break;
                    case "Country":
                        result = (from iry in entity.GF_SECURITY_BASEVIEW
                                  select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.ASEC_SEC_COUNTRY_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME).Distinct()
                                  .ToList();
                        break;
                    case "Sector":
                        result = (from iry in entity.GF_SECURITY_BASEVIEW
                                  select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.GICS_SECTOR_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME).Distinct()
                                  .ToList();
                        break;
                    case "Industry":
                        result = (from iry in entity.GF_SECURITY_BASEVIEW
                                  select new { ASEC_SEC_COUNTRY_ZONE_NAME = iry.GICS_INDUSTRY_NAME }).AsEnumerable().Select(t => t.ASEC_SEC_COUNTRY_ZONE_NAME).Distinct()
                                  .ToList();
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
                List<CustomSelectionData> result = new List<CustomSelectionData>();

                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<SCREENING_DISPLAY_REFERENCE> data = new List<SCREENING_DISPLAY_REFERENCE>();

                data = entity.SCREENING_DISPLAY_REFERENCE.OrderBy(record => record.DATA_DESC).ToList();

                if (data == null || data.Count < 1)
                    return result;

                foreach (SCREENING_DISPLAY_REFERENCE item in data)
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
                    return result;

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
                    return result;

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
                    return result;

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
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean? SaveUserDataPointsPreference(string userPreference, string username)
        {
            try
            {
                bool isSaveSuccessful = true;
                if (userPreference == null || username == null)
                    return false;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

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
        /// <param name="username"></param>
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
                    throw new Exception("Services are not available");

                List<CSTUserPreferenceInfo> result = new List<CSTUserPreferenceInfo>();

                CustomScreeningToolEntities entity = new CustomScreeningToolEntities();
                List<CustomScreeningUserPreferences> data = new List<CustomScreeningUserPreferences>();

                data = entity.GetCustomScreeningUserPreferences(username).ToList();

                if (data == null || data.Count < 1)
                    return result;

                foreach (CustomScreeningUserPreferences item in data)
                {
                    result.Add(new CSTUserPreferenceInfo()
                    {
                        UserName = item.UserName,
                        ScreeningId = item.ScreeningId,
                        DataDescription = item.DataDescription,
                        //LongDescription = item.LONG_DESC,
                        //DataColumn = item.TABLE_COLUMN
                        ListName = item.ListName,
                        TableColumn = item.TableColumn,
                        Accessibility = item.Accessibilty,
                        DataSource = item.DataSource,
                        PeriodType = item.PeriodType,
                        YearType = item.YearType,
                        FromDate = Convert.ToInt32(item.FromDate),
                        ToDate = Convert.ToInt32(item.ToDate),
                        DataPointsOrder = Convert.ToInt32(item.DataPointsOrder)
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
            EntitySelectionData benchmark, String region, String country, String sector, String industry, List<CSTUserPreferenceInfo> userPreference)
        {
            try
            {
                List<CustomScreeningSecurityData> result = new List<CustomScreeningSecurityData>();

                if(userPreference.Count == 1 && userPreference[0].ScreeningId == null)
                {
                    return result;
                }

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities externalEntity = new ExternalResearchEntities();
                CustomScreeningToolEntities cstEntity = new CustomScreeningToolEntities();
                List<CustomScreeningSecurityData> securityList = new List<CustomScreeningSecurityData>();
                securityList = RetrieveSecurityDetailsList(portfolio, benchmark, region, country, sector, industry);

                List<string> distinctSecurityId = securityList.Select(record => record.SecurityId).ToList();
                List<string> distinctIssuerId = securityList.Select(record => record.IssuerId).ToList();

                string _securityIds = StringBuilder(distinctSecurityId);
                string _issuerIds = StringBuilder(distinctIssuerId);

                #region dummy data
                ////dummy data
                //userPreference = new List<CSTUserPreferenceInfo>();
                //userPreference.Add(new CSTUserPreferenceInfo()
                //{
                //    ScreeningId = "FIN001",
                //    DataSource = "REUTERS",
                //    PeriodType = "A",
                //    YearType = "FISCAL",
                //    FromDate = 2009,
                //    DataID = 3,
                //    DataDescription = "ABC"
                //});
                //userPreference.Add(new CSTUserPreferenceInfo()
                //{
                //    ScreeningId = "FIN009",
                //    DataSource = "REUTERS",
                //    PeriodType = "A",
                //    YearType = "FISCAL",
                //    FromDate = 2005,
                //    DataID = 11,
                //    DataDescription = "XYZ"
                //});
                //userPreference.Add(new CSTUserPreferenceInfo()
                //{
                //    ScreeningId = "CUR001",
                //    DataSource = "REUTERS",
                //    DataID = 3,
                //    DataDescription = "ABC"
                //});
                //userPreference.Add(new CSTUserPreferenceInfo()
                //{
                //    ScreeningId = "REF009",
                //    TableColumnName = "SECURITY_VOLUME_AVG_30D"
                //});
                //userPreference.Add(new CSTUserPreferenceInfo()
                //{
                //    ScreeningId = "REF003",
                //    TableColumnName = "ISIN"
                //});
                //userPreference.Add(new CSTUserPreferenceInfo()
                //{
                //    ScreeningId = "FVA001",
                //    DataSource = "REUTERS",
                //    TableColumnName = "DATA_DESC"
                //});
                //userPreference.Add(new CSTUserPreferenceInfo()
                //{
                //    ScreeningId = "FVA002",
                //    DataSource = "REUTERS",
                //    TableColumnName = "FV_BUY"
                //});
                ////**** 
                #endregion

                #region Retrieving REF Data Items

                if (userPreference != null)
                {
                    List<CustomScreeningREFData> data = cstEntity.GetCustomScreeningREFData(_securityIds).ToList();
                    foreach (CSTUserPreferenceInfo item in userPreference)
                    {
                        if (item.ScreeningId != null)
                        {
                            if (item.ScreeningId.StartsWith("REF"))
                            {
                                foreach (CustomScreeningREFData record in data)
                                {
                                    CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();
                                    fillData.SecurityId = record.SECURITY_ID;
                                    fillData.IssueName = securityList.Where(a => a.SecurityId == record.SECURITY_ID).Select(a => a.IssueName).FirstOrDefault();
                                    fillData.Type = cstEntity.SCREENING_DISPLAY_REFERENCE.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.TABLE_COLUMN).FirstOrDefault();//item.TableColumnName;
                                    fillData.Value = record.GetType().GetProperty(fillData.Type).GetValue(record, null);
                                    result.Add(fillData);
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Retrieving FIN Data Items

                foreach (CSTUserPreferenceInfo item in userPreference)
                {
                    if (item.ScreeningId != null)
                    {
                        if (item.ScreeningId.StartsWith("FIN"))
                        {
                            List<CustomScreeningFINData> temp = new List<CustomScreeningFINData>();
                            if (item.PeriodType != null)
                            {
                                if (item.PeriodType.StartsWith("A"))
                                {
                                    temp = cstEntity.GetCustomScreeningFINData(_issuerIds, _securityIds, item.DataID, item.PeriodType.Substring(0, 1), item.FromDate, item.YearType, item.DataSource).ToList(); 
                                }
                                else
                                { 
                                    temp = cstEntity.GetCustomScreeningFINData(_issuerIds, _securityIds, item.DataID, item.PeriodType, item.FromDate, item.YearType, item.DataSource).ToList(); 
                                }

                                foreach (CustomScreeningFINData record in temp)
                                {
                                    CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();
                                    fillData.SecurityId = record.SecurityId;
                                    fillData.IssuerId = record.IssuerId;
                                    fillData.IssueName = securityList.Where(a => a.IssuerId == record.IssuerId || a.SecurityId == record.SecurityId).Select(a => a.IssueName).FirstOrDefault();
                                    fillData.Type = item.DataDescription;
                                    fillData.Multiplier = cstEntity.SCREENING_DISPLAY_PERIOD.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.MULTIPLIER).FirstOrDefault();
                                    decimal _amount = fillData.Multiplier != null ? Convert.ToDecimal(record.Amount * fillData.Multiplier) : record.Amount;
                                    fillData.DataSource = item.DataSource;
                                    fillData.PeriodYear = record.PeriodYear;
                                    fillData.PeriodType = item.PeriodType;
                                    fillData.YearType = item.YearType;
                                    fillData.Decimals = cstEntity.SCREENING_DISPLAY_PERIOD.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.DECIMAL).FirstOrDefault();
                                    fillData.IsPercentage = cstEntity.SCREENING_DISPLAY_PERIOD.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.PERCENTAGE).FirstOrDefault();
                                    _amount = fillData.Decimals != null ? Math.Round(Convert.ToDecimal(_amount), Convert.ToInt16(fillData.Decimals)) : _amount;
                                    fillData.Value = fillData.IsPercentage == "Y" ? Convert.ToString(_amount) + "%" : Convert.ToString(_amount);
                                    result.Add(fillData);
                                }
                            }
                        }
                    }
                }


                #endregion

                #region Retrieving CUR Data Items

                foreach (CSTUserPreferenceInfo item in userPreference)
                {
                    if (item.ScreeningId != null)
                    {
                        if (item.ScreeningId.StartsWith("CUR"))
                        {
                            List<CustomScreeningCURData> temp = new List<CustomScreeningCURData>();
                            temp = cstEntity.GetCustomScreeningCURData(_issuerIds, _securityIds, item.DataID, item.DataSource).ToList();

                            foreach (CustomScreeningCURData record in temp)
                            {
                                CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();
                                fillData.SecurityId = record.SecurityId;
                                fillData.IssuerId = record.IssuerId;
                                fillData.IssueName = securityList.Where(a => a.IssuerId == record.IssuerId || a.SecurityId == record.SecurityId).Select(a => a.IssueName).FirstOrDefault();
                                fillData.Type = item.DataDescription;
                                fillData.Multiplier = cstEntity.SCREENING_DISPLAY_PERIOD.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.MULTIPLIER).FirstOrDefault();
                                decimal _amount = fillData.Multiplier != null ? Convert.ToDecimal(record.Amount * fillData.Multiplier) : record.Amount;
                                fillData.DataSource = item.DataSource;
                                fillData.Decimals = cstEntity.SCREENING_DISPLAY_PERIOD.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.DECIMAL).FirstOrDefault();
                                fillData.IsPercentage = cstEntity.SCREENING_DISPLAY_PERIOD.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.PERCENTAGE).FirstOrDefault();
                                _amount = fillData.Decimals != null ? Math.Round(Convert.ToDecimal(_amount), Convert.ToInt16(fillData.Decimals)) : _amount;
                                fillData.Value = fillData.IsPercentage == "Y" ? Convert.ToString(_amount) + "%" : Convert.ToString(_amount);
                                result.Add(fillData);
                            }
                        }
                    }
                }

                #endregion

                #region Retrieving FVA Data Items

                if (userPreference != null)
                {
                    foreach (CSTUserPreferenceInfo item in userPreference)
                    {
                        if (item.ScreeningId != null)
                        {
                            if (item.ScreeningId.StartsWith("FVA"))
                            {
                                List<CustomScreeningFVAData> data = cstEntity.GetCustomScreeningFVAData(_securityIds, item.DataSource).ToList();
                                foreach (CustomScreeningFVAData record in data)
                                {
                                    CustomScreeningSecurityData fillData = new CustomScreeningSecurityData();
                                    fillData.SecurityId = record.SECURITY_ID;
                                    fillData.IssueName = securityList.Where(a => a.SecurityId == record.SECURITY_ID).Select(a => a.IssueName).FirstOrDefault();
                                    fillData.Type = cstEntity.SCREENING_DISPLAY_FAIRVALUE.Where(a => a.SCREENING_ID == item.ScreeningId).Select(a => a.TABLE_COLUMN).FirstOrDefault();//item.TableColumnName;
                                    fillData.Value = record.GetType().GetProperty(fillData.Type).GetValue(record, null);
                                    result.Add(fillData);
                                }
                            }
                        }
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
        /// Update user preferred Data Points List
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean? UpdateUserDataPointsPreference(string userPreference, string username, string existingListname, string newListname, string accessibility)
        {
            try
            {
                bool isSaveSuccessful = true;
                if (userPreference == null || username == null || existingListname == null || newListname == null || accessibility == null)
                    return false;

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

        #region Helper Methods

        public List<CustomScreeningSecurityData> RetrieveSecurityDetailsList(PortfolioSelectionData portfolio,
            EntitySelectionData benchmark, String region, String country, String sector, String industry)
        {
            try
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
                                                                                     && record.DIRTY_VALUE_PC > 0).ToList();
                    if (securitiesFromPortfolio == null)
                        return securityList;

                    securitiesFromPortfolio = securitiesFromPortfolio.Distinct().ToList();
                    foreach (GF_PORTFOLIO_HOLDINGS item in securitiesFromPortfolio)
                    {
                        GF_SECURITY_BASEVIEW securityIdRow = item.ASEC_SEC_SHORT_NAME != null ? entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == item.ASEC_SEC_SHORT_NAME)
                                                                                                                           .FirstOrDefault() : null;
                        securityList.Add(new CustomScreeningSecurityData()
                        {
                            SecurityId = securityIdRow != null ? (securityIdRow.SECURITY_ID).ToString() : null,
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
                                                                                            && record.PORTFOLIO_DATE == lastBusinessDate).ToList();

                    if (securitiesFromBenchmark == null)
                        return securityList;

                    securitiesFromBenchmark = securitiesFromBenchmark.Distinct().ToList();

                    foreach (GF_BENCHMARK_HOLDINGS item in securitiesFromBenchmark)
                    {
                        GF_SECURITY_BASEVIEW securityIdRow = item.ASEC_SEC_SHORT_NAME != null ? entity.GF_SECURITY_BASEVIEW.Where(a => a.ASEC_SEC_SHORT_NAME == item.ASEC_SEC_SHORT_NAME)
                                                                                                                           .FirstOrDefault() : null;
                        securityList.Add(new CustomScreeningSecurityData()
                        {
                            SecurityId = securityIdRow != null ? (securityIdRow.SECURITY_ID).ToString() : null,
                            IssuerId = item.ISSUER_ID,
                            IssueName = item.ISSUE_NAME
                        });
                    }
                    return securityList;
                }
                else if (region != null)
                {
                    List<GF_SECURITY_BASEVIEW> securitiesInRegion = new List<GF_SECURITY_BASEVIEW>();
                    securitiesInRegion = entity.GF_SECURITY_BASEVIEW.Where(record => record.ASEC_SEC_COUNTRY_ZONE_NAME == region).ToList();
                    if (securitiesInRegion != null)
                    {
                        securitiesInRegion = securitiesInRegion.Distinct().ToList();
                        securitiesFromCustomControls.AddRange(securitiesInRegion);
                    }
                }
                else if (country != null)
                {
                    List<GF_SECURITY_BASEVIEW> securitiesInCountry = new List<GF_SECURITY_BASEVIEW>();
                    securitiesInCountry = entity.GF_SECURITY_BASEVIEW.Where(record => record.ASEC_SEC_COUNTRY_NAME == country).ToList();
                    if (securitiesInCountry != null)
                    {
                        securitiesInCountry = securitiesInCountry.Distinct().ToList();
                        securitiesFromCustomControls.AddRange(securitiesInCountry);
                    }
                }
                else if (sector != null)
                {
                    List<GF_SECURITY_BASEVIEW> securitiesInSector = new List<GF_SECURITY_BASEVIEW>();
                    securitiesInSector = entity.GF_SECURITY_BASEVIEW.Where(record => record.GICS_SECTOR_NAME == sector).ToList();
                    if (securitiesInSector != null)
                    {
                        securitiesInSector = securitiesInSector.Distinct().ToList();
                        securitiesFromCustomControls.AddRange(securitiesInSector);
                    }
                }
                else if (industry != null)
                {
                    List<GF_SECURITY_BASEVIEW> securitiesInIndustry = new List<GF_SECURITY_BASEVIEW>();
                    securitiesInIndustry = entity.GF_SECURITY_BASEVIEW.Where(record => record.GICS_INDUSTRY_NAME == industry).ToList();
                    if (securitiesInIndustry != null)
                    {
                        securitiesInIndustry = securitiesInIndustry.Distinct().ToList();
                        securitiesFromCustomControls.AddRange(securitiesInIndustry);
                    }
                }
                if (securitiesFromCustomControls == null)
                {
                    return securityList;
                }

                securitiesFromCustomControls = securitiesFromCustomControls.Distinct().ToList();
                foreach (GF_SECURITY_BASEVIEW item in securitiesFromCustomControls)
                {
                    securityList.Add(new CustomScreeningSecurityData()
                    {
                        SecurityId = item.SECURITY_ID.ToString(),
                        IssuerId = item.ISSUER_ID,
                        IssueName = item.ISSUE_NAME
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

        #endregion
    }
}