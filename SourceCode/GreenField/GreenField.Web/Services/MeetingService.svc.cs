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

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for Security Reference
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MeetingService 
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
        /// retrieving the security data on ticker filter
        /// </summary>
        /// <returns>list of security overview data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public SecurityInformation RetrieveSecurityDetails(EntitySelectionData entitySelectionData)
        {
            try
            {
                if (entitySelectionData == null)
                    return new SecurityInformation();

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
                    return new SecurityInformation();

                SecurityInformation result = new SecurityInformation()
                {
                    SecurityTicker = data.TICKER,
                    SecurityName = data.ISSUE_NAME,
                    SecurityCountry = data.ISO_COUNTRY_CODE,
                    SecurityIndustry = data.GICS_INDUSTRY_NAME,
                    Analyst = data.ASHMOREEMM_PRIMARY_ANALYST,
                    Price = data.CLOSING_PRICE.ToString(),
                    FVCalc = "3",
                    SecurityBuySellvsCrnt = "$16.50(8*2013PE)-$21.50(10.5*2013PE)",
                    TotalCurrentHoldings = "$0mn",
                    PercentEMIF = "0%",
                    SecurityBMWeight = "1%",
                    SecurityActiveWeight = "Underweight",
                    YTDRet_Absolute = "-3.5%",
                    YTDRet_RELtoLOC = "+8%",
                    YTDRet_RELtoEM = "-2%",
                    SecurityRecommendation = "BUY",
                    SecurityMarketCapitalization = "$634"//resultCap[0].ToString()
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

        /// <summary>
        /// Retrieve Meetings Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> MeetingInfo</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MeetingInfo> GetMeetings()
        {
            List<MeetingInfo> result = new List<MeetingInfo>();

            ICPresentationEntities entity = new ICPresentationEntities();

            int MaxMeetingRecordAppKey = 0;
            if (null != int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]))
            {
                MaxMeetingRecordAppKey = int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]);
            }

            IQueryable<MeetingInfo> queryResult = entity.MeetingInfoes.Select(p => p).Take(MaxMeetingRecordAppKey).OrderByDescending(p => p.MeetingDateTime);

            return queryResult.ToList<MeetingInfo>();
        }


        /// <summary>
        /// Retrieve Meetings Information based on Meeting Date
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> MeetingInfo</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MeetingInfo> GetMeetingsByDate(DateTime meetingDate)
        {
            List<MeetingInfo> result = new List<MeetingInfo>();
            DateTime nextToMeetingDate = meetingDate + TimeSpan.FromDays(1);

            ICPresentationEntities entity = new ICPresentationEntities();

            int MaxMeetingRecordAppKey = 0;
            if (null != int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]))
            {
                MaxMeetingRecordAppKey = int.Parse(ConfigurationManager.AppSettings["maxMeetingRecordAppKey"]);
            }

            IQueryable<MeetingInfo> queryResult = entity.MeetingInfoes.Select(p => p).Where(p => p.MeetingDateTime >= meetingDate && p.MeetingDateTime < nextToMeetingDate).Take(MaxMeetingRecordAppKey);

            return queryResult.ToList<MeetingInfo>();
        }

        /// <summary>
        /// Retrieve Meeting Dates
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> DateTime</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DateTime?> GetMeetingDates()
        {
            try
            {
                List<DateTime?> result = new List<DateTime?>();
                ICPresentationEntities entity = new ICPresentationEntities();
                ObjectResult<DateTime?> queryResult = entity.usp_SelectMeetingInfoMeetingDates();
                return queryResult.ToList<DateTime?>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Create Meeting
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void CreateMeeting(MeetingInfo meetingInfo)
        {
            try
            {

                ICPresentationEntities entity = new ICPresentationEntities();

                entity.usp_InsertMeetingInformation(meetingInfo.MeetingDateTime,
                                                meetingInfo.MeetingClosedDateTime,
                                                    meetingInfo.Description,
                                                        meetingInfo.CreatedBy,
                                                            meetingInfo.CreatedOn,
                                                                meetingInfo.ModifiedBy,
                                                                    meetingInfo.ModifiedOn);

            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Update Meeting 
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void UpdateMeeting(MeetingInfo meetingInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.usp_UpdateMeetingInfo(meetingInfo.MeetingDateTime,
                                                meetingInfo.MeetingClosedDateTime,
                                                    meetingInfo.Description,
                                                        meetingInfo.ModifiedBy,
                                                            meetingInfo.ModifiedOn,
                                                                meetingInfo.MeetingID);

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Retrieve Presentations Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> RetrievePresentationInfo_Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PresentationInfoResult> GetPresentations()
        {
            try
            {

                ICPresentationEntities entity = new ICPresentationEntities();

                ObjectResult<PresentationInfoResult> resultSet = entity.usp_SelectPresentationInfo();
                if (resultSet != null)
                {
                    return resultSet.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Retrieve Presentations Information by Meeting ID
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> RetrievePresentationInfoByMeetingID_Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PresentationInfoResult> GetPresentationsByMeetingID(long meetingID)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();

                ObjectResult<PresentationInfoResult> resultSet = entity.usp_SelectPresentationInfoByMeetingID(meetingID);
                if (resultSet != null)
                {
                    return resultSet.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Retrieve Presentations Information by MeetingDate, Presenter,Status
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> RetrievePresentationInfoByMeetingDatePresenterStatus_Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PresentationInfoResult> GetPresentationsByMeetingDatePresenterStatus(DateTime? meetingDate, string presenter, string status)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();

                ObjectResult<PresentationInfoResult> resultSet = entity.usp_SelectPresentationInfoByMeetingDatePresenterStatus(meetingDate, presenter, status);
                if (resultSet != null)
                {
                    return resultSet.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }


        /// <summary>
        /// Retrieve distinct Presenters
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> RetrievePresentationInfoDistinctPresenters_Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<string> GetDistinctPresenters()
        {
            try
            {
                //List<string> result = new List<string>();
                //EMMGreenFieldEntities1 entity = new EMMGreenFieldEntities1();
                //ObjectResult<string> proxies = entity.usp_SelectPresentationInfoDistinctPresenters();
                //result = proxies.ToList<string>();
                //return result;


                ICPresentationEntities entity = new ICPresentationEntities();
                List<string> result = entity.usp_SelectPresentationInfoDistinctPresenters().ToList<string>();

                if (result != null)
                {
                    return result;
                }
                return null;

                //ObjectResult<string> resultSet = entity.usp_SelectPresentationInfoDistinctPresenters();
                //if (resultSet != null)
                //{
                //    return resultSet.ToList();
                //}
                //return null;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Retrieve Status Type 
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> tblICP_StatusType</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<StatusType> GetStatusTypes()
        {
            ICPresentationEntities entity = new ICPresentationEntities();
            //List<tblICP_StatusType> result = new List<tblICP_StatusType>();
            IQueryable<StatusType> queryResult = entity.StatusTypes.Select(p => p);
            return queryResult.ToList<StatusType>();
        }


        /// <summary>
        /// Update the Presentation Mapping Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void UpdateMeetingPresentationMapping(MeetingPresentationMappingInfo meetingPresentationMappingInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.usp_UpdateMeetingPresentationMappingInfo(meetingPresentationMappingInfo.MeetingID,
                                                                    meetingPresentationMappingInfo.PresentationID,
                                                                        meetingPresentationMappingInfo.ModifedBy,
                                                                            meetingPresentationMappingInfo.ModifiedOn);
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Update the Presentation Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void UpdatePresentation(PresentationInfo presentationInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.usp_UpdatePresentationInfo
                    (
                    presentationInfo.PresentationID,
                    presentationInfo.Presenter,
                    presentationInfo.StatusTypeID,
                    presentationInfo.SecurityTicker,
                    presentationInfo.SecurityName,
                    presentationInfo.SecurityCountry,
                    presentationInfo.SecurityCountryCode,
                    presentationInfo.SecurityIndustry,
                    presentationInfo.SecurityCashPosition,
                    presentationInfo.SecurityPosition,
                    presentationInfo.SecurityMSCIStdWeight,
                    presentationInfo.SecurityMSCIIMIWeight,
                    presentationInfo.SecurityGlobalActiveWeight,
                    presentationInfo.SecurityLastClosingPrice,
                    presentationInfo.SecurityMarketCapitalization,
                    presentationInfo.SecurityPFVMeasure,
                    presentationInfo.SecurityBuyRange,
                    presentationInfo.SecuritySellRange,
                    presentationInfo.SecurityRecommendation,
                    presentationInfo.InvestmentThesis,
                    presentationInfo.Background,
                    presentationInfo.Valuations,
                    presentationInfo.EarningsOutlook,
                    presentationInfo.CompetitiveAdvantage,
                    presentationInfo.CompetitiveDisadvantage,
                    presentationInfo.CommitteePFVMeasure,
                    presentationInfo.CommitteeBuyRange,
                    presentationInfo.CommitteeSellRange,
                    presentationInfo.CommitteeRecommendation,
                    presentationInfo.CommitteeRangeEffectiveThrough,
                    presentationInfo.AcceptWithoutDiscussionFlag,
                    presentationInfo.AdminNotes,
                    presentationInfo.CreatedBy,
                    presentationInfo.CreatedOn,
                    presentationInfo.ModifiedBy,
                    presentationInfo.ModifiedOn);

            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Update the Meeting Presentation Mappoing Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void CreateMeetingPresentationMapping(MeetingPresentationMappingInfo meetingPresentationMappingInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.usp_InsertMeetingPresentationMappingInfo(meetingPresentationMappingInfo.MeetingID,
                                                                    meetingPresentationMappingInfo.PresentationID,
                                                                        meetingPresentationMappingInfo.CreatedBy,
                                                                            meetingPresentationMappingInfo.CreatedOn,
                                                                                meetingPresentationMappingInfo.ModifedBy,
                                                                                    meetingPresentationMappingInfo.ModifiedOn);

            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Create Presentation
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> long </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public long? CreatePresentation(PresentationInfo presentationInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                ObjectResult<decimal?> proxies = entity.usp_InsertPresentationInfo(presentationInfo.Presenter,
                                                                                  presentationInfo.StatusTypeID,
                                                                                    presentationInfo.SecurityTicker,
                                                                                        presentationInfo.SecurityName,
                                                                                            presentationInfo.SecurityCountry,
                                                                                presentationInfo.SecurityCountryCode,
                                                                                    presentationInfo.SecurityIndustry,
                                                                                        presentationInfo.SecurityCashPosition,
                                                                                            presentationInfo.SecurityPosition,
                                                                                                presentationInfo.SecurityMSCIStdWeight,
                                                                                presentationInfo.SecurityMSCIIMIWeight,
                                                                                    presentationInfo.SecurityGlobalActiveWeight,
                                                                                        presentationInfo.SecurityLastClosingPrice,
                                                                                            presentationInfo.SecurityMarketCapitalization,
                                                                                                presentationInfo.SecurityPFVMeasure,
                                                                                presentationInfo.SecurityBuyRange,
                                                                                    presentationInfo.SecuritySellRange,
                                                                                        presentationInfo.SecurityRecommendation,
                                                                                            presentationInfo.InvestmentThesis,
                                                                                                presentationInfo.Background,
                                                                                presentationInfo.Valuations,
                                                                                    presentationInfo.EarningsOutlook,
                                                                                        presentationInfo.CompetitiveAdvantage,
                                                                                            presentationInfo.CompetitiveDisadvantage,
                                                                                                presentationInfo.CommitteePFVMeasure,
                                                                                presentationInfo.CommitteeBuyRange,
                                                                                    presentationInfo.CommitteeSellRange,
                                                                                        presentationInfo.CommitteeRecommendation,
                                                                                            presentationInfo.CommitteeRangeEffectiveThrough,
                                                                                                presentationInfo.AcceptWithoutDiscussionFlag,
                                                                                presentationInfo.AdminNotes,
                                                                                    presentationInfo.CreatedBy,
                                                                                        presentationInfo.CreatedOn,
                                                                                            presentationInfo.ModifiedBy,
                                                                                                presentationInfo.ModifiedOn);
                long? result = null;
                try { result = Convert.ToInt64(proxies.FirstOrDefault().ToString()); }
                catch (Exception) { }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }




        /// <summary>
        /// Insert Voter Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void CreateVoterInfo(VoterInfo voterInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                entity.usp_InsertVoterInfo(voterInfo.PresentationID,
                                                voterInfo.Name,
                                                    voterInfo.Notes,
                                                        voterInfo.VoteTypeID,
                                                            voterInfo.AttendanceTypeID,
                                                                voterInfo.PostMeetingFlag,
                                                                    voterInfo.DiscussionFlag,
                                                                        voterInfo.VoterPFVMeasure,
                                                                            voterInfo.VoterBuyRange,
                                                                                voterInfo.VoterSellRange,
                                                                                    voterInfo.VoterRecommendation,
                                                                                        voterInfo.CreatedBy,
                                                                                            voterInfo.CreatedOn,
                                                                                                voterInfo.ModifiedBy,
                                                                                                    voterInfo.ModifiedOn);
            }

            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Update the Presentation Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public void CreateFileInfo(ObservableCollection<AttachedFileInfo> fileInfoColl)
        {
            try
            {
                foreach (AttachedFileInfo fileInfo in fileInfoColl)
                {
                    ICPresentationEntities entity = new ICPresentationEntities();
                    entity.usp_InsertFileInfo(fileInfo.PresentationID, fileInfo.FileName, fileInfo.FileSerializedData, fileInfo.CreatedBy,
                                                fileInfo.CreatedOn, fileInfo.ModifiedBy, fileInfo.ModifiedOn);
                }
            }

            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Update the Presentation Information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns> void </returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<AttachedFileInfo> GetFileInfo(long presentationID)
        {
            List<AttachedFileInfo> result = new List<AttachedFileInfo>();

            ICPresentationEntities entity = new ICPresentationEntities();

            IQueryable<AttachedFileInfo> proxies = entity.AttachedFileInfoes.Select(p => p).Where(p => p.PresentationID == presentationID);

            result = proxies.ToList<AttachedFileInfo>();

            return result;
        }


    }
}






