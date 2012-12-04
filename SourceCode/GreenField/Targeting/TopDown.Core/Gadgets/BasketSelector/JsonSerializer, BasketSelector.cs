using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Gadgets.BasketSelector
{
	public class BasketSelectorJsonSerializer
	{
		public void RootToJson(IJsonWriter writer, RootModel root)
		{
			writer.Write(root.Groups, group =>
			{
				writer.Write(delegate
				{
					this.GroupToJson(writer, group);
				});
			});
		}

		public void GroupToJson(IJsonWriter writer, TargetingTypeGroupModel group)
		{
			writer.Write(group.Id, "id");
			writer.Write(group.Name, "name");
			writer.Write(group.Baskets, basket =>
			{
				writer.Write(delegate
				{
					writer.Write(basket.Id, "id");
					writer.Write(basket.Name, "name");
				});
			});
		}
	}
}
