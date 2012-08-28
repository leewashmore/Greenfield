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

        ///// <summary>
        ///// retrieving the security data on ticker filter
        ///// </summary>
        ///// <returns>list of security overview data</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public SecurityInformation RetrieveSecurityDetails(EntitySelectionData entitySelectionData)
        //{
        //    try
        //    {
        //        if (entitySelectionData == null)
        //            return new SecurityInformation();

        //        DimensionEntitiesService.Entities entity = DimensionEntity;
               
        //        ExternalResearchEntities research = new ExternalResearchEntities();
        //        ObjectResult<Decimal?> queryResult = research.GetMarketCap();
        //        List<Decimal?> resultCap = new List<Decimal?>();
        //        resultCap = queryResult.ToList<Decimal?>();
                

        //        bool isServiceUp;
        //        isServiceUp = CheckServiceAvailability.ServiceAvailability();

        //        if (!isServiceUp)
        //            throw new Exception("Services are not available");

        //        DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
        //            .Where(record => record.TICKER == entitySelectionData.ShortName
        //                && record.ISSUE_NAME == entitySelectionData.LongName
        //                && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
        //                && record.SECURITY_TYPE == entitySelectionData.SecurityType)
        //            .FirstOrDefault();              
                

        //        if (data == null)
        //            return new SecurityInformation();

        //        SecurityInformation result = new SecurityInformation()
        //        {
        //            SecurityTicker = data.TICKER,
        //            SecurityName = data.ISSUE_NAME,
        //            SecurityCountry = data.ISO_COUNTRY_CODE,
        //            SecurityIndustry = data.GICS_INDUSTRY_NAME,
        //            Analyst = data.ASHMOREEMM_PRIMARY_ANALYST,
        //            Price = data.CLOSING_PRICE.ToString(),
        //            FVCalc = "3",
        //            SecurityBuySellvsCrnt = "$16.50(8*2013PE)-$21.50(10.5*2013PE)",
        //            TotalCurrentHoldings = "$0mn",
        //            PercentEMIF = "0%",
        //            SecurityBMWeight = "1%",
        //            SecurityActiveWeight = "Underweight",
        //            YTDRet_Absolute = "-3.5%",
        //            YTDRet_RELtoLOC = "+8%",
        //            YTDRet_RELtoEM = "-2%",
        //            SecurityRecommendation = "BUY",
        //            SecurityMarketCapitalization = "$634"//resultCap[0].ToString()
        //        };

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionTrace.LogException(ex);
        //        string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
        //        throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
        //    }
        //}

        ///// <summary>
        ///// Retrieve Meetings Information
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> MeetingInfo</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<MeetingInfo> GetMeetings()
        //{
        //    List<MeetingInfo> result = new List<MeetingInfo>();

        //    ICPresentationEntities entity = new ICPresentationEntities();

        //    int MaxMeetingRecordAppKey = 0;
        //    if (null != int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]))
        //    {
        //        MaxMeetingRecordAppKey = int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]);
        //    }

        //    IQueryable<MeetingInfo> queryResult = entity.MeetingInfoes.Select(p => p).Take(MaxMeetingRecordAppKey).OrderByDescending(p => p.MeetingDateTime);

        //    return queryResult.ToList<MeetingInfo>();
        //}


        ///// <summary>
        ///// Retrieve Meetings Information based on Meeting Date
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> MeetingInfo</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<MeetingInfo> GetMeetingsByDate(DateTime meetingDate)
        //{
        //    List<MeetingInfo> result = new List<MeetingInfo>();
        //    DateTime nextToMeetingDate = meetingDate + TimeSpan.FromDays(1);

        //    ICPresentationEntities entity = new ICPresentationEntities();

        //    int MaxMeetingRecordAppKey = 0;
        //    if (null != int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]))
        //    {
        //        MaxMeetingRecordAppKey = int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]);
        //    }

        //    IQueryable<MeetingInfo> queryResult = entity.MeetingInfoes.Select(p => p).Where(p => p.MeetingDateTime >= meetingDate && p.MeetingDateTime < nextToMeetingDate).Take(MaxMeetingRecordAppKey);

        //    return queryResult.ToList<MeetingInfo>();
        //}

        ///// <summary>
        ///// Retrieve Meeting Dates
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> DateTime</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<DateTime?> GetMeetingDates()
        //{
        //    try
        //    {
        //        List<DateTime?> result = new List<DateTime?>();
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        ObjectResult<DateTime?> queryResult = entity.usp_SelectMeetingInfoMeetingDates();
        //        return queryResult.ToList<DateTime?>();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Create Meeting
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public void CreateMeeting(MeetingInfo meetingInfo)
        //{
        //    try
        //    {

        //        ICPresentationEntities entity = new ICPresentationEntities();

        //        entity.usp_InsertMeetingInformation(meetingInfo.MeetingDateTime,
        //                                        meetingInfo.MeetingClosedDateTime,
        //                                            meetingInfo.MeetingDescription,
        //                                                meetingInfo.CreatedBy,
        //                                                    meetingInfo.CreatedOn,
        //                                                        meetingInfo.ModifiedBy,
        //                                                            meetingInfo.ModifiedOn);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        ///// <summary>
        ///// Update Meeting 
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public void UpdateMeeting(MeetingInfo meetingInfo)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        entity.usp_UpdateMeetingInfo(meetingInfo.MeetingDateTime,
        //                                        meetingInfo.MeetingClosedDateTime,
        //                                            meetingInfo.MeetingDescription,
        //                                                meetingInfo.ModifiedBy,
        //                                                    meetingInfo.ModifiedOn,
        //                                                        meetingInfo.MeetingID);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        

        ///// <summary>
        ///// Retrieve Presentations Information by Meeting ID
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> RetrievePresentationInfoByMeetingID_Result</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<PresentationInfoResult> GetPresentationsByMeetingID(long meetingID)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();

        //        ObjectResult<PresentationInfoResult> resultSet = entity.usp_SelectPresentationInfoByMeetingID(meetingID);
        //        if (resultSet != null)
        //        {
        //            return resultSet.ToList();
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionTrace.LogException(ex);
        //        string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
        //        throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
        //    }

        //}

        ///// <summary>
        ///// Retrieve Presentations Information by MeetingDate, Presenter,Status
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> RetrievePresentationInfoByMeetingDatePresenterStatus_Result</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<PresentationInfoResult> GetPresentationsByMeetingDatePresenterStatus(DateTime? meetingDate, string presenter, string status)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();

        //        ObjectResult<PresentationInfoResult> resultSet = entity.usp_SelectPresentationInfoByMeetingDatePresenterStatus(meetingDate, presenter, status);
        //        if (resultSet != null)
        //        {
        //            return resultSet.ToList();
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionTrace.LogException(ex);
        //        string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
        //        throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
        //    }

        //}


        ///// <summary>
        ///// Retrieve distinct Presenters
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> RetrievePresentationInfoDistinctPresenters_Result</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<string> GetDistinctPresenters()
        //{
        //    try
        //    {
        //        //List<string> result = new List<string>();
        //        //EMMGreenFieldEntities1 entity = new EMMGreenFieldEntities1();
        //        //ObjectResult<string> proxies = entity.usp_SelectPresentationInfoDistinctPresenters();
        //        //result = proxies.ToList<string>();
        //        //return result;


        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        List<string> result = entity.usp_SelectPresentationInfoDistinctPresenters().ToList<string>();

        //        if (result != null)
        //        {
        //            return result;
        //        }
        //        return null;

        //        //ObjectResult<string> resultSet = entity.usp_SelectPresentationInfoDistinctPresenters();
        //        //if (resultSet != null)
        //        //{
        //        //    return resultSet.ToList();
        //        //}
        //        //return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionTrace.LogException(ex);
        //        string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
        //        throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
        //    }
        //}

        ///// <summary>
        ///// Retrieve Status Type 
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> tblICP_StatusType</returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<StatusType> GetStatusTypes()
        //{
        //    ICPresentationEntities entity = new ICPresentationEntities();
        //    //List<tblICP_StatusType> result = new List<tblICP_StatusType>();
        //    IQueryable<StatusType> queryResult = entity.StatusTypes.Select(p => p);
        //    return queryResult.ToList<StatusType>();
        //}


        ///// <summary>
        ///// Update the Presentation Mapping Information
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public void UpdateMeetingPresentationMapping(MeetingPresentationMappingInfo meetingPresentationMappingInfo)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        entity.usp_UpdateMeetingPresentationMappingInfo(meetingPresentationMappingInfo.MeetingID,
        //                                                            meetingPresentationMappingInfo.PresentationID,
        //                                                                meetingPresentationMappingInfo.ModifedBy,
        //                                                                    meetingPresentationMappingInfo.ModifiedOn);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        ///// <summary>
        ///// Update the Presentation Information
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public void UpdatePresentation(PresentationInfo presentationInfo)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        entity.usp_UpdatePresentationInfo
        //            (
        //            presentationInfo.PresentationID,
        //            presentationInfo.Presenter,
        //            presentationInfo.StatusTypeID,
        //            presentationInfo.SecurityTicker,
        //            presentationInfo.SecurityName,
        //            presentationInfo.SecurityCountry,
        //            presentationInfo.SecurityCountryCode,
        //            presentationInfo.SecurityIndustry,
        //            presentationInfo.SecurityCashPosition,
        //            presentationInfo.SecurityPosition,
        //            presentationInfo.SecurityMSCIStdWeight,
        //            presentationInfo.SecurityMSCIIMIWeight,
        //            presentationInfo.SecurityGlobalActiveWeight,
        //            presentationInfo.SecurityLastClosingPrice,
        //            presentationInfo.SecurityMarketCapitalization,
        //            presentationInfo.SecurityPFVMeasure,
        //            presentationInfo.SecurityBuyRange,
        //            presentationInfo.SecuritySellRange,
        //            presentationInfo.SecurityRecommendation,
        //            presentationInfo.InvestmentThesis,
        //            presentationInfo.Background,
        //            presentationInfo.Valuations,
        //            presentationInfo.EarningsOutlook,
        //            presentationInfo.CompetitiveAdvantage,
        //            presentationInfo.CompetitiveDisadvantage,
        //            presentationInfo.CommitteePFVMeasure,
        //            presentationInfo.CommitteeBuyRange,
        //            presentationInfo.CommitteeSellRange,
        //            presentationInfo.CommitteeRecommendation,
        //            presentationInfo.CommitteeRangeEffectiveThrough,
        //            presentationInfo.AcceptWithoutDiscussionFlag,
        //            presentationInfo.AdminNotes,
        //            presentationInfo.CreatedBy,
        //            presentationInfo.CreatedOn,
        //            presentationInfo.ModifiedBy,
        //            presentationInfo.ModifiedOn);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        ///// <summary>
        ///// Update the Meeting Presentation Mappoing Information
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public void CreateMeetingPresentationMapping(MeetingPresentationMappingInfo meetingPresentationMappingInfo)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        entity.usp_InsertMeetingPresentationMappingInfo(meetingPresentationMappingInfo.MeetingID,
        //                                                            meetingPresentationMappingInfo.PresentationID,
        //                                                                meetingPresentationMappingInfo.CreatedBy,
        //                                                                    meetingPresentationMappingInfo.CreatedOn,
        //                                                                        meetingPresentationMappingInfo.ModifedBy,
        //                                                                            meetingPresentationMappingInfo.ModifiedOn);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



        ///// <summary>
        ///// Create Presentation
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> long </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public long? CreatePresentation(PresentationInfo presentationInfo)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        ObjectResult<decimal?> proxies = entity.usp_InsertPresentationInfo(presentationInfo.Presenter,
        //                                                                          presentationInfo.StatusTypeID,
        //                                                                            presentationInfo.SecurityTicker,
        //                                                                                presentationInfo.SecurityName,
        //                                                                                    presentationInfo.SecurityCountry,
        //                                                                        presentationInfo.SecurityCountryCode,
        //                                                                            presentationInfo.SecurityIndustry,
        //                                                                                presentationInfo.SecurityCashPosition,
        //                                                                                    presentationInfo.SecurityPosition,
        //                                                                                        presentationInfo.SecurityMSCIStdWeight,
        //                                                                        presentationInfo.SecurityMSCIIMIWeight,
        //                                                                            presentationInfo.SecurityGlobalActiveWeight,
        //                                                                                presentationInfo.SecurityLastClosingPrice,
        //                                                                                    presentationInfo.SecurityMarketCapitalization,
        //                                                                                        presentationInfo.SecurityPFVMeasure,
        //                                                                        presentationInfo.SecurityBuyRange,
        //                                                                            presentationInfo.SecuritySellRange,
        //                                                                                presentationInfo.SecurityRecommendation,
        //                                                                                    presentationInfo.InvestmentThesis,
        //                                                                                        presentationInfo.Background,
        //                                                                        presentationInfo.Valuations,
        //                                                                            presentationInfo.EarningsOutlook,
        //                                                                                presentationInfo.CompetitiveAdvantage,
        //                                                                                    presentationInfo.CompetitiveDisadvantage,
        //                                                                                        presentationInfo.CommitteePFVMeasure,
        //                                                                        presentationInfo.CommitteeBuyRange,
        //                                                                            presentationInfo.CommitteeSellRange,
        //                                                                                presentationInfo.CommitteeRecommendation,
        //                                                                                    presentationInfo.CommitteeRangeEffectiveThrough,
        //                                                                                        presentationInfo.AcceptWithoutDiscussionFlag,
        //                                                                        presentationInfo.AdminNotes,
        //                                                                            presentationInfo.CreatedBy,
        //                                                                                presentationInfo.CreatedOn,
        //                                                                                    presentationInfo.ModifiedBy,
        //                                                                                        presentationInfo.ModifiedOn);
        //        long? result = null;
        //        try { result = Convert.ToInt64(proxies.FirstOrDefault().ToString()); }
        //        catch (Exception) { }

        //        return result;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}




        ///// <summary>
        ///// Insert Voter Information
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public void CreateVoterInfo(VoterInfo voterInfo)
        //{
        //    try
        //    {
        //        ICPresentationEntities entity = new ICPresentationEntities();
        //        entity.usp_InsertVoterInfo(voterInfo.PresentationID,
        //                                        voterInfo.Name,
        //                                            voterInfo.Notes,
        //                                                voterInfo.VoteTypeID,
        //                                                    voterInfo.AttendanceTypeID,
        //                                                        voterInfo.PostMeetingFlag,
        //                                                            voterInfo.DiscussionFlag,
        //                                                                voterInfo.VoterPFVMeasure,
        //                                                                    voterInfo.VoterBuyRange,
        //                                                                        voterInfo.VoterSellRange,
        //                                                                            voterInfo.VoterRecommendation,
        //                                                                                voterInfo.CreatedBy,
        //                                                                                    voterInfo.CreatedOn,
        //                                                                                        voterInfo.ModifiedBy,
        //                                                                                            voterInfo.ModifiedOn);
        //    }

        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



        ///// <summary>
        ///// Update the Presentation Information
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public void CreateFileInfo(ObservableCollection<AttachedFileInfo> fileInfoColl)
        //{
        //    try
        //    {
        //        foreach (AttachedFileInfo fileInfo in fileInfoColl)
        //        {
        //            ICPresentationEntities entity = new ICPresentationEntities();
        //            entity.usp_InsertFileInfo(fileInfo.PresentationID, fileInfo.FileName, fileInfo.FileSerializedData, fileInfo.CreatedBy,
        //                                        fileInfo.CreatedOn, fileInfo.ModifiedBy, fileInfo.ModifiedOn);
        //        }
        //    }

        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



        ///// <summary>
        ///// Update the Presentation Information
        ///// </summary>
        ///// <param name="objUserID"></param>
        ///// <returns> void </returns>
        //[OperationContract]
        //[FaultContract(typeof(ServiceFault))]
        //public List<AttachedFileInfo> GetFileInfo(long presentationID)
        //{
        //    List<AttachedFileInfo> result = new List<AttachedFileInfo>();

        //    ICPresentationEntities entity = new ICPresentationEntities();

        //    IQueryable<AttachedFileInfo> proxies = entity.AttachedFileInfoes.Select(p => p).Where(p => p.PresentationID == presentationID);

        //    result = proxies.ToList<AttachedFileInfo>();

        //    return result;
        //}

        //----------------------------------------------------------------------------------------------------------------------------------------

        

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
        public ICPresentationOverviewData RetrieveSecurityDetails(EntitySelectionData entitySelectionData, ICPresentationOverviewData presentationOverviewData)
        {
            try
            {
                if (entitySelectionData == null)
                    return presentationOverviewData;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                ExternalResearchEntities research = new ExternalResearchEntities();
                ObjectResult<Decimal?> queryResult = research.GetMarketCap();
                List<Decimal?> resultCap = new List<Decimal?>();
                resultCap = queryResult.ToList<Decimal?>();


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
                    return new ICPresentationOverviewData();

                presentationOverviewData.SecurityTicker = data.TICKER;
                presentationOverviewData.SecurityName = data.ISSUE_NAME;
                presentationOverviewData.SecurityCountry = data.ISO_COUNTRY_CODE;
                presentationOverviewData.SecurityIndustry = data.GICS_INDUSTRY_NAME;
                presentationOverviewData.Analyst = data.ASHMOREEMM_PRIMARY_ANALYST;
                presentationOverviewData.Price = data.CLOSING_PRICE.ToString();
                presentationOverviewData.FVCalc = String.Empty;//"3";
                presentationOverviewData.SecurityBuySellvsCrnt = String.Empty; //"$16.50(8*2013PE)-$21.50(10.5*2013PE)";
                presentationOverviewData.CurrentHoldings = String.Empty; //"$0mn";
                presentationOverviewData.PercentEMIF = String.Empty; //"0%";
                presentationOverviewData.SecurityBMWeight = String.Empty; //"1%";
                presentationOverviewData.SecurityActiveWeight = String.Empty;// "Underweight";
                presentationOverviewData.YTDRet_Absolute = String.Empty; //"-3.5%";
                presentationOverviewData.YTDRet_RELtoLOC = String.Empty;  //"+8%";
                presentationOverviewData.YTDRet_RELtoEM = String.Empty;// "-2%";
                presentationOverviewData.SecurityRecommendation = String.Empty; //"BUY";
                presentationOverviewData.SecurityMarketCapitalization = 0;
                

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
        public List<MeetingAttachedFileData> RetrieveMeetingAttachedFileDetails(Int64? meetingID)
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
        public Boolean UpdateMeetingAttachedFileStreamData(String userName, MeetingAttachedFileStreamData meetingAttachedFileStreamData)
        {
            try
            {
                if (meetingAttachedFileStreamData.FileStream != null)
                {
                    String filePath = @"\\10.101.13.146\IC Presentation Documents\" + meetingAttachedFileStreamData.MeetingAttachedFileData.FileName;
                    File.WriteAllBytes(filePath, meetingAttachedFileStreamData.FileStream);
                    meetingAttachedFileStreamData.MeetingAttachedFileData.FileLocation = @"\\10.101.13.146\IC Presentation Documents\";
                }

                XDocument xmlDoc = GetEntityXml<MeetingAttachedFileData>(new List<MeetingAttachedFileData> { meetingAttachedFileStreamData.MeetingAttachedFileData });
                String xmlScript = xmlDoc.ToString();
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.SetICPMeetingAttachedFileInfo(userName, xmlScript);
                return true;
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

    }
}






