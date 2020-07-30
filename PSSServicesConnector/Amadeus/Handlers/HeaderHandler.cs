using System;
using System.Collections.Generic;
using System.Text;

using System.ServiceModel;
using PSSServicesConnector.AmadeusWebServices;

namespace com.amadeus.cs.Handlers
{
        abstract class HeaderHandler
        {
            protected ClientBase<AmadeusWebServicesPT> mClient;

            public HeaderHandler(ClientBase<AmadeusWebServicesPT> client)
            {
                mClient = client;
            }
        }
}
