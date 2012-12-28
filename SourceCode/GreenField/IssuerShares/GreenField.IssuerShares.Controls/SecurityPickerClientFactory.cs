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

        public const Int32 MaxNumberOfSecurities = 30;
        private class Client : ISecurityPickerClient
        {
            private IClientFactory clientFactory;
            private String securityShortName;

            [DebuggerStepThrough]
            public Client(IClientFactory clientFactory, String securityShortName)
            {
                this.clientFactory = clientFactory;
                this.securityShortName = securityShortName;
            }


            public void RequestSecurities(String pattern, Action<IEnumerable<ISecurity>> callback, Action<Exception> errorHandler)
            {
                var client = this.clientFactory.CreateClient();
                client.GetIssuerSecuritiesCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Getting securities like \"" + pattern + "\" for security issuer (ID: " + this.securityShortName + ")", args, x => x.Result.Select(y => Helper.As<ISecurity>(y)), callback, errorHandler);

                client.GetIssuerSecuritiesAsync(pattern, MaxNumberOfSecurities, this.securityShortName);
            }
        }

        private IClientFactory clientFactory;

        [DebuggerStepThrough]
        public SecurityPickerClientFactory(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public void Initialize(string securityShortName)
        {
            this.SecurityShortName = securityShortName;
        }

        public String SecurityShortName { get; private set; }

        public ISecurityPickerClient CreateSecurityPickerClient()
        {
            var securityShortNameOpt = this.SecurityShortName;
            if (String.IsNullOrEmpty(securityShortNameOpt)) throw new ApplicationException("Unable to create the security picker client factory because no security has yet been picked.");

            var result = new Client(this.clientFactory, securityShortNameOpt);
            return result;
        }
    }
}
