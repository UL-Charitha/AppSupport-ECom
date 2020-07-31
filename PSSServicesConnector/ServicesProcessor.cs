using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayLater.Domain;
using com.amadeus.cs;
using com.amadeus.cs.Handlers;
using PSSServicesConnector.AmadeusWebServices;
using System.Configuration;
using PaylaterException.Fare;
using PayLater.Domain.Util;
using PaylaterException;
using PaylaterException.Ticket;
using System.Globalization;
using Domain;
using DBConnector;

namespace PSSServicesConnector
{
    public class ServicesProcessor
    {
        private ServiceHandler serviceHandler = new ServiceHandler(ConfigurationManager.AppSettings["wsap"], ConfigurationManager.AppSettings["portName"]);
        private string OfficeId;
        public ServicesProcessor(string officeId)
        {
            this.OfficeId = officeId;
        }

        public bool SetTotalFare(Pnr pnr)
        {
            double baseFare;
            double servicesFare = 0;
            string originCurrency;

            try
            {
                // Calculate for PNR BaseFare
                PNR_Reply reply = serviceHandler.RetrievePnr(SessionHandler.TransactionStatusCode.Start,
                        TransactionFlowLinkHandler.TransactionFlowLinkAction.New, pnr.pnrID, OfficeId);

                baseFare = ProcessResponsePnrRetrieve(reply);
                originCurrency = GetOriginCurrency(reply);

                var OfficeIdX = reply.sbrCreationPosDetails.sbrUserIdentificationOwn.originIdentification.inHouseIdentification1;
                var ofcX2 = reply.sbrUpdatorPosDetails.sbrUserIdentificationOwn.originIdentification.inHouseIdentification1;
                var x2 = reply.technicalData.purgeDateData.dateTime;

                pnr.expDate = SetPnrExpiryDate(reply);
                if (pnr.expDate < DateTime.UtcNow)
                {
                    throw new PaylaterCommonException(3002, StatusMessage.PNR_EXPIRED);
                }
                //todo check if pnr is expired - DONE
                //todo check whether the pnr is already paid - DONE
                // Calculate for PNR Services Fare
                Ticket_RetrieveListOfTSMReply RetrieveListOfTSMReply = serviceHandler.RetrieveListOfTSM(SessionHandler.TransactionStatusCode.Continue,
                                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, OfficeId);
                servicesFare = ProcessRetrieveListOfTSMReply(RetrieveListOfTSMReply, pnr);

                // Total Amount to be paid for the PNR : returned val of API/PNR/....
                pnr.totalAmountOrigin = baseFare + servicesFare;
                pnr.originCurrency = originCurrency;
                // Get Pax List from PNR
                pnr.paxList = GetPNRPaxList(reply);

                return true;
            }

            catch (PnrAlreadyPaidException)
            {
                throw;
            }
            catch (PaylaterCommonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                if (ex.Message.Contains(AmadeusStatusMessage.INVALID_RECORD_LOCATOR) || ex.Message.Contains(AmadeusStatusMessage.NO_MATCH_FOR_RECORD_LOCATOR))
                {
                    throw new PnrNotFoundException(3000, StatusMessage.PNR_NOT_FOUND);
                }
                else
                {
                    throw new PaylaterCommonException(2001, StatusMessage.INTERNAL_ERROR);
                }
            }
        }

        public bool SetTotalFareForCspModel(Pnr pnr , CspModel model)
        {
            double baseFare;
            double servicesFare = 0;
            string originCurrency;

            try
            {
                // Calculate for PNR BaseFare
                PNR_Reply reply = serviceHandler.RetrievePnr(SessionHandler.TransactionStatusCode.Start,
                        TransactionFlowLinkHandler.TransactionFlowLinkAction.New, pnr.pnrID, OfficeId);

                baseFare = ProcessResponsePnrRetrieveForCspModel(reply , model);
                originCurrency = GetOriginCurrency(reply);

                model.OfficeId = reply.sbrCreationPosDetails.sbrUserIdentificationOwn.originIdentification.inHouseIdentification1;
                model.OfficeId += " ( " + reply.sbrUpdatorPosDetails.sbrUserIdentificationOwn.originIdentification.inHouseIdentification1 + " )";
                var purgeDate = reply.technicalData.purgeDateData.dateTime;
                var p11 = reply.technicalData.purgeDateData.dateTime.ToString();

                //pnr.expDate = SetPnrExpiryDate(reply);
                model.TicketedDateString = SetPnrTicketedDate(reply);
                //if (pnr.expDate < DateTime.UtcNow)
                //{
                //    throw new PaylaterCommonException(3002, StatusMessage.PNR_EXPIRED);
                //}

                // Calculate for PNR Services Fare
                Ticket_RetrieveListOfTSMReply RetrieveListOfTSMReply = serviceHandler.RetrieveListOfTSM(SessionHandler.TransactionStatusCode.Continue,
                                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, OfficeId);

                if (RetrieveListOfTSMReply.detailsOfRetrievedTSMs == null)
                {
                    model.hasServices = "No";
                }
                else
                {
                    model.hasServices = "Yes";
                }

                servicesFare = ProcessRetrieveListOfTSMReply(RetrieveListOfTSMReply, pnr);
                model.servicesFare = servicesFare;

                // Total Amount to be paid for the PNR : returned val of API/PNR/....
                pnr.totalAmountOrigin = baseFare + servicesFare;
                pnr.originCurrency = originCurrency;
                // Get Pax List from PNR
                pnr.paxList = GetPNRPaxList(reply);

                model.PaymentAmount = pnr.totalAmountOrigin;
                model.paymentCurrency = originCurrency;
                return true;
            }

            catch (PnrAlreadyPaidException)
            {
                throw;
            }
            catch (PaylaterCommonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                if (ex.Message.Contains(AmadeusStatusMessage.INVALID_RECORD_LOCATOR) || ex.Message.Contains(AmadeusStatusMessage.NO_MATCH_FOR_RECORD_LOCATOR))
                {
                    throw new PnrNotFoundException(3000, StatusMessage.PNR_NOT_FOUND);
                }
                else
                {
                    throw new PaylaterCommonException(2001, StatusMessage.INTERNAL_ERROR);
                }
            }
        }
        private List<string> GetPNRPaxList(PNR_Reply reply)
        {
            var travellerInfoList = reply.travellerInfo;
            List<string> PaxList = null;
            if (travellerInfoList != null)
            {
                PaxList = new List<string>();
                try
                {
                    foreach (var paxinfo in travellerInfoList)
                    {
                        foreach (var pax in paxinfo.passengerData)
                        {
                            string fName;
                            string lName;
                            try
                            {
                                fName = pax.travellerInformation.passenger[0].firstName;
                            }
                            catch (Exception)
                            {
                                fName = " ";
                            }

                            try
                            {
                                lName = pax.travellerInformation.traveller.surname;
                            }
                            catch (Exception)
                            {
                                lName = "";
                            }
                            PaxList.Add(lName + " " + fName);
                        }
                    }
                    return PaxList;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private DateTime SetPnrExpiryDate(PNR_Reply reply)
        {
            DBUtility dbUtility = new DBUtility();
            var dataElementsIndivList = reply.dataElementsMaster.dataElementsIndiv;
            foreach (var dataElementsIndiv in dataElementsIndivList)
            {
                var ticketElement = dataElementsIndiv.ticketElement;
                if (ticketElement != null)
                {
                    if (ticketElement.ticket.indicator == "XL")
                    {
                        string time = ticketElement.ticket.time;// 1300;

                        DateTime date = DateTime.ParseExact(ticketElement.ticket.date, "ddMMyy", CultureInfo.InvariantCulture);
                        TimeSpan ts = new TimeSpan(Convert.ToInt16(time.Substring(0, 2)), Convert.ToInt16(time.Substring(2, 2)), 00);
                        date = date + ts;

                        //above date is departure station local time. get UTC time
                        var originDestinationDetails = reply.originDestinationDetails;
                        var itineraryInfoList = originDestinationDetails[0].itineraryInfo;
                        string boardPoint = itineraryInfoList[0].travelProduct.boardpointDetail.cityCode;

                        double timeDifference = dbUtility.GetUTCDifference(boardPoint);                        
                        return date-TimeSpan.FromMinutes(timeDifference);
                        //todo : Code for UTC Fix
                        //date.Subtract(new TimeSpan(1,1,0));
                        //date.Add(new TimeSpan(1,1,0));
                        
                    }
                }
            }
            return DateTime.Now;
            //throw new PaylaterCommonException(2010, StatusMessage.MISSING_EXPIRY_DATE);
        }

        private string SetPnrTicketedDate(PNR_Reply reply)
        {
            var dataElementsIndivList = reply.dataElementsMaster.dataElementsIndiv;
            foreach (var dataElementsIndiv in dataElementsIndivList)
            {
                var ticketElement = dataElementsIndiv.ticketElement;
                if (ticketElement != null)
                {
                    if (ticketElement.ticket.indicator == "OK")
                    {
                        //string time = ticketElement.ticket.time;// 1300;
                        DateTime date = DateTime.ParseExact(ticketElement.ticket.date, "ddMMyy", CultureInfo.InvariantCulture);
                        var dateString = date.ToString("dd.MMM.yyyy") +" ("+ ticketElement.ticket.officeId + ")";

                        //above date is departure station local time. get UTC time
                        //var originDestinationDetails = reply.originDestinationDetails;
                        //var itineraryInfoList = originDestinationDetails[0].itineraryInfo;
                        //string boardPoint = itineraryInfoList[0].travelProduct.boardpointDetail.cityCode;
                        return dateString;
                    }
                }
            }
            return "";
        }
        

        public bool SetConvertedAmount(Pnr pnrObj)
        {
            try
            {
                Fare_ConvertCurrencyReply fareConvertReply = serviceHandler.RetrievefareConvert(SessionHandler.TransactionStatusCode.Continue,
    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, pnrObj.originCurrency, pnrObj.DestinationCurrency, OfficeId, pnrObj.totalAmountOrigin);

                //double convertedAmount = pnrObj.totalAmountOrigin * GetConversionRate(fareConvertReply);//500EUR--> 1980AED:rate=3.942682 :OK.
                //pnrObj.totalAmountDestination = Math.Round(convertedAmount, 2);

                // updated as the UAT finding : Set to default RAF type value
                pnrObj.totalAmountDestination = GetConvertedFareAmount(fareConvertReply);

                return true;
            }
            catch (PaylaterCommonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2021, StatusMessage.PNR_FARE_CONVERSION_FAILED);
            }
        }

        private double GetConvertedFareAmount(Fare_ConvertCurrencyReply fareConvertReply)
        {
            try
            {
                var conversionDetails = fareConvertReply.conversionDetails;
                var convertedAmounts = conversionDetails[0].convertedAmount;

                foreach (var convertedAmount in convertedAmounts)
                {
                    if (convertedAmount.monetaryInfo.monetaryDetails.typeQualifier == "RAF")
                    {
                        return Convert.ToDouble(convertedAmount.monetaryInfo.monetaryDetails.amount);
                    }
                }
                // if no "RAF" element type
                throw new PaylaterCommonException(2021, StatusMessage.PNR_FARE_CONVERSION_FAILED);
            }
            catch (Exception)
            {
                throw new PaylaterCommonException(2021, StatusMessage.PNR_FARE_CONVERSION_FAILED);
            }
        }

        //       #region SupportMethods
        #region SupportMethods

        private double ProcessResponsePnrRetrieve(PNR_Reply reply)
        {
            string amountTotal = "";
            String currency;
            List<string> amountsList = new List<string>();
            List<Int16> paxCountList = new List<Int16>();
            double baseFare = 0;
            string fpcash = "";
            // Start : Reject If Already Paid !! -- Charitha 25APR
            var dataElementsIndiv = reply.dataElementsMaster.dataElementsIndiv;

            foreach (var element in dataElementsIndiv)
            {
                if (element.elementManagementData.segmentName == "FP")
                {
                    fpcash = "FP";
                    if (element.otherDataFreetext[0].longFreetext == "CASH")
                    {
                        fpcash = "FPCASH";
                    }
                    break;
                }
            }

            if (fpcash == "FPCASH")
            {
                string PaymentMode = "Paid"; // Paid
            }
            // End Reject section
            try
            {
                if (reply.tstData != null)
                {
                    PNR_ReplyTstData[] tstDataList = reply.tstData;
                    foreach (var tstData in tstDataList)
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
                }
                else
                {
                    if (reply.originDestinationDetails == null)
                    {
                        //throw new PaylaterCommonException(3002, StatusMessage.PNR_EXPIRED);
                    }
                    else
                    {
                        //throw new PaylaterCommonException(3001, StatusMessage.PNR_NOT_PRICED);
                    }

                }

                return baseFare;
            }
            catch (PaylaterCommonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2002, StatusMessage.INTERNAL_ERROR);
            }
        }

        
        private double ProcessResponsePnrRetrieveForCspModel(PNR_Reply reply , CspModel model)
        {
            string amountTotal = "";
            String currency;
            List<string> amountsList = new List<string>();
            List<Int16> paxCountList = new List<Int16>();
            double baseFare = 0;
            string fpcash = "";
            model.paymentStatus = "";
            model.contactInfo = "";
            model.emailOnPnr = "";
            // Start : Reject If Already Paid !! -- Charitha 25APR
            var dataElementsIndiv = reply.dataElementsMaster.dataElementsIndiv;

            foreach (var element in dataElementsIndiv)
            {
                if (element.elementManagementData.segmentName == "FP")
                {
                    fpcash = "FP";
                    model.paymentStatus = "FP : " + element.otherDataFreetext[0].longFreetext;
                    if (element.otherDataFreetext[0].longFreetext == "CASH")
                    {
                        fpcash = "FPCASH";
                    }
                    break;
                }
            }

            foreach (var element in dataElementsIndiv)
            {
                if (element.elementManagementData.segmentName == "AP")
                {                   
                    if (element.otherDataFreetext[0].longFreetext.Contains("@") && model.emailOnPnr == "")
                    {
                        model.emailOnPnr = element.otherDataFreetext[0].longFreetext;
                    }
                    else
                    {
                        if (!model.contactInfo.Contains(element.otherDataFreetext[0].longFreetext))
                        {
                            model.contactInfo += element.otherDataFreetext[0].longFreetext + " . ";
                        }                    
                    }
                }
            }

            if (fpcash == "FPCASH")
            {
                string PaymentMode = "Paid"; // Paid
            }
            // End Reject section
            try
            {
                if (reply.tstData != null)
                {
                    PNR_ReplyTstData[] tstDataList = reply.tstData;
                    foreach (var tstData in tstDataList)
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
                }
                else
                {
                    if (reply.originDestinationDetails == null)
                    {
                        //throw new PaylaterCommonException(3002, StatusMessage.PNR_EXPIRED);
                    }
                    else
                    {
                        //throw new PaylaterCommonException(3001, StatusMessage.PNR_NOT_PRICED);
                    }

                }

                return baseFare;
            }
            catch (PaylaterCommonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2002, StatusMessage.INTERNAL_ERROR);
            }
        }

        private double ProcessRetrieveListOfTSMReply(Ticket_RetrieveListOfTSMReply RetrieveListOfTSMReply, Pnr pnr)
        {
            List<string> amountsList = new List<string>();
            String currency;
            double ServicesFare = 0;

            try
            {
                if (RetrieveListOfTSMReply.detailsOfRetrievedTSMs != null)
                {
                    pnr.hasServices = true;
                    var detailsOfRetrievedTSMs = RetrieveListOfTSMReply.detailsOfRetrievedTSMs;
                    foreach (var retrievedTSM in detailsOfRetrievedTSMs)
                    {
                        amountsList.Add(retrievedTSM.totalAmount.monetaryDetails.amount);
                        currency = retrievedTSM.totalAmount.monetaryDetails.currency;
                    }
                    foreach (string val in amountsList)
                    {
                        ServicesFare = ServicesFare + Convert.ToDouble(val);
                    }
                }
                return ServicesFare;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2003, StatusMessage.INTERNAL_ERROR);
            }
        }

        public double GetConversionRate(Fare_ConvertCurrencyReply fareConvertReply)
        {
            try
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
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2021, StatusMessage.PNR_FARE_CONVERSION_FAILED);
            }
        }

        private string GetOriginCurrency(PNR_Reply reply)
        {
            try
            {
                String currency = "";
                PNR_ReplyTstData[] tstDataList = reply.tstData;

                if (tstDataList == null)
                {
                    return currency;
                }
                foreach (var tstData in tstDataList)
                {
                    var monetaryInfo = tstData.fareData.monetaryInfo;
                    foreach (var info in monetaryInfo)
                    {
                        if (info.qualifier == "T")
                        {
                            currency = info.currencyCode;
                            return currency;
                        }
                    }
                }
                return currency;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2004, StatusMessage.INTERNAL_ERROR);
            }
        }

        private void ProcessFormOfPaymentReply(FOP_CreateFormOfPaymentReply formOfPaymentReply)
        {
            try
            {
                FOP_CreateFormOfPaymentReplyFopDescription[] fopDes = formOfPaymentReply.fopDescription;
                if (fopDes[0].fpElementError != null)
                {
                    if (fopDes[0].fpElementError.errorWarningDescription.freeText[0] == "FARE ELEMENT ALREADY EXISTS FOR PASSENGER/SEGMENT")
                    {
                        throw new PnrAlreadyPaidException(4003, StatusMessage.PNR_ALREADY_PAID);
                    }
                }

                var mopDes = fopDes[0].mopDescription;
                var fopPNRDetails = mopDes[0].mopDetails.fopPNRDetails;
                var pnrSupplementaryData = mopDes[0].mopDetails.pnrSupplementaryData;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2007, StatusMessage.INTERNAL_ERROR);
            }
        }

        private void ProcessPNRAddMultiElementsReply(PNR_Reply PNRAddMultiElementsReply)
        {
            try
            {
                string pass = "";
                var headers = PNRAddMultiElementsReply.pnrHeader;
                var securityInfo = PNRAddMultiElementsReply.securityInformation;
                var travellerInfo = PNRAddMultiElementsReply.travellerInfo;
                var tstData = PNRAddMultiElementsReply.tstData;
                var pricingRecordGroup = PNRAddMultiElementsReply.pricingRecordGroup;
                var dataElementsIndiv = PNRAddMultiElementsReply.dataElementsMaster.dataElementsIndiv;
                foreach (var element in dataElementsIndiv)
                {
                    if (element.elementManagementData.segmentName == "FP")
                    {
                        pass = "FP";
                        if (element.otherDataFreetext[0].longFreetext == "CASH")
                        {
                            pass = "FPCASH";
                        }
                        break;
                    }
                }

                if (pass != "FPCASH")
                {
                    throw new PaylaterCommonException(2018, StatusMessage.INTERNAL_ERROR);
                }
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2008, StatusMessage.INTERNAL_ERROR);
            }
        }

        private bool ProcessIssueTicketReply(DocIssuance_IssueTicketReply issueTicketReply)
        {
            var statusCode = issueTicketReply.processingStatus.statusCode;
            var errorCode = issueTicketReply.errorGroup.errorOrWarningCodeDetails.errorDetails.errorCode;
            var msg = issueTicketReply.errorGroup.errorWarningDescription.freeText;

            if (statusCode != "O" || errorCode != "OK")
            {
                PaylaterLogger.Error(msg);
                throw new TicketingFailedAfterPricingException(4001, StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS);
            }
            return true;
        }

        public bool ProcessissueCombinedReply(DocIssuance_IssueCombinedReply issueCombinedReply)
        {
            var statusCode = issueCombinedReply.processingStatus.statusCode;
            var errorCode = issueCombinedReply.errorGroup.errorOrWarningCodeDetails.errorDetails.errorCode;
            var msg = issueCombinedReply.errorGroup.errorWarningDescription.freeText;

            if (statusCode != "O" || errorCode != "OK")
            {
                PaylaterLogger.Error(msg);
                throw new TicketingFailedAfterPricingException(4001, StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS);
            }
            return true;
        }

        private bool ProcessqueuePlacePnrReply(Queue_PlacePNRReply queuePlacePnrReply)
        {
            try
            {
                var controlNumber = queuePlacePnrReply.recordLocator.reservation.controlNumber;
                return true;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2020, StatusMessage.QUEUE_PROCESS_FAILED);
            }
        }

        #endregion

        public bool ConfirmPayment()
        {
            try
            {
                FOP_CreateFormOfPaymentReply formOfPaymentReply = serviceHandler.CreateFormOfPayment(SessionHandler.TransactionStatusCode.Continue,
                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, OfficeId);

                ProcessFormOfPaymentReply(formOfPaymentReply);

                PNR_Reply PNRAddMultiElementsReply = serviceHandler.CreatePNR_AddMultiElements(SessionHandler.TransactionStatusCode.Continue,
                                    TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, OfficeId);

                ProcessPNRAddMultiElementsReply(PNRAddMultiElementsReply);
                // End of Payment Services !!! Cheers !!!                
                return true;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2019, StatusMessage.INTERNAL_ERROR);
            }
        }


        public bool IssueDocuments(Pnr relatedPnr)
        {
            try
            {
                if (relatedPnr.hasServices)
                {
                    DocIssuance_IssueCombinedReply issueCombinedReply = serviceHandler.CreateIssueCombined(SessionHandler.TransactionStatusCode.Continue,
                     TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, OfficeId);
                    return ProcessissueCombinedReply(issueCombinedReply);
                }
                else
                {
                    DocIssuance_IssueTicketReply issueTicketReply = serviceHandler.CreateIssueTicket(SessionHandler.TransactionStatusCode.Continue,
                                        TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, OfficeId);
                    return ProcessIssueTicketReply(issueTicketReply);
                }

            }
            catch (TicketingFailedAfterPricingException ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(4001, StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS);
            }
        }


        public bool PlacePnrToQueue(string pnr, string queueId, string queueOfficeId)
        {
            Queue_PlacePNRReply queuePlacePnrReply = serviceHandler.CreateQueuePlacePNR(SessionHandler.TransactionStatusCode.Continue,
                                TransactionFlowLinkHandler.TransactionFlowLinkAction.FollowUp, pnr, queueId, OfficeId, queueOfficeId);
            bool isSuccess = ProcessqueuePlacePnrReply(queuePlacePnrReply);
            return isSuccess;
        }



        public void SetTicketInfo(Payment payment)
        {
            try
            {
                PNR_Reply reply = serviceHandler.RetrievePnr(SessionHandler.TransactionStatusCode.Start,
                        TransactionFlowLinkHandler.TransactionFlowLinkAction.New, payment.pnrID, OfficeId);

                SetPNRTicketInfo(payment, reply);

            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message + " : at SetTicketInfo");
                throw;
            }
        }

        private void SetPNRTicketInfo(Payment payment, PNR_Reply reply)
        {
            try
            {
                payment.tickets = new List<Ticket>();
                var dataElementsIndiv = reply.dataElementsMaster.dataElementsIndiv;

                foreach (var dataElement in dataElementsIndiv)
                {
                    if (dataElement.elementManagementData.segmentName == "FA")
                    {
                        string ticketLongFreetext = dataElement.otherDataFreetext[0].longFreetext;
                        var list = ticketLongFreetext.Split('/');
                        if (list[1] == "ETUL")
                        {
                            Ticket ticket = new Ticket();
                            ticket.paxType = list[0].Split(' ')[0];
                            ticket.ticket = list[0].Split(' ')[1];
                            try
                            {
                                ticket.currency = list[2].Substring(0, 3);
                                ticket.price = Convert.ToDecimal(list[2].Substring(3));
                            }
                            catch (Exception)
                            {// Skip capturing Currency & Amount from Ticket since missing "FARE ON TICKET IT" !!
                                ticket.currency = "";
                                ticket.price = 0;
                            }
                            payment.tickets.Add(ticket);
                        }
                    }
                }
                if (payment.tickets.Count == 0) // >> Not Ticketed !!
                {
                    throw new TicketingFailedAfterPricingException(4001, StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS);
                }
            }

            catch (Exception)
            {
                throw;

            }
        }

        public bool SetTicketInfo(TicketInfo ticketInfo)
        {
            try
            {
                // Get PNR Retrieve Response !!
                PNR_Reply reply = serviceHandler.RetrievePnr(SessionHandler.TransactionStatusCode.Start,
                        TransactionFlowLinkHandler.TransactionFlowLinkAction.New, ticketInfo.pnrID, OfficeId);

                ticketInfo.itineraryInfoList = SetItineraryInfoList(reply);
                ticketInfo.ticketList = SetPNRTicketInfo(reply);
                bool paxSet = SetPaxNames(ticketInfo.ticketList, reply.travellerInfo);

                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool SetPaxNames(List<Ticket> ticketList, PNR_ReplyTravellerInfo[] travellerInfoList)
        {
            try
            {
                foreach (var ticket in ticketList)
                {
                    foreach (var travellerInfo in travellerInfoList)
                    {
                        if (travellerInfo.elementManagementPassenger.reference.qualifier == "PT" && travellerInfo.elementManagementPassenger.reference.number == ticket.paxRef)
                        {
                            var paxList = travellerInfo.enhancedPassengerData;
                            foreach (var pax in paxList)
                            {
                                // set all non-INF to PAX :: to match with Ticket.type >> {PAX/INF}
                                if (pax.enhancedTravellerInformation.travellerNameInfo.type != "INF")
                                {
                                    pax.enhancedTravellerInformation.travellerNameInfo.type = "PAX";
                                }

                                if (pax.enhancedTravellerInformation.travellerNameInfo.type == ticket.paxType)
                                {
                                    ticket.paxFName = pax.enhancedTravellerInformation.otherPaxNamesDetails[0].givenName;
                                    ticket.paxLName = pax.enhancedTravellerInformation.otherPaxNamesDetails[0].surname;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool SetPaxNames(List<Ticket> list, PNR_Reply reply)
        {
            throw new NotImplementedException();
        }

        private List<ItineraryInfo> SetItineraryInfoList(PNR_Reply reply)
        {
            try
            {
                var originDestinationDetails = reply.originDestinationDetails;
                var itineraryInfoList = originDestinationDetails[0].itineraryInfo;

                List<ItineraryInfo> list = new List<ItineraryInfo>();

                foreach (var itineraryInfo in itineraryInfoList)
                {
                    ItineraryInfo itinerary = new ItineraryInfo();

                    itinerary.flightID = itineraryInfo.travelProduct.companyDetail.identification + itineraryInfo.travelProduct.productDetails.identification;
                    itinerary.stationDep = itineraryInfo.travelProduct.boardpointDetail.cityCode;
                    itinerary.stationArr = itineraryInfo.travelProduct.offpointDetail.cityCode;

                    //DateTime date = DateTime.ParseExact(itineraryInfo.travelProduct.product.depDate, "ddMMyy", CultureInfo.InvariantCulture);
                    //TimeSpan ts = new TimeSpan(Convert.ToInt16(itineraryInfo.travelProduct.product.depTime.Substring(0,2)),Convert.ToInt16(itineraryInfo.travelProduct.product.depTime.Substring(2,2)),00);
                    itinerary.dateTimeDep = DateTime.ParseExact(itineraryInfo.travelProduct.product.depDate, "ddMMyy", CultureInfo.InvariantCulture) +
                        new TimeSpan(Convert.ToInt16(itineraryInfo.travelProduct.product.depTime.Substring(0, 2)), Convert.ToInt16(itineraryInfo.travelProduct.product.depTime.Substring(2, 2)), 00);

                    itinerary.dateTimeArr = DateTime.ParseExact(itineraryInfo.travelProduct.product.arrDate, "ddMMyy", CultureInfo.InvariantCulture) +
                        new TimeSpan(Convert.ToInt16(itineraryInfo.travelProduct.product.arrTime.Substring(0, 2)), Convert.ToInt16(itineraryInfo.travelProduct.product.arrTime.Substring(2, 2)), 00);

                    list.Add(itinerary);
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<Ticket> SetPNRTicketInfo(PNR_Reply reply)
        {
            try
            {
                List<Ticket> ticketList = new List<Ticket>();
                var dataElementsIndiv = reply.dataElementsMaster.dataElementsIndiv;

                foreach (var dataElement in dataElementsIndiv)
                {
                    if (dataElement.elementManagementData.segmentName == "FA")
                    {
                        string ticketLongFreetext = dataElement.otherDataFreetext[0].longFreetext;
                        var list = ticketLongFreetext.Split('/');
                        if (list[1] == "ETUL")
                        {
                            Ticket ticket = new Ticket();
                            ticket.paxType = list[0].Split(' ')[0];
                            ticket.ticket = list[0].Split(' ')[1];
                            ticket.currency = list[2].Substring(0, 3);
                            ticket.price = Convert.ToDecimal(list[2].Substring(3));

                            foreach (var item in dataElement.referenceForDataElement)
                            {
                                if (item.qualifier == "PT")
                                {
                                    ticket.paxRef = item.number;
                                    break;
                                }
                            }
                            //ticket.paxRef = dataElement.referenceForDataElement

                            ticketList.Add(ticket);
                        }
                    }
                }
                if (ticketList.Count == 0) // >> Not Ticketed !!
                {
                    throw new TicketingFailedAfterPricingException(4001, StatusMessage.TICKETING_FAILED_AFTER_PAYMENT_SUCCESS);
                }
                else
                {
                    return ticketList;
                }
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
