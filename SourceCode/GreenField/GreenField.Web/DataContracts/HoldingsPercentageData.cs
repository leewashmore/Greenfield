using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class HoldingsPercentageData
    {
        [DataMember]
        public String SegmentName { get; set; }

        [DataMember]
        public int SegmentHoldingsShare { get; set; }
    }
}