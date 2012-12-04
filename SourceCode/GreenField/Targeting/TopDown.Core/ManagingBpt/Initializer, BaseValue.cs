using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBpt;
using TopDown.Core.Persisting;

namespace TopDown.Core
{
	/// <summary>
	/// Populates base values in a breakdown model by matching them using BasketId.
	/// For yet unsaved country baskets this won't work, so they need to be initialized separately.
	/// </summary>
	public class BaseValueInitializer
	{
		private GlobeTraverser traverser;

		public BaseValueInitializer(GlobeTraverser traverser)
		{
			this.traverser = traverser;
		}

		public void Initialize(RootModel root, BaseValueResolver valueResolver)
		{
			var models = this.traverser.TraverseGlobe(root.Globe);
			foreach (var model in models)
			{
				this.InitializeOnceResolved(model, valueResolver);
			}
		}

		public void InitializeOnceResolved(IModel model, BaseValueResolver valueResolver)
		{
			model.Accept(new InitializeMultimethod(this, valueResolver));
		}

		private class InitializeMultimethod : IModelResolver
		{
			private BaseValueResolver valueResolver;
			private BaseValueInitializer initializer;

			public InitializeMultimethod(BaseValueInitializer initializer, BaseValueResolver valueResolver)
			{
				this.valueResolver = valueResolver;
				this.initializer = initializer;
			}

			public void Resolve(BasketCountryModel model)
			{
                model.Base.InitialValue = this.valueResolver.GetBaseValue(model.Basket.Id);
			}

			public void Resolve(BasketRegionModel model)
			{
                model.Base.InitialValue = this.valueResolver.GetBaseValue(model.Basket.Id);
			}

			public void Resolve(CountryModel model)
			{
				// do nothing
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
				// do nothing (there is no basket Id assigned to model yet)
			}
		}
	}
}
