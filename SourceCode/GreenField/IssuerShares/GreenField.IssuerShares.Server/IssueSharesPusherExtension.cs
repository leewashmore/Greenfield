using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLoader.Core;

namespace GreenField.IssuerShares.Server
{
    public class IssueSharesPusherExtension : IssuerSharesPusher
    {
        public IssueSharesPusherExtension(Monitor monitor, String connectionString, Int32 recordsPerBulk) : base(monitor, connectionString, recordsPerBulk)
        {
            
        }

        public void ExecuteGetDataProcedure(String issuerId)
        {
            using (var connection = this.CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "dbo.Get_data";
                    var param = command.CreateParameter();
                    param.Value = issuerId;
                    param.ParameterName = "ISSUER_ID";
                    command.Parameters.Add(param);

                    
                    connection.Open();
                    var result = command.ExecuteNonQuery();
                }
            }
        }
    }
}
