using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TopDown.Core.Persisting
{
	[Obsolete("HACK! Remove from PROD version.")]
    public class FakeDataManagerFactory : IDataManagerFactory
    {
		[Obsolete("HACK! Remove from PROD version.")]
        public IDataManager CreateDataManager(SqlConnection connection, SqlTransaction transactionOpt)
        {
			var fakeManager = new FakeDataManager(connection, transactionOpt);
            return fakeManager;
        }
    }
}