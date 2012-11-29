using System;
using System.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class RelativePerformanceQueryData
    {
        [DataMember]
        public String SecurityName { get; set; }

        [DataMember]
        public String CountryCode { get; set; }

        [DataMember]
        public String SectorCode { get; set; }

        [DataMember]
        public String SectorName { get; set; }

        [DataMember]
        public Decimal? FundWeight { get; set; }

        [DataMember]
        public Decimal? BenchmarkWeight { get; set; }

        [DataMember]
        public Decimal? Alpha { get; set; }
    }
}
