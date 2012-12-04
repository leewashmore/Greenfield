﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TopDown.Core.Persisting
{
    public interface IDataManagerFactory
    {
        IDataManager CreateDataManager(SqlConnection connection, SqlTransaction transationOpt);
    }
}
