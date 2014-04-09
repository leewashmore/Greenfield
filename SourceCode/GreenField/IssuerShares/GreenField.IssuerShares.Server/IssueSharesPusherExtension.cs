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
            int recCount = 0;
            using (var connection = this.CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 1200;
                    command.CommandText = "Select count(*) from period_financials_issuer_stage where issuer_id = '"+issuerId+"'";
                    recCount = int.Parse(command.ExecuteScalar().ToString());

                  
                }


                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandTimeout = 1200;
                    command.CommandText = "dbo.Get_data";
                    var param = command.CreateParameter();
                    param.Value = issuerId;
                    param.ParameterName = "ISSUER_ID";
                    command.Parameters.Add(param);

                    var param2 = command.CreateParameter();
                    param2.Value = "N";
                    param2.ParameterName = "CALC_LOG";
                    command.Parameters.Add(param2);

                    var param3 = command.CreateParameter();
                    param3.Value = "N";
                    param3.ParameterName = "VERBOSE";
                    command.Parameters.Add(param3);

                    var param4 = command.CreateParameter();
                    param4.Value = "I";
                    param4.ParameterName = "RUN_MODE";
                    command.Parameters.Add(param4);

                    var param5 = command.CreateParameter();
                    if (recCount != 0)
                    {
                        param4.Value = "Y";
                    }
                    else
                    {
                        param4.Value = "N";
                    }
                    param4.ParameterName = "STAGE";
                    command.Parameters.Add(param4);


                    connection.Open();
                    var result = command.ExecuteNonQuery();
                }
            }
        }
    }
}
