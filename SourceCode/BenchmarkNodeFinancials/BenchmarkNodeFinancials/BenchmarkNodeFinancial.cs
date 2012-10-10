﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using BenchmarkNodeFinancials.DimensionServiceReference;

namespace BenchmarkNodeFinancials
{
    class BenchmarkNodeFinancial
    {
        #region Fields
        /// <summary>
        ///AIMSDataEntity object for connection to the database
        /// </summary>
        private static AIMSDataEntity entity;
        /// <summary>
        /// DimensionServiceReference.Entities object for connection to the views
        /// </summary>
        private static DimensionServiceReference.Entities dimensionEntity;
        /// <summary>
        /// log4net required for logging
        /// </summary>
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Methods
        /// <summary>
        /// Retrieve data for Market data gadget
        /// </summary>
        /// <param name="lastBusinessDate">last business date available in the view </param>
        public void RetrieveData(DateTime lastBusinessDate)
        {
            try
            {
                dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
                List<String> benchmarkIds = new List<string>();
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();
                if (!isServiceUp)
                {
                    throw new Exception("Services are not available");
                }
                entity = new AIMSDataEntity();
                bool exists = File.Exists(@"BenchmarkIDs.xml");
                if (exists)
                {
                    var doc = XDocument.Load(@"BenchmarkIDs.xml");
                    benchmarkIds = doc.Descendants("BenchmarkId")
                        .Select(a => a.Value)
                        .ToList();
                }
                foreach (String benId in benchmarkIds)
                {
                    List<GF_BENCHMARK_HOLDINGS> dataBenchmarkHoldings = new List<GF_BENCHMARK_HOLDINGS>();
                    dataBenchmarkHoldings = dimensionEntity.GF_BENCHMARK_HOLDINGS.Where(record => record.BENCHMARK_ID == benId
                                                             && record.PORTFOLIO_DATE == lastBusinessDate
                                                              && record.BENCHMARK_WEIGHT > 0).ToList();

                    var benchData = dataBenchmarkHoldings != null ? (from p in dataBenchmarkHoldings
                                                                     select new
                                                                     {
                                                                         IssuerId = p.ISSUER_ID,
                                                                         AsecShortName = p.ASEC_SEC_SHORT_NAME,
                                                                         IssueName = p.ISSUE_NAME,
                                                                         Region = p.ASHEMM_PROP_REGION_CODE,
                                                                         Country = p.ISO_COUNTRY_CODE,
                                                                         Sector = p.GICS_SECTOR,
                                                                         Industry = p.GICS_INDUSTRY,
                                                                         Weight = p.BENCHMARK_WEIGHT
                                                                     }).Distinct() : null;

                    List<String> asecShortNames = benchData != null ? (from p in benchData
                                                                       select p.AsecShortName).ToList() : null;
                    //retrieve security Id's              
                    List<SecurityData> securityData = RetrieveSecurityIds(asecShortNames);
                    List<String> distinctSecurityId = securityData.Select(record => record.SecurityId).ToList();
                    List<String> distinctIssuerId = securityData.Select(record => record.IssuerId).ToList();

                    String _securityIds = StringBuilder(distinctSecurityId);
                    String _issuerIds = StringBuilder(distinctIssuerId);     

                    List<PeriodFinancialForwardRatios> periodFinancialData = new List<PeriodFinancialForwardRatios>();
                    periodFinancialData = entity.usp_GetDataForBenchmarkNodefinancials(_issuerIds, _securityIds).ToList();

                    List<BenchmarkNodeFinancialsData> forwardRatioData = new List<BenchmarkNodeFinancialsData>();
                    forwardRatioData = FillBenchmarkNodeFinancials(periodFinancialData, securityData, dataBenchmarkHoldings, benId);

                    List<PeriodFinancialPeriodRatios> periodFinancialDataPeriodRatios = new List<PeriodFinancialPeriodRatios>();
                    periodFinancialDataPeriodRatios = entity.usp_GetDataForBenchNodefinPeriodYear(_issuerIds, _securityIds).ToList();

                    List<BenchmarkNodeFinancialsData> bothRatiosData = new List<BenchmarkNodeFinancialsData>();
                    bothRatiosData = FillBenchmarkNodeFinancialsPeriodData(periodFinancialDataPeriodRatios, securityData, dataBenchmarkHoldings, 
                        benId, forwardRatioData);

                    //grouping the data and calculating the harmonic mean for insertion into the Benchmark_Node_Financials table
                    List<GroupedBenchmarkNodeData> groupedFinalData = new List<GroupedBenchmarkNodeData>();
                    groupedFinalData = GroupBenchmarkData(bothRatiosData);

                    //creation of an Xml for inserting data into the Benchmark_Node_Financials table
                    CreateXMLInsertData(groupedFinalData, entity, benId);
                }
            }
            catch (Exception ex)
            {
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }

        /// <summary>
        /// Retrieve security Id's according to asecShortNames from GF_SECURITY_BASEVIEW
        /// </summary>
        /// <param name="asecShortNames"></param>
        /// <returns></returns>
        private List<SecurityData> RetrieveSecurityIds(List<String> asecShortNames)
        {
            List<SecurityData> secData = new List<SecurityData>();
            try
            {
                foreach (String asec in asecShortNames)
                {
                    GF_SECURITY_BASEVIEW item = (dimensionEntity.GF_SECURITY_BASEVIEW.Where(record => record.ASEC_SEC_SHORT_NAME == asec).
                        FirstOrDefault());
                    if (item != null)
                    {
                        secData.Add(new SecurityData()
                        {
                            SecurityId = item.SECURITY_ID.ToString(),
                            IssuerId = item.ISSUER_ID,
                            IssueName = item.ISSUE_NAME,
                            AsecShortName = item.ASEC_SEC_SHORT_NAME
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            return secData;
        }

        /// <summary>
        /// String builder that adds ' between Id's
        /// </summary>
        /// <param name="param">String of Id's</param>
        /// <returns></returns>
        private string StringBuilder(List<String> param)
        {
            StringBuilder var = new StringBuilder();
            int check = 1;
            foreach (String item in param)
            {
                check = 0;
                var.Append(",'" + item + "'");
            }
            var = check == 0 ? var.Remove(0, 1) : null;
            string result = var == null ? null : var.ToString();
            return result;
        }

        /// <summary>
        /// Fills Benchmark Node finacial data for forward ratios
        /// </summary>
        /// <param name="periodFinancialData">Forward ratios data returned form the stored procedure</param>
        /// <param name="securityData">Security data from GF_SECURITY_BASEVIEW</param>
        /// <param name="dataBenchmarkHoldings">data from benchmark holdings view</param>
        /// <param name="benId">benchmark Id</param>
        /// <returns></returns>
        private List<BenchmarkNodeFinancialsData> FillBenchmarkNodeFinancials(List<PeriodFinancialForwardRatios> periodFinancialData, List<SecurityData> securityData, List<GF_BENCHMARK_HOLDINGS> dataBenchmarkHoldings, String benId)
        {
            List<BenchmarkNodeFinancialsData> forwardRatioData = new List<BenchmarkNodeFinancialsData>();
            try
            {
                if (periodFinancialData != null && periodFinancialData.Count() > 0)
                {
                    foreach (int dataId in periodFinancialData.Select(t => t.DataID).Distinct())
                    {
                        List<String> dinstinctIssuerIds = periodFinancialData.Where(t => t.DataID == dataId)
                            .Select(t => t.IssuerID).Distinct().ToList();
                        List<String> dinstinctSecurityIds = periodFinancialData.Where(t => t.DataID == dataId)
                            .Select(t => t.SecurityID).Distinct().ToList();
                        foreach (String s in dinstinctIssuerIds)
                        {
                            foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmarkHoldings.Where(t => t.ISSUER_ID == s).ToList())
                            {
                                BenchmarkNodeFinancialsData obj = new BenchmarkNodeFinancialsData();
                                obj.BenchmarkName = benId;
                                obj.IssuerId = s;
                                obj.SecurityId = securityData.Where(t => t.IssuerId == s).Select(t => t.SecurityId).FirstOrDefault();
                                obj.IssueName = securityData.Where(t => t.IssuerId == s).Select(t => t.IssueName).FirstOrDefault();
                                obj.AsecShortName = row.ASEC_SEC_SHORT_NAME;
                                obj.Region = row.ASHEMM_PROP_REGION_CODE;
                                obj.Country = row.ISO_COUNTRY_CODE;
                                obj.Sector = row.GICS_SECTOR;
                                obj.industry = row.GICS_INDUSTRY;
                                obj.DataId = dataId;
                                obj.PeriodType = "C";
                                obj.PeriodYear = 0;
                                obj.Currency = "USD";
                                obj.Amount = periodFinancialData.Where(t => t.IssuerID == s && t.DataID == dataId)
                                    .Select(t => t.Amount).FirstOrDefault();
                                obj.InvAmount = obj.Amount != 0 ? 1 / obj.Amount : 0;
                                obj.BenWeight = row.BENCHMARK_WEIGHT;
                                obj.TypeNode = "Forw";
                                forwardRatioData.Add(obj);
                            }
                        }
                        foreach (String s in dinstinctSecurityIds)
                        {
                            foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmarkHoldings
                                .Where(t => t.ASEC_SEC_SHORT_NAME == (securityData.Where(p => p.SecurityId == s).Select(p => p.AsecShortName)
                                    .FirstOrDefault())).ToList())
                            {
                                BenchmarkNodeFinancialsData obj = new BenchmarkNodeFinancialsData();
                                obj.BenchmarkName = benId;
                                obj.IssuerId = securityData.Where(t => t.SecurityId == s).Select(t => t.IssuerId).FirstOrDefault();
                                obj.SecurityId = s;
                                obj.IssueName = securityData.Where(t => t.SecurityId == s).Select(t => t.IssueName).FirstOrDefault();
                                obj.AsecShortName = row.ASEC_SEC_SHORT_NAME;
                                obj.Region = row.ASHEMM_PROP_REGION_CODE;
                                obj.Country = row.ISO_COUNTRY_CODE;
                                obj.Sector = row.GICS_SECTOR;
                                obj.industry = row.GICS_INDUSTRY;
                                obj.DataId = dataId;
                                obj.PeriodType = "C";
                                obj.PeriodYear = 0;
                                obj.Currency = "USD";
                                obj.Amount = periodFinancialData.Where(t => t.SecurityID == s && t.DataID == dataId)
                                    .Select(t => t.Amount).FirstOrDefault();
                                obj.InvAmount = obj.Amount != 0 ? 1 / obj.Amount : 0;
                                obj.BenWeight = row.BENCHMARK_WEIGHT;
                                obj.TypeNode = "Forw";
                                forwardRatioData.Add(obj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            return forwardRatioData;
        }      

        /// <summary>
        /// Fills Benchmark Node finacial data for Period ratios
        /// </summary>
        /// <param name="periodFinancialRatioData">Period ratios data returned form the stored procedure</param>
        /// <param name="securityData">Security data from GF_SECURITY_BASEVIEW</param>
        /// <param name="dataBenchmarkHoldings">data from benchmark holdings view</param>
        /// <param name="benId">benchmark Id</param>
        /// <param name="periodRatiosData"></param>
        /// <returns></returns>
        private List<BenchmarkNodeFinancialsData> FillBenchmarkNodeFinancialsPeriodData(List<PeriodFinancialPeriodRatios> periodFinancialRatioData, 
            List<SecurityData> securityData, List<GF_BENCHMARK_HOLDINGS> dataBenchmarkHoldings, String benId,
            List<BenchmarkNodeFinancialsData> periodRatiosData)
        {
            try
            {
                if (periodFinancialRatioData != null && periodFinancialRatioData.Count() > 0)
                {
                    foreach (int dataId in periodFinancialRatioData.Select(t => t.DataID).Distinct())
                    {
                        foreach (int perYear in periodFinancialRatioData.Where(t => t.DataID == dataId).Select(t => t.PeriodYear).Distinct())
                        {
                            List<String> dinstinctIssuerIds = periodFinancialRatioData.Where(t => t.DataID == dataId && t.PeriodYear == perYear)
                                .Select(t => t.IssuerID).Distinct().ToList();
                            List<String> dinstinctSecurityIds = periodFinancialRatioData.Where(t => t.DataID == dataId && t.PeriodYear == perYear)
                                .Select(t => t.SecurityID).Distinct().ToList();

                            foreach (String s in dinstinctIssuerIds)
                            {
                                foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmarkHoldings.Where(t => t.ISSUER_ID == s).ToList())
                                {
                                    BenchmarkNodeFinancialsData obj = new BenchmarkNodeFinancialsData();
                                    obj.BenchmarkName = benId;
                                    obj.IssuerId = s;
                                    obj.SecurityId = securityData.Where(t => t.IssuerId == s).Select(t => t.SecurityId).FirstOrDefault();
                                    obj.IssueName = securityData.Where(t => t.IssuerId == s).Select(t => t.IssueName).FirstOrDefault();
                                    obj.AsecShortName = row.ASEC_SEC_SHORT_NAME;
                                    obj.Region = row.ASHEMM_PROP_REGION_CODE;
                                    obj.Country = row.ISO_COUNTRY_CODE;
                                    obj.Sector = row.GICS_SECTOR;
                                    obj.industry = row.GICS_INDUSTRY;
                                    obj.DataId = dataId;
                                    obj.PeriodType = "A";
                                    obj.PeriodYear = perYear;
                                    obj.Currency = "USD";
                                    obj.Amount = periodFinancialRatioData.Where(t => t.IssuerID == s && t.DataID == dataId && 
                                        t.PeriodYear == perYear).Select(t => t.Amount).FirstOrDefault();
                                    obj.InvAmount = obj.Amount != 0 ? 1 / obj.Amount : 0;
                                    obj.BenWeight = row.BENCHMARK_WEIGHT;
                                    obj.TypeNode = "Per";
                                    periodRatiosData.Add(obj);
                                }
                            }

                            foreach (String s in dinstinctSecurityIds)
                            {
                                foreach (GF_BENCHMARK_HOLDINGS row in dataBenchmarkHoldings
                                    .Where(t => t.ASEC_SEC_SHORT_NAME == (securityData.Where(p => p.SecurityId == s).Select(p => p.AsecShortName)
                                        .FirstOrDefault())).ToList())
                                {
                                    BenchmarkNodeFinancialsData obj = new BenchmarkNodeFinancialsData();
                                    obj.BenchmarkName = benId;
                                    obj.IssuerId = securityData.Where(t => t.SecurityId == s).Select(t => t.IssuerId).FirstOrDefault();
                                    obj.SecurityId = s;
                                    obj.IssueName = securityData.Where(t => t.SecurityId == s).Select(t => t.IssueName).FirstOrDefault();
                                    obj.AsecShortName = row.ASEC_SEC_SHORT_NAME;
                                    obj.Region = row.ASHEMM_PROP_REGION_CODE;
                                    obj.Country = row.ISO_COUNTRY_CODE;
                                    obj.Sector = row.GICS_SECTOR;
                                    obj.industry = row.GICS_INDUSTRY;
                                    obj.DataId = dataId;
                                    obj.PeriodType = "A";
                                    obj.PeriodYear = perYear;
                                    obj.Currency = "USD";
                                    obj.Amount = periodFinancialRatioData.Where(t => t.SecurityID == s && t.DataID == dataId && 
                                        t.PeriodYear == perYear).Select(t => t.Amount).FirstOrDefault();
                                    obj.InvAmount = obj.Amount != 0 ? 1 / obj.Amount : 0;
                                    obj.BenWeight = row.BENCHMARK_WEIGHT;
                                    obj.TypeNode = "Per";
                                    periodRatiosData.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            return periodRatiosData;
        }

        /// <summary>
        /// Grouping the data returned from the Benchmark hodings
        /// </summary>
        /// <param name="bothRatiosData">data combined for both ratios into this list</param>
        /// <returns></returns>
        private List<GroupedBenchmarkNodeData> GroupBenchmarkData(List<BenchmarkNodeFinancialsData> bothRatiosData)
        {
            List<GroupedBenchmarkNodeData> groupedForData = new List<GroupedBenchmarkNodeData>();
            try
            {
                if (bothRatiosData != null && bothRatiosData.Count() > 0)
                {
                    foreach (int d in bothRatiosData.Select(t => t.DataId).Distinct().ToList())
                    {
                        foreach (int perYear in bothRatiosData.Where(t => t.DataId == d).Select(t => t.PeriodYear).Distinct().ToList())
                        {
                            #region Grouping by Region
                            //group by region 
                            foreach (String reg in bothRatiosData.Where(t => t.DataId == d && t.PeriodYear == perYear).
                                Select(t => t.Region).Distinct().ToList())
                            {
                                GroupedBenchmarkNodeData obj = new GroupedBenchmarkNodeData();
                                List<BenchmarkNodeFinancialsData> specificRegionData = new List<BenchmarkNodeFinancialsData>();
                                specificRegionData = bothRatiosData.Where(t => t.DataId == d && t.Region == reg && t.PeriodYear == perYear).
                                    ToList();
                                obj.NodeName1 = "Region";
                                obj.NodeID1 = reg;
                                obj.NodeName2 = String.Empty;
                                obj.NodeID2 = String.Empty;
                                obj.BenchmarkID = specificRegionData.Select(t => t.BenchmarkName).FirstOrDefault();
                                obj.DataID = d;
                                obj.PeriodType = specificRegionData.Select(t => t.PeriodType).FirstOrDefault();
                                obj.PeriodYear = specificRegionData.Select(t => t.PeriodYear).FirstOrDefault();
                                obj.Currency = specificRegionData.Select(t => t.Currency).FirstOrDefault();
                                obj.UpdateDate = DateTime.Today;
                                Decimal harmonicMean = CalculateHarmonicMeanBenchmark(specificRegionData);
                                obj.Amount = harmonicMean;
                                groupedForData.Add(obj);

                                //group by region and sector
                                foreach (String sec in bothRatiosData.Where(t => t.DataId == d && t.Region == reg && t.PeriodYear == perYear).
                                    Select(t => t.Sector).Distinct().ToList())
                                {
                                    GroupedBenchmarkNodeData objInner = new GroupedBenchmarkNodeData();
                                    List<BenchmarkNodeFinancialsData> specificRegionSectorData = new List<BenchmarkNodeFinancialsData>();
                                    specificRegionSectorData = bothRatiosData.Where(t => t.DataId == d && t.Region == reg && t.Sector == sec && 
                                        t.PeriodYear == perYear).ToList();
                                    objInner.NodeName1 = "Region";
                                    objInner.NodeID1 = reg;
                                    objInner.NodeName2 = "Sector";
                                    objInner.NodeID2 = sec;
                                    objInner.BenchmarkID = specificRegionSectorData.Select(t => t.BenchmarkName).FirstOrDefault();
                                    objInner.DataID = d;
                                    objInner.PeriodType = specificRegionSectorData.Select(t => t.PeriodType).FirstOrDefault();
                                    objInner.PeriodYear = specificRegionSectorData.Select(t => t.PeriodYear).FirstOrDefault();
                                    objInner.Currency = specificRegionSectorData.Select(t => t.Currency).FirstOrDefault();
                                    objInner.UpdateDate = DateTime.Today;
                                    Decimal harmonicMeanSec = CalculateHarmonicMeanBenchmark(specificRegionSectorData);
                                    objInner.Amount = harmonicMeanSec;
                                    groupedForData.Add(objInner);
                                }

                                //group by region and industry
                                foreach (String indus in bothRatiosData.Where(t => t.DataId == d && t.Region == reg && t.PeriodYear == perYear).
                                    Select(t => t.industry).Distinct().ToList())
                                {
                                    GroupedBenchmarkNodeData objInner = new GroupedBenchmarkNodeData();
                                    List<BenchmarkNodeFinancialsData> specificRegionIndusData = new List<BenchmarkNodeFinancialsData>();
                                    specificRegionIndusData = bothRatiosData.Where(t => t.DataId == d && t.Region == reg && t.industry == indus && 
                                        t.PeriodYear == perYear).ToList();
                                    objInner.NodeName1 = "Region";
                                    objInner.NodeID1 = reg;
                                    objInner.NodeName2 = "Industry";
                                    objInner.NodeID2 = indus;
                                    objInner.BenchmarkID = specificRegionIndusData.Select(t => t.BenchmarkName).FirstOrDefault();
                                    objInner.DataID = d;
                                    objInner.PeriodType = specificRegionIndusData.Select(t => t.PeriodType).FirstOrDefault();
                                    objInner.PeriodYear = specificRegionIndusData.Select(t => t.PeriodYear).FirstOrDefault();
                                    objInner.Currency = specificRegionIndusData.Select(t => t.Currency).FirstOrDefault();
                                    objInner.UpdateDate = DateTime.Today;
                                    Decimal harmonicMeanSec = CalculateHarmonicMeanBenchmark(specificRegionIndusData);
                                    objInner.Amount = harmonicMeanSec;
                                    groupedForData.Add(objInner);
                                }
                            }
                            #endregion

                            #region Grouping by Country
                            //group by country 
                            foreach (String cou in bothRatiosData.Where(t => t.DataId == d && t.PeriodYear == perYear).Select(t => t.Country)
                                .Distinct().ToList())
                            {
                                GroupedBenchmarkNodeData obj = new GroupedBenchmarkNodeData();
                                List<BenchmarkNodeFinancialsData> specificCountryData = new List<BenchmarkNodeFinancialsData>();
                                specificCountryData = bothRatiosData.Where(t => t.DataId == d && t.Country == cou && t.PeriodYear == perYear)
                                    .ToList();
                                obj.NodeName1 = "Country";
                                obj.NodeID1 = cou;
                                obj.NodeName2 = String.Empty;
                                obj.NodeID2 = String.Empty;
                                obj.BenchmarkID = specificCountryData.Select(t => t.BenchmarkName).FirstOrDefault();
                                obj.DataID = d;
                                obj.PeriodType = specificCountryData.Select(t => t.PeriodType).FirstOrDefault();
                                obj.PeriodYear = specificCountryData.Select(t => t.PeriodYear).FirstOrDefault();
                                obj.Currency = specificCountryData.Select(t => t.Currency).FirstOrDefault();
                                obj.UpdateDate = DateTime.Today;
                                Decimal harmonicMean = CalculateHarmonicMeanBenchmark(specificCountryData);
                                obj.Amount = harmonicMean;
                                groupedForData.Add(obj);

                                //group by country and sector
                                foreach (String sec in bothRatiosData.Where(t => t.DataId == d && t.Country == cou && t.PeriodYear == perYear)
                                    .Select(t => t.Sector).Distinct().ToList())
                                {
                                    GroupedBenchmarkNodeData objInner = new GroupedBenchmarkNodeData();
                                    List<BenchmarkNodeFinancialsData> specificCountrySectorData = new List<BenchmarkNodeFinancialsData>();
                                    specificCountrySectorData = bothRatiosData.Where(t => t.DataId == d && t.Country == cou && t.Sector == sec && 
                                        t.PeriodYear == perYear).ToList();
                                    objInner.NodeName1 = "Country";
                                    objInner.NodeID1 = cou;
                                    objInner.NodeName2 = "Sector";
                                    objInner.NodeID2 = sec;
                                    objInner.BenchmarkID = specificCountrySectorData.Select(t => t.BenchmarkName).FirstOrDefault();
                                    objInner.DataID = d;
                                    objInner.PeriodType = specificCountrySectorData.Select(t => t.PeriodType).FirstOrDefault();
                                    objInner.PeriodYear = specificCountrySectorData.Select(t => t.PeriodYear).FirstOrDefault();
                                    objInner.Currency = specificCountrySectorData.Select(t => t.Currency).FirstOrDefault();
                                    objInner.UpdateDate = DateTime.Today;
                                    Decimal harmonicMeanSec = CalculateHarmonicMeanBenchmark(specificCountrySectorData);
                                    objInner.Amount = harmonicMeanSec;
                                    groupedForData.Add(objInner);
                                }

                                //group by country and industry
                                foreach (String indus in bothRatiosData.Where(t => t.DataId == d && t.Country == cou && t.PeriodYear == perYear)
                                    .Select(t => t.industry).Distinct().ToList())
                                {
                                    GroupedBenchmarkNodeData objInner = new GroupedBenchmarkNodeData();
                                    List<BenchmarkNodeFinancialsData> specificCountryIndusData = new List<BenchmarkNodeFinancialsData>();
                                    specificCountryIndusData = bothRatiosData.Where(t => t.DataId == d && t.Country == cou && t.industry == indus 
                                        && t.PeriodYear == perYear).ToList();
                                    objInner.NodeName1 = "Country";
                                    objInner.NodeID1 = cou;
                                    objInner.NodeName2 = "Industry";
                                    objInner.NodeID2 = indus;
                                    objInner.BenchmarkID = specificCountryIndusData.Select(t => t.BenchmarkName).FirstOrDefault();
                                    objInner.DataID = d;
                                    objInner.PeriodType = specificCountryIndusData.Select(t => t.PeriodType).FirstOrDefault();
                                    objInner.PeriodYear = specificCountryIndusData.Select(t => t.PeriodYear).FirstOrDefault();
                                    objInner.Currency = specificCountryIndusData.Select(t => t.Currency).FirstOrDefault();
                                    objInner.UpdateDate = DateTime.Today;
                                    Decimal harmonicMeanSec = CalculateHarmonicMeanBenchmark(specificCountryIndusData);
                                    objInner.Amount = harmonicMeanSec;
                                    groupedForData.Add(objInner);
                                }
                            }
                            #endregion

                            #region grouping by sector
                            foreach (String sec in bothRatiosData.Where(t => t.DataId == d && t.PeriodYear == perYear)
                                .Select(t => t.Sector).Distinct().ToList())
                            {
                                GroupedBenchmarkNodeData objInner = new GroupedBenchmarkNodeData();
                                List<BenchmarkNodeFinancialsData> specificSectorData = new List<BenchmarkNodeFinancialsData>();
                                specificSectorData = bothRatiosData.Where(t => t.DataId == d && t.Sector == sec && t.PeriodYear == perYear)
                                    .ToList();
                                objInner.NodeName1 = "Sector";
                                objInner.NodeID1 = sec;
                                objInner.NodeName2 = String.Empty;
                                objInner.NodeID2 = String.Empty;
                                objInner.BenchmarkID = specificSectorData.Select(t => t.BenchmarkName).FirstOrDefault();
                                objInner.DataID = d;
                                objInner.PeriodType = specificSectorData.Select(t => t.PeriodType).FirstOrDefault();
                                objInner.PeriodYear = specificSectorData.Select(t => t.PeriodYear).FirstOrDefault();
                                objInner.Currency = specificSectorData.Select(t => t.Currency).FirstOrDefault();
                                objInner.UpdateDate = DateTime.Today;
                                Decimal harmonicMeanSec = CalculateHarmonicMeanBenchmark(specificSectorData);
                                objInner.Amount = harmonicMeanSec;
                                groupedForData.Add(objInner);
                            }
                            #endregion

                            #region grouping by industry
                            foreach (String indus in bothRatiosData.Where(t => t.DataId == d && t.PeriodYear == perYear).
                                Select(t => t.industry).Distinct().ToList())
                            {
                                GroupedBenchmarkNodeData objInner = new GroupedBenchmarkNodeData();
                                List<BenchmarkNodeFinancialsData> specificIndustryData = new List<BenchmarkNodeFinancialsData>();
                                specificIndustryData = bothRatiosData.Where(t => t.DataId == d && t.industry == indus && t.PeriodYear == perYear)
                                    .ToList();
                                objInner.NodeName1 = "Industry";
                                objInner.NodeID1 = indus;
                                objInner.NodeName2 = String.Empty;
                                objInner.NodeID2 = String.Empty;
                                objInner.BenchmarkID = specificIndustryData.Select(t => t.BenchmarkName).FirstOrDefault();
                                objInner.DataID = d;
                                objInner.PeriodType = specificIndustryData.Select(t => t.PeriodType).FirstOrDefault();
                                objInner.PeriodYear = specificIndustryData.Select(t => t.PeriodYear).FirstOrDefault();
                                objInner.Currency = specificIndustryData.Select(t => t.Currency).FirstOrDefault();
                                objInner.UpdateDate = DateTime.Today;
                                Decimal harmonicMeanSec = CalculateHarmonicMeanBenchmark(specificIndustryData);
                                objInner.Amount = harmonicMeanSec;
                                groupedForData.Add(objInner);
                            }
                            #endregion

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            return groupedForData;
        }

        /// <summary>
        /// Calculates harmonic mean
        /// </summary>
        /// <param name="filteredList">filtered list according to various groups formed</param>
        /// <returns></returns>
        private Decimal CalculateHarmonicMeanBenchmark(List<BenchmarkNodeFinancialsData> filteredList)
        {
            Decimal? initialSumBenchmarkWeight = 0;
            Decimal? multipliedAmount = 0;
            Decimal? totalAmount = 0;
            Decimal? harmonicMean = 0;
            Decimal? benchWeight = 0;
            try
            {
                initialSumBenchmarkWeight = filteredList.Sum(t => t.BenWeight);

                if (filteredList[0].DataId == 197 || filteredList[0].DataId == 187 || filteredList[0].DataId == 189 ||
                    filteredList[0].DataId == 188 || filteredList[0].DataId == 198 || filteredList[0].DataId == 170 ||
                    filteredList[0].DataId == 166 || filteredList[0].DataId == 171 || filteredList[0].DataId == 164 ||
                    filteredList[0].DataId == 193)
                {
                    foreach (BenchmarkNodeFinancialsData row in filteredList)
                    {
                        benchWeight = initialSumBenchmarkWeight != 0 ? (row.BenWeight / initialSumBenchmarkWeight) : 0;
                        multipliedAmount = row.InvAmount * benchWeight;
                        totalAmount = totalAmount + multipliedAmount;
                    }
                    harmonicMean = totalAmount != 0 ? 1 / totalAmount : 0;
                }
                else
                {
                    foreach (BenchmarkNodeFinancialsData row in filteredList)
                    {
                        benchWeight = initialSumBenchmarkWeight != 0 ? (row.BenWeight / initialSumBenchmarkWeight) : 0;
                        multipliedAmount = row.Amount * benchWeight;
                        totalAmount = totalAmount + multipliedAmount;
                    }
                    harmonicMean = totalAmount;
                }
            }
            catch (Exception ex)
            {
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            return Convert.ToDecimal(harmonicMean);
        }

        /// <summary>
        /// Creates XML fri=om data for insertion into the database
        /// </summary>
        /// <param name="groupedFinalData"></param>
        /// <param name="entity"></param>
        /// <param name="benId"></param>
        private void CreateXMLInsertData(List<GroupedBenchmarkNodeData> groupedFinalData, AIMSDataEntity entity, String benId)
        {
            try
            {
                XDocument doc = GetEntityXml<GroupedBenchmarkNodeData>(groupedFinalData);
                entity.InsertIntoBenchmarkNodeFinancials(doc.ToString());
                log.Debug(String.Format("Insertion Completed for Benchmark {0}. Number of rows: {1}", benId, groupedFinalData.Count));
            }
            catch (Exception ex)
            {
                log.Debug("Insertion failed for Benchmark : " + benId);
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }

        /// <summary>
        /// Generic ethod for the creation of xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="xmlDoc">xml doc</param>
        /// <param name="strictlyInclusiveProperties"></param>
        /// <returns></returns>
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
                            {
                                continue;
                            }
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
        #endregion
    }
}
