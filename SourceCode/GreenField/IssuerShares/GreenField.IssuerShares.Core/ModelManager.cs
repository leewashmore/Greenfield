using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;
using Aims.Core.Sql;
using Aims.Core.Persisting;
using GreenField.IssuerShares.Core.Persisting;

namespace GreenField.IssuerShares.Core
{
    public class ModelManager
    {
        private ModelBuilder modelBuilder;
        private ISqlConnectionFactory connectionFactory;
        private IDataManagerFactory dataManagerFactory;
        private RepositoryManager repositoryManager;

        public ModelManager(
            ISqlConnectionFactory connectionFactory,
            IDataManagerFactory dataManagerFactory,
            RepositoryManager repositoryManager,
            ModelBuilder modelBuilder
        )
        {
            this.connectionFactory = connectionFactory;
            this.dataManagerFactory = dataManagerFactory;
            this.repositoryManager = repositoryManager;
            this.modelBuilder = modelBuilder;
        }

        public RootModel GetRootModel(String issuerId)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var manager = this.dataManagerFactory.CreateDataManager(connection);

                // we may or may not need the securities
                var ondemandSecurities = new Func<IEnumerable<SecurityInfo>>(delegate
                {
                    var securities = manager.GetAllSecurities();
                    return securities.Where(x => x.IssuerId != null);
                });

                var securityRepository = this.repositoryManager.ClaimSecurityRepository(ondemandSecurities, manager);
                var issuerRepository = this.repositoryManager.ClaimIssuerRepository(ondemandSecurities);

                var issuer = issuerRepository.GetIssuer(issuerId);
                var items = this.GetItems(issuerId, manager, securityRepository);
                var result = new RootModel(issuer, items);
                return result;
            }
        }

        private IEnumerable<ItemModel> GetItems(
            String issuerId,
            IssuerShares.Core.Persisting.IDataManager manager,
            SecurityRepository securityRepository
        )
        {
            var result = new List<ItemModel>();
            var infos = manager.GetIssuerSharesComposition(issuerId);
            foreach (var compositionInfo in infos)
            {
                var item = this.modelBuilder.CreateItem(compositionInfo, securityRepository);
                result.Add(item);
            }
            return result;
        }
    }
}
