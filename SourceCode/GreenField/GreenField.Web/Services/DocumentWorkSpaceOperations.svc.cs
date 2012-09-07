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
using System.Security.Principal;
using System.Configuration;
using System.IO;
using System.Net;
using GreenField.Web.DocumentCopyService;
using GreenField.DataContracts;
using GreenField.DAL;
using GreenField.Web.ListsDefinitions;
using System.Xml;
using GreenField.Web.DimensionEntitiesService;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DocumentWorkspaceOperations
    {
        private String DocumentLibrary
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocumentLibrary");
            }
        }

        private String DocumentServerUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocumentServerUrl");
            }
        }

        private String UserName
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocumentServerUserName");
            }
        }

        private String Password
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocumentServerPassword");
            }
        }

        private String Domain
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocumentServerDomain");
            }
        }

        private String DocumentServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocumentWebServiceUrl");
            }
        }

        private String ListsWebServiceUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("ListsWebServiceUrl");
            }
        }

        private ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        /// <summary>
        /// Dimension Service Entity
        /// </summary>
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

        /// <summary>
        /// Instance of CopyWebService
        /// </summary>
        private Copy _copyService = null;
        public Copy CopyService
        {
            get
            {
                if (_copyService == null)
                {
                    _copyService = new Copy();
                    _copyService.Credentials = new NetworkCredential(UserName, Password, Domain);
                    _copyService.Url = DocumentServiceUrl;
                }

                return _copyService;
            }
        }


        /// <summary>
        /// Instance of ListsWebService
        /// </summary>
        private Lists _listsService = null;
        public Lists ListsService
        {
            get
            {
                if (_listsService == null)
                {
                    _listsService = new Lists();
                    _listsService.Credentials = new NetworkCredential(UserName, Password, Domain);
                    _listsService.Url = ListsWebServiceUrl;
                }

                return _listsService;
            }
        }

        /// <summary>
        /// Returns the url of file after upload is successful
        /// </summary>
        /// <param name="fileName">name of the file to upload</param>
        /// <param name="fileByteStream"> byte streams to return</param>
        /// <returns>file url is upload is successful;empty otherwise</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public String UploadDocument(String fileName, Byte[] fileByteStream)
        {
            try
            {
                String resultUrl = String.Empty;
                try
                {
                    String[] destinationUrl = { DocumentServerUrl + DocumentLibrary + "/" + "[" + DateTime.UtcNow.ToString("ddMMyyyy") + "]" + fileName };

                    DocumentCopyService.CopyResult[] cResultArray = { new DocumentCopyService.CopyResult() };
                    DocumentCopyService.FieldInformation[] ffieldInfoArray = { new DocumentCopyService.FieldInformation() };

                    UInt32 copyResult = CopyService.CopyIntoItems(destinationUrl[0], destinationUrl, ffieldInfoArray, fileByteStream, out cResultArray);

                    if (cResultArray[0].ErrorCode == CopyErrorCode.Success)
                        resultUrl = cResultArray[0].DestinationUrl;
                }
                catch (Exception)
                {
                    throw;
                }

                return resultUrl;
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
        public bool DeleteDocument(String fileName)
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

                XmlNode ndReturn = ListsService.UpdateListItems(DocumentLibrary, elBatch);

                if (ndReturn.InnerText.ToLower() == "0x00000000".ToLower())
                {
                    fileDeleted = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

            return fileDeleted;
        }

        /// <summary>
        /// Retruns the bytes of the requested file
        /// </summary>
        /// <param name="fileName">name of the file</param>
        /// <returns>byte array is successful;null otherwise</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Byte[] RetrieveDocument(String fileName)
        {
            Byte[] result = null;
            try
            {
                String sourceUrl = DocumentServerUrl + DocumentLibrary + "/" + fileName;
                DocumentCopyService.FieldInformation[] ffieldInfoArray = { new DocumentCopyService.FieldInformation() };
                UInt32 retrieveResult = CopyService.GetItem(sourceUrl, out ffieldInfoArray, out result);

            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DocumentCategoricalData> RetrieveDocumentsData(String searchString)
        {
            try
            {
                List<DocumentCategoricalData> result = new List<DocumentCategoricalData>();

                ICPresentationEntities entity = new ICPresentationEntities();
                List<DocumentsData> documentsDataInfo = entity.GetDocumentsData(searchString).ToList();

                if (documentsDataInfo == null)
                    return result;

                List<DocumentsData> distinctDocumentsData = documentsDataInfo
                    .GroupBy(record => record.FileID, (key, group) => group.First()).ToList();

                foreach (DocumentsData distinctInfo in distinctDocumentsData)
                {
                    List<DocumentsData> distinctDocumentsDataInfo = documentsDataInfo
                        .Where(record => record.FileID == distinctInfo.FileID).ToList();

                    List<CommentDetails> commentsDetails = new List<CommentDetails>();

                    foreach (DocumentsData documentData in distinctDocumentsDataInfo)
                    {
                        commentsDetails.Add(new CommentDetails()
                        {
                            Comment = documentData.Comment,
                            CommentBy = documentData.CommentBy,
                            CommentOn = Convert.ToDateTime(documentData.CommentOn)
                        });
                    }

                    result.Add(new DocumentCategoricalData()
                       {
                           DocumentCategoryType = (DocumentCategoryType)EnumUtils.ToEnum(distinctInfo.Type, typeof(DocumentCategoryType)),
                           DocumentCompanyName = distinctInfo.SecurityName,
                           DocumentCompanyTicker = distinctInfo.SecurityTicker,
                           DocumentCatalogData = new DocumentCatalogData()
                           {
                               FileId = distinctInfo.FileID,
                               FileMetaTags = distinctInfo.MetaTags,
                               FileName = distinctInfo.Name,
                               FilePath = distinctInfo.Location,
                               FileUploadedBy = distinctInfo.CreatedBy,
                               FileUploadedOn = Convert.ToDateTime(distinctInfo.CreatedOn)
                           },
                           CommentDetails = commentsDetails
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
        public Boolean SetUploadFileInfo(String userName, String Name, String Location, String SecurityName
                    , String SecurityTicker, String Type, String MetaTags, String Comments)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetUploadFileInfo(userName, Name, Location, SecurityName
                    , SecurityTicker, Type, MetaTags, Comments).FirstOrDefault();
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
        public List<string> GetDocumentsMetaTags()
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                List<string> metaTagsInfo = entity.FileMasters.Select(a => a.MetaTags).Distinct().ToList();
                metaTagsInfo.AddRange(entity.FileMasters.Select(a => a.SecurityName).Distinct().ToList());
                metaTagsInfo.AddRange(entity.FileMasters.Select(a => a.SecurityTicker).Distinct().ToList());


                return metaTagsInfo;

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
        public List<DocumentCategoricalData> RetrieveDocumentsDataForUser(String userName)
        {
            try
            {
                List<DocumentCategoricalData> result = new List<DocumentCategoricalData>();
                ICPresentationEntities entity = new ICPresentationEntities();
                List<FileMaster> data = entity.FileMasters.Where(record => record.CreatedBy == userName).ToList();
                if (data == null)
                    return result;

                foreach (FileMaster fileMasterRecord in data)
                {
                    DocumentCatalogData documentCatalogData = new DocumentCatalogData()
                    {
                        FileId = fileMasterRecord.FileID,
                        FileMetaTags = fileMasterRecord.MetaTags,
                        FileName = fileMasterRecord.Name,
                        FilePath = fileMasterRecord.Location,
                        FileUploadedBy = fileMasterRecord.CreatedBy,
                        FileUploadedOn = Convert.ToDateTime(fileMasterRecord.CreatedOn)
                    };

                    List<CommentInfo> commentInfo = fileMasterRecord.CommentInfoes.ToList();
                    List<CommentDetails> commentDetails = new List<CommentDetails>();
                    foreach (CommentInfo info in commentInfo)
                    {
                        commentDetails.Add(new CommentDetails()
                        {
                            Comment = info.Comment,
                            CommentBy = info.CommentBy,
                            CommentOn = Convert.ToDateTime(info.CommentOn)
                        });
                    }

                    result.Add(new DocumentCategoricalData()
                    {
                        CommentDetails = commentDetails,
                        DocumentCatalogData = documentCatalogData,
                        DocumentCategoryType = (DocumentCategoryType)EnumUtils.ToEnum(fileMasterRecord.Type, typeof(DocumentCategoryType)),
                        DocumentCompanyName = fileMasterRecord.SecurityName,
                        DocumentCompanyTicker = fileMasterRecord.SecurityTicker
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
        /// Inserts Comment on a file
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="fileId">file Id</param>
        /// <param name="comment">comment</param>
        /// <returns>True for successful insertion, else false</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean SetDocumentComment(String userName, Int64 fileId, String comment)
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                Int32? result = entity.SetFileCommentInfo(userName, fileId, comment).FirstOrDefault();
                return result == 0;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        #region ExcelModelUpload

        /// <summary>
        /// Get Reuters Data 
        /// </summary>
        /// <param name="issuerID">issuerId of Selected Security</param>
        /// <param name="currency">Currency</param>
        /// <returns>Collection of FinancialStatementsData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public byte[] RetrieveStatementData(EntitySelectionData selectedSecurity)
        {
            try
            {
                ExternalResearchEntities entity = new ExternalResearchEntities();
                if (selectedSecurity == null)
                    return new byte[1];

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == selectedSecurity.InstrumentID &&
                        record.ISSUE_NAME == selectedSecurity.LongName &&
                        record.TICKER == selectedSecurity.ShortName).FirstOrDefault();
                if (securityDetails == null)
                {
                    return new byte[1];
                }
                External_Country_Master countryDetails = entity.External_Country_Master
                .Where(record => record.COUNTRY_CODE == securityDetails.ISO_COUNTRY_CODE &&
                 record.COUNTRY_NAME == securityDetails.ASEC_SEC_COUNTRY_NAME)
                .FirstOrDefault();

                string issuerID = securityDetails.ISSUER_ID;
                string currency = countryDetails.CURRENCY_CODE;

                if (issuerID == null)
                {
                    return new byte[1];
                }

                List<ModelConsensusEstimatesData> resultConsensus = new List<ModelConsensusEstimatesData>();
                List<FinancialStatementData> resultReuters = new List<FinancialStatementData>();
                List<FinancialStatementData> resultStatement = new List<FinancialStatementData>();
                List<FinancialStatementType> statementType = new List<FinancialStatementType>() { FinancialStatementType.INCOME_STATEMENT, FinancialStatementType.BALANCE_SHEET, FinancialStatementType.CASH_FLOW_STATEMENT };

                List<FinancialStatementPeriodType> periodType = new List<FinancialStatementPeriodType>() { FinancialStatementPeriodType.ANNUAL, FinancialStatementPeriodType.QUARTERLY };


                foreach (FinancialStatementType item in statementType)
                {
                    string statement = EnumUtils.ToString(item);
                    foreach (FinancialStatementPeriodType period in periodType)
                    {
                        if (currency != null)
                        {
                            resultStatement = entity.Get_Statement_Models(issuerID, "REUTERS", EnumUtils.ToString(period).Substring(0, 1), "FISCAL", statement, currency).ToList();
                        }
                        if (resultStatement == null || resultStatement.Count == 0)
                        {
                            if (currency != "USD")
                            {
                                resultStatement = entity.Get_Statement_Models(issuerID, "REUTERS", EnumUtils.ToString(period).Substring(0, 1), "FISCAL", statement, "USD").ToList();
                            }
                        }

                        if (resultStatement != null)
                        {
                            resultReuters.AddRange(resultStatement);
                        }
                    }
                }
                resultReuters = resultReuters.Where(a => a.PeriodYear != 2300).ToList();

                List<ModelConsensusEstimatesData> data = new List<ModelConsensusEstimatesData>();

                foreach (FinancialStatementPeriodType item in periodType)
                {
                    if (currency != null)
                    {
                        data = entity.GetModelConsensusEstimates(issuerID, "REUTERS", EnumUtils.ToString(item).Substring(0, 1), "FISCAL", currency).ToList();
                    }
                    if (data == null || data.Count == 0)
                    {
                        if (currency != "USD")
                        {
                            data = entity.GetModelConsensusEstimates(issuerID, "REUTERS", EnumUtils.ToString(item).Substring(0, 1), "FISCAL", "USD").ToList();
                        }
                    }
                    if (data != null)
                    {
                        resultConsensus.AddRange(data);
                    }
                }

                return GenerateExcelModel.GenerateExcel(resultReuters, resultConsensus);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Get CE Data 
        /// </summary>
        /// <param name="issuerID">issuerId of Selected Security</param>
        /// <param name="currency">Currency</param>
        /// <returns>Collection of FinancialStatementsData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<ModelConsensusEstimatesData> RetrieveCEData(string issuerID, String currency)
        {
            List<ModelConsensusEstimatesData> resultConsensus = new List<ModelConsensusEstimatesData>();

            List<FinancialStatementPeriodType> periodType = new List<FinancialStatementPeriodType>() { FinancialStatementPeriodType.ANNUAL, FinancialStatementPeriodType.QUARTERLY };
            ExternalResearchEntities entity = new ExternalResearchEntities();
            List<ModelConsensusEstimatesData> data = new List<ModelConsensusEstimatesData>();

            foreach (FinancialStatementPeriodType item in periodType)
            {
                data = entity.GetModelConsensusEstimates(issuerID, "REUTERS", EnumUtils.ToString(item), "FISCAL", currency).ToList();
                if (data == null || data.Count == 0)
                {
                    data = entity.GetModelConsensusEstimates(issuerID, "REUTERS", EnumUtils.ToString(item), "FISCAL", "USD").ToList();
                }
                resultConsensus.AddRange(data);
            }

            return resultConsensus;
        }

        #endregion

    }
}
