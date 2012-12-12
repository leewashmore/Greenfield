using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingCountries;
using Aims.Core;

namespace TopDown.Core.ManagingBaskets
{
	public class CountryBasket : IBasket
	{
		[DebuggerStepThrough]
		public CountryBasket(Int32 id, Country country)
		{
			this.Id = id;
			this.Country = country;
		}

		public Int32 Id { get; private set; }
		public Country Country { get; private set; }

		[DebuggerStepThrough]
		public void Accept(IBasketResolver resolver)
		{
			resolver.Resolve(this);
		}
	}
}
