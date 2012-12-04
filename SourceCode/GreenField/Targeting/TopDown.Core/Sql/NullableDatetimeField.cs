using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
	public class NullableDatetimeField<TInfo> : IField<TInfo>
	{
		private Action<TInfo, DateTime?> setter;

		public NullableDatetimeField(Action<TInfo, DateTime?> setter)
		{
			this.setter = setter;
		}

		public void Populate(TInfo info, SqlDataReader reader, Int32 index)
		{
			if (reader.IsDBNull(index))
			{
				this.setter(info, null);
			}
			else
			{
				var value = reader.GetDateTime(index);
				this.setter(info, value);
			}
			
		}
	}
}
