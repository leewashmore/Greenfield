using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Aims.Core.Sql
{
	public class OuterStringField<TInfo, TSubInfo> : IField<TInfo>
	{
		private Func<TInfo, TSubInfo> creator;
		private Action<TSubInfo, String> setter;

		public OuterStringField(Func<TInfo, TSubInfo> creator, Action<TSubInfo, String> setter)
		{
			this.creator = creator;
			this.setter = setter;
		}

		public void Populate(TInfo info, SqlDataReader reader, Int32 index)
		{
			if (reader.IsDBNull(index))
			{
				// do nothing
			}
			else
			{
				var sub = this.creator(info);
				var value = (reader.GetString(index) ?? String.Empty).Trim();
				this.setter(sub, value);
			}
		}
	}
}
