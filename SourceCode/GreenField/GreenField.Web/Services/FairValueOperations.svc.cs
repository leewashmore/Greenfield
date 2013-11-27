using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Xml.Linq;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.DAL;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.Helpers;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;

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

        /*private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));

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
                GreenField.DAL.GF_SECURITY_BASEVIEW data = GetSecurityDataForSelectedSecurity(entitySelectionData);

                if (data == null)
                    return null;
               
                ////Retrieving data from Period Financials table
                resultDB = fairValueCompSummary.ExecuteStoreQuery<GetFairValueComposition_Result>("exec GetFairValueCompositionSummaryData @SECURITY_ID={0}", Convert.ToString(data.SECURITY_ID)).ToList();

                if (resultDB == null || resultDB.Count == 0)
                {
                    List<FairValueCompositionSummaryData> items = GetSummaryDataIfDatabaseContaisnNorecords(data);
                    result.AddRange(items);
                }

                foreach (GetFairValueComposition_Result record in resultDB)
                {
                    FairValueCompositionSummaryData item = new FairValueCompositionSummaryData();
                    if (!String.IsNullOrEmpty(record.SOURCE))
                    {
                        if (record.SOURCE.ToUpper() == "PRIMARY")
                        {
                            item.Source = "Primary Analyst";
                        }
                        else
                        {
                            if (record.SOURCE.ToUpper() == "INDUSTRY")
                            {
                                item.Source = "Industry Analyst";
                            }
                            else
                            {
                                item.Source = record.SOURCE;
                            }
                        }
                    }
                    item.Measure = record.MEASURE;
                    item.Buy = record.BUY;
                    item.Sell = record.SELL;
                    item.Upside = record.UPSIDE;
                    if (record.DATE != null)
                        item.Date = record.DATE.Value;
                    item.DataId = record.DATA_ID;
                    item.PrimaryAnalyst = data.ASHMOREEMM_PRIMARY_ANALYST;
                    item.IndustryAnalyst = data.ASHMOREEMM_INDUSTRY_ANALYST;
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
        public List<FairValueCompositionSummaryData> RetrieveFairValueCompostionSummaryData(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<FairValueCompositionSummaryData> result = new List<FairValueCompositionSummaryData>();
                List<GetFairValueComposition_Result> resultDB = new List<GetFairValueComposition_Result>();
                ExternalResearchEntities fairValueCompSummary = new ExternalResearchEntities();

                if (entitySelectionData == null)
                {
                    return null;
                }

                //retrieving data from security view
                GreenField.DAL.GF_SECURITY_BASEVIEW data = GetSecurityDataForSelectedSecurity(entitySelectionData);

                if (data == null)
                {
                    return null;
                }

                //retrieving data from period financials table
                resultDB = fairValueCompSummary.ExecuteStoreQuery<GetFairValueComposition_Result>
                    ("exec GetFairValueCompositionSummaryData @SECURITY_ID={0}", Convert.ToString(data.SECURITY_ID)).ToList();

                string sourceNames = string.Empty;
                foreach (GetFairValueComposition_Result record in resultDB)
                {
                    FairValueCompositionSummaryData item = new FairValueCompositionSummaryData();
                    if (!String.IsNullOrEmpty(record.SOURCE))
                    {
                        if (record.SOURCE.ToUpper() == "PRIMARY")
                        {
                            item.Source = "Primary Analyst";
                        }
                        else if (record.SOURCE.ToUpper() == "INDUSTRY")
                        {
                            item.Source = "Industry Analyst";
                        }
                        else if (record.SOURCE.ToUpper() == "DCF_PE")
                        {
                            item.Source = "DCF-PE";
                        }
                        else if (record.SOURCE.ToUpper() == "DCF_PBV")
                        {
                            item.Source = "DCF-PBV";
                        }
                        else
                        {
                            item.Source = record.SOURCE;
                        }                       
                    }
                    sourceNames += item.Source + ",";
                    item.Measure = record.MEASURE;
                    item.Buy = record.BUY;
                    item.Sell = record.SELL;
                    item.Upside = record.UPSIDE;
                    if (record.DATE != null)
                    {
                        item.Date = record.DATE.Value;
                    }
                    item.DataId = record.DATA_ID;
                    result.Add(item);
                }
                if (!sourceNames.Contains("Primary Analyst"))
                {
                    result.Add(new FairValueCompositionSummaryData { Source = "Primary Analyst" });
                }
                if (!sourceNames.Contains("Industry Analyst"))
                {
                    result.Add(new FairValueCompositionSummaryData { Source = "Industry Analyst" });
                }
                if (!sourceNames.Contains("DCF-PE"))
                {
                    result.Add(new FairValueCompositionSummaryData { Source = "DCF-PE" });
                }
                if (!sourceNames.Contains("DCF-PBV"))
                {
                    result.Add(new FairValueCompositionSummaryData { Source = "DCF-PBV" });
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
                {
                    return null;
                }

                if (entitySelectionData == null)
                {
                    return null;
                }

                //retrieving data from security view
                GreenField.DAL.GF_SECURITY_BASEVIEW data = GetSecurityDataForSelectedSecurity(entitySelectionData);

                if (data == null)
                {
                    return null;
                }

                string securityId = Convert.ToString(data.SECURITY_ID);
                int? dataId = editedFairValueData.DataId;
                string dataSource = editedFairValueData.Source;

                ExternalResearchEntities entity = new ExternalResearchEntities();

                decimal? amountValue = entity.GetAmountForUpsideCalculation(securityId, dataId, dataSource).FirstOrDefault();

                if (amountValue != null)
                {
                    if (amountValue == 0 || editedFairValueData.Sell == null)
                    {
                        upsideValue = 0;
                    }
                    else
                    {
                        if (dataId != null && dataId != 236)
                        {
                            upsideValue = (decimal)(editedFairValueData.Sell / amountValue) - 1;
                        }
                        else
                        {
                            if (editedFairValueData.Sell != 0)
                            {
                                upsideValue = (decimal)(amountValue / (editedFairValueData.Sell / 100)) - 1;
                            }
                        }
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
                {
                    return null;
                }

                GreenField.DAL.GF_SECURITY_BASEVIEW data = GetSecurityDataForSelectedSecurity(entitySelectionData);

                if (data == null)
                {
                    return null;
                }

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
                            {
                                item.Source = "Primary Analyst";
                            }
                            else
                            {
                                if (record.SOURCE.ToUpper() == "INDUSTRY")
                                {
                                    item.Source = "Industry Analyst";
                                }
                                else
                                {
                                    item.Source = record.SOURCE;
                                }
                            }
                        }
                        item.Measure = record.MEASURE;
                        item.Buy = record.BUY;
                        item.Sell = record.SELL;
                        item.Upside = record.UPSIDE;
                        if (record.DATE != null)
                        {
                            item.Date = record.DATE.Value;
                        }
                        item.DataId = record.DATA_ID;
                        item.PrimaryAnalyst = data.ASHMOREEMM_PRIMARY_ANALYST;
                        item.IndustryAnalyst = data.ASHMOREEMM_INDUSTRY_ANALYST;

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
                Buy = editedFairValueData.Buy,
                Sell = editedFairValueData.Sell,
                Upside = upsideValue,
                Source = editedFairValueData.Source,
                DataId = editedFairValueData.DataId,
                Date = editedFairValueData.Date,
                Measure= editedFairValueData.Measure                
            };            

            return data;
        }

        private void UpdateSoureValues(ref List<FairValueCompositionSummaryData> dataList)
        {            
            foreach (FairValueCompositionSummaryData data in dataList)
            {
                if (data.Source == "Primary Analyst")
                    data.Source = "PRIMARY";
                if (data.Source == "Industry Analyst")
                    data.Source = "INDUSTRY";
            }
        }


        private GreenField.DAL.GF_SECURITY_BASEVIEW GetSecurityDataForSelectedSecurity(EntitySelectionData entitySelectionData)
        {
            DimensionEntities entity = DimensionEntity;

            bool isServiceUp;
            isServiceUp = CheckServiceAvailability.ServiceAvailability();

            if (!isServiceUp)
                throw new Exception("Services are not available");
            //Retrieving data from security view
            GreenField.DAL.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
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

        private List<FairValueCompositionSummaryData> GetSummaryDataIfDatabaseContaisnNorecords(GreenField.DAL.GF_SECURITY_BASEVIEW data)
        {
            List<FairValueCompositionSummaryData> dataList = new List<FairValueCompositionSummaryData>();

            FairValueCompositionSummaryData item1 = new FairValueCompositionSummaryData();
            item1.Source = "Primary Analyst";
            item1.Measure = null;
            item1.Buy = null;
            item1.Sell = null;
            item1.Upside = null;
            item1.DataId = null;
            item1.PrimaryAnalyst = data.ASHMOREEMM_PRIMARY_ANALYST;
            item1.IndustryAnalyst = data.ASHMOREEMM_INDUSTRY_ANALYST;
            dataList.Add(item1);

            FairValueCompositionSummaryData item2 = new FairValueCompositionSummaryData();
            item2.Source = "Industry";
            item2.Measure = null;
            item2.Buy = null;
            item2.Sell = null;
            item2.Upside = null;
            item2.DataId = null;
            item2.PrimaryAnalyst = data.ASHMOREEMM_PRIMARY_ANALYST;
            item2.IndustryAnalyst = data.ASHMOREEMM_INDUSTRY_ANALYST;
            dataList.Add(item2);

            return dataList;
        }
    }
}
