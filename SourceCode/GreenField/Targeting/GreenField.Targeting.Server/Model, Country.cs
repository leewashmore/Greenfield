using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class CountryModel
    {
        [DebuggerStepThrough]
        public CountryModel()
        {
        }

        [DebuggerStepThrough]
        public CountryModel(String isoCode, String name)
        {
            this.IsoCode = isoCode;
            this.Name = name;
        }

        [DataMember]
        public String IsoCode { get; set; }

        [DataMember]
        public String Name { get; set; }
    }
}
