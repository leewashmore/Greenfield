using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBaskets
{
	/// <summary>
	/// Turns a basket to text.
	/// </summary>
	public class BasketToTextRenderer
	{
		/// <summary>
		/// Gets some text about what the given basket is.
		/// </summary>
		[DebuggerStepThrough]
		public String RenderBasketOnceResolved(IBasket basket)
		{
			var resolver = new RenderBasket_IBasketResolver(this);
			basket.Accept(resolver);
			return resolver.Result;
		}

		private class RenderBasket_IBasketResolver : IBasketResolver
		{
			private BasketToTextRenderer renderer;

			public RenderBasket_IBasketResolver(BasketToTextRenderer renderer)
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
			var result = basket.Country.Name + "(" + basket.Country.IsoCode + ")";
			return result;
		}

		public String RenderBasket(RegionBasket basket)
		{
			var result = basket.Name + "(" + String.Join(", ", basket.Countries.Select(x => x.IsoCode).ToArray()) + ")";
			return result;
		}
	}
}
