using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.IssuerShares.Core.Persisting;

namespace GreenField.IssuerShares.Core
{
    public class DataManagerFactory : IDataManagerFactory
    {

        public IDataManager CreateDataManager(System.Data.SqlClient.SqlConnection connection)
        {
            var manager = new DataManager(connection, null);
            return manager;
        }

        public IDataManager CreateDataManager(System.Data.SqlClient.SqlConnection connection, System.Data.SqlClient.SqlTransaction transactionOpt)
        {
            var manager = new DataManager(connection, transactionOpt);
            return manager;
        }
    }
}
