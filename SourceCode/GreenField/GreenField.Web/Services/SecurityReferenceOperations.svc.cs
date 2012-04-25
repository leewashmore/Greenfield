﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GreenField.DAL;
using System.Data;
using GreenField.Web.DataContracts;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using GreenField.Web.Helpers;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using System.Drawing;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for Security Reference
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SecurityReferenceOperations
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
        /// retrieving the security data for security overview
        /// </summary>
        /// <returns>list of security overview data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<SecurityOverviewData> RetrieveSecurityReferenceData()
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_SECURITY_BASEVIEW> data = entity.GF_SECURITY_BASEVIEW.ToList();

                List<SecurityOverviewData> result = new List<SecurityOverviewData>();
                foreach (DimensionEntitiesService.GF_SECURITY_BASEVIEW record in data)
                {
                    result.Add(new SecurityOverviewData()
                    {
                        IssueName = record.ISSUE_NAME,
                        Ticker = record.TICKER,
                        Country = record.ISO_COUNTRY_CODE,
                        Sector = record.GICS_SECTOR_NAME,
                        Industry = record.GICS_INDUSTRY_NAME,
                        SubIndustry = record.GICS_SUB_INDUSTRY_NAME,
                        PrimaryAnalyst = record.ASHMOREEMM_PRIMARY_ANALYST,
                        Currency = record.TRADING_CURRENCY,
                        FiscalYearEnd = record.FISCAL_YEAR_END,
                        Website = record.WEBSITE,
                        Description = record.BLOOMBERG_DESCRIPTION
                    });
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
        /// retrieving the security data on ticker filter
        /// </summary>
        /// <returns>list of security overview data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public SecurityOverviewData RetrieveSecurityReferenceDataByTicker(string ticker)
        {
            try
            {
                DimensionEntitiesService.Entities entity = DimensionEntity;
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW.Where(o => o.TICKER == ticker).FirstOrDefault();

                if (data == null)
                    return new SecurityOverviewData();

                SecurityOverviewData result = new SecurityOverviewData()
                {
                    IssueName = data.ISSUE_NAME,
                    Ticker = data.TICKER,
                    Country = data.ISO_COUNTRY_CODE,
                    Sector = data.GICS_SECTOR_NAME,
                    Industry = data.GICS_INDUSTRY_NAME,
                    SubIndustry = data.GICS_SUB_INDUSTRY_NAME,
                    PrimaryAnalyst = data.ASHMOREEMM_PRIMARY_ANALYST,
                    Currency = data.TRADING_CURRENCY,
                    FiscalYearEnd = data.FISCAL_YEAR_END,
                    Website = data.WEBSITE,
                    Description = data.BLOOMBERG_DESCRIPTION
                };

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
        /// Retrieving the Pricing Reference Data for selected Entity.
        /// </summary>
        /// <param name="entityIdentifiers"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="totalReturnCheck"></param>
        /// <param name="frequencyDuration"></param>
        /// <param name="chartEntityTypes"></param>
        /// <returns>List of PricingReferenceData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<PricingReferenceData> RetrievePricingReferenceData(List<EntitySelectionData> entityIdentifiers, DateTime startDateTime, DateTime endDateTime, bool totalReturnCheck, string frequencyDuration)
        {
            try
            {
                decimal objAdjustedDollarPrice = 0;
                decimal objPreviousDailySpotFx = 0;
                decimal objIndexedPrice = 0;
                decimal objReturn = 0;

                decimal curPrice = 0;
                decimal curReturn = 0;
                decimal calculatedPrice = 0;
                string entityType = "";
                string entityInstrumentID = "";
                DateTime startDate = Convert.ToDateTime(startDateTime);
                DateTime endDate = Convert.ToDateTime(endDateTime);

                //List Containing the names of Securities/Commodities/Indexes to be added
                List<string> entityNames = (from p in entityIdentifiers
                                            select p.InstrumentID).ToList();

                List<PricingReferenceData> pricingDataResult = new List<PricingReferenceData>();

                DimensionEntitiesService.Entities entity = DimensionEntity;

                //Plotting a Single Line Chart
                #region SingleLineChart

                if (entityIdentifiers.Count() == 1)
                {
                    entityInstrumentID = Convert.ToString(entityIdentifiers[0].InstrumentID);
                    entityType = Convert.ToString(entityIdentifiers[0].Type);


                    DateTime webServiceStartTime = DateTime.Now;


                    List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData = entity.GF_PRICING_BASEVIEW
                        .Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >= startDate) && (r.FROMDATE < endDate))
                        .OrderByDescending(res => res.FROMDATE).ToList();

                    DateTime webServiceEndTime = DateTime.Now;



                    // Calcluating the values of curPrice,curReturn,calculatedPrice
                    if (dimensionServicePricingData.Count != 0)
                    {
                        bool dataNotFound = true;
                        while (dataNotFound)
                        {
                            if ((dimensionServicePricingData[0].DAILY_CLOSING_PRICE == null) || (dimensionServicePricingData[0].DAILY_PRICE_RETURN == null) || (dimensionServicePricingData[0].DAILY_GROSS_RETURN == null))
                            {
                                dimensionServicePricingData.RemoveAt(0);
                            }
                            else
                            {
                                dataNotFound = false;
                            }
                        }
                        curPrice = Convert.ToDecimal(dimensionServicePricingData[0].DAILY_CLOSING_PRICE);
                        curReturn = (totalReturnCheck) ? (Convert.ToDecimal(dimensionServicePricingData[0].DAILY_GROSS_RETURN)) : (Convert.ToDecimal(dimensionServicePricingData[0].DAILY_PRICE_RETURN));
                        calculatedPrice = curPrice;

                        foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                        {
                            if (pricingItem.DAILY_SPOT_FX == 0)
                                continue;
                            PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                            objPricingReferenceData.Type = pricingItem.TYPE;
                            objPricingReferenceData.Ticker = pricingItem.TICKER + ((totalReturnCheck) ? " (Total)" : "");
                            objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                            objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                            objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                            objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                            objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                            objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                            objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                            objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);

                            //Checking if the Item is the first item in the list
                            if ((pricingItem.INSTRUMENT_ID == dimensionServicePricingData[0].INSTRUMENT_ID) && (pricingItem.FROMDATE == dimensionServicePricingData[0].FROMDATE))
                            {
                                // if it is the first item in the list then simply save the value of calculated price
                                objPricingReferenceData.IndexedPrice = calculatedPrice;
                            }
                            else
                            {
                                //if it is not the first item then executing the logic.
                                calculatedPrice = (curPrice / ((curReturn / 100) + 1));
                                curPrice = calculatedPrice;
                                objPricingReferenceData.IndexedPrice = calculatedPrice;
                                curReturn = (totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN));
                            }
                            pricingDataResult.Add(objPricingReferenceData);
                        }
                    }
                }

                #endregion

                //Plotting a Multi-Line Comparison Chart
                #region MultiLineChart

                if (entityIdentifiers.Count() > 1)
                {
                    foreach (EntitySelectionData item in entityIdentifiers)
                    {
                        if (Convert.ToString(item.Type).ToUpper() == "SECURITY")
                        {
                            entityInstrumentID = Convert.ToString(item.InstrumentID);


                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate)).OrderByDescending(res => res.FROMDATE).ToList();


                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    if (pricingItem.DAILY_SPOT_FX == 0)
                                        continue;

                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER + ((totalReturnCheck) ? " (Total)" : "");
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);

                                    //Checking if the current object is first in the series
                                    if (pricingItem.FROMDATE == dimensionServicePricingData[0].FROMDATE)
                                    {
                                        objPricingReferenceData.AdjustedDollarPrice = (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / objPricingReferenceData.DailySpotFX);
                                    }
                                    else
                                    {
                                        objPricingReferenceData.AdjustedDollarPrice =
                                            objAdjustedDollarPrice / ((1 + (objReturn / 100)) * (Convert.ToDecimal(pricingItem.DAILY_SPOT_FX) / objPreviousDailySpotFx));
                                    }
                                    objAdjustedDollarPrice = objPricingReferenceData.AdjustedDollarPrice;
                                    objPreviousDailySpotFx = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                                    pricingDataResult.Add(objPricingReferenceData);
                                }
                            }
                        }

                        else if ((Convert.ToString(item.Type).ToUpper() == "COMMODITY") || ((Convert.ToString(item.Type).ToUpper() == "INDEX")) || ((Convert.ToString(item.Type).ToUpper() == "CURRENCY")))
                        {
                            entityInstrumentID = Convert.ToString(item.InstrumentID);
                            List<DimensionEntitiesService.GF_PRICING_BASEVIEW> dimensionServicePricingData =
                                entity.GF_PRICING_BASEVIEW.Where(r => (r.INSTRUMENT_ID == entityInstrumentID) && (r.FROMDATE >=
                                    startDate) && (r.FROMDATE <= endDate)).OrderBy(res => res.FROMDATE).ToList();

                            if (dimensionServicePricingData.Count != 0)
                            {
                                foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionServicePricingData)
                                {
                                    if (pricingItem.DAILY_SPOT_FX == 0)
                                        continue;
                                    PricingReferenceData objPricingReferenceData = new PricingReferenceData();
                                    objPricingReferenceData.Type = pricingItem.TYPE;
                                    objPricingReferenceData.Ticker = pricingItem.TICKER;
                                    objPricingReferenceData.IssueName = pricingItem.ISSUE_NAME;
                                    objPricingReferenceData.FromDate = pricingItem.FROMDATE;
                                    objPricingReferenceData.Volume = Convert.ToDecimal(pricingItem.VOLUME);
                                    objPricingReferenceData.DailySpotFX = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                                    objPricingReferenceData.InstrumentID = pricingItem.INSTRUMENT_ID;
                                    objPricingReferenceData.DailyClosingPrice = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                                    objPricingReferenceData.DailyPriceReturn = Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN);
                                    objPricingReferenceData.DailyGrossReturn = Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN);
                                    objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                                    objPricingReferenceData.AdjustedDollarPrice =
                                        (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / Convert.ToDecimal(pricingItem.DAILY_SPOT_FX));
                                    pricingDataResult.Add(objPricingReferenceData);
                                }
                            }
                        }

                        pricingDataResult = pricingDataResult.OrderBy(r => r.FromDate).ToList();

                        if ((pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID))).ToList().Count() > 0)
                        {
                            pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).FirstOrDefault().IndexedPrice = 100;
                        }

                        foreach (PricingReferenceData objPricingDataResult in pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).ToList())
                        {
                            if (objPricingDataResult.FromDate == pricingDataResult.Where(r => r.InstrumentID == Convert.ToString(item.InstrumentID)).OrderBy(r => r.FromDate).First().FromDate)
                            {
                                objAdjustedDollarPrice = objPricingDataResult.AdjustedDollarPrice;
                                objIndexedPrice = objPricingDataResult.IndexedPrice;
                            }
                            else
                            {
                                objPricingDataResult.IndexedPrice = (objPricingDataResult.AdjustedDollarPrice / objAdjustedDollarPrice) * objIndexedPrice;
                                objIndexedPrice = objPricingDataResult.IndexedPrice;
                                objAdjustedDollarPrice = objPricingDataResult.AdjustedDollarPrice;
                            }
                        }
                    }

                    foreach (PricingReferenceData item in pricingDataResult)
                    {
                        item.IndexedPrice = item.IndexedPrice - 100;
                    }
                }

                #endregion

                #region FilterDataAccordingToFrequency

                List<DateTime> endDates = new List<DateTime>();
                endDates = (from p in pricingDataResult
                            select p.FromDate).Distinct().ToList();

                List<DateTime> allEndDates = new List<DateTime>();

                allEndDates = FrequencyCalculator.RetrieveDatesAccordingToFrequency(endDates, startDateTime, endDateTime, frequencyDuration);

                List<PricingReferenceData> result = new List<PricingReferenceData>();

                if (frequencyDuration != "Daily")
                {
                    foreach (EntitySelectionData item in entityIdentifiers)
                    {
                        List<PricingReferenceData> individualSeriesResult = RetrievePricingDataAccordingFrequency(pricingDataResult.Where(r => r.InstrumentID == item.InstrumentID).OrderBy(r => r.FromDate).ToList(), allEndDates);
                        result.AddRange(individualSeriesResult);
                    }

                }
                else
                {
                    result = pricingDataResult;
                }
                #endregion

                return result;
            }

            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// retrieve list of securities for security selector
        /// </summary>
        /// <returns>list of entity selection data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<EntitySelectionData> RetrieveEntitySelectionData()
        {
            try
            {
                List<DimensionEntitiesService.GF_SELECTION_BASEVIEW> data = DimensionEntity.GF_SELECTION_BASEVIEW.ToList();
                List<EntitySelectionData> result = new List<EntitySelectionData>();
                if (data != null)
                {
                    foreach (DimensionEntitiesService.GF_SELECTION_BASEVIEW record in data)
                    {
                        result.Add(new EntitySelectionData()
                        {
                            SortOrder = EntityTypeSortOrder.GetSortOrder(record.TYPE),
                            ShortName = record.SHORT_NAME,
                            LongName = record.LONG_NAME,
                            InstrumentID = record.INSTRUMENT_ID,
                            Type = record.TYPE,
                            SecurityType = record.SECURITY_TYPE
                        });
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

        #region Pricing Chart Helper Methods

        /// <summary>
        /// Method to Retrieve Data Filtered according to Frequency.
        /// If for a particular day , data is not present then the data for 1-day before is considered.
        /// </summary>
        /// <param name="objPricingData"></param>
        /// <param name="objEndDates"></param>
        /// <returns></returns>
        private List<PricingReferenceData> RetrievePricingDataAccordingFrequency(List<PricingReferenceData> objPricingData, List<DateTime> objEndDates)
        {
            try
            {
                List<PricingReferenceData> resultFrequency = new List<PricingReferenceData>();
                List<DateTime> EndDates = objEndDates;

                foreach (DateTime item in EndDates)
                {
                    int i = 1;
                    bool dateObjectFound = true;

                    if (objPricingData.Any(r => r.FromDate.Date == item.Date))
                    {
                        resultFrequency.Add(objPricingData.Where(r => r.FromDate == item.Date).First());
                        dateObjectFound = false;
                        continue;
                    }
                    else
                    {
                        dateObjectFound = true;
                    }

                    while (dateObjectFound)
                    {
                        //Checking Data for 1-Day before
                        bool objDataFoundDec = objPricingData.Any(r => r.FromDate.Date == item.AddDays(-i).Date);
                        if (objDataFoundDec)
                        {
                            resultFrequency.Add(objPricingData.Where(r => r.FromDate.Date == item.AddDays(-i).Date).First());
                            dateObjectFound = false;
                        }
                        else
                        {
                            i++;
                            //If data for 30 days before doesn't exist, then move to next Date.
                            if (i > 30)
                            {
                                dateObjectFound = false;
                                continue;
                            }
                        }
                    }
                }
                return resultFrequency.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method to Retrieve Data Filtered according to Frequency.
        /// If for a particular day , data is not present then the data for 1-day before is considered.
        /// </summary>
        /// <param name="objUnrealizedGainLossData"></param>
        /// <param name="objEndDates"></param>
        /// <returns></returns>
        private List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(List<UnrealizedGainLossData> objUnrealizedGainLossData, List<DateTime> objEndDates)
        {
            try
            {
                List<UnrealizedGainLossData> resultFrequency = new List<UnrealizedGainLossData>();

                List<DateTime> EndDates = objEndDates;
                foreach (DateTime item in EndDates)
                {
                    int i = 1;
                    bool dateObjectFound = true;

                    if (objUnrealizedGainLossData.Any(r => r.FromDate.Date == item.Date))
                    {
                        resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate.Date == item.Date).First());
                        dateObjectFound = false;
                        continue;
                    }
                    else
                    {
                        dateObjectFound = true;
                    }

                    while (dateObjectFound)
                    {
                        bool objDataFoundDec = objUnrealizedGainLossData.Any(r => r.FromDate.Date == item.AddDays(-i).Date);
                        if (objDataFoundDec)
                        {
                            resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate.Date == item.AddDays(-i).Date).First());
                            dateObjectFound = false;
                        }
                        else
                        {
                            i++;
                            if (i > 30)
                            {
                                dateObjectFound = false;
                                continue;
                            }
                        }
                    }
                }
                return resultFrequency.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        #endregion

        #region Connection String Methods
        private string GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"ND1DDYYB6Q1\SQLEXPRESS";
            builder.InitialCatalog = "AshmoreEMMPOC";
            builder.UserID = "sa";
            builder.Password = "India@123";
            builder.MultipleActiveResultSets = true;
            return builder.ConnectionString;
        }

        private DataTable GetDataTable(string queryString)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = command;
                DataTable dataTable = new DataTable();
                dataTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

                try
                {
                    sqlDataAdapter.Fill(dataTable);
                    connection.Close();
                }
                catch (Exception)
                {

                    return null;
                }

                return dataTable;
            }
        }
        #endregion

        #region Unrealized Gain Loss Operation contract
        /// <summary>
        /// Retrieves the Theoretical Unrealized Gain Loss Data for selected Entity.
        /// </summary>
        /// <param name="entityIdentifier">Ticker for the security</param>
        /// <param name="startDateTime">Start Date of the Time Period that is selected</param>
        /// <param name="endDateTime">End Date of the Time Period that is selected</param>       
        /// <param name="frequencyInterval">Frequency Duration selected</param>       
        /// <returns>List of UnrealozedGainLossData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(EntitySelectionData entityIdentifier, DateTime startDateTime, DateTime endDateTime, string frequencyInterval)
        {
            try
            {
                List<UnrealizedGainLossData> result = new List<UnrealizedGainLossData>();

                if (entityIdentifier == null || entityIdentifier.ShortName == null || endDateTime < startDateTime)
                    return result;

                DimensionEntitiesService.Entities entity = DimensionEntity;
                List<DimensionEntitiesService.GF_PRICING_BASEVIEW> resultSet
                    = entity.GF_PRICING_BASEVIEW
                        .Where(record => (record.TICKER == entityIdentifier.ShortName))
                        .OrderByDescending(record => record.FROMDATE)
                        .ToList();


                int noOfRows = resultSet.Count();

                if (noOfRows < 90)
                    return result;

                //Calculating the Adjusted price for a security and storing it in the list.
                List<UnrealizedGainLossData> adjustedPriceResult = UnrealizedGainLossCalculations.CalculateAdjustedPrice(resultSet);

                //Calculating the Moving Average for a security and storing it in the list.
                List<UnrealizedGainLossData> movingAverageResult = UnrealizedGainLossCalculations.CalculateMovingAverage(adjustedPriceResult);

                //Calculating the Ninety Day Weight for a security and storing it in the list.
                List<UnrealizedGainLossData> ninetyDayWtResult = UnrealizedGainLossCalculations.CalculateNinetyDayWtAvg(movingAverageResult);

                //Calculating the Cost for a security and storing it in the list.
                List<UnrealizedGainLossData> costResult = UnrealizedGainLossCalculations.CalculateCost(ninetyDayWtResult);

                //Calculating the Weighted Average Cost for a security and storing it in the list.
                List<UnrealizedGainLossData> wtAvgCostResult = UnrealizedGainLossCalculations.CalculateWtAvgCost(costResult);

                //Calculating the Unrealized Gain loss for a security and storing it in the list.
                List<UnrealizedGainLossData> unrealizedGainLossResult = UnrealizedGainLossCalculations.CalculateUnrealizedGainLoss(wtAvgCostResult);

                //Filtering the list according to the time period selected
                List<UnrealizedGainLossData> timeFilteredUnrealizedGainLossResult
                    = unrealizedGainLossResult
                        .Where(record => (record.FromDate >= startDateTime) && (record.FromDate < endDateTime))
                        .ToList();

                //Filtering the list according to the frequency selected.
                List<DateTime> EndDates
                    = timeFilteredUnrealizedGainLossResult
                        .Select(record => record.FromDate)
                        .ToList();

                //Calculating the date points based on Data Frequency
                List<DateTime> allEndDates = FrequencyCalculator.RetrieveDatesAccordingToFrequency(EndDates, startDateTime, endDateTime, frequencyInterval);

                result = UnrealizedGainLossCalculations.RetrieveUnrealizedGainLossData(timeFilteredUnrealizedGainLossResult, allEndDates);

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion




    }
}
