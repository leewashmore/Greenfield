using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace GreenField.IssuerShares.Core.Persisting
{
    public class DataManager : Aims.Core.Persisting.DataManager, IDataManager
    {
        public DataManager(SqlConnection connection, SqlTransaction transactionOpt)
            : base(connection, transactionOpt)
        {

        }

        public IEnumerable<IssuerSharesCompositionInfo> GetIssuerSharesComposition(String issuerId)
        {
            using (var builder = this.CreateQueryCommandBuilder<IssuerSharesCompositionInfo>())
            {
                return builder.Text("select ")
                        .Field("  [ISSUER_ID]", (info, value) => info.IssuerId = value, true)
                        .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                        .Field(", [PREFERRED]", (info, value) => info.Preferred = value[0], true)
                    .Text(" from [" + TableNames.ISSUER_SHARES_COMPOSITION + "]")
                    .Text(" where [ISSUER_ID] = ")
                        .Parameter(issuerId)
                    .PullAll();
            }
        }
    }
}
