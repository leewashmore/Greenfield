using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Persisting;
using Aims.Core.Sql;
using System.Diagnostics;
using Aims.Core;
using GreenField.IssuerShares.Core.Persisting;
using Aims.Data.Server;

namespace GreenField.IssuerShares.Server
{
    public class Deserializer : Aims.Data.Server.Deserializer<Aims.Core.Persisting.IDataManager>
    {
        private RepositoryManager repositoryManager;
        private ISqlConnectionFactory connectionFactory;
        private IDataManagerFactory dataManagerFactory;

        [DebuggerStepThrough]
        public Deserializer(
            ISqlConnectionFactory connectionFactory,
            IDataManagerFactory dataManagerFactory,
            RepositoryManager repositoryManager
        )
            : base(connectionFactory, dataManagerFactory, repositoryManager)
        {
            this.connectionFactory = connectionFactory;
            this.dataManagerFactory = dataManagerFactory;
            this.repositoryManager = repositoryManager;
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
