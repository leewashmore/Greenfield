using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{

    [DataContract]
    public class PerformanceData
    {
        [DataMember]
         public int ID { get; set; }

        [DataMember]
        public String PORTFOLIOCODE { get; set; }

        [DataMember]
        public String QUOTATIONCURRENCYCODE { get; set; }

        [DataMember]
        public String PERFORMANCECURRENCYCODE { get; set; }

        [DataMember]
        public Decimal CONTRIBRETBM1CURRRCMTD { get; set; }

        [DataMember]
        public Decimal CONTRIBRETBM1CURRTOPRCMTD { get; set; }

        [DataMember]
        public Decimal IRP_TWRBM1CURRMTDRC { get; set; }

        [DataMember]
        public Decimal A_PERFREP_BM1WEIGHT_TO_TOP_EOD { get; set; }

        [DataMember]
        public Decimal A_PERFREP_BM1WEIGHT_TO_TOP_SOD { get; set; }

        [DataMember]
        public DateTime A_PERFREP_TO_DATE { get; set; }
               
    }
}