using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core
{
    public class UnknownCountryIsoCodesDetector
    {
		public IEnumerable<String> FindUnknownCodes(IEnumerable<String> knownIsoCodes, IEnumerable<String> whateverIsoCodes)
        {
			var hash = new HashSet<String>(knownIsoCodes.Distinct());
            var unknowContries = whateverIsoCodes.Where(x => !hash.Contains(x));
            return unknowContries;
        }        
    }
}
