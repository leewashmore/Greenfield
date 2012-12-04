using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Persisting
{
    public class InfoCopier
    {
        public BuPortfolioSecurityTargetInfo Copy(BuPortfolioSecurityTargetInfo info)
        {
            var result = new BuPortfolioSecurityTargetInfo(
                info.BottomUpPortfolioId,
                info.SecurityId,
                info.Target,
                info.ChangeId
            );
            return result;
        }
    }
}
