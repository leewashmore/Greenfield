using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class BenchmarkSelectionData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string InstrumentID { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}