using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aims.Core;
using Aims.Core.Sql;
using GreenField.IssuerShares.Core;
using GreenField.IssuerShares.Server;
using System.Diagnostics;

namespace GreenField.IssuerShares.App.Web.Helpers
{
    public class Tester
    {
        public class InMemoryStorage<TValue> : IStorage<TValue>
        where TValue : class
        {
            private Dictionary<String, TValue> map;
            [DebuggerStepThrough]
            public InMemoryStorage()
            {
                this.map = new Dictionary<String, TValue>();
            }
            public TValue this[String key]
            {
                get
                {
                    TValue found;
                    if (this.map.TryGetValue(key, out found)) return found;
                    return null;
                }
                set
                {
                    TValue found;
                    if (this.map.TryGetValue(key, out found))
                    {
                        if (Object.ReferenceEquals(found, null))
                        {
                            this.map[key] = value;
                        }
                        else
                        {
                            throw new ApplicationException("There is an entry with the same key \"" + key + "\" already.");
                        }
                    }
                    else
                    {
                        this.map.Add(key, value);
                    }
                }
            }
        }


        public void Test()
        {
            //var connectionString = "Data Source=lonweb1t.ashmore.local;Initial Catalog=AIMS_Data_QA;Persist Security Info=True;User ID=WPSuperUser;Password=Password1;MultipleActiveResultSets=True";
            var connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Aims;Integrated Security=True";
            var connectionFactory = new SqlConnectionFactory(connectionString);
            var dataManagerFactory = new DataManagerFactory();


            var monitor = new Monitor();

            var countryRepositoryStorage = new InMemoryStorage<CountryRepository>();
            var countryManager = new CountryManager(countryRepositoryStorage);
            var securityStorage = new InMemoryStorage<SecurityRepository>();
            var securityManager = new SecurityManager(securityStorage, monitor);

            var issuerRepositoryStorage = new InMemoryStorage<IssuerRepository>();
            var issuerManager = new IssuerManager(monitor, issuerRepositoryStorage);

            var repositoryManager = new RepositoryManager(monitor, countryManager, securityManager, null, issuerManager);

            var modelBuilder = new ModelBuilder();
            var manager = new Core.ModelManager(connectionFactory, dataManagerFactory, repositoryManager, modelBuilder);

            var commonSerializer = new Aims.Data.Server.Serializer();
            var serializer = new Serializer(commonSerializer);
            var facade = new Facade(manager, commonSerializer, serializer, connectionFactory, dataManagerFactory, repositoryManager);
            //var found = facade.GetIssuerSecurities("SB", 1000, "RUSBERBPN __");
            var model = facade.GetRootModel("RUSBERBPN __");
            
        }
    }
}