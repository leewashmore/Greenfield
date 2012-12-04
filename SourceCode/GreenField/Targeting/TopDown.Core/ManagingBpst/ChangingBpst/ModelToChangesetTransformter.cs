using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpst.ChangingBpst
{
    public class ModelToChangesetTransformter
    {
        public Changeset TryTransformToChangeset(String username, RootModel model)
        {
            var changes = model.Core.Securities.SelectMany(
                x => x.PortfolioTargets
                    .Select(y => this.TryTransformToChange(x.Security.Id, y))
                    .Where(y => y != null)
            ).ToArray();

            Changeset result;
            
            if (changes.Any())
            {
                result = new Changeset(
                    model.LatestPortfolioTargetChangeset,
                    model.Core.Basket.Id,
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

        public IChange TryTransformToChange(String securityId, PortfolioTargetModel model)
        {
			var value = model.Target;
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
							model.BroadGlobalActivePortfolio.Id,
							securityId,
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
                        model.BroadGlobalActivePortfolio.Id,
                        securityId,
                        value.InitialValue.Value,
						value.Comment
                    );
                }
            }
            else
            {
                if (model.Target.EditedValue.HasValue)
                {
                    // insert
                    return new InsertChange(
                        model.BroadGlobalActivePortfolio.Id,
                        securityId,
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
