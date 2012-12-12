using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Aims.Data.Server
{
    [DataContract]
    public class BottomUpPortfolioModel : PortfolioModel
    {
        [DebuggerStepThrough]
        public BottomUpPortfolioModel()
        {
        }

        [DebuggerStepThrough]
        public BottomUpPortfolioModel(String id, String name, FundModel fund)
            : base(id, name)
        {
            this.Fund = fund;
        }

        [DataMember]
        public FundModel Fund { get; set; }
    }
}
