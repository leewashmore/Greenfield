using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core.Persisting;

namespace Aims.Core
{
    public class RepositoryManager
    {
        private IMonitor monitor;
        private CountryManager countryManager;
        private SecurityManager securityManager;
        private PortfolioManager portfolioManager;
        private IssuerManager issuerManager;

        [DebuggerStepThrough]
        public RepositoryManager(
            IMonitor monitor,
            CountryManager countryManager,
            SecurityManager securityManager,
            PortfolioManager portfolioManager,
            IssuerManager issuerManager
        )
        {
            this.monitor = monitor;
            this.countryManager = countryManager;
            this.securityManager = securityManager;
            this.portfolioManager = portfolioManager;
            this.issuerManager = issuerManager;
        }


        public SecurityRepository ClaimSecurityRepository(IOnDemand<IDataManager> ondemandManager)
        {
            return this.securityManager.ClaimSecurityRepository(
                ondemandManager,
                delegate
                {
                    return this.ClaimCountryRepository(ondemandManager);
                }
            );
        }
        public SecurityRepository ClaimSecurityRepository(IDataManager manager)
        {
            return this.securityManager.ClaimSecurityRepository(
                manager,
                delegate
                {
                    return this.ClaimCountryRepository(manager);
                }
            );
        }

        public SecurityRepository ClaimSecurityRepository(Func<IEnumerable<SecurityInfo>> ondemandSecurities, IDataManager manager)
        {
            return this.securityManager.ClaimSecurityRepository(ondemandSecurities, delegate
            {
                return this.ClaimCountryRepository(manager);
            });
        }


        public CountryRepository ClaimCountryRepository(IOnDemand<IDataManager> ondemandManager)
        {
            return this.countryManager.ClaimCountryRepository(ondemandManager);
        }
        public CountryRepository ClaimCountryRepository(IDataManager manager)
        {
            return this.countryManager.ClaimCountryRepository(manager);
        }

        public PortfolioRepository ClaimPortfolioRepository(IOnDemand<IDataManager> ondemandManager)
        {
            return this.portfolioManager.ClaimPortfolioRepository(
                ondemandManager,
                delegate
                {
                    return this.ClaimSecurityRepository(ondemandManager);
                }
            );
        }
        public PortfolioRepository ClaimPortfolioRepository(IDataManager manager)
        {
            return this.portfolioManager.ClaimPortfolioRepository(
                manager,
                delegate
                {
                    return this.ClaimSecurityRepository(manager);
                }
            );
        }

        public IssuerRepository ClaimIssuerRepository(Func<IEnumerable<SecurityInfo>> ondemandSecurities)
        {
            var result = this.issuerManager.ClaimIssuerRepository(ondemandSecurities);
            return result;
        }



        public virtual void DropEverything()
        {
            this.DropSecurityRepository();
            this.DropCountryRepository();
            this.DropPortfolioRepository();
            this.DropIssuerRepository();
        }

        public void DropIssuerRepository()
        {
            this.issuerManager.DropIssuerRepository();
        }
        public void DropSecurityRepository()
        {
            this.securityManager.DropSecurityRepository();
        }
        public void DropCountryRepository()
        {
            this.countryManager.DropCountryRepository();
        }
        public void DropPortfolioRepository()
        {
            this.portfolioManager.DropPortfolioRepository();
        }

    }
}
