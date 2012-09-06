using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using GreenField.Web.Helpers.Service_Faults;
using System.Resources;
using GreenField.DataContracts.DataContracts;
using GreenField.Web.Helpers;

namespace GreenField.Web.Services
{
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CustomScreeningToolOperations" in code, svc and config file together.
	public class CustomScreeningToolOperations
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

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CustomSelectionData> RetrieveCustomSelectionData()
        {
            try
            {
                List<CustomSelectionData> result = new List<CustomSelectionData>();

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

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
        public List<String> RetrieveCommaSeparatedList()
        {
            try
            {
                List<String> result = new List<String>();

                //checking if the service is down
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                StringBuilder issuerIDs = new StringBuilder();
                StringBuilder securityIDs = new StringBuilder();
                int check = 1;
                Dictionary<String, String> listForPortfolio = new Dictionary<string, string>();

                List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> dataPortfolio = new List<GF_PORTFOLIO_HOLDINGS>();
                List<DimensionEntitiesService.GF_BENCHMARK_HOLDINGS> dataBenchmark = new List<GF_BENCHMARK_HOLDINGS>();

                List<String> distinctSecuritiesForPortfolio = new List<String>();

                DimensionEntitiesService.Entities entity = DimensionEntity;

                dataPortfolio = entity.GF_PORTFOLIO_HOLDINGS
                               .Where(t => t.PORTFOLIO_ID == "ABPEQ" && t.PORTFOLIO_DATE == Convert.ToDateTime("05/31/2012")  && (t.A_SEC_INSTR_TYPE == "Equity" || t.A_SEC_INSTR_TYPE == "GDR/ADR") &&  t.DIRTY_VALUE_PC > 0)
                               .ToList();

                if (dataPortfolio != null && dataPortfolio.Count() > 0)
                {
                    distinctSecuritiesForPortfolio = dataPortfolio.Select(record => record.ISSUE_NAME).Distinct().ToList();
                }

                foreach (String issueName in distinctSecuritiesForPortfolio)
                {
                    GF_SECURITY_BASEVIEW securityDetails = entity.GF_SECURITY_BASEVIEW
                     .Where(record => record.ISSUE_NAME == issueName).FirstOrDefault();
                    if (securityDetails != null)
                    {
                        check = 0;
                        issuerIDs.Append(",'" + securityDetails.ISSUER_ID + "'");
                        securityIDs.Append(",'" + securityDetails.SECURITY_ID + "'");
                        listForPortfolio.Add(securityDetails.SECURITY_ID.ToString(), securityDetails.ISSUE_NAME);
                    }
                }

                issuerIDs = check == 0 ? issuerIDs.Remove(0, 1) : null;
                securityIDs = check == 0 ? securityIDs.Remove(0, 1) : null;

                string _issuerIds = issuerIDs == null ? null : issuerIDs.ToString();
                string _securityIds = securityIDs == null ? null : securityIDs.ToString();
                
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
