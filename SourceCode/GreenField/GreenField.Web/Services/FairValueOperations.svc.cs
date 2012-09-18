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
using GreenField.DataContracts;
using GreenField.DAL;
using System.Data.Objects;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using GreenField.Web.DataContracts;
using System.Data;
using GreenField.DataContracts.DataContracts;

namespace GreenField.Web.Services
{
    /// <summary>
    /// FAIR VALUE OPERATIONS
    /// </summary>
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    public class FairValueOperations
    {
       public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

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
        /// Gets FAIR VALUE COMPOSITION SUMMARY  Data
        /// </summary>
        /// <param name="securityId"></param>
        /// <returns>FairValueCompositionSummary data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FairValueCompositionSummaryData> RetrieveFairValueCompostionSummary(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<FairValueCompositionSummaryData> result = new List<FairValueCompositionSummaryData>();
                List<GetFairValueComposition_Result> resultDB = new List<GetFairValueComposition_Result>();
                ExternalResearchEntities fairValueCompSummary = new ExternalResearchEntities();

                if (entitySelectionData == null)
                    return null;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");
                //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (data == null)
                    return null;
               
                ////Retrieving data from Period Financials table
                resultDB = fairValueCompSummary.ExecuteStoreQuery<GetFairValueComposition_Result>("exec GetFairValueCompositionSummaryData @SECURITY_ID={0}", Convert.ToString(data.SECURITY_ID)).ToList();

                foreach (GetFairValueComposition_Result record in resultDB)
                {
                    FairValueCompositionSummaryData item = new FairValueCompositionSummaryData();
                    if (!String.IsNullOrEmpty(record.SOURCE))
                    {
                        if(record.SOURCE.ToUpper() == "PRIMARY")
                            item.SOURCE = "Primary Analyst";
                        else if (record.SOURCE.ToUpper() == "INDUSTRY")
                            item.SOURCE = "Industry Analyst";
                        else if (record.SOURCE.ToUpper() == "PFV_PE")
                            item.SOURCE = "DCF-PE";
                        else if (record.SOURCE.ToUpper() == "PFV_PBV")
                            item.SOURCE = "DCF-PBV";
                    }
                    item.MEASURE = record.MEASURE;
                    item.BUY = record.BUY;
                    item.SELL = record.SELL;
                    item.UPSIDE = record.UPSIDE;
                    if (record.DATE != null)
                        item.DATE = record.DATE.Value;
                    item.DATA_ID = record.DATA_ID;
                    result.Add(item);
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
