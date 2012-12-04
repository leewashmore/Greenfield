using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Persisting
{
	public interface IChangesetInfoResolver
	{
		void Resolve(BgaPortfolioSecurityFactorChangesetInfo changesetInfo);
		void Resolve(BasketPortfolioSecurityTargetChangesetInfo changesetInfo);
		void Resolve(BuPortfolioSecurityTargetChangesetInfo changesetInfo);
		void Resolve(TargetingTypeBasketBaseValueChangesetInfo changesetInfo);
		void Resolve(TargetingTypeBasketPortfolioTargetChangesetInfo changesetInfo);
		void Resolve(TargetingTypeGroupBasketSecurityBaseValueChangesetInfo changesetInfo);
	}
}
