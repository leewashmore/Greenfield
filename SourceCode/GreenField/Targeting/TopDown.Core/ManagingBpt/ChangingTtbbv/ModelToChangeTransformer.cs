using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.ChangingTtbbv
{
	public class ModelToChangesetTransformer
	{
		private GlobeTraverser traverser;
		public ModelToChangesetTransformer(GlobeTraverser traverser)
		{
			this.traverser = traverser;
		}

		/// <summary>
		/// Changeset isn't guaranteed because ther could be no changes.
		/// </summary>
		public Changeset TryTransformToChangeset(RootModel root, String username)
		{
			var models = this.traverser.TraverseGlobe(root.Globe);
			var changes = models
				.Select(model => this.TryTransformToChangeOnceResolved(root.TargetingType.Id, model))
				.Where(model => model != null);

			Changeset result;
			if (changes.Any())
			{
				result = new Changeset(
					root.TargetingType.Id,
					root.LatestTtbbvChangeset,
					username,
					changes
				);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public IChange TryTransformToChangeOnceResolved(Int32 targetingTypeId, IModel model)
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

			public void Resolve(UnsavedBasketCountryModel model)
			{
				throw new ApplicationException("There can be no unsaved baskets by this moment. All baskets need to be saved already.");
			}
		}

		public IChange TryTransformToChange(BasketCountryModel model)
		{
			var result = this.TransformToChange(model.Basket.Id, model.Base);
			return result;
		}

		public IChange TryTransformToChange(BasketRegionModel model)
		{
			var result = this.TransformToChange(model.Basket.Id, model.Base);
			return result;
		}

		protected IChange TransformToChange(Int32 basketId, EditableExpression value)
		{
			if (value.InitialValue.HasValue)
			{
				if (value.EditedValue.HasValue)
				{
					if (CalculationHelper.NoDifference(value.InitialValue.Value, value.EditedValue.Value))
					{
						return null;
					}
					else
					{
						// update
						return new UpdateChange(
							basketId,
							value.InitialValue.Value,
							value.EditedValue.Value,
							value.Comment
						);
					}
				}
				else
				{
					// delete
					return new DeleteChange(
						basketId,
						value.InitialValue.Value,
						value.Comment
					);
				}
			}
			else
			{
				if (value.EditedValue.HasValue)
				{
					// insert
					return new InsertChange(
						basketId,
						value.EditedValue.Value,
						value.Comment
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
