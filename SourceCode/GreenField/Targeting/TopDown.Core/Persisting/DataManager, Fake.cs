using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TopDown.Core.Persisting
{
	public class FakeDataManager : DataManager
	{
		[DebuggerStepThrough]
		public FakeDataManager(SqlConnection connection, SqlTransaction transactionOpt)
			: base(connection, transactionOpt)
		{
		}

		[Obsolete("HACK! Remove from the production version!")]
		public override IEnumerable<CountryInfo> GetAllCountries()
		{
			var result = FakeCountries.All;
			return result;
		}
	}
}