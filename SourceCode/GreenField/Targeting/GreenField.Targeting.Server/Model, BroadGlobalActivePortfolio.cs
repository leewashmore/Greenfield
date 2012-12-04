using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class BroadGlobalActivePortfolioModel : PortfolioModel
    {
        [DebuggerStepThrough]
        public BroadGlobalActivePortfolioModel()
        {
        }

        [DebuggerStepThrough]
        public BroadGlobalActivePortfolioModel(String id, String name)
            : base(id, name)
        {
        }
    }
}
