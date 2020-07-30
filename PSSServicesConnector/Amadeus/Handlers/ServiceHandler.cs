using System;
using System.Collections.Generic;
using System.Text;
//using com.amadeus.cs.ServiceReference;
using com.amadeus.cs;
using PSSServicesConnector.AmadeusWebServices;

namespace com.amadeus.cs.Handlers
{
        class ServiceHandler
        {
            #region Attributes
            private AmadeusWebServicesPTClient client;
            private SessionHandler hSession;
            private TransactionFlowLinkHandler hLink;
            private SecurityHandler hSecurity;
            private AddressingHandler hAddressing;
            #endregion

            #region Constructor
            public ServiceHandler(String wsap, String endpointConfigurationName)
            {
                client = new AmadeusWebServicesPTClient(endpointConfigurationName);
                hSecurity = new SecurityHandler(client);
                hSession = new SessionHandler(client, hSecurity);
                hLink = new TransactionFlowLinkHandler(client);
                hAddressing = new AddressingHandler(client, wsap);
            }
            #endregion

            #region Members
            public PNR_Reply RetrievePnr(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string pnr, string officeId)
            { 
                BeforeRequest(transactionStatusCode, linkAction,officeId);
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                PNR_Reply reply = client.PNR_Retrieve(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildPnrRetrieveRequest(pnr));
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply;
            }                       

            public Security_SignOutReply security_SignOut(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string officeId)
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId);
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                Security_SignOutReply reply = client.Security_SignOut(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.buildSignOutRequest());
                hSession.Session = session;
                hLink.Link = link;
                return reply;
            }

            private void BeforeRequest(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string officeId)
            {
                hSession.handleSession(transactionStatusCode, officeId);
                hLink.handleLinkAction(linkAction);
                hAddressing.update();
            }

            private void AfterReply(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction) 
            {
                hSession.AfterReply(transactionStatusCode);
                hLink.AfterReply(linkAction);
            }

            #endregion

            public Ticket_RetrieveListOfTSMReply RetrieveListOfTSM(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction,string officeId)
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId);
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                Ticket_RetrieveListOfTSMReply reply = client.Ticket_RetrieveListOfTSM(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildTicket_RetrieveListOfTSMRequest());
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply;
            }



            public Fare_ConvertCurrencyReply RetrievefareConvert(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string origCur, string destCur, string officeId, double baseAmount)
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId);
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                Fare_ConvertCurrencyReply reply = client.Fare_ConvertCurrency(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildFareConvertRequest(origCur, destCur, baseAmount));
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply;
            }

            public FOP_CreateFormOfPaymentReply CreateFormOfPayment(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction,string officeId) 
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId);
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                FOP_CreateFormOfPaymentReply reply = client.FOP_CreateFormOfPayment(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildCreateFormOfPaymentRequest());
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply; 
            }

            public PNR_Reply CreatePNR_AddMultiElements(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string officeId)
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId);
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                PNR_Reply reply = client.PNR_AddMultiElements(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildPNRAddMultiElementsRequest());
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply;
            }

            public DocIssuance_IssueTicketReply CreateIssueTicket(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string officeId)
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId); 
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                DocIssuance_IssueTicketReply reply = client.DocIssuance_IssueTicket(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildDocIssuance_IssueTicketRequest());
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply;
            }

            public DocIssuance_IssueCombinedReply CreateIssueCombined(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string officeId)
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId); 
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                DocIssuance_IssueCombinedReply reply = client.DocIssuance_IssueCombined(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildDocIssuance_IssueCombinedRequest());
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply;
            }

            public Queue_PlacePNRReply CreateQueuePlacePNR(SessionHandler.TransactionStatusCode transactionStatusCode, TransactionFlowLinkHandler.TransactionFlowLinkAction linkAction, string pnr, string queueId, string officeId, string queueOfficeId)
            {
                BeforeRequest(transactionStatusCode, linkAction,officeId);
                Session session = hSession.Session;
                TransactionFlowLinkType link = hLink.Link;
                Queue_PlacePNRReply reply = client.Queue_PlacePNR(ref session, ref link, hSecurity.getHostedUser(), MessageFactory.BuildCreateQueuePlacePNRRequest(pnr, queueId, queueOfficeId));
                hSession.Session = session;
                hLink.Link = link;
                AfterReply(transactionStatusCode, linkAction);
                return reply;
            }
        }
}
