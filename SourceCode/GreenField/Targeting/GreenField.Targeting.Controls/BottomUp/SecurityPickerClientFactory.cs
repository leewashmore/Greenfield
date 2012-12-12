using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Controls;
using System.Diagnostics;
using Aims.Data.Client;

namespace GreenField.Targeting.Controls.BottomUp
{
    internal class SecurityPickerClientFactory : ISecurityPickerClientFactory
    {
        public const Int32 MaxNumberOfSecuritiesToPick = 20;
        private class Client : ISecurityPickerClient
        {
            private IClientFactory clientFactory;

            [DebuggerStepThrough]
            public Client(IClientFactory clientFactory)
            {
                this.clientFactory = clientFactory;
            }

            public void RequestSecurities(String pattern, Action<IEnumerable<ISecurity>> callback, Action<Exception> errorHandler)
            {
                var client = this.clientFactory.CreateClient();
                client.PickSecuritiesAsync(pattern, MaxNumberOfSecuritiesToPick);
                client.PickSecuritiesCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Getting securities like \"" + pattern + "\"", args, x => x.Result.Select(y => Helper.As<ISecurity>(y)), callback, errorHandler);
            }
        }


        private IClientFactory clientFactory;

        [DebuggerStepThrough]
        public SecurityPickerClientFactory(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public ISecurityPickerClient CreateSecurityPickerClient()
        {
            var result = new Client(this.clientFactory);
            return result;
        }
    }
}
