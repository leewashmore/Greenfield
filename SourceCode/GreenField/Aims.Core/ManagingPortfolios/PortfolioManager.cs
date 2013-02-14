using System;
using System.Diagnostics;
using Aims.Core.Persisting;

namespace Aims.Core
{
	public class PortfolioManager
	{
		public const String PortfolioRepositoryStorageKey = "PortfolioRepository";

		private IStorage<PortfolioRepository> portfolioRepositoryStorage;

		[DebuggerStepThrough]
		public PortfolioManager(IStorage<PortfolioRepository> portfolioRepositoryStorage)
		{
			this.portfolioRepositoryStorage = portfolioRepositoryStorage;
		}

		public PortfolioRepository ClaimPortfolioRepository(
			IOnDemand<IDataManager> ondemandManager,
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
