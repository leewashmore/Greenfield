using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{   
    [DataContract]
    public class FreeCashFlowsData
    {
        [DataMember]
        public decimal[] RevenueGrowth { get; set; }

        [DataMember]
        public decimal[] EBITDAMargins { get; set; }

        [DataMember]
        public decimal[] EBITDA { get; set; }

        [DataMember]
        public decimal[] Taxes { get; set; }

        [DataMember]
        public decimal[] ChangeInWC { get; set; }

        [DataMember]
        public decimal[] Capex { get; set; }

        [DataMember]
        public decimal[] FreeCashFlow { get; set; }        

    }
}
