using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class MorningSnapshotData
    {
        [DataMember]
        public UserBenchmarkPreference MorningSnapshotPreferenceData { get; set; }

        [DataMember]
        public double DTD { get; set; }

        [DataMember]
        public double WTD { get; set; }

        [DataMember]
        public double MTD { get; set; }

        [DataMember]
        public double QTD { get; set; }

        [DataMember]
        public double YTD { get; set; }

        [DataMember]
        public double PreviousYearPrice { get; set; }

        [DataMember]
        public double IIPreviousYearPrice { get; set; }

        [DataMember]
        public double IIIPreviousYearPrice { get; set; }
    }

}