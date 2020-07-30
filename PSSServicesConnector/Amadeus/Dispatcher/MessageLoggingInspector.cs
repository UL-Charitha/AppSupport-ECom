using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Dispatcher;

using log4net;

namespace com.amadeus.cs
{
    class MessageLoggingInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MessageLoggingInspector));

        #region IClientMessageInspector members
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            log.Debug(reply.ToString());
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            log.Debug(request.ToString());
            return null;
        }
        #endregion

        #region IDispatchMessageInspector members
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            return;
        }
        #endregion
    }
}
