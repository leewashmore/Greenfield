using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Web.Models
{
	public static class CalculationWithChangesetsExtender
	{
		public static IEnumerable<ChangesetInfoBase> GetChangesets(this CalculationWithChangesets info)
		{
			yield return info.BpstChangesetOpt;
			yield return info.PsfChangesetOpt;
			yield return info.PstChangesetOpt;
			yield return info.TtbbvChangesetOpt;
			yield return info.TtbptChangesetOpt;
			yield return info.TtgbsbvChangesetOpt;
		}


		public static String Render(this ChangesetInfoBase info)
		{
			var resolver = new Renderer_Resolver();
			info.Accept(resolver);
			return resolver.Result;
		}

		private class Renderer_Resolver : IChangesetInfoResolver
		{
			public String Result { get; private set; }
			public void Resolve(BgaPortfolioSecurityFactorChangesetInfo changesetInfo)
			{
				this.Result = "Portfolio/Security overlay";
			}
			public void Resolve(BasketPortfolioSecurityTargetChangesetInfo changesetInfo)
			{
				this.Result = "Basket/Security target";
			}
			public void Resolve(BuPortfolioSecurityTargetChangesetInfo changesetInfo)
			{
				this.Result = "Bottom-up-portfolio/Security target";
			}
			public void Resolve(TargetingTypeBasketBaseValueChangesetInfo changesetInfo)
			{
				this.Result = "Targeting-type/Basket base";
			}
			public void Resolve(TargetingTypeBasketPortfolioTargetChangesetInfo changesetInfo)
			{
				this.Result = "Targeting-type/Basket/Broad-global-active-portfolio target";
			}
			public void Resolve(TargetingTypeGroupBasketSecurityBaseValueChangesetInfo changesetInfo)
			{
				this.Result = "Targeting-type-group/Basket/Security base";
			}
		}
	}

	public class RecalculatePageModel
	{
		[DebuggerStepThrough]
		public RecalculatePageModel(IEnumerable<CalculationWithChangesets> calculations)
		{
			this.Calculations = calculations.ToArray();
		}

		public IEnumerable<CalculationWithChangesets> Calculations { get; private set; }
	}
}