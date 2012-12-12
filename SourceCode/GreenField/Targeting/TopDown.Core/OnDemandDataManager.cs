using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using Aims.Core.Sql;
using System.Data.SqlClient;

namespace TopDown.Core
{
    internal class OnDemandDataManager : Aims.Core.Persisting.OnDemandDataManager<IDataManager>
    {
        public OnDemandDataManager(ISqlConnectionFactory connectionFactory, Aims.Core.Persisting.IDataManagerFactory<IDataManager> dataManagerFactory)
            : base(connectionFactory, dataManagerFactory)
        {
        }
        
        public OnDemandDataManager(SqlConnection connection, Aims.Core.Persisting.IDataManagerFactory<IDataManager> dataManagerFactory)
            : base(connection, dataManagerFactory)
        {
        }

    }
}
