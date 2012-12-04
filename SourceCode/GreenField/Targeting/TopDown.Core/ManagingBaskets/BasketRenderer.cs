using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBaskets
{
    public class BasketRenderer
    {
        public String RenderBasketOnceResolved(IBasket basket)
        {
            var resolver = new RenderBasketOnceResolved_IBasketResolver(this);
            basket.Accept(resolver);
            return resolver.Result;
        }

        private class RenderBasketOnceResolved_IBasketResolver : IBasketResolver
        {
            private BasketRenderer renderer;

            public RenderBasketOnceResolved_IBasketResolver(BasketRenderer renderer)
            {
                this.renderer = renderer;
            }
            
            public String Result { get; private set; }

            public void Resolve(CountryBasket basket)
            {
                this.Result = this.renderer.RenderBasket(basket);
            }

            public void Resolve(RegionBasket basket)
            {
                this.Result = this.renderer.RenderBasket(basket);
            }
        }

        public String RenderBasket(CountryBasket basket)
        {
            return basket.Country.Name + " (" + basket.Country.IsoCode + ")";
        }

        public String RenderBasket(RegionBasket basket)
        {
            return basket.Name;
        }
    }
}
