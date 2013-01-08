using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Sql;
using GreenField.IssuerShares.Core.Persisting;
using Aims.Core;
using System.Diagnostics;

namespace GreenField.IssuerShares.Server
{
    public class FacadeSettings
    {
        [DebuggerStepThrough]
        public FacadeSettings(Core.ModelManager modelManager, Aims.Data.Server.Serializer commonSerializer, Serializer serializer, Deserializer deserializer, ISqlConnectionFactory connectionFactory, IDataManagerFactory dataManagerFactory, RepositoryManager repositoryManager)
        {
            this.ModelManager = modelManager;
            this.CommonSerializer = commonSerializer;
            this.Serializer = serializer;
            this.Deserializer = deserializer;
            this.ConnectionFactory = connectionFactory;
            this.DataManagerFactory = dataManagerFactory;
            this.RepositoryManager = repositoryManager;
        }

        public Core.ModelManager ModelManager { get; set; }
        public Aims.Data.Server.Serializer CommonSerializer { get; set; }
        public Serializer Serializer { get; set; }
        public Deserializer Deserializer { get; set; }
        public ISqlConnectionFactory ConnectionFactory { get; set; }
        public IDataManagerFactory DataManagerFactory { get; set; }
        public RepositoryManager RepositoryManager { get; set; }


    }
}
