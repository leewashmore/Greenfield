using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class BgaPortfolioSecurityTargetInfo
    {
        [DebuggerStepThrough]
        public BgaPortfolioSecurityTargetInfo()
        {
        }


        [DebuggerStepThrough]
        public BgaPortfolioSecurityTargetInfo(
            String broadGlobalActivePortfolioId,
            String securityId,
            Decimal target
        )
        {
            this.BroadGlobalActivePortfolioId = broadGlobalActivePortfolioId;
            this.SecurityId = securityId;
            this.Target = target;
        }

        public String BroadGlobalActivePortfolioId { get; set; }
        public String SecurityId { get; set; }
        public Decimal Target { get; set; }
    }
}
