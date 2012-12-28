using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;

namespace Aims.Data.Server
{
    public class Serializer
    {
        public IEnumerable<SecurityModel> SerializeSecurities(IEnumerable<ISecurity> securities)
        {
            var result = securities.Select(x => this.SerializeSecurityOnceResolved(x)).ToArray();
            return result;
        }

        public SecurityModel SerializeSecurityOnceResolved(ISecurity security)
        {
            var resolver = new SerializeSecurityOnceResolved_ISecurityResolver(this);
            security.Accept(resolver);
            return resolver.Result;
        }

        private class SerializeSecurityOnceResolved_ISecurityResolver : ISecurityResolver
        {
            private Serializer serializer;

            public SerializeSecurityOnceResolved_ISecurityResolver(Serializer serializer)
            {
                this.serializer = serializer;
            }

            public SecurityModel Result { get; private set; }

            public void Resolve(CompanySecurity stock)
            {
                this.Result = this.serializer.SerializeCompanySecurity(stock);
            }

            public void Resolve(Fund fund)
            {
                this.Result = this.serializer.SerializeFund(fund);
            }
        }


        private FundModel SerializeFund(Fund fund)
        {
            var result = new FundModel(
                fund.Id,
                fund.Name,
                fund.ShortName,
                fund.Ticker,
                fund.IssuerId,
                fund.SecurityType
            );
            return result;
        }

        public CompanySecurityModel SerializeCompanySecurity(CompanySecurity security)
        {
            var result = new CompanySecurityModel(
                security.Id,
                security.Name,
                security.ShortName,
                security.Ticker,
                this.SerializeCountry(security.Country),
                security.IssuerId,
                security.SecurityType
            );
            return result;
        }

        public CountryModel SerializeCountry(Country country)
        {
            var result = new CountryModel(
                country.IsoCode,
                country.Name
            );
            return result;
        }

        public IEnumerable<CountryModel> SerializeCountries(IEnumerable<Country> models)
        {
            var result = models.Select(x => this.SerializeCountry(x)).ToArray();
            return result;
        }

        public IssuerModel SerializeIssuer(Issuer issuer)
        {
            var result = new IssuerModel(
                issuer.Id,
                issuer.Name
            );
            return result;
        }
    }
}