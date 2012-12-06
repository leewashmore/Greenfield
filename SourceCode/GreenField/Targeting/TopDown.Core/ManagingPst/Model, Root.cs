using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
    /// <summary>
    /// Represents a portfolio-security-target composition.
    /// </summary>
    public class RootModel : Repository<ItemModel>
    {
        [DebuggerStepThrough]
        public RootModel(
			String porfolioId,
			BuPortfolioSecurityTargetChangesetInfo latestChangesetInfo,
            IEnumerable<ItemModel> items,
            NullableSumExpression targetTotalExpression,
            IExpression<Decimal?> cashExpression
		)
        {
            this.PortfolioId = porfolioId;
            this.LatestChangeset = latestChangesetInfo;
            this.Items = items.ToList();
            this.TargetTotal = targetTotalExpression;
            this.Cash = cashExpression;
        }

        public String PortfolioId { get; private set; }
        public BuPortfolioSecurityTargetChangesetInfo LatestChangeset { get; private set; }
        public IEnumerable<ItemModel> Items { get; private set; }
        public NullableSumExpression TargetTotal { get; private set; }
        public IExpression<Decimal?> Cash { get; private set; }
    }
}
