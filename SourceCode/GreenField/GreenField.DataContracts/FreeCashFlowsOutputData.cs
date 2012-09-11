using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class FreeCashFlowsOutputData
    {
        [DataMember]
        public string FieldName { get; set; }

        [DataMember]
        public string ValueY0 { get; set; }

        [DataMember]
        public string ValueY1 { get; set; }

        [DataMember]
        public string ValueY2 { get; set; }

        [DataMember]
        public string ValueY3 { get; set; }

        [DataMember]
        public string ValueY4 { get; set; }

        [DataMember]
        public string ValueY5 { get; set; }

        [DataMember]
        public string ValueY6 { get; set; }

        [DataMember]
        public string ValueY7 { get; set; }

        [DataMember]
        public string ValueY8 { get; set; }

        [DataMember]
        public string ValueY9 { get; set; }

    }
}
