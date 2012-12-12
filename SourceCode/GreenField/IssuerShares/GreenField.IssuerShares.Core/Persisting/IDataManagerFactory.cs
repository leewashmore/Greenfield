using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace GreenField.IssuerShares.Core.Persisting
{
    public interface IDataManagerFactory : Aims.Core.Persisting.IDataManagerFactory<IDataManager>
    {
        IDataManager CreateDataManager(SqlConnection connection);
    }
}
