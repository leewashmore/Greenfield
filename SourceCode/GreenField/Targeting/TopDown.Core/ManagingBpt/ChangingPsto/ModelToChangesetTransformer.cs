using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.ChangingPsto
{
    /// <summary>
    /// Scrapes overlay numbers into a changeset.
    /// </summary>
    public class ModelToChangesetTransformer
    {
        public Changeset TryTransformToChangeset(RootModel model, String username)
        {
            var changes = model.Factors.Items
                .Select(x => this.TryTransformToChange(x))
                .Where(x => x != null);

            Changeset result;
            if (changes.Any())
            {
                result = new Changeset(model.Portfolio.Id, model.LatestPstoChangeset, username, changes);
            }
            else
            {
                result = null;
            }
            return result;
        }

        protected IChange TryTransformToChange(Overlaying.ItemModel model)
        {
            var value = model.OverlayFactor;
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
							model.BottomUpPortfolio.Fund != null ? model.BottomUpPortfolio.Fund.Id : model.BottomUpPortfolio.Id,
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
                        model.BottomUpPortfolio.Fund != null ? model.BottomUpPortfolio.Fund.Id : model.BottomUpPortfolio.Id,
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
                        model.BottomUpPortfolio.Fund != null ? model.BottomUpPortfolio.Fund.Id : model.BottomUpPortfolio.Id,
                        value.EditedValue.Value,
                        value.Comment
                    );
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
