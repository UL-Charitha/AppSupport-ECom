using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;

namespace com.amadeus.cs.Dispatcher
{
    class SecurityBehavior : IEndpointBehavior
    {
        #region Attributes
        public SecurityTokenInspector inspector { get; set; }
        #endregion

        #region IEndpointBehavior Members
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            return;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            return;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            return;
        }
        #endregion
    }
}
