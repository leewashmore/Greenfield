using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TopDown.Core.Persisting
{
    public class DataManagerFactory : IDataManagerFactory
    {
        public IDataManager CreateDataManager(SqlConnection connection, SqlTransaction transationOpt)
        {
            var manager = new DataManager(connection, transationOpt);
            return manager;
        }
    }
}