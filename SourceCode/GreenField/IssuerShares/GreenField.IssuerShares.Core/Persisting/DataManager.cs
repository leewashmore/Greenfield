using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Aims.Data.Server;

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


        public IEnumerable<Aims.Data.Server.SecurityModel> GetIssuerSecurities(string securityShortName)
        {
            using (var builder = this.CreateQueryCommandBuilder<SecurityModel>())
            {
                return builder.Text("select ")
                        .Field("  [TICKER]", (info, value) => info.Id = value, true)
                        .Field(", [ISSUE_NAME]", (info, value) => info.Name = value, true)
                    .Text(" from [" + TableNames.GF_SECURITY_BASEVIEW + "]")
                    .Text(" where [ISSUER_ID] IN ( SELECT [ISSUER_ID] FROM [" + TableNames.GF_SECURITY_BASEVIEW + "] where [ASEC_SEC_SHORT_NAME] = ")
                        .Parameter(securityShortName)
                        .Text(" )")
                    .PullAll();
            }
        }
    }
}
