using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GreenField.DAL;
using System.Data;
using GreenField.DataContracts;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using GreenField.Web.Helpers;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using System.Collections.ObjectModel;
using System.Data.Objects;
using GreenField.Web.Services;
using System.Linq;
using System.Configuration;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.DataContracts;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using System.Web.Security;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for Security Reference
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MeetingOperations 
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

        #region Presentation Overview
        /// <summary>
        /// Retrieve Presentations Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> RetrievePresentationInfo_Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<ICPresentationOverviewData> RetrievePresentationOverviewData()
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.RetrieveICPresentationOverviewData().ToList();                
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }
        #endregion

        #region New Presentation
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean CreatePresentation(String userName, ICPresentationOverviewData presentationOverviewData)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();

                XDocument xmlDoc = GetEntityXml<ICPresentationOverviewData>(new List<ICPresentationOverviewData> { presentationOverviewData });
                String[] votingUsers = Roles.GetUsersInRole("IC_MEMBER_VOTING");
                foreach (String user in votingUsers)
                {
                    if (user != userName)
                    {
                        XElement element = new XElement("VotingUser", user);
                        xmlDoc.Root.Add(element);
                    }

                }

                String xmlScript = xmlDoc.ToString();
                Int32? result = entity.SetPresentationInfo(userName, xmlScript).FirstOrDefault();              


                return result == 0;
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
        public ICPresentationOverviewData RetrieveSecurityDetails(EntitySelectionData entitySelectionData,ICPresentationOverviewData presentationOverviewData,PortfolioSelectionData portfolio)
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();

               // ExternalResearchEntities research = new ExternalResearchEntities();
               // ObjectResult<Decimal?> queryResult = research.GetMarketCap();
                //List<Decimal?> resultCap = new List<Decimal?>();
                //resultCap = queryResult.ToList<Decimal?>();

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                //if (securityData == null)
                //    return new ICPresentationOverviewData();
                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> portfolioData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                                                                                            record => record.PORTFOLIO_ID == portfolio.PortfolioId
                                                                                            && record.PORTFOLIO_DATE == Convert.ToDateTime(DateTime.Today.AddDays(-1)))
                                                                                            .ToList();


                decimal? sumDirtyValuePC = portfolioData.Sum(record => record.DIRTY_VALUE_PC);
                GF_PORTFOLIO_HOLDINGS securityInPortfolio = portfolioData.Where(a => a.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID).FirstOrDefault();

                string benchmarkID = portfolioData.Select(a => a.BENCHMARK_ID).FirstOrDefault();

               DimensionEntitiesService.GF_BENCHMARK_HOLDINGS benchmarkData = entity.GF_BENCHMARK_HOLDINGS.Where(
                                                                                                record => record.BENCHMARK_ID == benchmarkID
                                                                                                && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                                                                                                && record.PORTFOLIO_DATE == Convert.ToDateTime(DateTime.Today.AddDays(-1)))
                                                                                                .FirstOrDefault();

               List<NewICPresentationSecurityData> securityDetails = new List<NewICPresentationSecurityData>();
               securityDetails = externalResearchEntity.GetNewICPresentationSecurityData(securityData.SECURITY_ID.ToString()).ToList();

               decimal? tempNAV;
                presentationOverviewData.SecurityTicker = securityData.TICKER;
                presentationOverviewData.SecurityName = securityData.ISSUE_NAME;
                presentationOverviewData.SecurityCountry = securityData.ASEC_SEC_COUNTRY_NAME;
                presentationOverviewData.SecurityIndustry = securityData.GICS_INDUSTRY_NAME;
                presentationOverviewData.Analyst = securityData.ASHMOREEMM_PRIMARY_ANALYST;
                presentationOverviewData.Price = securityData.CLOSING_PRICE.ToString() + securityData.TRADING_CURRENCY.ToString();
                presentationOverviewData.FVCalc = String.Empty;//"3";
                presentationOverviewData.SecurityBuySellvsCrnt = String.Empty; //"$16.50(8*2013PE)-$21.50(10.5*2013PE)";

                if (securityInPortfolio != null && securityInPortfolio.DIRTY_VALUE_PC > 0)
                {
                    presentationOverviewData.CurrentHoldings = "YES";
                    if (sumDirtyValuePC != 0)
                        presentationOverviewData.PercentEMIF = (((securityInPortfolio.DIRTY_VALUE_PC / sumDirtyValuePC) * 100) + "%").ToString();
                    tempNAV = ((securityInPortfolio.DIRTY_VALUE_PC / sumDirtyValuePC) * 100);
                }
                else
                {
                    presentationOverviewData.CurrentHoldings = "No";
                    presentationOverviewData.PercentEMIF = "0%";
                    tempNAV = 0;
                }

                if (benchmarkData != null)
                {
                    presentationOverviewData.SecurityBMWeight = benchmarkData.BENCHMARK_WEIGHT.ToString();
                    tempNAV = (tempNAV - benchmarkData.BENCHMARK_WEIGHT);
                }
                else
                {
                    presentationOverviewData.SecurityBMWeight = "0%";
                    tempNAV = tempNAV - 0;
                }

                presentationOverviewData.SecurityActiveWeight = tempNAV.ToString()+"%";

                presentationOverviewData.YTDRet_Absolute = String.Empty; //"-3.5%";
                presentationOverviewData.YTDRet_RELtoLOC = String.Empty;  //"+8%";
                presentationOverviewData.YTDRet_RELtoEM = String.Empty;// "-2%";
                presentationOverviewData.SecurityRecommendation = String.Empty; //"BUY";
                if (securityDetails != null && securityDetails.Count > 0)
                {
                    presentationOverviewData.SecurityMarketCapitalization = (float)(securityDetails[0].AMOUNT);
                }
                else
                    presentationOverviewData.SecurityMarketCapitalization = null;

                return presentationOverviewData;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        } 
        #endregion

        #region UploadEdit Presentation Documents


        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FileMaster> RetrievePresentationAttachedFileDetails(Int64? presentationID)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.RetrieveICPresentationAttachedFileDetails(presentationID).ToList();
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
        public Boolean UpdatePresentationAttachedFileStreamData(String userName, Int64 presentationId, FileMaster fileMasterInfo, Boolean deletionFlag)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.SetICPresentationAttachedFileInfo(userName, presentationId, fileMasterInfo.Name
                    , fileMasterInfo.SecurityName, fileMasterInfo.SecurityTicker, fileMasterInfo.Location
                    , fileMasterInfo.MetaTags, fileMasterInfo.Category, fileMasterInfo.Type, fileMasterInfo.FileID, deletionFlag);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public Boolean UpdatePresentationAttachedFileStreamData(String userName, Int64 presentationId, String url, FileMaster presentationAttachedFileData)
        //{
        //    try
        //    {
        //        if (presentationAttachedFileData != null)
        //        {
        //            presentationAttachedFileData.Location = url;
        //        }
        //        else
        //        {
        //            String filePath = @"\\10.101.13.146\IC Presentation Documents\" + presentationAttachedFileData.Name;
        //            if (File.Exists(filePath))
        //            {
        //                File.Delete(filePath);
        //            }
        //        }

        //        XDocument xmlDoc = GetEntityXml<FileMaster>(new List<FileMaster> { presentationAttachedFileData }
        //            , strictlyInclusiveProperties: new List<string> { "FileID", "Name", "SecurityName", "SecurityTicker", "Location", "MetaTags", "Type" });
        //        String xmlScript = xmlDoc.ToString();
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        entity.SetICPMeetingAttachedFileInfo(userName, presentationId, xmlScript);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionTrace.LogException(ex);
        //        string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
        //        throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
        //    }
        //}


        #endregion


        #region Meeting Configuration Schedule

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public MeetingConfigurationSchedule GetMeetingConfigSchedule()
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                MeetingConfigurationSchedule result = entity.MeetingConfigurationSchedules.FirstOrDefault();
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
        public List<MeetingInfo> GetAvailablePresentationDates()
        {
            try
            {
                int months = Convert.ToInt16(ConfigurationManager.AppSettings["ConfigurableMeetingMonths"]);

                ICPresentationEntities entity = new ICPresentationEntities();
                MeetingConfigurationSchedule meetingConfigurationSchedule = entity.MeetingConfigurationSchedules.FirstOrDefault();

                DateTime presentationDateTime = meetingConfigurationSchedule.PresentationDateTime;
                DateTime preMeetingVotingDeadline = meetingConfigurationSchedule.PreMeetingVotingDeadline;
                DateTime presentationDeadline = meetingConfigurationSchedule.PresentationDeadline;

                List<MeetingInfo> result = new List<MeetingInfo>();

                for (DateTime id = DateTime.Now; id < DateTime.Now.AddMonths(months); id = id.AddDays(7))
                {
                    DateTime tempPresentationDeadline = id.Date.Add(presentationDeadline.TimeOfDay);

                    if (tempPresentationDeadline < DateTime.Now)
                        continue;

                    while (tempPresentationDeadline.DayOfWeek != presentationDeadline.DayOfWeek)
                        tempPresentationDeadline = tempPresentationDeadline.AddDays(1);

                    DateTime tempPreMeetingVotingDeadline = tempPresentationDeadline.Date.Add(preMeetingVotingDeadline.TimeOfDay);
                    while (tempPreMeetingVotingDeadline.DayOfWeek != preMeetingVotingDeadline.DayOfWeek)
                        tempPreMeetingVotingDeadline = tempPreMeetingVotingDeadline.AddDays(1);

                    DateTime meetingDateTime = tempPreMeetingVotingDeadline.Date.Add(presentationDateTime.TimeOfDay);
                    while (meetingDateTime.DayOfWeek != presentationDateTime.DayOfWeek)
                        meetingDateTime = meetingDateTime.AddDays(1);

                    result.Add(new MeetingInfo()
                    {
                        MeetingDateTime = meetingDateTime,
                        MeetingClosedDateTime = tempPresentationDeadline,
                        MeetingVotingClosedDateTime = tempPreMeetingVotingDeadline
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean UpdateMeetingConfigSchedule(String userName, MeetingConfigurationSchedule meetingConfigurationSchedule)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetMeetingConfigSchedule(meetingConfigurationSchedule.PresentationDateTime,
                                                            meetingConfigurationSchedule.PresentationTimeZone,
                                                            meetingConfigurationSchedule.PresentationDeadline,
                                                            meetingConfigurationSchedule.PreMeetingVotingDeadline,
                                                            userName).FirstOrDefault();

                return result == 0;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        } 
        #endregion        

        #region Decision Entry
        /// <summary>
        /// Retrieve Voter Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> RetrievePresentationInfo_Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<VoterInfo> RetrievePresentationVoterData(Int64 presentationId)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.VoterInfoes.Where(record => record.PresentationID == presentationId).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Retrieve Security PFVMeasure Current Prices
        /// </summary>
        /// <param name="securityId">Security ID</param>
        /// <param name="pfvTypeInfo">List of PFV Types</param>
        /// <returns>Dictionary element with PFVType as key and prices as values</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Dictionary<String, Decimal?> RetrieveSecurityPFVMeasureCurrentPrices(String securityId, List<String> pfvTypeInfo)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                Dictionary<String, Decimal?> result = new Dictionary<string, decimal?>();
                foreach (String pfvType in pfvTypeInfo)
                {
                    Decimal? pfvTypeCurrentPrice = entity.RetrieveSecurityPFVMeasureCurrentPrice(securityId, pfvType).FirstOrDefault();
                    if (!result.Any(record => record.Key == pfvType))
                    {
                        result.Add(pfvType, pfvTypeCurrentPrice); 
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean UpdateDecisionEntryDetails(String userName, ICPresentationOverviewData presentationOverViewData, List<VoterInfo> voterInfo)
        {
            try
            {
                XDocument xmlDoc = GetEntityXml<ICPresentationOverviewData>(new List<ICPresentationOverviewData> { presentationOverViewData }
                    , strictlyInclusiveProperties: new List<string> { "PresentationID", "AdminNotes", "CommitteePFVMeasure", "CommitteeBuyRange",
                    "CommitteeSellRange" });
                xmlDoc = GetEntityXml<VoterInfo>(parameters: voterInfo, xmlDoc: xmlDoc, strictlyInclusiveProperties: new List<string> { "VoterID",
                    "VoterPFVMeasure", "VoterBuyRange", "VoterSellRange", "VoteType" });
                String xmlScript = xmlDoc.ToString();
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetICPresentationDecisionEntryDetails(userName, xmlScript).FirstOrDefault();
                return result == 0;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #endregion

        #region Meeting Minutes
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MeetingInfo> RetrieveMeetingInfoByPresentationStatus(String presentationStatus)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.RetrieveICMeetingInfoByStatusType(presentationStatus).ToList();
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
        public List<MeetingMinuteData> RetrieveMeetingMinuteDetails(Int64? meetingID)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.RetrieveICPMeetingMinuteDetails(meetingID).ToList();
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
        public List<FileMaster> RetrieveMeetingAttachedFileDetails(Int64? meetingID)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.RetrieveICPMeetingAttachedFileDetails(meetingID).ToList();
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
        public Boolean UpdateMeetingMinuteDetails(String userName, MeetingInfo meetingInfo, List<MeetingMinuteData> meetingMinuteData)
        {
            try
            {
                XDocument xmlDoc = GetEntityXml<MeetingInfo>(new List<MeetingInfo> { meetingInfo });
                xmlDoc = GetEntityXml<MeetingMinuteData>(parameters: meetingMinuteData, xmlDoc: xmlDoc);
                String xmlScript = xmlDoc.ToString();
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.SetICPMeetingMinutesDetails(userName, xmlScript);
                return true;
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
        public Boolean UpdateMeetingAttachedFileStreamData(String userName, Int64 meetingId, MeetingAttachedFileStreamData meetingAttachedFileStreamData)
        {
            try
            {
                if (meetingAttachedFileStreamData.FileStream != null)
                {
                    String filePath = @"\\10.101.13.146\IC Presentation Documents\" + meetingAttachedFileStreamData.MeetingAttachedFileData.Name;
                    File.WriteAllBytes(filePath, meetingAttachedFileStreamData.FileStream);
                    meetingAttachedFileStreamData.MeetingAttachedFileData.Location = @"\\10.101.13.146\IC Presentation Documents\";
                }
                else
                {
                    String filePath = @"\\10.101.13.146\IC Presentation Documents\" + meetingAttachedFileStreamData.MeetingAttachedFileData.Name;
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                XDocument xmlDoc = GetEntityXml<FileMaster>(new List<FileMaster> { meetingAttachedFileStreamData.MeetingAttachedFileData }
                    , strictlyInclusiveProperties: new List<string> { "FileID", "Name", "SecurityName", "SecurityTicker", "Location", "MetaTags", "Category", "Type" });
                String xmlScript = xmlDoc.ToString();
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.SetICPMeetingAttachedFileInfo(userName, meetingId, xmlScript);
                return true;
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
        public Boolean SetMeetingPresentationStatus(String userName, Int64 meetingId, String status)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetICPMeetingPresentationStatus(userName, meetingId, status).FirstOrDefault();
                return result == 0;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        private XDocument GetEntityXml<T>(List<T> parameters, XDocument xmlDoc = null, List<String> strictlyInclusiveProperties = null)
        {
            XElement root;
            if (xmlDoc == null)
            {
                root = new XElement("Root");
                xmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            }
            else
            {
                root = xmlDoc.Root;
            }

            try
            {
                foreach (T item in parameters)
                {
                    XElement node = new XElement(typeof(T).Name);
                    PropertyInfo[] propertyInfo = typeof(T).GetProperties();
                    foreach (PropertyInfo prop in propertyInfo)
                    {
                        if (strictlyInclusiveProperties != null)
                        {
                            if (!strictlyInclusiveProperties.Contains(prop.Name))
                                continue;
                        }

                        if (prop.GetValue(item, null) != null)
                        {
                            node.Add(new XAttribute(prop.Name, prop.GetValue(item, null)));
                        }
                    }

                    root.Add(node);
                }
            }
            catch (Exception)
            {

                throw;
            }


            return xmlDoc;
        }  
        #endregion

        #region Presentation Vote
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CommentInfo> RetrievePresentationComments(Int64 presentationId)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.CommentInfoes.Where(record => record.PresentationID == presentationId)
                    .OrderByDescending(record => record.CommentOn).ToList();
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
        public List<CommentInfo> SetPresentationComments(string userName, Int64 presentationId, String comment)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.SetICPresentationComments(userName, presentationId, comment).ToList();
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
        public Boolean UpdatePreMeetingVoteDetails(String userName, List<VoterInfo> voterInfo)
        {
            try
            {               
                XDocument xmlDoc = GetEntityXml<VoterInfo>(parameters: voterInfo, strictlyInclusiveProperties: new List<string> { "VoterID",
                    "VoterPFVMeasure", "VoterBuyRange", "VoterSellRange", "VoteType", "Notes", "DiscussionFlag"});
                String xmlScript = xmlDoc.ToString();
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetICPresentationVoteDetails(userName, xmlScript).FirstOrDefault();
                return result == 0;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion

        #region Change Date
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean UpdateMeetingPresentationDate(String userName, Int64 presentationId, MeetingInfo meetingInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetICPMeetingPresentationDate(userName, presentationId, meetingInfo.MeetingID
                    , meetingInfo.MeetingDateTime, meetingInfo.MeetingClosedDateTime, meetingInfo.MeetingVotingClosedDateTime).FirstOrDefault();
                return result == 0;
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






