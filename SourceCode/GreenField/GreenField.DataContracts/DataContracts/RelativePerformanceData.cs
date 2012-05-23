using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class RelativePerformanceData
    {
        [DataMember]
        public string CountryID { get; set; }

        [DataMember]
        public List<RelativePerformanceCountrySpecificData> RelativePerformanceCountrySpecificInfo { get; set; }

        [DataMember]
        public decimal? AggregateCountryAlpha { get; set; }

        [DataMember]
        public decimal? AggregateCountryPortfolioShare { get; set; }

        [DataMember]
        public decimal? AggregateCountryBenchmarkShare { get; set; }

        [DataMember]
        public decimal? AggregateCountryActivePosition { get; set; }
    }

    [DataContract]
    public class RelativePerformanceCountrySpecificData
    {
        [DataMember]
        public string SectorID { get; set; }

        [DataMember]
        public string SectorName { get; set; }

        [DataMember]
        public decimal? Alpha { get; set; }

        [DataMember]
        public decimal? PortfolioShare { get; set; }

        [DataMember]
        public decimal? BenchmarkShare { get; set; }

        [DataMember]
        public decimal? ActivePosition { get; set; }
    }

    [DataContract]
    public class RelativePerformanceSectorData : IEquatable<RelativePerformanceSectorData>
    {
        [DataMember]
        public string SectorID { get; set; }

        [DataMember]
        public string SectorName { get; set; }

        public bool Equals(RelativePerformanceSectorData other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return SectorID.Equals(other.SectorID) && SectorName.Equals(other.SectorName);
        }

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null.
            int hashProductName = SectorID.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = SectorName.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }

    }

    

}