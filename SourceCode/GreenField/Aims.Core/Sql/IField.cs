using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Aims.Core.Sql
{
    public interface IField<TInfo>
    {
        void Populate(TInfo info, SqlDataReader reader, Int32 index);
    }
}
