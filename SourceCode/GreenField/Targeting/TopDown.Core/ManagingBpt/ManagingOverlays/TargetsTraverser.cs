using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core.Overlaying
{
    public class TargetsTraverser<TState>
    {
        private Func<TState, BuPortfolioSecurityTargetInfo, BuPortfolioSecurityTargetInfo> targetFlattener;
        private Func<TState, BuPortfolioSecurityTargetInfo, TState> stateDriller;

        public TargetsTraverser(
            Func<TState, BuPortfolioSecurityTargetInfo, BuPortfolioSecurityTargetInfo> targetFlattener,
            Func<TState, BuPortfolioSecurityTargetInfo, TState> stateDriller
        )
        {
            this.targetFlattener = targetFlattener;
            this.stateDriller = stateDriller;
        }

        public IEnumerable<BuPortfolioSecurityTargetInfo> Traverse(
            String portfolioId,
            ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository,
            TState state,
            HashSet<String> processedPortfolioIds,
			ISecurityIdToPortfolioIdResolver portfolioRepository
        )
        {
            // in order not to process the same portfolio infinitely many times we have to save it
            processedPortfolioIds.Add(portfolioId);

            // get only targets related to the portfolo in question
            var targets = portfolioSecurityTargetRepository.GetTargets(portfolioId, true);
            foreach (var target in targets)
            {
				var nestedPortfolioIdOpt = portfolioRepository.TryResolveToPortfolioId(target.SecurityId);
                if (nestedPortfolioIdOpt == null)
                {
                    var convertedTarget = this.targetFlattener(state, target);
                    yield return convertedTarget;
                }
                else if (processedPortfolioIds.Contains(nestedPortfolioIdOpt))
                {
                    throw new ApplicationException("Cannot unwrap portfolio/security composition for the overlay column due to getting into a infinite loop: portfolio \"" + nestedPortfolioIdOpt + "\" has already been processed.");
                }
                else
                {
                    var nestedTargets = this.Traverse(
                        nestedPortfolioIdOpt,
                        portfolioSecurityTargetRepository,
                        stateDriller(state, target),
                        processedPortfolioIds,
						portfolioRepository
                    );

                    foreach (var nestedTarget in nestedTargets)
                    {
                        yield return nestedTarget;
                    }
                }
            }
        }
    }
}
