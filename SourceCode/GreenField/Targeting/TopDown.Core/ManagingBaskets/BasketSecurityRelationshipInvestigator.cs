using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;

namespace TopDown.Core.ManagingBaskets
{
    public class BasketSecurityRelationshipInvestigator
    {
        public Boolean IsSecurityFromBasketOnceResolved(ISecurity security, IBasket basket)
        {
            var resolver = new IsSecurityFromBasket_IBasketResolver(this, security);
            basket.Accept(resolver);
            return resolver.Result;
        }

        private class IsSecurityFromBasket_IBasketResolver : IBasketResolver
        {
            private ISecurity security;
            private BasketSecurityRelationshipInvestigator investigator;

            public IsSecurityFromBasket_IBasketResolver(BasketSecurityRelationshipInvestigator investigator, ISecurity security)
            {
                this.security = security;
                this.investigator = investigator;
            }
            public Boolean Result { get; private set; }

            public void Resolve(CountryBasket basket)
            {
                this.Result = this.investigator.IsSecurityFromCountryBasket(security, basket);
            }

            public void Resolve(RegionBasket basket)
            {
                this.Result = this.investigator.IsSecurityFromRegionBasket(security, basket);
            }
        }

        public Boolean IsSecurityFromCountryBasket(ISecurity security, CountryBasket basket)
        {
            var stockOpt = security.TryAsCompanySecurity();
            if (stockOpt != null)
            {
                var result = basket.Country.IsoCode == stockOpt.Country.IsoCode;
                return result;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsSecurityFromRegionBasket(ISecurity security, RegionBasket basket)
        {
            var stockOpt = security.TryAsCompanySecurity();
            if (stockOpt != null)
            {
                var result = basket.Countries.Any(x => x.IsoCode == stockOpt.Country.IsoCode);
                return result;
            }
            else
            {
                return false;
            }
        }
    }
}
