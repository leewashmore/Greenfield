using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class EntitySelectionData
    {
        [DataMember]
        public String Type { get; set; }

        [DataMember]
        public String SecurityType { get; set; }

        [DataMember]
        public String ShortName { get; set; }

        [DataMember]
        public String LongName { get; set; }

        [DataMember]
        public String InstrumentID { get; set; }
    }
}