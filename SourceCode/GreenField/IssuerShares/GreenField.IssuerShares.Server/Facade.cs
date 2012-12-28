using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Sql;
using GreenField.IssuerShares.Core.Persisting;
using System.Data.SqlClient;
using Aims.Core;

namespace GreenField.IssuerShares.Server
{
    public class Facade : IFacade
    {
        private Serializer serializer;
        private Core.ModelManager modelManager;
        private ISqlConnectionFactory connectionFactory;
        private IDataManagerFactory dataManagerFactory;
        private Aims.Data.Server.Serializer commonSerializer;

        public RepositoryManager RepositoryManager { get; private set; }
        
        public Facade(
            Core.ModelManager modelManager,
            Aims.Data.Server.Serializer commonSerializer,
            Serializer serializer,
            ISqlConnectionFactory connectionFactory,
            IDataManagerFactory dataManagerFactory,
            RepositoryManager repositoryManager
        )
        {
            this.modelManager = modelManager;
            this.serializer = serializer;
            this.connectionFactory = connectionFactory;
            this.dataManagerFactory = dataManagerFactory;
            this.RepositoryManager = repositoryManager;
            this.commonSerializer = commonSerializer;
        }

        public RootModel GetRootModel(String securityShortName)
        {
            using (var ondemandManager = new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory))
            {
                var repository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var security = repository.FindSecurityByShortName(securityShortName);
                if (security != null)
                {
                    var model = this.modelManager.GetRootModel(security.IssuerId);
                    var serializedModel = this.serializer.SerializeRoot(model);
                    return serializedModel;
                }
                else
                    return new RootModel { Issuer = null, Items = new List<ItemModel>() };
            }
        }

        

        public IEnumerable<Aims.Data.Server.SecurityModel> GetIssuerSecurities(String pattern, Int32 atMost, String securityShortName)
        {
            using (var ondemandManager = new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory))
            {
                var repository = this.RepositoryManager.ClaimSecurityRepository(ondemandManager);
                var security = repository.FindSecurityByShortName(securityShortName);
                if (security != null)
                {
                    var securities = repository.FindSomeUsingPattern(pattern, atMost, x => x.IssuerId == security.IssuerId && (x.SecurityType == "EQUITY" || x.SecurityType == "ADR" || x.SecurityType == "GDR"));
                    var result = this.commonSerializer.SerializeSecurities(securities);
                    return result;
                }
                else
                    return new List<Aims.Data.Server.SecurityModel>();
            }
        }

        internal OnDemandDataManager CreateOnDemandDataManager()
        {
            return new OnDemandDataManager(this.connectionFactory, this.dataManagerFactory);
        }

        internal OnDemandDataManager CreateOnDemandDataManager(SqlConnection connection)
        {
            return new OnDemandDataManager(connection, this.dataManagerFactory);
        }

        
    }
}
