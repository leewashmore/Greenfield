using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingBpt
{
	public class PortfolioAdjustmentInitializer
	{
		private GlobeTraverser traverser;
		
		[DebuggerStepThrough]
		public PortfolioAdjustmentInitializer(GlobeTraverser traverser)
		{
			this.traverser = traverser;
		}

		public void InitializePortfolioAdjustments(RootModel root, IDataManager manager)
		{
			var values = manager.GetTargetingTypeBasketPortfolioTarget(
				root.TargetingType.Id,
				root.Portfolio.Id
			);

			var valuesByBasketId = values.ToDictionary(x => x.BasketId);
			var models = this.traverser.TraverseGlobe(root.Globe);
			foreach (var model in models)
			{
				this.InitializeModelOnceResolved(model, valuesByBasketId);
			}
		}

		protected void InitializeModelOnceResolved(IModel model, Dictionary<Int32, TargetingTypeBasketPortfolioTargetInfo> valuesByBasketId)
		{
			var resolver = new InitializeModelOnceResolved_IModelResolver(this, valuesByBasketId);
			model.Accept(resolver);
		}

		private class InitializeModelOnceResolved_IModelResolver : IModelResolver
		{
			private PortfolioAdjustmentInitializer initializer;
			private Dictionary<Int32, TargetingTypeBasketPortfolioTargetInfo> valuesByBasketId;

			public InitializeModelOnceResolved_IModelResolver(
				PortfolioAdjustmentInitializer initializer,
				Dictionary<Int32, TargetingTypeBasketPortfolioTargetInfo> valuesByBasketId)
			{
				this.initializer = initializer;
				this.valuesByBasketId = valuesByBasketId;
			}

			public void Resolve(BasketCountryModel model)
			{
				this.initializer.InitializeBasketCountry(model, this.valuesByBasketId);
			}

			public void Resolve(BasketRegionModel model)
			{
				this.initializer.InitializeBasketRegion(model, this.valuesByBasketId);
			}

			public void Resolve(CountryModel model)
			{
				// do nonthing
			}

			public void Resolve(OtherModel model)
			{
				// do nonthing
			}

			public void Resolve(RegionModel model)
			{
				// do nonthing
			}

			public void Resolve(UnsavedBasketCountryModel model)
			{
				// do nothing, there can be no values for unsaved baskets
			}
		}

		protected void InitializeBasketCountry(
			BasketCountryModel model,
			Dictionary<Int32, TargetingTypeBasketPortfolioTargetInfo> dictionary)
		{
			TargetingTypeBasketPortfolioTargetInfo info;
			if (dictionary.TryGetValue(model.Basket.Id, out info))
			{
				model.PortfolioAdjustment.InitialValue = info.Target;
			}
		}

		protected void InitializeBasketRegion(
			BasketRegionModel model,
			Dictionary<Int32, TargetingTypeBasketPortfolioTargetInfo> dictionary)
		{
			TargetingTypeBasketPortfolioTargetInfo info;
			if (dictionary.TryGetValue(model.Basket.Id, out info))
			{
				model.PortfolioAdjustment.InitialValue = info.Target;
			}
		}

	}
}
