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
    public abstract partial class SecurityModel : ISecurity
    {
        public SecurityModel()
        {
        }

        public SecurityModel(String id, String name, String issuerId, String shortName, String securityType, String ticker)
            : this()
        {
            this.Id = id;
            this.Name = name;
            this.IssuerId = issuerId;
            this.ShortName = shortName;
            this.SecurityType = securityType;
            this.Ticker = ticker;
            
        }

        public abstract void Accept(ISecurityResolver resolver);
    }
}
