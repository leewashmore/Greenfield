using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class MarketCapitalizationData
    {
        [DataMember]
        public string MegaLowerLimit { get; set; }

        [DataMember]
        public string LargeLowerLimit { get; set; }

        [DataMember]
        public string MediumLowerLimit { get; set; }

        [DataMember]
        public string SmallLowerLimit { get; set; }

        [DataMember]
        public long PortfolioWeightedAverage { get; set; }

        [DataMember]
        public long BenchmarkWeightedAverage { get; set; }

        [DataMember]
        public long PortfolioWeightedMedian { get; set; }

        [DataMember]
        public long BenchmarkWeightedMedian { get; set; }

        [DataMember]
        public double PortfolioMegaShare { get; set; }

        [DataMember]
        public double BenchmarkMegaShare { get; set; }

        [DataMember]
        public double PortfolioLargeShare { get; set; }

        [DataMember]
        public double BenchmarkLargeShare { get; set; }
        
        [DataMember]
        public double PortfolioMediumShare { get; set; }

        [DataMember]
        public double BenchmarkMediumShare { get; set; }

        [DataMember]
        public double PortfolioSmallShare { get; set; }

        [DataMember]
        public double BenchmarkSmallShare { get; set; }

        [DataMember]
        public double PortfolioMicroShare { get; set; }

        [DataMember]
        public double BenchmarkMicroShare { get; set; }
    }
}