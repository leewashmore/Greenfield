﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GreenField.DAL;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculations concerning Market Performance Snapshot Gadget
    /// </summary>
    public static class MarketPerformanceSnapshotDataCalculations
    {
        /// <summary>
        /// Gets performance data for a specific snapshot preference where entity type is benchmark
        /// </summary>
        /// <param name="entity">Dimension service entity instance</param>
        /// <param name="preference">MarketSnapshotPreference object</param>
        /// <returns>MarketSnapshotPerformanceData</returns>
        public static MarketSnapshotPerformanceData GetBenchmarkPerformanceData(DimensionEntities entity, MarketSnapshotPreference preference)
        {
            MarketSnapshotPerformanceData result = new MarketSnapshotPerformanceData();

            try
            {
                List<GreenField.DAL.GF_PERF_DAILY_ATTRIBUTION> benchmarkRecords = entity.GF_PERF_DAILY_ATTRIBUTION
                .Where(record => record.NODE_NAME == (preference.EntityNodeType == "Country" ? "Country" : "GICS Level 1")
                    && record.AGG_LVL_1 == (preference.EntityNodeType == null ? "Undefined" : preference.EntityNodeValueCode)
                    && record.AGG_LVL_1_LONG_NAME == (preference.EntityNodeType == null ? "-" : preference.EntityNodeValueName)
                    && record.BM == preference.EntityId.ToUpper()
                    && record.BMNAME == preference.EntityName
                    && record.TO_DATE != null
                    && record.POR_INCEPTION_DATE != null)
                .OrderByDescending(record => record.TO_DATE).ToList();

                GreenField.DAL.GF_PERF_DAILY_ATTRIBUTION lastRecord = benchmarkRecords.FirstOrDefault();

                GreenField.DAL.GF_PERF_DAILY_ATTRIBUTION benchmarkRecord = lastRecord != null ? GetMinInceptionDateRecord<GreenField.DAL.GF_PERF_DAILY_ATTRIBUTION>(benchmarkRecords
                    .Where(record => record.TO_DATE == lastRecord.TO_DATE).ToList()) : null;

                result.DateToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null 
                    ? benchmarkRecord.BM1_TOP_RC_TWR_1D * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_1D * Convert.ToDecimal(100)) : null;
                result.WeekToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null 
                    ? benchmarkRecord.BM1_TOP_RC_TWR_1W * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_1W * Convert.ToDecimal(100)) : null;
                result.MonthToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null 
                    ? benchmarkRecord.BM1_TOP_RC_TWR_MTD * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_MTD * Convert.ToDecimal(100)) : null;
                result.QuarterToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null 
                    ? benchmarkRecord.BM1_TOP_RC_TWR_QTD * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_QTD * Convert.ToDecimal(100)) : null;
                result.YearToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null 
                    ? benchmarkRecord.BM1_TOP_RC_TWR_YTD * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100)) : null;

                if (preference.EntityNodeType == null)
                {
                    GreenField.DAL.GF_PERF_TOPLEVELYEAR benchmarkLastYearRecord = GetMinInceptionDateRecord<GreenField.DAL.GF_PERF_TOPLEVELYEAR>(entity.GF_PERF_TOPLEVELYEAR
                                .Where(g => g.CURRENCY.ToUpper() == "USD"
                                    && g.RETURN_TYPE.ToUpper() == "NET"
                                    && g.TO_DATE == "31/12/" + (DateTime.Today.Year - 1).ToString()
                                    && g.BM1ID.ToUpper() == preference.EntityId.ToUpper()
                                    && g.BM1NAME.ToUpper() == preference.EntityName.ToUpper()
                                    && g.POR_INCEPTION_DATE != null)
                                .ToList());

                    result.LastYearReturn = preference.EntityNodeType == null ? (benchmarkLastYearRecord != null 
                        ? benchmarkLastYearRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100) : null) : null;

                    GreenField.DAL.GF_PERF_TOPLEVELYEAR benchmarkSecondLastYearRecord = GetMinInceptionDateRecord<GreenField.DAL.GF_PERF_TOPLEVELYEAR>(entity.GF_PERF_TOPLEVELYEAR
                        .Where(g => g.CURRENCY.ToUpper() == "USD"
                            && g.RETURN_TYPE.ToUpper() == "NET"
                            && g.TO_DATE == "31/12/" + (DateTime.Today.Year - 2).ToString()
                            && g.BM1ID.ToUpper() == preference.EntityId.ToUpper()
                            && g.BM1NAME.ToUpper() == preference.EntityName.ToUpper()
                            && g.POR_INCEPTION_DATE != null)
                        .ToList());

                    result.SecondLastYearReturn = preference.EntityNodeType == null ? (benchmarkSecondLastYearRecord != null 
                        ? benchmarkSecondLastYearRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100) : null) : null;

                    GreenField.DAL.GF_PERF_TOPLEVELYEAR benchmarkThirdLastYearRecord = GetMinInceptionDateRecord<GreenField.DAL.GF_PERF_TOPLEVELYEAR>(entity.GF_PERF_TOPLEVELYEAR
                        .Where(g => g.CURRENCY.ToUpper() == "USD"
                            && g.RETURN_TYPE.ToUpper() == "NET"
                            && g.TO_DATE == "31/12/" + (DateTime.Today.Year - 3).ToString()
                            && g.BM1ID.ToUpper() == preference.EntityId.ToUpper()
                            && g.BM1NAME.ToUpper() == preference.EntityName.ToUpper()
                            && g.POR_INCEPTION_DATE != null)
                        .ToList());

                    result.ThirdLastYearReturn = preference.EntityNodeType == null ? (benchmarkThirdLastYearRecord != null 
                        ? benchmarkThirdLastYearRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100) : null) : null; 
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Gets performance data for a specific snapshot preference where entity type is security, index or commodity
        /// </summary>
        /// <param name="entity">Dimension service entity instance</param>
        /// <param name="preference">MarketSnapshotPreference object</param>
        /// <returns>MarketSnapshotPerformanceData</returns>
        public static MarketSnapshotPerformanceData GetSecurityCommodityIndexPerformanceData(DimensionEntities entity, MarketSnapshotPreference preference)
        {
            MarketSnapshotPerformanceData result = new MarketSnapshotPerformanceData();
            try
            {
                DateTime TrackDate = DateTime.Today;

                GreenField.DAL.GF_PRICING_BASEVIEW lastDateToDateRecord = entity.GF_PRICING_BASEVIEW
                    .Where(record => record.INSTRUMENT_ID == preference.EntityId
                        && record.TYPE == preference.EntityType
                        && record.ISSUE_NAME == preference.EntityName
                        && record.FROMDATE != null)
                    .OrderByDescending(record => record.FROMDATE)
                    .Take(1).FirstOrDefault();

                if (lastDateToDateRecord == null)
                    return result;

                GreenField.DAL.GF_PRICING_BASEVIEW firstRecord = entity.GF_PRICING_BASEVIEW
                    .Where(record => record.INSTRUMENT_ID == preference.EntityId
                        && record.TYPE == preference.EntityType
                        && record.ISSUE_NAME == preference.EntityName
                        && record.FROMDATE != null)
                    .OrderBy(record => record.FROMDATE)
                    .Take(1).FirstOrDefault();

                DateTime firstRecordDateTime = DateTime.Today.AddYears(-5);
                if (firstRecord != null)
                    firstRecordDateTime = Convert.ToDateTime(firstRecord.FROMDATE);

                DateTime lastBusinessDateTime = TrackDate = Convert.ToDateTime(lastDateToDateRecord.FROMDATE);
                result.DateToDateReturn = lastDateToDateRecord != null ? (preference.EntityReturnType == "Price"
                    ? lastDateToDateRecord.DAILY_PRICE_RETURN != null ? lastDateToDateRecord.DAILY_PRICE_RETURN : 0
                    : lastDateToDateRecord.DAILY_GROSS_RETURN != null ? lastDateToDateRecord.DAILY_GROSS_RETURN : 0) : null;

                result.WeekToDateReturn = GetReturn(entity, preference, lastBusinessDateTime.AddDays(-6) > firstRecordDateTime 
                    ? lastBusinessDateTime.AddDays(-6) : firstRecordDateTime, lastBusinessDateTime);
                result.MonthToDateReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) > firstRecordDateTime 
                    ? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) : firstRecordDateTime, lastBusinessDateTime);
                result.QuarterToDateReturn = GetReturn(entity, preference
                    , new DateTime(DateTime.Today.Year, (DateTime.Today.Month - ((DateTime.Today.Month - 1) % 3)), 1) > firstRecordDateTime
                    ? new DateTime(DateTime.Today.Year, (DateTime.Today.Month - ((DateTime.Today.Month - 1) % 3)), 1) : firstRecordDateTime, lastBusinessDateTime);
                result.YearToDateReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year, 1, 1) > firstRecordDateTime 
                    ? new DateTime(DateTime.Today.Year, 1, 1) : firstRecordDateTime, lastBusinessDateTime);
                result.LastYearReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year - 1, 1, 1) > firstRecordDateTime 
                    ? new DateTime(DateTime.Today.Year - 1, 1, 1) : firstRecordDateTime, new DateTime(DateTime.Today.Year - 1, 12, 31));
                result.SecondLastYearReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year - 2, 1, 1) > firstRecordDateTime 
                    ? new DateTime(DateTime.Today.Year - 2, 1, 1) : firstRecordDateTime, new DateTime(DateTime.Today.Year - 2, 12, 31));
                result.ThirdLastYearReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year - 3, 1, 1) > firstRecordDateTime 
                    ? new DateTime(DateTime.Today.Year - 3, 1, 1) : firstRecordDateTime, new DateTime(DateTime.Today.Year - 3, 12, 31));
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Gets return value for a preference between two specific dates
        /// </summary>
        /// <param name="entity">dimension service instance</param>
        /// <param name="preference">MarketSnapshotPreference object</param>
        /// <param name="fromDate">start date</param>
        /// <param name="toDate">end date</param>
        /// <returns>security return value</returns>
        private static Decimal? GetReturn(DimensionEntities entity, MarketSnapshotPreference preference, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<GreenField.DAL.GF_PRICING_BASEVIEW> datedRecords = entity.GF_PRICING_BASEVIEW
                            .Where(record => record.INSTRUMENT_ID == preference.EntityId
                                && record.TYPE == preference.EntityType
                                && record.ISSUE_NAME == preference.EntityName
                                && record.FROMDATE >= fromDate
                                && record.FROMDATE <= toDate)
                            .ToList();

                Decimal result = -1;
                foreach (GreenField.DAL.GF_PRICING_BASEVIEW record in datedRecords)
                {
                    Decimal? returnValue = preference.EntityReturnType == "Price"
                    ? (record.DAILY_PRICE_RETURN != null ? record.DAILY_PRICE_RETURN : 0)
                    : (record.DAILY_GROSS_RETURN != null ? record.DAILY_GROSS_RETURN : 0);

                    if (result == -1)
                        result = (1 + ((returnValue == null ? 0 : Convert.ToDecimal(returnValue)) / 100));
                    else
                        result = result * (1 + ((returnValue == null ? 0 : Convert.ToDecimal(returnValue)) / 100));
                }

                if (result == -1)
                    return null;

                return (result - 1) * 100;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static GreenField.DAL.GF_PERF_TOPLEVELYEAR GetTopLevelYearMinInceptionDateRecord(List<GreenField.DAL.GF_PERF_TOPLEVELYEAR> list)
        {
            if (list == null)
                return null;

            GreenField.DAL.GF_PERF_TOPLEVELYEAR result = list.FirstOrDefault();
            foreach (GreenField.DAL.GF_PERF_TOPLEVELYEAR item in list)
            {
                DateTime resultInceptionDate;
                DateTime itemInceptionDate;

                if (DateTime.TryParse(item.POR_INCEPTION_DATE, out itemInceptionDate).Equals(false))
                    continue;
                                
                if (DateTime.TryParse(result.POR_INCEPTION_DATE, out resultInceptionDate).Equals(false))
                    continue;

                if (itemInceptionDate <= resultInceptionDate)
                    result = item;
            }

            return result;
        }

        private static T GetMinInceptionDateRecord<T>(List<T> list)
        {
            if (list == null)
                return default(T);

            T result = list.FirstOrDefault();
            foreach (T item in list)
            {
                DateTime resultInceptionDate;
                DateTime itemInceptionDate;

                PropertyInfo[] propertyInfo = typeof(T).GetProperties();
                if (propertyInfo.Any(record => record.Name == "POR_INCEPTION_DATE"))
                {
                    if (DateTime.TryParse((String)typeof(T).GetProperty("POR_INCEPTION_DATE").GetValue(item, null), out itemInceptionDate).Equals(false))
                        continue;

                    if (DateTime.TryParse((String)typeof(T).GetProperty("POR_INCEPTION_DATE").GetValue(result, null), out resultInceptionDate).Equals(false))
                        continue;

                    if (itemInceptionDate <= resultInceptionDate)
                        result = item;
                }
            }

            return result;
        }
    }
}


