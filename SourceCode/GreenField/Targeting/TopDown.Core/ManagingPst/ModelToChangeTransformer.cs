using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
    public class ModelToChangesetTransformer
    {
        public Changeset TryTransformToChangeset(String username, RootModel root)
        {
            var items = root.Items;
            var changes = this.TransformItems(root.PortfolioId, items);


            Changeset result;
            
            if (changes.Any())
            {
                result = new Changeset(
                    root.LatestChangeset,
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

        public IEnumerable<IPstChange> TransformItems(String portfolioId, IEnumerable<ItemModel> items)
        {
            var result = new List<IPstChange>();

            foreach (var item in items)
            {
                IPstChange change;
                if (item.Target.InitialValue.HasValue)
                {
                    // update or delete
                    if (item.Target.EditedValue.HasValue)
                    {
                        if (CalculationHelper.NoDifference(item.Target.InitialValue.Value, item.Target.EditedValue.Value))
                        {
                            continue;
                        }
                        else
                        {
                            // update
                            change = new PstUpdateChange(
                                portfolioId,
                                item.Security.Id,
                                item.Target.InitialValue.Value,
                                item.Target.EditedValue.Value,
                                item.Target.Comment
                            );
                        }
                    }
                    else
                    {
                        // delete
                        change = new PstDeleteChange(
                            portfolioId,
                            item.Security.Id,
                            item.Target.InitialValue.Value,
                            item.Target.Comment
                        );
                    }
                }
                else
                {
                    // insert or nothing
                    if (item.Target.EditedValue.HasValue)
                    {
                        // insert
                        change = new PstInsertChange(
                            portfolioId,
                            item.Security.Id,
                            item.Target.EditedValue.Value,
                            item.Target.Comment
                        );
                    }
                    else
                    {
                        // nothing, just skip this guy
                        continue;
                    }
                }
                result.Add(change);
            }

            return result;
        }
    }
}
