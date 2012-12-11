using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingBaskets;
using TInfo = TopDown.Core.Persisting.TargetingTypeGroupBasketSecurityBaseValueInfo;
using TopDown.Core.ManagingSecurities;
using Aims.Core;

namespace TopDown.Core.ManagingBpst
{
	public class TargetingTypeGroupBasketSecurityBaseValueRepository
	{
		private static readonly TInfo[] nothing = new TInfo[] { };
		private Dictionary<Int32, IEnumerable<TInfo>> byTargetingTypeGroup;
		private Dictionary<Int32, Dictionary<Int32, IEnumerable<TInfo>>> byTargetingTypeGroupByBasket;

		public TargetingTypeGroupBasketSecurityBaseValueRepository(IEnumerable<TInfo> baseValues)
		{
			this.byTargetingTypeGroup = baseValues
				.GroupBy(x => x.TargetingTypeGroupId)
				.ToDictionary(x => x.Key, x => x.Select(y => y));

			this.byTargetingTypeGroupByBasket = this.byTargetingTypeGroup
				.ToDictionary(
					x => x.Key,
					x => x.Value.GroupBy(y => y.BasketId).ToDictionary(y => y.Key, y => y.Select(z => z))
				);
		}

		public IEnumerable<TInfo> GetBaseValues(TargetingTypeGroup targetingTypeGroup, IBasket basket)
		{
			Dictionary<Int32, IEnumerable<TInfo>> found;
			if (!this.byTargetingTypeGroupByBasket.TryGetValue(targetingTypeGroup.Id, out found)) return nothing;

			IEnumerable<TInfo> result;
			if (!found.TryGetValue(basket.Id, out result)) return nothing;
			return result;
		}

		public TInfo TryGetBaseValue(TargetingTypeGroup targetingTypeGroup, IBasket basket, ISecurity security)
		{
			var foundOpt = this
				.GetBaseValues(targetingTypeGroup, basket)
				.Where(x => x.SecurityId == security.Id)
				.FirstOrDefault();
			return foundOpt;
		}
	}
}
