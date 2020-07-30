using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using com.amadeus.cs;
using com.amadeus.cs.Handlers;
using PSSServicesConnector.AmadeusWebServices;

using log4net;
using log4net.Config;

namespace com.amadeus.cs
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            //RunExample();
            
            Console.Out.WriteLine("\nTerminated session - Press enter to close"); 
            Console.In.Read();
        }

        //public static void RunExample()
        //{
        //    double baseFare;
        //    double servicesFare;
        //    double totalFare;
        //    string pnr = "ASD123";
        //    // enable logging with log4net by adding the log4net reference
        //    // (http://logging.apache.org/log4net/) to the path
        //    string config = Directory.GetCurrentDirectory() + @"\..\..\resources\log4net_config.xml";
        //    XmlConfigurator.Configure(new FileInfo(config));
        //    // To log XML Requests and responses, see app.config

        //    // ---- Start Chari >>>>
        //    ServiceHandler serviceHandler = new ServiceHandler("1ASIWPYLUL", "AmadeusWebServicesPort");   
        //    //serviceHandler.Air_FlightInfo
        //    PNR_Reply reply = serviceHandler.RetrievePnr(SessionHandler.TransactionStatusCode.Start,
        //                        TransactionFlowLinkHandler.TransactionFlowLinkAction.New,pnr);
        //    baseFare = ProcessResponsePnrRetrieve(reply);

        //    Ticket_RetrieveListOfTSMReply RetrieveListOfTSMReply = serviceHandler.RetrieveListOfTS(SessionHandler.TransactionStatusCode.Continue,
        //                        TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    servicesFare = ProcessRetrieveListOfTSMReply(RetrieveListOfTSMReply);
        //    // Total Amount to be paid for the PNR : returned val of API/PNR/....
        //    totalFare = baseFare + servicesFare;

        //    //Fare_ConvertCurrencyReply fareConvertReply = serviceHandler.RetrievefareConvert(SessionHandler.TransactionStatusCode.Continue,
        //    //                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    //double convertedAmount = totalFare * GetConversionRate(fareConvertReply);//500EUR--> 1980AED:rate=3.942682 :OK.
        //    //log.Info("Converted Amount : " + Math.Round(convertedAmount,2));

        //    // //End of Fare Calculation Services !!! Cheers !!!

        //    //FOP_CreateFormOfPaymentReply formOfPaymentReply = serviceHandler.CreateFormOfPayment(SessionHandler.TransactionStatusCode.Continue,
        //    //                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);

        //    //PNR_Reply PNRAddMultiElementsReply = serviceHandler.CreatePNR_AddMultiElements(SessionHandler.TransactionStatusCode.Continue,
        //    //                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    //// End of Payment Services !!! Cheers !!!
        //    //===============================================

        //    //DocIssuance_IssueTicketReply issueTicketReply = serviceHandler.CreateIssueTicket(SessionHandler.TransactionStatusCode.Continue,
        //    //                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);

        //    //DocIssuance_IssueCombinedReply issueCombinedReply = serviceHandler.CreateIssueCombined(SessionHandler.TransactionStatusCode.Continue,
        //    //                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    //// End of Doc Issuance Services !!! Cheers !!!

        //    //Queue_PlacePNRReply queuePlacePnrReply = serviceHandler.CreateQueuePlacePNR(SessionHandler.TransactionStatusCode.Continue,
        //    //                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    // End of Queue Service !!! Cheers !!!

        //    // ---- End Chari !!!!

        //    // Stateless call
        //    log.Info("Stateless call"); 
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.None,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.None);

        //    // Stateful call
        //    log.Info("Stateful call - Start");
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.Start,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.None);
        //    log.Info("Stateful call - Continue");
        //    serviceHandler.Air_FlightInfo(
        //            SessionHandler.TransactionStatusCode.Continue,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.None);
        //    log.Info("Stateful call - End");
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.End,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.None);

        //    // Link usage over stateless
        //    log.Info("Link usage over stateless");
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.None,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.New);
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.None,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.None,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.Reset);

        //    // Link usage over stateful
        //    log.Info("Link usage over statefull");
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.Start,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.New);
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.End,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.Start,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.End,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);

        //    // Link usage over stateless and stateful
        //    log.Info("Link usage over stateless and stateful");
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.None,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.New);
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.Start,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);
        //    serviceHandler.Air_FlightInfo(SessionHandler.TransactionStatusCode.End,
        //            TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp);

        //    log.Info("done.");
        //}

        public static double GetConversionRate(Fare_ConvertCurrencyReply fareConvertReply)
        {
            var conversionDetails = fareConvertReply.conversionDetails;
            string originCurr = conversionDetails[0].conversionRate.conversionRateDetails.originCurrency;
            string rateType = conversionDetails[0].conversionRate.conversionRateDetails.rateType;
            // Important one is the Rate !!
            decimal rate = conversionDetails[0].conversionRate.conversionRateDetails.rate;
            bool rateSpecified = conversionDetails[0].conversionRate.conversionRateDetails.rateSpecified;

            string ConvertedAmount = conversionDetails[0].convertedAmount[0].monetaryInfo.monetaryDetails.amount;
            string desCurrency = conversionDetails[0].convertedAmount[0].monetaryInfo.monetaryDetails.currency;

            return Convert.ToDouble(rate);
        }

        private static double ProcessRetrieveListOfTSMReply(Ticket_RetrieveListOfTSMReply RetrieveListOfTSMReply)
        {
            List<string> amountsList = new List<string>();
            String currency;
            double ServicesFare = 0;
            var detailsOfRetrievedTSMs = RetrieveListOfTSMReply.detailsOfRetrievedTSMs;
            foreach (var retrievedTSM in detailsOfRetrievedTSMs)
            {
                amountsList.Add(retrievedTSM.totalAmount.monetaryDetails.amount);
                currency = retrievedTSM.totalAmount.monetaryDetails.currency;
            }
            foreach(string val in amountsList)
            {
                ServicesFare = ServicesFare + Convert.ToDouble(val);
            }
            return ServicesFare;
        }

        private static double ProcessResponsePnrRetrieve(PNR_Reply reply)
        {
            FareDataType d1;
            string amountTotal="";
            String currency;
            List<string> amountsList = new List<string>();
            List<Int16> paxCountList = new List<Int16>(); 
            double baseFare = 0;

            PNR_ReplyTstData[] tstDataList = reply.tstData;

            foreach(var tstData in tstDataList)
            {
                Int16 count = 0;
                var monetaryInfo = tstData.fareData.monetaryInfo;
                    foreach (var info in monetaryInfo)
                    {
                        if (info.qualifier == "T")
                        {
                            amountTotal = info.amount;
                            currency = info.currencyCode;
                        }
                    } // ok
                var refInfo = tstData.referenceForTstData;
                    foreach (var info in refInfo)
                    {
                        if (info.qualifier == "PT")
                        { count++; }
                    }
                    amountsList.Add(amountTotal);
                    paxCountList.Add(count);
            }
            for (int i = 0; i < amountsList.Count; i++)
            {
                baseFare = baseFare + Convert.ToDouble(amountsList[i]) * Convert.ToInt16(paxCountList[i]);
            }
            return baseFare;
        }
    }
}
    