using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class SecurityInformation
    {
        [DataMember]
        public string SecurityTicker { get; set; }

        [DataMember]
        public string SecurityName { get; set; }

        [DataMember]
        public string SecurityCountry { get; set; }

        [DataMember]
        public string SecurityIndustry { get; set; }

        [DataMember]
        public string SecurityMarketCapitalization { get; set; }

        [DataMember]
        public string Price { get; set; }

        [DataMember]
        public string Analyst { get; set; }

        [DataMember]
        public string FVCalc { get; set; }

        [DataMember]
        public string SecurityBuySellvsCrnt { get; set; }

        [DataMember]
        public string TotalCurrentHoldings { get; set; }

        [DataMember]
        public string PercentEMIF { get; set; }

        [DataMember]
        public string SecurityBMWeight { get; set; }

        [DataMember]
        public string SecurityActiveWeight { get; set; }

        [DataMember]
        public string YTDRet_Absolute { get; set; }

        [DataMember]
        public string YTDRet_RELtoLOC { get; set; }

        [DataMember]
        public string YTDRet_RELtoEM { get; set; }       

        [DataMember]
        public string SecurityRecommendation { get; set; }
    }
}