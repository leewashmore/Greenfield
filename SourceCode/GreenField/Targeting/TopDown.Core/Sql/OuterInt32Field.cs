using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
	public class OuterInt32Field<TInfo, TSubInfo> : IField<TInfo>
	{
		private Func<TInfo, TSubInfo> creator;
		private Action<TSubInfo, Int32> setter;

		public OuterInt32Field(Func<TInfo, TSubInfo> creator, Action<TSubInfo, Int32> setter)
		{
			this.creator = creator;
			this.setter = setter;
		}
		
		public void Populate(TInfo info, SqlDataReader reader, Int32 index)
		{
			if (reader.IsDBNull(index))
			{
				// do nothing
#warning Unfinished
			}
			else
			{
				var sub = creator(info);
				var value = reader.GetInt32(index);
				this.setter(sub, value);
			}

		}
	}
}
