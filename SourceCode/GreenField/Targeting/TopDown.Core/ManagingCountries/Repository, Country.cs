using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingCountries
{
    public class CountryRepository : KeyedRepository<String, Country>
    {
        [DebuggerStepThrough]
        public CountryRepository(IEnumerable<CountryInfo> countryInfos)
        {
            countryInfos.ForEach(
                x => base.RegisterValue(
                    new Country(x.CountryCode, x.CountryName),
                    x.CountryCode
                )
            );
        }

        public Country GetCountry(String isoCode)
        {
            var found = base.FindValue(isoCode);
            if (found == null) throw new ApplicationException("There is no country with the ISO code \"" + isoCode + "\".");
            return found;
        }

        public Country FindCountry(String isoCode)
        {
            return base.FindValue(isoCode);
        }
    }
}
