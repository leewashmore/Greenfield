using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
    public class Int32Field<TInfo> : IField<TInfo>
    {
        [DebuggerStepThrough]
        public Int32Field(Action<TInfo, Int32> setter)
        {
            this.Setter = setter;
        }

        public Action<TInfo, Int32> Setter { get; private set; }

        public void Populate(TInfo info, SqlDataReader reader, Int32 index)
        {
            if (reader.IsDBNull(index))
            {
                throw new ApplicationException();
#warning Unfinished
            }
            else
            {
                var value = reader.GetInt32(index);
                this.Setter(info, value);
            }
        }
    }
}
