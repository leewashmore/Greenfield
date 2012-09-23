using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SqlClient;

using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using CommandLine;
using ICPSystemAlert.DocumentCopyReference;
using ICPSystemAlert.DocumentListsReference;
using System.Xml;


namespace ICPSystemAlert
{
    class Program
    {
        #region Fields
        private static ICPEntities _entity;
        private static UserEntities _userEntity;

        private static DimensionServiceReference.Entities _dimensionEntity;

        private static Copy _copyService;
        private static Lists _listsService;
        private static String _documentLibrary;
        private static String _documentServerUrl;
        private static String _documentServerUserName;
        private static String _documentServerPassword;
        private static String _documentServerDomain;
        private static String _documentCopyServiceUrl;
        private static String _documentListsServiceUrl;

        private static Int32 _scheduledRunMinutes;

        private static String _networkWebmasterEmail;
        private static String _networkCredentialPassword;
        private static String _networkCredentialUsername;
        private static String _networkCredentialDomain;
        private static Int32 _networkConnectionPort;
        private static String _networkConnectionHost;
        private static Boolean _sendFilesAsAttachment;

        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Main Method
        static void Main(string[] args)
        {
            _log.Debug("Application initiated");

            _documentLibrary = ConfigurationManager.AppSettings.Get("DocumentLibrary");
            _documentServerUrl = ConfigurationManager.AppSettings.Get("DocumentServerUrl");
            _documentServerUserName = ConfigurationManager.AppSettings.Get("DocumentServerUserName");
            _documentServerPassword = ConfigurationManager.AppSettings.Get("DocumentServerPassword");
            _documentServerDomain = ConfigurationManager.AppSettings.Get("DocumentServerDomain");
            _documentCopyServiceUrl = ConfigurationManager.AppSettings.Get("DocumentCopyServiceUrl");
            _documentListsServiceUrl = ConfigurationManager.AppSettings.Get("DocumentListsServiceUrl");

            if (_copyService == null)
            {
                _copyService = new Copy();
                _copyService.Credentials = new NetworkCredential(_documentServerUserName, _documentServerPassword, _documentServerDomain);
                _copyService.Url = _documentCopyServiceUrl;
            }

            if (_listsService == null)
            {
                _listsService = new Lists();
                _listsService.Credentials = new NetworkCredential(_documentServerUserName, _documentServerPassword, _documentServerDomain);
                _listsService.Url = _documentListsServiceUrl;
            }

            if (null == _dimensionEntity)
            {
                _dimensionEntity = new DimensionServiceReference.Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
            }

            _networkWebmasterEmail = ConfigurationManager.AppSettings.Get("NetworkWebmasterEmail");
            _networkCredentialPassword = ConfigurationManager.AppSettings.Get("NetworkCredentialPassword");
            _networkCredentialUsername = ConfigurationManager.AppSettings.Get("NetworkCredentialUsername");
            _networkCredentialDomain = Convert.ToString(ConfigurationManager.AppSettings.Get("NetworkCredentialDomain"));
            _networkConnectionPort = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NetworkConnectionPort"));
            _networkConnectionHost = Convert.ToString(ConfigurationManager.AppSettings.Get("NetworkConnectionHost"));
            _sendFilesAsAttachment = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("SendFileAsAttachment"));
            _entity = new ICPEntities();
            _userEntity = new UserEntities();

            var options = new Options();
            if (CommandLineParser.Default.ParseArguments(args, options))
            {
                _scheduledRunMinutes = options.ScheduledRunMinutes;
                _log.Info("Scheduled run minutes - " + _scheduledRunMinutes + " min(s)");

                switch (options.ForcedRunParameter)
                {
                    case 1:
                        _log.Info("forced run 1");
                        PreVotingReportImplementation();
                        MessagePush();
                        break;
                    case 2:
                        _log.Info("forced run 2");
                        PreMeetingReportImplementation();
                        MessagePush();
                        break;
                    case 3:
                        _log.Info("forced run 3");
                        PostMeetingReportImplementation();
                        MessagePush();
                        break;
                    case 4:
                        _log.Info("forced run 4");
                        MessagePush();
                        break;
                    default:
                        _log.Info("default run");
                        PreVotingReportImplementation();
                        PreMeetingReportImplementation();
                        PostMeetingReportImplementation();
                        MessagePush();
                        break;
                }

            }
        }
        #endregion

        #region Implementation Methods
        /// <summary>
        /// Retrieves joined results on presentationInfo, meetingInfo, FileMaster for presentations that have exceeded
        /// their presentation deadline. Updates meeting presentations to Ready for voting status and registers mails on
        /// message info
        /// </summary>
        private static void PreVotingReportImplementation()
        {
            try
            {
                List<PresentationDeadlineDetails> presentationDeadlineInfo = _entity.GetPresentationDeadlineDetails(_scheduledRunMinutes).ToList();

                List<Int64?> distinctMeetingIds = presentationDeadlineInfo.Select(record => record.MeetingID).ToList().Distinct().ToList();
                foreach (Int64 meetingId in distinctMeetingIds)
                {
                    String emailTo = String.Empty;
                    String emailSubject = String.Empty;
                    String emailMessageBody = String.Empty;
                    String emailAttachments = String.Empty;

                    #region Email To population
                    List<String> voterUserNames = presentationDeadlineInfo.Where(record => record.MeetingID == meetingId)
                        .Select(record => record.Name).ToList().Distinct().ToList();
                    voterUserNames.AddRange(_userEntity.GetUsersInRoles("GreenField", "IC_ADMIN"));
                    List<String> voterEmails = new List<String>();
                    foreach (String item in voterUserNames)
                    {
                        Membership membership = _userEntity.GetUserByName("GreenField", item, null, false).FirstOrDefault();
                        if (membership != null)
                        {
                            voterEmails.Add(membership.Email);
                        }
                    }
                    emailTo = String.Join("|", voterEmails.ToArray());
                    #endregion

                    #region Email Subject population
                    emailSubject = "Investment Committee Ready for Voting Notification - "
                        + Convert.ToDateTime(presentationDeadlineInfo.Where(record => record.MeetingID == meetingId)
                        .FirstOrDefault().MeetingDateTime).ToString("MMMM dd, yyyy");
                    #endregion

                    #region Email Message body population
                    emailMessageBody = "The Investment Committee Meeting dated " + Convert.ToDateTime(presentationDeadlineInfo
                        .Where(record => record.MeetingID == meetingId).FirstOrDefault().MeetingDateTime).ToString("MMMM dd, yyyy")
                        + " UTC has exceeded its presentation submission deadline. Please find the attached presentation packets for companies to be presented in the meeting."
                        + " Voting members, please enter AIMS to submit your comments and pre-meeting votes.";
                    #endregion

                    #region Email attachment population
                    List<Int64?> distinctMeetingPresentationIds = presentationDeadlineInfo.Where(record => record.MeetingID == meetingId)
                                .Select(record => record.PresentationID).ToList().Distinct().ToList();

                    foreach (Int64 presentationId in distinctMeetingPresentationIds)
                    {
                        List<PresentationDeadlineDetails> distinctMeetingPresentationRecord = presentationDeadlineInfo
                            .Where(record => record.MeetingID == meetingId
                                && record.PresentationID == presentationId).ToList();

                        if (distinctMeetingPresentationRecord == null || distinctMeetingPresentationRecord.Count == 0)
                            continue;

                        //All distinct files are used to create IC Packet which is uploaded on to share point server. Locations are then
                        //taken as pipe joined emailattachment.
                    }
                    emailAttachments = emailAttachments != null ? emailAttachments.Substring(0, emailAttachments.Length - 1) : null;
                    #endregion

                    _entity.SetMessageInfo(emailTo, null, emailSubject, emailMessageBody, emailAttachments, "System");
                    _entity.SetICPMeetingPresentationStatus("System", meetingId, "Ready for Voting");
                }
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

        }

        /// <summary>
        /// Retrieves joined results on presentationInfo, meetingInfo, FileMaster for presentations that have exceeded
        /// their presentation voting deadline. Updates meeting presentation to Closed for voting status and registers mails on
        /// message info
        /// </summary>
        private static void PreMeetingReportImplementation()
        {
            try
            {
                String emailTo = null;
                String emailSubject = null;
                String emailMessageBody = null;
                String emailAttachments = null;

                List<PresentationVotingDeadlineDetails> presentationVotingDeadlineInfo = _entity.GetPresentationVotingDeadlineDetails(_scheduledRunMinutes).ToList();
                List<Int64?> distinctMeetingIds = presentationVotingDeadlineInfo.Select(record => record.MeetingID).ToList().Distinct().ToList();
                foreach (Int64 meetingId in distinctMeetingIds)
                {
                    List<Int64?> distinctMeetingPresentationIds = presentationVotingDeadlineInfo.Where(record => record.MeetingID == meetingId)
                        .Select(record => record.PresentationID).ToList().Distinct().ToList();

                    #region Email To population
                    List<String> voterUserNames = presentationVotingDeadlineInfo.Where(record => record.MeetingID == meetingId)
                        .Select(record => record.Name).ToList().Distinct().ToList();
                    voterUserNames.AddRange(_userEntity.GetUsersInRoles("GreenField", "IC_ADMIN"));
                    List<String> voterEmails = new List<String>();
                    foreach (String item in voterUserNames)
                    {
                        Membership membership = _userEntity.GetUserByName("GreenField", item, null, false).FirstOrDefault();
                        if (membership != null)
                        {
                            voterEmails.Add(membership.Email);
                        }
                    }
                    emailTo = String.Join("|", voterEmails.ToArray());
                    #endregion

                    #region Email Subject population
                    emailSubject = "Investment Committee Closed for Voting Notification - "
                        + Convert.ToDateTime(presentationVotingDeadlineInfo.Where(record => record.MeetingID == meetingId)
                        .FirstOrDefault().MeetingDateTime).ToString("MMMM dd, yyyy");
                    #endregion

                    #region Email Message body population
                    emailMessageBody = "The Investment Committee Meeting dated " + Convert.ToDateTime(presentationVotingDeadlineInfo.Where(record => record.MeetingID == meetingId).FirstOrDefault().MeetingDateTime).ToString("MMMM dd, yyyy")
                        + " UTC has exceeded its voting deadline and all applicable presentations have been closed for voting. Please find the attached pre-meeting voting reports for companies to be presented in the meeting";
                    #endregion

                    #region Email attachments population
                    foreach (Int64 presentationId in distinctMeetingPresentationIds)
                    {
                        List<PresentationVotingDeadlineDetails> distinctMeetingPresentationRecord = presentationVotingDeadlineInfo
                            .Where(record => record.MeetingID == meetingId
                                && record.PresentationID == presentationId).ToList();

                        if (distinctMeetingPresentationRecord == null || distinctMeetingPresentationRecord.Count == 0)
                            continue;

                        Int64? fileID = distinctMeetingPresentationRecord.First().FileID;

                        List<PresentationVotingDeadlineDetails> distinctMeetingPresentationFileInfo = presentationVotingDeadlineInfo
                            .Where(record => record.MeetingID == meetingId && record.PresentationID == presentationId && record.FileID == fileID).ToList();

                        String securityDescription = String.Empty;
                        String securityName = distinctMeetingPresentationRecord.First().SecurityName;

                        DimensionServiceReference.GF_SECURITY_BASEVIEW securityDescriptionRecord
                            = _dimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.ISSUE_NAME == securityName).FirstOrDefault();

                        if (securityDescriptionRecord != null)
                        {
                            securityDescription = securityDescriptionRecord.ASHEMM_ONE_LINER_DESCRIPTION;
                        }

                        String preMeetingReportOutFile = GeneratePreMeetingReport(distinctMeetingPresentationFileInfo, securityDescription);

                        //Skips step of uploading to sharepoint server, deleting temp file
                        if (!String.IsNullOrEmpty(preMeetingReportOutFile))
                        {
                            String fileName = preMeetingReportOutFile.Substring(preMeetingReportOutFile.LastIndexOf(@"\") + 1);
                            String documentUploadLocation = UploadDocument(fileName, File.ReadAllBytes(preMeetingReportOutFile), String.Empty);
                            File.Delete(preMeetingReportOutFile);
                            Int32? updateFileMasterResult = _entity.SetICPresentationAttachedFileInfo("System", presentationId, fileName, distinctMeetingPresentationFileInfo.First().SecurityName
                                , distinctMeetingPresentationFileInfo.First().SecurityTicker, preMeetingReportOutFile
                                , distinctMeetingPresentationFileInfo.First().Presenter + "|" + Convert.ToDateTime(distinctMeetingPresentationFileInfo.First().MeetingDateTime).ToString("MM-dd-yyyy")
                                , "Investment Committee Pre-Meeting Voting Report", "IC Presentations", 0, false).FirstOrDefault();
                            if (updateFileMasterResult == 0)
                            {
                                emailAttachments = emailAttachments + documentUploadLocation + "|";
                            }
                        }


                    }
                    emailAttachments = emailAttachments != null ? emailAttachments.Substring(0, emailAttachments.Length - 1) : null;
                    #endregion

                    _entity.SetMessageInfo(emailTo, null, emailSubject, emailMessageBody, emailAttachments, "System");
                    _entity.SetICPMeetingPresentationStatus("System", meetingId, "Closed for Voting");

                }
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }

        /// <summary>
        /// Retrieves joined results on presentationInfo, meetingInfo, FileMaster for presentations that are in Final status
        /// and were updated in this cycle. Registers mails on message info with final meeting minute reports.
        /// </summary>
        private static void PostMeetingReportImplementation()
        {
            try
            {
                List<PresentationFinalizeDetails> presentationFinalizeInfo = _entity.GetPresentationFinalizeDetails(_scheduledRunMinutes).ToList();
                List<Int64?> distinctMeetingIds = presentationFinalizeInfo.Select(record => record.MeetingID).ToList().Distinct().ToList();
                foreach (Int64 meetingId in distinctMeetingIds)
                {
                    String emailTo = null;
                    String emailSubject = null;
                    String emailMessageBody = null;
                    String emailAttachments = null;

                    #region Email To population
                    List<String> voterEmails = new List<String>();
                    List<String> voterNames = presentationFinalizeInfo.Where(record => record.MeetingID == meetingId)
                        .Select(record => record.Name).ToList().Distinct().ToList();
                    voterNames.AddRange(_userEntity.GetUsersInRoles("GreenField", "IC_ADMIN"));
                    foreach (String item in voterNames)
                    {
                        Membership membership = _userEntity.GetUserByName("GreenField", item, null, false).FirstOrDefault();
                        if (membership != null)
                        {
                            voterEmails.Add(membership.Email);
                        }
                    }
                    emailTo = String.Join("|", voterEmails.ToArray());
                    #endregion

                    #region Email Subject population
                    emailSubject = "Investment Committee Meeting Minutes - "
                        + Convert.ToDateTime(presentationFinalizeInfo.Where(record => record.MeetingID == meetingId)
                        .FirstOrDefault().MeetingDateTime).ToString("MMMM dd, yyyy");
                    #endregion

                    #region Email Message body population
                    emailMessageBody = "Please find the attached Meeting Minutes for the Investment Committee Meeting dated " + Convert.ToDateTime(presentationFinalizeInfo
                        .Where(record => record.MeetingID == meetingId).FirstOrDefault().MeetingDateTime).ToString("MMMM dd, yyyy");
                    #endregion

                    #region Email attachment population
                    String postMeetingReportOutFile = GeneratePostMeetingReport(presentationFinalizeInfo);

                    if (!String.IsNullOrEmpty(postMeetingReportOutFile))
                    {
                        String fileName = postMeetingReportOutFile.Substring(postMeetingReportOutFile.LastIndexOf(@"\") + 1);
                        String documentUploadLocation = UploadDocument(fileName, File.ReadAllBytes(postMeetingReportOutFile), String.Empty);
                        File.Delete(postMeetingReportOutFile);
                        String securityNames = String.Join("|", presentationFinalizeInfo.Select(record => record.SecurityName).ToList().Distinct().ToArray());
                        String securityTickers = String.Join("|", presentationFinalizeInfo.Select(record => record.SecurityTicker).ToList().Distinct().ToArray());

                        Int32? updateFileMasterResult = _entity.SetICPMeetingAttachedFileInfo("System", meetingId, fileName
                            , securityNames
                            , securityTickers
                            , documentUploadLocation
                            , presentationFinalizeInfo.First().Presenter + "|" + Convert.ToDateTime(presentationFinalizeInfo.First().MeetingDateTime).ToString("MM-dd-yyyy")
                            , "Investment Committee Meeting Minutes Report", "IC Presentations", 0, false).FirstOrDefault();
                        if (updateFileMasterResult == 0)
                        {
                            emailAttachments = emailAttachments + documentUploadLocation;
                        }
                    }
                    //Pending implementation of uploading to sharepoint and posting that location while deleting temp file location being posted right now.

                    #endregion

                    _entity.SetMessageInfo(emailTo, null, emailSubject, emailMessageBody, emailAttachments, "System");

                }
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

        }

        /// <summary>
        /// Mails all items on MessageInfo that are yet to be sent.
        /// </summary>
        private static void MessagePush()
        {
            try
            {


                ICPEntities entity = new ICPEntities();
                List<MessageInfo> messageInfos = entity.MessageInfoes.Where(record => record.EmailSent == false).ToList();

                foreach (MessageInfo messageInfo in messageInfos)
                {
                    try
                    {
                        List<String> tempLocations = new List<String>();

                        MailMessage mm = new MailMessage();
                        if (_networkWebmasterEmail != "")
                            mm.From = new MailAddress(_networkWebmasterEmail);

                        if (messageInfo.EmailTo == null)
                            continue;

                        String[] emailTo = messageInfo.EmailTo.Split('|');
                        foreach (String email in emailTo)
                        {
                            mm.To.Add(new MailAddress(email));
                        }

                        if (messageInfo.EmailCc != null)
                        {
                            String[] emailCc = messageInfo.EmailCc.Split('|');
                            foreach (String email in emailCc)
                            {
                                mm.CC.Add(new MailAddress(email));
                            }
                        }

                        if (messageInfo.EmailAttachment != null)
                        {
                            String[] emailAttachment = messageInfo.EmailAttachment.Split('|');
                            foreach (String attachment in emailAttachment)
                            {
                                if (_sendFilesAsAttachment)
                                {
                                    Byte[] downloadFile = RetrieveDocument(attachment);
                                    String tempLocation = System.IO.Path.GetTempPath() + @"\" + Guid.NewGuid() + @"_temp" + attachment.Substring(attachment.LastIndexOf(@"\") + 1);
                                    tempLocations.Add(tempLocation);
                                    File.WriteAllBytes(tempLocation, downloadFile);
                                    mm.Attachments.Add(new Attachment(attachment));
                                }
                                else
                                {
                                    messageInfo.EmailMessageBody = messageInfo.EmailMessageBody + "\n\n" + attachment;
                                }
                            }
                        }


                        mm.Subject = messageInfo.EmailSubject;
                        mm.Body = messageInfo.EmailMessageBody;
                        mm.IsBodyHtml = false;

                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.Host = _networkConnectionHost;
                        smtpClient.Port = _networkConnectionPort;
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.EnableSsl = true;

                        NetworkCredential NetworkCred = new NetworkCredential();
                        if (_networkCredentialUsername != null)
                            NetworkCred.UserName = _networkCredentialUsername;
                        NetworkCred.Domain = _networkCredentialDomain;
                        if (_networkCredentialPassword != "")
                            NetworkCred.Password = _networkCredentialPassword;
                        smtpClient.Credentials = NetworkCred;

                        ServicePointManager.ServerCertificateValidationCallback = delegate(object s
                            , X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                        smtpClient.Send(mm);

                        Int32? updateResult = entity.UpdateMessageInfo(messageInfo.EmailId, true, "System").FirstOrDefault();
                        foreach (String tempLocation in tempLocations)
                        {
                            if (File.Exists(tempLocation))
                            {
                                File.Delete(tempLocation);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
                    }


                }
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion

        #region Document Library Methods
        public static String UploadDocument(String fileName, Byte[] fileByteStream, String deleteFileUrl)
        {
            String resultUrl = String.Empty;
            try
            {
                if (deleteFileUrl != String.Empty)
                {
                    DeleteDocument(deleteFileUrl);
                }

                String[] destinationUrl = { _documentServerUrl + "/" + "[" + DateTime.UtcNow.ToString("ddMMyyyyhhmmssffff") + "]" + fileName };

                CopyResult[] cResultArray = { new CopyResult() };
                FieldInformation[] ffieldInfoArray = { new FieldInformation() };

                UInt32 copyResult = _copyService.CopyIntoItems(destinationUrl[0], destinationUrl, ffieldInfoArray, fileByteStream, out cResultArray);

                if (cResultArray[0].ErrorCode == CopyErrorCode.Success)
                    resultUrl = cResultArray[0].DestinationUrl;
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

            return resultUrl;
        }

        public static bool DeleteDocument(String fileName)
        {
            bool fileDeleted = false;
            try
            {
                string strBatch = "<Method ID='1' Cmd='Delete'>" +
                    "<Field Name='ID'>3</Field>" +
                    "<Field Name='FileRef'>" +
                    fileName +
                    "</Field>" +
                    "</Method>";

                XmlDocument xmlDoc = new XmlDocument();
                System.Xml.XmlElement elBatch = xmlDoc.CreateElement("Batch");
                elBatch.SetAttribute("OnError", "Continue");
                elBatch.SetAttribute("PreCalc", "TRUE");
                elBatch.SetAttribute("ListVersion", "0");
                elBatch.SetAttribute("ViewName", String.Empty);
                elBatch.InnerXml = strBatch;

                XmlNode ndReturn = _listsService.UpdateListItems(_documentLibrary, elBatch);

                if (ndReturn.InnerText.ToLower() == "0x00000000".ToLower())
                {
                    fileDeleted = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

            return fileDeleted;
        }

        public static Byte[] RetrieveDocument(String fileName)
        {
            Byte[] result = null;
            try
            {
                String sourceUrl = _documentServerUrl + "/" + fileName;
                FieldInformation[] ffieldInfoArray = { new FieldInformation() };
                UInt32 retrieveResult = _copyService.GetItem(sourceUrl, out ffieldInfoArray, out result);

            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }

            return result;
        }
        #endregion

        #region Helper Method
        private static String GeneratePreMeetingReport(List<PresentationVotingDeadlineDetails> presentationDeadlineInfo, String securityDesc)
        {
            try
            {
                if (presentationDeadlineInfo == null)
                    return null;

                PresentationVotingDeadlineDetails presentationDetails = presentationDeadlineInfo.FirstOrDefault();
                if (presentationDetails == null)
                    return null;

                String securityTicker = presentationDetails.SecurityTicker;
                String meetingDate = Convert.ToDateTime(presentationDetails.MeetingDateTime).ToString("MM-dd-yyyy");
                Document doc = new Document(PageSize.A4, 10F, 10F, 10F, 10F);

                String reportOutputFile = System.IO.Path.GetTempPath() + @"\" + meetingDate + "_" + securityTicker + @"_ICPPreMeetingReport.pdf";
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
                return reportOutputFile;
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }

        private static String GeneratePostMeetingReport(List<PresentationFinalizeDetails> presentationFinalizeInfo)
        {
            try
            {
                if (presentationFinalizeInfo == null)
                    return null;

                PresentationFinalizeDetails presentationFinalizeDetails = presentationFinalizeInfo.FirstOrDefault();
                if (presentationFinalizeDetails == null)
                    return null;

                Document doc = new Document(PageSize.A4, 10F, 10F, 10F, 10F);

                DateTime meetingDate = Convert.ToDateTime(presentationFinalizeDetails.MeetingDateTime);
                String reportOutputFile = System.IO.Path.GetTempPath() + @"\" + meetingDate.ToString("MM-dd-yyyy") + @"_ICPMeetingMinutes.pdf";

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
                    List<PresentationFinalizeDetails> distinctPresentationFinalizeDetails = presentationFinalizeInfo
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

                    foreach (PresentationFinalizeDetails voterDetails in distinctPresentationFinalizeDetails)
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
                return reportOutputFile;
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }

        private static void AddTextCell(PdfPTable table, PdfPCell cell, int HorizontalAlignment = Element.ALIGN_LEFT, int VerticalAlignment = Element.ALIGN_MIDDLE, int Border = 0)
        {
            try
            {
                cell.HorizontalAlignment = HorizontalAlignment; //0=Left, 1=Centre, 2=Right
                cell.VerticalAlignment = VerticalAlignment;
                cell.Border = Border;
                cell.BorderWidth = 1;
                table.AddCell(cell);
            }
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }
        #endregion
    }

    public class Options : CommandLineOptionsBase
    {
        [Option("r", "RunMinutes", Required = false, DefaultValue = 5, HelpText = "minutes at which run is scheduled")]
        public int ScheduledRunMinutes { get; set; }

        [Option("f", "ForcedRun", Required = false, DefaultValue = 0, HelpText = "force run of processes\n0 - All\n1 - Pre Voting Report Implementation" +
            "\n2 - Pre Meeting Report Implementation\n3 - Post Meeting Report Implementation\n4 - MessagePush")]
        public int ForcedRunParameter { get; set; }
    }
}
