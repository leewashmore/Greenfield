using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class RelativePerformanceSecurityData
    {
        [DataMember]
        public string SecurityName { get; set; }

        [DataMember]
        public string SecurityCountryID { get; set; }

        [DataMember]
        public string SecuritySectorName { get; set; }

        [DataMember]
        public decimal? SecurityMarketValue { get; set; }

        [DataMember]
        public decimal? SecurityAlpha { get; set; }
    }
}