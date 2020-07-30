using System;
using System.Collections.Generic;
using System.Text;
//using com.amadeus.cs.ServiceReference;
using System.Diagnostics;
using PSSServicesConnector.AmadeusWebServices;

namespace com.amadeus.cs.Handlers
{
    class SessionHandler : HeaderHandler
    {
        #region Attributes
        public Session Session {get; set;}
        private SecurityHandler hSecurity;
        #endregion

        public enum TransactionStatusCode { None, Start, Continue, End };

        #region Constructor
        public SessionHandler(AmadeusWebServicesPTClient client, SecurityHandler h)
            : base(client)
        {
            this.Session = null;
            hSecurity = h;
        }
        #endregion

        #region Members
        public void handleSession(TransactionStatusCode transactionStatusCode, string officeId)
        {
            switch (transactionStatusCode)
            {
                case TransactionStatusCode.Start:
                    startSession(officeId);
                    break;
                case TransactionStatusCode.Continue:
                    continueSession();
                    break;
                case TransactionStatusCode.End:
                    endSession();
                    break;
                case TransactionStatusCode.None:
                    hSecurity.fill(officeId);
                    resetSession();
                    break;
                default:
                    break;
            }
        }

        public void AfterReply(TransactionStatusCode requestedStatusCode)
        {
            switch (requestedStatusCode)
            {
                case TransactionStatusCode.Start:
                    Trace.Assert(Session.TransactionStatusCode.Equals("InSeries"));
                    break;
                case TransactionStatusCode.Continue:
                    Trace.Assert(Session.TransactionStatusCode.Equals("InSeries"));
                    break;
                case TransactionStatusCode.End:
                    Trace.Assert(Session.TransactionStatusCode.Equals("End"));
                    break;
                case TransactionStatusCode.None:
                    Trace.Assert(Session.TransactionStatusCode.Equals("End"));
                    break;
                default:
                    break;
            }
        }

        private void startSession(string officeId)
        {
            hSecurity.fill(officeId);
            this.Session = new Session();
            this.Session.TransactionStatusCode = "Start";
        }

        private void continueSession()
        {
            hSecurity.reset();
            int sequenceNumber = int.Parse(this.Session.SequenceNumber) + 1;
            this.Session.SequenceNumber = sequenceNumber.ToString();
        }

        private void endSession()
        {
            continueSession();
            this.Session.TransactionStatusCode = "End";
        }

        private void resetSession()
        {
            this.Session = null;
        }
        #endregion
    }
}
