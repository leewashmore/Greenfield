﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract for MultiLineBenchmark UI Chart
    /// </summary>
    [DataContract]
    public class BenchmarkChartReturnData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public decimal OneD { get; set; }

        [DataMember]
        public decimal WTD { get; set; }

        [DataMember]
        public decimal MTD { get; set; }

        [DataMember]
        public decimal QTD { get; set; }

        [DataMember]
        public decimal YTD { get; set; }

        [DataMember]
        public decimal OneY { get; set; }

        [DataMember]
        public decimal PreviousYearData { get; set; }

        [DataMember]
        public decimal TwoPreviousYearData { get; set; }

        [DataMember]
        public decimal ThreePreviousYearData { get; set; }

        [DataMember]
        public decimal IndexedValue { get; set; }
    }

    /// <summary>
    /// Data Contract for MultiLineBenchmark UI Grid
    /// </summary>
    [DataContract]
    public class BenchmarkGridReturnData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public decimal MTD { get; set; }

        [DataMember]
        public decimal QTD { get; set; }

        [DataMember]
        public decimal YTD { get; set; }

        [DataMember]
        public decimal PreviousYearReturn { get; set; }

        [DataMember]
        public decimal TwoPreviousYearReturn { get; set; }

        [DataMember]
        public decimal ThreePreviousYearReturn { get; set; }
    }
}

