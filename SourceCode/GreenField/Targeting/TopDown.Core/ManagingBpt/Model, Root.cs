using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.Overlaying;
using Aims.Expressions;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core.ManagingBpt
{
    public class RootModel
    {
        [DebuggerStepThrough]
        public RootModel(
            TargetingType targetingType,
            BroadGlobalActivePortfolio portfolio,
            TargetingTypeBasketBaseValueChangesetInfo latestTtbbvChangesetInfo,
            TargetingTypeBasketPortfolioTargetChangesetInfo latestTtbptChangesetInfo,
            BgaPortfolioSecurityFactorChangesetInfo latestPstoChangesetInfo,
            BuPortfolioSecurityTargetChangesetInfo latestPstChangesetInfo,
            GlobeModel globe,
            CashModel cash,
            Overlaying.RootModel factors,
            IExpression<Decimal?> portfolioScaledTotalExpression,
            DateTime benchmarkDate
        )
        {
            this.TargetingType = targetingType;
            this.Portfolio = portfolio;
            this.LatestTtbbvChangeset = latestTtbbvChangesetInfo;
            this.LatestTtbptChangeset = latestTtbptChangesetInfo;
            this.LatestPstoChangeset = latestPstoChangesetInfo;
            this.LatestPstChangeset = latestPstChangesetInfo;
            this.Globe = globe;
            this.Cash = cash;
            this.Factors = factors;
            this.PortfolioScaledTotal = portfolioScaledTotalExpression;
            this.BenchmarkDate = benchmarkDate;
        }

        public TargetingType TargetingType { get; private set; }
        public BroadGlobalActivePortfolio Portfolio { get; private set; }
        public TargetingTypeBasketBaseValueChangesetInfo LatestTtbbvChangeset { get; private set; }
        public TargetingTypeBasketPortfolioTargetChangesetInfo LatestTtbptChangeset { get; private set; }
        public BgaPortfolioSecurityFactorChangesetInfo LatestPstoChangeset { get; private set; }
        public BuPortfolioSecurityTargetChangesetInfo LatestPstChangeset { get; private set; }
        public GlobeModel Globe { get; private set; }
        public CashModel Cash { get; private set; }
        public Overlaying.RootModel Factors { get; private set; }
        public IExpression<Decimal?> PortfolioScaledTotal { get; private set; }
        public DateTime BenchmarkDate { get; private set; }
    }
}