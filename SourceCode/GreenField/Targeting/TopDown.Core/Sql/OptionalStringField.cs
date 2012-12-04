using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
    public class OptionalStringField<TInfo> : IField<TInfo>
    {
        public OptionalStringField(Action<TInfo, String> setter)
        {
            this.Setter = setter;
        }
        public Action<TInfo, String> Setter { get; private set; }

        public void Populate(TInfo info, SqlDataReader reader, Int32 index)
        {
            if (reader.IsDBNull(index))
            {
                this.Setter(info, null);
            }
            else
            {
                var value = (reader.GetString(index) ?? String.Empty).Trim();
                this.Setter(info, value);
            }
        }
    }
}
