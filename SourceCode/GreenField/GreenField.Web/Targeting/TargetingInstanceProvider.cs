using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.Web.Services.Description;
using GreenField.Targeting.Server;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace GreenField.Web.Targeting
{
    public class TargetingInstanceProvider: IInstanceProvider, IContractBehavior
    {
        private readonly FacadeSettings settings;

        [DebuggerStepThrough]
        public TargetingInstanceProvider(FacadeSettings settings)
        {
            this.settings = settings;
        }

        // instance provider methods

        public object GetInstance(InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            return this.GetInstance(instanceContext);
        }

        public Object GetInstance(InstanceContext instanceContext)
        {
            var facade = new GreenField.Web.Services.TargetingOperations(this.settings);
            return facade;
        }

        public void ReleaseInstance(InstanceContext instanceContext, Object instance)
        {
        }


        // contract behavior methods

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceProvider = this;
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }
    }
}