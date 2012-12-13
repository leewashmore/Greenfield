using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using Aims.Core;
using Aims.Core.Sql;

namespace Aims.Core.Persisting
{
    public class OnDemandDataManager<TDataManager> : IOnDemand<TDataManager>
        where TDataManager : IDataManager
	{
		private ISqlConnectionFactory connectionFactory;
        private IDataManagerFactory<TDataManager> dataManagerFactory;
		private SqlConnection connection;
        private TDataManager manager;

		[DebuggerStepThrough]
		public OnDemandDataManager(ISqlConnectionFactory connectionFactory, IDataManagerFactory<TDataManager> dataManagerFactory)
		{
			this.connectionFactory = connectionFactory;
			this.dataManagerFactory = dataManagerFactory;
		}

        public OnDemandDataManager(SqlConnection connection, IDataManagerFactory<TDataManager> dataManagerFactory)
		{
			this.connection = connection;
			this.dataManagerFactory = dataManagerFactory;
		}

		public TDataManager Claim()
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
