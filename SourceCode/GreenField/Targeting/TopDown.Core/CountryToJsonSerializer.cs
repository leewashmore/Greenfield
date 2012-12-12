using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;

namespace TopDown.Core.ManagingCountries
{
	public class CountryToJsonSerializer
	{
		public void SerializeCountry(Country country, IJsonWriter writer)
		{
            if (country != null)
            {
                writer.Write(country.IsoCode, JsonNames.IsoCode);
                writer.Write(country.Name, JsonNames.Name);
            }
		}
	}
}
