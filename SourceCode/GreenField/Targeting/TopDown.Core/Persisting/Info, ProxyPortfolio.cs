using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class ProxyPortfolioInfo
    {
        [DebuggerStepThrough]
        public ProxyPortfolioInfo()
        {
        }
        public String PortfolioId { get; set; }
        public String ProxyPortfolioId { get; set; }
        
    }
}
