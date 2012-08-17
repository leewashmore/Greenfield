using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    /// <summary>
    /// DataContract class for ChartExtension Gadget, Holdings 4.4.1
    /// </summary>
    [DataContract]
    public class ConsensusEstimatesSummaryData
    {
        [DataMember]
        public String NetIncome { get; set; }

        [DataMember]
        public String YEAR1 { get; set; }

        [DataMember]
        public String YEAR2 { get; set; }

        [DataMember]
        public String YEAR3 { get; set; }

        [DataMember]
        public String YEAR4 { get; set; }

        [DataMember]
        public String YEAR5 { get; set; }

        [DataMember]
        public String currency { get; set; }


    }
}
