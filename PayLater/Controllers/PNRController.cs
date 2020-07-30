using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DBConnector;
using PSSServicesConnector;
using PayLater.Domain;
using PayLater.Domain.Util;
using PaylaterException;
using PaylaterException.Fare;
using PaylaterException.Common;
using Domain;

namespace PayLater.Controllers
{
    public class PNRController : ApiController
    {
        private string test;
        Helper helper = new Helper();
        /// <summary>
        /// paylater new
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="bankId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Route("api/pnr/{pnr}/{bankcode}/{currency}")]
        public IHttpActionResult GetPNR(string pnr, string bankCode, string currency)
        {
            currency = currency.ToUpper();
            Helper helper = new Helper();
            // Test for request Validation @ 27APR
            try
            {
                ValidatePnrFareRequest(pnr, bankCode, currency);
            }
            catch (InvalidRequestInputException ex)
            {
                PnrClientWrapper pnrResponse = new PnrClientWrapper();
                pnrResponse.responseMessage = ex.Message;
                pnrResponse.responseCode = ex.ErrorCode;
                pnrResponse.currency = currency;
                pnrResponse.pnr = pnr;
                pnrResponse.expiry = null;
                pnrResponse.pnrTotalValue = null;
                
                var response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.BadRequest, pnrResponse);
                return ResponseMessage(response);
            }
            // End : Test for request Validation

            Pnr pnrRet;

            try
            {
                PaylaterLogger.Info("Booking received. " + pnr + " " + bankCode + " " + currency);
                DBUtility dbUtil = new DBUtility();

                //Get office id related to bank id
                //string officeId = "CMBUL08P2";//dbUtil.GetOfficeId(bankCode);

                string officeId = "LONUL08P1"; // dbUtil.GetOfficeIdNew(bankCode);

                PaylaterLogger.Info("Using office id - " + officeId);
                PNRService pnrService = new PNRService(officeId);
                
                //todo check the bank id format in existing paylater

                //Get pnr object with calculated fare in it.
                pnrRet = pnrService.GetPnrInfo(pnr, currency);

                // Saving to DB : dbo.PYL_T_PNR_FARE 
                pnrRet.requestingBankCode = bankCode;
                pnrRet.transactionUTCTime = System.DateTime.UtcNow;
                bool saved = dbUtil.SavePnrFareRequest(pnrRet);                
                return Ok(helper.CreatePnrClientResponse(pnrRet));
            }
            
            catch (PaylaterCommonException ex)
            {                
                PnrClientWrapper pnrResponse = new PnrClientWrapper() { pnr = pnr, pnrTotalValue = null, currency = currency, expiry = null };
                pnrResponse.responseMessage = ex.Message;
                pnrResponse.responseCode = ex.ErrorCode;                
                var response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.BadRequest, pnrResponse);
                if (ex.ErrorCode > 1999 && ex.ErrorCode < 3000) //internal error
                {
                    response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.InternalServerError, pnrResponse);
                }
                return ResponseMessage(response);
   
            }
            catch (Exception ex)
            {
                PaylaterLogger.Error(ex.Message);
                PnrClientWrapper pnrResponse = new PnrClientWrapper() { pnr = pnr, pnrTotalValue = null, currency = currency, expiry = null };
                pnrResponse.responseMessage = StatusMessage.INTERNAL_ERROR;
                pnrResponse.responseCode = 2015;
                var response = Request.CreateResponse<PnrClientWrapper>(System.Net.HttpStatusCode.InternalServerError, pnrResponse);
                return ResponseMessage(response);                
            }
        }

        private void ValidatePnrFareRequest(string pnr, string bankCode, string currency)
        {
            try
            {
                if (pnr.Length != 6)
                {
                    throw new InvalidRequestInputException(4004, StatusMessage.INCORRECT_PNR_INPUT);
                }
                if (bankCode.Length > 8 || bankCode.Length < 4)
                {
                    throw new InvalidRequestInputException(4007, StatusMessage.INCORRECT_BANK_CODE_INPUT);
                }
                if (currency.Length != 3)
                {
                    throw new InvalidRequestInputException(4005, StatusMessage.INCORRECT_PNR_DESTINATION_CURRENCY_INPUT);
                }
            }
            catch (InvalidRequestInputException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InvalidRequestInputException(4010, StatusMessage.WRONG_REQUEST_INPUTS);
            }
        }
      

        private string GetPnrInfo(string pnr, string cur)
        {
            return String.Format("The Request for PNR: {0} in {1} was Recieved", pnr, cur);
        }

        [Route("api/pnr2/{pnr}")]
        public IHttpActionResult GetPNRx(string pnr) 
        {
            // http://localhost:13235/api/PNR/WVH93G/fP9n/LKR
            string currency = "LKR";
            Pnr pnrRet;
            TicketInfo ticketInfo;
            string paymentID = "1234";
            CspModel model = new CspModel();

            try
            {
                DBUtility dbUtil = new DBUtility();

                //Get office id related to bank id
                //string officeId = "CMBUL08P2";//dbUtil.GetOfficeId(bankCode);
                string officeId = "LONUL08P1"; // dbUtil.GetOfficeIdNew(bankCode);

                PaylaterLogger.Info("Using office id - " + officeId);
                PNRService pnrService = new PNRService(officeId);
                //todo check the bank id format in existing paylater

                //Get pnr object with calculated fare in it.
                pnrRet = pnrService.GetPnrInfoModel(pnr, currency, model);

                ticketInfo = new TicketInfo(Convert.ToInt32(paymentID));
                ticketInfo.pnrID = pnr;
                // Validate via DB Info
                //bool DBvaluesUpdated = SetTicketInfoValues(ticketInfo);
                // Fill with Amadeus Info
                bool TicketInfoUpdated = pnrService.GetTicketInfo(ticketInfo);
                //Finally Generate HTML
                bool done = GenerateHtmlTicketInformation(ticketInfo);

                return Ok(helper.CreatePnrClientResponse(pnrRet));
            }


            catch (Exception ex)
            {
                throw;
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


    }
}