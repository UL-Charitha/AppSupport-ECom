using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using PSSServicesConnector.AmadeusWebServices;
using log4net;

namespace com.amadeus.cs.Handlers
{
    class AddressingHandler : HeaderHandler
    {
        #region Attributes
        private String mWSAP;
        private static readonly ILog log = LogManager.GetLogger(typeof(SecurityHandler));
        #endregion

        #region Constructor
        public AddressingHandler(AmadeusWebServicesPTClient client, String wsap)
            : base(client)
        {
            this.mWSAP = wsap;
        }
        #endregion

        #region Members
        public void setWSAP(String wsap)
        {
            mWSAP = wsap;
        }

        public String getWSAP()
        {
            return mWSAP;
        }

        public void update()
        {
            setWSAP();
        }

        private void setWSAP()
        {
            Uri uri = mClient.Endpoint.Address.Uri;
            String newUri = uri.Scheme + "://" + uri.Host + "/" + this.mWSAP;
            EndpointAddress address = new EndpointAddress(newUri);
            mClient.Endpoint.Address = address;
        }
        #endregion
    }
}
