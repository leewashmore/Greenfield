using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class DCFSummaryData
    {
        [DataMember]
        public decimal PresentValue { get; set; }

        [DataMember]
        public decimal TerminalValue { get; set; }

        [DataMember]
        public decimal Cash { get; set; }

        [DataMember]
        public decimal TotalEnterpriseValue { get; set; }

        [DataMember]
        public decimal FVInvestments { get; set; }

        [DataMember]
        public decimal GrossDebt { get; set; }

        [DataMember]
        public decimal FVMinorities { get; set; }

        [DataMember]
        public decimal EquityValue { get; set; }

        [DataMember]
        public decimal NumberOfShares { get; set; }

        [DataMember]
        public decimal DCFvaluePerShare { get; set; }

        [DataMember]
        public decimal UpsideDownSide { get; set; }
    }
}
