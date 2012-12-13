using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Aims.Core.Sql
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private string connectionString;
        
        public SqlConnectionFactory(String connectionString)
        {
            this.connectionString = connectionString;
        }
        
        public SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(this.connectionString);
			try
			{
				connection.Open();
			}
			catch (Exception exception)
			{
				throw new ApplicationException("Unable to open a connection to the database. Reason: " + exception.Message, exception);
			}
            return connection;
        }
    }
}