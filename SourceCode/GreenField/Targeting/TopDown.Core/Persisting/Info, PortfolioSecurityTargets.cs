using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class PortfolioSecurityTargetsInfo
    {
        [DebuggerStepThrough]
        public PortfolioSecurityTargetsInfo()
        {
        }

        public String PortfolioId { get; set; }
        public String SecurityId { get; set; }
        public Decimal Target { get; set; }
        public DateTime? Updated { get; set; }
    }
}
