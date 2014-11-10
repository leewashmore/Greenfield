using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web.Security;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using GreenField.DAL;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web;
using System.Net.Configuration;
//using System.Drawing;
namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for Security Reference
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MeetingOperations
    {
        #region Properties
        /// <summary>
        /// Stores Dimension Service entities
        /// </summary>
        /*private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                {
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
                }
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
        /// MaxWidth of a Column
        /// </summary>
        private static DoubleValue maxWidth;
        public static DoubleValue MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        /// <summary>
        /// Stores service fault exception custom exception resource manager
        /// </summary>
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }
        #endregion

        #region Presentation Overview
        /// <summary>
        /// Retrieve presentations overview information
        /// </summary>
        /// <param name="objUserID"></param>
        /// <returns>List of ICPresentationOverviewData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<ICPresentationOverviewData> RetrievePresentationOverviewData(String userName,String status)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                return entity.RetrieveICPresentationOverviewData(userName,status).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Retrieves application user information for specific usernames
        /// </summary>
        /// <param name="userNames">application user names</param>
        /// <returns>List of MembershipUserInfo</returns>
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
        #region Operation Contracts
        /// <summary>
        /// Creates new presentation with voting objects and powerpoint presentation
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationOverviewData">presentation overview information</param>
        /// <returns>True if successful</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public PresentationFile CreatePresentation(String userName, ICPresentationOverviewData presentationOverviewData,string template)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                PresentationFile pf = new PresentationFile();
                #region Creation of PresentationInfo and VoterInfo objects
                XDocument xmlDoc = GetEntityXml<ICPresentationOverviewData>(new List<ICPresentationOverviewData> { presentationOverviewData });
                String[] votingUsers = Roles.GetUsersInRole("IC_MEMBER_VOTING");

                //add to voting list all ic voting members, ic chief executive and presenter role users
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
               
                #endregion

                #region Generate powerpoint presentation
                string presentationFile=null;
                if (template == "Full")
                {

                    presentationFile = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\Templates\Full IC Report template.pptx";
                }
                else if (template == "Abbreviated")
                {
                    presentationFile = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\Templates\Abbrev IC Report template.pptx";
                }
                if (!File.Exists(presentationFile))
                {
                    throw new Exception("Missing IC Presentation Template file");
                }


                //generate presentation info and voter info in database
                String xmlScript = xmlDoc.ToString();
                Int64? result = entity.SetPresentationInfo(userName, xmlScript).FirstOrDefault();
                presentationOverviewData.PresentationID = (long)result;
                pf.PresentationId = (long)result;
                if (result < 0)
                { throw new Exception("Exception occurred while creating the presentation!!!"); }

                String copiedFilePath = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_NewICPresentation.pptx";
                File.Copy(presentationFile, copiedFilePath, true);

                try
                {
                    using (PresentationDocument presentationDocument = PresentationDocument.Open(copiedFilePath, true))
                    {
                        PresentationPart presentatioPart = presentationDocument.PresentationPart;
                        OpenXmlElementList slideIds = presentatioPart.Presentation.SlideIdList.ChildElements;

                        string relId = (slideIds[0] as SlideId).RelationshipId;

                        //get the slide part from the relationship id
                        SlidePart slidePart = (SlidePart)presentatioPart.GetPartById(relId);
                       
                        //SetSlideTitleonFullPresentation(slidePart, presentationOverviewData);
                        SetSlideTitle(slidePart, presentationOverviewData);
                        SetSlideContent(slidePart, presentationOverviewData);

                        //save the slide and presentation
                        slidePart.Slide.Save();
                        presentatioPart.Presentation.Save();
                    }
                }
                catch
                {
                    throw new Exception("Exception occurred while opening powerpoint presentation!!!");
                }
                #endregion

                //#region Upload generated powerpoint presentation and create file master object
                Byte[] fileStream = File.ReadAllBytes(copiedFilePath);
                pf.FileStream = fileStream;
               /* String fileName = removeSpecialCharacters(presentationOverviewData.SecurityName) + "_" + (presentationOverviewData.MeetingClosedDateTime.HasValue
                    ? Convert.ToDateTime(presentationOverviewData.MeetingClosedDateTime).ToString("ddMMyyyy") : String.Empty) + ".pptx";*/

                /*GreenField.DAL.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                .Where(record => record.ISSUE_NAME == presentationOverviewData.SecurityName
                    && record.TICKER == presentationOverviewData.SecurityTicker).FirstOrDefault();*/

               /* GreenField.DAL.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                .Where(record => record.SECURITY_ID == presentationOverviewData.Security_id).FirstOrDefault();
                String issuerName = securityRecord == null ? null : securityRecord.ISSUER_NAME;

                DocumentWorkspaceOperations documentWorkspaceOperations = new DocumentWorkspaceOperations();
                String url = documentWorkspaceOperations.UploadDocument(fileName, File.ReadAllBytes(copiedFilePath), String.Empty);

                if (url == String.Empty)
                    throw new Exception("Exception occurred while uploading template powerpoint presentation!!!");

                FileMaster fileMaster = new FileMaster()
                {
                    Category = "Power Point Presentation",
                    Location = url,
                    Name = removeSpecialCharacters(presentationOverviewData.SecurityName) + "_" + (presentationOverviewData.MeetingClosedDateTime.HasValue
                    ? Convert.ToDateTime(presentationOverviewData.MeetingClosedDateTime).ToString("ddMMyyyy") : String.Empty) + ".pptx",
                    IssuerName = issuerName,
                    SecurityName = presentationOverviewData.SecurityName,
                    SecurityTicker = presentationOverviewData.SecurityTicker,
                    Type = EnumUtils.ToString(DocumentCategoryType.IC_PRESENTATIONS),
                    CreatedBy = userName,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = userName,
                    ModifiedOn = DateTime.UtcNow
                };
                #endregion
                if (UpdatePresentationAttachedFileStreamData(userName, Convert.ToInt64(result), fileMaster, false))
                {
                    return (long)result;
                }
                else { return -1; };*/

                //return UpdatePresentationAttachedFileStreamData(userName, Convert.ToInt64(result), fileMaster, false);
                return pf;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }


   



        private String removeSpecialCharacters(String str)
        {
            return Regex.Replace(str, @"[^\w\.@-]", "");
                
        }


        /// <summary>
        /// Delete Presentation information from database and files from sharepoint
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationOverviewData">presentation overview information</param>
        /// <returns>True if successful</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean DeletePresentation(String userName, ICPresentationOverviewData presentationOverviewData)
        {
            try
            {
                //Deleting files from Sharepoint
                DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
                List<FileMaster> presentationAttachedFiles = RetrievePresentationAttachedFileDetails(presentationOverviewData.PresentationID);
                if (presentationAttachedFiles != null)
                {
                    List<FileMaster> icPacketFiles = presentationAttachedFiles.ToList();
                    foreach (FileMaster record in icPacketFiles)
                    {
                        documentOperations.DeleteDocument(record.Location);
                    }
                }

                //Deleting entries from database
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.DeletePresentationInfo(presentationOverviewData.PresentationID).FirstOrDefault();


                return result==0;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
                
            }
           

        }


        
        /// <summary>
        /// Changes status type of a specific presentation
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationId">presentationId</param>
        /// <param name="status">status type</param>
        /// <returns>True if successful</returns>
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

        /// <summary>
        /// Retrieves details concerning ic presentation for specific security and portfolio
        /// </summary>
        /// <param name="entitySelectionData">EntitySelectionData</param>
        /// <param name="presentationOverviewData">ICPresentationOverviewData</param>
        /// <param name="portfolio">PortfolioSelectionData</param>
        /// <returns>enriched ICPresentationOverviewData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public ICPresentationOverviewData RetrieveSecurityDetails(EntitySelectionData entitySelectionData
            , ICPresentationOverviewData presentationOverviewData, PortfolioSelectionData portfolio)
        {
            try
            {
                DimensionEntities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();

                /*bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }*/
                //manual inputs
 

                presentationOverviewData.PortfolioId = portfolio.PortfolioId;
                presentationOverviewData.YTDRet_Absolute = "0.00%";
                presentationOverviewData.YTDRet_RELtoLOC = "0.00%";
                presentationOverviewData.YTDRet_RELtoEM = "0.00%";

                #region GF_SECURITY_BASEVIEW info
                GreenField.DAL.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                            .Where(record => record.SECURITY_ID == entitySelectionData.SecurityId)
                            .FirstOrDefault();
                GreenField.DAL.PRICE prices = externalResearchEntity.PRICES.Where(record => record.SECURITY_ID == entitySelectionData.SecurityId).OrderByDescending(x => x.PRICE_DATE).FirstOrDefault();

                presentationOverviewData.SecurityTicker = securityData.TICKER ;
                presentationOverviewData.SecurityName = securityData.ISSUE_NAME ;
                presentationOverviewData.SecurityCountry = securityData.ASEC_SEC_COUNTRY_NAME ;
                presentationOverviewData.SecurityCountryCode = securityData.ISO_COUNTRY_CODE;
                presentationOverviewData.SecurityIndustry = securityData.GICS_INDUSTRY_NAME;
                presentationOverviewData.Analyst = securityData.ASHMOREEMM_PRIMARY_ANALYST;
                presentationOverviewData.Security_id = securityData.SECURITY_ID;
                presentationOverviewData.Issuer_id = securityData.ISSUER_ID;

                if (securityData.CLOSING_PRICE != null)
                {
                    presentationOverviewData.SecurityLastClosingPrice = Convert.ToSingle(securityData.CLOSING_PRICE);
                }
                presentationOverviewData.Price = (prices == null ? "" : String.Format("{0:n2}", prices.PRICE1))
                    + " " + (securityData.TRADING_CURRENCY == null ? "" : securityData.TRADING_CURRENCY.ToString());
                #endregion

                #region Current Holdings info
                //portfolio.IsComposite
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
          
                GreenField.DAL.GF_PORTFOLIO_HOLDINGS lastBusinessRecordPortfolio = null;
                GreenField.DAL.GF_COMPOSITE_LTHOLDINGS lastBusinessRecordComposite = null;
                if (portfolio.IsComposite)
                {
                    lastBusinessRecordComposite = externalResearchEntity.GF_COMPOSITE_LTHOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                }else
                {
                    lastBusinessRecordPortfolio = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                }

                if (lastBusinessRecordComposite != null)
                {
                    if (lastBusinessRecordComposite.PORTFOLIO_DATE != null)
                    {
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecordComposite.PORTFOLIO_DATE);
                    }
                }
                else if (lastBusinessRecordPortfolio != null)
                {
                    if (lastBusinessRecordPortfolio.PORTFOLIO_DATE != null)
                    {
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecordComposite.PORTFOLIO_DATE);
                    }
                }

                decimal? sumSecDirtyValuePC;
                decimal? sumSecBalanceNominal;
                decimal? sumDirtyValuePC;
                decimal? tempNAV;
                string benchmarkID;
                if (portfolio.IsComposite)
                {
                    List<GF_COMPOSITE_LTHOLDINGS> securityHoldingData = externalResearchEntity.GF_COMPOSITE_LTHOLDINGS.Where(
                        record => record.ASEC_SEC_SHORT_NAME== entitySelectionData.InstrumentID && record.PORTFOLIO_DATE == lastBusinessDate
                        && record.DIRTY_VALUE_PC > 0).ToList();

                    sumSecDirtyValuePC = securityHoldingData.Sum(record => record.DIRTY_VALUE_PC);
                    sumSecBalanceNominal = securityHoldingData.Sum(record => record.BALANCE_NOMINAL);

                    List<GF_COMPOSITE_LTHOLDINGS> portfolioData = externalResearchEntity.GF_COMPOSITE_LTHOLDINGS.Where(record => record.PORTFOLIO_ID == portfolio.PortfolioId && record.PORTFOLIO_DATE == lastBusinessDate).ToList();
                    sumDirtyValuePC = portfolioData.Sum(record => record.DIRTY_VALUE_PC);

                    if (securityHoldingData != null && sumSecDirtyValuePC > 0)
                    {
                        presentationOverviewData.CurrentHoldings = "YES";
                        if (sumDirtyValuePC != 0)
                        {
                            presentationOverviewData.PercentEMIF = String.Format("{0:n2}%", ((sumSecDirtyValuePC / sumDirtyValuePC) * 100));
                        }
                        tempNAV = ((sumSecDirtyValuePC / sumDirtyValuePC) * 100);
                    }
                    else
                    {
                        presentationOverviewData.CurrentHoldings = "No";
                        presentationOverviewData.PercentEMIF = "0%";
                        tempNAV = 0;
                    }
                   benchmarkID = portfolioData.Select(a => a.BENCHMARK_ID).FirstOrDefault();

                }
                else
                {
                    List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> securityHoldingData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID && record.PORTFOLIO_DATE == lastBusinessDate
                    && record.DIRTY_VALUE_PC > 0)
                    .ToList();

                    sumSecDirtyValuePC = securityHoldingData.Sum(record => record.DIRTY_VALUE_PC);
                    sumSecBalanceNominal = securityHoldingData.Sum(record => record.BALANCE_NOMINAL);

                    List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> portfolioData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                 record => record.PORTFOLIO_ID == portfolio.PortfolioId && record.PORTFOLIO_DATE == lastBusinessDate).ToList();
                     sumDirtyValuePC = portfolioData.Sum(record => record.DIRTY_VALUE_PC);


                     if (securityHoldingData != null && sumSecDirtyValuePC > 0)
                     {
                         presentationOverviewData.CurrentHoldings = "YES";
                         if (sumDirtyValuePC != 0)
                         {
                             presentationOverviewData.PercentEMIF = String.Format("{0:n2}%", ((sumSecDirtyValuePC / sumDirtyValuePC) * 100));
                         }
                         tempNAV = ((sumSecDirtyValuePC / sumDirtyValuePC) * 100);
                     }
                     else
                     {
                         presentationOverviewData.CurrentHoldings = "No";
                         presentationOverviewData.PercentEMIF = "0%";
                         tempNAV = 0;
                     }
                      benchmarkID = portfolioData.Select(a => a.BENCHMARK_ID).FirstOrDefault();
                }

                if (sumSecDirtyValuePC != null)
                {
                    presentationOverviewData.SecurityCashPosition = Convert.ToSingle(sumSecDirtyValuePC);
                    presentationOverviewData.SecurityPosition = Convert.ToInt64(sumSecBalanceNominal);
                }
             

            

           /*     if (securityInPortfolio != null && sumSecurityDirtyValuePC > 0)
                {
                    presentationOverviewData.CurrentHoldings = "YES";
                    if (sumDirtyValuePC != 0)
                    {
                        presentationOverviewData.PercentEMIF = String.Format("{0:n2}%", ((sumSecurityDirtyValuePC / sumDirtyValuePC) * 100));
                    }
                    tempNAV = ((sumSecurityDirtyValuePC / sumDirtyValuePC) * 100);
                }
                else
                {
                    presentationOverviewData.CurrentHoldings = "No";
                    presentationOverviewData.PercentEMIF = "0%";
                    tempNAV = 0;
                } */
                #endregion

                #region GF_BENCHMARK_HOLDINGS Info
              
                GreenField.DAL.GF_BENCHMARK_HOLDINGS benchmarkData = entity.GF_BENCHMARK_HOLDINGS.Where(record => record.BENCHMARK_ID == benchmarkID
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.PORTFOLIO_DATE == lastBusinessDate)
                    .FirstOrDefault();

                if (benchmarkData != null)
                {
                    presentationOverviewData.SecurityBMWeight = String.Format("{0:n2}%", benchmarkData.BENCHMARK_WEIGHT);
                    tempNAV = (tempNAV - benchmarkData.BENCHMARK_WEIGHT);
                    presentationOverviewData.SecurityActiveWeight = String.Format("{0:n2}%", tempNAV);
                }
                else
                {
                    presentationOverviewData.SecurityBMWeight = "0%";
                    presentationOverviewData.SecurityActiveWeight = String.Format("{0:n2}%", tempNAV);
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
                        //presentationOverviewData.Data_id     = (int?)dataMasterRecord.DATA_ID;
                        presentationOverviewData.SecurityBuyRange = Convert.ToSingle(fairValueRecord.FV_BUY);
                        presentationOverviewData.SecuritySellRange = Convert.ToSingle(fairValueRecord.FV_SELL);
                        presentationOverviewData.SecurityPFVMeasureValue = fairValueRecord.CURRENT_MEASURE_VALUE;
                        presentationOverviewData.FVCalc = String.Concat(String.Format("{0:n2}",fairValueRecord.UPSIDE ),"x ",dataMasterRecord.DATA_DESC);

                        if (fairValueRecord.CURRENT_MEASURE_VALUE != 0)
                        {
                          /*  presentationOverviewData.SecurityBuySellvsCrnt = String.Format("{0} {1:n2} - {0} {2:n2}",
                                securityData.TRADING_CURRENCY,
                                ((fairValueRecord.FV_BUY * (decimal)securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE),
                                ((fairValueRecord.FV_SELL * (decimal)securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE));
                            
                           */
                            presentationOverviewData.SecurityBuySellvsCrnt = String.Concat(String.Format("{0:n2}",  fairValueRecord.CURRENT_MEASURE_VALUE),"x ", dataMasterRecord.DATA_DESC );

                        }
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
                        String securityID = securityData.SECURITY_ID.ToString();
                        PERIOD_FINANCIALS_SECURITY periodFinancialRecord = externalResearchEntity.PERIOD_FINANCIALS_SECURITY
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

        /// <summary>
        /// retrieved current measure values for all PFV measure types
        /// </summary>
        /// <param name="PFVTypeInfo">PFV measure types</param>
        /// <param name="securityTicker">security ticker</param>
        /// <returns>Dictionary object with PFV measure type as key and current measure as it's value</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Dictionary<String, Decimal?> RetrieveCurrentPFVMeasures(List<String> PFVTypeInfo, String securityTicker)
        {
            try
            {
                DimensionEntities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();
                Dictionary<String, Decimal?> result = new Dictionary<string, decimal?>();

              /*  bool isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }*/
                GreenField.DAL.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
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
                            PERIOD_FINANCIALS_SECURITY periodFinancialRecord = externalResearchEntity.PERIOD_FINANCIALS_SECURITY
                                .Where(record => record.SECURITY_ID == securityID
                                    && record.DATA_ID == dataMasterRecord.DATA_ID
                                    && record.CURRENCY == "USD"
                                    && record.PERIOD_TYPE == "C").FirstOrDefault();
                            if (periodFinancialRecord != null)
                            {
                                value = periodFinancialRecord.AMOUNT;
                                result.Add(pfvType, value);
                            }
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

        #region Private methods
        /// <summary>
        /// Get the title string of the slide
        /// </summary>
        /// <param name="slidePart">SlidePart</param>
        /// <param name="presentationOverviewData">ICPresentationOverviewData</param>
        private void SetSlideTitle(SlidePart slidePart, ICPresentationOverviewData presentationOverviewData)
        {
            if (slidePart == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            //declare a paragraph separator.

            if (slidePart.Slide != null)
            {
                //find all the title shapes.
                var shapes = from shape in slidePart.Slide.Descendants<Shape>()
                             where IsTitleShape(shape)
                             select shape;


                foreach (var shape in shapes)
                {
                    //get the text in each paragraph in this shape.
                    foreach (var paragraph in shape.TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        
                        
                        foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            text.Text = presentationOverviewData.SecurityName ;
                        }
                        
                    }
                }
            }
        }


        /// <summary>
        /// Get the title string of the slide
        /// </summary>
        /// <param name="slidePart">SlidePart</param>
        /// <param name="presentationOverviewData">ICPresentationOverviewData</param>
        private void SetSlideTitleonFullPresentation(SlidePart slidePart, ICPresentationOverviewData presentationOverviewData)
        {
            if (slidePart == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            //declare a paragraph separator.
           

            if (slidePart.Slide != null)
            {
                                
                //find all the title shapes.
                var shapes = from shape in slidePart.Slide.Descendants<Shape>()
                             where IsTitleShape(shape)
                             select shape;

                foreach (var shape in shapes)
                {
                    //get the text in each paragraph in this shape.
                   
                    foreach (var paragraph in shape.TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        //sdd a line break.
                       

                        foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            
                            text.Text = presentationOverviewData.SecurityName ;
                            
                            
                        }
                        
                    }
                    
                }
                
            }
        }


        /// <summary>
        /// Get the title string of the slide
        /// </summary>
        /// <param name="slidePart">SlidePart</param>
        /// <param name="presentationOverviewData">ICPresentationOverviewData</param>
        private void SetSlideContent(SlidePart slidePart, ICPresentationOverviewData presentationOverviewData)
        {
            //get security information    
            CommonSlideData commonSlideData = slidePart.Slide.Descendants<CommonSlideData>().FirstOrDefault();
            List<GraphicFrame> graphicFrames = commonSlideData.Descendants<GraphicFrame>().ToList();
            DocumentFormat.OpenXml.Drawing.Table tblSecurityOverview = graphicFrames[0].Graphic.GraphicData.Descendants<DocumentFormat.OpenXml.Drawing.Table>().FirstOrDefault();

            SwapPlaceholderText(tblSecurityOverview, 0, 2, presentationOverviewData.Analyst);
            SwapPlaceholderText(tblSecurityOverview, 0, 4, presentationOverviewData.CurrentHoldings);  
            SwapPlaceholderText(tblSecurityOverview, 1, 2, presentationOverviewData.SecurityCountry);
            SwapPlaceholderText(tblSecurityOverview, 1, 4, presentationOverviewData.PercentEMIF);
            SwapPlaceholderText(tblSecurityOverview, 2, 2, presentationOverviewData.SecurityIndustry);
            SwapPlaceholderText(tblSecurityOverview, 2, 4, presentationOverviewData.SecurityBMWeight); 
            SwapPlaceholderText(tblSecurityOverview, 3, 2, presentationOverviewData.SecurityMarketCapitalization.ToString());
            SwapPlaceholderText(tblSecurityOverview, 3, 4, presentationOverviewData.SecurityActiveWeight);
            SwapPlaceholderText(tblSecurityOverview, 4, 2, presentationOverviewData.Price.ToString());
            SwapPlaceholderText(tblSecurityOverview, 4, 4, presentationOverviewData.YTDRet_Absolute);
            SwapPlaceholderText(tblSecurityOverview, 5, 2, presentationOverviewData.FVCalc);
           
            SwapPlaceholderText(tblSecurityOverview, 5, 4, presentationOverviewData.YTDRet_RELtoLOC);
            SwapPlaceholderText(tblSecurityOverview, 6, 2, presentationOverviewData.SecurityBuySellvsCrnt);
            SwapPlaceholderText(tblSecurityOverview, 6, 4, presentationOverviewData.YTDRet_RELtoEM);
         
            SwapPlaceholderText(tblSecurityOverview, 7, 2, presentationOverviewData.SecurityPFVMeasure.ToString() + " " +
         presentationOverviewData.SecurityBuyRange.ToString() + "-" + presentationOverviewData.SecuritySellRange.ToString());
            SwapPlaceholderText(tblSecurityOverview, 7, 4, " ");
        }

        /// <summary>
        /// Determines whether the shape is a title shape
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <returns>True if shape is title</returns>
        private bool IsTitleShape(Shape shape)
        {
            var placeholderShape = shape.NonVisualShapeProperties.ApplicationNonVisualDrawingProperties.GetFirstChild<PlaceholderShape>();
            if (placeholderShape != null && placeholderShape.Type != null && placeholderShape.Type.HasValue)
            {
                switch ((PlaceholderValues)placeholderShape.Type)
                {
                    // Any title shape.
                    case PlaceholderValues.Title:

                    // A centered title.
                 //   case PlaceholderValues.CenteredTitle:
                        return true;

                    default:
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Swaps text in a powerpoint table placeholder
        /// </summary>
        /// <param name="tbl">Table</param>
        /// <param name="rowNum">Row number</param>
        /// <param name="columnNum">Column number</param>
        /// <param name="value">Rreplacement value</param>
        private void SwapPlaceholderText(DocumentFormat.OpenXml.Drawing.Table tbl, int rowNum, int columnNum, string value)
        {
            DocumentFormat.OpenXml.Drawing.TableRow row = tbl.Descendants<DocumentFormat.OpenXml.Drawing.TableRow>().ElementAt(rowNum);
            DocumentFormat.OpenXml.Drawing.TableCell cell = row.Descendants<DocumentFormat.OpenXml.Drawing.TableCell>().ElementAt(columnNum - 1);


            DocumentFormat.OpenXml.Drawing.TextBody textBody = cell.Descendants<DocumentFormat.OpenXml.Drawing.TextBody>().FirstOrDefault();
            DocumentFormat.OpenXml.Drawing.Paragraph paragraph = textBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>().FirstOrDefault();
            DocumentFormat.OpenXml.Drawing.Run run = paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Run>().FirstOrDefault();
            DocumentFormat.OpenXml.Drawing.Text text = run.Text;//run.Descendants<DocumentFormat.OpenXml.Drawing.Text>().FirstOrDefault();
                                            

            //DocumentFormat.OpenXml.Drawing.Paragraph paragraph = cell.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>().FirstOrDefault();
           // DocumentFormat.OpenXml.Drawing.Text text = cell.Descendants<DocumentFormat.OpenXml.Drawing.Text>().FirstOrDefault();
            //DocumentFormat.OpenXml.Drawing.Text text = paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>().FirstOrDefault();
            text.Text = value;
        } 
        #endregion
        #endregion

        #region Upload/Edit Presentation Documents
        #region Operation Contracts
        /// <summary>
        /// Retrieves attached file details for a specific presentation
        /// </summary>
        /// <param name="presentationID">Presentation ID</param>
        /// <returns>List of FileMaster</returns>
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

        /// <summary>
        /// resubmits presentation - updates powerpoint presentation and ic packet, sends alert to voting group based on input criterion
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationOverviewData">ICPresentationOverviewData</param>
        /// <param name="sendAlert">option to send alert to voting group</param>
        /// <returns>True if successful</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean ReSubmitPresentation(String userName, ICPresentationOverviewData presentationOverviewData, Boolean sendAlert)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
                //recalculate presentation overview data
                presentationOverviewData = RetrieveUpdatedSecurityDetails(presentationOverviewData);
                //update presentation info object
                XDocument xmlDoc = GetEntityXml<ICPresentationOverviewData>(new List<ICPresentationOverviewData> { presentationOverviewData });
                string xmlScript = xmlDoc.ToString();
                Int32? result = entity.UpdatePresentationInfo(userName, xmlScript).FirstOrDefault();
                if (result == null)
                    throw new Exception("Unable to update presentation info object");
                List<FileMaster> presentationAttachedFiles = RetrievePresentationAttachedFileDetails(presentationOverviewData.PresentationID);
                GreenField.DAL.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.SECURITY_ID == presentationOverviewData.Security_id).FirstOrDefault();
                String issuerName = securityRecord == null ? null : securityRecord.ISSUER_NAME;

               FileMaster icPacket = null;
                #region IC Packet
                if (presentationAttachedFiles != null)
                {
                    icPacket = presentationAttachedFiles.Where(record => record.Category == "Investment Committee Packet").FirstOrDefault();
                    if (icPacket != null)
                    {

                        documentOperations.DeleteDocument(icPacket.Location);
                    }
                    
                }

                Byte[] generatedICPacketStream = GenerateICPacketReport(presentationOverviewData.PresentationID);
                String emailAttachments = null;
                String uploadFileName = String.Format("{0}_{1}_ICPacket.pdf"
                            , Convert.ToDateTime(presentationOverviewData.MeetingDateTime).ToString("MMddyyyy")
                            , removeSpecialCharacters(presentationOverviewData.SecurityTicker));
                if (generatedICPacketStream != null)
                {
                    String uploadFileLocation = documentOperations.UploadDocument(uploadFileName, generatedICPacketStream, String.Empty);
                    FileMaster fileMaster_ICPacket = null;
                    
                    if(icPacket == null)
                    {
                            fileMaster_ICPacket = new FileMaster()
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
                    }else
                    {
                        fileMaster_ICPacket = icPacket;
                        
                        fileMaster_ICPacket.Location=uploadFileLocation;
                        fileMaster_ICPacket.Name = uploadFileName;
                        fileMaster_ICPacket.CreatedBy = userName;
                        fileMaster_ICPacket.CreatedOn = DateTime.UtcNow;
                        fileMaster_ICPacket. ModifiedBy = userName;
                        fileMaster_ICPacket.ModifiedOn = DateTime.UtcNow;
                    }

                    UpdatePresentationAttachedFileStreamData(userName, presentationOverviewData.PresentationID, fileMaster_ICPacket,false);

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

        /// <summary>
        /// Updates attached files for a specific presentation and optionally deletes a pre-existing document
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationId">presentation id</param>
        /// <param name="fileMasterInfo">file master information</param>
        /// <param name="deletionFlag">deletion flag</param>
        /// <returns>True if successful</returns>
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

        /// <summary>
        /// generates ic packet report for a specific presentation
        /// </summary>
        /// <param name="presentationId">presentation id</param>
        /// <returns>byte stream of the generated file</returns>
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
                        && (record.Category == "Presentation"
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
        #endregion

        #region Private Methods
        /// <summary>
        /// Retrieves updated security details of a specific presentation and returns enriched object
        /// </summary>
        /// <param name="presentationOverviewData">ICPresentationOverviewData object</param>
        /// <returns>enriched ICPresentationOverviewData object</returns>
        private ICPresentationOverviewData RetrieveUpdatedSecurityDetails(ICPresentationOverviewData presentationOverviewData)
        {
            try
            {
                DimensionEntities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();

                /*bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }*/

                #region GF_SECURITY_BASEVIEW info
                GreenField.DAL.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                            .Where(record => record.SECURITY_ID==presentationOverviewData.Security_id)
                            .FirstOrDefault();

                presentationOverviewData.SecurityCountry = securityData.ASEC_SEC_COUNTRY_NAME;
                presentationOverviewData.SecurityName = securityData.ISSUE_NAME;
                presentationOverviewData.SecurityCountryCode = securityData.ISO_COUNTRY_CODE;
                presentationOverviewData.SecurityIndustry = securityData.GICS_INDUSTRY_NAME;
                presentationOverviewData.Analyst = securityData.ASHMOREEMM_PRIMARY_ANALYST;
                presentationOverviewData.Security_id = securityData.SECURITY_ID;
                presentationOverviewData.Issuer_id = securityData.ISSUER_ID;
                if (securityData.CLOSING_PRICE != null)
                    presentationOverviewData.SecurityLastClosingPrice = Convert.ToSingle(securityData.CLOSING_PRICE);

                presentationOverviewData.Price = (securityData.CLOSING_PRICE == null ? "" : securityData.CLOSING_PRICE.ToString())
                    + " " + (securityData.TRADING_CURRENCY == null ? "" : securityData.TRADING_CURRENCY.ToString());
                #endregion

                #region GF_PORTFOLIO_HOLDINGS info
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GreenField.DAL.GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);

                List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> securityHoldingData = entity.GF_PORTFOLIO_HOLDINGS.Where(
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

                List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> portfolioData = entity.GF_PORTFOLIO_HOLDINGS.Where(
                    record => record.PORTFOLIO_ID == presentationOverviewData.PortfolioId && record.PORTFOLIO_DATE == lastBusinessDate)
                    .ToList();
                decimal? sumDirtyValuePC = portfolioData.Sum(record => record.DIRTY_VALUE_PC);
                decimal? tempNAV;

                List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> securityInPortfolio = portfolioData
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
                GreenField.DAL.GF_BENCHMARK_HOLDINGS benchmarkData = entity.GF_BENCHMARK_HOLDINGS.Where(
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
                        presentationOverviewData.Data_id = (int?) dataMasterRecord.DATA_ID;
                        presentationOverviewData.SecurityPFVMeasure = dataMasterRecord.DATA_DESC;
                        presentationOverviewData.SecurityBuyRange = Convert.ToSingle(fairValueRecord.FV_BUY);
                        presentationOverviewData.SecuritySellRange = Convert.ToSingle(fairValueRecord.FV_SELL);
                        presentationOverviewData.SecurityPFVMeasureValue = fairValueRecord.CURRENT_MEASURE_VALUE;
                        presentationOverviewData.FVCalc = String.Format("{0} {1:n2} - {2:n2}",
                            dataMasterRecord.DATA_DESC, fairValueRecord.FV_BUY, fairValueRecord.FV_SELL) ;
                        if (fairValueRecord.CURRENT_MEASURE_VALUE != 0)
                        {
                            presentationOverviewData.SecurityBuySellvsCrnt = String.Format("{0} {1:n2} - {0} {2:n2}",
                                securityData.TRADING_CURRENCY,
                                ((fairValueRecord.FV_BUY * (decimal) securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE),
                                ((fairValueRecord.FV_SELL * (decimal)securityData.CLOSING_PRICE) / fairValueRecord.CURRENT_MEASURE_VALUE));
                        }

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
                        String securityID = securityData.SECURITY_ID.ToString();

                        PERIOD_FINANCIALS_SECURITY periodFinancialRecord = externalResearchEntity.PERIOD_FINANCIALS_SECURITY
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

        /// <summary>
        /// Downloads and returns location of all attached files for a specific presentation
        /// </summary>
        /// <param name="fileMasterInfo">List of FileMaster objects</param>
        /// <param name="presentationInfo">PresentationInfo object</param>
        /// <returns>List of temp locations of presentation attached files</returns>
        private List<String> GetICPacketSegmentFiles(List<FileMaster> fileMasterInfo, PresentationInfo presentationInfo)
        {
            List<String> result = new List<String>();
           /* FileMaster powerPointFile = fileMasterInfo.Where(record => record.Category == "Power Point Presentation").FirstOrDefault();
            if (powerPointFile != null)
            {
                String uploadLocation = ConvertPowerpointPresentationTpPdf(powerPointFile, presentationInfo);
                if (uploadLocation != null)
                {
                    result.Add(uploadLocation);
                }
            }
            */

            FileMaster presentationFile = fileMasterInfo.Where(record => record.Category == "Presentation").FirstOrDefault();
            if (presentationFile != null)
            {
                String uploadLocation = ConvertImagePdfFileToLocalPdf(presentationFile);
                if (uploadLocation != null)
                {
                    result.Add(uploadLocation);
                }
            }

            FileMaster finstatFile = fileMasterInfo.Where(record => record.Category == "FinStat Report").FirstOrDefault();
            if (finstatFile != null)
            {
                String convertedPdf = ConvertImagePdfFileToLocalPdf(finstatFile);
                if (convertedPdf != null)
                {
                    result.Add(convertedPdf);
                }
            }

            FileMaster investmentContextFile = fileMasterInfo.Where(record => record.Category == "Investment Context Report").FirstOrDefault();
            if (investmentContextFile != null)
            {
                String convertedPdf = ConvertImagePdfFileToLocalPdf(investmentContextFile);
                if (convertedPdf != null)
                {
                    result.Add(convertedPdf);
                }
            }

            FileMaster dcfFile = fileMasterInfo.Where(record => record.Category == "DCF Model").FirstOrDefault();
            if (dcfFile != null)
            {
                String convertedPdf = ConvertImagePdfFileToLocalPdf(dcfFile);
                if (convertedPdf != null)
                {
                    result.Add(convertedPdf);
                }
            }

            foreach (FileMaster file in fileMasterInfo.Where(record => record.Category == "Additional Attachment"))
            {
                String convertedPdf = ConvertImagePdfFileToLocalPdf(file);
                if (convertedPdf != null)
                {
                    result.Add(convertedPdf);
                }
            }

            return result;
        }

        /// <summary>
        /// downloads image(jpeg/jpg) and pdf files from sharepoint and converts them to local pdf files
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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

        /// <summary>
        /// downloads .pptx file from sharepoint and converts it to local pdf file
        /// </summary>
        /// <param name="powerpointStreamedData">PowerpointStreamData</param>
        /// <param name="presentationInfo">PresentationInfo</param>
        /// <returns></returns>
        private String ConvertPowerpointPresentationTpPdf(FileMaster powerpointStreamedData, PresentationInfo presentationInfo)
        {
            String result = null;

            try
            {
                if (presentationInfo == null || powerpointStreamedData == null)
                    return result;

                DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
                Byte[] fileData = documentOperations.RetrieveDocument(powerpointStreamedData.Location
                    .Substring(powerpointStreamedData.Location.LastIndexOf(@"/") + 1));
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

                //ICPresentation presentationData = PptRead.Fetch(localFile, securityInformation);

                ICPresentation presentationData = PptRead.FetchDataFromPpt(localFile, securityInformation);
                result = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_temp.pdf";
               // PptRead.Generate(result, presentationData);
                PptRead.GenerateFullVersionPdf(result, presentationData);
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Merges pdf files into a single pdf file
        /// </summary>
        /// <param name="pdfFileNames">location of local pdf files</param>
        /// <returns></returns>
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
        #endregion

        #region Meeting Configuration Schedule
        /// <summary>
        /// Retrieves meeting configuration schedule details
        /// </summary>
        /// <returns>MeetingConfigurationSchedule</returns>
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

        /// <summary>
        /// Retrieves available future ic meeting/presentation dates
        /// </summary>
        /// <returns>List of MeetingInfo objects</returns>
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

                    if (tempPresentationDeadline.DayOfWeek == presentationDeadlineTime.DayOfWeek &&
                            tempPresentationDeadline < DateTime.UtcNow)
                    {
                        continue;
                    }
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
        /// <param name="presentationId">presentation id</param>
        /// <param name="includeICAdminInfo">inclusion of IC Admin in returned voting users list</param>
        /// <returns>Lis of VoterInfo</returns>
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

        /// <summary>
        /// Updates ic decision entry for a specific presentation
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationOverViewData">ICPresentationOverviewData object</param>
        /// <param name="voterInfo">List of VoterInfo objects</param>
        /// <returns>True if successful</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean UpdateDecisionEntryDetails(String userName, ICPresentationOverviewData presentationOverViewData, List<VoterInfo> voterInfo)
        {
            try
            {
                /*bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }*/

                XDocument xmlDoc = GetEntityXml<ICPresentationOverviewData>(new List<ICPresentationOverviewData> { presentationOverViewData }
                    , strictlyInclusiveProperties: new List<string> { "PresentationID", "AdminNotes", "CommitteePFVMeasure", "CommitteeBuyRange",
                    "CommitteeSellRange", "CommitteeRecommendation", "CommitteePFVMeasureValue" });
                xmlDoc = GetEntityXml<VoterInfo>(parameters: voterInfo, xmlDoc: xmlDoc, strictlyInclusiveProperties: new List<string> { "VoterID",
                    "VoterPFVMeasure", "VoterBuyRange", "VoterSellRange", "VoteType" });
                String xmlScript = xmlDoc.ToString();
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetICPresentationDecisionEntryDetails(userName, xmlScript).FirstOrDefault();

                GreenField.DAL.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ISSUE_NAME == presentationOverViewData.SecurityName &&
                        record.TICKER == presentationOverViewData.SecurityTicker).FirstOrDefault();

                ExternalResearchEntities externalEntity = new ExternalResearchEntities();

                if (securityRecord != null && securityRecord.SECURITY_ID != null)
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
        #region Operation Contracts
        /// <summary>
        /// Retrieves information for meetings in which all presentations are having a specific status type
        /// </summary>
        /// <param name="presentationStatus">status type</param>
        /// <returns>list of MeetingInfo objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MeetingInfo> RetrieveMeetingInfoByPresentationStatus(String presentationStatus)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                List<MeetingInfo> result = new List<MeetingInfo>();
                List<ICPresentationOverviewData> data = entity.RetrieveICMeetingInfoByStatusType(presentationStatus).ToList();
                List<DateTime?> distinctMeetingDateTime = data.Select(record => record.MeetingDateTime).ToList().Distinct().ToList();

                foreach (DateTime? meetingDateTime in distinctMeetingDateTime)
                {
                    Boolean icVotingComplete = true;
                    foreach (ICPresentationOverviewData item in data.Where(record => record.MeetingDateTime == meetingDateTime))
                    {
                        if (item.CommitteeBuyRange == null || item.CommitteePFVMeasure == null || item.CommitteePFVMeasureValue == null
                            || item.CommitteeRecommendation == null || item.CommitteeSellRange == null)
                        {
                            icVotingComplete = false;
                            break;
                        }
                    }

                    if (icVotingComplete)
                    {
                        MeetingInfo eligibleMeeting = entity.RetrieveICMeetingInfoByMeetingDateTime(meetingDateTime).FirstOrDefault();
                        if (eligibleMeeting != null)
                        {
                            result.Add(eligibleMeeting);
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

        /// <summary>
        /// Retrieves meeting minutes details
        /// </summary>
        /// <param name="meetingID">Meeting Id</param>
        /// <returns>List of MeetingMinutesData objects</returns>
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

        /// <summary>
        /// Retrieves attached file details for a specific meeting
        /// </summary>
        /// <param name="meetingID">Meeting Id</param>
        /// <returns>List of FileMaster objects</returns>
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

        /// <summary>
        /// Updates meeting minute details for a specific meeting
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="meetingInfo">MeetingInfo object</param>
        /// <param name="meetingMinuteData">MeetingMinutesData objects</param>
        /// <returns>True if successful</returns>
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

        /// <summary>
        /// Updates attached file data for a specific meeting
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="meetingId">Meeting Id</param>
        /// <param name="fileMasterInfo">FileMaster object</param>
        /// <param name="deletionFlag">flag to delete original filemaster record</param>
        /// <returns>True if successful</returns>
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
                    GreenField.DAL.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
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

        /// <summary>
        /// Updates status of all presentations in a specific meeting to a specific status type
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="meetingId">Meeting id</param>
        /// <param name="status">status type</param>
        /// <returns>True if successful</returns>
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

        /// <summary>
        /// Retrives all application user information
        /// </summary>
        /// <returns></returns>
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
                {
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

/*        /// <summary>
        /// Generates meeting minutes report for a specific meeting
        /// </summary>
        /// <param name="meetingId">Meeting Id</param>
        /// <returns>Byte stream of the generated report</returns>
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
        } */

        /// <summary>
        /// Generates pre-meeting voting report for a specific presentation
        /// </summary>
        /// <param name="presentationId">presentation id</param>
        /// <returns>byte stream of the generated report</returns>
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

                GreenField.DAL.GF_SECURITY_BASEVIEW securityRecord = DimensionEntity.GF_SECURITY_BASEVIEW
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
        #endregion

        #region Private Methods
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

        /// <summary>
        /// Converts a class structure list object to xml document with 'Root' node, class name as childeren nodes with property attributes
        /// </summary>
        /// <typeparam name="T">generic class</typeparam>
        /// <param name="parameters">list of generic class</param>
        /// <param name="xmlDoc">preexisting document with root node to which list items will be added</param>
        /// <param name="strictlyInclusiveProperties">names of properties to be included only, rest will be ignored</param>
        /// <returns>XDocument</returns>
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

       /* /// <summary>
        /// Generate post meeting voting report for a specific presentation
        /// </summary>
        /// <param name="presentationFinalizeInfo">MeetingMinutesReportData object</param>
        /// <returns>byte stream of the generated report</returns>
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

                String buySellPresented = String.Format("{0} {1:n2}-{2:n2}", distinctPresentationFinalizeDetails.First().SecurityPFVMeasure
                    , distinctPresentationFinalizeDetails.First().SecurityBuyRange, distinctPresentationFinalizeDetails.First().SecuritySellRange);

                PdfPCell presentedBuySellRangeValueCell = new PdfPCell(new Phrase(buySellPresented, PDFFontStyle.STYLE_3));
                AddTextCell(icdecisionValueTable, presentedBuySellRangeValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP_BOTTOM);

                PdfPCell decisionValueCell = new PdfPCell(new Phrase(distinctPresentationFinalizeDetails.First().CommitteeRecommendation, PDFFontStyle.STYLE_3));
                AddTextCell(icdecisionValueTable, decisionValueCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.RIGHT_TOP_BOTTOM);

                String buySellCommittee = String.Format("{0} {1:n2}-{2:n2}", distinctPresentationFinalizeDetails.First().CommitteePFVMeasure
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

                List<String> distinctVoterPresentationFinalizeDetails = distinctPresentationFinalizeDetails
                    .Select(record => record.Name).ToList().Distinct().ToList();

                foreach (String voterName in distinctVoterPresentationFinalizeDetails)
                {
                    MeetingMinutesReportData voterDetails = distinctPresentationFinalizeDetails
                        .Where(record => record.Name == voterName).FirstOrDefault();

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
        } */

        /// <summary>
        /// Generates pre meeting report
        /// </summary>
        /// <param name="presentationDeadlineInfo">PresentationVotingDeadlineDetails</param>
        /// <param name="securityDesc">security description</param>
        /// <returns></returns>
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
            iTextSharp.text.Rectangle page = doc.PageSize;

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
            String presentationSecurityBuySellRange = String.Format("{0:n2} to {1:n2}", presentationDetails.SecurityBuyRange, presentationDetails.SecuritySellRange);
            PdfPCell securityBuySellRange = new PdfPCell(new Phrase(presentationSecurityBuySellRange, PDFFontStyle.STYLE_5));
            AddTextCell(topHeaderTableContent, securityBuySellRange, Element.ALIGN_LEFT, Element.ALIGN_BOTTOM, PDFBorderType.TOP);
            PdfPCell securityRecommendation = new PdfPCell(new Phrase(string.Empty, PDFFontStyle.STYLE_5)); // Per felicia no required to display the Recommendation
            securityRecommendation.RightIndent = 5;
            AddTextCell(topHeaderTableContent, securityRecommendation, Element.ALIGN_RIGHT, Element.ALIGN_BOTTOM, PDFBorderType.RIGHT_TOP);
            
            doc.Add(topHeaderTableContent);

            PdfPTable topHeaderTableDesc = new PdfPTable(1);
            topHeaderTableDesc.WidthPercentage = 100;
            topHeaderTableDesc.SetWidths(new float[] { 1 });

            PdfPCell securityDescription = new PdfPCell(new Phrase(string.Empty, PDFFontStyle.STYLE_3));
            securityDescription.PaddingLeft = 5;
            securityDescription.PaddingBottom = 5;
            AddTextCell(topHeaderTableDesc, securityDescription, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PDFBorderType.LEFT_RIGHT_BOTTOM);

            doc.Add(topHeaderTableDesc);

            
            PdfPTable PresentedTable = new PdfPTable(1);
            PresentedTable.WidthPercentage = 100;
            PresentedTable.SetWidths(new float[] { 1 });
            PdfPCell AnalystCell = new PdfPCell(new Phrase("Presented By: " + presentationDetails.Presenter, PDFFontStyle.STYLE_2));
            AddTextCell(PresentedTable, AnalystCell, Element.ALIGN_CENTER, Element.ALIGN_CENTER, PDFBorderType.NONE);
            doc.Add(PresentedTable);

            PdfPTable IndustryAnalystTable = new PdfPTable(1);
            IndustryAnalystTable.WidthPercentage = 100;
            IndustryAnalystTable.SetWidths(new float[] { 1 });
            PdfPCell IndustryAnalystCell = new PdfPCell(new Phrase("Industry Analyst: " + presentationDetails.ASHMOREEMM_INDUSTRY_ANALYST, PDFFontStyle.STYLE_2));
            AddTextCell(IndustryAnalystTable, IndustryAnalystCell, Element.ALIGN_CENTER, Element.ALIGN_CENTER, PDFBorderType.NONE);
            doc.Add(IndustryAnalystTable);


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

        /// <summary>
        /// adds text to cell in pdf generation
        /// </summary>
        /// <param name="table">PdfPTable</param>
        /// <param name="cell">PdfPCell</param>
        /// <param name="horizontalAlignment">HorizontalAlignment</param>
        /// <param name="verticalAlignment">VerticalAlignment</param>
        /// <param name="border">Border type</param>
        private static void AddTextCell(PdfPTable table, PdfPCell cell, int horizontalAlignment = Element.ALIGN_LEFT
            , int verticalAlignment = Element.ALIGN_MIDDLE, int border = 0)
        {
            cell.HorizontalAlignment = horizontalAlignment;
            cell.VerticalAlignment = verticalAlignment;
            cell.Border = border;
            cell.BorderWidth = 1;
            table.AddCell(cell);
        } 
        #endregion
        #endregion

        #region Presentation Vote
        /// <summary>
        /// Retrieves presentation comments
        /// </summary>
        /// <param name="presentationId">presentation id</param>
        /// <returns>List of CommentInfo objects</returns>
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

        /// <summary>
        /// Sets comment for a specific presentation
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationId">presentation id</param>
        /// <param name="comment">comment</param>
        /// <returns>List of updated comments for the presentation</returns>
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

        /// <summary>
        /// Updates pre meeting voting details
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="voterInfo">list of VoreInfo objects</param>
        /// <returns>True if successful</returns>
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
        /// <summary>
        /// Updated presentation date - moves to a pre-existing meeting or creates a new meeting slot
        /// </summary>
        /// <param name="userName">logged in user</param>
        /// <param name="presentationId">presentation id</param>
        /// <param name="meetingInfo">MeetingInfo object</param>
        /// <returns>True if successful</returns>
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
        /// <summary>
        /// creates new entry to the messageinfo table
        /// </summary>
        /// <param name="emailTo">emailTo</param>
        /// <param name="emailCc">emailCc</param>
        /// <param name="emailSubject">emailSubject</param>
        /// <param name="emailMessageBody">emailMessageBody</param>
        /// <param name="emailAttachment">emailAttachment</param>
        /// <param name="userName">logged in user</param>
        /// <returns>True if successful</returns>
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
        /// <summary>
        /// Retrieves summy report data between specified dates
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <returns>List of SummaryReportData objects</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<SummaryReportData> RetrieveSummaryReportData(DateTime startDate, DateTime endDate)
        {
            try
            {
                ICPresentationEntities iCPEntity = new ICPresentationEntities();
                DimensionEntities entity = DimensionEntity;
                ExternalResearchEntities externalResearchEntity = new ExternalResearchEntities();

                List<SummaryReportData> result = iCPEntity.RetrieveSummaryReportDetails(startDate, endDate).ToList();

                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GreenField.DAL.GF_PORTFOLIO_HOLDINGS lastBusinessRecord = entity.GF_PORTFOLIO_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                {
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                    {
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);
                    }
                }

                foreach (SummaryReportData item in result)
                {
                    GreenField.DAL.GF_SECURITY_BASEVIEW securityData = entity.GF_SECURITY_BASEVIEW
                            .Where(record => record.TICKER == item.SecurityTicker
                                && record.ISSUE_NAME == item.SecurityName)
                            .FirstOrDefault();

                    List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> securityHoldingData = entity.GF_PORTFOLIO_HOLDINGS.Where(
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

        #region Distribute IC Pack

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean DistributeICPacks()
        {


            DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
            ICPresentationEntities entity = new ICPresentationEntities();
            try
            {
                List<ICPackLocationData> icPackDataList = entity.RetrieveICPack().ToList();
                
                Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
                 

                MailMessage message = new MailMessage();

                message.From = new MailAddress(settings.Smtp.From);

                //message.To.Add("Akhtar.Nazirali@Ashmoregroup.com");
                message.To.Add(ConfigurationManager.AppSettings["ICDistributionTo"]);
                message.CC.Add(ConfigurationManager.AppSettings["ICDistributionCC"]);
                message.Subject = "Presentations ready for voting "+DateTime.Now;
                message.Body = "Please see attached the packets for the IC Meeting";

                //string smtpserver = "smtpexternal.ashmore.local";
                string smtpserver = settings.Smtp.Network.Host;
                Attachment att = null;

                foreach (ICPackLocationData icpack in icPackDataList)
                {

                    Byte[] fileData = documentOperations.RetrieveDocument(icpack.Location.Substring(icpack.Location.LastIndexOf(@"/") + 1));
                    att = new Attachment(new MemoryStream(fileData), icpack.Name);
                    message.Attachments.Add(att);

                }
                if (message.Attachments.Count() > 0)
                {
                    SmtpClient client = new SmtpClient(smtpserver);
                    client.Send(message);
                    return true;
                }
                else
                {
                    return false;
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


        #region Voting Closed

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean VotingClosed(string fromstatus, string tostatus)
        {


            DocumentWorkspaceOperations documentOperations = new DocumentWorkspaceOperations();
            ICPresentationEntities entity = new ICPresentationEntities();
            try
            {


                //Update status from Ready for voting to Voting closed
               int status =  entity.UpdatePresentationInfoStatus(fromstatus, tostatus);
                //Generate Pre meeting voting results pdf and merge pdf;
               List<ICPresentationOverviewData> icpresentationdataList  = entity.RetrieveICPresentationOverviewData("", "Voting").ToList();
               if(icpresentationdataList.Count()>0)
               {
               String outFile = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"PreMeetingVotingResult.pdf";

               Byte[] all;
               int pageOffset = 0;
               int f = 0;
               Document document = null;
               PdfCopy writer = null;
               foreach (ICPresentationOverviewData icData in icpresentationdataList)
               {
                   Byte[] fileData = GeneratePreMeetingVotingReport(icData.PresentationID);
                   PdfReader reader = new PdfReader(fileData);
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
               if (document != null)
               {
                   document.Close();
               }

               all = File.ReadAllBytes(outFile);

                //Send email to the distribution group

               Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
               MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");


               MailMessage message = new MailMessage();

               message.From = new MailAddress(settings.Smtp.From);

              
               message.To.Add(ConfigurationManager.AppSettings["ICDistributionTo"]);
               message.CC.Add(ConfigurationManager.AppSettings["ICDistributionCC"]);
               message.Subject = "Pre meeting voting results " + DateTime.Now;
               message.Body = "Please see attached pre meeting voting results";

               string smtpserver = settings.Smtp.Network.Host;
               Attachment att = null;

               att = new Attachment(new MemoryStream(all), "PreMeetingVotingResults.pdf");
               message.Attachments.Add(att);

              

                   SmtpClient client = new SmtpClient(smtpserver);
                   client.Send(message);
                   return true;
               }
               else
               {
                   return false;
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
        #region Publish Decision

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean PublishDecision(string fromstatus, string tostatus)
        {
              ICPresentationEntities entity = new ICPresentationEntities();
              List<VoterInfo> voterDataList;
              try
              {
                  //Update status from Ready for voting to Voting closed
                  List<ICPresentationOverviewData> icpresentationdataList = entity.RetrieveICPresentationOverviewData("", "Voting").ToList();
                  if (icpresentationdataList.Count() > 0)
                  {
                      string fileName = GetFileName();

                      SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);

                      // Add a WorkbookPart to the document.
                      WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                      workbookpart.Workbook = new Workbook();

                      // Add a WorksheetPart to the WorkbookPart.
                      WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                      worksheetPart.Worksheet = new Worksheet();


                      //Set a custom width for the first column
                      DocumentFormat.OpenXml.Spreadsheet.Columns columns = new DocumentFormat.OpenXml.Spreadsheet.Columns();
                      columns.AppendChild(new Column { Min = 1, Max = 1, CustomWidth = true, Width = 25 });
                      columns.AppendChild(new Column { Min = 2, Max = 2, CustomWidth = true, Width = 25 });
                      worksheetPart.Worksheet.AppendChild(columns);


                      SheetData sheetData = new SheetData();
                      worksheetPart.Worksheet.AppendChild(sheetData);

                      // add styles to sheet
                      WorkbookStylesPart wbsp = workbookpart.AddNewPart<WorkbookStylesPart>();
                      wbsp.Stylesheet = GenerateStyleSheet();// CreateStylesheet();
                      wbsp.Stylesheet.Save();

                      // Add Sheets to the Workbook.
                      Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                          AppendChild<Sheets>(new Sheets());

                      // Append a new worksheet and associate it with the workbook.
                      Sheet sheet = new Sheet()
                      {
                          Id = spreadsheetDocument.WorkbookPart.
                              GetIdOfPart(worksheetPart),
                          SheetId = 1,
                          Name = "Post Meeting Results"
                      };
                      sheets.Append(sheet);

                      UInt32 rowIndex = 1;
                      string cell1Name;
                      string cell2Name;
                      Worksheet worksheet = worksheetPart.Worksheet;
                      // SheetData sheetData = worksheet.GetFirstChild<SheetData>();



                      foreach (ICPresentationOverviewData icdata in icpresentationdataList)
                      {

                          rowIndex += 1;

                          Cell cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Company Name") };
                          cell.StyleIndex = 1;
                          //cell.CellValue = new CellValue("Company Name");


                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                          cell.StyleIndex = 1;

                          cell1Name = "A" + rowIndex;
                          cell2Name = "B" + rowIndex;
                          MergeTwoCells(worksheetPart, cell1Name, cell2Name);

                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(icdata.ISSUER_NAME) };
                          cell.StyleIndex = 2;

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                          cell.StyleIndex = 2;
                          cell1Name = "A" + rowIndex;
                          cell2Name = "B" + rowIndex;
                          MergeTwoCells(worksheetPart, cell1Name, cell2Name);

                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                          cell1Name = "A" + rowIndex;
                          cell2Name = "B" + rowIndex;
                          MergeTwoCells(worksheetPart, cell1Name, cell2Name);


                          rowIndex += 1;

                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Country") };
                          cell.StyleIndex = 1;
                          //cell.CellValue = new CellValue("Company Name");


                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Industry ") };
                          cell.StyleIndex = 1;

                          rowIndex += 1;

                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(icdata.SecurityCountry) };
                          cell.StyleIndex = 6;

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(icdata.SecurityIndustry) };
                          cell.StyleIndex = 6;


                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                          cell1Name = "A" + rowIndex;
                          cell2Name = "B" + rowIndex;
                          MergeTwoCells(worksheetPart, cell1Name, cell2Name);


                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Proposed Range") };
                          cell.StyleIndex = 1;

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Approved Range") };
                          cell.StyleIndex = 1;

                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(icdata.SecurityPFVMeasure + "  " + icdata.SecurityBuyRange + " - " + icdata.SecuritySellRange) };
                          cell.StyleIndex = 6;

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(icdata.CommitteePFVMeasure + "  " + icdata.CommitteeBuyRange + " - " + icdata.CommitteeSellRange) };
                          cell.StyleIndex = 6;

                          rowIndex += 1;
                          Row row = new Row() { RowIndex = rowIndex };
                          sheetData.Append(row);

                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Voting Members") };
                          cell.StyleIndex = 1;
                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Range") };
                          cell.StyleIndex = 1;


                          voterDataList = entity.VoterInfoes.Where(record => record.PresentationID == icdata.PresentationID && record.PostMeetingFlag == true).ToList();
                          foreach (VoterInfo voterData in voterDataList)
                          {

                              rowIndex += 1;
                              cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                              cell.DataType = CellValues.InlineString;
                              cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(voterData.Name) };
                              cell.StyleIndex = 6;
                              cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                              cell.DataType = CellValues.InlineString;
                              if (voterData.VoterPFVMeasure != null)
                              {

                                  cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(voterData.VoterPFVMeasure + "  " + voterData.VoterBuyRange + " - " + voterData.VoterSellRange) };
                              }
                              else
                              {

                                  cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                              }
                              cell.StyleIndex = 6;
                          }

                          rowIndex += 1;
                          row = new Row() { RowIndex = rowIndex };
                          sheetData.Append(row);



                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text("Notes: ") };
                          cell.StyleIndex = 5;

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                          cell.StyleIndex = 5;

                          rowIndex += 1;
                          cell = InsertCellInWorksheet("A", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                          cell.StyleIndex = 5;

                          cell = InsertCellInWorksheet("B", rowIndex, worksheetPart);
                          cell.DataType = CellValues.InlineString;
                          cell.InlineString = new InlineString() { Text = new DocumentFormat.OpenXml.Spreadsheet.Text(" ") };
                          cell.StyleIndex = 5;

                          cell1Name = "A" + (rowIndex - 1);
                          cell2Name = "B" + rowIndex;
                          MergeTwoCells(worksheetPart, cell1Name, cell2Name);

                          rowIndex += 1;
                          row = new Row() { RowIndex = rowIndex };
                          sheetData.Append(row);

                      }
                      workbookpart.Workbook.Save();

                      //workbookpart.Workbook.Save();

                      // Close the document.
                      spreadsheetDocument.Close();


                      Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                      MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");


                      MailMessage message = new MailMessage();

                      message.From = new MailAddress(settings.Smtp.From);


                      message.To.Add(ConfigurationManager.AppSettings["ICDistributionTo"]);
                      message.CC.Add(ConfigurationManager.AppSettings["ICDistributionCC"]);
                      message.Subject = "Post meeting results " + DateTime.Now;
                      message.Body = "Please see attached post meeting voting results";

                      string smtpserver = settings.Smtp.Network.Host;
                      Attachment att = null;

                      att = new Attachment(new MemoryStream(GetBytsForFile(fileName)), "PostMeetingVotingResults.xlsx");
                      message.Attachments.Add(att);



                      SmtpClient client = new SmtpClient(smtpserver);
                      client.Send(message);


                      int status = entity.UpdatePresentationInfoStatus(fromstatus, tostatus);
                      return true;
                  }
                  else
                  {
                      return false;
                  }
              }
              catch (Exception ex)
              {
                  ExceptionTrace.LogException(ex);
                  string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                  throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
              }
        }




       



        private static void MergeTwoCells(WorksheetPart worksheetPart, string cell1Name, string cell2Name)
        {
            // Open the document for editing.
            Worksheet worksheet = worksheetPart.Worksheet;

                MergeCells mergeCells;
                if (worksheet.Elements<MergeCells>().Count() > 0)
                {
                    mergeCells = worksheet.Elements<MergeCells>().First();
                }
                else
                {
                    mergeCells = new MergeCells();

                    // Insert a MergeCells object into the specified position.
                    if (worksheet.Elements<CustomSheetView>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<CustomSheetView>().First());
                    }
                    else if (worksheet.Elements<DataConsolidate>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<DataConsolidate>().First());
                    }
                    else if (worksheet.Elements<SortState>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<SortState>().First());
                    }
                    else if (worksheet.Elements<AutoFilter>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<AutoFilter>().First());
                    }
                    else if (worksheet.Elements<Scenarios>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<Scenarios>().First());
                    }
                    else if (worksheet.Elements<ProtectedRanges>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<ProtectedRanges>().First());
                    }
                    else if (worksheet.Elements<SheetProtection>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetProtection>().First());
                    }
                    else if (worksheet.Elements<SheetCalculationProperties>().Count() > 0)
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetCalculationProperties>().First());
                    }
                    else
                    {
                        worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                    }
                }

                // Create the merged cell and append it to the MergeCells collection.
                MergeCell mergeCell = new MergeCell() { Reference = new StringValue(cell1Name + ":" + cell2Name) };
                mergeCells.Append(mergeCell);

                worksheet.Save();
            
        }



        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }


        private static Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 0 - The default font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 1 - The bold font.
                        new Bold(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 2 - The Italic font.
                        new Italic(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new DocumentFormat.OpenXml.Spreadsheet.Font(                                                               // Index 2 - The Times Roman font. with 16 size
                        new FontSize() { Val = 16 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Times New Roman" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(                                                           // Index 1 - The default fill of gray 125 (required)
                        new PatternFill(
                                new BackgroundColor() { Rgb = new HexBinaryValue { Value = "D9D9D9" } }
                                ) { PatternType = PatternValues.Gray125 }),
                    new Fill(                                                           // Index 2 - The yellow fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "D9D9D9" } }
                        ) { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(                                                         // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true},                          // Index 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                    new CellFormat(
                            new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                        ) { FontId = 1, FillId = 2, BorderId = 1, ApplyFont = true, ApplyAlignment = true },       // Index 1 - Bold  Center Aligned with border
                    new CellFormat(
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                        ) { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true },       // Index 2 - Regular Center Aligned with border
                    new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 3 
                    new CellFormat() { FontId = 0, FillId = 2, BorderId = 0, ApplyFill = true },       // Index 4 
                    new CellFormat(                                                                   // Index 5 Bold Top Left Aligned with Border
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Top } 
                    ) { FontId = 1, FillId = 0, BorderId = 1, ApplyAlignment = true },
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }      // Index 6 - Regular with Border
                )
            ); // return
        }



        
        private static Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth)
        {
            Column column;
            column = new Column();
            column.Min = StartColumnIndex;
            column.Max = EndColumnIndex;
            column.Width = ColumnWidth;
            column.CustomWidth = true;
            return column;
        }


                  /// <summary>
        /// Generate Filename
        /// </summary>
        /// <returns>Name of File</returns>
        private static string GetFileName()
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid() + "_PostMeetingResults.xlsx";
            return fileName;
        }

        /// <returns>Byte Stream of the File</returns>
        private static byte[] GetBytsForFile(string filePath)
        {
            try
            {
                FileStream fileStream;
                byte[] fileByte;
                using (fileStream = File.OpenRead(filePath))
                {
                    fileByte = new byte[fileStream.Length];
                    fileStream.Read(fileByte, 0, Convert.ToInt32(fileStream.Length));
                }
                return fileByte;
            }
            catch (Exception ex)
            {
                GreenField.Web.Helpers.ExceptionTrace.LogException(ex);
                return null;
            }
        }


      
        #endregion




    }
}