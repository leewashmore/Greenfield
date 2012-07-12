using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace GreenField.DataContracts
{
    [DataContract]
    public class ConsensusEstimatesValuations
    {
        [DataMember]
        public string IssueName { get; set; }

        [DataMember]
        public decimal PRevenue { get; set; }

        [DataMember]
        public decimal P_CE { get; set; }

        [DataMember]
        public decimal P_E { get; set; }
        
        [DataMember]
        public decimal P_EGrowth { get; set; }
        
        [DataMember]
        public decimal P_BV { get; set; }
        
        [DataMember]
        public decimal DividendYield { get; set; }       
    }
}
