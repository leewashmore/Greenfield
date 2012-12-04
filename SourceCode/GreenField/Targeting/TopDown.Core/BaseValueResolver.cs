using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;

namespace TopDown.Core
{
    /// <summary>
    /// Gets the saved base value (if present) for a given basket ID if found or nothing if not found.
    /// </summary>
    public class BaseValueResolver
    {
        private Dictionary<Int32, Decimal> map;

		public BaseValueResolver(IEnumerable<TargetingTypeBasketBaseValueInfo> values)
        {
            this.map = new Dictionary<Int32, Decimal>();
			foreach (var value in values)
			{
				this.map.Add(value.BasketId, value.BaseValue);
			}
		}

        public Decimal? GetBaseValue(Int32 basketId)
        {
            Decimal found;
            if (this.map.TryGetValue(basketId, out found))
            {
                return found;
            }
            else
            {
                return null;
            }
        }
    }
}
