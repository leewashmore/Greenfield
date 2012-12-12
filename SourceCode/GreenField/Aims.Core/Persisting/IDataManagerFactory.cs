using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Aims.Core.Persisting
{
    public interface IDataManagerFactory<out TDataManager>
        where TDataManager : IDataManager
    {
        TDataManager CreateDataManager(SqlConnection connection, SqlTransaction transactionOpt);
    }

    internal interface IDataManagerFactory : IDataManagerFactory<IDataManager>
    {
    }
}
