using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Core.Persisting
{
    /// <summary>
    /// Represents a record of the COUNTRY_MASTER table.
    /// </summary>
    public class CountryInfo
    {
        [DebuggerStepThrough]
        public CountryInfo()
        {
        }

        [DebuggerStepThrough]
        public CountryInfo(String countryCode, String countryName)
        {
            this.CountryCode = countryCode;
            this.CountryName = countryName;
        }

        public String CountryCode { get; set; }
        public String CountryName { get; set; }
    }
}
