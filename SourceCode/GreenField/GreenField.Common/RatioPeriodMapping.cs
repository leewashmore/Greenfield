using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.DataContracts;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Reflection;

namespace GreenField.Common
{
    public static class RatioPeriodMapping
    {
        public class Data
        {
            public String Ratio { get; set; }
            public ScatterGraphPeriod Period { get; set; }
            public Int32? DataId { get; set; }
            public Int32? EstimationId { get; set; }
        }

        public static List<Data> MappingData { get; set; }

        static RatioPeriodMapping()
        {
            if (MappingData == null)
                MappingData = new List<Data>();

            Stream stream = typeof(RatioPeriodMapping).Assembly
                .GetManifestResourceStream("GreenField.Common.ScatterGraphRatioDataMapping.xml");

            using (stream)
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "Data")
                        {
                            Int32? dataId = null;
                            Int32? estimationId = null;

                            if (reader["DataId"] != "")
                                dataId = Convert.ToInt32(reader["DataId"]);

                            if (reader["EstimateId"] != "")
                                estimationId = Convert.ToInt32(reader["EstimateId"]);


                            MappingData.Add(new Data()
                            {
                                Ratio = reader["Ratio"],
                                Period = EnumUtils.GetEnumFromDescription<ScatterGraphPeriod>(reader["Period"]),
                                DataId = dataId,
                                EstimationId = estimationId
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get DataId value of a ratio for a specific period
        /// </summary>
        /// <param name="ratio">ScatterValuationRatio or ScatterFinancialRatio</param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static Int32? GetDataId(object ratio, ScatterGraphPeriod period)
        {
            String desc = ratio.ToString();

            if (ratio.GetType().Equals(typeof(ScatterGraphFinancialRatio)))
                desc = EnumUtils.GetDescriptionFromEnumValue<ScatterGraphFinancialRatio>(ratio);
            else if (ratio.GetType().Equals(typeof(ScatterGraphValuationRatio)))
                desc = EnumUtils.GetDescriptionFromEnumValue<ScatterGraphValuationRatio>(ratio);

            Data mappingData = MappingData.Where(record => record.Ratio == desc
            && record.Period == period).FirstOrDefault();

            return mappingData == null ? null : mappingData.DataId;


        }

        public static Int32? GetEstimationId(object ratio, ScatterGraphPeriod period)
        {
            String desc = ratio.ToString();

            if (ratio.GetType().Equals(typeof(ScatterGraphFinancialRatio)))
                desc = EnumUtils.GetDescriptionFromEnumValue<ScatterGraphFinancialRatio>(ratio);
            else if (ratio.GetType().Equals(typeof(ScatterGraphValuationRatio)))
                desc = EnumUtils.GetDescriptionFromEnumValue<ScatterGraphValuationRatio>(ratio);

            Data mappingData = MappingData.Where(record => record.Ratio == desc
            && record.Period == period).FirstOrDefault();

            return mappingData == null ? null : mappingData.EstimationId;
        }
    }
}
