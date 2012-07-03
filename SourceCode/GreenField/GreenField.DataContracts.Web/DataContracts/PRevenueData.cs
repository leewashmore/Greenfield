using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    [DataContract]
    public class PRevenueData
    {
        [DataMember]
        public string PeriodLabel { get; set;}

        [DataMember]
        public decimal PRevenueVal { get; set; }

        [DataMember]
        public decimal Average { get; set; }

        [DataMember]
        public decimal StdDevPlus { get; set; }

        [DataMember]
        public decimal StdDevMinus { get; set; }
    }
}
