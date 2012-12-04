using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
    public class DatetimeField<TInfo> : IField<TInfo>
    {
        [DebuggerStepThrough]
		public DatetimeField(Action<TInfo, DateTime> setter)
        {
            this.Setter = setter;
        }

		public Action<TInfo, DateTime> Setter { get; private set; }

        public void Populate(TInfo info, SqlDataReader reader, Int32 index)
        {
            if (reader.IsDBNull(index))
            {
                throw new ApplicationException();
#warning Unfinished
            }
            else
            {
                var value = reader.GetDateTime(index);
                this.Setter(info, value);
            }
        }
    }
}
