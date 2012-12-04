using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingPortfolios;

namespace TopDown.Core.Overlaying
{
	public class OverlayManager
	{
		private ModelBuilder modelBuilder;

		public OverlayManager(ModelBuilder modelBuilder)
		{
			this.modelBuilder = modelBuilder;
		}

		public RootModel GetOverlayModel(
			TargetingType targetingType,
			String porfolioId,
			PortfolioRepository portfolioRepository,
			SecurityRepository securityRepository,
			IDataManager manager
		)
		{
			// first, we get already saved overlays
			// (with overlay factor numbers)
			var savedFactors = manager.GetBgaPortfolioSecurityFactors(porfolioId);
			var itemsForSavedOverlays = savedFactors.Select(overlay =>
			{
				var security = securityRepository.GetSecurity(overlay.SecurityId);
				var portfolio = portfolioRepository.ResolveToBottomUpPortfolio(overlay.SecurityId);
				var item = new ItemModel(
					portfolio,
					this.modelBuilder.CreateOverlayFactorExpression(portfolio.Name)
				);
				item.OverlayFactor.InitialValue = overlay.Factor;
				return item;
			});

			// second we need to get potentially enabled overlays that haven't been saved yet,
			// but need to be offered to the user who, may be, is going to save them
			// (there is no overlay factor numbers)

			var itemsForPotentialOverlays = targetingType.BottomUpPortfolios						// for all the portoflios defined for the targeting type
				.Where(x => ! itemsForSavedOverlays.Select(y => y.BottomUpPortfolio).Contains(x))	// get only those which are not in the 'saved' list yet
				.Select(bottomUpPortfolio => {
					var item = new ItemModel(
						bottomUpPortfolio,
						this.modelBuilder.CreateOverlayFactorExpression(bottomUpPortfolio.Name)
					);
					return item;
				});

			// order by name
			var items = itemsForSavedOverlays
				.Union(itemsForPotentialOverlays)
				.OrderBy(x => x.BottomUpPortfolio.Name);

			var result = new RootModel(items);

			return result;
		}

	}
}
