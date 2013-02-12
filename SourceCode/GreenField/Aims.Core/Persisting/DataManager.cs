using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using Aims.Core.Persisting;
using Aims.Core.Sql;

namespace Aims.Core.Persisting
{
    /// <summary>
    /// Knows how to talk to the database.
    /// </summary>
    public class DataManager : IDataManager
    {
        [DebuggerStepThrough]
        public DataManager(SqlConnection connection, SqlTransaction transactionOpt)
        {
            this.Connection = connection;
            this.TransactionOpt = transactionOpt;
        }

        protected SqlConnection Connection { get; private set; }
        protected SqlTransaction TransactionOpt { get; private set; }

        protected SqlQueryCommandBuilder<TInfo> CreateQueryCommandBuilder<TInfo>()
            where TInfo : class, new()
        {
            var command = Connection.CreateCommand();
            if (this.TransactionOpt != null)
            {
                command.Transaction = this.TransactionOpt;
            }
            return new SqlQueryCommandBuilder<TInfo>(command);
        }

        protected Aims.Core.Sql.SqlCommandBuilder CreateCommandBuilder()
        {
            var command = Connection.CreateCommand();
            if (this.TransactionOpt != null)
            {
                command.Transaction = this.TransactionOpt;
            }
            return new Aims.Core.Sql.SqlCommandBuilder(command);
        }

        protected IEnumerator<Int32> ReserveIds(String sequenceName, Int32 howMany)
        {
            Int32 firstId;
            using (var command = this.Connection.CreateCommand())
            {
                if (this.TransactionOpt != null)
                {
                    command.Transaction = this.TransactionOpt;
                }
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "CLAIM_IDS";

                var sequenceNameParameter = command.CreateParameter();
                sequenceNameParameter.ParameterName = "@sequenceName";
                sequenceNameParameter.Value = sequenceName;
                command.Parameters.Add(sequenceNameParameter);

                var howManyParameter = command.CreateParameter();
                howManyParameter.ParameterName = "@howMany";
                howManyParameter.Value = howMany;
                command.Parameters.Add(howManyParameter);

                var something = command.ExecuteScalar();
                if (DBNull.Value.Equals(something))
                {
                    throw new ApplicationException("There is no sequence named \"" + sequenceName + "\".");
                }

                firstId = (Int32)something;
            }
            var ids = new List<Int32>(howMany);
            for (var index = 0; index < howMany; index++)
            {
                var id = firstId + index;
                ids.Add(id);
            }
            return ids.GetEnumerator();
        }

        public virtual IEnumerable<SecurityInfo> GetAllSecurities()
        {
            using (var builder = this.CreateQueryCommandBuilder<SecurityInfo>())
            {
#warning SECURITY_ID needs to be changed to mandatory once all problem with securities that have NULL as a SECURITY_ID are removed from the GF_SECURITY_BASEVIEW table.
                return builder.Text("select")
                    .Field("  [SECURITY_ID]", (info, value) => info.Id = value, false)
                    .Field(", [TICKER]", (info, value) => info.Ticker = value, false)
                    .Field(", [ASEC_SEC_SHORT_NAME]", (info, value) => info.ShortName = value, false)
                    .Field(", [ISSUE_NAME]", (info, value) => info.Name = value, false)
                    .Field(", [ISO_COUNTRY_CODE]", (info, value) => info.IsoCountryCode = value, false)
                    .Field(", [LOOK_THRU_FUND]", (info, value) => info.LookThruFund = value, false)
                    .Field(", [ISSUER_ID]", (info, value) => info.IssuerId = value, false)
                    .Field(", [ISSUER_NAME]", (info, value) => info.IssuerName = value, false)
                    .Field(", [SECURITY_TYPE]", (info, value) => info.SecurityType = value, false)
                    .Field(", [TRADING_CURRENCY]", (info, value) => info.Currency = value, false)
                    .Field(", [ISIN]", (info, value) => info.Isin = value, false)
                    .Field(", [ISO_COUNTRY_CODE]", (info, value) => info.IsoCountryCode = value, false)
                    .Field(", [ASEC_SEC_COUNTRY_NAME]", (info, value) => info.AsecCountryName = value, false)
                .Text(" from " + TableNames.GF_SECURITY_BASEVIEW)
                .PullAll();
            }
        }
        public virtual IEnumerable<PortfolioInfo> GetAllPortfolios()
        {
            using (var builder = this.CreateQueryCommandBuilder<PortfolioInfo>())
            {
                return builder.Text("select ")
                    .Field("  [ID]", (info, value) => info.Id = value, true)
                    .Field(", [NAME]", (info, value) => info.Name = value, true)
                    .Field(", [IS_BOTTOM_UP]", (PortfolioInfo info, Int32 value) => info.IsBottomUp = (value == 1))
                    .Text(" from [" + TableNames.PORTFOLIO + "]")
                    .PullAll();
            }
        }
        public virtual IEnumerable<CountryInfo> GetAllCountries()
        {
            using (var builder = this.CreateQueryCommandBuilder<CountryInfo>())
            {
                return builder.Text("select distinct ")
                    .Field("  [ISO_COUNTRY_CODE]", (field, value) => field.CountryCode = value, false)
                    .Field(", [ASEC_SEC_COUNTRY_NAME]", (field, value) => field.CountryName = value, false)
                    .Text(@" from " + TableNames.GF_SECURITY_BASEVIEW + " where ISO_COUNTRY_CODE is not null and ASEC_SEC_COUNTRY_NAME is not null")
                    .PullAll();
            }
        }
    }
}
