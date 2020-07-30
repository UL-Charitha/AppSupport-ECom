using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;

namespace com.amadeus.cs
{
    class LoggingBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        #region IEndpointBehavior Members
        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            return;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            return;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MessageLoggingInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MessageLoggingInspector());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            return;
        }
        #endregion


        #region BehaviorExtensionElement Members
        public override Type BehaviorType
        {
            get { return this.GetType(); }
        }
        
        protected override object CreateBehavior()
        {
            return new LoggingBehavior();
        }
        #endregion

    }
}
