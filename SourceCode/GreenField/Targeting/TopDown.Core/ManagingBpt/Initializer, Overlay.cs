using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingPortfolios;

namespace TopDown.Core.ManagingBpt
{
	public class OverlayInitializer
	{
		private GlobeTraverser traverser;
		private TargetsFlattener targetsFlattener;

		public OverlayInitializer(
			GlobeTraverser traverser,
			TargetsFlattener targetsFlattener
		)
		{
			this.traverser = traverser;
			this.targetsFlattener = targetsFlattener;
		}

		public void Initialize(
			RootModel root,
			SecurityRepository securityRepository,
			PortfolioRepository portfolioRepository,
            ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository
		)
		{
            var allFlatTargets = new List<BuPortfolioSecurityTargetInfo>();
            foreach (var overlayItem in root.Factors.Items)
            {
                var factor = overlayItem.OverlayFactor.EditedValue;
                if (!factor.HasValue) continue;

                var flatTargets = this.targetsFlattener.Flatten(
                    overlayItem.BottomUpPortfolio.Id,
                    factor.Value,
                    portfolioSecurityTargetRepository,
                    portfolioRepository
                ).ToArray();

                allFlatTargets.AddRange(flatTargets);
                var s = allFlatTargets.Sum(a => a.Target);
            }
            var resolver = new IsoCodeToOverlayTargetValueResolver(securityRepository, allFlatTargets);
			this.Initialize(root, resolver);
		}

		public void Initialize(RootModel root, IsoCodeToOverlayTargetValueResolver valueResolver)
		{
            //var s =
			var models = this.traverser.TraverseGlobe(root.Globe);
			foreach (var model in models)
			{
				this.InitializeOnceResolved(model, valueResolver);
			}
		}

		protected void InitializeOnceResolved(IModel model, IsoCodeToOverlayTargetValueResolver valueResolver)
		{
			var resolver = new InitializeOnceResolved_IModelResolver(this, valueResolver);
			model.Accept(resolver);
		}

		private class InitializeOnceResolved_IModelResolver : IModelResolver
		{
			private IsoCodeToOverlayTargetValueResolver valueResolver;
			private OverlayInitializer initializer;

			public InitializeOnceResolved_IModelResolver(OverlayInitializer initializer, IsoCodeToOverlayTargetValueResolver valueResolver)
			{
				this.valueResolver = valueResolver;
				this.initializer = initializer;
			}

			public void Resolve(BasketCountryModel model)
			{
				this.initializer.Initialize(model, valueResolver);
			}

			public void Resolve(BasketRegionModel model)
			{
				// do nothing (it is based on CountryIso)
			}

			public void Resolve(CountryModel model)
			{
				this.initializer.Initialize(model, valueResolver);
			}

			public void Resolve(OtherModel model)
			{
				// do nothing
			}

			public void Resolve(RegionModel model)
			{
				// do nothing
			}

			public void Resolve(UnsavedBasketCountryModel model)
			{
				this.initializer.Initialize(model, valueResolver);
			}
		}

		protected void Initialize(BasketCountryModel model, IsoCodeToOverlayTargetValueResolver valueResolver)
		{
			model.Overlay.InitialValue = valueResolver.ResolveOverlayTargetValue(model.Basket.Country.IsoCode, model.Overlay.DefaultValue);
		}

		protected void Initialize(UnsavedBasketCountryModel model, IsoCodeToOverlayTargetValueResolver valueResolver)
		{
			model.Overlay.InitialValue = valueResolver.ResolveOverlayTargetValue(model.Country.IsoCode, model.Overlay.DefaultValue);
		}

		protected void Initialize(CountryModel model, IsoCodeToOverlayTargetValueResolver valueResolver)
		{
			model.Overlay.InitialValue = valueResolver.ResolveOverlayTargetValue(model.Country.IsoCode, model.Overlay.DefaultValue);
		}
	}
}
