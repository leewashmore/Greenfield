using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;

namespace Aims.Core
{
    /// <summary>
    /// Security with a country.
    /// </summary>
	public class CompanySecurity : ISecurity
	{
		[DebuggerStepThrough]
        public CompanySecurity(String id, String ticker, String shortName, String name, Country country)
		{
			this.Id = id;
			this.Ticker = ticker;
			this.ShortName = shortName;
			this.Name = name;
			this.Country = country;
		}

        public String Id { get; private set; }
		public String Ticker { get; private set; }
		public String ShortName { get; private set; }
		public String Name { get; private set; }
		public Country Country { get; private set; }

		[DebuggerStepThrough]
		public override Int32 GetHashCode()
		{
			return this.Id.GetHashCode();
		}
		public override Boolean Equals(Object obj)
		{
            var security = obj as CompanySecurity;
			return security.Id == this.Id;
		}

        [DebuggerStepThrough]
        public void Accept(ISecurityResolver resolver)
        {
            resolver.Resolve(this);
        }

        /// <summary>
        /// Gets a signature of the security.
        /// </summary>
        public override string ToString()
        {
            var result = String.Format(
                "Company security, ID: {0}, Ticker: {1}, Short name: {2}, Name: {3}, Country: {4}",
                this.Id,
                this.Ticker,
                this.Name,
                this.ShortName,
                this.Country.Name
            );
            return result;
        }
    }
}