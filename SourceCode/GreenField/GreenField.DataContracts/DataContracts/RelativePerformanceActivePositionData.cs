using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
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
        /// Country / Sector / Security group for which the active position record is being referenced
        /// </summary>
        [DataMember]
        public string EntityGroup { get; set; }

        /// <summary>
        /// Market value with respect to the entity
        /// </summary>
        [DataMember]
        public decimal? MarketValue { get; set; }

        /// <summary>
        /// Percentage representation of weight entity holds in the Fund/Composite
        /// </summary>
        [DataMember]
        public decimal? FundWeight { get; set; }

        /// <summary>
        /// Percentage representation of weight entity holds in the Benchmark
        /// </summary>
        [DataMember]
        public decimal? BenchmarkWeight { get; set; }

        /// <summary>
        ///Difference between the percentage representation of weight entity holds in the Fund/Composite and Benchmark
        /// </summary>
        [DataMember]
        public decimal? ActivePosition { get; set; }
    }    
}