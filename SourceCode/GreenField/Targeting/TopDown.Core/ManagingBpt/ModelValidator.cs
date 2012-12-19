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
                issues.AddRange(base.ValidateScope("Overlays", this.ValidateFactors(root.Factors)));
                issues.AddRange(this.ValidateGlobe(root.Globe, ticket));
                issues.AddRange(base.ValidateScope("Cash", this.ValidateCash(root.Cash, ticket)));
            });
        }

        protected IEnumerable<IValidationIssue> ValidateCash(CashModel cash, CalculationTicket ticket)
        {
            return cash.Base.Validate(ticket);
        }

        protected IEnumerable<IValidationIssue> ValidateFactors(Overlaying.RootModel overlay)
        {
            return overlay.Items.SelectMany(item =>
            {
                return base.ValidateScope(item.BottomUpPortfolio.Name, item.OverlayFactor.Validate());
            });
        }

        protected IEnumerable<IValidationIssue> ValidateGlobe(GlobeModel globe, CalculationTicket ticket)
        {
            var models = this.tarverser.TraverseGlobe(globe);
            return models.SelectMany(model =>
            {
                return this.ValidateOnceResolved(model, ticket);
            });
        }

        public IEnumerable<IValidationIssue> ValidateOnceResolved(IModel model, CalculationTicket ticket)
        {
            var resolver = new ValidateOnceResolved_IModelResolver(this, ticket);
            model.Accept(resolver);
            return resolver.Result;
        }

        private class ValidateOnceResolved_IModelResolver : IModelResolver
        {
            private ModelValidator validator;
            private CalculationTicket ticket;

            public ValidateOnceResolved_IModelResolver(ModelValidator validator, CalculationTicket ticket)
            {
                this.validator = validator;
                this.ticket = ticket;
            }

            public IEnumerable<IValidationIssue> Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.validator.ValidateBasketCountry(model, this.ticket);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.validator.ValidateBasketRegion(model, this.ticket);
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
                this.Result = this.validator.ValidateUnsavedBasketCountry(model, this.ticket);
            }
        }

        protected IEnumerable<IValidationIssue> ValidateBasketCountry(BasketCountryModel model, CalculationTicket ticket)
        {
            return base.ValidateScope(model.Basket.Country.Name, issues =>
            {
                var overlay = model.Overlay.Value(ticket);
                var baseValue = model.Base.Value(ticket);
                if (baseValue.HasValue)
                {
                    if (baseValue.Value < overlay)
                    {
                        model.Base.InjectProblems(new IValidationIssue[] { new WariningIssue("Base is less than overlay.") });
                    }
                }
                issues.AddRange(model.Base.Validate());
                issues.AddRange(model.PortfolioAdjustment.Validate());
            });
        }
        protected IEnumerable<IValidationIssue> ValidateBasketRegion(BasketRegionModel model, CalculationTicket ticket)
        {
            return base.ValidateScope(model.Basket.Name, issues =>
            {
                var overlay = model.Overlay.Value(ticket);
                var baseValue = model.Base.Value(ticket);
                if (baseValue.HasValue)
                {
                    if (baseValue.Value < overlay)
                    {
                        model.Base.InjectProblems(new IValidationIssue[] { new WariningIssue("Base is less than overlay.") });
                    }
                }
                issues.AddRange(model.Base.Validate());
                issues.AddRange(model.PortfolioAdjustment.Validate());
            });
        }
        protected IEnumerable<IValidationIssue> ValidateUnsavedBasketCountry(UnsavedBasketCountryModel model, CalculationTicket ticket)
        {
            return base.ValidateScope(model.Country.Name, issues =>
            {
                var overlay = model.Overlay.Value(ticket);
                var baseValue = model.Base.Value(ticket);
                if (baseValue.HasValue)
                {
                    if (baseValue.Value < overlay)
                    {
                        model.Base.InjectProblems(new IValidationIssue[] { new WariningIssue("Base is less than overlay.") });
                    }
                }
                issues.AddRange(model.Base.Validate());
                issues.AddRange(model.PortfolioAdjustment.Validate());
            });
        }
    }
}
