using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;
using TopDown.Core.Persisting;

namespace TopDown.Core.Gadgets.BasketPicker
{
	public class TargetingGroupModel : Repository<BasketModel>
	{
		[DebuggerStepThrough]
		public TargetingGroupModel(Int32 targetingGroupId, String targetingGroupName, IEnumerable<BasketModel> baskets, IEnumerable<UsernameBasketInfo> userBaskets)
		{
			this.TargetingGroupId = targetingGroupId;
			this.TargetingGroupName = targetingGroupName;
            foreach (var basket in baskets)
            {
                foreach(var userBasket in userBaskets)
                {
                    if (userBasket.BasketId == basket.Id)
                    {
                        this.RegisterValue(basket);
                        break;
                    }
                }
            }
		}
		
		public Int32 TargetingGroupId { get; private set; }
		public String TargetingGroupName { get; private set; }

		[DebuggerStepThrough]
		public IEnumerable<BasketModel> GetBaskets()
		{
			return base.GetValues();
		}

		
	}
}
