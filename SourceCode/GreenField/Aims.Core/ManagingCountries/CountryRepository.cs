using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core.Persisting;

namespace Aims.Core
{
    public class CountryNotFoundException : ApplicationException
    {
        public CountryNotFoundException(string message) : base(message){} 
        
    }

    public class CountryRepository : KeyedRepository<String, Country>
    {
        [DebuggerStepThrough]
        public CountryRepository(IEnumerable<CountryInfo> countryInfos)
        {
            foreach (var countryInfo in countryInfos)
            {
                var country = new Country(countryInfo.CountryCode, countryInfo.CountryName);
                base.RegisterValue(
                    country,
                    countryInfo.CountryCode
                );
            }
        }

        public Country GetCountry(String isoCode)
        {
            var found = base.FindValue(isoCode);
            if (found == null) throw new CountryNotFoundException("There is no country with the ISO code \"" + isoCode + "\".");
            return found;
        }

        public Country FindCountry(String isoCode)
        {
            return base.FindValue(isoCode);
        }
    }
}
