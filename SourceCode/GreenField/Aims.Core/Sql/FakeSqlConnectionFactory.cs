using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Aims.Core.Sql
{
	public class FakeSqlConnectionFactory : ISqlConnectionFactory
	{
		public SqlConnection CreateConnection()
		{
			return null;
		}
	}
}
