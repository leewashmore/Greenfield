using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.Sql;
using System.Diagnostics;
using System.Data.SqlClient;
using Aims.Core;

namespace TopDown.Core
{
	public class OnDemandDataManager : IOnDamand<IDataManager>
	{
		private ISqlConnectionFactory connectionFactory;
		private IDataManagerFactory dataManagerFactory;
		private SqlConnection connection;
		private IDataManager manager;

		[DebuggerStepThrough]
		public OnDemandDataManager(ISqlConnectionFactory connectionFactory, IDataManagerFactory dataManagerFactory)
		{
			this.connectionFactory = connectionFactory;
			this.dataManagerFactory = dataManagerFactory;
		}

		public OnDemandDataManager(SqlConnection connection, IDataManagerFactory dataManagerFactory)
		{
			this.connection = connection;
			this.dataManagerFactory = dataManagerFactory;
		}

		public IDataManager Claim()
		{
			if (this.manager == null)
			{
				if (this.connection == null)
				{
					this.connection = this.connectionFactory.CreateConnection();
				}
				this.manager = this.dataManagerFactory.CreateDataManager(this.connection, null);
			}
			return this.manager;
		}

		public void Dispose()
		{
			if (this.connection != null)
			{
				this.connection.Close();
				this.connection.Dispose();
				this.connection = null;
			}
		}
	}
}
