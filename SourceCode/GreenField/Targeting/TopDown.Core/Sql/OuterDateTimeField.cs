using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
	public class OuterDateTimeField<TInfo, TSubInfo> : IField<TInfo>
	{
		private Func<TInfo, TSubInfo> creator;
		private Action<TSubInfo, DateTime> setter;

		public OuterDateTimeField(Func<TInfo, TSubInfo> creator, Action<TSubInfo, DateTime> setter)
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
				var value = reader.GetDateTime(index);
				this.setter(sub, value);
			}
		}
	}
}
