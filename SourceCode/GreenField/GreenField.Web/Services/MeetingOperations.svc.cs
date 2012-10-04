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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;

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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MembershipUserInfo> GetUsersByNames(List<String> userNames)
        {
            try
            {
                List<MembershipUserInfo> membershipUserInfo = new List<MembershipUserInfo>();

                for (int i = 0; i < userNames.Count; i++)
                {
                    MembershipUser user = Membership.GetUser(userNames[i]);
                    membershipUserInfo.Add(ConvertMembershipUser(user));
                }

                return membershipUserInfo;
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
                    XElement element = new XElement("VotingUser", user.ToLower());
                    xmlDoc.Root.Add(element);
                }

                String[] chiefExecutiveUsers = Roles.GetUsersInRole("IC_CHIEF_EXECUTIVE");
                foreach (String user in chiefExecutiveUsers)
                {
                    XElement element = new XElement("VotingUser", user.ToLower());
                    xmlDoc.Root.Add(element);
                }

                if (!votingUsers.Contains(userName) && !chiefExecutiveUsers.Contains(userName))
                {
                    XElement element = new XElement("VotingUser", userName.ToLower());
                    xmlDoc.Root.Add(element);
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

                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                .Where(record => record.ISSUE_NAME == presentationOverviewData.SecurityName
                    && record.TICKER == presentationOverviewData.SecurityTicker).FirstOrDefault();
                String issuerName = securityRecord == null ? null : securityRecord.ISSUER_NAME;

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
                    IssuerName = issuerName,
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

        private Boolean UploadPresentationFile(String userName, ICPresentationOverviewData presentationOverviewData)
        {
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

            DimensionEntitiesService.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                .Where(record => record.ISSUE_NAME == presentationOverviewData.SecurityName
                    && record.TICKER == presentationOverviewData.SecurityTicker).FirstOrDefault();
            String issuerName = securityRecord == null ? null : securityRecord.ISSUER_NAME;

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
                IssuerName = issuerName,
                SecurityName = presentationOverviewData.SecurityName,
                SecurityTicker = presentationOverviewData.SecurityTicker,
                Type = EnumUtils.ToString(DocumentCategoryType.IC_PRESENTATIONS),
                CreatedBy = userName,
                CreatedOn = DateTime.UtcNow,
                ModifiedBy = userName,
                ModifiedOn = DateTime.UtcNow
            };
            return UpdatePresentationAttachedFileStreamData(userName, presentationOverviewData.PresentationID, fileMaster, false);
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
            SwapPlaceholderText(tblSecurityOverview, 1, 4, presentationOverviewData.PercentEMIF);
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
                presentationOverviewData.PortfolioId = portfolio.PortfolioId;
                presentationOverviewData.YTDRet_Absolute = "0.0000%";
                presentationOverviewData.YTDRet_RELtoLOC = "0.0000%";
                presentationOverviewData.YTDRet_RELtoEM = "0.0000%";

                #region GF_SECURITY_BASEVIEW info
                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                            .Where(record => record.TICKER == entitySelectionData.ShortName
                                && record.ISSUE_NAME == entitySelectionData.LongName)
                            .FirstOrDefault();

                presentationOverviewData.SecurityTicker = securityData.TICKER;
                presentationOverviewData.SecurityName = securityData.ISSUE_NAME;
                presentationOverviewData.SecurityCountry = securityData.ASEC_SEC_COUNTRY_NAME;
                presentationOverviewData.SecurityCountryCode = securityData.ISO_COUNTRY_CODE;
                presentationOverviewData.SecurityIndustry = securityData.GICS_INDUSTRY_NAME;
                presentationOverviewData.Analyst = securityData.ASHMOREEMM_PRIMARY_ANALYST;
                if (securityData.CLOSING_PRICE != null)
                    presentationOverviewData.SecurityLastClosingPrice = Convert.ToSingle(securityData.CLOSING_PRICE);

                presentationOverviewData.Price = (securityData.CLOSING_PRICE == null ? "" : securityData.CLOSING_PRICE.ToString())
                    + " " + (securityData.TRADING_CURRENCY == null ? "" : securityData.TRADING_CURRENCY.ToString());
                #endregion

                #region GF_PORTFOLIO_HOLDINGS info
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> securityHoldingData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.TICKER == entitySelectionData.ShortName && record.PORTFOLIO_DATE == lastBusinessDate
                    && record.DIRTY_VALUE_PC > 0)
                    .ToList();

                decimal? sumSecDirtyValuePC = securityHoldingData.Sum(record => record.DIRTY_VALUE_PC);
                decimal? sumSecBalanceNominal = securityHoldingData.Sum(record => record.BALANCE_NOMINAL);
                if (sumSecDirtyValuePC != null)
                {
                    presentationOverviewData.SecurityCashPosition = Convert.ToSingle(sumSecDirtyValuePC);
                    presentationOverviewData.SecurityPosition = Convert.ToInt64(sumSecBalanceNominal);
                }

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> portfolioData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.PORTFOLIO_ID == portfolio.PortfolioId && record.PORTFOLIO_DATE == lastBusinessDate)
                    .ToList();
                decimal? sumDirtyValuePC = portfolioData.Sum(record => record.DIRTY_VALUE_PC);
                decimal? tempNAV;

                List<GF_PORTFOLIO_HOLDINGS> securityInPortfolio = portfolioData
                    .Where(a => a.TICKER == entitySelectionData.ShortName
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
                        && record.TICKER == entitySelectionData.ShortName
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
                        presentationOverviewData.SecurityPFVMeasureValue = fairValueRecord.CURRENT_MEASURE_VALUE;
                        //presentationOverviewData.CommitteePFVMeasure = dataMasterRecord.DATA_DESC;
                        //presentationOverviewData.CommitteeBuyRange = Convert.ToSingle(fairValueRecord.FV_BUY);
                        //presentationOverviewData.CommitteeSellRange = Convert.ToSingle(fairValueRecord.FV_SELL);                        

                        presentationOverviewData.SecurityBuySellvsCrnt = securityData.TRADING_CURRENCY + " " +
                            String.Format("{0:n4}", fairValueRecord.FV_BUY) +
                            "- " + securityData.TRADING_CURRENCY + " " +
                            String.Format("{0:n4}", fairValueRecord.FV_SELL);

                        Decimal upperLimit = fairValueRecord.FV_BUY >= fairValueRecord.FV_SELL ? fairValueRecord.FV_BUY : fairValueRecord.FV_SELL;
                        Decimal lowerLimit = fairValueRecord.FV_BUY <= fairValueRecord.FV_SELL ? fairValueRecord.FV_BUY : fairValueRecord.FV_SELL;
                        if (presentationOverviewData.CurrentHoldings == "YES")
                        {
                            presentationOverviewData.SecurityRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= upperLimit
                                ? "Hold" : "Sell";
                            //presentationOverviewData.CommitteeRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= upperLimit
                            //    ? "Hold" : "Sell";
                        }
                        else
                        {
                            presentationOverviewData.SecurityRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= lowerLimit
                                ? "Buy" : "Watch";
                            //presentationOverviewData.CommitteeRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= lowerLimit
                            //    ? "Buy" : "Watch";
                        }


                        if (fairValueRecord.CURRENT_MEASURE_VALUE != 0)
                        {
                            presentationOverviewData.FVCalc = dataMasterRecord.DATA_DESC + " " + securityData.TRADING_CURRENCY + " " +
                                String.Format("{0:n4}", ((fairValueRecord.FV_BUY * securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE)) +
                                "- " + securityData.TRADING_CURRENCY + " " +
                                String.Format("{0:n4}", ((fairValueRecord.FV_SELL * securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE));
                        }

                        String securityID = securityData.SECURITY_ID.ToString();

                        PERIOD_FINANCIALS periodFinancialRecord = externalResearchEntity.PERIOD_FINANCIALS
                            .Where(record => record.SECURITY_ID == securityID
                                && record.DATA_ID == 185
                                && record.CURRENCY == "USD"
                                && record.PERIOD_TYPE == "C").FirstOrDefault();

                        if (periodFinancialRecord != null)
                        {
                            presentationOverviewData.SecurityMarketCapitalization = Convert.ToSingle(periodFinancialRecord.AMOUNT);
                        }

                    }
                }
                #endregion



                return presentationOverviewData;
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
        public Dictionary<String, Decimal?> RetrieveCurrentPFVMeasures(List<String> PFVTypeInfo, String securityTicker)
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();
                Dictionary<String, Decimal?> result = new Dictionary<string, decimal?>();

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == securityTicker).FirstOrDefault();

                foreach (String pfvType in PFVTypeInfo)
                {
                    Decimal? value = null;
                    DATA_MASTER dataMasterRecord = externalResearchEntity.DATA_MASTER
                        .Where(record => record.DATA_DESC == pfvType).FirstOrDefault();
                    if (dataMasterRecord != null)
                    {
                        if (securityData != null)
                        {
                            String securityID = securityData.SECURITY_ID.ToString();
                            PERIOD_FINANCIALS periodFinancialRecord = externalResearchEntity.PERIOD_FINANCIALS
                                .Where(record => record.SECURITY_ID == securityID
                                    && record.DATA_ID == dataMasterRecord.DATA_ID
                                    && record.CURRENCY == "USD"
                                    && record.PERIOD_TYPE == "C").FirstOrDefault();
                            if (periodFinancialRecord != null)
                            {
                                value = periodFinancialRecord.AMOUNT;
                            }
                        }
                    }

                    result.Add(pfvType, value);
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
        public Boolean ReSubmitPresentation(String userName, ICPresentationOverviewData presentationOverviewData, Boolean sendAlert)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
                //Recalculate presentation overview data
                presentationOverviewData = RetrieveUpdatedSecurityDetails(presentationOverviewData);
                //Update presentation info object
                XDocument xmlDoc = GetEntityXml<ICPresentationOverviewData>(new List<ICPresentationOverviewData> { presentationOverviewData });
                string xmlScript = xmlDoc.ToString();
                Int64? result = entity.UpdatePresentationInfo(userName, xmlScript).FirstOrDefault();
                if (result == null)
                    throw new Exception("Unable to update presentation info object");

                #region Update power point presentation file
                #region Retrieve presentation file or create new one if not exists
                String copiedFilePath = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_ICPresentation.pptx";
                List<FileMaster> presentationAttachedFiles = RetrievePresentationAttachedFileDetails(presentationOverviewData.PresentationID);
                FileMaster presentationPowerPointAttachedFile = null;
                if (presentationAttachedFiles != null)
                {
                    presentationPowerPointAttachedFile = presentationAttachedFiles.Where(record => record.Category == "Power Point Presentation").FirstOrDefault();
                    if (presentationPowerPointAttachedFile != null)
                    {
                        Byte[] powerPointFileStream = documentOperations.RetrieveDocument(presentationPowerPointAttachedFile.Location
                            .Substring(presentationPowerPointAttachedFile.Location.LastIndexOf(@"/") + 1));

                        if (powerPointFileStream == null)
                            throw new Exception("Unable to download power point file from repository");

                        File.WriteAllBytes(copiedFilePath, powerPointFileStream);
                    }
                    else
                    {
                        String presentationFile = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\Templates\ICPresentationTemplate.pptx";
                        File.Copy(presentationFile, copiedFilePath, true);
                    }
                }
                else
                {
                    String presentationFile = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\Templates\ICPresentationTemplate.pptx";
                    File.Copy(presentationFile, copiedFilePath, true);
                }
                #endregion

                #region Edit presentation file
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
                #endregion

                #region Upload power point to share point and modify database
                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                .Where(record => record.ISSUE_NAME == presentationOverviewData.SecurityName
                    && record.TICKER == presentationOverviewData.SecurityTicker).FirstOrDefault();
                String issuerName = securityRecord == null ? null : securityRecord.ISSUER_NAME;

                Byte[] fileStream = File.ReadAllBytes(copiedFilePath);
                String fileName = presentationOverviewData.SecurityName + "_" + (presentationOverviewData.MeetingDateTime.HasValue
                    ? Convert.ToDateTime(presentationOverviewData.MeetingDateTime).ToString("ddMMyyyy") : String.Empty) + ".pptx";

                String url = documentOperations.UploadDocument(fileName, File.ReadAllBytes(copiedFilePath), String.Empty);

                if (url == String.Empty)
                    throw new Exception("Exception occurred while uploading template powerpoint presentation!!!");

                File.Delete(copiedFilePath);

                FileMaster fileMaster = new FileMaster()
                {
                    Category = "Power Point Presentation",
                    Location = url,
                    Name = presentationOverviewData.SecurityName + "_" + Convert.ToDateTime(presentationOverviewData.MeetingDateTime).ToString("ddMMyyyy") + ".pptx",
                    IssuerName = issuerName,
                    SecurityName = presentationOverviewData.SecurityName,
                    SecurityTicker = presentationOverviewData.SecurityTicker,
                    Type = EnumUtils.ToString(DocumentCategoryType.IC_PRESENTATIONS),
                    CreatedBy = userName,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = userName,
                    ModifiedOn = DateTime.UtcNow
                };

                Boolean insertedFileMasterRecord = UpdatePresentationAttachedFileStreamData(userName, presentationOverviewData.PresentationID, fileMaster, false);
                if (presentationPowerPointAttachedFile != null && insertedFileMasterRecord)
                {
                    documentOperations.DeleteDocument(presentationPowerPointAttachedFile.Location);
                    entity.DeleteFileMaster(presentationPowerPointAttachedFile.FileID);
                }
                #endregion
                #endregion

                #region IC Packet
                if (presentationAttachedFiles != null)
                {
                    List<FileMaster> icPacketFiles = presentationAttachedFiles.Where(record => record.Category == "Investment Committee Packet").ToList();
                    foreach (FileMaster record in icPacketFiles)
                    {
                        documentOperations.DeleteDocument(record.Location);
                    }
                }

                Byte[] generatedICPacketStream = GenerateICPacketReport(presentationOverviewData.PresentationID);
                String emailAttachments = null;
                String uploadFileName = String.Format("{0}_{1}_ICPacket.pdf"
                            , Convert.ToDateTime(presentationOverviewData.MeetingDateTime).ToString("MMddyyyy")
                            , presentationOverviewData.SecurityTicker);
                if (generatedICPacketStream != null)
                {
                    String uploadFileLocation = documentOperations.UploadDocument(uploadFileName, generatedICPacketStream, String.Empty);

                    FileMaster fileMaster_ICPacket = new FileMaster()
                    {
                        Category = "Investment Committee Packet",
                        Location = uploadFileLocation,
                        Name = uploadFileName,
                        IssuerName = issuerName,
                        SecurityName = presentationOverviewData.SecurityName,
                        SecurityTicker = presentationOverviewData.SecurityTicker,
                        Type = EnumUtils.ToString(DocumentCategoryType.IC_PRESENTATIONS),
                        CreatedBy = userName,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedBy = userName,
                        ModifiedOn = DateTime.UtcNow
                    };

                    UpdatePresentationAttachedFileStreamData(userName, presentationOverviewData.PresentationID, fileMaster_ICPacket, false);

                    if (!String.IsNullOrEmpty(uploadFileLocation))
                    {
                        emailAttachments += uploadFileLocation;
                    }
                }
                #endregion

                #region Alert
                if (sendAlert)
                {
                    List<VoterInfo> voterInfo = RetrievePresentationVoterData(presentationOverviewData.PresentationID, true).ToList();
                    List<String> userNames = voterInfo.Where(record => record.PostMeetingFlag == false).Select(record => record.Name).ToList();
                    List<MembershipUserInfo> users = GetUsersByNames(userNames);

                    String emailTo = String.Join("|", users.Select(record => record.Email).ToArray());

                    String messageSubject = "Investment Committee Presentation Edit Notification – " +
                        Convert.ToDateTime(presentationOverviewData.MeetingClosedDateTime).ToString("MMMM dd, yyyy");
                    String messageBody = "The attached presentation scheduled for the Investment Committee Meeting dated "
                        + Convert.ToDateTime(presentationOverviewData.MeetingClosedDateTime).ToString("MMMM dd, yyyy")
                        + " UTC has been edited since its original submission.  Voting members, please enter AIMS to modify your comments and pre-meeting votes, if necessary.";
                    SetMessageInfo(emailTo, null, messageSubject, messageBody, emailAttachments, userName);
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        private ICPresentationOverviewData RetrieveUpdatedSecurityDetails(ICPresentationOverviewData presentationOverviewData)
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                #region GF_SECURITY_BASEVIEW info
                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                            .Where(record => record.TICKER == presentationOverviewData.SecurityTicker 
                                && record.ISSUE_NAME == presentationOverviewData.SecurityName)
                            .FirstOrDefault();

                presentationOverviewData.SecurityCountry = securityData.ASEC_SEC_COUNTRY_NAME;
                presentationOverviewData.SecurityCountryCode = securityData.ISO_COUNTRY_CODE;
                presentationOverviewData.SecurityIndustry = securityData.GICS_INDUSTRY_NAME;
                presentationOverviewData.Analyst = securityData.ASHMOREEMM_PRIMARY_ANALYST;
                if (securityData.CLOSING_PRICE != null)
                    presentationOverviewData.SecurityLastClosingPrice = Convert.ToSingle(securityData.CLOSING_PRICE);

                presentationOverviewData.Price = (securityData.CLOSING_PRICE == null ? "" : securityData.CLOSING_PRICE.ToString())
                    + " " + (securityData.TRADING_CURRENCY == null ? "" : securityData.TRADING_CURRENCY.ToString());
                #endregion

                #region GF_PORTFOLIO_HOLDINGS info
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> securityHoldingData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.TICKER == presentationOverviewData.SecurityTicker && record.PORTFOLIO_DATE == lastBusinessDate
                    && record.DIRTY_VALUE_PC > 0)
                    .ToList();

                decimal? sumSecDirtyValuePC = securityHoldingData.Sum(record => record.DIRTY_VALUE_PC);
                decimal? sumSecBalanceNominal = securityHoldingData.Sum(record => record.BALANCE_NOMINAL);
                if (sumSecDirtyValuePC != null)
                {
                    presentationOverviewData.SecurityCashPosition = Convert.ToSingle(sumSecDirtyValuePC);
                    presentationOverviewData.SecurityPosition = Convert.ToInt64(sumSecBalanceNominal);
                }

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> portfolioData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.PORTFOLIO_ID == presentationOverviewData.PortfolioId && record.PORTFOLIO_DATE == lastBusinessDate)
                    .ToList();
                decimal? sumDirtyValuePC = portfolioData.Sum(record => record.DIRTY_VALUE_PC);
                decimal? tempNAV;

                List<GF_PORTFOLIO_HOLDINGS> securityInPortfolio = portfolioData
                    .Where(a => a.TICKER == presentationOverviewData.SecurityTicker
                        && a.PORTFOLIO_ID == presentationOverviewData.PortfolioId
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
                        && record.TICKER == presentationOverviewData.SecurityTicker
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
                        presentationOverviewData.SecurityPFVMeasureValue = fairValueRecord.CURRENT_MEASURE_VALUE;
                        presentationOverviewData.SecurityBuySellvsCrnt = securityData.TRADING_CURRENCY + " " +
                            String.Format("{0:n4}", fairValueRecord.FV_BUY) +
                            "- " + securityData.TRADING_CURRENCY + " " +
                            String.Format("{0:n4}", fairValueRecord.FV_SELL);

                        Decimal upperLimit = fairValueRecord.FV_BUY >= fairValueRecord.FV_SELL ? fairValueRecord.FV_BUY : fairValueRecord.FV_SELL;
                        Decimal lowerLimit = fairValueRecord.FV_BUY <= fairValueRecord.FV_SELL ? fairValueRecord.FV_BUY : fairValueRecord.FV_SELL;
                        if (presentationOverviewData.CurrentHoldings == "YES")
                        {
                            presentationOverviewData.SecurityRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= upperLimit
                                ? "Hold" : "Sell";
                        }
                        else
                        {
                            presentationOverviewData.SecurityRecommendation = fairValueRecord.CURRENT_MEASURE_VALUE <= lowerLimit
                                ? "Buy" : "Watch";
                        }

                        if (fairValueRecord.CURRENT_MEASURE_VALUE != 0)
                        {
                            presentationOverviewData.FVCalc = dataMasterRecord.DATA_DESC + " " + securityData.TRADING_CURRENCY + " " +
                                String.Format("{0:n4}", ((fairValueRecord.FV_BUY * securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE)) +
                                "- " + securityData.TRADING_CURRENCY + " " +
                                String.Format("{0:n4}", ((fairValueRecord.FV_SELL * securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE));
                        }

                        String securityID = securityData.SECURITY_ID.ToString();

                        PERIOD_FINANCIALS periodFinancialRecord = externalResearchEntity.PERIOD_FINANCIALS
                            .Where(record => record.SECURITY_ID == securityID
                                && record.DATA_ID == 185
                                && record.CURRENCY == "USD"
                                && record.PERIOD_TYPE == "C").FirstOrDefault();

                        if (periodFinancialRecord != null)
                        {
                            presentationOverviewData.SecurityMarketCapitalization = Convert.ToSingle(periodFinancialRecord.AMOUNT);
                        }

                    }
                }
                #endregion



                return presentationOverviewData;
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
                if (deletionFlag)
                {
                    documentWorkspaceOperations.DeleteDocument(fileMasterInfo.Location);
                }

                entity.SetICPresentationAttachedFileInfo(userName, presentationId, fileMasterInfo.Name, fileMasterInfo.IssuerName
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Byte[] GenerateICPacketReport(Int64 presentationId)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                List<FileMaster> presentationAttachedFileData = RetrievePresentationAttachedFileDetails(presentationId);
                presentationAttachedFileData = presentationAttachedFileData
                    .Where(record => record.Type == "IC Presentations"
                        && (record.Category == "Power Point Presentation"
                            || record.Category == "FinStat Report"
                            || record.Category == "Investment Context Report"
                            || record.Category == "DCF Model"
                            || record.Category == "Additional Attachment")).ToList();

                PresentationInfo presentationInfo = entity.PresentationInfoes.Where(record => record.PresentationID == presentationId).FirstOrDefault();

                List<String> downloadedDocumentLocations = GetICPacketSegmentFiles(presentationAttachedFileData, presentationInfo);
                Byte[] result = MergePDFFiles(downloadedDocumentLocations);

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        private List<String> GetICPacketSegmentFiles(List<FileMaster> fileMasterInfo, PresentationInfo presentationInfo)
        {
            List<String> result = new List<String>();

            foreach (FileMaster file in fileMasterInfo)
            {
                String convertedPdf = null;
                if (file.Category == "Power Point Presentation")
                {
                    convertedPdf = ConvertPowerpointPresentationTpPdf(file, presentationInfo);
                }
                else
                {
                    convertedPdf = ConvertImagePdfFileToLocalPdf(file);
                }

                if (convertedPdf != null)
                    result.Add(convertedPdf);
            }

            return result;
        }

        private String ConvertImagePdfFileToLocalPdf(FileMaster file)
        {
            String result = null;
            try
            {
                DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
                Byte[] fileData = documentOperations.RetrieveDocument(file.Location.Substring(file.Location.LastIndexOf(@"/") + 1));
                if (fileData == null)
                    return result;

                if (file.Location.Contains(".pdf"))
                {
                    result = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_temp.pdf";
                    File.WriteAllBytes(result, fileData);
                }

                else if (file.Location.Contains(".jpeg") || file.Location.Contains(".jpg"))
                {
                    String localFile = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_temp" +
                        (file.Location.Contains(".jpeg") ? ".jpeg" : ".jpg");
                    result = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_temp.pdf";
                    File.WriteAllBytes(localFile, fileData);
                    Document doc = new Document(PageSize.A4, 10F, 10F, 10F, 10F);
                    PdfWriter.GetInstance(doc, new FileStream(result, FileMode.Create));
                    doc.Open();
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(localFile);
                    //image.SetAbsolutePosition(10F, 10F); 
                    image.ScaleToFit(doc.PageSize.Width - (10F + 10F), doc.PageSize.Height - (10F + 10F));
                    doc.Add(image);
                    doc.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private String ConvertPowerpointPresentationTpPdf(FileMaster powerpointStreamedData, PresentationInfo presentationInfo)
        {
            String result = null;

            try
            {
                if (presentationInfo == null || powerpointStreamedData == null)
                    return result;

                DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
                Byte[] fileData = documentOperations.RetrieveDocument(powerpointStreamedData.Location.Substring(powerpointStreamedData.Location.LastIndexOf(@"/") + 1));
                if (fileData == null)
                    return result;

                String localFile = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_temp.pptx";
                File.WriteAllBytes(localFile, fileData);

                Web.Helpers.SecurityInformation securityInformation = new Web.Helpers.SecurityInformation()
                {
                    ActiveWeight = presentationInfo.SecurityActiveWeight,
                    Analyst = presentationInfo.Analyst,
                    BenchmarkWeight = presentationInfo.SecurityBMWeight,
                    BSR = presentationInfo.SecurityBuySellvsCrnt,
                    Country = presentationInfo.SecurityCountry,
                    CurrentHoldings = presentationInfo.CurrentHoldings,
                    FVCalc = presentationInfo.FVCalc,
                    Industry = presentationInfo.SecurityIndustry,
                    MktCap = presentationInfo.SecurityMarketCapitalization.ToString(),
                    NAV = presentationInfo.PercentEMIF,
                    Price = presentationInfo.Price,
                    Recommendation = presentationInfo.SecurityRecommendation,
                    RetAbsolute = presentationInfo.YTDRet_Absolute,
                    RetEMV = presentationInfo.YTDRet_RELtoEM,
                    RetLoc = presentationInfo.YTDRet_RELtoLOC,
                    SecurityName = presentationInfo.SecurityName
                };

                ICPresentation presentationData = PptRead.Fetch(localFile, securityInformation);
                result = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_temp.pdf";
                PptRead.GeneratePresentationPdf(result, presentationData);
            }
            catch (Exception)
            {                
                throw;
            }

            return result;

        }

        private Byte[] MergePDFFiles(List<String> pdfFileNames)
        {
            Byte[] result = new byte[] { };
            try
            {
                String outFile = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_ICPacket.pdf";

                int pageOffset = 0;
                ArrayList master = new ArrayList();
                int f = 0;
                Document document = null;
                PdfCopy writer = null;

                while (f < pdfFileNames.Count())
                {
                    if (pdfFileNames[f].Contains(".pdf"))
                    {
                        PdfReader reader = new PdfReader(pdfFileNames[f]);
                        reader.ConsolidateNamedDestinations();

                        int n = reader.NumberOfPages;
                        pageOffset += n;
                        if (f == 0)
                        {
                            document = new Document(reader.GetPageSizeWithRotation(1));
                            writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));
                            document.Open();
                        }

                        for (int i = 0; i < n; )
                        {
                            ++i;
                            if (writer != null)
                            {
                                PdfImportedPage page = writer.GetImportedPage(reader, i);
                                writer.AddPage(page);
                            }
                        }

                        PRAcroForm form = reader.AcroForm;
                        if (form != null && writer != null)
                        {
                            writer.CopyAcroForm(reader);
                        }
                        f++;
                    }
                }

                if (document != null)
                {
                    document.Close();
                }

                result = File.ReadAllBytes(outFile);
            }
            catch (Exception)
            {
                throw;
            }

            return result;
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

                String presentationDay = meetingConfigurationSchedule.PresentationDay;
                DateTime presentationTime = new DateTime(meetingConfigurationSchedule.PresentationTime.Ticks, DateTimeKind.Utc);
                String preMeetingVotingDeadlineDay = meetingConfigurationSchedule.PreMeetingVotingDeadlineDay;
                DateTime preMeetingVotingDeadlineTime = new DateTime(meetingConfigurationSchedule.PreMeetingVotingDeadlineTime.Ticks, DateTimeKind.Utc);
                String presentationDeadlineDay = meetingConfigurationSchedule.PresentationDeadlineDay;
                DateTime presentationDeadlineTime = new DateTime(meetingConfigurationSchedule.PresentationDeadlineTime.Ticks, DateTimeKind.Utc);

                List<MeetingInfo> result = new List<MeetingInfo>();

                for (DateTime id = DateTime.UtcNow; id < DateTime.Now.AddMonths(months); id = id.AddDays(7))
                {

                    DateTime tempPresentationDeadline = id.Date.Add(presentationDeadlineTime.TimeOfDay);

                    if (tempPresentationDeadline < DateTime.UtcNow)
                        continue;

                    while (tempPresentationDeadline.DayOfWeek.ToString() != presentationDeadlineDay)
                        tempPresentationDeadline = tempPresentationDeadline.AddDays(1);

                    DateTime tempPreMeetingVotingDeadline = tempPresentationDeadline.Date.Add(preMeetingVotingDeadlineTime.TimeOfDay);
                    while (tempPreMeetingVotingDeadline.DayOfWeek.ToString() != preMeetingVotingDeadlineDay)
                        tempPreMeetingVotingDeadline = tempPreMeetingVotingDeadline.AddDays(1);

                    DateTime meetingDateTime = tempPreMeetingVotingDeadline.Date.Add(presentationTime.TimeOfDay);
                    while (meetingDateTime.DayOfWeek.ToString() != presentationDay)
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
                Int32? result = entity.SetMeetingConfigSchedule(userName, meetingConfigurationSchedule.PresentationDay, meetingConfigurationSchedule.PresentationTime
                    , meetingConfigurationSchedule.PresentationTimeZone, meetingConfigurationSchedule.PresentationDeadlineDay
                    , meetingConfigurationSchedule.PresentationDeadlineTime, meetingConfigurationSchedule.PreMeetingVotingDeadlineDay
                    , meetingConfigurationSchedule.PreMeetingVotingDeadlineTime).FirstOrDefault();

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
        public List<VoterInfo> RetrievePresentationVoterData(Int64 presentationId, Boolean includeICAdminInfo)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                List<VoterInfo> result = entity.VoterInfoes.Where(record => record.PresentationID == presentationId).ToList();
                if (includeICAdminInfo)
                {
                    String[] users = Roles.GetUsersInRole("IC_ADMIN");
                    foreach (String user in users)
                    {
                        result.Add(new VoterInfo()
                        {
                            Name = user,
                            PostMeetingFlag = false
                        });

                        result.Add(new VoterInfo()
                        {
                            Name = user,
                            PostMeetingFlag = true
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
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                XDocument xmlDoc = GetEntityXml<ICPresentationOverviewData>(new List<ICPresentationOverviewData> { presentationOverViewData }
                    , strictlyInclusiveProperties: new List<string> { "PresentationID", "AdminNotes", "CommitteePFVMeasure", "CommitteeBuyRange",
                    "CommitteeSellRange", "CommitteeRecommendation", "CommitteePFVMeasureValue" });
                xmlDoc = GetEntityXml<VoterInfo>(parameters: voterInfo, xmlDoc: xmlDoc, strictlyInclusiveProperties: new List<string> { "VoterID",
                    "VoterPFVMeasure", "VoterBuyRange", "VoterSellRange", "VoteType" });
                String xmlScript = xmlDoc.ToString();
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetICPresentationDecisionEntryDetails(userName, xmlScript).FirstOrDefault();

                DimensionEntitiesService.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ISSUE_NAME == presentationOverViewData.SecurityName &&
                        record.TICKER == presentationOverViewData.SecurityTicker).FirstOrDefault();

                ExternalResearchEntities externalEntity = new ExternalResearchEntities();

                if(securityRecord != null && securityRecord.SECURITY_ID != null)
                {                    
                    result = externalEntity.SetFairValue(securityRecord.SECURITY_ID.ToString(), presentationOverViewData.CommitteePFVMeasure, presentationOverViewData.CommitteePFVMeasureValue
                        , Convert.ToDecimal(presentationOverViewData.CommitteeBuyRange), Convert.ToDecimal(presentationOverViewData.CommitteeSellRange)).FirstOrDefault();
                }

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
        public Boolean UpdateMeetingAttachedFileStreamData(String userName, Int64 meetingId, FileMaster fileMasterInfo, Boolean deletionFlag)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                DocumentWorkspaceOperations documentWorkspaceOperations = new DocumentWorkspaceOperations();
                if (deletionFlag)
                {
                    documentWorkspaceOperations.DeleteDocument(fileMasterInfo.Location);
                }


                if (fileMasterInfo.IssuerName == null)
                {
                    DimensionEntitiesService.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                        .Where(record => record.ISSUE_NAME == fileMasterInfo.SecurityName
                            && record.TICKER == fileMasterInfo.SecurityTicker).FirstOrDefault();
                    fileMasterInfo.IssuerName = securityRecord == null ? null : securityRecord.ISSUER_NAME;
                }

                entity.SetICPMeetingAttachedFileInfo(userName, meetingId, fileMasterInfo.Name, fileMasterInfo.IssuerName 
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MembershipUserInfo> GetAllUsers()
        {
            try
            {
                MembershipUserCollection membershipUserCollection = new MembershipUserCollection();
                List<MembershipUserInfo> membershipUserInfo = new List<MembershipUserInfo>();

                membershipUserCollection = Membership.GetAllUsers();
                foreach (MembershipUser user in membershipUserCollection)
                    membershipUserInfo.Add(ConvertMembershipUser(user));
                return membershipUserInfo;
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
        public Byte[] GenerateMeetingMinutesReport(Int64 meetingId)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                List<MeetingMinutesReportData> metaData = entity.GetMeetingMinutesReportDetails(meetingId)
                    .Where(record => record.Presenter.ToLower() != record.Name.ToLower()).ToList();
                
                return GeneratePostMeetingReport(metaData);
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
        public Byte[] GeneratePreMeetingVotingReport(Int64 presentationId)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();

                PresentationInfo presentationInfo = entity.PresentationInfoes.Where(record => record.PresentationID == presentationId).FirstOrDefault();
                if (presentationInfo == null)
                    return null;

                String securityName = presentationInfo.SecurityName;
                String securityTicker = presentationInfo.SecurityTicker;

                GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ISSUE_NAME == securityName && record.TICKER == securityTicker).FirstOrDefault();

                String securityDescription = String.Empty;
                if (securityRecord != null)
                {
                    securityDescription = securityRecord.ASHEMM_ONE_LINER_DESCRIPTION;
                }

                List<PresentationVotingDeadlineDetails> records = entity.GetPreMeetingVotingReportDetails(presentationId).ToList();
                List<PresentationVotingDeadlineDetails> metaData = new List<PresentationVotingDeadlineDetails>();

                if (records != null && records.Count > 0)
                {
                    Int64? defaultFileId = records.First().FileID;
                    metaData = records.Where(record => record.FileID == defaultFileId && record.Presenter.ToLower() != record.Name.ToLower()).ToList();
                }


                return GeneratePreMeetingReport(metaData, securityDescription);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Convert System.Web.Security.MembershipUser to GreenField.Web.DataContracts.MembershipUserInfo
        /// </summary>
        /// <param name="user">MembershipUser</param>
        /// <returns>MembershipUserInfo</returns>
        private MembershipUserInfo ConvertMembershipUser(MembershipUser user)
        {
            if (user == null)
                return null;

            return new MembershipUserInfo
            {
                UserName = user.UserName,
                Email = user.Email,
                IsApproved = user.IsApproved,
                IsLockedOut = user.IsLockedOut,
                IsOnline = user.IsOnline,
                Comment = user.Comment,
                CreateDate = user.CreationDate,
                LastActivityDate = user.LastActivityDate,
                LastLockoutDate = user.LastLockoutDate,
                LastLogInDate = user.LastLoginDate,
                ProviderUserKey = user.ProviderUserKey.ToString(),
                ProviderName = user.ProviderName,
                PasswordQuestion = user.PasswordQuestion,
                LastPasswordChangedDate = user.LastPasswordChangedDate
            };
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

        private static Byte[] GeneratePostMeetingReport(List<MeetingMinutesReportData> presentationFinalizeInfo)
        {
            if (presentationFinalizeInfo == null)
                return null;

            MeetingMinutesReportData presentationFinalizeDetails = presentationFinalizeInfo.FirstOrDefault();
            if (presentationFinalizeDetails == null)
                return null;

            Document doc = new Document(PageSize.A4, 10F, 10F, 10F, 10F);

            DateTime meetingDate = Convert.ToDateTime(presentationFinalizeDetails.MeetingDateTime);
            String reportOutputFile = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_ICPMeetingMinutes.pdf";

            PdfWriter.GetInstance(doc, new FileStream(reportOutputFile, FileMode.Create));
            doc.Open();
            Rectangle page = doc.PageSize;

            PdfPTable topHeaderTableContent = new PdfPTable(1);
            topHeaderTableContent.WidthPercentage = 100;

            PdfPCell topHeaderCell = new PdfPCell(new Phrase("INVESTMENT COMMITTEE - MEETING MINUTES", PDFFontStyle.STYLE_0));
            AddTextCell(topHeaderTableContent, topHeaderCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);


            String meetingDayOfTheWeek = meetingDate.DayOfWeek.ToString().ToUpper();
            PdfPCell topSubHeaderCell = new PdfPCell(new Phrase(meetingDayOfTheWeek + " " + meetingDate.ToString("MMMM dd, yyyy"), PDFFontStyle.STYLE_4));
            topSubHeaderCell.PaddingTop = 5;
            AddTextCell(topHeaderTableContent, topSubHeaderCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.NONE);


            String attendees = String.Join(", ", presentationFinalizeInfo.Where(record => record.AttendanceType == "Attended")
                .Select(record => record.Name).ToList().Distinct().ToArray());
            String videoconference = String.Join(", ", presentationFinalizeInfo.Where(record => record.AttendanceType == "Video Conference")
                .Select(record => record.Name).ToList().Distinct().ToArray());
            String teleconference = String.Join(", ", presentationFinalizeInfo.Where(record => record.AttendanceType == "Tele Conference")
                .Select(record => record.Name).ToList().Distinct().ToArray());

            Paragraph attendeePara = new Paragraph();
            attendeePara.Add(new Phrase("Attendees: ", PDFFontStyle.STYLE_2));
            attendeePara.Add(new Phrase(attendees, PDFFontStyle.STYLE_3));
            PdfPCell attendeesCell = new PdfPCell(attendeePara);
            attendeesCell.PaddingTop = 20;
            AddTextCell(topHeaderTableContent, attendeesCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            Paragraph videoConferencePara = new Paragraph();
            videoConferencePara.Add(new Phrase("Videoconference: ", PDFFontStyle.STYLE_2)); ;
            videoConferencePara.Add(new Phrase(videoconference, PDFFontStyle.STYLE_3));
            PdfPCell videoConferenceCell = new PdfPCell(videoConferencePara);
            videoConferenceCell.PaddingTop = 5;
            AddTextCell(topHeaderTableContent, videoConferenceCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            Paragraph teleConferencePara = new Paragraph();
            teleConferencePara.Add(new Phrase("Teleconference: ", PDFFontStyle.STYLE_2)); ;
            teleConferencePara.Add(new Phrase(teleconference, PDFFontStyle.STYLE_3));
            PdfPCell teleConferenceCell = new PdfPCell(teleConferencePara);
            teleConferenceCell.PaddingTop = 5;
            AddTextCell(topHeaderTableContent, teleConferenceCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            PdfPCell discussionStatementCell = new PdfPCell(new Phrase("The following items were discussed at the IC meeting at " + meetingDate.ToString("h:mm tt") + " UTC:"
                , PDFFontStyle.STYLE_3));
            discussionStatementCell.PaddingTop = 20;
            AddTextCell(topHeaderTableContent, discussionStatementCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.NONE);

            doc.Add(topHeaderTableContent);

            Boolean industryReportPresent = presentationFinalizeInfo.Where(record => record.Category == "Industry Report").Count() > 0;

            ZapfDingbatsList discussionList = new ZapfDingbatsList(118, 10);
            discussionList.IndentationLeft = 2;
            discussionList.Symbol.Font.Size = 7F;
            discussionList.Add(new ListItem(new Phrase("Industry reports: " + (industryReportPresent ? "" : "No ") + "Industry reports were presented", PDFFontStyle.STYLE_3)));
            discussionList.Add(new ListItem(new Phrase(presentationFinalizeDetails.MeetingDescription, PDFFontStyle.STYLE_3)));
            doc.Add(discussionList);

            Paragraph securityDescriptor = new Paragraph(new Phrase("The following company reports were discussed during this week’s investment committee meeting:", PDFFontStyle.STYLE_3));
            securityDescriptor.SpacingBefore = 5;
            doc.Add(securityDescriptor);

            List<Int64?> distinctMeetingPresentationIds = presentationFinalizeInfo.Select(record => record.PresentationID).ToList().Distinct().ToList();
            Int32 securityCounter = 0;

            foreach (Int64 presentationId in distinctMeetingPresentationIds)
            {
                List<MeetingMinutesReportData> distinctPresentationFinalizeDetails = presentationFinalizeInfo
                    .Where(record => record.PresentationID == presentationId).ToList();

                if (distinctPresentationFinalizeDetails.Count == 0)
                    continue;

                Paragraph securityName = new Paragraph(new Phrase((++securityCounter).ToString() + ". "
                    + distinctPresentationFinalizeDetails.First().SecurityName, PDFFontStyle.STYLE_4));
                securityName.SpacingBefore = 10;
                doc.Add(securityName);

                ZapfDingbatsList securityDescriptionList = new ZapfDingbatsList(108, 10);
                securityDescriptionList.IndentationLeft = 20;
                securityDescriptionList.Symbol.Font.Size = 5F;
                securityDescriptionList.Lettered = true;

                Paragraph countryPara = new Paragraph();
                countryPara.SpacingBefore = -5;
                countryPara.Add(new Phrase("Country: ", PDFFontStyle.STYLE_2));
                countryPara.Add(new Phrase(distinctPresentationFinalizeDetails.First().SecurityCountry, PDFFontStyle.STYLE_3));
                securityDescriptionList.Add(new ListItem(countryPara));

                Paragraph industryPara = new Paragraph();
                industryPara.SpacingBefore = -5;
                industryPara.Add(new Phrase("Industry: ", PDFFontStyle.STYLE_2));
                industryPara.Add(new Phrase(distinctPresentationFinalizeDetails.First().SecurityIndustry, PDFFontStyle.STYLE_3));
                securityDescriptionList.Add(new ListItem(industryPara));

                Paragraph presenterPara = new Paragraph();
                presenterPara.SpacingBefore = -5;
                presenterPara.Add(new Phrase("Presented By: ", PDFFontStyle.STYLE_2));
                presenterPara.Add(new Phrase(distinctPresentationFinalizeDetails.First().Presenter, PDFFontStyle.STYLE_3));
                securityDescriptionList.Add(new ListItem(presenterPara));

                doc.Add(securityDescriptionList);

                PdfPTable icdecisionTable = new PdfPTable(4);
                icdecisionTable.WidthPercentage = 100;
                icdecisionTable.SpacingBefore = 10;
                icdecisionTable.SetWidths(new float[] { 1, 1, 1, 1 });

                PdfPCell recommendationCell = new PdfPCell(new Phrase("RECOMMENDATION", PDFFontStyle.STYLE_2));
                recommendationCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(icdecisionTable, recommendationCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP);

                PdfPCell presentedBuySellRangeCell = new PdfPCell(new Phrase("BUY/SELL (PRESENTED)", PDFFontStyle.STYLE_2));
                presentedBuySellRangeCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(icdecisionTable, presentedBuySellRangeCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP);

                PdfPCell decisionCell = new PdfPCell(new Phrase("DECISION", PDFFontStyle.STYLE_2));
                decisionCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(icdecisionTable, decisionCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP);

                PdfPCell committeeBuySellRangeCell = new PdfPCell(new Phrase("BUY/SELL (COMMITTEE)", PDFFontStyle.STYLE_2));
                committeeBuySellRangeCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(icdecisionTable, committeeBuySellRangeCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP);

                doc.Add(icdecisionTable);

                PdfPTable icdecisionValueTable = new PdfPTable(4);
                icdecisionValueTable.WidthPercentage = 100;
                icdecisionValueTable.SetWidths(new float[] { 1, 1, 1, 1 });

                PdfPCell recommendationValueCell = new PdfPCell(new Phrase(distinctPresentationFinalizeDetails.First().SecurityRecommendation, PDFFontStyle.STYLE_3));
                AddTextCell(icdecisionValueTable, recommendationValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                String buySellPresented = String.Format("{0} {1:n4}-{2:n4}", distinctPresentationFinalizeDetails.First().SecurityPFVMeasure
                    , distinctPresentationFinalizeDetails.First().SecurityBuyRange, distinctPresentationFinalizeDetails.First().SecuritySellRange);

                PdfPCell presentedBuySellRangeValueCell = new PdfPCell(new Phrase(buySellPresented, PDFFontStyle.STYLE_3));
                AddTextCell(icdecisionValueTable, presentedBuySellRangeValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP_BOTTOM);

                PdfPCell decisionValueCell = new PdfPCell(new Phrase(distinctPresentationFinalizeDetails.First().CommitteeRecommendation, PDFFontStyle.STYLE_3));
                AddTextCell(icdecisionValueTable, decisionValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP_BOTTOM);

                String buySellCommittee = String.Format("{0} {1:n4}-{2:n4}", distinctPresentationFinalizeDetails.First().CommitteePFVMeasure
                    , distinctPresentationFinalizeDetails.First().CommitteeBuyRange, distinctPresentationFinalizeDetails.First().CommitteeSellRange);

                PdfPCell committeeBuySellRangeValueCell = new PdfPCell(new Phrase(buySellCommittee, PDFFontStyle.STYLE_3));
                AddTextCell(icdecisionValueTable, committeeBuySellRangeValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP_BOTTOM);

                doc.Add(icdecisionValueTable);


                PdfPTable votingTable = new PdfPTable(7);
                votingTable.WidthPercentage = 100;
                votingTable.SpacingBefore = 10;
                votingTable.SetWidths(new float[] { 4, 1, 1, 1, 4, 1, 1 });

                PdfPCell voterCell = new PdfPCell();
                AddTextCell(votingTable, voterCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                PdfPCell abstainCell = new PdfPCell(new Phrase("ABSTAIN", PDFFontStyle.STYLE_2));
                abstainCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(votingTable, abstainCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                PdfPCell agreeCell = new PdfPCell(new Phrase("AGREE", PDFFontStyle.STYLE_2));
                agreeCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(votingTable, agreeCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                PdfPCell modifyCell = new PdfPCell(new Phrase("MODIFY", PDFFontStyle.STYLE_2));
                modifyCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(votingTable, modifyCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                PdfPCell pfvCell = new PdfPCell(new Phrase("P/FV", PDFFontStyle.STYLE_2));
                pfvCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(votingTable, pfvCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                PdfPCell buyCell = new PdfPCell(new Phrase("BUY", PDFFontStyle.STYLE_2));
                buyCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(votingTable, buyCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                PdfPCell sellCell = new PdfPCell(new Phrase("SELL", PDFFontStyle.STYLE_2));
                sellCell.BackgroundColor = new BaseColor(255, 240, 240);
                AddTextCell(votingTable, sellCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                doc.Add(votingTable);

                foreach (MeetingMinutesReportData voterDetails in distinctPresentationFinalizeDetails)
                {
                    if (voterDetails.VoteType == null)
                        continue;

                    PdfPTable votingValueTable = new PdfPTable(7);
                    votingValueTable.WidthPercentage = 100;
                    votingValueTable.SetWidths(new float[] { 4, 1, 1, 1, 4, 1, 1 });

                    PdfPCell voterValueCell = new PdfPCell(new Phrase(voterDetails.Name, PDFFontStyle.STYLE_3));
                    AddTextCell(votingValueTable, voterValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                    PdfPCell abstainValueCell = new PdfPCell(new Phrase((voterDetails.VoteType == "Abstain" ? "X" : ""), PDFFontStyle.STYLE_3));
                    AddTextCell(votingValueTable, abstainValueCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                    PdfPCell agreeValueCell = new PdfPCell(new Phrase((voterDetails.VoteType == "Agree" ? "X" : ""), PDFFontStyle.STYLE_3));
                    AddTextCell(votingValueTable, agreeValueCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                    PdfPCell modifyValueCell = new PdfPCell(new Phrase((voterDetails.VoteType == "Modify" ? "X" : ""), PDFFontStyle.STYLE_3));
                    AddTextCell(votingValueTable, modifyValueCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                    PdfPCell pfvValueCell = new PdfPCell(new Phrase(voterDetails.VoterPFVMeasure, PDFFontStyle.STYLE_3));
                    AddTextCell(votingValueTable, pfvValueCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                    PdfPCell buyValueCell = new PdfPCell(new Phrase(String.Format("{0:n4}", voterDetails.VoterBuyRange), PDFFontStyle.STYLE_3));
                    AddTextCell(votingValueTable, buyValueCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                    PdfPCell sellValueCell = new PdfPCell(new Phrase(String.Format("{0:n4}", voterDetails.VoterSellRange), PDFFontStyle.STYLE_3));
                    AddTextCell(votingValueTable, sellValueCell, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_TOP_BOTTOM);

                    doc.Add(votingValueTable);

                }

                Paragraph notes = new Paragraph();
                notes.Add(new Phrase("NOTES: ", PDFFontStyle.STYLE_2));
                notes.Add(new Phrase(distinctPresentationFinalizeDetails.First().AdminNotes, PDFFontStyle.STYLE_3));
                securityDescriptor.SpacingBefore = 10;
                doc.Add(notes);
            }

            doc.Close();
            Byte[] result = File.ReadAllBytes(reportOutputFile);
            File.Delete(reportOutputFile);
            return result;
        }

        private static Byte[] GeneratePreMeetingReport(List<PresentationVotingDeadlineDetails> presentationDeadlineInfo, String securityDesc)
        {
            if (presentationDeadlineInfo == null)
                return null;

            PresentationVotingDeadlineDetails presentationDetails = presentationDeadlineInfo.FirstOrDefault();
            if (presentationDetails == null)
                return null;

            String securityTicker = presentationDetails.SecurityTicker;
            String meetingDate = Convert.ToDateTime(presentationDetails.MeetingDateTime).ToString("MM-dd-yyyy");
            Document doc = new Document(PageSize.A4, 10F, 10F, 10F, 10F);

            String reportOutputFile = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_ICPPreMeetingReport.pdf";
            PdfWriter.GetInstance(doc, new FileStream(reportOutputFile, FileMode.Create));
            doc.Open();
            Rectangle page = doc.PageSize;

            PdfPTable topHeaderTableContent = new PdfPTable(5);
            topHeaderTableContent.WidthPercentage = 100;
            topHeaderTableContent.SetWidths(new float[] { 4, 1, 1, 1, 1 });

            Paragraph securityPara = new Paragraph();
            securityPara.Add(new Phrase(presentationDetails.SecurityName, PDFFontStyle.STYLE_1));
            securityPara.Add(new Phrase(" (" + presentationDetails.SecurityTicker + ")", PDFFontStyle.STYLE_5));

            PdfPCell securityName = new PdfPCell(securityPara);
            securityName.PaddingLeft = 5;

            AddTextCell(topHeaderTableContent, securityName, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM, PDFBorderType.LEFT_TOP);

            PdfPCell securityCountry = new PdfPCell(new Phrase(presentationDetails.SecurityCountry, PDFFontStyle.STYLE_5));
            AddTextCell(topHeaderTableContent, securityCountry, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM, PDFBorderType.TOP);
            PdfPCell securityPFVMeasure = new PdfPCell(new Phrase(presentationDetails.SecurityPFVMeasure, PDFFontStyle.STYLE_5));
            AddTextCell(topHeaderTableContent, securityPFVMeasure, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM, PDFBorderType.TOP);
            String presentationSecurityBuySellRange = String.Format("{0:n4} to {1:n4}", presentationDetails.SecurityBuyRange, presentationDetails.SecuritySellRange);
            PdfPCell securityBuySellRange = new PdfPCell(new Phrase(presentationSecurityBuySellRange, PDFFontStyle.STYLE_5));
            AddTextCell(topHeaderTableContent, securityBuySellRange, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM, PDFBorderType.TOP);
            PdfPCell securityRecommendation = new PdfPCell(new Phrase(presentationDetails.SecurityRecommendation, PDFFontStyle.STYLE_5));
            securityRecommendation.RightIndent = 5;
            AddTextCell(topHeaderTableContent, securityRecommendation, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM, PDFBorderType.RIGHT_TOP);

            doc.Add(topHeaderTableContent);

            PdfPTable topHeaderTableDesc = new PdfPTable(1);
            topHeaderTableDesc.WidthPercentage = 100;
            topHeaderTableDesc.SetWidths(new float[] { 1 });

            PdfPCell securityDescription = new PdfPCell(new Phrase(securityDesc, PDFFontStyle.STYLE_3));
            securityDescription.PaddingLeft = 5;
            securityDescription.PaddingBottom = 5;
            AddTextCell(topHeaderTableDesc, securityDescription, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_BOTTOM);

            doc.Add(topHeaderTableDesc);

            PdfPTable contentTable = new PdfPTable(6);
            contentTable.WidthPercentage = 100;
            contentTable.SpacingBefore = 20;
            contentTable.SpacingAfter = 1;
            contentTable.SetWidths(new float[] { 1, 1, 1, 1, 1, 4 });

            PdfPCell voterNameContent = new PdfPCell(new Phrase("VoterName", PDFFontStyle.STYLE_2));
            voterNameContent.PaddingLeft = 5;
            AddTextCell(contentTable, voterNameContent, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);

            PdfPCell voteContent = new PdfPCell(new Phrase("Vote", PDFFontStyle.STYLE_2));
            voteContent.PaddingLeft = 5;
            AddTextCell(contentTable, voteContent, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);

            PdfPCell modifiedBuyContent = new PdfPCell(new Phrase("Modified Buy", PDFFontStyle.STYLE_2));
            modifiedBuyContent.PaddingLeft = 5;
            AddTextCell(contentTable, modifiedBuyContent, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);

            PdfPCell modifiedSellContent = new PdfPCell(new Phrase("Modified Sell", PDFFontStyle.STYLE_2));
            modifiedSellContent.PaddingLeft = 5;
            AddTextCell(contentTable, modifiedSellContent, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);

            PdfPCell discussionContent = new PdfPCell(new Phrase("Discussion", PDFFontStyle.STYLE_2));
            discussionContent.PaddingLeft = 5;
            AddTextCell(contentTable, discussionContent, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);

            PdfPCell notesContent = new PdfPCell(new Phrase("Notes", PDFFontStyle.STYLE_2));
            notesContent.PaddingLeft = 5;
            AddTextCell(contentTable, notesContent, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.BOTTOM);

            doc.Add(contentTable);

            for (int i = 0; i < presentationDeadlineInfo.Count; i++)
            {
                PdfPTable contentTableRow = new PdfPTable(6);
                contentTableRow.WidthPercentage = 100;
                contentTableRow.SetWidths(new float[] { 1, 1, 1, 1, 1, 4 });

                PdfPCell voterName = new PdfPCell(new Phrase(presentationDeadlineInfo[i].Name, PDFFontStyle.STYLE_3));
                voterName.PaddingLeft = 5;
                if (i % 2 == 0)
                {
                    voterName.BackgroundColor = new BaseColor(255, 240, 240);
                }
                AddTextCell(contentTableRow, voterName, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);

                PdfPCell vote = new PdfPCell(new Phrase(presentationDeadlineInfo[i].VoteType, PDFFontStyle.STYLE_3));
                vote.PaddingLeft = 5;
                if (i % 2 == 0)
                {
                    vote.BackgroundColor = new BaseColor(255, 240, 240);
                }
                AddTextCell(contentTableRow, vote, Element.ALIGN_CENTER, Element.ALIGN_TOP, PDFBorderType.NONE);

                PdfPCell modifiedBuy = new PdfPCell(new Phrase(presentationDeadlineInfo[i].VoteType == "Modify"
                    ? String.Format("{0:n4}", presentationDeadlineInfo[i].VoterBuyRange) : "", PDFFontStyle.STYLE_3));
                modifiedBuy.PaddingLeft = 5;
                if (i % 2 == 0)
                {
                    modifiedBuy.BackgroundColor = new BaseColor(255, 240, 240);
                }
                AddTextCell(contentTableRow, modifiedBuy, Element.ALIGN_CENTER, Element.ALIGN_TOP, PDFBorderType.NONE);

                PdfPCell modifiedSell = new PdfPCell(new Phrase(presentationDeadlineInfo[i].VoteType == "Modify"
                    ? String.Format("{0:n4}", presentationDeadlineInfo[i].VoterSellRange) : "", PDFFontStyle.STYLE_3));
                modifiedSell.PaddingLeft = 5;
                if (i % 2 == 0)
                {
                    modifiedSell.BackgroundColor = new BaseColor(255, 240, 240);
                }
                AddTextCell(contentTableRow, modifiedSell, Element.ALIGN_CENTER, Element.ALIGN_TOP, PDFBorderType.NONE);

                PdfPCell discussion = new PdfPCell(new Phrase(presentationDeadlineInfo[i].DiscussionFlag == true ? "X" : "", PDFFontStyle.STYLE_3));
                discussion.PaddingLeft = 5;
                if (i % 2 == 0)
                {
                    discussion.BackgroundColor = new BaseColor(255, 240, 240);
                }
                AddTextCell(contentTableRow, discussion, Element.ALIGN_CENTER, Element.ALIGN_TOP, PDFBorderType.NONE);

                PdfPCell notes = new PdfPCell(new Phrase(presentationDeadlineInfo[i].Notes, PDFFontStyle.STYLE_3));
                notes.PaddingLeft = 5;
                if (i % 2 == 0)
                {
                    notes.BackgroundColor = new BaseColor(255, 240, 240);
                }
                AddTextCell(contentTableRow, notes, Element.ALIGN_LEFT, Element.ALIGN_TOP, PDFBorderType.NONE);


                doc.Add(contentTableRow);

            }

            doc.Close();
            Byte[] result = File.ReadAllBytes(reportOutputFile);
            File.Delete(reportOutputFile);
            return result;

        }

        private static void AddTextCell(PdfPTable table, PdfPCell cell, int HorizontalAlignment = Element.ALIGN_LEFT, int VerticalAlignment = Element.ALIGN_MIDDLE, int Border = 0)
        {
            //cell.Colspan = Colspan;
            cell.HorizontalAlignment = HorizontalAlignment; //0=Left, 1=Centre, 2=Right
            cell.VerticalAlignment = VerticalAlignment;
            cell.Border = Border;
            cell.BorderWidth = 1;
            //cell.Rowspan = Rowspan;
            table.AddCell(cell);
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

        #region Messaging
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean SetMessageInfo(String emailTo, String emailCc, String emailSubject
            , String emailMessageBody, String emailAttachment, String userName)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetMessageInfo(emailTo, emailCc, emailSubject, emailMessageBody, emailAttachment, userName).FirstOrDefault();
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

        #region Summary Report
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<SummaryReportData> RetrieveSummaryReportData(DateTime startDate, DateTime endDate)
        {
            try
            {
                ICPresentationEntities iCPEntity = new ICPresentationEntities();
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();

                List<SummaryReportData> result = iCPEntity.RetrieveSummaryReportDetails(startDate, endDate).ToList();

                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);

                foreach (SummaryReportData item in result)
                {

                    DimensionEntitiesService.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                            .Where(record => record.TICKER == item.SecurityTicker
                                && record.ISSUE_NAME == item.SecurityName)
                            .FirstOrDefault();

                    List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> securityHoldingData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.ISSUE_NAME == item.SecurityName && record.TICKER == item.SecurityTicker && record.PORTFOLIO_DATE == lastBusinessDate
                    && record.DIRTY_VALUE_PC > 0)
                    .ToList();

                    decimal? sumSecDirtyValuePC = securityHoldingData.Sum(record => record.DIRTY_VALUE_PC);
                    decimal? sumSecBalanceNominal = securityHoldingData.Sum(record => record.BALANCE_NOMINAL);
                    if (sumSecDirtyValuePC != null)
                    {
                        item.CurrentCashPosition = Convert.ToSingle(sumSecDirtyValuePC);
                        item.CurrentPosition = Convert.ToInt64(sumSecBalanceNominal);
                    }

                    String securityId = securityData.SECURITY_ID == null ? null : securityData.SECURITY_ID.ToString();
                    FAIR_VALUE fairValueRecord = externalResearchEntity.FAIR_VALUE.Where(record => record.VALUE_TYPE == "PRIMARY"
                        && record.SECURITY_ID == securityId).FirstOrDefault();

                    if (fairValueRecord != null)
                    {
                        DATA_MASTER dataMasterRecord = externalResearchEntity.DATA_MASTER
                            .Where(record => record.DATA_ID == fairValueRecord.FV_MEASURE).FirstOrDefault();

                        if (dataMasterRecord != null)
                        {
                            item.CurrentPFVMeasure = dataMasterRecord.DATA_DESC;
                            item.CurrentBuySellRange = String.Format("{0:n2} - {1:n2}", fairValueRecord.FV_BUY, fairValueRecord.FV_SELL);
                            item.CurrentPFVMeasureValue = fairValueRecord.CURRENT_MEASURE_VALUE;
                            if (fairValueRecord.UPSIDE != null)
                                item.CurrentUpside = Convert.ToSingle(fairValueRecord.UPSIDE);
                        }
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
        #endregion

    }
}






