using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class BenchmarkSelectionData
    {
        [DataMember]
        public string Name { get; set; }
    }
}