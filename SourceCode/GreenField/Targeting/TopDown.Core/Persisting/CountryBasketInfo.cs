using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	public class CountryBasketInfo
	{
		[DebuggerStepThrough]
		public CountryBasketInfo()
		{
		}

		[DebuggerStepThrough]
		public CountryBasketInfo(Int32 id, String isoCountryCode)
		{
			this.Id = id;
			this.IsoCountryCode = isoCountryCode;
		}

		public Int32 Id { get; set; }
		public String IsoCountryCode { get; set; }
	}
}

