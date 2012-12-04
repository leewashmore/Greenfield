using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.ChangingTtbpt
{
    /// <summary>
    /// Portfolio adjustments scraper.
    /// </summary>
    public class ModelToChangesetTransformer
    {
        private GlobeTraverser traverser;

        [DebuggerStepThrough]
        public ModelToChangesetTransformer(GlobeTraverser traverser)
        {
            this.traverser = traverser;
        }

        /// <summary>
        /// Scrapes a changeset with portfolio adjustments.
        /// Changeset isn't guaranteed because changeset may not have any changes at all.
        /// </summary>
        public Changeset TryTransformToChangeset(RootModel model, String username)
        {
            var models = this.traverser.TraverseGlobe(model.Globe);

            IEnumerable<IChange> changes = models
                .Select(x => this.TryTransformToChangeOnceResolved(x))
                .Where(x => x != null);

            Changeset result;
            if (changes.Any())
            {
                result = new Changeset(model.LatestTtbptChangeset, model.TargetingType.Id, model.Portfolio.Id, username, changes);
            }
            else
            {
                result = null;
            }
            return result;
        }

        protected IChange TryTransformToChangeOnceResolved(IModel model)
        {
            var resolver = new TryTransformToChangeOnceResolved_IModelResolver(this);
            model.Accept(resolver);
            return resolver.ResultOpt;
        }

        private class TryTransformToChangeOnceResolved_IModelResolver : IModelResolver
        {
            private ModelToChangesetTransformer transformer;

            public TryTransformToChangeOnceResolved_IModelResolver(ModelToChangesetTransformer transformer)
            {
                this.transformer = transformer;
            }
            
            public IChange ResultOpt { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.ResultOpt = this.transformer.TryTransformToChange(model);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.ResultOpt = this.transformer.TryTransformToChange(model);
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.ResultOpt = this.transformer.TryTransformToChange(model);
            }

            public void Resolve(CountryModel model)
            {
                this.ResultOpt = null;
            }

            public void Resolve(OtherModel model)
            {
                this.ResultOpt = null;
            }

            public void Resolve(RegionModel model)
            {
                this.ResultOpt = null;
            }
        }

        protected IChange TryTransformToChange(BasketCountryModel model)
        {
            return this.TryTransformToChange(model.Basket.Id, model.PortfolioAdjustment);
        }

        protected IChange TryTransformToChange(BasketRegionModel model)
        {
            return this.TryTransformToChange(model.Basket.Id, model.PortfolioAdjustment);
        }

        protected IChange TryTransformToChange(UnsavedBasketCountryModel model)
        {
            var basketIdOpt = model.BasketId;
            if (basketIdOpt.HasValue)
            {
                return this.TryTransformToChange(basketIdOpt.Value, model.PortfolioAdjustment);
            }
            else
            {
                throw new ApplicationException("Unsaved basket for the country \"" + model.Country.IsoCode + "\" hasn't gotten its ID yet.");
            }
        }


        protected IChange TryTransformToChange(Int32 basketId, EditableExpression portfolioAdustment)
        {
            if (portfolioAdustment.InitialValue.HasValue)
            {
                if (portfolioAdustment.EditedValue.HasValue)
                {
					if (CalculationHelper.NoDifference(portfolioAdustment.InitialValue.Value, portfolioAdustment.EditedValue.Value))
					{
						return null;
					}
					else
					{
						// update
						return new UpdateChange(
							basketId,
							portfolioAdustment.InitialValue.Value,
							portfolioAdustment.EditedValue.Value,
							portfolioAdustment.Comment
						);
					}
                }
                else
                {
                    // delete
                    return new DeleteChange(
                        basketId,
                        portfolioAdustment.InitialValue.Value,
                        portfolioAdustment.Comment
                    );
                }
            }
            else
            {
                if (portfolioAdustment.EditedValue.HasValue)
                {
                    // insert
                    return new InsertChange(
                        basketId,
                        portfolioAdustment.EditedValue.Value,
                        portfolioAdustment.Comment
                    );
                }
                else
                {
                    // nothing
                    return null;
                }
            }
        }
    }
}
