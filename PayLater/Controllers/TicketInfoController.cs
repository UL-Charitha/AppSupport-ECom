using DBConnector;
using Domain;
using PayLater.Domain.Util;
using PaylaterException;
using PaylaterException.Ticket;
using PSSServicesConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayLater.Controllers
{
    public class TicketInfoController : ApiController
    {
        DBUtility dbUtil = new DBUtility();
        TicketInfo ticketInfo;
        Helper helper = new Helper();

        [Route("api/TicketInfo/{paymentID}/{bankcode}")]
        public IHttpActionResult GetPNR(string paymentID, string bankCode)
        {
            try
            {
                PaylaterLogger.Info("TicketInfo call received. " + paymentID + " " + bankCode);
                
                //Get office id related to bank id
                string officeId = dbUtil.GetOfficeId(bankCode);
                PaylaterLogger.Info("Using office id - " + officeId);
                PNRService pnrService = new PNRService(officeId);

                ticketInfo = new TicketInfo(Convert.ToInt32(paymentID));
                // Validate via DB Info
                bool DBvaluesUpdated = SetTicketInfoValues(ticketInfo);
                // Fill with Amadeus Info
                bool TicketInfoUpdated = pnrService.GetTicketInfo(ticketInfo);
                //Finally Generate HTML
                bool done = GenerateHtmlTicketInformation(ticketInfo);
                ticketInfo.responseCode = 0;
                ticketInfo.msg = StatusMessage.SUCCESS;
                // return the wrapper Response !
                return Ok(helper.CreateTicketInfoClientResponse(ticketInfo));
            }
            catch (TicketingFailedAfterPricingException ex)
            {
                ticketInfo.responseCode = ex.ErrorCode;
                ticketInfo.msg = ex.Message;
                return Ok(helper.CreateTicketInfoClientResponse(ticketInfo));
            }
            catch (PaylaterCommonException xx)
            {
                ticketInfo.msg = xx.Message;
                ticketInfo.responseCode = xx.ErrorCode;
                var response = Request.CreateResponse<TicketInfoClientWrapper>(System.Net.HttpStatusCode.BadRequest, helper.CreateTicketInfoClientResponse(ticketInfo));
                if (xx.ErrorCode > 1999 && xx.ErrorCode < 3000) //internal error
                {
                    response = Request.CreateResponse<TicketInfoClientWrapper>(System.Net.HttpStatusCode.InternalServerError, helper.CreateTicketInfoClientResponse(ticketInfo));
                }
                return ResponseMessage(response);
            }
            catch (Exception)
            {
                ticketInfo.msg = StatusMessage.INTERNAL_ERROR;
                ticketInfo.responseCode = 2020;
                var response = Request.CreateResponse<TicketInfoClientWrapper>(System.Net.HttpStatusCode.InternalServerError, helper.CreateTicketInfoClientResponse(ticketInfo));
                return ResponseMessage(response);
            }            
        }

        private bool GenerateHtmlTicketInformation(TicketInfo ticketInfo)
        {
            try
            {
                //todo : bind to html template !!
                ticketInfo.ticketInformation = helper.GenerateHtmlTicketInformation(ticketInfo);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool SetTicketInfoValues(TicketInfo ticketInfo)
        {
            try
            {   
                var info = dbUtil.GetPaymentInfoByPaymentID(ticketInfo.paymentId);

                ticketInfo.pnrID = info.Pnr;
                ticketInfo.bankRef = info.BankRef;
                ticketInfo.TotalPrice = Convert.ToDecimal(info.TotalAmountDestination);
                ticketInfo.TotalPriceCurrency = info.CurrencyDestination;

                return true;
            }
            catch (Exception)
            {               
                throw;
            }          
        }

    }
}
