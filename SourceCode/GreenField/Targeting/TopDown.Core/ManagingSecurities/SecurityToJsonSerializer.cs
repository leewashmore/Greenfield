using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingSecurities
{
	public class SecurityToJsonSerializer
	{
		private ManagingCountries.CountryToJsonSerializer countrySerializer;
		
		[DebuggerStepThrough]
		public SecurityToJsonSerializer(
			ManagingCountries.CountryToJsonSerializer countrySerializer
		)
		{
			this.countrySerializer = countrySerializer;
		}

        public void SerializeSecurityOnceResolved(ISecurity security, IJsonWriter writer)
		{
            var resolver = new SerializeSecurity_Resolver(this, writer);
            security.Accept(resolver);
		}

        private class SerializeSecurity_Resolver : ISecurityResolver
        {
            private IJsonWriter writer;
            private SecurityToJsonSerializer serializer;
            public SerializeSecurity_Resolver(SecurityToJsonSerializer serializer, IJsonWriter writer)
            {
                this.serializer = serializer;
                this.writer = writer;
            }
            public void Resolve(CompanySecurity stock)
            {
                this.serializer.SerializeStock(stock, this.writer);
            }
            public void Resolve(Fund fund)
            {
                this.serializer.SerializeFund(fund, this.writer);
            }
        }

        public void SerializeStock(CompanySecurity stock, IJsonWriter writer)
        {
            writer.Write(stock.Id, JsonNames.Id);
            writer.Write(stock.Ticker, JsonNames.Ticker);
            writer.Write(stock.ShortName, JsonNames.ShortName);
            writer.Write(stock.Name, JsonNames.Name);
            writer.Write(JsonNames.Country, delegate
            {
                this.countrySerializer.SerializeCountry(stock.Country, writer);
            });
        }

        public void SerializeFund(Fund fund, IJsonWriter writer)
        {
            writer.Write(fund.Id, JsonNames.Id);
            writer.Write(fund.Ticker, JsonNames.Ticker);
            writer.Write(fund.ShortName, JsonNames.ShortName);
            writer.Write(fund.Name, JsonNames.Name);
        }
    }
}
