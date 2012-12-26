using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Aims.Data.Client;

namespace GreenField.IssuerShares.Client.Backend.IssuerShares
{
    public static class Extender
    {
        public static SecurityModel ToSecurityModel(this ISecurity security)
        {
            var resolver = new ToSecurityModel_ISecurityResolver();
            security.Accept(resolver);
            return resolver.Result;
        }

        private class ToSecurityModel_ISecurityResolver : ISecurityResolver
        {
            public SecurityModel Result { get; private set; }

            public void Resolve(IFund fund)
            {
                this.Result = fund.ToFundModel();
            }

            public void Resolve(ICompanySecurity security)
            {
                this.Result = security.ToCompanySecurityModel();
            }
        }

        public static FundModel ToFundModel(this IFund fund)
        {
            var result = new FundModel(
                fund.Id,
                fund.Name,
                fund.ShortName,
                fund.Ticker
            );
            return result;
        }

        public static CompanySecurityModel ToCompanySecurityModel(this ICompanySecurity companySecurity)
        {
            var result = new CompanySecurityModel(
                companySecurity.Id,
                companySecurity.Name,
                companySecurity.ShortName,
                companySecurity.Ticker,
                companySecurity.Country.ToCountryModel()
            );
            return result;
        }

        public static CountryModel ToCountryModel(this ICountry country)
        {
            var result = new CountryModel(
                country.Name,
                country.IsoCode
            );
            return result;
        }
    }
}
