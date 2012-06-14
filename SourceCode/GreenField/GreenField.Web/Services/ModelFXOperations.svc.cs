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
                //foreach (var i in myList)
                //{
                //    MacroDatabaseKeyAnnualReportData entry = new MacroDatabaseKeyAnnualReportData();
                //    entry.CATEGORY_NAME = i.CATEGORY_NAME;
                //    entry.CountryName = i.CountryName;
                //    entry.Description = i.Description;
                //    entry.DisplayType = i.DisplayType;
                //    entry.FiveYEAR_Avg = i.FiveYEAR_Avg;
                //    entry.YEAR_1987 = i.YEAR_1987;
                //    entry.YEAR_1988 = i.YEAR_1988;
                //    entry.YEAR_1989 = i.YEAR_1989;
                //    entry.YEAR_1990 = i.YEAR_1990;
                //    entry.YEAR_1991 = i.YEAR_1991;
                //    entry.YEAR_1992 = i.YEAR_1992;
                //    entry.YEAR_1993 = i.YEAR_1993;
                //    entry.YEAR_1994 = i.YEAR_1994;
                //    entry.YEAR_1995 = i.YEAR_1995;
                //    entry.YEAR_1996 = i.YEAR_1996;
                //    entry.YEAR_1997 = i.YEAR_1997;
                //    entry.YEAR_1998 = i.YEAR_1998;
                //    entry.YEAR_1999 = i.YEAR_1999;
                //    entry.YEAR_2000 = i.YEAR_2000;
                //    entry.YEAR_2001 = i.YEAR_2001;
                //    entry.YEAR_2002 = i.YEAR_2002;
                //    entry.YEAR_2003 = i.YEAR_2003;
                //    entry.YEAR_2004 = i.YEAR_2004;
                //    entry.YEAR_2005 = i.YEAR_2005;
                //    entry.YEAR_2006 = i.YEAR_2006;
                //    entry.YEAR_2007 = i.YEAR_2007;
                //    entry.YEAR_2008 = i.YEAR_2008;
                //    entry.YEAR_2009 = i.YEAR_2009;
                //    entry.YEAR_2010 = i.YEAR_2010;
                //    entry.YEAR_2011 = i.YEAR_2011;
                //    entry.YEAR_2012 = i.YEAR_2012;
                //    entry.YEAR_2013 = i.YEAR_2013;
                //    entry.YEAR_2014 = i.YEAR_2014;
                //    entry.YEAR_2015 = i.YEAR_2015;
                //    entry.YEAR_2016 = i.YEAR_2016;
                //    entry.YEAR_2017 = i.YEAR_2017;
                //    entry.YEAR_2018 = i.YEAR_2018;
                //    entry.YEAR_2019 = i.YEAR_2019;
                //    entry.YEAR_2020 = i.YEAR_2020;
                //    entry.YEAR_2021 = i.YEAR_2021;
                //    entry.YEAR_2022 = i.YEAR_2022;
                //    entry.YEAR_2023 = i.YEAR_2023;
                //    entry.YEAR_2024 = i.YEAR_2024;
                //    entry.YEAR_2025 = i.YEAR_2025;                  
                //    result.Add(entry);
                //}
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
        public List<MacroDatabaseKeyAnnualReportData> RetrieveMacroDatabaseKeyAnnualReportDataEMSummary(String countryNameVal)
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
                result = research.ExecuteStoreQuery<MacroDatabaseKeyAnnualReportData>("exec RetrieveEMSummaryDataReportPerCountry @country={0}", countryNameVal).ToList();
                if (result.Count == 0 || result == null)
                    return result;
                //foreach (var i in myList)
                //{
                //    MacroDatabaseKeyAnnualReportData entry = new MacroDatabaseKeyAnnualReportData();
                //    entry.CATEGORY_NAME = i.CATEGORY_NAME;
                //    entry.CountryName = i.CountryName;
                //    entry.Description = i.Description;
                //    entry.DisplayType = i.DisplayType;
                //    entry.FiveYEAR_Avg = i.FiveYEAR_Avg;
                //    entry.YEAR_1987 = i.YEAR_1987;
                //    entry.YEAR_1988 = i.YEAR_1988;
                //    entry.YEAR_1989 = i.YEAR_1989;
                //    entry.YEAR_1990 = i.YEAR_1990;
                //    entry.YEAR_1991 = i.YEAR_1991;
                //    entry.YEAR_1992 = i.YEAR_1992;
                //    entry.YEAR_1993 = i.YEAR_1993;
                //    entry.YEAR_1994 = i.YEAR_1994;
                //    entry.YEAR_1995 = i.YEAR_1995;
                //    entry.YEAR_1996 = i.YEAR_1996;
                //    entry.YEAR_1997 = i.YEAR_1997;
                //    entry.YEAR_1998 = i.YEAR_1998;
                //    entry.YEAR_1999 = i.YEAR_1999;
                //    entry.YEAR_2000 = i.YEAR_2000;
                //    entry.YEAR_2001 = i.YEAR_2001;
                //    entry.YEAR_2002 = i.YEAR_2002;
                //    entry.YEAR_2003 = i.YEAR_2003;
                //    entry.YEAR_2004 = i.YEAR_2004;
                //    entry.YEAR_2005 = i.YEAR_2005;
                //    entry.YEAR_2006 = i.YEAR_2006;
                //    entry.YEAR_2007 = i.YEAR_2007;
                //    entry.YEAR_2008 = i.YEAR_2008;
                //    entry.YEAR_2009 = i.YEAR_2009;
                //    entry.YEAR_2010 = i.YEAR_2010;
                //    entry.YEAR_2011 = i.YEAR_2011;
                //    entry.YEAR_2012 = i.YEAR_2012;
                //    entry.YEAR_2013 = i.YEAR_2013;
                //    entry.YEAR_2014 = i.YEAR_2014;
                //    entry.YEAR_2015 = i.YEAR_2015;
                //    entry.YEAR_2016 = i.YEAR_2016;
                //    entry.YEAR_2017 = i.YEAR_2017;
                //    entry.YEAR_2018 = i.YEAR_2018;
                //    entry.YEAR_2019 = i.YEAR_2019;
                //    entry.YEAR_2020 = i.YEAR_2020;
                //    entry.YEAR_2021 = i.YEAR_2021;
                //    entry.YEAR_2022 = i.YEAR_2022;
                //    entry.YEAR_2023 = i.YEAR_2023;
                //    entry.YEAR_2024 = i.YEAR_2024;
                //    entry.YEAR_2025 = i.YEAR_2025;                  
                //    result.Add(entry);
                //}
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
        /// Gets Commodity Data
        /// </summary>
        /// <param name="countryNameVal"></param>
        /// <returns>Commodity Result</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        //TODO Seema: Add Input Param Country
        public List<FXCommodityData> RetrieveCommodityData(string selectedCommodityID)
        {
            try
            {
                //bool isServiceUp;
                //isServiceUp = CheckServiceAvailability.ServiceAvailability();

                //if (!isServiceUp)
                //    throw new Exception();

                List<CommodityResult> resultDB = new List<CommodityResult>();
                List<FXCommodityData> resultVW = new List<FXCommodityData>();
                List<FXCommodityData> result = new List<FXCommodityData>();
                ResearchEntities research = new ResearchEntities();

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimSvcPricingViewData = null;

                if (entity.GF_PRICING_BASEVIEW == null && entity.GF_PRICING_BASEVIEW.Count() == 0)
                    return null;

                //TODO Seema: Input Parameter has to be added - Country
                //Retrieving data from database
                //TODO Seema: Remove HardCoded Value below
                //selectedCommodityID = "ZINC";
                resultDB = research.ExecuteStoreQuery<CommodityResult>("exec GetCOMMODITY_FORECASTS @commodityID={0}", selectedCommodityID).ToList();

                
                //TODO Seema: Input Parameter has to be added - Country
                //Retrieving Data from WCF svc
                //dimSvcPricingViewData = entity.GF_PRICING_BASEVIEW.ToList();
                var res = from p in entity.GF_PRICING_BASEVIEW
                          where p.INSTRUMENT_ID == Convert.ToString(selectedCommodityID).ToUpper()
                          select p;
                dimSvcPricingViewData = res.ToList();
                foreach(GF_PRICING_BASEVIEW item in dimSvcPricingViewData)
                {
                    FXCommodityData data = new FXCommodityData();
                    data.CommodityID = item.INSTRUMENT_ID;
                    data.FromDate = item.FROMDATE;
                    resultVW.Add(data);
                }
                FXCommodityData calculatedData = FXCommodityCalculations.CalculateCommodityData(resultVW);
                for (int _index = 0; _index < resultDB.Count; _index++)
                {
                    FXCommodityData commodityData = new FXCommodityData();
                    commodityData.CommodityID = resultDB[_index].COMMODITY_ID;
                    commodityData.CurrentYearEnd = Convert.ToDecimal(resultDB[_index].CURRENT_YEAR_END);
                    //commodityData.LastUpdate = Convert.ToDateTime(resultDB[_index].LASTUPDATE);
                    commodityData.LongTerm = Convert.ToDecimal(resultDB[_index].LONG_TERM);
                    commodityData.NextYearEnd = Convert.ToDecimal(resultDB[_index].NEXT_YEAR_END);
                    if (commodityData.CommodityID == calculatedData.CommodityID)
                    {
                        commodityData.YTD = calculatedData.YTD;
                        commodityData.Year1 = calculatedData.Year1;
                        commodityData.Year3 = calculatedData.Year3;
                    }
                    result.Add(commodityData);
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
                data.CommodityID = commodityData[i].COMMODITY_ID;
                result.Add(data);
            }
            return result;
        }
    }
}
