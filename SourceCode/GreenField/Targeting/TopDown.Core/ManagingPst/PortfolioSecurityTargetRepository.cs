using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingPst
{
    public class PortfolioSecurityTargetRepository
    {
        private InfoCopier copier;
        private Dictionary<String, IGrouping<String, BuPortfolioSecurityTargetInfo>> targetByPortfolioId;

        public PortfolioSecurityTargetRepository(InfoCopier copier, IEnumerable<BuPortfolioSecurityTargetInfo> targetInfos)
        {
            this.copier = copier;
            this.targetByPortfolioId = targetInfos.GroupBy(x => x.BottomUpPortfolioId).ToDictionary(x => x.Key);
        }

        public IEnumerable<BuPortfolioSecurityTargetInfo> GetTargets(String portfolioId, Boolean rescaled)
        {
            IGrouping<String, BuPortfolioSecurityTargetInfo> found;
            if (this.targetByPortfolioId.TryGetValue(portfolioId, out found))
            {
                if (found.Any())
                {
                    if (rescaled)
                    {
                        var total = found.Sum(x => x.Target);
                        return found.Select(x =>
                        {
                            var copy = this.copier.Copy(x);
                            copy.Target = copy.Target / total;
                            return copy;
                        });
                    }
                    else
                    {
                        return found;
                    }
                }
                else
                {
                    return found;
                }
            }
            else
            {
                return No.PortfolioSecurityTargets;
            }
        }
    }
}
