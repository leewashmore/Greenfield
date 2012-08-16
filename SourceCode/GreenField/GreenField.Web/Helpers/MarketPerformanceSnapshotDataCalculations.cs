using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DataContracts;
using GreenField.DAL;
using GreenField.Web.DimensionEntitiesService;
using System.Reflection;

namespace GreenField.Web.Helpers
{
    public static class MarketPerformanceSnapshotDataCalculations
    {
        public static MarketSnapshotPerformanceData GetBenchmarkPerformanceData(Entities entity, MarketSnapshotPreference preference)
        {
            MarketSnapshotPerformanceData result = new MarketSnapshotPerformanceData();

            try
            {
                List<GF_PERF_DAILY_ATTRIBUTION> benchmarkRecords = entity.GF_PERF_DAILY_ATTRIBUTION
                .Where(record => record.NODE_NAME == (preference.EntityNodeType == "Country" ? "Country" : "GICS Level 1")
                    && record.AGG_LVL_1 == (preference.EntityNodeType == null ? "Undefined" : preference.EntityNodeValueCode)
                    && record.AGG_LVL_1_LONG_NAME == (preference.EntityNodeType == null ? "-" : preference.EntityNodeValueName)
                    && record.BM == preference.EntityId.ToUpper()
                    && record.BMNAME == preference.EntityName
                    && record.TO_DATE != null
                    && record.POR_INCEPTION_DATE != null)
                .OrderByDescending(record => record.TO_DATE).ToList();

                GF_PERF_DAILY_ATTRIBUTION lastRecord = benchmarkRecords.FirstOrDefault();

                GF_PERF_DAILY_ATTRIBUTION benchmarkRecord = lastRecord != null ? GetMinInceptionDateRecord<GF_PERF_DAILY_ATTRIBUTION>(benchmarkRecords
                    .Where(record => record.TO_DATE == lastRecord.TO_DATE).ToList()) : null;

                result.DateToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null ? benchmarkRecord.BM1_TOP_RC_TWR_1D * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_1D * Convert.ToDecimal(100)) : null;
                result.WeekToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null ? benchmarkRecord.BM1_TOP_RC_TWR_1W * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_1W * Convert.ToDecimal(100)) : null;
                result.MonthToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null ? benchmarkRecord.BM1_TOP_RC_TWR_MTD * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_MTD * Convert.ToDecimal(100)) : null;
                result.QuarterToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null ? benchmarkRecord.BM1_TOP_RC_TWR_QTD * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_QTD * Convert.ToDecimal(100)) : null;
                result.YearToDateReturn = benchmarkRecord != null ? (preference.EntityNodeType == null ? benchmarkRecord.BM1_TOP_RC_TWR_YTD * Convert.ToDecimal(100) : benchmarkRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100)) : null;

                if (preference.EntityNodeType == null)
                {
                    GF_PERF_TOPLEVELYEAR benchmarkLastYearRecord = GetMinInceptionDateRecord<GF_PERF_TOPLEVELYEAR>(entity.GF_PERF_TOPLEVELYEAR
                                .Where(g => g.CURRENCY.ToUpper() == "USD"
                                    && g.RETURN_TYPE.ToUpper() == "NET"
                                    && g.TO_DATE == "31/12/" + (DateTime.Today.Year - 1).ToString()
                                    && g.BM1ID.ToUpper() == preference.EntityId.ToUpper()
                                    && g.BM1NAME.ToUpper() == preference.EntityName.ToUpper()
                                    && g.POR_INCEPTION_DATE != null)
                                .ToList());


                    result.LastYearReturn = preference.EntityNodeType == null ? (benchmarkLastYearRecord != null ? benchmarkLastYearRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100) : null) : null;

                    GF_PERF_TOPLEVELYEAR benchmarkSecondLastYearRecord = GetMinInceptionDateRecord<GF_PERF_TOPLEVELYEAR>(entity.GF_PERF_TOPLEVELYEAR
                        .Where(g => g.CURRENCY.ToUpper() == "USD"
                            && g.RETURN_TYPE.ToUpper() == "NET"
                            && g.TO_DATE == "31/12/" + (DateTime.Today.Year - 2).ToString()
                            && g.BM1ID.ToUpper() == preference.EntityId.ToUpper()
                            && g.BM1NAME.ToUpper() == preference.EntityName.ToUpper()
                            && g.POR_INCEPTION_DATE != null)
                        .ToList());

                    result.SecondLastYearReturn = preference.EntityNodeType == null ? (benchmarkSecondLastYearRecord != null ? benchmarkSecondLastYearRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100) : null) : null;

                    GF_PERF_TOPLEVELYEAR benchmarkThirdLastYearRecord = GetMinInceptionDateRecord<GF_PERF_TOPLEVELYEAR>(entity.GF_PERF_TOPLEVELYEAR
                        .Where(g => g.CURRENCY.ToUpper() == "USD"
                            && g.RETURN_TYPE.ToUpper() == "NET"
                            && g.TO_DATE == "31/12/" + (DateTime.Today.Year - 3).ToString()
                            && g.BM1ID.ToUpper() == preference.EntityId.ToUpper()
                            && g.BM1NAME.ToUpper() == preference.EntityName.ToUpper()
                            && g.POR_INCEPTION_DATE != null)
                        .ToList());

                    result.ThirdLastYearReturn = preference.EntityNodeType == null ? (benchmarkThirdLastYearRecord != null ? benchmarkThirdLastYearRecord.BM1_RC_TWR_YTD * Convert.ToDecimal(100) : null) : null; 
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static MarketSnapshotPerformanceData GetSecurityCommodityIndexPerformanceData(Entities entity, MarketSnapshotPreference preference)
        {
            MarketSnapshotPerformanceData result = new MarketSnapshotPerformanceData();
            try
            {
                DateTime TrackDate = DateTime.Today;

                GF_PRICING_BASEVIEW lastDateToDateRecord = entity.GF_PRICING_BASEVIEW
                    .Where(record => record.INSTRUMENT_ID == preference.EntityId
                        && record.TYPE == preference.EntityType
                        && record.ISSUE_NAME == preference.EntityName
                        && record.FROMDATE != null)
                    .OrderByDescending(record => record.FROMDATE)
                    .Take(1).FirstOrDefault();

                if (lastDateToDateRecord == null)
                    return result;

                GF_PRICING_BASEVIEW firstRecord = entity.GF_PRICING_BASEVIEW
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

                result.WeekToDateReturn = GetReturn(entity, preference, lastBusinessDateTime.AddDays(-6) > firstRecordDateTime ? lastBusinessDateTime.AddDays(-6) : firstRecordDateTime, lastBusinessDateTime);
                result.MonthToDateReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) > firstRecordDateTime ? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) : firstRecordDateTime, lastBusinessDateTime);
                result.QuarterToDateReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year, (DateTime.Today.Month - (DateTime.Today.Month % 3) + 1), 1) > firstRecordDateTime ? new DateTime(DateTime.Today.Year, (DateTime.Today.Month - (DateTime.Today.Month % 3) + 1), 1) : firstRecordDateTime, lastBusinessDateTime);
                result.YearToDateReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year, 1, 1) > firstRecordDateTime ? new DateTime(DateTime.Today.Year, 1, 1) : firstRecordDateTime, lastBusinessDateTime);
                result.LastYearReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year - 1, 1, 1) > firstRecordDateTime ? new DateTime(DateTime.Today.Year - 1, 1, 1) : firstRecordDateTime, new DateTime(DateTime.Today.Year - 1, 12, 31));
                result.SecondLastYearReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year - 2, 1, 1) > firstRecordDateTime ? new DateTime(DateTime.Today.Year - 2, 1, 1) : firstRecordDateTime, new DateTime(DateTime.Today.Year - 2, 12, 31));
                result.ThirdLastYearReturn = GetReturn(entity, preference, new DateTime(DateTime.Today.Year - 3, 1, 1) > firstRecordDateTime ? new DateTime(DateTime.Today.Year - 3, 1, 1) : firstRecordDateTime, new DateTime(DateTime.Today.Year - 3, 12, 31));
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        private static Decimal? GetReturn(Entities entity, MarketSnapshotPreference preference, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<GF_PRICING_BASEVIEW> datedRecords = entity.GF_PRICING_BASEVIEW
                            .Where(record => record.INSTRUMENT_ID == preference.EntityId
                                && record.TYPE == preference.EntityType
                                && record.ISSUE_NAME == preference.EntityName
                                && record.FROMDATE >= fromDate
                                && record.FROMDATE <= toDate)
                            .ToList();

                Decimal result = -1;
                foreach (GF_PRICING_BASEVIEW record in datedRecords)
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

        private static GF_PERF_TOPLEVELYEAR GetTopLevelYearMinInceptionDateRecord(List<GF_PERF_TOPLEVELYEAR> list)
        {
            if (list == null)
                return null;

            GF_PERF_TOPLEVELYEAR result = list.FirstOrDefault();
            foreach (GF_PERF_TOPLEVELYEAR item in list)
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


