using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using BenchmarkNodeFinancials.DimensionServiceReference;
using System.Configuration;

namespace BenchmarkNodeFinancials
{
    class BenchmarkEntities
    {
        #region Fields
        private static AIMSDataEntity _entity;
        private static DimensionServiceReference.Entities _dimensionEntity;
        #endregion

        static void RetrieveBenchmarkEntityData()
        {
            _dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));

            List<DimensionServiceReference.GF_SELECTION_BASEVIEW> data = _dimensionEntity.GF_SELECTION_BASEVIEW.ToList();
            List<EntitySelectionData> result = new List<EntitySelectionData>();
            if (data != null)
            {
                foreach (DimensionServiceReference.GF_SELECTION_BASEVIEW record in data)
                {
                    result.Add(new EntitySelectionData()
                    {
                        SortOrder = EntityTypeSortOrder.GetSortOrder(record.TYPE),
                        ShortName = record.SHORT_NAME == null ? String.Empty : record.SHORT_NAME,
                        LongName = record.LONG_NAME == null ? String.Empty : record.LONG_NAME,
                        InstrumentID = record.INSTRUMENT_ID == null ? String.Empty : record.INSTRUMENT_ID,
                        Type = record.TYPE == null ? String.Empty : record.TYPE,
                        SecurityType = record.SECURITY_TYPE == null ? String.Empty : record.SECURITY_TYPE
                    });
                }
            }

            List<DimensionServiceReference.GF_PERF_DAILY_ATTRIB_DIST_BM> benchmarkData = _dimensionEntity.GF_PERF_DAILY_ATTRIB_DIST_BM.ToList();
            if (benchmarkData != null)
            {
                foreach (DimensionServiceReference.GF_PERF_DAILY_ATTRIB_DIST_BM benchmark in benchmarkData)
                {
                    result.Add(new EntitySelectionData()
                    {

                        SortOrder = EntityTypeSortOrder.GetSortOrder("BENCHMARK"),
                        ShortName = benchmark.BM == null ? String.Empty : benchmark.BM,
                        LongName = benchmark.BMNAME == null ? String.Empty : benchmark.BMNAME,
                        InstrumentID = benchmark.BM == null ? String.Empty : benchmark.BM,
                        Type = "BENCHMARK",
                        SecurityType = null
                    });
                }
            }
        }
    }

    public class EntitySelectionData
    {
        public int SortOrder { get; set; }

        public String Type { get; set; }

        public String SecurityType { get; set; }

        public String ShortName { get; set; }

        public String LongName { get; set; }

        public String InstrumentID { get; set; }
    }

    public static class EntityTypeSortOrder
    {
        public static int Currency = 1;
        public static int Commodity = 2;
        public static int Index = 3;
        public static int Security = 4;
        public static int Benchmark = 5;

        public static int GetSortOrder(String type)
        {
            switch (type.ToUpper())
            {
                case "CURRENCY":
                    return Currency;
                case "COMMODITY":
                    return Commodity;
                case "INDEX":
                    return Index;
                case "SECURITY":
                    return Security;
                case "BENCHMARK":
                    return Benchmark;
                default:
                    return 0;
            }
        }
    }
}
