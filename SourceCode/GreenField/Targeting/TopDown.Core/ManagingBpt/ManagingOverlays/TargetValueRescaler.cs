using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;

namespace TopDown.Core.Overlaying
{
    /// <summary>
    /// Rescales target values inside each of the portfolios.
    /// </summary>
    public class TargetValueRescaler
    {
        private InfoCopier copier;
        
        public TargetValueRescaler(InfoCopier copier)
        {
            this.copier = copier;
        }
        
        public IEnumerable<BuPortfolioSecurityTargetInfo> Rescale(IEnumerable<BuPortfolioSecurityTargetInfo> unrescaledTargets)
        {
            var result = new List<BuPortfolioSecurityTargetInfo>();

            var groupped = unrescaledTargets.GroupBy(x => x.BottomUpPortfolioId).Select(x => new { PortfolioId = x.Key, Total = x.Sum(y => y.Target) });
            var map = new Dictionary<String, Decimal>();
            
            foreach (var group in groupped)
            {
                map.Add(group.PortfolioId, group.Total);
            }

            foreach (var target in unrescaledTargets)
            {
                var total = map[target.BottomUpPortfolioId];
                var rescaledTarget = this.copier.Copy(target);
                rescaledTarget.Target = target.Target / total;
                result.Add(rescaledTarget);
            }

            return result;
        }
    }
}
