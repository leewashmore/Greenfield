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
                    //result = copyResult == 0 && cResultArray.First().ErrorCode == DocumentCopyService.CopyErrorCode.Success;
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
        
    }
}
