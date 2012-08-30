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

        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
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
                    String[] destinationUrl = { DocumentServerUrl + DocumentLibrary + "/" + fileName };                    

                    DocumentCopyService.CopyResult[] cResultArray = { new DocumentCopyService.CopyResult() };
                    DocumentCopyService.FieldInformation[] ffieldInfoArray = { new DocumentCopyService.FieldInformation() };

                    UInt32 copyResult = CopyService.CopyIntoItems(null, destinationUrl, ffieldInfoArray, fileByteStream, out cResultArray);

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
                #region Dummy Data entry
                //result.Add(new DocumentCategoricalData()
                //       {
                //           DocumentCategoryType = DocumentCategoryType.COMPANY_MEETING_NOTES,
                //           DocumentCompanyName = "Company1",
                //           DocumentCompanyTicker = "CompanyTicker1",
                //           DocumentCatalogData = new DocumentCatalogData()
                //           {
                //               FileId = 1,
                //               FileMetaTags = "Finance, specific catalog",
                //               FileName = "Financial Statement 27-07-2012.docx",
                //               FilePath = @"http://sharepointLocalSite/Documents/Financial Statement 27-07-2012.docx",
                //               FileUploadedBy = "Rahul Vig",
                //               FileUploadedOn = DateTime.Now.AddDays(-5)
                //           },
                //           CommentDetails = new List<CommentDetails>
                //    {
                //        new CommentDetails() { Comment = "Comment1", CommentBy = "Neeraj Jindal", CommentOn = DateTime.Now.AddDays(-1) },
                //        new CommentDetails() { Comment = "Comment2", CommentBy = "Neeraj Jindal", CommentOn = DateTime.Now.AddDays(-2) },
                //        new CommentDetails() { Comment = "Comment3", CommentBy = "Neeraj Jindal", CommentOn = DateTime.Now.AddDays(-3) }
                //    }
                //       });
                //result.Add(new DocumentCategoricalData()
                //{
                //    DocumentCategoryType = DocumentCategoryType.COMPANY_MEETING_NOTES,
                //    DocumentCompanyName = "Company2",
                //    DocumentCompanyTicker = "CompanyTicker2",
                //    DocumentCatalogData = new DocumentCatalogData()
                //    {
                //        FileId = 1,
                //        FileMetaTags = "Finance, specific catalog 2",
                //        FileName = "Financial Statement 30-07-2012.docx",
                //        FilePath = @"http://sharepointLocalSite/Documents/Financial Statement 27-07-2012.docx",
                //        FileUploadedBy = "Rahul Vig",
                //        FileUploadedOn = DateTime.Now.AddDays(-2)
                //    },
                //    CommentDetails = new List<CommentDetails>
                //    {
                //        new CommentDetails() { Comment = "Comment1", CommentBy = "Neeraj Jindal", CommentOn = DateTime.Now.AddDays(-1) },
                //        new CommentDetails() { Comment = "Comment2", CommentBy = "Neeraj Jindal", CommentOn = DateTime.Now.AddDays(-1) },
                //        new CommentDetails() { Comment = "Comment3", CommentBy = "Neeraj Jindal", CommentOn = DateTime.Now.AddDays(-2) }
                //    }
                //});

                //result.Add(new DocumentCategoricalData()
                //{
                //    DocumentCategoryType = DocumentCategoryType.BLOG,
                //    DocumentCompanyName = "Company1",
                //    DocumentCompanyTicker = "CompanyTicker1",
                //    DocumentCatalogData = null,
                //    CommentDetails = new List<CommentDetails>
                //    {
                //        new CommentDetails() { Comment = "Comment1", CommentBy = "Abhinav Singh", CommentOn = DateTime.Now.AddDays(-1) },
                //        new CommentDetails() { Comment = "Comment2", CommentBy = "Abhinav Singh", CommentOn = DateTime.Now.AddDays(-22) },
                //        new CommentDetails() { Comment = "Comment3", CommentBy = "Abhinav Singh", CommentOn = DateTime.Now.AddDays(-31) }
                //    }
                //}); 
                #endregion

                List<DocumentsData> data = new List<DocumentsData>();
                ICPresentationEntities entity = new ICPresentationEntities();
                data = entity.GetDocumentsData(searchString).ToList();

                if (data == null)
                    return result;

                List<DocumentsData> distinctFiles = data.GroupBy(
                                                            i => i.FileID,
                                                            (key, group) => group.First()).ToList();

                foreach (DocumentsData record in distinctFiles)
                {
                    List<DocumentsData> temp = new List<DocumentsData>();
                    temp = data.Where(a => a.FileID == record.FileID).ToList();

                    List<CommentDetails> commentsDetails = new List<CommentDetails>();

                    foreach (DocumentsData item in temp)
                    {
                        commentsDetails.Add(new CommentDetails()
                        {
                            Comment = item.Comment,
                            CommentBy = item.CommentBy,
                            CommentOn = Convert.ToDateTime(item.CommentOn)
                        });
                    }

                    result.Add(new DocumentCategoricalData()
                       {
                           DocumentCategoryType = (DocumentCategoryType)EnumUtils.ToEnum(record.Type, typeof(DocumentCategoryType)),
                           DocumentCompanyName = record.SecurityName,
                           DocumentCompanyTicker = record.SecurityTicker,
                           DocumentCatalogData = new DocumentCatalogData()
                           {
                               FileId = record.FileID,
                               FileMetaTags = record.MetaTags,
                               FileName = record.Name,
                               FilePath = '@' + '"' + record.Location +'"',
                               FileUploadedBy = record.CreatedBy,
                               FileUploadedOn = Convert.ToDateTime(record.CreatedOn)
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
        public List<string> GetDocumentsMetaTags()
        {
            try
            {
                ICPresentationEntities entity = new ICPresentationEntities();
                List<string> metaTagsInfo = entity.FileMasters.Select(a => a.MetaTags).Distinct().ToList();

                return metaTagsInfo;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
    }
}
