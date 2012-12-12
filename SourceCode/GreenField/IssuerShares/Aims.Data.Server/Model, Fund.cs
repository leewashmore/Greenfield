using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Aims.Data.Server
{
    [DataContract]
    public class FundModel : SecurityModel
    {
        [DebuggerStepThrough]
        public FundModel(String id, String name, String shortName, String ticker)
        {
            this.Id = id;
            this.Name = name;
            this.ShortName = shortName;
            this.Ticker = ticker;
        }

        [DataMember]
        public String ShortName { get; set; }

        [DataMember]
        public String Ticker { get; set; }
    }
}
