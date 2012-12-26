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
using System.Diagnostics;

namespace GreenField.IssuerShares.Client.Backend.IssuerShares
{
    public partial class CompanySecurityModel : ICompanySecurity
    {
        public CompanySecurityModel()
        {
        }

        public CompanySecurityModel(String id, String name, String shortName, String ticker, CountryModel countryModel)
            : base(id, name)
        {
            this.ShortName = shortName;
            this.Ticker = ticker;
            this.Country = countryModel;
        }

        [DebuggerStepThrough]
        public override void Accept(ISecurityResolver resolver)
        {
            resolver.Resolve(this);
        }

        ICountry ICompanySecurity.Country { get { return this.Country; } }
    }
}
