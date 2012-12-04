using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class CompanySecurityModel : SecurityModel
    {
        [DebuggerStepThrough]
        public CompanySecurityModel()
        {
        }

        [DebuggerStepThrough]
        public CompanySecurityModel(String id, String name, String shortName, String ticker, CountryModel country)
        {
            this.Id = id;
            this.Name = name;
            this.ShortName = shortName;
            this.Ticker = ticker;
            this.Country = country;
        }

        [DataMember]
        public String ShortName { get; set; }

        [DataMember]
        public String Ticker { get; set; }

        [DataMember]
        public CountryModel Country { get; set; }
    }
}
