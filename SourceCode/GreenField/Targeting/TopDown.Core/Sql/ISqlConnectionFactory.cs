using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
