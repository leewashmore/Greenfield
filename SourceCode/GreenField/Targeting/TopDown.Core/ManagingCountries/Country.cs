using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingCountries
{
    public class Country
    {
        [DebuggerStepThrough]
        public Country(String code, String name)
        {
            this.IsoCode = code;
            this.Name = name;
        }

        public String IsoCode { get; private set; }
        public String Name { get; private set; }

		[DebuggerStepThrough]
		public override Boolean Equals(object obj)
		{
			var country = obj as Country;
			return this.IsoCode == country.IsoCode;
		}

		[DebuggerStepThrough]
		public override Int32 GetHashCode()
		{
			return this.IsoCode.GetHashCode();
		}
    }
}
