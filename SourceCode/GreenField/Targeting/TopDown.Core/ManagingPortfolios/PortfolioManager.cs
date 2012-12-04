using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using TopDown.Core.ManagingSecurities;

namespace TopDown.Core.ManagingPortfolios
{
	public class PortfolioManager
	{
		public const String PortfolioRepositoryStorageKey = "PortfolioRepository";

		private PortfolioToJsonSerializer portfolioSerializer;
		private IStorage<PortfolioRepository> portfolioRepositoryStorage;

		[DebuggerStepThrough]
		public PortfolioManager(
			IStorage<PortfolioRepository> portfolioRepositoryStorage,
			ManagingPortfolios.PortfolioToJsonSerializer portfolioSerializer
		)
		{
			this.portfolioSerializer = portfolioSerializer;
			this.portfolioRepositoryStorage = portfolioRepositoryStorage;
		}

		public String SerializeToJson(IEnumerable<BottomUpPortfolio> portfolios)
		{
			var builder = new StringBuilder();
			using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
			{
				writer.WriteArray(portfolios, portfolio =>
				{
					writer.Write(delegate
					{
						this.portfolioSerializer.SerializeBottomUpPortfolio(portfolio, writer);
					});
				});
			}
			return builder.ToString();
		}


		public PortfolioRepository ClaimPortfolioRepository(
			IOnDamand<IDataManager> ondemandManager,
			Func<SecurityRepository> ondemandSecurityRepository
		)
		{
			return this.portfolioRepositoryStorage.Claim(PortfolioRepositoryStorageKey, delegate
			{
				var manager = ondemandManager.Claim();
				var portfolioInfos = manager.GetAllPortfolios();
				var result = new PortfolioRepository(
					portfolioInfos,
					ondemandSecurityRepository()
				);
				return result;
			});
		}

		public PortfolioRepository ClaimPortfolioRepository(IDataManager manager, Func<SecurityRepository> ondemandSecurityRepository)
		{
			return this.portfolioRepositoryStorage.Claim(PortfolioRepositoryStorageKey, delegate
			{
				var portfolioInfos = manager.GetAllPortfolios();
				var result = new PortfolioRepository(
					portfolioInfos,
					ondemandSecurityRepository()
				);
				return result;
			});
		}

		public void DropPortfolioRepository()
		{
			this.portfolioRepositoryStorage[PortfolioRepositoryStorageKey] = null;
		}
	}
}
