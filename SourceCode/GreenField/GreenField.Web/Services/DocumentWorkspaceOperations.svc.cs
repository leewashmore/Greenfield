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

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DocumentWorkspaceOperations
    {
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        private DocumentWorkspaceService.DwsSoapClient _documentWorkspaceClient;
        public DocumentWorkspaceService.DwsSoapClient DocumentWorkspaceClient
        {
            get
            {
                if (_documentWorkspaceClient == null)
                {                    
                    _documentWorkspaceClient = new DocumentWorkspaceService.DwsSoapClient();                    
                }
                return _documentWorkspaceClient;
            }
        }

        private DocumentCopyService.CopySoapClient _documentCopySoapClient;
        public DocumentCopyService.CopySoapClient DocumentCopySoapClient
        {
            get
            {
                if (_documentCopySoapClient == null)
                {
                    _documentCopySoapClient = new DocumentCopyService.CopySoapClient();
                }
                return _documentCopySoapClient;
            }
        }        

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean UploadDocument(String fileName, Byte[] fileByteStream)
        {           
            try
            {
                Boolean result = false;
                String documentLibrary = ConfigurationManager.AppSettings.Get("DocumentLibrary");
                String documentServerUrl = ConfigurationManager.AppSettings.Get("DocumentServerUrl");
                
                //DocumentCopyService.Copy copyService = new DocumentCopyService.Copy();

                //if (DocumentCopySoapClient.ClientCredentials != null)
                //{
                //    Client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
                //}

                try
                {
                    DocumentCopySoapClient.Open();
                    String[] destinationUrl = { documentServerUrl + documentLibrary + "/" + fileName };
                    DocumentCopyService.CopyResult cResult1 = new DocumentCopyService.CopyResult();
                    DocumentCopyService.CopyResult cResult2 = new DocumentCopyService.CopyResult();
                    DocumentCopyService.CopyResult[] cResultArray = { cResult1, cResult2 };
                    DocumentCopyService.FieldInformation fFiledInfo = new DocumentCopyService.FieldInformation();
                    fFiledInfo.DisplayName = "Description";
                    fFiledInfo.Type = DocumentCopyService.FieldType.Text;
                    fFiledInfo.Value = "Sample Description";
                    DocumentCopyService.FieldInformation[] ffieldInfoArray = { fFiledInfo };
                                        
                    //Byte[] fileContents = new Byte[fileStream.Length];                    
                    //Int32 bufferReadChar = fileStream.Read(fileContents, 0, Convert.ToInt32(fileStream.Length));
                    //fileStream.Close();

                    //UInt32 copyResult = DocumentCopySoapClient.CopyIntoItems(fileName, destinationUrl, ffieldInfoArray, fileContents, out cResultArray);
                    UInt32 copyResult = DocumentCopySoapClient.CopyIntoItems(fileName, destinationUrl, ffieldInfoArray, fileByteStream, out cResultArray);
                    result = copyResult == 0;
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (DocumentCopySoapClient.State == CommunicationState.Faulted)
                    {
                        DocumentCopySoapClient.Abort();
                    }

                    if (DocumentCopySoapClient.State != CommunicationState.Closed)
                    {
                        DocumentCopySoapClient.Close();
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
    }
}
