using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Gadgets.BasketPicker
{
    public class ModelToJsonSerializer
    {
        public void Serialize(RootModel root, IJsonWriter writer)
        {
            writer.WriteArray(root.GetGroups(), "groups", group =>
            {
                writer.Write(delegate
                {
                    this.Serialize(group, writer);
                });
            });
        }

        public void Serialize(TargetingGroupModel group, IJsonWriter writer)
        {
            writer.Write(group.TargetingGroupId, "id");
            writer.Write(group.TargetingGroupName, "name");
            writer.WriteArray(group.GetBaskets(), "baskets", basket =>
            {
                writer.Write(delegate
                {
                    this.Serialize(basket, writer);
                });
            });
        }

		public void Serialize(BasketModel basket, IJsonWriter writer)
		{
			writer.Write(basket.Id, "id");
			writer.Write(basket.Name, "name");
		}
    }
}
