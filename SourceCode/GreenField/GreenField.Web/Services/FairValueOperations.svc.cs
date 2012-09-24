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
using System.Xml.Linq;
using System.Reflection;

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

               //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = GetSecurityDataForSelectedSecurity(entitySelectionData);

                if (data == null)
                    return null;
               
                ////Retrieving data from Period Financials table
                resultDB = fairValueCompSummary.ExecuteStoreQuery<GetFairValueComposition_Result>("exec GetFairValueCompositionSummaryData @SECURITY_ID={0}", Convert.ToString(data.SECURITY_ID)).ToList();

                foreach (GetFairValueComposition_Result record in resultDB)
                {
                    FairValueCompositionSummaryData item = new FairValueCompositionSummaryData();
                    if (!String.IsNullOrEmpty(record.SOURCE))
                    {
                        if (record.SOURCE.ToUpper() == "PRIMARY")
                        {
                            item.SOURCE = "Primary Analyst";
                        }
                        else
                        {
                            if (record.SOURCE.ToUpper() == "INDUSTRY")
                            {
                                item.SOURCE = "Industry Analyst";
                            }
                            else
                            {
                                item.SOURCE = record.SOURCE;
                            }
                        }
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

        /// <summary>
        /// Gets FAIR VALUE COMPOSITION SUMMARY  Data
        /// </summary>
        /// <param name="securityId"></param>
        /// <returns>FairValueCompositionSummary data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public FairValueCompositionSummaryData RetrieveFairValueDataWithNewUpside(EntitySelectionData entitySelectionData,
            FairValueCompositionSummaryData editedFairValueData)
        {
            FairValueCompositionSummaryData result = null;
            decimal upsideValue = 0;
            try
            {
                if (entitySelectionData == null || editedFairValueData == null)
                    return null;

                if (entitySelectionData == null)
                    return null;

                //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = GetSecurityDataForSelectedSecurity(entitySelectionData);

                if (data == null)
                    return null;

                string securityId = Convert.ToString(data.SECURITY_ID);
                int? dataId = editedFairValueData.DATA_ID;
                string dataSource = editedFairValueData.SOURCE;

                ExternalResearchEntities entity = new ExternalResearchEntities();

                decimal? amountValue = entity.GetAmountForUpsideCalculation(securityId, dataId, dataSource).FirstOrDefault();

                if (amountValue != null)
                {
                    if (amountValue == 0 || editedFairValueData.SELL == null)
                    {
                        upsideValue = 0;
                    }
                    else
                    {                        
                        upsideValue = (decimal)(editedFairValueData.SELL / amountValue) - 1;
                    }
                }

                result = GetFairValueSummary(editedFairValueData, upsideValue);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

            return result;
        }

        /// <summary>
        /// Persists the updated FairValue MeasureData in Database
        /// </summary>
        /// <param name="securityId">securityId</param>
        /// <returns>FairValueCompositionSummary data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FairValueCompositionSummaryData> SaveUpdatedFairValueData(EntitySelectionData entitySelectionData,
            List<FairValueCompositionSummaryData> editedFairValueData)
        {
            List<FairValueCompositionSummaryData> result = null;            
            try
            {
                if (entitySelectionData == null || editedFairValueData == null)
                    return null;

                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = GetSecurityDataForSelectedSecurity(entitySelectionData);

                if (data == null)
                    return null;

                string securityId = Convert.ToString(data.SECURITY_ID);
              
                ExternalResearchEntities entity = new ExternalResearchEntities();
                UpdateSoureValues(ref editedFairValueData);
                XDocument doc = GetEntityXml<FairValueCompositionSummaryData>(editedFairValueData);

                var updatedResultSet = entity.SaveUpdatedFairValueMeasures(securityId, doc.ToString()).ToList();

                if (updatedResultSet != null)
                {
                    result = new List<FairValueCompositionSummaryData>();
                    foreach (GetFairValueComposition_Result record in updatedResultSet)
                    {
                        FairValueCompositionSummaryData item = new FairValueCompositionSummaryData();
                        if (!String.IsNullOrEmpty(record.SOURCE))
                        {
                            if (record.SOURCE.ToUpper() == "PRIMARY")
                                item.SOURCE = "Primary Analyst";
                            else if (record.SOURCE.ToUpper() == "INDUSTRY")
                                item.SOURCE = "Industry Analyst";                            
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
                }                
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }

            return result;
        }

        private FairValueCompositionSummaryData GetFairValueSummary(FairValueCompositionSummaryData editedFairValueData, decimal upsideValue)
        {
            FairValueCompositionSummaryData data = new FairValueCompositionSummaryData()
            {
                BUY = editedFairValueData.BUY,
                SELL = editedFairValueData.SELL,
                UPSIDE = upsideValue,
                SOURCE = editedFairValueData.SOURCE,
                DATA_ID = editedFairValueData.DATA_ID,
                DATE = editedFairValueData.DATE,
                MEASURE= editedFairValueData.MEASURE                
            };            

            return data;
        }

        private void UpdateSoureValues(ref List<FairValueCompositionSummaryData> dataList)
        {            
            foreach (FairValueCompositionSummaryData data in dataList)
            {
                if (data.SOURCE == "Primary Analyst")
                    data.SOURCE = "PRIMARY";
                if (data.SOURCE == "Industry Analyst")
                    data.SOURCE = "INDUSTRY";
            }
        }


        private GF_SECURITY_BASEVIEW GetSecurityDataForSelectedSecurity(EntitySelectionData entitySelectionData)
        {
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
            else
                return data;

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
    }
}
