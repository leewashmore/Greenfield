using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;

namespace TopDown.Core.Overlaying
{
    /// <summary>
    /// Speeds up accessing information about targets by pre-groupping them by portfolio ID.
    /// </summary>
    public class OverlayTargetsBreakdown
    {
        private Dictionary<String, IGrouping<String, PortfolioSecurityTargetInfo>> map;
        
        protected OverlayTargetsBreakdown()
        {
            this.map = new Dictionary<String, IGrouping<String, PortfolioSecurityTargetInfo>>();
        }

        public OverlayTargetsBreakdown(IEnumerable<PortfolioSecurityTargetInfo> targets)
            : this()
        {
            foreach (var group in targets.GroupBy(x => x.PortfolioId))
            {
                this.map.Add(group.Key, group);
            }
        }

        public IEnumerable<PortfolioSecurityTargetInfo> GetTargets(String portfolioId)
        {
            IGrouping<String, PortfolioSecurityTargetInfo> found;
            if (this.map.TryGetValue(portfolioId, out found))
            {
                return found;
            }
            else
            {
                return No.PortfolioSecurityTargets;
            }
        }
    }
}
