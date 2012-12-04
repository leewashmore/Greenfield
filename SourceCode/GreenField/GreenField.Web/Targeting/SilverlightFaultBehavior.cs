using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace GreenField.Web.Targeting
{
    /// <summary>
    /// Behavior file for Silverlight fault exceptions
    /// </summary>
    public class SilverlightFaultBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        /// <summary>
        /// Apply Dispatch Behavior
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint</param>
        /// <param name="endpointDispatcher">EndpointDispatcher</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            SilverlightFaultMessageInspector inspector = new SilverlightFaultMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        /// <summary>
        /// Silverlight Fault Message Inspector
        /// </summary>
        public class SilverlightFaultMessageInspector : IDispatchMessageInspector
        {
            /// <summary>
            /// Before Send Reply
            /// </summary>
            /// <param name="reply">Message</param>
            /// <param name="correlationState">Correlation State</param>
            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                if (reply.IsFault)
                {
                    HttpResponseMessageProperty property = new HttpResponseMessageProperty();

                    // Here the response code is changed to 200.
                    property.StatusCode = System.Net.HttpStatusCode.OK;

                    reply.Properties[HttpResponseMessageProperty.Name] = property;
                }
            }

            /// <summary>
            /// After Receive Request - No custom implimentation applied
            /// </summary>
            /// <param name="request">Message</param>
            /// <param name="channel">IClientChannel</param>
            /// <param name="instanceContext">InstanceContext</param>
            /// <returns>null</returns>
            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                // Do nothing to the incoming message.
                return null;
            }
        }

        /// <summary>
        /// Add Binding Parameters
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint</param>
        /// <param name="bindingParameters">BindingParameterCollection</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Apply Client Behavior
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint</param>
        /// <param name="clientRuntime">ClientRuntime</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="endpoint">ServiceEndpoint</param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        /// <summary>
        /// Behavior Type
        /// </summary>
        public override System.Type BehaviorType
        {
            get { return typeof(SilverlightFaultBehavior); }
        }

        /// <summary>
        /// Create Behavior
        /// </summary>
        /// <returns>SilverlightFaultBehavior</returns>
        protected override object CreateBehavior()
        {
            return new SilverlightFaultBehavior();
        }
    }
}