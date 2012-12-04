using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using GreenField.Targeting.Server;
using System.ServiceModel.Dispatcher;

namespace GreenField.Targeting.Backend.Helpers
{
    public class TargetingServiceHost : ServiceHost
    {
        public TargetingServiceHost(FacadeSettings settings, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            foreach (var contractDescription in this.ImplementedContracts.Values)
            {
                var provider = new TargetingInstanceProvider(settings);
                contractDescription.Behaviors.Add(provider);
            }
        }
    }
}