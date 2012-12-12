using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;
using Aims.Core.Sql;
using Aims.Core.Persisting;

namespace Aims.Data.Server
{
    public class Deserializer<TDataManager>
        where TDataManager : class, IDataManager
    {
        private RepositoryManager repositoryManager;
        private ISqlConnectionFactory connectionFactory;
        private IDataManagerFactory<TDataManager> dataManagerFactory;

        [DebuggerStepThrough]
        public Deserializer(
            ISqlConnectionFactory connectionFactory,
            IDataManagerFactory<TDataManager> dataManagerFactory,
            RepositoryManager repositoryManager
        )
        {
            this.connectionFactory = connectionFactory;
            this.dataManagerFactory = dataManagerFactory;
            this.repositoryManager = repositoryManager;
        }

        protected IOnDemand<TDataManager> CreateOnDemandDataManager()
        {
            var result = new OnDemandDataManager<TDataManager>(this.connectionFactory, this.dataManagerFactory);
            return result;
        }


        public Country DeserializeCountry(CountryModel model)
        {
            CountryRepository countryRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                countryRepository = this.repositoryManager.ClaimCountryRepository(ondemandManager);
            }
            var result = countryRepository.GetCountry(model.IsoCode);
            return result;
        }


        public BottomUpPortfolio DeserializeBottomUpPortfolio(BottomUpPortfolioModel model)
        {
            PortfolioRepository portfolioRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(ondemandManager);
            }
            var result = portfolioRepository.GetBottomUpPortfolio(model.Id);
            return result;
        }

        public ISecurity DeserializeSecurity(SecurityModel model)
        {
            SecurityRepository securityRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                securityRepository = this.repositoryManager.ClaimSecurityRepository(ondemandManager);
            }
            var security = securityRepository.GetSecurity(model.Id);
            return security;
        }
    }
}
