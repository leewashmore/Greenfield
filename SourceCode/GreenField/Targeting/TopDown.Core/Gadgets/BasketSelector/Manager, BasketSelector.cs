using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingBaskets;

namespace TopDown.Core.Gadgets.BasketSelector
{
	/// <summary>
	/// Knows how to deal with the selector.
	/// </summary>
	public class BasketSelectorManager
	{
		private BasketSelectorJsonSerializer serializer;
		private BasketExtractor basketExtractor;
		private BasketToTextRenderer basketRenderer;
		
		public BasketSelectorManager(
			BasketSelectorJsonSerializer serializer,
			BasketExtractor basketExtractor,
			BasketToTextRenderer basketRenderer
		)
		{
			this.serializer = serializer;
			this.basketExtractor = basketExtractor;
			this.basketRenderer = basketRenderer;
		}

		public RootModel CreateRootModel(IEnumerable<TargetingTypeGroup> targetingTypeGroups)
		{
			var result = new RootModel(
				targetingTypeGroups
					.Select(x => new TargetingTypeGroupModel(
						x.Id,
						x.Name,
						this.basketExtractor
							.ExtractBaskets(x)
							.Select(
								y => new BasketModel(y.Id, basketRenderer.RenderBasketOnceResolved(y))
							)
					)
				)
			);
			return result;
		}

		public String RootModelToJson(RootModel root)
		{
			var builder = new StringBuilder();
			using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
			{
				this.serializer.RootToJson(writer, root);
			}
			return builder.ToString();
		}
	}
}
