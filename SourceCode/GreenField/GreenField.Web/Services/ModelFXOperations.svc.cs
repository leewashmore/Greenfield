using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.Helpers;
using System.Data;
using System.Data.SqlClient;
using GreenField.DAL;
using System.Collections;
using System.Data.Common;
using GreenField.DataContracts;

namespace GreenField.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ModelFXOperations" in code, svc and config file together.
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ModelFXOperations 
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

        /// <summary>
        /// Retrives data for Macro database key annual report
        /// </summary>
        /// <param name="CountryName"></param>
        /// <param name="regionName"></param>
        /// <returns>report data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MacroDatabaseKeyAnnualReportData> RetrieveMacroDatabaseKeyAnnualReportData(String countryNameVal)
        {
            try
            {
                //bool isServiceUp;
                //isServiceUp = CheckServiceAvailability.ServiceAvailability();

                //if (!isServiceUp)
                //    throw new Exception();

                List<MacroDatabaseKeyAnnualReportData> result = new List<MacroDatabaseKeyAnnualReportData>();
                //MacroDatabaseKeyAnnualReportData entry = new MacroDatabaseKeyAnnualReportData();
                DimensionEntitiesService.Entities entity = DimensionEntity;
                ResearchEntities research = new ResearchEntities();
                //IList macroDatalist =  research.RetrieveCTYSUMMARYDataReport("AR").ToList();
                result = research.ExecuteStoreQuery<MacroDatabaseKeyAnnualReportData>("exec RetrieveCTYSUMMARYDataReportPerCountry @country={0}", countryNameVal).ToList();
                
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
        public List<CountrySelectionData> RetrieveCountrySelectionData()
        {
            List<CountrySelectionData> result = new List<CountrySelectionData>();
            ResearchEntities research = new ResearchEntities();
            List<Country_Master> countryData = new List<Country_Master>();
            countryData = research.Country_Master.ToList();
            for (int i = 0; i < countryData.Count; i++)
            {
                CountrySelectionData entry = new CountrySelectionData();
                entry.CountryCode = countryData[i].COUNTRY_CODE;
                entry.CountryName = countryData[i].COUNTRY_NAME;
                result.Add(entry);
            }
            return result;
        }

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RegionSelectionData> RetrieveRegionSelectionData()
        {
            List<RegionSelectionData> result = new List<RegionSelectionData>();
            ResearchEntities research = new ResearchEntities();
            List<Country_Master> countryData = new List<Country_Master>();
            countryData = research.Country_Master.ToList();
            for (int i = 0; i < countryData.Count; i++)
            {
                RegionSelectionData entry = new RegionSelectionData();
                entry.Region = countryData[i].ASHEMM_PROPRIETARY_REGION_NAME;
                entry.Country = countryData[i].COUNTRY_CODE;
                result.Add(entry);
            }
            return result;
        
        }

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MacroDatabaseKeyAnnualReportData> RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(String countryNameVal,List<String> countryValues)
        {
            try
            {
                //bool isServiceUp;
                //isServiceUp = CheckServiceAvailability.ServiceAvailability();

                //if (!isServiceUp)
                //    throw new Exception();

                List<MacroDatabaseKeyAnnualReportData> result = new List<MacroDatabaseKeyAnnualReportData>();
                List<MacroDatabaseKeyAnnualReportData> finalResult = new List<MacroDatabaseKeyAnnualReportData>();
                //MacroDatabaseKeyAnnualReportData entry = new MacroDatabaseKeyAnnualReportData();
                DimensionEntitiesService.Entities entity = DimensionEntity;              
                ResearchEntities research = new ResearchEntities();
                //IList macroDatalist =  research.RetrieveCTYSUMMARYDataReport("AR").ToList();
                foreach (String c in countryValues)
                {
                    result = research.ExecuteStoreQuery<MacroDatabaseKeyAnnualReportData>("exec RetrieveEMSummaryDataReportPerCountry @country={0}", c).ToList();
                    if (result != null && result.Count != 0)
                    {
                        foreach (MacroDatabaseKeyAnnualReportData r in result)
                        {
                            finalResult.Add(r);
                        }
                    }
                }
                if (finalResult.Count == 0 || finalResult == null)
                    return finalResult;

                return finalResult;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Gets Commodity Data
        /// </summary>
        /// <param name="countryNameVal"></param>
        /// <returns>Commodity Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        
        public List<FXCommodityData> RetrieveCommodityData(string selectedCommodityID)
        {
            try
            {
                //bool isServiceUp;
                //isServiceUp = CheckServiceAvailability.ServiceAvailability();

                //if (!isServiceUp)
                //    throw new Exception();

                List<CommodityResult> resultDB = new List<CommodityResult>();                
                List<FXCommodityData> calculatedViewResult = new List<FXCommodityData>();
                List<FXCommodityData> result = new List<FXCommodityData>();
                ResearchEntities research = new ResearchEntities();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimSvcPricingViewData = null;
                List<DimensionEntitiesService.GF_SELECTION_BASEVIEW> dimSvcSelectionViewData = null;

                if (entity.GF_SELECTION_BASEVIEW == null && entity.GF_SELECTION_BASEVIEW.Count() == 0)
                    return null;

                if (entity.GF_PRICING_BASEVIEW == null && entity.GF_PRICING_BASEVIEW.Count() == 0)
                    return null;
                
                //Retrieving data from database             
                resultDB = research.ExecuteStoreQuery<CommodityResult>("exec GetCOMMODITY_FORECASTS @commodityID={0}", selectedCommodityID).ToList();
                //resultDB = research.ExecuteStoreQuery<CommodityResult>("exec GetCOMMODITY_FORECASTS").ToList();

                //Retrieving Data from Views

                if (selectedCommodityID != null && selectedCommodityID != string.Empty)
                {
                    if (selectedCommodityID.ToUpper() != GreenfieldConstants.COMMODITY_ALL)
                        dimSvcSelectionViewData = entity.GF_SELECTION_BASEVIEW
                                                .Where(g => (g.AIMS_COMMODITY_ID != null) && (g.AIMS_COMMODITY_ID.ToUpper() == selectedCommodityID.ToUpper()))
                                                .ToList();                
                    else                
                        dimSvcSelectionViewData = entity.GF_SELECTION_BASEVIEW
                                              .Where(g => g.AIMS_COMMODITY_ID != null)
                                              .ToList();
                }

                if (dimSvcSelectionViewData != null && dimSvcSelectionViewData.Count > 0)
                {
                    foreach (DimensionEntitiesService.GF_SELECTION_BASEVIEW item in dimSvcSelectionViewData)
                    {
                        if (item.INSTRUMENT_ID != null && item.INSTRUMENT_ID != string.Empty)
                        {
                            dimSvcPricingViewData = entity.GF_PRICING_BASEVIEW
                                                    .Where(g => g.INSTRUMENT_ID.ToUpper() == item.INSTRUMENT_ID.ToUpper())
                                                    .ToList();
                            if (dimSvcPricingViewData != null && dimSvcPricingViewData.Count > 0)
                            {
                                List<FXCommodityData> resultView = new List<FXCommodityData>();
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW itemPricing in dimSvcPricingViewData)
                                {
                                    FXCommodityData data = new FXCommodityData();

                                    data.CommodityId = item.AIMS_COMMODITY_ID;    // Assigning Commodity ID from above list
                                    data.FromDate = itemPricing.FROMDATE;
                                    data.DailyClosingPrice = itemPricing.DAILY_CLOSING_PRICE;
                                    data.InstrumentId = item.INSTRUMENT_ID;         //Assigning Instrument Id from above list

                                    resultView.Add(data);
                                }
                                FXCommodityData calculatedData = new FXCommodityData();

                                //calling method to perform calculations
                                FXCommodityData calculatedDataForCommodity = FXCommodityCalculations.CalculateCommodityData(resultView);
                                calculatedData.CommodityId = calculatedDataForCommodity.CommodityId;
                                calculatedData.InstrumentId = calculatedDataForCommodity.InstrumentId;
                                calculatedData.YTD = calculatedDataForCommodity.YTD;
                                calculatedData.Year1 = calculatedDataForCommodity.Year1;
                                calculatedData.Year3 = calculatedDataForCommodity.Year3;

                                calculatedViewResult.Add(calculatedData);
                            }

                        }
                    }
                }                
                for (int _index = 0; _index < resultDB.Count; _index++)
                {
                    foreach (FXCommodityData item in calculatedViewResult)
                    {
                        if (resultDB[_index].COMMODITY_ID == item.CommodityId)
                        {
                            FXCommodityData commodityData = new FXCommodityData();

                            commodityData.CommodityId = resultDB[_index].COMMODITY_ID;

                            //Columns coming from Commodity table
                            commodityData.CurrentYearEnd = Convert.ToDecimal(resultDB[_index].CURRENT_YEAR_END);
                            //commodityData.LastUpdate = Convert.ToDateTime(resultDB[_index].LASTUPDATE);
                            commodityData.LongTerm = Convert.ToDecimal(resultDB[_index].LONG_TERM);
                            commodityData.NextYearEnd = Convert.ToDecimal(resultDB[_index].NEXT_YEAR_END);

                            //Columns coming from Pricing View
                            commodityData.YTD = item.YTD;
                            commodityData.Year1 = item.Year1;
                            commodityData.Year3 = item.Year3;

                            result.Add(commodityData);
                        }
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
        /// <summary>
        /// Gets Commodity ID List
        /// </summary>
        /// <returns>List of Commodity ID</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FXCommodityData> RetrieveCommoditySelectionData()
        {
            List<FXCommodityData> result = new List<FXCommodityData>();
            ResearchEntities research = new ResearchEntities();
            List<COMMODITY_FORECASTS> commodityData = new List<COMMODITY_FORECASTS>();
            commodityData = research.COMMODITY_FORECASTS.ToList();
            for (int i = 0; i < commodityData.Count; i++)
            {
                FXCommodityData data = new FXCommodityData();
                data.CommodityId = commodityData[i].COMMODITY_ID;
                result.Add(data);
            }
            result.Add(new FXCommodityData { CommodityId = GreenfieldConstants.COMMODITY_ALL});
            return result;
        }
    }
}
