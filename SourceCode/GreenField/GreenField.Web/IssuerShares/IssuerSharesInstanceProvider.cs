using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.IssuerShares.Server;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace GreenField.Web.IssuerShares
{
    public class IssuerSharesInstanceProvider : IInstanceProvider, IContractBehavior
    {
        private readonly FacadeSettings settings;

        [DebuggerStepThrough]
        public IssuerSharesInstanceProvider(FacadeSettings settings)
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
            var facade = new GreenField.Web.Services.IssuerSharesOperations(this.settings);
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
