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
using Aims.Controls;
using System.Diagnostics;
using System.Collections.Generic;
using Aims.Data.Client;
using System.Linq;

namespace GreenField.IssuerShares.Controls
{
    public class SecurityPickerClientFactory : ISecurityPickerClientFactory
    {

        public const Int32 MaxNumberOfSecurities = 20;
        private class Client : ISecurityPickerClient
        {
            private IClientFactory clientFactory;
            private String issuerId;

            [DebuggerStepThrough]
            public Client(IClientFactory clientFactory, String issuerId)
            {
                this.clientFactory = clientFactory;
                this.issuerId = issuerId;
            }

            public void RequestSecurities(String pattern, Action<IEnumerable<ISecurity>> callback, Action<Exception> errorHandler)
            {
                var client = this.clientFactory.CreateClient();
                client.GetIssuerSecuritiesCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Getting securities like \"" + pattern + "\" for issuer (ID: " + this.issuerId + ")", args, x => x.Result.Select(y => Helper.As<ISecurity>(y)), callback, errorHandler);

                client.GetIssuerSecuritiesAsync(pattern, MaxNumberOfSecurities, this.issuerId);
            }
        }

        private IClientFactory clientFactory;

        [DebuggerStepThrough]
        public SecurityPickerClientFactory(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public String IssuerId { get; set; }

        public ISecurityPickerClient CreateSecurityPickerClient()
        {
            var issuerIdOpt = this.IssuerId;
            if (String.IsNullOrEmpty(issuerIdOpt)) throw new ApplicationException("Unable to create the security picker client factory because no security has yet been picked.");

            var result = new Client(this.clientFactory, issuerIdOpt);
            return result;
        }
    }
}
