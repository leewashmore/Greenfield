using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using Aims.Core;
using System.Web.Caching;
using System.Diagnostics;
using Aims.Core.Sql;
using GreenField.IssuerShares.Server;
using System.Runtime.Caching;
using System.Configuration;

namespace GreenField.Web.IssuerShares
{
    public class IssuerSharesServiceHost : ServiceHost
    {
        public IssuerSharesServiceHost(FacadeSettings settings, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            foreach (var contractDescription in this.ImplementedContracts.Values)
            {
                var provider = new IssuerSharesInstanceProvider(settings);
                contractDescription.Behaviors.Add(provider);
            }
        }
    }

    public class IssuerSharesServiceHostFactory : ServiceHostFactory
    {
        private readonly GreenField.IssuerShares.Server.FacadeSettings settings;

        public IssuerSharesServiceHostFactory()
        {
            this.settings = CreateFacadeSettings(
                ConfigurationSettings.AimsConnectionString,
                ConfigurationSettings.ShouldDropRepositoriesOnEachReload
            );
        }

        protected override ServiceHost CreateServiceHost(Type serviceType,
            Uri[] baseAddresses)
        {
            var serviceHost = new IssuerSharesServiceHost(
                this.settings,
                serviceType,
                baseAddresses
            );
            return serviceHost;
        }

        private class CacheStorage<TValue> : IStorage<TValue>
            where TValue : class
        {
            private ObjectCache cache;
            public CacheStorage(ObjectCache cache)
            {
                this.cache = cache;
            }

            public TValue this[String key]
            {
                get
                {
                    var result = this.cache.Get(key) as TValue;
                    return result;
                }
                set
                {
                    if (value == null)
                    {
                        this.cache.Remove(key);
                    }
                    else
                    {
                        CacheItemPolicy policy = new CacheItemPolicy();
                        policy.AbsoluteExpiration = DateTime.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["SecuritiesCacheTime"]));
                        this.cache.Set(key, value, policy);
                        this.cache.Set(key + "Policy", policy, null);
                    }
                }
            }
        }

        protected static GreenField.IssuerShares.Server.FacadeSettings CreateFacadeSettings(String connectionString, Boolean shouldDropRepositories)
        {
            try
            {
                return CreateFacadeSettingsUnsafe(connectionString, shouldDropRepositories);
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unable to create a facade for targeting operations. Reason: " + exception.Message, exception);
            }
        }

        private static GreenField.IssuerShares.Server.FacadeSettings CreateFacadeSettingsUnsafe(string connectionString, bool shouldDropRepositories)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["Aims"].ConnectionString;
            var connectionFactory = new SqlConnectionFactory(connectionString);
            var dataManagerFactory = new GreenField.IssuerShares.Core.DataManagerFactory();


            var cache = MemoryCache.Default;
            var monitor = new Monitor();

            var countryRepositoryStorage = new CacheStorage<CountryRepository>(cache);
            var countryManager = new CountryManager(countryRepositoryStorage);
            var securityStorage = new CacheStorage<SecurityRepository>(cache);
            var securityManager = new SecurityManager(securityStorage, monitor);

            var issuerRepositoryStorage = new CacheStorage<IssuerRepository>(cache);
            var issuerManager = new IssuerManager(monitor, issuerRepositoryStorage);

            var repositoryManager = new RepositoryManager(monitor, countryManager, securityManager, null, issuerManager);

            var modelBuilder = new GreenField.IssuerShares.Core.ModelBuilder();
            var manager = new GreenField.IssuerShares.Core.ModelManager(connectionFactory, dataManagerFactory, repositoryManager, modelBuilder);

            var commonSerializer = new Aims.Data.Server.Serializer();
            var serializer = new GreenField.IssuerShares.Server.Serializer(commonSerializer);
            var deserializer = new GreenField.IssuerShares.Server.Deserializer(connectionFactory, dataManagerFactory, repositoryManager);
            //var facade = new GreenField.IssuerShares.Server.Facade(manager, commonSerializer, serializer, deserializer, connectionFactory, dataManagerFactory, repositoryManager);
            var result = new GreenField.IssuerShares.Server.FacadeSettings(manager, commonSerializer, serializer, deserializer, connectionFactory, dataManagerFactory, repositoryManager);
            return result;
        }
    }
}