using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBaskets
{
	public static class BasketExtender
	{
		public static CountryBasket TryAsCountryBasket(this IBasket basket)
		{
			var resolver = new TryAs_IBasketResolver();
			basket.Accept(resolver);
			return resolver.CountryBasketOpt;
		}
		public static CountryBasket AsCountryBasket(this IBasket basket)
		{
			var casted = basket.TryAsCountryBasket();
			if (casted == null) throw new ApplicationException();
			return casted;
		}

        public static RegionBasket TryAsRegionBasket(this IBasket basket)
        {
            var resolver = new TryAs_IBasketResolver();
            basket.Accept(resolver);
            return resolver.RegionBasketOpt;
        }
        public static RegionBasket AsRegionBasket(this IBasket basket)
        {
            var casted = basket.TryAsRegionBasket();
            if (casted == null) throw new ApplicationException();
            return casted;
        }

		private class TryAs_IBasketResolver : IBasketResolver
		{
			public CountryBasket CountryBasketOpt { get; private set; }
			public void Resolve(CountryBasket basket)
			{
				this.CountryBasketOpt = basket;
			}

            public RegionBasket RegionBasketOpt { get; private set; }
            public void Resolve(RegionBasket basket)
            {
                this.RegionBasketOpt = basket;
            }
        }

	}
}
