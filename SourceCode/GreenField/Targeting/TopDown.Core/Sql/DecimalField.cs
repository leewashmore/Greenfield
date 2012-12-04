using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
    public class DecimalField<TInfo> : IField<TInfo>
    {
        [DebuggerStepThrough]
        public DecimalField(Action<TInfo, Decimal> setter)
        {
            this.Setter = setter;
        }

        public Action<TInfo, Decimal> Setter { get; private set; }

        public void Populate(TInfo info, SqlDataReader reader, Int32 index)
        {
            if (reader.IsDBNull(index))
            {
                throw new ApplicationException();
            }
            else
            {
                var value = reader.GetDecimal(index);
                this.Setter(info, value);
            }
        }
    }
}
