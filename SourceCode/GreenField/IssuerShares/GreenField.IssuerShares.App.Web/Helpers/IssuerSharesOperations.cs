using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.IssuerShares.App.Web.Services;
using GreenField.IssuerShares.Server;
using Aims.Core.Sql;
using System.Configuration;
using GreenField.IssuerShares.Core;
using Aims.Core;
using System.Web.Caching;
using System.Diagnostics;
//using GreenField.IssuerShares.App.Web.Helpers;

namespace GreenField.IssuerShares.App.Web
{
    public class CacheStorage<TValue> : IStorage<TValue>
            where TValue : class
    {
        private Cache cache;
        public CacheStorage(Cache cache)
        {
            this.cache = cache;
        }

        public TValue this[String key]
        {
            [DebuggerStepThrough]
            get
            {
                var result = this.cache.Get(key) as TValue;
                return result;
            }
            [DebuggerStepThrough]
            set
            {
                if (value == null)
                {
                    this.cache.Remove(key);
                }
                else
                {
                    this.cache.Insert(key, value);
                }
            }
        }
    }

    public class IssuerSharesOperationsHostFactory: Aims.Data.Server.ServiceHostFactoryBase<IssuerSharesOperations, IFacade>
    {

        

        protected override IFacade CreateSettings()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Aims"].ConnectionString;
            var connectionFactory = new SqlConnectionFactory(connectionString);
            var dataManagerFactory = new DataManagerFactory();


            var cache = HttpContext.Current.Cache;
            var monitor = new Monitor();

            var countryRepositoryStorage = new CacheStorage<CountryRepository>(cache);
            var countryManager = new CountryManager(countryRepositoryStorage);
            var securityStorage = new CacheStorage<SecurityRepository>(cache);
            var securityManager = new SecurityManager(securityStorage, monitor);

            var issuerRepositoryStorage = new CacheStorage<IssuerRepository>(cache);
            var issuerManager = new IssuerManager(monitor, issuerRepositoryStorage);

            var repositoryManager = new RepositoryManager(monitor, countryManager, securityManager, null, issuerManager);

            var modelBuilder = new ModelBuilder();
            var manager = new Core.ModelManager(connectionFactory, dataManagerFactory, repositoryManager, modelBuilder);

            var commonSerializer = new Aims.Data.Server.Serializer();
            var serializer = new Serializer(commonSerializer);
            var facade = new Facade(manager, commonSerializer, serializer, connectionFactory, dataManagerFactory, repositoryManager);
            return facade;
        }

        protected override IssuerSharesOperations CreateService(IFacade facade)
        {
            IssuerSharesOperations service;
            if (facade != null)
                service = new IssuerSharesOperations(facade);
            else
            {
                service = new IssuerSharesOperations(CreateSettings());
            }
            
            return service;
        }


        
    }
}