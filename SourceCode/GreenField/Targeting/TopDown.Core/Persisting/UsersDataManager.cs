using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.ManagingTaxonomies;
using System.Diagnostics;
using System.Data;
using Aims.Core.Persisting;
using Aims.Core.Sql;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Knows how to talk to the database.
    /// </summary>
    public class UsersDataManager : IUsersDataManager
    {

        [DebuggerStepThrough]
        public UsersDataManager(SqlConnection connection, SqlTransaction transactionOpt)
        {
            this.Connection = connection;
            this.TransactionOpt = transactionOpt;
        }

        protected SqlConnection Connection { get; private set; }
        protected SqlTransaction TransactionOpt { get; private set; }

        public string GetUserEmail(string username)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "select distinct [LoweredEmail] from [aspnet_Membership] where userId in (select userid from aspnet_Users where LoweredUserName = @uname)";
                command.Parameters.Add(new SqlParameter("uname", username));
                using (var reader = command.ExecuteReader())
                {
                    string result = "";
                    if (reader.Read())
                    {
                        result = (string)reader[0];
                    }

                    return result;
                }
            }
        }
    }
}
