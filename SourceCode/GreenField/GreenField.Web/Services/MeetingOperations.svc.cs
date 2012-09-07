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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Presentation;

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

                #region Creation of PresentationInfo and VoterInfo objects
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
                Int64? result = entity.SetPresentationInfo(userName, xmlScript).FirstOrDefault();

                if (result < 0)
                    return false; 
                #endregion

                String presentationFile = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\Templates\ICPresentationTemplate.pptx";

                if (!File.Exists(presentationFile))
                    throw new Exception("Missing IC Presentation Template file");
                
                String copiedFilePath = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_ICPresentation.pptx";
                File.Copy(presentationFile, copiedFilePath, true);

                try
                {
                    using (PresentationDocument presentationDocument = PresentationDocument.Open(copiedFilePath, true))
                    {
                        PresentationPart presentatioPart = presentationDocument.PresentationPart;
                        OpenXmlElementList slideIds = presentatioPart.Presentation.SlideIdList.ChildElements;

                        string relId = (slideIds[0] as SlideId).RelationshipId;

                        // Get the slide part from the relationship ID.
                        SlidePart slidePart = (SlidePart)presentatioPart.GetPartById(relId);

                        SetSlideTitle(slidePart, presentationOverviewData);
                        SetSlideContent(slidePart, presentationOverviewData);

                        //save the Slide and Presentation
                        slidePart.Slide.Save();
                        presentatioPart.Presentation.Save();

                    }
                }
                catch
                {
                    throw new Exception("Exception occurred while opening powerpoint presentation!!!");
                }

                Byte[] fileStream = File.ReadAllBytes(copiedFilePath);
                String fileName = presentationOverviewData.SecurityName + "_" + (presentationOverviewData.MeetingDateTime.HasValue
                    ? Convert.ToDateTime(presentationOverviewData.MeetingDateTime).ToString("ddMMyyyy") : String.Empty) + ".pptx";

                DocumentWorkspaceOperations documentWorkspaceOperations = new DocumentWorkspaceOperations();
                String url = documentWorkspaceOperations.UploadDocument(fileName, File.ReadAllBytes(copiedFilePath), String.Empty);

                if (url == String.Empty)
                    throw new Exception("Exception occurred while uploading template powerpoint presentation!!!");

                File.Delete(copiedFilePath);

                FileMaster fileMaster = new FileMaster()
                {
                    Category = "Power Point Presentation",
                    Location = url,
                    Name = presentationOverviewData.SecurityName + "_" + Convert.ToDateTime(presentationOverviewData.MeetingDateTime).ToString("ddMMyyyy") + ".pptx",
                    SecurityName = presentationOverviewData.SecurityName,
                    SecurityTicker = presentationOverviewData.SecurityTicker,
                    Type = EnumUtils.ToString(DocumentCategoryType.IC_PRESENTATIONS),
                    CreatedBy = userName,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = userName,
                    ModifiedOn = DateTime.UtcNow
                };
                return UpdatePresentationAttachedFileStreamData(userName, Convert.ToInt64(result), fileMaster, false);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
               

        // Get the title string of the slide.
        private void SetSlideTitle(SlidePart slidePart, ICPresentationOverviewData presentationOverviewData)
        {
            if (slidePart == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            // Declare a paragraph separator.
            string paragraphSeparator = null;

            if (slidePart.Slide != null)
            {
                // Find all the title shapes.
                var shapes = from shape in slidePart.Slide.Descendants<Shape>()
                             where IsTitleShape(shape)
                             select shape;

                StringBuilder paragraphText = new StringBuilder();

                foreach (var shape in shapes)
                {
                    // Get the text in each paragraph in this shape.
                    foreach (var paragraph in shape.TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        // Add a line break.
                        paragraphText.Append(paragraphSeparator);

                        foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            text.Text = presentationOverviewData.SecurityName + " - " + presentationOverviewData.SecurityRecommendation;
                        }

                        paragraphSeparator = "\n";
                    }
                }
            }
        }

        // Get the title string of the slide.
        private void SetSlideContent(SlidePart slidePart, ICPresentationOverviewData presentationOverviewData)
        {
            //get Security Information    
            DocumentFormat.OpenXml.Drawing.Table tblSecurityOverview = slidePart.Slide
                .Descendants<DocumentFormat.OpenXml.Drawing.Table>().FirstOrDefault();

            SwapPlaceholderText(tblSecurityOverview, 0, 2, presentationOverviewData.Analyst);
            SwapPlaceholderText(tblSecurityOverview, 0, 4, presentationOverviewData.CurrentHoldings);
            SwapPlaceholderText(tblSecurityOverview, 1, 2, presentationOverviewData.SecurityCountry);
            SwapPlaceholderText(tblSecurityOverview, 1, 4, presentationOverviewData.Price);
            SwapPlaceholderText(tblSecurityOverview, 2, 2, presentationOverviewData.SecurityIndustry);
            SwapPlaceholderText(tblSecurityOverview, 2, 4, presentationOverviewData.SecurityBMWeight);
            SwapPlaceholderText(tblSecurityOverview, 3, 2, presentationOverviewData.SecurityMarketCapitalization.ToString());
            SwapPlaceholderText(tblSecurityOverview, 3, 4, presentationOverviewData.SecurityActiveWeight);
            SwapPlaceholderText(tblSecurityOverview, 4, 2, presentationOverviewData.Price);
            SwapPlaceholderText(tblSecurityOverview, 4, 4, presentationOverviewData.YTDRet_Absolute);
            SwapPlaceholderText(tblSecurityOverview, 5, 2, presentationOverviewData.FVCalc);
            SwapPlaceholderText(tblSecurityOverview, 5, 4, presentationOverviewData.YTDRet_RELtoLOC);
            SwapPlaceholderText(tblSecurityOverview, 6, 2, presentationOverviewData.SecurityPFVMeasure.ToString() + " " +
                presentationOverviewData.SecurityBuyRange.ToString() + "-" + presentationOverviewData.SecuritySellRange.ToString());
            SwapPlaceholderText(tblSecurityOverview, 6, 4, presentationOverviewData.YTDRet_RELtoEM);
        }

        // Determines whether the shape is a title shape.
        private static bool IsTitleShape(Shape shape)
        {
            var placeholderShape = shape.NonVisualShapeProperties.ApplicationNonVisualDrawingProperties.GetFirstChild<PlaceholderShape>();
            if (placeholderShape != null && placeholderShape.Type != null && placeholderShape.Type.HasValue)
            {
                switch ((PlaceholderValues)placeholderShape.Type)
                {
                    // Any title shape.
                    case PlaceholderValues.Title:

                    // A centered title.
                    case PlaceholderValues.CenteredTitle:
                        return true;

                    default:
                        return false;
                }
            }
            return false;
        }



        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean SetICPPresentationStatus(String userName, Int64 presentationId, String status)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetICPPresentationStatus(userName, presentationId, status).FirstOrDefault();
                return result == 0;                
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        private void SwapPlaceholderText(DocumentFormat.OpenXml.Drawing.Table tbl, int rowNum, int columnNum, string value)
        {
            DocumentFormat.OpenXml.Drawing.TableRow row = tbl.Descendants<DocumentFormat.OpenXml.Drawing.TableRow>().ElementAt(rowNum);
            DocumentFormat.OpenXml.Drawing.TableCell cell = row.Descendants<DocumentFormat.OpenXml.Drawing.TableCell>().ElementAt(columnNum - 1);
            DocumentFormat.OpenXml.Drawing.Text text = cell.Descendants<DocumentFormat.OpenXml.Drawing.Text>().FirstOrDefault();
            text.Text = value;
        }

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public ICPresentationOverviewData RetrieveSecurityDetails(EntitySelectionData entitySelectionData, ICPresentationOverviewData presentationOverviewData, PortfolioSelectionData portfolio)
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

                //Manual inputs
                presentationOverviewData.YTDRet_Absolute = "0.0000%";
                presentationOverviewData.YTDRet_RELtoLOC = "0.0000%";
                presentationOverviewData.YTDRet_RELtoEM = "0.0000%";

                #region GF_SECURITY_BASEVIEW info
                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                            .Where(record => record.TICKER == entitySelectionData.ShortName
                                && record.ISSUE_NAME == entitySelectionData.LongName
                                && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                                && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                            .FirstOrDefault();

                presentationOverviewData.SecurityTicker = securityData.TICKER;
                presentationOverviewData.SecurityName = securityData.ISSUE_NAME;
                presentationOverviewData.SecurityCountry = securityData.ASEC_SEC_COUNTRY_NAME;
                presentationOverviewData.SecurityCountryCode = securityData.ISO_COUNTRY_CODE;
                presentationOverviewData.SecurityIndustry = securityData.GICS_INDUSTRY_NAME;
                presentationOverviewData.Analyst = securityData.ASHMOREEMM_PRIMARY_ANALYST;
                presentationOverviewData.Price = (securityData.CLOSING_PRICE == null ? "" : securityData.CLOSING_PRICE.ToString())
                    + " " + (securityData.TRADING_CURRENCY == null ? "" : securityData.TRADING_CURRENCY.ToString()); 
                #endregion

                #region GF_PORTFOLIO_HOLDINGS info
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> portfolioData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.PORTFOLIO_ID == portfolio.PortfolioId && record.PORTFOLIO_DATE == lastBusinessDate)
                    .ToList();
                decimal? sumDirtyValuePC = portfolioData.Sum(record => record.DIRTY_VALUE_PC);
                decimal? tempNAV;

                List<GF_PORTFOLIO_HOLDINGS> securityInPortfolio = portfolioData
                    .Where(a => a.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID 
                        && a.PORTFOLIO_ID == portfolio.PortfolioId
                        && a.PORTFOLIO_DATE == lastBusinessDate).ToList();
                decimal? sumSecurityDirtyValuePC = securityInPortfolio.Sum(record => record.DIRTY_VALUE_PC);

                if (securityInPortfolio != null && sumSecurityDirtyValuePC > 0)
                {
                    presentationOverviewData.CurrentHoldings = "YES";
                    if (sumDirtyValuePC != 0)
                        presentationOverviewData.PercentEMIF = String.Format("{0:n4}%", ((sumSecurityDirtyValuePC / sumDirtyValuePC) * 100));
                    tempNAV = ((sumSecurityDirtyValuePC / sumDirtyValuePC) * 100);
                }
                else
                {
                    presentationOverviewData.CurrentHoldings = "No";
                    presentationOverviewData.PercentEMIF = "0%";
                    tempNAV = 0;
                } 
                #endregion

                #region GF_BENCHMARK_HOLDINGS Info
                string benchmarkID = portfolioData.Select(a => a.BENCHMARK_ID).FirstOrDefault();
                DimensionEntitiesService.GF_BENCHMARK_HOLDINGS benchmarkData = entity.GF_BENCHMARK_HOLDINGS.Where(
                    record => record.BENCHMARK_ID == benchmarkID
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.PORTFOLIO_DATE == lastBusinessDate)
                    .FirstOrDefault();

                if (benchmarkData != null)
                {
                    presentationOverviewData.SecurityBMWeight = String.Format("{0:n4}%", benchmarkData.BENCHMARK_WEIGHT);
                    tempNAV = (tempNAV - benchmarkData.BENCHMARK_WEIGHT);
                    presentationOverviewData.SecurityActiveWeight = String.Format("{0:n4}%", tempNAV);
                }
                else
                {
                    presentationOverviewData.SecurityBMWeight = "0%";
                    presentationOverviewData.SecurityActiveWeight = String.Format("{0:n4}%", tempNAV);
                } 
                #endregion

                #region FAIR_VALUE Info
                String securityId = securityData.SECURITY_ID == null ? null : securityData.SECURITY_ID.ToString();
                FAIR_VALUE fairValueRecord = externalResearchEntity.FAIR_VALUE.Where(record => record.VALUE_TYPE == "PRIMARY"
                            && record.SECURITY_ID == securityId).FirstOrDefault();

                if (fairValueRecord != null)
                {
                    DATA_MASTER dataMasterRecord = externalResearchEntity.DATA_MASTER
                        .Where(record => record.DATA_ID == fairValueRecord.FV_MEASURE).FirstOrDefault();

                    if (dataMasterRecord != null)
                    {
                        presentationOverviewData.SecurityPFVMeasure = dataMasterRecord.DATA_DESC;
                        presentationOverviewData.SecurityBuyRange = Convert.ToSingle(fairValueRecord.FV_BUY);
                        presentationOverviewData.SecuritySellRange = Convert.ToSingle(fairValueRecord.FV_SELL);

                        presentationOverviewData.CommitteePFVMeasure = dataMasterRecord.DATA_DESC;
                        presentationOverviewData.CommitteeBuyRange = Convert.ToSingle(fairValueRecord.FV_BUY);
                        presentationOverviewData.CommitteeSellRange = Convert.ToSingle(fairValueRecord.FV_SELL);

                        Decimal upperLimit = fairValueRecord.FV_BUY >= fairValueRecord.FV_SELL ? fairValueRecord.FV_BUY : fairValueRecord.FV_SELL;
                        Decimal lowerLimit = fairValueRecord.FV_BUY <= fairValueRecord.FV_SELL ? fairValueRecord.FV_BUY : fairValueRecord.FV_SELL;
                        if (presentationOverviewData.CurrentHoldings == "YES")
                        {
                            presentationOverviewData.SecurityRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= upperLimit
                                ? "Hold" : "Sell";
                            presentationOverviewData.CommitteeRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= upperLimit
                                ? "Hold" : "Sell";
                        }
                        else
                        {
                            presentationOverviewData.SecurityRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= lowerLimit
                                ? "Buy" : "Watch";
                            presentationOverviewData.CommitteeRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= lowerLimit
                                ? "Buy" : "Watch";
                        }


                        if (fairValueRecord.CURRENT_MEASURE_VALUE != 0)
                        {
                            presentationOverviewData.FVCalc = dataMasterRecord.DATA_DESC + " " + securityData.TRADING_CURRENCY + " " +
                                String.Format("{0:n4}",((fairValueRecord.FV_BUY * securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE)) +
                                "- " + securityData.TRADING_CURRENCY + " " +
                                String.Format("{0:n4}",((fairValueRecord.FV_SELL * securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE)); 
                        }

                        String securityID = securityData.SECURITY_ID.ToString();

                        PERIOD_FINANCIALS periodFinancialRecord = externalResearchEntity.PERIOD_FINANCIALS
                            .Where(record => record.SECURITY_ID == securityID
                                && record.DATA_ID == dataMasterRecord.DATA_ID
                                && record.CURRENCY == "USD"
                                && record.PERIOD_TYPE == "C").FirstOrDefault();

                        if (periodFinancialRecord != null)
                        {
                            presentationOverviewData.SecurityMarketCapitalization = Convert.ToSingle(periodFinancialRecord.AMOUNT);
                        }

                    }                    
                } 
                #endregion

                presentationOverviewData.SecurityBuySellvsCrnt = String.Empty; //"$16.50(8*2013PE)-$21.50(10.5*2013PE)";

                

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
                DocumentWorkspaceOperations documentWorkspaceOperations = new DocumentWorkspaceOperations();
                documentWorkspaceOperations.DeleteDocument(fileMasterInfo.Location);

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
                        MeetingDateTime = meetingDateTime.ToUniversalTime(),
                        MeetingClosedDateTime = tempPresentationDeadline.ToUniversalTime(),
                        MeetingVotingClosedDateTime = tempPreMeetingVotingDeadline.ToUniversalTime(),
                        CreatedBy = "System",
                        CreatedOn = DateTime.UtcNow,
                        ModifiedBy = "System",
                        ModifiedOn = DateTime.UtcNow
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






