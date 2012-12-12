using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingBaskets;
using TInfo = TopDown.Core.Persisting.BasketPortfolioSecurityTargetInfo;
using TopDown.Core.ManagingSecurities;
using Aims.Core;

namespace TopDown.Core.ManagingBpst
{
    public class BasketSecurityPortfolioTargetRepository
    {
		private readonly static TInfo[] nothing = new TInfo[] {};

		private Dictionary<Int32, IEnumerable<TInfo>> byBasket;
		private Dictionary<Int32, Dictionary<String, IEnumerable<TInfo>>> byBasketByPortfolio;
		private Dictionary<Int32, Dictionary<String, IEnumerable<TInfo>>> byBasketBySecurity;

		public BasketSecurityPortfolioTargetRepository(IEnumerable<TInfo> targetInfos)
        {
            this.byBasket = targetInfos
                .GroupBy(x => x.BasketId)
                .ToDictionary(x => x.Key, x => x.Select(y => y));

            this.byBasketByPortfolio = this
                .byBasket.ToDictionary(
                    x => x.Key,
                    x => x.Value.GroupBy(y => y.PortfolioId).ToDictionary(
                        y => y.Key, y => y.Select(z => z)
                ));

            this.byBasketBySecurity = this
                .byBasket.ToDictionary(
                    x => x.Key,
                    x => x.Value.GroupBy(y => y.SecurityId).ToDictionary(
                        y => y.Key, y => y.Select(z => z)
                ));
        }

		public IEnumerable<TInfo> GetTargets(IBasket basket)
        {
			IEnumerable<TInfo> found;
			if (!this.byBasket.TryGetValue(basket.Id, out found)) return nothing;
			return found;
        }

		public IEnumerable<TInfo> GetTargets(IBasket basket, ISecurity security)
        {
			Dictionary<String, IEnumerable<TInfo>> byBasket;
            if (!this.byBasketBySecurity.TryGetValue(basket.Id, out byBasket)) return nothing;
            
			IEnumerable<TInfo> found;
            if (!byBasket.TryGetValue(security.Id, out found)) return nothing;
            
			return found;
        }
    }
}
