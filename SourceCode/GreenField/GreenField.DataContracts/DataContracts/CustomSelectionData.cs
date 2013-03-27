using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class CustomSelectionData
    {
        [DataMember]
        public string ScreeningId { get; set; }

        [DataMember]
        public string DataDescription { get; set; }

        [DataMember]
        public string LongDescription { get; set; }

        [DataMember]
        public string ShortDescription { get; set; }

        [DataMember]
        public string Quaterly { get; set; }

        [DataMember]
        public string Annual { get; set; }

        [DataMember]
        public int DataID { get; set; }

        [DataMember]
        public int? EstimateID { get; set; }

        [DataMember]
        public string DataColumn { get; set; }
    }
}
