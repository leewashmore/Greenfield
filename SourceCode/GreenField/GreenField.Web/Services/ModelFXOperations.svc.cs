﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Resources;
using GreenField.DAL;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;
using System.Linq.Expressions;


namespace GreenField.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ModelFXOperations" in code, svc and config file together.
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ModelFXOperations 
    {
        /*private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                {
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
                }
                return dimensionEntity;
            }
        }
        */
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
        /// <param name="countryNameVal">Country Selected by the user</param>      
        /// <returns>report data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MacroDatabaseKeyAnnualReportData> RetrieveMacroDatabaseKeyAnnualReportData(String countryNameVal)
        {
            try
            {
             /*   bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception();
                }*/
                List<MacroDatabaseKeyAnnualReportData> result = new List<MacroDatabaseKeyAnnualReportData>();              
                DimensionEntities entity = DimensionEntity;
                ResearchEntities research = new ResearchEntities();              
                result = research.ExecuteStoreQuery<MacroDatabaseKeyAnnualReportData>
                    ("exec RetrieveCTYSUMMARYDataReportPerCountry @country={0}", countryNameVal).ToList();                
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
        /// Retrieves the list of countries
        /// </summary>
        /// <returns>List of countries in result of type CountrySelectionData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<CountrySelectionData> RetrieveCountrySelectionData()
        {
            // use cache if available
            var fromCache = (List<CountrySelectionData>)new DefaultCacheProvider().Get(CacheKeyNames.CountrySelectionDataCache);
            if (fromCache != null)
                return fromCache;

            // otherwise fetch the data and cache it
            List<CountrySelectionData> result = new List<CountrySelectionData>();
            ResearchEntities research = new ResearchEntities();
            List<Country_Master> countryData = new List<Country_Master>();
            countryData = research.Country_Master.Where(t=>t.MACRO_ECON_DATA_CURRENT == "Y").ToList();
            for (int i = 0; i < countryData.Count; i++)
            {
                CountrySelectionData entry = new CountrySelectionData();
                entry.CountryCode = countryData[i].COUNTRY_CODE;
                entry.CountryName = countryData[i].COUNTRY_NAME;
                result.Add(entry);
            }
            new DefaultCacheProvider().Set(CacheKeyNames.CountrySelectionDataCache, result, Int32.Parse(ConfigurationManager.AppSettings["CacheTime"]));
            return result;
        }
        /// <summary>
        /// Retrieves a list of regions 
        /// </summary>
        /// <returns>List of regions in result of type RegionSelectionData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<RegionSelectionData> RetrieveRegionSelectionData()
        {
            // use cache if available
            var fromCache = (List<RegionSelectionData>)new DefaultCacheProvider().Get(CacheKeyNames.RegionSelectionDataCache);
            if (fromCache != null)
                return fromCache;

            // otherwise fetch the data and cache it

            List<RegionSelectionData> result = new List<RegionSelectionData>();
            ResearchEntities research = new ResearchEntities();
            List<Country_Master> countryData = new List<Country_Master>();
            countryData = research.Country_Master.Where(t => t.MACRO_ECON_DATA_CURRENT == "Y").ToList();
            for (int i = 0; i < countryData.Count; i++)
            {
                RegionSelectionData entry = new RegionSelectionData();
                entry.Region = countryData[i].ASHEMM_PROPRIETARY_REGION_NAME;
                entry.Country = countryData[i].COUNTRY_CODE;
                entry.CountryNames = countryData[i].COUNTRY_NAME;
                result.Add(entry);
            }
            new DefaultCacheProvider().Set(CacheKeyNames.RegionSelectionDataCache, result, Int32.Parse(ConfigurationManager.AppSettings["CacheTime"]));
            return result;        
        }
        /// <summary>
        /// Retrives Data for MacroDatabaseKeyAnnualReportDataEMSummary
        /// </summary>
        /// <param name="countryNameVal">Country Name</param>
        /// <param name="countryValues">CountryValues in a Region</param>
        /// <returns>returns report data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MacroDatabaseKeyAnnualReportData> RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(String countryNameVal,List<String> countryValues)
        {
            try
            {
                /*bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception();
                }*/
                List<MacroDatabaseKeyAnnualReportData> result = new List<MacroDatabaseKeyAnnualReportData>();
                List<MacroDatabaseKeyAnnualReportData> finalResult = new List<MacroDatabaseKeyAnnualReportData>();            
                DimensionEntities entity = DimensionEntity;              
                ResearchEntities research = new ResearchEntities();            
                foreach (String c in countryValues)
                {
                    result = research.ExecuteStoreQuery<MacroDatabaseKeyAnnualReportData>
                        ("exec RetrieveEMSummaryDataReportPerCountry @country={0}", c).ToList();
                    if (result != null && result.Count != 0)
                    {
                        foreach (MacroDatabaseKeyAnnualReportData r in result)
                        {
                            finalResult.Add(r);
                        }
                    }
                }
                if (finalResult.Count == 0 || finalResult == null)
                {
                    return finalResult;
                }
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
              /*  bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();*/

                List<CommodityResult> resultDB = new List<CommodityResult>();                
                List<FXCommodityData> calculatedViewResult = new List<FXCommodityData>();
                List<FXCommodityData> result = new List<FXCommodityData>();
                ResearchEntities research = new ResearchEntities();

                DimensionEntities entity = DimensionEntity;
                List<GreenField.DAL.GF_PRICING_BASEVIEW> dimSvcPricingViewData = null;
                List<GreenField.DAL.GF_SELECTION_BASEVIEW> dimSvcSelectionViewData = null;

                if (entity.GF_SELECTION_BASEVIEW == null && entity.GF_SELECTION_BASEVIEW.Count() == 0)
                    return null;

                if (entity.GF_PRICING_BASEVIEW == null && entity.GF_PRICING_BASEVIEW.Count() == 0)
                    return null;
                
                //Retrieving data from database   
                if (String.IsNullOrEmpty(selectedCommodityID))
                {
                    selectedCommodityID = GreenfieldConstants.COMMODITY_ALL;
                    resultDB = research.ExecuteStoreQuery<CommodityResult>("exec GetCOMMODITY_FORECASTS @commodityID={0}", selectedCommodityID).ToList();
                }
                else
                    resultDB = research.ExecuteStoreQuery<CommodityResult>("exec GetCOMMODITY_FORECASTS @commodityID={0}", selectedCommodityID).ToList();
                 
                //Retrieving Data from Views
                DateTime CurrentDate = System.DateTime.Now;
                DateTime Date1DayBack = Convert.ToDateTime(FXCommodityCalculations.GetPreviousDate(CurrentDate));
                DateTime DateLastYearEnd = CurrentDate.AddYears(-1).AddMonths(-(CurrentDate.Month) + 12).AddDays(-(CurrentDate.Day) + 31);
                DateLastYearEnd = Convert.ToDateTime(FXCommodityCalculations.CheckBusinessDay(DateLastYearEnd));
                DateTime Date12MonthsAgo = CurrentDate.AddYears(-1);
                Date12MonthsAgo = Convert.ToDateTime(FXCommodityCalculations.CheckBusinessDay(Date12MonthsAgo));
                DateTime Date36MonthsAgo = CurrentDate.AddYears(-3);
                Date36MonthsAgo = Convert.ToDateTime(FXCommodityCalculations.CheckBusinessDay(Date36MonthsAgo));


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
                    List<GreenField.DAL.GF_PRICING_BASEVIEW> queryResultSet = new List<GreenField.DAL.GF_PRICING_BASEVIEW>();

                    Expression<Func<GreenField.DAL.GF_PRICING_BASEVIEW, bool>> searchPredicate1 = p => p.FROMDATE == Convert.ToDateTime(Date1DayBack.ToString()).Date;
                    searchPredicate1 = Utility.Or<GreenField.DAL.GF_PRICING_BASEVIEW>(searchPredicate1, g => g.FROMDATE == Convert.ToDateTime(DateLastYearEnd.ToString()).Date);
                    searchPredicate1 = Utility.Or<GreenField.DAL.GF_PRICING_BASEVIEW>(searchPredicate1, g => g.FROMDATE == Convert.ToDateTime(Date12MonthsAgo.ToString()).Date);
                    searchPredicate1 = Utility.Or<GreenField.DAL.GF_PRICING_BASEVIEW>(searchPredicate1, g => g.FROMDATE == Convert.ToDateTime(Date36MonthsAgo.ToString()).Date);

                    int recursionLimit = 10;

                    for (int j = 0; j < dimSvcSelectionViewData.Count(); j = j + recursionLimit)
                    {
                        Expression<Func<GreenField.DAL.GF_PRICING_BASEVIEW, bool>> searchPredicate2 = p => p.INSTRUMENT_ID.ToUpper() == dimSvcSelectionViewData[j].INSTRUMENT_ID.ToUpper();
                        for (int i = j + 1; i < j + recursionLimit && i < dimSvcSelectionViewData.Count(); i++)
                        {
                            if (dimSvcSelectionViewData[i].INSTRUMENT_ID == null)
                            {
                                continue;
                            }
                            string comparisonInstrumentId = dimSvcSelectionViewData[i].INSTRUMENT_ID.ToUpper();
                            searchPredicate2 = Utility.Or<GreenField.DAL.GF_PRICING_BASEVIEW>(searchPredicate2, p => p.INSTRUMENT_ID.ToUpper() == comparisonInstrumentId);
                        }

                        Expression<Func<GreenField.DAL.GF_PRICING_BASEVIEW, bool>> searchPredicate = Utility.And<GreenField.DAL.GF_PRICING_BASEVIEW>(searchPredicate1, searchPredicate2);
                        queryResultSet.AddRange(entity.GF_PRICING_BASEVIEW.Where(searchPredicate));
                    }                    

                    foreach (GreenField.DAL.GF_SELECTION_BASEVIEW item in dimSvcSelectionViewData)
                    {
                        if (item.INSTRUMENT_ID != null && item.INSTRUMENT_ID != string.Empty)
                        {
                            dimSvcPricingViewData = queryResultSet.Where(g => (g.INSTRUMENT_ID.ToUpper() == item.INSTRUMENT_ID.ToUpper())).ToList();                            
                            if (dimSvcPricingViewData != null && dimSvcPricingViewData.Count > 0)
                            {
                                List<FXCommodityData> resultView = new List<FXCommodityData>();
                                foreach (GreenField.DAL.GF_PRICING_BASEVIEW itemPricing in dimSvcPricingViewData)
                                {
                                    FXCommodityData data = new FXCommodityData();

                                    // Assigning Commodity ID from above list
                                    data.CommodityId = item.AIMS_COMMODITY_ID;    
                                    data.FromDate = itemPricing.FROMDATE;
                                    data.DailyClosingPrice = itemPricing.DAILY_CLOSING_PRICE;

                                    //Assigning Instrument Id from above list
                                    data.InstrumentId = item.INSTRUMENT_ID;         

                                    resultView.Add(data);
                                }
                                FXCommodityData calculatedData = new FXCommodityData();

                                //calling method to perform calculations
                                FXCommodityData calculatedDataForCommodity = FXCommodityCalculations.CalculateCommodityData(resultView, Date1DayBack.Date, DateLastYearEnd.Date, Date12MonthsAgo.Date, Date36MonthsAgo.Date);
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
            // use cache if available
            var fromCache = (List<FXCommodityData>)new DefaultCacheProvider().Get(CacheKeyNames.FXCommodityDataCache);
            if (fromCache != null)
                return fromCache;

            // otherwise fetch the data and cache it
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
            new DefaultCacheProvider().Set(CacheKeyNames.FXCommodityDataCache, result, Int32.Parse(ConfigurationManager.AppSettings["CacheTime"]));
            return result;
        }
    }
}
