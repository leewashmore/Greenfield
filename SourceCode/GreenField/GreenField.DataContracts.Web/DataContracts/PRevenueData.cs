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
        public DateTime PeriodEndDate { get; set; }

        [DataMember]
        public decimal? Amount { get; set; }

        [DataMember]
        public string PeriodType { get; set; }

        [DataMember]
        public Int32? PeriodYear { get; set; }

        [DataMember]
        public decimal? USDPrice { get; set; }

        [DataMember]
        public decimal? SharesOutstanding { get; set; }

        [DataMember]
        public decimal? PRevenueVal { get; set; }

        [DataMember]
        public decimal? Average { get; set; }

        [DataMember]
        public decimal? StdDevPlus { get; set; }

        [DataMember]
        public decimal? StdDevMinus { get; set; }
    }
}
