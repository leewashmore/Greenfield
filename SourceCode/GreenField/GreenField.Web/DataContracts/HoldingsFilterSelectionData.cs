using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class HoldingsFilterSelectionData
    {
        [DataMember]
        public String Filtertype { get; set; }

        [DataMember]
        public List<String> FilterValues { get; set; }

        [DataMember]
        public String SelectedFilterValue { get; set; }
    }
}