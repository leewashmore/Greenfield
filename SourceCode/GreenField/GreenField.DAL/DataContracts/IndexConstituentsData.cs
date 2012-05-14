using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.DAL
{
    [DataContract]
    public class IndexConstituentsData
    {
        [DataMember]
        public string ConstituentName { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Sector { get; set; }

        [DataMember]
        public string Industry { get; set; }

        [DataMember]
        public string SubIndustry { get; set; }

        [DataMember]
        public decimal? Weight { get; set; }

        [DataMember]
        public decimal? WeightCountry { get; set; }

        [DataMember]
        public decimal? WeightIndustry { get; set; }
               
    }
}