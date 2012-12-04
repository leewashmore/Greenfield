using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;

namespace TopDown.Core.Sql
{
    public class NullableInt32Field<TInfo> : IField<TInfo>
    {
        [DebuggerStepThrough]
        public NullableInt32Field(Action<TInfo, Int32?> setter)
        {
            this.Setter = setter;
        }

        public Action<TInfo, Int32?> Setter { get; private set; }

        public void Populate(TInfo info, SqlDataReader reader, Int32 index)
        {
            if (reader.IsDBNull(index))
            {
                this.Setter(info, null);
            }
            else
            {
                var value = reader.GetInt32(index);
                this.Setter(info, value);
            }
        }
    }
}
