using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.IssuerShares.Core.Persisting;
using Aims.Core.Sql;
using System.Data.SqlClient;

namespace GreenField.IssuerShares.Server
{
    internal class OnDemandDataManager : Aims.Core.Persisting.OnDemandDataManager<IDataManager>
    {
        public OnDemandDataManager(ISqlConnectionFactory connectionFactory, IDataManagerFactory dataManagerFactory)
            : base(connectionFactory, dataManagerFactory)
        {
        }

        public OnDemandDataManager(SqlConnection connection, IDataManagerFactory dataManagerFactory)
            : base(connection, dataManagerFactory)
        {
        }

    }
}
