using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpst.ChangingTtgbsbv
{
	public class ModelToChangesetTransformer
	{
		/// <summary>
		/// Changeset isn't guaranted, because there could be no changes at all.
		/// </summary>
		public Changeset TryTransformToChangeset(String username, RootModel root)
		{
			var changes = root.Core.Securities
				.Select(x => this.TryTransformToChange(x))
				.Where(x => x != null).ToArray();

			Changeset result;

			if (changes.Any())
			{
				result = new Changeset(
					root.Core.TargetingTypeGroup.Id,
					root.Core.Basket.Id,
					root.LatestBaseChangeset,
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

		public IChange TryTransformToChange(SecurityModel model)
		{
			var value = model.Base;
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
							model.Security.Id,
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
						model.Security.Id,
						value.InitialValue.Value,
						value.Comment
					);
				}
			}
			else
			{
				if (model.Base.EditedValue.HasValue)
				{
					// insert
					return new InsertChange(
						model.Security.Id,
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
