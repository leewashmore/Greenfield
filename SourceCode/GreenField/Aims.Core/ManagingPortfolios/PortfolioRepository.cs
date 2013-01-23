using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;
using Aims.Core.Persisting;

namespace Aims.Core
{
    public class PortfolioRepository : ISecurityIdToPortfolioIdResolver
    {
        private Dictionary<String, BottomUpPortfolio> bottomUpPortfolios;
        private Dictionary<String, BroadGlobalActivePortfolio> broadGlobalActivePortfolios;
        private Dictionary<String, BottomUpPortfolio> bottomUpPortfolioBySecurityId;

        public PortfolioRepository(
            IEnumerable<PortfolioInfo> portfolios,
            SecurityRepository securityRepository
        )
        {
            var bottomUpPortfolios = this.GetBottomUpPortfolios(portfolios, securityRepository);
            if (!bottomUpPortfolios.Any()) throw new ApplicationException("There are no bottom up portfolios at all, check your database.");
            this.bottomUpPortfolios = bottomUpPortfolios;

            this.bottomUpPortfolioBySecurityId = this.bottomUpPortfolios.Select(x => x.Value).ToDictionary(x => x.Fund.Id);

            this.broadGlobalActivePortfolios = portfolios
                .Where(x => !securityRepository.IsProrfolioAFund(x.Id))
                .Select(x => new BroadGlobalActivePortfolio(
                    x.Id,
                    x.Name
                )).ToDictionary(x => x.Id);
        }

        protected Dictionary<String, BottomUpPortfolio> GetBottomUpPortfolios(
            IEnumerable<PortfolioInfo> portfolioInfos,
            SecurityRepository securityRepository
        )
        {
            var result = new Dictionary<String, BottomUpPortfolio>();
            foreach (var portfolioInfo in portfolioInfos)
            {
                if (securityRepository.IsProrfolioAFund(portfolioInfo.Id))
                {
                    var bottomUpPortfolio = new BottomUpPortfolio(
                        portfolioInfo.Id,
                        portfolioInfo.Name,
                        securityRepository.GetFundByPorfolioId(portfolioInfo.Id)
                    );
                    result.Add(bottomUpPortfolio.Id, bottomUpPortfolio);
                }
            }
            return result;
        }

        public IEnumerable<BottomUpPortfolio> GetAllBottomUpPortfolios()
        {
            return this.bottomUpPortfolios.Select(x => x.Value);
        }

        public BroadGlobalActivePortfolio FindBroadGlobalActivePortfolio(String portfolioId)
        {
            BroadGlobalActivePortfolio found;
            if (this.broadGlobalActivePortfolios.TryGetValue(portfolioId, out found))
            {
                return found;
            }
            else
            {
                return null;
            }
        }

        public BroadGlobalActivePortfolio GetBroadGlobalActivePortfolio(String broadGlobalActivePortfolioId)
        {
            var found = this.FindBroadGlobalActivePortfolio(broadGlobalActivePortfolioId);
            if (found == null) throw new ApplicationException("There is no broad global active portfolio with the \"" + broadGlobalActivePortfolioId + "\" ID.");
            return found;
        }

        public BottomUpPortfolio FindBottomUpPortfolio(String portfolioId)
        {
            BottomUpPortfolio found;
            if (this.bottomUpPortfolios.TryGetValue(portfolioId, out found))
            {
                return found;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a portfolio with the given portfolio ID. Throws an exception if the portfolio with the given ID cannot be found.
        /// </summary>
        public BottomUpPortfolio GetBottomUpPortfolio(String portfolioId)
        {
            var found = this.FindBottomUpPortfolio(portfolioId);
            if (found == null) throw new ApplicationException("There is no bottom-up portfolio with the \"" + portfolioId + "\" ID.");
            return found;
        }

        String ISecurityIdToPortfolioIdResolver.TryResolveToPortfolioId(String securityId)
        {
            BottomUpPortfolio found;
            if (this.bottomUpPortfolioBySecurityId.TryGetValue(securityId, out found))
            {
                return found.Id;
            }
            else
            {
                return null;
            }
        }

        public BottomUpPortfolio ResolveToBottomUpPortfolio(String securityId)
        {
            BottomUpPortfolio found;
            if (this.bottomUpPortfolioBySecurityId.TryGetValue(securityId, out found))
            {
                return found;
            }
            else
            {
                throw new ApplicationException("There is no bottom-up portfolio for the \"" + securityId + "\" security ID.");
            }
        }

    }
}
