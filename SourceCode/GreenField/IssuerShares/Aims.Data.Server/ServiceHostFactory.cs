using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace Aims.Data.Server
{
    public abstract class ServiceHostFactoryBase<TService, TSettings> : ServiceHostFactory
    {
        private class ServiceHostBase : ServiceHost
        {
            private class ServiceInstanceProviderBase : IInstanceProvider, IContractBehavior
            {
                private readonly TSettings settings;
                private readonly Func<TSettings, TService> serviceFactory;

                [DebuggerStepThrough]
                public ServiceInstanceProviderBase(TSettings settings, Func<TSettings, TService> serviceFactory)
                {
                    this.settings = settings;
                    this.serviceFactory = serviceFactory;
                }

                // instance provider methods

                public object GetInstance(InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
                {
                    return this.GetInstance(instanceContext);
                }

                public Object GetInstance(InstanceContext instanceContext)
                {
                    var instance = this.serviceFactory(this.settings);
                    return instance;
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

            public ServiceHostBase(
                TSettings settings,
                Func<TSettings, TService> serviceFactory,
                Type serviceType,
                params Uri[] baseAddresses
            )
                : base(serviceType, baseAddresses)
            {
                foreach (var contractDescription in this.ImplementedContracts.Values)
                {
                    var provider = new ServiceInstanceProviderBase(settings, serviceFactory);
                    contractDescription.Behaviors.Add(provider);
                }
            }
        }

        private readonly TSettings settings;
        public ServiceHostFactoryBase()
        {
            this.settings = CreateSettings();
        }

        protected abstract TSettings CreateSettings();
        protected abstract TService CreateService(TSettings settings);

        protected override ServiceHost CreateServiceHost(Type serviceType,
            Uri[] baseAddresses)
        {
            var serviceHost = new ServiceHostBase(
                this.settings,
                this.CreateService,
                serviceType,
                baseAddresses
            );
            return serviceHost;
        }
    }
}