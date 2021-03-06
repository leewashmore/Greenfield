﻿using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class RegionBreakdownData
    {
        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Security { get; set; }

        [DataMember]
        public decimal? PortfolioShare { get; set; }

        [DataMember]
        public decimal? BenchmarkShare { get; set; }

        [DataMember]
        public decimal? ActivePosition { get; set; }

        public String RegionSortOrder { get; set; }

        public String CountrySortOrder { get; set; }
    }
}