using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayLater.Domain;
using PayLater.Domain.Util;
using com.amadeus.cs;
using com.amadeus.cs.Handlers;
using PSSServicesConnector.AmadeusWebServices;
using PaylaterException.Fare;
using PaylaterException;
using PaylaterException.Ticket;
using Domain;

namespace PSSServicesConnector
{
    public class PNRService
    {
        private string OfficeId;
        private ServicesProcessor servicesProc;
        public PNRService(string officeId)
        {
            this.OfficeId = officeId;
            servicesProc = new ServicesProcessor(OfficeId);
        }        
        
        /// <summary>
        /// Get Base fare / Services Fare + Converted Fare >> Fills the PNR obj
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="destCurrency"></param>
        /// <param name="officeId"></param>
        /// <returns></returns>
        public Pnr GetPnrInfo(string pnr, string destCurrency)
        {
            Pnr pnrObj = new Pnr { pnrID = pnr, expDate = System.DateTime.Now };

            bool isPnrUpdated = servicesProc.SetTotalFare(pnrObj);

            pnrObj.DestinationCurrency = destCurrency;
            if (destCurrency == pnrObj.originCurrency)
            {
                pnrObj.totalAmountDestination = pnrObj.totalAmountOrigin;
            }
            else
            {
                bool isFareConversionUpdated = servicesProc.SetConvertedAmount(pnrObj);
            }
            pnrObj.msg = StatusMessage.SUCCESS;
            pnrObj.responseCode = 0;
            return pnrObj;
        }

        public Pnr GetPnrInfoModel(string pnr, string destCurrency, CspModel model)
        {
            Pnr pnrObj = new Pnr { pnrID = pnr, expDate = System.DateTime.Now };

            bool isPnrUpdated = servicesProc.SetTotalFareForCspModel(pnrObj, model);

            pnrObj.DestinationCurrency = destCurrency;
            if (destCurrency == pnrObj.originCurrency)
            {
                pnrObj.totalAmountDestination = pnrObj.totalAmountOrigin;
            }
            else
            {
                bool isFareConversionUpdated = servicesProc.SetConvertedAmount(pnrObj);
            }
            pnrObj.msg = StatusMessage.SUCCESS;
            pnrObj.responseCode = 0;
            model.LastUpdated = DateTime.Now;
            model.pnrStatus = "Retrieved from 1A";
            return pnrObj;
        }


        public bool ProcessPayment(Payment payment)
        {
            try
            {
                Pnr relatedPnr = this.GetPnrInfo(payment.pnrID, payment.currencyDest);
                payment.totalAmountOrig = relatedPnr.totalAmountOrigin;
                payment.currencyOrig = relatedPnr.originCurrency;
                payment.expDate = relatedPnr.expDate;

                if (relatedPnr.totalAmountDestination != payment.totalAmountDest)
                {
                    PaylaterLogger.Info("incorrect amount for PNR - " + relatedPnr.pnrID);
                    throw new IncorrectAmountForPnrException(4000, StatusMessage.INCORRECT_AMOUNT);
                }

                payment.totalAmountDest = relatedPnr.totalAmountDestination;

                // Confirm Payment : [FPCASH]
                bool isPaymentConfirmed = servicesProc.ConfirmPayment();
                // Issue Tickets : [IssueTicket/Combined]
                bool status = servicesProc.IssueDocuments(relatedPnr);
                if (status)
                {
                    // Set Ticket Info
                    servicesProc.SetTicketInfo(payment);
                    // End Set Ticket Info
                    payment.msg = StatusMessage.SUCCESS;
                    payment.responseCode = 0;
                }
                return status;
            }

            catch (PaylaterCommonException ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw;
            } 
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2011, StatusMessage.INTERNAL_ERROR);
            }
        }

        public bool PlacePnrToQueue(string pnr, string queueId, string queueOfficeId) 
        {
            try
            {
                bool queued = servicesProc.PlacePnrToQueue(pnr, queueId, queueOfficeId);
                return queued;
            }
            catch (PaylaterCommonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                throw new PaylaterCommonException(2013, StatusMessage.INTERNAL_ERROR);
            }
        }

        public bool GetTicketInfo(TicketInfo ticketInfo)
        {
            try
            {
                bool updated = servicesProc.SetTicketInfo(ticketInfo);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
//pnr_retrieve
//if pnr not found
//if pnr expired
//ticket_retrieveListofTsm
//calculate fare
//if currency is different fare_convertcurrency
//save to db in a seperate thread