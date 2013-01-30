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
            IExpression<Decimal?> portfolioScaledGrandTotalExpression,
            IExpression<Decimal?> trueExposureGrandTotal,
            IExpression<Decimal?> trueActiveGrandTotal,
            DateTime benchmarkDate,
            Boolean isUserPermittedToSave
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
            this.PortfolioScaledGrandTotal = portfolioScaledGrandTotalExpression;
            this.TrueExposureGrandTotal = trueExposureGrandTotal;
            this.TrueActiveGrandTotal = trueActiveGrandTotal;
            this.BenchmarkDate = benchmarkDate;
            this.IsUserPermittedToSave = isUserPermittedToSave;
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
        public IExpression<Decimal?> PortfolioScaledGrandTotal { get; private set; }
        public DateTime BenchmarkDate { get; private set; }
        public IExpression<Decimal?> TrueExposureGrandTotal { get; private set; }
        public IExpression<Decimal?> TrueActiveGrandTotal { get; private set; }

        public Boolean IsUserPermittedToSave { get; private set; }
    }
}