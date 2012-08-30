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

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DocumentWorkspaceOperations
    {
        private String documentLibrary = ConfigurationManager.AppSettings.Get("DocumentLibrary");
        private String documentServerUrl = ConfigurationManager.AppSettings.Get("DocumentServerUrl");

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
                    
                    _documentCopySoapClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential() { Domain = "headstrong.com", Password = "noida@15", UserName = "rvig" };
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

                try
                {
                    DocumentCopySoapClient.Open();

                    String[] destinationUrl = { documentServerUrl + documentLibrary + "/" + fileName };
                    String sourceUrl = documentServerUrl + "MytestDocLib" + "/" + "AddEntity.png";

                    DocumentCopyService.CopyResult[] cResultArray = { new DocumentCopyService.CopyResult() };
                    DocumentCopyService.FieldInformation[] ffieldInfoArray = { new DocumentCopyService.FieldInformation() };
                    
                    UInt32 copyResult = DocumentCopySoapClient.CopyIntoItems(destinationUrl[0], destinationUrl, ffieldInfoArray, fileByteStream, out cResultArray);
                    result = copyResult == 0 && cResultArray.First().ErrorCode == DocumentCopyService.CopyErrorCode.Success;
                }
                catch (Exception)
                {
                    throw;
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Byte[] RetrieveDocument(String fileName)
        {
            Byte[] result = null;

            try
            {
                DocumentCopySoapClient.Open();
                String sourceUrl = documentServerUrl + documentLibrary + "/" + fileName;
                DocumentCopyService.FieldInformation[] ffieldInfoArray = { new DocumentCopyService.FieldInformation() };
                UInt32 retrieveResult = DocumentCopySoapClient.GetItem(sourceUrl, out ffieldInfoArray, out result);

            }
            catch (Exception)
            {                
                throw;
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
    }
}
