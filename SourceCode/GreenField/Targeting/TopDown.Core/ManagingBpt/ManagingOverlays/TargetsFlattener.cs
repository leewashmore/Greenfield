using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core.Overlaying
{
    public class TargetsFlattener
    {
        private InfoCopier copier;
        private TargetsTraverser<State> traverser;
        
        public class State
        {
            public Decimal ScaleFactor { get; set; }
        }
        
        public TargetsFlattener(InfoCopier copier)
        {
            this.copier = copier;
            this.traverser = new TargetsTraverser<State>(
                this.FlatTarget,
                this.DrillState
            );
        }

        public IEnumerable<BuPortfolioSecurityTargetInfo> Flatten(
			String portfolioId,
			Decimal target,
			ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository,
			ISecurityIdToPortfolioIdResolver portfolioRepository
		)
        {
            return this.traverser.Traverse(
				portfolioId,
                portfolioSecurityTargetRepository,
				new State { ScaleFactor = target },
				new HashSet<String>(),
				portfolioRepository
			);
        }

        private BuPortfolioSecurityTargetInfo FlatTarget(State state, BuPortfolioSecurityTargetInfo unscaledTarget)
        {
            var scaledTarget = this.copier.Copy(unscaledTarget);
            scaledTarget.Target *= state.ScaleFactor;
            return scaledTarget;
        }

        private State DrillState(State state, BuPortfolioSecurityTargetInfo scaledTarget)
        {
            return new State { ScaleFactor = scaledTarget.Target * state.ScaleFactor };
        }
    }
}
