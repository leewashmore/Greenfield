using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TopDown.Core.Persisting
{
    public class UsersDataManagerFactory : IUsersDataManagerFactory
    {
        public IUsersDataManager CreateDataManager(SqlConnection connection, SqlTransaction transationOpt)
        {
            var manager = new UsersDataManager(connection, transationOpt);
            return manager;
        }
    }
}