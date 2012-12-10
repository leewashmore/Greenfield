using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Controls;
using Aims.Data.Client;
using TopDown.FacingServer.Backend.Targeting;
using System.Diagnostics;

namespace GreenField.Targeting.Controls.BasketTargets
{
    internal class SecurityPickerClientFactory : ISecurityPickerClientFactory
    {
        public const Int32 MaxNumberOfSecurities = 20;
        private class Client : ISecurityPickerClient
        {
            private IClientFactory clientFactory;
            private int basketId;

            [DebuggerStepThrough]
            public Client(IClientFactory clientFactory, Int32 basketId)
            {
                this.clientFactory = clientFactory;
                this.basketId = basketId;
            }

            public void RequestSecurities(String pattern, Action<IEnumerable<ISecurity>> callback, Action<Exception> errorHandler)
            {
                var client = this.clientFactory.CreateClient();
                client.PickSecuritiesFromBasketCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Getting securities like \"" + pattern + "\" for basket (ID: " + this.basketId + ")", args, x => x.Result.Select(y => Helper.As<ISecurity>(y)), callback, errorHandler);

                client.PickSecuritiesFromBasketAsync(pattern, MaxNumberOfSecurities, this.basketId);
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
            var basketIdOpt = this.BasketId;
            if (!basketIdOpt.HasValue) throw new ApplicationException("Unable to create the security picker client factory because no basket has yet been picked.");
         
            var result = new Client(this.clientFactory, basketIdOpt.Value);
            return result;
        }

        public Int32? BasketId { get; set; }
    }
}
