using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingCountries;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingBt
{
	public class CountryBasketChange
	{
		[DebuggerStepThrough]
		public CountryBasketChange(UnsavedBasketCountryModel unsavedBasketCountry, OtherModel other)
		{
			this.UnsavedBasketCountry = unsavedBasketCountry;
			this.Other = other;
		}

		public OtherModel Other { get; private set; }
		public UnsavedBasketCountryModel UnsavedBasketCountry { get; private set; }
	}
}
