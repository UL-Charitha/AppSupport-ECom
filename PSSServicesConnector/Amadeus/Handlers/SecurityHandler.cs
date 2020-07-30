using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using PSSServicesConnector.AmadeusWebServices;
using System.Configuration;

using com.amadeus.cs.Dispatcher;

namespace com.amadeus.cs.Handlers
{
        class SecurityHandler : HeaderHandler
        {
            #region Attributes
            private AMA_SecurityHostedUser mHostedUser;
            private SecurityTokenInspector iSecurity;
            private static readonly ILog log = LogManager.GetLogger(typeof(SecurityHandler));
            
            #endregion

            #region Constructor
            public SecurityHandler(AmadeusWebServicesPTClient client) : base(client)
            {

                this.iSecurity = new SecurityTokenInspector(ConfigurationManager.AppSettings["userId"].ToString(), ConfigurationManager.AppSettings["password"].ToString());
                SecurityBehavior behavior = new SecurityBehavior();
                behavior.inspector = this.iSecurity;
                client.Endpoint.Behaviors.Insert(0, behavior);
            }
            #endregion

            #region Members
            public void fill(string officeId)
            {
                addWSSecurity();
                setHostedUser(officeId);
            }

            public void reset()
            {
                mHostedUser = null;
                removeWSSecurity();
            }

            private void removeWSSecurity()
            {
                iSecurity.Activated = false;
            }

            private void addWSSecurity()
            {
                iSecurity.Activated = true;
            }

            private void setHostedUser(string officeId)
            {
                mHostedUser = new AMA_SecurityHostedUser();
                mHostedUser.UserID = new AMA_SecurityHostedUserUserID();
                mHostedUser.UserID.AgentDutyCode = ConfigurationManager.AppSettings["dutyCode"].ToString() ;
                mHostedUser.UserID.POS_Type = "1";
                mHostedUser.UserID.PseudoCityCode = officeId;
                mHostedUser.UserID.RequestorType = "U";
                mHostedUser.UserID.RequestorID = new UniqueID_Type();
                mHostedUser.UserID.RequestorID.CompanyName = new CompanyNameType();
                mHostedUser.UserID.RequestorID.CompanyName.Value = "UL";
            }

            public AMA_SecurityHostedUser getHostedUser() 
            {
                return mHostedUser;
            }
            #endregion
        }
}
