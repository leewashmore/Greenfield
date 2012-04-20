using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class RelativePerformanceActivePositionData
    {
        /// <summary>
        /// Country / Sector / Security name for which the active position record is being referenced
        /// </summary>
        [DataMember]
        public string Entity { get; set; }

        /// <summary>
        /// Market value with respect to the entity
        /// </summary>
        [DataMember]
        public double? MarketValue { get; set; }

        /// <summary>
        /// Percentage representation of weight entity holds in the Fund/Composite
        /// </summary>
        [DataMember]
        public double? FundWeight { get; set; }

        /// <summary>
        /// Percentage representation of weight entity holds in the Benchmark
        /// </summary>
        [DataMember]
        public double? BenchmarkWeight { get; set; }

        /// <summary>
        ///Difference between the percentage representation of weight entity holds in the Fund/Composite and Benchmark
        /// </summary>
        [DataMember]
        public double? ActivePosition { get; set; }
    }
}