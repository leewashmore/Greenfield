using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
	/// <summary>
	/// Traverses the model and check that everything is valid.
	/// </summary>
	public class ModelValidator : ValidatorBase
	{
		private GlobeTraverser tarverser;

		[DebuggerStepThrough]
		public ModelValidator(GlobeTraverser traverser)
		{
			this.tarverser = traverser;
		}

		public IEnumerable<IValidationIssue> ValidateRoot(RootModel root, CalculationTicket ticket)
		{
			return base.ValidateScope(issues =>
			{
				issues.AddRange(base.ValidateScope("Overlays", this.Validate(root.Factors)));
				issues.AddRange(this.Validate(root.Globe));
				issues.AddRange(base.ValidateScope("Cash", this.Validate(root.Cash, ticket)));
			});
		}

		protected IEnumerable<IValidationIssue> Validate(CashModel cash, CalculationTicket ticket)
		{
			return cash.Base.Validate(ticket);
		}

		protected IEnumerable<IValidationIssue> Validate(Overlaying.RootModel overlay)
		{
			return overlay.Items.SelectMany(item =>
			{
				return base.ValidateScope(item.BottomUpPortfolio.Name, item.OverlayFactor.Validate());
			});
		}

		protected IEnumerable<IValidationIssue> Validate(GlobeModel globe)
		{
			var models = this.tarverser.TraverseGlobe(globe);
			return models.SelectMany(model =>
			{
				return this.ValidateOnceResolved(model);
			});
		}

		public IEnumerable<IValidationIssue> ValidateOnceResolved(IModel model)
		{
			var resolver = new ValidateOnceResolved_IModelResolver(this);
			model.Accept(resolver);
			return resolver.Result;
		}

		private class ValidateOnceResolved_IModelResolver : IModelResolver
		{
			private ModelValidator validator;

			public ValidateOnceResolved_IModelResolver(ModelValidator validator)
			{
				this.validator = validator;
			}
			
			public  IEnumerable<IValidationIssue> Result { get; private set; }

			public void Resolve(BasketCountryModel model)
			{
				this.Result = this.validator.ValidateBasketCountry(model);
			}

			public void Resolve(BasketRegionModel model)
			{
				this.Result = this.validator.ValidateBasketRegion(model);
			}

			public void Resolve(CountryModel model)
			{
				this.Result = No.ValidationIssues;
			}

			public void Resolve(OtherModel model)
			{
				this.Result = No.ValidationIssues;
			}

			public void Resolve(RegionModel model)
			{
				this.Result = No.ValidationIssues;
			}

			public void Resolve(UnsavedBasketCountryModel model)
			{
				this.Result = this.validator.ValidateUnsavedBasketCountry(model);
			}
		}

		protected IEnumerable<IValidationIssue> ValidateBasketCountry(BasketCountryModel model)
		{
			return base.ValidateScope(model.Basket.Country.Name, issues =>
			{
				issues.AddRange(model.Base.Validate());
				issues.AddRange(model.PortfolioAdjustment.Validate());
			});
		}
		protected IEnumerable<IValidationIssue> ValidateBasketRegion(BasketRegionModel model)
		{
			return base.ValidateScope(model.Basket.Name, issues =>
			{
				issues.AddRange(model.Base.Validate());
				issues.AddRange(model.PortfolioAdjustment.Validate());
			});
		}
		protected IEnumerable<IValidationIssue> ValidateUnsavedBasketCountry(UnsavedBasketCountryModel model)
		{
			return base.ValidateScope(model.Country.Name, issues =>
			{
				issues.AddRange(model.Base.Validate());
				issues.AddRange(model.PortfolioAdjustment.Validate());
			});
		}
	}
}
